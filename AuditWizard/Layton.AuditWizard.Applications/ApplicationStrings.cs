using System;
using System.Collections.Generic;
using System.Text;

namespace Layton.AuditWizard.Applications
{
	public class MiscStrings
	{
		public const string AllPublishers = "Software Publishers";
		public const string OperatingSystems = "Operating Systems";
		public const string Actions = "Actions";
		public const string Alerts = "Alerts";
		public const string ApplicationInstanceNode = "[instances]";
		public const string ApplicationLicenseNode = "[licenses]";
	}

	public class EventTopics
	{
		public const string PublisherSelectionChanged = "event://ApplicationsExplorerView/PublisherSelectionChanged";
		public const string ApplicationSelectionChanged = "event://ApplicationsExplorerView/ApplicationSelectionChanged";
		public const string ApplicationLicenseSelectionChanged = "event://ApplicationsExplorerView/ApplicationLicenseSelectionChanged";
		public const string ApplicationInstallsSelectionChanged = "event://ApplicationsExplorerView/ApplicationInstallsSelectionChanged";
		public const string OperatingSystemSelectionChanged = "event://ApplicationsExplorerView/OperatingSystemSelectionChanged";
		public const string ActionsSelectionChanged = "event://ApplicationsExplorerView/ActionsSelectionChanged";
		public const string AlertsSelectionChanged = "event://ApplicationsExplorerView/AlertsSelectionChanged";
	}

	public class RibbonNames
	{
		public const string applicationRibbonTabUISite = "ribbonTabs";
		public const string applicationRibbonGroupUISite = "applicationRibbonGroups";
        public const string licensingRibbonUISite = "applicationLicensingRibbonTools";
        public const string aliasingRibbonUISite = "applicationAliasingRibbonTools";
		public const string configRibbonUISite			= "applicationConfigRibbonTools";
		//
		public const string tabName						= "Applications";
        public const string licensingGroupName = "Licensing";
        public const string aliasingGroupName = "Aliasing";
		public const string configurationGroupName		= "Configuration";
		//
		public const string filtersRibbonUISite			= "applicationFiltersRibbonTools";
		public const string exportRibbonUISite			= "applicationExportRibbonTools";
        public const string reportsRibbonUISite         = "reportsExportRibbonTools";
		//
		public const string filtersGroupName			= "Filters";
		public const string exportGroupName				= "Export";
	}

	public class ToolNames
	{
		public const string CreateLicense = "Create License";
        public const string SetIgnored = "Ignore Application(s)";
        public const string AliasApplications = "Alias Application(s)";
        public const string AliasPublishers = "Alias Publisher(s)";
		public const string SetIgnoredTooltip = "Flag the selected application(s) to be ignored within AuditWizard";
		public const string SetIncluded = "Include Application(s)";
		public const string SetIncludedTooltip = "Flag the selected application(s) to be included within AuditWizard";
		public const string HideOS = "Hide Operating System";
		public const string HideOSTooltip = "Flag the selected Operating System as 'hidden'";
		public const string ShowOS = "Show Operating System";
		public const string ShowOSTooltip = "Clear the 'hidden' flag for the selected Operating System";
		//
		public const string NewLicense = "New License";
		public const string EditLicense = "Edit License";
		public const string DeleteLicense = "Delete License";
		//
		public const string LicenseTypes = "License Types";
		public const string SerialNumbers = "Serial Numbers";
	}

	
	public class GeneralOptionNames
	{
		public const string General_Security	= "Users and Security";
		public const string General_Security_Tooltip	= "Enable security and define users who may access AuditWizard";
		public const string General_Service		= "AuditWizardService";
		public const string General_Service_Tooltip = "Configure and control the AuditWizardService";
		public const string General_Email		= "Configure email settings";
		public const string General_Email_Tooltip = "Configure the settings used by AuditWizard to send Emails";
	}

	public class AuditingOptionNames
	{
		public const string Auditing_Upload = "Upload Settings";
		public const string Auditing_Upload_Tooltip = "Configure options relating to the upload of audit data files";
		public const string Auditing_Agent = "Audit Agent";
		public const string Auditing_Agent_Tooltip = "Configure the AuditWizard Agent Service";
	}

	public class DataSetupOptions
	{
		public const string Data_LicenseTypes = "License Types";
		public const string Data_LicenseTypes_Tooltip = "Handle the definition and modification of license types";
		public const string Data_SerialNumbers = "Serial Number Mappings";
		public const string Data_SerialNumbers_Tooltip = "Manage the mappings between application serial numbers and their associated registry keys";
	}

	public class ApplicationOptions
	{
		public const string Application_Suppliers = "Suppliers";
		public const string Application_Suppliers_Tooltip = "Manage Supplier Definitions";
	}
}