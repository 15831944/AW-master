using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using Microsoft.Practices.CompositeUI.SmartParts;
using Microsoft.Practices.CompositeUI;
using Microsoft.Practices.ObjectBuilder;
using Infragistics.Practices.CompositeUI.WinForms;
using Infragistics.Win.UltraWinExplorerBar;
using Infragistics.Win.UltraWinTabControl;
//
using Layton.Cab.Interface;
using Layton.AuditWizard.Common;

namespace Layton.AuditWizard.Administration
{
    [SmartPart]
    public partial class AdministrationExplorerView : UserControl, ILaytonView
    {
        private LaytonWorkItem workItem;
		
		//variable Declarations
		private Infragistics.Win.Appearance selectedItemAppearance;
        private string activeGroupName = "";

        [InjectionConstructor]
        public AdministrationExplorerView([ServiceDependency] WorkItem workItem)
        {
            this.workItem = workItem as LaytonWorkItem;
            InitializeComponent();

			// Set up a paint handler to display the ghosted image
			this.Paint += new PaintEventHandler(administrationExplorerBar_Paint);

			// Create an appearance so that we can show which is the selected item
			this.selectedItemAppearance = new Infragistics.Win.Appearance();
			this.selectedItemAppearance.BackColor = SystemColors.MenuHighlight;
			this.selectedItemAppearance.ForeColor = SystemColors.HighlightText;

			// Ensure that initially the 'General' group is the only one displayed
			UltraExplorerBarGroup generalGroup = this.administrationExplorerBar.Groups[GeneralOptionNames.generalGroup];
			DisplayGroup(GeneralOptionNames.generalGroup);
        }

        public void RefreshView()
        {
            base.Refresh();
        }

        public LaytonWorkItem WorkItem
        {
            get { return workItem; }
		}

		void administrationExplorerBar_Paint(object sender, PaintEventArgs e)
		{
			//Image bgImage = Properties.Resources.application_settings_ghosted_96;
			//e.Graphics.DrawImage(bgImage, 10, 150);
		}


