using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Layton.AuditWizard.DataAccess;

namespace AuditWizardv8
{
    public partial class FormSupportExpiry : Form
    {
        private string _messageText;

        public FormSupportExpiry(string aMessageText)
        {
            InitializeComponent();
            _messageText = aMessageText;
        }

        private void FormSupportExpiry_Load(object sender, EventArgs e)
        {
            labelMessageText.Text = _messageText;
        }

        private void FormSupportExpiry_FormClosing(object sender, FormClosingEventArgs e)
        {
            new SettingsDAO().SetSetting("NotifySupport", !checkBoxNotify.Checked);
        }
    }
}
