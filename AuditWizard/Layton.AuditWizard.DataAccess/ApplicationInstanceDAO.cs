using System;
using System.Configuration;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Data.SqlServerCe;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;

namespace Layton.AuditWizard.DataAccess
{
    public class ApplicationInstanceDAO
    {
        #region Data

        protected static readonly log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private static readonly bool isDebugEnabled = logger.IsDebugEnabled;
        private bool compactDatabaseType = false;
        private Configuration config = ConfigurationManager.OpenExeConfiguration(Path.Combine(Application.StartupPath, "AuditWizardv8.exe"));

        #endregion

        public ApplicationInstanceDAO()
        {
            compactDatabaseType = Convert.ToBoolean(config.AppSettings.Settings["CompactDatabaseType"].Value);
        }

        #region Application_instance Table

        /// <summary>
        /// Return a table of the intances of the specified application
        /// </summary>
        /// <returns></returns>
        public DataTable GetApplicationInstances(string aPublisher)
        {
            string commandText =
                "SELECT APPLICATION_INSTANCES._INSTANCEID ,APPLICATION_INSTANCES._APPLICATIONID ,APPLICATION_INSTANCES._ASSETID ,APPLICATION_INSTANCES._PRODUCTID ,APPLICATION_INSTANCES._CDKEY " +
                ",APPLICATIONS._NAME ,APPLICATIONS._VERSION ,APPLICATIONS._PUBLISHER ,APPLICATIONS._GUID ,APPLICATIONS._IGNORED ,APPLICATIONS._ALIASED_TOID ,APPLICATIONS._USER_DEFINED " +
                ",ASSETS._NAME AS ASSETNAME " +
                ",ASSET_TYPES._ICON AS ASSETICON " +
                ",LOCATIONS._FULLNAME AS FULLLOCATIONNAME " +
                ",LOCATIONS._NAME AS LOCATIONNAME " +
                ",DOMAINS._NAME AS DOMAINNAME " +
                "FROM APPLICATION_INSTANCES " +
                "LEFT JOIN APPLICATIONS ON (APPLICATION_INSTANCES._APPLICATIONID = APPLICATIONS._APPLICATIONID) " +
                "LEFT JOIN ASSETS ON (APPLICATION_INSTANCES._ASSETID = ASSETS._ASSETID) " +
                "LEFT JOIN ASSET_TYPES ON (ASSETS._ASSETTYPEID = ASSET_TYPES._ASSETTYPEID) " +
                "LEFT JOIN LOCATIONS ON (ASSETS._LOCATIONID = LOCATIONS._LOCATIONID) " +
                "LEFT JOIN DOMAINS ON (ASSETS._DOMAINID = DOMAINS._DOMAINID) " +
                "WHERE ASSETS._STOCK_STATUS <> 3";

            if (!string.IsNullOrEmpty(aPublisher))
                commandText += " AND _PUBLISHER = '" + aPublisher + "'";

			return PerformQuery(commandText);
        }

        public DataTable GetApplicationInstances()
        {
            return GetApplicationInstances("");
        }

        /// <summary>
        /// Return a table of the intances of the specified application
        /// </summary>
        /// <param name="applicationID"></param>
        /// <returns></returns>
        public DataTable GetApplicationInstances(int applicationID)
        {
            if (isDebugEnabled) logger.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " in");

            DataTable applicationsTable = new DataTable(TableNames.APPLICATION_INSTANCES);

            if (compactDatabaseType)
            {
                try
                {
                    using (SqlCeCommand command = new SqlCeCommand("SELECT APPLICATION_INSTANCES._INSTANCEID ,APPLICATION_INSTANCES._APPLICATIONID ,APPLICATION_INSTANCES._ASSETID ,APPLICATION_INSTANCES._PRODUCTID ,APPLICATION_INSTANCES._CDKEY " +
                        ",APPLICATIONS._NAME ,APPLICATIONS._VERSION ,APPLICATIONS._PUBLISHER ,APPLICATIONS._GUID ,APPLICATIONS._IGNORED ,APPLICATIONS._ALIASED_TOID ,APPLICATIONS._USER_DEFINED " +
                        ",ASSETS._NAME AS ASSETNAME " +
                        ",ASSET_TYPES._ICON AS ASSETICON " +
                        ",LOCATIONS._FULLNAME AS FULLLOCATIONNAME " +
                        ",LOCATIONS._NAME AS LOCATIONNAME " +
                        ",DOMAINS._NAME AS DOMAINNAME " +
                        "FROM APPLICATION_INSTANCES " +
                        "LEFT JOIN APPLICATIONS ON (APPLICATION_INSTANCES._APPLICATIONID = APPLICATIONS._APPLICATIONID) " +
                        "LEFT JOIN ASSETS ON (APPLICATION_INSTANCES._ASSETID = ASSETS._ASSETID) " +
                        "LEFT JOIN ASSET_TYPES ON (ASSETS._ASSETTYPEID = ASSET_TYPES._ASSETTYPEID) " +
                        "LEFT JOIN LOCATIONS ON (ASSETS._LOCATIONID = LOCATIONS._LOCATIONID) " +
                        "LEFT JOIN DOMAINS ON (ASSETS._DOMAINID = DOMAINS._DOMAINID) " +
                        "WHERE APPLICATION_INSTANCES._APPLICATIONID = @applicationID " +
                        "AND ASSETS._STOCK_STATUS <> 3", DatabaseConnection.OpenCeConnection()))
                    {
                        command.Parameters.AddWithValue("@applicationID", applicationID);
                        new SqlCeDataAdapter(command).Fill(applicationsTable);
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
                applicationsTable = lAuditWizardDataAccess.GetApplicationInstances(applicationID);
            }

            if (isDebugEnabled) logger.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " out");
            return applicationsTable;
        }