		/// <summary>
		/// Called as we change the Active Item in the explorer bar - we need to ensure that we display the 
		/// corresponding tab view for the new active item.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void administrationExplorerBar_ActiveItemChanged(object sender, ItemEventArgs e)
		{
			ILaytonView tabView;

			// Select the appropriate Tab View based on what has been clicked
			switch (e.Item.Text)
			{
				// General Options
				case GeneralOptionNames.general_email:
					tabView = WorkItem.Items[Properties.Settings.Default.EmailSettingsTabView] as ILaytonView;
					SwitchActiveTabView(tabView);
					break;

				case GeneralOptionNames.general_security:
					tabView = WorkItem.Items[Properties.Settings.Default.SecurityTabView] as ILaytonView;
					SwitchActiveTabView(tabView);
					break;

				case GeneralOptionNames.general_service:
					tabView = WorkItem.Items[Properties.Settings.Default.ServiceTabView] as ILaytonView;
					SwitchActiveTabView(tabView);
					break;

				case GeneralOptionNames.general_database:
					tabView = WorkItem.Items[Properties.Settings.Default.DatabaseTabView] as ILaytonView;
					SwitchActiveTabView(tabView);
					break;

				// Auditing Tab Options
				case AuditingOptionNames.auditing_upload:
					tabView = WorkItem.Items[Properties.Settings.Default.UploadSettingsTabView] as ILaytonView;
					SwitchActiveTabView(tabView);
					break;

				case AuditingOptionNames.auditing_agent:
					tabView = WorkItem.Items[Properties.Settings.Default.AuditAgentTabView] as ILaytonView;
					SwitchActiveTabView(tabView);
					break;

				case AuditingOptionNames.auditing_configuration:
					tabView = WorkItem.Items[Properties.Settings.Default.ScannerConfigurationTabView] as ILaytonView;
					SwitchActiveTabView(tabView);
					break;

				// Data Setup Tab Options
				case DataOptionNames.data_locations:
					tabView = WorkItem.Items[Properties.Settings.Default.LocationsTabView] as ILaytonView;
					SwitchActiveTabView(tabView);
					break;

				case DataOptionNames.data_licensetypes:
					tabView = WorkItem.Items[Properties.Settings.Default.LicenseTypesTabView] as ILaytonView;
					SwitchActiveTabView(tabView);
					break;

				case DataOptionNames.data_serialnumbers:
					tabView = WorkItem.Items[Properties.Settings.Default.SerialNumbersTabView] as ILaytonView;
					SwitchActiveTabView(tabView);
					break;

				case DataOptionNames.data_assettypes:
					tabView = WorkItem.Items[Properties.Settings.Default.AssetTypesTabView] as ILaytonView;
					SwitchActiveTabView(tabView);
					break;

				case DataOptionNames.data_userdata:
					tabView = WorkItem.Items[Properties.Settings.Default.UserDataTabView] as ILaytonView;
					SwitchActiveTabView(tabView);
					break;

				case DataOptionNames.data_picklists:
					tabView = WorkItem.Items[Properties.Settings.Default.PickListTabView] as ILaytonView;
					SwitchActiveTabView(tabView);
					break;

				case DataOptionNames.data_aliases:
					tabView = WorkItem.Items[Properties.Settings.Default.AliasingTabView] as ILaytonView;
					SwitchActiveTabView(tabView);
					break;

				case ApplicationOptionNames.application_supplier:
					tabView = WorkItem.Items[Properties.Settings.Default.SuppliersTabView] as ILaytonView;
					SwitchActiveTabView(tabView);
					break;

				// Tools Tab
				case ToolsOptionNames.tools_settings:
					tabView = WorkItem.Items[Properties.Settings.Default.ToolsTabView] as ILaytonView;
					SwitchActiveTabView(tabView);
					break;

				// AlertMonitor Tab
				case AlertMonitorOptionNames.alertMonitor_settings:
					tabView = WorkItem.Items[Properties.Settings.Default.AlertMonitorSettingsTabView] as ILaytonView;
					SwitchActiveTabView(tabView);
					break;
			}
		}

		#region Display Selection Functions

		/// <summary>
		/// Called when we have elected to display the 'General Options' Group
		/// We add the necessary items to the explorer bar removing any existing items
		/// </summary>
		public void DisplayGeneralOptions ()
		{
			DisplayGroup(GeneralOptionNames.generalGroup);
		}


		/// <summary>
		/// Called when we have elected to display the 'Location Options' Group
		/// We add the necessary items to the explorer bar removing any existing items
		/// </summary>
		public void DisplayLocationOptions()
		{
		}


		/// <summary>
		/// Called when we have elected to display the 'Auditing Options' Group
		/// We add the necessary items to the explorer bar removing any existing items
		/// </summary>
		public void DisplayAuditingOptions()
		{
			DisplayGroup(AuditingOptionNames.auditingGroup);
		}


		/// <summary>
		/// Called when we have elected to display the 'Data Setup' Group
		/// We add the necessary items to the explorer bar removing any existing items
		/// </summary>
		public void DisplayDataSetupOptions()
		{
			DisplayGroup(DataOptionNames.dataGroup);
		}


		/// <summary>
		/// Called when we have elected to display the 'Application Options' Group
		/// We add the necessary items to the explorer bar removing any existing items
		/// </summary>
		public void DisplayApplicationOptions()
		{
			DisplayGroup(ApplicationOptionNames.applicationGroup);
		}


		/// <summary>
		/// Called when we have elected to display the 'Alert Monitor Options' Group
		/// We add the necessary items to the explorer bar removing any existing items
		/// </summary>
		public void DisplayAlertMonitorOptions()
		{
			DisplayGroup(AlertMonitorOptionNames.alertMonitorGroup);
		}


