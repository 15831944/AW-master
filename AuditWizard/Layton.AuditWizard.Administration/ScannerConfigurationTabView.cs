using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using System.Xml.Serialization;
using Layton.AuditWizard.Common;
using Layton.AuditWizard.DataAccess;
using Layton.Cab.Interface;
using Microsoft.Practices.CompositeUI;
using Microsoft.Practices.CompositeUI.SmartParts;
using Microsoft.Practices.ObjectBuilder;
using System.Text;

namespace Layton.AuditWizard.Administration
{
    [SmartPart]
    public partial class ScannerConfigurationTabView : UserControl, ILaytonView, IAdministrationView
    {
        #region Data
        private LaytonWorkItem workItem;
        ScannerConfigurationTabViewPresenter presenter;
        AuditScannerDefinition auditScannerDefinition;

        #endregion Data

        #region Constructor
        [InjectionConstructor]
        public ScannerConfigurationTabView([ServiceDependency] WorkItem workItem)
        {
            this.workItem = workItem as LaytonWorkItem;
            auditScannerDefinition = new AuditScannerDefinition();
            InitializeComponent();
        }
        #endregion Constructor

        #region CAB Functions

        [CreateNew]
        public ScannerConfigurationTabViewPresenter Presenter
        {
            set { presenter = value; presenter.View = this; presenter.Initialize(); }
            get { return presenter; }
        }

        /// <summary>
        /// Called as this tab is activated to ensure that we display the latest possible data
        /// This function comes from the IAdministrationView Interface
        /// </summary>
        public void Activate()
        {
            // if we don't have a scanner, load default
            if (tbName.Text == String.Empty)
            {
                try
                {
                    string scannerPath = Path.Combine(Application.StartupPath, "scanners") + "\\default.xml";
                    auditScannerDefinition = AuditWizardSerialization.DeserializeObject(scannerPath);
                }
                catch (Exception)
                {
                    // if there was an error here simply log it and load blank config file
                    // JML TODO log this error
                }
            }

            else
            {
                string scannerPath = Path.Combine(Application.StartupPath, "scanners") + "\\" + tbName.Text + ".xml";
                auditScannerDefinition = AuditWizardSerialization.DeserializeObject(scannerPath);
            }

            // ...and then refresh the view
            RefreshDefaultView();
        }

        private void ScannerConfigurationTabView_Load(object sender, EventArgs e)
        {
            // perform some initial setup
            cbUploadSetting.SelectedIndex = 0;
            cbScannerMode.SelectedIndex = 0;
            nupReauditInterval.Value = 7;

            tbName.Text = String.Empty;
            tbDescription.Text = "<Default description text>";

            cbEnableAlertMonitor.Checked = true;
            cbSystemTray.Enabled = true;
            lblCheckAlerts.Enabled = true;
            lblCheckSettings.Enabled = true;
            nupSettingsChange.Enabled = true;
            nupAlertRescan.Enabled = true;
            lblAlertsSecs.Enabled = true;
            lblSettingsSecs.Enabled = true;
            lblSelectAlertDefintion.Enabled = true;
            tbAlertDefinition.Enabled = true;
            btnChooseAlertDefinition.Enabled = true;
            cbInvisible.Checked = true;
        }

        public void Close(object smartPart)
        {
            Save();
        }

        public void Save()
        {
            Save(true);
        }

