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
	public partial class FormImportPicklists : Layton.Common.Controls.ShadedImageForm
	{
		string _fromFile;
		private PickListList _listPickLists = new PickListList();
		
		public FormImportPicklists(string fromFile)
		{
			InitializeComponent();
			_fromFile = fromFile;
		}


		/// <summary>
		/// Load the records from the input file into the list view
		/// 
		/// When importing user defined data fields and their values the file should be in the following format
		/// 
		/// Picklist Name		PickItem
		/// 
		/// We assume that the first row will be labels but the user may elect to include it if not
		/// 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void FormImportPicklists_Load(object sender, EventArgs e)
		{
			// try and read the CSV file into the list view
			try
			{
				using (CSVReader csv = new CSVReader(_fromFile))
				{
					// Read each line from the file noting that we MUST have 2 columns and 2 only
					string[] fields;
					while ((fields = csv.GetCSVLine()) != null)
					{
						if (fields.Length != 2)
							continue;
							
						// Add the 2 fields to the ListView
						UltraListViewSubItem[] subItemArray = new UltraListViewSubItem[1];
						subItemArray[0] = new UltraListViewSubItem();
						subItemArray[0].Value = fields[1];
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
		/// Iterate through the checked items and import them into the database, loaction first then asset
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void bnOK_Click(object sender, EventArgs e)
		{
			// Loop through the records in the import file	
			foreach (UltraListViewItem lvi in lvRecords.Items)
			{
				if (lvi.CheckState == CheckState.Checked)
					AddPicklist(lvi);
			}
			
			MessageBox.Show("The Picklists and PickItems selected have been imported into the database" ,"Import Complete");
		}



		/// <summary>
		/// Add a uPicklist and/or PickItem to the database
		/// 
		/// </summary>
		/// <param name="lvi"></param>
		private void AddPicklist(UltraListViewItem lvi)
		{			
			// Recover the data fields
			string listName = lvi.Text;
			string itemName = lvi.SubItems[0].Text;
			
			// Does the Picklist exist?
			PickList pickList = _listPickLists.FindPickList(listName);
			if (pickList == null)
			{
				pickList = new PickList();
				pickList.Name = listName;
				pickList.Add();
				_listPickLists.Add(pickList);
			}
			
			// Does the pickitem already exist within the Picklist?
			PickItem pickItem = pickList.FindPickItem(itemName);
			if (pickItem == null)
			{
				pickItem = new PickItem();
				pickItem.Name = itemName;
				pickItem.ParentID = pickList.PicklistID;
				pickItem.Add();
			}
		}
	}
}

