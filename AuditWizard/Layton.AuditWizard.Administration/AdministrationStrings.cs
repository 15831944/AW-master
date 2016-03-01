using System;
using System.Collections.Generic;
using System.Text;

namespace Layton.AuditWizard.Administration
{
    public class MiscStrings
    {
	}

	public class EventTopics
	{
	}

    public class RibbonNames
    {
        public const string tabName					= "Administration";
		//
		public const string adminRibbonTabUISite	= "ribbonTabs";
		public const string adminRibbonGroupUISite	= "adminRibbonGroups";
		public const string adminRibbonUISite		= "adminRibbonTools";
		//
		public const string adminSupplierRibbonUISite	= "adminSuppliersRibbonTools";
		public const string adminUserRibbonUISite		= "adminUserRibbonTools";
		public const string adminLocationsRibbonUISite	= "adminLocationsRibbonTools";
		public const string adminAssetTypeRibbonUISite	= "adminAssetTypeRibbonTools";
		public const string adminPicklistRibbonUISite	= "adminPicklistRibbonTools";
		public const string adminUserDataRibbonUISite	= "adminUserDataRibbonTools";
		public const string adminScannerRibbonUISite	= "adminScannerRibbonTools";
		public const string adminScannerDeploymentRibbonUISite = "adminScannerDeploymentRibbonTools";
        public const string adminScannerDeploymentAuditAgentRibbonUISite = "adminScannerDeploymentAuditAgentRibbonUISite";


		// Ribbon Tab / Group Names
		public const string administrationGroupName = "Administration";
		public const string configRibbonUISite	= "adminConfigRibbonTools";
		public const string suppliersGroupName	= "Suppliers";
		public const string usersGroupName		= "Users";
		public const string locationsGroupName = "Locations";
		public const string assettypeGroupName = "Assettype";
		public const string picklistGroupName	= "Picklist";
		public const string userdataGroupName	= "User Data Views";
		public const string scannerConfigurationGroupName = "Scanner Configuration";
		public const string scannerDeploymentGroupName = "Scanner Deployment";
        public const string scannerDeploymentAuditAgentGroupName = "AuditAgent Deployment";
	}

    public class ToolNames
    {
		public const string general				= "General";
		public const string generalToolTip		= "Configure General AuditWizard Settings";
		//
		public const string locations			= "User Locations";
		public const string locationsToolTip	= "Configure the user defined locations structure";
		//
		public const string auditing			= "Auditing";
		public const string auditingToolTip		= "Configure settings related to the running of audits";
		//
		public const string dataSetup			= "Data Setup";
		public const string dataToolTip			= "Configure data lists used within AuditWizard";
		//
		public const string applications		= "Applications";
		public const string applicationsToolTip = "Configure settings related to the auditing of applications";
		//
		public const string alertmonitor		= "Alert Monitor";
		public const string alertmonitorToolTip	= "Configure settinsg related to the Alert Monitor feature";
		//
		public const string tools				= "Tools";
		public const string toolsToolTip		= "Configure external tools used by AuditWizard";

		// Supplier Tools
		public const string supplierAdd			= "Add Supplier";
		public const string supplierAddTootip	= "Create a new Supplier";
		public const string supplierEdit		= "Edit Supplier";
		public const string supplierEditTooltip = "Edit the definition for an existing Supplier";
		public const string supplierDelete		= "Delete Supplier";
		public const string supplierDeleteTooltip = "Delete a Supplier";

		// User Tools
		public const string userAdd				= "Add User";
		public const string userAddTootip		= "Create a new User";
		public const string userEdit			= "Edit User";
		public const string userEditTooltip		= "Edit the definition for an existing User";
		public const string userDelete			= "Delete User";
		public const string userDeleteTooltip	= "Delete a User";

		// Location Tools
		public const string locationAdd = "Add Location";
		public const string locationAddTootip = "Create a new Location";
		public const string locationEdit = "Modify Location";
		public const string locationEditTooltip = "Edit the definition for an existing Location";
		public const string locationDelete = "Delete Location";
		public const string locationDeleteTooltip = "Delete a Location";

		// Asset Type Tools
		public const string assettypeAdd		= "Add Asset Type";
		public const string assettypeAddTootip = "Create a new Asset Type";
		public const string assettypeEdit = "Modify Asset Type";
		public const string assettypeEditTooltip = "Edit the definition for an existing Asset Type";
		public const string assettypeDelete = "Delete Asset Type";
		public const string assettypeDeleteTooltip = "Delete an Asset Type";

