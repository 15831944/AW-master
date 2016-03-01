using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

using System.Security.AccessControl;
using System.Security.Principal;

using PickerSample;
//
using Infragistics.Win.UltraWinTree;
//
using Microsoft.Practices.CompositeUI;
using Microsoft.Practices.CompositeUI.Utility;
using Microsoft.Practices.CompositeUI.SmartParts;

using Layton.Cab.Interface;
using Layton.NetworkDiscovery;
using Layton.Common.Controls;
using Layton.AuditWizard.Common;
using Layton.AuditWizard.Network;
using Layton.AuditWizard.DataAccess;

namespace Layton.AuditWizard.Overview
{
    public partial class FormStartupWizard : Form
    {
        #region data

        /// <summary>Enumeration for the wizard steps</summary>
        private enum WIZARDSTEPS { welcome = 0, userdetails, auditmypc, discovertype, dodiscover, basicinformation, servicecredentials, audittype, logonscript, selectdeploy, finish };

        /// <summary>List of computers to deploy to</summary>
        private List<Asset> _listComputers = new List<Asset>();

        /// <summary>Network Work Item Controller used for agent deployment</summary>
        private NetworkWorkItemController _networkWorkItemController;

        /// <summary>work Item Controller</summary>
        private OverviewWorkItemController _OverviewWorkItemController;
        private NetworkDiscoveryWorkItemController _networkDiscoveryWorkItemController;

        /// <summary>The Scanner Configuration file itself</summary>
        private AuditScannerDefinition _auditScannerDefinition;

        /// <summary>The AuditWizard Configuration file</summary>
        private AuditWizardConfiguration _AuditWizardConfiguration = new AuditWizardConfiguration();

        // The active smart part on entry to this wizard
        ILaytonView _activeTabView;

        bool _showByDomain = true;
        #endregion data

        #region Constructor

        public FormStartupWizard(OverviewWorkItemController OverviewWorkItemController, NetworkWorkItemController networkWorkItemController, NetworkDiscoveryWorkItemController networkDiscoveryWorkItemController)
        {
            InitializeComponent();
            _OverviewWorkItemController = OverviewWorkItemController;
            _networkDiscoveryWorkItemController = networkDiscoveryWorkItemController;
            _networkWorkItemController = networkWorkItemController;
        }
        #endregion Constructor

        #region Form Control


        /// <summary>
        /// Called as the form is loaded - perform any initialization of the wizard here
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FormStartupWizard_Load(object sender, EventArgs e)
        {
            // Load the current profile
            //_auditScannerDefinition.ReadCurrent();

            Application.UseWaitCursor = false;

            string scannerPath = Path.Combine(Application.StartupPath, "scanners") + "\\default.xml";

            // check if the default.xml file exists, if not we should create it
            if (!File.Exists(scannerPath))
            {
                AuditScannerDefinition defaultConfiguration = new AuditScannerDefinition();
                defaultConfiguration.Description = "Default Scanner Configuration";
                defaultConfiguration.Filename = scannerPath;

                // basic scan
                defaultConfiguration.HardwareScan = true;
                defaultConfiguration.SoftwareScanApplications = true;
                defaultConfiguration.SoftwareScanOs = true;

                AuditWizardSerialization.SerializeObjectToFile(defaultConfiguration, scannerPath);
                _auditScannerDefinition = defaultConfiguration;
            }
            else
            {
                _auditScannerDefinition = AuditWizardSerialization.DeserializeObject(scannerPath);
            }

            // Save the current smart part so that it can be restored once we have finished the network discovery
            _activeTabView = (ILaytonView)_OverviewWorkItemController.WorkItem.RootWorkItem.Workspaces[WorkspaceNames.TabWorkspace].ActiveSmartPart;

            // Initialize User Details
            InitializeUserDetails();

            // Initialize Basic Information Tab
            InitializeBasicInformationTab();

            // Initialize Service Credentials Tab
            InitializeServiceCredentialsTab();
        }



        /// <summary>
        /// Called as the form is closing - if we are not at the finish page then let the user back-out of 
        /// closing the wizard as it is not yet complete
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FormStartupWizard_FormClosing(object sender, FormClosingEventArgs e)
        {
            // Ensure that any changes to the scanner configuration are flushed to disk
            //_auditScannerDefinition.Save(false);

            //string scannerPath = Path.Combine(Application.StartupPath, "scanners");
            //scannerPath += "\\default.xml";

            //AuditWizardSerialization.SerializeObjectToFile(_auditScannerDefinition, scannerPath);

            // ...and also the AuditWizard configuration
            _AuditWizardConfiguration.SaveConfiguration();
        }



