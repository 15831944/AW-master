using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Layton.AuditWizard.Common
{
    public partial class FormAWMessageBox : Form
    {
        public FormAWMessageBox(string aMessageText)
        {
            InitializeComponent();

            ultraLabelMessage.Text = aMessageText;
        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
