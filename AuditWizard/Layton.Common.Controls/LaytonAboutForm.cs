using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Reflection;

namespace Layton.Common.Controls
{
    public partial class LaytonAboutForm : Form
    {
		LaytonProductKey _productKey = null;
		string _aboutBitmap;
		string _productIcon;
		string _productVersion;

        public LaytonAboutForm(LaytonProductKey productKey ,string bitmap ,string icon ,string version)
        {
			// Save inoput arguments
			_productKey = productKey;
			_aboutBitmap = bitmap;
			_productIcon = icon;
			_productVersion = version;
			
			// Initialize the form
            InitializeComponent();
            try
            {
                // set the form's background image and icon
				Bitmap bgImage = new Bitmap(bitmap);
                this.BackgroundImage = bgImage;
                this.Size = bgImage.Size;

                // set the form's icon
				this.Icon = new Icon(icon);
            }
            catch
            {
                // Use existing default images
            }
			this.versionLabel.Text = version;
            UpdateLicensing();
            AddModules();
        }

        private void UpdateLicensing()
        {
            if (_productKey.IsTrial)
            {
                licensingTextBox.Text = "Product is not registered." + Environment.NewLine;
                licensingTextBox.Text += "Trial expires in " + _productKey.TrialDaysRemaining.ToString() + " days.";
            }
            else
            {
                licensingTextBox.Text = "Product is registered:" + Environment.NewLine;
                licensingTextBox.Text += "Company Name:  " + _productKey.CompanyName + Environment.NewLine;
                licensingTextBox.Text += "Company ID:  " + _productKey.CompanyID.ToString() + Environment.NewLine;
                licensingTextBox.Text += "Licensed for :  " + _productKey.AssetCount.ToString() + " Assets";
            }
        }

        private void AddModules()
        {
            Assembly[] assemblies = System.AppDomain.CurrentDomain.GetAssemblies();
            foreach (Assembly assembly in assemblies)
            {
                AssemblyCompanyAttribute company = (AssemblyCompanyAttribute)Attribute.GetCustomAttribute(assembly, typeof(AssemblyCompanyAttribute));
                if (company != null)
                {
                    if (company.Company.Contains("Layton"))
                    {
                        modulesTextBox.Text += assembly.GetName().Name + ", " + assembly.GetName().Version.ToString() + System.Environment.NewLine;
                    }
                }
            }
        }

        public string LicenseInfo
        {
            get { return licensingTextBox.Text; }
            set { licensingTextBox.Text = value; }
        }


		/// <summary>
		/// Called as we click on the register button within the about screen - we need to recover any current
		/// product key and allow the user to (re)register this product
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
        private void registerButton_Click(object sender, EventArgs e)
        {
            LaytonRegistrationForm registerForm = new LaytonRegistrationForm(_productKey ,_aboutBitmap ,_productIcon);
            registerForm.BringToFront();
            registerForm.ShowDialog();
        }
    }
}