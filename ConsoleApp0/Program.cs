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
            string connString = "Data Source=pr_reau; User ID=pr_reau_dev; Password=pr_reau_dev;";

            using (var activator = new BuiltinHandlerActivator())
            {
                Configure.With(activator)
                    //.Transport(t => t.UseMsmqAsOneWayClient())
                    .Transport(t => t.UseOracleAsOneWayClient(connString, "mbus_messages"))
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
                        bus.Send(new ApplicationReceivedMessage(keyChar));
                    }
                }

            consideredHarmful:;
                Console.WriteLine("Quitting!");
            }
        }
    }
}
