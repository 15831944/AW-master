using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
//
using Infragistics.Win;
//
using Layton.AuditWizard.Common;
using Layton.Common.Controls;
using Layton.AuditWizard.DataAccess;

namespace Layton.AuditWizard.Applications
{
    public partial class FormLicenseProperties : ShadedImageForm
    {
        #region Data

        private String _applicationName;
        private ApplicationLicense _applicationLicense;

        #endregion Data

        #region Properties

        /// <summary>
        /// Return the application license object
        /// </summary>
        public ApplicationLicense GetLicense
        {
            get { return _applicationLicense; }
        }

        #endregion Properties

        #region Constructor

        public FormLicenseProperties(String applicationName, ApplicationLicense theLicense)
        {
            InitializeComponent();
            _applicationName = applicationName;
            _applicationLicense = theLicense;

            // Set the link types for the Supplier Email and Web Site fields
            tbSupplierContactEmail.LinkType = Layton.Common.Controls.LinkTypes.Email;
            tbSupplierWWW.LinkType = Layton.Common.Controls.LinkTypes.Http;
        }

        #endregion Constructor

        #region Form Control Functions

        private void FormLicenseProperties_Load(object sender, EventArgs e)
        {
            // Initialize the data to be displayed on each tab
            InitializeLicenseTab();
            //
            InitializeSupportContractTab();
            //
            InitializeSupplierTab();
            //
            InitializeNotesTab();
            //
            InitializeDocumentsTab();
        }


        /// <summary>
        /// Called as we click the OK button - save the definition back to the database
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void bnOK_Click(object sender, EventArgs e)
        {
            // Take a copy of the source license (if any) so that we know what changes have been
            // made.  We can then check these later
            ApplicationLicense updatedLicense = new ApplicationLicense(_applicationLicense);

            // Save any changes made on the General tab
            SaveLicenseTab(updatedLicense);

            // Save Support Contract Tab
            SaveSupportContractTab(updatedLicense);

            // Save Supplier Tab
            SaveSupplierTab(updatedLicense);

            // Update the definition with entered values remembering that we need to audit any
            // changes so that we know hat has happened and when
            LicenseType licenseType = cbLicenseType.SelectedItem.DataValue as LicenseType;
            updatedLicense.LicenseTypeID = licenseType.LicenseTypeID;
            updatedLicense.LicenseTypeName = licenseType.Name;
            updatedLicense.UsageCounted = licenseType.PerComputer;
            updatedLicense.Count = (int)nupLicenseCount.Value;

            // ...and update the database if the object has changed
            updatedLicense.Update(_applicationLicense);

            // update any notes in notesControl with the new found application license id
            foreach (Note note in notesControl.Notes)
            {
                if (note.ParentID == 0)
                {
                    note.ParentID = updatedLicense.LicenseID;
                    note.Scope = SCOPE.License;
                    note.Update();
                }
            }

            // same for documents
            foreach (Document document in documentsControl.Documents)
            {
                if (document.ParentID == 0)
                {
                    document.ParentID = updatedLicense.LicenseID;
                    document.Scope = SCOPE.License;
                    document.Update();
                }
            }

            _applicationLicense = updatedLicense;
        }

        #endregion Form Control Functions

        #region License Tab Functions


        /// <summary>
        /// Initialize the data displayed on the license tab
        /// </summary>
        private void InitializeLicenseTab()
        {
            // Load the application license types
            // Populate the list view with the existing license types
            LicenseTypesDAO lwDataAccess = new LicenseTypesDAO();
            DataTable licenseTypesTable = lwDataAccess.EnumerateLicenseTypes();

            // Iterate through the returned items
            cbLicenseType.BeginUpdate();
            foreach (DataRow row in licenseTypesTable.Rows)
            {
                LicenseType licenseType = new LicenseType(row);
                cbLicenseType.Items.Add(licenseType, licenseType.Name);
            }
            cbLicenseType.SortStyle = ValueListSortStyle.Ascending;
            cbLicenseType.EndUpdate();

            // OK are we editing an existing definition or trying to create a new one?
            if (_applicationLicense.LicenseID == 0)
            {
                cbLicenseType.SelectedIndex = 0;
                footerPictureBox.Image = Properties.Resources.application_license_add_corner;
                this.Text = "Add Application License";
            }
            else
            {
                footerPictureBox.Image = Properties.Resources.application_license_edit_corner;
                this.Text = "Edit Application License";
                cbLicenseType.SelectedIndex = cbLicenseType.FindStringExact(_applicationLicense.LicenseTypeName);
                nupLicenseCount.Value = _applicationLicense.Count;
            }

            tbApplication.Text = _applicationName;
        }


