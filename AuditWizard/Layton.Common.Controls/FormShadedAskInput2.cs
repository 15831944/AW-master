using System;
using System.Drawing;
using System.Windows.Forms;
using Microsoft.Win32;

namespace Layton.Common.Controls
{
	public partial class FormShadedAskInput2 : AWForm
	{
		public delegate bool ValidateInputValues(string strValue1, string strValue2, ref string strError, ref string strTitle);

		private bool _isPassword;
	    private bool _hKeyLocalMachineOnly;

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

        public FormShadedAskInput2(string strDescription, string strTitle, string strLabel1, string strLabel2, string strValue1, string strValue2, Bitmap image, bool hKeyLocalMachineOnly)
		{
			InitializeComponent();

			Text = strTitle;
			lblDescription.Text = strDescription;
			lblInput1.Text = strLabel1;
			lblInput2.Text = strLabel2;
			tbValue1Entered.Text = strValue1;
			tbValue2Entered.Text = strValue2;
			_isPassword = false;
            _hKeyLocalMachineOnly = hKeyLocalMachineOnly;
		}


		private void FormShadedAskInput2_Load(object sender, EventArgs e)
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
				if (ValidateEntry(tbValue1Entered.Text, tbValue2Entered.Text, ref strErrorText ,ref strErrorTitle) != true)
				{
					MessageBox.Show(strErrorText ,strErrorTitle);
					DialogResult = DialogResult.None;
				}
			}
		}

        private void bnRegistryBrowse_Click(object sender, EventArgs e)
        {
            FormRegistryBrowser form = new FormRegistryBrowser(_hKeyLocalMachineOnly);
            if (form.ShowDialog() == DialogResult.OK)
            {
                tbValue1Entered.Text = _hKeyLocalMachineOnly ? form.RegKey.Replace(Registry.LocalMachine.Name + "\\", "") : form.RegKey;
                tbValue2Entered.Text = form.RegValue;
            }
        }
	}
}

