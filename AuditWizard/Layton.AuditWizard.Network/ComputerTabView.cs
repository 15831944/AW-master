using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
//
using Microsoft.Practices.CompositeUI.SmartParts;
using Microsoft.Practices.ObjectBuilder;
using Microsoft.Practices.CompositeUI.EventBroker;
using Microsoft.Practices.CompositeUI;
using Microsoft.Practices.CompositeUI.Utility;
//
using Infragistics.Win.UltraWinListView;
using Infragistics.Win.IGControls;
//
using Layton.Cab.Interface;
using Layton.AuditWizard.Common;
using Layton.AuditWizard.DataAccess;

namespace Layton.AuditWizard.Network
{
    [SmartPart]
    public partial class ComputerTabView : UserControl, ILaytonView
    {
		ComputerTabViewPresenter presenter;
		private LaytonWorkItem workItem;
		Asset _displayedAsset = null;

        [InjectionConstructor]
        public ComputerTabView([ServiceDependency] WorkItem workItem)
        {
            this.workItem = workItem as LaytonWorkItem;
            InitializeComponent();

			// Set the List View style
			assetListView.ShowGroups = true;
			assetListView.View = Properties.Settings.Default.ViewStyle;

			// Setup the context menu used by the list view
			assetListView.ContextMenu = this.contextMenu1;

			//	Populate the context menu with the constants of the UltraListViewStyle
			//	enumeration, so we can allow the end user to change the view. Note that we
			// will not support the 'Details' view as this is not really appropriate
			IGMenuItem menuItem = null;
			string[] enumNames = Enum.GetNames(typeof(UltraListViewStyle));
			Array enumValues = Enum.GetValues(typeof(UltraListViewStyle));

			for (int i = 0; i < enumValues.Length; i++)
			{
				// Skip details mode
				if (enumNames[i] == UltraListViewStyle.Details.ToString())
					continue;

				menuItem = new IGMenuItem(enumNames[i], new EventHandler(this.contextMenuItem_Click));
				UltraListViewStyle enumValue = (UltraListViewStyle)enumValues.GetValue(i);
				menuItem.Tag = enumValue;

				//	If this value is the same one that the control's View
				//	property is set to, check it
				if (assetListView.View == enumValue)
					menuItem.Checked = true;

				this.contextMenu1.MenuItems.Add(menuItem);
			}
        }

		[CreateNew]
		public ComputerTabViewPresenter Presenter
		{
			set
			{
				presenter = value;
				presenter.View = this;
				presenter.Initialize();
			}
			get
			{
				return presenter;
			}
		}

		public void RefreshView()
		{
			presenter.Initialize();
			base.Refresh();
		}

        public LaytonWorkItem WorkItem
        {
            get { return workItem; }
        }


