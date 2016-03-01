using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Layton.Common.Controls;

namespace Layton.AuditWizard.Applications
{
    public partial class FormEditPublisherName : AWForm
    {
        private string _publisherName;

        public string PublisherName
        {
            get
            {
                return _publisherName;
            }
        }

        public FormEditPublisherName(string publisherName)
        {
            InitializeComponent();
            textBox1.Text = publisherName;
        }

        private void Cancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void bnOK_Click(object sender, EventArgs e)
        {
            _publisherName = textBox1.Text;
            Close();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            bnOK.Enabled = textBox1.Text.Length > 0;
        }
    }
}
