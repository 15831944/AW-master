using System;
using System.Configuration;
using System.Data.SqlClient;
using System.IO;
using System.Windows.Forms;
using Layton.AuditWizard.DataAccess;
using Layton.Cab.Interface;
using Microsoft.Practices.CompositeUI;
using Microsoft.Practices.CompositeUI.SmartParts;
using Microsoft.Practices.ObjectBuilder;

namespace Layton.AuditWizard.Administration
{
    [SmartPart]
    public partial class DatabaseTabView : UserControl, ILaytonView, IAdministrationView
    {
        private LaytonWorkItem workItem;
        DatabaseTabViewPresenter presenter;
        DatabaseSettings _databaseSettings = new DatabaseSettings();
        private System.Configuration.Configuration config = ConfigurationManager.OpenExeConfiguration(Path.Combine(Application.StartupPath, "AuditWizardv8.exe"));

        [InjectionConstructor]
        public DatabaseTabView([ServiceDependency] WorkItem workItem)
        {
            this.workItem = workItem as LaytonWorkItem;
            InitializeComponent();
        }

        [CreateNew]
        public DatabaseTabViewPresenter Presenter
        {
            set { presenter = value; presenter.View = this; presenter.Initialize(); }
            get { return presenter; }
        }

        public void RefreshViewSinglePublisher()
        {
        }

        /// <summary>
        /// Refresh the current view
        /// </summary>
        public void RefreshView()
        {
            _databaseSettings.LoadSettings();
            base.Refresh();

            cbEnablePurge.Checked = _databaseSettings.AutoPurge;
            nupHistory.Value = _databaseSettings.HistoryPurge;
            cbHistoryUnits.SelectedIndex = (int)_databaseSettings.HistoryPurgeUnits;
            //
            nupInternet.Value = _databaseSettings.InternetPurge;
            cbInternetUnits.SelectedIndex = (int)_databaseSettings.InternetPurgeUnits;
            //
            nupAssets.Value = _databaseSettings.AssetPurge;
            cbAssetsUnits.SelectedIndex = (int)_databaseSettings.AssetPurgeUnits;
        }

        /// <summary>
        /// Called as this tab is activated to ensure that we display the latest possible data
        /// This function comes from the IAdministrationView Interface
        /// </summary>
        public void Activate()
		{
            System.Configuration.Configuration config = ConfigurationManager.OpenExeConfiguration(Path.Combine(Application.StartupPath, "AuditWizardv8.exe"));              
            
            bool _compactDatabaseType = Convert.ToBoolean(config.AppSettings.Settings["CompactDatabaseType"].Value);

            if (_compactDatabaseType)
            {
                lbDBType.Text = "You are currently connected to : SQL Server Compact 3.5";
                lbDBDetails.Text = "Data Source=" + Application.StartupPath + Convert.ToString(config.AppSettings.Settings["ConnectionStringCompact"].Value);
            }
            else
            {
                SqlConnectionStringBuilder cb = new SqlConnectionStringBuilder();
                cb.DataSource = config.AppSettings.Settings["ConnectionStringExpressDataSource"].Value;
                cb.InitialCatalog = config.AppSettings.Settings["ConnectionStringExpressInitialCatalog"].Value;
                cb.IntegratedSecurity = Convert.ToBoolean(config.AppSettings.Settings["ConnectionStringExpressIntegratedSecurity"].Value);

                if (!cb.IntegratedSecurity)
                {
                    cb.UserID = config.AppSettings.Settings["ConnectionStringExpressUserID"].Value;
                    cb.Password = AES.Decrypt(config.AppSettings.Settings["ConnectionStringExpressPassword"].Value);
                }

                string lDbName = String.Empty;
                string lDbSource = String.Empty;
                string lDBVersion = String.Empty;

                try
                {
                    using (SqlConnection sqlConn = new SqlConnection(cb.ToString()))
                    {
                        sqlConn.Open();
                        string commandText = 
                            "SELECT 'SQL Server ' " +
                            "+ CAST(SERVERPROPERTY('productversion') AS VARCHAR) + ' - ' " +
                            "+ CAST(SERVERPROPERTY('productlevel') AS VARCHAR) + ' (' " + 
                            "+ CAST(SERVERPROPERTY('edition') AS VARCHAR) + ')'";

                        using (SqlCommand command = new SqlCommand(commandText, sqlConn))
                        {
                            lDBVersion = Convert.ToString(command.ExecuteScalar());
                        }

                        lDbName = sqlConn.Database;
                        lDbSource = sqlConn.DataSource;
                    }
                }
                catch (Exception)
                {
                }

                if (lDBVersion.Length > 0 && lDbName.Length > 0)
                {
                    lbDBType.Text = "You are currently connected to : " + lDBVersion;
                    lbDBDetails.Text = "Data source : " + lDbSource + ", Database : " + lDbName;
                }
            }
		}

        private void Leave_Control(object sender, EventArgs e)
        {
            Save();
        }


        /// <summary>
        /// save function for the IAdministrationView Interface
        /// </summary>
        public void Save()
        {
            _databaseSettings.AutoPurge = cbEnablePurge.Checked;
            _databaseSettings.HistoryPurge = (int)nupHistory.Value;
            _databaseSettings.HistoryPurgeUnits = (DatabaseSettings.PURGEUNITS)cbHistoryUnits.SelectedIndex;
            //
            _databaseSettings.InternetPurge = (int)nupInternet.Value;
            _databaseSettings.InternetPurgeUnits = (DatabaseSettings.PURGEUNITS)cbInternetUnits.SelectedIndex;
            //
            _databaseSettings.AssetPurge = (int)nupAssets.Value;
            _databaseSettings.AssetPurgeUnits = (DatabaseSettings.PURGEUNITS)cbAssetsUnits.SelectedIndex;
            //
            _databaseSettings.SaveSettings();
        }

