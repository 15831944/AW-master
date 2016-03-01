using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using Microsoft.Practices.CompositeUI.SmartParts;
using Layton.Cab.Interface;
using Microsoft.Practices.CompositeUI;
using Microsoft.Practices.CompositeUI.Utility;
using Microsoft.Practices.ObjectBuilder;
//
using Infragistics.Win;
using Infragistics.Win.Misc;
using Infragistics.Win.UltraWinExplorerBar;
using Infragistics.Win.UltraWinListView;
using Infragistics.Win.UltraWinTree;
using Infragistics.Win.UltraWinToolbars;
//
using Layton.AuditWizard.Common;
using Layton.Common.Controls;
using Layton.AuditWizard.DataAccess;

namespace Layton.AuditWizard.Administration
{
    [SmartPart]
    public partial class LocationsTabView : UserControl, ILaytonView, IAdministrationView
    {
		#region Local Data

        protected static readonly log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private static readonly bool isDebugEnabled = logger.IsDebugEnabled;

		private LaytonWorkItem workItem;
		LocationsTabViewPresenter presenter;
		private bool eatNextKeyPress = false;

		#endregion Local Data

		#region Constructor

		[InjectionConstructor]
		public LocationsTabView([ServiceDependency] WorkItem workItem)
        {
            this.workItem = workItem as LaytonWorkItem;
            InitializeComponent();

			// Initialize the list and tree views
			//	Set the DefaultImage to a folder icon, since that is the most common
			//	type of item we will be displaying
			this.locationsList.ItemSettings.DefaultImage = Properties.Resources.location_16;

			// Populate the UltraListView's SubItemColumns collection with the columns that will represent the 
			// additional information associated with the locations
			this.locationsList.SubItemColumns.Clear();
			UltraListViewSubItemColumn column = null;

			//	Set the text for the MainColumn 
			this.locationsList.MainColumn.Text = "Name";

			//	Add a column for the Starting IP address
			column = this.locationsList.SubItemColumns.Add("Starting IP Address");
			column.DataType = typeof(string);

			//	Add a column for the Ending IP Address.
			column = this.locationsList.SubItemColumns.Add("Ending IP Address");
			column.DataType = typeof(string);

			//  Set the UltraWinListView.ColumnAutoSizeMode property so that when the end user double-clicks
			//  on the right edge of a column header, the column's width is adjusted to fully
			//  display the text for all visible items and the header.
			this.locationsList.ViewSettingsDetails.ColumnAutoSizeMode = Infragistics.Win.UltraWinListView.ColumnAutoSizeMode.VisibleItemsAndHeader;

			//  Set UltraWinListView.AutoFitColumns to 'ResizeAllColumns' so that all columns fit in the available
			//  horizontal space.
			this.locationsList.ViewSettingsDetails.AutoFitColumns = Infragistics.Win.UltraWinListView.AutoFitColumns.ResizeAllColumns;

			// Tree View
			// =========
			//
			// Set the tree view to be dynamically expanded
			this.locationsTree.Override.ShowExpansionIndicator = ShowExpansionIndicator.CheckOnExpand;

			// Set the display style foe the tree so that we can swap between vista and 'standard' trees
			AuditWizardConfiguration configuration = new AuditWizardConfiguration();
			this.locationsTree.DisplayStyle = (configuration.VistaTrees) ? UltraTreeDisplayStyle.WindowsVista : UltraTreeDisplayStyle.Standard;

			// Add the root node to the tree - we expand the tree as and when expanded
			PopulateRoot();
		}
		#endregion Constructor

		[CreateNew]
		public LocationsTabViewPresenter Presenter
		{
			set { presenter = value; presenter.View = this; presenter.Initialize(); }
			get { return presenter; }
		}

        public void RefreshViewSinglePublisher()
        {
        }

		/// <summary>
		/// Refresh the current view
		/// </summary>
		public void RefreshView()
		{
			// First save any existing selections
			SelectedNodesCollection selectedNodes = locationsTree.SelectedNodes;

			// ...then save any 'expanded' nodes
			List<UltraTreeNode> expandedNodes = new List<UltraTreeNode>();
			GetExpandedNodes(expandedNodes, locationsTree.Nodes);

			// Initialize the view
			presenter.Initialize();

			// Call our base class refresh code next
			base.Refresh();

			// ...then reinstate any selections
			RestoreSelectedNodes(selectedNodes);

			// ...then re-instate any expansions
			RestoreExpandedNodes(expandedNodes);
		}


