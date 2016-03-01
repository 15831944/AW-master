using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Drawing;
using System.Data;
using System.IO;
using System.Text;
using System.Windows.Forms;
using Microsoft.Practices.CompositeUI.SmartParts;
using Layton.Cab.Interface;
using Microsoft.Practices.CompositeUI;
using Microsoft.Practices.CompositeUI.Utility;
using Microsoft.Practices.ObjectBuilder;
//
using Infragistics.Win.UltraWinExplorerBar;
using Infragistics.Win.UltraWinListView;
using Infragistics.Win;
//
using Layton.AuditWizard.Common;
using Layton.Common.Controls;
using Layton.AuditWizard.DataAccess;

namespace Layton.AuditWizard.Administration
{
    [SmartPart]
    public partial class AssetTypesTabView : UserControl, ILaytonView, IAdministrationView
    {
        private LaytonWorkItem			workItem;
		private AssetTypesTabViewPresenter presenter;
		private AssetTypeList			_listAssetTypes = new AssetTypeList();
		private UltraExplorerBarItem	_activeItem = null;

        [InjectionConstructor]
		public AssetTypesTabView([ServiceDependency] WorkItem workItem)
        {
            this.workItem = workItem as LaytonWorkItem;
            InitializeComponent();
		}

		[CreateNew]
		public AssetTypesTabViewPresenter Presenter
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
		/// Called to refresh the information displayed on this tab
		/// </summary>
		protected void RefreshTab()
		{
			this.assettypesExplorerBar.Groups[0].Items.Clear();
			_listAssetTypes = new AssetTypeList();
			_listAssetTypes.Populate();

			// We now need to display the Asset Type categories in the main ExplorerBar
			AssetTypeList categories = _listAssetTypes.EnumerateCategories();

			foreach (AssetType category in categories)
			{
				UltraExplorerBarItem item = this.assettypesExplorerBar.Groups[0].Items.Add(category.Name, category.Name);
				item.Settings.AppearancesLarge.Appearance.Image = IconMapping.LoadIcon(category.Icon, IconMapping.Iconsize.Medium);
				item.Tag = category;
			}

			// If nothing is selected in the Explorer View then select the first entry
            if (this.assettypesExplorerBar.ActiveItem == null)
            {
                if ((_activeItem == null) || (!this.assettypesExplorerBar.Groups[0].Items.Contains(_activeItem)))
                {
                    this.assettypesExplorerBar.ActiveItem = this.assettypesExplorerBar.Groups[0].Items[0];
                    _activeItem = this.assettypesExplorerBar.Groups[0].Items[0];
                }
                else
                {
                    this.assettypesExplorerBar.ActiveItem = _activeItem;
                }
            }
		}



		/// <summary>
		/// Refresh the list of Asset Types for the currently selected asset type category
		/// </summary>
		/// <param name="categoryID"></param>
		private void RefreshList(int categoryID)
		{
			// Call BeginUpdate and set the wait cursor while we refresh
			this.ulvAssetTypes.BeginUpdate();
			this.Cursor = Cursors.WaitCursor;

			// Clear any existing items from the view
			ulvAssetTypes.Items.Clear();

			// ...Get the children to display
			AssetTypeList listSubTypes = _listAssetTypes.EnumerateChildren(categoryID);

			// ...and add to the list view
			foreach (AssetType assettype in listSubTypes)
			{
				Bitmap icon = IconMapping.LoadIcon(assettype.Icon, IconMapping.Iconsize.Small);
				UltraListViewItem lvi = new UltraListViewItem(assettype ,null);
				lvi.Appearance.Image = icon;
				lvi.Tag = assettype;
				ulvAssetTypes.Items.Add(lvi);
			}

			// Restore the cursor and end the update
			this.Cursor = Cursors.Default;
			this.ulvAssetTypes.EndUpdate(true);

		}

