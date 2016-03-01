using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlServerCe;
using System.IO;
using System.Windows.Forms;

namespace Layton.AuditWizard.DataAccess
{
    public class DatabaseMaintenanceDAO
    {
        #region Data

        protected static readonly log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private static readonly bool isDebugEnabled = logger.IsDebugEnabled;
        private bool compactDatabaseType = false;
        private Configuration config = ConfigurationManager.OpenExeConfiguration(Path.Combine(Application.StartupPath, "AuditWizardv8.exe"));

        #endregion

        public DatabaseMaintenanceDAO()
        {
            compactDatabaseType = Convert.ToBoolean(config.AppSettings.Settings["CompactDatabaseType"].Value);
        }

        #region Private Database Helper Methods

        private string ReplaceStringQuotes(string replaceString)
        {
            if (replaceString.Length > 0)
            {
                // replace single quote with two single quotes
                replaceString.Replace("'", "''");
            }

            return replaceString;
        }

        #endregion

        #region Database Maintenance Functions

        /// <summary>
        /// Return a table containing all of the Groups which have been defined
        /// </summary>
        /// <returns></returns>
        public int DatabasePurge(DatabaseSettings settings)
        {
            if (isDebugEnabled) logger.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " in");
            int rowsPurged = 0;

            // First we need to check for the different types of purging and call each in turn
            if (settings.HistoryPurgeUnits != DatabaseSettings.PURGEUNITS.never)
            {
                DateTime purgeBeforeDate = GetPurgeDate(settings.HistoryPurge, settings.HistoryPurgeUnits);
                rowsPurged += DatabasePurgeAuditTrail(purgeBeforeDate);
            }

            // Now purge the Internet data 
            if (settings.InternetPurgeUnits != DatabaseSettings.PURGEUNITS.never)
            {
                DateTime purgeBeforeDate = GetPurgeDate(settings.InternetPurge, settings.InternetPurgeUnits);
                rowsPurged += DatabasePurgeInternet(purgeBeforeDate);
            }

            // Now purge the Audited Assets
            if (settings.AssetPurgeUnits != DatabaseSettings.PURGEUNITS.never)
            {
                DateTime purgeBeforeDate = GetPurgeDate(settings.AssetPurge, settings.AssetPurgeUnits);
                rowsPurged += DatabasePurgeAssets(purgeBeforeDate);
            }

            // Now purge the Operations - we only store operations for 7 days irrespective any other settings
            DateTime purgeOperationsBeforeDate = GetPurgeDate(7, DatabaseSettings.PURGEUNITS.days);
            rowsPurged += DatabasePurgeOperations(purgeOperationsBeforeDate);

