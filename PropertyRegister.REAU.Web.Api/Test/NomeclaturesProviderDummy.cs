using PropertyRegister.REAU.Domain;
using PropertyRegister.REAU.Nomenclatures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PropertyRegister.REAU.Web.Api.Test
{
    public class NomenclaturesProviderDummy : INomenclaturesProvider
    {
        public ICollection<ApplicationServiceType> GetApplicationServiceTypes()
        {
            return new List<ApplicationServiceType>
            {
                new ApplicationServiceType() {
                    ApplicationTypeID = 1,
                    Title = "Услуга платена стандартна",
                    IsFree = false,
                    IsReport = false,
                    PaymentAfterRegistration = false,
                    PaymentAmount = 10m,
                    PaymentDeadline = new TimeSpan(3, 0, 0, 0),
                    XmlNamespace = "https://registryagency.bg/schema/v1/CertificateForPerson"
                },
                new ApplicationServiceType() {
                    ApplicationTypeID = 2,
                    Title = "Услуга безплатна стандартна",
                    IsFree = true,
                    IsReport = false,
                    PaymentAfterRegistration = false,
                    PaymentAmount = 0m,
                    XmlNamespace = "https://registryagency.bg/schema/v1/CertificateForPersonFree"
                },
                new ApplicationServiceType() {
                    ApplicationTypeID = 3,
                    Title = "Корекция",
                    IsFree = true,
                    IsReport = false,
                    PaymentAfterRegistration = false,
                    PaymentAmount = 0m,
                    XmlNamespace = "https://registryagency.bg/schema/v1/Correction"
                },
                new ApplicationServiceType() {
                    ApplicationTypeID = 4,
                    Title = "Справка безплатна стандартна",
                    IsFree = true,
                    IsReport = true,
                    PaymentAfterRegistration = false,
                    PaymentAmount = 10m,
                    PaymentDeadline = new TimeSpan(3, 0, 0, 0),
                    XmlNamespace = "https://registryagency.bg/schema/v1/Report1"
                },

                new ApplicationServiceType() {
                    ApplicationTypeID = 5,
                    Title = "Справка платена стандартна",
                    IsFree = false,
                    IsReport = true,
                    PaymentAfterRegistration = false,
                    PaymentAmount = 10m,
                    PaymentDeadline = new TimeSpan(3, 0, 0, 0),
                    XmlNamespace = "https://registryagency.bg/schema/v1/Report2"
                },

            };
        }
    }
}
