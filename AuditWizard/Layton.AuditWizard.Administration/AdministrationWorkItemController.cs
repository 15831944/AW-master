using System;
using System.IO;
using System.Windows.Forms;
using Layton.AuditWizard.Common;
using Layton.AuditWizard.DataAccess;
using Layton.Cab.Interface;
using Microsoft.Practices.CompositeUI;
using Microsoft.Practices.CompositeUI.EventBroker;
using Microsoft.Practices.CompositeUI.SmartParts;

namespace Layton.AuditWizard.Administration
{
    public class AdministrationWorkItemController : Layton.Cab.Interface.LaytonWorkItemController
    {
        #region Data
		AdministrationWorkItem workItem;

        protected static readonly log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

		/// <summary>
		/// If we need to select a specific tab on entry to settings then we set this field to the
		/// key of the tab that should be selected.
		/// </summary>
		private String _selectedTabKey = "";

		/// <summary>
		/// As the Scanner Configuration is accessed from both the ScannerConfigurationTabView and from 
		/// the AlertMonitorSettingsTabView we need to actually hold it here rather that in either of these
		/// two tab views so that it is centrally available to both
		/// </summary>
		private AuditScannerDefinition _auditScannerDefinition = new AuditScannerDefinition();

		/// <summary>
		/// This is the current publisher filter which will determine which publishers we display.  It can
		/// be updated by the PublisherFilterChanged event being fired.
		/// </summary>
		private String _publisherFilter = "";

		/// <summary>
		/// Flag to indicate whether or not we should be showing applications and operating systems
		/// which have been flagged as 'NotIgnore' or 'non-NotIgnore'. They can be updated by the 
		/// PublisherFilterChanged event being fired.
		/// </summary>
		private bool _showIncludedApplications = true;
		private bool _showIgnoredApplications = true;

        #endregion

        #region Properties

		public String SelectedTabKey
		{
			get { return _selectedTabKey; }
			set { _selectedTabKey = value; }
		}

		public AuditScannerDefinition AuditScannerDefinition
		{
			get { return _auditScannerDefinition; }
			set { _auditScannerDefinition = value; }
		}

		/// <summary>
		/// Recover the filter that has been set for Publishers
		/// </summary>
		public String PublisherFilter
		{
			get { return _publisherFilter; }
			set { _publisherFilter = value; }
		}

		/// <summary>
		/// Show or hide applications that have been flagged as 'hidden' in the database
		/// </summary>
		public bool ShowIncludedApplications
		{
			get { return _showIncludedApplications; }
		}

		/// <summary>
		/// Show or hide applications that have been flagged as 'hidden' in the database
		/// </summary>
		public bool ShowIgnoredApplications
		{
			get { return _showIgnoredApplications; }
		}
		
        #endregion Properties

        #region Constructor
		public AdministrationWorkItemController(WorkItem workItem) : base(workItem) 
		{
			this.workItem = workItem as AdministrationWorkItem;

			// We need to pull the publisher filter list from the database
			SettingsDAO lwDataAccess = new SettingsDAO();
			_publisherFilter = lwDataAccess.GetPublisherFilter();
		}
        #endregion Constructor

		public void HideWorkspace()
		{
			WorkItem.RootWorkItem.SmartParts.Remove(workItem.ExplorerView);
			WorkItem.RootWorkItem.SmartParts.Remove(workItem.TabView);
			WorkItem.RootWorkItem.SmartParts.Remove(workItem.ToolbarsController);
		}

		#region Handlers to select the correct display

		/// <summary>
		/// Called to force the display of 'General' options
		/// </summary>
		public void DisplayGeneralOptions()
		{
			AdministrationExplorerView explorerView = workItem.ExplorerView as AdministrationExplorerView;
			explorerView.DisplayGeneralOptions();
		}

		public void DisplayLocationOptions()
		{
			AdministrationExplorerView explorerView = workItem.ExplorerView as AdministrationExplorerView;
			explorerView.DisplayLocationOptions();
		}

		public void DisplayAuditingOptions()
		{
			AdministrationExplorerView explorerView = workItem.ExplorerView as AdministrationExplorerView;
			explorerView.DisplayAuditingOptions();
		}

		public void DisplayDataSetupOptions()
		{
			AdministrationExplorerView explorerView = workItem.ExplorerView as AdministrationExplorerView;
			explorerView.DisplayDataSetupOptions();
		}

		public void DisplayApplicationOptions()
		{
			AdministrationExplorerView explorerView = workItem.ExplorerView as AdministrationExplorerView;
			explorerView.DisplayApplicationOptions();
		}

