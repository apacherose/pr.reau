using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PropertyRegister.REAU.Applications.Models
{
    public enum ServicеActionTypes
    {
        /// <summary>
        /// пъровначално подаване на заявление
        /// </summary>
        ApplicationAcceptance = 1,

        /// <summary>
        /// наредено задължение
        /// </summary>
        RequestedPayment = 2,

        WaitingRegistrationInPR = 3,

        /// <summary>
        /// Регистриране в ИС на ИР
        /// </summary>
        ApplicationRegistrationInPR = 4,

        /// <summary>
        /// изпълнение на ел.справка с отдалечен достъп
        /// </summary>
        ExecuteRemoteReportInPR = 5,

        PaymentChange = 6
    }


    /// <summary>
    /// Действия по услугите
    /// </summary>
    public class ServiceInstanceAction
    {
        /// <summary>
        /// PK
        /// </summary>
        public long? ServiceActionID { get; set; }

        /// <summary>
        /// Идентификатор на операция в рамките на която е действието
        /// </summary>
        public Guid OperationID { get; set; }

        /// <summary>
        /// Идентификатор на операция в рамките на която е действието
        /// </summary>
        public long? ServiceOperationID { get; set; }

        /// <summary>
        /// Връзка към услугата.
        /// </summary>
        public long ServiceInstanceID { get; set; }

        /// <summary>
        /// Връзка към заявлението
        /// </summary>
        public long ApplicationID { get; set; }

        /// <summary>
        /// Вид на действието
        /// </summary>
        public ServicеActionTypes? ActionTypeID { get; set; }

        /// <summary>
        /// Статус на заявлението, до който води действието.
        /// </summary>
        public ApplicationStatuses ApplicationStatus { get; set; }

        /// <summary>
        /// Връзка към документ-резултат от действието (може и да липсва).
        /// </summary>
        public long? ServiceDocumentID { get; set; }

        /// <summary>
        /// пояснителен текст съпътсващ действието.
        /// </summary>
        public string Description { get; set; }
    }
}
