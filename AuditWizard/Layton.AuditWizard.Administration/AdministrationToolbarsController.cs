using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using Microsoft.Practices.CompositeUI;
using Microsoft.Practices.CompositeUI.EventBroker;
//
using Layton.Cab.Interface;
//
using Infragistics.Win.UltraWinToolbars;

namespace Layton.AuditWizard.Administration
{
    public class AdministrationToolbarsController : LaytonToolbarsController
    {
#region Data
		private RibbonTab ribbonTab;
		private RibbonGroup administrationRibbonGroup;

		/// <summary>
		/// This ribbon group is only displayed when the 'Users' tab is displayed
		/// </summary>
		private RibbonGroup usersRibbonGroup;

		/// <summary>
		/// This ribbon group is only displayed when the 'Locations' tab is displayed
		/// </summary>
		private RibbonGroup locationsRibbonGroup;

		/// <summary>
		/// This ribbon group is only displayed when the 'Asset Types' tab is displayed
		/// </summary>
		private RibbonGroup assettypeRibbonGroup;

		/// <summary>
		/// This ribbon group is only displayed when the 'Picklist' tab is displayed
		/// </summary>
		private RibbonGroup picklistRibbonGroup;

		/// <summary>
		/// This ribbon group is only displayed when the 'Suppliers' tab is displayed
		/// </summary>
		private RibbonGroup suppliersRibbonGroup;

		/// <summary>
		/// This ribbon group is only displayed when the 'User Defined Data' tab is displayed
		/// </summary>
		private RibbonGroup userdataRibbonGroup;

		/// <summary>
		/// This ribbon group is only displayed when the 'Scanner Configuration' tab is displayed
		/// </summary>
		private RibbonGroup scannerConfigurationRibbonGroup;

		/// <summary>
		/// This ribbon group is only displayed when the 'Scanner Configuration' tab is displayed
		/// </summary>
		private RibbonGroup scannerDeploymentRibbonGroup;

        /// <summary>
        /// This ribbon group is only displayed when the 'Scanner Configuration for Audit Agent' tab is displayed
        /// </summary>
        private RibbonGroup scannerDeploymentAuditAgentRibbonGroup;

		/// <summary>
		/// This is the workItem
		/// </summary>
		private AdministrationWorkItem workItem;
#endregion

#region Data Accessors
		public override RibbonTab RibbonTab
		{
			get { return ribbonTab; }
		}
		#endregion

#region Constructor
		[Microsoft.Practices.ObjectBuilder.InjectionConstructor]
		public AdministrationToolbarsController([ServiceDependency] WorkItem workItem)
        {
            this.workItem = workItem as AdministrationWorkItem;
        }
#endregion

