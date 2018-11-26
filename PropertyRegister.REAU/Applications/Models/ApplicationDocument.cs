using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PropertyRegister.REAU.Applications.Models
{
    /// <summary>
    /// Документи към заявление
    /// </summary>
    public class ApplicationDocument
    {
        public long? ApplicationDocumentID { get; set; }

        /// <summary>
        /// идентификатор на заявление
        /// </summary>
        public long ApplicationID { get; set; }

        /// <summary>
        /// 0 - application xml 
        /// 1 - attached document
        /// </summary>
        public int? DocumentType { get; set; }

        /// <summary>
        /// идентификатор/връзка към DocumentData
        /// </summary>
        public long DocumentID { get; set; }        
    }
}
