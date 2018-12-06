using CNSys.Data;
using PropertyRegister.REAU.Applications;
using PropertyRegister.REAU.Applications.Models;
using PropertyRegister.REAU.Applications.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PropertyRegister.REAU.Test.Mocks
{
    public class ApplicationEntityMock : IApplicationRepository
    {
        private readonly DbDataSets DataSets;

        public bool IsReadOnly => throw new NotImplementedException();

        public ApplicationEntityMock(DbDataSets dataSets)
        {
            DataSets = dataSets;
        }

        public void AddApplication(Applications.Models.Application application)
        {
            if (application.ServiceInstanceID == default(long) ||
                application.Status == null ||
                application.ApplicationTypeID == null)
                throw new ArgumentException("IApplicationEntity.AddApplication has wrong parameters!");

            application.ApplicationID = 1000;
            application.ApplicationIdentifier = $"{DateTime.Now.ToString("yyyymmdd")}-0001";
        }

        public void SaveApplicationDocument(ApplicationDocument applicationDocument)
        {
            if (applicationDocument.DocumentID == default(long) || applicationDocument.DocumentType == null)
                throw new ArgumentException(nameof(applicationDocument));
        }

        public void SaveServiceDocument(ServiceDocument serviceDocument)
        {
            if (serviceDocument.DocumentID == default(long) || serviceDocument.ServiceDocumentTypeID == null)
                throw new ArgumentException(nameof(serviceDocument));
        }

        public void SavePaymentRequest(ServicePayment paymentRequest)
        {
            paymentRequest.PaymentRequestID = 1000;
        }

        public IEnumerable<Applications.Models.Application> SearchApplications(long? applicationID, string appNumber)
        {
            return DataSets.Applications.Where(a => (applicationID.HasValue && a.ApplicationID == applicationID.Value) ||
                (!string.IsNullOrEmpty(appNumber) && a.ApplicationIdentifier == appNumber));
        }

        public IEnumerable<ServiceInstanceAction> SearchServiceActions(string operationID, ServicеActionTypes actionType)
        {
            return DataSets.ServiceActions.Where(sa => sa.OperationID.ToString() == operationID && sa.ActionTypeID == actionType);
        }

        public void UpdateApplication(Applications.Models.Application application)
        {           
        }

        public void SaveServiceAction(ServiceInstanceAction serviceAction)
        {
            serviceAction.ServiceActionID = 1000;
        }

        public void AddServiceInstance(ServiceInstance serviceInstance)
        {
            serviceInstance.ServiceInstanceID = 1000;
        }

        public void CreateServicePayment(ServicePayment paymentRequest)
        {
            throw new NotImplementedException();
        }

        public void Create(Applications.Models.Application item)
        {
            throw new NotImplementedException();
        }

        public void Delete(long? key)
        {
            throw new NotImplementedException();
        }

        public void Delete(Applications.Models.Application item)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Applications.Models.Application> Search(ApplicationSearchCriteria searchCriteria)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Applications.Models.Application> Search(object state, ApplicationSearchCriteria searchCriteria)
        {
            throw new NotImplementedException();
        }

        public void Update(Applications.Models.Application item)
        {
            throw new NotImplementedException();
        }

        public Applications.Models.Application Read(long? key)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Applications.Models.Application> Search(PagedDataState state, ApplicationSearchCriteria searchCriteria)
        {
            throw new NotImplementedException();
        }
    }
}
