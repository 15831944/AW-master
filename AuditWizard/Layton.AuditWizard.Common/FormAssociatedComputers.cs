using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
//
using Layton.AuditWizard.DataAccess;
using Layton.Common.Controls;

namespace Layton.AuditWizard.Common
{
	public partial class FormAssociatedComputers : ShadedImageForm
	{
		Action _theAction;

		public FormAssociatedComputers(Action action)
		{
			InitializeComponent();
			_theAction = action;

			// Load the associated computers into the list
			string[] listComputers = action.AssociatedAssets.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);

			foreach (string computer in listComputers)
			{
				clbComputers.Items.Add(computer ,true);
			} 
		}

		/// <summary>
		/// Called as we click to exit from this form - update the action with any changes
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void bnOK_Click(object sender, EventArgs e)
		{
			// Get the existing list of computers
			string[] listComputers = _theAction.AssociatedAssets.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
			if (clbComputers.CheckedItems.Count == listComputers.Length)
				return;

			// OK we have had some changes so we need to update the Action
			string selectedComputers = "";
			foreach (object itemChecked in clbComputers.CheckedItems)
			{
				if (selectedComputers == "")
					selectedComputers = itemChecked.ToString();
				else
					selectedComputers = selectedComputers + ";" + itemChecked.ToString();
			}

			// ...update the action object
			_theAction.AssociatedAssets = selectedComputers;

			// ...and update the database

		}
	}
}