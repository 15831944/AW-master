using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlServerCe;
using System.IO;
using System.Windows.Forms;

namespace Layton.AuditWizard.DataAccess
{
    public class AuditTrailDAO
    {
        #region Data

        protected static readonly log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private static readonly bool isDebugEnabled = logger.IsDebugEnabled;
        private bool compactDatabaseType = false;
        private Configuration config = ConfigurationManager.OpenExeConfiguration(Path.Combine(Application.StartupPath, "AuditWizardv8.exe"));

        #endregion

        public AuditTrailDAO()
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

        #region AuditTrail Table Functions

        /// <summary>
        /// Add a new Asset to the database
        /// </summary>
        /// <param name="theAsset"></param>
        /// <returns></returns>
        public int AuditTrailAdd(AuditTrailEntry ate)
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
                            "INSERT INTO AUDITTRAIL (_ASSETID ,_USERNAME ,_DATE ,_CLASS ,_TYPE ,_KEY ,_VALUE1 ,_VALUE2) " +
                            " VALUES (@nAssetID, @cUsername, @cSqlDate, @nClass, @nType, @cKey, @cOldValue, @cNewValue)";

                        SqlCeParameter[] spParams = new SqlCeParameter[8];
                        spParams[0] = new SqlCeParameter("@nAssetID", ate.AssetID);
                        spParams[1] = new SqlCeParameter("@cUsername", ate.Username);
                        spParams[2] = new SqlCeParameter("@cSqlDate", ate.Date);
                        spParams[3] = new SqlCeParameter("@nClass", ate.Class);
                        spParams[4] = new SqlCeParameter("@nType", ate.Type);
                        spParams[5] = new SqlCeParameter("@cKey", ate.Key);
                        spParams[6] = new SqlCeParameter("@cOldValue", ate.OldValue);
                        spParams[7] = new SqlCeParameter("@cNewValue", ate.NewValue);

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
                lItemID = lAuditWizardDataAccess.AuditTrailAdd(ate);
            }

