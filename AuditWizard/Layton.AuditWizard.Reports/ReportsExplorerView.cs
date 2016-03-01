using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Windows.Forms;
using Infragistics.Win.UltraWinSchedule;
using Layton.AuditWizard.Common;
using Layton.AuditWizard.DataAccess;
using Layton.Cab.Interface;
using Microsoft.Practices.CompositeUI;
using Microsoft.Practices.CompositeUI.SmartParts;
using Microsoft.Practices.ObjectBuilder;

namespace Layton.AuditWizard.Reports
{
    [SmartPart]
    public partial class ReportsExplorerView : UserControl, ILaytonView
    {
        private LaytonWorkItem workItem;
        private DateTime minDate = new DateTime(2000, 1, 1);

        /// <summary>
        /// The following fields maintian information about the assets and locations selected to be
        /// included in the report
        /// </summary>
        private AssetGroupList _listSelectedGroups = new AssetGroupList();
        private AssetList _listSelectedAssets = new AssetList();
        private RecurrenceDialog _recurrenceDialog;
        //private BackgroundWorker _worker;
        private FormProgress _progressForm = new FormProgress();

        [InjectionConstructor]
        public ReportsExplorerView([ServiceDependency] WorkItem workItem)
        {
            this.workItem = workItem as LaytonWorkItem;
            InitializeComponent();
            //InitializeControls();
        }

        public void RefreshView()
        {
            base.Refresh();
            Initialize();
        }

        public LaytonWorkItem WorkItem
        {
            get { return workItem; }
        }


        /// <summary>
        /// Initializes the date for the view, including the Users and Computers. 
        /// </summary>
        private void Initialize()
        {
            // Display those options/fields which are applicable to the currently loaded report
            InitializeReport();
        }

        /// <summary>
        /// Update the fields displayed on this form
        /// </summary>
        private void InitializeReport()
        {
            PopulateCustomReportComboBox();
            PopulateCompliantReportComboBox();
            PopulateSQLReportComboBox();

            cmbSoftwareReportType.SelectedIndex = (cmbSoftwareReportType.SelectedIndex == -1) ? 0 : cmbSoftwareReportType.SelectedIndex;
            cmbAssetReportType.SelectedIndex = (cmbAssetReportType.SelectedIndex == -1) ? 0 : cmbAssetReportType.SelectedIndex;
            cmbAuditReportType.SelectedIndex = (cmbAuditReportType.SelectedIndex == -1) ? 0 : cmbAuditReportType.SelectedIndex;
            cmbHardwareReportType.SelectedIndex = (cmbHardwareReportType.SelectedIndex == -1) ? 0 : cmbHardwareReportType.SelectedIndex;

            DataTable dataTable = new ApplicationsDAO().GetAllPublisherNamesAsDataTable(String.Empty);

            cmbPublishers.Items.Clear();

            foreach (DataRow row in dataTable.Rows)
            {
                cmbPublishers.Items.Add(row["_PUBLISHER"]);
            }

            if (cmbPublishers.Items.Count > 0)
                cmbPublishers.SelectedIndex = 0;
        }

        private void PopulateCustomReportComboBox()
        {
            int selectedIndex = cmbCustomReports.SelectedIndex;
            cmbCustomReports.DataSource = new ReportsDAO().GetReportsByType(ReportsDAO.ReportType.CustomReport);

            if (cmbCustomReports.Items.Count > 0)
            {
                cmbCustomReports.DisplayMember = "_REPORTNAME";
                cmbCustomReports.ValueMember = "_REPORTID";

                cmbCustomReports.SelectedIndex = (selectedIndex == -1 || selectedIndex > cmbCustomReports.Items.Count - 1) ? 0 : selectedIndex;
            }
        }

        private void PopulateCompliantReportComboBox()
        {
            int index = cmbComplianceReports.SelectedIndex;
            cmbComplianceReports.DataSource = new ReportsDAO().GetReportsByType(ReportsDAO.ReportType.ComplianceReport);

            if (cmbComplianceReports.Items.Count > 0)
            {
                cmbComplianceReports.DisplayMember = "_REPORTNAME";
                cmbComplianceReports.ValueMember = "_REPORTID";

                cmbComplianceReports.SelectedIndex = (index == -1 || index > cmbComplianceReports.Items.Count - 1) ? 0 : index;
            }
        }

