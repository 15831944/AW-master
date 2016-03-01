using System;
using System.Configuration;
using System.Data;
using System.Data.SqlServerCe;
using System.IO;
using System.Windows.Forms;

namespace Layton.AuditWizard.DataAccess
{
    public class LicenseTypesDAO
    {
        #region Data

        private string connectionStringCompact = null;
        protected static readonly log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private static readonly bool isDebugEnabled = logger.IsDebugEnabled;
        private bool compactDatabaseType = false;
        private Configuration config = ConfigurationManager.OpenExeConfiguration(Path.Combine(Application.StartupPath, "AuditWizardv8.exe"));

        #endregion

        public LicenseTypesDAO()
        {
            compactDatabaseType = Convert.ToBoolean(config.AppSettings.Settings["CompactDatabaseType"].Value);
            connectionStringCompact = "Data Source=" + Application.StartupPath + Convert.ToString(config.AppSettings.Settings["ConnectionStringCompact"].Value);
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

        #region License_Types Table
        /// <summary>
        /// Add a new License Type to the database
        /// </summary>
        /// <param name="theLicenseType"></param>
        /// <returns></returns>
        public int LicenseTypeAdd(LicenseType theLicenseType)
        {
            if (isDebugEnabled) logger.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " in");
            int lItemID = 0;

            if (compactDatabaseType)
            {
                try
                {
                    string commandText =
                        "INSERT INTO LICENSE_TYPES (_NAME, _COUNTED) " +
                        "VALUES (@cName, @bPerPC)";

                    using (SqlCeConnection conn = DatabaseConnection.CreateOpenCEConnection())
                    {
                        SqlCeParameter[] spParams = new SqlCeParameter[2];
                        spParams[0] = new SqlCeParameter("@cName", theLicenseType.Name);
                        spParams[1] = new SqlCeParameter("@bPerPC", theLicenseType.PerComputer);

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
                lItemID = lAuditWizardDataAccess.LicenseTypeAdd(theLicenseType);
            }

            if (isDebugEnabled) logger.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " out");
            return lItemID;
        }


        /// <summary>
        /// Update the definition stored for the specified LicenseType
        /// </summary>
        /// <param name="theAsset"></param>
        /// <returns></returns>
        public int LicenseTypeUpdate(LicenseType aLicenseType)
        {
            if (isDebugEnabled) logger.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " in with license type id : " + aLicenseType.LicenseTypeID);

            if (compactDatabaseType)
            {
                try
                {
                    using (SqlCeConnection conn = DatabaseConnection.CreateOpenCEConnection())
                    {
                        string commandText =
                            "UPDATE LICENSE_TYPES SET _COUNTED = @bPerPC " +
                            "WHERE _LICENSETYPEID = @nLicenseTypeID";

                        using (SqlCeCommand command = new SqlCeCommand(commandText, conn))
                        {
                            command.Parameters.AddWithValue("@bPerPC", aLicenseType.PerComputer);
                            command.Parameters.AddWithValue("@nLicenseTypeID", aLicenseType.LicenseTypeID);

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
                lAuditWizardDataAccess.LicenseTypeUpdate(aLicenseType);
            }

            if (isDebugEnabled) logger.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " out");
            return 0;
        }


        /// <summary>
        /// Return a table containing all of the license types that have been declared 
        /// </summary>
        /// <returns></returns>
        public DataTable EnumerateLicenseTypes()
        {
            if (isDebugEnabled) logger.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " in");
            DataTable licensetypeTable = new DataTable(TableNames.LICENSETYPES);

            if (compactDatabaseType)
            {
                try
                {
                    using (SqlCeConnection conn = DatabaseConnection.CreateOpenCEConnection())
                    {
                        string commandText =
                            "SELECT * FROM LICENSE_TYPES";

                        using (SqlCeCommand command = new SqlCeCommand(commandText, conn))
                        {
                            new SqlCeDataAdapter(command).Fill(licensetypeTable);
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
                licensetypeTable = lAuditWizardDataAccess.EnumerateLicenseTypes();
            }

            if (isDebugEnabled) logger.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " out");
            return licensetypeTable;
        }


        /// <summary>
        /// Delete the specified License TYPE from the database
        /// </summary>
        /// <param name="licenseTypeID"></param>
        /// <returns></returns>
        public int LicenseTypeDelete(LicenseType aLicenseType)
        {
            if (isDebugEnabled) logger.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " in");

            if (compactDatabaseType)
            {
                if (aLicenseType.LicenseTypeID != 0)
                {
                    SqlCeTransaction transaction = null;

                    using (SqlCeConnection conn = DatabaseConnection.CreateOpenCEConnection())
                    {
                        try
                        {
                            transaction = conn.BeginTransaction();

                            string commandText =
                            "DELETE FROM LICENSES WHERE _LICENSETYPEID = @nLicenseTypeID";

                            using (SqlCeCommand command = new SqlCeCommand(commandText, conn))
                            {
                                command.Parameters.AddWithValue("@nLicenseTypeID", aLicenseType.LicenseTypeID);
                                command.ExecuteNonQuery();
                            }

                            commandText =
                            "DELETE FROM LICENSE_TYPES WHERE _LICENSETYPEID = @nLicenseTypeID";

                            using (SqlCeCommand command = new SqlCeCommand(commandText, conn))
                            {
                                command.Parameters.AddWithValue("@nLicenseTypeID", aLicenseType.LicenseTypeID);
                                command.ExecuteNonQuery();
                            }

                            transaction.Commit();
                        }
                        catch (SqlCeException ex)
                        {
                            transaction.Rollback();

                            Utility.DisplayErrorMessage("A database error has occurred in AuditWizard." + Environment.NewLine + Environment.NewLine +
                                    "Please see the log file for further details.");
                            logger.Error("Exception in " + System.Reflection.MethodBase.GetCurrentMethod().Name, ex);
                        }
                        catch (Exception ex)
                        {
                            transaction.Rollback();

                            Utility.DisplayErrorMessage("A database error has occurred in AuditWizard." + Environment.NewLine + Environment.NewLine +
                                    "Please see the log file for further details.");

                            logger.Error("Exception in " + System.Reflection.MethodBase.GetCurrentMethod().Name, ex);
                        }
                    }
                }
            }
            else
            {
                AuditWizardDataAccess lAuditWizardDataAccess = new AuditWizardDataAccess();
                lAuditWizardDataAccess.LicenseTypeDelete(aLicenseType);
            }

            if (isDebugEnabled) logger.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " out");
            return 0;
        }




        /// <summary>
        /// Return the database index of the specified LicenseType
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public int LicenseTypeFind(string aName)
        {
            if (isDebugEnabled) logger.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " in");
            int lItemId = 0;

            if (compactDatabaseType)
            {
                try
                {
                    using (SqlCeConnection conn = DatabaseConnection.CreateOpenCEConnection())
                    {
                        string commandText =
                            "SELECT _LICENSETYPEID FROM LICENSE_TYPES WHERE _name = @cName";

                        using (SqlCeCommand command = new SqlCeCommand(commandText, conn))
                        {
                            command.Parameters.AddWithValue("@cName", aName);

                            object result = command.ExecuteScalar();

                            if ((result != null) && (result.GetType() != typeof(DBNull)))
                                lItemId = Convert.ToInt32(result);
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
                lItemId = lAuditWizardDataAccess.LicenseTypeFind(aName);
            }

            if (isDebugEnabled) logger.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " out");
            return lItemId;
        }

        #endregion License_Types Table
    }
}
