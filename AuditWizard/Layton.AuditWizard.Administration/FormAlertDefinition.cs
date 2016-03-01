using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
//
using Layton.Common.Controls;
using Layton.AuditWizard.Common;

namespace Layton.AuditWizard.Administration
{
	public partial class FormAlertDefinition : ShadedImageForm
	{
		private AlertDefinition _alertDefinition;

		public AlertDefinition AlertDefinition
		{
			get { return _alertDefinition; }
			set { _alertDefinition = value; }
		}
		
		public FormAlertDefinition(AlertDefinition alertDefinition)
		{
			InitializeComponent();
			_alertDefinition = alertDefinition;
		}

		private void FormAlertDefinition_Load(object sender, EventArgs e)
		{
			tbName.Text = _alertDefinition.Name;
			//cbDisplayAlert.Checked = _alertDefinition.DisplayAlert;
			cbEmailAlert.Checked = _alertDefinition.EmailAlert;
			
			// Set focus to the alert name and select the text
			tbName.Focus();
			tbName.SelectAll();
		}


		private void bnOK_Click(object sender, EventArgs e)
		{
			_alertDefinition.Name = tbName.Text;
			_alertDefinition.EmailAlert = cbEmailAlert.Checked;
			//_alertDefinition.DisplayAlert = cbDisplayAlert.Checked;
		}

		private void tbName_TextChanged(object sender, EventArgs e)
		{
			bnOK.Enabled = (tbName.Text != "");
		}
	}
}