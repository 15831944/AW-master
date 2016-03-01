using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlServerCe;
using System.IO;
using System.Windows.Forms;

namespace Layton.AuditWizard.DataAccess
{
    public class NotesDAO
    {
        #region Data

        private string connectionStringCompact = null;
        protected static readonly log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private static readonly bool isDebugEnabled = logger.IsDebugEnabled;
        private bool compactDatabaseType = false;
        private Configuration config = ConfigurationManager.OpenExeConfiguration(Path.Combine(Application.StartupPath, "AuditWizardv8.exe"));

        #endregion

        public NotesDAO()
        {
            compactDatabaseType = Convert.ToBoolean(config.AppSettings.Settings["CompactDatabaseType"].Value);
            connectionStringCompact = "Data Source=" + Application.StartupPath + Convert.ToString(config.AppSettings.Settings["ConnectionStringCompact"].Value);
        }

        #region Notes Table

        /// <summary>
        /// Return a table containing all of the Notes defined for an asset
        /// </summary>
        /// <returns></returns>
        public DataTable EnumerateNotes(Asset forAsset)
        {
            return EnumerateNotes(SCOPE.Asset, forAsset.AssetID);
        }


        /// <summary>
        /// This function is the base function for returning notes for a specific item or the specified scope
        /// and database ID.  It is called from the public specific functions
        /// </summary>
        /// <param name="aScope"></param>
        /// <param name="aParentID"></param>
        /// <returns></returns>
        public DataTable EnumerateNotes(SCOPE aScope, int aParentID)
        {
            if (isDebugEnabled) logger.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " in");

            DataTable table = new DataTable(TableNames.NOTES);

            if (compactDatabaseType)
            {
                try
                {
                    using (SqlCeConnection conn = DatabaseConnection.CreateOpenCEConnection())
                    {
                        string commandText =
                            "SELECT NOTES.* " +
                            "FROM NOTES " +
                            "WHERE _SCOPE = @nScope AND _PARENTID = @nParentID " +
                            "ORDER BY _DATE DESC";

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
                table = lAuditWizardDataAccess.EnumerateNotes(aScope, aParentID);
            }

            if (isDebugEnabled) logger.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " out");
            return table;
        }

        public DataTable GetNotesNameAndIdByScopeAndParent(SCOPE aScope, int aParentID)
        {
            if (isDebugEnabled) logger.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " in");

            DataTable table = new DataTable(TableNames.NOTES);

