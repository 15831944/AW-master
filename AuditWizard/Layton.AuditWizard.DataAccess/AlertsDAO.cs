using System;
using System.Configuration;
using System.Data;
using System.Data.SqlServerCe;
using System.IO;
using System.Windows.Forms;

namespace Layton.AuditWizard.DataAccess
{
    public class AlertsDAO
    {
        #region Data

        private string connectionStringCompact;
        protected static readonly log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private static readonly bool isDebugEnabled = logger.IsDebugEnabled;
        private bool compactDatabaseType;
        private Configuration config = ConfigurationManager.OpenExeConfiguration(Path.Combine(Application.StartupPath, "AuditWizardv8.exe"));

        #endregion

        public AlertsDAO()
        {
            compactDatabaseType = Convert.ToBoolean(config.AppSettings.Settings["CompactDatabaseType"].Value);
            connectionStringCompact = "Data Source=" + Application.StartupPath + Convert.ToString(config.AppSettings.Settings["ConnectionStringCompact"].Value);
        }

        #region Alerts Table

        /// <summary>
        /// Return a table containing all of the Alerts have been triggered since the specified date
        /// </summary>
        /// <returns></returns>
        public DataTable EnumerateAlerts(DateTime aSinceDate)
        {
            if (isDebugEnabled) logger.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " in");

            DataTable alertsTable = new DataTable(TableNames.ALERTS);

            if (compactDatabaseType)
            {
                try
                {
                    using (SqlCeConnection conn = DatabaseConnection.CreateOpenCEConnection())
                    {
                        string commandText =
                            "SELECT ALERTS.* " +
	                        "FROM ALERTS " +
                            "WHERE _ALERTDATE >= @dtSinceDate " +
	                        "ORDER BY _ALERTID";

                        using (SqlCeCommand command = new SqlCeCommand(commandText, conn))
                        {
                            command.Parameters.AddWithValue("@dtSinceDate", aSinceDate);
                            new SqlCeDataAdapter(command).Fill(alertsTable);
                        }
                    }
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
            else
            {
                AuditWizardDataAccess lAuditWizardDataAccess = new AuditWizardDataAccess();
                alertsTable = lAuditWizardDataAccess.EnumerateAlerts(aSinceDate);
            }

            if (isDebugEnabled) logger.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " out");
            return alertsTable;
        }



        /// <summary>
        /// Add a new Alert to the database
        /// </summary>
        /// <param name="theAlert"></param>
        /// <returns></returns>
        public int AlertAdd(Alert aAlert)
        {
            if (isDebugEnabled) logger.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " in");

            int lItemID = 0;

            if (compactDatabaseType)
            {
                try
                {
                    using (SqlCeConnection conn = DatabaseConnection.CreateOpenCEConnection())
                    {
                        string commandText =
                            "INSERT INTO ALERTS " +
			                "(_TYPE, _CATEGORY ,_MESSAGE ,_FIELD1 ,_FIELD2 ,_STATUS ,_ALERTDATE ,_ASSETNAME ,_ALERTNAME) " +
		                    "VALUES " +
			                "(@nType, @nCategory, @cMessage, @cField1, @cField2 ,0 ,@dtmDate ,@cAssetName ,@cAlertName)";

                        SqlCeParameter[] spParams = new SqlCeParameter[8];
                        spParams[0] = new SqlCeParameter("@nType", (int)aAlert.Type);
                        spParams[1] = new SqlCeParameter("@nCategory", (int)aAlert.Category);
                        spParams[2] = new SqlCeParameter("@cMessage", aAlert.Message);
                        spParams[3] = new SqlCeParameter("@cField1", aAlert.Field1);
                        spParams[4] = new SqlCeParameter("@cField2", aAlert.Field2);
                        spParams[5] = new SqlCeParameter("@dtmDate", DateTime.Now);
                        spParams[6] = new SqlCeParameter("@cAssetName", aAlert.AssetName);
                        spParams[7] = new SqlCeParameter("@cAlertName", aAlert.AlertName);

                        using (SqlCeCommand command = new SqlCeCommand(commandText, conn))
                        {
                            command.Parameters.AddRange(spParams);
                            command.ExecuteNonQuery();
                        }

                        using (SqlCeCommand commandReturnValue = new SqlCeCommand("SELECT @@IDENTITY", conn))
                        {
                            lItemID = Convert.ToInt32(commandReturnValue.ExecuteScalar());
                        }
                    }
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
            else
            {
                AuditWizardDataAccess lAuditWizardDataAccess = new AuditWizardDataAccess();
                lItemID = lAuditWizardDataAccess.AlertAdd(aAlert);
            }

            if (isDebugEnabled) logger.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " out");
            return lItemID;
        }


        /// <summary>
        /// Update the last alert date for the specified alert
        /// </summary>
        /// <param name="theAsset"></param>
        /// <returns></returns>
        public int AlertDateUpdate(Alert aAlert)
        {
            if (isDebugEnabled) logger.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " in");

