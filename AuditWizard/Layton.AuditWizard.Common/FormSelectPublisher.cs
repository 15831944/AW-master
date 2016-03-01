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
//
using Infragistics.Win.UltraWinListView;

namespace Layton.AuditWizard.Common
{
	public partial class FormSelectPublisher : ShadedImageForm
	{
		/// <summary>
		/// Returns the publisher selected
		/// </summary>
		public string SelectedPublisher
		{ get { return (lvPublishers.SelectedItems.Count == 0) ? "" : lvPublishers.SelectedItems[0].Text; } }

		
		public FormSelectPublisher(bool allowNew)
		{
			InitializeComponent();

			// If we are to allow a new publisher to be specified then enable the new button
			bnNewPublisher.Visible = allowNew;

			// Load the publishers into the list
            ApplicationsDAO lwDataAccess = new ApplicationsDAO();
			DataTable publishersTable = lwDataAccess.GetPublishers("");
			//
			lvPublishers.BeginUpdate();
			foreach (DataRow row in publishersTable.Rows)
			{
				string publisher = (string)row["_PUBLISHER"];
				if (publisher != "")
					lvPublishers.Items.Add(new UltraListViewItem(publisher, null));
			}
			lvPublishers.EndUpdate();
		}

		/// <summary>
		/// Called as we change the selection in the list box - if we have selected a publisher then we allow
		/// the OK buton to be pressed
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void lvPublishers_ItemSelectionChanged(object sender, Infragistics.Win.UltraWinListView.ItemSelectionChangedEventArgs e)
		{
			bnOK.Enabled = lvPublishers.SelectedItems.Count != 0;
		}


		/// <summary>
		/// Called as we click the 'New' button to add a new publisher
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void bnNewPublisher_Click(object sender, EventArgs e)
		{
			FormAskInput1 askPublisher = new FormAskInput1("Please enter the name of a new Publisher", "New Publisher", "Publisher:");
			if (askPublisher.ShowDialog() == DialogResult.OK)
			{
				// Ensure that this name is not already in our list and if not add and select it
				string newPublisher = askPublisher.ValueEntered();
				UltraListViewItem itemToSelect = null;
				int index = lvPublishers.Items.IndexOf(newPublisher);
				if (index == -1)
					index = lvPublishers.Items.Add(new UltraListViewItem(newPublisher ,null));
				itemToSelect = lvPublishers.Items[index];

				// Bring the new item into view and select it
				itemToSelect.BringIntoView();
				lvPublishers.SelectedItems.Add(itemToSelect, true);
			}
		}
	}
}