        public override void Initialize()
        {
			// Create the ribbon groups and tools to add to the ribbon tab
			ribbonTab = new RibbonTab(Properties.Settings.Default.Title, Properties.Settings.Default.Title);

			// Now add the RibbonTab to the ribbonTabs collection and register the new ribbon tab's Groups collection
			WorkItem.RootWorkItem.UIExtensionSites[RibbonNames.adminRibbonTabUISite].Add<RibbonTab>(ribbonTab);
			WorkItem.RootWorkItem.UIExtensionSites.RegisterSite(RibbonNames.adminRibbonGroupUISite, ribbonTab.Groups);

			// Add the 'Administration' ribbon group
			administrationRibbonGroup = new RibbonGroup(RibbonNames.administrationGroupName);
			administrationRibbonGroup.Caption = RibbonNames.administrationGroupName;
			administrationRibbonGroup.PreferredToolSize = RibbonToolSize.Large;
			WorkItem.UIExtensionSites[RibbonNames.adminRibbonGroupUISite].Add<RibbonGroup>(administrationRibbonGroup);
			WorkItem.RootWorkItem.UIExtensionSites.RegisterSite(RibbonNames.adminRibbonUISite, administrationRibbonGroup.Tools);
			InitializeAdministrationTools();

			// Add the 'Users' ribbon group - note this is the first toolbar for the first tab so we create it as visible
			usersRibbonGroup = new RibbonGroup(RibbonNames.usersGroupName);
			usersRibbonGroup.Caption = RibbonNames.locationsGroupName;
			usersRibbonGroup.PreferredToolSize = RibbonToolSize.Large;
			usersRibbonGroup.Visible = true;
			WorkItem.UIExtensionSites[RibbonNames.adminRibbonGroupUISite].Add<RibbonGroup>(usersRibbonGroup);
			WorkItem.RootWorkItem.UIExtensionSites.RegisterSite(RibbonNames.adminUserRibbonUISite, usersRibbonGroup.Tools);
			InitializeUserTools();

			// Add the 'Suppliers' ribbon group
			suppliersRibbonGroup = new RibbonGroup(RibbonNames.suppliersGroupName);
			suppliersRibbonGroup.Caption = RibbonNames.suppliersGroupName;
			suppliersRibbonGroup.PreferredToolSize = RibbonToolSize.Large;
			suppliersRibbonGroup.Visible = false;
			WorkItem.UIExtensionSites[RibbonNames.adminRibbonGroupUISite].Add<RibbonGroup>(suppliersRibbonGroup);
			WorkItem.RootWorkItem.UIExtensionSites.RegisterSite(RibbonNames.adminSupplierRibbonUISite, suppliersRibbonGroup.Tools);
			InitializeSupplierTools();

			// Add the 'Locations' ribbon group
			locationsRibbonGroup = new RibbonGroup(RibbonNames.locationsGroupName);
			locationsRibbonGroup.Caption = RibbonNames.locationsGroupName;
			locationsRibbonGroup.PreferredToolSize = RibbonToolSize.Large;
			locationsRibbonGroup.Visible = false;
			WorkItem.UIExtensionSites[RibbonNames.adminRibbonGroupUISite].Add<RibbonGroup>(locationsRibbonGroup);
			WorkItem.RootWorkItem.UIExtensionSites.RegisterSite(RibbonNames.adminLocationsRibbonUISite, locationsRibbonGroup.Tools);
			InitializeLocationTools();

			// Add the 'Asset Type' ribbon group
			assettypeRibbonGroup = new RibbonGroup(RibbonNames.assettypeGroupName);
			assettypeRibbonGroup.Caption = RibbonNames.assettypeGroupName;
			assettypeRibbonGroup.PreferredToolSize = RibbonToolSize.Large;
			assettypeRibbonGroup.Visible = false;
			WorkItem.UIExtensionSites[RibbonNames.adminRibbonGroupUISite].Add<RibbonGroup>(assettypeRibbonGroup);
			WorkItem.RootWorkItem.UIExtensionSites.RegisterSite(RibbonNames.adminAssetTypeRibbonUISite, assettypeRibbonGroup.Tools);
			InitializeAssetTypeTools();

			// Add the 'Picklist' ribbon group
			picklistRibbonGroup = new RibbonGroup(RibbonNames.picklistGroupName);
			picklistRibbonGroup.Caption = RibbonNames.picklistGroupName;
			picklistRibbonGroup.PreferredToolSize = RibbonToolSize.Large;
			picklistRibbonGroup.Visible = false;
			WorkItem.UIExtensionSites[RibbonNames.adminRibbonGroupUISite].Add<RibbonGroup>(picklistRibbonGroup);
			WorkItem.RootWorkItem.UIExtensionSites.RegisterSite(RibbonNames.adminPicklistRibbonUISite, picklistRibbonGroup.Tools);
			InitializePicklistTools();

			// Add the 'User Data' ribbon group
			userdataRibbonGroup = new RibbonGroup(RibbonNames.userdataGroupName);
			userdataRibbonGroup.Caption = RibbonNames.userdataGroupName;
			userdataRibbonGroup.PreferredToolSize = RibbonToolSize.Large;
			userdataRibbonGroup.Visible = false;
			WorkItem.UIExtensionSites[RibbonNames.adminRibbonGroupUISite].Add<RibbonGroup>(userdataRibbonGroup);
			WorkItem.RootWorkItem.UIExtensionSites.RegisterSite(RibbonNames.adminUserDataRibbonUISite, userdataRibbonGroup.Tools);
			InitializeUserDataTools();

			// Add the 'Scanner Configuration' ribbon group
			scannerConfigurationRibbonGroup = new RibbonGroup(RibbonNames.scannerConfigurationGroupName);
			scannerConfigurationRibbonGroup.Caption = RibbonNames.scannerConfigurationGroupName;
			scannerConfigurationRibbonGroup.PreferredToolSize = RibbonToolSize.Large;
			scannerConfigurationRibbonGroup.Visible = false;
			WorkItem.UIExtensionSites[RibbonNames.adminRibbonGroupUISite].Add<RibbonGroup>(scannerConfigurationRibbonGroup);
			WorkItem.RootWorkItem.UIExtensionSites.RegisterSite(RibbonNames.adminScannerRibbonUISite, scannerConfigurationRibbonGroup.Tools);
			InitializeScannerTools();

			// Add the 'Scanner Deployment' ribbon group
			scannerDeploymentRibbonGroup = new RibbonGroup(RibbonNames.scannerDeploymentGroupName);
			scannerDeploymentRibbonGroup.Caption = RibbonNames.scannerDeploymentGroupName;
			scannerDeploymentRibbonGroup.PreferredToolSize = RibbonToolSize.Large;
			scannerDeploymentRibbonGroup.Visible = false;
			WorkItem.UIExtensionSites[RibbonNames.adminRibbonGroupUISite].Add<RibbonGroup>(scannerDeploymentRibbonGroup);
			WorkItem.RootWorkItem.UIExtensionSites.RegisterSite(RibbonNames.adminScannerDeploymentRibbonUISite, scannerDeploymentRibbonGroup.Tools);
			InitializeScannerDeploymentTools();

            // Add the 'Scanner Deployment' ribbon group
            scannerDeploymentAuditAgentRibbonGroup = new RibbonGroup(RibbonNames.scannerDeploymentAuditAgentGroupName);
            scannerDeploymentAuditAgentRibbonGroup.Caption = RibbonNames.scannerDeploymentAuditAgentGroupName;
            scannerDeploymentAuditAgentRibbonGroup.PreferredToolSize = RibbonToolSize.Large;
            scannerDeploymentAuditAgentRibbonGroup.Visible = false;
            WorkItem.UIExtensionSites[RibbonNames.adminRibbonGroupUISite].Add<RibbonGroup>(scannerDeploymentAuditAgentRibbonGroup);
            WorkItem.RootWorkItem.UIExtensionSites.RegisterSite(RibbonNames.adminScannerDeploymentAuditAgentRibbonUISite, scannerDeploymentAuditAgentRibbonGroup.Tools);
            InitializeScannerDeploymentAuditAgentTools();
		}

