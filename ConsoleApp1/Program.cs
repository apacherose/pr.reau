using PropertyRegister.REAU.Applications;
using PropertyRegister.REAU.Applications.Results;
using Rebus.Activation;
using Rebus.Config;
using Rebus.Handlers;
using Rebus.Retry.Simple;
using Rebus.Routing.TypeBased;
using System;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    class Program
    {
        static void Main(string[] args)
        {
            MainAsync().GetAwaiter().GetResult();
        }

        static async Task MainAsync()
        {
            string connString = "Data Source=pr_reau; User ID=pr_reau_dev; Password=pr_reau_dev;";

            using (var activator = new BuiltinHandlerActivator())
            {                
                //activator.Handle<ApplicationReceivedMessage>((message) =>
                //{
                //    Console.WriteLine($"Got message of {nameof(message)} with value {(char)message.ID}.");

                //    return Task.CompletedTask;
                //});

                activator.Register(() => new ApplicationAcceptedResultHandler1());

                var bus = Configure.With(activator)                    
                    //.Transport(t => t.UseMsmq("consumer.input"))
                    .Transport(t => t.UseOracle(connString, "mbus_messages", "error"))
                    .Routing(r => r.TypeBased()
                                        .Map<ApplicationReceivedMessage>("consumer.input")
                                        .Map<ApplicationAcceptedResult>("error"))

                    .Options(c => c.SimpleRetryStrategy(maxDeliveryAttempts: 3, errorQueueAddress: "q_error"))
                    .Start();

               //bus.Subscribe<ApplicationReceivedMessage>().Wait();

               Console.WriteLine("This is Console 1: Subscriber");
                Console.WriteLine("Press ENTER to quit");
                Console.ReadLine();
                Console.WriteLine("Quitting...");
            }
        }
    }

    public class ApplicationAcceptedResultHandler1 : IHandleMessages<ApplicationAcceptedResult>
    {        
        public Task Handle(ApplicationAcceptedResult message)
        {
            Console.WriteLine($"Handled message of {nameof(message)} with value {message.ApplicationNumber}.");

            return Task.CompletedTask;
        }
    }
}