            if (isDebugEnabled) logger.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " out with rows purged : " + rowsPurged);
            return rowsPurged;
        }


        /// <summary>
        /// Purge the AuditTrail table given a date before which items should be discarded
        /// </summary>
        /// <param name="purgeBeforeDate"></param>
        /// <returns></returns>
        public int DatabasePurgeAuditTrail(DateTime purgeBeforeDate)
        {
            if (isDebugEnabled) logger.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " in");
            int lPurgedRecords = 0;

            try
            {
                if (compactDatabaseType)
                {
                    using (SqlCeConnection conn = DatabaseConnection.CreateOpenCEConnection())
                    {
                        string commandText = "DELETE FROM AUDITTRAIL where _DATE < @dtPurgeDate";

                        SqlCeParameter[] spParams = new SqlCeParameter[1];
                        spParams[0] = new SqlCeParameter("@dtPurgeDate", purgeBeforeDate);

                        using (SqlCeCommand command = new SqlCeCommand(commandText, conn))
                        {
                            command.Parameters.AddRange(spParams);
                            lPurgedRecords = command.ExecuteNonQuery();
                        }
                    }
                }
                else
                {
                    using (SqlConnection conn = DatabaseConnection.CreateOpenStandardConnection())
                    {
                        string commandText = "DELETE FROM AUDITTRAIL where _DATE < @dtPurgeDate";

                        SqlParameter[] spParams = new SqlParameter[1];
                        spParams[0] = new SqlParameter("@dtPurgeDate", purgeBeforeDate);

                        using (SqlCommand command = new SqlCommand(commandText, conn))
                        {
                            command.Parameters.AddRange(spParams);
                            lPurgedRecords = command.ExecuteNonQuery();
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

            if (isDebugEnabled) logger.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " out with purged count : " + lPurgedRecords);
            return lPurgedRecords;
        }




        /// <summary>
        /// Purge the Internet table given a date before which items should be discarded
        /// </summary>
        /// <param name="purgeBeforeDate"></param>
        /// <returns></returns>
        public int DatabasePurgeInternet(DateTime purgeBeforeDate)
        {
            if (isDebugEnabled) logger.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " in");
            int lPurgedRecords = 0;
            try
            {
                if (compactDatabaseType)
                {
                    using (SqlCeConnection conn = DatabaseConnection.CreateOpenCEConnection())
                    {
                        string commandText =
                            "DELETE FROM AUDITEDITEMS WHERE _CATEGORY LIKE 'Internet|Cookie|%' " +
                            "AND convert(datetime, _VALUE, 103) < @dtPurgeDate";

                        using (SqlCeCommand command = new SqlCeCommand(commandText, conn))
                        {
                            command.Parameters.AddWithValue("@dtPurgeDate", purgeBeforeDate);
                            lPurgedRecords = command.ExecuteNonQuery();
                        }

                        commandText =
                            "DELETE FROM AUDITEDITEMS WHERE _AUDITEDITEMID IN " +
                            "(SELECT _AUDITEDITEMID FROM AUDITEDITEMS " +
                            "WHERE _CATEGORY LIKE 'Internet|History|%' " +
                            "AND CONVERT(datetime ,SUBSTRING(_CATEGORY, 18, 10), 103) < @dtPurgeDate)";

                        using (SqlCeCommand command = new SqlCeCommand(commandText, conn))
                        {
                            command.Parameters.AddWithValue("@dtPurgeDate", purgeBeforeDate);
                            lPurgedRecords += command.ExecuteNonQuery();
                        }
                    }
                }
                else
                {
                    using (SqlConnection conn = DatabaseConnection.CreateOpenStandardConnection())
                    {
                        string commandText =
                            "DELETE FROM AUDITEDITEMS WHERE _CATEGORY LIKE 'Internet|Cookie|%' " +
                            "AND convert(datetime, _VALUE, 103) < @dtPurgeDate";

                        using (SqlCommand command = new SqlCommand(commandText, conn))
                        {
                            command.Parameters.AddWithValue("@dtPurgeDate", purgeBeforeDate);
                            lPurgedRecords = command.ExecuteNonQuery();
                        }

                        commandText =
                            "DELETE FROM AUDITEDITEMS WHERE _AUDITEDITEMID IN " +
                            "(SELECT _AUDITEDITEMID FROM AUDITEDITEMS " +
                            "WHERE _CATEGORY LIKE 'Internet|History|%' " +
                            "AND CONVERT(datetime ,SUBSTRING(_CATEGORY, 18, 10), 103) < @dtPurgeDate)";

                        using (SqlCommand command = new SqlCommand(commandText, conn))
                        {
                            command.Parameters.AddWithValue("@dtPurgeDate", purgeBeforeDate);
                            lPurgedRecords += command.ExecuteNonQuery();
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

            if (isDebugEnabled) logger.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " out with purged count : " + lPurgedRecords);
            return lPurgedRecords;
        }




        /// <summary>
        /// Purge unaudited assets given a date before which items should be discarded
        /// </summary>
        /// <param name="purgeBeforeDate"></param>
        /// <returns></returns>
        public int DatabasePurgeAssets(DateTime purgeBeforeDate)
        {
            if (isDebugEnabled) logger.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " in");

            AssetDAO lAssetDAO = new AssetDAO();
            int lAssetsPurged = 0;

            try
            {
                if (compactDatabaseType)
                {
                    using (SqlCeConnection conn = DatabaseConnection.CreateOpenCEConnection())
                    {
                        string commandText =
                            "SELECT _ASSETID FROM ASSETS " +
                            "WHERE ( (_LASTAUDIT IS NOT NULL) AND (_LASTAUDIT < @dtPurgeDate) ) OR (_LASTAUDIT IS NULL)";

                        SqlCeParameter[] spParams = new SqlCeParameter[1];
                        spParams[0] = new SqlCeParameter("@dtPurgeDate", purgeBeforeDate);

                        using (SqlCeCommand command = new SqlCeCommand(commandText, conn))
                        {
                            command.Parameters.AddRange(spParams);

                            using (SqlCeDataReader reader = command.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    lAssetDAO.AssetDelete(reader.GetInt32(0));
                                    lAssetsPurged++;
                                }
                            }
                        }
                    }
                }
                else
                {
                    using (SqlConnection conn = DatabaseConnection.CreateOpenStandardConnection())
                    {
                        string commandText =
                            "SELECT _ASSETID FROM ASSETS " +
                            "WHERE ( (_LASTAUDIT IS NOT NULL) AND (_LASTAUDIT < @dtPurgeDate) ) OR (_LASTAUDIT IS NULL)";

                        SqlParameter[] spParams = new SqlParameter[1];
                        spParams[0] = new SqlParameter("@dtPurgeDate", purgeBeforeDate);

                        using (SqlCommand command = new SqlCommand(commandText, conn))
                        {
                            command.Parameters.AddRange(spParams);

                            using (SqlDataReader reader = command.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    lAssetDAO.AssetDelete(reader.GetInt32(0));
                                    lAssetsPurged++;
                                }
                            }
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

            if (isDebugEnabled) logger.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " out with assets purged : " + lAssetsPurged);
            return lAssetsPurged;
        }


        /// <summary>
        /// Purge the Operations table given a date before which items should be discarded
        /// </summary>
        /// <param name="purgeBeforeDate"></param>
        /// <returns></returns>
        public int DatabasePurgeOperations(DateTime purgeBeforeDate)
        {
            if (isDebugEnabled) logger.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " in");
            int lPurgedRecords = 0;

            try
            {
                if (compactDatabaseType)
                {
                    using (SqlCeConnection conn = DatabaseConnection.CreateOpenCEConnection())
                    {
                        string commandText =
                            "DELETE FROM OPERATIONS " +
                            "WHERE _START_DATE < @dtPurgeDate";

                        SqlCeParameter[] spParams = new SqlCeParameter[1];
                        spParams[0] = new SqlCeParameter("@dtPurgeDate", purgeBeforeDate);

                        using (SqlCeCommand command = new SqlCeCommand(commandText, conn))
                        {
                            command.Parameters.AddRange(spParams);
                            lPurgedRecords = command.ExecuteNonQuery();
                        }
                    }
                }
                else
                {
                    using (SqlConnection conn = DatabaseConnection.CreateOpenStandardConnection())
                    {
                        string commandText =
                            "DELETE FROM OPERATIONS " +
                            "WHERE _START_DATE < @dtPurgeDate";

                        SqlParameter[] spParams = new SqlParameter[1];
                        spParams[0] = new SqlParameter("@dtPurgeDate", purgeBeforeDate);

                        using (SqlCommand command = new SqlCommand(commandText, conn))
                        {
                            command.Parameters.AddRange(spParams);
                            lPurgedRecords = command.ExecuteNonQuery();
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

            if (isDebugEnabled) logger.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " out with purged count : " + lPurgedRecords);
            return lPurgedRecords;
        }


        /// <summary>
        /// Calculate the date before records should be purged given the units and count
        /// </summary>
        /// <param name="?"></param>
        /// <returns></returns>
        protected DateTime GetPurgeDate(int count, DatabaseSettings.PURGEUNITS units)
        {
            if (isDebugEnabled) logger.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " in");

            DateTime purgeBeforeDate = DateTime.Now;
            switch ((int)units)
            {
                case (int)DatabaseSettings.PURGEUNITS.days:
                    purgeBeforeDate = purgeBeforeDate.AddDays(-count);
                    break;
                case (int)DatabaseSettings.PURGEUNITS.months:
                    purgeBeforeDate = purgeBeforeDate.AddMonths(-count);
                    break;
                case (int)DatabaseSettings.PURGEUNITS.years:
                    purgeBeforeDate = purgeBeforeDate.AddYears(-count);
                    break;
            }

            if (isDebugEnabled) logger.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " out");
            return purgeBeforeDate;
        }

        public string RunManintenanceScript(string aCmdText)
        {
            return compactDatabaseType ? RunManintenanceScriptCompact(aCmdText) : RunManintenanceScriptFull(aCmdText);
        }

        public string RunManintenanceScriptCompact(string aCmdText)
        {
            string lMsg = "Command(s) completed successfully.";

            using (SqlCeConnection conn = DatabaseConnection.CreateOpenCEConnection())
            {
                SqlCeTransaction transaction = conn.BeginTransaction();
                try
                {
                    string[] commandTexts = aCmdText.Split(new[] { ";" }, StringSplitOptions.RemoveEmptyEntries);

                    foreach (string commandText in commandTexts)
                    {
                        using (SqlCeCommand command = new SqlCeCommand(commandText, conn))
                        {
                            command.Transaction = transaction;
                            command.ExecuteNonQuery();
                        }
                    }

                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    lMsg = ex.Message;
                    logger.Error("Exception in " + System.Reflection.MethodBase.GetCurrentMethod().Name, ex);

                    try
                    {
                        transaction.Rollback();
                    }
                    catch (Exception)
                    {
                        logger.Error("Exception in " + System.Reflection.MethodBase.GetCurrentMethod().Name, ex);
                    }
                }
            }

            return lMsg;
        }

        public string RunManintenanceScriptFull(string aCmdText)
        {
            string lMsg = "Command(s) completed successfully.";

            using (SqlConnection conn = DatabaseConnection.CreateOpenStandardConnection())
            {
                SqlTransaction transaction = conn.BeginTransaction();

                try
                {
                    string[] commandTexts = aCmdText.Split(new[] { ";" }, StringSplitOptions.RemoveEmptyEntries);

                    foreach (string commandText in commandTexts)
                    {
                        using (SqlCommand command = new SqlCommand(commandText, conn))
                        {
                            command.Transaction = transaction;
                            command.ExecuteNonQuery();
                        }
                    }

                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    lMsg = ex.Message;
                    logger.Error("Exception in " + System.Reflection.MethodBase.GetCurrentMethod().Name, ex);

                    try
                    {
                        transaction.Rollback();
                    }
                    catch (Exception)
                    {
                        logger.Error("Exception in " + System.Reflection.MethodBase.GetCurrentMethod().Name, ex);
                    }
                }
            }

            return lMsg;
        }

        #endregion Database Maintenance Functions
    }
}
