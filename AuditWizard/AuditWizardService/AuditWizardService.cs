using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.ServiceProcess;
using System.Text;
using System.Windows.Forms;
//
using Layton.Common.Controls;
using Layton.AuditWizard.Common;
using Layton.Cab.Interface;
using Layton.AuditWizard.DataAccess;

using Infragistics.Win.UltraWinSchedule;
using Microsoft.Practices.CompositeUI;

namespace Layton.AuditWizard.AuditWizardService
{
    public partial class AuditWizardService : ServiceBase
    {
        #region data

        private const int TrialCount = 10;

        // This is the refresh timer used to periodically check for any changes in configuration
        private System.Timers.Timer timerRefresh;

        // The Email Controller used by the Email Task
        private EmailController emailController;

        // The Email Controller used by the Email Task
        private AlerterController alerterController;

        // The Database Purge Controller
        private DatabasePurgeController purgeController;

        // The TCP (Audit Upload) Controller)
        private TcpUploadController tcpUploadController;

        // The Audit Upload Controller)
        private AuditUploadController auditUploadController;

        private DiscoveryController discoveryController;

        // The Operations Controller
        private OperationController operationController;

        /// <summary>The License / Product key Object</summary>
        protected Layton.Cab.Interface.LaytonProductKey _productKey = null;

        //private UltraMonthViewSingle awTaskMonthView;
        private UltraCalendarInfo ultraCalendarInfo;

        /// <summary>
        /// Return the Email Controller
        /// </summary>
        /// <returns></returns>
        public EmailController GetEmailController()
        {
            return emailController;
        }

        /// <summary>
        /// Get the current license count
        /// </summary>
        public int LicenseCount
        {
            get
            {
                // Bug #110  - always get the latest version of Product Key in case of change
                LoadProductKey();
                return (_productKey.IsTrial) ? TrialCount : _productKey.AssetCount;
            }
        }

        #endregion data

        #region Constructor

        public AuditWizardService()
        {
            InitializeComponent();

            // Tidy up old EventLog sources
            try
            {
                if (!System.Diagnostics.EventLog.SourceExists("AWServiceSource"))
                    System.Diagnostics.EventLog.DeleteEventSource("AWServiceSource");
                if (!System.Diagnostics.EventLog.SourceExists("AuditWizardAutoLoaderLog"))
                    System.Diagnostics.EventLog.DeleteEventSource("AuditWizardAutoLoaderLog");
            }
            catch (Exception)
            { }


            // Create a new EventSource if necessary for the LicenseService
            if (!System.Diagnostics.EventLog.SourceExists("LWServiceSource"))
                System.Diagnostics.EventLog.CreateEventSource("LWServiceSource", "LWServiceLog");

            // the event log source by which the application is registered on the computer
            eventLog1.Source = "LWServiceSource";
            eventLog1.Log = "LWServiceLog";
            eventLog1.ModifyOverflowPolicy(OverflowAction.OverwriteAsNeeded, 0);
        }

        #endregion Constructor

        #region Start/Stop Handlers