        public LaytonWorkItem WorkItem
        {
            get { return workItem; }
        }

        private void bnPurgeNow_Click(object sender, EventArgs e)
        {
            // First save the settings 
            Save();

            // ...then call the database purge function with these settings
            DatabaseMaintenanceDAO lwDataAccess = new DatabaseMaintenanceDAO();
            int nRowsPurged = lwDataAccess.DatabasePurge(_databaseSettings);
            MessageBox.Show("The AuditWizard database has been purged, " + nRowsPurged.ToString() + " row(s) were deleted", "Database Purged");
        }


        /// <summary>
        /// Called to import assets and locations from a CSV format file
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void bnImportAssets_click(object sender, EventArgs e)
        {
            if (this.openFileDialog.ShowDialog() == DialogResult.OK)
            {
                string inputFileName = openFileDialog.FileName;
                InputAssets(inputFileName);

            }
        }

        /// <summary>
        /// Called to import user defined data fields and their values
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void bnImportUserData_Click(object sender, EventArgs e)
        {
            if (this.openFileDialog.ShowDialog() == DialogResult.OK)
            {
                string inputFileName = openFileDialog.FileName;
                ImportUserData(inputFileName);
            }
        }


        /// <summary>
        /// Called as we click to import Picklists and PickItems into the database
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void bnImportPicklists_Click(object sender, EventArgs e)
        {
            if (this.openFileDialog.ShowDialog() == DialogResult.OK)
            {
                string inputFileName = openFileDialog.FileName;
                ImportPickLists(inputFileName);
            }
        }


        /// <summary>
        /// Called as we click to import Asset History records into the database
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void bnImportHistory_Click(object sender, EventArgs e)
        {
            if (this.openFileDialog.ShowDialog() == DialogResult.OK)
            {
                string inputFileName = openFileDialog.FileName;
                ImportHistory(inputFileName);
            }
        }



        /// <summary>
        /// Called to browse for a folder for AuditWizard database backups (NOT IMPLEMENTED)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void bnBrowse_Click(object sender, EventArgs e)
        {
        }


        /// <summary>
        /// Called as we change the units - if never then the count is not valid either
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cbHistoryUnits_SelectedIndexChanged(object sender, EventArgs e)
        {
            nupHistory.Enabled = (cbHistoryUnits.SelectedIndex != (int)DatabaseSettings.PURGEUNITS.never);
        }


        /// <summary>
        /// Called as we change the units - if never then the count is not valid either
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cbInternetUnits_SelectedIndexChanged(object sender, EventArgs e)
        {
            nupInternet.Enabled = (cbInternetUnits.SelectedIndex != (int)DatabaseSettings.PURGEUNITS.never);
        }


        /// <summary>
        /// Called as we change the units - if never then the count is not valid either
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cbAssetsUnits_SelectedIndexChanged(object sender, EventArgs e)
        {
            nupAssets.Enabled = (cbAssetsUnits.SelectedIndex != (int)DatabaseSettings.PURGEUNITS.never);
        }

        /// <summary>
        /// Input a comma-separated file containing locations/assets which should be imported
        /// the file must have the following format:
        /// 
        /// LOCATION, IP ADDRESS FROM, IP ADDRESS TO, ASSET NAME
        /// 
        /// The first row should really contain the above labels however so long as there are 4 columns we let it pass
        /// </summary>
        /// <param name="fileName"></param>
        private void InputAssets(string fileName)
        {
            FormImportLocations form = new FormImportLocations(fileName);
            form.ShowDialog();
        }

        /// <summary>
        /// Input a comma-separated file containing User Defined data Fields and Values which should be imported
        /// the file must have the following format:
        /// 
        /// ASSET NAME		FIELD NAME		FIELD VALUE
        /// 
        /// The first row should really contain the above labels however so long as there are 4 columns we let it pass
        /// </summary>
        /// <param name="fileName"></param>
        private void ImportUserData(string fileName)
        {
            //if (new UserDataDefinitionsDAO().SelectCountUserData() > 0)
            //{
            //   MessageBox.Show("AuditWizard has detected existing User Data. Existing data can only be imported into an empty table.",
            //        "AuditWizard", MessageBoxButtons.OK, MessageBoxIcon.Information);
			//
            //    return;
            //}
            FormImportUserDefinedData form = new FormImportUserDefinedData(fileName);
            form.ShowDialog();
        }


        /// <summary>
        /// Import a CSV format file containing Picklist and PickItems
        /// </summary>
        /// <param name="fileName"></param>
        private void ImportPickLists(string fileName)
        {
            FormImportPicklists form = new FormImportPicklists(fileName);
            form.ShowDialog();
        }


        /// <summary>
        /// Import a CSV format file containing Picklist and PickItems
        /// </summary>
        /// <param name="fileName"></param>
        private void ImportHistory(string fileName)
        {
            FormImportHistory form = new FormImportHistory(fileName);
            form.ShowDialog();
        }

        private void btnChangeDB_Click(object sender, EventArgs e)
        {
            FormChangeDBWizard form = new FormChangeDBWizard();
            form.ShowDialog();
        }

        private void bnDatabaseMaintenance_Click(object sender, EventArgs e)
        {
            FormDatabaseMaintenance form = new FormDatabaseMaintenance();
            form.ShowDialog();
        }
    }
}
