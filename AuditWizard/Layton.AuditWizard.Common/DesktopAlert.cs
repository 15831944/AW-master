using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Text;
using System.Windows.Forms;
using Layton.AuditWizard.DataAccess;
using Infragistics.Win.UltraWinSchedule;

using Infragistics.Win;
using Infragistics.Win.Misc;

namespace Layton.AuditWizard.Common
{
    public class DesktopAlert
    {
        private static UltraDesktopAlert _desktopAlert = new UltraDesktopAlert();

        /// <summary>
        /// Displays a desktop alert
        /// </summary>
        /// <param name="messageText">string to be displayed in the desktop alert</param>
        public static void ShowDesktopAlert(string messageText)
        {
            Configuration config = ConfigurationManager.OpenExeConfiguration(Path.Combine(Application.StartupPath, "AuditWizardv8.exe"));
            int autoCloseDelay = (Convert.ToInt32(config.AppSettings.Settings["DesktopAlertFadeOutSeconds"].Value) * 1000);
            //float alertOpacity = (Convert.ToSingle(config.AppSettings.Settings["DesktopAlertOpacityPercent"].Value) / 100);

            Infragistics.Win.Appearance normalLinkAppearance = new Infragistics.Win.Appearance();
            normalLinkAppearance.ForeColor = System.Drawing.SystemColors.WindowText;
            normalLinkAppearance.FontData.Name = "Calibri";
            normalLinkAppearance.FontData.SizeInPoints = 9.5F;

            _desktopAlert.CaptionAppearance = normalLinkAppearance;
            _desktopAlert.TextAppearance = normalLinkAppearance;
            _desktopAlert.FooterTextAppearance = normalLinkAppearance;

            _desktopAlert.Opacity = 0.9F;// alertOpacity;
            _desktopAlert.Visible = true;
            _desktopAlert.AutoClose = DefaultableBoolean.True;
            _desktopAlert.AutoCloseDelay = autoCloseDelay;
            _desktopAlert.ImageSize = new System.Drawing.Size(32, 32);
            _desktopAlert.AllowMove = DefaultableBoolean.False;
            _desktopAlert.TreatCaptionAsLink = DefaultableBoolean.False;
            _desktopAlert.TreatTextAsLink = DefaultableBoolean.False;

            _desktopAlert.FixedSize = new System.Drawing.Size(350, 0);

            _desktopAlert.AnimationSpeed = AnimationSpeed.Slow;
            _desktopAlert.AnimationStyleShow = AnimationStyle.Fade;
            _desktopAlert.AnimationStyleAutoClose = AnimationStyle.Fade;
            _desktopAlert.AnimationScrollDirectionShow = AnimationScrollDirection.BottomToTop;
            _desktopAlert.AnimationScrollDirectionAutoClose = AnimationScrollDirection.TopToBottom;
            _desktopAlert.MultipleWindowDisplayStyle = MultipleWindowDisplayStyle.Tiled;

            //  Create a new instance of the UltraDesktopAlertShowWindowInfo class.
            UltraDesktopAlertShowWindowInfo showInfo = new UltraDesktopAlertShowWindowInfo();
            showInfo.Text = "<span style=\"text-decoration:none\">" + messageText + "  " + "</span>";
            showInfo.Caption = "<span style=\"text-decoration:none\">AuditWizard notification</span>";
            showInfo.Screen = Screen.AllScreens[Screen.AllScreens.Length - 1];

            _desktopAlert.Show(showInfo);
        }

        /// <summary>
        /// Displays a desktop alert for tasks
        /// </summary>
        /// <param name="messageText">string to be displayed in the desktop alert</param>
        public static void ShowDesktopAlertForNewVersion(string messageText)
        {
            Infragistics.Win.Appearance normalLinkAppearance = new Infragistics.Win.Appearance();
            normalLinkAppearance.ForeColor = System.Drawing.SystemColors.WindowText;
            normalLinkAppearance.FontData.Name = "Verdana";
            normalLinkAppearance.FontData.SizeInPoints = 8;

            _desktopAlert.CaptionAppearance = normalLinkAppearance;
            _desktopAlert.TextAppearance = normalLinkAppearance;
            _desktopAlert.FooterTextAppearance = normalLinkAppearance;

            _desktopAlert.Opacity = 0.9F;// alertOpacity;
            _desktopAlert.Visible = true;
            _desktopAlert.AutoClose = DefaultableBoolean.False;
            _desktopAlert.ImageSize = new System.Drawing.Size(32, 32);
            _desktopAlert.AllowMove = DefaultableBoolean.False;
            _desktopAlert.TreatCaptionAsLink = DefaultableBoolean.False;
            _desktopAlert.TreatTextAsLink = DefaultableBoolean.True;

            _desktopAlert.FixedSize = new System.Drawing.Size(350, 0);

            _desktopAlert.AnimationSpeed = AnimationSpeed.Slow;
            _desktopAlert.AnimationStyleShow = AnimationStyle.Fade;
            _desktopAlert.AnimationStyleAutoClose = AnimationStyle.Fade;
            _desktopAlert.AnimationScrollDirectionShow = AnimationScrollDirection.BottomToTop;
            _desktopAlert.AnimationScrollDirectionAutoClose = AnimationScrollDirection.TopToBottom;
            _desktopAlert.MultipleWindowDisplayStyle = MultipleWindowDisplayStyle.Tiled;

            _desktopAlert.DesktopAlertLinkClicked -= new DesktopAlertLinkClickedHandler(_desktopAlert_DesktopAlertLinkClicked);
            _desktopAlert.DesktopAlertLinkClicked += new DesktopAlertLinkClickedHandler(_desktopAlert_SupportDesktopAlertLinkClicked);

            //  Create a new instance of the UltraDesktopAlertShowWindowInfo class.
            UltraDesktopAlertShowWindowInfo showInfo = new UltraDesktopAlertShowWindowInfo();
            showInfo.Text = "<span style=\"text-decoration:none\">" + messageText + "</span>";
            showInfo.Caption = "<span style=\"text-decoration:none\">New AuditWizard version available</span>";
            showInfo.Screen = Screen.AllScreens[Screen.AllScreens.Length - 1];

            _desktopAlert.Show(showInfo);
        }

