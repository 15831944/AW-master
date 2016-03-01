using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
//
using Layton.AuditWizard.Common;
using Layton.AuditWizard.DataAccess;
using Layton.Common.Controls;

namespace Layton.AuditWizard.Network
{
	public partial class FormAddAsset : AWForm
	{
		Asset _asset;
		AssetTypeList _listAssetTypes = new AssetTypeList();

		public FormAddAsset(Asset asset)
		{
			InitializeComponent();
			_asset = asset;
		}

		private void FormAddAsset_Load(object sender, EventArgs e)
		{
			AuditWizardConfiguration configuration = new AuditWizardConfiguration();
			this.labelLocation.Text = (configuration.ShowByDomain) ? "Parent Domain;" : "Parent Location:";
			this.tbAssetName.Text = _asset.Name;
			this.tbParentLocation.Text = (configuration.ShowByDomain) ? _asset.Domain : _asset.Location;

			// Get Asset Types and load into the combo box
			_listAssetTypes.Populate();

			// Load just the categories
			AssetTypeList categories = _listAssetTypes.EnumerateCategories();

			// ...and add to the combo
			foreach (AssetType assetType in categories)
			{
				// Add categories only if they have children
				if (_listAssetTypes.EnumerateChildren(assetType.AssetTypeID).Count != 0)
					this.cbAssetCategory.Items.Add(assetType);
			}

			// Select the first asset type
			if (this.cbAssetCategory.Items.Count > 0)
				this.cbAssetCategory.SelectedIndex = 0;

			// Select the asset name as the user will need to change this
			this.tbAssetName.Focus();
			this.tbAssetName.SelectAll();
		}


		/// <summary>
		/// Called as we click OK to exit from this form - add the new asset
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void bnOK_Click(object sender, EventArgs e)
		{
            AddAsset();
		}

        private void AddAsset()
        {
            AssetDAO lwDataAccess = new AssetDAO();
            _asset.Name = this.tbAssetName.Text;

            // Set the asset type
            AssetType assetType = cbAssetType.Value as AssetType;
            _asset.AssetTypeID = assetType.AssetTypeID;

            // Do we have a name for the asset and is it unique?
            if ((_asset.Name == "") || (lwDataAccess.AssetFind(_asset) != 0))
            {
                MessageBox.Show("You must specify a unique name for this asset", "Invalid Asset Name");
                this.DialogResult = DialogResult.None;
                return;
            }

            // OK - unique asset so create it
            _asset.AssetID = _asset.Add();
        }


		/// <summary>
		/// Called as we change the selected asset category to display the sub-types (if any)
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void cbAssetCategory_ValueChanged(object sender, EventArgs e)
		{
			this.cbAssetType.BeginUpdate();
			this.cbAssetType.Items.Clear();
			//
			AssetType category = this.cbAssetCategory.Value as AssetType;
			foreach (AssetType assetType in _listAssetTypes.EnumerateChildren(category.AssetTypeID))
			{
				this.cbAssetType.Items.Add(assetType);
			}

			// Select the first asset type
			if (this.cbAssetType.Items.Count > 0)
				this.cbAssetType.SelectedIndex = 0;

			this.cbAssetType.EndUpdate();
		}
	}
}

