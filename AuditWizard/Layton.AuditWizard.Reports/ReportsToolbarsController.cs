using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
//
using Microsoft.Practices.CompositeUI;
using Microsoft.Practices.CompositeUI.EventBroker;
//
using Layton.Cab.Interface;
using Layton.AuditWizard.Common;
using Layton.AuditWizard.DataAccess;
//
using Infragistics.Win.UltraWinToolbars;

namespace Layton.AuditWizard.Reports
{
    public class ReportsToolbarsController : LaytonToolbarsController
    {
        private RibbonTab ribbonTab;
		private RibbonGroup exportRibbonGroup;
		private RibbonGroup printRibbonGroup;
		private RibbonGroup filtersRibbonGroup;
		private RibbonGroup maintenanceRibbonGroup;
        private RibbonGroup scheduleRibbonGroup;
		private ReportsWorkItem workItem;

#region Constructor
		[Microsoft.Practices.ObjectBuilder.InjectionConstructor]
		public ReportsToolbarsController([ServiceDependency] WorkItem workItem)
        {
            this.workItem = workItem as ReportsWorkItem;
        }
#endregion

        public override RibbonTab RibbonTab
        {
            get { return ribbonTab; }
        }

        public override void Initialize()
        {
            // Create the ribbon groups and tools to add to the ribbon tab
            ribbonTab = new RibbonTab(Properties.Settings.Default.Title, Properties.Settings.Default.Title);

			// Now add the RibbonTab to the ribbonTabs collection and register the new ribbon tab's Groups collection
			WorkItem.RootWorkItem.UIExtensionSites[RibbonNames.reportRibbonTabUISite].Add<RibbonTab>(ribbonTab);
			WorkItem.RootWorkItem.UIExtensionSites.RegisterSite(RibbonNames.reportRibbonGroupUISite, ribbonTab.Groups);

			// MAINTENANCE MENU GROUP
			// ====================
			//
			// Set the name, caption and image size for the 'Load & Save' menu group
            //maintenanceRibbonGroup = new RibbonGroup(RibbonNames.maintenanceGroupName);
            //maintenanceRibbonGroup.Caption = RibbonNames.maintenanceGroupName;
            //maintenanceRibbonGroup.PreferredToolSize = RibbonToolSize.Large;
            //WorkItem.UIExtensionSites[RibbonNames.reportRibbonGroupUISite].Add<RibbonGroup>(maintenanceRibbonGroup);
            //WorkItem.RootWorkItem.UIExtensionSites.RegisterSite(RibbonNames.maintenanceRibbonUISite, maintenanceRibbonGroup.Tools);
            //InitializeMaintenanceTools();


			// FILTERS MENU GROUP
			// =================
			//
			// Set the name, caption and image size for the 'filters' group
			filtersRibbonGroup = new RibbonGroup(RibbonNames.filtersGroupName);
			filtersRibbonGroup.Caption = RibbonNames.filtersGroupName;
			filtersRibbonGroup.PreferredToolSize = RibbonToolSize.Normal;
			WorkItem.UIExtensionSites[RibbonNames.reportRibbonGroupUISite].Add<RibbonGroup>(filtersRibbonGroup);
			WorkItem.RootWorkItem.UIExtensionSites.RegisterSite(RibbonNames.filtersRibbonUISite, filtersRibbonGroup.Tools);
			InitializeFilterTools();

			// PRINT MENU GROUP
			// =================
			//
			// Set the name, caption and image size for the 'Print' group
			printRibbonGroup = new RibbonGroup(RibbonNames.printGroupName);
			printRibbonGroup.Caption = RibbonNames.printGroupName;
			printRibbonGroup.PreferredToolSize = RibbonToolSize.Normal;

			// Add this group to the ribbon 
			WorkItem.UIExtensionSites[RibbonNames.reportRibbonGroupUISite].Add<RibbonGroup>(printRibbonGroup);

			// Now register the Configuration group Tools collection
			WorkItem.RootWorkItem.UIExtensionSites.RegisterSite(RibbonNames.printRibbonUISite, printRibbonGroup.Tools);
			InitializePrintTools();

            // SCHEDULE MENU GROUP
            // =================
            //
            // Set the name, caption and image size for the 'Print' group
            scheduleRibbonGroup = new RibbonGroup(RibbonNames.scheduleGroupName);
            scheduleRibbonGroup.Caption = RibbonNames.scheduleGroupName;
            scheduleRibbonGroup.PreferredToolSize = RibbonToolSize.Normal;

            // Add this group to the ribbon 
            WorkItem.UIExtensionSites[RibbonNames.reportRibbonGroupUISite].Add<RibbonGroup>(scheduleRibbonGroup);

            // Now register the Configuration group Tools collection
            WorkItem.RootWorkItem.UIExtensionSites.RegisterSite(RibbonNames.scheduleRibbonUISite, scheduleRibbonGroup.Tools);
            InitializeScheduleTools();

            // EXPORT MENU GROUP
            // =================
            //
            // Set the name, caption and image size for the 'Export' group
            exportRibbonGroup = new RibbonGroup(RibbonNames.exportGroupName);
            exportRibbonGroup.Caption = RibbonNames.exportGroupName;
            exportRibbonGroup.PreferredToolSize = RibbonToolSize.Normal;

            // Add this group to the ribbon 
            WorkItem.UIExtensionSites[RibbonNames.reportRibbonGroupUISite].Add<RibbonGroup>(exportRibbonGroup);

            // Now register the Export group Tools collection
            WorkItem.RootWorkItem.UIExtensionSites.RegisterSite(RibbonNames.exportRibbonUISite, exportRibbonGroup.Tools);
            InitializeExportTools();
		}