		/// <summary>
		/// Called as this tab is activated to ensure that we display the latest possible data
		/// This function comes from the IAdministrationView Interface
		/// </summary>
		public void Activate()
		{
			InitializeData();
            this.locationsTree.Nodes.Clear();
            PopulateRoot();
		}


		/// <summary>
		/// save function for the IAdministrationView Interface
		/// </summary>
		public void Save()
		{
		}


        public LaytonWorkItem WorkItem
        {
            get { return workItem; }
        }


		/// <summary>
		/// Load data into the controls
		/// </summary>
		private void InitializeData()
		{
			RefreshTab();
		}



		/// <summary>
		/// Called to refresh the information displayed on the users tab
		/// </summary>
		protected void RefreshTab()
		{
			// First clear the entries from 
			this.locationsList.Items.Clear();
		}


		/// <summary>
		/// Add the root item(s) to the locations tree
		/// </summary>
		private void PopulateRoot()
		{
			// Get the root node (they should only be one and one only)
			LocationsDAO lwDataAccess = new LocationsDAO();
			DataTable table = lwDataAccess.GetGroups(new AssetGroup(AssetGroup.GROUPTYPE.userlocation));
			if (table.Rows.Count == 0)
				return;
			AssetGroup rootLocation = new AssetGroup(table.Rows[0] ,AssetGroup.GROUPTYPE.userlocation);
			UltraTreeNode rootNode = this.locationsTree.Nodes.Add("home", rootLocation.Name);
			rootNode.Override.NodeAppearance.Image = Properties.Resources.location_16;
			rootNode.Override.ExpandedNodeAppearance.Image = Properties.Resources.location_16;
			rootNode.Tag = rootLocation;

			// ...and force an automatic expansion of the root node as we want to display the top level nodes 
			// on entry to this pane
			rootNode.Expanded = true;

			// Ensure that the root node is selected
			rootNode.Selected = true;
		}

		#region Context Menu Handling

		#region tree View Context menu

		/// <summary>
		/// Called as the Locations menu is opening to enable/disable options as appropriate
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void cmsTvLocations_Opening(object sender, CancelEventArgs e)
		{
			// Can only delete if we have at least one node selected
			miDeleteLocation.Enabled = (locationsTree.SelectedNodes.Count > 0);

			// Can only get properties for a single item at once
			miLocationProperties.Enabled = (locationsTree.SelectedNodes.Count == 1);
		}



		/// <summary>
		/// Called to create a new Location
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void miNewLocation_Click(object sender, EventArgs e)
		{
			AddLocation();
		}

		
		/// <summary>
		/// Called as we select to delete a location - note that we cannot delete a location
		/// without deleting all sub-loctaions and also cannot delete while there are references to it from
		/// any asset
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void miDeleteLocation_Click(object sender, EventArgs e)
		{
			DeleteTreeLocation();
		}



		/// <summary>
		/// Display the properties of the currently selected Location when invoked from the Tree
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void miLocationProperties_Click(object sender, EventArgs e)
		{
			EditTreeLocation();
		}


		#endregion

		#region ListView context menu handlers

		/// <summary>
		/// Called as the ListView Locations menu is opening to enable/disable options as appropriate
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void cmsLvLocations_Opening(object sender, CancelEventArgs e)
		{
			// Can only delete if we have at least one list item selected
			miDeleteLocation.Enabled = (locationsList.SelectedItems.Count > 0);

			// Can only get properties for a single item at once
			miLvProperties.Enabled = (locationsList.SelectedItems.Count == 1);
		}


		/// <summary>
		/// Called to Add a new location as a child of the currently selected location.  This is called via
		/// the right click menu from the List View
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void miLvNewItem_Click(object sender, EventArgs e)
		{
			AddLocation();
		}


		/// <summary>
		/// Display the properties of the currently selected item in the list and potentially
		/// allow these properties to be edited.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void miLvProperties_Click(object sender, EventArgs e)
		{
			EditListLocation();
		}


		/// <summary>
		/// Context menu handler from the ListView menu to delete a location
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void miLvDeleteItem_Click(object sender, EventArgs e)
		{
			DeleteListLocation();
		}


		#endregion

		#endregion Context Menu Handling

		#region TreeView State Functions