		/// <summary>
		/// Called as we click a mouse button within the list view.  If this is the right button then we should 
		/// select (any) item which the mouse is over
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void ulvAssetTypes_MouseDown(object sender, MouseEventArgs e)
		{
			if (e.Button == MouseButtons.Right)
			{
				//Get the position of the mouse
				Point currentPoint = new Point(e.X, e.Y);

				//Get the node the mouse is over.
				UltraListViewItem item = ulvAssetTypes.ItemFromPoint(currentPoint);

				if (item != null)
				{
					ulvAssetTypes.SelectedItems.Clear();
					ulvAssetTypes.SelectedItems.Add(item);
				}
			}
		}



		// Double clicking an item edits it
		private void ulvAssetTypes_ItemDoubleClick(object sender, ItemDoubleClickEventArgs e)
		{
            miPropertiesType_Click(sender, null);
		}

		#region Explorer Bar Handlers

		private void assettypesExplorerBar_ActiveItemChanged(object sender, Infragistics.Win.UltraWinExplorerBar.ItemEventArgs e)
		{
			// Display the sub-items
			_activeItem = e.Item;
			AssetType selectedCategory = e.Item.Tag as AssetType;
			RefreshList(selectedCategory.AssetTypeID);

		    bnEditCategory.Enabled = true;
		}

		#endregion Explorer Bar Handlers

		#region Context Menu Handling

		/// <summary>
		/// Called as we open the context menu for the Asset Cateories - we cannot delete any of the standard
		/// asset categories as the system would not function correctly without them.  Standard types are:
		/// 
		/// Computers
		/// Peripherals
		/// USB Devices
		/// Mobile Devices
		/// 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void cmsCategories_Opening(object sender, CancelEventArgs e)
		{
			AssetType activeAssetType = _activeItem.Tag as AssetType;
			miDeleteCategory.Enabled = !activeAssetType.Internal;

			// CMD - 8.3.6
			// Disable properties on the first category as we cannot allow the user to rename it as this upsets later code which looks
			// specifically for a 'Computers' category
			miCategoryProperties.Enabled = (_activeItem.Index != 0);
		}


		/// <summary>
		/// Called as we open the context menu strip for the asset types (not categories).  We need to prevent 
		/// deletion of the 'internal'
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void cmsFields_Opening(object sender, CancelEventArgs e)
		{
			// Can only add if no item selected
			if (ulvAssetTypes.SelectedItems.Count == 0)
			{
				miDeleteType.Enabled = false;
				miTypeProperties.Enabled = false;
			}
			
			else
			{
				miTypeProperties.Enabled = true;
				AssetType subType = this.ulvAssetTypes.SelectedItems[0].Tag as AssetType;
				miDeleteType.Enabled = !subType.Internal;
			}
		}

		private void miNewCategory_Click(object sender, EventArgs e)
		{
            AddCategory();
		}

        private void AddCategory()
        {
            AssetType newCategory = new AssetType();
            newCategory.Name = "<new category>";
            FormAssetCategory form = new FormAssetCategory(newCategory);
            if (form.ShowDialog() == DialogResult.OK)
                RefreshTab();
        }


		/// <summary>
		/// Called as we select to delete a category of asset - note that we cannot delete a category
		/// without deleting all sub-categories which in turn must not be being referenced by any assets
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void miDeleteCategory_Click(object sender, EventArgs e)
		{
            DeleteCategory();
		}