        public override void UpdateTools()
        {
        }

		/// <summary>
		/// Initialize the toolbar items for the CONFIGURATION Group
		/// </summary>
		private void InitializeAdministrationTools()
		{
			// Create a button for 'General' and add to the Group
			ButtonTool generalTool = new ButtonTool("administration" + ToolNames.general);
			generalTool.SharedProps.Caption = ToolNames.general;
			generalTool.SharedProps.ToolTipText = ToolNames.generalToolTip;
			Image bgImage = Properties.Resources.general_setting_32;
			generalTool.SharedProps.AppearancesLarge.Appearance.Image = bgImage;
			WorkItem.UIExtensionSites[RibbonNames.adminRibbonUISite].Add<ButtonTool>(generalTool);
			generalTool.ToolClick += new ToolClickEventHandler(admin_ToolClick);

			// Create a button for 'auditing' and add to the Group
			ButtonTool auditingTool = new ButtonTool("administration" + ToolNames.auditing);
			auditingTool.SharedProps.Caption = ToolNames.auditing;
			auditingTool.SharedProps.ToolTipText = ToolNames.auditingToolTip;
			bgImage = Properties.Resources.audit_settings_32;
			auditingTool.SharedProps.AppearancesLarge.Appearance.Image = bgImage;
			WorkItem.UIExtensionSites[RibbonNames.adminRibbonUISite].Add<ButtonTool>(auditingTool);
			auditingTool.ToolClick += new ToolClickEventHandler(admin_ToolClick);

			// Create a button for 'Data Setup' and add to the Group
			ButtonTool dataTool = new ButtonTool("administration" + ToolNames.dataSetup);
			dataTool.SharedProps.Caption = ToolNames.dataSetup;
			dataTool.SharedProps.ToolTipText = ToolNames.dataToolTip;
			bgImage = Properties.Resources.data_list_32;
			dataTool.SharedProps.AppearancesLarge.Appearance.Image = bgImage;
			WorkItem.UIExtensionSites[RibbonNames.adminRibbonUISite].Add<ButtonTool>(dataTool);
			dataTool.ToolClick += new ToolClickEventHandler(admin_ToolClick);

			// Create a button for 'Tools' and add to the Group
			ButtonTool toolsTool = new ButtonTool("administration" + ToolNames.tools);
			toolsTool.SharedProps.Caption = ToolNames.tools;
			toolsTool.SharedProps.ToolTipText = ToolNames.toolsToolTip;
			bgImage = Properties.Resources.Toolbox_32;
			toolsTool.SharedProps.AppearancesLarge.Appearance.Image = bgImage;
			WorkItem.UIExtensionSites[RibbonNames.adminRibbonUISite].Add<ButtonTool>(toolsTool);
			toolsTool.ToolClick += new ToolClickEventHandler(admin_ToolClick);
		}