            try
            {
                string cmdText =
                    "SELECT _NOTEID, _NAME " +
                    "FROM NOTES " +
                    "WHERE _SCOPE = @nScope AND _PARENTID = @nParentID " +
                    "ORDER BY _DATE";

                if (compactDatabaseType)
                {
                    using (SqlCeConnection conn = DatabaseConnection.CreateOpenCEConnection())
                    {
                        SqlCeParameter[] spParams = new SqlCeParameter[2];
                        spParams[0] = new SqlCeParameter("@nScope", (int)aScope);
                        spParams[1] = new SqlCeParameter("@nParentID", aParentID);

                        using (SqlCeCommand command = new SqlCeCommand(cmdText, conn))
                        {
                            command.Parameters.AddRange(spParams);
                            new SqlCeDataAdapter(command).Fill(table);
                        }
                    }
                }
                else
                {
                    using (SqlConnection conn = DatabaseConnection.CreateOpenStandardConnection())
                    {
                        SqlParameter[] spParams = new SqlParameter[2];
                        spParams[0] = new SqlParameter("@nScope", (int)aScope);
                        spParams[1] = new SqlParameter("@nParentID", aParentID);

                        using (SqlCommand command = new SqlCommand(cmdText, conn))
                        {
                            command.Parameters.AddRange(spParams);
                            new SqlDataAdapter(command).Fill(table);
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
            return table;
        }


        /// <summary>
        /// Add a new Note to the database
        /// </summary>
        /// <param name="aNote"></param>
        /// <returns></returns>
        public int NoteAdd(Note aNote)
        {
            if (isDebugEnabled) logger.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " in");

            int lItemId = 0;

            try
            {
                string cmdText =
                    "INSERT INTO NOTES " +
                    "(_DATE ,_SCOPE ,_PARENTID ,_TEXT ,_USER) " +
                    "VALUES " +
                    "(@dtDate, @nScope, @nParentID, @cText, @cUser)";

                if (compactDatabaseType)
                {
                    using (SqlCeConnection conn = DatabaseConnection.CreateOpenCEConnection())
                    {
                        SqlCeParameter[] spParams = new SqlCeParameter[5];
                        spParams[0] = new SqlCeParameter("@dtDate", aNote.DateOfNote);
                        spParams[1] = new SqlCeParameter("@nScope", (int) aNote.Scope);
                        spParams[2] = new SqlCeParameter("@nParentID", aNote.ParentID);
                        spParams[3] = new SqlCeParameter("@cText", aNote.Text);
                        spParams[4] = new SqlCeParameter("@cUser", aNote.User);

                        using (SqlCeCommand command = new SqlCeCommand(cmdText, conn))
                        {
                            command.Parameters.AddRange(spParams);
                            command.ExecuteNonQuery();
                        }

                        using (SqlCeCommand command = new SqlCeCommand("SELECT @@IDENTITY", conn))
                        {
                            lItemId = Convert.ToInt32(command.ExecuteScalar());
                        }
                    }
                }
                else
                {
                    using (SqlConnection conn = DatabaseConnection.CreateOpenStandardConnection())
                    {
                        SqlParameter[] spParams = new SqlParameter[5];
                        spParams[0] = new SqlParameter("@dtDate", aNote.DateOfNote);
                        spParams[1] = new SqlParameter("@nScope", (int)aNote.Scope);
                        spParams[2] = new SqlParameter("@nParentID", aNote.ParentID);
                        spParams[3] = new SqlParameter("@cText", aNote.Text);
                        spParams[4] = new SqlParameter("@cUser", aNote.User);

                        using (SqlCommand command = new SqlCommand(cmdText, conn))
                        {
                            command.Parameters.AddRange(spParams);
                            command.ExecuteNonQuery();
                        }

                        using (SqlCommand command = new SqlCommand("SELECT @@IDENTITY", conn))
                        {
                            lItemId = Convert.ToInt32(command.ExecuteScalar());
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
            return lItemId;
        }


        /// <summary>
        /// Update the specified Note
        /// </summary>
        /// <param name="aNote"></param>
        /// <returns></returns>
        public int NoteUpdate(Note aNote)
        {
            if (isDebugEnabled) logger.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " in");

            try
            {
                string cmdText =
                    "UPDATE NOTES " +
                    "SET " +
                    "_DATE = @dDate, " +
                    "_TEXT = @cText, " +
                    "_PARENTID = @nParentId, " +
                    "_SCOPE = @cScope " +
                    "WHERE _NOTEID = @nNoteID";

                if (compactDatabaseType)
                {
                    using (SqlCeConnection conn = DatabaseConnection.CreateOpenCEConnection())
                    {
                        SqlCeParameter[] spParams = new SqlCeParameter[5];
                        spParams[0] = new SqlCeParameter("@nNoteID", aNote.NoteID);
                        spParams[1] = new SqlCeParameter("@dDate", DateTime.Now);
                        spParams[2] = new SqlCeParameter("@cText", aNote.Text);
                        spParams[3] = new SqlCeParameter("@cScope", (int)aNote.Scope);
                        spParams[4] = new SqlCeParameter("@nParentId", aNote.ParentID);

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
                        spParams[0] = new SqlParameter("@nNoteID", aNote.NoteID);
                        spParams[1] = new SqlParameter("@dDate", DateTime.Now);
                        spParams[2] = new SqlParameter("@cText", aNote.Text);
                        spParams[3] = new SqlParameter("@cScope", (int)aNote.Scope);
                        spParams[4] = new SqlParameter("@nParentId", aNote.ParentID);

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
        /// Delete the specified Note from the database
        /// </summary>
        /// <returns></returns>
        public int NoteDelete(Note aNote)
        {
            if (isDebugEnabled) logger.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " in");

            if (compactDatabaseType)
            {
                try
                {
                    using (SqlCeConnection conn = DatabaseConnection.CreateOpenCEConnection())
                    {
                        string commandText =
                            "DELETE FROM NOTES WHERE _NOTEID = @nNoteID";

                        SqlCeParameter[] spParams = new SqlCeParameter[1];
                        spParams[0] = new SqlCeParameter("@nNoteID", aNote.NoteID);

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
                lAuditWizardDataAccess.NoteDelete(aNote);
            }

            if (isDebugEnabled) logger.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " out");
            return 0;
        }

        #endregion Notes Table
    }
}
