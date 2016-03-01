using System;
using Microsoft.Practices.CompositeUI;
using Microsoft.Practices.CompositeUI.SmartParts;
using Infragistics.Win.UltraWinToolbars;
using Infragistics.Practices.CompositeUI.WinForms;

namespace Layton.Cab.Interface
{
    public abstract class LaytonWorkItemController : Controller
    {
        private WorkItem workItem;
        private ILaytonView currentExplorerView;
        private ILaytonView currentTabView;

        [Microsoft.Practices.ObjectBuilder.InjectionConstructor]
        public LaytonWorkItemController([ServiceDependency] WorkItem workItem)
        {
            this.workItem = workItem;
        }

        new public LaytonWorkItem WorkItem
        {
            get { return workItem as LaytonWorkItem; }
        }

        public virtual void Initialize()
        {
            // Create the SmartPartInfo object for the default ExplorerView to be put in the ExplorerWorkspace
            UltraExplorerBarSmartPartInfo explorerInfo = new UltraExplorerBarSmartPartInfo();
            explorerInfo.Image = WorkItem.Image;
            explorerInfo.Title = WorkItem.Title;
            explorerInfo.Description = WorkItem.Description;

            // Create the SmartPartInfo object for default TabView to be put in the TabWorkspace
            UltraTabSmartPartInfo tabInfo = new UltraTabSmartPartInfo();
            tabInfo.Image = WorkItem.Image;
            tabInfo.Title = WorkItem.Title;
            tabInfo.Description = WorkItem.Description;

            // Create the SmartPartInfo object for the default SettingsTabView
            UltraTabSmartPartInfo settingsInfo = new UltraTabSmartPartInfo();
            settingsInfo.Image = WorkItem.Image;
            settingsInfo.Title = WorkItem.Title;
            settingsInfo.ActivateTab = false;

            WorkItem.ToolbarsController.Initialize();

            if (WorkItem.ExplorerView != null)
            {
                // Show the ExplorerView
                try
                {
                    WorkItem.RootWorkItem.Workspaces[WorkspaceNames.ExplorerWorkspace].Show(WorkItem.ExplorerView, explorerInfo);
                    currentExplorerView = WorkItem.ExplorerView;
                }
                catch (Exception)
                {
                }

            }
            if (WorkItem.TabView != null)
            {
                // Show the TabView
                WorkItem.RootWorkItem.Workspaces[WorkspaceNames.TabWorkspace].Show(WorkItem.TabView, tabInfo);
                currentTabView = WorkItem.TabView;
            }
            if (WorkItem.SettingsView != null)
            {
                // Show the SettingsTabView
                WorkItem.RootWorkItem.Workspaces[WorkspaceNames.SettingsTabWorkspace].Show(WorkItem.SettingsView, settingsInfo);
            }
            if (WorkItem.ToolbarsController != null)
            {
                // Get the Toolbars Workspace and set the RibbonTab
                UltraToolbarsManagerWorkspace toolbarsWorkspace = (UltraToolbarsManagerWorkspace)WorkItem.RootWorkItem.Workspaces[WorkspaceNames.ToolbarsWorkspace];
                toolbarsWorkspace.Ribbon.SelectedTab = WorkItem.ToolbarsController.RibbonTab;
            }

            // Activate the WorkItem
            WorkItem.Activate();
        }

        public virtual void SetExplorerView(ILaytonView explorerView)
        {
            if (workItem.Status == WorkItemStatus.Inactive)
            {
                ActivateWorkItem();
            }
            WorkItem.RootWorkItem.Workspaces[WorkspaceNames.ExplorerWorkspace].Show(explorerView);
            currentExplorerView = explorerView;
        }

        public virtual void SetTabView(ILaytonView tabView)
        {
            if (workItem.Status == WorkItemStatus.Inactive)
            {
                ActivateWorkItem();
            }
            WorkItem.RootWorkItem.Workspaces[WorkspaceNames.TabWorkspace].Show(tabView);
            currentTabView = tabView;
        }

        public virtual void ActivateWorkItem()
        {
            if (WorkItem.ExplorerView != null && WorkItem.TabView != null && WorkItem.ToolbarsController != null)
            {
                // AC:  For some reason the explorerbar for the last module loaded, has issues when activating for the first time
                // it appears that the ActiveSmartPart value for the ExplorerWorkspace is always set to the last module - even though
                // a different WorkItem's Explorer Workspace view is displayed - the code below works around the issue
                if (WorkItem.RootWorkItem.Workspaces[WorkspaceNames.ExplorerWorkspace].ActiveSmartPart == WorkItem.ExplorerView)
                {
                    WorkItem.RootWorkItem.Workspaces[WorkspaceNames.ExplorerWorkspace].Hide(WorkItem.ExplorerView);
                }

                // Show the TabView and ExplorerView
                // First try setting the ExplorerWorkspace/TabWorkspace to use the last shown views for this WorkItem
                // ...otherwise set the main view to the default ExplorerView/TabView 
                if (currentExplorerView != null)
                {
                    try
                    {
                        WorkItem.RootWorkItem.Workspaces[WorkspaceNames.ExplorerWorkspace].Show(currentExplorerView);
                    }
                    catch (Exception)
                    {
                    }

                }
                else
                {
                    WorkItem.RootWorkItem.Workspaces[WorkspaceNames.ExplorerWorkspace].Show(WorkItem.ExplorerView);
                    currentExplorerView = WorkItem.ExplorerView;
                }

                // Now the TabWorkspace...
                if (currentTabView != null)
                {
                    WorkItem.RootWorkItem.Workspaces[WorkspaceNames.TabWorkspace].Show(currentTabView);
                }
                else
                {
                    WorkItem.RootWorkItem.Workspaces[WorkspaceNames.TabWorkspace].Show(WorkItem.TabView);
                    currentTabView = WorkItem.TabView;
                }

                // Get the Toolbars Workspace and set the RibbonTab
                UltraToolbarsManagerWorkspace toolbarsWorkspace = (UltraToolbarsManagerWorkspace)WorkItem.RootWorkItem.Workspaces[WorkspaceNames.ToolbarsWorkspace];
                toolbarsWorkspace.Ribbon.SelectedTab = WorkItem.ToolbarsController.RibbonTab;

                // Activate the WorkItem
                WorkItem.Activate();
            }
        }

        public virtual void ShowSettings()
        {
            if (WorkItem.SettingsView != null)
            {
                IWorkspace settingsWorkspace = WorkItem.RootWorkItem.Workspaces[WorkspaceNames.SettingsTabWorkspace];

                ((System.Windows.Forms.UserControl)(WorkItem.SettingsView)).Height = 1544;
                settingsWorkspace.Show(WorkItem.SettingsView);
                workItem.RootWorkItem.WorkItems[WorkItemNames.SettingsWorkItem].Activate();
            }
        }
    }
}
