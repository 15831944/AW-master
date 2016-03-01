using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Infragistics.Win.UltraWinSchedule;
using Infragistics.Win.UltraWinGrid;
using Layton.AuditWizard.DataAccess;

namespace Layton.AuditWizard.Common
{
    public partial class FormTasks : Form
    {
        private RecurrenceDialog _recurrenceDialog;
        private static TaskSchedulesDAO lTaskSchedulesDAO;

        public FormTasks()
        {
            InitializeComponent();
            lTaskSchedulesDAO = new TaskSchedulesDAO();
        }

        private void FormTasks_Load(object sender, EventArgs e)
        {
            PopulateGrid();
            ultraGridTaskSchedules.DisplayLayout.Bands[0].Columns[0].Hidden = true;

            ultraGridTaskSchedules.DisplayLayout.Bands[0].Columns[1].Width = 140;
            ultraGridTaskSchedules.DisplayLayout.Bands[0].Columns[1].CellAppearance.TextTrimming = Infragistics.Win.TextTrimming.EllipsisWord;
            ultraGridTaskSchedules.DisplayLayout.Bands[0].Columns[1].CellMultiLine = Infragistics.Win.DefaultableBoolean.True;

            ultraGridTaskSchedules.DisplayLayout.Bands[0].Columns[2].Width = 140;
            ultraGridTaskSchedules.DisplayLayout.Bands[0].Columns[2].CellAppearance.TextTrimming = Infragistics.Win.TextTrimming.EllipsisWord;
            ultraGridTaskSchedules.DisplayLayout.Bands[0].Columns[2].CellMultiLine = Infragistics.Win.DefaultableBoolean.True;
            ultraGridTaskSchedules.DisplayLayout.Override.RowSizing = Infragistics.Win.UltraWinGrid.RowSizing.AutoFixed;
        }

        private void bnDeleteAllSchedules_Click(object sender, EventArgs e)
        {
            int lCounter = 0;
            foreach (DataRow row in lTaskSchedulesDAO.GetAllAppointments().Rows)
            {
                object rawAppointmentData = row["_APPOINTMENTDATA"];

                if (rawAppointmentData is byte[] == false)
                    continue;

                Appointment appointment = Appointment.FromBytes(rawAppointmentData as byte[]);

                if ((appointment.Recurrence.RangeLimit != RecurrenceRangeLimit.NoLimit) && (appointment.EndDateTime < DateTime.Now.AddMinutes(-1.0)))
                {
                    lTaskSchedulesDAO.DeleteAppt(Convert.ToInt32(row["_TASKSCHEDULEID"]));
                    lCounter++;
                }
            }

            string message = String.Format("Deleted {0} expired tasks.", lCounter);

            if (lCounter == 0)
                message = "There were no expired tasks to delete.";

            MessageBox.Show(
                message,
                "Delete tasks",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);

            PopulateGrid();
        }

        private void bnEditSchedule_Click(object sender, EventArgs e)
        {
            EditSchedule();
        }

