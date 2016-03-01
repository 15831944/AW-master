using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Windows.Forms;
// 
using Infragistics.Win.UltraWinListView;
//
using Layton.AuditWizard.Common;
using Layton.Common.Controls;
using Layton.AuditWizard.DataAccess;

namespace Layton.AuditWizard.Network
{
    public partial class FormUploadAudits : ShadedImageForm
    {
        protected List<AuditDataFile> _listToUpload = new List<AuditDataFile>();
        protected string _uploadFolder = "";
        protected FileSystemWatcher watcher = new FileSystemWatcher();
        private List<AuditDataFile> _uploadedFiles; 
        protected static readonly log4net.ILog Logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        
        DateTime _startUploadTime;
        int _licenseCount = 0;

        public FormUploadAudits(int licenseCount)
        {
            InitializeComponent();
            _licenseCount = licenseCount;

            lblProgress.Text = "Ready...";
        }

        /// <summary>
        /// Called as the form loads - initialize the default path from the current scanner configuration
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FormUploadAudits_Load(object sender, EventArgs e)
        {
            string lLastFolder = new SettingsDAO().GetSetting("LastUploadFolder", false);

            try
            {
                if (lLastFolder != "")
                    tbAuditFolder.Text = lLastFolder;
                else
                {
                    AuditScannerDefinition auditScannerDefinition = AuditWizardSerialization.DeserializeDefaultScannerObject();
                    tbAuditFolder.Text = auditScannerDefinition.DeployPathData;
                }

                if (tbAuditFolder.Text != String.Empty)
                {
                    watcher.Path = tbAuditFolder.Text;
                    watcher.NotifyFilter = NotifyFilters.LastWrite;

                    watcher.Filter = "*.adf";
                    watcher.Changed += new FileSystemEventHandler(watcher_Changed);
                    watcher.Deleted += new FileSystemEventHandler(watcher_Changed);

                    watcher.EnableRaisingEvents = true;

                    RefreshList();
                }
            }
            catch(ArgumentException ex)
            {
                Logger.Error(ex.Message);
            }
        }

        private void watcher_Changed(object source, FileSystemEventArgs e)
        {
            try
            {
                RefreshList();
            }
            catch (Exception)
            {
                
            }
        }

        /// <summary>
        /// This function is called as we change the text displayed in the audit folder field
        /// the idea here is that we start a timer which will refresh the list 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tbAuditFolder_TextChanged(object sender, EventArgs e)
        {

        }

        private void tbAuditFolder_Leave(object sender, EventArgs e)
        {
            try
            {
                watcher.Path = tbAuditFolder.Text;
            }
            catch (ArgumentException ex)
            {
                Logger.Error(ex.Message);
            }
        }

