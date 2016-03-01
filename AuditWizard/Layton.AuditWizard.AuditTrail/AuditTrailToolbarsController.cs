using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
//
using Infragistics.Win.UltraWinToolbars;
//
using Microsoft.Practices.CompositeUI;
using Microsoft.Practices.CompositeUI.EventBroker;
using Microsoft.Practices.CompositeUI.Utility;
//
using Layton.Cab.Interface;
using Layton.AuditWizard.Common;
using Layton.AuditWizard.DataAccess;

namespace Layton.AuditWizard.AuditTrail
{
    public class AuditTrailToolbarsController : LaytonToolbarsController
    {
		private LaytonWorkItem workItem;
		private RibbonTab audittrailRibbonTab;
		private RibbonGroup rangeRibbonGroup;
		private RibbonGroup maintenanceRibbonGroup;
		private RibbonGroup exportRibbonGroup;
		//
		private RibbonDateRangeControl _ribbonDateRangeControl;
		private AuditTrailPurgeControl _purgeControl;
		//
		private DateTime minDate = new DateTime(2000, 1, 1);

        public override RibbonTab RibbonTab
        {
			get { return audittrailRibbonTab; }
        }

        [Microsoft.Practices.ObjectBuilder.InjectionConstructor]
		public AuditTrailToolbarsController([ServiceDependency] WorkItem workItem)
        {
            this.workItem = workItem as LaytonWorkItem;
        }

        public override void Initialize()
        {
			// Now create a Ribbon Tab and add it to the ribbonTabs collection
			audittrailRibbonTab = new RibbonTab(RibbonNames.tabName, RibbonNames.tabName);
			workItem.RootWorkItem.UIExtensionSites[RibbonNames.audittrailRibbonTabUISite].Add<RibbonTab>(audittrailRibbonTab);

			// Set the Tag property to the WorkItem
			// this will allow the Shell to activate the WorkItem given the RibbonTab
			audittrailRibbonTab.Tag = workItem;

			// Now register the ribbon tab's Groups collection
			workItem.RootWorkItem.UIExtensionSites.RegisterSite(RibbonNames.audittrailRibbonGroupUISite, audittrailRibbonTab.Groups);

			// Add the "Date Range" RibbonGroup to the RibbonTab
			rangeRibbonGroup = new RibbonGroup(RibbonNames.rangeGroupName);
			rangeRibbonGroup.Caption = RibbonNames.rangeGroupName;
			rangeRibbonGroup.PreferredToolSize = RibbonToolSize.Large;
			workItem.UIExtensionSites[RibbonNames.audittrailRibbonGroupUISite].Add<RibbonGroup>(rangeRibbonGroup);
			workItem.RootWorkItem.UIExtensionSites.RegisterSite(RibbonNames.rangeRibbonUISite, rangeRibbonGroup.Tools);

			// Add the "Maintenance" RibbonGroup to the RibbonTab
			maintenanceRibbonGroup = new RibbonGroup(RibbonNames.maintenanceGroupName);
			maintenanceRibbonGroup.Caption = RibbonNames.maintenanceGroupName;
			maintenanceRibbonGroup.PreferredToolSize = RibbonToolSize.Large;
			workItem.UIExtensionSites[RibbonNames.audittrailRibbonGroupUISite].Add<RibbonGroup>(maintenanceRibbonGroup);
			workItem.RootWorkItem.UIExtensionSites.RegisterSite(RibbonNames.maintenanceRibbonUISite, maintenanceRibbonGroup.Tools);

			// EXPORT MENU GROUP
			// =================
			//
			// Set the name, caption and image size for the 'Configuration' group
			exportRibbonGroup = new RibbonGroup(RibbonNames.exportGroupName);
			exportRibbonGroup.Caption = RibbonNames.exportGroupName;
			exportRibbonGroup.PreferredToolSize = RibbonToolSize.Normal;
			WorkItem.UIExtensionSites[RibbonNames.audittrailRibbonGroupUISite].Add<RibbonGroup>(exportRibbonGroup);
			WorkItem.RootWorkItem.UIExtensionSites.RegisterSite(RibbonNames.exportRibbonUISite, exportRibbonGroup.Tools);

			InitializeExportTools();
			InitializeDateTools();
			InitializeMaintenanceTools();
		}

		private void InitializeDateTools()
		{
			ControlContainerTool dateRangeTool = new ControlContainerTool(ToolNames.DateRange);
			_ribbonDateRangeControl = new RibbonDateRangeControl();
			dateRangeTool.Control = _ribbonDateRangeControl;
			dateRangeTool.SharedProps.ToolTipText = "Select the dates to filter the audit trail records with";
			workItem.UIExtensionSites[RibbonNames.rangeRibbonUISite].Add<ControlContainerTool>(dateRangeTool);

			// We need to be informed when the date range is changed in the sub-control so that we
			// can request a refresh of the main view.  To this end we need to listen to the events
			_ribbonDateRangeControl.FilterStartDateChanged += new EventHandler<AuditTrailFilterEventArgs>(AuditTrailFilterStartDateChangedHandler);
			_ribbonDateRangeControl.FilterEndDateChanged += new EventHandler<AuditTrailFilterEventArgs>(AuditTrailFilterEndDateChangedHandler);
		}


