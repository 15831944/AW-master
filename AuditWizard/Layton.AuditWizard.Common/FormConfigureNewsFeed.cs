using System;
using System.Windows.Forms;
using Layton.AuditWizard.DataAccess;

namespace Layton.AuditWizard.Common
{
    public partial class FormConfigureNewsFeed : Form
    {
        public FormConfigureNewsFeed()
        {
            InitializeComponent();
        }

        private void tbDiskSpace_Scroll(object sender, EventArgs e)
        {
            lbDiskSpace.Text = String.Format("Less Than {0}% Disk Space Remaining", tbDiskSpace.Value);
        }

        private void tbLicenses_Scroll(object sender, EventArgs e)
        {
            lbLicenses.Text = String.Format("Greater Than {0}% Software Licenses Used", tbLicenses.Value);
        }

        private void tbPrinterSupplies_Scroll(object sender, EventArgs e)
        {
            lbPrinterSupplies.Text = String.Format("Less Than {0}% Printer Supplies Remaining", tbPrinterSupplies.Value);
        }

        private void bnCancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void bnSave_Click(object sender, EventArgs e)
        {
            SettingsDAO lSettingsDAO = new SettingsDAO();

            lSettingsDAO.SetSetting("NewsFeedDiskSpace", tbDiskSpace.Value.ToString(), false);
            lSettingsDAO.SetSetting("NewsFeedLicenses", tbLicenses.Value.ToString(), false);
            lSettingsDAO.SetSetting("NewsFeedPrinters", tbPrinterSupplies.Value.ToString(), false);
            lSettingsDAO.SetSetting("NewsFeedUpdateAsset", cbAssetUpdated.Checked);

            Close();
        }

        private void FormConfigureNewsFeed_Load(object sender, EventArgs e)
        {
            SettingsDAO lSettingsDAO = new SettingsDAO();
            cbAssetUpdated.Checked = lSettingsDAO.GetSettingAsBoolean("NewsFeedUpdateAsset", false);

            tbDiskSpace.Value = lSettingsDAO.GetSettingAsString("NewsFeedDiskSpace", "25") == String.Empty ? 25 : Convert.ToInt32(lSettingsDAO.GetSettingAsString("NewsFeedDiskSpace", "25"));
            lbDiskSpace.Text = String.Format("Less Than {0}% Disk Space Remaining", tbDiskSpace.Value);

            tbLicenses.Value = lSettingsDAO.GetSettingAsString("NewsFeedLicenses", "100") == String.Empty ? 100 : Convert.ToInt32(lSettingsDAO.GetSettingAsString("NewsFeedLicenses", "100"));
            lbLicenses.Text = String.Format("Greater Than {0}% Software Licenses Used", tbLicenses.Value);

            tbPrinterSupplies.Value = lSettingsDAO.GetSettingAsString("NewsFeedPrinters", "25") == String.Empty ? 25 : Convert.ToInt32(lSettingsDAO.GetSettingAsString("NewsFeedPrinters", "25"));
            lbPrinterSupplies.Text = String.Format("Less Than {0}% Printer Supplies Remaining", tbPrinterSupplies.Value);
        }

        private void tbDays_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Back || e.KeyChar == (char)Keys.Delete)
                e.Handled = false;

            else if (!System.Text.RegularExpressions.Regex.IsMatch(e.KeyChar.ToString(), "\\d+"))
                e.Handled = true;
        }
    }
}
