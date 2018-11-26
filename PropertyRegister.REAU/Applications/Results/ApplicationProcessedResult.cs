using PropertyRegister.REAU.Applications.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PropertyRegister.REAU.Applications.Results
{
    public class ApplicationProcessedResult
    {
        public long ApplicationID { get; set; }
        public string ApplicationNumber { get; set; }
        public ApplicationStatuses? ApplicationStatus { get; set; }

        public decimal? PaymentAmount { get; set; }
        public DateTime? PaymentDeadline { get; set; }
        public string PaymentIdentifier { get; set; }
        public string RegisterOutcomeNumber { get; set; }

        public ICollection<string> Errors { get; set; }
    }
}
