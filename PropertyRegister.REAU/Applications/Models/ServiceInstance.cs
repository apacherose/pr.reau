using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PropertyRegister.REAU.Applications.Models
{
    /// <summary>
    /// Заявена услуга
    /// </summary>
    public class ServiceInstance
    {
        public long? ServiceInstanceID { get; set; }

        /// <summary>
        /// Идентификатор на служба по вписвания
        /// </summary>
        public int OfficeID { get; set; }

        /// <summary>
        /// идентификатор на потребителския профил на заявителя в ЕПЗЕУ
        /// </summary>
        public string ApplicantCIN { get; set; }


    }
}