            if (isDebugEnabled) logger.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " out with id : " + lItemID);
            return lItemID;
        }



        /// <summary>
        /// Delete the specified Audit TRail Entry from the database
        /// </summary>
        /// <param name="ate">The audit trail entry to remove</param>
        /// <returns></returns>
        public int AuditTrailDelete(AuditTrailEntry ate)
        {
            if (isDebugEnabled) logger.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " in");

            if (compactDatabaseType)
            {
                if (ate.AuditTrailID != 0)
                {
                    try
                    {
                        using (SqlCeConnection conn = DatabaseConnection.CreateOpenCEConnection())
                        {
                            string commandText = "DELETE FROM AUDITTRAIL WHERE _AUDITTRAILID = @nAuditTrailID";

                            using (SqlCeCommand command = new SqlCeCommand(commandText, conn))
                            {
                                command.Parameters.AddWithValue("@nAuditTrailID", ate.AuditTrailID);
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
                lAuditWizardDataAccess.AuditTrailDelete(ate);
            }

            if (isDebugEnabled) logger.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " out");
            return 0;
        }

        /// <summary>
        /// Return a list of all of the audit trail entries
        /// </summary>
        /// <returns></returns>
        public DataTable GetAuditTrailEntries(int requiredClass)
        {
            if (isDebugEnabled) logger.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " in");

            DataTable ateTable = new DataTable(TableNames.AUDITTRAIL);

            if (compactDatabaseType)
            {
                try
                {
                    using (SqlCeConnection conn = DatabaseConnection.CreateOpenCEConnection())
                    {
                        string commandText =
                            "SELECT AUDITTRAIL.* ,ASSETS._NAME AS ASSETNAME ,LOCATIONS._FULLNAME AS FULLLOCATIONNAME " +
                            "FROM AUDITTRAIL " +
                            "LEFT JOIN ASSETS ON (AUDITTRAIL._ASSETID = ASSETS._ASSETID) " +
                            "LEFT JOIN LOCATIONS ON (ASSETS._LOCATIONID = LOCATIONS._LOCATIONID) " +
                            "WHERE AUDITTRAIL._CLASS ";

                        if (requiredClass != -1)
                            commandText += " = @nClass ORDER BY _AUDITTRAILID";
                        else
                            commandText += " >= 100 ORDER BY _AUDITTRAILID";

                        using (SqlCeCommand command = new SqlCeCommand(commandText, conn))
                        {
                            if (requiredClass != -1)
                                command.Parameters.AddWithValue("@nClass", requiredClass);

                            new SqlCeDataAdapter(command).Fill(ateTable);
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
                ateTable = lAuditWizardDataAccess.GetAuditTrailEntries(requiredClass);
            }

            if (isDebugEnabled) logger.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " out");
            return ateTable;
        }

        public DataTable GetAuditTrailByDate(string aStartDate, string aEndDate)
        {
            DataTable auditTrailDataTable = GetAssetAuditHistoryForReport();

            DataTable resultsDataTable = new DataTable();
            DataColumn dataColumn;

            dataColumn = new DataColumn();
            dataColumn.DataType = Type.GetType("System.String");
            dataColumn.ColumnName = "Location";
            resultsDataTable.Columns.Add(dataColumn);

            dataColumn = new DataColumn();
            dataColumn.DataType = Type.GetType("System.String");
            dataColumn.ColumnName = "Asset Name";
            resultsDataTable.Columns.Add(dataColumn);

            dataColumn = new DataColumn();
            dataColumn.DataType = Type.GetType("System.String");
            dataColumn.ColumnName = "Date / Time";
            resultsDataTable.Columns.Add(dataColumn);

            dataColumn = new DataColumn();
            dataColumn.DataType = Type.GetType("System.String");
            dataColumn.ColumnName = "Category";
            resultsDataTable.Columns.Add(dataColumn);

            dataColumn = new DataColumn();
            dataColumn.DataType = Type.GetType("System.String");
            dataColumn.ColumnName = "Operation";
            resultsDataTable.Columns.Add(dataColumn);

            foreach (DataRow row in auditTrailDataTable.Rows)
            {
                AuditTrailEntry record = new AuditTrailEntry(row);

                if (CheckDate(record.Date, aStartDate, aEndDate))
                {
                    resultsDataTable.Rows.Add(
                        new object[]
					{
                        record.Location,
					    record.AssetName,
					    //record.Date.ToShortDateString() + " " + record.Date.ToShortTimeString(),
					    record.Date.ToString("yyyy-MM-dd HH:mm"),
					    record.ClassString,
					    record.GetTypeDescription() 
                    });
                }
            }

            return resultsDataTable;
        }

        private bool CheckDate(DateTime aRecordDate, string aStartDate, string aEndDate)
        {
            if (aStartDate == "" || aEndDate == "")
                return true;

            DateTime lStartDate = Convert.ToDateTime(aStartDate);
            DateTime lEndDate = Convert.ToDateTime(aEndDate);

            if (aRecordDate.Date >= lStartDate && aRecordDate.Date <= lEndDate)
                return true;
            else
                return false;
        }

        /// <summary>
        /// Return a list of all of the audit trail entries which are audit history records for
        /// the specified asset
        /// </summary>
        /// <returns></returns>
        public DataTable GetAssetAuditHistory(Asset aAsset, DateTime aStartDate, DateTime aEndDate)
        {
            if (isDebugEnabled) logger.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " in");

            DataTable ateTable = new DataTable(TableNames.AUDITTRAIL);

            if (compactDatabaseType)
            {
                try
                {
                    using (SqlCeConnection conn = DatabaseConnection.CreateOpenCEConnection())
                    {
                        string commandText =
                            "SELECT AUDITTRAIL.* ,ASSETS._NAME AS ASSETNAME ,LOCATIONS._FULLNAME AS FULLLOCATIONNAME " +
                            "FROM AUDITTRAIL " +
                            "LEFT JOIN ASSETS ON (AUDITTRAIL._ASSETID = ASSETS._ASSETID) " +
                            "LEFT JOIN LOCATIONS ON (ASSETS._LOCATIONID = LOCATIONS._LOCATIONID) " +
                            "WHERE _CLASS <= 3 ";

                        if (aAsset.AssetID == 0)
                            commandText += "AND AUDITTRAIL._ASSETID <> 0 ";
                        else
                            commandText += "AND AUDITTRAIL._ASSETID = @nAssetID ";

                        if (aStartDate != DateTime.MinValue && aEndDate != DateTime.MinValue)
                        {
                            commandText +=
                                "AND CONVERT(datetime ,AUDITTRAIL._DATE ,120) " +
                                "BETWEEN @cStartDate AND @cEndDate ";
                        }

                        commandText += "ORDER BY _AUDITTRAILID";

                        using (SqlCeCommand command = new SqlCeCommand(commandText, conn))
                        {
                            if (aAsset.AssetID != 0)
                                command.Parameters.AddWithValue("@nAssetID", aAsset.AssetID);

                            if (aStartDate != DateTime.MinValue && aEndDate != DateTime.MinValue)
                            {
                                command.Parameters.AddWithValue("@cStartDate", aStartDate);
                                command.Parameters.AddWithValue("@cEndDate", aEndDate);
                            }

                            new SqlCeDataAdapter(command).Fill(ateTable);
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
                ateTable = lAuditWizardDataAccess.GetAssetAuditHistory(aAsset, aStartDate, aEndDate);
            }

            if (isDebugEnabled) logger.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " out");
            return ateTable;
        }


        public DataTable GetAssetAuditHistoryForReport()
        {
            if (isDebugEnabled) logger.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " in");

            DataTable ateTable = new DataTable(TableNames.AUDITTRAIL);

            string commandText =
                            "SELECT AUDITTRAIL.* ,ASSETS._NAME AS ASSETNAME ,LOCATIONS._FULLNAME AS FULLLOCATIONNAME " +
                            "FROM AUDITTRAIL " +
                            "LEFT JOIN ASSETS ON (AUDITTRAIL._ASSETID = ASSETS._ASSETID) " +
                            "LEFT JOIN LOCATIONS ON (ASSETS._LOCATIONID = LOCATIONS._LOCATIONID) " +
                            "WHERE _CLASS <= 3 " +
                            "AND AUDITTRAIL._ASSETID <> 0 ";

            string lAssetIds = new AssetDAO().GetSelectedAssets();

            if (lAssetIds != "")
                commandText += "AND ASSETS._ASSETID IN  (" + lAssetIds + ") ";

            commandText += "ORDER BY _AUDITTRAILID";

            try
            {
                if (compactDatabaseType)
                {
                    using (SqlCeConnection conn = DatabaseConnection.CreateOpenCEConnection())
                    {
                        using (SqlCeCommand command = new SqlCeCommand(commandText, conn))
                        {
                            new SqlCeDataAdapter(command).Fill(ateTable);
                        }
                    }
                }
                else
                {
                    using (SqlConnection conn = DatabaseConnection.CreateOpenStandardConnection())
                    {
                        using (SqlCommand command = new SqlCommand(commandText, conn))
                        {
                            new SqlDataAdapter(command).Fill(ateTable);
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

            if (isDebugEnabled) logger.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " out");
            return ateTable;
        }

        /// <summary>
        /// Return a list of all of the audit trail entries which are audit history records for
        /// the specified asset
        /// </summary>
        /// <returns></returns>
        public DataTable GetAssetLastAuditDate(Asset aAsset, int aDays, bool aHasBeenAudited)
        {
            if (isDebugEnabled) logger.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " in");
            DataTable ateTable = new DataTable(TableNames.AUDITTRAIL);

            try
            {
                string commandText =
                            "SELECT a._ASSETID, a._LASTAUDIT AS _DATE, a._NAME AS ASSETNAME, l._FULLNAME AS FULLLOCATIONNAME " +
                            "FROM ASSETS a, LOCATIONS l " +
                            "WHERE a._LOCATIONID = l._LOCATIONID ";

                if (aHasBeenAudited)
                    commandText += "AND a._LASTAUDIT IS NOT NULL ";

                if (aAsset.AssetID != 0)
                    commandText += "AND a._ASSETID = @nAssetID ";
                else
                    commandText += "AND  a._ASSETID <> 0 ";

                if (aDays != 0)
                {
                    if (aHasBeenAudited)
                        commandText += "AND (datediff(day, a._LASTAUDIT ,GETDATE()) <= @nDays) ";
                    else
                        commandText += "AND (a._LASTAUDIT IS NULL OR datediff(day, a._LASTAUDIT ,GETDATE()) > @nDays) ";
                }

                commandText += "ORDER BY a._NAME";

                if (compactDatabaseType)
                {
                    using (SqlCeConnection conn = DatabaseConnection.CreateOpenCEConnection())
                    {
                        using (SqlCeCommand command = new SqlCeCommand(commandText, conn))
                        {
                            if (aAsset.AssetID != 0)
                                command.Parameters.AddWithValue("@nAssetID", aAsset.AssetID);

                            if (aDays != 0)
                                command.Parameters.AddWithValue("@nDays", aDays);

                            new SqlCeDataAdapter(command).Fill(ateTable);
                        }
                    }
                }
                else
                {
                    using (SqlConnection conn = DatabaseConnection.CreateOpenStandardConnection())
                    {
                        using (SqlCommand command = new SqlCommand(commandText, conn))
                        {
                            if (aAsset.AssetID != 0)
                                command.Parameters.AddWithValue("@nAssetID", aAsset.AssetID);

                            if (aDays != 0)
                                command.Parameters.AddWithValue("@nDays", aDays);

                            new SqlDataAdapter(command).Fill(ateTable);
                        }
                    }

                    //AuditWizardDataAccess lAuditWizardDataAccess = new AuditWizardDataAccess();
                    //ateTable = lAuditWizardDataAccess.GetAssetLastAuditDate(aAsset, aDays, aHasBeenAudited);
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

            if (isDebugEnabled) logger.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " out");
            return ateTable;
        }



        /// <summary>
        /// Purge the Audit Trail records from the database
        /// </summary>
        /// <returns></returns>
        public int AuditTrailPurge(DateTime dtPurge)
        {
            if (isDebugEnabled) logger.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " in");
            int purgeCount = 0;

            try
            {
                string commandText = "DELETE FROM AUDITTRAIL where (datediff(day, _DATE, @dtPurgeDate) > 0)";

                if (compactDatabaseType)
                {
                    using (SqlCeConnection conn = DatabaseConnection.CreateOpenCEConnection())
                    {
                        SqlCeParameter[] spParams = new SqlCeParameter[2];
                        spParams[0] = new SqlCeParameter("@dtPurgeDate", SqlDbType.DateTime);
                        spParams[0].Value = dtPurge;

                        using (SqlCeCommand command = new SqlCeCommand(commandText, conn))
                        {
                            command.Parameters.AddWithValue("@dtPurgeDate", dtPurge);
                            purgeCount = command.ExecuteNonQuery();
                        }
                    }
                }
                else
                {
                    using (SqlConnection conn = DatabaseConnection.CreateOpenStandardConnection())
                    {
                        SqlParameter[] spParams = new SqlParameter[2];
                        spParams[0] = new SqlParameter("@dtPurgeDate", SqlDbType.DateTime);
                        spParams[0].Value = dtPurge;

                        using (SqlCommand command = new SqlCommand(commandText, conn))
                        {
                            command.Parameters.AddWithValue("@dtPurgeDate", dtPurge);
                            purgeCount = command.ExecuteNonQuery();
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

            if (isDebugEnabled) logger.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " out");
            return purgeCount;
        }

        #endregion AuditTrail functions
    }
}
