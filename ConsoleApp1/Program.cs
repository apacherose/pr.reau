using PropertyRegister.REAU.Applications;
using Rebus.Activation;
using Rebus.Config;
using Rebus.Handlers;
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
            using (var activator = new BuiltinHandlerActivator())
            {
                //activator.Register(() => new ApplicationReceivedMessageHanlder());

                activator.Handle<ApplicationReceivedMessage>((message) =>
                {
                    Console.WriteLine($"Got message of {nameof(message)} with value {(char)message.ID}.");
                    return Task.CompletedTask;
                });

                var bus = Configure.With(activator)
                    .Transport(t => t.UseMsmq("consumer.input"))
                    //.Subscriptions(s => s.StoreInMemory())
                    //.Routing(r => r.TypeBased().Map<ApplicationReceivedMessage>("publisher")) 
                    .Routing(r => r.TypeBased().Map<ApplicationReceivedMessage>("consumer.input"))
                    .Start();

                //bus.Subscribe<ApplicationReceivedMessage>().Wait();

                Console.WriteLine("This is Console 1: Subscriber");
                Console.WriteLine("Press ENTER to quit");
                Console.ReadLine();
                Console.WriteLine("Quitting...");
            }
        }
    }

    public class ApplicationReceivedMessageHanlder : IHandleMessages<ApplicationReceivedMessage>
    {
        public Task Handle(ApplicationReceivedMessage message)
        {
            Console.WriteLine($"Got message of {nameof(message)} with value {message.ID}.");
            return Task.CompletedTask;
        }
    }
}
