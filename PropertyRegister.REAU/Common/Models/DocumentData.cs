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
        // metadata 

        public long? DocID { get; set; }
        public Guid? Identifier { get; set; }
        public string ContentType { get; set; }
        public string Filename { get; set; }
        public string ContentHash { get; set; }

        /// <summary>
        /// Дали е наличен. 
        /// В процесът на заявявяне документът може да се изтрие, и само да се остави следа.
        /// </summary>
        public bool IsAvailable { get; set; }

        public Stream Content { get; set; }
    }
}
