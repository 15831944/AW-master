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
	public partial class FormImportUserDefinedData : Layton.Common.Controls.ShadedImageForm
	{
		string _fromFile;

		// Existing user defined data categories (asset data)
		private UserDataCategoryList _listCategories = new UserDataCategoryList(UserDataCategory.SCOPE.Asset);
		
		public FormImportUserDefinedData(string fromFile)
		{
			InitializeComponent();
			_fromFile = fromFile;
		}


		/// <summary>
		/// Load the records from the input file into the list view
		/// 
		/// When importing user defined data fields and their values the file should be in the following format
		/// 
		/// Asset Name		Category     Field Name		Field Value
		/// 
		/// Note that assets should have been previously created.  Any assets which do not currently exist within
		/// the database will be created as PCs
		/// 
		/// We assume that the first row will be labels but the user may elect to include it if not
		/// 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void FormImportUserDefinedData_Load(object sender, EventArgs e)
		{
			lvRecords.BeginUpdate();
			lvRecords.Items.Clear();

			// User data categories
			_listCategories.Populate();

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
							
						// Add the 3 fields to the ListView
						UltraListViewSubItem[] subItemArray = new UltraListViewSubItem[3];
						subItemArray[0] = new UltraListViewSubItem();
						subItemArray[0].Value = fields[2];
						subItemArray[1] = new UltraListViewSubItem();
						subItemArray[1].Value = fields[3];
						subItemArray[2] = new UltraListViewSubItem();
						subItemArray[2].Value = fields[1];
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
			lvRecords.EndUpdate();
		}


		/// <summary>
		/// Iterate through the checked items and import them into the database, loaction first then asset
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
			

		private void backgroundWorker_DoWork(object sender, DoWorkEventArgs e)
		{
			// OK how many items are we importing?
			int total = lvRecords.CheckedItems.Count;
			int count = 0;
			using (new WaitCursor())
			{
				foreach (UltraListViewItem lvi in lvRecords.CheckedItems)
				{
					AddUserDataField(lvi);
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
			MessageBox.Show(lvRecords.CheckedItems.Count.ToString() + " User Defined Data Records have been imported into the database", "Import Complete");
			bnOK.Enabled = true;
		}
	

		/// <summary>
		/// Add a user defined data field and its value.
		/// 
		/// First locate the specified asset (or create a new one if this does not exist)
		/// Locate the user defined data field (or create a new one if this does not exist)
		/// Set the value for the field given its ID and asset
		/// 
		/// </summary>
		/// <param name="lvi"></param>
		private void AddUserDataField(UltraListViewItem lvi)
		{
			AssetDAO lwDataAccess = new AssetDAO();
			
			// Recover the data fields
			string assetName = lvi.Text;
			string category = lvi.SubItems[2].Text;
			if (category == "Asset Data")
				category = "General";
			string fieldName = lvi.SubItems[0].Text;
			string fieldValue = lvi.SubItems[1].Text;
			
			// Does the Asset exist?
			// Note that as we only have an asset name we can only possibly match on that
			int assetID = lwDataAccess.AssetFind(assetName);
				
			// If the asset exists then add the history record for it
			if (assetID != 0)
			{
				// Does the User Data Category exist?  If not create it also
				UserDataCategory parentCategory = _listCategories.FindCategory(category);
				if (parentCategory == null)
				{
					parentCategory = new UserDataCategory(UserDataCategory.SCOPE.Asset);
					parentCategory.Name = category;
					parentCategory.Add();
					_listCategories.Add(parentCategory);
				}


				// Now look at the User Data Field and see if we have one already with this name 
				UserDataField thisField = parentCategory.FindField(fieldName);
				if (thisField == null)
				{
					// No existing field of this name in the General Category so add it
					thisField = new UserDataField();
					thisField.ParentID = parentCategory.CategoryID;
					thisField.Name = fieldName;
					thisField.Add();
					parentCategory.Add(thisField);
				}

				// ...and set the value for the asset both here and in the database
				thisField.SetValueFor(assetID, fieldValue, true);
			}
		}
	}
}