        /// <summary>
        /// Refresh the list of files in the specified folder
        /// </summary>
        private void RefreshList()
        {
            // Clear the list view initially
            lvAudits.BeginUpdate();
            lvAudits.Items.Clear();

            // If we have no data folder then we may as well just exit
            _uploadFolder = tbAuditFolder.Text;
            if (_uploadFolder != "")
            {
                // build a temporary list of matching files
                List<AuditFileInfo> listAuditFiles = new List<AuditFileInfo>();
                AuditUploader auditLoader = new AuditUploader(_uploadFolder, _licenseCount);
                auditLoader.EnumerateFiles(listAuditFiles);

                // sort the list by audit scan date
                listAuditFiles.Sort();

                UltraListViewItem[] items = new UltraListViewItem[listAuditFiles.Count];

                for (int i = 0; i < listAuditFiles.Count; i++)
                {
                    AuditFileInfo thisAuditFile = listAuditFiles[i];

                    UltraListViewSubItem[] subItemArray = new UltraListViewSubItem[3];
                    subItemArray[0] = new UltraListViewSubItem();
                    subItemArray[0].Value = thisAuditFile.Assetname;
                    subItemArray[1] = new UltraListViewSubItem();
                    subItemArray[1].Value = thisAuditFile.AuditDate.ToString();
                    subItemArray[2] = new UltraListViewSubItem();
                    subItemArray[2].Value = thisAuditFile.StatusAsText;
                    //
                    UltraListViewItem item = new UltraListViewItem(thisAuditFile.Filename, subItemArray);
                    item.Tag = thisAuditFile;
                    item.Key = thisAuditFile.AuditFile.FileName;

                    items[i] = item;
                }

                lvAudits.Items.AddRange(items);

                // Also build a list of LYNC audit files which should be uploaded
                LyncAuditFileList listLyncAudits = new LyncAuditFileList();
                listLyncAudits.Populate(_uploadFolder);

                // then add to the file list control
                foreach (LyncAuditFile lyncAuditFile in listLyncAudits.LyncAuditFiles)
                {
                    AuditDataFile thisAuditFile = lyncAuditFile.ConvertedFile();
                    if (thisAuditFile == null)
                        continue;

                    // We need an AuditFileInfo object for this file
                    AuditFileInfo auditFileInfo = new AuditFileInfo();
                    auditFileInfo.Assetname = thisAuditFile.AssetName;
                    auditFileInfo.AuditDate = thisAuditFile.AuditDate;
                    auditFileInfo.AuditFile = thisAuditFile;
                    auditFileInfo.Filename = lyncAuditFile.FileName;
                    
                    UltraListViewSubItem[] subItemArray = new UltraListViewSubItem[3];
                    subItemArray[0] = new UltraListViewSubItem();
                    subItemArray[0].Value = auditFileInfo.Assetname;
                    subItemArray[1] = new UltraListViewSubItem();
                    subItemArray[1].Value = auditFileInfo.AuditDate.ToString();
                    subItemArray[2] = new UltraListViewSubItem();
                    subItemArray[2].Value = auditFileInfo.StatusAsText;
                    //
                    UltraListViewItem item = new UltraListViewItem(auditFileInfo.Filename, subItemArray);
                    item.Tag = auditFileInfo;
                    item.Key = auditFileInfo.Filename;
                    lvAudits.Items.Add(item);
                }
            }

            lvAudits.EndUpdate();
        }

        /// <summary>
        /// Called when we change the selection in the list view
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void lvAudits_ItemSelectionChanged(object sender, ItemSelectionChangedEventArgs e)
        {
            bnUpload.Enabled = (lvAudits.SelectedItems.Count != 0);
        }

        /// <summary>
        /// The user has requested to browse for the data folder
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void bnBrowseSingle_Click(object sender, EventArgs e)
        {
            folderBrowserDialog1.SelectedPath = tbAuditFolder.Text;
            if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
                tbAuditFolder.Text = folderBrowserDialog1.SelectedPath;

            watcher.Path = tbAuditFolder.Text;

            RefreshList();
        }

        private void Form_DoubleClick(object sender, EventArgs e)
        {
            UploadAudits(true);
        }


        private void bnUpload_Click(object sender, EventArgs e)
        {
            UploadAudits(true);
        }

        private void bnUploadAll_Click(object sender, EventArgs e)
        {
            UploadAudits(false);
        }



        /// <summary>
        /// The cancel button is only available while an upload is in progress and causes the 
        /// upload to abort.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void bnCancel_Click(object sender, EventArgs e)
        {
            if (!this.backgroundWorkerUpload.CancellationPending)
            {
                this.backgroundWorkerUpload.CancelAsync();
                bnCancel.Enabled = false;
            }
        }


