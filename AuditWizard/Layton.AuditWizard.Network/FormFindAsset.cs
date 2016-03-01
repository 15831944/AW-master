using System;
using System.Data;
using System.Windows.Forms;
using Infragistics.Win.UltraWinListView;
using Infragistics.Win.UltraWinTree;
using Layton.AuditWizard.DataAccess;

namespace Layton.AuditWizard.Network
{
	public partial class FormFindAsset : Form
	{
		UltraTree _explorerTree;
		
		public FormFindAsset(UltraTree explorerTree)
		{
			InitializeComponent();
			_explorerTree = explorerTree;
		}

		private void FormFindAsset_Load(object sender, EventArgs e)
		{
			try
			{
				this.UseWaitCursor = true;

				// CMD 8.4.1
				// Populate the 'Select Locations' control noting that we hide all assets as these are not required in the Search function
				selectLocationsControl.Populate(true, true, true);
				selectLocationsControl.RestoreSelections("", "");
			}
			finally
			{
				this.UseWaitCursor = false;
			}

            tbFindAssetName.Text = "";
            tbFindAssetName.Select();
            bnFind.Enabled = false;
            checkBoxAssetNames.Checked = true;
		}


		/// <summary>
		/// Called as the selection cahnges in the list view - show is only valid when we have a selection
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void lvResults_ItemSelectionChanged(object sender, Infragistics.Win.UltraWinListView.ItemSelectionChangedEventArgs e)
		{
			bnOK.Enabled = (lvResults.SelectedItems.Count != 0);
		}

		
		/// <summary>
		/// Called as we click 'Find' - locate any assets which match the criteria.  The search is done on a 
		/// case-insensitive partial match basis.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void bnFind_Click(object sender, EventArgs e)
		{
            lblResults.Visible = false;

            if (!checkBoxAssetMake.Checked &&
                !checkBoxAssetModel.Checked &&
                !checkBoxAssetNames.Checked &&
                !checkBoxAssetTag.Checked &&
                !checkBoxHardware.Checked &&
                !checkBoxInstalledApplications.Checked &&
                !checkBoxInternet.Checked &&
                !checkBoxIPAddress.Checked &&
                !checkBoxMACAddress.Checked &&
                !checkBoxScannedFiles.Checked &&
                !checkBoxSerialNumber.Checked &&
                !checkBoxUserDefined.Checked)
            {
                MessageBox.Show(
                    "Please select at least one search category.",
                    "Select search category",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Exclamation);

                return;
            }


            try
            {
                bnFind.Enabled = false;
				this.UseWaitCursor = true;
		        FindAsset();
			}
            finally
            {
				this.UseWaitCursor = false;
                bnFind.Enabled = true;
            }
		}

        private void FindAsset()
        {
            // Clear any previous results
            lvResults.Items.Clear();
            lvResults.Refresh();
            lblResults.Visible = true;
            lblResults.Text = "Searching, please wait...";
            lblResults.Refresh();

            // Suspend updating of the tree while we do the search
            _explorerTree.BeginUpdate();

			// Get a list of the selected GROUPS as we cannot select individual assets
            PopulateMatches(tbFindAssetName.Text.ToUpper(), GetSelectedGroups());
            
            lblResults.Text = String.Format("Found {0} {1}", lvResults.Items.Count, (lvResults.Items.Count == 1) ? "result" : "results");

            // end updating
            _explorerTree.EndUpdate();
        }        

