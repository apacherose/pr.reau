using Newtonsoft.Json;
using PropertyRegister.REAU.Applications;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace PropertyRegister.REAU.Web.Api.Test
{
    public class ActionDispatcherDummy : IActionDispatcher
    {
        private readonly JsonSerializer Serializer;

        public ActionDispatcherDummy()
        {
            Serializer = new JsonSerializer();
        }

        public Task SendAsync(string actionName, object actionData)
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
