using System;
using System.Windows.Forms;
//
using Layton.AuditWizard.Common;
using Layton.AuditWizard.DataAccess;

namespace Layton.AuditWizard.Administration
{
	public partial class FormFtpSettingsBackup : Layton.Common.Controls.ShadedImageForm
	{
		AuditScannerDefinition _scannerConfiguration;

        public FormFtpSettingsBackup(AuditScannerDefinition scannerConfiguration)
		{
			InitializeComponent();
			_scannerConfiguration = scannerConfiguration;
		}


		/// <summary>
		/// Called as the form is being loaded to initialize all of the fields
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void FormFtpSettings_Load(object sender, EventArgs e)
		{
            cbFTPType.SelectedText = (_scannerConfiguration.FTPTypeBackup == null) ? "FTP" : _scannerConfiguration.FTPTypeBackup;
            tbFtpPort.Value = (_scannerConfiguration.FTPPortBackup == 0) ? 21 : _scannerConfiguration.FTPPortBackup;
            tbFtpSiteName.Text = _scannerConfiguration.FTPSiteBackup;
            tbFtpDefaultDirectory.Text = _scannerConfiguration.FTPDefDirBackup;
            if (string.IsNullOrEmpty(_scannerConfiguration.FTPUserBackup))
            {
                cbLoginAnonymous.Checked = true;
            }
            else
            {
                cbLoginAnonymous.Checked = _scannerConfiguration.FTPAnonymousBackup;
            }
            tbFtpUser.Text = _scannerConfiguration.FTPUserBackup;
            tbFtpPassword.Text = AES.DecryptFTPPassword(_scannerConfiguration.FTPPasswordBackup);
		}

		private void cbLoginAnonymous_CheckedChanged(object sender, EventArgs e)
		{
			panelFTPCredentials.Enabled = !cbLoginAnonymous.Checked;
            tbFtpUser.Clear();
            tbFtpPassword.Clear();
		}

		private void bnOK_Click(object sender, EventArgs e)
		{
			if (tbFtpSiteName.Text == "")
			{
				MessageBox.Show("You must specify at least the FTP Site Name or Address", "Validation Fail");
				tbFtpSiteName.Focus();
				DialogResult = DialogResult.None;
				return;
			}

			else if (!cbLoginAnonymous.Checked && tbFtpUser.Text == "")
			{
				MessageBox.Show("You must specify a username to log in to the FTP site with", "Validation Fail");
				tbFtpUser.Focus();
				DialogResult = DialogResult.None;
				return;
			}
            else if (!cbLoginAnonymous.Checked && tbFtpPassword.Text == "")
            {
                MessageBox.Show("You must specify a password to log in to the FTP site with", "Validation Fail");
                tbFtpPassword.Focus();
                DialogResult = DialogResult.None;
                return;
            }

            _scannerConfiguration.FTPTypeBackup = cbFTPType.Text;
            _scannerConfiguration.FTPSiteBackup = tbFtpSiteName.Text;
            _scannerConfiguration.FTPDefDirBackup = tbFtpDefaultDirectory.Text;
            _scannerConfiguration.FTPAnonymousBackup = cbLoginAnonymous.Checked;
            _scannerConfiguration.FTPUserBackup = tbFtpUser.Text;
            _scannerConfiguration.FTPPasswordBackup =  AES.EncryptFTPPassword(tbFtpPassword.Text);
            _scannerConfiguration.FTPPortBackup = (int)tbFtpPort.Value;
		}

        private void cbFTPType_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbFTPType.SelectedIndex == 0)
                tbFtpPort.Value = 21;
            else
                tbFtpPort.Value = 22;
        }
	}
}