        /// <summary>
        /// Called as the 'Next' button is clicked to perform any necessary initialiation of the page
        /// that will be displayed next
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void wizardControl_NextButtonClick(WizardBase.WizardControl sender, WizardBase.WizardNextButtonClickEventArgs args)
        {

            // The flow of pages is as follows:
            //
            //   Welcome -> User Details -> Ask Discover -> Discover Type -> DoDiscover ->
            //		Basic Information -> Service credentials -> Audit My PC -> Audit Type -> Logon Script -> 
            //      Select to Deploy -> Finish
            //
            //	Alternate flow:
            //		Ask Discover -> Basic Information					(discovery not requested)
            //		Ask Discover -> Discover Type						(discovery requested)
            //		Audit Type -> Logon Script -> Finish				(if a logon script audit)
            //		Audit Type -> Select to Deploy 						(if AuditAgent audit)
            //
            switch (wizardControl.CurrentStepIndex)
            {
                case (int)WIZARDSTEPS.welcome:
                    break;

                case (int)WIZARDSTEPS.userdetails:
                    SaveUserDetails();
                    InitializeAuditNowTab();
                    wizardControl.NextButtonText = "&Skip >";
                    break;

                case (int)WIZARDSTEPS.auditmypc:
                    wizardControl.NextButtonText = "&Skip >";
                    wizardControl.CancelButtonText = "&Cancel";
                    break;

                //case (int)WIZARDSTEPS.askdiscover:
                //    if (rdNoDiscover.Checked)
                //        args.NextStepIndex = (int)WIZARDSTEPS.basicinformation;
                //    break;

                case (int)WIZARDSTEPS.discovertype:
                    //PerformNetworkDiscoveryStep();
                    //wizardControl.CurrentStepIndex = (int)WIZARDSTEPS.dodiscover;
                    wizardControl.NextButtonText = "&Next >";
                    wizardControl.CancelButtonText = "&Cancel";
                    args.NextStepIndex = (int)WIZARDSTEPS.basicinformation;
                    break;

                case (int)WIZARDSTEPS.dodiscover:
                    wizardControl.NextButtonText = "&Next >";
                    break;

                case (int)WIZARDSTEPS.basicinformation:
                    if (!SaveBasicInformationTab())
                        args.Cancel = true;
                    break;

                case (int)WIZARDSTEPS.servicecredentials:
                    if (!ValidateServiceCredentialsTab())
                        args.Cancel = true;
                    break;

                case (int)WIZARDSTEPS.audittype:
                    if (rdAuditAgent.Checked)
                    {
                        logonScriptLabel.Visible = false;
                        agentLabel.Visible = true;

                        // Initialize the deployment tab and set this to be the next tab
                        InitializeDeployTab();
                        args.NextStepIndex = (int)WIZARDSTEPS.selectdeploy;
                    }
                    else
                    {
                        logonScriptLabel.Visible = true;
                        agentLabel.Visible = false;
                        InitializeLogonScript();
                    }
                    break;

                case (int)WIZARDSTEPS.logonscript:
                    args.NextStepIndex = (int)WIZARDSTEPS.finish;
                    break;

                case (int)WIZARDSTEPS.selectdeploy:
                    DoDeployment();
                    break;

                default:
                    break;
            }
        }



        /// <summary>
        /// Called as we skip back through the wizard pages
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void wizardControl_BackButtonClick(WizardBase.WizardControl sender, WizardBase.WizardClickEventArgs args)
        {
            // The flow of pages is as follows:
            //
            //   Welcome -> User Details -> Audit My PC -> Ask Discover -> Discover Type -> DoDiscover ->
            //		Basic Information -> Audit Type -> Logon Script -> Select to Deploy -> Finish
            //
            //	Alternate flow:
            //		Ask Discover -> Basic Information					(discovery not requested)
            //		Ask Discover -> Discover Type						(discovery requested)
            //		Audit Type -> Logon Script -> Finish				(if a logon script audit)
            //		Audit Type -> Select to Deploy						(if AuditAgent audit)
            //
            switch (wizardControl.CurrentStepIndex)
            {
                case (int)WIZARDSTEPS.finish:
                    if (this.rdLogonScript.Checked)
                    {
                        wizardControl.CurrentStepIndex = (int)WIZARDSTEPS.logonscript;
                        args.Cancel = true;
                    }
                    break;

                case (int)WIZARDSTEPS.auditmypc:
                    wizardControl.NextButtonText = "&Next >";
                    break;

                case (int)WIZARDSTEPS.discovertype:
                    wizardControl.NextButtonText = "&Skip >";
                    break;

                //case (int)WIZARDSTEPS.askdiscover:
                //    InitializeAuditNowTab();
                //    break;

                case (int)WIZARDSTEPS.basicinformation:
                    //if (rdNoDiscover.Checked)
                    //{
                    //    wizardControl.CurrentStepIndex = (int)WIZARDSTEPS.askdiscover;
                    //    args.Cancel = true;
                    //}
                    //else
                    //{
                    wizardControl.NextButtonText = "&Skip >";
                    wizardControl.CurrentStepIndex = (int)WIZARDSTEPS.discovertype;
                    args.Cancel = true;
                    //}
                    break;

                default:
                    break;
            }
        }


        /// <summary>
        /// Handle canceling from the wizard
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void wizardControl_CancelButtonClick(object sender, EventArgs e)
        {
            Close();
        }


        private void wizardControl_FinishButtonClick(object sender, EventArgs e)
        {
            string scannerPath = Path.Combine(Application.StartupPath, "scanners");
            scannerPath += "\\default.xml";

            AuditWizardSerialization.SerializeObjectToFile(_auditScannerDefinition, scannerPath);

            // Close the wizard
            Close();
        }


