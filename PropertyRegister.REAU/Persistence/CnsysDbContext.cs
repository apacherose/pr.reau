using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Common;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace PropertyRegister.REAU.Persistence
{
    [Flags]
    public enum DbContextOptions
    {
        /// <summary>
        /// None.
        /// </summary>
        None = 0x0,

        /// <summary>
        /// UT.
        /// </summary>
        UseTransaction = 0x1,

        /// <summary>
        /// SC.
        /// </summary>
        SuppressConnection = 0x2
    }

    public class ConnectionStrings
    {
        private static string _defaultConnectionString = "defaultRWConnectionString";

        public static ConnectionStringSettings DefaultConnectionString
        {
            get
            {
                return System.Configuration.ConfigurationManager.ConnectionStrings[DefaultConnectionStringName];
            }
        }

        public static string DefaultConnectionStringName
        {
            get
            {
                return _defaultConnectionString;
            }
            set
            {
                System.Threading.Interlocked.Exchange(ref _defaultConnectionString, value);
            }
        }
    }

    public class DbContext : IDisposable
    {
        #region Fields

        private static ConcurrentDictionary<string, Stack<ReusableDbConnectionHolder>> _innerConnectionHoldersStacks = new ConcurrentDictionary<string, Stack<ReusableDbConnectionHolder>>();
        private static readonly string LogicalContextIDName = "DbContextHolders_" + Guid.NewGuid().ToString();

        private string _logicalContextID = null;
        private bool _releaseLogicalContextID = false;
        private Stack<ReusableDbConnectionHolder> _dbConnectionHolderStack;

        private ReusableDbConnectionHolder _innerConnectionHolder;
        private TransactionScope _activeScope;

        private bool _completed = false;
        private bool _disposed = false;

        #endregion

        #region Constructors

        public DbContext(ConnectionStringSettings connectionStringSettings, DbContextOptions options)
        {
            bool useTransaction = (options & DbContextOptions.UseTransaction) == DbContextOptions.UseTransaction;

            try
            {
                GetOrCreateContext();

                #region CreateTransaction

                if (useTransaction)
                {
                    _activeScope = new TransactionScope(
                                        TransactionScopeOption.Required,
                                        new TransactionOptions() { IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted }, TransactionScopeAsyncFlowOption.Enabled);
                }
                else
                {
                    _activeScope = null;

                    if (Transaction.Current != null)
                        throw new NotSupportedException("There is ambient transaction opened");
                }

                #endregion

                #region Use or Create InnerConnectionHolder

                if ((options & DbContextOptions.SuppressConnection) == DbContextOptions.SuppressConnection)
                {
                    _innerConnectionHolder = InitDbConnectionHolderAndUse(connectionStringSettings, options);

                    PushInnerConnection(_innerConnectionHolder);
                }
                else
                {
                    _innerConnectionHolder = CheckAndUseInnerConnection(connectionStringSettings.ConnectionString, options);

                    if (_innerConnectionHolder == null)
                    {
                        _innerConnectionHolder = InitDbConnectionHolderAndUse(connectionStringSettings, options);

                        PushInnerConnection(_innerConnectionHolder);
                    }
                }

                #endregion

                #region Enlist Connection into Transaction

                if (useTransaction)
                {
                    _innerConnectionHolder.Connection.EnlistTransaction(Transaction.Current);
                }
                else
                    _innerConnectionHolder.Connection.EnlistTransaction(null);

                #endregion

                InitConnection();
            }
            catch
            {
                Dispose();

                throw;
            }
        }

        #endregion

        #region Properties

        public DbConnection Connection
        {
            get { return _innerConnectionHolder.Connection; }
        }

        public IDictionary<string, object> ConnectionContextItems { get { return _innerConnectionHolder.ContextItems; } }

        private Stack<ReusableDbConnectionHolder> InnerConnectionHoldersStack
        {
            get
            {
                return _dbConnectionHolderStack;
            }
        }

        #endregion

        #region Static Interface

        public static DbContext CreateDefaultSerializationContext()
        {
            return CreateSerializationContext(ConnectionStrings.DefaultConnectionString, true);
        }

        public static DbContext CreateSerializationContext(ConnectionStringSettings connectionStringSettings, bool useTransaction)
        {
            if (useTransaction)
                return CreateContext(connectionStringSettings, DbContextOptions.UseTransaction);
            else
                return CreateContext(connectionStringSettings, DbContextOptions.None);
        }

        public static DbContext CreateContext(ConnectionStringSettings connectionStringSettings, DbContextOptions options)
        {
            if ((options & DbContextOptions.SuppressConnection) == DbContextOptions.SuppressConnection)
                throw new NotImplementedException("SuppressConnection option is not yet implemented.");

            return new DbContext(connectionStringSettings, options);
        }

        #endregion

        #region Public Interface

        public void Complete()
        {
            if (_activeScope == null)
                throw new NotSupportedException("This DbContext does not use a transaction!");

            if (_completed)
                throw new ObjectDisposedException("Complete Called");

            _completed = true;
        }

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            if (_disposed)
                throw new ObjectDisposedException("DbContext");

            try
            {
                ReleaseContext();
            }
            finally
            {
                if (_activeScope != null)
                {
                    if (_completed)
                        _activeScope.Complete();

                    _activeScope.Dispose();

                    _activeScope = null;
                }
            }

            _disposed = true;
        }

        #endregion

        #region Virtual Methods

        protected virtual void InitConnection()
        {
        }

        #endregion

        #region Helpers

        private void GetOrCreateContext()
        {
            _logicalContextID = (string)CallContext.LogicalGetData(LogicalContextIDName);

            _releaseLogicalContextID = false;

            if (string.IsNullOrEmpty(_logicalContextID))
            {
                _logicalContextID = "dbCtxId_" + Guid.NewGuid().ToString();

                CallContext.LogicalSetData(LogicalContextIDName, _logicalContextID);
            }

            /*Взимаме или създавме (ако няма DbConnectionHolder) за текущата логическа операция
             Може да се получи да има _logicalContextID, но да няма Stack в _innerConnectionHoldersStacks. 
             Това се случва когато dbContext - a се освобождава във вътрешни async методи. (Copy on write Logical Call context and Async).
             Качено с версия - 2.1.4*/
            _dbConnectionHolderStack = _innerConnectionHoldersStacks.GetOrAdd(_logicalContextID, (key) =>
            {
                _releaseLogicalContextID = true;
                return new Stack<ReusableDbConnectionHolder>();
            });
        }

        private void ReleaseContext()
        {
            CheckAndReleaseInnerConnection();

            Stack<ReusableDbConnectionHolder> tmpStack = null;

            if (_releaseLogicalContextID)
            {
                if (_dbConnectionHolderStack.Count != 0)
                    throw new NotImplementedException();

                if (!_innerConnectionHoldersStacks.TryRemove(_logicalContextID, out tmpStack))
                    throw new NotImplementedException();

                _logicalContextID = null;
                _dbConnectionHolderStack = null;

                CallContext.LogicalSetData(LogicalContextIDName, _logicalContextID);
            }
            else
            {
                _logicalContextID = null;
                _dbConnectionHolderStack = null;
            }
        }

        private ReusableDbConnectionHolder InitDbConnectionHolderAndUse(ConnectionStringSettings connectionStringSettings, DbContextOptions options)
        {
            DbConnection dbConnection = null;
            ReusableDbConnectionHolder ret = null;
            try
            {
                dbConnection = DbProviderFactories.GetFactory(connectionStringSettings.ProviderName).CreateConnection();
                dbConnection.ConnectionString = connectionStringSettings.ConnectionString;
                dbConnection.Open();

                ret = new ReusableDbConnectionHolder(GenerateIdentifier(connectionStringSettings.ConnectionString, options), dbConnection);
            }
            catch
            {
                if (dbConnection != null)
                {
                    dbConnection.Close();
                    dbConnection.Dispose();
                }

                throw;
            }

            return ret;
        }

        private void PushInnerConnection(ReusableDbConnectionHolder connectionHolder)
        {
            InnerConnectionHoldersStack.Push(connectionHolder);
        }

        private void CheckAndReleaseInnerConnection()
        {
            if (_innerConnectionHolder != null)
            {
                _innerConnectionHolder.ReleaseConnection();

                if (_innerConnectionHolder.RefCount == 0)
                {
                    var innerConnectionHolderStack = InnerConnectionHoldersStack;

                    var current = innerConnectionHolderStack.Peek();

                    if (current == _innerConnectionHolder)
                    {
                        innerConnectionHolderStack.Pop();

                        _innerConnectionHolder = null;
                    }
                    else
                        throw new NotImplementedException("current == _innerConnectionHolder");
                }
            }
        }

        private ReusableDbConnectionHolder CheckAndUseInnerConnection(string connectionString, DbContextOptions options)
        {
            var innerConnectionHolderStack = InnerConnectionHoldersStack;
            if (innerConnectionHolderStack.Count == 0)
                return null;

            ReusableDbConnectionHolder ret = innerConnectionHolderStack.Peek();

            if (ret.Identifier == GenerateIdentifier(connectionString, options))
            {
                ret.UseConnection();

                return ret;
            }
            else
                return null;
        }

        private string GenerateIdentifier(string connectionString, DbContextOptions options)
        {
            return string.Format("{0}:{1}:{2}",
                connectionString.GetHashCode(),
                options.GetHashCode(),
                System.Transactions.Transaction.Current != null ?
                System.Transactions.Transaction.Current.TransactionInformation.LocalIdentifier :
                "NoTransaction");
        }

        #endregion
    }

    internal class ReusableDbConnectionHolder : IDisposable
    {
        #region Fields

        private DbConnection _connection;
        private IDictionary<string, object> _contextItems;
        private string _identifier;
        private int _refCount;

        #endregion

        #region Constructors

        public ReusableDbConnectionHolder(string identifier, DbConnection connection)
        {
            _connection = connection;
            _identifier = identifier;
            _refCount = 1;
        }

        #endregion

        #region Properties

        public string Identifier
        {
            get
            {
                return _identifier;
            }
        }
        public int RefCount
        {
            get
            {
                return _refCount;
            }
        }
        public DbConnection Connection
        {
            get { return _connection; }
        }

        public IDictionary<string, object> ContextItems
        {
            get
            {
                if (_contextItems == null)
                    _contextItems = new Dictionary<string, object>();

                return _contextItems;
            }
        }

        #endregion

        #region Public Methods

        public void UseConnection()
        {
            if (_refCount <= 0)
                throw new ObjectDisposedException("ReusableDbConnectionHolder");

            _refCount++;
        }
        public int ReleaseConnection()
        {
            if (_refCount <= 0)
                throw new ObjectDisposedException("ReusableDbConnectionHolder");

            _refCount--;

            if (_refCount == 0)
                DisposeInner();

            return _refCount;
        }

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            DisposeInner();
        }

        #endregion

        #region Helpers

        private void DisposeInner()
        {
            _connection.Close();
            _connection.Dispose();
            _connection = null;
        }

        #endregion
    }
}
