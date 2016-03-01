using System;
using System.Collections.Specialized;
using System.Data;
using System.Text;
using System.Windows.Forms;
using Infragistics.Win.UltraWinListView;
using Layton.AuditWizard.DataAccess;

namespace Layton.NetworkDiscovery
{
    public partial class FormSNMPSettings : Form
    {
        public FormSNMPSettings()
        {
            InitializeComponent();
        }

        #region Methods

        private void SaveChanges()
        {
            SettingsDAO settingsDAO = new SettingsDAO();
            LayIpAddressDAO ipAddressDao = new LayIpAddressDAO();
            StringBuilder sb = new StringBuilder();

            // delete all existing before adding new ones
            ipAddressDao.DeleteAllSnmp();

            foreach (UltraListViewItem item in tcpipListView.Items)
            {
                IPAddressRange ipAddressRange = new IPAddressRange(item.Text, item.SubItems[0].Text, item.CheckState == CheckState.Checked, IPAddressRange.IPType.Snmp);
                ipAddressDao.Add(ipAddressRange);
            }

            foreach (string communityString in lbReadStrings.Items)
            {
                sb.Append(communityString + ",");
            }

            settingsDAO.SetSetting("SNMPRead", sb.ToString().TrimEnd(','), false);
        }

        #endregion

        #region Event Handlers

        private void tcpipListView_SelectChanged(object sender, ItemSelectionChangedEventArgs e)
        {
            if (tcpipListView.SelectedItems.Count == 1)
            {
                bnEditIP.Enabled = true;
            }
        }

        private void Form_Load(object sender, EventArgs e)
        {
            string snmpCommunityStrings = new SettingsDAO().GetSettingAsString("SNMPRead", String.Empty);

            foreach (string communityString in snmpCommunityStrings.Split(','))
            {
                if (communityString != String.Empty)
                    lbReadStrings.Items.Add(communityString);
            }

            DataTable dt = new LayIpAddressDAO().SelectAllSnmp();

            foreach (DataRow row in dt.Rows)
            {
                UltraListViewItem item = tcpipListView.Items.Add(row[0].ToString(), row[0].ToString());
                item.SubItems[0].Value = row[1].ToString();
                item.CheckState = (Convert.ToBoolean(row[2])) ? CheckState.Checked : CheckState.Unchecked;
            }
        }

        private void bnAddRead_Click(object sender, EventArgs e)
        {
            if (tbReadString.Text.Length > 0)
            {
                foreach (string communityString in lbReadStrings.Items)
                {
                    if (communityString == tbReadString.Text)
                    {
                        MessageBox.Show("The SNMP community string '" + tbReadString.Text + "' already exists in the list.", "AuditWizard", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return;
                    }
                }

                lbReadStrings.Items.Add(tbReadString.Text);
                tbReadString.Text = String.Empty;
            }
            else
            {
                MessageBox.Show("Please enter a value for the SNMP Read community string", "AuditWizard", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void lbReadStrings_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lbReadStrings.SelectedItems.Count == 1)
            {
                bnRemoveRead.Enabled = true;
            }
        }

        private void bnRemoveRead_Click(object sender, EventArgs e)
        {
            lbReadStrings.Items.Remove(lbReadStrings.SelectedItem);
        }

        private void bnAddIP_Click(object sender, EventArgs e)
        {
            IpAddressRangeForm ipForm = new IpAddressRangeForm();
            if (ipForm.ShowDialog() == DialogResult.OK)
            {
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

                IpAddressRangeForm ipForm = new IpAddressRangeForm(tcpipListView.SelectedItems[0].Key, (string)tcpipListView.SelectedItems[0].SubItems[0].Value);
                if (ipForm.ShowDialog() == DialogResult.OK)
                {
                    if (!tcpipListView.Items.Exists(ipForm.StartAddress))
                    {
                        tcpipListView.Items.Remove(tcpipListView.SelectedItems[0]);

                        UltraListViewItem item = tcpipListView.Items.Add(ipForm.StartAddress, ipForm.StartAddress);
                        item.SubItems[0].Value = ipForm.EndAddress;
                        item.CheckState = active;
                    }
                    else
                    {
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

        #endregion
    }
}
