using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;
//
using Layton.AuditWizard.Common;
using Layton.Common.Controls;
using Layton.AuditWizard.DataAccess;

namespace Layton.AuditWizard.Administration
{
	public partial class FormAssetCategory : ShadedImageForm
	{
		private AssetType _assetType;

		public FormAssetCategory(AssetType assettype )
		{
			InitializeComponent();
			_assetType = assettype;

			if (assettype.AssetTypeID == 0)
			{
				this.Text = "New Asset Category";
			}
			else
			{
				this.Text = "Asset Category Properties";
			}

			tbCategoryName.Text = _assetType.Name;
			tbIconFile.Text = _assetType.Icon;
		}


		/// <summary>
		/// Called as we browse for a new image file to display
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void bnBrowse_Click(object sender, EventArgs e)
		{
			string iconsFolder = Path.Combine(Application.StartupPath ,"Icons");
			this.openFileDialog.InitialDirectory = iconsFolder;
			if (this.openFileDialog.ShowDialog() == DialogResult.OK)
			{
				// Get the path as if this is NOT within the ICONS folder then we will need to copy the
				// specified file from it's current location into the icons folder
				string iconName = openFileDialog.FileName;
				string iconPath = Path.GetDirectoryName(iconName);
				string iconFileName = Path.GetFileName(iconName);

				if (iconPath != iconsFolder)
				{
					try
					{
						File.Copy(iconName, Path.Combine(iconsFolder, iconFileName));
					}
					catch (Exception ex)
					{
						//MessageBox.Show("Failed to copy selected icon into the programs icon folder, reason: " + ex.Message, "Copy Failed");
						return;
					}
				}

				// Set the name of the new file in the display
				tbIconFile.Text = iconFileName;
			}
		}


		/// <summary>
		/// Called as the displayed text changes as this should cause the preview icon to change also
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void tbIconFile_TextChanged(object sender, EventArgs e)
		{
			if (tbIconFile.Name == "")
				return;
			pbIcon.Image = IconMapping.LoadIcon(tbIconFile.Text, IconMapping.Iconsize.Medium);
		}


		/// <summary>
		/// Called as we try an exit from this form - we need to ensure that the category name is unique
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void bnOK_Click(object sender, EventArgs e)
		{
			// Update the asset type definition with any changes
			_assetType.Name = tbCategoryName.Text;
			_assetType.Icon = tbIconFile.Text;
			//
			AssetTypeList listAssetTypes = new AssetTypeList();
			listAssetTypes.Populate();

			// Does this name duplicate an existing item?
			AssetType existingAsset = listAssetTypes.FindByName(tbCategoryName.Text);
			if (existingAsset != null)
			{
				if (_assetType.AssetTypeID != existingAsset.AssetTypeID)
				{
					MessageBox.Show("An Asset Category of this name has already been created, please enter a different name for this category");
					tbCategoryName.Focus();
					this.DialogResult = DialogResult.None;
					return;
				}
			}

			// OK the name is fine so either create a new category or update an existing one
			_assetType.Add();
		}
	}
}