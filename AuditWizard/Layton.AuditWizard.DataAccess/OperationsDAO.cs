using System;
using System.Configuration;
using System.Data;
using System.Data.SqlServerCe;
using System.IO;
using System.Windows.Forms;

namespace Layton.AuditWizard.DataAccess
{
    public class OperationsDAO
    {
        #region Data

        private string connectionStringCompact = null;
        protected static readonly log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private static readonly bool isDebugEnabled = logger.IsDebugEnabled;
        private bool compactDatabaseType = false;
        private Configuration config = ConfigurationManager.OpenExeConfiguration(Path.Combine(Application.StartupPath, "AuditWizardv8.exe"));

        #endregion

        public OperationsDAO()
        {
            compactDatabaseType = Convert.ToBoolean(config.AppSettings.Settings["CompactDatabaseType"].Value);
            connectionStringCompact = "Data Source=" + Application.StartupPath + Convert.ToString(config.AppSettings.Settings["ConnectionStringCompact"].Value);
        }

        #region Operations Table

        /// <summary>
        /// Return a table containing all of the Operations defined
        /// </summary>
        /// <returns></returns>
        public DataTable EnumerateOperations(Operation.OPERATION aOperationType, Operation.STATUS aStatus)
        {
            if (isDebugEnabled) logger.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " in");

            DataTable table = new DataTable(TableNames.OPERATIONS);

            if (compactDatabaseType)
            {
                try
                {
                    using (SqlCeConnection conn = DatabaseConnection.CreateOpenCEConnection())
                    {
                        string commandText =
                            "SELECT OPERATIONS.* " +
		                    ",ASSETS._NAME AS ASSETNAME " +
                            "FROM OPERATIONS " +
		                    "LEFT JOIN ASSETS ON (ASSETS._ASSETID = OPERATIONS._ASSETID) ";

                        if ((int)aOperationType != -1)
                            commandText += "WHERE OPERATIONS._OPERATION = @nOperation ";

                        if ((int)aStatus != -1)
                        {
                            if ((int)aOperationType == -1)
                                commandText += "WHERE OPERATIONS._STATUS = @nStatus ";
                            else
                                commandText += "AND OPERATIONS._STATUS = @nStatus ";
                        }

                        commandText += "ORDER BY _OPERATIONID";

                        using (SqlCeCommand command = new SqlCeCommand(commandText, conn))
                        {
                            if ((int)aOperationType != -1)
                                command.Parameters.AddWithValue("@nOperation", (int)aOperationType);

                            if ((int)aStatus != -1)
                                command.Parameters.AddWithValue("@nStatus", (int)aStatus);

                            new SqlCeDataAdapter(command).Fill(table);
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
                table = lAuditWizardDataAccess.EnumerateOperations(aOperationType, aStatus);
            }

            if (isDebugEnabled) logger.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " out");
            return table;
        }


        /// <summary>
        /// Add a new Operation to the database
        /// </summary>
        /// <param name="operation">The Operation to add</param>
        /// <returns></returns>
        public int OperationAdd(Operation aOperation)
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
                               "INSERT INTO OPERATIONS " +
		                       "(_OPERATION ,_ASSETID ,_START_DATE ,_STATUS) " +
	                           "VALUES " +
		                       "(@nOperation, @nAssetID, @dtToday, 0)";

                        SqlCeParameter[] spParams = new SqlCeParameter[3];
                        spParams[0] = new SqlCeParameter("@nOperation", (int)aOperation.OperationType);
                        spParams[1] = new SqlCeParameter("@nAssetID", aOperation.AssetID);
                        spParams[2] = new SqlCeParameter("@dtToday", aOperation.StartDate);

                        using (SqlCeCommand command = new SqlCeCommand(commandText, conn))
                        {
                            command.Parameters.AddRange(spParams);
                            command.ExecuteNonQuery();
                        }

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
                lItemID = lAuditWizardDataAccess.OperationAdd(aOperation);
            }

            if (isDebugEnabled) logger.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " out");
            return lItemID;
        }


        /// <summary>
        /// Update the specified Operation
        /// </summary>
        /// <param name="theAsset"></param>
        /// <returns></returns>
        public int OperationUpdate(Operation aOperation)
        {
            if (isDebugEnabled) logger.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " in");

            if (compactDatabaseType)
            {
                try
                {
                    using (SqlCeConnection conn = DatabaseConnection.CreateOpenCEConnection())
                    {
                        string commandText =
                               "UPDATE OPERATIONS SET " +
		                       "_END_DATE = @dtEndDate, " +
		                       "_STATUS = @nStatus, " +
                               "_ERRORTEXT = @cErrorText " +
	                           "WHERE _OPERATIONID = @nOperationID";

                        SqlCeParameter[] spParams = new SqlCeParameter[4];
                        spParams[0] = new SqlCeParameter("@nOperationID", aOperation.OperationID);

                        if (aOperation.EndDate.Ticks == 0)
                            spParams[1] = new SqlCeParameter("@dtEndDate", DBNull.Value);
                        else
                            spParams[1] = new SqlCeParameter("@dtEndDate", aOperation.EndDate);

                        spParams[2] = new SqlCeParameter("@nStatus", (int)aOperation.Status);
                        spParams[3] = new SqlCeParameter("@cErrorText", aOperation.ErrorText);

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
                lAuditWizardDataAccess.OperationUpdate(aOperation);
            }

            if (isDebugEnabled) logger.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " out");
            return 0;
        }


        /// <summary>
        /// Delete the specified Operation from the database
        /// </summary>
        /// <returns></returns>
        public int OperationDelete(Operation aOperation)
        {
            if (isDebugEnabled) logger.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " in");

            if (compactDatabaseType)
            {
                try
                {
                    using (SqlCeConnection conn = DatabaseConnection.CreateOpenCEConnection())
                    {
                        string commandText =
                               "DELETE FROM OPERATIONS WHERE _OPERATIONID = @nOperationID";

                        SqlCeParameter[] spParams = new SqlCeParameter[1];
                        spParams[0] = new SqlCeParameter("@nOperationID", aOperation.OperationID);

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
                lAuditWizardDataAccess.OperationDelete(aOperation);
            }

            if (isDebugEnabled) logger.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " out");
            return 0;
        }



        /// <summary>
        /// Get the database index of the last operation in the database
        /// </summary>
        /// <returns></returns>
        public int OperationGetLastIndex()
        {
            if (isDebugEnabled) logger.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " in");
            
            int lReturnID = 0;

            if (compactDatabaseType)
            {
                try
                {
                    using (SqlCeConnection conn = DatabaseConnection.CreateOpenCEConnection())
                    {
                        string commandText =
                               "SELECT max(_OPERATIONID) FROM OPERATIONS";

                        using (SqlCeCommand command = new SqlCeCommand(commandText, conn))
                        {
                            object result = command.ExecuteScalar();

                            if ((result != null) && (result.GetType() != typeof(DBNull)))
                                lReturnID = (int)result;
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
                lReturnID = lAuditWizardDataAccess.OperationGetLastIndex();
            }

            if (isDebugEnabled) logger.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " out with id : " + lReturnID);
            return lReturnID;
        }

        #endregion Operations Table
    }
}