        /// <summary>
        /// Called as we change the application type - determine whether or not a count is valid
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cbLicenseType_SelectionChanged(object sender, EventArgs e)
        {
            LicenseType selectedType = cbLicenseType.SelectedItem.DataValue as LicenseType;
            lblLicenseCount.Enabled = selectedType.PerComputer;
            nupLicenseCount.Enabled = selectedType.PerComputer;
        }


        /// <summary>
        /// Save the data entered on the license tab
        /// </summary>
        /// <param name="updatedLicense"></param>
        private void SaveLicenseTab(ApplicationLicense updatedLicense)
        {

        }


        #endregion License Tab Functions

        #region Support Contract Tab Functions

        private void InitializeSupportContractTab()
        {
            // Support information
            cbSupported.Checked = _applicationLicense.Supported;
            if (cbSupported.Checked)
            {
                deSupportExpiryDate.Value = _applicationLicense.SupportExpiryDate;
                cbSupportAlert.Checked = (_applicationLicense.SupportAlertDays != -1);
                if (cbSupportAlert.Checked)
                {
                    nupSupportDays.Value = _applicationLicense.SupportAlertDays;
                    cbSupportAlertEmail.Checked = _applicationLicense.SupportAlertEmail;
                }

                // Display the alert if the support has lareday expired - or is about to
                TimeSpan ts = DateTime.Today - _applicationLicense.SupportExpiryDate;
                this.panelSupportExpired.Visible = (ts.Days >= 0);
                lblSupportExpired.Text = (ts.Days == 0) ? "ALERT - this support contract expires today." : (ts.Days > 0) ? "ALERT - this support contract has expired!" : "";
            }
        }


        private void cbSupported_CheckedChanged(object sender, EventArgs e)
        {
            this.panelSupported.Enabled = cbSupported.Checked;
        }

        private void cbSupportAlert_CheckedChanged(object sender, EventArgs e)
        {
            this.panelAlertSupportExpired.Enabled = cbSupportAlert.Checked;
        }


        /// <summary>
        /// Save any changes made on the Support Contract tab
        /// </summary>
        /// <param name="updatedLicense"></param>
        private void SaveSupportContractTab(ApplicationLicense updatedLicense)
        {
            // Support Information
            updatedLicense.Supported = cbSupported.Checked;
            if (updatedLicense.Supported)
            {
                updatedLicense.SupportExpiryDate = deSupportExpiryDate.DateTime;
                updatedLicense.SupportAlertDays = (cbSupportAlert.Checked) ? (int)nupSupportDays.Value : -1;
                updatedLicense.SupportAlertEmail = cbSupportAlertEmail.Checked;
            }
        }

        #endregion Support Contract Tab Functions

        #region Supplier Functions

        /// <summary>
        /// Initialize the supplier tab
        /// </summary>
        private void InitializeSupplierTab()
        {
            SuppliersDAO lwDataAccess = new SuppliersDAO();
            DataTable suppliersTable = lwDataAccess.EnumerateSuppliers();

            // Add these to our combo box
            foreach (DataRow row in suppliersTable.Rows)
            {
                this.cbSuppliers.Items.Add(new Supplier(row));
            }

            // Select the 'current' supplier or the first if none
            int index = 0;
            if (_applicationLicense.SupplierID > 1)
                index = cbSuppliers.FindStringExact(_applicationLicense.SupplierName);
            if (index == -1)
                index = 0;
            cbSuppliers.SelectedIndex = index;
        }



