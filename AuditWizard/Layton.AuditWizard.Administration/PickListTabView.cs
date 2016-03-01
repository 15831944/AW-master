using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using Infragistics.Win.UltraWinExplorerBar;
using Infragistics.Win.UltraWinListView;
using Layton.AuditWizard.DataAccess;
using Layton.Cab.Interface;
using Microsoft.Practices.CompositeUI;
using Microsoft.Practices.CompositeUI.SmartParts;
using Microsoft.Practices.ObjectBuilder;

namespace Layton.AuditWizard.Administration
{
    [SmartPart]
    public partial class PickListTabView : UserControl, ILaytonView, IAdministrationView
    {
        private LaytonWorkItem workItem;
		PickListTabViewPresenter presenter;
		private UltraExplorerBarItem _activeItem = null;
		private PickListList _listPickLists = new PickListList();

        [InjectionConstructor]
		public PickListTabView([ServiceDependency] WorkItem workItem)
        {
            this.workItem = workItem as LaytonWorkItem;
            InitializeComponent();
        }

		[CreateNew]
		public PickListTabViewPresenter Presenter
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
			base.Refresh();
			InitializeData();
		}

		/// <summary>
		/// Called as this tab is activated to ensure that we display the latest possible data
		/// This function comes from the IAdministrationView Interface
		/// </summary>
		public void Activate()
		{
			InitializeData();
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
			// Save the name of the currently selected explorer bar item
			PickList lastSelectedPickList = null;
			if (_activeItem != null)
				lastSelectedPickList = _activeItem.Tag as PickList;

			// Read the user defined data definitions from the database
			_listPickLists.Populate();

			// Add the categories to the Explorer View
			this.pickListsExplorerBar.BeginUpdate();
			this.pickListsExplorerBar.Groups[0].Items.Clear();
			//
			foreach (PickList pickList in _listPickLists)
			{
				UltraExplorerBarItem item = this.pickListsExplorerBar.Groups[0].Items.Add(pickList.Name, pickList.Name);
				item.Settings.AppearancesLarge.Appearance.Image = Properties.Resources.picklist_32;
				item.Tag = pickList;

				// If this was the selected item previously then flag to select it again
				if ((lastSelectedPickList != null)
				&&  (lastSelectedPickList.Name == pickList.Name))
					this.pickListsExplorerBar.ActiveItem = item;
			}

			// If nothing is selected in the Explorer View then select the first entry if there are any
			if (this.pickListsExplorerBar.Groups[0].Items.Count != 0)
				this.pickListsExplorerBar.ActiveItem = this.pickListsExplorerBar.Groups[0].Items[0];

			// Finish updating
			this.pickListsExplorerBar.EndUpdate();
		}



		/// <summary>
		/// Refresh the list of Asset Types for the currently selected asset type category
		/// </summary>
		/// <param name="categoryID"></param>
		private void RefreshList(PickList picklist)
		{
			UltraListViewItem lvi = null;
			this.ulvPickItems.Items.Clear();

			foreach (PickItem item in picklist)
			{
				lvi = new UltraListViewItem(item, null);
				lvi.Tag = item;
				this.ulvPickItems.Items.Add(lvi);
			}
		}

		#region Explorer Bar Handlers

		private void userDataExplorerBar_ActiveItemChanged(object sender, Infragistics.Win.UltraWinExplorerBar.ItemEventArgs e)
		{
			// Display the sub-items
			_activeItem = e.Item;
			PickList selectedPickList = e.Item.Tag as PickList;
			RefreshList(selectedPickList);

		    bnEditList.Enabled = true;
		}

		#endregion Explorer Bar Handlers

		#region Context Menu Handling

		/// <summary>
		/// Called as the Categories menu is opening to enable/disable options as appropriate
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void cmsPickLists_Opening(object sender, CancelEventArgs e)
		{
			// Delete enabled if we have no fields 
			this.miDeletePickList.Enabled = (ulvPickItems.Items.Count == 0);

            this.miDeletePickList.Enabled = (pickListsExplorerBar.SelectedGroup.Items.Count > 0);
            this.miPickListProperties.Enabled = (pickListsExplorerBar.SelectedGroup.Items.Count > 0);
		}