        /// <summary>
        /// Start the AuditWizard service
        /// </summary>
        protected bool StartAuditWizardService()
        {
            // Create the Service Controller object and get its current status
            AuditWizardServiceController serviceController = new AuditWizardServiceController();
            LaytonServiceController.ServiceStatus serviceStatus = serviceController.CheckStatus();

            // If the service is installed and/or active we need tio remove it first before we can change
            // the settings for it - ask the user first though if this is the case
            if (serviceStatus != LaytonServiceController.ServiceStatus.NotInstalled)
            {
                if (MessageBox.Show("The AuditWizard Service is already installed and must be removed before the credentials can be updated.  Would you like to remove and re-install the service now?", "Service Installed", MessageBoxButtons.YesNo) == DialogResult.No)
                    return true;

                // The user wants to re-install so stop if necessary and then remove
                using (new WaitCursor())
                {
                    if (serviceStatus == LaytonServiceController.ServiceStatus.Running)
                        serviceController.Stop();

                    // Remove now that it is stopped	
                    if (!serviceController.Remove())
                    {
                        MessageBox.Show("Failed to remove the AuditWizard Service", "Service Control Error");
                        return false;
                    }
                }
            }

            // Install the service using the specified credentials
            serviceController.ResetCredentials();

            using (new WaitCursor())
            {
                // Now install the service (Under the specified username / password) unless it is already
                // installed
                try
                {
                    serviceController.Install();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(String.Format("Failed to install the AuditWizard Service, the error was {0}", ex.Message), "Service Control Error");
                    return false;
                }

                // OK now start the service
                bool started = false;

                try
                {
                    serviceController.Start();

                    for (int index = 0; index < 5; index++)
                    {
                        if (serviceController.CheckStatus() == LaytonServiceController.ServiceStatus.Running)
                        {
                            started = true;
                            break;
                        }

                        System.Threading.Thread.Sleep(1000);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(String.Format("Failed to start the AuditWizard Service, the error was {0}", ex.Message), "Service Control Error");
                    return false;
                }

                if (!started)
                {
                    lblServiceFailed.Visible = true;
                    return false;
                }
            }

            return true;
        }

        #endregion Form Control

        #region User Details Step Functions

        private void hideWizardCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.DoNotShowWizard = hideWizardCheckBox.Checked;
            Properties.Settings.Default.Save();
        }

        private void InitializeUserDetails()
        {
            // Pickup any user details from the configuration file
            //tbOrganization.Text = _AuditWizardConfiguration.Organization;
            tbOrganization.Text = new AssetGroup(new LocationsDAO().GetRootLocation().Rows[0], AssetGroup.GROUPTYPE.userlocation).Name;

            tbUser.Text = _AuditWizardConfiguration.Name == String.Empty ? Environment.UserName : _AuditWizardConfiguration.Name;
        }


        private void userdetails_TextChanged(object sender, EventArgs e)
        {
            bool canContinue = ((tbOrganization.Text != "") && (tbUser.Text != ""));
            this.wizardControl.NextButtonEnabled = canContinue;
        }


        private void bnEmailConfiguration_Click(object sender, EventArgs e)
        {
            FormEmailConfiguration form = new FormEmailConfiguration();
            form.ShowDialog();
        }

        /// <summary>
        /// Caled as we move off the User Details Step to save any changes
        /// </summary>
        private void SaveUserDetails()
        {
            _AuditWizardConfiguration.Organization = tbOrganization.Text;
            _AuditWizardConfiguration.Name = tbUser.Text;
            _AuditWizardConfiguration.SaveConfiguration();

            // The Organization is used as the root for both the domains and locations tables
            LocationsDAO lLocationsDAO = new LocationsDAO();
            lLocationsDAO.SetOrganization(tbOrganization.Text);

            AssetGroup rootLocation = new AssetGroup(lLocationsDAO.GetRootLocation().Rows[0], AssetGroup.GROUPTYPE.userlocation);
            lLocationsDAO.UpdateChildLocationsFullName(rootLocation.GroupID);
        }

        #endregion User Details Step Functions

        #region Select Audit Type Tab

        #endregion Select Audit Type Tab

        #region Basic Information Tab


        /// <summary>
        /// Initialization function for the 'Basic Information' tab
        /// </summary>
        private void InitializeBasicInformationTab()
        {
            //tbAuditFolder.Text = _auditScannerDefinition.DeployPathRoot;
            if (_auditScannerDefinition != null) nupReauditInterval.Value = _auditScannerDefinition.ReAuditInterval;
        }

        /// <summary>
        /// Called as we click browse to select a path for the scanner
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void bnBrowse_Click(object sender, EventArgs e)
        {
            folderBrowserDialog1.Description = "Please select the root folder for the audit scanner";
            if (this.folderBrowserDialog1.ShowDialog() == DialogResult.OK)
                this.tbAuditFolder.Text = folderBrowserDialog1.SelectedPath;
        }

        /// <summary>
        /// Called as move off the single folder  so that we can validate the path specified
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tbAuditFolder_Leave(object sender, EventArgs e)
        {
            if ((!tbAuditFolder.Text.StartsWith(@"\\")) && tbAuditFolder.Text.Length > 0)
                lblUNCWarning.Visible = true;
        }

        /// <summary>
        /// Called as the path text is changed so that we can validate the path specified
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tbAuditFolder_TextChanged(object sender, EventArgs e)
        {
            if ((!tbAuditFolder.Text.StartsWith(@"\\")) && tbAuditFolder.Text.Length > 0)
                lblUNCWarning.Visible = true;
            else
                lblUNCWarning.Visible = false;
        }


