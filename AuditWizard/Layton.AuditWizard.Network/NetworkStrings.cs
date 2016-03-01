using System;
using System.Collections.Generic;
using System.Text;

namespace Layton.AuditWizard.Network
{
	public class EventTopics
	{
		public const string TreeSelectionChanged = "event://NetworkExplorerView/TreeSelectionChanged";

		public const string GroupSelectionChanged = "event://NetworkExplorerView/GroupSelectionChanged";
		public const string ComputerSelectionChanged = "event://NetworkExplorerView/ComputerSelectionChanged";
		public const string SummarySelected = "event://NetworkExplorerView/SummarySelected";
		public const string ApplicationsSelected = "event://NetworkExplorerView/ApplicationsSelected";
		public const string HardwareSelected = "event://NetworkExplorerView/HardwareSelected";
	}

    public class RibbonNames
    {
        public const string tabName = "Network";
        public const string viewGroupName = "View";
		public const string agentGroupName = "Audit Agent";
		public const string auditGroupName = "Audit";
		//
		public const string networkRibbonTabUISite		= "ribbonTabs";
		public const string networkRibbonGroupUISite	= "networkRibbonGroups";
		public const string viewRibbonUISite			= "networkViewRibbonTools";
		public const string agentRibbonUISite			= "networkAgentRibbonTools";
		public const string auditRibbonUISite			= "networkAuditRibbonTools";
		//
		public const string filtersRibbonUISite			= "networkFiltersRibbonTools";
		public const string exportRibbonUISite			= "networkExportRibbonTools";
		public const string findRibbonUISite			= "networkFindRibbonTools";
		//
		public const string filtersGroupName			= "Filters";
		public const string exportGroupName				= "Export";
		public const string findGroupName				= "Search";
	}

    public class ToolNames
    {
		public const string ShowStockAssets			= "Show Stock Assets";
		public const string ShowStockAssetsTooltip	= "Show assets whose state has been set to 'Stock'";
		public const string ShowInUseAssets			= "Show In Use Assets";
		public const string ShowInUseAssetsTooltip	= "Show assets whose state has been set to 'In Use'";
		public const string ShowPendingAssets		= "Show Pending Disposal Assets";
		public const string ShowPendingAssetsTooltip = "Show assets whose state has been set to 'Pending Disposal'";
		public const string ShowDisposedAssets		= "Show Disposed Assets";
		public const string ShowDisposedAssetsTooltip = "Show assets whose state has been set to 'Disposed'";


		public const string HideComputer = "Hide Asset(s)";
		public const string HideComputerToolTip = "Hide the selected computer within the AuditWizard Views";
		public const string ShowComputer = "Show Asset(s)";
		public const string ShowComputerToolTip = "Clear the 'hidden' flag for the selected computer";
		public const string ShowAllComputers = "Show All Computers";
		public const string ShowAllComputersTooltip = "Show ALL computers regardless of whether or not they are flagged as 'hidden'";
		public const string DomainView = "Domain View";
		public const string DomainTooltip = "Display computers grouped within their network domains and workgroups";
		public const string UserLocationsView = "Locations View";
		public const string UserLocationsTooltip = "Display computers grouped within User Defined Locations";
		//
		public const string AuditMyNetwork = "Audit My Network";
		public const string Deploy = "Deploy Agent";
		public const string Start = "Start Agent";
		public const string Stop = "Stop Agent";
		public const string Remove = "Remove Agent";
		public const string CheckStatus = "Check Agent Status";
		public const string UploadData = "Upload Audits";
		public const string EnableLogging = "Enable Agent Logging";
		public const string DisableLogging = "Disable Agent Logging";
		public const string ViewLogFile = "View Agent Log File";
		public const string ClearLogFile = "Clear Agent Log File";
		public const string ReauditComputer = "Audit Now";
		public const string OperationsLog = "Operations Log";
		public const string AlertLog = "Alert Log";
		public const string FindAsset = "Search Database";
	}


}