		public void DisplayAlertMonitorOptions()
		{
			AdministrationExplorerView explorerView = workItem.ExplorerView as AdministrationExplorerView;
			explorerView.DisplayAlertMonitorOptions();
		}

		public void DisplayToolsOptions()
		{
			AdministrationExplorerView explorerView = workItem.ExplorerView as AdministrationExplorerView;
			explorerView.DisplayToolsOptions();
		}

		#endregion

		#region Configuration Menu Functions

		/// <summary>
		/// Invoke the 'Manage License Types' form
		/// </summary>
		public void ManageLicenseTypes()
		{
			if (WorkItem.SettingsView != null)
			{
				IWorkspace settingsWorkspace = WorkItem.RootWorkItem.Workspaces[WorkspaceNames.SettingsTabWorkspace];
				_selectedTabKey = "licensetypes";
				settingsWorkspace.Show(WorkItem.SettingsView);
				workItem.RootWorkItem.WorkItems[WorkItemNames.SettingsWorkItem].Activate();
			}
		}


		/// <summary>
		/// Invoke the 'Manage Serial Numbers' form
		/// </summary>
		public void ManageSerialNumbers()
		{
			if (WorkItem.SettingsView != null)
			{
				IWorkspace settingsWorkspace = WorkItem.RootWorkItem.Workspaces[WorkspaceNames.SettingsTabWorkspace];
				_selectedTabKey = "serialnumbermappings";
				settingsWorkspace.Show(WorkItem.SettingsView);
				workItem.RootWorkItem.WorkItems[WorkItemNames.SettingsWorkItem].Activate();

			}
		}

		#endregion Configuration Menu Functions

		#region Handlers to work on Suppliers

		public void SupplierAdd()
		{
			SuppliersTabView supplierTabView = WorkItem.Items[Properties.Settings.Default.SuppliersTabView] as SuppliersTabView;
			supplierTabView.AddSupplier();
		}


		/// <summary>
		/// Edit the (first) supplier selected in the tab view (if any)
		/// </summary>
		public void SupplierEdit()
		{
			SuppliersTabView supplierTabView = WorkItem.Items[Properties.Settings.Default.SuppliersTabView] as SuppliersTabView;
			supplierTabView.EditSupplier();
		}


		public void SupplierDelete()
		{
			SuppliersTabView supplierTabView = WorkItem.Items[Properties.Settings.Default.SuppliersTabView] as SuppliersTabView;
			supplierTabView.DeleteSupplier();
		}

		#endregion
		
		#region Handlers to work on Users

		public void UserAdd()
		{
			SecurityTabView securityTabView = WorkItem.Items[Properties.Settings.Default.SecurityTabView] as SecurityTabView;
			securityTabView.AddUser();
		}


		/// <summary>
		/// Edit the (first) user selected in the tab view (if any)
		/// </summary>
		public void UserEdit()
		{
			SecurityTabView securityTabView = WorkItem.Items[Properties.Settings.Default.SecurityTabView] as SecurityTabView;
			securityTabView.EditUser();
		}


		public void UserDelete()
		{
			SecurityTabView securityTabView = WorkItem.Items[Properties.Settings.Default.SecurityTabView] as SecurityTabView;
			securityTabView.DeleteUser();
		}

		#endregion

		#region Handlers to work on Locations

		public void LocationAdd()
		{
			LocationsTabView tabview = WorkItem.Items[Properties.Settings.Default.LocationsTabView] as LocationsTabView;
			tabview.AddLocation();
		}


		/// <summary>
		/// Edit the (first) location selected in the tab view (if any)
		/// </summary>
		public void LocationEdit()
		{
			LocationsTabView tabview = WorkItem.Items[Properties.Settings.Default.LocationsTabView] as LocationsTabView;
			tabview.EditLocation();
		}


		public void LocationDelete()
		{
			LocationsTabView tabview = WorkItem.Items[Properties.Settings.Default.LocationsTabView] as LocationsTabView;
			tabview.DeleteLocation();
		}

		#endregion

		#region Handlers to work on Asset Types

		public void AssetTypeAdd()
		{
			AssetTypesTabView tabview = WorkItem.Items[Properties.Settings.Default.AssetTypesTabView] as AssetTypesTabView;
			//tabview.AddAssetType();
		}


		/// <summary>
		/// Edit the (first) supplier selected in the tab view (if any)
		/// </summary>
		public void AssetTypeEdit()
		{
			AssetTypesTabView tabview = WorkItem.Items[Properties.Settings.Default.AssetTypesTabView] as AssetTypesTabView;
			//tabview.EditAssetType();
		}


