using System;
using System.IO;
using System.Windows.Forms;
using Infragistics.Win.UltraWinSchedule;

namespace Layton.AuditWizard.Reports
{
    public partial class FormScheduleReport : Form
    {
        private string _reportName;
        private UltraMonthViewSingle awMonthView;

        public FormScheduleReport(string reportName)
        {
            InitializeComponent();

            cmbScheduleInterval.SelectedIndex = 0;
            _reportName = reportName;
            gbMain.Text = "Create schedule for " + _reportName + " report";

            awMonthView = new UltraMonthViewSingle();
        }

        private void cmbScheduleInterval_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (cmbScheduleInterval.SelectedIndex)
            {
                case 0:

                    gbDaily.Visible = true;
                    gbWeekly.Visible = false;
                    gbMonthly.Visible = false;
                    gbOnce.Visible = false;
                    break;

                case 1:

                    gbDaily.Visible = false;
                    gbWeekly.Visible = true;
                    gbMonthly.Visible = false;
                    gbOnce.Visible = false;
                    break;

                case 2:

                    gbDaily.Visible = false;
                    gbWeekly.Visible = false;
                    gbMonthly.Visible = true;
                    gbOnce.Visible = false;
                    break;

                case 3:
                    gbDaily.Visible = false;
                    gbWeekly.Visible = false;
                    gbMonthly.Visible = false;
                    gbOnce.Visible = true;
                    break;

                default:
                    break;
            }
        }

        private void HandleAppointmentTimes()
        {
            int chosenHour = nudStartTime.Value.Hour;
            int chosenMinutes = nudStartTime.Value.Minute;
            DateTime firstDateTime = new DateTime();

            if (cmbScheduleInterval.SelectedItem.ToString() == "Daily")
            {
                int days = (int)nudDailyFreq.Value;
                AddNewSchedules(days, chosenHour, chosenMinutes);
            }

            else if (cmbScheduleInterval.SelectedItem.ToString() == "Weekly")
            {
                int weeks = (int)nudWeeklyFreq.Value;
                int days;

                if (cbWeeklyMon.Checked)
                {
                    days = DaysToAdd(System.DateTime.Now.DayOfWeek, System.DayOfWeek.Monday) + (weeks * 7);
                    AddNewSchedules(days, chosenHour, chosenMinutes);
                }
                if (cbWeeklyTue.Checked)
                {
                    days = DaysToAdd(System.DateTime.Now.DayOfWeek, System.DayOfWeek.Tuesday) + (weeks * 7);
                    AddNewSchedules(days, chosenHour, chosenMinutes);
                }
                if (cbWeeklyWed.Checked)
                {
                    days = DaysToAdd(System.DateTime.Now.DayOfWeek, System.DayOfWeek.Wednesday) + (weeks * 7);
                    AddNewSchedules(days, chosenHour, chosenMinutes);
                }
                if (cbWeeklyThu.Checked)
                {
                    days = DaysToAdd(System.DateTime.Now.DayOfWeek, System.DayOfWeek.Thursday) + (weeks * 7);
                    AddNewSchedules(days, chosenHour, chosenMinutes);
                }
                if (cbWeeklyFri.Checked)
                {
                    days = DaysToAdd(System.DateTime.Now.DayOfWeek, System.DayOfWeek.Friday) + (weeks * 7);
                    AddNewSchedules(days, chosenHour, chosenMinutes);
                }
                if (cbWeeklySat.Checked)
                {
                    days = DaysToAdd(System.DateTime.Now.DayOfWeek, System.DayOfWeek.Saturday) + (weeks * 7);
                    AddNewSchedules(days, chosenHour, chosenMinutes);
                }
                if (cbWeeklySun.Checked)
                {
                    days = DaysToAdd(System.DateTime.Now.DayOfWeek, System.DayOfWeek.Sunday) + (weeks * 7);
                    AddNewSchedules(days, chosenHour, chosenMinutes);
                }
            }
            else if (cmbScheduleInterval.SelectedItem.ToString() == "Monthly")
            {
                int chosenDayOfMonth = (int)nudMonthlyFreq.Value;

                if (DateTime.Now.Day > chosenDayOfMonth)
                {
                    // selected day has passed so set for next month
                    DateTime now = DateTime.Now.AddMonths(1);
                    chosenDayOfMonth = ProcessInvalidDay(chosenDayOfMonth, now);
                    firstDateTime = new DateTime(now.Year, now.Month, chosenDayOfMonth, chosenHour, chosenMinutes, 0);
                }

                else if (DateTime.Now.Day == chosenDayOfMonth)
                {
                    // equal to current day, check the specified time
                    if (DateTime.Now.Hour <= chosenHour && DateTime.Now.Minute < chosenMinutes)
                    {
                        // required time still in future so set for this month
                        DateTime now = DateTime.Now;
                        chosenDayOfMonth = ProcessInvalidDay(chosenDayOfMonth, now);
                        firstDateTime = new DateTime(now.Year, now.Month, chosenDayOfMonth, chosenHour, chosenMinutes, 0);
                    }
                    else
                    {
                        // time has already passed so set for next month
                        DateTime now = DateTime.Now.AddMonths(1);
                        chosenDayOfMonth = ProcessInvalidDay(chosenDayOfMonth, now);
                        firstDateTime = new DateTime(now.Year, now.Month, chosenDayOfMonth, chosenHour, chosenMinutes, 0);
                    }
                }

                else
                {
                    // selected day still in the future so use this month
                    DateTime now = DateTime.Now;
                    chosenDayOfMonth = ProcessInvalidDay(chosenDayOfMonth, now);
                    firstDateTime = new DateTime(now.Year, now.Month, chosenDayOfMonth, chosenHour, chosenMinutes, 0);
                }

                AddNewSchedules(firstDateTime);
            }

            else
            {
                firstDateTime = new DateTime((int)dtpOnce.Value.Year, (int)dtpOnce.Value.Month, (int)dtpOnce.Value.Day, chosenHour, chosenMinutes, 0);

                Appointment reportAppointment;

                reportAppointment = new Appointment(firstDateTime, firstDateTime);
                reportAppointment.Subject = _reportName + " scheduled report";
                reportAppointment.Description = "<AW_REPORT_SCHEDULER>" + _reportName;
                awMonthView.CalendarInfo.Appointments.Add(reportAppointment);
            }
        }

