using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PropertyRegister.REAU.Applications.Models
{
    /// <summary>
    /// статуси на наредено плащане в Модул плащания
    /// </summary>
    public enum PaymentStatuses
    {
        /// Наредено плащане
        Ordered = 1,

        /// Неуспешно обработено
        Error = 2,

        // Отказано
        Cancelled = 3
    }

    /// <summary>
    /// наредено плащане в Модул плащания от РЕАУ по заявление по услуга 
    /// ОЩЕ НЕ Е УТОЧНЕН ТОЗИ МОДЕЛ!!!
    /// </summary>
    public class ServicePayment
    {
        public long? PaymentRequestID { get; set; }

        public long? ServiceInstanceID { get; set; }

        public string PaymentIdentifier { get; set; } // PaymentNumber?

        public PaymentStatuses? Status { get; set; } // ?

        public decimal ObligationAmount { get; set; }

        public decimal ServiceFee { get; set; }

        public DateTime? PaymentDeadline { get; set; }

        public string ErrorDescription { get; set; }
    }
}
