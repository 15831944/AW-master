using System;
using System.Threading;
using System.Timers;
using Layton.AuditWizard.DataAccess;
using Layton.Common.Controls;
using Layton.NetworkDiscovery;

namespace Layton.AuditWizard.AuditWizardService
{
    public class DiscoveryController
    {
        private AuditWizardService _service;
        private System.Timers.Timer discoveryTimer;

        /// <summary>
        /// This is the primary thread for the Operations Controller.
        /// </summary>
        Thread _mainThread = null;

        public DiscoveryController(AuditWizardService service)
        {
            _service = service;
        }

        /// <summary>
        /// Called to start the Audit Uploader Task
        /// </summary>
        public void Start()
        {
            LogFile ourLog = LogFile.Instance;
            ourLog.Write("Creating the Audit Upload Controller Thread...", true);

            // The Controller runs in its own thread to prevent any errors here causing issues
            // with other parts of the AuditWizard Service
            if (new SettingsDAO().GetSettingAsBoolean("AutoScanNetwork", false))
            {
                long timerValue = 28800000;
                string intervalUnits = new SettingsDAO().GetSetting("AutoScanIntervalUnits", false);
                double intervalValue = Convert.ToDouble(new SettingsDAO().GetSetting("AutoScanIntervalValue", false));

                switch (intervalUnits)
                {
                    case "hours":
                        timerValue = Convert.ToInt64(intervalValue * 3600000);
                        break;
                    case "minutes":
                        timerValue = Convert.ToInt64(intervalValue * 60000);
                        break;
                    case "days":
                        timerValue = Convert.ToInt64(intervalValue * 86400000);
                        break;
                    default:
                        break;
                }

                discoveryTimer = new System.Timers.Timer(timerValue);

                _mainThread = new Thread(new ThreadStart(DiscoveryThreadStart));
                _mainThread.Start();
            }

            // Log that the thread has started
            ourLog.Write("Main Audit Upload Controller Thread Running", true);
        }


        /// <summary>
        /// Called to stop the Operations Controlller
        /// </summary>
        public void Stop()
        {
            if (discoveryTimer != null) discoveryTimer.Stop();
        }

        /// <summary>
        /// This is the main entry point for the Audit upload Controller Main Thread.
        /// We sit in this thread until we are requested to close down
        /// </summary>
        private void DiscoveryThreadStart()
        {
            LogFile ourLog = LogFile.Instance;

            try
            {
                ourLog.Write("AuditUploadThreadStart:: Audit Upload Controller Thread active...", true);

                discoveryTimer.Elapsed += new ElapsedEventHandler(discoveryTimer_Elapsed);
                discoveryTimer.Start();

            }
            catch (Exception ex)
            {
                ourLog.Write("Exception occurred in AuditUploadThreadStart, error is " + ex.Message, true);
            }

            ourLog.Write("AuditUploadThreadStart:: Audit upload Controller Thread exited...", true);
        }

        /// <summary>
        /// Called as the discovery timer expires
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void discoveryTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            // Temparily disable the timer as we don't want it stacking up if we are busy
            discoveryTimer.Elapsed -= discoveryTimer_Elapsed;
            discoveryTimer.Stop();

            // perform network discovery
            TcpipNetworkDiscovery tcpNet = new TcpipNetworkDiscovery(Utility.GetComputerIpRanges());
            tcpNet.Start();

            // Re-enable the timer
            discoveryTimer.Elapsed += new ElapsedEventHandler(discoveryTimer_Elapsed);
            discoveryTimer.Start();
        }
    }
}