        /// <summary>
        /// This function is called as the service is starting.  We simply initialize and start
        /// a file system watcher to check for activity in the scanner data folder
        /// </summary>
        /// <param name="args"></param>
        protected override void OnStart(string[] args)
        {
            // Connect to the event log
            eventLog1.WriteEntry("The AuditWizard Service has started");

            // Create a log file
            LogFile ourLog = LogFile.Instance;

            // Define working directory (For a service, this is set to System dir by default...)
            Process pc = Process.GetCurrentProcess();
            System.IO.Directory.SetCurrentDirectory(pc.MainModule.FileName.Substring(0, pc.MainModule.FileName.LastIndexOf(@"\")));

            // Before we go any further lets load the Product / license key to recover the current license count
            // and check to ensure that the product is indeed licensed
            if (!LoadProductKey())
            {
                eventLog1.WriteEntry("Failed to load the AuditWizard Product ID - it may be invalid or may have expired.   The service will now be stopped");
                ourLog.Write("Failed to load the AuditWizard Product ID - it may be invalid or may have expired.   The service will now be stopped", true);
                Stop();
                return;
            }

            // OK log the license details
            if (_productKey.IsTrial)
                ourLog.Write("The AuditWizard is running in Trial Mode and will be limited to uploading a total of " + LicenseCount.ToString() + " computers into the database.  The trial will expire in " + _productKey.TrialDaysRemaining + " day(s)", true);
            else
                ourLog.Write("This is a licensed copy of AuditWizard.  It is limited to uploading a total of " + LicenseCount.ToString() + " computers into the database.", true);

            // Tasks for the AuditWizard Service
            //	Email
            //  AutoLoader
            //  Asset Discovery & Deployment
            //  Alert detector
			//  Auto Purge
            bool startEmailStatus = false;
            bool startTcpUploaderStatus = false;
            bool startAuditUploaderStatus = false;
            bool startDiscoveryStatus = false;
            bool startAlerterStatus = false;
            bool startOperationStatus = false;
			bool startPurgeStatus = false;

            try
            {
                // Start the email task
                startEmailStatus = StartEmailTask();
                if (startEmailStatus)
                    ourLog.Write("Email Controller Task Started Successfully", true);
                else
                    ourLog.Write("Email Controller Task Failed to Start", true);

                // Start the Audit Auto Loader task
                startAuditUploaderStatus = StartAuditUploaderTask();
                if (startAuditUploaderStatus)
                    ourLog.Write("Audit AutoLoader Controller Task Started Successfully", true);
                else
                    ourLog.Write("Audit AutoLoader Controller Task Failed to Start", true);

                // Start the TCP Loader task
                startTcpUploaderStatus = StartTcpLoaderTask();
                if (startTcpUploaderStatus)
                    ourLog.Write("TCP AutoLoader Controller Task Started Successfully", true);
                else
                    ourLog.Write("TCP AutoLoader Controller Task Failed to Start", true);

                // start the discovery task
                startDiscoveryStatus = StartDiscoveryTask();
                if (startDiscoveryStatus)
                    ourLog.Write("Discovery Controller Task Started Successfully", true);
                else
                    ourLog.Write("Discovery Controller Task Failed to Start", true);

                // start the alerter task
                startAlerterStatus = StartAlerterTask();
                if (startAlerterStatus)
                    ourLog.Write("Alerter Task Started Successfully", true);
                else
                    ourLog.Write("Alerter Controller Task Failed to Start", true);

				// start the Operation Controller task
				startOperationStatus = StartOperationControllerTask();
				if (startOperationStatus)
					ourLog.Write("Operations Controller Task Started Successfully", true);
				else
					ourLog.Write("Operations Controller Task Failed to Start", true);

				// start the Purge Controller task
				startPurgeStatus = StartDatabasePurgeTask();
				if (startPurgeStatus)
					ourLog.Write("Purge Controller Task Started Successfully", true);
				else
					ourLog.Write("Purge Controller Task Failed to Start", true);                

            }
            catch (Exception ex)
            {
                ourLog.Write("Exception occurred in [OnStart], Exception Text is " + ex.Message, true);
            }

            // If any of the tasks have failed then we need to stop the service again and exit
            if (!startEmailStatus | !startAuditUploaderStatus | !startTcpUploaderStatus | !startDiscoveryStatus | !startAlerterStatus | !startOperationStatus)
            {
                ourLog.Write("One or more of the AuditWizard Services have failed to start, the service will now be stopped", true);
                eventLog1.WriteEntry("One or more of the AuditWizard Services have failed to start, the service will now be stopped");
                Stop();
            }


            // Running - start the refresh timer
            // Create a timer which will fire every 30 seconds and will check for any significant changes in the 
            // configuration of this service
            try
            {
                this.timerRefresh = new System.Timers.Timer();
                this.timerRefresh.Enabled = true;
                this.timerRefresh.Interval = 61000;						// 61 seconds between checks
                this.timerRefresh.Elapsed += new System.Timers.ElapsedEventHandler(this.timerRefresh_Elapsed);
            }
            catch (Exception ex)
            {
                eventLog1.WriteEntry("Exception starting refresh timer : " + ex.Message, EventLogEntryType.Error);
            }
        }

        private void HandleCalendarEvent()
        {
            LogFile ourLog = new LogFile();

            try
            {
                SettingsDAO lSettingsDAO = new SettingsDAO();
                ReportSchedulesDAO lReportSchedulesDAO = new ReportSchedulesDAO();
                Appointment appointment;
                object rawAppointmentData;
                ultraCalendarInfo.Appointments.Clear();

                foreach (DataRow row in lReportSchedulesDAO.GetAllAppointments().Rows)
                {
                    rawAppointmentData = row["_APPOINTMENTDATA"];

                    if (rawAppointmentData is byte[] == false)
                        continue;

                    appointment = Appointment.FromBytes(rawAppointmentData as byte[]);
                    ultraCalendarInfo.Appointments.Add(appointment);
                }

                string strLastReportRunTime = lSettingsDAO.GetSetting("LastReportRun", false);

                DateTime lLastReportRunDateTime = (strLastReportRunTime == "") ? DateTime.MinValue : Convert.ToDateTime(strLastReportRunTime);
                DateTime lReportRunTime = DateTime.Now;
                
                AppointmentsSubsetCollection expiredAppointments = ultraCalendarInfo.GetAppointmentsInRange(lLastReportRunDateTime, lReportRunTime);
                lSettingsDAO.SetSetting("LastReportRun", lReportRunTime.ToString(), false);

                foreach (Appointment expiredAppointment in expiredAppointments)
                {
                    // need to re-check that this appointment is between the LastTaskReportRun date and DateTime.Now
                    // there is a hole in the ultraCalendarInfo.GetAppointmentsInRange logic above 
                    if ((lLastReportRunDateTime < expiredAppointment.StartDateTime) && (lReportRunTime > expiredAppointment.StartDateTime))
                    {
                        string[] lSubject = expiredAppointment.Subject.Split('|');
                        string lReportCategory = "";
                        string lReportName = "";

                        if (lSubject.Length == 2)
                        {
                            lReportCategory = lSubject[0];
                            lReportName = lSubject[1];
                        }
                        else if (lSubject.Length == 3)
                        {
                            lReportCategory = lSubject[0];
                            lReportName = lSubject[1] + " | " + lSubject[2];
                        }

                        if (lReportCategory.StartsWith("Custom Report"))
                        {
                            int lReportId = Convert.ToInt32(expiredAppointment.Description);
                            emailController.SendCustomReportByEmail(expiredAppointment.Subject, lReportId, expiredAppointment.Location);
                        }
                        else if (lReportCategory.StartsWith("Compliance Report"))
                        {
                            int lReportId = Convert.ToInt32(expiredAppointment.Description);
                            emailController.SendComplianceReportByEmail(expiredAppointment.Subject, lReportId, expiredAppointment.Location);
                        }
                        else if (lReportCategory.StartsWith("User-Defined SQL Report"))
                        {
                            int lReportId = Convert.ToInt32(expiredAppointment.Description);
                            emailController.SendSQLReportByEmail(expiredAppointment.Subject, lReportId, expiredAppointment.Location);
                        }
                        else
                        {
                            emailController.SendReportByEmail(expiredAppointment.Subject, expiredAppointment.Location);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ourLog.Write(ex.Message, true);
            }
        }

        /// <summary>
        /// Stop
        /// ====
        /// 
        /// Called as the AuditWizard Service is stopping
        /// </summary>
        protected override void OnStop()
        {
            CloseControllers();
        }

        protected override void OnShutdown()
        {
            CloseControllers();
        }

        protected void CloseControllers()
        {
            LogFile ourLog = LogFile.Instance;

            // Stop the controllers
            if (emailController != null)
            {
                ourLog.Write("Closing the Email Controller Task...", true);
                emailController.Stop();
            }

            if (alerterController != null)
            {
                ourLog.Write("Closing the Alert Controller Task...", true);
                alerterController.Stop();
            }

            if (purgeController != null)
            {
                ourLog.Write("Closing the Database Purge Controller Task...", true);
                purgeController.Stop();
            }

            if (auditUploadController != null)
            {
                ourLog.Write("Closing the Audit Upload Controller Task...", true);
                auditUploadController.Stop();
            }

            if (tcpUploadController != null)
            {
                ourLog.Write("Closing the TCP Upload Controller Task...", true);
                tcpUploadController.Stop();
            }

            if (discoveryController != null)
            {
                ourLog.Write("Closing the Auto-Discovery Controller Task...", true);
                discoveryController.Stop();
            }

            if (operationController != null)
            {
                ourLog.Write("Closing the Operation Controller Task...", true);
                operationController.Stop();
            }

            eventLog1.WriteEntry("The AuditWizard Service has stopped");
            ourLog.Write("The AuditWizard Service is closing down", true);
        }

        #endregion Start/Stop Handlers

        /// <summary>
        /// This function is called to load (any) AuditWizard license details from the main configuration file
        /// </summary>
        /// <returns></returns>
        public bool LoadProductKey()
        {
            LogFile ourLog = LogFile.Instance;

            // get the product key and installation settings
            string key = "";
            int productID = 0;
            string companyName = "";
            int companyID = 0;
            string installDateString = "";

            // Load the configuration for the Layton Framework
            Configuration config = ConfigurationManager.OpenExeConfiguration(Path.Combine(Application.StartupPath, "AuditWizardv8.exe"));

            // Now try and read the product license key details from it
            try
            {
                key = config.AppSettings.Settings["ProductKey"].Value;
            }
            catch
            {

            }
            try
            {
                productID = Convert.ToInt32(config.AppSettings.Settings["ProductID"].Value);
            }
            catch
            {

            }
            try
            {
                companyName = config.AppSettings.Settings["CompanyName"].Value;
            }
            catch
            {

            }
            try
            {
                companyID = Convert.ToInt32(config.AppSettings.Settings["CompanyID"].Value);
            }
            catch
            {

            }
            try
            {
                installDateString = config.AppSettings.Settings["Code1"].Value;
            }
            catch
            {

            }
            if (installDateString == "")
            {
                installDateString = Layton.Cab.Interface.LaytonProductKey.GenerateCode2();
                config.AppSettings.Settings["Code1"].Value = installDateString;
                config.Save();
            }

            // Create the ProductKey object
            _productKey = new Layton.Cab.Interface.LaytonProductKey(key, productID, companyName, companyID, installDateString);
            //ourLog.Write("Product Key Details: ProductID is " + productID.ToString() + "  Company Name is " + companyName + "  Company ID is " + companyID.ToString() + "  Install date string is " + installDateString, true);

            // Has this license expired (trial or otherwise)
            return true;
            //!(_productKey.IsTrialExpired);
        }


        #region Email Task Functions

        /// <summary>
        /// This is the main function for the Email Task - it starts the task
        /// </summary>
        /// <returns></returns>
        protected bool StartEmailTask()
        {
            // Start the script controllers
            eventLog1.WriteEntry("Starting Email Task", EventLogEntryType.Information);
            emailController = new EmailController();
            emailController.Start();
            eventLog1.WriteEntry("Email Task Started", EventLogEntryType.Information);
            return true;
        }

        #endregion Email Task Functions

        #region AutoLoader Task

        /// <summary>
        /// This function is responsible for starting the AutoLoader Task
        /// </summary>
        /// <returns></returns>
        protected bool StartAuditUploaderTask()
        {
            eventLog1.WriteEntry("Starting Audit Auto Loader task", EventLogEntryType.Information);
            LogFile ourLog = LogFile.Instance;

            // Start the Audit Upload Controller
            LaunchAuditUploadController();

            // Log that the task has started
            eventLog1.WriteEntry("AutoLoader Task Started", EventLogEntryType.Information);
            return true;
        }


        /// <summary>
        /// Launch the Audit Upload Controller to upload audits from watched folders
        /// </summary>
        private void LaunchAuditUploadController()
        {
            try
            {
                auditUploadController = new AuditUploadController(this);
                auditUploadController.Start();
            }
            catch (Exception e)
            {
                eventLog1.WriteEntry("AuditWizard Audit Upload Controller failed to start due to exception: " + e.Message, EventLogEntryType.Error);
            }
        }

        #endregion AutoLoader Task

        #region TCP Loader Task Functions
        /// <summary>
        /// This function is responsible for starting the AutoLoader Task
        /// </summary>
        /// <returns></returns>
        protected bool StartTcpLoaderTask()
        {
            eventLog1.WriteEntry("Starting TCP Loader task", EventLogEntryType.Information);
            LogFile ourLog = LogFile.Instance;

            // We start the TCP listener regardless of any other settings 
            LaunchTcpUploadController();

            // Log the task starting
            eventLog1.WriteEntry("TCP Loader Task Started", EventLogEntryType.Information);
            return true;
        }

        /// <summary>
        /// Called to stop the AutoLoader task. This simply shuts down the listeners
        /// </summary>
        private void StopAutoLoaderTask()
        {
            LogFile ourLog = LogFile.Instance;
            eventLog1.WriteEntry("Stopping AutoLoader Task - Disabled in settings", EventLogEntryType.Information);
            ourLog.Write("Auto-Upload is not enabled, this task will be disabled in the service", true);
        }


        /// <summary>
        /// Launch the TCP Controller to upload audits from deployed scanners and clients
        /// </summary>
        private void LaunchTcpUploadController()
        {
            try
            {
                tcpUploadController = new TcpUploadController(this);
                tcpUploadController.Start();
            }
            catch (Exception e)
            {
                eventLog1.WriteEntry("AuditWizard TCP Upload Controller failed to start due to exception: " + e.Message, EventLogEntryType.Error);
            }
        }

        #endregion AutoLoader Task Functions

        #region Discovery Task Functions

        protected bool StartDiscoveryTask()
        {
            eventLog1.WriteEntry("Starting Discovery Task", EventLogEntryType.Information);

            // Here we need to be able to perform network discovery and agent deployment
            //LaunchDiscoveryController();
            discoveryController = new DiscoveryController(this);
            discoveryController.Start();

            eventLog1.WriteEntry("Discovery task started", EventLogEntryType.Information);
            return true;
        }

        /// <summary>
        /// Launch the Audit Upload Controller to upload audits from watched folders
        /// </summary>
        private void LaunchDiscoveryController()
        {
            try
            {
                discoveryController = new DiscoveryController(this);
                discoveryController.Start();
            }
            catch (Exception e)
            {
                eventLog1.WriteEntry("AuditWizard Audit Upload Controller failed to start due to exception: " + e.Message, EventLogEntryType.Error);
            }
        }


        #endregion Discovery Task Functions

        #region Alerter Task Functions
        /// <summary>
        /// This function is responsible for starting the Alerter Task
        /// </summary>
        /// <returns></returns>
        protected bool StartAlerterTask()
        {
            eventLog1.WriteEntry("Starting Alerter task", EventLogEntryType.Information);
            LogFile ourLog = LogFile.Instance;

            alerterController = new AlerterController(this);
            alerterController.Start();

            eventLog1.WriteEntry("Alerter Task Started", EventLogEntryType.Information);
            return true;
        }

        #endregion Alerter Task Functions

        #region Database Purge Task Functions
        /// <summary>
        /// This function is responsible for starting the Database Purge Task
        /// </summary>
        /// <returns></returns>
        protected bool StartDatabasePurgeTask()
        {
            eventLog1.WriteEntry("Starting Database Purge task", EventLogEntryType.Information);
            LogFile ourLog = LogFile.Instance;

            purgeController = new DatabasePurgeController(this);
            purgeController.Start();

            eventLog1.WriteEntry("Database Purge Task Started", EventLogEntryType.Information);
            return true;
        }



        #endregion Database Purge  Task Functions

        #region Operation Controller Task Functions
        /// <summary>
        /// This function is responsible for Handling the Operations Queue
        /// </summary>
        /// <returns></returns>
        protected bool StartOperationControllerTask()
        {
            eventLog1.WriteEntry("Starting Operation Controller task", EventLogEntryType.Information);
            LogFile ourLog = LogFile.Instance;

            operationController = new OperationController(this);
            operationController.Start();

            eventLog1.WriteEntry("Operation Controller Task Started", EventLogEntryType.Information);
            return true;
        }



        #endregion Database Purge  Task Functions

        #region Refresh and Timer handlers

        /// <summary>
        /// This timer is called to refresh the configuration of the tasks
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void timerRefresh_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            timerRefresh.Stop();
            timerRefresh.Enabled = false;

            RefreshEmailTask();
            RefreshDiscoveryTask();
            HandleCalendarEvent();

            timerRefresh.Enabled = true;
            timerRefresh.Start();
        }


        /// <summary>
        /// Refresh function for the Email task.
        /// </summary>
        private void RefreshEmailTask()
        {
        }



        /// <summary>
        /// Refresh function for the Network Discovery task.
        /// </summary>
        private void RefreshDiscoveryTask()
        {
        }

        #endregion Refresh and Timer handlers
    }
}
