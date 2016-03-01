using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlServerCe;
using System.IO;
using System.Windows.Forms;

namespace Layton.AuditWizard.DataAccess
{
    public class AssetTypesDAO
    {
        #region Data

        protected static readonly log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private static readonly bool isDebugEnabled = logger.IsDebugEnabled;
        private bool compactDatabaseType = false;
        private Configuration config = ConfigurationManager.OpenExeConfiguration(Path.Combine(Application.StartupPath, "AuditWizardv8.exe"));        

        #endregion   
     
        public AssetTypesDAO()
        {
            compactDatabaseType = Convert.ToBoolean(config.AppSettings.Settings["CompactDatabaseType"].Value);
        }

        #region Asset Types Table

        /// <summary>
        /// Return a table containing all of the Asset Types which have been defined
        /// </summary>
        /// <returns></returns>
        public DataTable EnumerateAssetTypes()
        {
            if (isDebugEnabled) logger.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " in");

            DataTable table = new DataTable(TableNames.ASSET_TYPES);

            if (compactDatabaseType)
            {
                try
                {
                    using (SqlCeConnection conn = DatabaseConnection.CreateOpenCEConnection())
                    {
                        string commandText =
                               "SELECT ASSET_TYPES.* " +
	                           "FROM ASSET_TYPES " +
	                           "ORDER BY _ASSETTYPEID";

                        using (SqlCeCommand command = new SqlCeCommand(commandText, conn))
                        {
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
                table = lAuditWizardDataAccess.EnumerateAssetTypes();
            }

            if (isDebugEnabled) logger.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " out");
            return table;
        }

        public int GetAssetTypeIDByName(string aAssetTypeName)
        {
            if (isDebugEnabled) logger.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " in");

            int lAssetTypeId = -1;

            if (compactDatabaseType)
            {
                try
                {
                    using (SqlCeConnection conn = DatabaseConnection.CreateOpenCEConnection())
                    {
                        string commandText =
                               "SELECT _ASSETTYPEID " +
                               "FROM ASSET_TYPES " +
                               "WHERE _NAME = @assetTypeName";

                        using (SqlCeCommand command = new SqlCeCommand(commandText, conn))
                        {
                            command.Parameters.AddWithValue("@assetTypeName", aAssetTypeName);
                            object result = command.ExecuteScalar();

                            if ((result != null) && (result.GetType() != typeof(DBNull)))
                                lAssetTypeId = Convert.ToInt32(result);
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
                lAssetTypeId = lAuditWizardDataAccess.GetAssetTypeIDByName(aAssetTypeName);
            }

            if (isDebugEnabled) logger.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " out");
            return lAssetTypeId;
        }


        /// <summary>
        /// Add a new Asset Type to the database
        /// </summary>
        /// <param name="theAlert"></param>
        /// <returns></returns>
        public int AssetTypeAdd(AssetType aAssetType)
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
                            "INSERT INTO ASSET_TYPES " +
		                    "(_NAME ,_PARENTID ,_AUDITABLE ,_ICON) " +
	                        "VALUES " +
		                    "(@cName, @nParentID, @bAuditable ,@cIcon)";

                        SqlCeParameter[] spParams = new SqlCeParameter[4];
                        spParams[0] = new SqlCeParameter("@cName", aAssetType.Name);
                        
                        if (aAssetType.ParentID == 0)
                            spParams[1] = new SqlCeParameter("@nParentID", DBNull.Value);
                        else
                            spParams[1] = new SqlCeParameter("@nParentID", aAssetType.ParentID);

                        spParams[2] = new SqlCeParameter("@bAuditable", Convert.ToInt32(aAssetType.Auditable));
                        spParams[3] = new SqlCeParameter("@cIcon", aAssetType.Icon);

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
                lItemID = lAuditWizardDataAccess.AssetTypeAdd(aAssetType);
            }

            if (isDebugEnabled) logger.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " out");
            return lItemID;
        }


        /// <summary>
        /// Update the specified Asset Type
        /// </summary>
        /// <param name="theAsset"></param>
        /// <returns></returns>
        public int AssetTypeUpdate(AssetType aAssetType)
        {
            if (isDebugEnabled) logger.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " in");

            if (compactDatabaseType)
            {
                try
                {
                    using (SqlCeConnection conn = DatabaseConnection.CreateOpenCEConnection())
                    {
                        string commandText =
                               "UPDATE ASSET_TYPES " +
		                       "SET " + 
		                       "_NAME = @cName, " +
		                       "_PARENTID = @nParentID, " +
		                       "_ICON = @cIcon " +
                               "WHERE _ASSETTYPEID = @nAssetTypeID";

                        SqlCeParameter[] spParams = new SqlCeParameter[4];
                        spParams[0] = new SqlCeParameter("@nAssetTypeID", aAssetType.AssetTypeID);
                        spParams[1] = new SqlCeParameter("@cName", aAssetType.Name);
                        
                        if (aAssetType.ParentID == 0)
                            spParams[2] = new SqlCeParameter("@nParentID", DBNull.Value);
                        else
                            spParams[2] = new SqlCeParameter("@nParentID", aAssetType.ParentID);

                        spParams[3] = new SqlCeParameter("@cIcon", aAssetType.Icon);

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
                lAuditWizardDataAccess.AssetTypeUpdate(aAssetType);
            }

            if (isDebugEnabled) logger.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " out");
            return 0;
        }