		/// <summary>
		/// Iterative function to check for all nodes in the tree which have been expanded
		/// </summary>
		/// <param name="expandedNodes"></param>
		/// <param name="childNodes"></param>
		/// <returns></returns>
		protected void GetExpandedNodes(List<UltraTreeNode> expandedNodes, TreeNodesCollection childNodes)
		{
			// Starting from the root item traverse down looking for nodes which are expanded
			foreach (UltraTreeNode thisNode in childNodes)
			{
				if (thisNode.Expanded)
				{
					expandedNodes.Add(thisNode);
					GetExpandedNodes(expandedNodes, thisNode.Nodes);
				}
			}
		}


		/// <summary>
		/// Restore the selection state of any nodes contained within the supplied list
		/// </summary>
		/// <param name="selectedNodes"></param>
		protected void RestoreSelectedNodes(SelectedNodesCollection selectedNodes)
		{
			foreach (UltraTreeNode node in selectedNodes)
			{
				UltraTreeNode selectedNode = locationsTree.GetNodeByKey(node.Key);
				if (selectedNode != null)
				{
					selectedNode.BringIntoView();
					if (selectedNode.Selected == false)
						selectedNode.Selected = true;
				}
			}
		}


		/// <summary>
		/// Restore the expanded state of the supplied nodes
		/// </summary>
		/// <param name="expandedNodes"></param>
		protected void RestoreExpandedNodes(List<UltraTreeNode> expandedNodes)
		{
			foreach (UltraTreeNode node in expandedNodes)
			{
				UltraTreeNode expandedNode = locationsTree.GetNodeByKey(node.Key);
				if (expandedNode != null)
				{
					if (expandedNode.Expanded == false)
						expandedNode.Expanded = true;
				}
			}
		}

		#endregion TreeView State Functions

		#region TreeView Event Handlers

		private void locationsTree_BeforeExpand(object sender, CancelableNodeEventArgs e)
		{
			PopulateChildLocations(e.TreeNode);
		}


		/// <summary>
		/// Handles the UltraTree's AfterSelect event
		/// </summary>
		private void locationsTree_AfterSelect(object sender, SelectEventArgs e)
		{
			UltraTreeNode selectedNode = e.NewSelections.Count == 1 ? e.NewSelections[0] : null;
			if (selectedNode == null)
				return;
			PopulateListView(selectedNode);
		}


		/// <summary>
		/// Double-clicking on a node in the tree will edit that node
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void locationsTree_DoubleClick(object sender, EventArgs e)
		{
			if (locationsTree.SelectedNodes.Count == 0)
				return;

			// Get the location being edited and its parent location (if any)
			UltraTreeNode selectedNode = locationsTree.SelectedNodes[0];
			AssetGroup location = selectedNode.Tag as AssetGroup;
			AssetGroup parentLocation = null;

			if (selectedNode.Parent != null)
				parentLocation = selectedNode.Parent.Tag as AssetGroup;

			// Display the properties of this location
			if (EditLocation(parentLocation, location))
				selectedNode.Text = location.Name;
		}



		/// <summary>
		/// Handles the UltraTree's MouseUp event
		/// </summary>
		private void locationsTree_MouseUp(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			UltraTree ultraTree = sender as UltraTree;
			Point pointClicked = new Point(e.X, e.Y);
			UltraTreeNode nodeAtPoint = ultraTree.GetNodeFromPoint(pointClicked);
			if (nodeAtPoint != null && nodeAtPoint.Expanded == false)
			{
				Infragistics.Win.UltraWinTree.ExpansionIndicatorUIElement expansionIndicatorElement = null;
				expansionIndicatorElement = ultraTree.UIElement.ElementFromPoint(pointClicked) as Infragistics.Win.UltraWinTree.ExpansionIndicatorUIElement;
				if (expansionIndicatorElement == null)
					nodeAtPoint.Expanded = true;
			}
		}


		/// <summary>
		/// Mouse button down on a node will select that node
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void locationsTree_MouseDown(object sender, MouseEventArgs e)
		{
			if (e.Button == MouseButtons.Right)
			{
				//Get the position of the mouse
				Point currentPoint = new Point(e.X, e.Y);

				//Get the node the mouse is over.
				UltraTreeNode nodeAtPoint = locationsTree.GetNodeFromPoint(currentPoint);
				if (nodeAtPoint != null)
				{
					locationsTree.SelectedNodes.Clear();
					nodeAtPoint.Selected = true;
				}
			}
		}

		#endregion TreeView Event Handlers

		#region PopulateChildLocations

