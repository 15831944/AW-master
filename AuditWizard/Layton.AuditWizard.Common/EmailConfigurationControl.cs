using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
//
using Layton.AuditWizard.DataAccess;

namespace Layton.AuditWizard.Common
{
	public partial class EmailConfigurationControl : UserControl
	{

		// Email data
		private EmailSettingsForm emailForm = new EmailSettingsForm();
		private bool doSaveAdvancedSettings;

		public EmailConfigurationControl()
		{
			InitializeComponent();
		}



		/// <summary>
		/// Perform initialization of fields on the Email tab
		/// </summary>
		public void InitializeEmailSettings()
		{
            SettingsDAO lSettingsDao = new SettingsDAO();

            tbSendingEmailAddress.Text = lSettingsDao.GetSetting(MailSettingsKeys.MailSender, false);
            tbRecipientEmailAddress.Text = lSettingsDao.GetSetting(MailSettingsKeys.MailAddress, false);
            smtpTextBox.Text = lSettingsDao.GetSetting(MailSettingsKeys.MailServer, false);

            string strPort = lSettingsDao.GetSetting(MailSettingsKeys.MailPort, false);
            int port = 0;

            if (strPort != String.Empty)
                port = Convert.ToInt32(strPort);

			if (port != 0)
				emailForm.Port = port;

            emailForm.UseAuthentication = lSettingsDao.GetSettingAsBoolean(MailSettingsKeys.MailRequiresAuthentication, false);
			if (emailForm.UseAuthentication)
			{
                emailForm.Username = lSettingsDao.GetSetting(MailSettingsKeys.MailUserName, false);
                emailForm.Password = lSettingsDao.GetSetting(MailSettingsKeys.MailPassword, true);
			}

			// Set the mail frequency
            string mailFrequency = lSettingsDao.GetSetting(MailSettingsKeys.MailFrequency, false);
			
            switch (mailFrequency)
            {
                case MailFrequencyValues.Daily:
                    dailyRadioButton.Checked = true;
                    break;
                case MailFrequencyValues.Weekly:
                    weeklyRadioButton.Checked = true;
                    break;
                case MailFrequencyValues.Monthly:
                    monthlyRadioButton.Checked = true;
                    break;
                default:
                    neverRadioButton.Checked = true;
                    break;
            }

            // email type
            if (lSettingsDao.GetSettingAsBoolean("EmailAsHtml", true))
                rbHtml.Checked = true;
            else
                rbText.Checked = true;
		    

			// Check if the settings are valid
			CheckEmailConfiguration();
		}

		/// <summary>
		/// Save any data entered on the Options tab
		/// </summary>
		public void SaveEmailSettings()
		{
			// Save the settings
			SettingsDAO lSettingsDao = new SettingsDAO();
			if (doSaveAdvancedSettings)
			{
				lSettingsDao.SetSetting(MailSettingsKeys.MailRequiresAuthentication, emailForm.UseAuthentication.ToString(), false);
				lSettingsDao.SetSetting(MailSettingsKeys.MailPort, emailForm.Port.ToString(), false);
				if (emailForm.UseAuthentication)
				{
					lSettingsDao.SetSetting(MailSettingsKeys.MailUserName, emailForm.Username, false);
					lSettingsDao.SetSetting(MailSettingsKeys.MailPassword, emailForm.Password, true);
				}

                lSettingsDao.SetSetting(MailSettingsKeys.MailSSLEnabled, emailForm.EnabledSSL.ToString(), false); // Added for ID 66125/66652
			}

			// Save general email settings
			lSettingsDao.SetSetting(MailSettingsKeys.MailSender, tbSendingEmailAddress.Text, false);
			lSettingsDao.SetSetting(MailSettingsKeys.MailServer, smtpTextBox.Text, false);
			lSettingsDao.SetSetting(MailSettingsKeys.MailAddress, tbRecipientEmailAddress.Text, false);
            lSettingsDao.SetSetting("EmailAsHtml", rbHtml.Checked);

			if (dailyRadioButton.Checked)
				lSettingsDao.SetSetting(MailSettingsKeys.MailFrequency, MailFrequencyValues.Daily, false);
			else if (weeklyRadioButton.Checked)
				lSettingsDao.SetSetting(MailSettingsKeys.MailFrequency, MailFrequencyValues.Weekly, false);
			else if (monthlyRadioButton.Checked)
				lSettingsDao.SetSetting(MailSettingsKeys.MailFrequency, MailFrequencyValues.Monthly, false);
			else
				lSettingsDao.SetSetting(MailSettingsKeys.MailFrequency, MailFrequencyValues.Never, false);
		}

		/// <summary>
		/// Called as we click on the Email Advanced button
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void advancedButton_Click(object sender, EventArgs e)
		{
			if (emailForm.ShowDialog() == DialogResult.OK)
			{
				// mark the settings to be saved
				doSaveAdvancedSettings = true;
			}
		}


		/// <summary>
		/// Called to send a test email using the credential specified
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void emailButton_Click(object sender, EventArgs e)
		{
			try
			{
				SaveEmailSettings();
				EmailController emailer = new EmailController();
				emailer.SendStatusEmail(true ,true ,null);
			    DesktopAlert.ShowDesktopAlert("An email has been sent to the specified address.");
				//MessageBox.Show("An email has been sent to the specified address.", "AuditWizard", MessageBoxButtons.OK, MessageBoxIcon.Information);
			}
			catch (Exception ex)
			{
				MessageBox.Show("Error sending email:  " + ex.Message, "Error Sending Sample Email", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		private void tbSendingEmailAddress_TextChanged(object sender, EventArgs e)
		{
			CheckEmailConfiguration();
		}


		private void tbRecipientEmailAddress_TextChanged(object sender, EventArgs e)
		{
			CheckEmailConfiguration();
		}

		private void smtpTextBox_TextChanged(object sender, EventArgs e)
		{
			CheckEmailConfiguration();
		}

		/// <summary>
		/// Only allow send mail when the email configuration has been set correctly
		/// </summary>
		private void CheckEmailConfiguration()
		{
			if ((tbRecipientEmailAddress.Text == "")
			|| (tbSendingEmailAddress.Text == "")
			|| (smtpTextBox.Text == ""))
				bnSendEmail.Enabled = false;
			else
				bnSendEmail.Enabled = true;
		}

        private void bnSaveEmailSettings_Click(object sender, EventArgs e)
        {
            SaveEmailSettings();
            DesktopAlert.ShowDesktopAlert("Email settings saved okay.");
        }

        private void EmailConfigurationControl_Leave(object sender, EventArgs e)
        {
            SaveEmailSettings();
        }
	}
}
