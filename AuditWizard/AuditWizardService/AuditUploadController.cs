using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Timers;
using System.Windows.Forms;
//
using Layton.AuditWizard.Common;
using Layton.Common.Controls;
using Layton.AuditWizard.DataAccess;

using Layton.NetworkDiscovery;

using Microsoft.Practices.CompositeUI;

namespace Layton.AuditWizard.AuditWizardService
{
    public class AuditUploadController
    {
        // The list of folders being watched by the uploader task
        private AutoLoaderFolderList _listUploadFolders = new AutoLoaderFolderList();

        // Auto-upload available
        protected bool _autoUploadEnabled = true;

        // the service itself
        private AuditWizardService _service;

        // Timer being used to refresh the list of watched folders (fires every 10 minutes)
        private System.Timers.Timer refreshTimer = new System.Timers.Timer(600000);

        // Timer being used to have another go at performing the upload (fires every minute)
        private System.Timers.Timer uploadTimer = new System.Timers.Timer(60000);

        private Configuration config = ConfigurationManager.OpenExeConfiguration(Path.Combine(Application.StartupPath, "AuditWizardv8.exe"));

        /// <summary>
        /// This is the primary thread for the Operations Controller.
        /// </summary>
        Thread _mainThread = null;

        /// <summary>This flag should be set when we want the underlying main thread to exit</summary>
        bool _mainThreadStop = false;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="service"></param>
        public AuditUploadController(AuditWizardService service)
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
            _mainThread = new Thread(new ThreadStart(AuditUploadThreadStart));
            _mainThread.Start();

            // Log that the thread has started
            ourLog.Write("Main Audit Upload Controller Thread Running", true);
        }


        /// <summary>
        /// Called to stop the Operations Controlller
        /// </summary>
        public void Stop()
        {
            LogFile ourLog = LogFile.Instance;
            ourLog.Write("Stopping the Audit Upload Controller Thread...", true);
            _mainThreadStop = true;
        }


        /// <summary>
        /// This is the main entry point for the Audit upload Controller Main Thread.
        /// We sit in this thread until we are requested to close down
        /// </summary>
        private void AuditUploadThreadStart()
        {
            LogFile ourLog = LogFile.Instance;

            try
            {
                ourLog.Write("AuditUploadThreadStart:: Audit Upload Controller Thread active...", true);

                // Initially set the folders to watch
                RefreshAutoLoaderTask();

                // and perform an initial check of these folders if any uploading existing files
                foreach (AutoLoaderFolder thisFolder in _listUploadFolders)
                {
                    UploadNow(thisFolder.DataFolder);
                }

                // ...set a timer to go off every 10 minutes to refresh the folder list - no point doing this too often
                refreshTimer.Elapsed += new ElapsedEventHandler(refreshTimer_Elapsed);
                refreshTimer.Start();

                // ...set a second timer with a 60 second period which will actually do the uploading for us
                uploadTimer.Elapsed += new ElapsedEventHandler(uploadTimer_Elapsed);
                uploadTimer.Start();

                // We simply loop around here waiting for the thread to be terminated.  The file watchers will still
                // fire as and when we have some work to do.
                while (!_mainThreadStop)
                {
                    Thread.Sleep(5000);
                }

                // If we get here then the thread has been requested to close down - stop the timer first then exit
                refreshTimer.Elapsed -= refreshTimer_Elapsed;
                refreshTimer.Stop();

                // Close down the upload timer also
                uploadTimer.Elapsed -= refreshTimer_Elapsed;
                uploadTimer.Stop();

                // CLear our the auoloaders
                _listUploadFolders.Clear();
            }
            catch (Exception ex)
            {
                ourLog.Write("Exception occurred in AuditUploadThreadStart, error is " + ex.Message, true);
            }

            ourLog.Write("AuditUploadThreadStart:: Audit upload Controller Thread exited...", true);
        }

        /// <summary>
        /// Called as the refresh timer expires
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void refreshTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            // Temparily disable the timer as we don't want it stacking up if we are busy
            refreshTimer.Elapsed -= refreshTimer_Elapsed;
            refreshTimer.Stop();

            // Now check the folders to watch
            RefreshAutoLoaderTask();

