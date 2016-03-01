using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using Infragistics.Win;
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
    public partial class UserDefinedDataTabView : UserControl, ILaytonView, IAdministrationView
    {
        private LaytonWorkItem workItem;
        UserDefinedDataTabViewPresenter presenter;
        private UltraExplorerBarItem _activeItem = null;
        private UserDataCategory.SCOPE _currentScope = UserDataCategory.SCOPE.Asset;
        private UserDataCategoryList _listCategories = new UserDataCategoryList(UserDataCategory.SCOPE.Asset);

        // Drag and drop
        private Nullable<Point> lastMouseDown;
        private UltraListViewItem dragItem;
        private UltraListViewItem dropItem;
        private UltraExplorerBarItem dragExplorerItem;
        private UltraExplorerBarItem dropExplorerItem;
        private object lastMouseDownLocation;


        [InjectionConstructor]
        public UserDefinedDataTabView([ServiceDependency] WorkItem workItem)
        {
            this.workItem = workItem as LaytonWorkItem;
            InitializeComponent();
        }

        [CreateNew]
        public UserDefinedDataTabViewPresenter Presenter
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


        #region Refresh Functions

        /// <summary>
        /// Called by our controller to control the types of fields which are to be displayed
        /// </summary>
        /// <param name="newScope"></param>
        public void SelectDisplayFields(UserDataCategory.SCOPE newScope)
        {
            if (_currentScope == newScope)
                return;
            _currentScope = newScope;
            RefreshTab();
        }

        /// <summary>
        /// Called to refresh the information displayed on the users tab
        /// </summary>
        protected void RefreshTab()
        {
            _listCategories.Scope = _currentScope;
            UserDataCategory dummy = new UserDataCategory(_currentScope);
            headerLabel.Text = "User Defined Data [" + dummy.ScopeAsString + "]";

            // Read the user defined data definitions from the database making sure that we set the 
            // required scope first
            _listCategories.Populate();

            // Clear the list
            ulvUserData.Items.Clear();

            // Add the categories to the Explorer View
            userDataExplorerBar.BeginUpdate();
            userDataExplorerBar.Groups[0].Items.Clear();
            //
            foreach (UserDataCategory category in _listCategories)
            {
                UltraExplorerBarItem item = userDataExplorerBar.Groups[0].Items.Add(category.Name, category.Name);
                item.Settings.AppearancesLarge.Appearance.Image = IconMapping.LoadIcon(category.Icon, IconMapping.Iconsize.Medium);
                item.Tag = category;
            }

            // If nothing is selected in the Explorer View then select the first entry if there are any
            if (userDataExplorerBar.Groups[0].Items.Count != 0)
            {
                if (_activeItem == null)
                {
                    userDataExplorerBar.ActiveItem = userDataExplorerBar.Groups[0].Items[0];
                    _activeItem = userDataExplorerBar.Groups[0].Items[0];
                }

                else
                {
                    // Can we find the item
                    int index = userDataExplorerBar.Groups[0].Items.IndexOf(_activeItem.Key);
                    if (index != -1)
                    {
                        userDataExplorerBar.ActiveItem = userDataExplorerBar.Groups[0].Items[index];
                    }
                    else
                    {
                        userDataExplorerBar.ActiveItem = userDataExplorerBar.Groups[0].Items[0];
                        _activeItem = userDataExplorerBar.Groups[0].Items[0];
                    }
                }
            }

            userDataExplorerBar.EndUpdate();
        }



        /// <summary>
        /// Refresh the list of Asset Types for the currently selected asset type category
        /// </summary>
        /// <param name="category"></param>
        private void RefreshList(UserDataCategory category)
        {
            ulvUserData.BeginUpdate();
            ulvUserData.Items.Clear();

            // Get the small icon which we will display
            Bitmap image = IconMapping.LoadIcon(category.Icon, IconMapping.Iconsize.Small);

            // Add the items to the list
            foreach (UserDataField field in category)
            {
                // Subitems are type and description
                UltraListViewSubItem[] subItemArray = new UltraListViewSubItem[2];
                subItemArray[0] = new UltraListViewSubItem { Value = field.TypeAsString };
                subItemArray[1] = new UltraListViewSubItem { Value = field.Description() };
                //
                UltraListViewItem lvi = new UltraListViewItem(field.Name, subItemArray);
                lvi.Tag = field;
                lvi.Appearance.Image = image;
                ulvUserData.Items.Add(lvi);
            }
            ulvUserData.EndUpdate();
        }

        #endregion Refresh Functions

        #region Explorer Bar Handlers

        private void userDataExplorerBar_ActiveItemChanged(object sender, Infragistics.Win.UltraWinExplorerBar.ItemEventArgs e)
        {
            // Display the sub-items
            _activeItem = e.Item;
            if (e.Item != null)
            {
                UserDataCategory selectedCategory = e.Item.Tag as UserDataCategory;
                RefreshList(selectedCategory);
            }

            bnEditUserCategory.Enabled = true;
        }


        /// <summary>
        /// Double-clicking an item effectively edits it
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void userDataExplorerBar_ItemDoubleClick(object sender, Infragistics.Win.UltraWinExplorerBar.ItemEventArgs e)
        {
            EditUserDataCategory();
        }


        /// <summary>
        /// Key processor for the Explorer bar
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void userDataExplorerBar_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete)
            {
                DeleteUserDataCategory();
                e.Handled = true;
            }

            // The insert key will create a new field
            else if (e.KeyCode == Keys.Insert)
            {
                AddUserDataCategory();
                e.Handled = true;
            }
        }

        #endregion Explorer Bar Handlers

        #region Context Menu Handling

        #region TreeView Context Menu handlers (Categories)

        /// <summary>
        /// Called as the Categories menu is opening to enable/disable options as appropriate
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cmsCategories_Opening(object sender, CancelEventArgs e)
        {
            // Delete enabled if we have no fields 
            miDeleteCategory.Enabled = (ulvUserData.Items.Count == 0);

            miDeleteCategory.Enabled = (userDataExplorerBar.SelectedGroup.Items.Count > 0);
            miCategoryProperties.Enabled = (userDataExplorerBar.SelectedGroup.Items.Count > 0);
        }


        /// <summary>
        /// Called to create a new category of user data
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void miNewCategory_Click(object sender, EventArgs e)
        {
            AddUserDataCategory();
        }


        /// <summary>
        /// Called as we select to delete a category of user data - note that we cannot delete a category
        /// without deleting all sub-categories which in turn must not be being referenced by any assets
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void miDeleteCategory_Click(object sender, EventArgs e)
        {
            DeleteUserDataCategory();
        }


        /// <summary>
        /// Display the properties of the currently selected User Data Category
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void miCategoryProperties_Click(object sender, EventArgs e)
        {
            EditUserDataCategory();
        }

        #endregion TreeView Context Menu handlers (Categories)

        #region ListView Context Menu handlers (Fields)

        /// <summary>
        /// Called as the Fields menu is opening to enable/disable options as appropriate
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cmsFields_Opening(object sender, CancelEventArgs e)
        {
            // Add only available if we have a category selected
            miNewType.Enabled = (userDataExplorerBar.Groups[0].Items.Count != 0);

            // Delete and Properties enabled if we have just one item selected
            miDeleteType.Enabled = (ulvUserData.SelectedItems.Count == 1);
            miTypeProperties.Enabled = (ulvUserData.SelectedItems.Count == 1);
        }


        /// <summary>
        /// Called when we want to create a new User Data Field (as a child of the selected category)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void miNewField_Click(object sender, EventArgs e)
        {
            AddUserDataField();
        }


        /// <summary>
        /// Delete the currently selected user data field
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void miDeleteField_Click(object sender, EventArgs e)
        {
            DeleteUserDataField();
        }



        /// <summary>
        /// Display the properties of and allow the user to update the selected User Data Field
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void miPropertiesField_Click(object sender, EventArgs e)
        {
            EditUserDataField();
        }

        #endregion ListView Context Menu handlers (Fields)

        #endregion Context Menu Handling

        #region ListView Message Handlers

        /// <summary>
        /// Key processor for the list view
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ulvUserData_KeyDown(object sender, KeyEventArgs e)
        {
            // Handle the delete key - this should delete the currently selected field
            if (e.KeyCode == Keys.Delete)
            {
                DeleteUserDataField();
                e.Handled = true;
            }

            // The insert key will create a new field
            else if (e.KeyCode == Keys.Insert)
            {
                AddUserDataField();
                e.Handled = true;
            }
        }


        /// <summary>
        /// Called as we click a mouse button within the list view.  If this is the right button then we should 
        /// select (any) item which the mouse is over
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ulvUserData_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
                lastMouseDown = e.Location;
            else
                lastMouseDown = null;

            UltraListView listViewDrag = sender as UltraListView;
            UltraListViewItem itemAtPoint = listViewDrag.ItemFromPoint(e.X, e.Y, true);
            if (itemAtPoint != null)
            {
                lastMouseDownLocation = new Point(e.X, e.Y);

                if (e.Button == MouseButtons.Right)
                {
                    ulvUserData.SelectedItems.Clear();
                    ulvUserData.SelectedItems.Add(itemAtPoint);
                }
            }
            else
            {
                lastMouseDownLocation = null;
            }
        }

        private void ulvUserData_DragOver(object sender, DragEventArgs e)
        {
            UltraListView listView = sender as UltraListView;
            Point clientPos = listView.PointToClient(new Point(e.X, e.Y));

            if (dragItem != null)
            {
                dropItem = listView.ItemFromPoint(clientPos);
                e.Effect = dropItem != null && dropItem != dragItem ? DragDropEffects.Move : DragDropEffects.None;
            }

            //  If the cursor is within {dragScrollAreaHeight} pixels
            //  of the top or bottom edges of the control, scroll
            int dragScrollAreaHeight = 8;

            Rectangle displayRect = listView.DisplayRectangle;
            Rectangle topScrollArea = displayRect;
            topScrollArea.Height = (dragScrollAreaHeight * 2);

            Rectangle bottomScrollArea = displayRect;
            bottomScrollArea.Y = bottomScrollArea.Bottom - dragScrollAreaHeight;
            bottomScrollArea.Height = dragScrollAreaHeight;

            ISelectionManager selectionManager = listView;
            if (topScrollArea.Contains(clientPos) || bottomScrollArea.Contains(clientPos))
                selectionManager.DoDragScrollVertical(0);


        }

        private void ulvUserData_MouseMove(object sender, MouseEventArgs e)
        {
            UltraListView listView = sender as UltraListView;

            //  If the mouse has moved outside the area in which it was pressed,
            //  start a drag operation
            if (lastMouseDown.HasValue)
            {
                Size dragSize = SystemInformation.DragSize;
                Rectangle dragRect = new Rectangle(lastMouseDown.Value, dragSize);
                dragRect.X -= dragSize.Width / 2;
                dragRect.Y -= dragSize.Height / 2;

                if (!dragRect.Contains(e.Location))
                {
                    UltraListViewItem itemAtPoint = listView.ItemFromPoint(e.Location);

                    if (itemAtPoint != null)
                    {
                        lastMouseDown = null;
                        dragItem = itemAtPoint;
                        listView.DoDragDrop(dragItem, DragDropEffects.Move);
                    }
                }
            }
        }

        private void ulvUserData_DragDrop(object sender, DragEventArgs e)
        {
            UltraListView listView = sender as UltraListView;
            OnDragEnd(listView, false);
        }

        private void ulvUserData_QueryContinueDrag(object sender, QueryContinueDragEventArgs e)
        {
            UltraListView listView = sender as UltraListView;

            //  Cancel the drag operation if the escape key was pressed
            if (e.EscapePressed)
            {
                OnDragEnd(listView, true);
                e.Action = DragAction.Cancel;
            }
        }

        private void OnDragEnd(UltraListView listView, bool canceled)
        {
            if (canceled == false && dragItem != null && dropItem != null)
            {
                listView.BeginUpdate();

                int index = dropItem.Index;
                listView.Items.Remove(dragItem);
                listView.Items.Insert(index, dragItem);

                listView.EndUpdate();

                UpdateFieldsTabOrder();
                RefreshTab();
            }

            dragItem = dropItem = null;
            lastMouseDown = null;
        }

        private void UpdateFieldsTabOrder()
        {
            UserDataDefinitionsDAO lUserDataDefinitionsDao = new UserDataDefinitionsDAO();

            for (int i = 0; i < ulvUserData.Items.Count; i++)
            {
                UserDataField udf = ulvUserData.Items[i].Tag as UserDataField;
                if (udf != null)
                {
                    udf.TabOrder = i;
                    lUserDataDefinitionsDao.UserDataFieldUpdate(udf);
                }
            }
        }

        private void OnDragEnd(UltraExplorerBar explorerBar, bool canceled)
        {
            if (canceled == false && dragExplorerItem != null && dropExplorerItem != null)
            {
                explorerBar.BeginUpdate();

                int index = dropExplorerItem.Index;
                explorerBar.Groups[0].Items.Remove(dragExplorerItem);
                explorerBar.Groups[0].Items.Insert(index, dragExplorerItem);

                explorerBar.EndUpdate();

                UpdateCategoriesTabOrder();
                RefreshTab();
            }

            dragExplorerItem = dropExplorerItem = null;
            lastMouseDown = null;
        }

        private void UpdateCategoriesTabOrder()
        {
            UserDataDefinitionsDAO lUserDataDefinitionsDao = new UserDataDefinitionsDAO();

            for (int i = 0; i < userDataExplorerBar.Groups[0].Items.Count; i++)
            {
                UserDataCategory userDataCategory = userDataExplorerBar.Groups[0].Items[i].Tag as UserDataCategory;
                if (userDataCategory != null)
                {
                    userDataCategory.TabOrder = i;
                    lUserDataDefinitionsDao.UserDataCategoryUpdate(userDataCategory);
                }
            }
        }

        /// <summary>
        /// Double-clicking an item displays it's properties
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ulvUserData_ItemDoubleClick(object sender, ItemDoubleClickEventArgs e)
        {
            EditUserDataField();
        }




        #endregion ListView Message Handlers

        #region User Data Category Add/Delete/Edit Functions

        /// <summary>
        /// Add a new user data category
        /// </summary>
        protected void AddUserDataCategory()
        {
            UserDataCategory newCategory = new UserDataCategory();
            newCategory.Scope = _currentScope;
            newCategory.Name = "<new category>";
            FormUserDataCategory form = new FormUserDataCategory(newCategory, false);
            if (form.ShowDialog() == DialogResult.OK)
                RefreshTab();
        }


        /// <summary>
        /// Delete the currently selected user data category
        /// </summary>
        protected void DeleteUserDataCategory()
        {
            if (_activeItem == null)
                return;

            UserDataCategory userDataCategory = _activeItem.Tag as UserDataCategory;

            if (userDataCategory != null)
            {
                if (userDataCategory.Count == 0)
                {
                    if (MessageBox.Show("Are you sure that you want to delete this User Data Category?", "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) != DialogResult.Yes)
                        return;
                }
                else
                {
                    if (MessageBox.Show("Are you sure that you want to delete this User Data Category?  Deleting the category will also delete all User Data Fields defined within this category.", "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) != DialogResult.Yes)
                        return;
                }

                // Delete this user data category and any sub-types
                if (!userDataCategory.Delete())
                {
                    MessageBox.Show("Failed to delete the selected category.", "Delete Failed");
                }
                else
                {
                    UpdateCategoriesTabOrder();
                }
            }

            _activeItem = null;

            // We should still refresh as we have partially deleted the category
            RefreshTab();
        }


        /// <summary>
        /// Edit the currently selected user data category definition
        /// </summary>
        protected void EditUserDataCategory()
        {
            if (_activeItem != null)
            {
                UserDataCategory userDataCategory = _activeItem.Tag as UserDataCategory;
                
                FormUserDataCategory form = new FormUserDataCategory(userDataCategory, true);
                if (form.ShowDialog() == DialogResult.OK)
                    RefreshTab();
            }
        }

        #endregion User Data Category Add/Delete/Edit Functions

        #region User Data Field Add/Delete/Edit Functions


        /// <summary>
        /// Add a new User Data Field
        /// </summary>
        protected void AddUserDataField()
        {
            if (_activeItem != null)
            {
                UserDataCategory userDataCategory = _activeItem.Tag as UserDataCategory;
                UserDataField field = new UserDataField();
                field.Name = "<new field>";
                field.ParentID = userDataCategory.CategoryID;
                field.ParentScope = userDataCategory.Scope;

                // ...and display a foerm to create this new field
                FormUserDataField form = new FormUserDataField(userDataCategory, field, false);
                if (form.ShowDialog() == DialogResult.OK)
                    RefreshTab();
            }
        }


        /// <summary>
        /// Delete the selected user data field(s)
        /// </summary>
        protected void DeleteUserDataField()
        {
            if (ulvUserData.SelectedItems.Count == 0)
                return;

            // Confirm the deletion
            if (MessageBox.Show(
                "Are you sure that you want to delete this User Data Field? " + Environment.NewLine + Environment.NewLine +
                "Deleting the field will remove the definition and will delete all instances of this field from all assets.",
                "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) !=
                DialogResult.Yes)
                return;

            // Delete this user data field
            UltraListViewItem lvi = ulvUserData.SelectedItems[0];
            UserDataField field = lvi.Tag as UserDataField;

            if (field != null)
            {
                if (!field.Delete())
                {
                    MessageBox.Show("Failed to delete the selected User Data Field", "Delete Failed");
                }
                else
                {
                    if (_activeItem != null)
                    {
                        UserDataCategory userDataCategory = _activeItem.Tag as UserDataCategory;
                        if (userDataCategory != null)
                        {
                            userDataCategory.Remove(field);
                            RefreshList(userDataCategory);
                            UpdateFieldsTabOrder();
                        }
                    }
                }
            }
        }



        /// <summary>
        /// Called to edit the currently selected user data field definition
        /// </summary>
        protected void EditUserDataField()
        {
            if (ulvUserData.SelectedItems.Count == 0)
                return;

            // Get the currently selected user data field
            if (_activeItem != null)
            {
                UserDataCategory userDataCategory = _activeItem.Tag as UserDataCategory;
                UltraListViewItem lvi = ulvUserData.SelectedItems[0];
                UserDataField field = lvi.Tag as UserDataField;

                // ...and display it's properties
                FormUserDataField form = new FormUserDataField(userDataCategory, field, true);
                if (form.ShowDialog() == DialogResult.OK)
                    RefreshTab();
            }
        }

        #endregion User Data Field Add/Delete/Edit Functions


        /// <summary>
        /// Drop an item onto the Explorer Bar - items can only be UserDataFields
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void userDataExplorerBar_ItemDropped(object sender, ItemDroppedEventArgs e)
        {

        }

        private void userDataExplorerBar_DragDrop(object sender, DragEventArgs e)
        {
            UltraExplorerBar explorerBar = sender as UltraExplorerBar;
            OnDragEnd(explorerBar, false);
        }

        private void userDataExplorerBar_DragOver(object sender, DragEventArgs e)
        {
            UltraExplorerBar explorerBar = sender as UltraExplorerBar;
            Point clientPos = explorerBar.PointToClient(new Point(e.X, e.Y));

            if (dragExplorerItem != null)
            {
                dropExplorerItem = explorerBar.ItemFromPoint(clientPos);
                e.Effect = dropExplorerItem != null && dropExplorerItem != dragExplorerItem ? DragDropEffects.Move : DragDropEffects.None;
            }

            //  If the cursor is within {dragScrollAreaHeight} pixels
            //  of the top or bottom edges of the control, scroll
            int dragScrollAreaHeight = 8;

            Rectangle displayRect = explorerBar.DisplayRectangle;
            Rectangle topScrollArea = displayRect;
            topScrollArea.Height = (dragScrollAreaHeight * 2);

            Rectangle bottomScrollArea = displayRect;
            bottomScrollArea.Y = bottomScrollArea.Bottom - dragScrollAreaHeight;
            bottomScrollArea.Height = dragScrollAreaHeight;

            ISelectionManager selectionManager = explorerBar;
            if (topScrollArea.Contains(clientPos) || bottomScrollArea.Contains(clientPos))
                selectionManager.DoDragScrollVertical(0);
        }

        private void userDataExplorerBar_QueryContinueDrag(object sender, QueryContinueDragEventArgs e)
        {
            UltraExplorerBar explorerBar = sender as UltraExplorerBar;

            //  Cancel the drag operation if the escape key was pressed
            if (e.EscapePressed)
            {
                OnDragEnd(explorerBar, true);
                e.Action = DragAction.Cancel;
            }
        }

        private void userDataExplorerBar_MouseMove(object sender, MouseEventArgs e)
        {
            UltraExplorerBar explorerBar = sender as UltraExplorerBar;

            //  If the mouse has moved outside the area in which it was pressed,
            //  start a drag operation
            if (lastMouseDown.HasValue)
            {
                Size dragSize = SystemInformation.DragSize;
                Rectangle dragRect = new Rectangle(lastMouseDown.Value, dragSize);
                dragRect.X -= dragSize.Width / 2;
                dragRect.Y -= dragSize.Height / 2;

                if (!dragRect.Contains(e.Location))
                {
                    if (explorerBar != null)
                    {
                        UltraExplorerBarItem itemAtPoint = explorerBar.ItemFromPoint(e.Location);

                        if (itemAtPoint != null)
                        {
                            lastMouseDown = null;
                            dragExplorerItem = itemAtPoint;
                            explorerBar.DoDragDrop(dragExplorerItem, DragDropEffects.Move);
                        }
                    }
                }
            }
        }

        private void userDataExplorerBar_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
                lastMouseDown = e.Location;
            else
                lastMouseDown = null;
        }

        private void bnNewCategory_Click(object sender, EventArgs e)
        {
            AddUserDataCategory();
        }

        private void bnDeleteCategory_Click(object sender, EventArgs e)
        {
            DeleteUserDataCategory();
        }

        private void bnRemoveUserDataField_Click(object sender, EventArgs e)
        {
            DeleteUserDataField();
        }

        private void bnAddUserDataField_Click(object sender, EventArgs e)
        {
           AddUserDataField();
        }

        private void bnEditUserCategory_Click(object sender, EventArgs e)
        {
            EditUserDataCategory();
        }

        private void bnEditUserField_Click(object sender, EventArgs e)
        {
            EditUserDataField();
        }

        private void ulvUserData_ItemSelectionChanged(object sender, ItemSelectionChangedEventArgs e)
        {
            bnEditUserField.Enabled = (ulvUserData.SelectedItems.Count == 1);
        }
    }
}