		public void AssetTypeDelete()
		{
			AssetTypesTabView tabview = WorkItem.Items[Properties.Settings.Default.AssetTypesTabView] as AssetTypesTabView;
			//tabview.DeleteAssetType();
		}

		#endregion

		#region Handlers to work on Picklists

		public void PicklistAdd()
		{
			PickListTabView tabview = WorkItem.Items[Properties.Settings.Default.PickListTabView] as PickListTabView;
			//tabview.AddPicklist();
		}


		/// <summary>
		/// Edit the (first) supplier selected in the tab view (if any)
		/// </summary>
		public void PicklistEdit()
		{
			PickListTabView tabview = WorkItem.Items[Properties.Settings.Default.PickListTabView] as PickListTabView;
			//tabview.EditPicklist();
		}


		public void PicklistDelete()
		{
			PickListTabView tabview = WorkItem.Items[Properties.Settings.Default.PickListTabView] as PickListTabView;
			//tabview.DeletePicklist();
		}

		#endregion

		#region Handlers to work on User Data

		/// <summary>
		/// Request the user data view to display fields for Assets
		/// </summary>
		public void SelectAssetFields()
		{
			UserDefinedDataTabView tabview = WorkItem.Items[Properties.Settings.Default.UserDataTabView] as UserDefinedDataTabView;
			tabview.SelectDisplayFields(UserDataCategory.SCOPE.Asset);
		}


		/// <summary>
		/// request the user data view to display fields for applications
		/// </summary>
		public void SelectApplicationFields()
		{
			UserDefinedDataTabView tabview = WorkItem.Items[Properties.Settings.Default.UserDataTabView] as UserDefinedDataTabView;
			tabview.SelectDisplayFields(UserDataCategory.SCOPE.Application);
		}

		#endregion

		#region Handlers to work on the Scanner Configuration

		/// <summary>
		/// Create a new scanner configuration
		/// </summary>
		public void NewScannerConfiguration()
		{
			// First ask the user if they would like to save any changes made to the current scanner configuration
			if (MessageBox.Show("Would you like to save the current scanner configuration before proceeding?", "Save Scanner Configuration", MessageBoxButtons.YesNo) == DialogResult.Yes)
				SaveScannerConfiguration();

			// Now create a new default configuration.
			_auditScannerDefinition = new AuditScannerDefinition();
			_auditScannerDefinition.ScannerName = "new_scanner";
			_auditScannerDefinition.Description = "Enter new scanner description";

            _auditScannerDefinition.Hidden = true;
            _auditScannerDefinition.AutoUpload = true;

			_auditScannerDefinition.SoftwareScanApplications = true;
			_auditScannerDefinition.SoftwareScanOs = true;
			_auditScannerDefinition.HardwareScan = true;
            _auditScannerDefinition.IEScan = true;
            _auditScannerDefinition.IECookies = true;
            _auditScannerDefinition.IEHistory = true;
            _auditScannerDefinition.IEDays = 7;

            _auditScannerDefinition.AlertMonitorAlertSecs = 60;
            _auditScannerDefinition.AlertMonitorSettingSecs = 30;

			// ...and refresh the display
			ILaytonView tabView = (ILaytonView)WorkItem.RootWorkItem.Workspaces[WorkspaceNames.TabWorkspace].ActiveSmartPart;
			tabView.RefreshView();
		}


