using System;
using System.Timers;
using System.IO;
//
using Layton.AuditWizard.Common;
using Layton.Common.Controls;
using Layton.AuditWizard.DataAccess;

namespace Layton.AuditWizard.AuditWizardService
{
	
	/// <summary>
	/// The AlerterController handles the notification of users when 1 or more alerts are detected.  Note that alerts are 
	/// handled in two separate ways - first they are uploaded into the database by the AutoLoader task and secondly this 
	/// task will check every 60 seconds for any new alerts having been generated and email the user with details of ALL
	/// alerts uploaded.
	/// 
	/// We work in this way as it should reduce the number of emails being sent as multiple alerts can be combined into 
	/// a single email
	/// </summary>
	public class AlerterController
    {		
		/// <summary>
		/// This is the timer which fires every HOUR to check for alerts having been uploaded into the database
		/// </summary>
		private System.Timers.Timer alertTimer = new System.Timers.Timer(60 * 60 * 1000);
		private AuditWizardService _service;

		public AlerterController(AuditWizardService service)
		{
			_service = service;
		}

		/// <summary>
		/// Called to start the Alerter Task
		/// </summary>
        public void Start()
        {
			LogFile ourLog = LogFile.Instance;
			ourLog.Write("AlerterController Starting...", true);

			// We will wake up hourly regardless of what the configuration syas and will then check the settings and time
			alertTimer.Elapsed += new ElapsedEventHandler(alertTimer_Elapsed);
			ourLog.Write("AlerterController Timer is set to fire in " + (alertTimer.Interval / 1000).ToString() + " seconds (at " + DateTime.Now.AddMilliseconds(alertTimer.Interval).ToString() + ")", true);

			alertTimer.Start();
        }

		public void Stop()
		{
			alertTimer.Stop();
		}


		/// <summary>
		/// Called as the alert timer expires to check for any alerts having been generated and reporting them
		/// optionally sending emails if these have been configured
		/// 
		/// Alert checking is done daily and will result in a number of entries being added to the ALERT table
		/// These entries are in turn 
		/// </summary>
        public void CheckForAlerts()
        {
			LogFile ourLog = LogFile.Instance;
			ourLog.Write("CheckForAlerts", true);

			//
			try
			{
				SettingsDAO lSettingsDao = new SettingsDAO();

                string fileName = String.Empty;
                string selectedFileName = String.Empty;
                System.Xml.Serialization.XmlSerializer serializer;
                AlertDefinition definition;
				
				// We need to be sure that we do not email the same alerts multiple times so we recover the date at 
				// which we last send an email alert and only add alerts which occurred after that date to our email
				DateTime dateLastAlertEmail;
				string lastAlertDate = lSettingsDao.GetSettingAsString(DatabaseSettings.Setting_LastAlertEmailDate, "");

				// If we have not previously checked alerts then look for alerts in the last day
				if (lastAlertDate == "")
					dateLastAlertEmail = DateTime.Now.AddDays(-1);
				else
					dateLastAlertEmail = Convert.ToDateTime(lastAlertDate);

				// Log the last alert date (if any)
				ourLog.Write("Checking For Alerts generated since " + dateLastAlertEmail.ToString(), true);

				// Allocate a list to hold the alerts so that they can be emailed in one go
				AlertList listAlerts = new AlertList();

				// +8.3.3
				// Now check the email frequency as if this is set to daily and we have already emailed today then we do not email again
				string mailFrequency = lSettingsDao.GetSetting(DatabaseSettings.Setting_AlertMonitorEmailFrequency, false);
				if (mailFrequency == DatabaseSettings.Setting_AlertMonitorEmailDaily)
				{
					string mailtime = lSettingsDao.GetSetting(DatabaseSettings.Setting_AlertMonitorEmailTime, false);
					if (mailtime == "")
						mailtime = "18:00";
					DateTime emailTime = DateTime.Parse(mailtime);
					ourLog.Write("Daily checking set for " + emailTime.ToString("HH:mm") + " - Current time is " + DateTime.Now.ToString("HH:mm") + " Last Check Date was " + dateLastAlertEmail.Date.ToShortDateString(), true);

					// Are we still prior to the checking time?
					if (DateTime.Now.TimeOfDay < emailTime.TimeOfDay)
					{
						ourLog.Write("Check time not reached so exiting", true);
						return;
					}

					// We are past the check date - we check if we have not previously checked today
					else if (dateLastAlertEmail.Date == DateTime.Now.Date) 
					{
						ourLog.Write("Check yime reached but already checked today so exiting", true);
						return;
					}
				}

				// Populate the Alert date so that we do not re-do this process
				lSettingsDao.SetSetting(DatabaseSettings.Setting_LastAlertEmailDate, DateTime.Now.ToString(), false);
				
				// Now we read all alerts created since the specified date 
				listAlerts.Populate(dateLastAlertEmail);

				if (listAlerts.Count != 0)
				{
					// read the alert definitions as we need to know which if any of these alerts should be emailed
                    // first get the AuditScanner object
					// Loop through the alerts and weed out any AM alerts which do not require email
					for (int index=0; index<listAlerts.Count; )
					{
						Alert alert = listAlerts[index];
						if (alert.Type == Alert.AlertType.alertmonitor)
						{
                            fileName = alert.AlertName;
                            selectedFileName = Path.Combine(System.Windows.Forms.Application.StartupPath, "scanners\\alertmonitors\\") + fileName + ".xml";
                            serializer = new System.Xml.Serialization.XmlSerializer(typeof(AlertDefinition));
                            definition = (AlertDefinition)serializer.Deserialize(new StreamReader(selectedFileName));

                            //if ((definition.Name != alert.AlertName) || (!definition.EmailAlert))
                            if (!definition.EmailAlert)
                            {
                                listAlerts.Remove(alert);
                                continue;
                            }
						}
						
						index++;
					}					

					// If we still have some alerts left then email them					
					if (listAlerts.Count != 0)
					{				
						// get the Email Controller Task
						EmailController emailController = _service.GetEmailController();
				
						// and request it to send an Alerts email on our behalf
						ourLog.Write("....an alerts email is required", true);
						emailController.SendStatusEmail(true, false, listAlerts);
					}
				}
			}

			catch (Exception ex)
			{
				ourLog.Write("Exception occurred in [CheckForAlerts], Exception Text is " + ex.Message, true);
			}
		}

			

		/// <summary>
		/// Called as the Alert timer expired
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>

        void alertTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
			CheckForAlerts();
        }
    }
}
