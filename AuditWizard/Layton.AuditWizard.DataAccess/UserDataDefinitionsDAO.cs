using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlServerCe;
using System.IO;
using System.Windows.Forms;

namespace Layton.AuditWizard.DataAccess
{
    public class UserDataDefinitionsDAO
    {
        #region Data

        private string connectionStringCompact = null;
        protected static readonly log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private static readonly bool isDebugEnabled = logger.IsDebugEnabled;
        private bool compactDatabaseType = false;
        private Configuration config = ConfigurationManager.OpenExeConfiguration(Path.Combine(Application.StartupPath, "AuditWizardv8.exe"));

        #endregion

        public UserDataDefinitionsDAO()
        {
            compactDatabaseType = Convert.ToBoolean(config.AppSettings.Settings["CompactDatabaseType"].Value);
            connectionStringCompact = "Data Source=" + Application.StartupPath + Convert.ToString(config.AppSettings.Settings["ConnectionStringCompact"].Value);
        }

        #region User Defined Data Definitions

        /// <summary>
        /// Return a table containing all of the User Defined Data Field Definitions (of the specified type)
        /// </summary>
        /// <returns></returns>
        public DataTable EnumerateUserDataDefinitions(UserDataCategory.SCOPE aScope)
        {
            if (isDebugEnabled) logger.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " in");

            DataTable dt = new DataTable();

            string cmdText;

            if (aScope == UserDataCategory.SCOPE.Any)
            {
                cmdText =
                    "SELECT USERDATA_DEFINITIONS.* " +
                    ",ASSET_TYPES._NAME  AS ASSETTYPENAME " +
                    "FROM USERDATA_DEFINITIONS " +
                    "LEFT JOIN ASSET_TYPES on USERDATA_DEFINITIONS._APPLIESTO = ASSET_TYPES._ASSETTYPEID " +
                    "ORDER BY USERDATA_DEFINITIONS._PARENTID, USERDATA_DEFINITIONS._TABORDER ";
            }
            else if (aScope == UserDataCategory.SCOPE.Application)
            {
                cmdText =
                    "SELECT USERDATA_DEFINITIONS.* " +
                    ",ASSET_TYPES._NAME  AS ASSETTYPENAME " +
                    "FROM USERDATA_DEFINITIONS " +
                    "LEFT JOIN ASSET_TYPES on USERDATA_DEFINITIONS._APPLIESTO = ASSET_TYPES._ASSETTYPEID " +
                    "WHERE _SCOPE = 1 " +
                    "ORDER BY USERDATA_DEFINITIONS._PARENTID, USERDATA_DEFINITIONS._TABORDER ";
            }
            else
            {
                cmdText =
                    "SELECT USERDATA_DEFINITIONS.* " +
                    ",ASSET_TYPES._NAME  AS ASSETTYPENAME " +
                    "FROM USERDATA_DEFINITIONS " +
                    "LEFT JOIN ASSET_TYPES on USERDATA_DEFINITIONS._APPLIESTO = ASSET_TYPES._ASSETTYPEID " +
                    "WHERE _SCOPE = 0 OR _SCOPE = 2 " +
                    "ORDER BY USERDATA_DEFINITIONS._PARENTID, USERDATA_DEFINITIONS._TABORDER ";
            }

