using Microsoft.Extensions.Hosting;
using PropertyRegister.REAU.Applications;
using PropertyRegister.REAU.Applications.MessageHandlers;
using PropertyRegister.REAU.Applications.Results;
using Rebus.Activation;
using Rebus.Config;
using Rebus.Handlers;
using Rebus.Retry.Simple;
using Rebus.Routing.TypeBased;
using Rebus.ServiceProvider;
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace PropertyRegister.REAU.MessageProcessor
{
    class Program
    {
        static void Main(string[] args)
        {
            IHost host = null;

            var hostBuilder = new HostBuilder()
                .ConfigureServices((context, services) =>
                    {
                        // applications DI
                        services.AddApplicationsPersistence()
                            .AddApplicationsProcessing()
                            .AddIntegrationClients()
                            .AddREAUInfrastructureServices();

                        services.AutoRegisterHandlersFromAssemblyOf<ApplicationAcceptedResultHandler>();

                        services.AddRebus(rconfig => rconfig
                            .Transport(t => t.UseOracle(System.Configuration.ConfigurationManager.ConnectionStrings["defaultRWConnectionString"].ConnectionString, "mbus_messages", "q_appl_processing"))
                            .Routing(r => r.TypeBased().Map<ApplicationAcceptedResult>("q_appl_processing")
                                                       .Map<ApplicationProcessedResult>("q_appl_processing")
                                    ));
                    });
            try
            {
                host = hostBuilder.Build();

                // start rebus
                host.Services.UseRebus();

                host.Start();

                Console.WriteLine("Press ENTER to quit");
                Console.ReadLine();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                Console.WriteLine("Quitting...");

                host?.Dispose();
                host = null;
            }
        }       
    }    
}
