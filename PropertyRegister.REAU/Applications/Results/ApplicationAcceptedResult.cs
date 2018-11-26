using PropertyRegister.REAU.Applications.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PropertyRegister.REAU.Applications.Results
{    
    public class ApplicationAcceptedResult
    {
        public string ApplicationNumber { get; set; }
        public ApplicationStatuses? ApplicationStatus { get; set; }
        public DateTime RegistrationTime { get; set; }

        public ICollection<string> Errors { get; set; }
        public long ApplicationID { get; set; }
    }   
}