        /// <summary>
        /// Called as we exit this form to force all values to be saved back to the auditScannerDefinition
        /// internal object
        /// </summary>
        private bool SaveBasicInformationTab()
        {
            if (tbAuditFolder.Text == String.Empty)
            {
                Utility.DisplayApplicationErrorMessage("Please enter a value for the scanner audit folder.");
                return false;
            }

            if (_auditScannerDefinition == null)
                return false;

            _auditScannerDefinition.ReAuditInterval = (int)nupReauditInterval.Value;
            _auditScannerDefinition.DeployPathSingle = true;
            _auditScannerDefinition.DeployPathRoot = tbAuditFolder.Text;

            // check permissions
            //if (!CheckFolderPermissions(tbAuditFolder.Text))
            //    MessageBox.Show(
            //        "The folder you have specified does not appear to allow Full Control access to Everyone." + Environment.NewLine + Environment.NewLine);

            // create the data and scanner folders
            try
            {
                if (!Directory.Exists(_auditScannerDefinition.DeployPathData))
                    Directory.CreateDirectory(_auditScannerDefinition.DeployPathData);

                if (!Directory.Exists(_auditScannerDefinition.DeployPathScanner))
                    Directory.CreateDirectory(_auditScannerDefinition.DeployPathScanner);
            }
            catch (UnauthorizedAccessException)
            {
                MessageBox.Show("Access denied to folder '" + _auditScannerDefinition.DeployPathRoot + "'" +
                Environment.NewLine + Environment.NewLine + 
                "To continue, please enable read/write access on this folder for all possible user accounts.",
                "Access denied",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
                return false;
            }

            // need to update AuditScanner config file
            _auditScannerDefinition.Filename = Path.Combine(Application.StartupPath, @"scanners\default.xml");

            string scannerPath = Path.Combine(Application.StartupPath, "scanners") + "\\default.xml";
            AuditWizardSerialization.SerializeObjectToFile(_auditScannerDefinition, scannerPath);

            // AND AuditAgent config file
            _auditScannerDefinition.Filename = Path.Combine(Application.StartupPath, @"scanners\auditagent\default.xml");

            scannerPath = Path.Combine(Application.StartupPath, "scanners") + "\\auditagent\\default.xml";
            AuditWizardSerialization.SerializeObjectToFile(_auditScannerDefinition, scannerPath);

            // ...and return
            return true;
        }

        private bool CheckFolderPermissions(string aFolderPath)
        {
            NTAccount everyoneAccount = new NTAccount("Everyone");
            SecurityIdentifier securityIdentifier = everyoneAccount.Translate(typeof(SecurityIdentifier)) as SecurityIdentifier;
            DirectoryInfo directoryInfo = new DirectoryInfo(aFolderPath);
            DirectorySecurity directorySecurity = directoryInfo.GetAccessControl();
            AuthorizationRuleCollection rules = directorySecurity.GetAccessRules(true, true, typeof(SecurityIdentifier));

            // Iterate through the rules and see if we can find an entry for everyone
            //bool everyoneOK = false;
            foreach (FileSystemAccessRule accessRule in rules)
            {
                if (securityIdentifier.CompareTo(accessRule.IdentityReference as SecurityIdentifier) == 0)
                {
                    // There is an entry for 'Everyone' - what is it specifying though?
                    if ((accessRule.FileSystemRights | FileSystemRights.FullControl) != 0)
                        return true;
                }
            }

            return false;

            // If everyone does not have full control then warn the user and ask if we should add it
            //if (!everyoneOK)
            //    AddDirectorySecurity(aFolderPath, @"Everyone", FileSystemRights.FullControl, AccessControlType.Allow);

        }


        // Adds an ACL entry on the specified directory for the specified account.
        protected void AddDirectorySecurity(string FileName, string Account, FileSystemRights Rights, AccessControlType ControlType)
        {
            // Create a new DirectoryInfo object.
            DirectoryInfo dInfo = new DirectoryInfo(FileName);

            // Get a DirectorySecurity object that represents the 
            // current security settings.
            DirectorySecurity dSecurity = dInfo.GetAccessControl();

            // Add the FileSystemAccessRule to the security settings. 
            dSecurity.AddAccessRule(new FileSystemAccessRule(Account,
                                                            Rights,
                                                            ControlType));

            // Set the new access settings.
            dInfo.SetAccessControl(dSecurity);
        }


        #endregion Basic Information Tab

        #region AuditWizard Service Credentials Tab Functions

        /// <summary>
        /// Initialize fields on the Credentials tab
        /// </summary>
        private void InitializeServiceCredentialsTab()
        {
            Configuration config = ConfigurationManager.OpenExeConfiguration(Path.Combine(Application.StartupPath, "AuditWizardv8.exe"));
            cbServiceLocalSystem.Checked = Convert.ToBoolean(config.AppSettings.Settings["auditwizardservice_runassystem"].Value);
            if (!cbServiceLocalSystem.Checked)
            {
                tbServiceUsername.Text = config.AppSettings.Settings["auditwizardservice_username"].Value;
                string password = config.AppSettings.Settings["auditwizardservice_password"].Value;
                if (password != "")
                    password = AES.Decrypt(password);
                tbServicePassword.Text = password;
                tbConfirmServicePassword.Text = password;
            }
        }


        /// <summary>
        /// Called as we change the check state of the 'Use System Account' setting
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cbServiceLocalSystem_CheckedChanged(object sender, EventArgs e)
        {
            panelServiceRunSystemAs.Enabled = !cbServiceLocalSystem.Checked;
        }


        /// <summary>
        /// Validate and save the details entered on this tab
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private bool ValidateServiceCredentialsTab()
        {
            // Save any changes made...
            Configuration config = ConfigurationManager.OpenExeConfiguration(Path.Combine(Application.StartupPath, "AuditWizardv8.exe"));
            config.AppSettings.Settings["auditwizardservice_runassystem"].Value = (cbServiceLocalSystem.Checked) ? "True" : "False";

            if (!cbServiceLocalSystem.Checked)
            {
                // We must have at least a username specified
                if (tbServiceUsername.Text == "")
                {
                    MessageBox.Show("You must specify at least the username for the service to run under", "Validation Error");
                    tbServiceUsername.Focus();
                    return false;
                }

                config.AppSettings.Settings["auditwizardservice_username"].Value = tbServiceUsername.Text;

                // Ensure that (any) passwords entered match
                if (tbServicePassword.Text != tbConfirmServicePassword.Text)
                {
                    MessageBox.Show("The specified passwords do not match", "Validation Error");
                    tbConfirmServicePassword.Focus();
                    return false;
                }

                // Set the password noting that it is encrypted
                string password = tbServicePassword.Text;
                if (password != "")
                    config.AppSettings.Settings["auditwizardservice_password"].Value = AES.Encrypt(password);
                else
                    config.AppSettings.Settings["auditwizardservice_password"].Value = "";

                // If we have not (as yet) specified anything for the AuditAgent credentials then we should
                // set them to be the same as the AuditWizard service
                //if (config.AppSettings.Settings["agent_username"].Value == "")
                config.AppSettings.Settings["agent_username"].Value = config.AppSettings.Settings["auditwizardservice_username"].Value;
                //if (config.AppSettings.Settings["agent_password"].Value == "")
                config.AppSettings.Settings["agent_password"].Value = config.AppSettings.Settings["auditwizardservice_password"].Value;
            }

            // Save settings
            config.AppSettings.Settings["agent_runassystem"].Value = config.AppSettings.Settings["auditwizardservice_runassystem"].Value;
            config.Save();

            // Ensure that the AuditWizards ervice is running
            return StartAuditWizardService();
        }


        /// <summary>
        /// Called to allow the user to select a user from AD
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void bnServiceBrowseAD_Click(object sender, EventArgs e)
        {
            // show the object picker
            ADPicker picker = CreateAdPicker();
            DialogResult result;
            try
            {
                result = picker.ShowDialog();
            }
            catch
            {
                // the picker throws an exception when there are no locations available...ignore
                return;
            }
            if (DialogResult.OK == result)
                tbServiceUsername.Text = picker.ReturnValues[0].ToString();
        }


        /// <summary>
        /// Create an Active Directory picker object
        /// </summary>
        /// <returns></returns>
        private ADPicker CreateAdPicker()
        {
            // Initialize the Users and Groups picker with common settings
            ADPicker usersAndGroupsPicker = new ADPicker();
            usersAndGroupsPicker.DownlevelFilter = adPickerDownlevelFilter.DSOP_DOWNLEVEL_FILTER_USERS | adPickerDownlevelFilter.DSOP_DOWNLEVEL_FILTER_GLOBAL_GROUPS;
            usersAndGroupsPicker.ScopeFormat = adPickerScopeFormat.DSOP_SCOPE_FLAG_WANT_PROVIDER_LDAP
                | adPickerScopeFormat.DSOP_SCOPE_FLAG_WANT_PROVIDER_WINNT
                | adPickerScopeFormat.DSOP_SCOPE_FLAG_STARTING_SCOPE;
            usersAndGroupsPicker.ScopeType = ((adPickerScopeType)((((((((adPickerScopeType.DSOP_SCOPE_TYPE_UPLEVEL_JOINED_DOMAIN | adPickerScopeType.DSOP_SCOPE_TYPE_DOWNLEVEL_JOINED_DOMAIN)
                        | adPickerScopeType.DSOP_SCOPE_TYPE_ENTERPRISE_DOMAIN)
                        | adPickerScopeType.DSOP_SCOPE_TYPE_GLOBAL_CATALOG)
                        | adPickerScopeType.DSOP_SCOPE_TYPE_EXTERNAL_UPLEVEL_DOMAIN)
                        | adPickerScopeType.DSOP_SCOPE_TYPE_EXTERNAL_DOWNLEVEL_DOMAIN)
                        | adPickerScopeType.DSOP_SCOPE_TYPE_USER_ENTERED_UPLEVEL_SCOPE)
                        | adPickerScopeType.DSOP_SCOPE_TYPE_USER_ENTERED_DOWNLEVEL_SCOPE)));
            usersAndGroupsPicker.Options = adPickerOptions.DSOP_FLAG_SKIP_TARGET_COMPUTER_DC_CHECK;

            // set the filter for searching for users or groups
            usersAndGroupsPicker.UplevelFilter = adPickerUplevelFilter.DSOP_FILTER_USERS;
            usersAndGroupsPicker.ReturnFormat = adPickerReturnFormat.UPN;
            return usersAndGroupsPicker;
        }

        #endregion AuditAgent Credentials Tab Functions

        #region Audit My PC Tab Functions

        private void InitializeAuditNowTab()
        {
            // Set label state to allow the user to request an audit
            labelAuditNow.Visible = true;
            labelAuditing.Visible = false;
            labelAudited.Visible = false;
            //
            bnAuditNow.Visible = true;
            pbAuditing.Visible = false;
            pbAuditing.Image = Properties.Resources.progress;
        }


        /// <summary>
        /// Called as we click to audit the current PC
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void bnAuditNow_Click(object sender, EventArgs e)
        {
            if (backgroundWorkerAudit.IsBusy)
            {
                MessageBox.Show("Another audit is currently taking place. Please wait a few moments and try again.");
                return;
            }

            labelAuditNow.Visible = false;
            labelAuditing.Visible = true;
            labelAudited.Visible = false;
            //
            bnAuditNow.Visible = false;
            pbAuditing.Visible = true;

            // Disable back and next while we do the audit
            wizardControl.BackButtonEnabled = false;
            wizardControl.NextButtonEnabled = false;
            Refresh();

            // Start the thread 
            backgroundWorkerAudit.RunWorkerAsync();

            for (int i = 0; i < 50; i++)
            {
                System.Threading.Thread.Sleep(100);
                Application.DoEvents();
            }

            labelAuditNow.Visible = false;
            labelAuditing.Visible = false;
            labelAudited.Visible = true;
            //
            pbAuditing.Image = Properties.Resources.computer_audited96;
            //
            wizardControl.BackButtonEnabled = true;
            wizardControl.NextButtonEnabled = true;
            wizardControl.CancelButtonText = "Finish";
            wizardControl.NextButtonText = "&Next >";
        }


        /// <summary>
        /// This is the thread which will audit the local PC
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void backgroundWorkerAudit_DoWork(object sender, DoWorkEventArgs e)
        {
            _auditScannerDefinition.AuditMyPC();
        }

        /// <summary>
        /// Called as the audit thread completes to allow the user to continue
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void backgroundWorkerAudit_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            labelAuditNow.Visible = false;
            labelAuditing.Visible = false;
            labelAudited.Visible = true;
            //
            pbAuditing.Image = Properties.Resources.computer_audited96;
            //
            wizardControl.BackButtonEnabled = true;
            wizardControl.NextButtonEnabled = true;
            wizardControl.CancelButtonText = "Finish";
            wizardControl.NextButtonText = "&Next >";
        }

