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
//
using Infragistics.Win.UltraWinToolbars;

namespace Layton.AuditWizard.Applications
{
    public class ApplicationsToolbarsController : LaytonToolbarsController
	{
#region Data
		private RibbonTab ribbonTab;
		private RibbonGroup filtersRibbonGroup;
		private RibbonGroup licensingRibbonGroup;
		private RibbonGroup exportRibbonGroup;
        private RibbonGroup reportsRibbonGroup;
        private RibbonGroup aliasingRibbonGroup;
		private ApplicationsWorkItem workItem;
		#endregion

#region Data Accessors
		public override RibbonTab RibbonTab
		{
			get { return ribbonTab; }
		}
		#endregion

#region Constructor
		[Microsoft.Practices.ObjectBuilder.InjectionConstructor]
		public ApplicationsToolbarsController([ServiceDependency] WorkItem workItem)
        {
            this.workItem = workItem as ApplicationsWorkItem;
        }
#endregion


        public override void Initialize()
        {
			// Create the ribbon groups and tools to add to the ribbon tab
			ribbonTab = new RibbonTab(Properties.Settings.Default.Title, Properties.Settings.Default.Title);

			// Now add the RibbonTab to the ribbonTabs collection and register the new ribbon tab's Groups collection
			WorkItem.RootWorkItem.UIExtensionSites[RibbonNames.applicationRibbonTabUISite].Add<RibbonTab>(ribbonTab);
			WorkItem.RootWorkItem.UIExtensionSites.RegisterSite(RibbonNames.applicationRibbonGroupUISite, ribbonTab.Groups);

			// LICENSING MENU GROUP
			// ====================
			//
			// Set the name, caption and image size
			licensingRibbonGroup = new RibbonGroup(RibbonNames.licensingGroupName);
			licensingRibbonGroup.Caption = RibbonNames.licensingGroupName;
			licensingRibbonGroup.PreferredToolSize = RibbonToolSize.Large;
			WorkItem.UIExtensionSites[RibbonNames.applicationRibbonGroupUISite].Add<RibbonGroup>(licensingRibbonGroup);
			WorkItem.RootWorkItem.UIExtensionSites.RegisterSite(RibbonNames.licensingRibbonUISite, licensingRibbonGroup.Tools);
			InitializeLicensingTools();

			// FILTERS MENU GROUP
			// =================
			//
			// Set the name, caption and image size for the 'filters' group
			filtersRibbonGroup = new RibbonGroup(RibbonNames.filtersGroupName);
			filtersRibbonGroup.Caption = RibbonNames.filtersGroupName;
			filtersRibbonGroup.PreferredToolSize = RibbonToolSize.Normal;
			WorkItem.UIExtensionSites[RibbonNames.applicationRibbonGroupUISite].Add<RibbonGroup>(filtersRibbonGroup);
			WorkItem.RootWorkItem.UIExtensionSites.RegisterSite(RibbonNames.filtersRibbonUISite, filtersRibbonGroup.Tools);
			InitializeFilterTools();

            // ALIASING MENU GROUP
            // ====================
            //
            // Set the name, caption and image size
            aliasingRibbonGroup = new RibbonGroup(RibbonNames.aliasingGroupName);
            aliasingRibbonGroup.Caption = RibbonNames.aliasingGroupName;
            aliasingRibbonGroup.PreferredToolSize = RibbonToolSize.Large;
            WorkItem.UIExtensionSites[RibbonNames.applicationRibbonGroupUISite].Add<RibbonGroup>(aliasingRibbonGroup);
            WorkItem.RootWorkItem.UIExtensionSites.RegisterSite(RibbonNames.aliasingRibbonUISite, aliasingRibbonGroup.Tools);
            InitializeAliasingTools();

            // REPORTS MENU GROUP
            // =================
            //
            // Set the name, caption and image size for the 'Configuration' group
            reportsRibbonGroup = new RibbonGroup("QuickReports");
            reportsRibbonGroup.Caption = "Quick Report";
            reportsRibbonGroup.PreferredToolSize = RibbonToolSize.Normal;
            WorkItem.UIExtensionSites[RibbonNames.applicationRibbonGroupUISite].Add<RibbonGroup>(reportsRibbonGroup);
            WorkItem.RootWorkItem.UIExtensionSites.RegisterSite(RibbonNames.reportsRibbonUISite, reportsRibbonGroup.Tools);
            InitializeReportTools();

            // EXPORT MENU GROUP
            // =================
            //
            // Set the name, caption and image size for the 'Configuration' group
            exportRibbonGroup = new RibbonGroup(RibbonNames.exportGroupName);
            exportRibbonGroup.Caption = RibbonNames.exportGroupName;
            exportRibbonGroup.PreferredToolSize = RibbonToolSize.Normal;
            WorkItem.UIExtensionSites[RibbonNames.applicationRibbonGroupUISite].Add<RibbonGroup>(exportRibbonGroup);
            WorkItem.RootWorkItem.UIExtensionSites.RegisterSite(RibbonNames.exportRibbonUISite, exportRibbonGroup.Tools);
            InitializeExportTools();
		}