		/// <summary>
		/// Initialize the toolbar items for the Suppliers Group
		/// </summary>
		private void InitializeSupplierTools()
		{
			// Create a button for 'Add Supplier' and add to the Group
			ButtonTool addTool = new ButtonTool("administration" + ToolNames.supplierAdd);
			addTool.SharedProps.Caption = ToolNames.supplierAdd;
			addTool.SharedProps.ToolTipText = ToolNames.supplierAdd;
			Image bgImage = Properties.Resources.supplier_add_32;
			addTool.SharedProps.AppearancesLarge.Appearance.Image = bgImage;
			WorkItem.UIExtensionSites[RibbonNames.adminSupplierRibbonUISite].Add<ButtonTool>(addTool);
			addTool.ToolClick += new ToolClickEventHandler(supplier_ToolClick);


			// Create a button for 'Edit Supplier' and add to the Group
            //ButtonTool editTool = new ButtonTool("administration" + ToolNames.supplierEdit);
            //editTool.SharedProps.Caption = ToolNames.supplierEdit;
            //editTool.SharedProps.ToolTipText = ToolNames.supplierEditTooltip;
            //bgImage = Properties.Resources.supplier_edit_32;
            //editTool.SharedProps.AppearancesLarge.Appearance.Image = bgImage;
            //WorkItem.UIExtensionSites[RibbonNames.adminSupplierRibbonUISite].Add<ButtonTool>(editTool);
            //editTool.ToolClick += new ToolClickEventHandler(supplier_ToolClick);

			// Create a button for 'Delete Tooltip' and add to the Group
            //ButtonTool deleteTool = new ButtonTool("administration" + ToolNames.supplierDelete);
            //deleteTool.SharedProps.Caption = ToolNames.supplierDelete;
            //deleteTool.SharedProps.ToolTipText = ToolNames.supplierDeleteTooltip;
            //bgImage = Properties.Resources.supplier_delete_32;
            //deleteTool.SharedProps.AppearancesLarge.Appearance.Image = bgImage;
            //WorkItem.UIExtensionSites[RibbonNames.adminSupplierRibbonUISite].Add<ButtonTool>(deleteTool);
            //deleteTool.ToolClick += new ToolClickEventHandler(supplier_ToolClick);
		}



		/// <summary>
		/// Initialize the toolbar items for the Users Group
		/// </summary>
		private void InitializeUserTools()
		{
			// Create a button for 'Add User' and add to the Group
			ButtonTool addTool = new ButtonTool("administration" + ToolNames.userAdd);
			addTool.SharedProps.Caption = ToolNames.userAdd;
			addTool.SharedProps.ToolTipText = ToolNames.userAddTootip;
			Image bgImage = Properties.Resources.user_add_32;
			addTool.SharedProps.AppearancesLarge.Appearance.Image = bgImage;
			WorkItem.UIExtensionSites[RibbonNames.adminUserRibbonUISite].Add<ButtonTool>(addTool);
			addTool.ToolClick += new ToolClickEventHandler(user_ToolClick);

			// Create a button for 'Edit User' and add to the Group
			ButtonTool editTool = new ButtonTool("administration" + ToolNames.supplierEdit);
			editTool.SharedProps.Caption = ToolNames.userEdit;
			editTool.SharedProps.ToolTipText = ToolNames.userEditTooltip;
			bgImage = Properties.Resources.user_edit_32;
			editTool.SharedProps.AppearancesLarge.Appearance.Image = bgImage;
			WorkItem.UIExtensionSites[RibbonNames.adminUserRibbonUISite].Add<ButtonTool>(editTool);
			editTool.ToolClick += new ToolClickEventHandler(user_ToolClick);

			// Create a button for 'Delete Tooltip' and add to the Group
			ButtonTool deleteTool = new ButtonTool("administration" + ToolNames.supplierDelete);
			deleteTool.SharedProps.Caption = ToolNames.userDelete;
			deleteTool.SharedProps.ToolTipText = ToolNames.userDeleteTooltip;
			bgImage = Properties.Resources.user_delete_32;
			deleteTool.SharedProps.AppearancesLarge.Appearance.Image = bgImage;
			WorkItem.UIExtensionSites[RibbonNames.adminUserRibbonUISite].Add<ButtonTool>(deleteTool);
			deleteTool.ToolClick += new ToolClickEventHandler(user_ToolClick);
		}



		/// <summary>
		/// Initialize the toolbar items for the Locations Group
		/// </summary>
		private void InitializeLocationTools()
		{
			// Create a button for 'Add Locations' and add to the Group
			ButtonTool addTool = new ButtonTool("administration" + ToolNames.locationAdd);
			addTool.SharedProps.Caption = ToolNames.locationAdd;
			addTool.SharedProps.ToolTipText = ToolNames.locationAddTootip;
			Image bgImage = Properties.Resources.location_add_32;
			addTool.SharedProps.AppearancesLarge.Appearance.Image = bgImage;
			WorkItem.UIExtensionSites[RibbonNames.adminLocationsRibbonUISite].Add<ButtonTool>(addTool);
			addTool.ToolClick += new ToolClickEventHandler(location_ToolClick);


			// Create a button for 'Edit Location' and add to the Group
			ButtonTool editTool = new ButtonTool("administration" + ToolNames.locationEdit);
			editTool.SharedProps.Caption = ToolNames.locationEdit;
			editTool.SharedProps.ToolTipText = ToolNames.locationEditTooltip;
			bgImage = Properties.Resources.location_edit_32;
			editTool.SharedProps.AppearancesLarge.Appearance.Image = bgImage;
			WorkItem.UIExtensionSites[RibbonNames.adminLocationsRibbonUISite].Add<ButtonTool>(editTool);
			editTool.ToolClick += new ToolClickEventHandler(location_ToolClick);

			// Create a button for 'Delete Location' and add to the Group
			ButtonTool deleteTool = new ButtonTool("administration" + ToolNames.locationDelete);
			deleteTool.SharedProps.Caption = ToolNames.locationDelete;
			deleteTool.SharedProps.ToolTipText = ToolNames.locationDeleteTooltip;
			bgImage = Properties.Resources.location_delete_32;
			deleteTool.SharedProps.AppearancesLarge.Appearance.Image = bgImage;
			WorkItem.UIExtensionSites[RibbonNames.adminLocationsRibbonUISite].Add<ButtonTool>(deleteTool);
			deleteTool.ToolClick += new ToolClickEventHandler(location_ToolClick);
		}