        private void PopulateSQLReportComboBox()
        {
            int selectedIndex = cmbSQLReports.SelectedIndex;
            cmbSQLReports.DataSource = new ReportsDAO().GetReportsByType(ReportsDAO.ReportType.SqlReport);

            if (cmbSQLReports.Items.Count > 0)
            {
                cmbSQLReports.DisplayMember = "_REPORTNAME";
                cmbSQLReports.ValueMember = "_REPORTID";

                cmbSQLReports.SelectedIndex = (selectedIndex == -1 || selectedIndex > cmbSQLReports.Items.Count - 1) ? 0 : selectedIndex;
            }
        }

        /// <summary>
        /// Called before we act on the report - possibly running or saving the report to ensure that all
        /// applicable options for this report are saved.
        /// </summary>
        public void UpdateReportOptions()
        {
        }

        #region Report Generation Functions

        /// <summary>
        /// Generate a report targeted at Application Licensing
        /// </summary>
        protected void GeneratePresetReport(string reportName)
        {
            // Get our work item controller as some of the report filters are not specific to the report as such
            ReportsWorkItemController wiController = (ReportsWorkItemController)workItem.Controller;

            // Pass this report definition to the WIC who will in turn pass it to the TabView to display
            wiController.RunReport(reportName);
        }

        /// <summary>
        /// Generate a report targeted at Application Licensing
        /// </summary>
        protected void GenerateLicensingReport()
        {
            // Get our work item controller as some of the report filters are not specific to the report as such
            ReportsWorkItemController wiController = (ReportsWorkItemController)workItem.Controller;

            // Create the licensing report object
            LicensingReportDefinition report = (LicensingReportDefinition)wiController.CurrentReport;
            report.PublisherFilter = wiController.PublisherFilter;

            // Set show included/ignored applications flags
            report.ShowIgnoredApplications = wiController.ShowIgnoredApplications;
            report.ShowIncludedApplications = wiController.ShowIncludedApplications;

            // Pass this report definition to the WIC who will in turn pass it to the TabView to display
            wiController.RunReport(report);
        }

        /// <summary>
        /// Generate a report targeted at Internet Usage
        /// </summary>
        protected void GenerateInternetReport()
        {
            // Ensure any options are saved
            UpdateReportOptions();

            // Get our work item controller as some of the report filters are not specific to the report as such
            ReportsWorkItemController wiController = (ReportsWorkItemController)workItem.Controller;

            // Create the licensing report object
            InternetReportDefinition report = (InternetReportDefinition)wiController.CurrentReport;
            report.PublisherFilter = wiController.PublisherFilter;

            // Pass this report definition to the WIC who will in turn pass it to the TabView to display
            wiController.RunReport(report);
        }



        /// <summary>
        /// Generate a report targeted at Audit Trail / History Details
        /// </summary>
        protected void GenerateAuditTrailReport()
        {
            // Ensure any options are saved
            UpdateReportOptions();

            // Get our work item controller as some of the report filters are not specific to the report as such
            ReportsWorkItemController wiController = (ReportsWorkItemController)workItem.Controller;

            // Create the licensing report object
            HistoryReportDefinition report = (HistoryReportDefinition)wiController.CurrentReport;
            report.PublisherFilter = wiController.PublisherFilter;

            // Pass this report definition to the WIC who will in turn pass it to the TabView to display
            wiController.RunReport(report);
        }



        /// <summary>
        /// Generate a report targeted at File System Usage
        /// </summary>
        protected void GenerateFileSystemReport()
        {
            // Ensure any options are saved
            UpdateReportOptions();

            // Get our work item controller as some of the report filters are not specific to the report as such
            ReportsWorkItemController wiController = (ReportsWorkItemController)workItem.Controller;

            // Create the licensing report object
            FileSystemReportDefinition report = (FileSystemReportDefinition)wiController.CurrentReport;
            report.PublisherFilter = wiController.PublisherFilter;

            // Pass this report definition to the WIC who will in turn pass it to the TabView to display
            wiController.RunReport(report);
        }


