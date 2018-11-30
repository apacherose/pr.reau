using Dapper;
using Oracle.ManagedDataAccess.Client;
using Oracle.ManagedDataAccess.Types;
using PropertyRegister.REAU.Common.Models;
using PropertyRegister.REAU.Persistence;
using System;
using System.Data;
using System.Data.Common;
using System.IO;
using System.Linq;

namespace PropertyRegister.REAU.Common.Persistence
{
    public class CommonDataContext : BaseDataContext
    {
        public CommonDataContext(DbConnection dbConnection) : base(dbConnection)
        {
            SqlMapper.SetTypeMap(typeof(ServiceOperation), DataContextHelper.ColumnMap<ServiceOperation>());
            SqlMapper.SetTypeMap(typeof(DocumentData), DataContextHelper.ColumnMap<DocumentData>());
        }

        public void ServiceOperationCreate(string operationID, int serviceOperationTypeID,
            out long serviceOperationID, out bool isCompleted, out string result)
        {
            var parameters = new OracleDynamicParameters();
            parameters.Add("p_OperationID", OracleDbType.Varchar2, operationID);
            parameters.Add("p_OperationTypeID", OracleDbType.Int32, serviceOperationTypeID);
            parameters.Add("p_ServiceOperationID_out", OracleDbType.Int32, null, System.Data.ParameterDirection.Output);
            parameters.Add("p_IsCompleted_out", OracleDbType.Int32, null, System.Data.ParameterDirection.Output);
            parameters.Add("p_Result_out", OracleDbType.Varchar2, null, System.Data.ParameterDirection.Output, 2000);

            DbConnection.SPExecute("pkg_velin.p_Service_Operation_Create", parameters);

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
            parameters.Add("p_Result", OracleDbType.Varchar2, result);

            DbConnection.SPExecute("pkg_velin.p_Service_Operation_Update", parameters);
        }

        public CnsysGridReader ServiceOperationSearch(long? serviceOperationID, string operationID, int? operationTypeID)
        {
            var parameters = new OracleDynamicParameters();

            parameters.Add("p_ServiceOperationID", OracleDbType.Int64, serviceOperationID);
            parameters.Add("p_OperationID", OracleDbType.Varchar2, operationID);
            parameters.Add("p_OperationTypeID", OracleDbType.Int32, operationTypeID);
            parameters.Add("c_Data", OracleDbType.RefCursor, ParameterDirection.Output);

            return DbConnection.SPExecuteReader("PKG_VELIN.p_Service_Operation_Search", parameters);
        }

        #region documents

        public void DocumentDataCreate(string docIdentifier, string contentType, string filename, bool isTemporal, OracleBlob content, out long documentId)
        {
            var parameters = new OracleDynamicParameters();
            parameters.Add("p_Guid", OracleDbType.Varchar2, docIdentifier);
            parameters.Add("p_ContentType", OracleDbType.Varchar2, contentType);
            parameters.Add("p_Filename", OracleDbType.Varchar2, filename);
            parameters.Add("p_IsTemp", OracleDbType.Int32, isTemporal);
            parameters.Add("p_Content", OracleDbType.Blob, content);
            parameters.Add("p_DocumentId_out", OracleDbType.Int32, null, System.Data.ParameterDirection.Output);

            DbConnection.SPExecute("pkg_velin.p_Document_Data_Create", parameters);

            documentId = parameters.GetLongNumber("p_DocumentId_out");
        }

        public void DocumentDataUpdate(string docIdentifier, string contentType, string filename, bool isTemporal)
        {
            var parameters = new OracleDynamicParameters();
            parameters.Add("p_Guid", OracleDbType.Varchar2, docIdentifier);
            parameters.Add("p_ContentType", OracleDbType.Varchar2, contentType);
            parameters.Add("p_Filename", OracleDbType.Varchar2, filename);
            parameters.Add("p_IsTemp", OracleDbType.Int32, isTemporal);

            DbConnection.SPExecute("pkg_velin.p_Document_Data_Update", parameters);
        }

        public void DocumentDataDelete(Guid key)
        {
            var parameters = new OracleDynamicParameters();
            parameters.Add("p_Guid", OracleDbType.Varchar2, key);

            DbConnection.SPExecute("pkg_velin.p_Document_Data_Delete", parameters);
        }

        public CnsysGridReader DocumentDataSearch(string documentIDs, string documentIdentifiers, int startIndex, int pageSize, out int? count)
        {
            var parameters = new OracleDynamicParameters();

            parameters.Add("p_Document_Ids", OracleDbType.Varchar2, documentIDs);
            parameters.Add("p_Guid_Ids", OracleDbType.Varchar2, documentIdentifiers);
            parameters.Add("p_StartIndex", OracleDbType.Int32, startIndex);
            parameters.Add("p_PageSize", OracleDbType.Int32, pageSize);
            parameters.Add("p_ResultsCount_out", OracleDbType.Int32, null, System.Data.ParameterDirection.Output);
            parameters.Add("p_Data", OracleDbType.RefCursor, ParameterDirection.Output);

            var reader = DbConnection.SPExecuteReader("PKG_VELIN.p_Document_Data_Search", parameters);
            count = parameters.GetIntNumber("p_ResultsCount_out");

            return reader;
        }

        public IDataReader DocumentContentGet(string docIdentifier)
        {
            var parameters = new OracleDynamicParameters();
            parameters.Add("p_Guid", OracleDbType.Varchar2, docIdentifier);
            parameters.Add("c_Data", OracleDbType.RefCursor, ParameterDirection.Output);

            return DbConnection.SPExecuteReader("PKG_VELIN.p_Document_Data_GetContent", parameters, CommandBehavior.SequentialAccess);
        }

        #endregion
    }
}
