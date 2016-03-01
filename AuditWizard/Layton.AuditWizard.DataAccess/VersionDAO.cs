using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlServerCe;
using System.IO;
using System.Windows.Forms;

namespace Layton.AuditWizard.DataAccess
{
    public class VersionDAO
    {
        #region Data

        private string connectionStringCompact = null;
        protected static readonly log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private static readonly bool isDebugEnabled = logger.IsDebugEnabled;
        private bool compactDatabaseType = false;
        private Configuration config = ConfigurationManager.OpenExeConfiguration(Path.Combine(Application.StartupPath, "AuditWizardv8.exe"));

        #endregion

        public VersionDAO()
        {
            compactDatabaseType = Convert.ToBoolean(config.AppSettings.Settings["CompactDatabaseType"].Value);
            connectionStringCompact = "Data Source=" + Application.StartupPath + Convert.ToString(config.AppSettings.Settings["ConnectionStringCompact"].Value);
        }

        #region Version Table

        /// <summary>
        /// Recover the version information from the database
        /// </summary>
        /// <param name="assetID"></param>
        /// <param name="theApplication"></param>
        /// <returns></returns>
        public DataTable GetDatabaseVersion()
        {
            if (isDebugEnabled) logger.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " in");

            DataTable table = new DataTable("VERSION");

            if (compactDatabaseType)
            {
                using (SqlCeConnection conn = DatabaseConnection.CreateOpenCEConnection())
                {
                    try
                    {
                        string commandText = "SELECT * FROM VERSION";

                        conn.Open();
                        new SqlCeDataAdapter(new SqlCeCommand(commandText, conn)).Fill(table);
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
                    finally
                    {
                        conn.Dispose();
                        conn.Close();
                    }
                }
            }
            else
            {
                AuditWizardDataAccess lAuditWizardDataAccess = new AuditWizardDataAccess();
                table = lAuditWizardDataAccess.GetDatabaseVersion();
            }

            if (isDebugEnabled) logger.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " out");
            return table;
        }

        public void CheckHBStoredProcs()
        {
            try
            {
                if (compactDatabaseType)
                {
                    return;
                }
                else
                {
                    SettingsDAO lSettingsDao = new SettingsDAO();

                    using (SqlConnection conn = DatabaseConnection.CreateOpenStandardConnection())
                    {
                        string latestHBSPVersion = "1.1";

                        if (lSettingsDao.GetSetting("HBSPAdded", false) != latestHBSPVersion)
                        {
                            ExecuteDatabaseCreateScript(Application.StartupPath + @".\db\CreateHBStoredProcedures.sql", conn);
                            lSettingsDao.SetSetting("HBSPAdded", latestHBSPVersion, false);
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
            catch (Exception ex)
            {
                Utility.DisplayErrorMessage("There was an error adding the HelpBox stored procedures." + Environment.NewLine + Environment.NewLine +
                        "Please see the log file for further details.");

                logger.Error("Exception in " + System.Reflection.MethodBase.GetCurrentMethod().Name, ex);
            }
        }

        private void ExecuteDatabaseCreateScript(string filename, SqlConnection conn)
        {
            try
            {
                FileInfo file = new FileInfo(filename);
                string scriptText = file.OpenText().ReadToEnd();

                string[] splitter = new string[] { "\r\nGO\r\n" };
                string[] commandTexts = scriptText.Split(splitter, StringSplitOptions.RemoveEmptyEntries);
                foreach (string commandText in commandTexts)
                {
                    using (SqlCommand command = new SqlCommand(commandText, conn))
                    {
                        command.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion Version Table
    }
}
