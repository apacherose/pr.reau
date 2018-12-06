using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PropertyRegister.REAU.Applications.Models
{
    /// <summary>
    /// Статуси на Заявление за услуга
    /// </summary>
    public enum ApplicationStatuses
    {
        /// <summary>
        /// Прието.
        /// </summary>
        Accepted = 1,

        /// <summary>
        /// Чака регистрация.
        /// </summary>
        WaitingRegistration = 2,

        /// <summary>
        ///  В процес на изпълнение.
        /// </summary>
        InProgress = 3,

        /// <summary>
        /// Чака плащане.
        /// </summary>
        WaitingPayment = 4,

        /// <summary>
        /// Прекратено .
        /// </summary>
        Terminated = 5,

        /// <summary>
        /// Изпълнено.
        /// </summary>
        Completed = 6,

        //
        // Service only
        //

        /// <summary>
        /// Регистрирано 
        /// </summary>
        Registered = 11,

        /// <summary>
        /// Без движение – за преразглеждане
        /// </summary>
        WithoutMovementForReview = 12,

        /// <summary>
        /// Без движение
        /// </summary>
        WithoutMovement = 13,

        /// <summary>
        /// Отказано 
        /// </summary>
        Denied = 14,
    }

    /// <summary>
    /// Заявление за услуга
    /// </summary>
    public class Application
    {
        public long? ApplicationID { get; set; }

        /// <summary>
        /// връзка към заявена услуга
        /// </summary>
        public long ServiceInstanceID { get; set; }

        /// <summary>
        /// Връзка към основното заявление от ServiceInstanceApplication
        /// </summary>
        public long? MainApplicationID { get; set; }

        /// <summary>
        /// уникален идентификатор (референтен номер)
        /// </summary>
        public string ApplicationIdentifier { get; set; }

        /// <summary>
        /// уникален регистров номер на заявление за справка (различен е от референтния номер)
        /// </summary>
        public string ReportIdentifier { get; set; }

        /// <summary>
        /// Актуален Статус (Денормализация?)
        /// </summary>
        public ApplicationStatuses? Status { get; set; }

        /// <summary>
        /// Дата на статус (Денормализация?).
        /// </summary>
        public DateTime StatusTime { get; set; }

        /// <summary>
        /// Идентификатор на вида на заявлението от номенклатурата на ИС на ИР
        /// </summary>
        public int? ApplicationTypeID { get; set; }

        /// <summary>
        /// дата на регистрация на заявлението в РЕАУ.
        /// </summary>
        public DateTime RegistrationTime { get; set; }

        public bool IsReport { get; set; }

        public bool IsMainApplication => MainApplicationID == null;
    }    
}
