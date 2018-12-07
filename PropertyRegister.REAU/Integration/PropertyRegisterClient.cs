using System.IO;
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
            return Task.FromResult(new PropertyRegisterApplicationResult());
        }

        public Task<ServiceApplicationResult> RegisterServiceApplicationAsync(object request)
        {
            return Task.FromResult(new ServiceApplicationResult());
        }
    }
}