        #endregion Audit My PC Tab Functions

        #region Logon Script Functions

        /// <summary>
        /// Called as we move to the Loon Script page.  We need to ensure that the various script 
        /// commands are set up correctly
        /// </summary>
        public void InitializeLogonScript()
        {
            string scannerPath = Path.Combine(_auditScannerDefinition.DeployPathScanner, "AuditScanner.exe");

            // Build the scanner to the specified folder
            if (_auditScannerDefinition.Deploy(false))
                //MessageBox.Show("The Audit Scanner has been deployed to " + _auditScannerDefinition.DeployPathScanner, "Deployment Successful");
                DesktopAlert.ShowDesktopAlert("The Audit Scanner has been deployed to " + _auditScannerDefinition.DeployPathScanner);

            // Pre-load the script command text boxes with the appropriate command
            string windowsCommand = "Start " + scannerPath;
            string novellCommand = "@" + scannerPath;
            tbWindowsLogonCommand.Text = windowsCommand;
            tbNovellLogonCommand.Text = novellCommand;
        }

        #endregion Logon Script Functions

        #region Network Discovery functions

        /// <summary>
        /// Called as we click to audit the current PC
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void bnDiscover_Click(object sender, EventArgs e)
        {
            bnDiscoverNow.Visible = false;
            pbDiscovering.Visible = true;

            lbDiscIntro.Visible = false;
            lblProgressInfo.Visible = true;
            lbIPRange.Visible = true;

            System.Collections.Specialized.NameValueCollection ipRanges = Utility.GetComputerIpRanges();
            string[] startIp = ipRanges.AllKeys;
            lbIPRange.Text = String.Format("Scanning IP range: {0} - {1}", startIp[0], ipRanges[startIp[0]]);

            // Disable back and next while we do the audit
            wizardControl.BackButtonEnabled = false;
            wizardControl.NextButtonEnabled = false;

            PerformNetworkDiscoveryStep();
        }

