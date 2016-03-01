using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.IO;
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

namespace Layton.AuditWizard.Network
{
	public class NetworkToolbarsController : LaytonToolbarsController
	{
		private RibbonTab ribbonTab;
		private RibbonGroup viewRibbonGroup;
		private RibbonGroup exportRibbonGroup;
		private RibbonGroup auditRibbonGroup;
		private RibbonGroup filtersRibbonGroup;
		private RibbonGroup findRibbonGroup;
		private NetworkWorkItem workItem;

        [Microsoft.Practices.ObjectBuilder.InjectionConstructor]
		public NetworkToolbarsController([ServiceDependency] WorkItem workItem)
        {
            this.workItem = workItem as NetworkWorkItem;
        }

		public override RibbonTab RibbonTab
		{
			get { return ribbonTab; }
		}

		public override void UpdateTools()
		{
		}

		public override void Initialize()
		{
			// Create the ribbon groups and tools to add to the ribbon tab
			ribbonTab = new RibbonTab(Properties.Settings.Default.Title, Properties.Settings.Default.Title);

			// Now add the RibbonTab to the ribbonTabs collection and register the new ribbon tab's Groups collection
			WorkItem.RootWorkItem.UIExtensionSites[RibbonNames.networkRibbonTabUISite].Add<RibbonTab>(ribbonTab);
			WorkItem.RootWorkItem.UIExtensionSites.RegisterSite(RibbonNames.networkRibbonGroupUISite, ribbonTab.Groups);

			// Add the 'Audit' ribbon group
			auditRibbonGroup = new RibbonGroup(RibbonNames.auditGroupName);
			auditRibbonGroup.Caption = RibbonNames.auditGroupName;
			auditRibbonGroup.PreferredToolSize = RibbonToolSize.Large;
			WorkItem.UIExtensionSites[RibbonNames.networkRibbonGroupUISite].Add<RibbonGroup>(auditRibbonGroup);
			WorkItem.RootWorkItem.UIExtensionSites.RegisterSite(RibbonNames.auditRibbonUISite, auditRibbonGroup.Tools);
			InitializeAuditTools();

			// Add the 'View' ribbon group
			viewRibbonGroup = new RibbonGroup(RibbonNames.viewGroupName);
			viewRibbonGroup.Caption = RibbonNames.viewGroupName;
			viewRibbonGroup.PreferredToolSize = RibbonToolSize.Normal;
			WorkItem.UIExtensionSites[RibbonNames.networkRibbonGroupUISite].Add<RibbonGroup>(viewRibbonGroup);
			WorkItem.RootWorkItem.UIExtensionSites.RegisterSite(RibbonNames.viewRibbonUISite, viewRibbonGroup.Tools);
			InitializeViewTools();

			// FILTERS MENU GROUP
			// =================
			//
			// Set the name, caption and image size for the 'filters' group
			filtersRibbonGroup = new RibbonGroup(RibbonNames.filtersGroupName);
			filtersRibbonGroup.Caption = RibbonNames.filtersGroupName;
			filtersRibbonGroup.PreferredToolSize = RibbonToolSize.Normal;
			WorkItem.UIExtensionSites[RibbonNames.networkRibbonGroupUISite].Add<RibbonGroup>(filtersRibbonGroup);
			WorkItem.RootWorkItem.UIExtensionSites.RegisterSite(RibbonNames.filtersRibbonUISite, filtersRibbonGroup.Tools);
			InitializeFilterTools();

			// Add the 'Export' ribbon group
			exportRibbonGroup = new RibbonGroup(RibbonNames.exportGroupName);
			exportRibbonGroup.Caption = RibbonNames.exportGroupName;
			exportRibbonGroup.PreferredToolSize = RibbonToolSize.Normal;
			WorkItem.UIExtensionSites[RibbonNames.networkRibbonGroupUISite].Add<RibbonGroup>(exportRibbonGroup);
			WorkItem.RootWorkItem.UIExtensionSites.RegisterSite(RibbonNames.exportRibbonUISite, exportRibbonGroup.Tools);
			InitializeExportTools();

			// Add the 'Find' ribbon group
			findRibbonGroup = new RibbonGroup(RibbonNames.findGroupName);
			findRibbonGroup.Caption = RibbonNames.findGroupName;
			findRibbonGroup.PreferredToolSize = RibbonToolSize.Large;
			WorkItem.UIExtensionSites[RibbonNames.networkRibbonGroupUISite].Add<RibbonGroup>(findRibbonGroup);
			WorkItem.RootWorkItem.UIExtensionSites.RegisterSite(RibbonNames.findRibbonUISite, findRibbonGroup.Tools);
			InitializeFindTools();
		}


