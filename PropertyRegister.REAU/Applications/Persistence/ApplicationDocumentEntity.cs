using PropertyRegister.REAU.Applications.Models;
using PropertyRegister.REAU.Persistence;

namespace PropertyRegister.REAU.Applications.Persistence
{
    public interface IApplicationDocumentEntity : IEntity<ApplicationDocument, long?, object>
    {
    }

    public class ApplicationDocumentEntity : EntityBase<ApplicationDocument, long?, object, ApplicationProcessDataContext>, IApplicationDocumentEntity
    {
        public ApplicationDocumentEntity() : base(false)
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