        private void bnDeleteSchedule_Click(object sender, EventArgs e)
        {
            if (ultraGridTaskSchedules.Selected.Rows.Count > 0)
            {
                string message = (ultraGridTaskSchedules.Selected.Rows.Count == 1) ?
                    "Are you sure you want to delete this task?" : "Are you sure you want to delete these tasks";

                if (MessageBox.Show(message, "Confirm schedule delete", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
                    return;

                foreach (UltraGridRow selectedrow in ultraGridTaskSchedules.Selected.Rows)
                {
                    lTaskSchedulesDAO.DeleteAppt((int)selectedrow.Cells[0].Value);
                }

                PopulateGrid();
            }
        }

        private void EditSchedule()
        {
            if (ultraGridTaskSchedules.Selected.Rows.Count == 1)
            {
                int selectedAppointmentId = (int)ultraGridTaskSchedules.Selected.Rows[0].Cells[0].Value;
                DataTable selectedAppointment = lTaskSchedulesDAO.GetAppointmentById(selectedAppointmentId);

                object rawAppointmentData = selectedAppointment.Rows[0][1];

                if (rawAppointmentData is byte[] == false)
                    return;

                byte[] appointmentBytes = rawAppointmentData as byte[];
                Appointment appointment = Appointment.FromBytes(appointmentBytes);

                FormEditTasks form = new FormEditTasks(appointment);
                if (form.ShowDialog() == DialogResult.OK)
                {
                    lTaskSchedulesDAO.Update(selectedAppointmentId, form.EditedAppointment.Save());
                    PopulateGrid();
                }
            }
            else
            {
                MessageBox.Show("Please select one task from the table.", "Tasks", MessageBoxButtons.OK, MessageBoxIcon.Information);
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

        private void PopulateGrid()
        {
            DataTable lApptTable = lTaskSchedulesDAO.GetAllAppointments();

            DataTable lResultsDataTable = new DataTable();
            lResultsDataTable.Columns.Add("TaskID", typeof(int));
            lResultsDataTable.Columns.Add("Task Subject", typeof(string));
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
                lResultsDataTable.Rows.Add((int)row[0], appointment.Subject, lRecurrenceDescription);
            }

            ultraGridTaskSchedules.DataSource = lResultsDataTable;
        }

        private void ultraGridReportSchedules_InitializeRow(object sender, Infragistics.Win.UltraWinGrid.InitializeRowEventArgs e)
        {
            int selectedAppointmentId = Convert.ToInt32(e.Row.Cells[0].Value);
            DataTable selectedAppointment = lTaskSchedulesDAO.GetAppointmentById(selectedAppointmentId);

            object rawAppointmentData = selectedAppointment.Rows[0][1];

            if (rawAppointmentData is byte[] == false)
                return;

            byte[] appointmentBytes = rawAppointmentData as byte[];
            Appointment appointment = Appointment.FromBytes(appointmentBytes);

            if ((appointment.Recurrence.RangeLimit != RecurrenceRangeLimit.NoLimit) &&(appointment.EndDateTime < DateTime.Now.AddMinutes(-1)))
                e.Row.Appearance.ForeColor = Color.Red;
        }

        private void ultraGridTaskSchedules_DoubleClickRow(object sender, Infragistics.Win.UltraWinGrid.DoubleClickRowEventArgs e)
        {
            EditSchedule();
        }

        private void ultraGridTaskSchedules_AfterSelectChange(object sender, Infragistics.Win.UltraWinGrid.AfterSelectChangeEventArgs e)
        {
            bnDeleteSchedule.Enabled = ultraGridTaskSchedules.Selected.Rows.Count > 0;
            bnEditSchedule.Enabled = ultraGridTaskSchedules.Selected.Rows.Count == 1;
        }

        private void bnAdd_Click(object sender, EventArgs e)
        {
            AddTask();
        }

        private void AddTask()
        {
            FormTaskName form = new FormTaskName();
            if (form.ShowDialog() == DialogResult.OK)
            {
                Appointment rootAppt = new Appointment(DateTime.Now, DateTime.Now);
                rootAppt.Subject = form.Subject;
                rootAppt.Description = form.Description;
                rootAppt.Recurrence = DefineReportRecurrence(rootAppt);

                if (rootAppt.Recurrence != null)
                {
                    if (rootAppt.Recurrence.OccurrenceStartTime < DateTime.Now)
                    {
                        DateTime lDateTime = rootAppt.Recurrence.OccurrenceStartTime;
                        rootAppt.Recurrence.OccurrenceStartTime =
                                new DateTime(lDateTime.Year, lDateTime.Month, lDateTime.Day, lDateTime.Hour, lDateTime.Minute + 1, 0);
                    }

                    lTaskSchedulesDAO.Insert(rootAppt.Save());
                    PopulateGrid();
                }
            }
        }

        private void bnClose_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
