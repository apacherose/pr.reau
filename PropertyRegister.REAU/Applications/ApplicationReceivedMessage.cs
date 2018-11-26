using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PropertyRegister.REAU.Applications
{
    /// <summary>
    /// Dummy - тест за rebus!
    /// </summary>
    public class ApplicationReceivedMessage
    {
        public ApplicationReceivedMessage(long id)
        {
            ID = id;
        }

        public long ID { get; private set; }
    }
}