        private void PopulateMatches(string aSearchText, string aGroupIdList)
        {
            if (checkBoxAssetNames.Checked)
                PopulateAssetNameMatches(aSearchText, aGroupIdList);

            if (checkBoxAssetTag.Checked)
                PopulateAssetTagMatches(aSearchText, aGroupIdList);

            if (checkBoxAssetMake.Checked)
                PopulateAssetMakeMatches(aSearchText, aGroupIdList);

            if (checkBoxAssetModel.Checked)
                PopulateAssetModelMatches(aSearchText, aGroupIdList);

            if (checkBoxSerialNumber.Checked)
                PopulateAssetSerialNumberMatches(aSearchText, aGroupIdList);

            if (checkBoxIPAddress.Checked)
                PopulateAssetIPAddressMatches(aSearchText, aGroupIdList);

            if (checkBoxMACAddress.Checked)
                PopulateAssetMACAddressMatches(aSearchText, aGroupIdList);

            if (checkBoxUserDefined.Checked)
                PopulateAssetUserDataMatches(aSearchText, aGroupIdList);

            if (checkBoxHardware.Checked)
                PopulateAssetHardwareMatches(aSearchText, aGroupIdList);

            if (checkBoxScannedFiles.Checked)
                PopulateAssetFileMatches(aSearchText, aGroupIdList);

            if (checkBoxInternet.Checked)
                PopulateAssetInternetMatches(aSearchText, aGroupIdList);

            if (checkBoxInstalledApplications.Checked)
                PopulateAssetApplicationMatches(aSearchText, aGroupIdList);
        }

		private void PopulateAssetNameMatches(string aSearchText, string aGroupIdList)
        {
			DataTable lDataTable = new AssetDAO().FindAssetByName(aSearchText, aGroupIdList);
            PopulateResultsListView(lDataTable, "Asset Name");
        }

		private void PopulateAssetTagMatches(string aSearchText, string aGroupIdList)
        {
			DataTable lDataTable = new AssetDAO().FindAssetByTag(aSearchText, aGroupIdList);
            PopulateResultsListView(lDataTable, "Asset Tag");
        }

		private void PopulateAssetMakeMatches(string aSearchText, string aGroupIdList)
        {
			DataTable lDataTable = new AssetDAO().FindAssetByMake(aSearchText, aGroupIdList);
            PopulateResultsListView(lDataTable, "Asset Make");
        }

		private void PopulateAssetModelMatches(string aSearchText, string aGroupIdList)
        {
			DataTable lDataTable = new AssetDAO().FindAssetByModel(aSearchText, aGroupIdList);
            PopulateResultsListView(lDataTable, "Asset Model");
        }

		private void PopulateAssetSerialNumberMatches(string aSearchText, string aGroupIdList)
        {
			DataTable lDataTable = new AssetDAO().FindAssetBySerialNumber(aSearchText, aGroupIdList);
            PopulateResultsListView(lDataTable, "Serial Number");
        }

		private void PopulateAssetIPAddressMatches(string aSearchText, string aGroupIdList)
        {
			DataTable lDataTable = new AssetDAO().FindAssetByIPAddress(aSearchText, aGroupIdList);
            PopulateResultsListView(lDataTable, "IP Address");
        }

		private void PopulateAssetMACAddressMatches(string aSearchText, string aGroupIdList)
        {
			DataTable lDataTable = new AssetDAO().FindAssetByMACAddress(aSearchText, aGroupIdList);
            PopulateResultsListView(lDataTable, "MAC Address");
        }

		private void PopulateAssetUserDataMatches(string aSearchText, string aGroupIdList)
        {
			DataTable lDataTable = new AssetDAO().FindAssetByUserData(aSearchText, aGroupIdList);
            PopulateResultsListView(lDataTable, "User Defined Fields");
        }

		private void PopulateAssetHardwareMatches(string aSearchText, string aGroupIdList)
        {
			DataTable lDataTable = new AssetDAO().FindAssetByHardware(aSearchText, aGroupIdList);
            PopulateResultsListView(lDataTable, "Hardware Items");
        }

		private void PopulateAssetFileMatches(string aSearchText, string aGroupIdList)
        {
			DataTable lDataTable = new AssetDAO().FindAssetByFiles(aSearchText, aGroupIdList);
            PopulateResultsListView(lDataTable, "Scanned Files");
        }