        /// <summary>
        /// Displays a desktop alert for tasks
        /// </summary>
        /// <param name="messageText">string to be displayed in the desktop alert</param>
        public static void ShowDesktopAlertForTasks(string messageText, int aTaskID)
        {
            Infragistics.Win.Appearance normalLinkAppearance = new Infragistics.Win.Appearance();
            normalLinkAppearance.ForeColor = System.Drawing.SystemColors.WindowText;
            normalLinkAppearance.FontData.Name = "Verdana";
            normalLinkAppearance.FontData.SizeInPoints = 8;

            _desktopAlert.CaptionAppearance = normalLinkAppearance;
            _desktopAlert.TextAppearance = normalLinkAppearance;
            _desktopAlert.FooterTextAppearance = normalLinkAppearance;

            _desktopAlert.Opacity = 0.9F;// alertOpacity;
            _desktopAlert.Visible = true;
            _desktopAlert.AutoClose = DefaultableBoolean.False;
            _desktopAlert.ImageSize = new System.Drawing.Size(32, 32);
            _desktopAlert.AllowMove = DefaultableBoolean.False;
            _desktopAlert.TreatCaptionAsLink = DefaultableBoolean.False;
            _desktopAlert.TreatTextAsLink = DefaultableBoolean.True;

            _desktopAlert.FixedSize = new System.Drawing.Size(350, 0);

            _desktopAlert.AnimationSpeed = AnimationSpeed.Slow;
            _desktopAlert.AnimationStyleShow = AnimationStyle.Fade;
            _desktopAlert.AnimationStyleAutoClose = AnimationStyle.Fade;
            _desktopAlert.AnimationScrollDirectionShow = AnimationScrollDirection.BottomToTop;
            _desktopAlert.AnimationScrollDirectionAutoClose = AnimationScrollDirection.TopToBottom;
            _desktopAlert.MultipleWindowDisplayStyle = MultipleWindowDisplayStyle.Tiled;

            _desktopAlert.DesktopAlertLinkClicked -= new DesktopAlertLinkClickedHandler(_desktopAlert_SupportDesktopAlertLinkClicked);
            _desktopAlert.DesktopAlertLinkClicked += new DesktopAlertLinkClickedHandler(_desktopAlert_DesktopAlertLinkClicked);

            //  Create a new instance of the UltraDesktopAlertShowWindowInfo class.
            UltraDesktopAlertShowWindowInfo showInfo = new UltraDesktopAlertShowWindowInfo();
            showInfo.Text = "<span style=\"text-decoration:none\">" + messageText + "</span>";
            showInfo.Caption = "<span style=\"text-decoration:none\">AuditWizard notification</span>";
            showInfo.Data = aTaskID;
            showInfo.Screen = Screen.AllScreens[Screen.AllScreens.Length - 1];

            _desktopAlert.Show(showInfo);
        }

        static void _desktopAlert_SupportDesktopAlertLinkClicked(object sender, DesktopAlertLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start("http://www.laytontechnology.com/index.php?Itemid=94");
        }

        static void _desktopAlert_DesktopAlertLinkClicked(object sender, DesktopAlertLinkClickedEventArgs e)
        {
            DataTable lDataTable = new TaskSchedulesDAO().GetAppointmentById((int)e.WindowInfo.Data);

            object rawAppointmentData = lDataTable.Rows[0][1];

            if (rawAppointmentData is byte[] == false)
                return;

            byte[] appointmentBytes = rawAppointmentData as byte[];
            Appointment appointment = Appointment.FromBytes(appointmentBytes);

            MessageBox.Show(
                appointment.StartDateTime.AddMinutes(1).ToString() + Environment.NewLine + Environment.NewLine + appointment.Description,
                appointment.Subject,
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);
        }
    }
}
