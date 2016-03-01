using System;
using System.Configuration;
using System.Data;
using System.Data.SqlServerCe;
using System.IO;
using System.Windows.Forms;

namespace Layton.AuditWizard.DataAccess
{
    class PicklistsDAO
    {
        #region Data

        private string connectionStringCompact = null;
        protected static readonly log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private static readonly bool isDebugEnabled = logger.IsDebugEnabled;
        private bool compactDatabaseType = false;
        private Configuration config = ConfigurationManager.OpenExeConfiguration(Path.Combine(Application.StartupPath, "AuditWizardv8.exe"));

        #endregion

        public PicklistsDAO()
        {
            compactDatabaseType = Convert.ToBoolean(config.AppSettings.Settings["CompactDatabaseType"].Value);
            connectionStringCompact = "Data Source=" + Application.StartupPath + Convert.ToString(config.AppSettings.Settings["ConnectionStringCompact"].Value);
        }

        #region PickList Table

        /// <summary>
        /// Return a table containing all of the PickLists and PickItem entries
        /// </summary>
        /// <returns></returns>
        public DataTable EnumeratePickLists()
        {
            if (isDebugEnabled) logger.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " in");
            DataTable table = new DataTable(TableNames.UDDD);

            if (compactDatabaseType)
            {
                try
                {
                    using (SqlCeConnection conn = DatabaseConnection.CreateOpenCEConnection())
                    {
                        string commandText =
                            "SELECT _PICKLISTID, _NAME ,_PARENTID " + 
	                        "FROM PICKLISTS " +
	                        "ORDER BY _PICKLISTID";

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
                table = lAuditWizardDataAccess.EnumeratePickLists();
            }

            if (isDebugEnabled) logger.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " out");
            return table;
        }

        /// <summary>
        /// Return a table containing all of the PickItem entries for the specfiied picklist
        /// </summary>
        /// <returns></returns>
        public DataTable EnumeratePickItems(int aPicklist)
        {
            if (isDebugEnabled) logger.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " in");
            DataTable table = new DataTable(TableNames.UDDD);

            if (compactDatabaseType)
            {
                try
                {
                    using (SqlCeConnection conn = DatabaseConnection.CreateOpenCEConnection())
                    {
                        string commandText =
                            "SELECT _PICKLISTID, _NAME ,_PARENTID " +
	                        "FROM PICKLISTS " +
                            "WHERE _PARENTID = @nPicklistID " +
	                        "ORDER BY _PICKLISTID";

                        using (SqlCeCommand command = new SqlCeCommand(commandText, conn))
                        {
                            command.Parameters.AddWithValue("@nPicklistID", aPicklist);
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
                table = lAuditWizardDataAccess.EnumeratePickItems(aPicklist);
            }

            if (isDebugEnabled) logger.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " out");
            return table;
        }



        /// <summary>
        /// Add a new PickList to the database
        /// </summary>
        /// <param name="theAlert"></param>
        /// <returns></returns>
        public int PickListAdd(PickList aPickList)
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
                            "INSERT INTO PICKLISTS " +
		                    "(_NAME ,_PARENTID) " +
	                        "VALUES " +
		                    "(@cName, @nParentID)";

                        SqlCeParameter[] spParams = new SqlCeParameter[2];
                        spParams[0] = new SqlCeParameter("@cName", aPickList.Name);
                        spParams[1] = new SqlCeParameter("@nParentID", DBNull.Value);

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
                lItemID = lAuditWizardDataAccess.PickListAdd(aPickList);
            }

            if (isDebugEnabled) logger.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " out");
            return lItemID;
        }



        /// <summary>
        /// Add a new PickItem to the database
        /// </summary>
        /// <param name="theAlert"></param>
        /// <returns></returns>
        public int PickItemAdd(PickItem aPickItem)
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
                            "INSERT INTO PICKLISTS " +
                            "(_NAME ,_PARENTID) " +
                            "VALUES " +
                            "(@cName, @nParentID)";

                        SqlCeParameter[] spParams = new SqlCeParameter[2];
                        spParams[0] = new SqlCeParameter("@cName", aPickItem.Name);
                        spParams[1] = new SqlCeParameter("@nParentID", aPickItem.ParentID);

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
                lItemID = lAuditWizardDataAccess.PickItemAdd(aPickItem);
            }

            if (isDebugEnabled) logger.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " out");
            return lItemID;
        }


        /// <summary>
        /// Update the definintion for a PickList
        /// </summary>
        /// <param name="category"></param>
        /// <returns></returns>
        public int PickListUpdate(PickList aPickList)
        {
            if (isDebugEnabled) logger.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " in");

            if (compactDatabaseType)
            {
                try
                {
                    using (SqlCeConnection conn = DatabaseConnection.CreateOpenCEConnection())
                    {
                        string commandText =
                            "UPDATE PICKLISTS SET " +
		                    "_NAME = @cName " +
                            "WHERE " +
		                    "_PICKLISTID = @nPicklistID";

                        SqlCeParameter[] spParams = new SqlCeParameter[2];
                        spParams[0] = new SqlCeParameter("@nPicklistID", aPickList.PicklistID);
                        spParams[1] = new SqlCeParameter("@cName", aPickList.Name);

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
                lAuditWizardDataAccess.PickListUpdate(aPickList);
            }

            if (isDebugEnabled) logger.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " out");
            return 0;
        }



        /// <summary>
        /// Update the definintion for a PickItem
        /// </summary>
        /// <param name="category"></param>
        /// <returns></returns>
        public int PickItemUpdate(PickItem aPickItem)
        {
            if (isDebugEnabled) logger.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " in");

            if (compactDatabaseType)
            {
                try
                {
                    using (SqlCeConnection conn = DatabaseConnection.CreateOpenCEConnection())
                    {
                        string commandText =
                            "UPDATE PICKLISTS SET " +
                            "_NAME = @cName " +
                            "WHERE " +
                            "_PICKLISTID = @nPicklistID";

                        SqlCeParameter[] spParams = new SqlCeParameter[2];
                        spParams[0] = new SqlCeParameter("@nPicklistID", aPickItem.PickItemID);
                        spParams[1] = new SqlCeParameter("@cName", aPickItem.Name);

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
                lAuditWizardDataAccess.PickItemUpdate(aPickItem);
            }

            if (isDebugEnabled) logger.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " out");
            return 0;
        }



        /// <summary>
        /// Delete the specified PickList from the database
        /// Note that it is the responsibility of the caller to ensure that the referential integrity of the
        /// database is retained - i.e. do not delete PickLists that are still being referenced
        /// </summary>
        /// <param name="licenseID"></param>
        /// <returns></returns>
        public int PickListDelete(int aIndex)
        {
            if (isDebugEnabled) logger.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " in");

            if (compactDatabaseType)
            {
                // The delete code will perform a sanity check to ensure that we do not
                // delete user defined categories/fields which are still referenced so we simply delete it now

                try
                {
                    using (SqlCeConnection conn = DatabaseConnection.CreateOpenCEConnection())
                    {
                        string commandText =
                            "DELETE FROM PICKLISTS WHERE _PICKLISTID = @nPicklistID";

                        SqlCeParameter[] spParams = new SqlCeParameter[1];
                        spParams[0] = new SqlCeParameter("@nPicklistID", aIndex);

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
                lAuditWizardDataAccess.PickListDelete(aIndex);
            }

            if (isDebugEnabled) logger.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " out");
            return 0;
        }


        #endregion User Defined data Definitions
    }
}
