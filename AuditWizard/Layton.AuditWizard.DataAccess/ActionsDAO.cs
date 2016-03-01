using System;
using System.Configuration;
using System.Data;
using System.Data.SqlServerCe;
using System.IO;
using System.Windows.Forms;

namespace Layton.AuditWizard.DataAccess
{
    public class ActionsDAO
    {
        #region Data

        private string connectionStringCompact = null;
        protected static readonly log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private static readonly bool isDebugEnabled = logger.IsDebugEnabled;
        private bool compactDatabaseType = false;
        private Configuration config = ConfigurationManager.OpenExeConfiguration(Path.Combine(Application.StartupPath, "AuditWizardv8.exe"));  

        #endregion

        public ActionsDAO()
        {
            compactDatabaseType = Convert.ToBoolean(config.AppSettings.Settings["CompactDatabaseType"].Value);
            connectionStringCompact = "Data Source=" + Application.StartupPath + Convert.ToString(config.AppSettings.Settings["ConnectionStringCompact"].Value);
        }

        #region Actions Table

        /// <summary>
        /// Return a table containing all of the Actions that have been defined
        /// </summary>
        /// <returns></returns>
        public DataTable EnumerateActions()
        {
            if (isDebugEnabled) logger.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " in");

            DataTable actionTable = new DataTable(TableNames.ACTIONS);

            if (compactDatabaseType)
            {
                try
                {
                    using (SqlCeConnection conn = DatabaseConnection.CreateOpenCEConnection())
                    {
                        string commandText =
                            "SELECT ACTIONS.[_ACTIONID] " +
		                    ", ACTIONS.[_TYPE] " +
	                        ", ACTIONS.[_APPLICATIONID] " + 
	                        ", ACTIONS.[_ASSETS] " + 
	                        ", ACTIONS.[_STATUS] " + 
	                        ", ACTIONS.[_NOTES] " +
	                        ", APPLICATIONS.[_NAME] " +
                            "FROM dbo.ACTIONS " +
                            "LEFT JOIN dbo.APPLICATIONS ON (ACTIONS._APPLICATIONID = dbo.APPLICATIONS.[_APPLICATIONID]) " +
                            "ORDER BY _ACTIONID";

                        using (SqlCeCommand command = new SqlCeCommand(commandText, conn))
                        {
                            new SqlCeDataAdapter(command).Fill(actionTable);
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
                actionTable = lAuditWizardDataAccess.EnumerateActions();
            }

            if (isDebugEnabled) logger.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " out");
            return actionTable;
        }


        /// <summary>
        /// Add a new Action to the database
        /// </summary>
        /// <param name="theAlert"></param>
        /// <returns></returns>
        public int ActionAdd(Action aAction)
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
                            "INSERT INTO ACTIONS (_APPLICATIONID ,_TYPE, _STATUS ,_ASSETS, _NOTES) " +
                            "VALUES (@nApplicationID ,@nType, @nStatus, @cAssets, @cNotes)";

                        SqlCeParameter[] spParams = new SqlCeParameter[5];
                        spParams[0] = new SqlCeParameter("@nType", (int)aAction.ActionType);
                        spParams[1] = new SqlCeParameter("@nApplicationID", (int)aAction.ApplicationID);
                        spParams[2] = new SqlCeParameter("@nStatus", (int)aAction.Status);
                        spParams[3] = new SqlCeParameter("@cAssets", aAction.AssociatedAssets);
                        spParams[4] = new SqlCeParameter("@cNotes", aAction.Notes);

                        using (SqlCeCommand command = new SqlCeCommand(commandText, conn))
                        {
                            command.Parameters.AddRange(spParams);
                            command.ExecuteNonQuery();
                        }

                        // then get id of newly inserted record
                        using (SqlCeCommand command = new SqlCeCommand("SELECT @@IDENTITY", conn))
                        {
                            lItemID = Convert.ToInt32(command.ExecuteScalar());
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
                lItemID = lAuditWizardDataAccess.ActionAdd(aAction);
            }

            if (isDebugEnabled) logger.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " out");
            return lItemID;
        }


        /// <summary>
        /// Update the specified action
        /// </summary>
        /// <param name="theAsset"></param>
        /// <returns></returns>
        public int ActionUpdate(Action aAction)
        {
            if (isDebugEnabled) logger.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " in");

            if (compactDatabaseType)
            {
                try
                {
                    using (SqlCeConnection conn = DatabaseConnection.CreateOpenCEConnection())
                    {
                        string commandText =
                            "UPDATE ACTIONS SET _STATUS = @nStatus " + 
			                ", _ASSETS = @cAssets " +
                            ", _NOTES = @cNotes " +
			                "WHERE _ACTIONID = @nActionID";

                        SqlCeParameter[] spParams = new SqlCeParameter[4];
                        spParams[0] = new SqlCeParameter("@nActionID", aAction.ActionID);
                        spParams[1] = new SqlCeParameter("@nStatus", (int)aAction.Status);
                        spParams[2] = new SqlCeParameter("@cAssets", aAction.AssociatedAssets);
                        spParams[3] = new SqlCeParameter("@cNotes", aAction.Notes);

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
            }

            if (isDebugEnabled) logger.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " out");
            return 0;
        }



        /// <summary>
        /// Delete the specified Action from the database
        /// </summary>
        /// <param name="licenseID"></param>
        /// <returns></returns>
        public int ActionDelete(Action aAction)
        {
            if (isDebugEnabled) logger.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " in");

            if (compactDatabaseType)
            {
                if (aAction.ActionID != 0)
                {
                    try
                    {
                        using (SqlCeConnection conn = DatabaseConnection.CreateOpenCEConnection())
                        {
                            string commandText =
                            "DELETE FROM ACTIONS WHERE _ACTIONID = @nActionID";

                            SqlCeParameter[] spParams = new SqlCeParameter[1];
                            spParams[0] = new SqlCeParameter("@nActionID", aAction.ActionID);

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
            }

            if (isDebugEnabled) logger.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " out");
            return 0;
        }


        #endregion Action Table
    }
}