		private void InitializeMaintenanceTools()
		{
			ControlContainerTool datePurgeTool = new ControlContainerTool(ToolNames.PurgeData);
			_purgeControl = new AuditTrailPurgeControl();
			datePurgeTool.Control = _purgeControl;
			datePurgeTool.SharedProps.ToolTipText = "Select the date to purge the audit trail records";
			workItem.UIExtensionSites[RibbonNames.maintenanceRibbonUISite].Add<ControlContainerTool>(datePurgeTool);

			// We need to be informed when a purge is requested.  To this end we need to listen to the events
			_purgeControl.PurgeRequested += new EventHandler<AuditTrailPurgeEventArgs>(purgeControl_PurgeRequested);
		}

		/// <summary>
		/// Initialize the toolbar items for the EXPORT Group
		/// </summary>
		private void InitializeExportTools()
		{
			// Create a button for 'Export PDF' 
			ButtonTool tool = new ButtonTool(CommonToolNames.ExportPDF);
			tool.SharedProps.Caption = CommonToolNames.ExportPDF;
			tool.SharedProps.ToolTipText = "Export the grid data in Acrobat Portable Document Format (.PDF)";
			Image bgImage = Properties.Resources.pdf;
			tool.SharedProps.AppearancesSmall.Appearance.Image = bgImage;
			tool.SharedProps.AppearancesLarge.Appearance.Image = bgImage;
			WorkItem.UIExtensionSites[RibbonNames.exportRibbonUISite].Add<ButtonTool>(tool);
			tool.ToolClick += new ToolClickEventHandler(export_ToolClick);

			// Create a button for 'Export Excel'
			tool = new ButtonTool(CommonToolNames.ExportXLS);
			tool.SharedProps.Caption = CommonToolNames.ExportXLS;
			tool.SharedProps.ToolTipText = "Export the grid data in Microsoft Excel (.XLS) format";
			bgImage = Properties.Resources.excel_16;
			tool.SharedProps.AppearancesSmall.Appearance.Image = bgImage;
			tool.SharedProps.AppearancesLarge.Appearance.Image = bgImage;
			WorkItem.UIExtensionSites[RibbonNames.exportRibbonUISite].Add<ButtonTool>(tool);
			tool.ToolClick += new ToolClickEventHandler(export_ToolClick);

			// Create a button for 'Export XPS'
			tool = new ButtonTool(CommonToolNames.ExportXPS);
			tool.SharedProps.Caption = CommonToolNames.ExportXPS;
			tool.SharedProps.ToolTipText = "Export the grid data in XML Paper Specification (.XPS) format";
			bgImage = Properties.Resources.xps_16;
			tool.SharedProps.AppearancesSmall.Appearance.Image = bgImage;
			tool.SharedProps.AppearancesLarge.Appearance.Image = bgImage;
			WorkItem.UIExtensionSites[RibbonNames.exportRibbonUISite].Add<ButtonTool>(tool);
			tool.ToolClick += new ToolClickEventHandler(export_ToolClick);
		}

        public override void UpdateTools()
        {
            // TODO:  Update the state of any tools
        }


		/// <summary>
		/// This is the handler for the event fired when the filter start date is changed.
		/// We simply fire the FilterChanged event to pass the full event on
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		public void AuditTrailFilterStartDateChangedHandler(object sender, AuditTrailFilterEventArgs e)
		{
			// Inform the explorer view of the new start date - the explorer view will combine this
			// with other filters specified and will announce the filters as a whole
			AuditTrailExplorerView exploreView = workItem.ExplorerView as AuditTrailExplorerView;
			exploreView.FilterStartDateChanged(e.StartDate);
		}


		/// <summary>
		/// This is the handler for the event fired when the filter end date is changed.
		/// We simply fire the FilterChanged event to pass the full event on
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		public void AuditTrailFilterEndDateChangedHandler(object sender, AuditTrailFilterEventArgs e)
		{
			// Inform the explorer view of the new start date - the explorer view will combine this
			// with other filters specified and will announce the filters as a whole
			AuditTrailExplorerView exploreView = workItem.ExplorerView as AuditTrailExplorerView;
			exploreView.FilterEndDateChanged(e.EndDate);
		}


		/// <summary>
		/// Called when we are to purge the database of audit trail entries
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		void purgeControl_PurgeRequested(object sender, AuditTrailPurgeEventArgs e)
		{
			AuditTrailDAO lwDataAccess = new AuditTrailDAO();
			int purgeCount = lwDataAccess.AuditTrailPurge(e.PurgeDate);
			if (purgeCount == 0)
				MessageBox.Show("No audit trail entries were purged" ,"No Items Purged");
			else if (purgeCount == 1)
				MessageBox.Show("1 audit trail entry was", "Item Purged");
			else
				MessageBox.Show(String.Format("{0} audit trail entries were purged", purgeCount) ,"Items Purged");
		}

		/// <summary>
		/// This is the tool click handler for the Applications>Export ribbon group
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void export_ToolClick(object sender, ToolClickEventArgs e)
		{
			AuditTrailWorkItemController controller = workItem.Controller as AuditTrailWorkItemController;
			switch (e.Tool.SharedProps.Caption)
			{
				case CommonToolNames.ExportPDF:
					controller.ExportToPDF();
					break;

				case CommonToolNames.ExportXLS:
					controller.ExportToXLS();
					break;

				case CommonToolNames.ExportXPS:
					controller.ExportToXPS();
					break;

				default:
					break;
			}
		}


		/// <summary>
		/// Force a refresh of the filters bar
		/// </summary>
		public void RefreshFilters()
		{
		}

	}
}