		/// <summary>
		/// Initialize the toolbar items for the Picklist Group
		/// </summary>
		private void InitializePicklistTools()
		{
			// Create a button for 'Add Picklist' and add to the Group
			ButtonTool addTool = new ButtonTool("administration" + ToolNames.picklistAdd);
			addTool.SharedProps.Caption = ToolNames.picklistAdd;
			addTool.SharedProps.ToolTipText = ToolNames.picklistAddTootip;
			Image bgImage = Properties.Resources.pickitem_add_32;
			addTool.SharedProps.AppearancesLarge.Appearance.Image = bgImage;
			WorkItem.UIExtensionSites[RibbonNames.adminPicklistRibbonUISite].Add<ButtonTool>(addTool);
			addTool.ToolClick += new ToolClickEventHandler(picklist_ToolClick);


			// Create a button for 'Edit Picklist' and add to the Group
			ButtonTool editTool = new ButtonTool("administration" + ToolNames.picklistEdit);
			editTool.SharedProps.Caption = ToolNames.picklistEdit;
			editTool.SharedProps.ToolTipText = ToolNames.picklistEditTooltip;
			bgImage = Properties.Resources.pickitem_edit_32;
			editTool.SharedProps.AppearancesLarge.Appearance.Image = bgImage;
			WorkItem.UIExtensionSites[RibbonNames.adminPicklistRibbonUISite].Add<ButtonTool>(editTool);
			editTool.ToolClick += new ToolClickEventHandler(picklist_ToolClick);

			// Create a button for 'Delete Picklist' and add to the Group
			ButtonTool deleteTool = new ButtonTool("administration" + ToolNames.picklistDelete);
			deleteTool.SharedProps.Caption = ToolNames.picklistDelete;
			deleteTool.SharedProps.ToolTipText = ToolNames.picklistDeleteTooltip;
			bgImage = Properties.Resources.pickitem_delete_32;
			deleteTool.SharedProps.AppearancesLarge.Appearance.Image = bgImage;
			WorkItem.UIExtensionSites[RibbonNames.adminPicklistRibbonUISite].Add<ButtonTool>(deleteTool);
			deleteTool.ToolClick += new ToolClickEventHandler(picklist_ToolClick);
		}



		/// <summary>
		/// Initialize the toolbar items for the Asset Types Group
		/// </summary>
		private void InitializeAssetTypeTools()
		{
			// Create a button for 'Add Asset Type' and add to the Group
			ButtonTool addTool = new ButtonTool("administration" + ToolNames.assettypeAdd);
			addTool.SharedProps.Caption = ToolNames.assettypeAdd;
			addTool.SharedProps.ToolTipText = ToolNames.assettypeAddTootip;
			Image bgImage = Properties.Resources.asset_types_add_32;
			addTool.SharedProps.AppearancesLarge.Appearance.Image = bgImage;
			WorkItem.UIExtensionSites[RibbonNames.adminAssetTypeRibbonUISite].Add<ButtonTool>(addTool);
			addTool.ToolClick += new ToolClickEventHandler(assettype_ToolClick);


			// Create a button for 'Edit Asset Type' and add to the Group
			ButtonTool editTool = new ButtonTool("administration" + ToolNames.assettypeEdit);
			editTool.SharedProps.Caption = ToolNames.assettypeEdit;
			editTool.SharedProps.ToolTipText = ToolNames.assettypeEditTooltip;
			bgImage = Properties.Resources.asset_types_edit_32;
			editTool.SharedProps.AppearancesLarge.Appearance.Image = bgImage;
			WorkItem.UIExtensionSites[RibbonNames.adminAssetTypeRibbonUISite].Add<ButtonTool>(editTool);
			editTool.ToolClick += new ToolClickEventHandler(assettype_ToolClick);

			// Create a button for 'Delete Asset Type' and add to the Group
			ButtonTool deleteTool = new ButtonTool("administration" + ToolNames.assettypeDelete);
			deleteTool.SharedProps.Caption = ToolNames.assettypeDelete;
			deleteTool.SharedProps.ToolTipText = ToolNames.assettypeDeleteTooltip;
			bgImage = Properties.Resources.asset_types_delete_32;
			deleteTool.SharedProps.AppearancesLarge.Appearance.Image = bgImage;
			WorkItem.UIExtensionSites[RibbonNames.adminAssetTypeRibbonUISite].Add<ButtonTool>(deleteTool);
			deleteTool.ToolClick += new ToolClickEventHandler(assettype_ToolClick);
		}



