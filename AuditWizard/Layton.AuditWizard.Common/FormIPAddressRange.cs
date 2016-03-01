using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
//
using Infragistics.Win.UltraWinTree;
//
using Layton.AuditWizard.DataAccess;
using Layton.Common.Controls;

namespace Layton.AuditWizard.Common
{
	public partial class FormIPAddressRange : ShadedImageForm
	{
		private string _lower;
		private string _upper;
		
		public string Lower
		{ get { return _lower; } }

		public string Upper
		{ get { return _upper; } }

		public FormIPAddressRange(string lower ,string upper)
		{
			InitializeComponent();

			// Set initial values
			startIPAddress.Text = lower;
			endIPAddress.Text = upper;
		}


		/// <summary>
		/// Ensure that the IP address range entered is valid
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void bnOK_Click(object sender, EventArgs e)
		{
			// If either address has not been specified then this is not valid
			if ((startIPAddress.Text == "") || (endIPAddress.Text == ""))
			{
				MessageBox.Show("You must specify both a start and end IP address" ,"Range Error");
				startIPAddress.Focus();
				DialogResult = DialogResult.None;
			}
			
			else if (IPAddressComparer.IsGreater(startIPAddress.Text ,endIPAddress.Text))
			{
				MessageBox.Show("The starting IP Address must be less than the ending address" ,"Range Error");
				startIPAddress.Focus();
				DialogResult = DialogResult.None;
			}
				
			else
			{
				// OK save the values
				_lower = startIPAddress.Text;
				_upper = endIPAddress.Text;
			}
		}

	}
}