        /// <summary>
        /// This function is called when we actually want to upload audit data
        /// </summary>
        /// <param name="selectedOnly"></param>
        private void UploadAudits(bool selectedOnly)
        {
            // Build a list of the Audit Files that are to be uploaded	
            _listToUpload.Clear();

            if (selectedOnly)
            {
                foreach (UltraListViewItem item in lvAudits.SelectedItems)
                {
                    // The tag is the info object about the audit file
                    AuditFileInfo auditFileInfo = item.Tag as AuditFileInfo;
                    AuditDataFile auditDataFile = auditFileInfo.AuditFile;
                    if (auditDataFile != null)
                        _listToUpload.Add(auditDataFile);
                }
            }
            else
            {
                // Add all items
                foreach (UltraListViewItem item in lvAudits.Items)
                {
                    AuditFileInfo auditFileInfo = item.Tag as AuditFileInfo;
                    AuditDataFile auditDataFile = auditFileInfo.AuditFile;
                    if (auditDataFile != null)
                        _listToUpload.Add(auditDataFile);
                }
            }

            // Have we any items to upload?
            if (_listToUpload.Count == 0)
            {
                MessageBox.Show("There are no audits selected to upload", "Upload Error");
                return;
            }

            // Whilst the upload is running we must not allow the user to exit this form 
            bnClose.Visible = false;
            bnClose.Enabled = false;
            bnCancel.Visible = true;
            bnUpload.Enabled = false;
            bnUploadAll.Enabled = false;

            // Update the status bar text to show what's happening now
            lblProgress.Text = "Preparing to upload " + _listToUpload.Count.ToString() + " assets...";

            // We actually use the worker thread to perform the uploads for us so start it here
            _startUploadTime = DateTime.Now;
            this.backgroundWorkerUpload.RunWorkerAsync();
        }


        #region Upload Worker Thread functions

        /// <summary>
        /// This is the main processing loop for the worker thread
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void backgroundWorkerUpload_DoWork(object sender, DoWorkEventArgs e)
        {
            e.Result = 0;				// Assume success
            _uploadedFiles = new List<AuditDataFile>();

            // We can work on the '_listToUpload' data member as nothing will happen to it 
            // in the main UI thread as that thread is stalled waiting for the background thread
            // to complete 
            int uploading = 1;
            foreach (AuditDataFile dataFile in _listToUpload)
            {
                // If we have been requested to close then do so
                if (this.backgroundWorkerUpload.CancellationPending)
                    break;

                // Use the report progress facility to communicate to this control's UI thread to indicate another
                // audit file uploaded
                string statusText = String.Format("Uploading {0} of {1} Audits...", uploading, _listToUpload.Count);
                this.backgroundWorkerUpload.ReportProgress(0, statusText);

                // Now perform the upload of this data file
                try
                {
                    AuditUploader uploader = new AuditUploader(_uploadFolder, _licenseCount);
                    uploader.UploadAuditDataFile(dataFile);
                    _uploadedFiles.Add(dataFile);
                }

                // Any exceptions from the upload should be caught and reported - the exception will abort the 
                // upload process.
                catch (Exception ex)
                {
                    RefreshList();
                    statusText = "Upload failed: " + ex.Message;
                    this.backgroundWorkerUpload.ReportProgress(0, statusText);
                    e.Result = -1;
                    //break;
                }

                uploading++;
            }
        }


        /// <summary>
        /// This function is called as a result of the 'ReportProgress' function being called within the 
        /// background thread.  This function is called in the context of the UI thread and as such can 
        /// access UI elements.  Here we simply update the status bar text and check for a cancellation
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void backgroundWorkerUpload_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            lblProgress.Text = e.UserState.ToString();
        }


        /// <summary>
        /// The background worker has completed and as such we need to signal the UI thread of this fact
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void backgroundWorkerUpload_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            bnClose.Visible = true;
            bnClose.Enabled = true;
            bnCancel.Visible = false;
            bnUploadAll.Enabled = true;
            lvAudits.SelectedItems.Clear();

            // If we have been returned an error status then we do not display the summary but instead allow the user to see
            // the message reported.
            if ((int)e.Result == -1)
            {
                DesktopAlert.ShowDesktopAlert("Upload complete.");
            }
            else
            {
                DesktopAlert.ShowDesktopAlert("The selected Audit Data file(s) have been successfully uploaded.");

                foreach (AuditDataFile file in _uploadedFiles)
                {
                    foreach (UltraListViewItem listViewItem in lvAudits.Items)
                    {
                        if (listViewItem.Key == file.FileName)
                        {
                            lvAudits.Items.Remove(listViewItem);
                            break;
                        }
                    }
                }

                // Update the status bar
                lblProgress.Text = "Audit(s) uploaded.";
            }
        }

        #endregion

        private void FormUploadAudits_FormClosing(object sender, FormClosingEventArgs e)
        {
            new SettingsDAO().SetSetting("LastUploadFolder", tbAuditFolder.Text, false);
        }

        private void tbAuditFolder_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
                RefreshList();
        }
    }
}