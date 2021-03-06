﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using Oracle.ManagedDataAccess.Client;
using Oracle.ManagedDataAccess.Types;
using PropertyRegister.REAU.Applications.Models;
using PropertyRegister.REAU.Persistence;

namespace PropertyRegister.REAU.Applications.Persistence
{
    public class ApplicationProcessDataContext : BaseDataContext
    {
        public ApplicationProcessDataContext(DbConnection dbConnection) : base(dbConnection)
        {
            SqlMapper.SetTypeMap(typeof(Application), DataContextHelper.ColumnMap<Application>());
        }

        public void ApplicationCreate(long? mainApplicationID, long serviceInstanceID, bool isReport,
            int status, DateTime statusTime, int applicationTypeID, DateTime registrationTime, 
            out long applicationID, out string applIdentifier, out string reportIdentifier)
        {
            var parameters = new OracleDynamicParameters();
            parameters.Add("p_ServiceInstance_Id", OracleDbType.Int64, serviceInstanceID);
            parameters.Add("p_MainApplication_Id", OracleDbType.Int64, mainApplicationID);
            parameters.Add("p_Is_Report", OracleDbType.Int32, isReport);
            parameters.Add("p_Status", OracleDbType.Int16, status);
            parameters.Add("p_StatusTime", OracleDbType.TimeStamp, statusTime);
            parameters.Add("p_ApplicationType_Id", OracleDbType.Int16, applicationTypeID);
            parameters.Add("p_RegistrationTime", OracleDbType.TimeStamp, registrationTime);
            parameters.Add("p_Application_Id_Out", OracleDbType.Int64, null, System.Data.ParameterDirection.Output);
            parameters.Add("p_Application_Identifier_Out", OracleDbType.Varchar2, null, System.Data.ParameterDirection.Output, 100);
            parameters.Add("p_Report_Identifier_Out", OracleDbType.Varchar2, null, System.Data.ParameterDirection.Output, 100);

            DbConnection.SPExecute("pkg_services.p_Applications_Create", parameters);

            applicationID = parameters.GetLongNumber("p_Application_Id_Out");
            applIdentifier = parameters.GetString("p_Application_Identifier_Out");
            reportIdentifier = parameters.GetString("p_Report_Identifier_Out");
        }

        public void ApplicationUpdate(long applicationID, long? mainApplicationID, long serviceInstanceID, string applIdentifier, string reportIdentifier, 
            int status, DateTime statusTime, int applicationTypeID, DateTime registrationTime)
        {
            var parameters = new OracleDynamicParameters();
            parameters.Add("p_Application_Id", OracleDbType.Int64, applicationID);
            parameters.Add("p_ServiceInstance_Id", OracleDbType.Int64, serviceInstanceID);
            parameters.Add("p_MainApplication_Id", OracleDbType.Int64, mainApplicationID);
            parameters.Add("p_ApplicationIdentifier", OracleDbType.Varchar2, applIdentifier);
            parameters.Add("p_ReportIdentifier", OracleDbType.Varchar2, reportIdentifier);
            parameters.Add("p_Status", OracleDbType.Int16, status);
            parameters.Add("p_StatusTime", OracleDbType.TimeStamp, statusTime);
            parameters.Add("p_ApplicationType_Id", OracleDbType.Int16, applicationTypeID);
            parameters.Add("p_RegistrationTime", OracleDbType.TimeStamp, registrationTime);

            DbConnection.SPExecute("pkg_services.p_Applications_Update", parameters);
        }

        public CnsysGridReader ApplicationSearch(
            string applicationIDs, long? mainApplicationID, long? serviceInstanceID, string applIdentifier, string reportIdentifier,
            int? status, int? applicationTypeID, int p_StartIndex, int p_PageSize, out int count)
        {
            var parameters = new OracleDynamicParameters();
            parameters.Add("p_Application_Ids", OracleDbType.Varchar2, applicationIDs);
            parameters.Add("p_ServiceInstance_Id", OracleDbType.Int64, serviceInstanceID);
            parameters.Add("p_MainApplication_Id", OracleDbType.Int64, mainApplicationID);
            parameters.Add("p_ApplicationIdentifier", OracleDbType.Varchar2, applIdentifier);
            parameters.Add("p_ReportIdentifier", OracleDbType.Varchar2, reportIdentifier);
            parameters.Add("p_Status", OracleDbType.Int16, status);
            parameters.Add("p_ApplicationType_Id", OracleDbType.Int16, applicationTypeID);
            parameters.Add("p_StartIndex", OracleDbType.Int32, p_StartIndex);
            parameters.Add("p_PageSize", OracleDbType.Int32, p_PageSize);
            parameters.Add("p_ResultsCount_out", OracleDbType.Int32, null, System.Data.ParameterDirection.Output);
            parameters.Add("cv_1", OracleDbType.RefCursor, ParameterDirection.Output);

            var result = DbConnection.SPExecuteReader("pkg_services.p_Applications_Search", parameters);
            count = parameters.Get<OracleDecimal>("p_ResultsCount_out").ToInt32();

            return result;
        }

