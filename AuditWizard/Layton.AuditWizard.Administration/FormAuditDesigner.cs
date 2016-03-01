using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
//
using Layton.Common.Controls;
using Layton.AuditWizard.Common;

namespace Layton.AuditWizard.Administration
{
	public partial class FormAuditDesigner : ShadedImageForm
	{
		private AuditScannerDefinition _scannerConfiguration = null;

		public FormAuditDesigner(AuditScannerDefinition scannerConfiguration)
		{
			InitializeComponent();

			_scannerConfiguration = scannerConfiguration;
		}


		/// <summary>
		/// Called as the form is loaded to set any fields already defined in the configuration
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void FormAuditDesigner_Load(object sender, EventArgs e)
		{
			cbAllowCancel.Checked = _scannerConfiguration.InteractiveAllowCancel;
			//
			cbDisplayBasicInformation.Checked = _scannerConfiguration.InteractiveDisplayAssetInformation;
			cbAssetCategory.Checked = _scannerConfiguration.InteractiveEnableAssetCategory;
            cbAssetSerial.Checked = _scannerConfiguration.InteractiveEnableAssetSerial;
			cbAssetMake.Checked = _scannerConfiguration.InteractiveEnableAssetMake;
			cbAssetModel.Checked = _scannerConfiguration.InteractiveEnableAssetModel;
			//
			cbDisplayAssetLocation.Checked = _scannerConfiguration.InteractiveDisplayLocations;
			//cbAddLocations.Checked = _scannerConfiguration.InteractiveEnableAddLocation;
			//
			cbAssetDataScreens.Checked = _scannerConfiguration.InteractiveDisplayAssetData;
		}

		private void cbDisplayBasicInformation_CheckedChanged(object sender, EventArgs e)
		{
			gbAssetInformation.Enabled = cbDisplayBasicInformation.Checked;
		}

		private void cbDisplayAssetLocation_CheckedChanged(object sender, EventArgs e)
		{
			//gbAssetLocation.Enabled = cbDisplayAssetLocation.Checked;
		}


		/// <summary>
		/// Called as we exit this form - save data back to the configuration
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void bnOK_Click(object sender, EventArgs e)
		{
			_scannerConfiguration.InteractiveAllowCancel = cbAllowCancel.Checked;
			//
            _scannerConfiguration.InteractiveEnableAssetSerial = cbAssetSerial.Checked;
			_scannerConfiguration.InteractiveDisplayAssetInformation = cbDisplayBasicInformation.Checked;
			_scannerConfiguration.InteractiveEnableAssetCategory = cbAssetCategory.Checked;
			_scannerConfiguration.InteractiveEnableAssetMake = cbAssetMake.Checked;
			_scannerConfiguration.InteractiveEnableAssetModel = cbAssetModel.Checked;
			//
			_scannerConfiguration.InteractiveDisplayLocations = cbDisplayAssetLocation.Checked;
			//_scannerConfiguration.InteractiveEnableAddLocation = cbAddLocations.Checked;
            _scannerConfiguration.InteractiveEnableAddLocation = false;
			//
			_scannerConfiguration.InteractiveDisplayAssetData = cbAssetDataScreens.Checked;
		}


	}
}