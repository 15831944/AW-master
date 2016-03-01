using System;
using System.Configuration;
using System.Data;
using System.Data.SqlServerCe;
using System.Data.SqlClient;
using System.IO;
using System.Windows.Forms;

namespace Layton.AuditWizard.DataAccess
{
    public class SuppliersDAO
    {
        #region Data

        private string connectionStringCompact = null;
        protected static readonly log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private static readonly bool isDebugEnabled = logger.IsDebugEnabled;
        private bool compactDatabaseType = false;
        private Configuration config = ConfigurationManager.OpenExeConfiguration(Path.Combine(Application.StartupPath, "AuditWizardv8.exe"));

        #endregion

        public SuppliersDAO()
        {
            compactDatabaseType = Convert.ToBoolean(config.AppSettings.Settings["CompactDatabaseType"].Value);
            connectionStringCompact = "Data Source=" + Application.StartupPath + Convert.ToString(config.AppSettings.Settings["ConnectionStringCompact"].Value);
        }

        #region Suppliers Table

        /// <summary>
        /// Return a table containing a single supplier
        /// </summary>
        /// <returns></returns>
        public DataTable SelectSupplierByID(int aSupplierID)
        {
            if (isDebugEnabled) logger.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " in");

            DataTable supplierTable = new DataTable(TableNames.ALERTS);

