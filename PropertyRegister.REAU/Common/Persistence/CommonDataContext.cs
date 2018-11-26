using Dapper;
using Oracle.ManagedDataAccess.Client;
using Oracle.ManagedDataAccess.Types;
using PropertyRegister.REAU.Common.Models;
using PropertyRegister.REAU.Persistence;
using System.Data;
using System.Data.Common;

namespace PropertyRegister.REAU.Common.Persistence
{
    public class CommonDataContext : BaseDataContext
    {
        public CommonDataContext(DbConnection dbConnection) : base(dbConnection)
        {
            SqlMapper.SetTypeMap(typeof(ServiceOperation), DataContextHelper.ColumnMap<ServiceOperation>());
        }

        public void ServiceOperationCreate(string operationID, int serviceOperationTypeID, 
            out long serviceOperationID, out bool isCompleted, out string result)
        {
            var parameters = new OracleDynamicParameters();
            parameters.Add("p_OperationID", OracleDbType.Varchar2, operationID);
            parameters.Add("p_OperationTypeID", OracleDbType.Int32, serviceOperationTypeID);
            parameters.Add("p_ServiceOperationID_out", OracleDbType.Int32, null, System.Data.ParameterDirection.Output);
            parameters.Add("p_IsCompleted_out", OracleDbType.Int32, null, System.Data.ParameterDirection.Output);
            parameters.Add("p_Result_out", OracleDbType.NVarchar2, null, System.Data.ParameterDirection.Output);

            _dbConnection.SPExecute("pkg_velin.p_Service_Operation_Create", parameters);

            serviceOperationID = parameters.GetLongNumber("p_ServiceOperationID_out");
            isCompleted = parameters.GetBoolean("p_IsCompleted_out");
            result = parameters.GetString("p_Result_out");
        }

        public void ServiceOperationUpdate(long serviceOperationID, string operationID, bool isCompleted, string result)
        {
            var parameters = new OracleDynamicParameters();
            parameters.Add("p_ServiceOperationID", OracleDbType.Int64, serviceOperationID);
            parameters.Add("p_OperationID", OracleDbType.Varchar2, operationID);
            parameters.Add("p_IsCompleted", OracleDbType.Int32, isCompleted);
            parameters.Add("p_Result", OracleDbType.NVarchar2, result);

            _dbConnection.SPExecute("pkg_velin.p_Service_Operation_Update", parameters);
        }

        public CnsysGridReader ServiceOperationSearch(long? serviceOperationID, string operationID, int? operationTypeID)
        {
            var parameters = new OracleDynamicParameters();

            parameters.Add("p_ServiceOperationID", OracleDbType.Int64, serviceOperationID);
            parameters.Add("p_OperationID", OracleDbType.Varchar2, operationID);
            parameters.Add("p_OperationTypeID", OracleDbType.Int32, operationTypeID);
            parameters.Add("c_Data", OracleDbType.RefCursor, ParameterDirection.Output);

            return _dbConnection.SPExecuteReader("PKG_VELIN.p_Service_Operation_Search", parameters);
        }
    }
}
