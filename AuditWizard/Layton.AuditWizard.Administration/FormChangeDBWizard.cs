using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlServerCe;
using System.IO;
using System.Windows.Forms;
using Layton.AuditWizard.DataAccess;

namespace Layton.AuditWizard.Administration
{
    public partial class FormChangeDBWizard : Form
    {
        #region Data

        protected static readonly log4net.ILog Logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private static readonly bool IsDebugEnabled = Logger.IsDebugEnabled;
        string newExpressConnectionString = String.Empty;
        private Configuration config = ConfigurationManager.OpenExeConfiguration(Path.Combine(Application.StartupPath, "AuditWizardv8.exe"));
        private bool _compactDatabaseType = true;

        #endregion

        #region Constructor

        public FormChangeDBWizard()
        {
            InitializeComponent();
            ddlAuthType.SelectedItem = "Windows Authentication";
            rdCreateNewDB.Checked = true;

            _compactDatabaseType = Convert.ToBoolean(config.AppSettings.Settings["CompactDatabaseType"].Value);
        }

        #endregion

        #region Database Methods

        private SqlConnection CreateOpenConnection()
        {
            SqlConnection conn = null;

            try
            {
                conn = new SqlConnection(newExpressConnectionString);
                conn.Open();
            }
            catch (Exception ex)
            {
                Utility.DisplayApplicationErrorMessage(
                    "There was an error opening the connection to the database." + Environment.NewLine + Environment.NewLine +
                    "Please see the logfile for further details.");

                if (IsDebugEnabled) Logger.Debug("Attempted connection string : " + newExpressConnectionString);
                Logger.Error("Exception in " + System.Reflection.MethodBase.GetCurrentMethod().Name, ex);
            }

            return conn;
        }

        #endregion