        /// <summary>
        /// Perform the network discovery
        /// </summary>
        private void PerformNetworkDiscoveryStep()
        {
            // Get the work item
            WorkItem workItem = _networkWorkItemController.WorkItem;

            // Get references to the NetworkDiscovery Module's relevent objects
            List<WorkItem> workItemList = (List<WorkItem>)workItem.RootWorkItem.WorkItems.FindByType(typeof(NetworkDiscoveryWorkItem));
            if (workItemList.Count > 0)
            {
                // Disable back and next while we do the discover
                wizardControl.BackButtonVisible = false;
                wizardControl.NextButtonEnabled = false;

                // Get the various controllers we will need during the discovery 
                NetworkDiscoveryWorkItem netDiscWorkItem = workItemList[0] as NetworkDiscoveryWorkItem;
                NetworkDiscoveryWorkItemController controller = netDiscWorkItem.Controller as NetworkDiscoveryWorkItemController;

                // ...and set the completion handler - this function will be called as the discovery process completes
                // and is responsible for adding the assets and groups to the database
                controller.NetworkDiscoveryComplete += new EventHandler<DataEventArgs<List<string[]>>>(controller_NetworkDiscoveryComplete);
                controller.ActivateWorkItem();

                controller.ChangeView = false;
                controller.RunBothDiscovery = true;
                controller.RunTcpipNetworkDiscovery();
                _OverviewWorkItemController.SetTabView(_activeTabView);
            }
        }


        private delegate void CompleteDiscoveringStepDelegate();
        private void CompleteDiscoveringStep()
        {
            if (InvokeRequired)
            {
                this.BeginInvoke(new CompleteDiscoveringStepDelegate(CompleteDiscoveringStep));
                return;
            }

            _networkWorkItemController.ActivateWorkItem();

            WorkItem workItem = _networkWorkItemController.WorkItem;
            List<WorkItem> workItemList = (List<WorkItem>)workItem.RootWorkItem.WorkItems.FindByType(typeof(NetworkDiscoveryWorkItem));
            NetworkDiscoveryWorkItem netDiscWorkItem = workItemList[0] as NetworkDiscoveryWorkItem;
            NetworkDiscoveryWorkItemController controller = netDiscWorkItem.Controller as NetworkDiscoveryWorkItemController;

            controller.ChangeView = true;

            lbDiscIntro.Visible = false;
            lbIPRange.Visible = false;
            lblProgressInfo.Visible = false;
            lblDiscComplete.Visible = true;

            pbDiscovering.Image = Properties.Resources.computer_audited96;

            wizardControl.BackButtonVisible = true;
            wizardControl.BackButtonEnabled = true;
            wizardControl.NextButtonEnabled = true;
            wizardControl.NextButtonText = "&Next >";
            wizardControl.CancelButtonText = "Finish";
        }


