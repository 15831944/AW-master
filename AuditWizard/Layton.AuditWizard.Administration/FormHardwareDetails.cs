using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
//
using Layton.AuditWizard.Common;

namespace Layton.AuditWizard.Administration
{
	public partial class FormHardwareDetails : Layton.Common.Controls.ShadedImageForm
	{
		AuditScannerDefinition _scannerConfiguration;

		public FormHardwareDetails(AuditScannerDefinition scannerConfiguration)
		{
			_scannerConfiguration = scannerConfiguration;
			InitializeComponent();
		}

		/// <summary>
		/// Called as the form loads - set the initial status of the items on this form
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void FormHardwareDetails_Load(object sender, EventArgs e)
		{
			this.cbActiveSystemComponents.Checked = _scannerConfiguration.HardwareSystem;
			this.cbNetworkDrives.Checked = _scannerConfiguration.HardwareNetworkDrives;
			this.cbPhysicalDisks.Checked = _scannerConfiguration.HardwarePhysicalDisks;
			this.cbSecuritySettings.Checked = _scannerConfiguration.HardwareSecurity;
			this.cbGeneralSettings.Checked = _scannerConfiguration.HardwareSettings;
		}

		private void bnOK_Click(object sender, EventArgs e)
		{
			_scannerConfiguration.HardwareActiveSystem = cbActiveSystemComponents.Checked;
			_scannerConfiguration.HardwareNetworkDrives = cbNetworkDrives.Checked;
			_scannerConfiguration.HardwarePhysicalDisks = cbPhysicalDisks.Checked;
			_scannerConfiguration.HardwareSecurity = cbSecuritySettings.Checked;
			_scannerConfiguration.HardwareSettings = cbGeneralSettings.Checked;
		}
	}
}