		private void PopulateAssetInternetMatches(string aSearchText, string aGroupIdList)
        {
			DataTable lDataTable = new AssetDAO().FindAssetByInternet(aSearchText, aGroupIdList);
            PopulateResultsListView(lDataTable, "Internet History / Cookies");
        }

		private void PopulateAssetApplicationMatches(string aSearchText, string aGroupIdList)
        {
			DataTable lDataTable = new AssetDAO().FindAssetByApplicationName(aSearchText, aGroupIdList);
            PopulateResultsListView(lDataTable, "Installed Applications");
        }

        private void PopulateResultsListView(DataTable aDataTable, string aSearchType)
        {
            UltraListViewItem[] items = new UltraListViewItem[aDataTable.Rows.Count];

            lvResults.BeginUpdate();

            for (int i = 0; i < aDataTable.Rows.Count; i++)
            {
                DataRow row = aDataTable.Rows[i];
                UltraListViewSubItem[] subItemArray = new UltraListViewSubItem[3];
                subItemArray[0] = new UltraListViewSubItem();
                subItemArray[0].Value = row[1];

                subItemArray[1] = new UltraListViewSubItem();
                subItemArray[1].Value = aSearchType;

                subItemArray[2] = new UltraListViewSubItem();
                subItemArray[2].Value = row[2];

                UltraListViewItem item = new UltraListViewItem(row[0], subItemArray);
                items[i] = item;
            }

            lvResults.Items.AddRange(items);
            lvResults.EndUpdate();
        }

		private void bnOK_Click(object sender, EventArgs e)
		{
            Cursor.Current = Cursors.WaitCursor;
            GotoSelectedAsset();
            Cursor.Current = Cursors.Default;
		}

        private void GotoSelectedAsset()
        {
            if (lvResults.SelectedItems.Count == 0) return;

            UltraListViewItem lvi = lvResults.SelectedItems[0];
            
            if (lvi == null) return;

            // display the node
            UltraTreeNode rootNode = _explorerTree.Nodes[0];
            UltraTreeNode node = AddMatches(rootNode, lvi.Value.ToString());

            node.BringIntoView();
            _explorerTree.SelectedNodes.Clear();
            node.Selected = true;
            node.Expanded = true;
        }

        private string GetSelectedGroups()
        {
            string lGroupIdList = selectLocationsControl.GetSelectedGroups().ToString();
			lGroupIdList = lGroupIdList.Replace(';', ',').TrimEnd(',');
			return lGroupIdList;
        }

        private UltraTreeNode AddMatches(UltraTreeNode parentNode, string assetName)
        {
            foreach (UltraTreeNode childNode in parentNode.Nodes)
            {
                // If this node represents an asset group then we should check it's children first
                if (childNode.Tag is AssetGroup)
                {
                    // If this branch is not currently expanded then we need to expand it now 
                    // in order to search it
                    bool currentState = childNode.Expanded;
                    //
                    if (!childNode.Expanded)
                        childNode.Expanded = true;
                    //	
                    UltraTreeNode foundNode = AddMatches(childNode, assetName);
                    if (foundNode != null)
                        return foundNode;

                    // Contract the branch if it was NOT previously expanded
                    if (!currentState)
                        childNode.Expanded = false;
                }

                else if (childNode.Tag is Asset)
                {
                    Asset thisAsset = childNode.Tag as Asset;
                    string upperAssetname = thisAsset.Name.ToUpper();
                    //
                    if (upperAssetname.Equals(assetName.ToUpper()))
                        return childNode;
                }
            }

            return null;
        }

        private void tbFindAssetName_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)13)
            {
                FindAsset();
            }
        }

        private void tbFindAssetName_TextChanged(object sender, EventArgs e)
        {
            bnFind.Enabled = tbFindAssetName.Text.Length > 0;
        }

        private void lvResults_ItemDoubleClick(object sender, ItemDoubleClickEventArgs e)
        {
            GotoSelectedAsset();
            Close();
        }
	}
}