        /// <summary>
        /// This function is called from the network discovery process when that process completes
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void controller_NetworkDiscoveryComplete(object sender, DataEventArgs<List<string[]>> e)
        {
            WorkItem workItem = _networkWorkItemController.WorkItem;

            // Complete the discovery process
            CompleteDiscoveringStep();

            List<WorkItem> workItemList = (List<WorkItem>)workItem.RootWorkItem.WorkItems.FindByType(typeof(NetworkDiscoveryWorkItem));
            if (workItemList.Count > 0)
            {
                NetworkDiscoveryWorkItem netDiscWorkItem = workItemList[0] as NetworkDiscoveryWorkItem;
                NetworkDiscoveryWorkItemController controller = netDiscWorkItem.Controller as NetworkDiscoveryWorkItemController;
                controller.NetworkDiscoveryComplete -= controller_NetworkDiscoveryComplete;
            }
        }

        #endregion Network Discovery Steps

        #region Deploy Tab


        /// <summary>
        /// Initialize fields on the Deployment tab
        /// </summary>
        private void InitializeDeployTab()
        {
            // Get the current display style
            AuditWizardConfiguration configuration = new AuditWizardConfiguration();
            _showByDomain = configuration.ShowByDomain;

            // Flag that we should begin updating the tree
            deploymentTree.BeginUpdate();
            this.Cursor = Cursors.WaitCursor;

            // Recover the appropriate root item from the database
            LocationsDAO lwDataAccess = new LocationsDAO();
            AssetGroup rootGroup = new AssetGroup(AssetGroup.GROUPTYPE.domain);
            DataTable table = lwDataAccess.GetGroups(rootGroup);
            if (table.Rows.Count == 0)
                return;
            rootGroup = new AssetGroup(table.Rows[0], AssetGroup.GROUPTYPE.domain);

            // ...and populate it fully with all child groups and items
            rootGroup.Populate(true, false, true);

            // Clear the current deployment tree
            deploymentTree.Nodes.Clear();

            // Add a new root node to the tree
            UltraTreeNode rootNode = new UltraTreeNode("root>" + rootGroup.FullName, rootGroup.Name);
            Bitmap rootImage = (_showByDomain) ? Properties.Resources.domain16 : Properties.Resources.location_16;
            rootNode.Override.NodeAppearance.Image = rootImage;
            rootNode.Override.ExpandedNodeAppearance.Image = rootImage;
            rootNode.Tag = rootGroup;
            deploymentTree.Nodes.Add(rootNode);

            // Add each of the child GROUPS to the root
            foreach (AssetGroup childGroup in rootGroup.Groups)
            {
                AddGroupChildren(rootNode, rootGroup);
            }

            // Expand the root node at least
            rootNode.ExpandAll();

            // Restore the cursor and end the update
            this.Cursor = Cursors.Default;
            this.deploymentTree.EndUpdate(true);
        }


        /// <summary>
        /// Add children of a group to the Tree Node
        /// </summary>
        /// <param name="parentNode"></param>
        /// <param name="parentGroup"></param>
        protected void AddGroupChildren(UltraTreeNode parentNode, AssetGroup parentGroup)
        {
            // First add the groups to the parent node
            foreach (AssetGroup childGroup in parentGroup.Groups)
            {
                string childKey = parentNode.Key + "|" + childGroup.Name;

                // Do we have this group already?  If not then add it
                UltraTreeNode childNode = null;
                int existingGroupID = parentNode.Nodes.IndexOf(childKey);
                if (existingGroupID == -1)
                {
                    childNode = new UltraTreeNode(childKey, childGroup.Name);

                    // Set the correct image for this group type
                    Bitmap image = (childGroup.GroupType == AssetGroup.GROUPTYPE.domain) ? Properties.Resources.domain16 : Properties.Resources.location_16;
                    childNode.LeftImages.Add(image);

                    // Set the tag for the node to be the AssetGroup object
                    childNode.Tag = childGroup;

                    // ...and add the node to the tree
                    parentNode.Nodes.Add(childNode);

                    // ...and then populate this node
                    AddGroupChildren(childNode, childGroup);
                }
            }

            // Now add in the child assets (if any)
            foreach (Asset asset in parentGroup.Assets)
            {
                if (asset.Auditable)
                {
                    string childKey = parentNode.Key + "|" + asset.Name;

                    int existingGroupID = parentNode.Nodes.IndexOf(childKey);
                    if (existingGroupID == -1)
                    {
                        // Create the new tree node for this asset
                        UltraTreeNode childNode = new UltraTreeNode(childKey, asset.Name);
                        childNode.LeftImages.Add(Properties.Resources.computer16);

                        // Set the tag for the node to be the 'asset' object
                        childNode.Tag = asset;

                        // ...and add the node to the tree                
                        parentNode.Nodes.Add(childNode);
                    }
                }
            }
        }



        /// <summary>
        /// Called as we check a node on the deployment tab
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void deploymentTree_AfterCheck(object sender, NodeEventArgs e)
        {
            this.deploymentTree.AfterCheck -= deploymentTree_AfterCheck;
            if (e.TreeNode.CheckedState == CheckState.Checked)
            {
                // If we have child nodes then ensure that ALL are checked 
                CheckAllChildNodes(e.TreeNode, true);

                // If we have a parent node then we need to let the parent know that all children in this
                // branch are checked as this will mean the parent (and potentially parents above it) need
                // to either be checked or indeterminate (part checked)
                CheckAllParentNodes(e.TreeNode);
            }
            else
            {
                // Uncheck this item
                e.TreeNode.CheckedState = CheckState.Unchecked;
                CheckAllChildNodes(e.TreeNode, false);

                // ...and set the check state of our parent(s) also
                UncheckAllParentNodes(e.TreeNode);
            }

            // Re-establish the check function
            this.deploymentTree.AfterCheck += deploymentTree_AfterCheck;
        }