        public override void UpdateTools()
        {
            // TODO:  Update the state of any tools
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


        /// <summary>
        /// Initialize the toolbar items for the PRINT Group
        /// </summary>
		private void InitializePrintTools()
		{
			// Create a button for 'Print...' 
			ButtonTool tool = new ButtonTool(CommonToolNames.Print);
			tool.SharedProps.Caption = CommonToolNames.Print;
			tool.SharedProps.ToolTipText = "Print the contents of the grid";
			Image bgImage = Properties.Resources.printer32;
			tool.SharedProps.AppearancesSmall.Appearance.Image = bgImage;
			tool.SharedProps.AppearancesLarge.Appearance.Image = bgImage;
			WorkItem.UIExtensionSites[RibbonNames.printRibbonUISite].Add<ButtonTool>(tool);
			tool.ToolClick += new ToolClickEventHandler(print_ToolClick);
			
			// Create a button for 'Print Preview' 
			tool = new ButtonTool(CommonToolNames.PrintPreview);
			tool.SharedProps.Caption = CommonToolNames.PrintPreview;
			tool.SharedProps.ToolTipText = "Preview how a print of the contents of the grid would appear";
			bgImage = Properties.Resources.print_preview_32;
			tool.SharedProps.AppearancesSmall.Appearance.Image = bgImage;
			tool.SharedProps.AppearancesLarge.Appearance.Image = bgImage;
			WorkItem.UIExtensionSites[RibbonNames.printRibbonUISite].Add<ButtonTool>(tool);
			tool.ToolClick += new ToolClickEventHandler(print_ToolClick);
		}

        /// <summary>
        /// Initialize the toolbar items for the SCHEDULE Group
        /// </summary>
        private void InitializeScheduleTools()
        {
            // Create a button for 'Schedule a report...' 
            ButtonTool tool = new ButtonTool(CommonToolNames.Schedule);
            tool.SharedProps.Caption = CommonToolNames.Schedule;
            tool.SharedProps.ToolTipText = "Schedule a report";
            Image bgImage = Properties.Resources.clock;
            tool.SharedProps.AppearancesSmall.Appearance.Image = bgImage;
            tool.SharedProps.AppearancesLarge.Appearance.Image = bgImage;
            WorkItem.UIExtensionSites[RibbonNames.scheduleRibbonUISite].Add<ButtonTool>(tool);
            tool.ToolClick += new ToolClickEventHandler(schedule_ToolClick);

            // Create a button for 'Locations Filter...' 
            tool = new ButtonTool(CommonToolNames.LocationFilter);
            tool.SharedProps.Caption = CommonToolNames.LocationFilter;
            tool.SharedProps.ToolTipText = "Filter reports by assets/locations";
            bgImage = Properties.Resources.location_16;
            tool.SharedProps.AppearancesSmall.Appearance.Image = bgImage;
            tool.SharedProps.AppearancesLarge.Appearance.Image = bgImage;
            WorkItem.UIExtensionSites[RibbonNames.scheduleRibbonUISite].Add<ButtonTool>(tool);
            tool.ToolClick += new ToolClickEventHandler(schedule_ToolClick);
        }

		/// <summary>
		/// Initialize the toolbar items for the View Group
		/// </summary>
		private void InitializeFilterTools()
		{
			// Create a button tool for 'Filter Publishers' and add it to the Settings Group
            //ButtonTool tool = new ButtonTool("reports" + CommonToolNames.FilterPublishers);
            //tool.SharedProps.Caption = CommonToolNames.FilterPublishers;
            //tool.SharedProps.ToolTipText = CommonToolNames.FilterPublishersTooltip;
            Image bgImage = Properties.Resources.filter_publishers_32;
            //tool.SharedProps.AppearancesLarge.Appearance.Image = bgImage;
            //ToolBase filterPublisherTool = WorkItem.UIExtensionSites[RibbonNames.filtersRibbonUISite].Add<ButtonTool>(tool);
            //filterPublisherTool.InstanceProps.PreferredSizeOnRibbon = RibbonToolSize.Large;
            //tool.ToolClick += new ToolClickEventHandler(filter_ToolClick);

			// Create StateButtons for 'View Included Applications' and 'View Ignored Applications'
			StateButtonTool viewIncludedTool = new StateButtonTool("reports" + CommonToolNames.ViewIncluded);
			viewIncludedTool.Checked = true;
			viewIncludedTool.MenuDisplayStyle = Infragistics.Win.UltraWinToolbars.StateButtonMenuDisplayStyle.DisplayCheckmark;
			viewIncludedTool.ToolbarDisplayStyle = Infragistics.Win.UltraWinToolbars.StateButtonToolbarDisplayStyle.Glyph;
			viewIncludedTool.SharedProps.Caption = CommonToolNames.ViewIncluded;
			viewIncludedTool.SharedProps.ToolTipText = CommonToolNames.ViewIncludedTooltip;
			bgImage = Properties.Resources.show_application_16;
			viewIncludedTool.SharedProps.AppearancesSmall.Appearance.Image = bgImage;
			viewIncludedTool.SharedProps.AppearancesLarge.Appearance.Image = bgImage;
			viewIncludedTool.ToolClick += new ToolClickEventHandler(filter_ToolClick);
			WorkItem.UIExtensionSites[RibbonNames.filtersRibbonUISite].Add<ButtonTool>(viewIncludedTool);
			//
			StateButtonTool viewIgnoredTool = new StateButtonTool("reports" + CommonToolNames.ViewIgnored);
			viewIgnoredTool.Checked = true;
			viewIgnoredTool.MenuDisplayStyle = Infragistics.Win.UltraWinToolbars.StateButtonMenuDisplayStyle.DisplayCheckmark;
			viewIgnoredTool.ToolbarDisplayStyle = Infragistics.Win.UltraWinToolbars.StateButtonToolbarDisplayStyle.Glyph;
			viewIgnoredTool.SharedProps.Caption = CommonToolNames.ViewIgnored;
			viewIgnoredTool.SharedProps.ToolTipText = CommonToolNames.ViewIgnoredTooltip;
			bgImage = Properties.Resources.hide_application_16;
			viewIgnoredTool.SharedProps.AppearancesSmall.Appearance.Image = bgImage;
			viewIgnoredTool.SharedProps.AppearancesLarge.Appearance.Image = bgImage;
			viewIgnoredTool.ToolClick += new ToolClickEventHandler(filter_ToolClick);
			WorkItem.UIExtensionSites[RibbonNames.filtersRibbonUISite].Add<ButtonTool>(viewIgnoredTool);
		}


		/// <summary>
		/// Initialize the toolbar items for the Report Type Group
		/// </summary>
		private void InitializeReportTypeTools()
		{
			// Create a button for 'Application by Application' 
			ButtonTool tool = new ButtonTool("reports" + ToolNames.ApplicationByApplication);
			tool.SharedProps.Caption = ToolNames.ApplicationByApplication;
			tool.SharedProps.ToolTipText = "Select 'Application by Application' reporting";
			Image bgImage = Properties.Resources.application_reporting_32;
			tool.SharedProps.AppearancesLarge.Appearance.Image = bgImage;
			WorkItem.UIExtensionSites[RibbonNames.reportTypeRibbonUISite].Add<ButtonTool>(tool);
			tool.ToolClick += new ToolClickEventHandler(reportType_ToolClick);

			// Create a button for 'Application by Application' 
			tool = new ButtonTool("reports" + ToolNames.ComputerByComputer);
			tool.SharedProps.Caption = ToolNames.ComputerByComputer;
			tool.SharedProps.ToolTipText = "Select 'Asset by Asset' reporting";
			bgImage = Properties.Resources.computer_reporting_32;
			tool.SharedProps.AppearancesLarge.Appearance.Image = bgImage;
			WorkItem.UIExtensionSites[RibbonNames.reportTypeRibbonUISite].Add<ButtonTool>(tool);
			tool.ToolClick += new ToolClickEventHandler(reportType_ToolClick);

			// Create a button for 'Support Contracts' 
			tool = new ButtonTool("reports" + ToolNames.SupportContracts);
			tool.SharedProps.Caption = ToolNames.SupportContracts;
			tool.SharedProps.ToolTipText = "Select 'Support Contract' reporting";
			bgImage = Properties.Resources.support_reporting_32;
			tool.SharedProps.AppearancesLarge.Appearance.Image = bgImage;
			WorkItem.UIExtensionSites[RibbonNames.reportTypeRibbonUISite].Add<ButtonTool>(tool);
			tool.ToolClick += new ToolClickEventHandler(reportType_ToolClick);
		}



        /// <summary>
        /// This is the tool click handler for the Applications>Export ribbon group
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void export_ToolClick(object sender, ToolClickEventArgs e)
        {
            ReportsWorkItemController controller = workItem.Controller as ReportsWorkItemController;
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
		/// This is the tool click handler for the Filters ribbon group
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void filter_ToolClick(object sender, ToolClickEventArgs e)
		{
			StateButtonTool sbt = null;
			ReportsWorkItemController controller = workItem.Controller as ReportsWorkItemController;
			switch (e.Tool.SharedProps.Caption)
			{
				case CommonToolNames.ViewIncluded:
					sbt = e.Tool as StateButtonTool;
					controller.ShowIncludedApplications = sbt.Checked;

					// Ensure that at least one option is selected
					StateButtonTool sbtShowIgnored = filtersRibbonGroup.Tools["reports" + CommonToolNames.ViewIgnored] as StateButtonTool;
					if (!sbt.Checked && !sbtShowIgnored.Checked)
						sbtShowIgnored.Checked = true;
					break;

				case CommonToolNames.ViewIgnored:
					sbt = e.Tool as StateButtonTool;
					controller.ShowIgnoredApplications = sbt.Checked;

					// Ensure that at least one option is selected
					StateButtonTool sbtShowIncluded = filtersRibbonGroup.Tools["reports" + CommonToolNames.ViewIncluded] as StateButtonTool;
					if (!sbt.Checked && !sbtShowIncluded.Checked)
						sbtShowIncluded.Checked = true;
					break;

				case CommonToolNames.FilterPublishers:
					controller.FilterPublishers();
					break;

				default:
					break;
			}
		}



		/// <summary>
		/// This is the tool click handler for the Applications>Export ribbon group
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void reportType_ToolClick(object sender, ToolClickEventArgs e)
		{
			ReportsWorkItemController controller = workItem.Controller as ReportsWorkItemController;
			switch (e.Tool.SharedProps.Caption)
			{
				case ToolNames.ApplicationByApplication:
					//controller.ApplicationByApplication();
					break;

				case ToolNames.ComputerByComputer:
					//controller.ComputerByComputer();
					break;

				case ToolNames.SupportContracts:
					//controller.SupportContracts();
					break;

				default:
					break;
			}
		}


		/// <summary>
		/// This is the tool click handler for the Print ribbon group
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void print_ToolClick(object sender, ToolClickEventArgs e)
		{
			ReportsWorkItemController controller = workItem.Controller as ReportsWorkItemController;
			switch (e.Tool.SharedProps.Caption)
			{
				case CommonToolNames.Print:
					controller.PrintGrid();
					break;
					
				case CommonToolNames.PrintPreview:
					controller.PrintPreviewGrid();
					break;

				default:
					break;
			}
		}

        /// <summary>
        /// This is the tool click handler for the Print ribbon group
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void schedule_ToolClick(object sender, ToolClickEventArgs e)
        {
            ReportsWorkItemController controller = workItem.Controller as ReportsWorkItemController;
            switch (e.Tool.SharedProps.Caption)
            {
                case CommonToolNames.Schedule:
                    controller.ScheduleReports();
                    break;

                case CommonToolNames.LocationFilter:
                    controller.FilterLocations();
                    break;

                default:
                    break;
            }
        }

		/// <summary>
		/// This is the handler for the GLOBAL PublisherFilterChangeEvent which is fired when 
		/// the Filter has been updated elsewhere in the application.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		[EventSubscription(CommonEventTopics.PublisherFilterChanged)]
		public void PublisherFilterChangedHandler(object sender, PublisherFilterEventArgs e)
		{
			// Set the toolbar state to mirror that passed to us
			StateButtonTool sbtShowIncluded = filtersRibbonGroup.Tools["reports" + CommonToolNames.ViewIncluded] as StateButtonTool;
			sbtShowIncluded.Checked = e.ViewIncludedApplications;
			//
			StateButtonTool sbtShowIgnored = filtersRibbonGroup.Tools["reports" + CommonToolNames.ViewIgnored] as StateButtonTool;
			sbtShowIgnored.Checked = e.ViewIgnoredApplications;
		}

    }
}
