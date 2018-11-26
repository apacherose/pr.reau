using PropertyRegister.REAU.Integration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PropertyRegister.REAU.Test.Mocks
{
    public class PropertyRegisterClientMock : IPropertyRegisterClient
    {
        public Task<PropertyRegisterApplicationResult> ExecuteReportApplicationAsync(object request)
        {
            return Task.FromResult(new PropertyRegisterApplicationResult()
            {
                DocumentIdentifier = $"PR_REP_{DateTime.Now.ToString("yyyymmdd")}"
            });
        }

        public Task<ServiceApplicationResult> RegisterServiceApplicationAsync(object request)
        {
            return Task.FromResult(new ServiceApplicationResult()
            {
                RegisterStatusID = 1,
                DocumentIdentifier = $"PR_{DateTime.Now.ToString("yyyymmdd")}_number"
            });
        }
    }
}
