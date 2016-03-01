using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Windows.Forms;
//
using Layton.Cab.Interface;
using Layton.AuditWizard.Common;
using Layton.Common.Controls;
//
using Microsoft.Practices.CompositeUI.SmartParts;
using Microsoft.Practices.CompositeUI.EventBroker;
using Microsoft.Practices.ObjectBuilder;
using Microsoft.Practices.CompositeUI;
//
using Infragistics.Win.UltraWinToolbars;

namespace Layton.AuditWizard.AuditTrail
{
    public class AuditTrailWorkItemController : LaytonWorkItemController
    {
		private AuditTrailWorkItem workItem;

		public AuditTrailWorkItemController(WorkItem workItem) : base(workItem) 
		{
			this.workItem = workItem as AuditTrailWorkItem;

			// Subscribe to the closing event for all smart parts
//			WorkItem.RootWorkItem.Workspaces[WorkspaceNames.TabWorkspace].SmartPartClosing +=
//				new EventHandler<WorkspaceCancelEventArgs>(AuditTrail_SmartPartClosing);
		}

		public override void ActivateWorkItem()
		{
			base.ActivateWorkItem();
            //WorkItem.ExplorerView.RefreshView();
            //WorkItem.TabView.RefreshView();

			// We cannot refresh the filters as part of the standard refresh as this causes a loop - refreshing the
			// filters changes the selection which causes a refresh and so on.  So we do it now
			(workItem.ToolbarsController as AuditTrailToolbarsController).RefreshFilters();
		}


		/// <summary>
		/// Called as a smart part is closing.  We need to save the layout if we are closing the tab view
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		protected void AuditTrail_SmartPartClosing(object sender, WorkspaceCancelEventArgs e)
		{
			// Ok which smart part is closing - if a tab view call it's save layout function
			if (e.SmartPart == WorkItem.Items[Layton.Cab.Interface.ViewNames.MainTabView])
			{
				AuditTrailTabView tabView = e.SmartPart as AuditTrailTabView;
				tabView.SaveLayout();
			}
		}


		//protected override void OnTerminating()
		//{
		//    // We need to now save the layouts for all tab views which may have been displayed
		//    ApplicationsTabView applicationsView = this.Items[Properties.Settings.Default.MainTabView] as ApplicationsTabView;
		//    if (applicationsView != null)
		//        applicationsView.SaveLayout();
		//    //
		//    InstancesTabView instancesTabView = this.Items[Properties.Settings.Default.InstancesTabView] as InstancesTabView;
		//    if (instancesTabView != null)
		//        instancesTabView.SaveLayout();
		//    //
		//    LicensesTabView licensesView = this.Items[Properties.Settings.Default.LicensesTabView] as LicensesTabView;
		//    licensesView.SaveLayout();
		//    //
		//    OSTabView osView = this.Items[Properties.Settings.Default.OSTabView] as OSTabView;
		//    osView.SaveLayout();

		//    // Call the base class implementation
		//    base.OnTerminating();
		//}



		#region Export Menu Functions

		/// <summary>
		/// Export to PDF format 
		/// We call the appropriate grid view to handle this request themselves
		/// </summary>
		public void ExportToPDF()
		{
			ILaytonView tabView = (ILaytonView)WorkItem.RootWorkItem.Workspaces[WorkspaceNames.TabWorkspace].ActiveSmartPart;
			if (tabView is AuditTrailTabView)
				((AuditTrailTabView)tabView).ExportToPDF();
		}


		/// <summary>
		/// Export to XLS format 
		/// We call the appropriate grid view to handle this request themselves
		/// </summary>
		public void ExportToXLS()
		{
			ILaytonView tabView = (ILaytonView)WorkItem.RootWorkItem.Workspaces[WorkspaceNames.TabWorkspace].ActiveSmartPart;
			if (tabView is AuditTrailTabView)
				((AuditTrailTabView)tabView).ExportToXLS();
		}


		/// <summary>
		/// Export to XPS format 
		/// We call the appropriate grid view to handle this request themselves
		/// </summary>
		public void ExportToXPS()
		{
			ILaytonView tabView = (ILaytonView)WorkItem.RootWorkItem.Workspaces[WorkspaceNames.TabWorkspace].ActiveSmartPart;
			if (tabView is AuditTrailTabView)
				((AuditTrailTabView)tabView).ExportToXPS();
		}

		#endregion Export Menu Functions


    }
}
