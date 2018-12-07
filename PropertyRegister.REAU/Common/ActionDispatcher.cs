using Rebus.Bus;
using System.Threading.Tasks;

namespace PropertyRegister.REAU.Common
{
    public interface IActionDispatcher
    {
        Task SendAsync(object actionData);
    }

    public class DefaultActionDispatcher : IActionDispatcher
    {
        private readonly IBus Bus;

        public DefaultActionDispatcher(IBus bus)
        {
            Bus = bus;
        }

        public Task SendAsync(object actionData) => Bus.Send(actionData);
    }
}