		public override void UpdateTools()
		{
		}


		/// <summary>
		/// Initialize the toolbar items for the View Group
		/// </summary>
		private void InitializeFilterTools()
		{
			// Create a button tool for 'Filter Publishers' and add it to the Settings Group
			ButtonTool tool = new ButtonTool("applications" + CommonToolNames.FilterPublishers);
			tool.SharedProps.Caption = CommonToolNames.FilterPublishers;
			tool.SharedProps.ToolTipText = CommonToolNames.FilterPublishersTooltip;
			Image bgImage = Properties.Resources.filter_publishers_32;
			tool.SharedProps.AppearancesLarge.Appearance.Image = bgImage;
			ToolBase filterPublisherTool = WorkItem.UIExtensionSites[RibbonNames.filtersRibbonUISite].Add<ButtonTool>(tool);
			filterPublisherTool.InstanceProps.PreferredSizeOnRibbon = RibbonToolSize.Large;
			tool.ToolClick += new ToolClickEventHandler(filter_ToolClick);

			// Create StateButtons for 'View Included Applications' and 'View Ignored Applications'
			StateButtonTool viewIncludedTool = new StateButtonTool("applications" + CommonToolNames.ViewIncluded);
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
			StateButtonTool viewIgnoredTool = new StateButtonTool("applications" + CommonToolNames.ViewIgnored);
			
			// The default for 'View Ignored' is 'true' for AuditWizard and 'False' for LicenseWizard
			#if _AUDITWIZARD_
			viewIgnoredTool.Checked = true;
			#else
			viewIgnoredTool.Checked = false;
			#endif			
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
		/// Initialize the toolbar items for the LICENSING Group
		/// </summary>
		private void InitializeLicensingTools()
		{
			// Create a button tool for 'Set Not-NotIgnore' and add it to the Group
			ButtonTool tool = new ButtonTool(ToolNames.SetIgnored);
			tool.SharedProps.Caption = ToolNames.SetIgnored;
			tool.SharedProps.ToolTipText = ToolNames.SetIgnoredTooltip;
			Image bgImage = Properties.Resources.hide_application_32;
			tool.SharedProps.AppearancesSmall.Appearance.Image = bgImage;
			tool.SharedProps.AppearancesLarge.Appearance.Image = bgImage;
			WorkItem.UIExtensionSites[RibbonNames.licensingRibbonUISite].Add<ButtonTool>(tool);
			tool.ToolClick += new ToolClickEventHandler(licensing_ToolClick);

			// Create a button tool for 'Set NotIgnore' and add it to the View Group
			tool = new ButtonTool(ToolNames.SetIncluded);
			tool.SharedProps.Caption = ToolNames.SetIncluded;
			tool.SharedProps.ToolTipText = ToolNames.SetIncludedTooltip;
			bgImage = Properties.Resources.show_application_32;
			tool.SharedProps.AppearancesSmall.Appearance.Image = bgImage;
			tool.SharedProps.AppearancesLarge.Appearance.Image = bgImage;
			WorkItem.UIExtensionSites[RibbonNames.licensingRibbonUISite].Add<ButtonTool>(tool);
			tool.ToolClick += new ToolClickEventHandler(licensing_ToolClick);

			// Create a button tool for 'New Application'
			tool = new ButtonTool(ToolNames.NewLicense);
			tool.SharedProps.Caption = ToolNames.NewLicense;
			tool.SharedProps.ToolTipText = "Create a new license for this application";
			bgImage = Properties.Resources.license_add_32;
			tool.SharedProps.AppearancesLarge.Appearance.Image = bgImage;
			tool.SharedProps.Enabled = false;
			WorkItem.UIExtensionSites[RibbonNames.licensingRibbonUISite].Add<ButtonTool>(tool);
			tool.ToolClick += new ToolClickEventHandler(licensing_ToolClick);

			// Create a button tool for 'Edit License'
			tool = new ButtonTool(ToolNames.EditLicense);
			tool.SharedProps.Caption = ToolNames.EditLicense;
			tool.SharedProps.ToolTipText = "Edit the currently selected application license definition";
			bgImage = Properties.Resources.license_edit_32;
			tool.SharedProps.AppearancesLarge.Appearance.Image = bgImage;
			tool.SharedProps.Enabled = false;
			WorkItem.UIExtensionSites[RibbonNames.licensingRibbonUISite].Add<ButtonTool>(tool);
			tool.ToolClick += new ToolClickEventHandler(licensing_ToolClick);

			// Create a button tool for 'Show All Applications' and add it to the View Group
			tool = new ButtonTool(ToolNames.DeleteLicense);
			tool.SharedProps.Caption = ToolNames.DeleteLicense;
			tool.SharedProps.ToolTipText = "Delete the currently selected application license";
			tool.SharedProps.Enabled = false;
			bgImage = Properties.Resources.license_delete_32;
			tool.SharedProps.AppearancesLarge.Appearance.Image = bgImage;
			WorkItem.UIExtensionSites[RibbonNames.licensingRibbonUISite].Add<ButtonTool>(tool);
			tool.ToolClick += new ToolClickEventHandler(licensing_ToolClick);
		}


        /// <summary>
        /// Initialize the toolbar items for the ALIASING Group
        /// </summary>
        private void InitializeAliasingTools()
        {
            // Create a button tool for 'Alias Applications' and add it to the Group
            ButtonTool tool = new ButtonTool(ToolNames.AliasApplications);
            tool.SharedProps.Caption = ToolNames.AliasApplications;
            tool.SharedProps.ToolTipText = "Alias one or more Applications";
            Image bgImage = Properties.Resources.alias_application_32;
            tool.SharedProps.AppearancesLarge.Appearance.Image = bgImage;
            WorkItem.UIExtensionSites[RibbonNames.aliasingRibbonUISite].Add<ButtonTool>(tool);
            tool.ToolClick += new ToolClickEventHandler(aliasing_ToolClick);

            // Create a button tool for 'Alias Publishers' and add it to the Group
            tool = new ButtonTool(ToolNames.AliasPublishers);
            tool.SharedProps.Caption = ToolNames.AliasPublishers;
            tool.SharedProps.ToolTipText = "Alias one or more Publishers";
            bgImage = Properties.Resources.alias_publisher_32;
            tool.SharedProps.AppearancesLarge.Appearance.Image = bgImage;
            WorkItem.UIExtensionSites[RibbonNames.aliasingRibbonUISite].Add<ButtonTool>(tool);
            tool.ToolClick += new ToolClickEventHandler(aliasing_ToolClick);
        }



		/// <summary>
		/// Initialize the toolbar items for the EXPORT Group
		/// </summary>
		private void InitializeExportTools()
		{
			// Create a button for 'Export PDF' 
			ButtonTool tool = new ButtonTool("applications" + CommonToolNames.ExportPDF);
			tool.SharedProps.Caption = CommonToolNames.ExportPDF;
			tool.SharedProps.ToolTipText = "Export the grid data in Acrobat Portable Document Format (.PDF)";
			Image bgImage = Properties.Resources.pdf;
			tool.SharedProps.AppearancesSmall.Appearance.Image = bgImage;
			tool.SharedProps.AppearancesLarge.Appearance.Image = bgImage;
			WorkItem.UIExtensionSites[RibbonNames.exportRibbonUISite].Add<ButtonTool>(tool);
			tool.ToolClick += new ToolClickEventHandler(export_ToolClick);

			// Create a button for 'Export Excel'
			tool = new ButtonTool("applications" + CommonToolNames.ExportXLS);
			tool.SharedProps.Caption = CommonToolNames.ExportXLS;
			tool.SharedProps.ToolTipText = "Export the grid data in Microsoft Excel (.XLS) format";
			bgImage = Properties.Resources.excel_16;
			tool.SharedProps.AppearancesSmall.Appearance.Image = bgImage;
			tool.SharedProps.AppearancesLarge.Appearance.Image = bgImage;
			WorkItem.UIExtensionSites[RibbonNames.exportRibbonUISite].Add<ButtonTool>(tool);
			tool.ToolClick += new ToolClickEventHandler(export_ToolClick);

			// Create a button for 'Export XPS'
			tool = new ButtonTool("applications" + CommonToolNames.ExportXPS);
			tool.SharedProps.Caption = CommonToolNames.ExportXPS;
			tool.SharedProps.ToolTipText = "Export the grid data in XML Paper Specification (.XPS) format";
			bgImage = Properties.Resources.xps_16;
			tool.SharedProps.AppearancesSmall.Appearance.Image = bgImage;
			tool.SharedProps.AppearancesLarge.Appearance.Image = bgImage;
			WorkItem.UIExtensionSites[RibbonNames.exportRibbonUISite].Add<ButtonTool>(tool);
			tool.ToolClick += new ToolClickEventHandler(export_ToolClick);
		}

        /// <summary>
        /// Initialize the toolbar items for the reports Group
        /// </summary>
        private void InitializeReportTools()
        {
            // Create a button for 'Export PDF' 
            ButtonTool tool = new ButtonTool("reportcompliant");
            tool.SharedProps.Caption = "View all compliant applications";
            tool.SharedProps.ToolTipText = "Report on all compliant applications";
            Image bgImage = Properties.Resources.tick;
            tool.SharedProps.AppearancesSmall.Appearance.Image = bgImage;
            tool.SharedProps.AppearancesLarge.Appearance.Image = bgImage;
            WorkItem.UIExtensionSites[RibbonNames.reportsRibbonUISite].Add<ButtonTool>(tool);
            tool.ToolClick += new ToolClickEventHandler(reportCompliantPublishers_ToolClick);

            // Create a button for 'Export Excel'
            tool = new ButtonTool("reportnoncompliant");
            tool.SharedProps.Caption = "View all non-compliant applications";
            tool.SharedProps.ToolTipText = "Report on all non-compliant applications";
            bgImage = Properties.Resources.cross;
            tool.SharedProps.AppearancesSmall.Appearance.Image = bgImage;
            tool.SharedProps.AppearancesLarge.Appearance.Image = bgImage;
            WorkItem.UIExtensionSites[RibbonNames.reportsRibbonUISite].Add<ButtonTool>(tool);
            tool.ToolClick += new ToolClickEventHandler(reportNonCompliantPublishers_ToolClick);

            // Create a button for 'Export XPS'
            tool = new ButtonTool("reportall");
            tool.SharedProps.Caption = "View all applications";
            tool.SharedProps.ToolTipText = "Report on all applications";
            bgImage = Properties.Resources.application_16;
            tool.SharedProps.AppearancesSmall.Appearance.Image = bgImage;
            tool.SharedProps.AppearancesLarge.Appearance.Image = bgImage;
            WorkItem.UIExtensionSites[RibbonNames.reportsRibbonUISite].Add<ButtonTool>(tool);
            tool.ToolClick += new ToolClickEventHandler(reportAllPublishers_ToolClick);
        }



		/// <summary>
		/// This is the tool click handler for the Filters ribbon group
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void filter_ToolClick(object sender, ToolClickEventArgs e)
		{
            Cursor.Current = Cursors.WaitCursor;

			StateButtonTool sbt = null;
			ApplicationsWorkItemController controller = workItem.Controller as ApplicationsWorkItemController;
			switch (e.Tool.SharedProps.Caption)
			{
				case CommonToolNames.ViewIncluded:
					sbt = e.Tool as StateButtonTool;
					controller.ShowIncludedApplications = sbt.Checked;

					// Ensure that at least one option is selected
					StateButtonTool sbtShowIgnored = filtersRibbonGroup.Tools["applications" + CommonToolNames.ViewIgnored] as StateButtonTool;
					if (!sbt.Checked && !sbtShowIgnored.Checked)
						sbtShowIgnored.Checked = true;
					break;

				case CommonToolNames.ViewIgnored:
					sbt = e.Tool as StateButtonTool;
					controller.ShowIgnoredApplications = sbt.Checked;

					// Ensure that at least one option is selected
					StateButtonTool sbtShowIncluded = filtersRibbonGroup.Tools["applications" + CommonToolNames.ViewIncluded] as StateButtonTool;
					if (!sbt.Checked && !sbtShowIncluded.Checked)
						sbtShowIncluded.Checked = true;
					break;

				case CommonToolNames.FilterPublishers:
					controller.FilterPublishers();
					break;

				default:
					break;
			}

            Cursor.Current = Cursors.Default;
		}


		/// <summary>
		/// This is the tool click handler for the Applications>Licensing ribbon group
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void licensing_ToolClick(object sender, ToolClickEventArgs e)
		{
			ApplicationsWorkItemController controller = workItem.Controller as ApplicationsWorkItemController;
			switch (e.Tool.SharedProps.Caption)
			{
				case ToolNames.SetIgnored:
					controller.SetIgnored();
					break;

				case ToolNames.SetIncluded:
					controller.SetIncluded();
					break;
				case ToolNames.NewLicense:
					controller.NewLicense();
					break;

				case ToolNames.EditLicense:
					controller.EditLicense();
					break;

				case ToolNames.DeleteLicense:
					controller.DeleteLicense();
					break;

				default:
					break;
			}
		}

        /// <summary>
        /// This is the tool click handler for the Applications>Aliasing ribbon group
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void aliasing_ToolClick(object sender, ToolClickEventArgs e)
        {
            ApplicationsWorkItemController controller = workItem.Controller as ApplicationsWorkItemController;
            switch (e.Tool.SharedProps.Caption)
            {
                case ToolNames.AliasApplications:
                    controller.AliasApplications();
                    break;

                case ToolNames.AliasPublishers:
                    controller.AliasPublishers();
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
		private void export_ToolClick(object sender, ToolClickEventArgs e)
		{
			ApplicationsWorkItemController controller = workItem.Controller as ApplicationsWorkItemController;
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
        /// This is the tool click handler for the Applications>Report ribbon group
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void reportAllPublishers_ToolClick(object sender, ToolClickEventArgs e)
        {
            ILaytonView applicationsTabView = WorkItem.Items[Layton.Cab.Interface.ViewNames.MainTabView] as ILaytonView;
            ((ApplicationsTabView)applicationsTabView).ReportAllPublishers();

            //ILaytonView tabView = (ILaytonView)WorkItem.RootWorkItem.Workspaces[WorkspaceNames.TabWorkspace].ActiveSmartPart as ApplicationsTabView;
            //((ApplicationsTabView)tabView).ReportAllPublishers();
        }

        /// <summary>
        /// This is the tool click handler for the Applications>Report ribbon group
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void reportCompliantPublishers_ToolClick(object sender, ToolClickEventArgs e)
        {
            ILaytonView applicationsTabView = WorkItem.Items[Layton.Cab.Interface.ViewNames.MainTabView] as ILaytonView;
            ((ApplicationsTabView)applicationsTabView).ReportCompliantPublishers();

            //ILaytonView tabView = (ILaytonView)WorkItem.RootWorkItem.Workspaces[WorkspaceNames.TabWorkspace].ActiveSmartPart;
            //((ApplicationsTabView)tabView).ReportCompliantPublishers();
        }

        /// <summary>
        /// This is the tool click handler for the Applications>Report ribbon group
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void reportNonCompliantPublishers_ToolClick(object sender, ToolClickEventArgs e)
        {
            ILaytonView applicationsTabView = WorkItem.Items[Layton.Cab.Interface.ViewNames.MainTabView] as ILaytonView;
            ((ApplicationsTabView)applicationsTabView).ReportNonCompliantPublishers();

            //ILaytonView tabView = (ILaytonView)WorkItem.RootWorkItem.Workspaces[WorkspaceNames.TabWorkspace].ActiveSmartPart;
            //((ApplicationsTabView)tabView).ReportNonCompliantPublishers();
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
			StateButtonTool sbtIncluded = filtersRibbonGroup.Tools["applications" + CommonToolNames.ViewIncluded] as StateButtonTool;
			sbtIncluded.Checked = e.ViewIncludedApplications;
			//
			StateButtonTool sbtShowIgnored = filtersRibbonGroup.Tools["applications" + CommonToolNames.ViewIgnored] as StateButtonTool;
			sbtShowIgnored.Checked = e.ViewIgnoredApplications;
		}
	
	
	}
}
