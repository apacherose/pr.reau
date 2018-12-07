using CNSys.Data;
using PropertyRegister.REAU.Applications.Models;
using System;
using System.Collections.Generic;
using CNSys.Extensions;

namespace PropertyRegister.REAU.Applications.Persistence
{
    public class ApplicationSearchCriteria
    {
        public List<long> ApplicationIDs { get; set; }
        public long? MainApplicationID { get; set; }
        public long? ServiceInstanceID { get; set; }
        public string ApplicationIdentifier { get; set; }
        public string ReportIdentifier { get; set; }
        public ApplicationStatuses? ApplicationStatus { get; set; }
        public int? ApplicationType { get; set; }
        public string PaymentIdentifier { get; set; }
    }

    public interface IApplicationRepository : IRepository<Application, long?, ApplicationSearchCriteria>
    {
        void CreateServicePayment(ServicePayment paymentRequest);
    }
   
    public class ApplicationRepository : RepositoryBase<Application, long?, ApplicationSearchCriteria, ApplicationProcessDataContext>, IApplicationRepository
    {
        public ApplicationRepository() : base(false)
        {
        }

        protected override void CreateInternal(ApplicationProcessDataContext context, Application item)
        {
            context.ApplicationCreate(item.MainApplicationID, item.ServiceInstanceID, item.IsReport,
                (int)item.Status, item.StatusTime, item.ApplicationTypeID.Value, item.RegistrationTime, out long applicationID, out string applIdentifier, out string reportIdentifier);

            item.ApplicationID = applicationID;
            item.ApplicationIdentifier = applIdentifier;
            item.ReportIdentifier = reportIdentifier;
        }

        protected override void UpdateInternal(ApplicationProcessDataContext context, Application item)
        {
            context.ApplicationUpdate(item.ApplicationID.Value, item.MainApplicationID, item.ServiceInstanceID, item.ApplicationIdentifier, item.ReportIdentifier,
                (int)item.Status, item.StatusTime, item.ApplicationTypeID.Value, item.RegistrationTime);
        }

        protected override IEnumerable<Application> SearchInternal(ApplicationProcessDataContext context, ApplicationSearchCriteria searchCriteria)
        {            
            IEnumerable<Application> res;

            string ids = string.Join(",", searchCriteria.ApplicationIDs.ToArray());

            using (var data = context.ApplicationSearch(ids, searchCriteria.MainApplicationID, searchCriteria.ServiceInstanceID, 
                searchCriteria.ApplicationIdentifier, searchCriteria.ReportIdentifier, (int?)searchCriteria.ApplicationStatus, searchCriteria.ApplicationType, 
                1, 20, out int count))
            {
                 res = data.ReadToList<Application>();
            }

            return res;
        }

        public void CreateServicePayment(ServicePayment paymentRequest)
        {
            throw new NotImplementedException();
        }
    }
}