        #endregion Report Generators

        #region Event Handlers

        /// <summary>
        /// Called by our Work Item Controller to inform us that the report has been changed
        /// </summary>
        public void ReportChanged()
        {
            // refresh our display for the new report
            InitializeReport();
        }


        /// <summary>
        /// Called as the report is saved - we need do nothing here
        /// </summary>
        public void ReportSaved()
        {
        }

        private void bnHardwareReportRun_Click(object sender, EventArgs e)
        {
            this.Cursor = Cursors.WaitCursor;
            GeneratePresetReport(cmbHardwareReportType.SelectedItem.ToString());
            this.Cursor = Cursors.Default;
        }

        private void bnSoftwareReportRun_Click(object sender, EventArgs e)
        {
            try
            {
                this.Cursor = Cursors.WaitCursor;

                string reportName = cmbSoftwareReportType.SelectedItem.ToString();

                if (cmbSoftwareReportType.SelectedItem.ToString() == "Application Compliance by Publisher" ||
                    cmbSoftwareReportType.SelectedItem.ToString() == "Over/Under Licensed by Publisher" ||
                    cmbSoftwareReportType.SelectedItem.ToString() == "License Keys by Publisher")
                {
                    if (cmbPublishers.Items.Count > 0)
                        reportName += "|" + cmbPublishers.SelectedItem.ToString();
                    else
                    {
                        this.Cursor = Cursors.Default;
                        return;
                    }
                }
                if (reportName == "Application By Location")
                {
                    RunApplicationByLocationOption(reportName);
                }
                else
                {
                    GeneratePresetReport(reportName);
                }
            }
            catch (Exception Ex)
            {
                MessageBox.Show(Ex.Message);
            }
            this.Cursor = Cursors.Default;
        }
        private void RunApplicationByLocationOption(string reportName)
        {
            //MessageBox.Show("Assets By Location Selected");
            //Hide the tab first
            //Display the asset selection dialog..
            FormAppByLocation AppByLocation = new FormAppByLocation();
            if (AppByLocation.ShowDialog() == DialogResult.OK)
            {
                //run the query based on for the recordset..

                string strQuery = "SELECT APP._PUBLISHER, APP._NAME + ' (' + APP._VERSION + ')' AS [Application & Version], A._NAME AS Asset, L._NAME AS Location " +
                "FROM APPLICATIONS APP " +
                "INNER JOIN APPLICATION_INSTANCES APPI ON (APP._APPLICATIONID = APPI._APPLICATIONID) " +
                "INNER JOIN ASSETS A ON (A._ASSETID = APPI._ASSETID) " +
                "INNER JOIN LOCATIONS L ON (A._LOCATIONID = L._LOCATIONID) " +
                "WHERE APP._IGNORED = 0 ";

                string strSelectedApplicationSQL = "";
                string strSelectedLocationSQL = "";

                if (AppByLocation.LastSavedSelectionItem.ApplicationItemList.Count > 0)
                {
                    strSelectedApplicationSQL = "AND (";

                    for (int i = 0; i < AppByLocation.LastSavedSelectionItem.ApplicationItemList.Count - 1; i++)
                    {
                        strSelectedApplicationSQL += "APP._NAME ='";
                        strSelectedApplicationSQL += AppByLocation.LastSavedSelectionItem.ApplicationItemList[i].ApplicationName;
                        strSelectedApplicationSQL += "' OR ";
                    }
                    strSelectedApplicationSQL += "APP._NAME =";
                    strSelectedApplicationSQL += AuditWizardDataAccess.PrepareSqlString(AppByLocation.LastSavedSelectionItem.ApplicationItemList[AppByLocation.LastSavedSelectionItem.ApplicationItemList.Count - 1].ApplicationName);
                    //strSelectedApplicationSQL += "";
                    strSelectedApplicationSQL += " )";

                    strQuery += strSelectedApplicationSQL;


                }
                if (AppByLocation.LastSavedSelectionItem.LocationNames.Count > 0)
                {
                    strSelectedLocationSQL = "AND (";

                    for (int i = 0; i < AppByLocation.LastSavedSelectionItem.LocationNames.Count - 1; i++)
                    {
                        strSelectedLocationSQL += "L._NAME ='";
                        strSelectedLocationSQL += AppByLocation.LastSavedSelectionItem.LocationNames[i];
                        strSelectedLocationSQL += "' OR ";
                    }
                    strSelectedLocationSQL += "L._NAME ='";
                    strSelectedLocationSQL += AppByLocation.LastSavedSelectionItem.LocationNames[AppByLocation.LastSavedSelectionItem.LocationNames.Count - 1];
                    strSelectedLocationSQL += "'";
                    strSelectedLocationSQL += " )";

                    strQuery += strSelectedLocationSQL;

                }
                strQuery += "ORDER BY L._NAME";

                ReportsWorkItemController wiController = (ReportsWorkItemController)workItem.Controller;
                wiController.RunSQLReport(reportName, strQuery);

                //Filter the recodset based on the dialog settings
                if (AppByLocation.SaveAsUserDefinedReport)
                {
                    List<string> _filterConditions = new List<string>();
                    _filterConditions.Add(strQuery);

                    BinaryFormatter bf = new BinaryFormatter();
                    MemoryStream mem = new MemoryStream();
                    bf.Serialize(mem, _filterConditions);
                    string lReportData = Convert.ToBase64String(mem.ToArray());
                    new ReportsDAO().Insert(AppByLocation.UserDefinedReportName, ReportsDAO.ReportType.SqlReport, lReportData);

                    RefreshView();
                    AppByLocation.SaveAllSQLProfilesFromTheList();
                }
                AppByLocation.SaveItemSelection();

                //Fill the Report grid
                //Show the grid to the user
            }
            else
            {

            }
        }
        private void bnAssetReportRun_Click(object sender, EventArgs e)
        {
            string reportName = cmbAssetReportType.SelectedItem.ToString();

            if (cmbAssetReportType.SelectedItem.ToString() == "Audit Trail History")
            {
                if (cbApplyDateFilter.Checked)
                    reportName += "|" + dtpStartDate.Value.ToShortDateString() + "|" + dtpEndDate.Value.ToShortDateString();

            }

            this.Cursor = Cursors.WaitCursor;
            GeneratePresetReport(reportName);
            this.Cursor = Cursors.Default;
        }

