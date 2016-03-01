using System;
using System.Collections.Generic;
using System.Collections;
using System.Text;
using System.Windows.Forms;
using Microsoft.Practices.CompositeUI;
using Infragistics.Practices.CompositeUI.WinForms;
using Infragistics.Win.UltraWinToolbars;
using System.Drawing;
using Layton.Cab.Interface;

namespace AuditWizardv8
{
    internal class LaytonToolbarsController : Controller
    {
        UltraToolbarsManagerWorkspace toolbarsWorkspace;
        UltraTabWorkspace tabWorkspace;
        UltraExplorerBarWorkspace explorerWorkspace;

        [Microsoft.Practices.ObjectBuilder.InjectionConstructor]
        public LaytonToolbarsController([ServiceDependency] WorkItem workItem)
        {
            this.WorkItem = workItem;
            this.toolbarsWorkspace = workItem.Workspaces[WorkspaceNames.ToolbarsWorkspace] as UltraToolbarsManagerWorkspace;
            this.tabWorkspace = workItem.Workspaces[WorkspaceNames.TabWorkspace] as UltraTabWorkspace;
            this.explorerWorkspace = workItem.Workspaces[WorkspaceNames.ExplorerWorkspace] as UltraExplorerBarWorkspace;

            // listen for the tool click event from the toolbarworkspace
            this.toolbarsWorkspace.ToolClick += new Infragistics.Win.UltraWinToolbars.ToolClickEventHandler(toolbarsWorkspace_ToolClick);
            this.toolbarsWorkspace.AfterRibbonTabSelected += new RibbonTabEventHandler(toolbarsWorkspace_AfterRibbonTabSelected);
        }

        void toolbarsWorkspace_AfterRibbonTabSelected(object sender, RibbonTabEventArgs e)
        {
            Application.UseWaitCursor = true;            

            // Loop through the RootWorkItem's ILaytonWorkItems to find the WorkItem of this Tab
            ICollection<LaytonWorkItem> workItems = WorkItem.RootWorkItem.WorkItems.FindByType<LaytonWorkItem>();
            foreach (LaytonWorkItem workItem in workItems)
            {
                if (workItem.ToolbarsController.RibbonTab != null &&
                    workItem.ToolbarsController.RibbonTab == e.Tab &&
                    WorkItem.RootWorkItem.Status != WorkItemStatus.Terminated &&
                    workItem.Status != WorkItemStatus.Terminated)
                {
                    // found the LaytonWorkItem for this Tab...now Activate it
                    workItem.Controller.ActivateWorkItem();
                }
            }

            Application.UseWaitCursor = false;
        }

        private void toolbarsWorkspace_ToolClick(object sender, Infragistics.Win.UltraWinToolbars.ToolClickEventArgs e)
        {
            switch (e.Tool.Key)
            {
                case "Refresh":
                    ILaytonView explorerView = (ILaytonView)explorerWorkspace.ActiveSmartPart;
                    explorerView.RefreshView();
                    ILaytonView tabView = (ILaytonView)tabWorkspace.ActiveSmartPart;
                    tabView.RefreshView();
                    break;
                case "Settings":
                    this.WorkItem.RootWorkItem.WorkItems[WorkItemNames.SettingsWorkItem].Activate();
                    break;
                case "Help":
                    try
                    {
                        //System.Diagnostics.Process.Start(Properties.Settings.Default.appHelpFile);
                        FormUserGuide form = new FormUserGuide();
                        form.ShowDialog();
                    }
                    catch
                    {
                        MessageBox.Show("Could not load help file:  " + Properties.Settings.Default.appHelpFile + System.Environment.NewLine + "Be sure the help file is located in the root of the application folder.", "Error Loading Help File", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    break;
                case "About":
                    LaytonAboutForm aboutForm = new LaytonAboutForm((LaytonCabShellWorkItem)WorkItem.RootWorkItem);
                    aboutForm.ShowDialog();

                    ILaytonView overviewExplorerView = (ILaytonView)explorerWorkspace.ActiveSmartPart;

                    if (overviewExplorerView is Layton.AuditWizard.Overview.OverviewExplorerView)
                        overviewExplorerView.RefreshView();

                    break;
                case "Visit Website":
                    try
                    {
                        System.Diagnostics.Process.Start("http://www.laytontechnology.com");
                    }
                    catch
                    {
                        MessageBox.Show("Unable to launch the default web browser.", "Error Opening Website", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    break;
                case "Exit":
                    Application.Exit();
                    break;
                case "Expand Main View":
                    LaytonCabShellWorkItem rootWorkItem = WorkItem.RootWorkItem as LaytonCabShellWorkItem;
                    if (rootWorkItem.Shell.ExplorerWorkspaceCollapsed)
                    {
                        rootWorkItem.Shell.ExplorerWorkspaceCollapsed = false;
                        e.Tool.InstanceProps.AppearancesSmall.Appearance.Image = Properties.Resources.expand_view_16;
                        e.Tool.SharedProps.Caption = e.Tool.Key;
                    }
                    else
                    {
                        rootWorkItem.Shell.ExplorerWorkspaceCollapsed = true;
                        e.Tool.InstanceProps.AppearancesSmall.Appearance.Image = Properties.Resources.collapse_view_16;
                        e.Tool.SharedProps.Caption = "Default Layout";                        
                    }
                    break;
                default:
                    break;
            }
        }
    }
}
