using System;
using System.Collections.Generic;
using System.ComponentModel;
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
using Infragistics.Win.UltraWinDataSource;
using Infragistics.Win.UltraWinGrid;
//
using Layton.AuditWizard.Common;
using Layton.AuditWizard.DataAccess;

namespace Layton.AuditWizard.Administration
{
    [SmartPart]
    public partial class SecurityTabView : UserControl, ILaytonView, IAdministrationView
    {
        private LaytonWorkItem workItem;
		SecurityTabViewPresenter presenter;

        [InjectionConstructor]
        public SecurityTabView([ServiceDependency] WorkItem workItem)
        {
            this.workItem = workItem as LaytonWorkItem;
            InitializeComponent();

			// Initialize the context menu for different products
#if !_AUDITWIZARD_
			this.contextMenuStrip1.Items[0].Visible = false;			// No New User
			this.contextMenuStrip1.Items[1].Visible = false;			// No Delete User
#endif
        }

		[CreateNew]
		public SecurityTabViewPresenter Presenter
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

			// Refresh the USERS tab first
			RefreshUsersTab();
		}


		/// <summary>
		/// Called as this tab is activated to ensure that we display the latest possible data
		/// This function comes from the IAdministrationView Interface
		/// </summary>
		public void Activate()
		{
			RefreshUsersTab();
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
		/// Called as we click to enable or disable security within AuditWizard
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void cbEnableSecurity_CheckedChanged(object sender, EventArgs e)
		{
			this.ultraGridUsers.Enabled = cbEnableSecurity.Checked;

			// Save the current security state to the database
			UsersDAO lwDataAccess = new UsersDAO();
			lwDataAccess.SecurityStatus(cbEnableSecurity.Checked);
		}
 

		/// <summary>
		/// Extra Initialization of the grid
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void ultraGridUsers_InitializeLayout(object sender, InitializeLayoutEventArgs e)
		{
			// Other miscellaneous settings
			// --------------------------------------------------------------------------------
			// Set the scroll style to immediate so the rows get scrolled immediately
			// when the vertical scrollbar thumb is dragged.
			//
			e.Layout.ScrollStyle = ScrollStyle.Immediate;

			// ScrollBounds of ScrollToFill will prevent the user from scrolling the
			// grid further down once the last row becomes fully visible.
			//
			e.Layout.ScrollBounds = ScrollBounds.ScrollToFill;

			//-----------------------------------------------------------------------------------
			//
			//    General Settings
			e.Layout.Override.CardAreaAppearance.BackColor = Color.Transparent;
			e.Layout.Override.CardCaptionAppearance.BackColorAlpha = Infragistics.Win.Alpha.UseAlphaLevel;
			e.Layout.Override.CardCaptionAppearance.AlphaLevel = 192;
			e.Layout.Override.RowAppearance.BackColorAlpha = Infragistics.Win.Alpha.Transparent;
			e.Layout.Override.CellAppearance.BackColorAlpha = Infragistics.Win.Alpha.UseAlphaLevel;
			e.Layout.Override.CellAppearance.AlphaLevel = 192;
			e.Layout.Override.HeaderAppearance.AlphaLevel = 192;
			e.Layout.Override.HeaderAppearance.BackColorAlpha = Infragistics.Win.Alpha.UseAlphaLevel;
            e.Layout.Override.CellClickAction = CellClickAction.RowSelect;
		}


		/// <summary>
		/// Called to refresh the information displayed on the users tab
		/// </summary>
		protected void RefreshUsersTab()
		{
			UsersDAO lwDataAccess = new UsersDAO();
			
			// Is security enabled
			bool enabled = lwDataAccess.SecurityStatus();
			this.cbEnableSecurity.Checked = enabled;

			// First clear the existing data held in the DataSource
			this.usersDataSet.Clear();

			// ...read the users defined in the database
			DataTable usersTable = lwDataAccess.GetUsers();
 
			// ...add these to the DataSource for the Users explorer bar
			foreach (DataRow thisRow in usersTable.Rows)
			{
				User thisUser = new User(thisRow);

				// Add the row to the data set
				usersDataSet.Tables[0].Rows.Add(new object[] 
				{ thisUser
					,thisUser.FirstName
					,thisUser.LastName
					,thisUser.AccessLevel
					,thisUser.Logon});
			}
		}


		#region Grid Control Handlers

		/// <summary>
		/// Called aw we attempt to delete one or more users from the database
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void ultraGrid1_BeforeRowsDeleted(object sender, Infragistics.Win.UltraWinGrid.BeforeRowsDeletedEventArgs e)
		{
            // Get the user being updated
			UltraGridRow updatedRow = e.Rows[0];
			User thisUser = (User)updatedRow.Cells[0].Value;			// Column 0 is the User Object
			DeleteUser(thisUser);
		}


		/// <summary>
		/// Double clicking will allow the user to be edited
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void ultraGridUsers_DoubleClickRow(object sender, DoubleClickRowEventArgs e)
		{
			EditUser();
		}

		
		
		/// <summary>
		/// Called before we update a cell - what we need toc heck here is that if the cell being
		/// updated is the access level that we do not lose the only administrator of the database
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void ultraGridUsers_BeforeCellUpdate(object sender, BeforeCellUpdateEventArgs e)
		{
			UsersDAO lwDataAccess = new UsersDAO();

			// Get the User being updated
			UltraGridRow updatedRow = e.Cell.Row;
			User user = (User)updatedRow.Cells[0].Value;				// Column 0 is the USER Object

			// Are we modifying the 'Access Level' Cell?
			if (e.Cell.Column.Key == "accesslevel")
			{
				if ((user.AccessLevel == User.ACCESSLEVEL.administrator)
				&& ((User.ACCESSLEVEL)updatedRow.Cells["accessLevel"].Value != User.ACCESSLEVEL.administrator))
				{
					int administratorCount = lwDataAccess.UserAdministratorCount();
					if (administratorCount <= 1)
					{
						MessageBox.Show("You cannot change the Access Level of this user as they are the only Administrator - please create another Administrator first", "Update Failed");
						e.Cancel = true;
					}
				}
			}
		}


		/// <summary>
		/// Called after we update a cell in the grid - we need to ensure that the underlying data source
		/// is also updated to reflect the change
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void ultraGridUsers_AfterCellUpdate(object sender, CellEventArgs e)
		{
			// Get the User being updated
			UltraGridRow updatedRow = e.Cell.Row;
			User user = (User)updatedRow.Cells[0].Value;				// Column 0 is the USER Object
			user.FirstName = (string)updatedRow.Cells["firstName"].Value;
			user.LastName = (string)updatedRow.Cells["lastName"].Value;
			user.AccessLevel = (User.ACCESSLEVEL)updatedRow.Cells["accessLevel"].Value;

			// ...and update the user in the database
			user.Update(null);
		}



		private void ultraGridUsers_KeyPress(object sender, KeyPressEventArgs e)
		{
			// Is this the <delete> key in which case we handle it as if the user has selected delete
			// from the context menu
			if (e.KeyChar == (char)Keys.Delete)
				deleteUserToolStripMenuItem_Click(sender, e);

				// Is this the <insert> key in which case we handle it as if the user has selected insert
			else if (e.KeyChar == (char)Keys.Delete)
				newUserToolStripMenuItem_Click(sender, e);
		}

		#endregion Grid Control Handlers

		#region Context Menu Handlers

		/// <summary>
		/// Invoke the 'Add New User' form
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void newUserToolStripMenuItem_Click(object sender, EventArgs e)
		{
			AddUser();
		}


		/// <summary>
		/// Called to delete the currently selected user
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void deleteUserToolStripMenuItem_Click(object sender, EventArgs e)
		{
			DeleteUser();
		}


		/// <summary>
		/// Called to edit the specified user
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void editUserToolStripMenuItem_Click(object sender, EventArgs e)
		{
			EditUser();
		}

		#endregion Context Menu Handlers

		public void AddUser()
		{
			User newUser = new User();
			newUser.Logon = "New User";
			//
			FormUserProperties addUserForm = new FormUserProperties(newUser);
			if (addUserForm.ShowDialog() == DialogResult.OK)
			{
				// add the new user to the DataSet
				usersDataSet.Tables[0].Rows.Add(new object[] 
				{ newUser
					,newUser.FirstName
					,newUser.LastName
					,newUser.AccessLevel
					,newUser.Logon});
				RefreshView();
			}
		}


		/// <summary>
		/// Called to allow the selected user to be edited
		/// </summary>
		public void EditUser()
		{
			// Ensure we have one and one only row selected
			if (this.ultraGridUsers.Selected.Rows.Count != 1)
				return;
			UltraGridRow selectedRow = this.ultraGridUsers.Selected.Rows[0];
			if (selectedRow.Cells[0].Value == null)
				return;
			//
			User thisUser = (User)selectedRow.Cells[0].Value;			// Column 0 is the User Object
			FormUserProperties form = new FormUserProperties(thisUser);
			if (form.ShowDialog() == DialogResult.OK)
				RefreshView();
		}



		/// <summary>
		/// Called to allow (any) selected user to be deleted
		/// </summary>
		public void DeleteUser()
		{
			// Ensure we have one and one only row selected
			if (this.ultraGridUsers.Selected.Rows.Count != 1)
				return;

			// Get the currently selected user in the grid
			UltraGridRow selectedRow = this.ultraGridUsers.Selected.Rows[0];
			if (selectedRow.Cells[0].Value == null)
				return;
			//
			User thisUser = (User)selectedRow.Cells[0].Value;			// Column 0 is the User Object
			DeleteUser(thisUser);
			RefreshView();
		}


		/// <summary>
		/// Delete a user from the database
		/// </summary>
		/// <param name="theUser"></param>
		private void DeleteUser(User theUser)
		{
			UsersDAO lwDataAccess = new UsersDAO();

			// Quick sanity check - ensure that if we are an Administrator and we are changing our access
			// level that we still have another administrator
			if (theUser.AccessLevel == User.ACCESSLEVEL.administrator)
			{
				int administratorCount = lwDataAccess.UserAdministratorCount();
				if (administratorCount <= 1)
					MessageBox.Show("You cannot delete this user as they are the only Administrator - please create another Administrator first", "Delete Failed");
			}

			// Delete this user after confirmation
			if (MessageBox.Show("Are you certain that you want to delete the user " + theUser.Logon + "?", "Please Confirm Delete", MessageBoxButtons.YesNo) == DialogResult.Yes)
				theUser.Delete();
		}
   }
}
