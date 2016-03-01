using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Layton.Common.Controls
{
	// Validation Delegate
	public delegate bool ValidateInput(string strValue ,ref string strError ,ref string strTitle);

	public partial class FormAskInput1 : Form
	{
		private bool _isPassword;

		// Event handler
		public event ValidateInput ValidateEntry;

		public string ValueEntered ()
		{ return this.tbValueEntered.Text; }

		public bool IsPassword
		{
			get { return _isPassword; }
			set { _isPassword = value; }
		}

		public FormAskInput1(string strDescription ,string strTitle ,string strLabel)
		{
			InitializeComponent();

			this.Text = strTitle;
			this.lblDescription.Text = strDescription;
			this.lblInput.Text = strLabel;
			this._isPassword = false;
		}


		/// <summary>
		/// Called as we load the form
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void FormAskInput1_Load(object sender, EventArgs e)
		{
			if (IsPassword)
				this.tbValueEntered.PasswordChar = '*';
		}

		/// <summary>
		/// OK only valid if we have entered  a server name
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void tbValueEntered_TextChanged(object sender, EventArgs e)
		{
			this.bnOK.Enabled = (this.tbValueEntered.Text.Length != 0);
		}

		private void bnOK_Click(object sender, EventArgs e)
		{
			// Do we have a validation event defined if so call it
			if (this.ValidateEntry != null)
			{
				string strErrorText = "";
				string strErrorTitle = "";
				if (this.ValidateEntry(this.tbValueEntered.Text, ref strErrorText, ref strErrorTitle) != true)
				{
					MessageBox.Show(strErrorText ,strErrorTitle);
					DialogResult = DialogResult.None;
				}
			}
		}
	}
}