            // Re-enable the timer
            refreshTimer.Elapsed += new ElapsedEventHandler(refreshTimer_Elapsed);
            refreshTimer.Start();
        }


        /// <summary>
        /// Called as the upload timer expires
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void uploadTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            LogFile ourLog = LogFile.Instance;

            // check if the global disable auto-upload setting is checked
            // if so, don't go any further
            if (new SettingsDAO().GetSettingAsBoolean(DatabaseSettingsKeys.Setting_DisableAllUpolads, false))
            {
                ourLog.Write("disable all uploads is set to TRUE - not uploading adf file(s)", true);
                return;
            }

            // Temparily disable the timer as we don't want it stacking up if we are busy
            uploadTimer.Elapsed -= uploadTimer_Elapsed;
            uploadTimer.Stop();

            // Now check the folders to watch
            foreach (AutoLoaderFolder thisFolder in _listUploadFolders)
            {
                UploadNow(thisFolder.DataFolder);
            }

            // Re-enable the timer
            uploadTimer.Elapsed += new ElapsedEventHandler(uploadTimer_Elapsed);
            uploadTimer.Start();
        }


        /// <summary>
        /// Refresh function for the AutoLoader task.
        /// </summary>
        private void RefreshAutoLoaderTask()
        {
            LogFile ourLog = LogFile.Instance;
            ourLog.Write("Refreshing the Auto-Loader Task Settings", true);

            // Check the database to see if auto-upload is required
            try
            {
                // AuditScanner files
                ProcessScannerFiles(FindScanners(Path.Combine(Application.StartupPath, "scanners")));

                // AuditAgent files
                ProcessScannerFiles(FindScanners(Path.Combine(Application.StartupPath, "scanners\\auditagent")));
            }
            catch (Exception ex)
            {
                ourLog.Write("Exception occurred in RefreshAutoLoaderTask, error is " + ex.Message, true);
            }

            ourLog.Write("RefreshAutoLoaderTask: Watching " + _listUploadFolders.Count.ToString() + " Folders", true);
        }

        private void ProcessScannerFiles(List<String> listScannerFileNames)
        {
            // For each of these scanners we need to validate them and read the audit data path
            foreach (string scannerFileName in listScannerFileNames)
            {
                // get the scanner object first
                AuditScannerDefinition currentScannerConfiguration = AuditWizardSerialization.DeserializeObject(scannerFileName);

                if (currentScannerConfiguration != null)
                {
                    // Do we already have this scanner in our list?  
                    AutoLoaderFolder thisFolder = _listUploadFolders.ContainsScanner(currentScannerConfiguration);

                    // If it doesn't already exist, add it
                    if (thisFolder == null)
                    {
                        AddUploadFolder(currentScannerConfiguration);
                    }
                    else
                    {
                        CheckUploadFolder(thisFolder);
                    }
                }
            }
        }

        /// <summary>
        /// Add a new upload folder to those being monitored
        /// </summary>
        /// <param name="scannerFile"></param>
        protected void AddUploadFolder(AuditScannerDefinition aScannerConfiguration)
        {
            LogFile ourLog = LogFile.Instance;

            try
            {
                // Get the timestamp for the file
                string fileName = aScannerConfiguration.Filename;
                DateTime lastWriteTime = File.GetLastWriteTime(fileName);

                if (Convert.ToBoolean(config.AppSettings.Settings["disable_all_uploads"].Value))
                {
                    ourLog.Write("disable_all_uploads is turned on - not adding to list of watched folders.", true);
                    return;
                }

                // Check to see if the file is a valid file and has auto-upload enabled
                if (!aScannerConfiguration.IsValidFile)
                {
                    ourLog.Write("The format of the scanner configuration file is not valid.", true);
                    return;
                }

                if (!aScannerConfiguration.AutoUpload)
                {
                    ourLog.Write("The scanner configuration " + aScannerConfiguration.ScannerName + " has auto-upload disabled - not processing.", true);
                    return;
                }

                try
                {
                    string dataPath = aScannerConfiguration.DeployPathData;
                    if (dataPath == "")
                        return;

                    // Create a new AutoLoaderFolder object to hold the details for this scanner
                    AutoLoaderFolder newFolder = new AutoLoaderFolder(fileName, aScannerConfiguration.ScannerName, dataPath, null, lastWriteTime);

                    // ...and add to our list
                    _listUploadFolders.Add(newFolder);
                }
                catch (Exception ex)
                {
                    ourLog.Write("Exception occurred creating the FileSystemWatcher, text is " + ex.Message, true);
                }

            }

            catch (Exception ex)
            {
                ourLog.Write("Exception occurred in [AddUploadFolder] for xml file: " + aScannerConfiguration.ScannerName +
                    " , the exception was '" + ex.Message + "'", true);
            }
        }



        /// <summary>
        /// UploadNow
        /// =========
        /// 
        /// Called when the file system watcher has determined that a .ADF file has appeared in one of the
        /// watched audit data folders.
        /// 
        /// Actually this is now called every 60 seconds as the file system watchers appeared to be very unreliable
        /// and could easily fail to initialize depending on system resources
        /// 
        /// We need to check for various types of files
        /// 
        ///		ADF - Audit Data Files created by v8 Scanners
        ///		ANF	- Alert Notification File created by Alert Monitor
        ///		AUD/SWA - Audit Files created by the USB/Mobile Device Scanners
        /// 
        /// </summary>
        protected void UploadNow(string uploadFolder)
        {
            LogFile ourLog = LogFile.Instance;
            try
            {
                ourLog.Write("Uploading any new audit data files from [" + uploadFolder + "]", true);

                // Find all of the upload files in the specified folder
                List<AuditFileInfo> listAuditFiles = new List<AuditFileInfo>();
                AuditUploader auditLoader = new AuditUploader(uploadFolder, this._service.LicenseCount);
                auditLoader.EnumerateFiles(listAuditFiles);
				ourLog.Write("There are " + listAuditFiles.Count.ToString() + " new audit data files available", true);

                // sort the list by audit scan date
                listAuditFiles.Sort();

                // then upload each in turn
				int index = 0;
                foreach (AuditFileInfo thisAuditFile in listAuditFiles)
                {
                    try
                    {
                        ourLog.Write("Uploading audit data file [" + Path.GetFileName(thisAuditFile.Filename) + "]  Index : " + index.ToString(), true);
                        auditLoader.UploadAuditDataFile(thisAuditFile.AuditFile);
                    }
                    catch (Exception ex)
                    {
                        ourLog.Write("An exception occurred while uploading the audit file [" + thisAuditFile.Filename + "].  The error was [" + ex.Message + "]", true);
                        string backupFile = thisAuditFile.AuditFile.FileName.Replace(Path.GetExtension(thisAuditFile.Filename), ".ADF.ERROR");

                        if (File.Exists(backupFile))
                            File.Delete(backupFile);

                        File.Move(thisAuditFile.AuditFile.FileName, backupFile);

                        ourLog.Write("Erroneous audit file renamed to [" + backupFile + "]", true);
                    }
                }

                // OK Do we have any Alert Notification files that need uploading?
                ourLog.Write("Uploading any new alert notification files from [" + uploadFolder + "]", true);
                AlertNotificationFileList notifications = new AlertNotificationFileList();
                if (notifications.Populate(uploadFolder) != 0)
                    UploadNotifications(notifications);

                // Do we have any USB/Mobile Device audits to upload
                LyncAuditFileList listLyncAudits = new LyncAuditFileList();
                if (listLyncAudits.Populate(uploadFolder) != 0)
                    UploadLyncAudits(listLyncAudits, uploadFolder);

                ourLog.Write("...done", true);
            }

            catch (Exception ex)
            {
                ourLog.Write("Exception occurred in [UploadNow], Exception Text is " + ex.Message, true);
            }
        }


        /// <summary>
        /// Called when we have determined that we have 1 or more alert notification files which need
        /// to be uploaded.  
        /// </summary>
        /// <param name="notifications"></param>
        protected void UploadNotifications(AlertNotificationFileList notificationFiles)
        {
            LogFile ourLog = LogFile.Instance;

            try
            {
                // Load the list of Alert Definitions as we will need to match these against the notifications
                AlertMonitorSettings alertMonitorSettings = AlertMonitorSettings.Load();
                //
                foreach (AlertNotificationFile notificationFile in notificationFiles.AlertNotificationFiles)
                {
                    ourLog.Write("Uploading Alert Notifications for " + notificationFile.AssetName, true);

                    // locate the asset for which these notifications have been created
                    //int assetID = lwDataAccess.AssetFind(notificationFile.AssetName, String.Empty);

                    // Skip any notifications for assets which we do not know about
                    //if (assetID == 0)
                    //    continue;

                    // The file may contain 1 or more individual alert triggers so iterate through them now
                    foreach (AlertNotification notification in notificationFile.AlertNotifications)
                    {
                        // We need to find the Alert Definition for which this alert has been generated
                        //AlertDefinition alertDefinition = alertMonitorSettings.FindAlert(notification.AlertName);
                        //if (alertDefinition == null)
                        //    continue;

                        // Create an alert in the database
                        Alert newAlert = new Alert();
                        newAlert.Type = Alert.AlertType.alertmonitor;
                        newAlert.Category = Alert.AlertCategory.changed;
                        newAlert.AssetName = notificationFile.AssetName;
                        newAlert.AlertName = notification.AlertName;
                        //
                        if (notification.Category != "")
                            newAlert.Message = notification.Category + @"\" + notification.Key;
                        else
                            newAlert.Message = notification.Key;
                        newAlert.Field1 = notification.OldValue;
                        newAlert.Field2 = notification.NewValue;

                        // Add the alert to the database
                        newAlert.Add();
                        ourLog.Write("....ADDING AN ALERT TO THE DATABASE, ID was " + newAlert.AlertID.ToString(), true);
                    }
                }
            }

            catch (Exception ex)
            {
                ourLog.Write("Exception occurred in [UploadNotifications], Exception Text is is " + ex.Message, true);
            }

        }




        /// <summary>
        /// Called when we have determined that we have 1 or more USB / Mobile Device files which need
        /// to be uploaded.  
        /// </summary>
        /// <param name="notifications"></param>
        protected void UploadLyncAudits(LyncAuditFileList lyncAuditFiles, string uploadFolder)
        {
            LogFile ourLog = LogFile.Instance;

            try
            {
                AuditUploader auditLoader = new AuditUploader(uploadFolder, this._service.LicenseCount);
                foreach (LyncAuditFile lyncFile in lyncAuditFiles.LyncAuditFiles)
                {
                    AuditDataFile convertedFile = lyncFile.ConvertedFile();

                    try
                    {
                        auditLoader.UploadAuditDataFile(convertedFile);
                    }
                    catch (Exception)
                    {
                    }
                }
            }

            catch (Exception ex)
            {
                ourLog.Write("Exception occurred in [UploadLyncAudits], Exception Text is is " + ex.Message, true);
            }
        }


        /// <summary>
        /// This function is called to check the supplied scanner to determine if it has been updated
        /// and if so to check and if necessary update the data folder being watched
        /// </summary>
        /// <param name="theFolder"></param>
        protected void CheckUploadFolder(AutoLoaderFolder aFolder)
        {
            LogFile ourLog = LogFile.Instance;

            try
            {
                // Get the timestamp for the file and if matches that stored exit
                DateTime lastWriteTime = File.GetLastWriteTime(aFolder.FileName);

                if (lastWriteTime == aFolder.LastTimeStamp)
                    return;

                ourLog.Write("File timestamp indicates changes made - validating", true);

                // the timestamp differs meaning that the file has been updated - this means that we
                // will have to re-read the file and check it's data  folder
                aFolder.LastTimeStamp = lastWriteTime;			// Update the timestamp anyway
                AuditScannerDefinition scannerConfiguration = AuditWizardSerialization.DeserializeObject(aFolder.FileName);

                if (Convert.ToBoolean(config.AppSettings.Settings["disable_all_uploads"].Value))
                {
                    ourLog.Write("disable_all_uploads is turned on - removing from list of watched folders.", true);
                    _listUploadFolders.Remove(aFolder);
                    return;
                }

                if (!scannerConfiguration.AutoUpload)
                {
                    // if the auto-upload has been changed to false then remove from the list
                    ourLog.Write("Auto-upload is set to false - removing from list of watched folders.", true);
                    _listUploadFolders.Remove(aFolder);
                    return;
                }

                // Check to see if the file is in fact a valid file and if not we ignore this file but update our timestamp
                if (!scannerConfiguration.IsValidFile)
                {
                    ourLog.Write("Scanner configuration File does not appear to be valid and will be ignored.", true);
                    return;
                }

                // Get the data Path - has it changed?  If not then return
                if (scannerConfiguration.DeployPathData == aFolder.DataFolder)
                {
                    ourLog.Write("Scanner data path does not appear to have changed - ignoring file update.", true);
                    return;
                }

                // Ok the path has changed so we need to change the path looked at by the watcher.
                aFolder.DataFolder = scannerConfiguration.DeployPathData;

            }

            catch (Exception ex)
            {
                ourLog.Write("Exception occurred in [CheckUploadFolder], Exception Text is is " + ex.Message, true);
            }
        }

        /// <summary>
        /// Return a list of all of the scanner filenames located in the specified folder
        /// </summary>
        /// <param name="scannerPath"></param>
        /// <returns></returns>
        private List<string> FindScanners(string scannerPath)
        {
            LogFile ourLog = LogFile.Instance;

            List<string> returnList = new List<string>();
            try
            {
                // Find our path and use this to identify where the scanner configuration files will be
                DirectoryInfo di = new DirectoryInfo(scannerPath);

                // Loop around each of the XML folders in this folder assuming that they are scanner configuration
                // files and then try and read each
                FileInfo[] rgFiles = di.GetFiles("*.xml");
                foreach (FileInfo fi in rgFiles)
                {
                    string scannerFileName = fi.FullName;
                    returnList.Add(scannerFileName);
                }
            }

            catch (Exception ex)
            {
                ourLog.Write("Exception occurred in [FindScanners], Exception Text is is " + ex.Message, true);
            }

            return returnList;
        }

    }
}