        /// <summary>
        /// Delete all application instance records for the specified asset
        /// </summary>
        /// <param name="assetID"></param>
        /// <returns></returns>
        public int ApplicationInstanceDelete(int assetID)
        {
            if (isDebugEnabled) logger.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " in");

            string commandText =
                            "select ai._instanceid from application_instances ai " +
                            "left join applications a ON (a._applicationid = ai._applicationid ) " +
                            "where ai._assetid = " + assetID + " " +
                            "and a._assigned_fileid = 0";
            try
            {
                if (compactDatabaseType)
                {
                    using (SqlCeConnection conn = DatabaseConnection.CreateOpenCEConnection())
                    {
                        using (SqlCeCommand command = new SqlCeCommand(commandText, conn))
                        {
                            using (SqlCeDataReader reader = command.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    ApplicationInstanceDeleteByInstanceId(reader.GetInt32(0));
                                }
                            }
                        }
                    }
                }
                else
                {
                    using (SqlConnection conn = DatabaseConnection.CreateOpenStandardConnection())
                    {
                        using (SqlCommand command = new SqlCommand(commandText, conn))
                        {
                            using (SqlDataReader reader = command.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    ApplicationInstanceDeleteByInstanceId(reader.GetInt32(0));
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


            if (isDebugEnabled) logger.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " out");
            return 0;
        }

        public void ApplicationInstanceDeleteByInstanceId(int aInstanceId)
        {
            if (isDebugEnabled) logger.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " in");

            try
            {
                string commandText = "DELETE FROM APPLICATION_INSTANCES WHERE _INSTANCEID = " + aInstanceId;

                if (compactDatabaseType)
                {
                    using (SqlCeConnection conn = DatabaseConnection.CreateOpenCEConnection())
                    {
                        using (SqlCeCommand command = new SqlCeCommand(commandText, conn))
                        {
                            command.ExecuteNonQuery();
                        }
                    }
                }
                else
                {
                    using (SqlConnection conn = DatabaseConnection.CreateOpenStandardConnection())
                    {
                        using (SqlCommand command = new SqlCommand(commandText, conn))
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


            if (isDebugEnabled) logger.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " out");

        }

        /// <summary>
        /// Update an application instance record
        /// </summary>
        /// <param name="applicationInstance"></param>
        /// <returns></returns>
        public int ApplicationInstanceUpdate(ApplicationInstance applicationInstance)
        {
            if (isDebugEnabled) logger.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " in");

            if (compactDatabaseType)
            {
                try
                {
                    using (SqlCeConnection conn = DatabaseConnection.CreateOpenCEConnection())
                    {
                        string commandText =
                            "UPDATE APPLICATION_INSTANCES SET " +
                            "_PRODUCTID = @cProductID " +
                            ",_CDKEY = @cCDKey " +
                            "WHERE _INSTANCEID = @nInstanceID";

                        using (SqlCeCommand command = new SqlCeCommand(commandText, conn))
                        {
                            command.Parameters.AddWithValue("@cProductID", applicationInstance.Serial.ProductId);
                            command.Parameters.AddWithValue("@cCDKey", applicationInstance.Serial.CdKey);
                            command.Parameters.AddWithValue("nInstanceID", applicationInstance.InstanceID);

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
                lAuditWizardDataAccess.ApplicationInstanceUpdate(applicationInstance);
            }

            if (isDebugEnabled) logger.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " out");
            return 0;
        }




        /// <summary>
        /// Return a table of any suppoprt contracts for which alerts should be generated
        /// </summary>
        /// <param name="applicationID"></param>
        /// <returns></returns>
        public DataTable EnumerateSupportContractAlerts()
        {
            if (isDebugEnabled) logger.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " in");

            DataTable alertsTable = new DataTable(TableNames.SUPPORT_STATISTICS);

            if (compactDatabaseType)
            {
                try
                {
                    using (SqlCeConnection conn = DatabaseConnection.CreateOpenCEConnection())
                    {
                        string commandText =
                            "SELECT LICENSES._SUPPORT_EXPIRES, LICENSES._SUPPORT_ALERTDAYS, LICENSES._SUPPORT_ALERTBYEMAIL, LICENSES._SUPPORT_ALERTRECIPIENTS " +
                            ",APPLICATIONS._NAME " +
                            "FROM LICENSES " +
                            "LEFT JOIN APPLICATIONS ON (LICENSES._APPLICATIONID = APPLICATIONS._APPLICATIONID) " +
                            "WHERE _SUPPORTED = 1 " +
                            "AND _SUPPORT_ALERTDAYS <> -1 " +
                            "AND ((_SUPPORT_EXPIRES - _SUPPORT_ALERTDAYS) <= GETDATE())";

                        using (SqlCeCommand command = new SqlCeCommand(commandText, conn))
                        {
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
                alertsTable = lAuditWizardDataAccess.EnumerateSupportContractAlerts();
            }

            if (isDebugEnabled) logger.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " out");
            return alertsTable;
        }


        /// <summary>
        /// Return a table of any suppoprt contracts for which alerts should be generated
        /// </summary>
        /// <param name="applicationID"></param>
        /// <returns></returns>
        public DataTable EnumerateSupportContractAssetAlerts()
        {
            if (isDebugEnabled) logger.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " in");

            DataTable alertsTable = new DataTable(TableNames.SUPPORT_STATISTICS);

            if (compactDatabaseType)
            {
                try
                {
                    using (SqlCeConnection conn = DatabaseConnection.CreateOpenCEConnection())
                    {
                        string commandText =
                            "SELECT     ASSET_SUPPORTCONTRACT._SUPPORT_EXPIRY, ASSET_SUPPORTCONTRACT._NOOFDAYS, ASSET_SUPPORTCONTRACT._ALERTBYEMAIL,ASSETS._NAME " +
                            "FROM       ASSET_SUPPORTCONTRACT INNER JOIN ASSETS ON ASSET_SUPPORTCONTRACT._ASSETID = ASSETS._ASSETID " +
                            "WHERE     (_ALERTFLAG = 1) AND (ASSET_SUPPORTCONTRACT._NOOFDAYS <> - 1) AND (ASSET_SUPPORTCONTRACT._SUPPORT_EXPIRY - ASSET_SUPPORTCONTRACT._NOOFDAYS <= GETDATE())";
                        using (SqlCeCommand command = new SqlCeCommand(commandText, conn))
                        {
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
                alertsTable = lAuditWizardDataAccess.EnumerateSupportContractAssetAlerts();
            }

            if (isDebugEnabled) logger.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " out");
            return alertsTable;
        }

        public DataTable GetAllInstances(bool showIncluded, bool showIgnored)
        {
            string commandText =
                "select _assetid, a._publisher, a._name, a._version " +
                "from applications a " +
                "left outer join application_instances ai on (ai._applicationid = a._applicationid)";

            if (showIncluded && !showIgnored)
                commandText += " where a._ignored = 0";

            else if (!showIncluded && showIgnored)
                commandText += " where a.ignored = 1";

            // if both true then just leave SQL as-is
            // both false is not allowed

            return PerformQuery(commandText);
        }

        public DataTable GetOSPickerValues(string columnName)
        {
            string commandText = String.Format(
                "select ai.{0} " +
                "from applications a " +
                "left outer join application_instances ai on (ai._applicationid = a._applicationid) " +
                "where a._isos = 1 " +
                "order by ai.{0}", columnName);

            return PerformQuery(commandText);
        }

        public string GetCustomOSValues(string aColumnName, int aAssetId)
        {
            string commandText = String.Format(
                "SELECT ai.{0} " +
                "FROM APPLICATIONS a " +
                "LEFT JOIN APPLICATION_INSTANCES ai ON (ai._applicationid = a._applicationid) " +
                "WHERE ai._assetid = {1} " +
                "AND a._isos = 1", aColumnName, aAssetId);

            return PerformScalarQuery(commandText);
        }

        public DataTable GetCompliantOSAssetIds(string columnName, string criteriaValue)
        {
            string commandText = String.Format(
                "select ai._assetid " +
                "from applications a " +
                "left outer join application_instances ai on (ai._applicationid = a._applicationid) " +
                "where a._isos = 1 " +
                "and a.{0} = {1}", columnName, criteriaValue);

            return PerformQuery(commandText);
        }

        public string GetCompliantAssetValueByAssetId(string complianceField, int assetID)
        {
            string commandText = String.Format(
                "select {0} " +
                "from applications a " +
                "left outer join application_instances ai on (ai._applicationid = a._applicationid) " +
                "where a._isos = 1 " +
                "and ai._assetid = {1}", complianceField, assetID);

            return PerformScalarQuery(commandText);
        }

        public void UpdateApplicationInstanceByApplicationId(int aNewApplicationId, int aBaseApplicationId, int aOriginalApplicationId)
        {
            string commandText = String.Format(
                "UPDATE APPLICATION_INSTANCES " +
                "SET _APPLICATIONID = {0}, _BASE_APPLICATIONID = {1} " +
                "WHERE _APPLICATIONID = {2}", aNewApplicationId, aBaseApplicationId, aOriginalApplicationId);

            ExecuteNonQuery(commandText);
        }

        public int GetApplicationInstanceCountByAssetName(string aPublisher, string aApplicationName, int aAssetId)
        {
            string commandText = String.Format(
                "SELECT COUNT(*) " +
                "FROM APPLICATIONS ap " +
                "LEFT JOIN APPLICATION_INSTANCES ai ON (ai._applicationid = ap._applicationid) " +
                "WHERE ai._assetid = '{0}' " +
                "AND ap._publisher = '{1}' " +
                "AND ap._name = '{2}'", aAssetId, aPublisher, aApplicationName);

            int returnValue = 0;

            try
            {
                if (compactDatabaseType)
                {
                    using (SqlCeConnection conn = DatabaseConnection.CreateOpenCEConnection())
                    {
                        using (SqlCeCommand command = new SqlCeCommand(commandText, conn))
                        {
                            returnValue = (int)command.ExecuteScalar();
                        }
                    }
                }
                else
                {
                    using (SqlConnection conn = DatabaseConnection.CreateOpenStandardConnection())
                    {
                        using (SqlCommand command = new SqlCommand(commandText, conn))
                        {
                            returnValue = (int)command.ExecuteScalar();
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

        private string PerformScalarQuery(string commandText)
        {
            string returnValue = String.Empty;

            try
            {
                if (compactDatabaseType)
                {
                    using (SqlCeConnection conn = DatabaseConnection.CreateOpenCEConnection())
                    {
                        using (SqlCeCommand command = new SqlCeCommand(commandText, conn))
                        {
                            returnValue = command.ExecuteScalar().ToString();
                        }
                    }
                }
                else
                {
                    using (SqlConnection conn = DatabaseConnection.CreateOpenStandardConnection())
                    {
                        using (SqlCommand command = new SqlCommand(commandText, conn))
                        {
                            returnValue = command.ExecuteScalar().ToString();
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
        /// Return drilldown statistics for Asset Table
        /// </summary>
        /// <returns></returns>
        private DataTable PerformQuery(string aCommandText)
        {
            DataTable statisticsTable = new DataTable();
            try
            {
                if (compactDatabaseType)
                {
                    using (SqlCeConnection conn = DatabaseConnection.CreateOpenCEConnection())
                    {
                        using (SqlCeCommand command = new SqlCeCommand(aCommandText, conn))
                        {
                            new SqlCeDataAdapter(command).Fill(statisticsTable);
                        }
                    }
                }
                else
                {
                    using (SqlConnection conn = DatabaseConnection.CreateOpenStandardConnection())
                    {
                        using (SqlCommand command = new SqlCommand(aCommandText, conn))
                        {
                            new SqlDataAdapter(command).Fill(statisticsTable);
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

            return statisticsTable;
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

        public DataTable GetInstalledOSes()
        {
            string commandText =
                "SELECT DISTINCT ASSETS._NAME AS \"Asset Name\", ASSETS._IPADDRESS AS \"IP Address\", " +
                "ASSETS._MAKE AS \"Make\", ASSETS._MODEL AS \"Model\", APPLICATIONS._PUBLISHER AS \"Publisher\", " +
                "APPLICATIONS._NAME AS \"OS Family\", APPLICATIONS._VERSION AS \"OS Version\" " +
                "FROM ASSETS INNER JOIN APPLICATION_INSTANCES ON " +
                "ASSETS._ASSETID = APPLICATION_INSTANCES._ASSETID " +
                "INNER JOIN APPLICATIONS ON " +
                "APPLICATION_INSTANCES._APPLICATIONID = APPLICATIONS._APPLICATIONID " +
                "INNER JOIN AUDITEDITEMS ON " +
                "AUDITEDITEMS._ASSETID = ASSETS._ASSETID " +
                "WHERE APPLICATIONS._ISOS = 1 " +
                "AND ASSETS._ASSETID IN (" + new AssetDAO().GetSelectedAssets() + ") " +                
                "ORDER BY ASSETS._NAME";

            return PerformQuery(commandText);
        }

        #endregion Application_instance Table
    }
}
