using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Layton.AuditWizard.Common
{
    public partial class FormTaskName : Form
    {
        private string _subject;
        private string _description;

        public string Subject { get { return _subject; } }
        public string Description { get { return _description; } }

        public FormTaskName()
        {
            InitializeComponent();
        }

        private void bnCancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void bnOK_Click(object sender, EventArgs e)
        {
            _subject = tbSubject.Text;
            _description = tbDescription.Text;
            Close();
        }

        private void tbSubject_TextChanged(object sender, EventArgs e)
        {
            bnOK.Enabled = tbSubject.Text.Length > 0;
            tbDescription.Enabled = tbSubject.Text.Length > 0;
        }
    }
}
