using Dapper;
using Oracle.ManagedDataAccess.Client;
using Oracle.ManagedDataAccess.Types;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;

namespace PropertyRegister.REAU.Persistence
{
    public static class DbContextHelper
    {
        public static void SPExecute(this IDbConnection connection, string procName, SqlMapper.IDynamicParameters parameters = null)
        {            
            connection.Execute(procName, parameters, commandType: CommandType.StoredProcedure);
        }

        public static object SPExecuteScalar(this IDbConnection connection, string procName, SqlMapper.IDynamicParameters parameters = null)
        {
            return connection.ExecuteScalar(string.Format("\"{0}\"", procName), parameters, commandType: CommandType.StoredProcedure);
        }

        public static CnsysGridReader SPExecuteReader(this DbConnection connection, string procName, SqlMapper.IDynamicParameters parameters)
        {
            var reader = connection.ExecuteReader(procName, parameters, commandType: CommandType.StoredProcedure);

            return new CnsysGridReader(reader);
        }

        public static IDataReader SPExecuteReader(this DbConnection connection, string schema, string procName, SqlMapper.IDynamicParameters parameters = null, CommandBehavior behavior = CommandBehavior.Default)
        {
            if (!String.IsNullOrWhiteSpace(schema))
            {
                schema = String.Format("\"{0}\".", schema);
            }
            else schema = "";

            CommandDefinition commandDefinition = new CommandDefinition(string.Format("{0}\"{1}\"", schema, procName), parameters, null, null, CommandType.StoredProcedure, CommandFlags.None);

            var reader = connection.ExecuteReader(commandDefinition, behavior);

            return reader;
        }

        public static async Task<T> TransactionalOperationAsync<T>(Func<Task<T>> func)
        {
            T res = default(T);

            try
            {
                using (var dbContext = REAUDbContext.CreateDefaultSerializationContext())
                {
                    res = await func();

                    dbContext.Complete();
                }
            }
            catch (Exception ex)
            {
                // log ex
                throw;
            }

            return res;
        }

        public static async Task TransactionalOperationAsync(Func<Task> func)
        {
            try
            {
                using (var dbContext = REAUDbContext.CreateDefaultSerializationContext())
                {
                    await func();

                    dbContext.Complete();
                }
            }
            catch (Exception ex)
            {
                // log ex
                throw;
            }
        }
    }

    public class CnsysGridReader : IDisposable
    {
        private IDataReader _reader = null;
        private bool _isFirstCall = true;

        public CnsysGridReader(IDataReader reader)
        {
            _reader = reader;
        }

        public List<T> ReadToList<T>()
        {
            return Read<T>().ToList();
        }

        public IEnumerable<T> Read<T>()
        {
            if (!_isFirstCall)
            {
                _reader.NextResult();
            }
            else
            {
                _isFirstCall = false;
            }

            return _reader.Parse<T>();
        }

        public void Dispose()
        {
            if (_reader != null)
            {
                if (!_reader.IsClosed)
                    _reader.Close();

                _reader.Dispose();
            }
            _reader = null;
        }
    }

    public class OracleDynamicParameters : SqlMapper.IDynamicParameters
    {
        private readonly DynamicParameters dynamicParameters = new DynamicParameters();
        private readonly List<OracleParameter> oracleParameters = new List<OracleParameter>();

        public void Add(string name, OracleDbType oracleDbType, object value = null, ParameterDirection? direction = null)
        {
            ParameterDirection p_direction = direction ?? ParameterDirection.Input;

            var oracleParameter = new OracleParameter(name, oracleDbType, value, p_direction);

            oracleParameters.Add(oracleParameter);
        }

        public void Add(string name, OracleDbType oracleDbType, ParameterDirection direction)
        {
            var oracleParameter = new OracleParameter(name, oracleDbType, direction);
            oracleParameters.Add(oracleParameter);
        }

        public void AddParameters(IDbCommand command, SqlMapper.Identity identity)
        {
            ((SqlMapper.IDynamicParameters)dynamicParameters).AddParameters(command, identity);

            var oracleCommand = command as OracleCommand;

            if (oracleCommand != null)
            {
                oracleCommand.Parameters.AddRange(oracleParameters.ToArray());
            }
        }

        public T Get<T>(string name)
        {
            var paramInfo = oracleParameters.Single(p => p.ParameterName == name);
            object val = paramInfo.Value;

            if (val == DBNull.Value)
            {
                if (default(T) != null)
                {
                    throw new ApplicationException("Attempting to cast a DBNull to a non nullable type! Note that out/return parameters will not have updated values until the data stream completes (after the 'foreach' for Query(..., buffered: false), or after the GridReader has been disposed for QueryMultiple)");
                }
                return default(T);
            }
            return (T)val;
        }

        public int GetIntNumber(string name) => Get<OracleDecimal>(name).ToInt32();

        public long GetLongNumber(string name) => Get<OracleDecimal>(name).ToInt64();

        public bool GetBoolean(string name) => Get<OracleDecimal>(name).ToByte() == 1;

        public string GetString(string name) => 
            Get<OracleString>(name) != null ? Get<OracleString>(name).Value : null;
    }
}
