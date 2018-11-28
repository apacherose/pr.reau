using PropertyRegister.REAU.Persistence;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PropertyRegister.REAU.Common.Models
{
    /// <summary>
    /// Документно съдържание + метаданни.
    /// </summary>
    public class DocumentData
    {
        [DapperColumn("document_id")]
        public long? DocID { get; set; }

        [DapperColumn("guid")]
        public string Identifier { get; set; }

        [DapperColumn("content_type")]
        public string ContentType { get; set; }

        [DapperColumn("filename")]
        public string Filename { get; set; }

        public string ContentHash { get; set; }

        /// <summary>
        /// Дали е наличен. 
        /// В процесът на заявявяне документът може да се изтрие, и само да се остави следа.
        /// </summary>
        public bool IsAvailable { get; set; }

        [DapperColumn("is_temp")]
        public bool IsTemporal { get; set; }

        public Stream Content { get; set; }
    }
}
