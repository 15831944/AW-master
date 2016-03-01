using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
//
using Layton.Common.Controls;
using Layton.AuditWizard.DataAccess;

namespace Layton.AuditWizard.Common
{
	public partial class FormFilterPublishers : Form
	{
		private String _publisherFilter = "";

		/// <summary>
		/// Return the Publisher Filter string specified
		/// </summary>
		public String PublisherFilter
		{
			get { return _publisherFilter; }
		}

		public FormFilterPublishers()
		{
			InitializeComponent();
		}

		private void FormFilterPublishers_Load(object sender, EventArgs e)
		{
            Application.UseWaitCursor = false;
            string publisherName = "";

			lbFilteredPublishers.EndUpdate();
			lbAvailablePublishers.EndUpdate();
			lbFilteredPublishers.Items.Clear();
			lbAvailablePublishers.Items.Clear();

			// Get the list of publishers currently in the filter list
			SettingsDAO lwDataAccess = new SettingsDAO();
			string publisherFilter = lwDataAccess.GetPublisherFilter();
			List<String> listFilterPublishers = new List<string>();
			listFilterPublishers.AddRange(publisherFilter.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries));

			// Recover a complete list of all publsiher names 
			DataTable dtAvailablePublishers = new ApplicationsDAO().GetAllPublisherNames();

            // add <unidentified> publisher if not present
            if (dtAvailablePublishers.Select("_PUBLISHER = '" + DataStrings.UNIDENIFIED_PUBLISHER + "'").Length == 0)
            {
                DataRow uidRow = dtAvailablePublishers.NewRow();
                uidRow["_PUBLISHER"] = DataStrings.UNIDENIFIED_PUBLISHER;
                dtAvailablePublishers.Rows.Add(uidRow);
            }

			// Add the publisher to the appropriate list
			foreach (DataRow row in dtAvailablePublishers.Rows)
			{
                publisherName = row[0].ToString();

				if (!listFilterPublishers.Contains(publisherName))
					lbAvailablePublishers.Items.Add(publisherName);
				else
					lbFilteredPublishers.Items.Add(publisherName);
			}

			lbFilteredPublishers.EndUpdate();
			lbAvailablePublishers.EndUpdate();
		}


		/// <summary>
		/// Called when we click the Add button to move the selected items to the filtered publisher list
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void bnAdd_Click(object sender, EventArgs e)
		{
			lbAvailablePublishers.BeginUpdate();
			lbFilteredPublishers.BeginUpdate();
			//
			while (lbAvailablePublishers.SelectedItems.Count > 0)
			{
				lbFilteredPublishers.Items.Add(lbAvailablePublishers.SelectedItems[0]);
				lbAvailablePublishers.Items.Remove(lbAvailablePublishers.SelectedItems[0]);
			}
			//
			lbAvailablePublishers.EndUpdate();
			lbFilteredPublishers.EndUpdate();
		}



		/// <summary>
		/// Called when we click the Remove button to move the selected items from the filtered publisher list
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void bnRemove_Click(object sender, EventArgs e)
		{
			lbAvailablePublishers.BeginUpdate();
			lbFilteredPublishers.BeginUpdate();
			//
			while (lbFilteredPublishers.SelectedItems.Count > 0)
			{
				lbAvailablePublishers.Items.Add(lbFilteredPublishers.SelectedItems[0]);
				lbFilteredPublishers.Items.Remove(lbFilteredPublishers.SelectedItems[0]);
			}
			//
			lbAvailablePublishers.EndUpdate();
			lbFilteredPublishers.EndUpdate();
		}


		/// <summary>
		/// Enable the Add button if there are selected items in the available publishers list
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void lbAvailablePublishers_SelectedIndexChanged(object sender, EventArgs e)
		{
			bnAdd.Enabled = lbAvailablePublishers.SelectedIndices.Count != 0;
		}


		/// <summary>
		/// Enable the Remove button if there are selected items in the filtered publishers list
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void lbFilteredPublishers_SelectedIndexChanged(object sender, EventArgs e)
		{
			bnRemove.Enabled = lbFilteredPublishers.SelectedIndices.Count != 0;
		}


		/// <summary>
		/// Called as we attempt to exit from this form - we need to save any changes made to the
		/// list of filtered publishers
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void bnOK_Click(object sender, EventArgs e)
		{
			// Construct the updated filter string
			foreach (String publisher in lbFilteredPublishers.Items)
			{
				// ...add to the filter
				if (_publisherFilter == "")
					_publisherFilter = publisher;
				else
					_publisherFilter = _publisherFilter + ";" + publisher;
			}

			// ...and save the publisher filter back to the database
			SettingsDAO lwDataAccess = new SettingsDAO();
			lwDataAccess.SetPublisherFilter(_publisherFilter);
		}


		private void lbAvailablePublishers_DoubleClick(object sender, EventArgs e)
		{
            //Point ptClick = lbAvailablePublishers.PointToClient(MousePosition);
            //int itemClicked = lbAvailablePublishers.IndexFromPoint(ptClick);
            //if (itemClicked == -1)
            //    return;

            //// An item was double clicked on so select it
            //lbAvailablePublishers.SelectedIndex = itemClicked;

            //// ...and then act as it the Add button was clicked
            //bnAdd_Click(sender, e);
		}

		private void lbFilteredPublishers_DoubleClick(object sender, EventArgs e)
		{
            //Point ptClick = lbFilteredPublishers.PointToClient(MousePosition);
            //int itemClicked = lbFilteredPublishers.IndexFromPoint(ptClick);
            //if (itemClicked == -1)
            //    return;

            //// An item was double clicked on so select it
            //lbFilteredPublishers.SelectedIndex = itemClicked;

            //// ...and then act as it the Add button was clicked
            //bnRemove_Click(sender, e);
		}



		/// <summary>
		/// Called to add a new Publisher to the list of those available to filter on.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void bnNewPublisher_Click(object sender, EventArgs e)
		{
			FormAskInput1 form = new FormAskInput1("Please enter a name for the new Publisher.", "New Publisher", "Name:");
			if (form.ShowDialog() == DialogResult.OK)
			{
				string newPublisher = form.ValueEntered();

				// Ensure that this value does not duplicate an existing publisher either in the available or selected lists
				foreach (string publisher in lbAvailablePublishers.Items)
				{
					if (newPublisher == publisher)
					{
						MessageBox.Show("The Publisher Specified already exists an has not been added.", "Publisher Exists");
						return;
					}
				}

				// Ensure that this value does not duplicate an existing publisher in the selected lists
				foreach (string publisher in lbFilteredPublishers.Items)
				{
					if (newPublisher == publisher)
					{
						MessageBox.Show("The publisher specified already exists and has not been added.", "Publisher Exists");
						return;
					}
				}

				// Ok this is a new Publisher so add it to the definitions file
				ApplicationDefinitionsFile definitionsFile = new ApplicationDefinitionsFile();
				definitionsFile.SetSection(ApplicationDefinitionsFile.PUBLISHERS_SECTION);
				definitionsFile.SetString(newPublisher, "", true);

				// ...and add it to the 'available' publishers list
				lbAvailablePublishers.Items.Add(newPublisher);
			}
		}
	}
}