		/// <summary>
		/// Populates the child folder of a tree node
		/// </summary>
		private void PopulateChildLocations(UltraTreeNode node)
		{
			//	If we have already populated this node's Nodes collection, return
			if (node.HasNodes == true)
				return;

			// Get the locations beneath that specified
			AssetGroup parentGroup = node.Tag as AssetGroup;

			// So request the database for a list of locations beneath that specified
			LocationsDAO lwDataAccess = new LocationsDAO();
			DataTable table = lwDataAccess.GetGroups(parentGroup);

			// ...and add these locations to the tree as children of the selected node
			foreach (DataRow row in table.Rows)
			{
				AssetGroup newLocation = new AssetGroup(row ,AssetGroup.GROUPTYPE.userlocation);
				UltraTreeNode childNode = node.Nodes.Add(newLocation.FullName, newLocation.Name);
				childNode.Tag = newLocation;
			}
		}
		#endregion PopulateChildFolders

		#region ListView Event Handlers

		/// <summary>
		/// Called as we click a mouse button within the list view.  If this is the right button then we should 
		/// select (any) item which the mouse is over
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void ulvUserData_MouseDown(object sender, MouseEventArgs e)
		{
			if (e.Button == MouseButtons.Right)
			{
				//Get the position of the mouse
				Point currentPoint = new Point(e.X, e.Y);

				//Get the node the mouse is over.
				UltraListViewItem item = locationsList.ItemFromPoint(currentPoint);

				if (item != null)
				{
					locationsList.SelectedItems.Clear();
					locationsList.SelectedItems.Add(item);
				}
			}
		}



		/// <summary>
		/// Key processor for the list view
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void ulvLocations_KeyDown(object sender, KeyEventArgs e)
		{
            UltraListView listView = sender as UltraListView;

            if (listView.ActiveItem != null && listView.ActiveItem.IsInEditMode == false)
            {
				// If the delete key is pressed, call the context menu item event handler code, so that 
				// the same logic gets precessed as would for a context menu delete
				if (e.KeyData == Keys.Delete)
				{
					//miDeleteLocation_Click(sender, null);
					//e.Handled = true;

                    DeleteListLocation();
				}

				// The insert key will create a new location
				else if (e.KeyCode == Keys.Insert)
				{
					miNewLocation_Click(sender, null);
					e.Handled = true;
				}
			}

			// If we have handled the keystroke, the KeyPress event can still fire so set a flag that will 
			// prevent that.
			if (e.Handled)
				this.eatNextKeyPress = true;
		}


		/// <summary>
		/// Called as we doublie click an item in the list view - this is a signal to edit the item
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void locationsList_ItemDoubleClick(object sender, ItemDoubleClickEventArgs e)
		{
			miLvProperties_Click(sender, null);
		}


		/// <summary>
		/// Capture a key press from the list view
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void locationsList_KeyPress(object sender, KeyPressEventArgs e)
		{
			//	If we handled the last KeyDown, mark this event as handled as well.
			if (this.eatNextKeyPress)
			{
				this.eatNextKeyPress = false;
				e.Handled = true;
			}
		}

		private void locationsList_ToolTipDisplaying(object sender, Infragistics.Win.UltraWinListView.ToolTipDisplayingEventArgs e)
		{

		}

		#endregion ListView Event Handlers

