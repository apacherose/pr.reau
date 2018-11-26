using PropertyRegister.REAU.Applications.Models;
using PropertyRegister.REAU.Applications.Persistence;
using PropertyRegister.REAU.Common;
using PropertyRegister.REAU.Domain;
using PropertyRegister.REAU.Integration;
using PropertyRegister.REAU.Payments;
using PropertyRegister.REAU.Persistence;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace PropertyRegister.REAU.Applications
{
    public interface IActionDispatcher : IDisposable
    {
        Task SendAsync(string actionName, object actionData);
    }

    public class ApplicationService
    {
        private readonly ICollection<ApplicationServiceType> ApplicationTypesCollection;
        private readonly IDocumentService DocumentService;
        private readonly IIdempotentOperationExecutor IdempotentOperationExecutor;
        private readonly IActionDispatcher ActionDispatcher;
        private readonly IPaymentManager PaymentManager;
        private readonly IApplicationInfoResolver ApplicationInfoResolver;

        private readonly IPropertyRegisterClient PropertyRegisterClient;

        private readonly IApplicationEntity ApplicationEntity;
        private readonly IApplicationDocumentEntity ApplicationDocumentEntity;
        private readonly IServiceDocumentEntity ServiceDocumentEntity;
        private readonly IServiceActionEntity ServiceActionEntity;
        private readonly IServiceInstanceEntity ServiceInstanceEntity;

        public ApplicationService(
            IApplicationEntity applicationEntity,
            IServiceInstanceEntity serviceInstanceEntity,
            IApplicationDocumentEntity applicationDocumentEntity,
            IServiceActionEntity serviceActionEntity,

            IDocumentService documentService,
            IPropertyRegisterClient propertyRegisterClient,
            IPaymentIntegrationClient paymentIntegrationClient,
            IApplicationServiceTypeCollection applicationTypesCollection,
            IIdempotentOperationExecutor idempotentOperationExecutor,
            IActionDispatcher actionDispatcher,
            IPaymentManager paymentManager,
            IApplicationInfoResolver applicationInfoResolver)
        {
            ApplicationEntity = applicationEntity;
            ServiceInstanceEntity = serviceInstanceEntity;
            ApplicationDocumentEntity = applicationDocumentEntity;
            DocumentService = documentService;
            PropertyRegisterClient = propertyRegisterClient;

            ApplicationTypesCollection = applicationTypesCollection.GetItems();
            IdempotentOperationExecutor = idempotentOperationExecutor;
            ServiceActionEntity = serviceActionEntity;
            ActionDispatcher = actionDispatcher;
            PaymentManager = paymentManager;
            ApplicationInfoResolver = applicationInfoResolver;
        }

        public async Task<Results.ApplicationAcceptedResult> AcceptApplicationAsync(string portalOperationID, Stream xml)
        {
            var operationResult = await IdempotentOperationExecutor.ExecuteAsync(portalOperationID, Common.Models.ServiceOperationTypes.AcceptServiceApplication, async (oid) =>
            {
                var application = await InitialSaveApplication(xml);

                var action = new ServiceAction()
                {
                    OperationID = oid,
                    ServiceInstanceID = application.ServiceInstanceID,
                    ApplicationID = application.ApplicationID.Value,
                    ApplicationStatus = application.Status.Value,
                    ActionTypeID = ServicеActionTypes.ApplicationAcceptance
                };
                ServiceActionEntity.Create(action);

                var result = new Results.ApplicationAcceptedResult()
                {
                    ApplicationID = application.ApplicationID.Value,
                    ApplicationNumber = application.ApplicationIdentifier,
                    ApplicationStatus = application.Status,
                    RegistrationTime = application.RegistrationTime
                };

                return result;
            });

            await ActionDispatcher.SendAsync("ApplicationAcceptance", operationResult.ApplicationID);

            return operationResult;

        }

        public async Task<Results.ApplicationProcessedResult> ProcessApplicationAsync(long applicationID)
        {
            var operationResult = await IdempotentOperationExecutor.ExecuteAsync(applicationID.ToString(), Common.Models.ServiceOperationTypes.ProcessServiceApplication, (oid) =>
            {
                var application = ApplicationEntity.Search(new ApplicationSearchCriteria() { ApplicationID = applicationID }).SingleOrDefault();

                if (application == null && application.Status != ApplicationStatuses.Accepted)
                    throw new InvalidOperationException();

                // decide which api to call

                if (application.IsMainApplication)
                {
                    return RequestApplicationPaymentAsync(oid, application);
                }
                else
                {
                    return SendCorrectionApplicationToPRAsync(oid, application);
                }
            });

            // when application is free should be prepared to be sent to PR immediatelly
            if (operationResult.ApplicationStatus == ApplicationStatuses.WaitingRegistration ||
                 operationResult.ApplicationStatus == ApplicationStatuses.InProgress)
            {
                await ActionDispatcher.SendAsync("ApplicationWaitingRegistration", operationResult.ApplicationID);
            }

            return operationResult;
        }

        public async Task ProcessServicePaymentChangeAsync(string paymentIdentifier, object paymentData)
        {
            var operationResult = await IdempotentOperationExecutor.ExecuteAsync(paymentIdentifier, Common.Models.ServiceOperationTypes.ProcessServicePayment, (oid) =>
            {
                var application = ApplicationEntity
                    .Search(new ApplicationSearchCriteria() { PaymentIdentifier = paymentIdentifier }).SingleOrDefault();

                if (application == null || application.Status != ApplicationStatuses.WaitingPayment)
                    throw new InvalidOperationException();

                var serviceType = ApplicationTypesCollection.Single(t => t.ApplicationTypeID == application.ApplicationTypeID);

                application.Status = serviceType.IsReport ? ApplicationStatuses.InProgress : ApplicationStatuses.WaitingRegistration;

                ApplicationEntity.Update(application);

                var action = new ServiceAction()
                {
                    OperationID = oid,
                    ServiceInstanceID = application.ServiceInstanceID,
                    ApplicationID = application.ApplicationID.Value,
                    ApplicationStatus = application.Status.Value,
                    ActionTypeID = ServicеActionTypes.PaymentChange,
                };
                ServiceActionEntity.Create(action);

                return Task.FromResult(new Results.PaymentProcessedResult()
                {
                    ApplicationID = application.ApplicationID.Value,
                    ApplicationStatus = application.Status.Value
                });
            });

            await ActionDispatcher.SendAsync("ApplicationWaitingRegistration", operationResult.ApplicationID);
        }

        private async Task<Application> InitialSaveApplication(Stream xml)
        {
            var appInfo = ApplicationInfoResolver.GetApplicationInfoFromXml(xml);

            var serviceType = ApplicationTypesCollection.SingleOrDefault(t => t.XmlNamespace == appInfo.XmlNamespace);

            if (serviceType == null)
                throw new InvalidOperationException("Unknown application type namespace from xml!");

            long? serviceInstanceID = null;
            long? mainApplicationID = null;

            if (string.IsNullOrEmpty(appInfo.MainApplicationNumber))
            {
                var serviceInstance = new ServiceInstance()
                {
                    OfficeID = 1,
                    ApplicantCIN = "100"
                };

                ServiceInstanceEntity.Create(serviceInstance);
                serviceInstanceID = serviceInstance.ServiceInstanceID;
            }
            else
            {
                var mainApp = ApplicationEntity.Search(new ApplicationSearchCriteria() { ApplicationIdentifier = appInfo.MainApplicationNumber }).SingleOrDefault();
                if (mainApp == null)
                    throw new InvalidOperationException("Cannot find initial main application!");

                serviceInstanceID = mainApp.ServiceInstanceID;
                mainApplicationID = mainApp.ApplicationID;
            }

            var application = new Models.Application()
            {
                ServiceInstanceID = serviceInstanceID.Value,
                ApplicationTypeID = serviceType.ApplicationTypeID,
                MainApplicationID = mainApplicationID,
                //OfficeID = 0, // from xml
                Status = ApplicationStatuses.Accepted,
                //ApplicantID = 0, // from authentication
            };

            ApplicationEntity.Create(application);

            await SaveApplicationDocumentsAsync(application.ApplicationID.Value, appInfo, xml);

            return application;
        }

        //////
        ////// API CALLS
        //////

        private async Task<Results.ApplicationProcessedResult> SendCorrectionApplicationToPRAsync(long operationID, Models.Application application)
        {
            var prResponse = await PropertyRegisterClient.RegisterServiceApplicationAsync(null);

            // TODO 
            // ANALYZE prResponse

            application.Status = ApplicationStatuses.Completed;
            ApplicationEntity.Update(application);

            var action = new ServiceAction()
            {
                OperationID = operationID,
                ServiceInstanceID = application.ServiceInstanceID,
                ApplicationID = application.ApplicationID.Value,
                ApplicationStatus = application.Status.Value,
                ActionTypeID = ServicеActionTypes.ApplicationRegistrationInPR,
            };
            ServiceActionEntity.Create(action);

            return new Results.ApplicationProcessedResult()
            {
                ApplicationID = application.ApplicationID.Value
            };
        }

        private async Task<Results.ApplicationProcessedResult> RequestApplicationPaymentAsync(long operationID, Models.Application application)
        {
            var servicePayment = await PaymentManager.RequestApplicationPaymentAsync(application, filedAmount: null /*from xml*/);

            if (servicePayment.Status != PaymentStatuses.Ordered)
                throw new Exception(servicePayment.ErrorDescription);

            ApplicationEntity.CreateServicePayment(servicePayment);

            var serviceType = ApplicationTypesCollection.Single(t => t.ApplicationTypeID == application.ApplicationTypeID);

            application.Status = serviceType.IsFree ?
                    (serviceType.IsReport ? ApplicationStatuses.InProgress : ApplicationStatuses.WaitingRegistration) : ApplicationStatuses.WaitingPayment;

            ApplicationEntity.Update(application);

            var action = new ServiceAction()
            {
                OperationID = operationID,
                ServiceInstanceID = application.ServiceInstanceID,
                ApplicationID = application.ApplicationID.Value,
                ApplicationStatus = application.Status.Value,
                ActionTypeID = ServicеActionTypes.RequestedPayment,
            };
            ServiceActionEntity.Create(action);
            
            return new Results.ApplicationProcessedResult
            {
                ApplicationStatus = application.Status,
                PaymentAmount = servicePayment.ObligationAmount,
                PaymentDeadline = servicePayment.PaymentDeadline,
                PaymentIdentifier = servicePayment.PaymentIdentifier
            };
        }

        private async Task SaveApplicationDocumentsAsync(long applicationID, ApplicationXmlInfo applInfo, Stream xml)
        {
            var packageDocumentData = await DocumentService.SaveDocumentAsync(new DocumentRequest()
            {
                ContentType = "application/xml",
                Content = xml
            });

            var packageDocument = new ApplicationDocument()
            {
                ApplicationID = applicationID,
                DocumentType = 1, // main package
                DocumentID = packageDocumentData.DocID.Value
            };

            List<ApplicationDocument> applicationDocuments = new List<ApplicationDocument>();

            applicationDocuments.Add(packageDocument);

            if (applInfo.AttachedDocumentIDs != null && applInfo.AttachedDocumentIDs.Any())
            {
                var attachedDocs = await DocumentService.GetDocumentsAsync(applInfo.AttachedDocumentIDs.ToList());

                applicationDocuments.AddRange(attachedDocs.Select(d => new ApplicationDocument()
                {
                    ApplicationID = applicationID,
                    DocumentID = d.DocID.Value,
                    DocumentType = 2 // attachment
                }));
            }

            foreach (var applDoc in applicationDocuments)
            {
                ApplicationDocumentEntity.Create(applDoc);
            }
        }
    }
}
