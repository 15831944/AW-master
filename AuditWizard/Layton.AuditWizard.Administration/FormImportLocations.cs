using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
//
using Infragistics.Win.UltraWinListView;
using Layton.AuditWizard.Common;
using Layton.Common.Controls;
using Layton.AuditWizard.DataAccess;

namespace Layton.AuditWizard.Administration
{
	public partial class FormImportLocations : Layton.Common.Controls.ShadedImageForm
	{
		string _fromFile;
		string _rootName;
		
		public FormImportLocations(string fromFile)
		{
			InitializeComponent();
			_fromFile = fromFile;
		}


		/// <summary>
		/// Load the records from the input file into the list view
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void FormImportLocations_Load(object sender, EventArgs e)
		{
			// recover the root group as we need its name to prefix to all created location names
			LocationsDAO lwDataAccess = new LocationsDAO();
			DataTable table = lwDataAccess.GetGroups(new AssetGroup(AssetGroup.GROUPTYPE.userlocation));
			AssetGroup rootAssetGroup = new AssetGroup(table.Rows[0], AssetGroup.GROUPTYPE.userlocation);
			_rootName = rootAssetGroup.Name;
		
			// try and read the CSV file into the list view
			try
			{
				using (CSVReader csv = new CSVReader(_fromFile))
				{
					// Read each line from the file noting that we MUST have 4 columns and 4 only
					string[] fields;
					while ((fields = csv.GetCSVLine()) != null)
					{
						if (fields.Length != 4)
							continue;
							
						// Add the 4 fields to the ListView
						UltraListViewSubItem[] subItemArray = new UltraListViewSubItem[3];
						subItemArray[0] = new UltraListViewSubItem();
						subItemArray[0].Value = fields[1];
						subItemArray[1] = new UltraListViewSubItem();
						subItemArray[1].Value = fields[2];
						subItemArray[2] = new UltraListViewSubItem();
						subItemArray[2].Value = fields[3];
						//
						UltraListViewItem item = new UltraListViewItem(fields[0], subItemArray);
						item.CheckState = (lvRecords.Items.Count == 0) ? CheckState.Unchecked : CheckState.Checked;
						lvRecords.Items.Add(item);
					}
				}		
			}
			
			catch (Exception ex)
			{
				MessageBox.Show("Failed to import the file " + _fromFile + ".  Reason: " + ex.Message, "Import Failed");
			}
		}


		/// <summary>
		/// Iterate through the checked items and import them into the database, location first then asset
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void bnOK_Click(object sender, EventArgs e)
		{
			if (bnOK.Text == "OK")
			{
				DialogResult = DialogResult.OK;
			}
			else
			{
				DialogResult = DialogResult.None;

				// Disable the 'OK/Import' button
				bnOK.Text = "OK";
				bnOK.Enabled = false;

				// Start the worker thread
				backgroundWorker.RunWorkerAsync();
			}
		}	
		
		
		private void AddLocation (UltraListViewItem lvi)
		{
			LocationsDAO lwDataAccess = new LocationsDAO();
			
			// Create the AssetGroup for this location noting that we need to add the root item name to all 
			// locations created as this will be missing from the import text
			string oldLocation = lvi.Text;
			AssetGroup newGroup = new AssetGroup();
			newGroup.GroupType = AssetGroup.GROUPTYPE.userlocation;
			if (oldLocation == "")
				newGroup.FullName = _rootName;
			else
				newGroup.FullName = _rootName + AssetGroup.LOCATIONDELIMITER + lvi.Text;
			newGroup.StartIP = lvi.SubItems[0].Text;
			newGroup.EndIP = lvi.SubItems[1].Text;
			
			// Add this group and get its database index
			newGroup.Add();
			
			// Now we need to add the asset (if there is one defined)
			string assetName = lvi.SubItems[2].Text;
			if (assetName != "")
			{
				Asset newAsset = new Asset();
				newAsset.LocationID = newGroup.GroupID;
				newAsset.Name = assetName;
				newAsset.Add();
			}
		}

		private void backgroundWorker_DoWork(object sender, DoWorkEventArgs e)
		{
			// OK how many items are we importing?
			int total = lvRecords.CheckedItems.Count;
			int count = 0;
			using (new WaitCursor())
			{
				foreach (UltraListViewItem lvi in lvRecords.CheckedItems)
				{
					AddLocation(lvi);
					count++;
					int percent = (int)(((float)count / (float)total) * 100);
					backgroundWorker.ReportProgress(percent ,null);
				}
			}
		}

		private void backgroundWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
		{
			importProgress.Value = e.ProgressPercentage;
		}

		private void backgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
		{
			MessageBox.Show(lvRecords.CheckedItems.Count.ToString() + " Locations and Assets have been imported into the database", "Import Complete");
			bnOK.Enabled = true;
		}
	}
}

