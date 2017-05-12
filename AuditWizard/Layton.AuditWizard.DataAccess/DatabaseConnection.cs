using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlServerCe;
using System.IO;
using System.Windows.Forms;

namespace Layton.AuditWizard.DataAccess
{
    public static class DatabaseConnection
    {
        private static SqlCeConnection _conn = null;
        private static Configuration config = ConfigurationManager.OpenExeConfiguration(Path.Combine(Application.StartupPath, "AuditWizardv8.exe"));
        private static string _connectionStringCompact =
            "Data Source=" + Application.StartupPath + Convert.ToString(config.AppSettings.Settings["ConnectionStringCompact"].Value +
            "; Max Database Size = 1024; Max Buffer Size = 2048");

        private static bool _compactDatabaseType;

        private static readonly log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        static DatabaseConnection()
        {
            _compactDatabaseType = Convert.ToBoolean(config.AppSettings.Settings["CompactDatabaseType"].Value);
        }

        public static bool TestConnection()
        {
            bool connected;

            if (_compactDatabaseType)
            {
                using (SqlCeConnection sqlCeConnection = CreateOpenCEConnection())
                {
                    connected = (sqlCeConnection != null && sqlCeConnection.State == ConnectionState.Open);
                }
            }
            else
            {
                using (SqlConnection sqlCeConnection = CreateOpenStandardConnection())
                {
                    connected = (sqlCeConnection != null && sqlCeConnection.State == ConnectionState.Open);
                }
            }

            return connected;
        }

        /// <summary>
        /// Open or get a connection to the Compact Database
        /// </summary>
        /// <returns>Compact Database SqlCeConnection object</returns>
        public static SqlCeConnection OpenCeConnection()
        {
            // if we are already have an open connection then use it
            try
            {
                if (_conn != null)
                {
                    // connection exists, check if alredy open
                    if (_conn.State == System.Data.ConnectionState.Closed)
                    {
                        _conn = new SqlCeConnection(_connectionStringCompact);
                        _conn.Open();
                    }
                }
                else
                {
                    // connection doesn't exist so create and open
                    config = ConfigurationManager.OpenExeConfiguration(Path.Combine(Application.StartupPath, "AuditWizardv8.exe"));
                    _conn = new SqlCeConnection(_connectionStringCompact);
                    _conn.Open();
                }
            }
            catch (Exception ex)
            {
                logger.Error("Error in OpenCeConnection()", ex);
                Utility.DisplayErrorMessage(ex.Message);
            }

            return _conn;
        }

        /// <summary>
        /// Close the connection to the Compact Database
        /// </summary>
        public static void CloseCeConnection()
        {
            try
            {
                if (_conn != null)
                {
                    _conn.Dispose();
                    _conn.Close();
                }
            }
            catch (Exception ex)
            {
                logger.Error("Error in OpenCeConnection()", ex);
            }
        }

        /// <summary>
        /// Create and open a new connection
        /// </summary>
        /// <returns>Newly created and opened SqlCeConnection object</returns>
        public static SqlCeConnection CreateOpenCEConnection()
        {
            SqlCeConnection connection = null;
            try
            {
                connection = new SqlCeConnection(_connectionStringCompact);
                connection.Open();
            }
            catch (Exception ex)
            {
                logger.Error("Error in OpenCeConnection()", ex);
            }

            return connection;
        }

        /// <summary>
        /// Create an open connection handling any errors by allowing the user to specify connection
        /// parameters
        /// </summary>
        /// <returns></returns>
        public static SqlConnection CreateOpenStandardConnection()
        {
            SqlConnection sqlConn = null;
            SqlConnectionStringBuilder cb = new SqlConnectionStringBuilder();

            try
            {
                Configuration config = ConfigurationManager.OpenExeConfiguration(Path.Combine(Application.StartupPath, "AuditWizardv8.exe"));
                cb.DataSource = config.AppSettings.Settings["ConnectionStringExpressDataSource"].Value;
                cb.InitialCatalog = config.AppSettings.Settings["ConnectionStringExpressInitialCatalog"].Value;
                cb.IntegratedSecurity = Convert.ToBoolean(config.AppSettings.Settings["ConnectionStringExpressIntegratedSecurity"].Value);

                if (!cb.IntegratedSecurity)
                {
                    cb.UserID = config.AppSettings.Settings["ConnectionStringExpressUserID"].Value;
                    cb.Password = AES.Decrypt(config.AppSettings.Settings["ConnectionStringExpressPassword"].Value);
                }

                sqlConn = new SqlConnection(cb.ToString());
                sqlConn.Open();
            }
            catch (Exception ex)
            {
                logger.Error("Error in OpenStandardConnection()", ex);
            }

            return sqlConn;
        }

        /// <summary>
        /// Create an open connection handling any errors by allowing the user to specify connection
        /// parameters
        /// </summary>
        /// <returns></returns>
        public static SqlConnection CreateOpenStandardConnectionnoTimeOut()
        {
            SqlConnection sqlConn = null;
            SqlConnectionStringBuilder cb = new SqlConnectionStringBuilder();

            try
            {
                Configuration config = ConfigurationManager.OpenExeConfiguration(Path.Combine(Application.StartupPath, "AuditWizardv8.exe"));
                cb.DataSource = config.AppSettings.Settings["ConnectionStringExpressDataSource"].Value;
                cb.InitialCatalog = config.AppSettings.Settings["ConnectionStringExpressInitialCatalog"].Value;
                cb.IntegratedSecurity = Convert.ToBoolean(config.AppSettings.Settings["ConnectionStringExpressIntegratedSecurity"].Value);
                cb.ConnectTimeout = 0;
                if (!cb.IntegratedSecurity)
                {
                    cb.UserID = config.AppSettings.Settings["ConnectionStringExpressUserID"].Value;
                    cb.Password = AES.Decrypt(config.AppSettings.Settings["ConnectionStringExpressPassword"].Value);
                }

                sqlConn = new SqlConnection(cb.ToString());
                sqlConn.Open();
            }
            catch (Exception ex)
            {
                logger.Error("Error in OpenStandardConnection()", ex);
            }

            return sqlConn;
        }
    }
}