            try
            {
                string commandText =
                           "SELECT _SUPPLIERID, _NAME ,_ADDRESS1 ,_ADDRESS2 ,_CITY, _STATE ,_ZIP ,_TELEPHONE " +
                           ",_CONTACT_NAME ,_CONTACT_EMAIL, _WWW ,_FAX ,_NOTES " +
                           "FROM SUPPLIERS " +
                           "WHERE _SUPPLIERID = " + aSupplierID +
                           " ORDER BY _SUPPLIERID";

                if (compactDatabaseType)
                {
                    using (SqlCeConnection conn = DatabaseConnection.CreateOpenCEConnection())
                    {
                        using (SqlCeCommand command = new SqlCeCommand(commandText, conn))
                        {
                            new SqlCeDataAdapter(command).Fill(supplierTable);
                        }
                    }
                }
                else
                {
                    using (SqlConnection conn = DatabaseConnection.CreateOpenStandardConnection())
                    {
                        using (SqlCommand command = new SqlCommand(commandText, conn))
                        {
                            new SqlDataAdapter(command).Fill(supplierTable);
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
            return supplierTable;
        }

        /// <summary>
        /// Return a table containing all suppliers
        /// </summary>
        /// <returns></returns>
        public DataTable EnumerateSuppliers()
        {
            if (isDebugEnabled) logger.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " in");

            DataTable supplierTable = new DataTable(TableNames.ALERTS);

            if (compactDatabaseType)
            {
                try
                {
                    using (SqlCeConnection conn = DatabaseConnection.CreateOpenCEConnection())
                    {
                        string commandText =
                               "SELECT _SUPPLIERID, _NAME ,_ADDRESS1 ,_ADDRESS2 ,_CITY, _STATE ,_ZIP ,_TELEPHONE " +
                               ",_CONTACT_NAME ,_CONTACT_EMAIL, _WWW ,_FAX ,_NOTES " +
                               "FROM SUPPLIERS " +
                               "ORDER BY _SUPPLIERID";

                        using (SqlCeCommand command = new SqlCeCommand(commandText, conn))
                        {
                            new SqlCeDataAdapter(command).Fill(supplierTable);
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
                supplierTable = lAuditWizardDataAccess.EnumerateSuppliers();
            }

            if (isDebugEnabled) logger.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " out");
            return supplierTable;
        }


        /// <summary>
        /// Add a new Supplier to the database
        /// </summary>
        /// <param name="theAlert"></param>
        /// <returns></returns>
        public int SupplierAdd(Supplier aSupplier)
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
                               "INSERT INTO SUPPLIERS (_NAME, _ADDRESS1, _ADDRESS2, _CITY, _STATE, _ZIP, _TELEPHONE, " +
                               "_CONTACT_NAME, _CONTACT_EMAIL, _WWW, _FAX ,_NOTES) " +
                               " VALUES (@cName, @cAddress1, @cAddress2, @cCity, @cState, @cZip, @cTelephone, @cContactName, " +
                               "@cContactEmail, @cWWW, @cFax, @cNotes)";

                        SqlCeParameter[] spParams = new SqlCeParameter[12];
                        spParams[0] = new SqlCeParameter("@cName", aSupplier.Name);
                        spParams[1] = new SqlCeParameter("@cAddress1", aSupplier.AddressLine1);
                        spParams[2] = new SqlCeParameter("@cAddress2", aSupplier.AddressLine2);
                        spParams[3] = new SqlCeParameter("@cCity", aSupplier.City);
                        spParams[4] = new SqlCeParameter("@cState", aSupplier.State);
                        spParams[5] = new SqlCeParameter("@cZip", aSupplier.Zip);
                        spParams[6] = new SqlCeParameter("@cTelephone", aSupplier.Telephone);
                        spParams[7] = new SqlCeParameter("@cContactName", aSupplier.Contact);
                        spParams[8] = new SqlCeParameter("@cContactEmail", aSupplier.Email);
                        spParams[9] = new SqlCeParameter("@cWWW", aSupplier.WWW);
                        spParams[10] = new SqlCeParameter("@cFax", aSupplier.Fax);
                        spParams[11] = new SqlCeParameter("@cNotes", aSupplier.Notes);

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
                lItemID = lAuditWizardDataAccess.SupplierAdd(aSupplier);
            }

            if (isDebugEnabled) logger.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " out");
            return lItemID;
        }


        /// <summary>
        /// Update the Supplier Information
        /// </summary>
        /// <param name="theAsset"></param>
        /// <returns></returns>
        public int SupplierUpdate(Supplier aSupplier)
        {
            if (isDebugEnabled) logger.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " in");

            if (compactDatabaseType)
            {
                try
                {
                    using (SqlCeConnection conn = DatabaseConnection.CreateOpenCEConnection())
                    {
                        string commandText =
                               "UPDATE SUPPLIERS SET " +
                                "_NAME = @cName " +
                                ",_ADDRESS1 = @cAddress1 " +
                                ",_ADDRESS2 = @cAddress2 " +
                                ",_CITY = @cCity " +
                                ",_STATE = @cState " +
                                ",_ZIP = @cZip " +
                                ",_TELEPHONE = @cTelephone " +
                                ",_CONTACT_NAME = @cContactName " +
                                ",_CONTACT_EMAIL = @cContactEmail " +
                                ",_WWW = @cWWW " +
                                ",_FAX = @cFax " +
                                ",_NOTES = @cNotes " +
                                "WHERE _SUPPLIERID = @nSupplierID";

                        SqlCeParameter[] spParams = new SqlCeParameter[13];
                        spParams[0] = new SqlCeParameter("@nSupplierID", aSupplier.SupplierID);
                        spParams[1] = new SqlCeParameter("@cName", aSupplier.Name);
                        spParams[2] = new SqlCeParameter("@cAddress1", aSupplier.AddressLine1);
                        spParams[3] = new SqlCeParameter("@cAddress2", aSupplier.AddressLine2);
                        spParams[4] = new SqlCeParameter("@cCity", aSupplier.City);
                        spParams[5] = new SqlCeParameter("@cState", aSupplier.State);
                        spParams[6] = new SqlCeParameter("@cZip", aSupplier.Zip);
                        spParams[7] = new SqlCeParameter("@cTelephone", aSupplier.Telephone);
                        spParams[8] = new SqlCeParameter("@cContactName", aSupplier.Contact);
                        spParams[9] = new SqlCeParameter("@cContactEmail", aSupplier.Email);
                        spParams[10] = new SqlCeParameter("@cWWW", aSupplier.WWW);
                        spParams[11] = new SqlCeParameter("@cFax", aSupplier.Fax);
                        spParams[12] = new SqlCeParameter("@cNotes", aSupplier.Notes);

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
                lAuditWizardDataAccess.SupplierUpdate(aSupplier);
            }

            if (isDebugEnabled) logger.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " out");
            return 0;
        }



        /// <summary>
        /// Delete the specified Supplier from the database
        /// </summary>
        /// <param name="supplierID"></param>
        /// <returns></returns>
        public int SupplierDelete(Supplier aSupplier)
        {
            if (isDebugEnabled) logger.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " in");

            if (compactDatabaseType)
            {
                if (aSupplier.SupplierID != 0)
                {
                    SqlCeTransaction transaction = null;

                    using (SqlCeConnection conn = DatabaseConnection.CreateOpenCEConnection())
                    {
                        try
                        {
                            transaction = conn.BeginTransaction();

                            string commandText =
                               "UPDATE LICENSES SET _SUPPLIERID = 1 WHERE _SUPPLIERID = @nSupplierID";

                            SqlCeParameter[] spParams = new SqlCeParameter[1];
                            spParams[0] = new SqlCeParameter("@nSupplierID", aSupplier.SupplierID);

                            using (SqlCeCommand command = new SqlCeCommand(commandText, conn))
                            {
                                command.Parameters.AddRange(spParams);
                                command.ExecuteNonQuery();
                            }

                            commandText =
                                "DELETE FROM SUPPLIERS WHERE _SUPPLIERID = @nSupplierID";

                            using (SqlCeCommand command = new SqlCeCommand(commandText, conn))
                            {
                                command.Parameters.AddWithValue("@nSupplierID", aSupplier.SupplierID);
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
                lAuditWizardDataAccess.SupplierDelete(aSupplier);
            }

            if (isDebugEnabled) logger.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " out");
            return 0;
        }


        /// <summary>
        /// Return the database index of the specified Supplier Record
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public int SupplierFind(string aName)
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
                            "SELECT _SUPPLIERID FROM SUPPLIERS WHERE _name = @cName";

                        using (SqlCeCommand commandReturnValue = new SqlCeCommand(commandText, conn))
                        {
                            commandReturnValue.Parameters.AddWithValue("@cName", aName);
                            object result = commandReturnValue.ExecuteScalar();

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
                lItemId = lAuditWizardDataAccess.SupplierFind(aName);
            }

            if (isDebugEnabled) logger.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " out");
            return lItemId;
        }

        #endregion Suppliers Table
    }
}