		// Picklist Tools
		public const string picklistAdd = "Add Picklist";
		public const string picklistAddTootip = "Create a new Picklist";
		public const string picklistEdit = "Modify Picklist";
		public const string picklistEditTooltip = "Edit the definition for an existing Picklist";
		public const string picklistDelete = "Delete Picklist";
		public const string picklistDeleteTooltip = "Delete a Picklist";

		// User Data Tools
		public const string userDataAsset = "View Asset Fields";
		public const string userdataAssetTootip = "Display the user data fields defined for assets";
		public const string userDataApps = "View Application Fields";
		public const string userDataAppsTooltip = "Display the user data fields defined for applications";

		// Scanner Configuration Tools
		public const string scannerNew = "New Configuration";
		public const string scannerNewTootip = "Create a new, default Audit Scanner Configuration";
		public const string scannerLoad = "Load Configuration";
		public const string scannerLoadTootip = "Load a previously saved Audit Scanner Configuration";
		public const string scannerSave = "Save Configuration";
		public const string scannerSaveTooltip = "Save this Scanner Configuration";

		// Scanner Deployment Tools
		public const string scannerDeploy = "Deploy to Network";
		public const string scannerDeployTootip = "Deploy the Audit Scanner to the specified folder";
		public const string scannerDeployMSI = "Deploy MSI Package";
		public const string scannerDeplyMSITooltip = "Builds and saves an MSI package which may be used to deploy the scanner via Group Policy or other manual means";
		public const string scannerDeployEmail = "Deploy Email Package";
		public const string scannerDeployEmailTooltip = "Builds and saves a self extracting executable package which may be used to deploy the scanner via Email or other manual means";
		public const string scannerShutdown = "Shutdown AlertMonitor";
		public const string scannerShutdownTooltip = "Shutdown ALL AlertMonitor Enabled Scanners";
		public const string scannerUpdate = "Update AuditAgents";
		public const string scannerUpdateTooltip = "Update the scanner configuration on ALL deployed AuditAgents";
	}

	public class GeneralOptionNames
	{
		public const string generalGroup = "general";
		public const string general_security = "Users and Security";
		public const string general_security_tooltip = "Define users and their access to AuditWizard";
		public const string general_email = "Email Settings";
		public const string general_email_tooltip = "Define how emails are to be sent";
		public const string general_service = "AuditWizard Services";
		public const string general_service_tooltip = "Define settings for and control the AuditWizard Service";
		public const string general_database = "Database Maintenance";
		public const string general_database_tooltip = "Perform regular maintenance of the AuditWizard database";
	}

	public class AuditingOptionNames
	{
		public const string auditingGroup = "auditing";
		public const string auditing_upload = "Upload Options";
		public const string auditing_upload_tooltip = "Define settings which affect the way in which audit data is uploaded";
		public const string auditing_agent = "AuditAgent Configuration";
		public const string auditing_agent_tooltip = "Define settings which affect the way in which the AuditWizard Agent will function";
		public const string auditing_configuration = "AuditScanner Configuration";
		public const string auditing_configuration_tooltip = "Define settings which affect the way in which the AuditWizard Software Discovery will function";
	}

	public class DataOptionNames
	{
		public const string dataGroup = "datasetup";
		public const string data_locations = "Location Structure";
		public const string data_licensetypes = "License Types";
		public const string data_serialnumbers = "Serial Number Mappings";
		public const string data_assettypes = "Asset Types";
		public const string data_userdata = "User Defined Data";
		public const string data_picklists = "Picklists";
		public const string data_aliases = "Application Aliases";
	}

	public class ApplicationOptionNames
	{
		public const string applicationGroup = "applications";
		public const string application_supplier = "Suppliers";
		public const string application_supplier_tooltip = "Define the names and contact details for Suppliers of our applications";
	}

	public class AlertMonitorOptionNames
	{
		public const string alertMonitorGroup = "alertmonitor";
		public const string alertMonitor_settings = "AlertMonitor Configuration";
		public const string alertMonitor_settings_tooltip = "Define general settings for Alert Monitor";
	}

	public class ToolsOptionNames
	{
		public const string toolsGroup = "tools";
		public const string tools_settings = "Settings";
		public const string tools_settings_tooltip = "Define how the remote desktop facility will operate";
	}


}
