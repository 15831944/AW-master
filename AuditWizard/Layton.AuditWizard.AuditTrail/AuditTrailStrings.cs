using System;
using System.Collections.Generic;
using System.Text;

namespace Layton.AuditWizard.AuditTrail
{
    public class MiscStrings
    {
		public const string NoFilter = "<No Filter>";
    }

	public class EventTopics
	{
		public const string AuditTrailFilterChanged = "event://AuditTrail/FilterChanged";
		public const string SelectedComputerChanged = "event://Layton.AuditWizard.AuditTrail/SelectedComputerChanged";
		public const string SelectedUserChanged = "event://Layton.AuditWizard.AuditTrail/SelectedUserChanged";
		public const string SelectedApplicationChanged = "event://Layton.AuditWizard.AuditTrail/SelectedApplicationChanged";
		public const string FilterStartDateChanged = "event://Layton.AuditWizard.AuditTrail/StartDateChanged";
		public const string FilterEndDateChanged = "event://Layton.AuditWizard.AuditTrail/EndDateChanged";
		public const string PurgeRequested = "event://Layton.AuditWizard.AuditTrail/PurgeRequested";
	}

    public class RibbonNames
    {
		public const string audittrailRibbonUISite = "audittrailRibbonTools";
		public const string audittrailRibbonGroupUISite = "audittrailRibbonGroups";
		public const string audittrailRibbonTabUISite = "ribbonTabs";
		//
		public const string rangeRibbonUISite = "audittrailRangeRibbonTools";
		public const string filtersRibbonUISite = "audittrailFiltersRibbonTools";
		public const string maintenanceRibbonUISite = "audittrailMaintenanceRibbonTools";
		public const string exportRibbonUISite = "audittrailExportRibbonTools";
		//
		public const string tabName = "Audit Trail";
		public const string rangeGroupName = "Date Range";
		public const string filtersGroupName = "Filters";
		public const string maintenanceGroupName = "Maintenance";
		public const string exportGroupName = "Export";
	}

    public class ToolNames
    {
		public const string DateRange = "Date Range";
		public const string FilterUser = "Filter By User";
		public const string FilterComputer = "Filter By Asset";
		public const string FilterApplication = "Filter By Application";
		public const string PurgeData = "Purge Data";
		public const string ExportXLS = "Excel";
		public const string ExportPDF = "PDF";
		public const string ExportXPS = "XPS";
	}

}
