using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
//
using Layton.AuditWizard.Common;
using Layton.Common.Controls;
using Layton.AuditWizard.DataAccess;

namespace Layton.AuditWizard.Overview
{
	public partial class FormLogin : ShadedImageForm
	{
		private User _loggedInUser = null;        

		public User LoggedInUser
		{
			get { return _loggedInUser; }
		}

		public FormLogin()
		{
			InitializeComponent();
		}

		/// <summary>
		/// Called as we try to OK out of this form - validate the password and do not allow the 
		/// user to exit normally if it is invalid
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void bnOK_Click(object sender, EventArgs e)
		{
            UsersDAO lwDataAccess = new UsersDAO();
			_loggedInUser = lwDataAccess.UserCheckPassword(tbUsername.Text, tbPassword.Text);
			if (_loggedInUser == null)
			{
				MessageBox.Show("Invalid password entered, please re-enter", "Password Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
				DialogResult = DialogResult.None;
			}
		}

        private void FormLogin_Load(object sender, EventArgs e)
        {
            Application.UseWaitCursor = false;
        }
	}
}

