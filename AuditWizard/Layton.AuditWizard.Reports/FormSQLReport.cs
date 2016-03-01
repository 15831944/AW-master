using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Layton.AuditWizard.DataAccess;
using Layton.AuditWizard.Common;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

namespace Layton.AuditWizard.Reports
{
    public partial class FormSQLReport : Form
    {
        private bool _reportSaved;
        private string _sqlString;

        public string SQLString
        {
            get { return _sqlString; }
            set { _sqlString = value; }
        }

        public string ReportName
        {
            get { return tbReportName.Text; }
        }

        public FormSQLReport(int aReportId)
        {
            InitializeComponent();

            DataTable reportsDataTable = new ReportsDAO().GetReportById(aReportId);
            tbReportName.Text = reportsDataTable.Rows[0][1].ToString();

            BinaryFormatter bf = new BinaryFormatter();
            MemoryStream mem = new MemoryStream(Convert.FromBase64String(reportsDataTable.Rows[0][2].ToString()));

            List<string> lSqlStringList = (List<string>)bf.Deserialize(mem);
            textBoxSQLString.Text = lSqlStringList[0];

            _reportSaved = true;
        }

        public FormSQLReport()
        {
            InitializeComponent();
            _reportSaved = false;
        }

        private void bnClose_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void bnOK_Click(object sender, EventArgs e)
        {
            if (!textBoxSQLString.Text.ToUpper().StartsWith("SELECT"))
            {
                MessageBox.Show(
                    "Only SELECT statements can be executed from this dialog.",
                    "SQL Query",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Exclamation);

                DialogResult = DialogResult.None;
                return;
            }

            if (!_reportSaved)
            {
                if (MessageBox.Show(
                    "The report has not yet been saved." + Environment.NewLine + Environment.NewLine +
                    "Do you wish to only run this report once without saving?",
                    "Run Report",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question) == DialogResult.No)
                {
                    DialogResult = DialogResult.None;
                    return;
                }
            }

            _sqlString = textBoxSQLString.Text;
            Close();
        }

        private void bnSave_Click(object sender, EventArgs e)
        {
            if (!textBoxSQLString.Text.ToUpper().StartsWith("SELECT"))
            {
                MessageBox.Show(
                    "Only SELECT statements can be executed from this dialog.",
                    "SQL Query",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Exclamation);
                return;
            }

            if (tbReportName.Text == "")
            {
                MessageBox.Show(
                    "Please enter a name for the SQL report.",
                    "Save SQL Report",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
                return;
            }

            if (textBoxSQLString.Text.Length == 0)
            {
                MessageBox.Show(
                    "Please enter a SQL string to run against the database.",
                    "Save SQL Report",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
                return;
            }

            List<string> _filterConditions = new List<string>();
            _filterConditions.Add(textBoxSQLString.Text);

            BinaryFormatter bf = new BinaryFormatter();
            MemoryStream mem = new MemoryStream();
            bf.Serialize(mem, _filterConditions);
            string lReportData = Convert.ToBase64String(mem.ToArray());

            DataTable lExistingReports = new ReportsDAO().GetReportsByTypeAndName(ReportsDAO.ReportType.SqlReport, tbReportName.Text);

            if (lExistingReports.Rows.Count > 0)
            {
                if (MessageBox.Show(
                    "A SQL report already exists with this name." + Environment.NewLine + Environment.NewLine +
                    "Do you wish to overwrite?",
                    "Save SQL Report",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    new ReportsDAO().Update(Convert.ToInt32(lExistingReports.Rows[0][0]), tbReportName.Text, lReportData);
                }
                else
                {
                    return;
                }
            }
            else
            {
                new ReportsDAO().Insert(tbReportName.Text, ReportsDAO.ReportType.SqlReport, lReportData);
            }

            DesktopAlert.ShowDesktopAlert(String.Format("The SQL report '{0}' has been saved.", tbReportName.Text));
            bnSave.Enabled = false;
            _reportSaved = true;
        }

        private void tbReportName_TextChanged(object sender, EventArgs e)
        {
            bnSave.Enabled = tbReportName.Text.Length > 0;
        }

        private void textBoxSQLString_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.A)
            {
                textBoxSQLString.SelectAll();
            }
            else if (e.Control && e.KeyCode == Keys.C)
            {
                textBoxSQLString.Copy();
            }
            else if (e.Control && e.KeyCode == Keys.X)
            {
                textBoxSQLString.Cut();
            }
        }

        private void textBoxSQLString_TextChanged(object sender, EventArgs e)
        {
            _reportSaved = false;
            bnOK.Enabled = textBoxSQLString.Text.Length > 0;
            bnSave.Enabled = textBoxSQLString.Text.Length > 0;
        }
    }
}
