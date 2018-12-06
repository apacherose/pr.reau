using PropertyRegister.REAU.Applications.Models;
using CNSys.Data;

namespace PropertyRegister.REAU.Applications.Persistence
{
    public interface IApplicationDocumentRepository : IRepository<ApplicationDocument, long?, object>
    {
    }

    public class ApplicationDocumentRepository : RepositoryBase<ApplicationDocument, long?, object, ApplicationProcessDataContext>, IApplicationDocumentRepository
    {
        public ApplicationDocumentRepository() : base(false)
        {
        }

        protected override void CreateInternal(ApplicationProcessDataContext context, ApplicationDocument item)
        {
            //base.CreateInternal(context, item);
        }

        protected override void DeleteInternal(ApplicationProcessDataContext context, ApplicationDocument item)
        {
            base.DeleteInternal(context, item);
        }
    }
}
