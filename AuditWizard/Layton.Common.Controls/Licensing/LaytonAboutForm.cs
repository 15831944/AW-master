using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Reflection;
using Layton.Cab.Interface;

namespace Layton.Cab.Shell
{
    public partial class LaytonAboutForm : Form
    {
        private LaytonCabShellWorkItem workItem;
        public LaytonAboutForm(LaytonCabShellWorkItem workItem)
        {
            InitializeComponent();
            this.workItem = workItem;
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
                // Use existing default images
            }
            this.versionLabel.Text = Properties.Settings.Default.appVersion;
            UpdateLicensing();
            AddModules();
        }

        private void UpdateLicensing()
        {
            LaytonProductKey productKey = workItem.Items[MiscStrings.ProductKey] as LaytonProductKey;
            if (productKey.IsTrial)
            {
                licensingTextBox.Text = "Product is not registered." + Environment.NewLine;
                licensingTextBox.Text += "Trial expires in " + productKey.TrialDaysRemaining.ToString() + " days.";
            }
            else
            {
                licensingTextBox.Text = "Product is registered:" + Environment.NewLine;
                licensingTextBox.Text += "Company Name:  " + productKey.CompanyName + Environment.NewLine;
                licensingTextBox.Text += "Company ID:  " + productKey.CompanyID.ToString() + Environment.NewLine;
                licensingTextBox.Text += "Licensed for :  " + productKey.AssetCount.ToString() + " Assets";
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

        private void registerButton_Click(object sender, EventArgs e)
        {
            LaytonProductKey productKey = workItem.Items[MiscStrings.ProductKey] as LaytonProductKey;
            LaytonRegistrationForm registerForm = new LaytonRegistrationForm(productKey);
            registerForm.BringToFront();
            DialogResult result = registerForm.ShowDialog();
            if (result == DialogResult.OK)
            {
                workItem.Items.Remove(productKey);
                productKey = registerForm.ProductKey;
                workItem.Items.Add(productKey, MiscStrings.ProductKey);
            }
        }
    }
}