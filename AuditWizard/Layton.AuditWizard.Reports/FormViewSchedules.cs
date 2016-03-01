using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Infragistics.Win.UltraWinGrid;
using Infragistics.Win.UltraWinSchedule;
using Layton.AuditWizard.DataAccess;
using Layton.AuditWizard.Common;

namespace Layton.AuditWizard.Reports
{
    public partial class FormViewSchedules : Form
    {
        private static ReportSchedulesDAO lReportSchedulesDAO;
        private RecurrenceDialog _recurrenceDialog;
        private string _selectedAssetIds;

        public FormViewSchedules()
        {
            InitializeComponent();
            lReportSchedulesDAO = new ReportSchedulesDAO();
            _selectedAssetIds = "";
        }

        private void FormViewSchedules_Load(object sender, EventArgs e)
        {
            PopulateGrid();
            ultraGridReportSchedules.DisplayLayout.Bands[0].Columns[0].Hidden = true;
            ultraGridReportSchedules.DisplayLayout.Bands[0].Columns[1].Width = 140;
            ultraGridReportSchedules.DisplayLayout.Bands[0].Columns[2].Width = 140;
            ultraGridReportSchedules.DisplayLayout.Bands[0].Columns[3].CellAppearance.TextTrimming = Infragistics.Win.TextTrimming.EllipsisWord;
            ultraGridReportSchedules.DisplayLayout.Bands[0].Columns[3].CellMultiLine = Infragistics.Win.DefaultableBoolean.True;
            ultraGridReportSchedules.DisplayLayout.Override.RowSizing = Infragistics.Win.UltraWinGrid.RowSizing.AutoFixed;

            if (comboBoxReportCategory.Items.Count > 0)
                comboBoxReportCategory.SelectedIndex = 0;

            cmbPublishers.DataSource = new ApplicationsDAO().GetPublishers(String.Empty);
            cmbPublishers.DisplayMember = "_PUBLISHER";

            if (cmbPublishers.Items.Count > 0)
                cmbPublishers.SelectedIndex = 0;

            CheckServiceRunning();
            PopulateLocationLabel();
        }

        private void PopulateLocationLabel()
        {
            string lNumberLocations = (_selectedAssetIds == "") ? "all" : _selectedAssetIds.Split(',').Length.ToString();
            tbLocations.Text = String.Format("You have currently selected {0} asset(s).", lNumberLocations);
        }

        private void CheckServiceRunning()
        {
            bool lServiceNotRunning = true;

            foreach (System.Diagnostics.Process process in System.Diagnostics.Process.GetProcesses())
            {
                if (process.ProcessName == "AuditWizardService")
                {
                    lServiceNotRunning = false;
                    break;
                }
            }

            lblServiceRunning.Visible = lServiceNotRunning;
        }

        private void PopulateGrid()
        {
            DataTable lApptTable = new ReportSchedulesDAO().GetAllAppointments();

            DataTable lResultsDataTable = new DataTable();
            lResultsDataTable.Columns.Add("ReportID", typeof(int));
            lResultsDataTable.Columns.Add("Report Category", typeof(string));
            lResultsDataTable.Columns.Add("Report Name", typeof(string));
            lResultsDataTable.Columns.Add("Schedule", typeof(string));

            foreach (DataRow row in lApptTable.Rows)
            {
                object rawAppointmentData = row["_APPOINTMENTDATA"];

                if (rawAppointmentData is byte[] == false)
                    continue;

                byte[] appointmentBytes = rawAppointmentData as byte[];

                //	Create an Appointment from the byte array, using the
                //	Appointment's static 'FromBytes' method.
                Appointment appointment = Appointment.FromBytes(appointmentBytes);

                string lRecurrenceDescription = (appointment.Recurrence == null) ? "" : appointment.Recurrence.Description;
                string lReportCategory = "";
                string lReportName = appointment.Subject;

                if (appointment.Subject.Split('|').Length == 2)
                {
                    lReportCategory = appointment.Subject.Split('|')[0];
                    lReportName = appointment.Subject.Split('|')[1];
                }
                else if (appointment.Subject.Split('|').Length == 3)
                {
                    lReportCategory = appointment.Subject.Split('|')[0];
                    lReportName = appointment.Subject.Split('|')[1] + " | " + appointment.Subject.Split('|')[2];
                }
                lResultsDataTable.Rows.Add((int)row[0], lReportCategory, lReportName, lRecurrenceDescription);
            }

            ultraGridReportSchedules.DataSource = lResultsDataTable;
        }

