using System;
using System.Windows.Forms;
using Layton.AuditWizard.DataAccess;
using Layton.Cab.Interface;
using Microsoft.Practices.CompositeUI;
using Microsoft.Practices.CompositeUI.SmartParts;
using Microsoft.Practices.ObjectBuilder;

namespace Layton.AuditWizard.Administration
{
    [SmartPart]
    public partial class UploadSettingsTabView : UserControl, ILaytonView, IAdministrationView
    {
        private LaytonWorkItem workItem;
        UploadSettingsTabViewPresenter presenter;
        private bool _uploadConfigurationChanged = false;

        [InjectionConstructor]
        public UploadSettingsTabView([ServiceDependency] WorkItem workItem)
        {
            this.workItem = workItem as LaytonWorkItem;
            InitializeComponent();
        }

        [CreateNew]
        public UploadSettingsTabViewPresenter Presenter
        {
            set { presenter = value; presenter.View = this; presenter.Initialize(); }
            get { return presenter; }
        }

        public void RefreshViewSinglePublisher()
        {
        }

        /// <summary>
        /// Refresh the current view
        /// </summary>
        public void RefreshView()
        {
            base.Refresh();
        }


        /// <summary>
        /// Called as this tab is activated to ensure that we display the latest possible data
        /// This function comes from the IAdministrationView Interface
        /// </summary>
        public void Activate()
        {
            InitializeUploadSettings();
        }

        public void Save()
        {
            SaveUploadTab();
        }

        public LaytonWorkItem WorkItem
        {
            get { return workItem; }
        }


        private void UploadTabView_Load(object sender, EventArgs e)
        {
            rbDeleteAfterUpload.Checked = true;

            cbGlobalAutoUpload.Checked = new SettingsDAO().GetSettingAsBoolean(DatabaseSettingsKeys.Setting_DisableAllUpolads, false);
            rbBackupAfterUpload.Checked = new SettingsDAO().GetSettingAsBoolean(DatabaseSettingsKeys.Setting_BackupAfterUpload, false);
            cbOverwriteUserData.Checked = new SettingsDAO().GetSettingAsBoolean(DatabaseSettingsKeys.Setting_OverwriteUserData, false);
            cbFindAssetByName.Checked = new SettingsDAO().GetSettingAsBoolean(DatabaseSettingsKeys.Setting_FindAssetByName, false);
        }


        private void InitializeUploadSettings()
        {
            
        }

        /// <summary>
        /// Called when we change the state of any fields on this tab to force a change on exit
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>

        private void uploadOptions_changed(object sender, EventArgs e)
        {
            _uploadConfigurationChanged = true;

            // Save the settings into the database
            SettingsDAO lwDataAccess = new SettingsDAO();
            lwDataAccess.SetSetting(DatabaseSettingsKeys.Setting_DeleteAfterUpload, rbDeleteAfterUpload.Checked.ToString(), false);
            lwDataAccess.SetSetting(DatabaseSettingsKeys.Setting_BackupAfterUpload, rbBackupAfterUpload.Checked.ToString(), false);
        }

        /// <summary>
        /// Called when we change the overwrite user data field on this tab to force a change on exit
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void overwriteUserData_Changed(object sender, EventArgs e)
        {
            _uploadConfigurationChanged = true;
            new SettingsDAO().SetSetting(DatabaseSettingsKeys.Setting_OverwriteUserData, cbOverwriteUserData.Checked.ToString(), false);
        }

        private void cbGlobalAutoUpload_CheckedChanged(object sender, EventArgs e)
        {
            _uploadConfigurationChanged = true;
            new SettingsDAO().SetSetting(DatabaseSettingsKeys.Setting_DisableAllUpolads, cbGlobalAutoUpload.Checked.ToString(), false);
        }

        private void cbFindAssetByName_CheckedChanged(object sender, EventArgs e)
        {
            _uploadConfigurationChanged = true;
            new SettingsDAO().SetSetting(DatabaseSettingsKeys.Setting_FindAssetByName, cbFindAssetByName.Checked.ToString(), false);
        }

        /// <summary>
        /// Save any data entered on the Upload tab
        /// </summary>
        private void SaveUploadTab()
        {
            // If we have made any changes, save them back to the database
            if (_uploadConfigurationChanged)
            {
                // Save the settings into the database
                SettingsDAO lwDataAccess = new SettingsDAO();
                lwDataAccess.SetSetting(DatabaseSettingsKeys.Setting_DeleteAfterUpload, rbDeleteAfterUpload.Checked.ToString(), false);
                lwDataAccess.SetSetting(DatabaseSettingsKeys.Setting_BackupAfterUpload, rbBackupAfterUpload.Checked.ToString(), false);
                lwDataAccess.SetSetting(DatabaseSettingsKeys.Setting_OverwriteUserData, cbOverwriteUserData.Checked.ToString(), false);
                lwDataAccess.SetSetting(DatabaseSettingsKeys.Setting_FindAssetByName, cbFindAssetByName.Checked.ToString(), false);
            }
        }
    }
}
