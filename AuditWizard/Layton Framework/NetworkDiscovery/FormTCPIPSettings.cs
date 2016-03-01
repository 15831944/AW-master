using System;
using System.Data;
using System.Windows.Forms;
using Infragistics.Win.UltraWinListView;
using Layton.AuditWizard.DataAccess;

namespace Layton.NetworkDiscovery
{
    public partial class FormTcpipSettings : Form
    {
        public FormTcpipSettings()
        {
            InitializeComponent();
        }

        private void Form_Load(object sender, EventArgs e)
        {
            DataTable dt = new LayIpAddressDAO().SelectAllTcpIp();

            foreach (DataRow row in dt.Rows)
            {
                UltraListViewItem item = tcpipListView.Items.Add(row[0].ToString(), row[0].ToString());
                item.SubItems[0].Value = row[1].ToString();
                item.CheckState = (Convert.ToBoolean(row[2])) ? CheckState.Checked : CheckState.Unchecked;
            }
        }

        private void SaveChanges()
        {
            LayIpAddressDAO ipAddressDAO = new LayIpAddressDAO();

            // delete all existing before adding new ones
            ipAddressDAO.DeleteAllTcpIp();

            foreach (UltraListViewItem item in tcpipListView.Items)
            {
                IPAddressRange ipAddressRange = new IPAddressRange(item.Text, item.SubItems[0].Text, item.CheckState == CheckState.Checked, IPAddressRange.IPType.Tcpip);
                ipAddressDAO.Add(ipAddressRange);
            }
        }

        private void TcpipListViewSelectChanged(object sender, ItemSelectionChangedEventArgs e)
        {
            if (tcpipListView.SelectedItems.Count == 1)
            {
                bnEditIP.Enabled = true;
            }
        }

        private void bnAddIP_Click(object sender, EventArgs e)
        {
            IpAddressRangeForm ipForm = new IpAddressRangeForm();
            if (ipForm.ShowDialog() != DialogResult.OK) return;

            if (!tcpipListView.Items.Exists(ipForm.StartAddress))
            {
                UltraListViewItem item = tcpipListView.Items.Add(ipForm.StartAddress, ipForm.StartAddress);
                item.SubItems[0].Value = ipForm.EndAddress;
                item.CheckState = CheckState.Checked;
            }
            else
            {
                MessageBox.Show("Start IP address already exists. Range will not be added.", "Invalid IP Range", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
            }
        }

        private void bnRemoveIP_Click(object sender, EventArgs e)
        {
            while (tcpipListView.SelectedItems.Count > 0)
            {
                tcpipListView.Items.Remove(tcpipListView.SelectedItems[0]);
            }
        }

        private void bnEditIP_Click(object sender, EventArgs e)
        {
            if (tcpipListView.SelectedItems.Count == 1)
            {
                CheckState active = tcpipListView.SelectedItems[0].CheckState;

                IpAddressRangeForm ipForm = new IpAddressRangeForm(tcpipListView.SelectedItems[0].Value.ToString(), (string)tcpipListView.SelectedItems[0].SubItems[0].Value);
                if (ipForm.ShowDialog() == DialogResult.OK)
                {
                    UltraListViewItem selectedItem = tcpipListView.SelectedItems[0];
                    tcpipListView.Items.Remove(selectedItem);

                    if (!tcpipListView.Items.Exists(ipForm.StartAddress))
                    {
                        UltraListViewItem item = tcpipListView.Items.Add(ipForm.StartAddress, ipForm.StartAddress);
                        item.SubItems[0].Value = ipForm.EndAddress;
                        item.CheckState = active;
                    }
                    else
                    {
                        tcpipListView.Items.Add(selectedItem);
                        MessageBox.Show("Start IP address already exists. Range will not be added.", "Invalid IP Range", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                    }
                }
            }
            else if (tcpipListView.SelectedItems.Count > 1)
            {
                MessageBox.Show("Only one entry can edited at a time.", "AuditWizard", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show("Please select an IP range to edit.", "AuditWizard", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void bnCancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void bnOK_Click(object sender, EventArgs e)
        {
            SaveChanges();
            Close();
        }
    }
}