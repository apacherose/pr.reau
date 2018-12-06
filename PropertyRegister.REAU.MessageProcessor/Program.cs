using Microsoft.Extensions.Hosting;
using PropertyRegister.REAU.Applications;
using PropertyRegister.REAU.Applications.Results;
using Rebus.Activation;
using Rebus.Config;
using Rebus.Handlers;
using Rebus.Retry.Simple;
using Rebus.Routing.TypeBased;
using Rebus.ServiceProvider;
using System;
using System.Threading.Tasks;

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
                        services.AutoRegisterHandlersFromAssemblyOf<ApplicationAcceptedResultHandler>();

                        services.AddRebus(rconfig => rconfig
                            .Transport(t => t.UseOracle(System.Configuration.ConfigurationManager.ConnectionStrings["defaultRWConnectionString"].ConnectionString, "mbus_messages", "q_appl_acceptance"))
                            .Routing(r => r.TypeBased().Map<ApplicationAcceptedResult>("q_appl_acceptance")));
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

                host.Dispose();
                host = null;
            }
        }

        static async Task MainAsync()
        {
            string connString = "Data Source=pr_reau; User ID=pr_reau_dev; Password=pr_reau_dev;";

            using (var activator = new BuiltinHandlerActivator())
            {
                activator.Handle<ApplicationReceivedMessage>((message) =>
                {
                    Console.WriteLine($"Got message of {nameof(message)} with value {(char)message.ID}.");

                    return Task.CompletedTask;
                });

                var bus = Configure.With(activator)
                    .Transport(t => t.UseOracle(connString, "mbus_messages", "consumer.input"))
                    .Routing(r => r.TypeBased().Map<ApplicationReceivedMessage>("consumer.input"))
                    .Options(c => c.SimpleRetryStrategy(maxDeliveryAttempts: 3, errorQueueAddress: "q_error"))
                    .Start();

                Console.WriteLine("Press ENTER to quit");
                Console.ReadLine();
                Console.WriteLine("Quitting...");
            }
        }
    }

    public class ApplicationAcceptedResultHandler : IHandleMessages<ApplicationAcceptedResult>
    {
        public Task Handle(ApplicationAcceptedResult message)
        {
            Console.WriteLine($"Handled message of {nameof(message)} with value {message.ApplicationNumber}.");

            return Task.CompletedTask;
        }
    }
}
