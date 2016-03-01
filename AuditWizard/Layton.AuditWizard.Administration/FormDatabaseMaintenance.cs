using System;
using System.IO;
using System.Windows.Forms;
using Layton.AuditWizard.DataAccess;

namespace Layton.AuditWizard.Administration
{
    public partial class FormDatabaseMaintenance : Form
    {
        public FormDatabaseMaintenance()
        {
            InitializeComponent();
        }

        private void bnOk_Click(object sender, EventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;
            string lMsg = new DatabaseMaintenanceDAO().RunManintenanceScript(tbSql.Text.Replace(Environment.NewLine, " "));
            Cursor.Current = Cursors.Default;

            MessageBox.Show(lMsg, "Database Maintenance");
        }

        private void tbSql_TextChanged(object sender, EventArgs e)
        {
            bnOk.Enabled = tbSql.Text.Length > 0;
        }

        private void bnCancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void tbSql_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.A)
            {
                tbSql.SelectAll();
            }
        }

        private void bnLoadSql_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.InitialDirectory = "c:\\";
            openFileDialog.Filter = "SQL files (*.sql)|*.sql";
            openFileDialog.FilterIndex = 2;
            openFileDialog.RestoreDirectory = true;

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    string file = openFileDialog.FileName;
                    tbSql.Text = File.ReadAllText(file);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: Could not read file from disk. Original error: " + ex.Message);
                }
            }

        }
    }
}
