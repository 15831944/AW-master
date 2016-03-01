using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.IO;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using PickerSample;
//
using Layton.AuditWizard.Common;
using Layton.Common.Controls;
using Layton.AuditWizard.DataAccess;

namespace Layton.AuditWizard.Common
{
	public partial class FormAuditWizardServiceControl : ShadedImageForm
	{
		// The service controller object
		private AuditWizardServiceController _serviceController = new AuditWizardServiceController();
			
		public FormAuditWizardServiceControl()
		{
			InitializeComponent();

			// Load the initial settings
			// Set the initial status of the 'RunAs' fields
			Configuration config = ConfigurationManager.OpenExeConfiguration(Path.Combine(Application.StartupPath, "AuditWizardv8.exe"));
			cbLocalSystem.Checked = (config.AppSettings.Settings["AuditWizardservice_runassystem"].Value == "True");
			if (!cbLocalSystem.Checked)
			{
				tbUsername.Text = config.AppSettings.Settings["AuditWizardservice_username"].Value;

				// Set the password noting that it is encrypted
				string password = config.AppSettings.Settings["AuditWizardservice_password"].Value;
				if (password == "")
					tbPassword.Text = "";
				else
					tbPassword.Text = AES.Decrypt(config.AppSettings.Settings["AuditWizardservice_password"].Value);
				tbConfirmPassword.Text = tbPassword.Text;
			}

			// Set display states
			SetDisplayStates();
		}

		private void cbLocalSystem_CheckedChanged(object sender, EventArgs e)
		{
			panelRunSystemAs.Enabled = !cbLocalSystem.Checked;
		}


		/// <summary>
		/// Set the state of the various buttons and other fields on this form based on the 
		/// current state of the service
		/// </summary>
		/// <param name="serviceStatus"></param>
		private void SetDisplayStates()
		{
			// Get the current status of the service
			LaytonServiceController.ServiceStatus serviceStatus = _serviceController.CheckStatus();

			// Select the appropriate status image to show
			switch (serviceStatus)
			{
				case LaytonServiceController.ServiceStatus.Running:
					pbServiceStatus.Image = Properties.Resources.active;
					break;
				case LaytonServiceController.ServiceStatus.Stopped:
					pbServiceStatus.Image = Properties.Resources.stopped;
					break;
				case LaytonServiceController.ServiceStatus.NotInstalled:
					pbServiceStatus.Image = Properties.Resources.notinstalled;
					break;
				default:
					pbServiceStatus.Image = Properties.Resources.unavailable;
					break;
			}

			// The start button is valid so long as the service is currently in a 'stopped' or 
			// 'not installed' state
			bnStart.Enabled = ((serviceStatus == LaytonServiceController.ServiceStatus.NotInstalled)
							|| (serviceStatus == LaytonServiceController.ServiceStatus.Stopped));

			// Stop is enabled if the service is currently in a 'Running' State
			bnStop.Enabled = (serviceStatus == LaytonServiceController.ServiceStatus.Running);

			// Remove is enabled if the service is stopped
			bnRemove.Enabled = (serviceStatus == LaytonServiceController.ServiceStatus.Stopped);

			// We cannot change the logon credentials UNLESS the service is stopped 
			panelLogonCredentials.Enabled = (serviceStatus == LaytonServiceController.ServiceStatus.NotInstalled);
		}

		#region Button Handlers