        /// <summary>
        /// Delete the specified AssetType from the database
        /// </summary>
        /// <param name="licenseID"></param>
        /// <returns></returns>
        public int AssetTypeDelete(AssetType aAssetType)
        {
            if (isDebugEnabled) logger.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " in");

            int references = 0;

            if (compactDatabaseType)
            {
                // The delete code will perform a sanity check to ensure that we do not
                // delete user defined categories/fields which are still referenced so we simply delete it now
                int lParentID = -1;
                int lReferences = -1;

                try
                {
                    using (SqlCeConnection conn = DatabaseConnection.CreateOpenCEConnection())
                    {
                        string commandText =
                               "SELECT _PARENTID FROM ASSET_TYPES WHERE _ASSETTYPEID = @nAssetTypeID";

                        using (SqlCeCommand command = new SqlCeCommand(commandText, conn))
                        {
                            command.Parameters.AddWithValue("@nAssetTypeID", aAssetType.AssetTypeID);
                            object result = command.ExecuteScalar();

                            if ((result != null) && (result.GetType() != typeof(DBNull)))
                                lParentID = (int)result;
                        }

                        if (lParentID == -1)
                        {
                            commandText =
                                "SELECT min(_ASSETTYPEID) FROM ASSET_TYPES WHERE _PARENTID = @nAssetTypeID";

                            using (SqlCeCommand command = new SqlCeCommand(commandText, conn))
                            {
                                command.Parameters.AddWithValue("@nAssetTypeID", aAssetType.AssetTypeID);
                                object result = command.ExecuteScalar();

                                if ((result != null) && (result.GetType() != typeof(DBNull)))
                                    lReferences = (int)result;
                            }
                        }
                        else
                        {
                            commandText =
                                "SELECT min(_ASSETID) FROM ASSETS WHERE _ASSETTYPEID = @nAssetTypeID";

                            using (SqlCeCommand command = new SqlCeCommand(commandText, conn))
                            {
                                command.Parameters.AddWithValue("@nAssetTypeID", aAssetType.AssetTypeID);
                                object result = command.ExecuteScalar();

                                if ((result != null) && (result.GetType() != typeof(DBNull)))
                                    lReferences = (int)result;
                            }
                        }

                        if (lReferences != -1)
                        {
                            references = -1;
                        }
                        else
                        {
                            commandText =
                                "DELETE FROM ASSET_TYPES WHERE _ASSETTYPEID = @nAssetTypeID";

                            using (SqlCeCommand command = new SqlCeCommand(commandText, conn))
                            {
                                command.Parameters.AddWithValue("@nAssetTypeID", aAssetType.AssetTypeID);
                                command.ExecuteNonQuery();
                            }
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
                lAuditWizardDataAccess.AssetTypeDelete(aAssetType);
            }

            if (isDebugEnabled) logger.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " out");
            return references;
        }

        public DataTable GetCompliantAssetCategoriesValue(string assetValue)
        {
            string commandText = String.Format(
                "SELECT _assetid " +
                "FROM ASSETS a " +
                "LEFT JOIN ASSET_TYPES at ON (at._ASSETTYPEID = a._ASSETTYPEID) " +
                "INNER JOIN ASSET_TYPES at1 ON (at._PARENTID = at1._ASSETTYPEID) " +
                "where at1._name = {0}", assetValue);

            return PerformQuery(commandText);
        }

        public DataTable GetAssetCategoriesPickerValues()
        {
            string commandText =
                "SELECT DISTINCT _name " +
                "FROM asset_types " +
                "WHERE _parentid IS NULL";

            return PerformQuery(commandText);
        }

        public DataTable GetCompliantAssetTypesValue(string assetValue)
        {
            string commandText = String.Format(
                "SELECT _assetid " +
                "FROM ASSETS a " +
                "LEFT JOIN ASSET_TYPES at ON (at._ASSETTYPEID = a._ASSETTYPEID) " + 
                "WHERE at._name = {0}", assetValue);

            return PerformQuery(commandText);
        }

        public DataTable GetAssetTypesPickerValues()
        {
            string commandText =
                "SELECT DISTINCT at._NAME " +
                "FROM ASSETS a " +
                "INNER JOIN ASSET_TYPES at ON (at._ASSETTYPEID = a._ASSETTYPEID)";

            return PerformQuery(commandText);
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


        #endregion Asset Types Table
    }
}
