using Newtonsoft.Json;
using PropertyRegister.REAU.Applications;
using Rebus.Bus;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace PropertyRegister.REAU.Web.Api.Test
{
    public class ActionDispatcherDummy : IActionDispatcher
    {
        private readonly IBus Bus;

        public ActionDispatcherDummy(IBus bus)
        {
            Bus = bus;
        }

        public Task SendAsync(object actionData) => Bus.Send(actionData);
    }
}