        private void bnEditSchedule_Click(object sender, EventArgs e)
        {
            EditSchedule();
        }

        private void EditSchedule()
        {
            if (ultraGridReportSchedules.Selected.Rows.Count == 1)
            {
                int selectedAppointmentId = (int)ultraGridReportSchedules.Selected.Rows[0].Cells[0].Value;
                DataTable selectedAppointment = lReportSchedulesDAO.GetAppointmentById(selectedAppointmentId);

                object rawAppointmentData = selectedAppointment.Rows[0][1];

                if (rawAppointmentData is byte[] == false)
                    return;

                byte[] appointmentBytes = rawAppointmentData as byte[];
                Appointment appointment = Appointment.FromBytes(appointmentBytes);

                FormEditReportSchedule form = new FormEditReportSchedule(appointment);
                if (form.ShowDialog() == DialogResult.OK)
                {
                    lReportSchedulesDAO.Update(selectedAppointmentId, form.EditedAppointment.Save());
                    PopulateGrid();
                }
            }
            else
            {
                MessageBox.Show("Please select one schedule from the table.", "Report Schedules", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void bnDeleteSchedule_Click(object sender, EventArgs e)
        {
            if (ultraGridReportSchedules.Selected.Rows.Count > 0)
            {
                string message = (ultraGridReportSchedules.Selected.Rows.Count == 1) ? 
                    "Are you sure you want to delete this report schedule?" : "Are you sure you want to delete these report schedules";

                if (MessageBox.Show(message, "Confirm schedule delete", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
                    return;

                foreach (UltraGridRow selectedrow in ultraGridReportSchedules.Selected.Rows)
                {
                    lReportSchedulesDAO.DeleteAppt((int)selectedrow.Cells[0].Value);
                }

                PopulateGrid();
            }
        }

        private void ultraGridReportSchedules_AfterSelectChange(object sender, Infragistics.Win.UltraWinGrid.AfterSelectChangeEventArgs e)
        {
            bnDeleteSchedule.Enabled = ultraGridReportSchedules.Selected.Rows.Count > 0;
            bnEditSchedule.Enabled = ultraGridReportSchedules.Selected.Rows.Count == 1;
        }

        private void ultraGridReportSchedules_DoubleClickRow(object sender, Infragistics.Win.UltraWinGrid.DoubleClickRowEventArgs e)
        {
            EditSchedule();
        }

        private void comboBoxReportCategory_SelectedIndexChanged(object sender, EventArgs e)
        {
            comboBoxReportName.DataSource = null;
            PopulateReportTypeComboBox(comboBoxReportCategory.SelectedItem.ToString());
            Invalidate();
            comboBoxReportCategory.Refresh();
            comboBoxReportCategory.Invalidate();
        }

        private void PopulateReportTypeComboBox(string aReportCategory)
        {
            groupBox4.Enabled = true;
            comboBoxReportName.Items.Clear();

            switch (aReportCategory)
            {
                case "Hardware":
                    comboBoxReportName.Items.AddRange(new object[] {
                    "Top 10 System Processors",
                    "System RAM Capacity",
                    "Processor Speeds",
                    "Assets By Type",
                    "Top 15 Manufacturers"});
                    break;

                case "Software":
                    comboBoxReportName.Items.AddRange(new object[] {
                    "Application Licensing",
                    "Operating systems",
                    "Top 15 Software Vendors",
                    "Microsoft Office Versions",
                    "Support Expiry Date",
                    "License Keys by Publisher",
                    "Over-Licensed Applications",
                    "Under-Licensed Applications",
                    "Overall Application Compliance",
                    "Application Compliance by Publisher",
                    "Over/Under Licensed by Publisher",
                    "Default Internet Browsers",
                    "Internet Browsers by Asset"});
                    break;

                case "Asset Management":
                    comboBoxReportName.Items.AddRange(new object[] {
                    "Asset status",
                    "Asset by location",
                    "Internet History",
                    "File System",
                    "Audit Trail History"});
                    break;

                case "Audit Status":
                    comboBoxReportName.Items.AddRange(new object[] {
                    "Audited / Unaudited",
                    "Agent deployment status",
                    "Discovery agent versions"});
                    break;

                case "Custom":
                    comboBoxReportName.DataSource = new ReportsDAO().GetReportsByType(ReportsDAO.ReportType.CustomReport);
                    comboBoxReportName.DisplayMember = "_REPORTNAME";
                    comboBoxReportName.ValueMember = "_REPORTID";
                    break;

                case "Compliance":
                    comboBoxReportName.DataSource = new ReportsDAO().GetReportsByType(ReportsDAO.ReportType.ComplianceReport); ;
                    comboBoxReportName.DisplayMember = "_REPORTNAME";
                    comboBoxReportName.ValueMember = "_REPORTID";
                    break;

                case "User-Defined SQL":
                    comboBoxReportName.DataSource = new ReportsDAO().GetReportsByType(ReportsDAO.ReportType.SqlReport); ;
                    comboBoxReportName.DisplayMember = "_REPORTNAME";
                    comboBoxReportName.ValueMember = "_REPORTID";
                    groupBox4.Enabled = false;
                    break;

                default:
                    break;
            }

            if (comboBoxReportName.Items.Count > 0)
                comboBoxReportName.SelectedIndex = 0;
        }

        private void bnCreateSchedule_Click(object sender, EventArgs e)
        {
            DateTime dt = DateTime.Now;
            Appointment rootAppt = new Appointment(dt, dt);
            string lReportCategory = comboBoxReportCategory.Text;
            string lReportName = comboBoxReportName.Text;

            if (lReportCategory == "Custom" || lReportCategory == "Compliance" || lReportCategory == "User-Defined SQL")
            {
                rootAppt.Description = comboBoxReportName.SelectedValue.ToString();
            }

            rootAppt.Subject = String.Format("{0} Report | {1}", lReportCategory, lReportName);
            rootAppt.Subject = (cmbPublishers.Visible) ? rootAppt.Subject + " | " + cmbPublishers.Text : rootAppt.Subject;

            // an empty lSelectedAssets means that all assets have been selected
            if (_selectedAssetIds == "")
            {
                DataTable lAllAssetDataTable = new AssetDAO().GetAllAssetIds();
                foreach (DataRow assetRow in lAllAssetDataTable.Rows)
                {
                    _selectedAssetIds += assetRow[0].ToString() + ",";
                }

                _selectedAssetIds = _selectedAssetIds.TrimEnd(',');
            }

            rootAppt.Location = _selectedAssetIds;
            rootAppt.Recurrence = DefineReportRecurrence(rootAppt);

            if (rootAppt.Recurrence != null)
            {
                if (rootAppt.Recurrence.OccurrenceStartTime < DateTime.Now)
                {
                    DateTime lDateTime = rootAppt.Recurrence.OccurrenceStartTime;

                    if (lDateTime.Minute == 59)
                    {
                        rootAppt.Recurrence.OccurrenceStartTime =
                                new DateTime(lDateTime.Year, lDateTime.Month, lDateTime.Day, lDateTime.Hour + 1, 0, 0);
                    }
                    else
                    {
                        rootAppt.Recurrence.OccurrenceStartTime =
                                new DateTime(lDateTime.Year, lDateTime.Month, lDateTime.Day, lDateTime.Hour, lDateTime.Minute + 1, 0);
                    }
                }

                new ReportSchedulesDAO().Insert(rootAppt.Save());
                PopulateGrid();
            }
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

        private void comboBoxReportName_SelectedIndexChanged(object sender, EventArgs e)
        {
            bnCreateSchedule.Enabled = (comboBoxReportName.SelectedIndex != -1);

            if (comboBoxReportName.Text == "Application Compliance by Publisher" ||
                comboBoxReportName.Text == "Over/Under Licensed by Publisher" ||
                comboBoxReportName.Text == "License Keys by Publisher")
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

        private void bnClose_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void bnLocationFilter_Click(object sender, EventArgs e)
        {
            //FormSelectLocations form = new FormSelectLocations("", new AssetDAO().ConvertIdListToNames(_selectedAssetIds, ','));
            FormSelectLocations form = new FormSelectLocations("", _selectedAssetIds);

            if (form.ShowDialog() == DialogResult.OK)
            {
                //string lAssetIdList = new AssetDAO().ConvertNameListToIds(form.SelectedAssetIds);
                string lAssetIdList = form.SelectedAssetIds;
                lAssetIdList = lAssetIdList.Replace(';', ',');
                _selectedAssetIds = lAssetIdList;
                PopulateLocationLabel();
            }
        }

        private void bnDeleteAllSchedules_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show(
                "Are you sure you want to delete all expired schedules?",
                "Delete schedules",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question) == DialogResult.Yes)
            {
                int lCounter = 0;
                foreach (DataRow row in lReportSchedulesDAO.GetAllAppointments().Rows)
                {
                    object rawAppointmentData = row["_APPOINTMENTDATA"];

                    if (rawAppointmentData is byte[] == false)
                        continue;

                    Appointment appointment = Appointment.FromBytes(rawAppointmentData as byte[]);

                    if (appointment.Recurrence.RangeEndDate < DateTime.Now.AddSeconds(-62))
                    {
                        lReportSchedulesDAO.DeleteAppt(Convert.ToInt32(row["_REPORTSCHEDULEID"]));
                        lCounter++;
                    }
                }

                string message = String.Format("Deleted {0} expired report schedules.", lCounter);

                if (lCounter == 0)
                    message = "There were no expired report schedules to delete.";

                DesktopAlert.ShowDesktopAlert(message);

                //MessageBox.Show(
                //    message,
                //    "Delete report schedules",
                //    MessageBoxButtons.OK,
                //    MessageBoxIcon.Information);

                PopulateGrid();
            }
        }

        private void ultraGridReportSchedules_InitializeRow(object sender, Infragistics.Win.UltraWinGrid.InitializeRowEventArgs e)
        {
            int selectedAppointmentId = Convert.ToInt32(e.Row.Cells[0].Value);
            DataTable selectedAppointment = lReportSchedulesDAO.GetAppointmentById(selectedAppointmentId);

            object rawAppointmentData = selectedAppointment.Rows[0][1];

            if (rawAppointmentData is byte[] == false)
                return;

            byte[] appointmentBytes = rawAppointmentData as byte[];
            Appointment appointment = Appointment.FromBytes(appointmentBytes);

            if (appointment.Recurrence.RangeEndDate < DateTime.Now.AddSeconds(-62))
                e.Row.Appearance.ForeColor = Color.Red;

            else if (appointment.Recurrence.RangeStartDate > DateTime.Now)
                e.Row.Appearance.ForeColor = Color.Orange;

            else
                e.Row.Appearance.ForeColor = Color.Green;
        }

        private void lblServiceRunning_Click(object sender, EventArgs e)
        {
            if (lblServiceRunning.Visible)
            {
                FormAuditWizardServiceControl serviceForm = new FormAuditWizardServiceControl();
                serviceForm.ShowDialog();
            }

            CheckServiceRunning();
        }
    }
}
