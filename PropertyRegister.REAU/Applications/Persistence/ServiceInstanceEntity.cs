using System.Collections.Generic;
using PropertyRegister.REAU.Applications.Models;
using PropertyRegister.REAU.Persistence;

namespace PropertyRegister.REAU.Applications.Persistence
{
    public interface IServiceInstanceEntity : IEntity<ServiceInstance, long?, object>
    {
    }

    public class ServiceInstanceEntity : EntityBase<ServiceInstance, long?, object, ApplicationProcessDataContext>, IServiceInstanceEntity
    {
        public ServiceInstanceEntity() : base(false)
        {
        }

        protected override void CreateInternal(ApplicationProcessDataContext context, ServiceInstance item)
        {
            context.ServiceInstanceCreate(item.OfficeID, int.Parse(item.ApplicantCIN), out long serviceInstanceID);
            item.ServiceInstanceID = serviceInstanceID;
        }

        protected override void UpdateInternal(ApplicationProcessDataContext context, ServiceInstance item)
        {
            context.ServiceInstanceUpdate(item.ServiceInstanceID.Value, item.OfficeID, int.Parse(item.ApplicantCIN));
        }

        protected override IEnumerable<ServiceInstance> SearchInternal(ApplicationProcessDataContext context, object searchCriteria)
        {
            IEnumerable<ServiceInstance> res;

            using (var data = context.ServiceInstanceSearch(null, null, null, 1, 20, out int count))
            {
                res = data.ReadToList<ServiceInstance>();
            }

            return res;
        }
    }
}
