using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
//
using Infragistics.Win;
using Layton.AuditWizard.Common;
using Layton.Common.Controls;
using Layton.AuditWizard.DataAccess;

namespace Layton.AuditWizard.Administration
{
	public partial class FormUserProperties : ShadedImageForm
	{
		// The user added / edited
		User _theUser = null;

		public User CurrentUser
		{ get { return _theUser; } }

		private const string passwordPlaceHolder = "[dummy]";

		public FormUserProperties(User theUser)
		{
			InitializeComponent();
			_theUser = theUser;

			tbLogonName.Text = theUser.Logon;
			tbFirstName.Text = theUser.FirstName;
			tbLastName.Text = theUser.LastName;
			ValueListItem valuelistitem = uceAccessLevel.Items.ValueList.FindByDataValue((int)theUser.AccessLevel);
			if (valuelistitem != null)
				uceAccessLevel.SelectedIndex = uceAccessLevel.FindStringExact(valuelistitem.DisplayText);

			// Set the title as appropriate
			this.Text = (theUser.UserID == 0) ? "Add User" : "User Properties";

			// If adding a new user allow the user to set the name
			tbLogonName.Enabled = (theUser.UserID == 0);

			// ...and the placeholder for the password
			tbPassword.Text = passwordPlaceHolder;
		}


		private void bnOK_Click(object sender, EventArgs e)
		{
			UsersDAO lwDataAccess = new UsersDAO();

			// Validate the data entered - we must specify at least the logon
			if (tbLogonName.Text == "")
			{
				MessageBox.Show("You must specify the logon name", "Validation Error");
				tbLogonName.Focus();
				DialogResult = DialogResult.None;
				return;
			}

			// Ensure that the passwords entered match unless we still have the placeholder in which case
			// just ignore it
			if ((tbPassword.Text != passwordPlaceHolder) 
			&&  (tbPassword.Text != tbConfirmPassword.Text))
			{
				MessageBox.Show("Passwords entered do not match, please re-enter", "Validation Error");
				tbPassword.Focus();
				DialogResult = DialogResult.None;
				return;
			}

			// Update our internal user object with the data specified
			_theUser.Logon = tbLogonName.Text;
			_theUser.FirstName = tbFirstName.Text;
			_theUser.LastName = tbLastName.Text;
			_theUser.AccessLevel = (User.ACCESSLEVEL)uceAccessLevel.SelectedItem.DataValue;

			// OK - but if we are changing the logon name does this new user duplicate an existing one?
			if (_theUser.UserID == 0)
			{
				// ...read the users defined in the database
				DataTable usersTable = lwDataAccess.GetUsers();

				// ...and check through them
				foreach (DataRow dataRow in usersTable.Rows)
				{
					if ((string)dataRow["_LOGIN"] == tbLogonName.Text)
					{
						MessageBox.Show("This logon name has already been defined, please enter a different logon", "Validation Error");
						tbLogonName.Focus();
						DialogResult = DialogResult.None;
						return;
					}
				}

				// OK happy with the new user so may as well add to the database
				_theUser.Add();
			}

			else
			{
				_theUser.Update(null);
			}

			// ...and set the password for the user in case it has changed
			if (tbPassword.Text != passwordPlaceHolder)
				lwDataAccess.UserSetPassword(_theUser.UserID, tbPassword.Text);
		}
	}
}