		/// <summary>
		/// Called as the Fields menu is opening to enable/disable options as appropriate
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void cmsPickItems_Opening(object sender, CancelEventArgs e)
		{
			// Delete and Properties enabled if we have just one item selected
			miDeleteItem.Enabled = (ulvPickItems.SelectedItems.Count == 1);
            miNewItem.Enabled = (pickListsExplorerBar.ActiveItem != null);
		}


		/// <summary>
		/// Called to create a new Picklist
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void miNewPickList_Click(object sender, EventArgs e)
		{
            AddNewPickList();
		}

        private void AddNewPickList()
        {
            PickList newPickList = new PickList();
            newPickList.Name = "<new picklist>";

            FormPicklist form = new FormPicklist(newPickList);
            if (form.ShowDialog() == DialogResult.OK)
                RefreshTab();
        }


		
		/// <summary>
		/// Called as we select to delete a Picklist - note that we cannot delete a picklist
		/// without deleting all pick items and also cannot delete while there are references to it from
		/// any user defined data field
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void miDeletePickList_Click(object sender, EventArgs e)
		{
            DeletePickList();
		}

        private void DeletePickList()
        {
            if (_activeItem == null)
                return;

            PickList picklist = _activeItem.Tag as PickList;
            if (MessageBox.Show("Are you sure that you want to delete this Picklist?", "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) != DialogResult.Yes)
                return;

            // Delete this Picklist and any associated pick items
            picklist.Delete();
            _activeItem = null;

            // We should still refresh as we have partially deleted the category
            RefreshTab();
        }



		/// <summary>
		/// Display the properties of the currently selected User Data Category
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void miPickListProperties_Click(object sender, EventArgs e)
		{
            EditPickList();
		}

        private void EditPickList()
        {
            if (_activeItem != null)
            {
                PickList picklist = _activeItem.Tag as PickList;
                //
                if (picklist != null)
                {
                    FormPicklist form = new FormPicklist(picklist);
                    if (form.ShowDialog() == DialogResult.OK)
                        RefreshTab();
                }
            }
        }


		/// <summary>
		/// Called when we want to create a new Pick Item (as a child of the selected PickList)
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void miNewItem_Click(object sender, EventArgs e)
		{
            AddNewItem();
		}

        private void AddNewItem()
        {
            if (_activeItem == null)
                return;

            // Get the picklist so we add to it's items
            PickList picklist = _activeItem.Tag as PickList;

            // Add a NULL item to the end of the list which we then edit to create the new item
            PickItem nullPickItem = new PickItem();
            nullPickItem.Name = "New PickItem";
            nullPickItem.ParentID = picklist.PicklistID;
            //added for fixing Bug No 859 Sojan E John, KTSInfotech

            FormAddPickItem form = new FormAddPickItem(nullPickItem, _activeItem);
            if (form.ShowDialog() == DialogResult.OK)
                RefreshTab();

            //UltraListViewItem lvi = new UltraListViewItem(nullPickItem, null);
            //lvi.Tag = nullPickItem;
            //ulvPickItems.Items.Add(lvi);

            //// Begin to edit the item
            //EditItem(lvi);
        }       

		/// <summary>
		/// Delete the currently selected pickitem
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void miDeleteItem_Click(object sender, EventArgs e)
		{
            DeleteItem();
		}

        private void DeleteItem()
        {
            if (ulvPickItems.SelectedItems.Count == 0)
                return;

            if (MessageBox.Show("Are you sure that you want to delete this Picklist Item?",
                "Confirm Delete",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) != DialogResult.Yes)
                return;

            UltraListViewItem lvi = ulvPickItems.SelectedItems[0];
            PickItem pickItem = lvi.Tag as PickItem;
            if (pickItem != null) pickItem.Delete();

            // Refresh the view
            RefreshTab();
        }

		#endregion Context Menu Handling


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
				UltraListViewItem item = ulvPickItems.ItemFromPoint(currentPoint);

				if (item != null)
				{
					ulvPickItems.SelectedItems.Clear();
					ulvPickItems.SelectedItems.Add(item);
				}
			}
		}



