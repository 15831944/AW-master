using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Layton.AuditWizard.DataAccess;

namespace Layton.AuditWizard.Reports
{
    public partial class FormSaveCustomReportName : Form
    {
        private string _reportName;
        private bool _cancelPressed;

        public string ReportName
        { get { return _reportName; } }

        public FormSaveCustomReportName()
        {
            InitializeComponent();
            _reportName = "";
            _cancelPressed = false;
        }

        private void bnCancel_Click(object sender, EventArgs e)
        {
            _cancelPressed = true;
            Close();
        }

        private void SaveForm()
        {
            DataTable lExistingReports = new ReportsDAO().GetReportsByTypeAndName(ReportsDAO.ReportType.CustomReport, textBox1.Text);

            if (lExistingReports.Rows.Count > 0)
            {
                MessageBox.Show(
                    "A custom report already exists with the name '" + textBox1.Text + "'.",
                    "Save Custom Report",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);

                return;
            }
            else
            {
                _reportName = textBox1.Text;
            }

            Close();
        }

        private void bnSave_Click(object sender, EventArgs e)
        {
            SaveForm(); 
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            bnSave.Enabled = (textBox1.Text.Length > 0);
        }

        private void FormSaveCustomReportName_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!_cancelPressed)
            {
                if (_reportName == "")
                    e.Cancel = true;
            }
        }
    }
}
