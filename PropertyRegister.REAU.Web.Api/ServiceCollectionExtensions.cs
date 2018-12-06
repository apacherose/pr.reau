using Microsoft.Extensions.Configuration;
using PropertyRegister.REAU.Applications.Results;
using Rebus.Config;
using Rebus.Routing.TypeBased;
using Rebus.ServiceProvider;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddRebusWithOracleTransport(this IServiceCollection services, IConfiguration configuration)
        {
             return services.AddRebus(rconfig => rconfig
                    .Transport(t => t.UseOracleAsOneWayClient(System.Configuration.ConfigurationManager.ConnectionStrings["defaultRWConnectionString"].ConnectionString, "mbus_messages"))
                    .Routing(r => r.TypeBased().Map<ApplicationAcceptedResult>("q_appl_acceptance")));
        }
    }
}
