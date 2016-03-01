using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Layton.Common.Controls
{
    public partial class FormSetPassword : Form
    {
        public FormSetPassword()
        {
            InitializeComponent();
        }

        public string Password
        {
            get { return tbPassword.Text; }
        }

        private void bnOK_Click(object sender, EventArgs e)
        {
            // Ensure that the password and confirmation agree and display an error if not
            if (tbPassword.Text != tbConfirm.Text)
            {
                MessageBox.Show("Passwords entered do not match, please re-enter", "Password Mis-match", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                tbPassword.Clear();
                tbConfirm.Clear();
                tbPassword.Focus();
                return;
            }

            DialogResult = DialogResult.OK;
            return;
        }
    }
}