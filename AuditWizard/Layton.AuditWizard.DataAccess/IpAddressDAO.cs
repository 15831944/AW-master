using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlServerCe;
using System.IO;
using System.Windows.Forms;

namespace Layton.AuditWizard.DataAccess
{
    public class LayIpAddressDAO
    {
        #region Data

        protected static readonly log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private static readonly bool isDebugEnabled = logger.IsDebugEnabled;
        private readonly bool compactDatabaseType;
        private readonly Configuration config = ConfigurationManager.OpenExeConfiguration(Path.Combine(Application.StartupPath, "AuditWizardv8.exe"));

        #endregion

        public LayIpAddressDAO()
        {
            compactDatabaseType = Convert.ToBoolean(config.AppSettings.Settings["CompactDatabaseType"].Value);
        }

        public void CheckTablePresent()
        {
            string commandText =
                "SELECT COUNT(*) FROM INFORMATION_SCHEMA.TABLES " +
                "WHERE TABLE_NAME = 'IP_ADDRESSES'";

            if (PerformScalarQuery(commandText) == 0)
            {
                commandText =
                    "CREATE TABLE IP_ADDRESSES ( " +
                    "_IP_ADDRESS_ID int IDENTITY(1,1) NOT NULL , " +
                    "_IP_START nvarchar(15) NOT NULL , " +
                    "_IP_END nvarchar(15) NOT NULL , " +
                    "_ACTIVE bit DEFAULT 0 NOT NULL , " +
                    "_IP_TYPE int NOT NULL )";

                ExecuteNonQuery(commandText);

                commandText = "ALTER TABLE IP_ADDRESSES ADD CONSTRAINT PK_IP_ADDRESSES PRIMARY KEY (_IP_ADDRESS_ID);";
                ExecuteNonQuery(commandText);
            }
        }

        public void DeleteAllTcpIp()
        {
            if (isDebugEnabled) logger.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " in");

            ExecuteNonQuery("DELETE FROM IP_ADDRESSES WHERE _IP_TYPE = 1");