		#region PopulateListView
		/// <summary>
		/// Populates the UltraListView control with the child locations of the Location
		/// represented by the specified UltraTreeNode.
		/// </summary>
		private void PopulateListView(UltraTreeNode node)
		{
			AssetGroup parentGroup;
			if (node != null && node.Tag != null)
				parentGroup = node.Tag as AssetGroup;
			else
				parentGroup = new AssetGroup(AssetGroup.GROUPTYPE.userlocation);

			//	Call BeginUpdate to prevent drawing while we are populating the control
			this.locationsList.BeginUpdate();

			//	Show a wait cursor since this could take a while
			this.Cursor = Cursors.WaitCursor;

			//	Clear the Items collection to removes the directories and files
			//	of the last folder we displayed
			this.locationsList.Items.Clear();

			// So request the database for a list of locations beneath that specified
			LocationsDAO lwDataAccess = new LocationsDAO();
			DataTable table = lwDataAccess.GetGroups(parentGroup);

			//	Create an array of UltraListViewItems, with a size equal to the number of directories
			UltraListViewItem[] itemArrayLocations = new UltraListViewItem[table.Rows.Count];

			// ...and add these locations to the tree as children of the selected node
			int index = 0;
			foreach (DataRow row in table.Rows)
			{
				AssetGroup newLocation = new AssetGroup(row ,AssetGroup.GROUPTYPE.userlocation);

				//	Create an array of UltraListViewSubItems
				UltraListViewSubItem[] subItemArray = new UltraListViewSubItem[2];

				//	Create new UltraListViewSubItem instances for this item
				subItemArray[0] = new UltraListViewSubItem();
				subItemArray[1] = new UltraListViewSubItem();

				//	Assign the values to the UltraListViewSubItem instances
				subItemArray[0].Value = newLocation.StartIP;
				subItemArray[1].Value = newLocation.EndIP;

				//	Create an UltraListViewItem instance
				UltraListViewItem item = new UltraListViewItem(newLocation.Name, subItemArray);

				//	Store a reference to the associated Location in the item's Tag property
				item.Tag = newLocation;

				//	Add the UltraListViewItem to the array
				itemArrayLocations[index++] = item;
			}

			// Add the items to the Items collection using the AddRange method, so that we only trigger two property 
			// change notifications, as opposed to one for each item added to the collection.
			this.locationsList.Items.AddRange(itemArrayLocations);

			//	Assign the items to their respective groups
			//this.AssignItemsToGroups();

			// Auto-size the width of the 'Name' column; if the size of the Items collection is not too large, 
			// size all items, but to avoid impacting performance, only size the ones that are currently on-screen 
			// if there are a relatively large number of items.
			Infragistics.Win.UltraWinListView.ColumnAutoSizeMode autoSizeMode = Infragistics.Win.UltraWinListView.ColumnAutoSizeMode.Header;

			if (this.locationsList.Items.Count <= 100)
				autoSizeMode |= Infragistics.Win.UltraWinListView.ColumnAutoSizeMode.AllItems;
			else
				autoSizeMode |= Infragistics.Win.UltraWinListView.ColumnAutoSizeMode.VisibleItems;

			this.locationsList.MainColumn.PerformAutoResize(autoSizeMode);

			//	Activate the first item
			if (this.locationsList.ActiveItem == null && this.locationsList.Items.Count > 0)
				this.locationsList.ActiveItem = this.locationsList.Items[0];

			//	Restore the cursor
			this.Cursor = Cursors.Default;

			//	Call EndUpdate to resume drawing operations
			this.locationsList.EndUpdate(true);
		}
		#endregion PopulateListView


		#region Location Add/Delete/Edit Functions

		#region Public accessible functions

		/// <summary>
		/// Adds a new location as a child of the location currently selected in the TreeView
		/// </summary>
		public void AddLocation()
		{
			if (locationsTree.SelectedNodes.Count == 0)
				return;
			//
			UltraTreeNode selectedNode = locationsTree.SelectedNodes[0];

			AssetGroup parentGroup = selectedNode.Tag as AssetGroup;
			AssetGroup newGroup = new AssetGroup();
			newGroup.GroupType = AssetGroup.GROUPTYPE.userlocation;
			newGroup.ParentID = parentGroup.GroupID;
			//
			FormUserLocation form = new FormUserLocation(parentGroup, newGroup);
			if (form.ShowDialog() == DialogResult.OK)
			{
				UltraTreeNode newNode = selectedNode.Nodes.Add(newGroup.FullName, newGroup.Name);

				newNode.Override.NodeAppearance.Image = Properties.Resources.location_16;
				newNode.Override.ExpandedNodeAppearance.Image = Properties.Resources.location_16;
				newNode.Tag = newGroup;
				newNode.BringIntoView();

				// Add the new location to the list view also
				PopulateListView(selectedNode);
			}
		}



		/// <summary>
		/// Publicly exposed function to edit the currently selected location
		/// </summary>
		public void EditLocation()
		{
			// OK - what are we editing - get the currently selected item
			if (locationsTree.Focused)
				EditTreeLocation();

			else if (locationsList.Focused)
				EditListLocation();
		}


		/// <summary>
		/// Publicly exposed function to delete the currently selected location
		/// </summary>
		public void DeleteLocation()
		{
			// OK - what are we editing - get the currently selected item
			if (locationsTree.Focused)
				DeleteTreeLocation();

			else if (locationsList.Focused)
				DeleteListLocation();
		}

		#endregion

		#region ListView Handlers