		/// <summary>
		/// Key processor for the list view
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void ulvPickItems_KeyDown(object sender, KeyEventArgs e)
		{
			// Handle the delete key - this should delete the currently selected item
			if (e.KeyCode == Keys.Delete)
			{
				miDeleteItem_Click(sender, null);
				e.Handled = true;
			}

			// The insert key will create a new PickItem
			else if (e.KeyCode == Keys.Insert)
			{
				miNewItem_Click(sender, null);
				e.Handled = true;
			}
		}



		private void ulvPickItems_ItemDoubleClick(object sender, ItemDoubleClickEventArgs e)
		{
			EditItem(e.Item);
		}

		
		/// <summary>
		/// Begin editing of a listview item
		/// </summary>
		/// <param name="lvi"></param>
		private void EditItem(UltraListViewItem lvi)
		{
            // Allow editing
            //this.ulvPickItems.ItemSettings.AllowEdit = Infragistics.Win.DefaultableBoolean.False;
            //lvi.BeginEdit();
            
            PickItem pickItem = lvi.Tag as PickItem;
            if (pickItem != null)
            {
                FormAddPickItem form = new FormAddPickItem(pickItem, _activeItem);
                if (form.ShowDialog() == DialogResult.OK)
                    RefreshTab();
            }
		}

		/// <summary>
		/// Called as we exit from edit mode - we need to ensure that what we have entered is valid
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void ulvPickItems_ItemExitingEditMode(object sender, ItemExitingEditModeEventArgs e)
		{
           
			UltraListViewItem lvi = e.Item;           
			PickItem editedPickItem = lvi.Tag as PickItem;
            //string name = (string)e.Editor.Value;
            string name = e.Editor.Value.ToString();

			// If the name is blank then it will be invalid
			if (name == "")
			{
                MessageBox.Show("Please specify a unique name for this pickitem", "Enter Name", MessageBoxButtons.OK, MessageBoxIcon.Information);
				e.Cancel = true;
				return;
			}

			// Get the picklist so we check it's items
			PickList picklist = _activeItem.Tag as PickList;

			// Does the specified value duplicate an existing pickitem?
			foreach (PickItem item in picklist)
			{
                
                if ((item.Name == name) && (item.PickItemID != editedPickItem.PickItemID))
                {
                    MessageBox.Show(
                        "The specified name matches that of an existing item in this picklist, please specify a unique name for this pickitem",
                        "Duplicate Name",
                        MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    e.Cancel = true;
                    return;
                }              
            
			}

			// If it's a new pickitem then we need to add it to the parent list also
			if (editedPickItem.PickItemID == 0)
				picklist.Add(editedPickItem);

			// Update the PickItem in the database
			editedPickItem.Name = name;
			editedPickItem.Update();

			// Prevent editing again for now
			this.ulvPickItems.ItemSettings.AllowEdit = Infragistics.Win.DefaultableBoolean.False;
		}

        private void bnDeleteList_Click(object sender, EventArgs e)
        {
            DeletePickList();
        }

        private void bnNewList_Click(object sender, EventArgs e)
        {
            AddNewPickList();
        }

        private void bnEditList_Click(object sender, EventArgs e)
        {
            EditPickList();
        }

        private void bnAddItem_Click(object sender, EventArgs e)
        {
            AddNewItem();
        }

        private void bnDeleteItem_Click(object sender, EventArgs e)
        {
            DeleteItem();
        }

        private void bnEditItem_Click(object sender, EventArgs e)
        {
            if (ulvPickItems.SelectedItems.Count == 0)
                return;
            EditItem(ulvPickItems.SelectedItems[0]);
        }

        private void ulvPickItems_ItemSelectionChanged(object sender, ItemSelectionChangedEventArgs e)
        {
            if (ulvPickItems.SelectedItems.Count == 1)
                bnEditItem.Enabled = true;
        }

        private void miPickItemProperties_Click(object sender, EventArgs e)
        {

        }
        
    }
}
