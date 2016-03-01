using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Drawing;
using System.Data;
using System.IO;
using System.Text;
using System.Windows.Forms;
using Microsoft.Practices.CompositeUI.SmartParts;
using Layton.Cab.Interface;
using Microsoft.Practices.CompositeUI;
using Microsoft.Practices.ObjectBuilder;
//
using Layton.AuditWizard.Common;
using Layton.AuditWizard.DataAccess;

namespace Layton.AuditWizard.Administration
{
    [SmartPart]
    public partial class AuditAgentTabView : UserControl, ILaytonView, IAdministrationView
    {
        private LaytonWorkItem workItem;
		AuditAgentTabViewPresenter presenter;
        AuditScannerDefinition auditAgentScannerDefinition;

        [InjectionConstructor]
		public AuditAgentTabView([ServiceDependency] WorkItem workItem)
        {
            this.workItem = workItem as LaytonWorkItem;
            auditAgentScannerDefinition = new AuditScannerDefinition();
            InitializeComponent();
        }

		[CreateNew]
		public AuditAgentTabViewPresenter Presenter
		{
			set { presenter = value; presenter.View = this; presenter.Initialize(); }
			get { return presenter; }
		}

        /// <summary>
        /// Refresh the default current view
        /// </summary>
        public void RefreshDefaultView()
        {
            base.Refresh();

            AuditScannerDefinition scannerDefinition = auditAgentScannerDefinition;

            // Refresh the fields on this tab - first General Settings
            tbName.Text = scannerDefinition.ScannerName;
            tbDescription.Text = scannerDefinition.Description;
            nupReauditInterval.Value = scannerDefinition.ReAuditInterval;

            // Audit Scanner Location Settings
            cbUploadSetting.SelectedIndex = (int)scannerDefinition.UploadSetting;
            cbSaveCopy.Checked = scannerDefinition.FTPCopyToNetwork;

            // Upload fields
            tbDataFolder.Text = scannerDefinition.DeployPathData;
            tbEmailAddress.Text = scannerDefinition.EmailAddress;
            cbAutoUpload.Checked = scannerDefinition.AutoUpload;

            // Audited Items Settings
            cbAuditHardware.Checked = scannerDefinition.HardwareScan;
            cbAuditRegistry.Checked = scannerDefinition.RegistryScan;
            cbAuditFileSystem.Checked = scannerDefinition.FileSystemScan;
            cbAuditSoftware.Checked = scannerDefinition.SoftwareScan;
        }

        public void RefreshViewSinglePublisher()
        {
        }

		/// <summary>
		/// Refresh the current view
		/// </summary>
		public void RefreshView()
		{
			base.Refresh();

            AdministrationWorkItemController wic = workItem.Controller as AdministrationWorkItemController;
            AuditScannerDefinition scannerDefinition = wic.AuditScannerDefinition;

            auditAgentScannerDefinition = scannerDefinition;

            // Refresh the fields on this tab - first General Settings
            tbName.Text = scannerDefinition.ScannerName;
            tbDescription.Text = scannerDefinition.Description;
            nupReauditInterval.Value = scannerDefinition.ReAuditInterval;

            // Audit Scanner Location Settings
            cbUploadSetting.SelectedIndex = (int)scannerDefinition.UploadSetting;
            cbSaveCopy.Checked = scannerDefinition.FTPCopyToNetwork;

            // Upload fields
            tbDataFolder.Text = scannerDefinition.DeployPathData;
            tbEmailAddress.Text = scannerDefinition.EmailAddress;
            cbAutoUpload.Checked = scannerDefinition.AutoUpload;

            // Audited Items Settings
            cbAuditHardware.Checked = scannerDefinition.HardwareScan;
            cbAuditRegistry.Checked = scannerDefinition.RegistryScan;
            cbAuditFileSystem.Checked = scannerDefinition.FileSystemScan;
            cbAuditSoftware.Checked = scannerDefinition.SoftwareScan;
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
                    string scannerPath = Path.Combine(Application.StartupPath, "scanners") + "\\auditagent\\default.xml";
                    auditAgentScannerDefinition = AuditWizardSerialization.DeserializeObject(scannerPath);
                }
                catch (Exception)
                {
                    // if there was an error here simply log it and load blank config file
                    // JML TODO log this error
                }
            }
            else
            {
                string scannerPath = Path.Combine(Application.StartupPath, "scanners") + "\\auditagent\\" + tbName.Text + ".xml";
                auditAgentScannerDefinition = AuditWizardSerialization.DeserializeObject(scannerPath);
            }

