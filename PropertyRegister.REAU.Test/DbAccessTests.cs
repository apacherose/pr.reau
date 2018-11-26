using Microsoft.VisualStudio.TestTools.UnitTesting;
using Oracle.ManagedDataAccess.Client;
using PropertyRegister.REAU.Persistence;
using System.Data;
using System.Data.Common;
using System.Linq;

namespace PropertyRegister.REAU.Test
{
    [TestClass]
    public class DbAccessTests
    {
        [TestMethod]
        public void Test_Oracle_Access()
        {
            string connString = "Data Source=comreg;User ID=comreg_dev2;Password=comreg_dev2;Pooling=true;HA Events=false;Connection Timeout=60";
            string ProviderName = "Oracle.ManagedDataAccess.Client";
            
            using (var con = DbProviderFactories.GetFactory(ProviderName).CreateConnection())
            {
                con.ConnectionString = connString;

                con.Open();

                // 01

                var dyParam = new OracleDynamicParameters();
                dyParam.Add("res", OracleDbType.RefCursor, ParameterDirection.Output);
                var query = "PKG_USERS.GetAllRoles";

                var reader = con.SPExecuteReader(query, dyParam);
                var data = reader.Read<Role>().ToList();

                // 02

                query = "PKG_USERS.getuserrolesbyid";
                dyParam = new OracleDynamicParameters();
                dyParam.Add("puser_id", OracleDbType.Int32, 835);
                dyParam.Add("res", OracleDbType.RefCursor, ParameterDirection.Output);
                reader = con.SPExecuteReader(query, dyParam);

                var userroles = reader.Read<UserRole>().ToList();

                // 03
                query = "pkg_integration_epzeu.p_indexedfields_search";
                dyParam = new OracleDynamicParameters();
                dyParam.Add("p_resultscount_out", OracleDbType.Int32, ParameterDirection.Output);
                dyParam.Add("p_startindex", OracleDbType.Int32, 1);
                dyParam.Add("p_pagesize", OracleDbType.Int32, 20);
                dyParam.Add("p_uic", OracleDbType.NVarchar2, null);
                dyParam.Add("p_name", OracleDbType.NVarchar2, "БЪЛГАРИЯ");
                dyParam.Add("p_typeid", OracleDbType.Int32, null);
                dyParam.Add("res", OracleDbType.RefCursor, ParameterDirection.Output);
                reader = con.SPExecuteReader(query, dyParam);

                var idxFields = reader.Read<IndexedField>().ToList();

                // 04

                dyParam = new OracleDynamicParameters();
                dyParam.Add("res1", OracleDbType.RefCursor, ParameterDirection.Output);
                dyParam.Add("res2", OracleDbType.RefCursor, ParameterDirection.Output);
                query = "pkg_test_2.GetAllRoles2";

                reader = con.SPExecuteReader(query, dyParam);
                var roles1 = reader.Read<Role>().ToList();
                var userroles1 = reader.Read<UserRole>().ToList();
            }

            Assert.IsTrue(true);
        }
    }

    public class Role
    {
        //[DapperColumn("code")]
        public string Role_name { get; set; }
    }

    public class UserRole
    {
        public int Role_ID { get; set; }
    }

    public class IndexedField
    {
        public string Name { get; set; }
    }
}