        private void DeleteCategory()
        {
            AssetType activeAssetType = _activeItem.Tag as AssetType;
            AssetTypeList listSubCategories = _listAssetTypes.EnumerateChildren(activeAssetType.AssetTypeID);

            if (listSubCategories.Count == 0)
            {
                if (MessageBox.Show("Are you sure that you want to delete this Asset Category?", "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) != DialogResult.Yes)
                    return;
            }
            else
            {
                if (MessageBox.Show("Are you sure that you want to delete this Asset Category?  Deleting the category will also delete all child asset types.", "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) != DialogResult.Yes)
                    return;
            }

            // Delete this asset type and any sub-types
            if (activeAssetType.Delete() != 0)
                MessageBox.Show("Failed to delete the selected Category - there may still be references to one of the child asset types which must be removed before the category may be deleted", "Delete Failed");
            _activeItem = null;

            // We should still refresh as we have partially deleted the category
            RefreshTab();
        }



		/// <summary>
		/// Display the properties of the currently selected Asset Category
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void miCategoryProperties_Click(object sender, EventArgs e)
		{
            EditCategory();
		}

        private void EditCategory()
        {
            AssetType activeAssetType = _activeItem.Tag as AssetType;
            FormAssetCategory form = new FormAssetCategory(activeAssetType);
            if (form.ShowDialog() == DialogResult.OK)
                RefreshTab();
        }


		/// <summary>
		/// Called when we want to create a new Asset Type (as a child of the selected category)
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void miNewType_Click(object sender, EventArgs e)
		{
            AddType();
		}

        private void AddType()
        {
            // Get the selected (active) category
            AssetType activeAssetType = _activeItem.Tag as AssetType;

            // Create a new asset type object
            AssetType newAssetType = new AssetType();
            newAssetType.SetParentCategory(activeAssetType);
            newAssetType.Icon = activeAssetType.Icon;

            // ...and invoke the AssetType Properties form
            FormAssetType form = new FormAssetType(activeAssetType, newAssetType);
            if (form.ShowDialog() == DialogResult.OK)
                RefreshTab();
        }


		/// <summary>
		/// Delete the currently selected asset type
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void miDeleteType_Click(object sender, EventArgs e)
		{
            DeleteType();
		}

        private void DeleteType()
        {
            // Get the currently selected asset type in the list view (if any)
            if (this.ulvAssetTypes.SelectedItems.Count == 0)
                return;
            AssetType subType = this.ulvAssetTypes.SelectedItems[0].Tag as AssetType;

            if (MessageBox.Show("Are you sure that you want to delete this Asset Type?", "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) != DialogResult.Yes)
                return;

            // Delete this asset type
            if (subType.Delete() != 0)
                MessageBox.Show("Failed to delete the selected Asset Type - there may still be references to this asset type which must be removed before this type can be deleted", "Delete Failed");

            // We should still refresh as we have partially deleted the category
            RefreshTab();
        }



		/// <summary>
		/// Display the properties of and allow the user to update the selected Asset Type
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void miPropertiesType_Click(object sender, EventArgs e)
		{
            EditType();

		}

        private void EditType()
        {
            // Get the currently selected asset type in the list view (if any)
            if (this.ulvAssetTypes.SelectedItems.Count == 0)
                return;
            AssetType subType = this.ulvAssetTypes.SelectedItems[0].Tag as AssetType;

            // Get the selected (active) category
            AssetType activeAssetCategory = _activeItem.Tag as AssetType;

            // ...and invoke the AssetType Properties form
            FormAssetType form = new FormAssetType(activeAssetCategory, subType);
            if (form.ShowDialog() == DialogResult.OK)
                RefreshTab();
        }

		#endregion Context Menu Handling

        private void bnNewCategory_Click(object sender, EventArgs e)
        {
            AddCategory();
        }

        private void bnDeleteCategory_Click(object sender, EventArgs e)
        {
            DeleteCategory();
        }

        private void bnEditCategory_Click(object sender, EventArgs e)
        {
            EditCategory();
        }

        private void bnAddField_Click(object sender, EventArgs e)
        {
            AddType();
        }

        private void bnDeleteField_Click(object sender, EventArgs e)
        {
            DeleteType();
        }

        private void bnEditField_Click(object sender, EventArgs e)
        {
            EditType();
        }

        private void ulvAssetTypes_ItemSelectionChanged(object sender, ItemSelectionChangedEventArgs e)
        {
            bnEditField.Enabled = (ulvAssetTypes.SelectedItems.Count == 1);

        }
	}
}