		/// <summary>
		/// Initialize the tools on the 'Views' ribbon tab
		/// </summary>
		private void InitializeViewTools()
		{
			// Get the initial settings for teh various show flags from teh configuration
			bool showStock = false;
			bool showInUse = true;
			bool showPending = false;
			bool showDisposed = false;

			// Get the current 'show' flags
			Configuration config = ConfigurationManager.OpenExeConfiguration(Path.Combine(Application.StartupPath, "AuditWizardv8.exe"));
			try
			{
				showStock = Convert.ToBoolean(config.AppSettings.Settings["ShowStock"].Value);
				showInUse = Convert.ToBoolean(config.AppSettings.Settings["ShowInUse"].Value);
				showPending = Convert.ToBoolean(config.AppSettings.Settings["ShowPending"].Value);
				showDisposed = Convert.ToBoolean(config.AppSettings.Settings["ShowDisposed"].Value);
			}
			catch (Exception)
			{ }		
		
			// Create a button tool for 'User Locations View'
			ButtonTool userViewTool = new ButtonTool("network" + ToolNames.UserLocationsView);
			userViewTool.SharedProps.Caption = ToolNames.UserLocationsView;
			userViewTool.SharedProps.ToolTipText = ToolNames.UserLocationsTooltip;
			userViewTool.SharedProps.AppearancesLarge.Appearance.Image = Properties.Resources.location_32;
			userViewTool.ToolClick += new ToolClickEventHandler(view_ToolClick);
			ToolBase userViewToolInstance = WorkItem.UIExtensionSites[RibbonNames.viewRibbonUISite].Add<ButtonTool>(userViewTool);
			userViewToolInstance.InstanceProps.PreferredSizeOnRibbon = RibbonToolSize.Large;

			// Create a button tool for 'Domain View'
			ButtonTool domainViewTool = new ButtonTool("network" + ToolNames.DomainView);
			domainViewTool.SharedProps.Caption = ToolNames.DomainView;
			domainViewTool.SharedProps.ToolTipText = ToolNames.DomainTooltip;
			domainViewTool.SharedProps.AppearancesLarge.Appearance.Image = Properties.Resources.domain32;
			domainViewTool.ToolClick += new ToolClickEventHandler(view_ToolClick);
			ToolBase domainViewToolInstance = WorkItem.UIExtensionSites[RibbonNames.viewRibbonUISite].Add<ButtonTool>(domainViewTool);
			domainViewToolInstance.InstanceProps.PreferredSizeOnRibbon = RibbonToolSize.Large;

			// Create a button tool for 'Show Stock Assets' and add it to the Group
			StateButtonTool showStockTool = new StateButtonTool("network" + ToolNames.ShowStockAssets);
			showStockTool.Checked = showStock;
			showStockTool.MenuDisplayStyle = Infragistics.Win.UltraWinToolbars.StateButtonMenuDisplayStyle.DisplayCheckmark;
			showStockTool.ToolbarDisplayStyle = Infragistics.Win.UltraWinToolbars.StateButtonToolbarDisplayStyle.Glyph;
			showStockTool.SharedProps.Caption = ToolNames.ShowStockAssets;
			showStockTool.SharedProps.ToolTipText = ToolNames.ShowStockAssetsTooltip;
			showStockTool.SharedProps.AppearancesSmall.Appearance.Image = Properties.Resources.computer_stock_16;
			showStockTool.SharedProps.AppearancesLarge.Appearance.Image = Properties.Resources.computer_stock_16;
			WorkItem.UIExtensionSites[RibbonNames.viewRibbonUISite].Add<ButtonTool>(showStockTool);
			showStockTool.ToolClick += new ToolClickEventHandler(view_ToolClick);

			// Create a button tool for 'Show In Use Assets' and add it to the Group
			StateButtonTool showInUseTool = new StateButtonTool("network" + ToolNames.ShowInUseAssets);
			showInUseTool.Checked = showInUse;
			showInUseTool.MenuDisplayStyle = Infragistics.Win.UltraWinToolbars.StateButtonMenuDisplayStyle.DisplayCheckmark;
			showInUseTool.ToolbarDisplayStyle = Infragistics.Win.UltraWinToolbars.StateButtonToolbarDisplayStyle.Glyph;
			showInUseTool.SharedProps.Caption = ToolNames.ShowInUseAssets;
			showInUseTool.SharedProps.ToolTipText = ToolNames.ShowInUseAssetsTooltip;
			showInUseTool.SharedProps.AppearancesSmall.Appearance.Image = Properties.Resources.computer16;
			showInUseTool.SharedProps.AppearancesLarge.Appearance.Image = Properties.Resources.computer16;
			WorkItem.UIExtensionSites[RibbonNames.viewRibbonUISite].Add<ButtonTool>(showInUseTool);
			showInUseTool.ToolClick += new ToolClickEventHandler(view_ToolClick);

			// Create a button tool for 'Show Pending Disposal Assets' and add it to the Group
			StateButtonTool showPendingTool = new StateButtonTool("network" + ToolNames.ShowPendingAssets);
			showPendingTool.Checked = showPending;
			showPendingTool.MenuDisplayStyle = Infragistics.Win.UltraWinToolbars.StateButtonMenuDisplayStyle.DisplayCheckmark;
			showPendingTool.ToolbarDisplayStyle = Infragistics.Win.UltraWinToolbars.StateButtonToolbarDisplayStyle.Glyph;
			showPendingTool.SharedProps.Caption = ToolNames.ShowPendingAssets;
			showPendingTool.SharedProps.ToolTipText = ToolNames.ShowPendingAssetsTooltip;
			showPendingTool.SharedProps.AppearancesSmall.Appearance.Image = Properties.Resources.computer_pending_16;
			showPendingTool.SharedProps.AppearancesLarge.Appearance.Image = Properties.Resources.computer_pending_16;
			WorkItem.UIExtensionSites[RibbonNames.viewRibbonUISite].Add<ButtonTool>(showPendingTool);
			showPendingTool.ToolClick += new ToolClickEventHandler(view_ToolClick);

			// Create a button tool for 'Show Disposed Assets' and add it to the Group
			StateButtonTool showDisposedTool = new StateButtonTool("network" + ToolNames.ShowDisposedAssets);
			showDisposedTool.Checked = showDisposed;
			showDisposedTool.MenuDisplayStyle = Infragistics.Win.UltraWinToolbars.StateButtonMenuDisplayStyle.DisplayCheckmark;
			showDisposedTool.ToolbarDisplayStyle = Infragistics.Win.UltraWinToolbars.StateButtonToolbarDisplayStyle.Glyph;
			showDisposedTool.SharedProps.Caption = ToolNames.ShowDisposedAssets;
			showDisposedTool.SharedProps.ToolTipText = ToolNames.ShowDisposedAssetsTooltip;
			showDisposedTool.SharedProps.AppearancesSmall.Appearance.Image = Properties.Resources.computer_disposed_16;
			showDisposedTool.SharedProps.AppearancesLarge.Appearance.Image = Properties.Resources.computer_disposed_16;
			WorkItem.UIExtensionSites[RibbonNames.viewRibbonUISite].Add<ButtonTool>(showDisposedTool);
			showDisposedTool.ToolClick += new ToolClickEventHandler(view_ToolClick);
		}

