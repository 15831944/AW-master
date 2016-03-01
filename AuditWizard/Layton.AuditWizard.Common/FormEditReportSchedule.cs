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
    public partial class FormEditReportSchedule : Form
    {
        private Appointment _appointment;

        public Appointment EditedAppointment
        {
            get { return _appointment; }
            set { _appointment = value; }
        }

        public FormEditReportSchedule(Appointment aAppointment)
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
            tbTitle.Text = String.Format("Edit schedule for report: '{0}'", _appointment.Subject);
            tbSchedule.Text = _appointment.Recurrence.Description;
            string lNumberLocations = (_appointment.Location == "") ? "all" : _appointment.Location.Split(',').Length.ToString();
            tbLocations.Text = String.Format("You have currently selected {0} asset(s).", lNumberLocations);
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

        private void bnEditLocations_Click(object sender, EventArgs e)
        {
            //FormSelectLocations form = new FormSelectLocations("", new AssetDAO().ConvertIdListToNames(_appointment.Location, ','));
            FormSelectLocations form = new FormSelectLocations("", _appointment.Location);

            if (form.ShowDialog() == DialogResult.OK)
            {
                //string lAssetIdList = new AssetDAO().ConvertNameListToIds(form.SelectedAssetIds);
                string lAssetIdList = form.SelectedAssetIds;
                lAssetIdList = lAssetIdList.Replace(';', ',');
                _appointment.Location = lAssetIdList;
                PopulateLabels();
            }
        }

        private void bnClose_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void bnOK_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