        #region Service Instance

        public void ServiceInstanceCreate(int officeID, int applicantCIN, out long serviceInstanceID)
        {
            var parameters = new OracleDynamicParameters();
            parameters.Add("p_Office_Id", OracleDbType.Int32, officeID);
            parameters.Add("p_Applicant_CIN", OracleDbType.Int32, applicantCIN);
            parameters.Add("p_ServiceInstance_Id_Out", OracleDbType.Int32, null, System.Data.ParameterDirection.Output);

            DbConnection.SPExecute("pkg_services.p_ServiceInstance_Create", parameters);

            serviceInstanceID = parameters.GetLongNumber("p_ServiceInstance_Id_Out");
        }

        public void ServiceInstanceUpdate(long serviceInstanceID, int officeID, int applicantCIN)
        {
            var parameters = new OracleDynamicParameters();
            parameters.Add("serviceInstanceID", OracleDbType.Int64, serviceInstanceID);
            parameters.Add("p_Office_Id", OracleDbType.Int32, officeID);
            parameters.Add("p_Applicant_CIN", OracleDbType.Int32, applicantCIN);

            DbConnection.SPExecute("pkg_services.p_ServiceInstance_Update", parameters);
        }

        public CnsysGridReader ServiceInstanceSearch(string p_ServiceInstance_Ids, long? p_Office_Id, long? p_Applicant_CIN, int p_StartIndex, int p_PageSize, out int count)
        {
            var parameters = new OracleDynamicParameters();
            
            parameters.Add("p_ServiceInstance_Ids", OracleDbType.Varchar2, p_ServiceInstance_Ids);
            parameters.Add("p_Office_Id", OracleDbType.Int32, p_Office_Id);
            parameters.Add("p_Applicant_CIN", OracleDbType.Int32, p_Applicant_CIN);
            parameters.Add("p_StartIndex", OracleDbType.Int32, p_StartIndex);
            parameters.Add("p_PageSize", OracleDbType.Int32, p_PageSize);
            parameters.Add("p_ResultsCount_out", OracleDbType.Int32, null, System.Data.ParameterDirection.Output);
            parameters.Add("cv_1", OracleDbType.RefCursor, ParameterDirection.Output);

            var reader = DbConnection.SPExecuteReader("PKG_VELIN.p_ServiceInstance_Search", parameters);

            count = parameters.GetIntNumber("p_ResultsCount_out");

            return reader;
        }

        #endregion

        #region Service Actions

        public void ServiceActionCreate(long operationID, long serviceInstanceID, long applicationID, int applicationStatus, int actionTypeID, out long serviceActionID)
        {
            var parameters = new OracleDynamicParameters();
            parameters.Add("p_OperationID", OracleDbType.Int64, operationID);
            parameters.Add("p_ServiceInstID", OracleDbType.Int64, serviceInstanceID);
            parameters.Add("p_ApplicationID", OracleDbType.Int64, applicationID);
            parameters.Add("p_ApplicationStatus", OracleDbType.Int32, applicationStatus);
            parameters.Add("p_ActionTypeID", OracleDbType.Int32, actionTypeID);
            parameters.Add("p_ServiceActionID_out", OracleDbType.Int64, null, System.Data.ParameterDirection.Output);

            DbConnection.SPExecute("pkg_velin.p_Service_Action_Create", parameters);

            serviceActionID = parameters.GetLongNumber("p_ServiceActionID_out");
        }

        #endregion

        #region Application Documents 

        public void ApplicationDocumentCreate(long applicationID, int documentTypeID, long documentID, out long appDocumentID)
        {
            var parameters = new OracleDynamicParameters();
            parameters.Add("p_Application_Id", OracleDbType.Int64, applicationID);
            parameters.Add("p_Document_Type", OracleDbType.Int32, documentTypeID);
            parameters.Add("p_Document_Id", OracleDbType.Int64, documentID);
            parameters.Add("p_App_Document_Id_Out", OracleDbType.Int64, null, System.Data.ParameterDirection.Output);

            DbConnection.SPExecute("pkg_services.p_App_Documents_Create", parameters);

            appDocumentID = parameters.GetLongNumber("p_App_Document_Id_Out");
        }

        #endregion
    }
}
