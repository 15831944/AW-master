using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Infragistics.Win.UltraWinSchedule;
using Layton.AuditWizard.DataAccess;

namespace Layton.AuditWizard.Common
{
    public partial class FormEditTasks : Form
    {
        private Appointment _appointment;

        public Appointment EditedAppointment
        {
            get { return _appointment; }
            set { _appointment = value; }
        }

        public FormEditTasks(Appointment aAppointment)
        {
            InitializeComponent();
            _appointment = aAppointment;
        }

        private void FormEditReportSchedule_Load(object sender, EventArgs e)
        {
            PopulateLabels();
        }

        private void PopulateLabels()
        {
            tbTitle.Text = String.Format("Edit task: '{0}'", _appointment.Subject);
            tbSchedule.Text = _appointment.Recurrence.Description;
            tbSubject.Text = _appointment.Subject;
            tbDescription.Text = _appointment.Description;
        }

        private void bnEditSchedule_Click(object sender, EventArgs e)
        {
            RecurrenceDialog rd = new RecurrenceDialog(_appointment, _appointment.Recurrence, true, false, false);
            rd.Text = "Schedule for " + _appointment.Subject;
            rd.ShowDialog();

            if (rd.Recurrence != null)
            {
                _appointment.Recurrence = rd.Recurrence;
                PopulateLabels();
            }
        }

        private void bnClose_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void bnOK_Click(object sender, EventArgs e)
        {
            _appointment.Subject = tbSubject.Text;
            _appointment.Description = tbDescription.Text;
            Close();
        }
    }
}