		/// <summary>
		/// Start the service - note that if it is currently not installed we will need to install
		/// it first and then start it.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void bnStart_Click(object sender, EventArgs e)
		{
			// Get the current status of the Service
			LaytonServiceController.ServiceStatus serviceStatus = _serviceController.CheckStatus();

			// Update the configuration both locally and in the service controller
			if (SaveConfiguration())
			{
				_serviceController.ResetCredentials();

				using (new WaitCursor())
				{
					// Now install the service (Under the specified username / password) unless it is already
					// installed
					try
					{
						if (serviceStatus == LaytonServiceController.ServiceStatus.NotInstalled)
							_serviceController.Install();
					}
					catch (Exception ex)
					{
						MessageBox.Show(String.Format("Failed to install the AuditWizard Service, the error was {0}", ex.Message), "Service Control Error");
						return;
					}

					// OK now start the service
					try
					{
						_serviceController.Start();
					}
					catch (Exception ex)
					{
						MessageBox.Show(String.Format("Failed to start the AuditWizard Service, the error was {0}", ex.Message), "Service Control Error");
						return;
					}

					// Now wait a few seconds and recover the latest status
					Thread.Sleep(2000);

					// ...and update the display
					SetDisplayStates();
				}
			}
		}


		/// <summary>
		/// Called to stop the AuditWizard service
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void bnStop_Click(object sender, EventArgs e)
		{
			_serviceController.ResetCredentials();

			using (new WaitCursor())
			{
				// Stop the service
				try
				{
					_serviceController.Stop();
				}
				catch (Exception ex)
				{
					MessageBox.Show(String.Format("Failed to stop the AuditWizard Service, the error is: \n{0}", ex.Message), "Service Control Error");
					return;
				}
				// Now wait a few seconds and recover the latest status
				Thread.Sleep(2000);

				// ...and update the display
				SetDisplayStates();
			}
		}

		private void bnRemove_Click(object sender, EventArgs e)
		{
			// Update the configuration both locally and in the service controller
			_serviceController.ResetCredentials();

			using (new WaitCursor())
			{							// Stop the service
				try
				{
					_serviceController.Remove();
				}
				catch (Exception ex)
				{
					MessageBox.Show(String.Format("Failed to Remove the AuditWizard Service, the error was {0}", ex.Message), "Service Control Error");
					return;
				}

				// Now wait a few seconds and recover the latest status
				Thread.Sleep(2000);

				// ...and update the display
				SetDisplayStates();
			}
		}


		/// <summary>
		/// View the AuditWizard service log file
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void bnViewLog_Click(object sender, EventArgs e)
		{
			string destinationFile = Path.Combine(Application.StartupPath + "\\logs" ,AuditWizardServiceController.AuditWizardServiceLog);

			// Does the file exist?
			if (File.Exists(destinationFile))
				System.Diagnostics.Process.Start(destinationFile);
			else
				MessageBox.Show("The AuditWizard Service Log File [" + destinationFile + "] could not be found, please ensure that the service started successfully", "No Such File");
		}


		/// <summary>
		/// Called as we exit from the service control form - save any changed values back to the 
		/// Application settings file
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void bnOK_Click(object sender, EventArgs e)
		{
			// Do not exit if there is an error
			if (!SaveConfiguration())
				DialogResult = DialogResult.None;				
		}



