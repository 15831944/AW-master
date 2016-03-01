using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;
using System.Reflection;
using System.Configuration;
using Layton.Cab.Interface;

namespace AuditWizardv8
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

        private void LaytonRegistrationForm_Load(object sender, EventArgs e)
        {
            Configuration config = ConfigurationManager.OpenExeConfiguration(Path.Combine(Application.StartupPath, "AuditWizardv8.exe"));
            tbProductKey.Text = config.AppSettings.Settings["ProductKey"].Value;
            tbCompanyName.Text = config.AppSettings.Settings["CompanyName"].Value;
            tbCompanyId.Text = config.AppSettings.Settings["CompanyID"].Value;
        }

        private void AddLicensing()
        {
            licensingTextBox.Text = "";

            if (productKey.IsTrial)
            {
                if (productKey.IsTrialExpired)
                {
                    licensingTextBox.Text += "Your trial period has expired." + Environment.NewLine;
                    continueButton.Text = ButtonTextExit;
                }
                else
                {
                    licensingTextBox.Text += "This trial version will expire in " + productKey.TrialDaysRemaining + " days." + Environment.NewLine;
                    continueButton.Text = ButtonTextContinue;
                }
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
            if (string.IsNullOrEmpty(tbCompanyName.Text))
            {
                MessageBox.Show("Please enter a value for the Company Name.", "Product Registration", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }
            if (string.IsNullOrEmpty(tbCompanyId.Text))
            {
                MessageBox.Show("Please enter a value for the Company ID.", "Product Registration", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }
            if (string.IsNullOrEmpty(tbProductKey.Text))
            {
                MessageBox.Show("Please enter a value for the Product Key.", "Product Registration", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }
            int companyID;
            try
            {
                companyID = Convert.ToInt32(tbCompanyId.Text);
            }
            catch
            {
                MessageBox.Show("Please enter a valid value for the Company ID.", "Product Registration", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }
            LaytonProductKey newKey = new LaytonProductKey(tbProductKey.Text, productKey.ProductID, tbCompanyName.Text, companyID, "");
            if (newKey.IsTrial)
            {
                MessageBox.Show("The registration information you have provided is invalid. If you feel this is a mistake, please contact support.", "Product Registration", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            else
            {
                // save the registration information
                Configuration config = ConfigurationManager.OpenExeConfiguration(Path.Combine(Application.StartupPath, "AuditWizardv8.exe"));
                config.AppSettings.Settings["ProductKey"].Value = newKey.ProductKey;
                config.AppSettings.Settings["CompanyName"].Value = newKey.CompanyName;
                config.AppSettings.Settings["CompanyID"].Value = newKey.CompanyID.ToString();
                config.AppSettings.Settings["Code2"].Value = LaytonProductKey.GenerateCode2();
                config.Save();

                MessageBox.Show("Successful registration. Press OK to continue.", "Product Registration");
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
			string activateUrl = "http://archive.laytontechnology.com/support/product_activation.asp";
            try
            {
                if (tbCompanyId.Text.Length > 0)
                {
                    long companyID = Convert.ToInt64(tbCompanyId.Text);
                    activateUrl += "?companyid=" + tbCompanyId.Text;
                    if (tbCompanyName.Text.Length > 0)
                    {
                        activateUrl += "&company=" + tbCompanyName.Text;
                    }
                }
            }
            catch
            {
                MessageBox.Show("Your company ID must be a numeric value", "Product Registration", MessageBoxButtons.OK, MessageBoxIcon.Information);
                tbCompanyId.Focus();
                return;
            }

            try
            {
                System.Diagnostics.Process.Start(activateUrl);
            }
            catch
            {
                MessageBox.Show("Failed to launch your default web browser to activate this product.  You will need to register manually by visiting the Layton Technology, Inc web site at www.laytontechnology.com", "Product Registration Failed", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

        }

        private void LaytonRegistrationForm_Shown(object sender, EventArgs e)
        {
            Activate();
        }
    }
}