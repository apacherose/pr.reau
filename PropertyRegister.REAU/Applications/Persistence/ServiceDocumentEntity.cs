using PropertyRegister.REAU.Applications.Models;
using PropertyRegister.REAU.Persistence;

namespace PropertyRegister.REAU.Applications.Persistence
{    
    public interface IServiceDocumentEntity : IEntity<ServiceDocument, long?, object>
    {
    }

    public class ServiceDocumentEntity : EntityBase<ServiceDocument, long?, object, ApplicationProcessDataContext>, IServiceDocumentEntity
    {
        public ServiceDocumentEntity() : base(false)
        {
        }

        protected override void CreateInternal(ApplicationProcessDataContext context, ServiceDocument item)
        {
            base.CreateInternal(context, item);
        }

        protected override void UpdateInternal(ApplicationProcessDataContext context, ServiceDocument item)
        {
            base.UpdateInternal(context, item);
        }

        protected override void DeleteInternal(ApplicationProcessDataContext context, ServiceDocument item)
        {
            base.DeleteInternal(context, item);
        }
    }
}