		/// <summary>
		/// Initialize the toolbar items for the User Data Group
		/// </summary>
		private void InitializeUserDataTools()
		{
			// Create a button for 'Asset Fields' and add to the Group
			ButtonTool assetTool = new ButtonTool("administration" + ToolNames.userDataAsset);
			assetTool.SharedProps.Caption = ToolNames.userDataAsset;
			assetTool.SharedProps.ToolTipText = ToolNames.userdataAssetTootip;
			Image bgImage = Properties.Resources.UserData_assets_32;
			assetTool.SharedProps.AppearancesLarge.Appearance.Image = bgImage;
			WorkItem.UIExtensionSites[RibbonNames.adminUserDataRibbonUISite].Add<ButtonTool>(assetTool);
			assetTool.ToolClick += new ToolClickEventHandler(userdata_ToolClick);

			// Create a button for 'application Fields' and add to the Group
			ButtonTool appsTool = new ButtonTool("administration" + ToolNames.userDataApps);
			appsTool.SharedProps.Caption = ToolNames.userDataApps;
			appsTool.SharedProps.ToolTipText = ToolNames.userDataAppsTooltip;
			bgImage = Properties.Resources.UserData_applications_32;
			appsTool.SharedProps.AppearancesLarge.Appearance.Image = bgImage;
			WorkItem.UIExtensionSites[RibbonNames.adminUserDataRibbonUISite].Add<ButtonTool>(appsTool);
			appsTool.ToolClick += new ToolClickEventHandler(userdata_ToolClick);
		}


		/// <summary>
		/// Initialize the toolbar items for the Scanner Configuration Group
		/// </summary>
		private void InitializeScannerTools()
		{
			// Create a button for 'New Configuration' and add to the Group
			ButtonTool newTool = new ButtonTool("administration" + ToolNames.scannerNew);
			newTool.SharedProps.Caption = ToolNames.scannerNew;
			newTool.SharedProps.ToolTipText = ToolNames.scannerNewTootip;
			Image bgImage = Properties.Resources.scanner_new_32;
			newTool.SharedProps.AppearancesLarge.Appearance.Image = bgImage;
			WorkItem.UIExtensionSites[RibbonNames.adminScannerRibbonUISite].Add<ButtonTool>(newTool);
			newTool.ToolClick += new ToolClickEventHandler(scanner_ToolClick);

			// Create a button for 'Load Configuration' and add to the Group
			ButtonTool loadTool = new ButtonTool("administration" + ToolNames.scannerLoad);
			loadTool.SharedProps.Caption = ToolNames.scannerLoad;
			loadTool.SharedProps.ToolTipText = ToolNames.scannerLoadTootip;
			bgImage = Properties.Resources.load_scanner_32;
			loadTool.SharedProps.AppearancesLarge.Appearance.Image = bgImage;
			WorkItem.UIExtensionSites[RibbonNames.adminScannerRibbonUISite].Add<ButtonTool>(loadTool);
			loadTool.ToolClick += new ToolClickEventHandler(scanner_ToolClick);

			// Create a button for 'Save configuration' and add to the Group
			ButtonTool saveTool = new ButtonTool("administration" + ToolNames.scannerSave);
			saveTool.SharedProps.Caption = ToolNames.scannerSave;
			saveTool.SharedProps.ToolTipText = ToolNames.scannerSaveTooltip;
			bgImage = Properties.Resources.save_scanner_32;
			saveTool.SharedProps.AppearancesLarge.Appearance.Image = bgImage;
			WorkItem.UIExtensionSites[RibbonNames.adminScannerRibbonUISite].Add<ButtonTool>(saveTool);
			saveTool.ToolClick += new ToolClickEventHandler(scanner_ToolClick);
		}


