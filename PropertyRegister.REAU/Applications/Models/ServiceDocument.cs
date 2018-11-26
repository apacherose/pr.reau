using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PropertyRegister.REAU.Applications.Models
{
    /// <summary>
    /// Документи по услуга.
    /// </summary>
    public class ServiceDocument
    {
        public long? ServiceInstanceDocumentID { get; set; }

        /// <summary>
        /// Заявена услуга
        /// </summary>
        public long ServiceInstanceID { get; set; }

        /// <summary>
        /// електронно издадено удостоверение за лице/имот/период;
        /// сканирана резолюция на съдия по вписвания за открити нередовности в заявление за удостоверение;
        /// електронен незаверен препис на документ;
        /// резултат от електронна справка;
        /// </summary>
        public int? ServiceDocumentTypeID { get; set; }

        /// <summary>
        /// идентификатор/връзка към DocumentData
        /// </summary>
        public long DocumentID { get; set; }
    }
}