		/// <summary>
		/// Called when we have elected to display the 'Tools Options' Group
		/// We add the necessary items to the explorer bar removing any existing items
		/// </summary>
		public void DisplayToolsOptions()
		{
			DisplayGroup(ToolsOptionNames.toolsGroup);
		}


		/// <summary>
		/// Display the specified explorer bar group and hide all others - this is generally called as a result
		/// of clicking on the ribbon which is how we select which administration group to display.  If we do not
		/// currently have an active item in the group then we will select the first
		/// </summary>
		/// <param name="groupName"></param>
		protected void DisplayGroup(string groupName)
		{
			// Save the name of the current active group just in case we need to reset
			if (administrationExplorerBar.ActiveGroup != null)
				activeGroupName = administrationExplorerBar.ActiveGroup.Key;

			// Ensure that only the specified group is visible
			foreach (UltraExplorerBarGroup group in administrationExplorerBar.Groups)
			{
				group.Visible = (group.Key == groupName);
			}

			// Find the group with the specified name and make it active
			UltraExplorerBarGroup requiredGroup = administrationExplorerBar.Groups[groupName];
			if (requiredGroup != null) 
			{
				// ...and select this group
				requiredGroup.Selected = true;

				// ...and make it active
				this.administrationExplorerBar.ActiveGroup = requiredGroup;
			}
		}

#endregion


		/// <summary>
		/// Called as we switch the tab view to one associated with the currently selected item in the
		/// explorer view noting that we must force the current active tab view to save its data before
		/// we switch to ensure that we are always up to date
		/// 
		/// This is typically required when we switch between items in the currently selected group but can
		/// also be indirectly invoked as we switch groups as we may select an item as part of this process
		/// </summary>
		/// <param name="newTabView"></param>
		public void SwitchActiveTabView(ILaytonView newTabView)
		{
			// Get the current tab view as we need to force it to save it's data before we move
			// off to another tab
            IAdministrationView activeAdministrationTabView = (IAdministrationView)workItem.RootWorkItem.Workspaces[WorkspaceNames.TabWorkspace].ActiveSmartPart;
            //activeAdministrationTabView.Save();
			
			// All done if no new tab
			if (newTabView == null)
				return;

			// tell the work item what the new tab view will be
			workItem.Controller.SetTabView(newTabView);

			// Request the new view to activate itself (this initializes the data displayed)
			activeAdministrationTabView = (IAdministrationView)workItem.RootWorkItem.Workspaces[WorkspaceNames.TabWorkspace].ActiveSmartPart;
			activeAdministrationTabView.Activate();
			((AdministrationWorkItem)workItem).ActiveTabView = activeAdministrationTabView;

			// Ask the toolbars controller to display the correct ribbons for the displayed tab
			AdministrationToolbarsController toolbarsController = workItem.ToolbarsController as AdministrationToolbarsController;
			toolbarsController.ResetRibbon(newTabView);
		}



		private void administrationExplorerBar_ActiveGroupChanged(object sender, GroupEventArgs e)
		{
			e.Group.Expanded = true;

			if (e.Group.Items.Count > 0)
			{
				try
				{
					e.Group.Items[0].Active = true;
				}
				catch (Exception)
				{
					if (activeGroupName != "")
						DisplayGroup(e.Group.Key);
				}
			}
		}

		protected void SelectFirstItem(UltraExplorerBarGroup group)
		{
			// If there is an ActiveGroup set, set the ActiveItem to the first item within the ActiveGroup.
			if (this.administrationExplorerBar.ActiveGroup != null &&
				this.administrationExplorerBar.ActiveGroup.Items.Count > 0)
			{
				this.administrationExplorerBar.ActiveItem = this.administrationExplorerBar.ActiveGroup.Items[0];
				return;
			}

			// If there is an ActiveItem set, set the ActiveGroup to the group containing the item.
			if (this.administrationExplorerBar.ActiveItem != null)
			{
				this.administrationExplorerBar.ActiveGroup = this.administrationExplorerBar.ActiveItem.Group;
				return;
			}
		}
	}
}