		/// <summary>
		/// Initialize the tab view display
		/// </summary>
		/// <param name="theAsset"></param>
		public void Display(Asset theAsset)
		{
			// Save the asset being displayed
			_displayedAsset = theAsset;

			// Clear the current list of groups
			assetListView.Groups.Clear();
			assetListView.Items.Clear();

			// Add the new group to the list view
			string groupName = "Audited Data for " + theAsset.Name;
			UltraListViewGroup group = assetListView.Groups.Add("auditeddata" ,groupName);

			// Set the group's properties
			group.Visible = true;
			group.Text = groupName;

			// Now add the items into the group which will act as the placeholders to allow the user
			// to drill down on the audited data
			UltraListViewItem item;
			try
			{
				item = assetListView.Items.Add(AWMiscStrings.SummaryNode, "Summary");
				item.Group = group;
				item.Appearance.Image = Properties.Resources.computer96;
				item.Appearance.FontData.Bold = Infragistics.Win.DefaultableBoolean.True;

                if ((theAsset.LastAudit != DateTime.MinValue) && theAsset.Auditable)
                {
                    //
                    item = assetListView.Items.Add(AWMiscStrings.ApplicationsNode, "Applications");
                    item.Group = group;
                    item.Appearance.Image = Properties.Resources.application_72;
                    item.Appearance.FontData.Bold = Infragistics.Win.DefaultableBoolean.True;
                    //
                    item = assetListView.Items.Add(AWMiscStrings.HardwareNode, "Hardware");
                    item.Group = group;
                    item.Appearance.Image = Properties.Resources.hardware;
                    item.Appearance.FontData.Bold = Infragistics.Win.DefaultableBoolean.True;
                    //
                    item = assetListView.Items.Add(AWMiscStrings.InternetNode, "Internet");
                    item.Group = group;
                    item.Appearance.Image = Properties.Resources.internet_explorer_32;
                    item.Appearance.FontData.Bold = Infragistics.Win.DefaultableBoolean.True;
                    //
                    item = assetListView.Items.Add(AWMiscStrings.FileSystem, "File System");
                    item.Group = group;
                    item.Appearance.Image = Properties.Resources.filesystem_32;
                    item.Appearance.FontData.Bold = Infragistics.Win.DefaultableBoolean.True;
                    //
                    item = assetListView.Items.Add(AWMiscStrings.HistoryNode, "History");
                    item.Group = group;
                    item.Appearance.Image = Properties.Resources.history;
                    item.Appearance.FontData.Bold = Infragistics.Win.DefaultableBoolean.True;
                    //
                    item = assetListView.Items.Add(AWMiscStrings.SystemNode, "System");
                    item.Group = group;
                    item.Appearance.Image = Properties.Resources.system_72;
                    item.Appearance.FontData.Bold = Infragistics.Win.DefaultableBoolean.True;

                    // Add on the User Defined Data Categories (if any)
                    NetworkWorkItemController wiController = WorkItem.Controller as NetworkWorkItemController;
                    UserDataCategoryList listCategories = new UserDataCategoryList(UserDataCategory.SCOPE.Asset);
                    listCategories.Populate();
                    foreach (UserDataCategory category in listCategories)
                    {
                        // Is this category restricted to a specific asset category or type?
                        if (wiController.CategoryAppliesTo(category, theAsset))
                        {
                            item = assetListView.Items.Add(AWMiscStrings.UserDataNode + "|" + category.Name, category.Name);
                            item.Group = group;
                            item.Appearance.Image = IconMapping.LoadIcon(category.Icon, IconMapping.Iconsize.Large);
                            item.Appearance.FontData.Bold = Infragistics.Win.DefaultableBoolean.True;
                        }
                    }
                }

			}
			catch (Exception ex)
			{
				MessageBox.Show("Failed to add list view item, reason: " + ex.Message);
			}

			// Set the resize for the columns
			//this.assetListView.MainColumn.PerformAutoResize(ColumnAutoSizeMode.Header | ColumnAutoSizeMode.AllItems);
		}

		/// <summary>
		/// Called as we click on one of the items in the list view - determine what the item
		/// is and then invoke the appropriate tab by fitring an event
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void computerListView_ItemDoubleClick(object sender, ItemDoubleClickEventArgs e)
		{
			NetworkExplorerView explorerView = workItem.ExplorerView as NetworkExplorerView;
			UltraListViewItem clickedItem = e.Item;

			// Request the main explorer view (LH Pane) to display the required view
			explorerView.SelectViewToDisplay(clickedItem.Key, _displayedAsset);
		}

		#region Context Menu Handler

		/// <summary>
		/// This is the handler for when we change an item on the context menu
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void contextMenuItem_Click(object sender, EventArgs e)
		{
			// Uncheck any currently checked menu item
			foreach (IGMenuItem thisMenuItem in contextMenu1.MenuItems)
			{
				if (thisMenuItem.Checked)
					thisMenuItem.Checked = false;
			}

			IGMenuItem menuItem = sender as IGMenuItem;
			if (menuItem != null)
			{
				if (menuItem.Tag is UltraListViewStyle)
				{
					menuItem.Checked = true;
					UltraListViewStyle view = (UltraListViewStyle)menuItem.Tag;
					this.assetListView.View = view;
					Properties.Settings.Default.ViewStyle = view;
					Properties.Settings.Default.Save();
					RefreshView();
				}
			}
		}

		#endregion 
	}
}
