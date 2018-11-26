using PropertyRegister.REAU.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PropertyRegister.REAU.Common.Models
{
    public enum ServiceOperationTypes
    {
        AcceptServiceApplication = 1,
        ProcessServiceApplication = 2,
        ProcessServicePayment = 3
    }

    public class ServiceOperation
    {
        [DapperColumn("service_operation_id")]
        public long? ServiceOperationID { get; set; }

        [DapperColumn("operation_id")]
        public string OperationID { get; set; }

        [DapperColumn("operation_type")]
        public ServiceOperationTypes ServiceOperationType { get; set; }

        [DapperColumn("is_completed")]
        public bool IsCompleted { get; set; }

        [DapperColumn("result")]
        public string Result { get; set; }
    }
}