            if (isDebugEnabled) logger.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " out");
        }

        public void DeleteAllSnmp()
        {
            if (isDebugEnabled) logger.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " in");

            ExecuteNonQuery("DELETE FROM IP_ADDRESSES WHERE _IP_TYPE = 2");

            if (isDebugEnabled) logger.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " out");
        }

        public string Add(IPAddressRange ipAddress)
        {
            if (isDebugEnabled) logger.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " in");

            string returnSetting = String.Empty;

            try
            {
                string cmdText =
                    "INSERT INTO IP_ADDRESSES ( " +
                    "_IP_START, _IP_END, _ACTIVE, _IP_TYPE) " +
                    "VALUES (@ipStart, @ipEnd, @active, @type)";

                if (compactDatabaseType)
                {
                    using (SqlCeConnection conn = DatabaseConnection.CreateOpenCEConnection())
                    {
                        using (SqlCeCommand command = new SqlCeCommand(cmdText, conn))
                        {
                            command.Parameters.AddWithValue("@ipStart", ipAddress.IpStart);
                            command.Parameters.AddWithValue("@ipEnd", ipAddress.IpEnd);
                            command.Parameters.AddWithValue("@active", ipAddress.Active);
                            command.Parameters.AddWithValue("@type", (int)ipAddress.TypeOfIp);

                            command.ExecuteNonQuery();
                        }
                    }
                }
                else
                {
                    using (SqlConnection conn = DatabaseConnection.CreateOpenStandardConnection())
                    {
                        using (SqlCommand command = new SqlCommand(cmdText, conn))
                        {
                            command.Parameters.AddWithValue("@ipStart", ipAddress.IpStart);
                            command.Parameters.AddWithValue("@ipEnd", ipAddress.IpEnd);
                            command.Parameters.AddWithValue("@active", ipAddress.Active);
                            command.Parameters.AddWithValue("@type", (int)ipAddress.TypeOfIp);

                            command.ExecuteNonQuery();
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                Utility.DisplayErrorMessage("A database error has occurred in AuditWizard." + Environment.NewLine + Environment.NewLine +
                        "Please see the log file for further details.");
                logger.Error("Exception in " + System.Reflection.MethodBase.GetCurrentMethod().Name, ex);
            }
            catch (SqlCeException ex)
            {
                Utility.DisplayErrorMessage("A database error has occurred in AuditWizard." + Environment.NewLine + Environment.NewLine +
                        "Please see the log file for further details.");
                logger.Error("Exception in " + System.Reflection.MethodBase.GetCurrentMethod().Name, ex);
            }
            catch (Exception ex)
            {
                Utility.DisplayErrorMessage("A database error has occurred in AuditWizard." + Environment.NewLine + Environment.NewLine +
                        "Please see the log file for further details.");

                logger.Error("Exception in " + System.Reflection.MethodBase.GetCurrentMethod().Name, ex);
            }


            if (isDebugEnabled) logger.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " out with setting : " + returnSetting);
            return returnSetting;
        }

        public DataTable SelectAllTcpIp()
        {
            if (isDebugEnabled) logger.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " in");

            DataTable dt = PerformQuery("SELECT _IP_START, _IP_END, _ACTIVE FROM IP_ADDRESSES WHERE _IP_TYPE = 1");

            if (isDebugEnabled) logger.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " out");

            return dt;
        }

        public DataTable SelectAllSnmp()
        {
            if (isDebugEnabled) logger.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " in");

            DataTable dt = PerformQuery("SELECT _IP_START, _IP_END, _ACTIVE FROM IP_ADDRESSES WHERE _IP_TYPE = 2");

            if (isDebugEnabled) logger.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " out");

            return dt;
        }

        public DataTable SelectAllActiveTcpIp()
        {
            if (isDebugEnabled) logger.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " in");

            DataTable dt = PerformQuery("SELECT _IP_START, _IP_END, _ACTIVE FROM IP_ADDRESSES WHERE _ACTIVE = 1 AND _IP_TYPE = 1");

            if (isDebugEnabled) logger.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " out");

            return dt;
        }

        public DataTable SelectAllActiveSnmp()
        {
            if (isDebugEnabled) logger.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " in");

            DataTable dt = PerformQuery("SELECT _IP_START, _IP_END, _ACTIVE FROM IP_ADDRESSES WHERE _ACTIVE = 1 AND _IP_TYPE = 2");

            if (isDebugEnabled) logger.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " out");

            return dt;
        }

        private DataTable PerformQuery(string cmdText)
        {
            DataTable dt = new DataTable();

            try
            {
                if (compactDatabaseType)
                {
                    using (SqlCeConnection conn = DatabaseConnection.CreateOpenCEConnection())
                    {
                        using (SqlCeCommand command = new SqlCeCommand(cmdText, conn))
                        {
                            new SqlCeDataAdapter(command).Fill(dt);
                        }
                    }
                }
                else
                {
                    using (SqlConnection conn = DatabaseConnection.CreateOpenStandardConnection())
                    {
                        using (SqlCommand command = new SqlCommand(cmdText, conn))
                        {
                            new SqlDataAdapter(command).Fill(dt);
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                Utility.DisplayErrorMessage("A database error has occurred in AuditWizard." + Environment.NewLine + Environment.NewLine +
                        "Please see the log file for further details.");
                logger.Error("Exception in " + System.Reflection.MethodBase.GetCurrentMethod().Name, ex);
            }
            catch (SqlCeException ex)
            {
                Utility.DisplayErrorMessage("A database error has occurred in AuditWizard." + Environment.NewLine + Environment.NewLine +
                        "Please see the log file for further details.");
                logger.Error("Exception in " + System.Reflection.MethodBase.GetCurrentMethod().Name, ex);
            }
            catch (Exception ex)
            {
                Utility.DisplayErrorMessage("A database error has occurred in AuditWizard." + Environment.NewLine + Environment.NewLine +
                        "Please see the log file for further details.");

                logger.Error("Exception in " + System.Reflection.MethodBase.GetCurrentMethod().Name, ex);
            }

            return dt;
        }

        private void ExecuteNonQuery(string cmdText)
        {
            try
            {
                if (compactDatabaseType)
                {
                    using (SqlCeConnection conn = DatabaseConnection.CreateOpenCEConnection())
                    {
                        using (SqlCeCommand command = new SqlCeCommand(cmdText, conn))
                        {
                            command.ExecuteNonQuery();
                        }
                    }
                }
                else
                {
                    using (SqlConnection conn = DatabaseConnection.CreateOpenStandardConnection())
                    {
                        using (SqlCommand command = new SqlCommand(cmdText, conn))
                        {
                            command.ExecuteNonQuery();
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                Utility.DisplayErrorMessage("A database error has occurred in AuditWizard." + Environment.NewLine + Environment.NewLine +
                        "Please see the log file for further details.");
                logger.Error("Exception in " + System.Reflection.MethodBase.GetCurrentMethod().Name, ex);
            }
            catch (SqlCeException ex)
            {
                Utility.DisplayErrorMessage("A database error has occurred in AuditWizard." + Environment.NewLine + Environment.NewLine +
                        "Please see the log file for further details.");
                logger.Error("Exception in " + System.Reflection.MethodBase.GetCurrentMethod().Name, ex);
            }
            catch (Exception ex)
            {
                Utility.DisplayErrorMessage("A database error has occurred in AuditWizard." + Environment.NewLine + Environment.NewLine +
                        "Please see the log file for further details.");

                logger.Error("Exception in " + System.Reflection.MethodBase.GetCurrentMethod().Name, ex);
            }
        }

        private int PerformScalarQuery(string commandText)
        {
            int returnValue = -1;

            try
            {
                if (compactDatabaseType)
                {
                    using (SqlCeConnection conn = DatabaseConnection.CreateOpenCEConnection())
                    {
                        using (SqlCeCommand command = new SqlCeCommand(commandText, conn))
                        {
                            returnValue = Convert.ToInt32(command.ExecuteScalar());
                        }
                    }
                }
                else
                {
                    using (SqlConnection conn = DatabaseConnection.CreateOpenStandardConnection())
                    {
                        using (SqlCommand command = new SqlCommand(commandText, conn))
                        {
                            returnValue = Convert.ToInt32(command.ExecuteScalar());
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                Utility.DisplayErrorMessage("A database error has occurred in AuditWizard." + Environment.NewLine + Environment.NewLine +
                        "Please see the log file for further details.");
                logger.Error("Exception in " + System.Reflection.MethodBase.GetCurrentMethod().Name, ex);
            }
            catch (SqlCeException ex)
            {
                Utility.DisplayErrorMessage("A database error has occurred in AuditWizard." + Environment.NewLine + Environment.NewLine +
                        "Please see the log file for further details.");
                logger.Error("Exception in " + System.Reflection.MethodBase.GetCurrentMethod().Name, ex);
            }
            catch (Exception ex)
            {
                Utility.DisplayErrorMessage("A database error has occurred in AuditWizard." + Environment.NewLine + Environment.NewLine +
                        "Please see the log file for further details.");

                logger.Error("Exception in " + System.Reflection.MethodBase.GetCurrentMethod().Name, ex);
            }

            return returnValue;
        }
    }
}
