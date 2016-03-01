using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
//
using Layton.AuditWizard.Common;
using Layton.Common.Controls;

namespace Layton.AuditWizard.Overview
{
	public partial class FormEmailConfiguration : ShadedImageForm
	{
		public FormEmailConfiguration()
		{
			InitializeComponent();
		
			// Load the initial email settings into the control
			emailConfigurationControl.InitializeEmailSettings();
		}

		private void bnOK_Click(object sender, EventArgs e)
		{
			emailConfigurationControl.SaveEmailSettings();
		}
	}
}