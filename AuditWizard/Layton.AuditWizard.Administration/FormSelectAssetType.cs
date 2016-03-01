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
//
using Infragistics.Win.UltraWinTree;

namespace Layton.AuditWizard.Administration
{
	public partial class FormSelectAssetType : Layton.Common.Controls.ShadedImageForm
	{
		protected int _selectedAssetTypeID = 0;
		protected string _selectedAssetTypeName = "";

		public int SelectedAssetType
		{ get { return _selectedAssetTypeID; } }

		public string SelectedAssetTypeName
		{ get { return _selectedAssetTypeName; } } 


		public FormSelectAssetType(int currentType ,string currentName)
		{
			InitializeComponent();

			// handle and pre-existing type
			_selectedAssetTypeID = currentType;
			_selectedAssetTypeName = currentName;
		}

		private void FormSelectAssetType_Load(object sender, EventArgs e)
		{
			tvAssetTypes.BeginUpdate();
			this.Cursor = Cursors.WaitCursor;

			// get the currebnt list of asset types
			AssetTypeList listAssetTypes = new AssetTypeList();
			listAssetTypes.Populate();

			// We now need to display the Asset Type categories in the main ExplorerBar
			AssetTypeList categories = listAssetTypes.EnumerateCategories();
			foreach (AssetType category in categories)
			{
				Bitmap icon = IconMapping.LoadIcon(category.Icon, IconMapping.Iconsize.Small);
				UltraTreeNode categoryNode = tvAssetTypes.Nodes.Add(category.Name, category.Name);
				categoryNode.LeftImages.Add(icon);
				categoryNode.Tag = category;

				// Is this the selected category?
				if (category.AssetTypeID == _selectedAssetTypeID)
				{
					this.tbSelectedType.Text = category.Name;
					this.tbSelectedType.Tag = category;
				}

				// Add the child asset types to the list also
				// ...Get the children to display
				AssetTypeList listSubTypes = listAssetTypes.EnumerateChildren(category.AssetTypeID);

				// ...and add to the list view
				foreach (AssetType assettype in listSubTypes)
				{
					UltraTreeNode itemNode = categoryNode.Nodes.Add(category.Name + "|" + assettype.Name, assettype.Name);
					Bitmap icon2 = IconMapping.LoadIcon(assettype.Icon, IconMapping.Iconsize.Small);
					itemNode.LeftImages.Add(icon2);
					itemNode.Tag = assettype;

					// Is this the selected category?
					if (assettype.AssetTypeID == _selectedAssetTypeID)
					{
						this.tbSelectedType.Text = assettype.Name;
						this.tbSelectedType.Tag = assettype;
					}
				}
			}

			this.Cursor = Cursors.Default;
			tvAssetTypes.EndUpdate();
		}


		/// <summary>
		/// Called after we select a node is the tree - enable buttons depending on whether anything is selected
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void tvAssetTypes_AfterSelect(object sender, SelectEventArgs e)
		{
			// Enable the select buitton if we have a node selected
			//this.bnSelect.Enabled = (tvAssetTypes.SelectedNodes.Count != 0);

            AssetType selectedType = tvAssetTypes.SelectedNodes[0].Tag as AssetType;
            this.tbSelectedType.Text = selectedType.Name;
            this.tbSelectedType.Tag = selectedType;

            bnOK.Enabled = true;
		}


		/// <summary>
		/// Called as we click the select button to select the specific asset type to associate with this
		/// user defined category
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void bnSelect_Click(object sender, EventArgs e)
		{
			if (tvAssetTypes.SelectedNodes.Count == 0)
				return;

			AssetType selectedType = tvAssetTypes.SelectedNodes[0].Tag as AssetType;
			this.tbSelectedType.Text = selectedType.Name;
			this.tbSelectedType.Tag = selectedType;
		}

		private void bnClear_Click(object sender, EventArgs e)
		{
			this.tbSelectedType.Text = "";
			this.tbSelectedType.Tag = null;
		}

		private void bnOK_Click(object sender, EventArgs e)
		{
			if (tbSelectedType.Tag == null)
			{
				_selectedAssetTypeID = 0;
				_selectedAssetTypeName = "";
			}
			else
			{
				AssetType assetType = tbSelectedType.Tag as AssetType;
				_selectedAssetTypeID = assetType.AssetTypeID;
				_selectedAssetTypeName = assetType.Name;
			}
		}
	}
}

