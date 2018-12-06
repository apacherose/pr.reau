using PropertyRegister.REAU.Applications;
using Rebus.Activation;
using Rebus.Config;
using Rebus.Routing.TypeBased;
using System;
using System.Threading.Tasks;

namespace ConsoleApp0
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
                Configure.With(activator)
                    .Transport(t => t.UseMsmq("publisher"))
                    //.Subscriptions(s => s.StoreInMemory())
                    .Routing(r => r.TypeBased().Map<ApplicationReceivedMessage>("consumer.input"))
                    .Start();                

                while (true)
                {
                    Console.WriteLine(@"a) Publish string");

                    var keyChar = char.ToLower(Console.ReadKey(true).KeyChar);
                    var bus = activator.Bus.Advanced.SyncBus;

                    if (keyChar == 'q')
                        goto consideredHarmful;
                    else
                    {
                        //bus.Publish(new ApplicationReceivedMessage(keyChar));
                        bus.Send(new ApplicationReceivedMessage(keyChar));
                    }
                }

            consideredHarmful:;
                Console.WriteLine("Quitting!");
            }
        }
    }
}
