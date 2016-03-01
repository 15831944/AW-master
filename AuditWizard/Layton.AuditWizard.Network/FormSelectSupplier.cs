using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Layton.AuditWizard.DataAccess;

namespace Layton.AuditWizard.Network
{
    public partial class FormSelectSupplier : Form
    {
        private string _supplierName;

        public FormSelectSupplier()
        {
            InitializeComponent();
        }

        public string SupplierName
        {
            get { return _supplierName; }
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lbSuppliers.SelectedItems.Count > 0)
                bnOK.Enabled = true;
        }

        private void Form_Load(object sender, EventArgs e)
        {
            DataTable suppliersTable = new SuppliersDAO().EnumerateSuppliers();
            lbSuppliers.Items.Clear();

            foreach (DataRow row in suppliersTable.Rows)
            {
                lbSuppliers.Items.Add(new Supplier(row).Name);
            }
        }

        private void bnOK_Click(object sender, EventArgs e)
        {
            _supplierName = lbSuppliers.SelectedItem.ToString(); 
            Close();
        }

        private void bnCancel_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
