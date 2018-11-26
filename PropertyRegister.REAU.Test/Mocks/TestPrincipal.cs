using PropertyRegister.REAU.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace PropertyRegister.REAU.Test.Mocks
{
    public class TestPrincipal : IPrincipal, IDataSourceUser
    {
        public TestPrincipal(string clientID) => ClientID = clientID;

        public IIdentity Identity => throw new NotImplementedException();

        public string ClientID { get; }

        public bool IsInRole(string role)
        {
            throw new NotImplementedException();
        }
    }
}