		/// <summary>
		/// Display the properties of the currently selected Location when invoked from the ListView
		/// </summary>
		protected void EditListLocation()
		{
			// Sanity check to ensure that only one item is selected in the list
			if (locationsList.SelectedItems.Count != 1)
				return;

			UltraListViewItem selectedItem = locationsList.SelectedItems[0];
			AssetGroup location = selectedItem.Tag as AssetGroup;

			// The parent item of the location being edited is the currently selected node in the tree
			UltraTreeNode selectedNode = locationsTree.SelectedNodes[0];
			AssetGroup parentLocation = selectedNode.Tag as AssetGroup;

			// Display the properties of this location
			string originalName = location.Name;
			if (EditLocation(parentLocation, location))
			{
				// Update the tree and then re-populate the list
				if (location.Name != originalName)
				{
					UltraTreeNode childNode = FindChildNode(selectedNode, originalName);
					if (childNode != null)
					{
						childNode.Text = location.Name;
						childNode.Tag = location;
					}
				}
				PopulateListView(selectedNode);
			}
		}


		/// <summary>
		/// Called to delete locations from the ListView (and also the tree)
		/// </summary>
		protected void DeleteListLocation()
		{
			// Sanity check to ensure that at least one item is selected in the list
			if (locationsList.SelectedItems.Count == 0)
				return;

			foreach (UltraListViewItem item in locationsList.SelectedItems)
			{
				AssetGroup location = item.Tag as AssetGroup;

				// Now delete the location from the database
				if (location.Delete())
				{
					// Successfully deleted so remove from the listview
					locationsList.Items.Remove(item);

					// ...and from the tree view also
					UltraTreeNode parentNode = locationsTree.SelectedNodes[0];
					UltraTreeNode childNode = FindChildNode(parentNode, location.Name);
					parentNode.Nodes.Remove(childNode);
				}
			}
		}

		#endregion ListView Handlers

		#region TreeView Handlers

		/// <summary>
		/// Display the properties of the currently selected Location when invoked from the Tree
		/// </summary>
		protected void EditTreeLocation()
		{
			if (locationsTree.SelectedNodes.Count == 0)
				return;

			// Get the location being edited and its parent location (if any)
			UltraTreeNode selectedNode = locationsTree.SelectedNodes[0];
			AssetGroup location = selectedNode.Tag as AssetGroup;
			AssetGroup parentLocation = null;

			if (selectedNode.Parent != null)
				parentLocation = selectedNode.Parent.Tag as AssetGroup;

			// Display the properties of this location
			if (EditLocation(parentLocation, location))
				selectedNode.Text = location.Name;
		}


		/// <summary>
		/// Delete a location from the tree
		/// </summary>
		protected void DeleteTreeLocation()
		{
            // Get the parent - as we don;t allow selection across levels we can be sure that the parent for all
            // selected items is the same
            UltraTreeNode parentNode = locationsTree.SelectedNodes[0].Parent;

            // first check that the user isn't trying to delete the root location
            if (parentNode == null)
            {
                MessageBox.Show("Unable to delete " + locationsTree.SelectedNodes[0].Text + " - the top-level location cannot be deleted", "AuditWizard", MessageBoxButtons.OK);
                return;
            }

            // Confirm the deletion as this is a pretty serious function
            if (MessageBox.Show("Are you sure that you want to delete the selected location(s)?  All child locations will also be deleted and any child assets moved to the parent location.", "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) != DialogResult.Yes)
                return;

			// Delete these locations
			foreach (UltraTreeNode node in locationsTree.SelectedNodes)
			{
				// get the location and delete from the database
				AssetGroup location = node.Tag as AssetGroup;
				location.Delete();

				// ...and remove it from the tree
				parentNode.Nodes.Remove(node);
			}

			// Select the parent node as this will cause the list to be refreshed
			parentNode.BringIntoView();
			parentNode.Selected = true;
		}
		
		#endregion TreeView Handlers

		#region Generic (Final) functions

		/// <summary>
		/// Called to display the properties of the currently selected node
		/// </summary>
		/// <param name="selectedNode"></param>
		protected bool EditLocation(AssetGroup parentLocation, AssetGroup location)
		{
			// ...and display the location editing form
			FormUserLocation form = new FormUserLocation(parentLocation, location);
			return (form.ShowDialog() == DialogResult.OK);
		}

		#endregion Generic (Final) functions

		#endregion Location add/edit/delete functions


		/// <summary>
		/// Find the (named) child node of the specified tree node
		/// </summary>
		/// <param name="parentNode"></param>
		/// <returns></returns>
		private UltraTreeNode FindChildNode (UltraTreeNode parentNode ,string childName)
		{
			foreach (UltraTreeNode childNode in parentNode.Nodes)
			{
				if (childNode.Text == childName)
					return childNode;
			}
			return null;
		}



	}
}
