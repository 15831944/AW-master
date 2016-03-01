using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
//
using Layton.AuditWizard.Common;
using Layton.Common.Controls;
using Layton.AuditWizard.DataAccess;

namespace Layton.AuditWizard.Applications
{
	public partial class FormAlertDetails : ShadedImageForm
	{
		private Alert _alert;

		public FormAlertDetails(Alert alert)
		{
			InitializeComponent();

			// Populate the form
			_alert = alert;
			this.tbDate.Text = alert.AlertedOnDate.ToString();
			this.tbAction.Text = alert.CategoryAsString;
			this.tbType.Text = alert.TypeAsString;
			this.tbStatus.Text = alert.StatusAsString;
			this.tbMessage.Text = alert.Message;
		}


		/// <summary>
		/// Called to delete this alert
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void bnDelete_Click(object sender, EventArgs e)
		{
			if (MessageBox.Show("Are you certain that you want to delete this alert?", "Confirm Delete", MessageBoxButtons.YesNo) == DialogResult.Yes)
			{
				// Delete the alert from the database
				_alert.Delete();
				MessageBox.Show("Alert Deleted", "Alert Deleted");

				// Exit the form
				DialogResult = DialogResult.OK;
				this.Close();
			}
		}
	}
}