            if (compactDatabaseType)
            {
                try
                {
                    using (SqlCeConnection conn = DatabaseConnection.CreateOpenCEConnection())
                    {
                        string commandText =
                            "UPDATE ALERTS SET _ALERTDATE = @dtAlertDate " +
				            "WHERE _ALERTID = @nAlertID";

                        SqlCeParameter[] spParams = new SqlCeParameter[2];
                        spParams[0] = new SqlCeParameter("@nAlertID", aAlert.AlertID);
                        spParams[1] = new SqlCeParameter("@dtAlertDate", aAlert.AlertedOnDate);

                        using (SqlCeCommand command = new SqlCeCommand(commandText, conn))
                        {
                            command.Parameters.AddRange(spParams);
                            command.ExecuteNonQuery();
                        }
                    }
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
            else
            {
                AuditWizardDataAccess lAuditWizardDataAccess = new AuditWizardDataAccess();
                lAuditWizardDataAccess.AlertDateUpdate(aAlert);
            }

            if (isDebugEnabled) logger.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " out");
            return 0;
        }



        /// <summary>
        /// Dismiss the specified Alert - this does not actually delete the alert - just marks it as dismissed
        /// </summary>
        /// <param name="licenseID"></param>
        /// <returns></returns>
        public int AlertSetStatus(Alert aAlert)
        {
            if (isDebugEnabled) logger.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " in");

            if (compactDatabaseType)
            {
                if (aAlert.AlertID != 0)
                {
                    try
                    {
                        using (SqlCeConnection conn = DatabaseConnection.CreateOpenCEConnection())
                        {
                            string commandText =
                                "UPDATE ALERTS SET _STATUS = @nStatus WHERE _ALERTID = @nAlertID";

                            SqlCeParameter[] spParams = new SqlCeParameter[2];
                            spParams[0] = new SqlCeParameter("@nAlertID", aAlert.AlertID);
                            spParams[0] = new SqlCeParameter("@nStatus", (int)aAlert.Status);

                            using (SqlCeCommand command = new SqlCeCommand(commandText, conn))
                            {
                                command.Parameters.AddRange(spParams);
                                command.ExecuteNonQuery();
                            }
                        }
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
            else
            {
                AuditWizardDataAccess lAuditWizardDataAccess = new AuditWizardDataAccess();
                lAuditWizardDataAccess.AlertSetStatus(aAlert);
            }

            if (isDebugEnabled) logger.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " out");
            return 0;
        }


        /// <summary>
        /// Delete the specified Alert from the database
        /// </summary>
        /// <param name="licenseID"></param>
        /// <returns></returns>
        public int AlertDelete(Alert aAlert)
        {
            if (isDebugEnabled) logger.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " in");

            if (compactDatabaseType)
            {
                if (aAlert.AlertID != 0)
                {                    
                    try
                    {
                        using (SqlCeConnection conn = DatabaseConnection.CreateOpenCEConnection())
                        {
                            string commandText =
                                "DELETE FROM ALERTS WHERE _ALERTID = @nAlertID";

                            SqlCeParameter[] spParams = new SqlCeParameter[1];
                            spParams[0] = new SqlCeParameter("@nAlertID", aAlert.AlertID);

                            using (SqlCeCommand command = new SqlCeCommand(commandText, conn))
                            {
                                command.Parameters.AddRange(spParams);
                                command.ExecuteNonQuery();
                            }
                        }
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
            else
            {
                AuditWizardDataAccess lAuditWizardDataAccess = new AuditWizardDataAccess();
                lAuditWizardDataAccess.AlertDelete(aAlert);
            }

            if (isDebugEnabled) logger.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " out");
            return 0;
        }


        /// <summary>
        /// Purge Alerts from the database
        /// </summary>
        /// <param name="dtBefore"></param>
        /// <returns></returns>
        public int AlertPurge(DateTime aDtBeforeDate)
        {
            if (isDebugEnabled) logger.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " in");

            if (compactDatabaseType)
            {                
                try
                {
                    using (SqlCeConnection conn = DatabaseConnection.CreateOpenCEConnection())
                    {
                        string commandText =
                            "DELETE FROM ALERTS " +
		                    "WHERE (datediff(day, _ALERTDATE, @dtPurgeDate) > 0)";

                        SqlCeParameter[] spParams = new SqlCeParameter[1];
                        spParams[0] = new SqlCeParameter("@dtPurgeDate", aDtBeforeDate);

                        using (SqlCeCommand command = new SqlCeCommand(commandText, conn))
                        {
                            command.Parameters.AddRange(spParams);
                            command.ExecuteNonQuery();
                        }
                    }
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
            else
            {
                AuditWizardDataAccess lAuditWizardDataAccess = new AuditWizardDataAccess();
                lAuditWizardDataAccess.AlertPurge(aDtBeforeDate);
            }

            if (isDebugEnabled) logger.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " out");
            return 0;
        }

        #endregion Alerts Table
    }
}