        public void Save(bool promptForSave)
        {
            if (!ValidateRequiredValues())
                return;

            auditScannerDefinition.ScannerName = tbName.Text;
            auditScannerDefinition.Filename = Path.Combine(Application.StartupPath, @"scanners\" + auditScannerDefinition.ScannerName + ".xml");

            if (File.Exists(auditScannerDefinition.Filename) && promptForSave)
            {
                if (MessageBox.Show("The config file '" + auditScannerDefinition.ScannerName + "' already exists." + Environment.NewLine + Environment.NewLine +
                    "Do you wish to overwrite the file?", "AuditWizard", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
                    return;
            }
            UpdateChanges();

            if (AuditWizardSerialization.SerializeObjectToFile(auditScannerDefinition) && promptForSave)
            {
                DesktopAlert.ShowDesktopAlert("The scanner configuration file '" + auditScannerDefinition.ScannerName + "' has been saved.");
            }
        }

        private bool ValidateRequiredValues()
        {
            List<string> errorStates = new List<string>();

            if (tbName.Text == "<Enter name>" || tbName.Text == String.Empty)
            {
                MessageBox.Show("Please enter a value for the configuration name.", "AuditWizard", MessageBoxButtons.OK, MessageBoxIcon.Information);
                tbName.Focus();
                tbName.SelectAll();
                return false;
            }

            if (tbAuditFolder.Text == String.Empty || tbDataFolder.Text == String.Empty)
            {
                MessageBox.Show("Please enter a value for the scanner folder.", "AuditWizard", MessageBoxButtons.OK, MessageBoxIcon.Information);
                tbAuditFolder.Focus();
                return false;
            }

            if (cbUploadSetting.SelectedIndex == (int)AuditScannerDefinition.eUploadSetting.ftp)
            {
                if (string.IsNullOrEmpty(auditScannerDefinition.FTPSite))
                {
                    errorStates.Add("- Upload method FTP Site Name or Address");
                }
                if (!auditScannerDefinition.FTPAnonymous)
                {
                    if (string.IsNullOrEmpty(auditScannerDefinition.FTPUser))
                    {
                        errorStates.Add("- Upload method FTP username to log in ");
                    }

                    if (string.IsNullOrEmpty(auditScannerDefinition.FTPPassword))
                    {
                        errorStates.Add("- Upload method FTP password to log in ");
                    }

                }
            }

            if (cbSaveCopy.Checked)
            {
                if (string.IsNullOrEmpty(auditScannerDefinition.FTPSiteBackup))
                {
                    errorStates.Add("- Upload copy FTP Site Name or Address");
                }
                if (!auditScannerDefinition.FTPAnonymousBackup)
                {
                    if (string.IsNullOrEmpty(auditScannerDefinition.FTPUserBackup))
                    {
                        errorStates.Add("- Upload copy FTP username to log in ");
                    }

                    if (string.IsNullOrEmpty(auditScannerDefinition.FTPPasswordBackup))
                    {
                        errorStates.Add("- Upload copy FTP password to log in ");
                    }

                }
            }

            if (errorStates.Count > 0)
            {
                StringBuilder errorList = new StringBuilder();

                errorList.Append(Environment.NewLine + Environment.NewLine);

                foreach (string errorState in errorStates)
                {
                    errorList = errorList.Append(errorState + Environment.NewLine + Environment.NewLine);
                }

                MessageBox.Show("The following fields are required - please enter values for: " + errorList,
                    "AuditWizard", MessageBoxButtons.OK, MessageBoxIcon.Information);

                return false;
            }


            return true;
        }

        public AuditScannerDefinition UpdateChanges()
        {
            // Save the fields on this tab - first General Settings
            auditScannerDefinition.ScannerName = tbName.Text;
            auditScannerDefinition.Description = tbDescription.Text;
            auditScannerDefinition.Hidden = cbInvisible.Checked;
            auditScannerDefinition.ReAuditInterval = (int)nupReauditInterval.Value;
            auditScannerDefinition.FTPCopyToNetwork = cbSaveCopy.Checked;

            // Audit Scanner Location Settings
            auditScannerDefinition.UploadSetting = (AuditScannerDefinition.eUploadSetting)cbUploadSetting.SelectedIndex;

            // Upload fields
            auditScannerDefinition.AutoUpload = cbAutoUpload.Checked;
            auditScannerDefinition.DeployPathRoot = tbAuditFolder.Text;
            auditScannerDefinition.DeployPathData = tbDataFolder.Text;
            auditScannerDefinition.EmailAddress = tbEmailAddress.Text;

            // Check the audit data folder to see whether or not we have specified differing scanner and data 
            // folders and set the paths accordingly
            auditScannerDefinition.DeployPathRoot = tbAuditFolder.Text;
            if (tbDataFolder.Text == (Path.Combine(auditScannerDefinition.DeployPathRoot, "Data")))
            {
                auditScannerDefinition.DeployPathSingle = true;
            }
            else
            {
                auditScannerDefinition.DeployPathSingle = false;
                auditScannerDefinition.DeployPathData = tbDataFolder.Text;
            }

            // USB / PDA Scanning
            auditScannerDefinition.ScanMDA = cbMobileDevices.Checked;
            auditScannerDefinition.ScanUSB = cbUSBDevices.Checked;

            // Hardware settings
            auditScannerDefinition.HardwareScan = cbAuditHardware.Checked;


            auditScannerDefinition.IEScan = cbAuditInternet.Checked;
            //JML TODO
            auditScannerDefinition.FileSystemScan = cbAuditFileSystem.Checked;
            if (!cbAuditFileSystem.Checked)
                auditScannerDefinition.ScanFolders = AuditScannerDefinition.eFolderSetting.noFolders;
            auditScannerDefinition.RegistryScan = cbAuditRegistry.Checked;
            auditScannerDefinition.SoftwareScan = cbAuditSoftware.Checked;

            // alert monitor settings
            auditScannerDefinition.AlertMonitorEnabled = cbEnableAlertMonitor.Checked;
            auditScannerDefinition.AlertMonitorDisplayTray = cbSystemTray.Checked;
            auditScannerDefinition.AlertMonitorSettingSecs = (int)nupSettingsChange.Value;
            auditScannerDefinition.AlertMonitorAlertSecs = (int)nupAlertRescan.Value;

            if (tbAlertDefinition.Text != String.Empty)
                auditScannerDefinition.AlertMonitorDefinition = GetAlertDefinitionFromFileName(Path.Combine(Application.StartupPath, "scanners\\alertmonitors\\") + tbAlertDefinition.Text + ".xml");

            // interactive settings
            auditScannerDefinition.ScannerMode = (AuditScannerDefinition.eAuditMode)cbScannerMode.SelectedIndex;

            //if (auditScannerDefinition.ScannerMode != AuditScannerDefinition.eAuditMode.modeNonInteractive)
            //{
                // Computers
                UpdateInteractiveComputers();

                // Picklists
                UpdateInteractivePicklists();

                // Locations
                UpdateInteractiveLocations();

                // User Data categories
                UpdateInteractiveUserDataCategories();
            //}

            // update publisher mappings
            UpdateSerialNumberMappings();

            return auditScannerDefinition;
        }

        private void UpdateSerialNumberMappings()
        {
            List<SerialNumberMapping> lSerialNumberMappingsList = new List<SerialNumberMapping>();
            ApplicationDefinitionsFile _applicationDefinitionsFile = new ApplicationDefinitionsFile();

            List<string> listRegistryMappings = new List<string>();
            _applicationDefinitionsFile.SetSection(ApplicationDefinitionsFile.APPLICATION_MAPPINGS_SECTION);
            _applicationDefinitionsFile.EnumerateKeys(ApplicationDefinitionsFile.APPLICATION_MAPPINGS_SECTION, listRegistryMappings);

            // Iterate through the keys and create an application mapping object
            foreach (string thisKey in listRegistryMappings)
            {
                // The key is the application name, the value the registry mapping
                ApplicationRegistryMapping applicationMapping = new ApplicationRegistryMapping(thisKey);
                string mappings = _applicationDefinitionsFile.GetString(thisKey, "");
                applicationMapping.AddMapping(mappings);

                SerialNumberMapping lSerialNumberMapping = new SerialNumberMapping();
                lSerialNumberMapping.ApplicationName = applicationMapping.ApplicationName;

                foreach (RegistryMapping lMapping in applicationMapping)
                {
                    SerialNumberMappingsRegistryKey lSerialNumberMappingRegKey = new SerialNumberMappingsRegistryKey();
                    lSerialNumberMappingRegKey.RegistryKeyName = lMapping.RegistryKey;
                    lSerialNumberMappingRegKey.RegistryKeyValue = lMapping.ValueName;

                    lSerialNumberMapping.RegistryKeysList.Add(lSerialNumberMappingRegKey);
                }

                lSerialNumberMappingsList.Add(lSerialNumberMapping);
            }

            auditScannerDefinition.SerialNumberMappingsList = lSerialNumberMappingsList;
        }

        private void UpdateInteractiveUserDataCategories()
        {
            auditScannerDefinition.InteractiveUserDataCategories.Clear();

            AssetTypeList listAssetTypes = new AssetTypeList();
            listAssetTypes.Populate();

            AssetTypeList computersList = listAssetTypes.EnumerateChildren(listAssetTypes.FindByName("Computers").AssetTypeID);

            UserDataCategoryList listCategories = new UserDataCategoryList(UserDataCategory.SCOPE.Asset);
            listCategories.Populate();

            // Sort the list to put the categories in Tab Order
            listCategories.Sort();

            // Iterate through the categories
            foreach (UserDataCategory category in listCategories)
            {
                List<UserDataField> fieldsToRemove = new List<UserDataField>();

                foreach (UserDataField userDataField in category)
                {
                    if (userDataField.ParentScope != UserDataCategory.SCOPE.Asset)
                        fieldsToRemove.Add(userDataField);
                }

                foreach (UserDataField userDataField in fieldsToRemove)
                {
                    category.Remove(userDataField);
                }

                if ((category.AppliesToName != "Computers") && (category.AppliesToName != String.Empty))
                {
                    // No - OK is it a type of computer?
                    bool exclude = true;
                    foreach (AssetType assetType in computersList)
                    {
                        if (assetType.Name == category.AppliesToName)
                        {
                            exclude = false;
                            break;
                        }
                    }

                    // If excluded, skip this category
                    if (exclude)
                        continue;
                }

                UserDataCategories lUserDataCategory = new UserDataCategories();
                lUserDataCategory.Name = category.Name;
                lUserDataCategory.UserFields = category;
                auditScannerDefinition.InteractiveUserDataCategories.Add(lUserDataCategory);
            }
        }

        private void UpdateInteractiveLocations()
        {
            auditScannerDefinition.InteractiveLocations.Clear();

            DataTable table = new LocationsDAO().GetGroups(new AssetGroup(AssetGroup.GROUPTYPE.userlocation));
            AssetGroup rootLocation = new AssetGroup(table.Rows[0], AssetGroup.GROUPTYPE.userlocation);

            // Populate all levels for this user location
            rootLocation.Populate(true, false, true);

            auditScannerDefinition.RootLocation = rootLocation.Name;

            foreach (AssetGroup childLocation in rootLocation.Groups)
            {
                childLocation.Assets.Clear();
                auditScannerDefinition.InteractiveLocations.Add(childLocation);
                SaveInteractiveLocation(childLocation);
            }
        }

        private void SaveInteractiveLocation(AssetGroup parentLocation)
        {
            foreach (AssetGroup childLocation in parentLocation.Groups)
            {
                if (!auditScannerDefinition.InteractiveLocations.Contains(childLocation))
                {
                    childLocation.Assets.Clear();
                    auditScannerDefinition.InteractiveLocations.Add(childLocation);
                }

                SaveInteractiveLocation(childLocation);
            }
        }

        private void UpdateInteractivePicklists()
        {
            auditScannerDefinition.InteractivePicklists.Clear();
            PickListList listPickLists = new PickListList();
            listPickLists.Populate();

            foreach (PickList picklist in listPickLists)
            {
                InteractivePickList lInteractivePickList = new InteractivePickList();
                lInteractivePickList.Name = picklist.Name;
                lInteractivePickList.PickListItems = picklist;

                auditScannerDefinition.InteractivePicklists.Add(lInteractivePickList);
            }
        }

        private void UpdateInteractiveComputers()
        {
            AssetTypeList listAssetTypes = new AssetTypeList();
            listAssetTypes.Populate();

            AssetType assetType = listAssetTypes.FindByName("Computers");
            AssetTypeList computersList = listAssetTypes.EnumerateChildren(assetType.AssetTypeID);
            auditScannerDefinition.InteractiveCategories = computersList.ToString();
        }

        private AlertDefinition GetAlertDefinitionFromFileName(string fileName)
        {
            TextReader textReader = null;
            AlertDefinition alertDefinition = null;
            try
            {
                XmlSerializer serializer = new XmlSerializer(typeof(AlertDefinition));
                textReader = new StreamReader(fileName);
                alertDefinition = (AlertDefinition)serializer.Deserialize(textReader);
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred whilst trying to load the alert definition : " + Environment.NewLine + Environment.NewLine + ex.Message,
                    "AuditWizard", MessageBoxButtons.OK, MessageBoxIcon.Error);

                // JML TODO log this exception
                return null;
            }
            finally
            {
                // close text writer
                textReader.Close();
            }

            return alertDefinition;
        }


        public LaytonWorkItem WorkItem
        {
            get { return workItem; }
        }

        #endregion CAB Functions

        public void RefreshViewSinglePublisher()
        {
        }

        /// <summary>
        /// Refresh the current view
        /// </summary>
        public void RefreshView()
        {
            base.Refresh();

            // get the current scanner definition
            AdministrationWorkItemController wic = workItem.Controller as AdministrationWorkItemController;
            AuditScannerDefinition scannerDefinition = wic.AuditScannerDefinition;

            auditScannerDefinition = scannerDefinition;

            // Refresh the fields on this tab - first General Settings
            tbName.Text = scannerDefinition.ScannerName;
            tbDescription.Text = scannerDefinition.Description;
            cbScannerMode.SelectedIndex = (int)scannerDefinition.ScannerMode;
            cbInvisible.Checked = scannerDefinition.Hidden;
            nupReauditInterval.Value = scannerDefinition.ReAuditInterval;

            // Audit Scanner Location Settings
            cbUploadSetting.SelectedIndex = (int)scannerDefinition.UploadSetting;
            cbSaveCopy.Checked = scannerDefinition.FTPCopyToNetwork;

            // Upload fields
            tbAuditFolder.Text = scannerDefinition.DeployPathRoot;
            tbDataFolder.Text = scannerDefinition.DeployPathData;
            tbEmailAddress.Text = scannerDefinition.EmailAddress;
            cbAutoUpload.Checked = scannerDefinition.AutoUpload;

            // Audited Items Settings
            cbAuditHardware.Checked = scannerDefinition.HardwareScan;
            cbAuditInternet.Checked = scannerDefinition.IEScan;
            cbAuditRegistry.Checked = scannerDefinition.RegistryScan;
            cbAuditFileSystem.Checked = scannerDefinition.FileSystemScan;
            cbMobileDevices.Checked = scannerDefinition.ScanMDA;
            cbUSBDevices.Checked = scannerDefinition.ScanUSB;
            cbAuditSoftware.Checked = scannerDefinition.SoftwareScan;

            // Alert Monitor Settings
            cbEnableAlertMonitor.Checked = scannerDefinition.AlertMonitorEnabled;
            cbSystemTray.Checked = scannerDefinition.AlertMonitorDisplayTray;
            nupAlertRescan.Value = (decimal)scannerDefinition.AlertMonitorAlertSecs;
            nupSettingsChange.Value = (decimal)scannerDefinition.AlertMonitorSettingSecs;
            tbAlertDefinition.Text = scannerDefinition.AlertMonitorDefinition.Name;
        }

        /// <summary>
        /// Refresh the current view
        /// </summary>
        public void RefreshDefaultView()
        {
            base.Refresh();

            if (auditScannerDefinition == null) return;

            // get the current scanner definition
            AuditScannerDefinition scannerDefinition = auditScannerDefinition;

            // Refresh the fields on this tab - first General Settings
            tbName.Text = scannerDefinition.ScannerName;
            tbDescription.Text = scannerDefinition.Description;
            cbScannerMode.SelectedIndex = (int)scannerDefinition.ScannerMode;
            cbInvisible.Checked = scannerDefinition.Hidden;
            nupReauditInterval.Value = scannerDefinition.ReAuditInterval;

            // Audit Scanner Location Settings
            cbUploadSetting.SelectedIndex = (int)scannerDefinition.UploadSetting;
            cbSaveCopy.Checked = scannerDefinition.FTPCopyToNetwork;

            // Upload fields
            tbAuditFolder.Text = scannerDefinition.DeployPathRoot;
            tbDataFolder.Text = scannerDefinition.DeployPathData;
            tbEmailAddress.Text = scannerDefinition.EmailAddress;
            cbAutoUpload.Checked = scannerDefinition.AutoUpload;

            // Audited Items Settings
            cbAuditHardware.Checked = scannerDefinition.HardwareScan;
            cbAuditInternet.Checked = scannerDefinition.IEScan;
            cbAuditRegistry.Checked = scannerDefinition.RegistryScan;
            cbAuditFileSystem.Checked = scannerDefinition.FileSystemScan;
            cbMobileDevices.Checked = scannerDefinition.ScanMDA;
            cbUSBDevices.Checked = scannerDefinition.ScanUSB;
            cbAuditSoftware.Checked = scannerDefinition.SoftwareScan;

            // Alert Monitor Settings
            cbEnableAlertMonitor.Checked = scannerDefinition.AlertMonitorEnabled;
            cbSystemTray.Checked = scannerDefinition.AlertMonitorDisplayTray;
            nupAlertRescan.Value = (decimal)scannerDefinition.AlertMonitorAlertSecs;
            nupSettingsChange.Value = (decimal)scannerDefinition.AlertMonitorSettingSecs;
            tbAlertDefinition.Text = scannerDefinition.AlertMonitorDefinition.Name;
        }


        #region General Settings Handlers

        /// <summary>
        /// Called when the data folder loses focus
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tbAuditFolder_LostFocus(object sender, EventArgs e)
        {
            // check if this is a UNC path
            if (tbDataFolder.Text.Length == 0)
            {
                lblUNCWarning.Visible = false;
                tbAuditFolder.ForeColor = Color.Black;
                tbDataFolder.ForeColor = Color.Black;
            }
            else if (!tbDataFolder.Text.StartsWith("\\"))
            {
                lblUNCWarning.Visible = true;
                tbAuditFolder.ForeColor = Color.Red;
                tbDataFolder.ForeColor = Color.Red;
            }
            else
            {
                lblUNCWarning.Visible = false;
                tbAuditFolder.ForeColor = Color.Black;
                tbDataFolder.ForeColor = Color.Black;
            }

            tbDataFolder.Text = Path.Combine(tbAuditFolder.Text, "Data");
        }

        private void tbAuditFolder_TextChanged(object sender, EventArgs e)
        {
            // check if this is a UNC path
            if (tbAuditFolder.Text.Length == 0)
            {
                lblUNCWarning.Visible = false;
                tbAuditFolder.ForeColor = Color.Black;
                tbDataFolder.ForeColor = Color.Black;
            }
            else if (!tbAuditFolder.Text.StartsWith(@"\\"))
            {
                lblUNCWarning.Visible = true;
                tbAuditFolder.ForeColor = Color.Red;
                tbDataFolder.ForeColor = Color.Red;
            }
            else
            {
                lblUNCWarning.Visible = false;
                tbAuditFolder.ForeColor = Color.Black;
                tbDataFolder.ForeColor = Color.Black;
            }

            tbDataFolder.Text = Path.Combine(tbAuditFolder.Text, "Data");
        }

        /// <summary>
        /// Called as we change the mode in which the scanner will run - this affects other fields which
        /// may or may not be displayed on this tab.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void scannerMode_ValueChanged(object sender, EventArgs e)
        {
            if ((int)cbScannerMode.Value == 0)
            {
                // Non-interactive - allow the user to select whether to run invisible but not design the screens
                cbInvisible.Visible = true;
                cbInvisible.Checked = auditScannerDefinition.Hidden;
                this.bnDesignLayout.Visible = false;
            }

            else if ((int)cbScannerMode.Value == 1)
            {
                // interactive - allow the user to design the screens but not run invisible
                auditScannerDefinition.Hidden = cbInvisible.Checked;
                cbInvisible.Visible = false;
                cbInvisible.Checked = false;
                this.bnDesignLayout.Visible = true;
            }

            else if ((int)cbScannerMode.Value == 2)
            {
                // First time Interactive - the user can design and run invisible
                cbInvisible.Visible = true;
                this.bnDesignLayout.Visible = true;
            }
        }


        /// <summary>
        /// Called as we click the 'Design Layout' button to design the interactive screens
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void bnDesignLayout_Click(object sender, EventArgs e)
        {
            // Get the current scanner definition
            FormAuditDesigner form = new FormAuditDesigner(auditScannerDefinition);
            form.ShowDialog();
        }

        #endregion General Settings Handlers

        #region Button Handlers


		/// <summary>
		/// Called when we click on the 'Details' button associated with the 'Hardware' checkbox.  This displays a form that
		/// allows the user to select which hardware items to audit
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void bnHardwareAdvanced_Click(object sender, EventArgs e)
		{
			FormHardwareDetails form = new FormHardwareDetails(auditScannerDefinition);
			form.ShowDialog();
		}

		/// <summary>
		/// Called when we click on the 'Details' button associated with the 'Software' checkbox.  This displays a form that
		/// allows the user to select which software items to audit
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void bnSoftwareAdvanced_Click(object sender, EventArgs e)
		{
			FormSoftwareDetails form = new FormSoftwareDetails(auditScannerDefinition);
			form.ShowDialog();
		}

        private void bnAuditFileSystemAdvanced_Click(object sender, EventArgs e)
        {
            FormFileSystemDetails form = new FormFileSystemDetails(auditScannerDefinition);
            form.ShowDialog();
        }


        /// <summary>
        /// Called when we want to audit some system registry keys.  Allow the user to enter one or motr
        /// registry keys to audit.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void bnAuditRegistry_Click(object sender, EventArgs e)
        {
            FormRegistryDetails form = new FormRegistryDetails(auditScannerDefinition);
            form.ShowDialog();
        }


        /// <summary>
        /// Called when we want to audit Internet details.  Allow the user to select what details are required
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void bnAuditInternet_Click(object sender, EventArgs e)
        {
            FormInternetDetails form = new FormInternetDetails(auditScannerDefinition);
            form.ShowDialog();
        }

        private void bnMobileDevices_Click(object sender, EventArgs e)
        {
            FormMobileDeviceDetails form = new FormMobileDeviceDetails(auditScannerDefinition);
            form.ShowDialog();
        }

        private void bnUsbDevices_Click(object sender, EventArgs e)
        {
            FormUsbDeviceDetails form = new FormUsbDeviceDetails(auditScannerDefinition);
            form.ShowDialog();
        }

        private void cbEnableAlertMonitor_CheckedChanged(object sender, EventArgs e)
        {
            bool bEnabled = cbEnableAlertMonitor.Checked;

            cbSystemTray.Enabled = bEnabled;
            lblCheckAlerts.Enabled = bEnabled;
            lblCheckSettings.Enabled = bEnabled;
            nupSettingsChange.Enabled = bEnabled;
            nupAlertRescan.Enabled = bEnabled;
            lblAlertsSecs.Enabled = bEnabled;
            lblSettingsSecs.Enabled = bEnabled;
            lblSelectAlertDefintion.Enabled = bEnabled;
            tbAlertDefinition.Enabled = bEnabled;
            btnChooseAlertDefinition.Enabled = bEnabled;
        }

        private void btnChooseAlertDefinition_Click(object sender, EventArgs e)
        {
            FormLoadAlertMonitorDefinition form = new FormLoadAlertMonitorDefinition();
            if (form.ShowDialog() == DialogResult.OK)
            {
                TextReader textReader = null;
                string selectedFileName = String.Empty;

                try
                {
                    selectedFileName = Path.Combine(Application.StartupPath, "scanners\\alertmonitors\\") + form.FileName + ".xml";

                    XmlSerializer serializer = new XmlSerializer(typeof(AlertDefinition));
                    textReader = new StreamReader(selectedFileName);

                    auditScannerDefinition.AlertMonitorDefinition = (AlertDefinition)serializer.Deserialize(textReader);
                    tbAlertDefinition.Text = auditScannerDefinition.AlertMonitorDefinition.Name;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("An error occurred whilst trying to load the alert definition : " + Environment.NewLine + Environment.NewLine + ex.Message,
                        "AuditWizard", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    // JML TODO log this exception
                }
                finally
                {
                    // close text writer
                    textReader.Close();
                }
            }
        }

        #endregion Button Handlers

        #region  Audit Scanner Location Handlers

        /// <summary>
        /// Called as click the '...' button to browse for the root folder for the audit scanner
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void bnBrowseSingle_Click(object sender, EventArgs e)
        {
            folderBrowserDialog1.Description = "Please select the root folder for the audit scanner";
            if (this.folderBrowserDialog1.ShowDialog() == DialogResult.OK)
            {
                this.tbAuditFolder.Text = folderBrowserDialog1.SelectedPath;
                tbAuditFolder_Leave(sender, e);
            }
        }


        /// <summary>
        /// Called as click the '...' button to browse for the root folder for the audit scanner
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void bnBrowseData_Click(object sender, EventArgs e)
        {
            folderBrowserDialog1.Description = "Please select the folder to which audit data should be saved";
            if (this.folderBrowserDialog1.ShowDialog() == DialogResult.OK)
                this.tbDataFolder.Text = folderBrowserDialog1.SelectedPath;
        }

        /// <summary>
        /// Called as move off the single folder  so that we can validate the path specified
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tbAuditFolder_Leave(object sender, EventArgs e)
        {

        }

        #endregion  Audit Scanner Location Handlers

        #region Audited Items Section Handlers

		/// <summary>
		/// Called as we change the check state of the 'Audit Hardware' setting.  If unchecked we cannot
		/// click on the 'Details' button
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void cbAuditHardware_CheckedChanged(object sender, EventArgs e)
		{
			this.bnHardwareAdvanced.Enabled = cbAuditHardware.Checked;
		}

		/// <summary>
		/// Called as we change the check state of the 'Audit Software' setting.  If unchecked we cannot
		/// click on the 'Details' button
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void cbAuditSoftware_CheckedChanged(object sender, EventArgs e)
		{
			this.bnSoftwareAdvanced.Enabled = cbAuditSoftware.Checked;
		}


        /// <summary>
        /// Called as we change the check state of the 'Audit FileSystem' setting.  If unchecked we cannot
        /// click on the 'Details' button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cbAuditFileSystem_CheckedChanged(object sender, EventArgs e)
        {
            this.bnAuditFileSystemAdvanced.Enabled = this.cbAuditFileSystem.Checked;
        }


        /// <summary>
        /// Called as we change the check state of the 'Audit Registry' setting.  If unchecked we cannot
        /// click on the 'Details' button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cbAuditRegistry_CheckedChanged(object sender, EventArgs e)
        {
            this.bnAuditRegistry.Enabled = this.cbAuditRegistry.Checked;
        }


        /// <summary>
        /// Called as we click on the 'Upload via FTP' button to display the FTP site settings
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void bnUploadViaFTP_Click(object sender, EventArgs e)
        {
            // Get the current scanner definition
            //AdministrationWorkItemController wic = workItem.Controller as AdministrationWorkItemController;
            //AuditScannerDefinition scannerDefinition = wic.AuditScannerDefinition;
            FormFtpSettings form = new FormFtpSettings(auditScannerDefinition);
            form.ShowDialog();
        }


        private void cbAuditInternet_CheckedChanged(object sender, EventArgs e)
        {
            // Get the current scanner definition
            //AdministrationWorkItemController wic = workItem.Controller as AdministrationWorkItemController;
            //AuditScannerDefinition scannerDefinition = wic.AuditScannerDefinition;
            this.bnAuditInternet.Enabled = this.cbAuditInternet.Checked;

            if (cbAuditInternet.Checked)
            {
                auditScannerDefinition.IEHistory = auditScannerDefinition.IECookies = true;
                auditScannerDefinition.IEDetails = false;
            }
        }



        /// <summary>
        /// Called as we change the check state of the audit Mobile devices.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cbMobileDevices_CheckedChanged(object sender, EventArgs e)
        {
            bnMobileDevices.Enabled = cbMobileDevices.Checked;
        }

        /// <summary>
        /// Called as we change the check state of the audit USB devices.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cbUSBDevices_CheckedChanged(object sender, EventArgs e)
        {
            bnUsbDevices.Enabled = cbUSBDevices.Checked;
        }


        #endregion Audited Items Section Handlers

        private void cbUploadSetting_SelectedIndexChanged(object sender, EventArgs e)
        {
            panelDataFolder.Visible = ((cbUploadSetting.SelectedIndex == (int)AuditScannerDefinition.eUploadSetting.network)
                                    || (cbUploadSetting.SelectedIndex == (int)AuditScannerDefinition.eUploadSetting.removablemedia));
            bnUploadViaFTP.Visible = (cbUploadSetting.SelectedIndex == (int)AuditScannerDefinition.eUploadSetting.ftp);
            panelEmail.Visible = (cbUploadSetting.SelectedIndex == (int)AuditScannerDefinition.eUploadSetting.email);
            panelTCPIP.Visible = (cbUploadSetting.SelectedIndex == (int)AuditScannerDefinition.eUploadSetting.tcp);

			// If FTP selected then we cannot save a copy to FTP as well
			cbSaveCopy.Visible = bnSaveCopyDetails.Visible = (cbUploadSetting.SelectedIndex != (int)AuditScannerDefinition.eUploadSetting.ftp);			// 8.3.4 CMD
        }

        private void bnSaveCopyDetails_Click(object sender, EventArgs e)
        {
            FormFtpSettingsBackup form = new FormFtpSettingsBackup(auditScannerDefinition);
            form.ShowDialog();
        }

        private void cbSaveCopy_CheckedChanged(object sender, EventArgs e)
        {
            bnSaveCopyDetails.Enabled = cbSaveCopy.Checked;
        }
    }
}
