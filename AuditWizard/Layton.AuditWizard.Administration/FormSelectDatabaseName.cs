using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using System.Data.Sql;

namespace Layton.AuditWizard.Administration
{
    public partial class FormSelectDatabaseName : Form
    {
        public string SelectedDatabaseName { get; set; }

        public FormSelectDatabaseName()
        {
            InitializeComponent();
        }

        private void lbDatabases_Load(object sender, EventArgs e)
        {
            SqlDataSourceEnumerator instance = SqlDataSourceEnumerator.Instance;
            System.Data.DataTable table = instance.GetDataSources();

            foreach (DataRow row in table.Rows)
            {
                if (row[1].ToString() == String.Empty)
                    lbDatabases.Items.Add(row[0].ToString());
                else
                    lbDatabases.Items.Add(row[0].ToString() + @"\" + row[1].ToString());
            }

            lbDatabases.Sorted = true;
            
        }

        private void bnOK_Click(object sender, EventArgs e)
        {
            this.SelectedDatabaseName = lbDatabases.SelectedItem.ToString();
            Close();
        }

        private void lbDatabases_SelectedIndexChanged(object sender, EventArgs e)
        {
            bnOK.Enabled = true;
        }

        private void bnCancel_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
