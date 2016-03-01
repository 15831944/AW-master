using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Layton.AuditWizard.DataAccess;

namespace Layton.AuditWizard.Common
{
    public partial class FormSelectLocations : Form
    {
        private string _selectedAssetIds;
        private string _selectedGroupNames;

        public string SelectedAssetIds
        { 
            get 
            {
                return _selectedAssetIds; 
            } 
        }

        public string SelectedGroupNames
        {
            get
            {
                return _selectedGroupNames;
            }
        }

        public FormSelectLocations(string selectedGroups, string selectedAssets)
        {
            InitializeComponent();
            selectLocationsControl.Populate(true, false, false);
            selectLocationsControl.RestoreSelections(selectedGroups, selectedAssets);
        }

        public FormSelectLocations()
        {
            InitializeComponent();
			selectLocationsControl.Populate(true, false, false);
            selectLocationsControl.RestoreSelections("", "");            
        }

        private void FormSelectLocations_FormClosing(object sender, FormClosingEventArgs e)
        {
            //_assetNames = selectLocationsControl.GetSelectedItemsLight().ToString();

            AssetGroupList listSelectedGroups;
            AssetList listSelectedAssets;

            selectLocationsControl.GetSelectedItems(out listSelectedGroups, out listSelectedAssets);

            _selectedAssetIds = listSelectedAssets.ToString();
            _selectedGroupNames = listSelectedGroups.ToString();
        }

        private void bnOK_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void bnCancel_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
