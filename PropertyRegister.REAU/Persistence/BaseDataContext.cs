using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PropertyRegister.REAU.Persistence
{
    public class BaseDataContext : IDisposable
    {
        public BaseDataContext(DbConnection dbConnection)
        {
            DbConnection = dbConnection;
        }

        public DbConnection DbConnection
        {
            get;
            protected set;
        }

        public void Dispose()
        {
        }
    }
}
