using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Reflection;
using System.Configuration;
using Layton.Cab.Interface;

namespace Layton.Cab.Shell
{
    public partial class LaytonRegistrationForm : Form
    {
        private LaytonProductKey productKey;
        private const string ButtonTextExit = "Exit";
        private const string ButtonTextContinue = "Continue";

        public LaytonRegistrationForm(LaytonProductKey productKey)
        {
            InitializeComponent();
            this.productKey = productKey;
            this.DialogResult = DialogResult.Cancel;

            try
            {
                // set the form's background image and icon
                Bitmap bgImage = new Bitmap(Properties.Settings.Default.appAboutScreen);
                this.BackgroundImage = bgImage;
                this.Size = bgImage.Size;

                // set the form's icon
                this.Icon = new Icon(Properties.Settings.Default.appIcon);
            }
            catch
            {
                // use the default layton images
            }
            AddLicensing();
        }

        public LaytonProductKey ProductKey
        {
            get { return productKey; }
        }

        private void AddLicensing()
        {
            licensingTextBox.Text = "";
            if (productKey.IsTrialExpired)
            {
                licensingTextBox.Text += "Your trial period has expired." + System.Environment.NewLine;
                continueButton.Text = ButtonTextExit;
            }
            else if (productKey.IsTrial)
            {
                licensingTextBox.Text += "This trial version will expire in " + productKey.TrialDaysRemaining.ToString() + " days." + System.Environment.NewLine;
                continueButton.Text = ButtonTextContinue;
            }
            else
            {
                licensingTextBox.Text += "Company Name:  " + productKey.CompanyName + Environment.NewLine;
                licensingTextBox.Text += "Company ID:  " + productKey.CompanyID.ToString() + Environment.NewLine;
                licensingTextBox.Text += "Licensed for :  " + productKey.AssetCount.ToString() + " Assets";
            }
            licensingTextBox.Text += "Please visit http://www.laytontechnology.com for licensing information.";
        }

        private void registerButton_Click(object sender, EventArgs e)
        {
            if (companyNameTextBox.Text == null || companyNameTextBox.Text == "")
            {
                MessageBox.Show("Please enter a value for the Company Name.", "Product Registration", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }
            if (companyIdTextBox.Text == null || companyIdTextBox.Text == "")
            {
                MessageBox.Show("Please enter a value for the Company ID.", "Product Registration", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }
            if (productKeyTextBox.Text == null || productKeyTextBox.Text == "")
            {
                MessageBox.Show("Please enter a value for the Product Key.", "Product Registration", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }
            int companyID;
            try
            {
                companyID = Convert.ToInt32(companyIdTextBox.Text);
            }
            catch
            {
                MessageBox.Show("Please enter a valid value for the Company ID.", "Product Registration", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }
            LaytonProductKey newKey = new LaytonProductKey(productKeyTextBox.Text, productKey.ProductID, companyNameTextBox.Text, companyID, "");
            if (newKey.IsTrial)
            {
                MessageBox.Show("The registration information you have provided is invalid. If you feel this is a mistake, please contact support.", "Product Registration", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            else
            {
                // save the registration information
                Configuration config = ConfigurationManager.OpenExeConfiguration("Layton.Cab.Shell.exe");
                config.AppSettings.Settings["ProductKey"].Value = newKey.ProductKey;
                config.AppSettings.Settings["CompanyName"].Value = newKey.CompanyName;
                config.AppSettings.Settings["CompanyID"].Value = newKey.CompanyID.ToString();
                config.AppSettings.Settings["Code2"].Value = LaytonProductKey.GenerateCode2();
                config.Save();

                MessageBox.Show("Successful registration! Press OK to continue.", "Product Registration");
                productKey = newKey;
                this.DialogResult = DialogResult.OK;
                Close();
            }
        }

        private void continueButton_Click(object sender, EventArgs e)
        {
            if (continueButton.Text == ButtonTextExit)
            {
                Application.Exit();
            }
            else
            {
                Close();
            }
        }

        private void getKeyButton_Click(object sender, EventArgs e)
        {
            string activateUrl = "http://www.laytontechnology.com/support/product_activation.asp";
            try
            {
                if (companyIdTextBox.Text.Length > 0)
                {
                    long companyID = Convert.ToInt64(companyIdTextBox.Text);
                    activateUrl += "?companyid=" + companyIdTextBox.Text;
                    if (companyNameTextBox.Text.Length > 0)
                    {
                        activateUrl += "&company=" + companyNameTextBox.Text;
                    }
                }
            }
            catch
            {
                MessageBox.Show("Your company ID must be a numeric value", "Product Registration", MessageBoxButtons.OK, MessageBoxIcon.Information);
                companyIdTextBox.Focus();
                return;
            }
            System.Diagnostics.Process.Start(activateUrl);
        }

        private void LaytonRegistrationForm_Shown(object sender, EventArgs e)
        {
            Activate();
        }
    }
}