            try
            {
                if (compactDatabaseType)
                {
                    using (SqlCeConnection conn = DatabaseConnection.CreateOpenCEConnection())
                    {
                        using (SqlCeCommand command = new SqlCeCommand(cmdText, conn))
                        {
                            new SqlCeDataAdapter(command).Fill(dt);
                        }
                    }
                }
                else
                {
                    using (SqlConnection conn = DatabaseConnection.CreateOpenStandardConnection())
                    {
                        using (SqlCommand command = new SqlCommand(cmdText, conn))
                        {
                            new SqlDataAdapter(command).Fill(dt);
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
            return dt;
        }



        /// <summary>
        /// Add a new User Data Category Definition to the database
        /// </summary>
        /// <param name="theAlert"></param>
        /// <returns></returns>
        public int UserDataCategoryAdd(UserDataCategory aCategory)
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
                               "INSERT INTO USERDATA_DEFINITIONS " +
                               " (_NAME, _PARENTID ,_ISMANDATORY ,_TYPE ,_APPLIESTO ,_VALUE1 ,_VALUE2 ,_TABORDER ,_ICON ,_SCOPE) " +
                               "VALUES " +
                               "(@cName, @nParentID ,@bIsMandatory ,@nType ,@nAppliesTo ,@cValue1 ,@cValue2 ,@nTabOrder ,@cIcon ,@nScope)";

                        SqlCeParameter[] spParams = new SqlCeParameter[10];
                        spParams[0] = new SqlCeParameter("@cName", aCategory.Name);
                        spParams[1] = new SqlCeParameter("@nParentID", DBNull.Value);
                        spParams[2] = new SqlCeParameter("@bIsMandatory", Convert.ToBoolean(0));
                        spParams[3] = new SqlCeParameter("@nType", Convert.ToInt32(0));
                        spParams[4] = new SqlCeParameter("@nAppliesTo", aCategory.AppliesTo);
                        spParams[5] = new SqlCeParameter("@cValue1", String.Empty);
                        spParams[6] = new SqlCeParameter("@cValue2", String.Empty);
                        spParams[7] = new SqlCeParameter("@nTabOrder", aCategory.TabOrder);
                        spParams[8] = new SqlCeParameter("@cIcon", aCategory.Icon);
                        spParams[9] = new SqlCeParameter("@nScope", (int)aCategory.Scope);

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
                lItemID = lAuditWizardDataAccess.UserDataCategoryAdd(aCategory);
            }

            if (isDebugEnabled) logger.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " out");
            return lItemID;
        }

        /// <summary>
        /// Add a new User Data Category Definition to the database
        /// </summary>
        /// <param name="theAlert"></param>
        /// <returns></returns>
        public int SelectCountUserData()
        {
            if (isDebugEnabled) logger.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " in");
            int lCount = 0;

            if (compactDatabaseType)
            {
                try
                {
                    using (SqlCeConnection conn = DatabaseConnection.CreateOpenCEConnection())
                    {
                        string commandText =
                               "SELECT COUNT(*) FROM USERDATA_DEFINITIONS";

                        using (SqlCeCommand command = new SqlCeCommand(commandText, conn))
                        {
                            lCount = Convert.ToInt32(command.ExecuteScalar());
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
                lCount = lAuditWizardDataAccess.SelectCountUserData();
            }

            if (isDebugEnabled) logger.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " out with count:" + lCount);
            return lCount;
        }



        /// <summary>
        /// Add a new User Data Field Definition to the database
        /// </summary>
        /// <param name="theAlert"></param>
        /// <returns></returns>
        public int UserDataFieldAdd(UserDataField aField)
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
                           "INSERT INTO USERDATA_DEFINITIONS " +
                           " (_NAME, _PARENTID ,_ISMANDATORY ,_TYPE ,_APPLIESTO ,_VALUE1 ,_VALUE2 ,_TABORDER ,_ICON ,_SCOPE) " +
                           "VALUES " +
                           "(@cName, @nParentID ,@bIsMandatory ,@nType ,@nAppliesTo ,@cValue1 ,@cValue2 ,@nTabOrder ,@cIcon ,@nScope)";

                        SqlCeParameter[] spParams = new SqlCeParameter[10];
                        spParams[0] = new SqlCeParameter("@cName", aField.Name);
                        spParams[1] = new SqlCeParameter("@nParentID", aField.ParentID);
                        spParams[2] = new SqlCeParameter("@bIsMandatory", Convert.ToInt32(aField.IsMandatory));
                        spParams[3] = new SqlCeParameter("@nType", (int)aField.Type);
                        spParams[4] = new SqlCeParameter("@nAppliesTo", Convert.ToInt32(0));
                        spParams[5] = new SqlCeParameter("@cValue1", aField.Value1);
                        spParams[6] = new SqlCeParameter("@cValue2", aField.Value2);
                        spParams[7] = new SqlCeParameter("@nTabOrder", aField.TabOrder);
                        spParams[8] = new SqlCeParameter("@cIcon", String.Empty);
                        spParams[9] = new SqlCeParameter("@nScope", (int)aField.ParentScope);

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
                lItemID = lAuditWizardDataAccess.UserDataFieldAdd(aField);
            }