		/// <summary>
		/// Load a new scanner configuration
		/// </summary>
		public void LoadScannerConfiguration()
		{
            ILaytonView tabView = (ILaytonView)WorkItem.RootWorkItem.Workspaces[WorkspaceNames.TabWorkspace].ActiveSmartPart;

			// Display the load configuration screen and if we click OK load the new configuration
            FormLoadScannerConfiguration form = new FormLoadScannerConfiguration(tabView is AuditAgentTabView);
			if (form.ShowDialog() == DialogResult.OK)
			{
                try
                {
                    // Use the Deserialize method to restore the object's state.
                    _auditScannerDefinition = AuditWizardSerialization.DeserializeObject(form.FileName);
                }
                catch (Exception ex)
                {
                    logger.Error("Error in LoadScannerConfiguration using " + form.FileName, ex);
                    MessageBox.Show("Unable to load scanner configuration file", "AuditWizard", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
				
				// If the active tab is either the scanner configuration or alert monitor tab then calls
				// its refresh function to pick up the new scanner configuration								
				if ((tabView is ScannerConfigurationTabView) || (tabView is AlertMonitorSettingsTabView) || tabView is AuditAgentTabView)
					tabView.RefreshView();
			}
		}


		/// <summary>
		/// Save the current scanner configuration (to a named file)
		/// </summary>
		public bool SaveScannerConfiguration()
		{
            // First we need to request the tabs which work with the scanner configuration to save
            // any changes which they may have made but not yet saved.
            if ((ILaytonView)WorkItem.RootWorkItem.Workspaces[WorkspaceNames.TabWorkspace].ActiveSmartPart is AuditAgentTabView)
            {
                AuditAgentTabView generalTabView = WorkItem.Items[Properties.Settings.Default.AuditAgentTabView] as AuditAgentTabView;
                generalTabView.Save();
            }
            else
            {
                ScannerConfigurationTabView generalTabView = WorkItem.Items[Properties.Settings.Default.ScannerConfigurationTabView] as ScannerConfigurationTabView;
                generalTabView.Save();
            }

            return true;
		}


		/// <summary>
		/// Deploy the scanner (Network Configuration)
		/// </summary>
		public void DeployNetwork()
		{
            try
            {
                // Ensure that the scanner name has been defined
                if (_auditScannerDefinition.ScannerName == "")
                {
                    MessageBox.Show("The scanner name is blank - please specify a unique name for this scanner");
                    return;
                }

                // First we need to request the tabs which work with the scanner configuration to save
                // any changes which they may have made but not yet saved.
                ScannerConfigurationTabView generalTabView = WorkItem.Items[Properties.Settings.Default.ScannerConfigurationTabView] as ScannerConfigurationTabView;
                _auditScannerDefinition = generalTabView.UpdateChanges();

                // save the changes so that the auto-loader can pick up the scanner folder
                generalTabView.Save(false);

                // create the scann folders (scanner and data)
                Directory.CreateDirectory(_auditScannerDefinition.DeployPathData);
                Directory.CreateDirectory(_auditScannerDefinition.DeployPathScanner);

                // ...and deploy it
                if (_auditScannerDefinition.Deploy(false))
                {
                    string scannerPath = _auditScannerDefinition.DeployPathScanner;
                    //MessageBox.Show("The Audit Scanner has been deployed to " + _auditScannerDefinition.DeployPathScanner, "Deployment Successful");
                    if (scannerPath.Length > 50)
                    {
                        scannerPath = scannerPath.Substring(0, 22) + "..~.." + scannerPath.Substring(scannerPath.Length - 22, 22);
                    }

                    DesktopAlert.ShowDesktopAlert("The Audit Scanner has been deployed to " + scannerPath);
                }
            }
            catch (Exception ex)
            {
                logger.Error("Error in DeployNetwork.", ex);
                Utility.DisplayApplicationErrorMessage(ex.Message);
            }
		}


		/// <summary>
		/// Deploy the scanner (Email Configuration)
		/// </summary>
		public void DeployEmail()
		{
			// First we need to request the tabs which work with the scanner configuration to save
			// any changes which they may have made but not yet saved.
			ScannerConfigurationTabView generalTabView = WorkItem.Items[Properties.Settings.Default.ScannerConfigurationTabView] as ScannerConfigurationTabView;
			generalTabView.UpdateChanges();
			//
            //AlertMonitorSettingsTabView alertsTabView = WorkItem.Items[Properties.Settings.Default.AlertMonitorSettingsTabView] as AlertMonitorSettingsTabView;
            //alertsTabView.SaveChanges();

			// Ensure that the scanner name has been defined
			if (_auditScannerDefinition.ScannerName == "")
			{
				MessageBox.Show("The scanner name is blank - please specify a unique name for this scanner");
				return;
			}

			// show the folder browser dialog and then generate the Executable with the selected path
			FolderBrowserDialog browser = new FolderBrowserDialog();
			browser.Description = "Select a path to store the AuditWizard Scanner Self Extracting Executable:";
			browser.ShowNewFolderButton = true;
			browser.RootFolder = Environment.SpecialFolder.Desktop;
			DialogResult result = browser.ShowDialog();
			if (result != DialogResult.OK)
				return;

			// Now create the executable
			GenerateClientExecutable(browser.SelectedPath);
		}


		/// <summary>
		/// Shutdown any active AlertMonitor enabled scanners
		/// </summary>
		public void ScannerShutdown()
		{
			// Set the shutdown flag in the scanner
			_auditScannerDefinition.Shutdown = true;

			// ...and deploy it
			if (_auditScannerDefinition.Deploy(false))
				MessageBox.Show("An Audit Scanner Shutdown request has been issued", "Shutdown Requested");
				
			// Clear the shutdown flag as we don;t want to perpetuate it
			_auditScannerDefinition.Shutdown = false;
		}


		/// <summary>
		/// Update the configuration for ALL deployed AuditAgents
		/// </summary>
		public void ScannerUpdate()
		{
			// Get the list of computers which currently have the AuditAgent deployed (running or not)
			AssetDAO lwDataAccess = new AssetDAO();
			AssetList listAssets = new AssetList(lwDataAccess.EnumerateDeployedAssets(), true);

			//if (listAssets.Count == 0)
			//{
			//    MessageBox.Show("Found no assets which already have an AuditAgent deployed.", "AuditWizard", MessageBoxButtons.OK, MessageBoxIcon.Information);
			//    return;
			//}

            FormLoadScannerConfiguration form = new FormLoadScannerConfiguration(true, "Select a configuration file to update all assets.");
            if (form.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    // Use the Deserialize method to restore the object's state.
                    //auditScannerDefinition = AuditWizardSerialization.DeserializeObject(form.FileName);

                    string agentIniFileName = Path.Combine(AuditAgentStrings.AuditAgentFilesPath, AuditAgentStrings.AuditAgentIni);
                    File.Copy(form.FileName, agentIniFileName, true);
                }
                catch (System.IO.FileNotFoundException)
                {
                    MessageBox.Show("An error has occurred whilst updating the AuditAgent, please see the log file for further information.",
                        "AuditWizard", MessageBoxButtons.OK, MessageBoxIcon.Error);

                    // JML TODO log this exception
                    return;
                }
                catch (System.IO.IOException)
                {
                    MessageBox.Show("An error has occurred whilst updating the AuditAgent, please see the log file for further information.",
                        "AuditWizard", MessageBoxButtons.OK, MessageBoxIcon.Error);

                    // JML TODO log this exception
                    return;
                }
                catch
                {
                    MessageBox.Show("An error has occurred whilst updating the AuditAgent, please see the log file for further information.",
                        "AuditWizard", MessageBoxButtons.OK, MessageBoxIcon.Error);

                    // JML TODO log this exception
                    return;

                }
            }
            else
            {
                return;
            }

			// Ensure that we at least have a data path
            //if (auditScannerDefinition.DeployPathData == "")
            //{
            //    MessageBox.Show("The audit scanner configuration has not been configured yet", "Audit Scanner Configuration Not Defined");
            //    return;
            //}

			// Write the Agent Ini File to the Agent folder as this will combine the scanner configuration
			// with data taken from the Application Definitions File (publisher mappings etc)
            //string agentIniFileName = Path.Combine(AuditAgentStrings.AuditAgentFilesPath, AuditAgentStrings.AuditAgentIni);
            //if (auditScannerDefinition.WriteTo(agentIniFileName) != 0)
            //{
            //    MessageBox.Show("Error : Failed to write the AuditAgent configuration file", "Deploy Error");
            //    return;
            //}

			// We will pend the operation by adding it to the Operations queue in the database
			// The AuditWizard service works on this queue
			foreach (Asset asset in listAssets)
			{
				Operation newOperation = new Operation(asset.AssetID, Operation.OPERATION.updateconfiguration);
				newOperation.Add();
			}

			MessageBox.Show("The AuditAgent Update Request has been queued for " + listAssets.Count.ToString() + " PC(s) and will be actioned by the AuditWizard Service\n\nYou can check its progress by viewing the Operations Log", "Operations Queued", MessageBoxButtons.OK, MessageBoxIcon.Information);
		}



		/// <summary>
		/// Generate the client audit scanner self extracting executable in the specified folder
		/// </summary>
		/// <param name="path"></param>
		protected bool GenerateClientExecutable(String path)
		{
			// Confirm that the scanner configuration file is present
			string scannerConfigurationFile = _auditScannerDefinition.ScannerConfigurationFile;
			
			// ...and create the executable
			if (_auditScannerDefinition.DeployToExecutable(path))
				MessageBox.Show("AuditWizard has created a zip file containing the scanner and associated configuration files.\n\nThis can be sent to your users via Email.  Unzip the files to a work folder and run 'AuditScanner.exe' to audit their PC" ,"Email Scanner Created");
			return true;
		}

		#endregion

		#region Event Handlers

		/// <summary>
		/// This is the handler for the GLOBAL PublisherFilterChangeEvent which is fired when 
		/// the Publisher Filter has been updated elsewhere in the application.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		[EventSubscription(CommonEventTopics.PublisherFilterChanged)]
		public void PublisherFilterChangedHandler(object sender, PublisherFilterEventArgs e)
		{
			_publisherFilter = e.PublisherFilter;
			_showIncludedApplications = e.ViewIncludedApplications;
			_showIgnoredApplications = e.ViewIgnoredApplications;
		}

		#endregion Event Handlers

	}
}
