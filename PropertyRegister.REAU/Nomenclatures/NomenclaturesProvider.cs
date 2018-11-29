using PropertyRegister.REAU.Domain;
using System;
using System.Collections.Generic;

namespace PropertyRegister.REAU.Nomenclatures
{
    public interface INomenclaturesProvider
    {
        ICollection<ApplicationServiceType> GetApplicationServiceTypes();
    }

    public class NomenclaturesProvider : INomenclaturesProvider
    {
        public ICollection<ApplicationServiceType> GetApplicationServiceTypes()
        {
            throw new NotImplementedException();
        }
    }
}
