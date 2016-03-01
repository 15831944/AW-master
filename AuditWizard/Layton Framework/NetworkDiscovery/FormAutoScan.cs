using System;
using System.Windows.Forms;
using Layton.AuditWizard.Common;
using Layton.AuditWizard.DataAccess;

namespace Layton.NetworkDiscovery
{
    public partial class FormAutoScan : Form
    {
        private bool saveRequired;

        public FormAutoScan()
        {
            saveRequired = false;
            InitializeComponent();
        }

        private void bnApply_Click(object sender, EventArgs e)
        {
            SaveScanSettings();
            saveRequired = false;
            bnApply.Enabled = false;
        }

        private void cbEnableScan_CheckedChanged(object sender, EventArgs e)
        {
            lblScanInterval.Enabled = cbEnableScan.Checked;
            cbScanInterval.Enabled = cbEnableScan.Checked;
            tbScanInterval.Enabled = cbEnableScan.Checked;
            cbDeployAgent.Enabled = cbEnableScan.Checked;

            bnApply.Enabled = true;
        }

        private void SaveScanSettings()
        {
            SettingsDAO settingsDao = new SettingsDAO();

            settingsDao.SetSetting("AutoScanIntervalValue", tbScanInterval.Text, false);
            settingsDao.SetSetting("AutoScanNetwork", Convert.ToString(cbEnableScan.Checked), false);
            settingsDao.SetSetting("AutoScanDeployAgent", Convert.ToString(cbDeployAgent.Checked), false);
            settingsDao.SetSetting("AutoScanIntervalUnits", Convert.ToString(cbScanInterval.SelectedItem.ToString()), false);

            AuditWizardServiceController _serviceController = new Layton.AuditWizard.Common.AuditWizardServiceController();

            LaytonServiceController.ServiceStatus serviceStatus = _serviceController.CheckStatus();

            if (serviceStatus == LaytonServiceController.ServiceStatus.Running)
                _serviceController.RestartService();
            else
            {
                if (serviceStatus != LaytonServiceController.ServiceStatus.NotInstalled) _serviceController.Start();
            }

            DesktopAlert.ShowDesktopAlert("Settings have been updated.");
        }

        private void Form_Load(object sender, EventArgs e)
        {
            SettingsDAO settingDAO = new SettingsDAO();

            cbEnableScan.Checked = settingDAO.GetSettingAsBoolean("AutoScanNetwork", false);
            cbDeployAgent.Checked = settingDAO.GetSettingAsBoolean("AutoScanDeployAgent", false);

            string intervalUnits = settingDAO.GetSetting("AutoScanIntervalValue", false);
            tbScanInterval.Text = (intervalUnits == String.Empty) ? "1" : intervalUnits;

            string intervalPeriod = settingDAO.GetSetting("AutoScanIntervalUnits", false);
            cbScanInterval.SelectedItem = (intervalPeriod == String.Empty) ? "days" : intervalPeriod;

            bnApply.Enabled = false;
        }

        private void bnOK_Click(object sender, EventArgs e)
        {
            if (saveRequired)
                SaveScanSettings();

            Close();
        }

        private void bnCancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void tbScanInterval_TextChanged(object sender, EventArgs e)
        {
            saveRequired = true;
            bnApply.Enabled = true;
        }

        private void cbScanInterval_SelectedIndexChanged(object sender, EventArgs e)
        {
            saveRequired = true;
            bnApply.Enabled = true;
        }

        private void cbDeployAgent_CheckedChanged(object sender, EventArgs e)
        {
            saveRequired = true;
            bnApply.Enabled = true;
        }                    
    }
}