        private void bnAuditReportRun_Click(object sender, EventArgs e)
        {
            this.Cursor = Cursors.WaitCursor;
            GeneratePresetReport(cmbAuditReportType.SelectedItem.ToString());
            this.Cursor = Cursors.Default;
        }

        #endregion Event Handlers

        private void CreateNewCustomReport_Click(object sender, EventArgs e)
        {
            ReportsWorkItemController wiController = (ReportsWorkItemController)workItem.Controller;
            wiController.NewReport();
            RefreshView();
        }

        private void RunCustomReport_Click(object sender, EventArgs e)
        {
            if (cmbCustomReports.SelectedValue == null)
                return;

            DataTable reportsDataTable = new ReportsDAO().GetReportById(Convert.ToInt32(cmbCustomReports.SelectedValue));
            string lReportData = reportsDataTable.Rows[0][2].ToString();
            string reportName = ((DataRowView) (cmbCustomReports.SelectedItem)).Row.ItemArray[1].ToString();

            BinaryFormatter bf = new BinaryFormatter();
            MemoryStream mem = new MemoryStream(Convert.FromBase64String(lReportData));

            List<string> lReportDataList = (List<string>)bf.Deserialize(mem);

            // is this a 'group results by asset' report
            bool lDisplayAsAssetRegister = false;

            foreach (string lReportCondition in lReportDataList)
            {
                if (lReportCondition.StartsWith("ASSET_REGISTER:"))
                {
                    lDisplayAsAssetRegister = Convert.ToBoolean(lReportCondition.Substring(15));
                    break;
                }
            }

            ReportsWorkItemController wiController = (ReportsWorkItemController)workItem.Controller;
            wiController.RunCustomReport(reportName, lReportDataList);

            cbAssetRegister1.Checked = lDisplayAsAssetRegister;
        }

