using PropertyRegister.REAU.Applications;
using PropertyRegister.REAU.Applications.Models;
using PropertyRegister.REAU.Persistence;
using System;
using System.Collections.Generic;
using System.Data.Common;

namespace PropertyRegister.REAU.Applications.Persistence
{
    public class ApplicationSearchCriteria
    {
        public long? ApplicationID { get; set; }
        public long? MainApplicationID { get; set; }
        public long? ServiceInstanceID { get; set; }
        public string ApplicationIdentifier { get; set; }
        public string ReportIdentifier { get; set; }
        public ApplicationStatuses? ApplicationStatus { get; set; }
        public int? ApplicationType { get; set; }
        public string PaymentIdentifier { get; set; }
    }

    public interface IApplicationEntity : IEntity<Application, long?, ApplicationSearchCriteria>
    {
        void CreateServicePayment(ServicePayment paymentRequest);
    }
   
    public class ApplicationEntity : EntityBase<Application, long?, ApplicationSearchCriteria, ApplicationProcessDataContext>, IApplicationEntity
    {
        public ApplicationEntity() : base(false)
        {
        }

        protected override void CreateInternal(ApplicationProcessDataContext context, Application item)
        {
            context.ApplicationCreate(item.MainApplicationID, item.ServiceInstanceID, item.ApplicationIdentifier, item.ReportIdentifier, 
                (int)item.Status, item.StatusTime, item.ApplicationTypeID.Value, item.RegistrationTime, out long applicationID);

            item.ApplicationID = applicationID;
        }

        protected override void UpdateInternal(ApplicationProcessDataContext context, Application item)
        {
            context.ApplicationUpdate(item.ApplicationID.Value, item.MainApplicationID, item.ServiceInstanceID, item.ApplicationIdentifier, item.ReportIdentifier,
                (int)item.Status, item.StatusTime, item.ApplicationTypeID.Value, item.RegistrationTime);
        }

        protected override IEnumerable<Application> SearchInternal(ApplicationProcessDataContext context, ApplicationSearchCriteria searchCriteria)
        {            
            IEnumerable<Application> res;

            using (var data = context.ApplicationSearch(null, searchCriteria.MainApplicationID, searchCriteria.ServiceInstanceID, 
                searchCriteria.ApplicationIdentifier, searchCriteria.ReportIdentifier, (int?)searchCriteria.ApplicationStatus, searchCriteria.ApplicationType, 
                1, 20, out int count))
            {
                 res = data.Read<Application>();
            }

            return res;
        }

        public void CreateServicePayment(ServicePayment paymentRequest)
        {
            throw new NotImplementedException();
        }
    }
}
