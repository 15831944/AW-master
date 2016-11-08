using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
//
using Layton.Common.Controls;

namespace Layton.AuditWizard.Common
{
    public partial class EmailSettingsForm : ShadedImageForm
    {
        public EmailSettingsForm()
        {
            InitializeComponent();
        }

        private void authCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            userTextBox.Enabled = authCheckBox.Checked;
            passTextBox.Enabled = authCheckBox.Checked;
        }

        public int Port
        {
            get { return Convert.ToInt32(portTextBox.Text); }
            set { portTextBox.Text = value.ToString(); }
        }

        public bool UseAuthentication
        {
            get { return authCheckBox.Checked; }
            set { authCheckBox.Checked = value; }
        }

        public string Username
        {
            get { return userTextBox.Text; }
            set { userTextBox.Text = value; }
        }

        public string Password
        {
            get { return passTextBox.Text; }
            set { passTextBox.Text = value; }
        }

        /// <summary>
        /// Added for ID 66125/66652
        /// </summary>
        public bool EnabledSSL
        {
            get { return sSLCheckBox.Checked; }
            set { sSLCheckBox.Checked = value; }
        }
    }
}