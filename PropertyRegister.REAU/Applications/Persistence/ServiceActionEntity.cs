using PropertyRegister.REAU.Applications.Models;
using PropertyRegister.REAU.Persistence;
using System;
using System.Collections.Generic;

namespace PropertyRegister.REAU.Applications.Persistence
{
    public class ServiceActionSearchCriteria
    {
        public long? ServiceOperationID { get; internal set; }
        public string OperationID { get; set; }
        public ServicеActionTypes? ActionType { get; internal set; }
    }

    public interface IServiceActionEntity : IEntity<ServiceAction, long?, ServiceActionSearchCriteria>
    {
    }

    public class ServiceActionEntity : EntityBase<ServiceAction, long?, ServiceActionSearchCriteria, ApplicationProcessDataContext>, IServiceActionEntity
    {
        public ServiceActionEntity() : base(false)
        {
        }

        protected override void CreateInternal(ApplicationProcessDataContext context, ServiceAction item)
        {
            context.ServiceActionCreate(item.OperationID, item.ServiceInstanceID, item.ApplicationID, (int)item.ApplicationStatus, (int)item.ActionTypeID.Value, out long serviceActionID);
            item.ServiceActionID = serviceActionID;
        }

        protected override IEnumerable<ServiceAction> SearchInternal(ApplicationProcessDataContext context, ServiceActionSearchCriteria searchCriteria)
        {
            return base.SearchInternal(context, searchCriteria);
        }
    }
}
