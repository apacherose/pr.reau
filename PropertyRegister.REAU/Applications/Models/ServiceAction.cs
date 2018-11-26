using PropertyRegister.REAU.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PropertyRegister.REAU.Applications.Models
{
    /// <summary>
    /// Класът описва действия/операции по промяна на състояние на Application/ServiceInstance
    /// </summary>
    public class ServiceAction
    {
        [DapperColumn("service_action_id")]
        public long? ServiceActionID { get; set; }

        [DapperColumn("operation_id")]
        public long OperationID { get; set; }

        [DapperColumn("service_inst_id")]
        public long ServiceInstanceID { get; set; }

        [DapperColumn("application_id")]
        public long ApplicationID { get; set; }

        [DapperColumn("application_status")]
        public ApplicationStatuses ApplicationStatus { get; set; }

        [DapperColumn("action_type_id")]
        public ServicеActionTypes? ActionTypeID { get; set; }
    }
}