		/// <summary>
		/// Validate the configuration before doing anything with it and save it
		/// </summary>
		/// <returns></returns>
		private bool SaveConfiguration()
		{
			// Set the initial status of the 'RunAs' fields
			Configuration config = ConfigurationManager.OpenExeConfiguration(Path.Combine(Application.StartupPath, "AuditWizardv8.exe"));
			config.AppSettings.Settings["AuditWizardservice_runassystem"].Value = (cbLocalSystem.Checked) ? "True" : "False";

			// If we are not using the local system account then ensure that we have a username and that 
			// the passwords match
			if (!cbLocalSystem.Checked)
			{
				// We must have at least a username specified
				if (tbUsername.Text == "")
				{
					MessageBox.Show("You must specify at least the username for the AuditAgent service to run under", "Validation Error");
					tbUsername.Focus();
					return false;
				}

				config.AppSettings.Settings["AuditWizardservice_username"].Value = tbUsername.Text;

				// Ensure that (any) passwords entered match
				if (tbPassword.Text != tbConfirmPassword.Text)
				{
					MessageBox.Show("The specified passwords do not match", "Validation Error");
					tbConfirmPassword.Focus();
					return false;
				}				

				// Set the password noting that it is encrypted
				string password = tbPassword.Text;
				if (password != "")
					config.AppSettings.Settings["AuditWizardservice_password"].Value = AES.Encrypt(password);
				else
					config.AppSettings.Settings["AuditWizardservice_password"].Value = "";

                // If we have not (as yet) specified anything for the AuditAgent credentials then we should
                // set them to be the same as the AuditWizard service
                //if (config.AppSettings.Settings["agent_username"].Value == "")
                    config.AppSettings.Settings["agent_username"].Value = config.AppSettings.Settings["auditwizardservice_username"].Value;
                //if (config.AppSettings.Settings["agent_password"].Value == "")
                    config.AppSettings.Settings["agent_password"].Value = config.AppSettings.Settings["auditwizardservice_password"].Value;
			}

            config.AppSettings.Settings["agent_runassystem"].Value = config.AppSettings.Settings["auditwizardservice_runassystem"].Value;

			// Save settings
			config.Save();
			return true;
		}

#endregion Button Handlers

		private ADPicker CreateAdPicker()
		{
			// Initialize the Users and Groups picker with common settings
			ADPicker usersAndGroupsPicker = new ADPicker();
			usersAndGroupsPicker.DownlevelFilter = adPickerDownlevelFilter.DSOP_DOWNLEVEL_FILTER_USERS | adPickerDownlevelFilter.DSOP_DOWNLEVEL_FILTER_GLOBAL_GROUPS;
			usersAndGroupsPicker.ScopeFormat = adPickerScopeFormat.DSOP_SCOPE_FLAG_WANT_PROVIDER_LDAP
				| adPickerScopeFormat.DSOP_SCOPE_FLAG_WANT_PROVIDER_WINNT
				| adPickerScopeFormat.DSOP_SCOPE_FLAG_STARTING_SCOPE;
			usersAndGroupsPicker.ScopeType = ((adPickerScopeType)((((((((adPickerScopeType.DSOP_SCOPE_TYPE_UPLEVEL_JOINED_DOMAIN | adPickerScopeType.DSOP_SCOPE_TYPE_DOWNLEVEL_JOINED_DOMAIN)
						| adPickerScopeType.DSOP_SCOPE_TYPE_ENTERPRISE_DOMAIN)
						| adPickerScopeType.DSOP_SCOPE_TYPE_GLOBAL_CATALOG)
						| adPickerScopeType.DSOP_SCOPE_TYPE_EXTERNAL_UPLEVEL_DOMAIN)
						| adPickerScopeType.DSOP_SCOPE_TYPE_EXTERNAL_DOWNLEVEL_DOMAIN)
						| adPickerScopeType.DSOP_SCOPE_TYPE_USER_ENTERED_UPLEVEL_SCOPE)
						| adPickerScopeType.DSOP_SCOPE_TYPE_USER_ENTERED_DOWNLEVEL_SCOPE)));
			usersAndGroupsPicker.Options = adPickerOptions.DSOP_FLAG_SKIP_TARGET_COMPUTER_DC_CHECK;

			// set the filter for searching for users or groups
			usersAndGroupsPicker.UplevelFilter = adPickerUplevelFilter.DSOP_FILTER_USERS;
			usersAndGroupsPicker.ReturnFormat = adPickerReturnFormat.UPN;
			return usersAndGroupsPicker;
		}

		private void bnBrowse_Click(object sender, EventArgs e)
		{
			// show the object picker
			ADPicker picker = CreateAdPicker();
			DialogResult result;
			try
			{
				result = picker.ShowDialog();
			}
			catch
			{
				// the picker throws an exception when there are no locations available...ignore
				return;
			}
			if (DialogResult.OK == result)
				tbUsername.Text = picker.ReturnValues[0].ToString();
		}
	}
}