        /// <summary>
        /// Called as we click New to create a new supplier
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void bnNewSupplier_Click(object sender, EventArgs e)
        {
            FormAskInput1 askSupplier = new FormAskInput1("Please enter the name of a new Supplier", "New Supplier", "Supplier:");
            if (askSupplier.ShowDialog() == DialogResult.OK)
            {
                // Ensure that this name is not already in our list and if not add and select it
                string newSupplierName = askSupplier.ValueEntered();
                int supplierIndex = cbSuppliers.FindStringExact(newSupplierName);
                if (supplierIndex != -1)
                {
                    MessageBox.Show("This Supplier already exists and will be selected", "Supplier Already Exists");
                    cbSuppliers.SelectedIndex = supplierIndex;
                }
                else
                {
                    // This is a new supplier - Add to the list and select it
                    Supplier newSupplier = new Supplier();
                    newSupplier.Name = newSupplierName;
                    ValueListItem newItem = cbSuppliers.Items.Add(newSupplier);
                    cbSuppliers.SelectedItem = newItem;
                }
            }
        }


        /// <summary>
        /// Called as we change the selection in the Supplier combo.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cbSuppliers_SelectionChanged(object sender, EventArgs e)
        {
            // Supplier Information - get the selected supplier
            Supplier theSupplier = cbSuppliers.SelectedItem.DataValue as Supplier;

            // If this is a new supplier then we need to allow the user to enter its details
            bool newSupplier = (theSupplier.SupplierID == 0);
            tbSupplierAddress1.ReadOnly = !newSupplier;
            tbSupplierAddress2.ReadOnly = !newSupplier;
            tbSupplierCity.ReadOnly = !newSupplier;
            tbSupplierState.ReadOnly = !newSupplier;
            tbSupplierZip.ReadOnly = !newSupplier;
            tbSupplierTelephone.ReadOnly = !newSupplier;
            tbSupplierContactName.ReadOnly = !newSupplier;
            tbSupplierContactEmail.ReadOnly = !newSupplier;
            tbSupplierFax.ReadOnly = !newSupplier;
            tbSupplierWWW.ReadOnly = !newSupplier;

            // Populate the supplier details
            tbSupplierAddress1.Text = theSupplier.AddressLine1;
            tbSupplierAddress2.Text = theSupplier.AddressLine2;
            tbSupplierCity.Text = theSupplier.City;
            tbSupplierState.Text = theSupplier.State;
            tbSupplierZip.Text = theSupplier.Zip;
            tbSupplierTelephone.Text = theSupplier.Telephone;
            tbSupplierContactName.Text = theSupplier.Contact;
            tbSupplierContactEmail.Text = theSupplier.Email;
            tbSupplierFax.Text = theSupplier.Fax;
            tbSupplierWWW.Text = theSupplier.WWW;
        }


        /// <summary>
        /// Save any changes made on the Support Contract tab
        /// </summary>
        /// <param name="updatedLicense"></param>
        private void SaveSupplierTab(ApplicationLicense updatedLicense)
        {
            Supplier theSupplier = cbSuppliers.SelectedItem.DataValue as Supplier;

            // If there is no supplier index this means that we are creating the supplier
            if (theSupplier.SupplierID == 0)
            {
                theSupplier.Name = cbSuppliers.Text;
                theSupplier.AddressLine1 = tbSupplierAddress1.Text;
                theSupplier.AddressLine2 = tbSupplierAddress2.Text;
                theSupplier.City = tbSupplierCity.Text;
                theSupplier.State = tbSupplierState.Text;
                theSupplier.Zip = tbSupplierZip.Text;
                theSupplier.Telephone = tbSupplierTelephone.Text;
                theSupplier.Contact = tbSupplierContactName.Text;
                theSupplier.Email = tbSupplierContactEmail.Text;
                theSupplier.Fax = tbSupplierFax.Text;
                theSupplier.WWW = tbSupplierWWW.Text;
            }

            // Add or update the supplier
            theSupplier.Update(null);
            updatedLicense.SupplierID = theSupplier.SupplierID;
        }

        #endregion Supplier Functions


        #region Notes Tab Functions

        protected void InitializeNotesTab()
        {
            if (_applicationLicense.LicenseID != 0)
                notesControl.LoadNotes(SCOPE.License, _applicationLicense.LicenseID);
        }

        #endregion Notes Tab Functions

        #region Documents Tab Functions

        protected void InitializeDocumentsTab()
        {
            if (_applicationLicense.LicenseID != 0)
                documentsControl.LoadDocuments(SCOPE.License, _applicationLicense.LicenseID);
        }

        #endregion Documents Tab Functions
    }
}