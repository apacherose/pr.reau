using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PropertyRegister.REAU.Integration
{
    public class PropertyRegisterApplicationResult
    {
        public string DocumentIdentifier { get; set; }
        public Stream DocumentContent { get; set; }
    }

    public class ServiceApplicationResult : PropertyRegisterApplicationResult
    {
        public int RegisterStatusID { get; set; }
    }

    public interface IPropertyRegisterClient
    {
        Task<ServiceApplicationResult> RegisterServiceApplicationAsync(object request);

        Task<PropertyRegisterApplicationResult> ExecuteReportApplicationAsync(object request);
    }

    public class PropertyRegisterClient : IPropertyRegisterClient
    {
        public Task<PropertyRegisterApplicationResult> ExecuteReportApplicationAsync(object request)
        {
            throw new NotImplementedException();
        }

        public Task<ServiceApplicationResult> RegisterServiceApplicationAsync(object request)
        {
            throw new NotImplementedException();
        }
    }
}