		/// <summary>
		/// Initialize the toolbar items for the EXPORT Group
		/// </summary>
		private void InitializeExportTools()
		{
			// Create a button for 'Export PDF' 
			ButtonTool tool = new ButtonTool("network" + CommonToolNames.ExportPDF);
			tool.SharedProps.Caption = CommonToolNames.ExportPDF;
			tool.SharedProps.ToolTipText = "Export the grid data in Acrobat Portable Document Format (.PDF)";
			Image bgImage = Properties.Resources.pdf;
			tool.SharedProps.AppearancesSmall.Appearance.Image = bgImage;
			tool.SharedProps.AppearancesLarge.Appearance.Image = bgImage;
			WorkItem.UIExtensionSites[RibbonNames.exportRibbonUISite].Add<ButtonTool>(tool);
			tool.ToolClick += new ToolClickEventHandler(export_ToolClick);

			// Create a button for 'Export Excel'
			tool = new ButtonTool("network" + CommonToolNames.ExportXLS);
			tool.SharedProps.Caption = CommonToolNames.ExportXLS;
			tool.SharedProps.ToolTipText = "Export the grid data in Microsoft Excel (.XLS) format";
			bgImage = Properties.Resources.excel_16;
			tool.SharedProps.AppearancesSmall.Appearance.Image = bgImage;
			tool.SharedProps.AppearancesLarge.Appearance.Image = bgImage;
			WorkItem.UIExtensionSites[RibbonNames.exportRibbonUISite].Add<ButtonTool>(tool);
			tool.ToolClick += new ToolClickEventHandler(export_ToolClick);

			// Create a button for 'Export XPS'
			tool = new ButtonTool("network" + CommonToolNames.ExportXPS);
			tool.SharedProps.Caption = CommonToolNames.ExportXPS;
			tool.SharedProps.ToolTipText = "Export the grid data in XML Paper Specification (.XPS) format";
			bgImage = Properties.Resources.xps_16;
			tool.SharedProps.AppearancesSmall.Appearance.Image = bgImage;
			tool.SharedProps.AppearancesLarge.Appearance.Image = bgImage;
			WorkItem.UIExtensionSites[RibbonNames.exportRibbonUISite].Add<ButtonTool>(tool);
			tool.ToolClick += new ToolClickEventHandler(export_ToolClick);
		}