            RefreshDefaultView();
		}

		/// <summary>
		/// save function for the IAdministrationView Interface
		/// </summary>
		public void Save()
		{
            if (!ValidateRequiredValues())
                return;
            
            auditAgentScannerDefinition.ScannerName = tbName.Text;
            auditAgentScannerDefinition.Filename = Path.Combine(Application.StartupPath, @"scanners\auditagent\" + auditAgentScannerDefinition.ScannerName + ".xml");

            if (File.Exists(auditAgentScannerDefinition.Filename))
            {
                if (MessageBox.Show("The config file '" + auditAgentScannerDefinition.ScannerName + "' already exists." + Environment.NewLine + Environment.NewLine +
                    "Do you wish to overwrite the file?", "AuditWizard", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
                    return;
            }
            UpdateChanges();

            if (AuditWizardSerialization.SerializeObjectToFile(auditAgentScannerDefinition))
            {
                DesktopAlert.ShowDesktopAlert("The scanner configuration file '" + auditAgentScannerDefinition.ScannerName + "' has been saved.");
            }
		}

        private bool ValidateRequiredValues()
        {
            List<string> errorStates = new List<string>();

            if (tbName.Text == "<Enter name>" || tbName.Text == String.Empty)
            {
                errorStates.Add("- Scanner Name");
                
            }

            if ( ((cbUploadSetting.SelectedIndex == (int)AuditScannerDefinition.eUploadSetting.network)
                || (cbUploadSetting.SelectedIndex == (int)AuditScannerDefinition.eUploadSetting.removablemedia))
                && tbDataFolder.Text == String.Empty)
            {
                errorStates.Add("- Results Location Data Folder");
            }

            if (cbUploadSetting.SelectedIndex == (int)AuditScannerDefinition.eUploadSetting.ftp)
            {
                if (string.IsNullOrEmpty(auditAgentScannerDefinition.FTPSite))
                {
                    errorStates.Add("- Upload method FTP Site Name or Address");
                }
                if (!auditAgentScannerDefinition.FTPAnonymous)
                {
                    if (string.IsNullOrEmpty(auditAgentScannerDefinition.FTPUser))
                    {
                        errorStates.Add("- Upload method FTP username to log in ");
                    }

                    if (string.IsNullOrEmpty(auditAgentScannerDefinition.FTPPassword))
                    {
                        errorStates.Add("- Upload method FTP password to log in ");
                    }

                }
            }

            if (cbSaveCopy.Checked)
            {
                if (string.IsNullOrEmpty(auditAgentScannerDefinition.FTPSiteBackup))
                {
                    errorStates.Add("- Upload copy FTP Site Name or Address");
                }
                if (!auditAgentScannerDefinition.FTPAnonymousBackup)
                {
                    if (string.IsNullOrEmpty(auditAgentScannerDefinition.FTPUserBackup ))
                    {
                        errorStates.Add("- Upload copy FTP username to log in ");
                    }

                    if (string.IsNullOrEmpty(auditAgentScannerDefinition.FTPPasswordBackup))
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
            //auditAgentScannerDefinition.ScannerName = tbName.Text;
            auditAgentScannerDefinition.Description = tbDescription.Text;
            auditAgentScannerDefinition.ReAuditInterval = (int)nupReauditInterval.Value;
            auditAgentScannerDefinition.FTPCopyToNetwork = cbSaveCopy.Checked;

            // Audit Scanner Location Settings
            auditAgentScannerDefinition.UploadSetting = (AuditScannerDefinition.eUploadSetting)cbUploadSetting.SelectedIndex;

            // Upload fields
            auditAgentScannerDefinition.AutoUpload = cbAutoUpload.Checked;
            auditAgentScannerDefinition.DeployPathData = tbDataFolder.Text;
            auditAgentScannerDefinition.EmailAddress = tbEmailAddress.Text;

            // Audited Items Settings
            auditAgentScannerDefinition.HardwareScan = cbAuditHardware.Checked;
            auditAgentScannerDefinition.FileSystemScan = cbAuditFileSystem.Checked;
			auditAgentScannerDefinition.SoftwareScan = cbAuditSoftware.Checked;
			if (!cbAuditFileSystem.Checked)
                auditAgentScannerDefinition.ScanFolders = AuditScannerDefinition.eFolderSetting.noFolders;
            auditAgentScannerDefinition.RegistryScan = cbAuditRegistry.Checked;

            auditAgentScannerDefinition.IEScan = false;

            UpdateSerialNumberMappings();
            UpdateInteractiveUserDataCategories();

            return auditAgentScannerDefinition;
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

            auditAgentScannerDefinition.SerialNumberMappingsList = lSerialNumberMappingsList;
        }

        private void UpdateInteractiveUserDataCategories()
        {
            auditAgentScannerDefinition.InteractiveUserDataCategories.Clear();

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
                auditAgentScannerDefinition.InteractiveUserDataCategories.Add(lUserDataCategory);
            }
        }

        public LaytonWorkItem WorkItem
        {
            get { return workItem; }
        }


		private void AuditAgentTabView_Load(object sender, EventArgs e)
		{
            // perform some initial setup
            cbUploadSetting.SelectedIndex = 0;
            nupReauditInterval.Value = 7;

            tbName.Text = String.Empty;
		}

        #region Event Handlers

        /// <summary>
        /// Called when the data folder loses focus
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tbDataFolder_LostFocus(object sender, EventArgs e)
        {
            // check if this is a UNC path
            if (tbDataFolder.Text.Length == 0)
            {
                lblUNCWarning.Visible = false;
                tbDataFolder.ForeColor = Color.Black;
            }
            else if (!tbDataFolder.Text.StartsWith("\\"))
            {
                lblUNCWarning.Visible = true;
                tbDataFolder.ForeColor = Color.Red;
            }
            else
            {
                lblUNCWarning.Visible = false;
                tbDataFolder.ForeColor = Color.Black;
            }
        }

        private void tbDataFolder_TextChanged(object sender, EventArgs e)
        {
            // check if this is a UNC path
            if (tbDataFolder.Text.Length == 0)
            {
                lblUNCWarning.Visible = false;
                tbDataFolder.ForeColor = Color.Black;
            }
            else if (!tbDataFolder.Text.StartsWith(@"\\"))
            {
                lblUNCWarning.Visible = true;
                tbDataFolder.ForeColor = Color.Red;
            }
            else
            {
                lblUNCWarning.Visible = false;
                tbDataFolder.ForeColor = Color.Black;
            }
        }

        private void bnAuditFileSystemAdvanced_Click(object sender, EventArgs e)
        {
            FormFileSystemDetails form = new FormFileSystemDetails(auditAgentScannerDefinition);
            form.ShowDialog();
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
            FormFtpSettings form = new FormFtpSettings(auditAgentScannerDefinition);
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
            // Get the current scanner definition
            //AdministrationWorkItemController wic = workItem.Controller as AdministrationWorkItemController;
            //AuditScannerDefinition scannerDefinition = wic.AuditScannerDefinition;
            FormRegistryDetails form = new FormRegistryDetails(auditAgentScannerDefinition);
            form.ShowDialog();
        }

        /// <summary>
        /// Called when we click on the 'Details' button associated with the 'Hardware' checkbox.  This displays a form that
        /// allows the user to select which hardware items to audit
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void bnHardwareAdvanced_Click(object sender, EventArgs e)
        {
            FormHardwareDetails form = new FormHardwareDetails(auditAgentScannerDefinition);
            form.ShowDialog();
        }

        private void bnDesignLayout_Click(object sender, EventArgs e)
        {
            FormAuditDesigner form = new FormAuditDesigner(auditAgentScannerDefinition);
            form.ShowDialog();
        }

        private void cbAuditHardware_CheckedChanged(object sender, EventArgs e)
        {
            bnHardwareAdvanced.Enabled = cbAuditHardware.Checked;
        }

        private void cbAuditRegistry_CheckedChanged(object sender, EventArgs e)
        {
            bnAuditRegistry.Enabled = cbAuditRegistry.Checked;
        }

        private void cbAuditFileSystem_CheckedChanged(object sender, EventArgs e)
        {
            bnAuditFileSystemAdvanced.Enabled = cbAuditFileSystem.Checked;
        }        

        private void cbUploadSetting_SelectedIndexChanged(object sender, EventArgs e)
        {
            panelDataFolder.Visible = ((cbUploadSetting.SelectedIndex == (int)AuditScannerDefinition.eUploadSetting.network)
                        || (cbUploadSetting.SelectedIndex == (int)AuditScannerDefinition.eUploadSetting.removablemedia));

            if (panelDataFolder.Visible)
            {
                // check if this is a UNC path
                if (tbDataFolder.Text.Length == 0)
                {
                    lblUNCWarning.Visible = false;
                    tbDataFolder.ForeColor = Color.Black;
                }
                else if (!tbDataFolder.Text.StartsWith(@"\\"))
                {
                    lblUNCWarning.Visible = true;
                    tbDataFolder.ForeColor = Color.Red;
                }
                else
                {
                    lblUNCWarning.Visible = false;
                    tbDataFolder.ForeColor = Color.Black;
                }
            }
            else
            {
                lblUNCWarning.Visible = false;
            }


            bnBrowseSingle.Visible = ((cbUploadSetting.SelectedIndex == (int)AuditScannerDefinition.eUploadSetting.network)
            || (cbUploadSetting.SelectedIndex == (int)AuditScannerDefinition.eUploadSetting.removablemedia));

            bnUploadViaFTP.Visible = (cbUploadSetting.SelectedIndex == (int)AuditScannerDefinition.eUploadSetting.ftp);
            panelEmail.Visible = (cbUploadSetting.SelectedIndex == (int)AuditScannerDefinition.eUploadSetting.email);
            panelTCPIP.Visible = (cbUploadSetting.SelectedIndex == (int)AuditScannerDefinition.eUploadSetting.tcp);
        }

        private void bnBrowseSingle_Click(object sender, EventArgs e)
        {
            folderBrowserDialog1.Description = "Please select a folder.";
            if (this.folderBrowserDialog1.ShowDialog() == DialogResult.OK)
            {
                tbDataFolder.Text = folderBrowserDialog1.SelectedPath;

                // check if this is a UNC path
                if (!tbDataFolder.Text.StartsWith("\\"))
                {
                    lblUNCWarning.Visible = true;
                    tbDataFolder.ForeColor = Color.Red;
                }
                else
                {
                    lblUNCWarning.Visible = false;
                    tbDataFolder.ForeColor = Color.Black;
                }
            }
        }

        #endregion
        
        private void bnSaveCopyDetails_Click(object sender, EventArgs e)
        {
            FormFtpSettingsBackup form = new FormFtpSettingsBackup(auditAgentScannerDefinition);
            form.ShowDialog();
        }

        private void cbSaveCopy_CheckedChanged(object sender, EventArgs e)
        {
            bnSaveCopyDetails.Enabled = cbSaveCopy.Checked;
		}

		private void bnSoftwareAdvanced_Click(object sender, EventArgs e)
		{
			FormSoftwareDetails form = new FormSoftwareDetails(auditAgentScannerDefinition);
			form.ShowDialog();
		}

		private void cbAuditSoftware_CheckedChanged(object sender, EventArgs e)
		{
			this.bnSoftwareAdvanced.Enabled = cbAuditSoftware.Checked;
		}
    }
}