        private void cmbSoftwareReportType_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbSoftwareReportType.SelectedItem.ToString() == "Application Compliance by Publisher" ||
                cmbSoftwareReportType.SelectedItem.ToString() == "Over/Under Licensed by Publisher" ||
                cmbSoftwareReportType.SelectedItem.ToString() == "License Keys by Publisher")
            {
                lblSelectPublisher.Visible = true;
                cmbPublishers.Visible = true;
            }
            else
            {
                lblSelectPublisher.Visible = false;
                cmbPublishers.Visible = false;
            }
        }

        private void bnComplianceReportRun_Click(object sender, EventArgs e)
        {
            FormComplianceReport form = new FormComplianceReport((ReportsWorkItemController)workItem.Controller);
            form.ShowDialog();
            RefreshView();
        }

        private void cbAssetRegister1_CheckedChanged(object sender, EventArgs e)
        {
            ReportsTabView tabView = WorkItem.TabView as ReportsTabView;
            tabView.RunCustomReport(cbAssetRegister1.Checked);

            //System.Threading.Thread thread = new System.Threading.Thread(RunReportOnThread);
            //thread.IsBackground = true;
            //thread.Start(); 

            //if (!_worker.IsBusy)
            //    _worker.RunWorkerAsync();
            //else
            //    MessageBox.Show("AuditWizard is currently processing another report.");
        }

        private void bnEditCustomReport_Click(object sender, EventArgs e)
        {
            if (cmbCustomReports.SelectedValue == null)
                return;

            FormCustomReport form = new FormCustomReport((ReportsWorkItemController)workItem.Controller, (int)cmbCustomReports.SelectedValue);
            form.ShowDialog();
            RefreshView();
        }

        private void bnDeleteCustomReport_Click(object sender, EventArgs e)
        {
            if (cmbCustomReports.SelectedValue == null)
                return;

            if (MessageBox.Show(
                "Are you sure you want to delete this custom report?",
                "Confirm delete",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question) == DialogResult.No)
            {
                return;
            }
            else
            {
                new ReportsDAO().Delete((int)cmbCustomReports.SelectedValue);
                RefreshView();
            }
        }

        private void bnSaveReport_Click(object sender, EventArgs e)
        {
            ReportsWorkItemController wiController = (ReportsWorkItemController)workItem.Controller;
            wiController.SaveReport();
            RefreshView();
        }

        private void bnEditExistingReport_Click(object sender, EventArgs e)
        {
            if (cmbComplianceReports.SelectedItem != null)
            {
                int lRepordId = Convert.ToInt32(cmbComplianceReports.SelectedValue);

                ReportsWorkItemController wiController = (ReportsWorkItemController)workItem.Controller;
                wiController.EditExistingComplianceReport(lRepordId);
                RefreshView();
            }
        }

        private void bnRunExistingComplianceReport_Click(object sender, EventArgs e)
        {
            if (cmbComplianceReports.SelectedItem != null)
            {
                int lRepordId = Convert.ToInt32(cmbComplianceReports.SelectedValue);

                ReportsWorkItemController wiController = (ReportsWorkItemController)workItem.Controller;
                wiController.RunExistingComplianceReport(lRepordId);
            }
        }

        private void bnDeleteExistingReport_Click(object sender, EventArgs e)
        {
            if (cmbComplianceReports.SelectedValue == null)
                return;

            if (MessageBox.Show(
                "Are you sure you want to delete this compliance report?",
                "Confirm delete",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question) == DialogResult.No)
            {
                return;
            }
            else
            {
                new ReportsDAO().Delete((int)cmbComplianceReports.SelectedValue);
                RefreshView();
            }
        }

        private void cbApplyDateFilter_CheckedChanged(object sender, EventArgs e)
        {
            lbStartDate.Enabled = cbApplyDateFilter.Checked;
            lbEndDate.Enabled = cbApplyDateFilter.Checked;
            dtpStartDate.Enabled = cbApplyDateFilter.Checked;
            dtpEndDate.Enabled = cbApplyDateFilter.Checked;
        }

        private void cmbAssetReportType_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbAssetReportType.SelectedItem.ToString() == "Audit Trail History")
                pnlDateFilter.Visible = true;
            else
                pnlDateFilter.Visible = false;
        }

        private AppointmentRecurrence DefineReportRecurrence(Appointment aRootAppointment)
        {
            aRootAppointment.Recurrence = new AppointmentRecurrence();
            aRootAppointment.Recurrence.PatternFrequency = RecurrencePatternFrequency.Weekly;
            aRootAppointment.Recurrence.PatternInterval = 1;
            aRootAppointment.Recurrence.RangeLimit = RecurrenceRangeLimit.NoLimit;

            _recurrenceDialog = new RecurrenceDialog(aRootAppointment, aRootAppointment.Recurrence, true, false, false);
            _recurrenceDialog.Text = "Schedule for " + aRootAppointment.Subject;
            _recurrenceDialog.ShowDialog();

            return _recurrenceDialog.Recurrence;
        }

        private void CreateNewSQLReport_Click(object sender, EventArgs e)
        {
            FormSQLReport form = new FormSQLReport();
            if ((form.ShowDialog() == DialogResult.OK) && (form.SQLString != null))
            {
                ReportsWorkItemController wiController = (ReportsWorkItemController)workItem.Controller;
                wiController.RunSQLReport(form.ReportName, form.SQLString);
                RefreshView();
            }
        }

        private void bnRunSQLReport_Click(object sender, EventArgs e)
        {
            if (cmbSQLReports.SelectedValue == null)
                return;

            DataTable reportsDataTable = new ReportsDAO().GetReportById(Convert.ToInt32(cmbSQLReports.SelectedValue));
            string lReportData = reportsDataTable.Rows[0][2].ToString();
            string reportName = ((DataRowView)(cmbSQLReports.SelectedItem)).Row.ItemArray[1].ToString();

            BinaryFormatter bf = new BinaryFormatter();
            MemoryStream mem = new MemoryStream(Convert.FromBase64String(lReportData));
            List<string> lReportDataList = (List<string>)bf.Deserialize(mem);

            ReportsWorkItemController wiController = (ReportsWorkItemController)workItem.Controller;
            wiController.RunSQLReport(reportName, lReportDataList[0]);
        }

        private void bnEditSQLReport_Click(object sender, EventArgs e)
        {
            if (cmbSQLReports.SelectedValue == null)
                return;

            FormSQLReport form = new FormSQLReport(Convert.ToInt32(cmbSQLReports.SelectedValue));
            if (form.ShowDialog() == DialogResult.OK)
            {
                ReportsWorkItemController wiController = (ReportsWorkItemController)workItem.Controller;
                wiController.RunSQLReport(form.ReportName, form.SQLString);
            }
        }
        private void DeleteReportsFromLocalProfile(string strSQLProfileName)
        {
            try
            {
                FormAppByLocation AppByLocation = new FormAppByLocation();
                AppByLocation.LoadAllSavedSQLProfiles();
                if (AppByLocation.DeleteFromProfileList(strSQLProfileName))
                {
                    AppByLocation.SaveAllSQLProfilesFromTheList();
                }
            }
            catch (Exception)
            {

            }

        }
        private void bnDeleteSQLReport_Click(object sender, EventArgs e)
        {
            if (cmbSQLReports.SelectedValue == null)
                return;
            string strReportName = cmbSQLReports.Text;

            if (MessageBox.Show(
                "Are you sure you want to delete this custom report?",
                "Confirm delete",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question) == DialogResult.No)
            {
                return;
            }
            else
            {
                new ReportsDAO().Delete((int)cmbSQLReports.SelectedValue);
                RefreshView();
                DeleteReportsFromLocalProfile(strReportName);

            }
        }
    }
}
