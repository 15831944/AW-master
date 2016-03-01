using System;
using System.Collections.Generic;
using System.Text;
using System.Timers;
using System.Diagnostics;
using System.Net;
using System.Net.Mail;
using System.Data;
using System.Configuration;
//
using Layton.AuditWizard.Common;
using Layton.Common.Controls;
using Layton.AuditWizard.DataAccess;

namespace Layton.AuditWizard.AuditWizardService
{
	
	public class DatabasePurgeController
    {		
		private System.Timers.Timer purgeTimer = new System.Timers.Timer();
		private AuditWizardService _service;

		public DatabasePurgeController(AuditWizardService service)
		{
			_service = service;
		}

        public void ConfigureTimer()
        {
            DateTime nextCheck = DateTime.Now.Date;

            // set the purge to be performed at 6am
            if (DateTime.Now.Hour >= 9)
				nextCheck = nextCheck.AddDays(1);
			nextCheck = nextCheck.AddHours(9);

            // configure the timer
			purgeTimer.Elapsed += new ElapsedEventHandler(purgeTimer_Elapsed);
			purgeTimer.AutoReset = false;
			TimeSpan ts = nextCheck.Subtract(DateTime.Now);
			purgeTimer.Interval = ts.TotalMilliseconds;

			LogFile ourLog = LogFile.Instance;
			ourLog.Write("Purge timer configured to fire at " + nextCheck + " in " + ts.TotalMinutes + " minutes time", true);
		}


		/// <summary>
		/// Called to start the Alerter Task
		/// </summary>
        public void Start()
        {
			// Try and run the purge immediately
			DoDatabasePurge();

			// ...set a daily timer to go off everyday
            ConfigureTimer();
			purgeTimer.Start();
        }

		public void Stop()
		{
			purgeTimer.Stop();
		}


		/// <summary>
		/// Called as the purge timer expires to perform a database purge based on the current settings
		/// </summary>
        public void DoDatabasePurge()
        {
			// Create a log file
			LogFile ourLog = LogFile.Instance;

			try
			{
				// Get the current purge settings as we may have been disabled
				DatabaseSettings databaseSettings = new DatabaseSettings();
				databaseSettings.LoadSettings();
				if (!databaseSettings.AutoPurge)
					return;

				// OK - now do the purge
                DatabaseMaintenanceDAO lwDataAccess = new DatabaseMaintenanceDAO();
				int nRowsPurged = lwDataAccess.DatabasePurge(databaseSettings);
				ourLog.Write("The AuditWizard database has been purged, " + nRowsPurged.ToString() + " row(s) were deleted" ,true);
			}

			catch (Exception ex)
			{
				ourLog.Write("Exception occurred in [DoDatabasePurge], Exception Text is is " + ex.Message, true);
			}
		}



		/// <summary>
		/// Called as the Purge timer expired
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>

        void purgeTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
			DoDatabasePurge();
			purgeTimer.Elapsed -= purgeTimer_Elapsed; 
            ConfigureTimer();
        }
    }
}
