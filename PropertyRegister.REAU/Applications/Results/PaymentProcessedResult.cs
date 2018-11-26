using PropertyRegister.REAU.Applications.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PropertyRegister.REAU.Applications.Results
{
    public class PaymentProcessedResult
    {
        public ApplicationStatuses ApplicationStatus { get; set; }
        public long ApplicationID { get; set; }
    }
}