        /// <summary>
        /// Builds the SQL Edition connection object
        /// </summary>
        /// <returns>true if successful</returns>
        private bool SetNewConnectionString()
        {
            Cursor = Cursors.WaitCursor;

            try
            {
                if (tbServerName.Text.Length == 0)
                {
                    this.Cursor = Cursors.Default;

                    MessageBox.Show("Please enter a value for the server name.",
                                    "AuditWizard", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return false;
                }

                if (rdCreateNewDB.Checked)
                {
                    SqlConnectionStringBuilder cb = new SqlConnectionStringBuilder();

                    // creating a new database
                    if (ddlAuthType.Text == "Windows Authentication")
                    {
                        cb = new SqlConnectionStringBuilder();
                        cb.DataSource = tbServerName.Text;
                        cb.IntegratedSecurity = true;
                    }
                    else
                    {
                        cb = new SqlConnectionStringBuilder();
                        cb.DataSource = tbServerName.Text;
                        cb.UserID = tbLogin.Text;
                        cb.Password = tbPassword.Text;
                        cb.IntegratedSecurity = false;
                    }

                    using (SqlConnection conn = new SqlConnection(cb.ToString()))
                    {
                        conn.Open();

                        using (SqlCommand command = new SqlCommand("SELECT COUNT(*) FROM master..sysdatabases WHERE name = '" + tbNewDBName.Text + "'", conn))
                        {
                            if (Convert.ToInt32(command.ExecuteScalar()) > 0)
                            {
                                this.Cursor = Cursors.Default;
                                MessageBox.Show("The database '" + tbNewDBName.Text + "' already exists. Please choose a different name.",
                                    "AuditWizard", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                return false;
                            }
                        }
                    }

                    newExpressConnectionString = cb.ToString();
                }
                else
                {
                    // connecting to an existing databse
                    SqlConnectionStringBuilder cb;

                    if (ddlAuthType.Text == "Windows Authentication")
                    {
                        cb = new SqlConnectionStringBuilder();
                        cb.DataSource = tbServerName.Text;
                        cb.InitialCatalog = ddlCatalog.Text;
                        cb.IntegratedSecurity = true;
                    }
                    else
                    {
                        cb = new SqlConnectionStringBuilder();
                        cb.DataSource = tbServerName.Text;
                        cb.InitialCatalog = ddlCatalog.Text;
                        cb.IntegratedSecurity = false;
                        cb.UserID = tbLogin.Text;
                        cb.Password = tbPassword.Text;
                    }

                    newExpressConnectionString = cb.ToString();
                }

                lbExistingMsg.Visible = true;
                rdMigrateData.Visible = true;
                rdNoMigrateData.Visible = true;
                rdNoMigrateData.Checked = true;
            }
            catch (SqlException ex)
            {
                Cursor = Cursors.Default;

                if (ex.Number == 53)
                {
                    MessageBox.Show("Unable to connect to server '" + tbServerName.Text + "'.",
                        "AuditWizard", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    Utility.DisplayApplicationErrorMessage(ex.Message);
                }

                Logger.Error("Exception in " + System.Reflection.MethodBase.GetCurrentMethod().Name, ex);

                return false;
            }
            catch (Exception ex)
            {
                Cursor = Cursors.Default;
                Logger.Error("Exception in " + System.Reflection.MethodBase.GetCurrentMethod().Name, ex);
                Utility.DisplayApplicationErrorMessage(ex.Message);

                return false;
            }

            Cursor = Cursors.Default;
            return true;
        }

        private bool MigrateAndCreateDB()
        {
            if (rdCreateNewDB.Checked)
            {
                // creating a new database
                if (!CreateNewDatabase())
                {
                    Utility.DisplayApplicationErrorMessage("An error has occurred whilst creating the database. The wizard will now close.");
                    Close();
                    return false;
                }
            }
            else
            {
                // - connecting to an existing database

                // - we need to update the existing ASSET table as the structure has changed
                // - also need to update the stored procedures
                using (SqlConnection conn = CreateOpenConnection())
                {
                    // - if currently connected to CE version this means we are about to connect to v8.0.0.0 db
                    // - in this case, we need to alter the structure of the target database slightly
                    // - if we are already using a SQL database then this process will have already happened and the db structure will be correct                    
                    UpdateAssetTableStructure(conn);

                    // add post-v8.000 tables
                    ExecuteDatabaseCreateScript(Application.StartupPath + @"\db\AddUpdatedTables.sql", conn);
                    ExecuteDatabaseCreateScript(Application.StartupPath + @"\db\AlterStoredProcedures.sql", conn);

                    // there are some SP's that are used by HelpBox that need to be added to the database
                    // if we are connecting to a >= 8.1.4 database then they will already exist but this script drops them first anyway
                    ExecuteDatabaseCreateScript(Application.StartupPath + @"\db\CreateHBStoredProcedures.sql", conn);
                    new SettingsDAO().SetSetting("HBSPAdded", true);
                }

                // if user wants to migrate data then do so
                if (rdMigrateData.Checked)
                {
                    progressBar1.Visible = true;
                    progressBar1.Minimum = 1;
                    progressBar1.Maximum = 26;
                    progressBar1.Step = 1;
                    lbProgressStage.Visible = true;

                    if (!UpdateExistingDatabase(false))
                    {
                        Utility.DisplayApplicationErrorMessage("An error has occurred whilst creating the database. The wizard will now close.");
                        Close();
                        return false;
                    }
                }
            }

            return true;
        }

        private bool CreateNewDatabase()
        {
            // initial setup of Cursor and progress bar
            Cursor = Cursors.WaitCursor;

            progressBar1.Visible = true;
            progressBar1.Minimum = 1;

            progressBar1.Maximum = rdMigrateData.Checked ? 31 : 4;

            progressBar1.Step = 1;
            lbProgressStage.Visible = true;

            // create the database
            if (!CreateDatabase())
            {
                Cursor = Cursors.Default;
                return false;
            }

            // create the tables, stored procs, constraints and add initial data
            if (!CreateDatabaseObjects())
            {
                Cursor = Cursors.Default;
                return false;
            }

            // if we need to migrate the existing data then copy data from CE to Express
            // NB this will delete target database first
            if (rdMigrateData.Checked)
            {
                if (!UpdateExistingDatabase(true))
                {
                    Cursor = Cursors.Default;
                    return false;
                }
            }

            Cursor = Cursors.Default;
            return true;
        }

        /// <summary>
        /// Create a new database
        /// </summary>
        /// <returns>true if successful</returns>
        private bool CreateDatabase()
        {
            using (SqlConnection conn = CreateOpenConnection())
            {
                try
                {
                    string commandText =
                        "CREATE DATABASE " + tbNewDBName.Text + "";

                    using (SqlCommand command = new SqlCommand(commandText, conn))
                    {
                        command.ExecuteNonQuery();
                    }

                    if (ddlAuthType.Text == "Windows Authentication")
                    {
                        SqlConnectionStringBuilder cb = new SqlConnectionStringBuilder();
                        cb.DataSource = tbServerName.Text;
                        cb.InitialCatalog = tbNewDBName.Text;
                        cb.IntegratedSecurity = true;

                        newExpressConnectionString = cb.ToString();
                    }
                    else
                    {
                        SqlConnectionStringBuilder cb = new SqlConnectionStringBuilder();
                        cb.DataSource = tbServerName.Text;
                        cb.InitialCatalog = tbNewDBName.Text;
                        cb.IntegratedSecurity = false;
                        cb.UserID = tbLogin.Text;
                        cb.Password = tbPassword.Text;

                        newExpressConnectionString = cb.ToString();
                    }
                }
                catch (SqlException ex)
                {
                    Logger.Error("Exception in " + System.Reflection.MethodBase.GetCurrentMethod().Name, ex);

                    Utility.DisplayApplicationErrorMessage("A database error has occurred in AuditWizard." + Environment.NewLine + Environment.NewLine +
                            "Please see the log file for further details.");

                    return false;
                }
                catch (Exception ex)
                {
                    Logger.Error("Exception in " + System.Reflection.MethodBase.GetCurrentMethod().Name, ex);

                    Utility.DisplayApplicationErrorMessage("A database error has occurred in AuditWizard." + Environment.NewLine + Environment.NewLine +
                            "Please see the log file for further details.");

                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Create database objects (tables, stored procs, constraints and initial data)
        /// </summary>
        /// <returns>true is successful</returns>
        private bool CreateDatabaseObjects()
        {
            try
            {
                using (SqlConnection conn = CreateOpenConnection())
                {
                    lbProgressStage.Text = "creating tables...";
                    this.Refresh();
                    ExecuteDatabaseCreateScript(Application.StartupPath + @"\db\CreateDatabase.sql", conn);

                    lbProgressStage.Text = "creating foreign keys...";
                    this.Refresh();
                    ExecuteDatabaseCreateScript(Application.StartupPath + @".\db\CreateForeignKeys.sql", conn);

                    lbProgressStage.Text = "creating stored procedures...";
                    this.Refresh();
                    ExecuteDatabaseCreateScript(Application.StartupPath + @".\db\CreateStoredProcedures.sql", conn);
                    ExecuteDatabaseCreateScript(Application.StartupPath + @".\db\CreateHBStoredProcedures.sql", conn);

                    lbProgressStage.Text = "adding initial data...";
                    this.Refresh();
                    ExecuteDatabaseCreateScript(Application.StartupPath + @".\db\AddInitialData.sql", conn);

                    new SettingsDAO().SetSetting("HBSPAdded", true);
                }
            }
            catch (SqlException ex)
            {
                Logger.Error("Exception in " + System.Reflection.MethodBase.GetCurrentMethod().Name, ex);

                Utility.DisplayApplicationErrorMessage("A database error has occurred in AuditWizard." + Environment.NewLine + Environment.NewLine +
                        "Please see the log file for further details.");

                return false;
            }
            catch (Exception ex)
            {
                Logger.Error("Exception in " + System.Reflection.MethodBase.GetCurrentMethod().Name, ex);

                Utility.DisplayApplicationErrorMessage("A database error has occurred in AuditWizard." + Environment.NewLine + Environment.NewLine +
                        "Please see the log file for further details.");

                return false;
            }

            return true;
        }

        /// <summary>
        /// Migrates data from default CE Edition database to SQL Edition
        /// </summary>
        /// <returns></returns>
        private bool UpdateExistingDatabase(bool aNewDatabase)
        {
            using (SqlConnection conn = CreateOpenConnection())
            {
                try
                {
                    lbProgressStage.Text = "purging existing table data...";
                    this.Refresh();
                    ExecuteDatabaseCreateScript(Application.StartupPath + @"\db\DeleteTableData.sql", conn);

                    // we might already be using a Express version here so check
                    if (_compactDatabaseType)
                        InsertCompactData(conn);
                    else
                        InsertExpressData(conn);
                }
                catch (SqlException ex)
                {
                    Logger.Error("Exception in " + System.Reflection.MethodBase.GetCurrentMethod().Name, ex);

                    Utility.DisplayApplicationErrorMessage("A database error has occurred in AuditWizard." + Environment.NewLine + Environment.NewLine +
                            "Please see the log file for further details.");

                    return false;
                }
                catch (Exception ex)
                {
                    Logger.Error("Exception in " + System.Reflection.MethodBase.GetCurrentMethod().Name, ex);

                    Utility.DisplayApplicationErrorMessage("A database error has occurred in AuditWizard." + Environment.NewLine + Environment.NewLine +
                            "Please see the log file for further details.");

                    return false;
                }

                return true;
            }
        }

        #region Database Query Methods

        private void InsertExpressData(SqlConnection conn)
        {
            try
            {                
                InsertTableDataExpress(conn, "VERSION");
                InsertTableDataExpress(conn, "USERDATA_VALUES");
                InsertTableDataExpress(conn, "USERS");
                InsertTableDataExpress(conn, "USERDATA_DEFINITIONS");
                InsertTableDataExpress(conn, "SCANNERS");
                InsertTableDataExpress(conn, "SETTINGS");
                InsertTableDataExpress(conn, "PICKLISTS");
                InsertTableDataExpress(conn, "OPERATIONS");
                InsertTableDataExpress(conn, "NOTES");
                InsertTableDataExpress(conn, "LOCATIONS");
                InsertTableDataExpress(conn, "FS_FOLDERS");
                InsertTableDataExpress(conn, "FS_FILES");
                InsertTableDataExpress(conn, "DOMAINS");
                InsertTableDataExpress(conn, "DOCUMENTS");
                InsertTableDataExpress(conn, "AUDITTRAIL");
                InsertTableDataExpress(conn, "AUDITEDITEMS");
                InsertTableDataExpress(conn, "ALERTS");
                InsertTableDataExpress(conn, "ASSETS");
                InsertTableDataExpress(conn, "ASSET_TYPES");
                InsertTableDataExpress(conn, "APPLICATIONS");
                InsertTableDataExpress(conn, "ACTIONS");
                InsertTableDataExpress(conn, "APPLICATION_INSTANCES");
                InsertTableDataExpress(conn, "LICENSES");
                InsertTableDataExpress(conn, "LICENSE_TYPES");
                InsertTableDataExpress(conn, "SUPPLIERS");
                InsertTableDataExpress(conn, "NEWS_FEED");
                InsertTableDataExpress(conn, "PUBLISHER_ALIAS");
                InsertTableDataExpress(conn, "PUBLISHER_ALIAS_APP");
                InsertTableDataExpress(conn, "REPORT_SCHEDULES");
                InsertTableDataExpress(conn, "REPORTS");
                InsertTableDataExpress(conn, "TASK_SCHEDULES");
                InsertTableDataExpress(conn, "LOCATION_SETTINGS");
                InsertTableDataExpress(conn, "IP_ADDRESSES");
                InsertTableDataCompact(conn, "ASSET_SUPPORTCONTRACT");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void InsertCompactData(SqlConnection conn)
        {
            try
            {                
                InsertTableDataCompact(conn, "VERSION");
                InsertTableDataCompact(conn, "USERDATA_VALUES");
                InsertTableDataCompact(conn, "USERS");
                InsertTableDataCompact(conn, "USERDATA_DEFINITIONS");
                InsertTableDataCompact(conn, "SCANNERS");
                InsertTableDataCompact(conn, "SETTINGS");
                InsertTableDataCompact(conn, "PICKLISTS");
                InsertTableDataCompact(conn, "OPERATIONS");
                InsertTableDataCompact(conn, "NOTES");
                InsertTableDataCompact(conn, "LOCATIONS");
                InsertTableDataCompact(conn, "FS_FOLDERS");
                InsertTableDataCompact(conn, "FS_FILES");
                InsertTableDataCompact(conn, "DOMAINS");
                InsertTableDataCompact(conn, "DOCUMENTS");
                InsertTableDataCompact(conn, "AUDITTRAIL");
                InsertTableDataCompact(conn, "AUDITEDITEMS");
                InsertTableDataCompact(conn, "ALERTS");
                InsertTableDataCompact(conn, "ASSETS");
                InsertTableDataCompact(conn, "ASSET_TYPES");
                InsertTableDataCompact(conn, "APPLICATIONS");
                InsertTableDataCompact(conn, "ACTIONS");
                InsertTableDataCompact(conn, "APPLICATION_INSTANCES");
                InsertTableDataCompact(conn, "LICENSES");
                InsertTableDataCompact(conn, "LICENSE_TYPES");
                InsertTableDataCompact(conn, "SUPPLIERS");
                InsertTableDataCompact(conn, "NEWS_FEED");
                InsertTableDataCompact(conn, "PUBLISHER_ALIAS");
                InsertTableDataCompact(conn, "PUBLISHER_ALIAS_APP");
                InsertTableDataCompact(conn, "REPORT_SCHEDULES");
                InsertTableDataCompact(conn, "REPORTS");
                InsertTableDataCompact(conn, "TASK_SCHEDULES");
                InsertTableDataCompact(conn, "LOCATION_SETTINGS");
                InsertTableDataCompact(conn, "IP_ADDRESSES");
                InsertTableDataCompact(conn, "ASSET_SUPPORTCONTRACT");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void UpdateAssetTableStructure(SqlConnection conn)
        {
            try
            {
                // - extra check that we don't already have these colummns
                // - could be possible if the user has re-installed
                SqlCommand cmd = new SqlCommand(
                    "SELECT COUNT(*) FROM INFORMATION_SCHEMA.COLUMNS " +
                    "WHERE TABLE_NAME = 'ASSETS' AND COLUMN_NAME = '_OVERWRITEDATA'", conn);

                if (Convert.ToInt32(cmd.ExecuteScalar()) == 0)
                {
                    cmd = new SqlCommand("ALTER TABLE ASSETS ADD _OVERWRITEDATA bit NULL DEFAULT (1)", conn);
                    cmd.ExecuteNonQuery();

                    cmd = new SqlCommand("UPDATE ASSETS SET _OVERWRITEDATA = 'True'", conn);
                    cmd.ExecuteNonQuery();
                }

                cmd = new SqlCommand(
                    "SELECT COUNT(*) FROM INFORMATION_SCHEMA.COLUMNS " +
                    "WHERE TABLE_NAME = 'ASSETS' AND COLUMN_NAME = '_ASSETTAG'", conn);

                if (Convert.ToInt32(cmd.ExecuteScalar()) == 0)
                {
                    cmd = new SqlCommand("ALTER TABLE ASSETS ADD _ASSETTAG varchar(255) NULL DEFAULT ('')", conn);
                    cmd.ExecuteNonQuery();

                    cmd = new SqlCommand("UPDATE ASSETS SET _ASSETTAG = ''", conn);
                    cmd.ExecuteNonQuery();
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void InsertTableDataCompact(SqlConnection conn, string tableName)
        {
            lbProgressStage.Text = "inserting data into table : " + tableName;
            this.Refresh();

            try
            {
                SqlBulkCopy sbc = new SqlBulkCopy(conn, SqlBulkCopyOptions.KeepIdentity, null);
                using (SqlCeConnection ceConn = DatabaseConnection.CreateOpenCEConnection())
                {
                    SqlCeCommand cmd = new SqlCeCommand(tableName, ceConn);
                    cmd.CommandType = CommandType.TableDirect;
                    using (SqlCeDataReader rdr = cmd.ExecuteReader())
                    {
                        sbc.DestinationTableName = "dbo." + tableName;
                        sbc.BulkCopyTimeout = 2000;
                        sbc.WriteToServer(rdr);
                    }
                }

                progressBar1.PerformStep();
                this.Refresh();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void InsertTableDataExpress(SqlConnection conn, string tableName)
        {
            AuditWizardDataAccess lAuditWizardDataAccess = new AuditWizardDataAccess();
            tableName = "dbo." + tableName;

            lbProgressStage.Text = "inserting data into table : " + tableName;
            this.Refresh();
            try
            {
                SqlBulkCopy sbc = new SqlBulkCopy(conn, SqlBulkCopyOptions.KeepIdentity, null);
                using (SqlConnection cConn = lAuditWizardDataAccess.CreateOpenConnection())
                {
                    SqlCommand cmd = new SqlCommand("SELECT * FROM " + tableName, cConn);
                    SqlDataReader rdr = cmd.ExecuteReader();

                    sbc.DestinationTableName = tableName;
					sbc.BulkCopyTimeout = 120;
                    sbc.WriteToServer(rdr);
                }

                progressBar1.PerformStep();
                this.Refresh();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void ExecuteDatabaseCreateScript(string filename, SqlConnection conn)
        {
            try
            {
                FileInfo file = new FileInfo(filename);
                string scriptText = file.OpenText().ReadToEnd();

                string[] splitter = new string[] { "\r\nGO\r\n" };
                string[] commandTexts = scriptText.Split(splitter, StringSplitOptions.RemoveEmptyEntries);
                foreach (string commandText in commandTexts)
                {
                    using (SqlCommand command = new SqlCommand(commandText, conn))
                    {
                        command.ExecuteNonQuery();
                    }
                }

                progressBar1.PerformStep();
                Refresh();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion

        #region Event Handlers

        /// <summary>
        /// Called as the 'Next' button is clicked to perform any necessary initialiation of the page
        /// that will be displayed next
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void wizardControl_NextButtonClick(WizardBase.WizardControl sender, WizardBase.WizardNextButtonClickEventArgs args)
        {
            switch (wizardControl1.CurrentStepIndex)
            {
                case 0:
                    break;
                case 1:
                    // after choosing DB
                    if (!SetNewConnectionString())
                        args.Cancel = true;
                    break;
                case 2:
                    // after migrate / new db option
                    break;
            }
        }

        private void ddlAuthType_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ddlAuthType.Text == "Windows Authentication")
            {
                tbLogin.Enabled = false;
                tbPassword.Enabled = false;

                lblLogin.Enabled = false;
                lblPassword.Enabled = false;
            }
            else
            {
                tbLogin.Enabled = true;
                tbPassword.Enabled = true;

                lblLogin.Enabled = true;
                lblPassword.Enabled = true;

                tbLogin.Focus();
                tbLogin.SelectAll();
            }
        }

        private void ddlCatalog_Changed(object sender, EventArgs e)
        {
            PopulateDatabaseList();
        }

        private void rdCreateNewDB_CheckedChanged(object sender, EventArgs e)
        {
            if (rdCreateNewDB.Checked)
            {
                rdExisting.Checked = false;
                ddlCatalog.Visible = false;
                tbNewDBName.Visible = true;
                lbDatabaseName.Text = "New Database Name:";
                tbNewDBName.Text = "AuditWizard";
            }
            else
            {
                rdExisting.Checked = true;
                ddlCatalog.Visible = true;
                tbNewDBName.Visible = false;
                lbDatabaseName.Text = "Catalog/Database:";
            }
        }

        private void rdExisting_CheckedChanged(object sender, EventArgs e)
        {
            if (rdExisting.Checked)
            {
                rdCreateNewDB.Checked = false;
                ddlCatalog.Visible = true;
                tbNewDBName.Visible = false;
                lbDatabaseName.Text = "Catalog/Database:";
            }
            else
            {
                rdCreateNewDB.Checked = true;
                ddlCatalog.Visible = false;
                tbNewDBName.Visible = true;
                lbDatabaseName.Text = "New Database Name:";
                tbNewDBName.Text = "AuditWizard";
            }
        }

        private void PopulateDatabaseList()
        {
            if (tbServerName.Text == String.Empty)
            {
                MessageBox.Show("Please enter a value for the server name.", "AuditWizard", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            Cursor = Cursors.WaitCursor;
            Application.DoEvents();

            ddlCatalog.Items.Clear();

            SqlConnectionStringBuilder cb = new SqlConnectionStringBuilder();

            try
            {
                if (ddlAuthType.Text == "SQL Server Authentication")
                {
                    cb = new SqlConnectionStringBuilder();
                    cb.DataSource = tbServerName.Text;
                    cb.IntegratedSecurity = false;
                    cb.UserID = tbLogin.Text;
                    cb.Password = tbPassword.Text;
                }

                else
                {
                    cb = new SqlConnectionStringBuilder();
                    cb.DataSource = tbServerName.Text;
                    cb.IntegratedSecurity = true;
                }

                SqlConnection conn = null;
                using (conn = new SqlConnection(cb.ToString()))
                {
                    conn.Open();

                    using (SqlCommand command = new SqlCommand())
                    {
                        command.Connection = conn;
                        command.CommandType = CommandType.StoredProcedure;
                        command.CommandText = "sp_databases";

                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                ddlCatalog.Items.Add(reader.GetString(0));
                            }
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                Logger.Error("Exception in ddlCatalog_DropDown using connection string : " + cb.ToString(), ex);

                MessageBox.Show("An error occurred whilst connecting to database (" + tbServerName.Text + ")." + Environment.NewLine + Environment.NewLine +
                    "'" + ex.Message + "'", "AuditWizard", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                Logger.Error("Exception in ddlCatalog_DropDown using connection string : " + cb.ToString(), ex);
            }
            finally
            {
                Cursor = Cursors.Default;
            }
        }

        private void CancelClicked(object sender, EventArgs e)
        {
            Close();
        }

        private void rdMigrateData_CheckedChanged(object sender, EventArgs e)
        {
            rdNoMigrateData.Checked = !rdMigrateData.Checked;
        }

        private void rdNoMigrateData_CheckedChanged(object sender, EventArgs e)
        {
            rdMigrateData.Checked = !rdNoMigrateData.Checked;
        }

        private void FinishClicked(object sender, EventArgs e)
        {
            if (!MigrateAndCreateDB())
            {
                Close();
                return;
            }

            // update the settings
            config.AppSettings.Settings["ConnectionStringExpressDataSource"].Value = tbServerName.Text;

            config.AppSettings.Settings["ConnectionStringExpressInitialCatalog"].Value = ddlCatalog.Text != String.Empty ? ddlCatalog.Text : tbNewDBName.Text;

            config.AppSettings.Settings["ConnectionStringExpressIntegratedSecurity"].Value = Convert.ToString(ddlAuthType.Text == "Windows Authentication");
            config.AppSettings.Settings["ConnectionStringExpressUserID"].Value = tbLogin.Text;
            config.AppSettings.Settings["ConnectionStringExpressPassword"].Value = AES.Encrypt(tbPassword.Text);
            config.AppSettings.Settings["CompactDatabaseType"].Value = "False";

            config.Save();

            // close form and restart application
            Close();

            MessageBox.Show(
                "In order to use the new database settings, AuditWizard will now restart.",
                "AuditWizard", MessageBoxButtons.OK, MessageBoxIcon.Information);

            System.Diagnostics.Process.Start(Application.ExecutablePath);
            Application.Exit();
            return;
        }

        private void bnChooseDatabases_Click(object sender, EventArgs e)
        {
            Cursor = Cursors.WaitCursor;

            FormSelectDatabaseName form = new FormSelectDatabaseName();
            if (form.ShowDialog() == DialogResult.OK)
            {
                tbServerName.Text = form.SelectedDatabaseName;
            }

            Cursor = Cursors.Default;
        }

        #endregion

    }
}
