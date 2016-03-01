using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
//
using Infragistics.Win.UltraWinListView;
//
using Layton.Common.Controls;
using Layton.AuditWizard.Common;

namespace Layton.AuditWizard.Administration
{
	public partial class FormInternetDetails : Layton.Common.Controls.ShadedImageForm
	{
		AuditScannerDefinition _scannerConfiguration;

		public FormInternetDetails(AuditScannerDefinition scannerConfiguration)
		{
			InitializeComponent();
			_scannerConfiguration = scannerConfiguration;
		}

		/// <summary>
		/// Called as the form is loading - populate from the configuration
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void FormInternetDetails_Load(object sender, EventArgs e)
		{
			cbHistory.Checked = _scannerConfiguration.IEHistory;
			//cbDetailedHistory.Checked = _scannerConfiguration.IEDetails;
			cbCookies.Checked = _scannerConfiguration.IECookies;
			nupLimitDays.Value = _scannerConfiguration.IEDays;
		}



		/// <summary>
		/// Called as we select to exit the form.  
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void bnOK_Click(object sender, EventArgs e)
		{
			_scannerConfiguration.IEHistory = cbHistory.Checked;
			//_scannerConfiguration.IEDetails = cbDetailedHistory.Checked;
			_scannerConfiguration.IECookies = cbCookies.Checked;
			_scannerConfiguration.IEDays = (int)nupLimitDays.Value;
		}

		private void cbHistory_CheckedChanged(object sender, EventArgs e)
		{
			//cbDetailedHistory.Enabled = cbHistory.Checked;
		}
	}
}

