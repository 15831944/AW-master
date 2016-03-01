using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlServerCe;
using System.IO;
using System.Windows.Forms;

namespace Layton.AuditWizard.DataAccess
{
    public class PublisherAliasAppDAO
    {
        #region Data

        private string connectionStringCompact = null;
        protected static readonly log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private static readonly bool isDebugEnabled = logger.IsDebugEnabled;
        private bool compactDatabaseType = false;
        private Configuration config = ConfigurationManager.OpenExeConfiguration(Path.Combine(Application.StartupPath, "AuditWizardv8.exe"));

        #endregion

        public PublisherAliasAppDAO()
        {
            compactDatabaseType = Convert.ToBoolean(config.AppSettings.Settings["CompactDatabaseType"].Value);
            connectionStringCompact = "Data Source=" + Application.StartupPath + Convert.ToString(config.AppSettings.Settings["ConnectionStringCompact"].Value);
        }

        public void CheckTablePresent()
        {
            string commandText =
                "SELECT COUNT(*) FROM INFORMATION_SCHEMA.TABLES " +
                "WHERE TABLE_NAME = 'PUBLISHER_ALIAS_APP'";

            if (PerformScalarQuery(commandText) == 0)
            {
                commandText =
                    "CREATE TABLE PUBLISHER_ALIAS_APP (" +
                    "_ID int IDENTITY(1,1) NOT NULL ," +
                    "_APPLICATIONID int NOT NULL ," +
                    "_PUBLISHER_ALIAS_ID int NOT NULL)";

                ExecuteNonQuery(commandText);

                commandText = "ALTER TABLE PUBLISHER_ALIAS_APP ADD CONSTRAINT PK_PUBLISHER_ALIAS_APP PRIMARY KEY (_ID)";
                ExecuteNonQuery(commandText);
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

        /// <summary>
        /// Insert a record into PUBLISHER_ALIAS_APP table
        /// </summary>
        /// <returns></returns>
        public void InsertNewApplication(int aApplicationId, string aOriginalPublisher)
        {
            if (isDebugEnabled) logger.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " in");

            string commandText =
                String.Format(
                "INSERT INTO PUBLISHER_ALIAS_APP " +
                "(_APPLICATIONID, _PUBLISHER_ALIAS_ID) " +
                "SELECT {0}, _PUBLISHER_ALIAS_ID FROM PUBLISHER_ALIAS WHERE _ORIG_PUBLISHER = '{1}'", aApplicationId, aOriginalPublisher);

            ExecuteNonQuery(commandText);

            if (isDebugEnabled) logger.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " out");
        }

        /// <summary>
        /// Insert a record into PUBLISHER_ALIAS_APP table
        /// </summary>
        /// <returns></returns>
        public void Insert(int aApplicationId, int aPublisherAliasId)
        {
            if (isDebugEnabled) logger.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " in");

            //string commandText =
            //    String.Format(
            //    "INSERT INTO PUBLISHER_ALIAS_APP " +
            //    "(_PUBLISHER_ALIAS_ID, _APPLICATIONID) " +
            //    "SELECT {0}, _APPLICATIONID FROM APPLICATIONS WHERE _PUBLISHER = '{1}'", aPublisherAliasId, aOriginalPublisher);

            string commandText =
                String.Format(
                "INSERT INTO PUBLISHER_ALIAS_APP " +
                "(_APPLICATIONID, _PUBLISHER_ALIAS_ID) " +
                "VALUES ({0}, {1})", aApplicationId, aPublisherAliasId);

            ExecuteNonQuery(commandText);

            if (isDebugEnabled) logger.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " out");
        }

        /// <summary>
        /// Deletes a record by PUBLISHER_ALIAS_ID
        /// </summary>
        /// <returns></returns>
        public void DeleteByPublisherAliasId(int aPublisherAliasId)
        {
            if (isDebugEnabled) logger.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " in");

            string commandText =
                String.Format(
                "DELETE FROM PUBLISHER_ALIAS_APP " +
                "WHERE _PUBLISHER_ALIAS_ID = {0}", aPublisherAliasId);

            ExecuteNonQuery(commandText);

            if (isDebugEnabled) logger.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " out");
        }

        public void DeleteByOriginalPublisherName(string aOriginalPublisherName)
        {
            if (isDebugEnabled) logger.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " in");

            string commandText = String.Format(
                "DELETE FROM PUBLISHER_ALIAS_APP " +
                "WHERE _PUBLISHER_ALIAS_ID IN " +
                "(SELECT _PUBLISHER_ALIAS_ID " +
                "FROM PUBLISHER_ALIAS " + 
                "WHERE _ORIG_PUBLISHER = '{0}')", aOriginalPublisherName);

            ExecuteNonQuery(commandText);

            if (isDebugEnabled) logger.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " out");
        }

        private void ExecuteNonQuery(string aCommandText)
        {
            try
            {
                if (compactDatabaseType)
                {
                    using (SqlCeConnection conn = DatabaseConnection.CreateOpenCEConnection())
                    {
                        using (SqlCeCommand command = new SqlCeCommand(aCommandText, conn))
                        {
                            command.ExecuteNonQuery();
                        }
                    }
                }
                else
                {
                    using (SqlConnection conn = DatabaseConnection.CreateOpenStandardConnection())
                    {
                        using (SqlCommand command = new SqlCommand(aCommandText, conn))
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
    }
}