        private int ProcessInvalidDay(int chosenDay, DateTime scheduledDate)
        {
            if (scheduledDate.Month == 2 && chosenDay > 28)
            {
                chosenDay = DateTime.IsLeapYear(DateTime.Now.Year) ? 29 : 28;
            }

            else if ((scheduledDate.Month == 4 || scheduledDate.Month == 6 || scheduledDate.Month == 9 || scheduledDate.Month == 11) && chosenDay > 30)
            {
                chosenDay = 30;
            }

            return chosenDay;
        }

        private void AddNewSchedules(int days, int hour, int minute)
        {
            Appointment reportAppointment;
            DateTime reportTime = DateTime.Now.AddDays(days);

            DateTime reportDateTime = new DateTime(reportTime.Year, reportTime.Month, reportTime.Day, hour, minute, 0);

            while (reportDateTime < DateTime.Now.AddYears(1))
            {
                reportAppointment = new Appointment(reportDateTime, reportDateTime);
                reportAppointment.Subject = _reportName + " scheduled report";
                reportAppointment.Description = "<AW_REPORT_SCHEDULER>" + _reportName;
                awMonthView.CalendarInfo.Appointments.Add(reportAppointment);

                reportDateTime = reportDateTime.AddDays(days);
            }
        }

        private void AddNewSchedules(DateTime firstScheduleDate)
        {
            Appointment reportAppointment;

            while (firstScheduleDate < DateTime.Now.AddYears(1))
            {
                reportAppointment = new Appointment(firstScheduleDate, firstScheduleDate);
                reportAppointment.Subject = _reportName + " scheduled report";
                reportAppointment.Description = "<AW_REPORT_SCHEDULER>" + _reportName;
                awMonthView.CalendarInfo.Appointments.Add(reportAppointment);

                firstScheduleDate = firstScheduleDate.AddMonths(1);
            }
        }

        private int DaysToAdd(System.DayOfWeek current, System.DayOfWeek desired)
        {
            int c = (int)current;
            int d = (int)desired;
            int n = (7 - c + d);

            return (n > 7) ? n % 7 : n;
        }

        private void bnCancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void bnRemoveExisting_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show(
                    "Are you sure you want to delete all report schedules for " + _reportName + "?",
                    "Confirm delete",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question) == DialogResult.No)
                return;

            FileStream fileStream = null;
            string _monthAppointmentXmlFile = Application.StartupPath + @"\reports\appointments\aw_appts_month.xml";

            using (fileStream = new FileStream(_monthAppointmentXmlFile, FileMode.Open, FileAccess.Read))
            {
                awMonthView.CalendarInfo.LoadFromXml(fileStream, CalendarInfoCategories.Appointments);
            }

            Infragistics.Win.UltraWinSchedule.Owner owner = null;
            owner = awMonthView.CalendarInfo.Owners.Add("tobedeleted");

            foreach (Appointment appointment in awMonthView.CalendarInfo.Appointments)
            {
                if (appointment.Description == "<AW_REPORT_SCHEDULER>" + _reportName)
                {
                    appointment.Owner = owner;
                }
            }

            awMonthView.CalendarInfo.Appointments.RemoveAllAppointmentsForOwner(owner);
            
            try
            {
                fileStream = new FileStream(_monthAppointmentXmlFile, FileMode.Create, FileAccess.Write);
                awMonthView.CalendarInfo.SaveAsXml(fileStream, CalendarInfoCategories.Appointments);
            }
            finally
            {
                if (fileStream != null)
                    fileStream.Close();
            }

            Close();            
        }

        private void FormScheduleReport_Load(object sender, EventArgs e)
        {
            this.Text = "Schedule " + _reportName + " Report";
        }

        private void bnCreateSchedule_Click(object sender, EventArgs e)
        {
            FileStream fileStream = null;
            string _monthAppointmentXmlFile = Application.StartupPath + @"\reports\appointments\aw_appts_month.xml";

            try
            {
                fileStream = new FileStream(_monthAppointmentXmlFile, FileMode.Open, FileAccess.Read);
                awMonthView.CalendarInfo.LoadFromXml(fileStream, CalendarInfoCategories.Appointments);
            }
            catch (Exception ex)
            {
            }
            finally
            {
                if (fileStream != null)
                    fileStream.Close();
            }

            HandleAppointmentTimes();

            try
            {
                fileStream = new FileStream(_monthAppointmentXmlFile, FileMode.Create, FileAccess.Write);
                awMonthView.CalendarInfo.SaveAsXml(fileStream, CalendarInfoCategories.Appointments);
            }
            catch (Exception ex)
            {
            }
            finally
            {
                if (fileStream != null)
                    fileStream.Close();
            }

            Close();
        }
    }
}
