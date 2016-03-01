using System;
using System.IO;
using System.Windows.Forms;
using System.Xml.Serialization;
using Microsoft.Practices.CompositeUI.SmartParts;
using Layton.Cab.Interface;
using Microsoft.Practices.CompositeUI;
using Microsoft.Practices.ObjectBuilder;
//
using Infragistics.Win.UltraWinListView;
//
using Layton.AuditWizard.Common;
using Layton.AuditWizard.DataAccess;

namespace Layton.AuditWizard.Administration
{
    [SmartPart]
    public partial class AlertMonitorSettingsTabView : UserControl, ILaytonView, IAdministrationView
    {
        #region Setup

        private LaytonWorkItem workItem;
		AlertMonitorSettingsTabViewPresenter presenter;
		
        [InjectionConstructor]
		public AlertMonitorSettingsTabView([ServiceDependency] WorkItem workItem)
        {
            this.workItem = workItem as LaytonWorkItem;
            InitializeComponent();
        }

		[CreateNew]
		public AlertMonitorSettingsTabViewPresenter Presenter
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
			InitializeTab();
		}

		/// <summary>
		/// save function for the IAdministrationView Interface
		/// </summary>
        public void Save()
        {
        }
		
		public LaytonWorkItem WorkItem
        {
            get { return workItem; }
        }

		/// <summary>
		/// Initialize this tab
		/// </summary>
		private void InitializeTab()
		{
            RefreshAlertMonitors();

			// Set the alert monitor email settings
			SettingsDAO lSettingsDao = new SettingsDAO();

			// Get time first to prevent it from being reset
			string time = lSettingsDao.GetSetting(DatabaseSettings.Setting_AlertMonitorEmailTime, false);
			if (time == "")
				time = "18:00";
			DateTime startTime = DateTime.Parse(time);
			udteEmailAtTime.DateTime = startTime;
			//
			string mailFrequency = lSettingsDao.GetSetting(DatabaseSettings.Setting_AlertMonitorEmailFrequency, false);
			hourlyRadioButton.Checked = ((mailFrequency == "") || (mailFrequency == DatabaseSettings.Setting_AlertMonitorEmailHourly));
			dailyRadioButton.Checked = !hourlyRadioButton.Checked;
        }

        #endregion

        #region Alert Definition Functions

        private void RefreshAlertMonitors()
        {
            try
            {
                lvAlertDefinitions.Items.Clear();

                // Get the path to the scanner configurations
                string scannerPath = Path.Combine(Application.StartupPath, "scanners\\alertmonitors\\");

                DirectoryInfo di = new DirectoryInfo(scannerPath);
                FileInfo[] rgFiles = di.GetFiles("*.xml");
                foreach (FileInfo fi in rgFiles)
                {
                    string selectedFileName = Path.Combine(Application.StartupPath, "scanners\\alertmonitors\\") + fi.Name;
                    AlertDefinition alertDefinition = GetAlertDefinitionFromFileName(selectedFileName);

                    UltraListViewSubItem[] subItemArray = new UltraListViewSubItem[2];
                    subItemArray[0] = new UltraListViewSubItem();
                    subItemArray[0].Value = alertDefinition.Description;
                    subItemArray[1] = new UltraListViewSubItem();
                    subItemArray[1].Value = fi.LastWriteTime.ToShortDateString() + " " + fi.LastWriteTime.ToShortTimeString();
                    //
                    UltraListViewItem item = new UltraListViewItem(alertDefinition.Name, subItemArray);
                    item.Tag = alertDefinition;

                    if (!lvAlertDefinitions.Items.Contains(item))
                        lvAlertDefinitions.Items.Add(item);
                }
            }

            catch (Exception ex)
            {
                //MessageBox.Show(ex.Message);
            }
        }

        private AlertDefinition GetAlertDefinitionFromFileName(string fileName)
        {
            TextReader textReader = null;
            AlertDefinition alertDefinition = null;
            try
            {
                XmlSerializer serializer = new XmlSerializer(typeof(AlertDefinition));
                textReader = new StreamReader(fileName);
                alertDefinition = (AlertDefinition)serializer.Deserialize(textReader);
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred whilst trying to load the alert definition : " + Environment.NewLine + Environment.NewLine + ex.Message,
                    "AuditWizard", MessageBoxButtons.OK, MessageBoxIcon.Error);

                // JML TODO log this exception
                return null;
            }
            finally
            {
                // close text writer
                textReader.Close();
            }

