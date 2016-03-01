using System;
using System.Collections.Generic;
using System.Text;
using Layton.Cab.Interface;
using Infragistics.Win.UltraWinToolbars;

namespace Layton.AuditWizard.Overview
{
    public class OverviewToolbarsController : LaytonToolbarsController
    {
        private RibbonTab ribbonTab;
        private RibbonGroup wizardRibbonGroup;
		private RibbonGroup setupRibbonGroup;
		private RibbonGroup alertsRibbonGroup;
        private RibbonGroup tasksRibbonGroup;

        public override RibbonTab RibbonTab
        {
            get { return ribbonTab; }
        }

        public override void Initialize()
        {
            // TODO:  Create the ribbon groups and tools to add to the ribbon tab
            ribbonTab = new RibbonTab(Properties.Settings.Default.Title, Properties.Settings.Default.Title);

            // Now add the RibbonTab to the ribbonTabs collection and register the new ribbon tab's Groups collection
            WorkItem.RootWorkItem.UIExtensionSites["ribbonTabs"].Add<RibbonTab>(ribbonTab);
            WorkItem.RootWorkItem.UIExtensionSites.RegisterSite("OverviewRibbonTabs", ribbonTab.Groups);

            InitializeWizardTools();
        }

        public override void UpdateTools()
        {
            // TODO:  Update the state of any tools
        }

        private void InitializeWizardTools()
        {
            // Add the "wizard" RibbonGroup to the Overview RibbonTab
            wizardRibbonGroup = new RibbonGroup(RibbonNames.wizardGroupName);
            wizardRibbonGroup.Caption = RibbonNames.wizardGroupName;
            wizardRibbonGroup.PreferredToolSize = RibbonToolSize.Large;
            WorkItem.UIExtensionSites["OverviewRibbonTabs"].Add<RibbonGroup>(wizardRibbonGroup);
            WorkItem.RootWorkItem.UIExtensionSites.RegisterSite(RibbonNames.wizardRibbonUISite, wizardRibbonGroup.Tools);

            // add the wizard RibbonGroup tools...
            ButtonTool wizardTool = new ButtonTool(ToolNames.SetupWizard);
            wizardTool.SharedProps.Caption = ToolNames.SetupWizard;
            wizardTool.SharedProps.ToolTipText = "Displays the AuditWizard Setup wizard";
            wizardTool.SharedProps.AppearancesLarge.Appearance.Image = Properties.Resources.setup_wizard_32;
            WorkItem.UIExtensionSites[RibbonNames.wizardRibbonUISite].Add<ButtonTool>(wizardTool);
            wizardTool.ToolClick += new ToolClickEventHandler(wizardTool_ToolClick);
			
			// Add the Alerts toolbar
			alertsRibbonGroup = new RibbonGroup(RibbonNames.alertsGroupName);
			alertsRibbonGroup.Caption = RibbonNames.alertsGroupName;
			alertsRibbonGroup.PreferredToolSize = RibbonToolSize.Large;
			WorkItem.UIExtensionSites["OverviewRibbonTabs"].Add<RibbonGroup>(alertsRibbonGroup);
			WorkItem.RootWorkItem.UIExtensionSites.RegisterSite(RibbonNames.alertsRibbonUISite, alertsRibbonGroup.Tools);

			// Add the "Alert Log" buttton
			ButtonTool alertLogFileTool = new ButtonTool("network" + ToolNames.AlertLog);
			alertLogFileTool.SharedProps.Caption = ToolNames.AlertLog;
			alertLogFileTool.SharedProps.ToolTipText = "Displays the AuditWizard Alert Log";
			alertLogFileTool.SharedProps.AppearancesLarge.Appearance.Image = Properties.Resources.alert_log_32;
			WorkItem.UIExtensionSites[RibbonNames.alertsRibbonUISite].Add<ButtonTool>(alertLogFileTool);
			alertLogFileTool.ToolClick += new ToolClickEventHandler(alertsTool_ToolClick);

            // Add the Tasks toolbar
            tasksRibbonGroup = new RibbonGroup("Manage Tasks");
            tasksRibbonGroup.Caption = RibbonNames.tasksGroupName;
            tasksRibbonGroup.PreferredToolSize = RibbonToolSize.Large;
            WorkItem.UIExtensionSites["OverviewRibbonTabs"].Add<RibbonGroup>(tasksRibbonGroup);
            WorkItem.RootWorkItem.UIExtensionSites.RegisterSite(RibbonNames.tasksRibbonUISite, tasksRibbonGroup.Tools);

            // Add the "Tasks" buttton
            ButtonTool tasksFileTool = new ButtonTool("Tasks");
            tasksFileTool.SharedProps.Caption = "Tasks";
            tasksFileTool.SharedProps.ToolTipText = "Manage Tasks";
            tasksFileTool.SharedProps.AppearancesLarge.Appearance.Image = Properties.Resources.clock_32;
            WorkItem.UIExtensionSites[RibbonNames.tasksRibbonUISite].Add<ButtonTool>(tasksFileTool);
            tasksFileTool.ToolClick += new ToolClickEventHandler(tasksTool_ToolClick);
			
        }

        void configureTool_ToolClick(object sender, ToolClickEventArgs e)
        {
			LaytonWorkItem workItem = WorkItem as LaytonWorkItem;
			((OverviewWorkItemController)workItem.Controller).ConfigureDashboard();
		}

		void wizardTool_ToolClick(object sender, ToolClickEventArgs e)
		{
			LaytonWorkItem workItem = WorkItem as LaytonWorkItem;
			((OverviewWorkItemController)workItem.Controller).RunStartupWizard();
		}

		void alertsTool_ToolClick(object sender, ToolClickEventArgs e)
		{
			LaytonWorkItem workItem = WorkItem as LaytonWorkItem;
			((OverviewWorkItemController)workItem.Controller).AlertLog();
		}

        void tasksTool_ToolClick(object sender, ToolClickEventArgs e)
        {
            LaytonWorkItem workItem = WorkItem as LaytonWorkItem;
            ((OverviewWorkItemController)workItem.Controller).AddTasks();
        }
	}
}
