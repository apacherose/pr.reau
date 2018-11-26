using PropertyRegister.REAU.Applications.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PropertyRegister.REAU.Test.Mocks
{
    public class DbDataSets
    {
        public IEnumerable<Applications.Models.Application> Applications = new List<Applications.Models.Application>()
        {
            new Applications.Models.Application()
            {
                ApplicationID = 100,
                ServiceInstanceID = 100,
                ApplicationIdentifier = "20181109/777",
                Status = REAU.Applications.Models.ApplicationStatuses.Registered,
                ApplicationTypeID = 1,                
            },

            new Applications.Models.Application()
            {
                ApplicationID = 101,
                ServiceInstanceID = 101,
                ApplicationIdentifier = "20181109/778",
                Status = REAU.Applications.Models.ApplicationStatuses.WaitingPayment,
                ApplicationTypeID = 1,
            },        
        };

        public IEnumerable<ServiceInstanceAction> ServiceActions = new List<ServiceInstanceAction>()
        {
            new ServiceInstanceAction()
            {
                OperationID = Guid.NewGuid(),
                ApplicationID = 1000,
                ServiceInstanceID = 1000,
                ActionTypeID = ServicеActionTypes.ApplicationAcceptance,
            }
        };
    }
}