            return alertDefinition;
        }

		#endregion Alert Definition Functions

        #region Event Handlers

        /// <summary>
        /// Called as we change the item(s) selected within the list view of alert definitions.
        /// Enable Edit and Delete if an alert definition has been selected (single selection only supported here)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void lvAlerts_ItemSelectionChanged(object sender, Infragistics.Win.UltraWinListView.ItemSelectionChangedEventArgs e)
        {
            bnDelete.Enabled = (lvAlertDefinitions.SelectedItems.Count != 0);
            btnEditAlert.Enabled = (lvAlertDefinitions.SelectedItems.Count != 0);
        }

        /// <summary>
        /// Create a new Alert Definition
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void bnCreate_Click(object sender, EventArgs e)
        {
            FormAlertMonitorWizard form = new FormAlertMonitorWizard();
            form.ShowDialog();

            RefreshAlertMonitors();
        }


        /// <summary>
        /// Delete an alert definition from the list
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void bnDelete_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Are you sure that you want to delete this Alert Definition?", "Confirm Delete", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                UltraListViewItem lvi = lvAlertDefinitions.SelectedItems[0];
                int index = lvi.Index;
                //
                // Remove the item from the list view
                lvAlertDefinitions.Items.Remove(lvi);

                string selectedFileName = Path.Combine(Application.StartupPath, "scanners\\alertmonitors\\") + lvi.Text + ".xml";

                if (File.Exists(selectedFileName))
                    File.Delete(selectedFileName);

                // Select the next definition in the list if any
                if (index >= lvAlertDefinitions.Items.Count)
                    index--;
                if (index >= 0)
                    lvAlertDefinitions.SelectedItems.Add(lvAlertDefinitions.Items[index]);
            }
        }

        private void btnEditAlert_Click(object sender, EventArgs e)
        {
            TextReader textReader = null;
            FormAlertMonitorWizard form = null;
            string fileName = String.Empty;
            try
            {
                UltraListViewItem lvi = lvAlertDefinitions.SelectedItems[0];
                int selectedIndex = lvi.Index;
                if (selectedIndex != -1)
                {
                    fileName = lvAlertDefinitions.Items[selectedIndex].Text;

                    string selectedFileName = Path.Combine(Application.StartupPath, "scanners\\alertmonitors\\") + fileName + ".xml";

                    XmlSerializer serializer = new XmlSerializer(typeof(AlertDefinition));
                    textReader = new StreamReader(selectedFileName);

                    form = new FormAlertMonitorWizard((AlertDefinition)serializer.Deserialize(textReader));
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred whilst trying to load the alert definition : " + Environment.NewLine + Environment.NewLine + ex.Message,
                    "AuditWizard", MessageBoxButtons.OK, MessageBoxIcon.Error);
                // JML TODO log this exception
            }
            finally
            {
                // close text writer
                textReader.Close();
            }

            if (form != null)
                form.ShowDialog();

            RefreshAlertMonitors();
        }


		/// <summary>
		/// v8.3.3
		/// Called as we change the daily/hourly email selector
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void hourlyRadioButton_CheckedChanged(object sender, EventArgs e)
		{
			panelEmailAt.Visible = (dailyRadioButton.Checked);

			// Save the change
			SettingsDAO lSettingsDao = new SettingsDAO();
			if (dailyRadioButton.Checked)
			{
				lSettingsDao.SetSetting(DatabaseSettings.Setting_AlertMonitorEmailFrequency, DatabaseSettings.Setting_AlertMonitorEmailDaily, false);
				lSettingsDao.SetSetting(DatabaseSettings.Setting_AlertMonitorEmailTime, udteEmailAtTime.DateTime.ToString("HH:mm"), false);
			}
			else
			{
				lSettingsDao.SetSetting(DatabaseSettings.Setting_AlertMonitorEmailFrequency, DatabaseSettings.Setting_AlertMonitorEmailHourly, false);
			}
		}


		private void udteEmailAtTime_ValueChanged(object sender, EventArgs e)
		{
			SettingsDAO lSettingsDao = new SettingsDAO();
			lSettingsDao.SetSetting(DatabaseSettings.Setting_AlertMonitorEmailTime, udteEmailAtTime.DateTime.ToString("HH:mm"), false);
		}

        #endregion
	}
}