		/// <summary>
		/// Initialize the toolbar items for the Scanner Deployment Group
		/// </summary>
		private void InitializeScannerDeploymentTools()
		{
			// Create a button for 'Deploy to Network' and add to the Group
			ButtonTool deployTool = new ButtonTool("administration" + ToolNames.scannerDeploy);
			deployTool.SharedProps.Caption = ToolNames.scannerDeploy;
			deployTool.SharedProps.ToolTipText = ToolNames.scannerDeployTootip;
			Image bgImage = Properties.Resources.scanner_deploy_network_32;
			deployTool.SharedProps.AppearancesLarge.Appearance.Image = bgImage;
			WorkItem.UIExtensionSites[RibbonNames.adminScannerDeploymentRibbonUISite].Add<ButtonTool>(deployTool);
			deployTool.ToolClick += new ToolClickEventHandler(scanner_ToolClick);

			// Create a button for 'Deploy to Email' and add to the Group
            //ButtonTool emailTool = new ButtonTool("administration" + ToolNames.scannerDeployEmail);
            //emailTool.SharedProps.Caption = ToolNames.scannerDeployEmail;
            //emailTool.SharedProps.ToolTipText = ToolNames.scannerDeployEmailTooltip;
            //bgImage = Properties.Resources.scanner_deploy_email_32;
            //emailTool.SharedProps.AppearancesLarge.Appearance.Image = bgImage;
            //WorkItem.UIExtensionSites[RibbonNames.adminScannerDeploymentRibbonUISite].Add<ButtonTool>(emailTool);
            //emailTool.ToolClick += new ToolClickEventHandler(scanner_ToolClick);

			// Create a button for 'Shutdown AlertMonitor' and add to the Group
            //ButtonTool shutdownTool = new ButtonTool("administration" + ToolNames.scannerShutdown);
            //shutdownTool.SharedProps.Caption = ToolNames.scannerShutdown;
            //shutdownTool.SharedProps.ToolTipText = ToolNames.scannerShutdownTooltip;
            //bgImage = Properties.Resources.scanner_shutdown_32;
            //shutdownTool.SharedProps.AppearancesLarge.Appearance.Image = bgImage;
            //WorkItem.UIExtensionSites[RibbonNames.adminScannerDeploymentRibbonUISite].Add<ButtonTool>(shutdownTool);
            //shutdownTool.ToolClick += new ToolClickEventHandler(scanner_ToolClick);

			// Create a button for 'Deploy to MSI' and add to the Group
			//ButtonTool msiTool = new ButtonTool("administration" + ToolNames.scannerDeployMSI);
			//msiTool.SharedProps.Caption = ToolNames.scannerDeployMSI;
			//msiTool.SharedProps.ToolTipText = ToolNames.scannerDeplyMSITooltip;
			//bgImage = Properties.Resources.msi32;
			//msiTool.SharedProps.AppearancesLarge.Appearance.Image = bgImage;
			//WorkItem.UIExtensionSites[RibbonNames.adminScannerDeploymentRibbonUISite].Add<ButtonTool>(msiTool);
			//msiTool.ToolClick += new ToolClickEventHandler(scanner_ToolClick);
		}

        /// <summary>
		/// Initialize the toolbar items for the Scanner Deployment Audit Agent Group
		/// </summary>
        private void InitializeScannerDeploymentAuditAgentTools()
        {
            // Create a button for 'Update AuditAgents' and add to the Group
            ButtonTool updateTool = new ButtonTool("administration" + ToolNames.scannerUpdate);
            updateTool.SharedProps.Caption = ToolNames.scannerUpdate;
            updateTool.SharedProps.ToolTipText = ToolNames.scannerUpdateTooltip;
            Image bgImage = Properties.Resources.scanner_update_32;
            updateTool.SharedProps.AppearancesLarge.Appearance.Image = bgImage;
            WorkItem.UIExtensionSites[RibbonNames.adminScannerDeploymentAuditAgentRibbonUISite].Add<ButtonTool>(updateTool);
            updateTool.ToolClick += new ToolClickEventHandler(scanner_ToolClick);
        }	
		

		/// <summary>
		/// This is the tool click handler for the Applications>Configuration ribbon group
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void admin_ToolClick(object sender, ToolClickEventArgs e)
		{
			AdministrationWorkItemController controller = workItem.Controller as AdministrationWorkItemController;
			switch (e.Tool.SharedProps.Caption)
			{
				case ToolNames.general:
					controller.DisplayGeneralOptions();
					break;

				case ToolNames.locations:
					controller.DisplayLocationOptions();
					break;

				case ToolNames.auditing:
					controller.DisplayAuditingOptions();
					break;

				case ToolNames.dataSetup:
					controller.DisplayDataSetupOptions();
					break;

				case ToolNames.tools:
					controller.DisplayToolsOptions();
					break;

				default:
					break;
			}
		}





		/// <summary>
		/// This is the tool click handler for the Data Setup>Users ribbon group
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void user_ToolClick(object sender, ToolClickEventArgs e)
		{
			AdministrationWorkItemController controller = workItem.Controller as AdministrationWorkItemController;
			switch (e.Tool.SharedProps.Caption)
			{
				case ToolNames.userAdd:
					controller.UserAdd();
					break;

				case ToolNames.userEdit:
					controller.UserEdit();
					break;

				case ToolNames.userDelete:
					controller.UserDelete();
					break;

				default:
					break;
			}
		}



		/// <summary>
		/// This is the tool click handler for the Data Setup>Supplier ribbon group
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void supplier_ToolClick(object sender, ToolClickEventArgs e)
		{
			AdministrationWorkItemController controller = workItem.Controller as AdministrationWorkItemController;
			switch (e.Tool.SharedProps.Caption)
			{
				case ToolNames.supplierAdd:
					controller.SupplierAdd();
					break;

				case ToolNames.supplierEdit:
					controller.SupplierEdit();
					break;

				case ToolNames.supplierDelete:
					controller.SupplierDelete();
					break;

				default:
					break;
			}
		}