        /// <summary>
        /// Called to check or uncheck all children of the specified parent node
        /// </summary>
        /// <param name="parentNode"></param>
        /// <param name="check"></param>
        protected void CheckAllChildNodes(UltraTreeNode parentNode, bool check)
        {
            foreach (UltraTreeNode childNode in parentNode.Nodes)
            {
                childNode.CheckedState = (check) ? CheckState.Checked : CheckState.Unchecked;

                // Iterate through our children also
                CheckAllChildNodes(childNode, check);
            }
        }


        /// <summary>
        /// Called to check or uncheck all parents nodes of the specified node
        /// </summary>
        /// <param name="parentNode"></param>
        /// <param name="check"></param>
        protected void CheckAllParentNodes(UltraTreeNode node)
        {
            // Determine the state of the immediate parent by determining whether any of its children are unchecked
            // meaning we set it's status to indeterminate or if all are checked we set to checked
            bool allChildrenChecked = true;
            UltraTreeNode parentNode = node.Parent;
            if (parentNode == null)
                return;

            foreach (UltraTreeNode childNode in parentNode.Nodes)
            {
                if (childNode.CheckedState == CheckState.Unchecked)
                {
                    allChildrenChecked = false;
                    break;
                }
            }

            // OK so what state should we set the parent to?
            if (!allChildrenChecked)
                parentNode.CheckedState = CheckState.Indeterminate;
            else
                parentNode.CheckedState = CheckState.Checked;

            // Not quite finished though as we need to check this items parent (if any) also
            CheckAllParentNodes(parentNode);
        }




        /// <summary>
        /// Called to uncheck all parents nodes of the specified node
        /// </summary>
        /// <param name="parentNode"></param>
        /// <param name="check"></param>
        protected void UncheckAllParentNodes(UltraTreeNode node)
        {
            UltraTreeNode parentNode = node.Parent;
            if (parentNode == null)
                return;

            bool noChildrenUnChecked = true;
            foreach (UltraTreeNode childNode in parentNode.Nodes)
            {
                if (childNode.CheckedState != CheckState.Unchecked)
                {
                    noChildrenUnChecked = false;
                    break;
                }
            }

            if (noChildrenUnChecked)
                parentNode.CheckedState = CheckState.Unchecked;
            else
                parentNode.CheckedState = CheckState.Indeterminate;

            // Not quite finished though as we need to check this items parent (if any) also
            UncheckAllParentNodes(parentNode);
        }


        /// <summary>
        /// Start the process of deploying the agent and auditing the selected Computers
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DoDeployment()
        {
            // We will need the scanner configuration file in order to be able to initiate a remote audit
            // so build one now and write it to the AuditAgent sub-folder
            //AuditScannerDefinition auditScannerDefinition = new AuditScannerDefinition();

            // ...and try and open it, if we fail then we shall just use the defaults
            //auditScannerDefinition.ReadCurrent();

            // Get some basic information from it
            //string scannerFileName = auditScannerDefinition.Filename;
            string scannerPath = Path.Combine(Application.StartupPath, "scanners") + "\\auditagent\\default.xml";
            string agentIniFilePath = Path.Combine(Application.StartupPath, AuditAgentStrings.AuditAgentFilesPath);
            //string agentIniFileName = Path.Combine(agentIniFilePath, AuditAgentStrings.AuditAgentIni);

            string agentIniFileName = Path.Combine(AuditAgentStrings.AuditAgentFilesPath, AuditAgentStrings.AuditAgentIni);

            // Ensure that the file is upto date by forcing a write
            //auditScannerDefinition.Save(false);

            // ...then copy it to the agent folder
            try
            {
                File.Copy(scannerPath, agentIniFileName, true);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to copy the scanner configuration file from " + scannerPath + " to " + agentIniFileName + ", reason: " + ex.Message);
                return;
            }

            // Add all selected computers into a list that we can work on - note that we start with the child
            // nodes beneath the root node as we know that there is only the one root node
            List<Asset> computers = new List<Asset>();
            foreach (UltraTreeNode domainNode in deploymentTree.Nodes[0].Nodes)
            {
                if (domainNode.CheckedState != CheckState.Unchecked)
                {
                    // add all the checked computers in this domain to the list
                    foreach (UltraTreeNode computerNode in domainNode.Nodes)
                    {
                        if (computerNode.CheckedState == CheckState.Checked)
                            computers.Add(computerNode.Tag as Asset);
                    }
                }
            }

            // Make use of the deployment functionality in the Network Tab to continue
            // pass in the location of default scanner
            _networkWorkItemController.DeployAgents(computers);
        }


        #endregion Deploy Tab

        private void bnSettings_Click(object sender, EventArgs e)
        {
            IWorkspace settingsWorkspace = _networkDiscoveryWorkItemController.WorkItem.RootWorkItem.Workspaces[WorkspaceNames.SettingsTabWorkspace];
            ILaytonView settingsView = _networkDiscoveryWorkItemController.WorkItem.SettingsView;
            settingsWorkspace.Show(settingsView);
            _networkDiscoveryWorkItemController.WorkItem.RootWorkItem.WorkItems[WorkItemNames.SettingsWorkItem].Activate();
        }
    }
}