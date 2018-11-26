using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rebus.Activation;
using Rebus.Config;
using Rebus.Handlers;
using Rebus.Pipeline;
using Rebus.Routing.TypeBased;
using System.Threading.Tasks;

namespace PropertyRegister.REAU.MsTests
{
    [TestClass]
    public class RebusTests
    {
        [TestMethod]
        public void TestInfrastructure2()
        {            
            Assert.IsTrue(true);
        }

        [TestMethod]
        public void TestInfrastructure()
        {
            using (var activator = new BuiltinHandlerActivator())
            {
                activator.Register((mx) =>
                {
                    return new StringHanlder(mx);
                });

                var bus = Configure.With(activator)
                    //.Transport(t => t..UseMsmq("inputqueue"))
                    .Routing(r => r.TypeBased().Map<string>("inputqueue"))
                    .Start();

                bus.SendLocal("test1");
                bus.SendLocal("test2");
            }

            Assert.IsTrue(true);
        }

    }

    public class StringHanlder : IHandleMessages<string>
    {
        private IMessageContext mx;

        public StringHanlder(IMessageContext mx)
        {
            this.mx = mx;
        }

        public async Task Handle(string message)
        {

        }
    }
}
