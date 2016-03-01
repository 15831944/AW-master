using System;
//
using Layton.AuditWizard.Common;

namespace Layton.AuditWizard.Administration
{
	public partial class FormSoftwareDetails : Layton.Common.Controls.ShadedImageForm
	{
		AuditScannerDefinition _scannerConfiguration;

		public FormSoftwareDetails(AuditScannerDefinition scannerConfiguration)
		{
			_scannerConfiguration = scannerConfiguration;
			InitializeComponent();
		}

		/// <summary>
		/// Called as the form loads - set the initial status of the items on this form
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void FormSoftwareDetails_Load(object sender, EventArgs e)
		{
			this.cbApplications.Checked = _scannerConfiguration.SoftwareScanApplications;
			this.cbOperatingSystem.Checked = _scannerConfiguration.SoftwareScanOs;
		}

		private void bnOK_Click(object sender, EventArgs e)
		{
			_scannerConfiguration.SoftwareScanApplications = cbApplications.Checked;
			_scannerConfiguration.SoftwareScanOs = cbOperatingSystem.Checked;
		}
	}
}

