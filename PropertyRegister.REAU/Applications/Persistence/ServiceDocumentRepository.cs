using PropertyRegister.REAU.Applications.Models;
using CNSys.Data;

namespace PropertyRegister.REAU.Applications.Persistence
{    
    public interface IServiceDocumentRepository : IRepository<ServiceDocument, long?, object>
    {
    }

    public class ServiceDocumentRepository : RepositoryBase<ServiceDocument, long?, object, ApplicationProcessDataContext>, IServiceDocumentRepository
    {
        public ServiceDocumentRepository() : base(false)
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
