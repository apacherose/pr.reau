using CNSys.Data;
using PropertyRegister.REAU.Applications.Models;
using PropertyRegister.REAU.Applications.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PropertyRegister.REAU.Test.Mocks
{
    public class ServiceInstanceActionEntityMock : IServiceActionRepository
    {
        public bool IsReadOnly => throw new NotImplementedException();

        public void Create(ServiceAction item)
        {
            item.ServiceActionID = new Random().Next(1, 100);
        }

        public void Delete(long? key)
        {
            throw new NotImplementedException();
        }

        public void Delete(ServiceAction item)
        {
            throw new NotImplementedException();
        }

        public ServiceAction Read(long? key)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<ServiceAction> Search(ServiceActionSearchCriteria searchCriteria)
        {
            return Enumerable.Empty<ServiceAction>();
        }

        public IEnumerable<ServiceAction> Search(object state, ServiceActionSearchCriteria searchCriteria)
        {
            return Enumerable.Empty<ServiceAction>();
        }

        public IEnumerable<ServiceAction> Search(PagedDataState state, ServiceActionSearchCriteria searchCriteria)
        {
            throw new NotImplementedException();
        }

        public void Update(ServiceAction item)
        {
            throw new NotImplementedException();
        }
    }
}
