using System;
using System.Text;
using System.Windows.Forms;
using Layton.AuditWizard.DataAccess;

namespace Layton.NetworkDiscovery
{
    public partial class FormADSettings : Form
    {
        public FormADSettings()
        {
            InitializeComponent();
        }

        #region Helper Methods

        private void SaveSettings()
        {
            SettingsDAO lSettingsDAO = new SettingsDAO();
            lSettingsDAO.SetSetting("UseADCustomString", rbCustomLocation.Checked);

            if (rbCustomLocation.Checked)
            {
                StringBuilder sb = new StringBuilder();
                foreach (string communityString in lbReadStrings.Items)
                {
                    sb.Append(communityString + "|");
                }

                lSettingsDAO.SetSetting("ADCustomString", sb.ToString().TrimEnd('|'), false);
            }
        }

        #endregion

        #region Event Handlers

        private void Form_Load(object sender, EventArgs e)
        {
            SettingsDAO lSettingsDAO = new SettingsDAO();

            string adLocationStrings = lSettingsDAO.GetSettingAsString("ADCustomString", String.Empty);

            foreach (string adLocationString in adLocationStrings.Split('|'))
            {
                if (adLocationString != String.Empty)
                    lbReadStrings.Items.Add(adLocationString);
            }

            rbCustomLocation.Checked = lSettingsDAO.GetSettingAsBoolean("UseADCustomString", false);
            rbRootLocation.Checked = !rbCustomLocation.Checked;

            ugbCustomLocations.Enabled = rbCustomLocation.Checked;
        } 

        private void rbRootLocation_CheckedChanged(object sender, EventArgs e)
        {
            ugbCustomLocations.Enabled = rbCustomLocation.Checked;
            bnApply.Enabled = true;
        }        

        private void rbCustomLocation_CheckedChanged(object sender, EventArgs e)
        {
            ugbCustomLocations.Enabled = rbCustomLocation.Checked;
            bnApply.Enabled = true;
        }

        private void bnOK_Click(object sender, EventArgs e)
        {
            if (rbCustomLocation.Checked && lbReadStrings.Items.Count == 0)
            {
                MessageBox.Show(
                    "Please enter at least one custom location string or select 'Root Location' as your discovery type.", 
                    "AuditWizard", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            SaveSettings();
            Close();
        }

        private void bnApply_Click(object sender, EventArgs e)
        {
            bnApply.Enabled = false;
            SaveSettings();            
        }

        private void bnCancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void bnAddRead_Click(object sender, EventArgs e)
        {
            if (tbReadString.Text.Length > 0)
            {
                foreach (string communityString in lbReadStrings.Items)
                {
                    if (communityString == tbReadString.Text)
                    {
                        MessageBox.Show("The custom location string '" + tbReadString.Text + "' already exists in the list.", "AuditWizard", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return;
                    }
                }

                lbReadStrings.Items.Add(tbReadString.Text);
                tbReadString.Text = String.Empty;
                bnApply.Enabled = true;
            }
            else
            {
                MessageBox.Show("Please enter a value for the custom location string", "AuditWizard", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void bnRemoveRead_Click(object sender, EventArgs e)
        {
            lbReadStrings.Items.Remove(lbReadStrings.SelectedItem);
            bnApply.Enabled = true;
        }

        private void lbReadStrings_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lbReadStrings.SelectedItems.Count == 1)
            {
                bnRemoveRead.Enabled = true;
            }
        }

        #endregion
    }
}
