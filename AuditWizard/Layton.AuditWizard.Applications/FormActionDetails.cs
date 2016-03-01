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
	public partial class FormActionDetails : ShadedImageForm
	{
		private Action _action;

		public FormActionDetails(Action action)
		{
			InitializeComponent();

			// Populate the form
			_action = action;
			tbAction.Text = _action.ActionTypeText;
			tbApplication.Text = _action.ApplicationName;
			tbAssociatedComputers.Text = _action.AssociatedAssets;
			tbNotes.Text = _action.Notes;
			int index = cbStatus.FindStringExact(_action.StatusText);
			if (index != -1)
				cbStatus.SelectedIndex = index;
		}


		/// <summary>
		/// Called to delete this action
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void bnDelete_Click(object sender, EventArgs e)
		{
			if (MessageBox.Show("Are you certain that you want to delete this action?", "Confirm Delete", MessageBoxButtons.YesNo) == DialogResult.Yes)
			{
				// Delete teh action from the database
				_action.Delete();
				MessageBox.Show("Action Deleted", "Action Deleted");

				// Exit the form
				DialogResult = DialogResult.OK;
				this.Close();
			}
		}


		/// <summary>
		/// Called when we click to 'Change' the list of associated computers
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void bnUpdateComputers_Click(object sender, EventArgs e)
		{
			FormAssociatedComputers form = new FormAssociatedComputers(_action);
			if (form.ShowDialog() == DialogResult.OK)
				tbAssociatedComputers.Text = _action.AssociatedAssets;
		}


		/// <summary>
		/// Called as we click OK to save any changes
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void bnOK_Click(object sender, EventArgs e)
		{
			// Save the existing Action definition so that we can generate audit trail data
			Action oldAction = new Action(_action);
			_action.Status = (Action.ACTIONSTATUS)cbStatus.SelectedItem.DataValue;
			_action.Notes = tbNotes.Text;
			//
			_action.Update(oldAction);
		}
	}
}