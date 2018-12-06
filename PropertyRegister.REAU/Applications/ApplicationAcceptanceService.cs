using PropertyRegister.REAU.Applications.Models;
using PropertyRegister.REAU.Applications.Persistence;
using PropertyRegister.REAU.Applications.Results;
using PropertyRegister.REAU.Common;
using PropertyRegister.REAU.Nomenclatures;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace PropertyRegister.REAU.Applications
{
    public interface IApplicationAcceptanceService
    {
        Task<ApplicationAcceptedResult> AcceptApplicationAsync(string portalOperationID, Stream xml);
    }

    public class ApplicationAcceptanceService : IApplicationAcceptanceService
    {
        private readonly INomenclaturesProvider NomenclaturesProvider;
        private readonly IDocumentService DocumentService;
        private readonly IIdempotentOperationExecutor IdempotentOperationExecutor;
        private readonly IActionDispatcher ActionDispatcher;
        private readonly IApplicationInfoResolver ApplicationInfoResolver;

        private readonly IApplicationRepository ApplicationRepository;
        private readonly IApplicationDocumentRepository ApplicationDocumentRepository;
        private readonly IServiceActionRepository ServiceActionRepository;
        private readonly IServiceInstanceRepository ServiceInstanceRepository;

        public ApplicationAcceptanceService(
            IApplicationRepository applicationRepository,
            IServiceInstanceRepository serviceInstanceRepository,
            IApplicationDocumentRepository applicationDocumentRepository,
            IServiceActionRepository serviceActionRepository,

            IDocumentService documentService,
            INomenclaturesProvider nomenclaturesProvider,
            IIdempotentOperationExecutor idempotentOperationExecutor,
            IActionDispatcher actionDispatcher,
            IApplicationInfoResolver applicationInfoResolver)
        {
            ApplicationRepository = applicationRepository;
            ServiceInstanceRepository = serviceInstanceRepository;
            ApplicationDocumentRepository = applicationDocumentRepository;
            ServiceActionRepository = serviceActionRepository;

            DocumentService = documentService;
            NomenclaturesProvider = nomenclaturesProvider;
            IdempotentOperationExecutor = idempotentOperationExecutor;
            ActionDispatcher = actionDispatcher;
            ApplicationInfoResolver = applicationInfoResolver;
        }

        public async Task<ApplicationAcceptedResult> AcceptApplicationAsync(string portalOperationID, Stream xml)
        {
            var operationResult = await IdempotentOperationExecutor.ExecuteAsync(portalOperationID, Common.Models.ServiceOperationTypes.AcceptServiceApplication,
                async (oid) =>
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
                    ServiceActionRepository.Create(action);

                    var result = new Results.ApplicationAcceptedResult()
                    {
                        ApplicationID = application.ApplicationID.Value,
                        ApplicationNumber = application.ApplicationIdentifier,
                        ApplicationStatus = application.Status,
                        RegistrationTime = application.RegistrationTime
                    };

                    return result;
                },
                async (r) =>
                {
                    await ActionDispatcher.SendAsync(r);
                });

            return operationResult;
        }

        private async Task<Application> InitialSaveApplication(Stream xml)
        {
            var appInfo = ApplicationInfoResolver.GetApplicationInfoFromXml(xml);

            var serviceType = NomenclaturesProvider.GetApplicationServiceTypes().SingleOrDefault(t => t.XmlNamespace == appInfo.XmlNamespace);

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

                ServiceInstanceRepository.Create(serviceInstance);
                serviceInstanceID = serviceInstance.ServiceInstanceID;
            }
            else
            {
                var mainApp = ApplicationRepository.Search(new ApplicationSearchCriteria() { ApplicationIdentifier = appInfo.MainApplicationNumber }).SingleOrDefault();
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
                RegistrationTime = DateTime.UtcNow,
                IsReport = serviceType.IsReport,
                Status = ApplicationStatuses.Accepted
            };

            ApplicationRepository.Create(application);

            await SaveApplicationDocumentsAsync(application.ApplicationID.Value, appInfo, xml);

            return application;
        }

        private async Task SaveApplicationDocumentsAsync(long applicationID, ApplicationXmlInfo applInfo, Stream xml)
        {
            var packageDocumentData = await DocumentService.SaveDocumentAsync(new DocumentCreateRequest()
            {
                Filename = "ApplicationXml",
                ContentType = "application/xml",
                Content = xml
            });

            var packageDocument = new ApplicationDocument()
            {
                ApplicationID = applicationID,
                DocumentType = 1, // main package
                DocumentID = packageDocumentData.DocumentDataID
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
                ApplicationDocumentRepository.Create(applDoc);
            }
        }
    }
}