		/// <summary>
		/// This is the tool click handler for the Applications>Locations ribbon group
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void location_ToolClick(object sender, ToolClickEventArgs e)
		{
			AdministrationWorkItemController controller = workItem.Controller as AdministrationWorkItemController;
			switch (e.Tool.SharedProps.Caption)
			{
				case ToolNames.locationAdd:
					controller.LocationAdd();
					break;

				case ToolNames.locationEdit:
					controller.LocationEdit();
					break;

				case ToolNames.locationDelete:
					controller.LocationDelete();
					break;

				default:
					break;
			}
		}



		/// <summary>
		/// This is the tool click handler for the Applications>Picklist ribbon group
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void picklist_ToolClick(object sender, ToolClickEventArgs e)
		{
			AdministrationWorkItemController controller = workItem.Controller as AdministrationWorkItemController;
			switch (e.Tool.SharedProps.Caption)
			{
				case ToolNames.picklistAdd:
					controller.PicklistAdd();
					break;

				case ToolNames.picklistEdit:
					controller.PicklistEdit();
					break;

				case ToolNames.picklistDelete:
					controller.PicklistDelete();
					break;

				default:
					break;
			}
		}



		/// <summary>
		/// This is the tool click handler for the Applications>AssetTypes ribbon group
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void assettype_ToolClick(object sender, ToolClickEventArgs e)
		{
			AdministrationWorkItemController controller = workItem.Controller as AdministrationWorkItemController;
			switch (e.Tool.SharedProps.Caption)
			{
				case ToolNames.assettypeAdd:
					controller.AssetTypeAdd();
					break;

				case ToolNames.assettypeEdit:
					controller.AssetTypeEdit();
					break;

				case ToolNames.assettypeDelete:
					controller.AssetTypeDelete();
					break;

				default:
					break;
			}
		}



		/// <summary>
		/// This is the tool click handler for the Administration>User Data ribbon group
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void userdata_ToolClick(object sender, ToolClickEventArgs e)
		{
			AdministrationWorkItemController controller = workItem.Controller as AdministrationWorkItemController;
			switch (e.Tool.SharedProps.Caption)
			{
				case ToolNames.userDataAsset:
					controller.SelectAssetFields();
					break;

				case ToolNames.userDataApps:
					controller.SelectApplicationFields();
					break;

				default:
					break;
			}
		}



		/// <summary>
		/// This is the tool click handler for the Administration>Scanner Configuration ribbon group
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void scanner_ToolClick(object sender, ToolClickEventArgs e)
		{
			AdministrationWorkItemController controller = workItem.Controller as AdministrationWorkItemController;
			switch (e.Tool.SharedProps.Caption)
			{
				case ToolNames.scannerNew:
					controller.NewScannerConfiguration();
					break;

				case ToolNames.scannerLoad:
					controller.LoadScannerConfiguration();
					break;

				case ToolNames.scannerSave:
					controller.SaveScannerConfiguration();
					break;

				case ToolNames.scannerDeploy:
					controller.DeployNetwork();
					break;

				case ToolNames.scannerDeployEmail:
					controller.DeployEmail();
					break;

				case ToolNames.scannerShutdown:
					controller.ScannerShutdown();
					break;

				case ToolNames.scannerUpdate:
					controller.ScannerUpdate();
					break;

				default:
					break;
			}
		}

		/// <summary>
		/// Populate the ribbon groups displayed to be applicable to the tab selected
		/// </summary>
		/// <param name="activeTabView"></param>
		public void ResetRibbon(ILaytonView activeTabView)
		{
			// Handle the display of the various sub-toolbars
			suppliersRibbonGroup.Visible = (activeTabView is SuppliersTabView);
			locationsRibbonGroup.Visible = (activeTabView is LocationsTabView);
			assettypeRibbonGroup.Visible = false;				// (activeTabView is AssetTypesTabView);
			picklistRibbonGroup.Visible = false;				// (activeTabView is PickListTabView);
			userdataRibbonGroup.Visible = (activeTabView is UserDefinedDataTabView);

			// Scanner Configuration ribbon valid if the scanner tab view is displayed
			scannerConfigurationRibbonGroup.Visible = (activeTabView is ScannerConfigurationTabView || activeTabView is AuditAgentTabView);
			
			// Scanner Deployment is valid when we are displaying the 'Auditing' menu
			scannerDeploymentRibbonGroup.Visible = activeTabView is ScannerConfigurationTabView;

            scannerDeploymentAuditAgentRibbonGroup.Visible = activeTabView is AuditAgentTabView;
												 
			// Users toolbar only displayed when displaying the Users Tab View
			usersRibbonGroup.Visible = (activeTabView is SecurityTabView);
		}

	}
}
