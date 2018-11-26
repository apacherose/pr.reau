using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PropertyRegister.REAU.Domain
{
    /// <summary>
    /// Описание на предоставяна услуга от ИС на ПР.
    /// </summary>
    public class ApplicationServiceType
    {
        public int ApplicationTypeID { get; set; }

        public string Title { get; set; }

        public bool IsReport { get; set; }

        public bool IsFree { get; set; }

        public TimeSpan? PaymentDeadline { get; set; }

        public decimal? PaymentAmount { get; set; }

        /// <summary>
        /// дължимата такса се заплаща след регистриране на заявлението в ИС на ИР
        /// </summary>
        public bool PaymentAfterRegistration { get; set; }

        public string XmlNamespace { get; set; }
    }
}
