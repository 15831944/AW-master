using System;
using System.Data;
using System.Windows.Forms;
using Layton.AuditWizard.DataAccess;
using Infragistics.Win.UltraWinSchedule;
using Layton.AuditWizard.Common;

namespace Layton.AuditWizard.Overview
{
    public partial class NewsFeedWidget : DefaultWidget
    {
        public new static string _widgetName = "News Feed Widget";
        protected static readonly log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private Timer timer;

        public override string WidgetName()
        {
            return _widgetName;
        }

        public NewsFeedWidget()
        {
            InitializeComponent();

            timer = new Timer();

            timer.Interval = 30000;
            timer.Tick += new EventHandler(timer_Tick);
            timer.Start();

            ultraGridNewsFeed.DataSource = new NewsFeedDAO().GetNewsFeed();

            if (ultraGridNewsFeed.DisplayLayout.Bands[0].Columns.Count > 0)
            {
                ultraGridNewsFeed.DisplayLayout.Bands[0].Columns[0].Width = 110;
                //ultraGridNewsFeed.DisplayLayout.Bands[0].Columns[1].Width = 90;
                ultraGridNewsFeed.DisplayLayout.Bands[0].Columns[0].Format = "dd MMM  HH:mm";
                ultraGridNewsFeed.DisplayLayout.Bands[0].Columns[0].SortIndicator = Infragistics.Win.UltraWinGrid.SortIndicator.Descending;
            }

            ultraGridNewsFeed.DisplayLayout.AutoFitStyle = Infragistics.Win.UltraWinGrid.AutoFitStyle.ExtendLastColumn;
            //ultraGridNewsFeed.DisplayLayout.Override.MergedCellStyle = Infragistics.Win.UltraWinGrid.MergedCellStyle.Always;

            ultraGridNewsFeed.UseOsThemes = Infragistics.Win.DefaultableBoolean.False;
            ultraGridNewsFeed.DisplayLayout.Override.HeaderAppearance.BackColor = System.Drawing.Color.White;
            ultraGridNewsFeed.DisplayLayout.Override.RowSelectors = Infragistics.Win.DefaultableBoolean.False;
            ultraGridNewsFeed.DisplayLayout.Override.HeaderAppearance.FontData.Bold = Infragistics.Win.DefaultableBoolean.True;
            ultraGridNewsFeed.DisplayLayout.Override.HeaderAppearance.ForeColor = System.Drawing.Color.DimGray;
            ultraGridNewsFeed.DisplayLayout.Override.HeaderAppearance.BorderColor = System.Drawing.Color.DimGray;
            ultraGridNewsFeed.DisplayLayout.Override.BorderStyleHeader = Infragistics.Win.UIElementBorderStyle.None;
            ultraGridNewsFeed.DisplayLayout.Override.CellClickAction = Infragistics.Win.UltraWinGrid.CellClickAction.RowSelect;
            ultraGridNewsFeed.DisplayLayout.Bands[0].Columns[1].CellAppearance.TextTrimming = Infragistics.Win.TextTrimming.EllipsisWord;
            ultraGridNewsFeed.DisplayLayout.Bands[0].Columns[1].CellMultiLine = Infragistics.Win.DefaultableBoolean.True;
            ultraGridNewsFeed.DisplayLayout.Override.RowSizing = Infragistics.Win.UltraWinGrid.RowSizing.AutoFixed;

            ultraGridNewsFeed.Refresh();

            this.ContextMenu = new ContextMenu();
        }

        void timer_Tick(object sender, EventArgs e)
        {
            timer.Stop();
            timer.Enabled = false;

            CheckTasks();
            PurgeNewsFeed();
            RefreshWidget();

            timer.Enabled = true;
            timer.Start();
        }

        public override void RefreshWidget()
        {
            try
            {
                if (ultraGridNewsFeed.DisplayLayout.Bands[0] != null)
                {
                    if (ultraGridNewsFeed.DisplayLayout.Bands[0].Columns.Count > 0)
                    {
                        int scrollPosition = ultraGridNewsFeed.DisplayLayout.RowScrollRegions[0].ScrollPosition;
                        ultraGridNewsFeed.DataSource = new NewsFeedDAO().GetNewsFeed();
                        ultraGridNewsFeed.DisplayLayout.RowScrollRegions[0].ScrollPosition = scrollPosition;
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message);
            }
        }

        private void CheckTasks()
        {
            UltraCalendarInfo ultraCalendarInfo = new UltraCalendarInfo();
            ultraCalendarInfo.AllowRecurringAppointments = true;
            TaskSchedulesDAO lTaskSchedulesDAO = new TaskSchedulesDAO();
            SettingsDAO lSettingsDAO = new SettingsDAO();
            Appointment appointment;
            object rawAppointmentData;
            try
            {
                foreach (DataRow row in lTaskSchedulesDAO.GetAllAppointments().Rows)
                {
                    rawAppointmentData = row["_APPOINTMENTDATA"];

                    if (rawAppointmentData is byte[] == false)
                        continue;

                    appointment = Appointment.FromBytes(rawAppointmentData as byte[]);
                    appointment.DataKey = row[0];
                    ultraCalendarInfo.Appointments.Add(appointment);
                }

                string strLastReportRunTime = lSettingsDAO.GetSetting("LastTaskReportRun", false);

                DateTime lLastReportRunDateTime = (strLastReportRunTime == "") ? DateTime.MinValue : Convert.ToDateTime(strLastReportRunTime);
                DateTime lReportRunTime = DateTime.Now;

                AppointmentsSubsetCollection expiredAppointments = ultraCalendarInfo.GetAppointmentsInRange(lLastReportRunDateTime, lReportRunTime);
                lSettingsDAO.SetSetting("LastTaskReportRun", DateTime.Now.ToString(), false);

                foreach (Appointment expiredAppointment in expiredAppointments)
                {
                    // need to re-check that this appointment is between the LastTaskReportRun date and DateTime.Now
                    // there is a hole in the ultraCalendarInfo.GetAppointmentsInRange logic above 
                    if ((lLastReportRunDateTime < expiredAppointment.StartDateTime) && (lReportRunTime > expiredAppointment.StartDateTime))
                    {
                        string lSubject = String.Format("The following task is due at {0}." + Environment.NewLine + Environment.NewLine +
                            expiredAppointment.Subject, expiredAppointment.StartDateTime.ToString());

                        DesktopAlert.ShowDesktopAlertForTasks(lSubject, (int)expiredAppointment.DataKey);

                        NewsFeed.AddNewsItem(NewsFeed.Priority.Information, "Task due: " + expiredAppointment.Subject);
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message);
            }
        }

        private void PurgeNewsFeed()
        {
            new NewsFeedDAO().PurgeNewsFeed();
        }

        private void bnRefreshFeed_Click(object sender, EventArgs e)
        {
            RefreshWidget();
        }

        private void bnEditNewsFeed_Click(object sender, EventArgs e)
        {
            FormConfigureNewsFeed form = new FormConfigureNewsFeed();
            if (form.ShowDialog() == DialogResult.OK)
                RefreshWidget();
        }

        private void NewsFeedWidget_VisibleChanged(object sender, EventArgs e)
        {
            RefreshWidget();
        }
    }
}