	    /// <summary>
		/// Initialize tools on the Audit tab
		/// </summary>
		private void InitializeAuditTools()
		{
			// Add the "Upload Results" button
			ButtonTool uploadTool = new ButtonTool("network" + ToolNames.UploadData);
			uploadTool.SharedProps.Caption = ToolNames.UploadData;
			uploadTool.SharedProps.ToolTipText = "Upload the results of audits performed";
			uploadTool.SharedProps.AppearancesLarge.Appearance.Image = Properties.Resources.audit_upload_32;
			WorkItem.UIExtensionSites[RibbonNames.auditRibbonUISite].Add<ButtonTool>(uploadTool);
			uploadTool.ToolClick += new ToolClickEventHandler(audit_ToolClick);

			// Add the "Audit Now" buttton
			ButtonTool reAuditFileTool = new ButtonTool("network" + ToolNames.ReauditComputer);
			reAuditFileTool.SharedProps.Caption = ToolNames.ReauditComputer;
			reAuditFileTool.SharedProps.ToolTipText = "Request the Audit Agent to immediately re-audit the selected computer";
			reAuditFileTool.SharedProps.AppearancesLarge.Appearance.Image = Properties.Resources.request_reaudit_32;
			WorkItem.UIExtensionSites[RibbonNames.auditRibbonUISite].Add<ButtonTool>(reAuditFileTool);
			reAuditFileTool.ToolClick += new ToolClickEventHandler(audit_ToolClick);

			// Add the "Operations Log" buttton
			ButtonTool operationsLogFileTool = new ButtonTool("network" + ToolNames.OperationsLog);
			operationsLogFileTool.SharedProps.Caption = ToolNames.OperationsLog;
			operationsLogFileTool.SharedProps.ToolTipText = "Displays a form showing the AuditWizard Operations Log";
			operationsLogFileTool.SharedProps.AppearancesLarge.Appearance.Image = Properties.Resources.OperationsLog_32;
			WorkItem.UIExtensionSites[RibbonNames.auditRibbonUISite].Add<ButtonTool>(operationsLogFileTool);
			operationsLogFileTool.ToolClick += new ToolClickEventHandler(audit_ToolClick);
		}



