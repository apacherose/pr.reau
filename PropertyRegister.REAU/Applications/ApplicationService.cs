using PropertyRegister.REAU.Applications.Models;
using PropertyRegister.REAU.Applications.Persistence;
using PropertyRegister.REAU.Applications.Results;
using PropertyRegister.REAU.Common;
using PropertyRegister.REAU.Integration;
using PropertyRegister.REAU.Nomenclatures;
using PropertyRegister.REAU.Payments;
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
        private readonly INomenclaturesProvider NomenclaturesProvider;
        private readonly IIdempotentOperationExecutor IdempotentOperationExecutor;
        private readonly IActionDispatcher ActionDispatcher;
        private readonly IPaymentManager PaymentManager;
        private readonly IPropertyRegisterClient PropertyRegisterClient;

        private readonly IApplicationEntity ApplicationEntity;
        private readonly IServiceActionEntity ServiceActionEntity;

        public ApplicationService(
            IApplicationEntity applicationEntity,
            IServiceActionEntity serviceActionEntity,            
            IPropertyRegisterClient propertyRegisterClient,
            INomenclaturesProvider nomenclaturesProvider,
            IIdempotentOperationExecutor idempotentOperationExecutor,
            IActionDispatcher actionDispatcher,
            IPaymentManager paymentManager,
            IApplicationInfoResolver applicationInfoResolver)
        {
            ApplicationEntity = applicationEntity;
            ServiceActionEntity = serviceActionEntity;

            PropertyRegisterClient = propertyRegisterClient;
            NomenclaturesProvider = nomenclaturesProvider;
            IdempotentOperationExecutor = idempotentOperationExecutor;            
            ActionDispatcher = actionDispatcher;
            PaymentManager = paymentManager;
        }
        
        public async Task<ApplicationProcessedResult> ProcessApplicationAsync(long applicationID)
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

                var serviceType = NomenclaturesProvider.GetApplicationServiceTypes().Single(t => t.ApplicationTypeID == application.ApplicationTypeID);

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
        
        //////
        ////// API CALLS
        //////

        private async Task<ApplicationProcessedResult> SendCorrectionApplicationToPRAsync(long operationID, Models.Application application)
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

        private async Task<ApplicationProcessedResult> RequestApplicationPaymentAsync(long operationID, Models.Application application)
        {
            var servicePayment = await PaymentManager.RequestApplicationPaymentAsync(application, filedAmount: null /*from xml*/);

            if (servicePayment.Status != PaymentStatuses.Ordered)
                throw new Exception(servicePayment.ErrorDescription);

            ApplicationEntity.CreateServicePayment(servicePayment);

            var serviceType = NomenclaturesProvider.GetApplicationServiceTypes().Single(t => t.ApplicationTypeID == application.ApplicationTypeID);

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
    }
}
