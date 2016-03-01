using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlServerCe;
using System.IO;
using System.Windows.Forms;

namespace Layton.AuditWizard.DataAccess
{
    public class AuditedItemsDAO
    {
        #region Data

        protected static readonly log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private static readonly bool isDebugEnabled = logger.IsDebugEnabled;
        private bool compactDatabaseType = false;
        private Configuration config = ConfigurationManager.OpenExeConfiguration(Path.Combine(Application.StartupPath, "AuditWizardv8.exe"));

        #endregion

        public AuditedItemsDAO()
        {
            compactDatabaseType = Convert.ToBoolean(config.AppSettings.Settings["CompactDatabaseType"].Value);
        }

        #region AuditedItems Table

        /// <summary>
        /// Return a table containing all of the audited items which have been declared for the 
        /// specified asset (with the specified parent)
        /// </summary>
        /// <param name="applicationID"></param>
        /// <returns></returns>
        public DataTable GetAuditedItems(int assetID, string parentCategory, bool all)
        {
            if (isDebugEnabled) logger.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " in");

            DataTable dataTable = new DataTable(TableNames.AUDITEDITEMS);

            if (compactDatabaseType)
            {
                try
                {
                    using (SqlCeConnection conn = DatabaseConnection.CreateOpenCEConnection())
                    {
                        string lWhereClause = String.Empty;
                        string commandText = "SELECT AUDITEDITEMS.* FROM AUDITEDITEMS ";

                        if (assetID != 0)
                        {
                            lWhereClause = "WHERE _ASSETID = '" + assetID + "' ";
                        }

                        if (parentCategory != "")
                        {
                            if (lWhereClause != String.Empty)
                                lWhereClause = "WHERE _CATEGORY LIKE @parentCategory " + "'%'";
                            else
                                lWhereClause += " AND _CATEGORY LIKE @parentCategory " + "'%'";

                            if (!all)
                                lWhereClause += " AND CHARINDEX(''|'', _CATEGORY, len('" + @parentCategory + "'|'') + 1) = 0";
                        }

                        commandText = commandText + lWhereClause;

                        using (SqlCeCommand command = new SqlCeCommand(commandText, conn))
                        {
                            command.Parameters.AddWithValue("@parentCategory", parentCategory);
                            new SqlCeDataAdapter(command).Fill(dataTable);
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
                dataTable = lAuditWizardDataAccess.GetAuditedItems(assetID, parentCategory, all);
            }

            if (isDebugEnabled) logger.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " out");
            return dataTable;
        }

        /// <summary>
        /// Return a table containing all of the audited items which have been declared for the 
        /// specified asset (with the specified parent)
        /// </summary>
        /// <param name="applicationID"></param>
        /// <returns></returns>
        public DataTable GetIconMappings()
        {
            if (isDebugEnabled) logger.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " in");

            DataTable dataTable = new DataTable(TableNames.AUDITEDITEMS);

            if (compactDatabaseType)
            {
                try
                {
                    using (SqlCeConnection conn = DatabaseConnection.CreateOpenCEConnection())
                    {
                        string commandText =
                            "SELECT _CATEGORY, _ICON FROM AUDITEDITEMS WHERE _ASSETID = 0";

                        using (SqlCeCommand command = new SqlCeCommand(commandText, conn))
                        {
                            new SqlCeDataAdapter(command).Fill(dataTable);
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
                dataTable = lAuditWizardDataAccess.GetIconMappings();
            }

            if (isDebugEnabled) logger.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " out");
            return dataTable;
        }


        /// <summary>
        /// Delete all audited item records for the specified asset
        /// </summary>
        /// <param name="assetID"></param>
        /// <returns></returns>
        public int AuditedItemsDelete(int assetID)
        {
            if (isDebugEnabled) logger.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " in");

            if (compactDatabaseType)
            {
                if (assetID != 0)
                {
                    try
                    {
                        using (SqlCeConnection conn = DatabaseConnection.CreateOpenCEConnection())
                        {
                            string commandText =
                                "DELETE FROM AUDITEDITEMS WHERE _ASSETID = @forAssetID";

                            using (SqlCeCommand command = new SqlCeCommand(commandText, conn))
                            {
                                command.Parameters.AddWithValue("@forAssetID", assetID);
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
                lAuditWizardDataAccess.AuditedItemsDelete(assetID);
            }

            if (isDebugEnabled) logger.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " out");
            return 0;
        }

        /// <summary>
        /// To fix an issue with System Services
        /// </summary>
        public void CheckNewFieldsAvailable()
        {
            string commandText = "SELECT COUNT(_CATEGORY) FROM AUDITEDITEMS WHERE _CATEGORY = 'System|Services'";
            if (PerformScalarQuery(commandText) == "0")
            {
                commandText = "INSERT INTO AUDITEDITEMS (_CATEGORY ,_ASSETID, _ICON) VALUES ('System|Services', 0, 'system.png')";
                ExecuteNonQuery(commandText);
            }

            commandText = "SELECT COUNT(_CATEGORY) FROM AUDITEDITEMS WHERE _CATEGORY = 'System|Local User Accounts'";
            if (PerformScalarQuery(commandText) == "0")
            {
                commandText = "INSERT INTO AUDITEDITEMS (_CATEGORY ,_ASSETID, _ICON) VALUES ('System|Local User Accounts', 0, 'system.png')";
                ExecuteNonQuery(commandText);
            }
        }

        /// <summary>
        /// Return a table containing all of the audited items which have been declared for the 
        /// specified asset (with the specified parent)
        /// </summary>
        /// <param name="parentCategory"></param>
        /// <returns></returns>
        public DataTable GetAuditedItemCategories(string parentCategory)
        {
            string cmdText = "";

            if (parentCategory == "System|Patches")
                cmdText = "SELECT DISTINCT(_CATEGORY) FROM AUDITEDITEMS WHERE _CATEGORY LIKE 'System|Patches|%'";
            else
                cmdText =
                "SELECT DISTINCT (_CATEGORY) " +
                "FROM AUDITEDITEMS " +
                "WHERE (_CATEGORY LIKE '" + parentCategory + "|%' " +
                "AND CHARINDEX('|', _CATEGORY, len('" + parentCategory + "|') + 1) = 0)";

            return PerformQuery(cmdText);
        }


        /// <summary>
        /// Return a table containing all of the audited item categories beneath the specified parent 
        /// which have a 'Value' field - that is the NAME column has a value
        /// </summary>
        /// <param name="applicationID"></param>
        /// <returns></returns>
        public DataTable GetAuditedItemCategoryNames(string parentCategory)
        {
            string commandText = String.Format(
                            "SELECT DISTINCT _CATEGORY, _NAME " +
                            "FROM AUDITEDITEMS " +
                            "WHERE _CATEGORY = '{0}' " +
                            "AND _NAME <> '' " +
                            "ORDER BY _NAME", parentCategory);

            return PerformQuery(commandText);
        }


        /// <summary>
        /// Return a table containing the AUDITED VALUES for the specified Audited Data Item category/name optionally
        /// returning just the data for a specific asset
        /// </summary>
        /// <param name="applicationID"></param>
        /// <returns></returns>
        public DataTable GetAuditedItemValues(Asset aAsset, string aCategory, string aName)
        {
            if (isDebugEnabled) logger.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " in");

            DataTable dataTable = new DataTable(TableNames.AUDITEDITEMS);

            if (compactDatabaseType)
            {
                try
                {
                    using (SqlCeConnection conn = DatabaseConnection.CreateOpenCEConnection())
                    {
                        string commandText =
                            "SELECT _ASSETID ,_VALUE ,_DISPLAY_UNITS ,_DATATYPE FROM AUDITEDITEMS " +
                            "WHERE _category = @cCategory AND _NAME = @cName";

                        if (aAsset != null)
                            commandText += " AND _ASSETID = @assetID";

                        using (SqlCeCommand command = new SqlCeCommand(commandText, conn))
                        {
                            if (aAsset != null)
                                command.Parameters.AddWithValue("@assetID", aAsset.AssetID);

                            command.Parameters.AddWithValue("@cCategory", aCategory);
                            command.Parameters.AddWithValue("@cName", aName);

                            new SqlCeDataAdapter(command).Fill(dataTable);
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
                dataTable = lAuditWizardDataAccess.GetAuditedItemValues(aAsset, aCategory, aName);
            }

            if (isDebugEnabled) logger.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " out");
            return dataTable;
        }

        /// <summary>
        /// Add a new Audited Item to the database
        /// </summary>
        /// <param name="aAuditedItems"></param>
        /// <returns></returns>
        public void AuditedItemAdd(List<AuditedItem> aAuditedItems)
        {
            if (isDebugEnabled) logger.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " in");

            try
            {
                if (compactDatabaseType)
                {
                    using (SqlCeConnection conn = DatabaseConnection.CreateOpenCEConnection())
                    {
                        foreach (AuditedItem aItem in aAuditedItems)
                        {
                            string commandText =
                                "INSERT INTO AUDITEDITEMS " +
                                "(_CATEGORY ,_NAME, _VALUE, _ICON, _ASSETID, _DISPLAY_UNITS, _DATATYPE ,_HISTORIED ,_GROUPED) " +
                                "VALUES " +
                                "(@cCategory, @cName, @cValue, @cIcon, @nAssetID, @cDisplayUnits, @nDataType, @bHistoried ,@bGrouped)";

                            using (SqlCeCommand command = new SqlCeCommand(commandText, conn))
                            {
                                command.Parameters.AddWithValue("@cCategory", aItem.Category);
                                command.Parameters.AddWithValue("@cName", aItem.Name);
                                command.Parameters.AddWithValue("@cValue", aItem.Value);
                                command.Parameters.AddWithValue("@cIcon", aItem.Icon);
                                command.Parameters.AddWithValue("@nAssetID", aItem.AssetID);
                                command.Parameters.AddWithValue("@cDisplayUnits", aItem.DisplayUnits);
                                command.Parameters.AddWithValue("@nDataType", aItem.Datatype);
                                command.Parameters.AddWithValue("@bHistoried", aItem.Historied);
                                command.Parameters.AddWithValue("@bGrouped", aItem.Grouped);

                                command.ExecuteNonQuery();
                            }
                        }
                    }
                }
                else
                {
                    AuditWizardDataAccess lAuditWizardDataAccess = new AuditWizardDataAccess();

                    foreach (AuditedItem aItem in aAuditedItems)
                    {
                        lAuditWizardDataAccess.AuditedItemAdd(aItem);
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

            if (isDebugEnabled) logger.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " out.");
        }

        /// <summary>
        /// Add a new Audited Item to the database
        /// </summary>
        /// <param name="theLicenseType"></param>
        /// <returns></returns>
        public int AuditedItemAdd(AuditedItem aItem)
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
                            "INSERT INTO AUDITEDITEMS " +
                            "(_CATEGORY ,_NAME, _VALUE, _ICON, _ASSETID, _DISPLAY_UNITS, _DATATYPE ,_HISTORIED ,_GROUPED) " +
                            "VALUES " +
                            "(@cCategory, @cName, @cValue, @cIcon, @nAssetID, @cDisplayUnits, @nDataType, @bHistoried ,@bGrouped)";

                        using (SqlCeCommand command = new SqlCeCommand(commandText, conn))
                        {
                            command.Parameters.AddWithValue("@cCategory", aItem.Category);
                            command.Parameters.AddWithValue("@cName", aItem.Name);
                            command.Parameters.AddWithValue("@cValue", aItem.Value);
                            command.Parameters.AddWithValue("@cIcon", aItem.Icon);
                            command.Parameters.AddWithValue("@nAssetID", aItem.AssetID);
                            command.Parameters.AddWithValue("@cDisplayUnits", aItem.DisplayUnits);
                            command.Parameters.AddWithValue("@nDataType", aItem.Datatype);
                            command.Parameters.AddWithValue("@bHistoried", aItem.Historied);
                            command.Parameters.AddWithValue("@bGrouped", aItem.Grouped);

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
                lItemID = lAuditWizardDataAccess.AuditedItemAdd(aItem);
            }

            if (isDebugEnabled) logger.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " out.");
            return lItemID;
        }

        private DataTable PerformQuery(string aCommandText)
        {
            if (isDebugEnabled) logger.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " in");

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


            if (isDebugEnabled) logger.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " out");
            return statisticsTable;
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
                            object result = command.ExecuteScalar();

                            if (result != null)
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
                            object result = command.ExecuteScalar();

                            if (result != null)
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

        #endregion

        #region Report Methods

        public DataTable GetPickerValue(string category, string name)
        {
            string commandText = String.Format(
                "select distinct _value " +
                "from auditeditems " +
                "where _category = '{0}' " +
                "and _name = '{1}' " +
                "order by _value", category, name);

            return PerformQuery(commandText);
        }

        public string GetDisplayUnitValue(string category, string name)
        {
            string commandText = String.Format(
                "select _display_units " +
                "from auditeditems " +
                "where _category = '{0}' " +
                "and _name = '{1}'", category, name);

            return PerformScalarQuery(commandText);
        }

        public DataTable GetCompliantAssetsIdsForPatch(string category)
        {
            string commandText = String.Format(
                "select distinct _assetid " +
                "from auditeditems " +
                "where _category = '{0}'", category);

            return PerformQuery(commandText);
        }

        public string GetCompliantValueForPatch(string category, string assetID)
        {
            string commandText = String.Format(
                "select count (_assetid) " +
                "from auditeditems " +
                "where _category = '{0}' " +
                "and _assetid = {1}", category, assetID);

            return PerformScalarQuery(commandText);
        }

        public string GetCustomValue(int aAssetId, string aCategory, string aName)
        {
            string commandText = String.Format(
                "SELECT _value " +
                "FROM auditeditems " +
                "WHERE _assetid = {0} " +
                "AND _category = '{1}' " +
                "AND _name = '{2}'", aAssetId, aCategory, aName);

            return PerformScalarQuery(commandText);
        }

        public DataTable GetCustomValues(string aAssetIds, string aCategory, string aName)
        {
            string commandText = String.Format(
                "SELECT _value " +
                "FROM auditeditems " +
                "WHERE _category = '{0}' " +
                "AND _name = '{1}' " +
                "AND _ASSETID IN ({2}) " +
                "ORDER BY _ASSETID", aCategory, aName, aAssetIds);

            return PerformQuery(commandText);
        }

        public DataTable GetDisplayAdapters()
        {
            string commandText =
                "SELECT ASSETS._NAME as \"Asset Name\", AUDITEDITEMS._NAME as \"Item\", " +
                "AUDITEDITEMS._VALUE + AUDITEDITEMS._DISPLAY_UNITS as \"Value\" " +
                "FROM AUDITEDITEMS INNER JOIN ASSETS ON " +
                "ASSETS._ASSETID = AUDITEDITEMS._ASSETID " +
                "WHERE AUDITEDITEMS._CATEGORY LIKE 'Hardware|Adapters|Display|%'";

            return PerformQuery(commandText);
        }

        public DataTable GetUSBDevicesFromLastAudit()
        {
            string commandText =
                "SELECT A._NAME AS \"Asset Name\", AI._VALUE AS \"USB Model\", A._LASTAUDIT AS \"Date Audited\" " +
                "FROM ASSETS A INNER JOIN AUDITEDITEMS AI ON (A._ASSETID = AI._ASSETID) " +
                "WHERE (AI._CATEGORY LIKE '%USB%') AND (AI._NAME = 'Model') " +
                "ORDER BY A._NAME";

            return PerformQuery(commandText);
        }

        public DataTable GetInstalledBrowsersByAsset()
        {
            string commandText =
                "SELECT a._NAME as \"Asset Name\", l._FULLNAME as \"Location\", SUBSTRING(_CATEGORY, 10, LEN(_CATEGORY) - 9) AS \"Installed Browser\" " +
                "FROM ASSETS a " +
                "INNER JOIN LOCATIONS l ON (l._LOCATIONID = a._LOCATIONID) " +
                "INNER JOIN AUDITEDITEMS ai ON (ai._ASSETID = a._ASSETID) " +
                "WHERE _CATEGORY LIKE 'Internet|%' " +
                "AND ai._NAME = 'Default Browser' " +
                "ORDER BY a._NAME";

            return PerformQuery(commandText);
        }

        public DataTable GetPrintersAssociatedWithAssets()
        {
            string commandText =
                "SELECT DISTINCT A._NAME AS \"Asset Name\", SUBSTRING(AI._CATEGORY, 19, 881) AS \"Printer Name\" " +
                "FROM ASSETS A " +
                "INNER JOIN AUDITEDITEMS AI ON (A._ASSETID = AI._ASSETID) " +
                "WHERE AI._CATEGORY LIKE 'Hardware|Printers|%' " +
                "ORDER BY A._NAME, \"Printer Name\"";

            return PerformQuery(commandText);
        }

        public DataTable GetChildrenFromParent(string aParentNode)
        {
            string commandText;

            if (aParentNode == "System|Patches")
                commandText = "SELECT DISTINCT(_CATEGORY) FROM AUDITEDITEMS WHERE (_CATEGORY LIKE 'System|Patches|%')";

            else
            {
                commandText = String.Format(
                    "SELECT DISTINCT (_CATEGORY) " +
                    "FROM AUDITEDITEMS " +
                    "WHERE (_CATEGORY LIKE '{0}|%' AND CHARINDEX('|', _CATEGORY, len('{0}|') + 1) = 0)", aParentNode);
            }

            return PerformQuery(commandText);
        }

        public DataTable GetValuesForChildNode(string aChildNode)
        {
            string commandText = String.Format(
                        "SELECT DISTINCT (_CATEGORY) + '|' + _NAME " +
                        "FROM AUDITEDITEMS " +
                        "WHERE _CATEGORY LIKE '{0}' AND _NAME <> ''", aChildNode);

            return PerformQuery(commandText);
        }

        public void UpdateHardwareDisplayCategory()
        {
            string cmdText = 
                "UPDATE AUDITEDITEMS " +
                "SET _CATEGORY = 'Hardware|Adapters' " +
                "WHERE _CATEGORY = 'Hardware|Display' " +
                "AND _ASSETID = 0";

            ExecuteNonQuery(cmdText);
        }

        public DataTable GetDefaultBrowsers()
        {
            string cmdTxt =
                "SELECT SUBSTRING(_CATEGORY, 10, LEN(_CATEGORY) - 9) " +
                "FROM AUDITEDITEMS " +
                "WHERE _NAME = 'Default Browser' " + 
                "GROUP BY _CATEGORY";

            return PerformQuery(cmdTxt);
        }

        public DataTable GetVersionBrowsers(string browserName)
        {
            string cmdTxt =
                "SELECT DISTINCT SUBSTRING(_CATEGORY, 10, LEN(_CATEGORY) - 9) , _VALUE " +
                "FROM AUDITEDITEMS " +
                "WHERE _CATEGORY = 'Internet|" + browserName + "'" +
                "AND _NAME = 'Version'";

            return PerformQuery(cmdTxt);
        }

        #endregion
    }
}
