using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Layton.Common.Controls
{
	// Validation Delegates
	public delegate bool ValidateInputValues(string strValue1 ,string strValue2 ,ref string strError ,ref string strTitle);

	public partial class FormAskInput2 : Form
	{
		private bool _isPassword;

		public bool IsPassword
		{
			get { return _isPassword; }
			set { _isPassword = value; }
		}

		// Event handler
		public event ValidateInputValues ValidateEntry;

		public string Value1Entered ()
		{ return this.tbValue1Entered.Text; }

		public string Value2Entered ()
		{ return this.tbValue2Entered.Text; }

		public void EnableField1 (bool bEnable)
		{ this.tbValue1Entered.Enabled = bEnable; }

		public void EnableField2 (bool bEnable)
		{ this.tbValue2Entered.Enabled = bEnable; }

		public void SetPasswordField1 (bool bEnable)
		{ this.tbValue1Entered.UseSystemPasswordChar = bEnable; }

		public void SetPasswordField2 (bool bEnable)
		{ this.tbValue2Entered.UseSystemPasswordChar = bEnable; }

		public FormAskInput2(string strDescription ,string strTitle ,string strLabel1 ,string strLabel2 ,string strValue1 ,string strValue2)
		{
			InitializeComponent();

			this.Text = strTitle;
			this.lblDescription.Text = strDescription;
			this.lblInput1.Text = strLabel1;
			this.lblInput2.Text = strLabel2;
			this.tbValue1Entered.Text = strValue1;
			this.tbValue2Entered.Text = strValue2;
			this._isPassword = false;
		}


		private void FormAskInput2_Load(object sender, EventArgs e)
		{
			if (IsPassword)
			{
				this.tbValue1Entered.PasswordChar = '*';
				this.tbValue2Entered.PasswordChar = '*';
			}
		}

		/// <summary>
		/// We insist in this dialog that both values are entered
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void tbValue1Entered_TextChanged(object sender, EventArgs e)
		{
			this.bnOK.Enabled = (this.tbValue1Entered.Text.Length != 0);
		}

		private void bnOK_Click(object sender, EventArgs e)
		{
			// Do we have a validation event defined if so call it
			if (this.ValidateEntry != null)
			{
				string strErrorText = "";
				string strErrorTitle = "";
				if (this.ValidateEntry(this.tbValue1Entered.Text, this.tbValue2Entered.Text, ref strErrorText ,ref strErrorTitle) != true)
				{
					MessageBox.Show(strErrorText ,strErrorTitle);
					DialogResult = DialogResult.None;
				}
			}
		}
	}
}