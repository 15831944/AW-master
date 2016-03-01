using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlServerCe;
using System.IO;
using System.Windows.Forms;

namespace Layton.AuditWizard.DataAccess
{
    class DocumentsDAO
    {
        #region Data

        private string connectionStringCompact = null;
        protected static readonly log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private static readonly bool isDebugEnabled = logger.IsDebugEnabled;
        private bool compactDatabaseType = false;
        private Configuration config = ConfigurationManager.OpenExeConfiguration(Path.Combine(Application.StartupPath, "AuditWizardv8.exe"));

        #endregion

        public DocumentsDAO()
        {
            compactDatabaseType = Convert.ToBoolean(config.AppSettings.Settings["CompactDatabaseType"].Value);
            connectionStringCompact = "Data Source=" + Application.StartupPath + Convert.ToString(config.AppSettings.Settings["ConnectionStringCompact"].Value);
        }

        #region Documents Table

        /// <summary>
        /// Return a table containing all of the Documents defined for an asset
        /// </summary>
        /// <returns></returns>
        public DataTable EnumerateDocuments(Asset forAsset)
        {
            return EnumerateDocuments(SCOPE.Asset, forAsset.AssetID);
        }

        /// <summary>
        /// Return a table containing all of the Documents defined for an application
        /// </summary>
        /// <returns></returns>
        public DataTable EnumerateDocuments(InstalledApplication forApplication)
        {
            return EnumerateDocuments(SCOPE.Application, forApplication.ApplicationID);
        }

        /// <summary>
        /// Return a table containing all of the Documents defined for an application license
        /// </summary>
        /// <returns></returns>
        public DataTable EnumerateDocuments(ApplicationLicense aLicense)
        {
            return EnumerateDocuments(SCOPE.License, aLicense.LicenseID);
        }

        /// <summary>
        /// This function is the base function for returning documents for a specific item or the specified scope
        /// and database ID.  It is called from the public specific functions
        /// </summary>
        /// <param name="scope"></param>
        /// <param name="parentID"></param>
        /// <returns></returns>
        public DataTable EnumerateDocuments(SCOPE aScope, int aParentID)
        {
            if (isDebugEnabled) logger.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " in");

            DataTable table = new DataTable(TableNames.DOCUMENTS);

            if (compactDatabaseType)
            {
                try
                {
                    using (SqlCeConnection conn = DatabaseConnection.CreateOpenCEConnection())
                    {
                        string commandText =
                            "SELECT DOCUMENTS.* " +
                            "FROM DOCUMENTS " +
                            "WHERE _SCOPE = @nScope AND _PARENTID = @nParentID";

                        SqlCeParameter[] spParams = new SqlCeParameter[2];
                        spParams[0] = new SqlCeParameter("@nScope", (int)aScope);
                        spParams[1] = new SqlCeParameter("@nParentID", aParentID);

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
                table = lAuditWizardDataAccess.EnumerateDocuments(aScope, aParentID);
            }

            if (isDebugEnabled) logger.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " out");
            return table;
        }


        /// <summary>
        /// Add a new Document to the database
        /// </summary>
        /// <param name="document">The document to add</param>
        /// <returns></returns>
        public int DocumentAdd(Document aDocument)
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
                            "INSERT INTO DOCUMENTS " +
                            "(_SCOPE ,_NAME ,_PATH ,_PARENTID) " +
                            "VALUES " +
                            "(@nScope, @cName, @cPath, @nParentID)";

                        SqlCeParameter[] spParams = new SqlCeParameter[4];
                        spParams[0] = new SqlCeParameter("@nScope", (int)aDocument.Scope);
                        spParams[1] = new SqlCeParameter("@nParentID", aDocument.ParentID);
                        spParams[2] = new SqlCeParameter("@cName", aDocument.Name);
                        spParams[3] = new SqlCeParameter("@cPath", aDocument.Path);

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
                lItemID = lAuditWizardDataAccess.DocumentAdd(aDocument);
            }

            if (isDebugEnabled) logger.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " out with id : " + lItemID);
            return lItemID;
        }


        /// <summary>
        /// Update the specified Document
        /// </summary>
        /// <param name="theAsset"></param>
        /// <returns></returns>
        public int DocumentUpdate(Document aDocument)
        {
            if (isDebugEnabled) logger.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " in");

            try
            {
                string cmdText =
                    "UPDATE DOCUMENTS " +
                    "SET _NAME = @cName, _PATH = @cPath, _SCOPE = @cScope, _PARENTID = @nParentId " +
                    "WHERE _DOCUMENTID = @nDocumentID";

                if (compactDatabaseType)
                {
                    using (SqlCeConnection conn = DatabaseConnection.CreateOpenCEConnection())
                    {
                        SqlCeParameter[] spParams = new SqlCeParameter[5];
                        spParams[0] = new SqlCeParameter("@nDocumentID", aDocument.DocumentID);
                        spParams[1] = new SqlCeParameter("@cPath", aDocument.Path);
                        spParams[2] = new SqlCeParameter("@cName", aDocument.Name);
                        spParams[3] = new SqlCeParameter("@cScope", (int)aDocument.Scope);
                        spParams[4] = new SqlCeParameter("@nParentId", aDocument.ParentID);

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
                        SqlParameter[] spParams = new SqlParameter[5];
                        spParams[0] = new SqlParameter("@nDocumentID", aDocument.DocumentID);
                        spParams[1] = new SqlParameter("@cPath", aDocument.Path);
                        spParams[2] = new SqlParameter("@cName", aDocument.Name);
                        spParams[3] = new SqlParameter("@cScope", (int)aDocument.Scope);
                        spParams[4] = new SqlParameter("@nParentId", aDocument.ParentID);

                        using (SqlCommand command = new SqlCommand(cmdText, conn))
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
        /// Delete the specified Document from the database
        /// </summary>
        /// <returns></returns>
        public int DocumentDelete(Document aDocument)
        {
            if (isDebugEnabled) logger.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " in");

            if (compactDatabaseType)
            {
                try
                {
                    using (SqlCeConnection conn = DatabaseConnection.CreateOpenCEConnection())
                    {
                        string commandText =
                            "DELETE FROM DOCUMENTS WHERE _DOCUMENTID = @nDocumentID";

                        SqlCeParameter[] spParams = new SqlCeParameter[1];
                        spParams[0] = new SqlCeParameter("@nDocumentID", aDocument.DocumentID);

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
                lAuditWizardDataAccess.DocumentDelete(aDocument);
            }

            if (isDebugEnabled) logger.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " out");
            return 0;
        }

        #endregion Documents Table
    }
}
