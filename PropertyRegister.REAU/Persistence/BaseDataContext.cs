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
        protected DbConnection _dbConnection;

        public BaseDataContext(DbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }

        public void Dispose()
        {
        }
    }
}
