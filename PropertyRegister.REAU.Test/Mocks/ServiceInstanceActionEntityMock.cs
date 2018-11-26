using PropertyRegister.REAU.Applications.Models;
using PropertyRegister.REAU.Applications.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PropertyRegister.REAU.Test.Mocks
{
    public class ServiceInstanceActionEntityMock : IServiceActionEntity
    {
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

        public IEnumerable<ServiceAction> Search(ServiceActionSearchCriteria searchCriteria)
        {
            return Enumerable.Empty<ServiceAction>();
        }

        public IEnumerable<ServiceAction> Search(object state, ServiceActionSearchCriteria searchCriteria)
        {
            return Enumerable.Empty<ServiceAction>();
        }

        public void Update(ServiceAction item)
        {
            throw new NotImplementedException();
        }
    }
}
