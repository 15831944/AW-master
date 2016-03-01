using System;
using System.Windows.Forms;
using Layton.AuditWizard.Common;
//
using Layton.AuditWizard.DataAccess;

namespace Layton.AuditWizard.Administration
{
	public partial class FormFtpSettings : Layton.Common.Controls.ShadedImageForm
	{
		AuditScannerDefinition _scannerConfiguration;

		public FormFtpSettings(AuditScannerDefinition scannerConfiguration)
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
            cbFTPType.SelectedText = (_scannerConfiguration.FTPType == null) ? "FTP" : _scannerConfiguration.FTPType;
            tbFtpPort.Value = (_scannerConfiguration.FTPPort == 0) ? 21 : _scannerConfiguration.FTPPort;
			tbFtpSiteName.Text = _scannerConfiguration.FTPSite;
			tbFtpDefaultDirectory.Text = _scannerConfiguration.FTPDefDir;
            if (string.IsNullOrEmpty(_scannerConfiguration.FTPUser))
            {
                cbLoginAnonymous.Checked = true;
            }
            else
            {
                cbLoginAnonymous.Checked = _scannerConfiguration.FTPAnonymous;
            }
			tbFtpUser.Text = _scannerConfiguration.FTPUser;
			tbFtpPassword.Text = AES.DecryptFTPPassword(_scannerConfiguration.FTPPassword);
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

            _scannerConfiguration.FTPType = cbFTPType.Text;
			_scannerConfiguration.FTPSite = tbFtpSiteName.Text;
			_scannerConfiguration.FTPDefDir = tbFtpDefaultDirectory.Text;
			_scannerConfiguration.FTPAnonymous = cbLoginAnonymous.Checked;
			_scannerConfiguration.FTPUser = tbFtpUser.Text;
			_scannerConfiguration.FTPPassword = AES.EncryptFTPPassword(tbFtpPassword.Text);
			_scannerConfiguration.FTPPort = (int)tbFtpPort.Value;
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

