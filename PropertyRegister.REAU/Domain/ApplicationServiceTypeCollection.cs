using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PropertyRegister.REAU.Domain
{
    public interface IApplicationServiceTypeCollection
    {
        ICollection<ApplicationServiceType> GetItems();
    }

    internal class ApplicationServiceTypeCollection : IApplicationServiceTypeCollection
    {
        public ICollection<ApplicationServiceType> GetItems()
        {
            throw new NotImplementedException();
        }
    }
}