		/// <summary>
		/// Initialize the toolbar items for the View Group
		/// </summary>
		private void InitializeFilterTools()
		{
			// Create a button tool for 'Filter Publishers' and add it to the Settings Group
			ButtonTool tool = new ButtonTool("network" + CommonToolNames.FilterPublishers);
			tool.SharedProps.Caption = CommonToolNames.FilterPublishers;
			tool.SharedProps.ToolTipText = CommonToolNames.FilterPublishersTooltip;
			tool.SharedProps.AppearancesLarge.Appearance.Image = Properties.Resources.filter_publishers_32;
			ToolBase filterPublisherTool = WorkItem.UIExtensionSites[RibbonNames.filtersRibbonUISite].Add<ButtonTool>(tool);
			filterPublisherTool.InstanceProps.PreferredSizeOnRibbon = RibbonToolSize.Large;
			tool.ToolClick += new ToolClickEventHandler(filter_ToolClick);

			// Create StateButtons for 'View Included Applications' and 'View Ignored Applications'
			StateButtonTool viewNotIgnoreTool = new StateButtonTool("network" + CommonToolNames.ViewIncluded);
			viewNotIgnoreTool.Checked = true;
			viewNotIgnoreTool.MenuDisplayStyle = Infragistics.Win.UltraWinToolbars.StateButtonMenuDisplayStyle.DisplayCheckmark;
			viewNotIgnoreTool.ToolbarDisplayStyle = Infragistics.Win.UltraWinToolbars.StateButtonToolbarDisplayStyle.Glyph;
			viewNotIgnoreTool.SharedProps.Caption = CommonToolNames.ViewIncluded;
			viewNotIgnoreTool.SharedProps.ToolTipText = CommonToolNames.ViewIncludedTooltip;
			Bitmap bgImage = Properties.Resources.show_application_16;
			viewNotIgnoreTool.SharedProps.AppearancesSmall.Appearance.Image = bgImage;
			viewNotIgnoreTool.SharedProps.AppearancesLarge.Appearance.Image = bgImage;
			viewNotIgnoreTool.ToolClick += new ToolClickEventHandler(filter_ToolClick);
			WorkItem.UIExtensionSites[RibbonNames.filtersRibbonUISite].Add<ButtonTool>(viewNotIgnoreTool);
			//
			StateButtonTool viewIgnoredTool = new StateButtonTool("network" + CommonToolNames.ViewIgnored);
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
		/// Initialize the toolbar items for the FIND Group
		/// </summary>
		private void InitializeFindTools()
		{
			// Create a button for 'Export PDF' 
			ButtonTool tool = new ButtonTool("network" + ToolNames.FindAsset);
			tool.SharedProps.Caption = ToolNames.FindAsset;
			tool.SharedProps.ToolTipText = "Search the database";
			Image bgImage = Properties.Resources.asset_find_32;
			tool.InstanceProps.PreferredSizeOnRibbon = RibbonToolSize.Large;
			tool.SharedProps.AppearancesSmall.Appearance.Image = bgImage;
			tool.SharedProps.AppearancesLarge.Appearance.Image = bgImage;
			WorkItem.UIExtensionSites[RibbonNames.findRibbonUISite].Add<ButtonTool>(tool);
			tool.ToolClick += new ToolClickEventHandler(find_ToolClick);
		}
		


		/// <summary>
		/// This is the tool click handler for the Network->View ribbon group
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void view_ToolClick(object sender, ToolClickEventArgs e)
		{
			StateButtonTool sbt = null;
			NetworkWorkItemController controller = workItem.Controller as NetworkWorkItemController;
			switch (e.Tool.SharedProps.Caption)
			{
				case ToolNames.ShowStockAssets:
					sbt = e.Tool as StateButtonTool;
					controller.ShowStockAssets(sbt.Checked);
					break;

				case ToolNames.ShowInUseAssets:
					sbt = e.Tool as StateButtonTool;
					controller.ShowInUseAssets(sbt.Checked);
					break;

				case ToolNames.ShowPendingAssets:
					sbt = e.Tool as StateButtonTool;
					controller.ShowPendingAssets(sbt.Checked);
					break;

				case ToolNames.ShowDisposedAssets:
					sbt = e.Tool as StateButtonTool;
					controller.ShowDisposedAssets(sbt.Checked);
					break;

				case ToolNames.UserLocationsView:
					controller.DomainViewStyle = false;
					break;

				case ToolNames.DomainView:
					controller.DomainViewStyle = true;
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
			NetworkWorkItemController controller = workItem.Controller as NetworkWorkItemController;
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
			NetworkWorkItemController controller = workItem.Controller as NetworkWorkItemController;
			switch (e.Tool.SharedProps.Caption)
			{
				case CommonToolNames.ViewIncluded:
					sbt = e.Tool as StateButtonTool;
					controller.ShowIncludedApplications = sbt.Checked;

					// Ensure that at least one option is selected
					StateButtonTool sbtShowIgnored = filtersRibbonGroup.Tools["network" + CommonToolNames.ViewIgnored] as StateButtonTool;
					if (!sbt.Checked && !sbtShowIgnored.Checked)
						sbtShowIgnored.Checked = true;
					break;

				case CommonToolNames.ViewIgnored:
					sbt = e.Tool as StateButtonTool;
					controller.ShowIgnoredApplications = sbt.Checked;

					// Ensure that at least one option is selected
					StateButtonTool sbtShowIncluded = filtersRibbonGroup.Tools["network" + CommonToolNames.ViewIncluded] as StateButtonTool;
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


	    void audit_ToolClick(object sender, ToolClickEventArgs e)
		{
			NetworkWorkItemController controller = workItem.Controller as NetworkWorkItemController;
			switch (e.Tool.SharedProps.Caption)
			{
				case ToolNames.UploadData:
					controller.UploadData();
					break;
				case ToolNames.ReauditComputer:
					controller.RequestReaudit();
					break;
				case ToolNames.OperationsLog:
					controller.OperationsLog();
					break;
				default:
					break;
			}
		}




		/// <summary>
		/// This is the tool click handler for the Network->Find ribbon group
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void find_ToolClick(object sender, ToolClickEventArgs e)
		{
			NetworkWorkItemController controller = workItem.Controller as NetworkWorkItemController;
			switch (e.Tool.SharedProps.Caption)
			{
				case ToolNames.FindAsset:
					controller.FindAsset();
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
			StateButtonTool sbtShowIncluded = filtersRibbonGroup.Tools["network" + CommonToolNames.ViewIncluded] as StateButtonTool;
			sbtShowIncluded.Checked = e.ViewIncludedApplications;
			//
			StateButtonTool sbtShowIgnored = filtersRibbonGroup.Tools["network" + CommonToolNames.ViewIgnored] as StateButtonTool;
			sbtShowIgnored.Checked = e.ViewIgnoredApplications;
		}

	}
}