            if (isDebugEnabled) logger.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " out");
            return lItemID;
        }


        /// <summary>
        /// Update the definintion for a User Data Category
        /// </summary>
        /// <param name="aCategory"></param>
        /// <returns></returns>
        public int UserDataCategoryUpdate(UserDataCategory aCategory)
        {
            if (isDebugEnabled) logger.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " in");

            try
            {
                string cmdText =
                               "UPDATE USERDATA_DEFINITIONS SET " +
                               "_NAME = @cName, " +
                               "_PARENTID = @nCategoryID, " +
                               "_ISMANDATORY = @bIsMandatory, " +
                               "_TYPE = @nType, " +
                               "_TABORDER = @nTabOrder, " +
                               "_APPLIESTO = @nAppliesTo, " +
                               "_VALUE1 = @cValue1, " +
                               "_VALUE2 = @cValue2, " +
                               "_ICON = @cIcon " +
                               "WHERE " +
                               "_USERDEFID = @nUserDefID";

                if (compactDatabaseType)
                {
                    using (SqlCeConnection conn = DatabaseConnection.CreateOpenCEConnection())
                    {
                        SqlCeParameter[] spParams = new SqlCeParameter[10];
                        spParams[0] = new SqlCeParameter("@nUserDefID", aCategory.CategoryID);
                        spParams[1] = new SqlCeParameter("@nCategoryID", DBNull.Value);
                        spParams[2] = new SqlCeParameter("@cName", aCategory.Name);
                        spParams[3] = new SqlCeParameter("@bIsMandatory", Convert.ToInt32(0));
                        spParams[4] = new SqlCeParameter("@nType", Convert.ToInt32(0));
                        spParams[5] = new SqlCeParameter("@nTabOrder", aCategory.TabOrder);
                        spParams[6] = new SqlCeParameter("@nAppliesTo", aCategory.AppliesTo);
                        spParams[7] = new SqlCeParameter("@cValue1", String.Empty);
                        spParams[8] = new SqlCeParameter("@cValue2", String.Empty);
                        spParams[9] = new SqlCeParameter("@cIcon", aCategory.Icon);

                        using (SqlCeCommand command = new SqlCeCommand(cmdText, conn))
                        {
                            command.Parameters.AddRange(spParams);
                            command.ExecuteNonQuery();
                        }
                    }
                }
                else
                {
                    using (SqlConnection conn = DatabaseConnection.CreateOpenStandardConnection())
                    {
                        SqlParameter[] spParams = new SqlParameter[10];
                        spParams[0] = new SqlParameter("@nUserDefID", aCategory.CategoryID);
                        spParams[1] = new SqlParameter("@nCategoryID", DBNull.Value);
                        spParams[2] = new SqlParameter("@cName", aCategory.Name);
                        spParams[3] = new SqlParameter("@bIsMandatory", Convert.ToInt32(0));
                        spParams[4] = new SqlParameter("@nType", Convert.ToInt32(0));
                        spParams[5] = new SqlParameter("@nTabOrder", aCategory.TabOrder);
                        spParams[6] = new SqlParameter("@nAppliesTo", aCategory.AppliesTo);
                        spParams[7] = new SqlParameter("@cValue1", String.Empty);
                        spParams[8] = new SqlParameter("@cValue2", String.Empty);
                        spParams[9] = new SqlParameter("@cIcon", aCategory.Icon);

                        using (SqlCommand command = new SqlCommand(cmdText, conn))
                        {
                            command.Parameters.AddRange(spParams);
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


            if (isDebugEnabled) logger.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " out");
            return 0;
        }

        public void UpdateUserDataValueParentType(int userDefId, int parentTypeId)
        {
            if (isDebugEnabled) logger.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " in");

            string commandText = "UPDATE USERDATA_VALUES SET _PARENTTYPE = @cParentType WHERE _USERDEFID = @nUserDefID";

            try
            {
                if (compactDatabaseType)
                {
                    using (SqlCeConnection conn = DatabaseConnection.CreateOpenCEConnection())
                    {
                        SqlCeParameter[] spParams = new SqlCeParameter[2];
                        spParams[0] = new SqlCeParameter("@nUserDefID", userDefId);
                        spParams[1] = new SqlCeParameter("@cParentType", parentTypeId);

                        using (SqlCeCommand command = new SqlCeCommand(commandText, conn))
                        {
                            command.Parameters.AddRange(spParams);
                            command.ExecuteNonQuery();
                        }
                    }
                }
                else
                {
                    using (SqlConnection conn = DatabaseConnection.CreateOpenStandardConnection())
                    {
                        SqlParameter[] spParams = new SqlParameter[2];
                        spParams[0] = new SqlParameter("@nUserDefID", userDefId);
                        spParams[1] = new SqlParameter("@cParentType", parentTypeId);

                        using (SqlCommand command = new SqlCommand(commandText, conn))
                        {
                            command.Parameters.AddRange(spParams);
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
        /// Update the definintion for a User Data Field
        /// </summary>
        /// <param name="category"></param>
        /// <returns></returns>
        public int UserDataFieldUpdate(UserDataField aField)
        {
            if (isDebugEnabled) logger.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " in");

            string commandText =
                               "UPDATE USERDATA_DEFINITIONS SET " +
                               "_NAME = @cName, " +
                               "_PARENTID = @nCategoryID, " +
                               "_ISMANDATORY = @bIsMandatory, " +
                               "_TYPE = @nType, " +
                               "_APPLIESTO = @nAppliesTo, " +
                               "_VALUE1 = @cValue1, " +
                               "_VALUE2 = @cValue2, " +
                               "_TABORDER = @nTabOrder, " +
                               "_ICON = @cIcon, " +
                               "_SCOPE = @nScope " +
                               "WHERE " +
                               "_USERDEFID = @nUserDefID";


            try
            {
                if (compactDatabaseType)
                {
                    using (SqlCeConnection conn = DatabaseConnection.CreateOpenCEConnection())
                    {
                        SqlCeParameter[] spParams = new SqlCeParameter[11];
                        spParams[0] = new SqlCeParameter("@nUserDefID", aField.FieldID);
                        spParams[1] = new SqlCeParameter("@nCategoryID", aField.ParentID);
                        spParams[2] = new SqlCeParameter("@cName", aField.Name);
                        spParams[3] = new SqlCeParameter("@bIsMandatory", Convert.ToInt32(aField.IsMandatory));
                        spParams[4] = new SqlCeParameter("@nType", (int)aField.Type);
                        spParams[5] = new SqlCeParameter("@nAppliesTo", Convert.ToInt32(0));
                        spParams[6] = new SqlCeParameter("@cValue1", aField.Value1);
                        spParams[7] = new SqlCeParameter("@cValue2", aField.Value2);
                        spParams[8] = new SqlCeParameter("@nTabOrder", aField.TabOrder);
                        spParams[9] = new SqlCeParameter("@cIcon", String.Empty);
                        spParams[10] = new SqlCeParameter("@nScope", (int)aField.ParentScope);

                        using (SqlCeCommand command = new SqlCeCommand(commandText, conn))
                        {
                            command.Parameters.AddRange(spParams);
                            command.ExecuteNonQuery();
                        }
                    }
                }
                else
                {
                    using (SqlConnection conn = DatabaseConnection.CreateOpenStandardConnection())
                    {
                        SqlParameter[] spParams = new SqlParameter[11];
                        spParams[0] = new SqlParameter("@nUserDefID", aField.FieldID);
                        spParams[1] = new SqlParameter("@nCategoryID", aField.ParentID);
                        spParams[2] = new SqlParameter("@cName", aField.Name);
                        spParams[3] = new SqlParameter("@bIsMandatory", Convert.ToInt32(aField.IsMandatory));
                        spParams[4] = new SqlParameter("@nType", (int)aField.Type);
                        spParams[5] = new SqlParameter("@nAppliesTo", Convert.ToInt32(0));
                        spParams[6] = new SqlParameter("@cValue1", aField.Value1);
                        spParams[7] = new SqlParameter("@cValue2", aField.Value2);
                        spParams[8] = new SqlParameter("@nTabOrder", aField.TabOrder);
                        spParams[9] = new SqlParameter("@cIcon", String.Empty);
                        spParams[10] = new SqlParameter("@nScope", (int)aField.ParentScope);

                        using (SqlCommand command = new SqlCommand(commandText, conn))
                        {
                            command.Parameters.AddRange(spParams);
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
            return 0;
        }

        /// <summary>
        /// Delete the specified User Data Category / Field
        /// </summary>
        /// <param name="aUserDefID"></param>
        /// <returns></returns>
        public bool UserDataDefinitionFieldDelete(int aUserDefID)
        {
            if (isDebugEnabled) logger.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " in");

            bool success = false;

            try
            {
                if (compactDatabaseType)
                {
                    using (SqlCeConnection conn = DatabaseConnection.CreateOpenCEConnection())
                    {
                        string commandText = "DELETE FROM USERDATA_VALUES WHERE _USERDEFID = @nUserDataID";

                        using (SqlCeCommand command = new SqlCeCommand(commandText, conn))
                        {
                            command.Parameters.AddWithValue("@nUserDataID", aUserDefID);
                            command.ExecuteNonQuery();
                        }

                        commandText = "DELETE FROM USERDATA_DEFINITIONS WHERE _USERDEFID = @nUserDataID";

                        using (SqlCeCommand command = new SqlCeCommand(commandText, conn))
                        {
                            command.Parameters.AddWithValue("@nUserDataID", aUserDefID);
                            command.ExecuteNonQuery();
                        }
                    }

                    success = true;
                }
                else
                {
                    using (SqlConnection conn = DatabaseConnection.CreateOpenStandardConnection())
                    {
                        string commandText = "DELETE FROM USERDATA_VALUES WHERE _USERDEFID = @nUserDataID";

                        using (SqlCommand command = new SqlCommand(commandText, conn))
                        {
                            command.Parameters.AddWithValue("@nUserDataID", aUserDefID);
                            command.ExecuteNonQuery();
                        }

                        commandText = "DELETE FROM USERDATA_DEFINITIONS WHERE _USERDEFID = @nUserDataID";

                        using (SqlCommand command = new SqlCommand(commandText, conn))
                        {
                            command.Parameters.AddWithValue("@nUserDataID", aUserDefID);
                            command.ExecuteNonQuery();
                        }
                    }

                    success = true;
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

            return success;
        }

        /// <summary>
        /// Delete the specified User Data Category / Field
        /// </summary>
        /// <param name="aUserDefID"></param>
        /// <returns></returns>
        public bool UserDataDefinitionCategoryDelete(int aUserDefID)
        {
            if (isDebugEnabled) logger.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " in");

            bool success = false;

            try
            {
                string cmdText = "DELETE FROM USERDATA_DEFINITIONS WHERE _USERDEFID = @nUserDataID";

                if (compactDatabaseType)
                {
                    using (SqlCeConnection conn = DatabaseConnection.CreateOpenCEConnection())
                    {
                        using (SqlCeCommand command = new SqlCeCommand(cmdText, conn))
                        {
                            command.Parameters.AddWithValue("@nUserDataID", aUserDefID);
                            command.ExecuteNonQuery();
                        }
                    }

                    success = true;
                }
                else
                {
                    using (SqlConnection conn = DatabaseConnection.CreateOpenStandardConnection())
                    {
                        using (SqlCommand command = new SqlCommand(cmdText, conn))
                        {
                            command.Parameters.AddWithValue("@nUserDataID", aUserDefID);
                            command.ExecuteNonQuery();
                        }
                    }

                    success = true;
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
            return success;
        }



        /// <summary>
        /// Return a table containing all of the values for the specified User Defined Data Category
        /// for the specified item
        /// </summary>
        /// <returns></returns>
        public DataTable EnumerateUserDataValues(UserDataCategory.SCOPE aScope, int aParentID, int aCategoryID)
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
                               "SELECT _VALUE ,_USERDEFID " +
                               "FROM USERDATA_VALUES " +
                               "WHERE _PARENTTYPE = @nParentType " +
                               "AND _PARENTID = @nParentID " +
                               "AND _USERDEFID IN " +
                               "(SELECT _USERDEFID FROM USERDATA_DEFINITIONS WHERE _PARENTID = @nCategoryID)";

                        SqlCeParameter[] spParams = new SqlCeParameter[3];
                        spParams[0] = new SqlCeParameter("@nParentType", (int)aScope);
                        spParams[1] = new SqlCeParameter("@nParentID", aParentID);
                        spParams[2] = new SqlCeParameter("@nCategoryID", aCategoryID);

                        using (SqlCeCommand command = new SqlCeCommand(commandText, conn))
                        {
                            command.Parameters.AddRange(spParams);
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
                table = lAuditWizardDataAccess.EnumerateUserDataValues(aScope, aParentID, aCategoryID);
            }

            if (isDebugEnabled) logger.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " out");
            return table;
        }



        /// <summary>
        /// Delete the specified User Data Category / Field from the database
        /// </summary>
        /// <returns></returns>
        public int UserDataUpdateValue(UserDataCategory.SCOPE aScope, int aParentID, int aFieldID, string aValue)
        {
            if (isDebugEnabled) logger.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " in");

            int returnID = 0;

            if (compactDatabaseType)
            {
                int lUserValueId = -1;

                try
                {
                    using (SqlCeConnection conn = DatabaseConnection.CreateOpenCEConnection())
                    {
                        string commandText =
                               "SELECT min(_USERVALUEID) FROM USERDATA_VALUES " +
                               "WHERE _PARENTTYPE = @nScope " +
                               "AND   _PARENTID = @nParentID " +
                               "AND	  _USERDEFID = @nUserDefID";

                        SqlCeParameter[] spParams = new SqlCeParameter[4];
                        spParams[0] = new SqlCeParameter("@nScope", (int)aScope);
                        spParams[1] = new SqlCeParameter("@nParentID", aParentID);
                        spParams[2] = new SqlCeParameter("@nUserDefID", aFieldID);
                        spParams[3] = new SqlCeParameter("@cValue", aValue);

                        using (SqlCeCommand command = new SqlCeCommand(commandText, conn))
                        {
                            command.Parameters.AddRange(spParams);

                            object result = command.ExecuteScalar();

                            if ((result != null) && (result.GetType() != typeof(DBNull)))
                                lUserValueId = (int)result;
                        }

                        if (lUserValueId == -1)
                        {
                            commandText =
                                "INSERT INTO USERDATA_VALUES " +
                                "(_PARENTTYPE, _PARENTID, _USERDEFID, _VALUE) " +
                                "VALUES " +
                                "(@nScope, @nParentID, @nUserDefID, @cValue)";

                            using (SqlCeCommand command = new SqlCeCommand(commandText, conn))
                            {
                                SqlCeParameter[] spParams1 = new SqlCeParameter[4];
                                spParams1[0] = new SqlCeParameter("@nScope", (int)aScope);
                                spParams1[1] = new SqlCeParameter("@nParentID", aParentID);
                                spParams1[2] = new SqlCeParameter("@nUserDefID", aFieldID);
                                spParams1[3] = new SqlCeParameter("@cValue", aValue);

                                command.Parameters.AddRange(spParams1);
                                command.ExecuteNonQuery();
                            }
                        }
                        else
                        {
                            commandText =
                                "UPDATE USERDATA_VALUES SET _VALUE = @cValue WHERE _USERVALUEID = @nReturnId";

                            using (SqlCeCommand command = new SqlCeCommand(commandText, conn))
                            {
                                command.Parameters.AddWithValue("@cValue", aValue);
                                command.Parameters.AddWithValue("@nReturnId", lUserValueId);

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
                returnID = lAuditWizardDataAccess.UserDataUpdateValue(aScope, aParentID, aFieldID, aValue);
            }

            if (isDebugEnabled) logger.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " out");
            return returnID;
        }


        /// <summary>
        /// Return a table containing all of the values for the specified User Defined Data Field
        /// </summary>
        /// <returns></returns>
        public DataTable EnumerateUserDataFieldValues(int aFieldID)
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
                               "SELECT _PARENTID, _VALUE " +
                               "FROM USERDATA_VALUES " +
                               "WHERE _USERDEFID = @nFieldID";

                        SqlCeParameter[] spParams = new SqlCeParameter[1];
                        spParams[0] = new SqlCeParameter("@nFieldID", (int)aFieldID);

                        using (SqlCeCommand command = new SqlCeCommand(commandText, conn))
                        {
                            command.Parameters.AddRange(spParams);
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
                table = lAuditWizardDataAccess.EnumerateUserDataFieldValues(aFieldID);
            }

            if (isDebugEnabled) logger.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " out");
            return table;
        }

        public string GetValueForItem(int itemId, int userDefId)
        {
            string returnValue = "<no value>";
            string cmdText = "SELECT _VALUE FROM USERDATA_VALUES WHERE _USERDEFID = @userDefId AND _PARENTID = @parentId";

            try
            {
                if (compactDatabaseType)
                {
                    using (SqlCeConnection conn = DatabaseConnection.CreateOpenCEConnection())
                    {
                        using (SqlCeCommand command = new SqlCeCommand(cmdText, conn))
                        {
                            command.Parameters.AddWithValue("@userDefId", userDefId);
                            command.Parameters.AddWithValue("@parentId", itemId);
                            returnValue = command.ExecuteScalar().ToString();
                        }
                    }
                }
                else
                {
                    using (SqlConnection conn = DatabaseConnection.CreateOpenStandardConnection())
                    {
                        using (SqlCommand command = new SqlCommand(cmdText, conn))
                        {
                            command.Parameters.AddWithValue("@userDefId", userDefId);
                            command.Parameters.AddWithValue("@parentId", itemId);
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

        private int PerformScalarQueryInt(string commandText)
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

        public DataTable GetCompliantUserData(string userDataName, string parentDefinition, string userDataValue)
        {
            string commandText = String.Format(
                "select uv._parentid " +
                "from userdata_definitions ud " +
                "left join userdata_values uv on (uv._userdefid = ud._userdefid) " +
                "where ud._name = '{0}' " +
                "and ud._parentid in (select _userdefid from userdata_definitions where _name = '{1}') " +
                "and _value = {2}", userDataName, parentDefinition, userDataValue);

            return PerformQuery(commandText);
        }

        public DataTable GetUserDataPickerValues(string userDataName, string parentDefinition)
        {
            string commandText = String.Format(
                "select distinct uv._value " +
                "from userdata_definitions ud " +
                "left join userdata_values uv on (uv._userdefid = ud._userdefid) " +
                "where ud._name = '{0}' " +
                "and ud._parentid in (select _userdefid from userdata_definitions where _name = '{1}') " +
                "order by uv._value", userDataName, parentDefinition);

            return PerformQuery(commandText);
        }

        public DataTable GetParentUserDataCategories()
        {
            string commandText =
                "SELECT _NAME " +
                "FROM USERDATA_DEFINITIONS " +
                "WHERE _PARENTID IS NULL";

            return PerformQuery(commandText);
        }

        public DataTable GetUserDataNamesByParent(string parentName)
        {
            string commandText = String.Format(
                "select 'UserData|' + '{0}' + '|' + _name " +
                "FROM USERDATA_DEFINITIONS " +
                "WHERE _parentid IN (SELECT _USERDEFID FROM USERDATA_DEFINITIONS WHERE _NAME = '{0}')", parentName);

            return PerformQuery(commandText);
        }

        public string GetCompliantUserDataValueByAssetId(string userDataName, string parentDefinition, string assetId)
        {
            string commandText = String.Format(
                "select uv._value " +
                "from userdata_definitions ud " +
                "left join userdata_values uv on (uv._userdefid = ud._userdefid) " +
                "where ud._name = '{0}' " +
                "and ud._parentid in (select _userdefid from userdata_definitions where _name = '{1}') " +
                "and uv._parentid = {2}", userDataName, parentDefinition, assetId);

            return PerformScalarQuery(commandText);
        }

        public int GetCountUserDataCategories(UserDataCategory.SCOPE aScope)
        {
            if (isDebugEnabled) logger.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " in");

            int count = 0;

            string cmdText;

            if (aScope == UserDataCategory.SCOPE.Any)
            {
                cmdText =
                    "SELECT COUNT(_USERDEFID) " +
                    "FROM USERDATA_DEFINITIONS " +
                    "WHERE _PARENTID IS NULL";
            }
            else if (aScope == UserDataCategory.SCOPE.Application)
            {
                cmdText =
                    "SELECT COUNT(_USERDEFID) " +
                    "FROM USERDATA_DEFINITIONS " +
                    "WHERE _PARENTID IS NULL " +
                    "AND _SCOPE = 1";
            }
            else
            {
                cmdText =
                    "SELECT COUNT(_USERDEFID) " +
                    "FROM USERDATA_DEFINITIONS " +
                    "WHERE _PARENTID IS NULL " +
                    "AND (_SCOPE = 0 OR _SCOPE = 2)";
            }

            try
            {
                if (compactDatabaseType)
                {
                    using (SqlCeConnection conn = DatabaseConnection.CreateOpenCEConnection())
                    {
                        using (SqlCeCommand command = new SqlCeCommand(cmdText, conn))
                        {
                            count = Convert.ToInt32(command.ExecuteScalar());
                        }
                    }
                }
                else
                {
                    using (SqlConnection conn = DatabaseConnection.CreateOpenStandardConnection())
                    {
                        using (SqlCommand command = new SqlCommand(cmdText, conn))
                        {
                            count = Convert.ToInt32(command.ExecuteScalar());
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
            return count;
        }

        public int GetAssignedUserId()
        {
            string cmdText =
                "SELECT ud1._USERDEFID " +
                "FROM USERDATA_DEFINITIONS ud1 " +
                "INNER JOIN USERDATA_DEFINITIONS ud2 ON (ud1._PARENTID = ud2._USERDEFID) " +
                "WHERE ud1._NAME = 'Assigned User' " +
                "AND ud2._NAME = 'Asset Management'";

            return PerformScalarQueryInt(cmdText);
        }

        public bool AssignedUserPopulated(int parentId, int userDefId)
        {
            bool alreadyAssigned = false;

            string cmdText =
                "SELECT COUNT(_USERVALUEID) " +
                "FROM USERDATA_VALUES " +
                "WHERE _PARENTID = @nParentId " +
                "AND _USERDEFID = @nUserDefId " +
                "AND _VALUE <> ''";

            try
            {
                if (compactDatabaseType)
                {
                    using (SqlCeConnection conn = DatabaseConnection.CreateOpenCEConnection())
                    {
                        using (SqlCeCommand command = new SqlCeCommand(cmdText, conn))
                        {
                            command.Parameters.AddWithValue("@nParentId", parentId);
                            command.Parameters.AddWithValue("@nUserDefId", userDefId);
                            alreadyAssigned = ((int)command.ExecuteScalar() == 1);
                        }
                    }
                }
                else
                {
                    using (SqlConnection conn = DatabaseConnection.CreateOpenStandardConnection())
                    {
                        using (SqlCommand command = new SqlCommand(cmdText, conn))
                        {
                            command.Parameters.AddWithValue("@nParentId", parentId);
                            command.Parameters.AddWithValue("@nUserDefId", userDefId);
                            alreadyAssigned = ((int)command.ExecuteScalar() == 1);
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

            return alreadyAssigned;
        }

        #endregion User Defined data Definitions
    }
}
