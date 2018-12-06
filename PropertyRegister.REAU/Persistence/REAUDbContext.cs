using CNSys.Data;
using CNSys.Security;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Configuration;
using System.Threading;

namespace PropertyRegister.REAU.Persistence
{
    public class REAUDbContext : DbContext
    {
        #region Constructors

        public REAUDbContext(ConnectionStringSettings connectionString, DbContextOptions options)
            : base(connectionString, options)
        {
        }

        #endregion

        #region Overriden Methods

        protected override void InitConnection()
        {            
            var dataSourceUser = Thread.CurrentPrincipal as IDataSourceUser;

            if (dataSourceUser != null)
            {
                int user = int.Parse(dataSourceUser.ClientID);

                const string currentUserKey = "currentUser";

                object currentUser = null;
                bool hasContextItemsUser = this.ConnectionContextItems.TryGetValue(currentUserKey, out currentUser);
                if (!hasContextItemsUser ||
                    int.Parse(currentUser.ToString()) != user)
                {
                    (Connection as OracleConnection).ClientId = user.ToString();

                    if (hasContextItemsUser)
                        ConnectionContextItems[currentUserKey] = user;
                    else
                        ConnectionContextItems.Add(currentUserKey, user);
                }
            }
        }

        #endregion

        #region Static Interface

        public static new REAUDbContext CreateDefaultSerializationContext()
        {
            return CreateSerializationContext(ConnectionStrings.DefaultConnectionString, true);
        }

        public static new REAUDbContext CreateSerializationContext(ConnectionStringSettings connectionStringSettings, bool useTransaction)
        {
            if (useTransaction)
                return CreateContext(connectionStringSettings, DbContextOptions.UseTransaction);
            else
                return CreateContext(connectionStringSettings, DbContextOptions.None);
        }

        public static new REAUDbContext CreateContext(ConnectionStringSettings connectionStringSettings, DbContextOptions options)
        {
            if ((options & DbContextOptions.SuppressConnection) == DbContextOptions.SuppressConnection)
                throw new NotImplementedException("SuppressConnection option is not yet implemented.");

            return new REAUDbContext(connectionStringSettings, options);
        }

        #endregion
    }
}
