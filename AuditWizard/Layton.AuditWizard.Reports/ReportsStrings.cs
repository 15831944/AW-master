using System;
using System.Collections.Generic;
using System.Text;

namespace Layton.AuditWizard.Reports
{
	public class ReportEventTopics
	{
		public const string ReportChanged = "event://AuditWizardReportEvents/ReportChanged";
		public const string ReportSaved = "event://AuditWizardReportEvents/ReportSaved";
	}

	public class RibbonNames
	{
		//
		public const string reportRibbonTabUISite = "ribbonTabs";
		public const string reportRibbonGroupUISite = "reportRibbonGroups";

		// Ribbon tab names
		public const string tabName					= "Reports";
		public const string exportGroupName			= "Export";
        public const string printGroupName = "Print";
        public const string scheduleGroupName = "Report Settings";
		public const string maintenanceGroupName	= "Maintenance";
		public const string reportTypeGroupName		= "Report Type";
		// 	
		public const string exportRibbonUISite		= "reportExportRibbonTools";
        public const string printRibbonUISite = "reportPrintRibbonTools";
        public const string scheduleRibbonUISite = "schedulePrintRibbonTools";
		public const string maintenanceRibbonUISite = "maintenancePrintRibbonTools";
		public const string reportTypeRibbonUISite	= "reportTypeRibbonTools";
		//
		public const string filtersRibbonUISite		= "reportFiltersRibbonTools";
		public const string filtersGroupName		= "Filters";
	}

	public class ToolNames
	{
		public const string ApplicationByApplication = "Application-by-Application";
		public const string ComputerByComputer = "Asset-by-Asset";
		public const string SupportContracts = "Support Contracts";
		public const string reportNew = "New Report";
		public const string reportNewTootip = "Invoke the Report Wizard to create a new Report Definition";
		public const string reportLoad = "Load Report";
		public const string reportLoadTootip = "Load a previously saved Report Definition";
		public const string reportSave = "Save Report";
		public const string reportSaveTooltip = "Save this Report Definition";
		public const string reportProperties = "Properties";
		public const string reportPropertiesTooltip = "Display the properties of this Report Definition";
		public const string reportRun = "Run Report";
		public const string reportRunTootip = "Run the currently defined report definition";
	}
}
