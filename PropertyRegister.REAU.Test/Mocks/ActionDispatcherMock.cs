using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using PropertyRegister.REAU.Applications;
using PropertyRegister.REAU.Common;
using Rebus.Activation;
using Rebus.Bus;
using Rebus.Config;
using Rebus.Routing.TypeBased;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PropertyRegister.REAU.Test.Mocks
{
    public class RebusActionDispatcherMock : IActionDispatcher
    {
        private readonly BuiltinHandlerActivator HandlerActivator = new BuiltinHandlerActivator();
        private readonly IBus Bus;

        public RebusActionDispatcherMock()
        {
            Bus = Configure.With(HandlerActivator)
                    .Transport(t => t.UseMsmq("apps_queue"))
                    .Routing(r => r.TypeBased().Map<long>("ApplicationAcceptance"))
                    .Start();
        }

        public Task SendAsync(object actionData)
        {
            //TODO check if {actionname} exists in routing mapping?
            
            return Bus.Send(actionData);
        }

        public void Dispose()
        {
            HandlerActivator.Dispose();
        }
    }

    public class DummyActionDispatcherMock : IActionDispatcher
    {
        private readonly JsonSerializer Serializer;

        public DummyActionDispatcherMock()
        {
            Serializer = new JsonSerializer();
        }

        public Task SendAsync(object actionData)
        {
            JsonSerializer serializer = new JsonSerializer();

            string path = @"c:\tmp\reau_dispacher_queue\" + $"{DateTime.Now.ToString("yyyyMMddhhmmss")}.txt";

            using (StreamWriter sw = new StreamWriter(path))
            {
                using (JsonWriter writer = new JsonTextWriter(sw))
                {
                    Serializer.Serialize(writer, actionData);
                }
            }

            return Task.CompletedTask;
        }

        public void Dispose()
        {
        }
    }
}
