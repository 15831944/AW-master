using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using Infragistics.Win;
using Infragistics.Win.UltraWinGrid;
using Layton.AuditWizard.Common;
using Layton.AuditWizard.DataAccess;
using Layton.Cab.Interface;
using Microsoft.Practices.CompositeUI;
using Microsoft.Practices.CompositeUI.SmartParts;
using Microsoft.Practices.ObjectBuilder;

namespace Layton.AuditWizard.Administration
{
    [SmartPart]
    public partial class SuppliersTabView : UserControl, ILaytonView, IAdministrationView
    {
        private LaytonWorkItem workItem;
        SuppliersTabViewPresenter presenter;

        [InjectionConstructor]
        public SuppliersTabView([ServiceDependency] WorkItem workItem)
        {
            this.workItem = workItem as LaytonWorkItem;
            InitializeComponent();

            // Center the header label
            CenterHeaderLabel();
        }

        [CreateNew]
        public SuppliersTabViewPresenter Presenter
        {
            set { presenter = value; presenter.View = this; presenter.Initialize(); }
            get { return presenter; }
        }

        public void RefreshViewSinglePublisher()
        {
        }

        /// <summary>
        /// Refresh the current view
        /// </summary>
        public void RefreshView()
        {
            base.Refresh();
            RefreshTab();
        }


        /// <summary>
        /// Called as this tab is activated to ensure that we display the latest possible data
        /// This function comes from the IAdministrationView Interface
        /// </summary>
        public void Activate()
        {
            RefreshTab();
        }

        /// <summary>
        /// save function for the IAdministrationView Interface
        /// </summary>
        public void Save()
        {
        }


        public LaytonWorkItem WorkItem
        {
            get { return workItem; }
        }



        /// <summary>
        /// Extra Initialization of the grid
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ultraGridSuppliers_InitializeLayout(object sender, InitializeLayoutEventArgs e)
        {
            // Other miscellaneous settings
            // --------------------------------------------------------------------------------
            // Set the scroll style to immediate so the rows get scrolled immediately
            // when the vertical scrollbar thumb is dragged.
            //
            e.Layout.ScrollStyle = ScrollStyle.Immediate;

            // ScrollBounds of ScrollToFill will prevent the user from scrolling the
            // grid further down once the last row becomes fully visible.
            //
            e.Layout.ScrollBounds = ScrollBounds.ScrollToFill;

            //-----------------------------------------------------------------------------------
            //
            //    General Settings
            e.Layout.Override.CardAreaAppearance.BackColor = Color.Transparent;
            e.Layout.Override.CardCaptionAppearance.BackColorAlpha = Infragistics.Win.Alpha.UseAlphaLevel;
            e.Layout.Override.CardCaptionAppearance.AlphaLevel = 192;
            e.Layout.Override.RowAppearance.BackColorAlpha = Infragistics.Win.Alpha.Transparent;
            e.Layout.Override.CellAppearance.BackColorAlpha = Infragistics.Win.Alpha.UseAlphaLevel;
            e.Layout.Override.CellAppearance.AlphaLevel = 192;
            e.Layout.Override.HeaderAppearance.AlphaLevel = 192;
            e.Layout.Override.HeaderAppearance.BackColorAlpha = Infragistics.Win.Alpha.UseAlphaLevel;
            e.Layout.Override.CellClickAction = CellClickAction.RowSelect;
        }


        /// <summary>
        /// Called to refresh the information displayed on the users tab
        /// </summary>
        protected void RefreshTab()
        {
            // First clear the existing data held in the DataSource
            this.suppliersDataSet.Clear();

            // ...read the users defined in the database
            SuppliersDAO lwDataAccess = new SuppliersDAO();
            DataTable supplierTable = lwDataAccess.EnumerateSuppliers();

            // ...add these to the DataSource for the Users explorer bar
            foreach (DataRow thisRow in supplierTable.Rows)
            {
                // Create the supplier object
                Supplier thisSupplier = new Supplier(thisRow);

                // Skip the default supplier
                if (thisSupplier.SupplierID == 1)
                    continue;

                // otherwise add to our data set
                UpdateDataSet(thisSupplier);
            }
        }


        #region Grid Control Handlers

        /// <summary>
        /// Called aw we attempt to delete one or more users from the database
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ultraGridSuppliers_BeforeRowsDeleted(object sender, Infragistics.Win.UltraWinGrid.BeforeRowsDeletedEventArgs e)
        {
            // Get the Supplier being updated
            UltraGridRow updatedRow = e.Rows[0];
            Supplier thisSupplier = (Supplier)updatedRow.Cells[0].Value;			// Column 0 is the Supplier Object
            DeleteSupplier(thisSupplier);
        }


        /// <summary>
        /// Called after we update a cell in the grid - we need to ensure that the underlying data source
        /// is also updated to reflect the change
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ultraGridSuppliers_AfterCellUpdate(object sender, CellEventArgs e)
        {
            // Get the Supplier being updated
            UltraGridRow updatedRow = e.Cell.Row;
            Supplier thisSupplier = (Supplier)updatedRow.Cells[0].Value;			// Column 0 is the Supplier Object
            UpdateSupplier(thisSupplier, updatedRow);
        }


        /// <summary>
        /// Called to edit the selected Supplier
        /// </summary>
        private void EditSelectedSupplier()
        {
            // Ensure we have one and one only row selected
            if (this.ultraGridSuppliers.Selected.Rows.Count != 1)
                return;
            UltraGridRow selectedRow = this.ultraGridSuppliers.Selected.Rows[0];
            if (selectedRow.Cells[0].Value == null)
                return;
            //
            Supplier supplier = (Supplier)selectedRow.Cells[0].Value;			// Column 0 is the Object
            FormSupplierProperties form = new FormSupplierProperties(supplier);
            if (form.ShowDialog() == DialogResult.OK)
                RefreshView();
        }


        /// <summary>
        /// Called as we double click a row to edit the current supplier
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ultraGridSuppliers_DoubleClickRow(object sender, DoubleClickRowEventArgs e)
        {
            EditSelectedSupplier();
        }



        /// <summary>
        /// Handle the re-sizing of the header box
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void headerGroupBox_SizeChanged(object sender, EventArgs e)
        {
            CenterHeaderLabel();
        }

        private void CenterHeaderLabel()
        {
            //int centerX = headerGroupBox.Width / 2;
            //int headerCenterX = headerLabel.Width / 2;
            //headerLabel.Left = centerX - headerCenterX;
        }

        #endregion Grid Control Handlers

        #region Context Menu Handlers

        /// <summary>
        /// Invoke the 'Add New Supplier' form
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void newSupplierToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AddSupplier();
        }


        /// <summary>
        /// Called to delete the currently selected user
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void deleteSupplierToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DeleteSupplier();
        }

        private void editSupplierToolStripMenuItem_Click(object sender, EventArgs e)
        {
            EditSelectedSupplier();
        }

        #endregion Context Menu Handlers

        #region Supplier Update Functions

        /// <summary>
        /// Called to add a new supplier
        /// </summary>
        public void AddSupplier()
        {
            Supplier newSupplier = new Supplier();
            newSupplier.Name = "<New Supplier>";
            FormSupplierProperties form = new FormSupplierProperties(newSupplier);
            if (form.ShowDialog() == DialogResult.OK)
            {
                UpdateDataSet(newSupplier);
                ultraGridSuppliers.Refresh();
            }
        }



        /// <summary>
        /// Edit the selected supplier.
        /// </summary>
        public void EditSupplier()
        {
            // Ensure we have one and one only row selected
            if (this.ultraGridSuppliers.Selected.Rows.Count != 1)
                return;

            // Get the currently selected supplier in the grid
            UltraGridRow selectedRow = this.ultraGridSuppliers.Selected.Rows[0];
            if (selectedRow.Cells[0].Value == null)
                return;
            //
            Supplier thisSupplier = (Supplier)selectedRow.Cells[0].Value;			// Column 0 is the Supplier Object
            FormSupplierProperties form = new FormSupplierProperties(thisSupplier);
            if (form.ShowDialog() == DialogResult.OK)
                UpdateDataSet(thisSupplier);
        }


        /// <summary>
        /// Update the supplied supplier definition with the current values
        /// </summary>
        /// <param name="theSupplier"></param>
        private void UpdateSupplier(Supplier theSupplier, UltraGridRow updatedRow)
        {
            theSupplier.AddressLine1 = GetCellValueAsString(updatedRow.Cells["addressline1"]);
            theSupplier.AddressLine2 = GetCellValueAsString(updatedRow.Cells["addressline2"]);
            theSupplier.City = GetCellValueAsString(updatedRow.Cells["city"]);
            theSupplier.State = GetCellValueAsString(updatedRow.Cells["state"]);
            theSupplier.Zip = GetCellValueAsString(updatedRow.Cells["zip"]);
            theSupplier.Telephone = GetCellValueAsString(updatedRow.Cells["telephone"]);
            theSupplier.Contact = GetCellValueAsString(updatedRow.Cells["contactname"]);
            theSupplier.Email = GetCellValueAsString(updatedRow.Cells["email"]);
            theSupplier.Fax = GetCellValueAsString(updatedRow.Cells["fax"]);

            // Update the DataSet with the changes to this supplier
            UpdateDataSet(theSupplier);
        }


        private string GetCellValueAsString(UltraGridCell dataCell)
        {
            return (dataCell.Value == null) ? "" : (string)dataCell.Value;
        }


        /// <summary>
        /// Delete all selected suppliers from the database subject to confirmation.
        /// </summary>
        public void DeleteSupplier()
        {
            // Ensure we have one and one only row selected
            if (this.ultraGridSuppliers.Selected.Rows.Count != 1)
                return;

            // Get the currently selected user in the grid
            UltraGridRow selectedRow = this.ultraGridSuppliers.Selected.Rows[0];
            if (selectedRow.Cells[0].Value == null)
                return;
            //
            Supplier thisSupplier = (Supplier)selectedRow.Cells[0].Value;			// Column 0 is the Supplier Object
            DeleteSupplier(thisSupplier);
        }


        /// <summary>
        /// Private function to actually delete a supplier from the database
        /// </summary>
        /// <param name="theSupplier"></param>
        private void DeleteSupplier(Supplier theSupplier)
        {
            // Delete this Supplier after confirmation
            if (MessageBox.Show("Are you certain that you want to delete the supplier '" + theSupplier.Name + "'?", "Confirm Delete", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                // Delete from the database
                theSupplier.Delete();

                // ...and from the DataSet
                DataRow supplierRow = SupplierFind(theSupplier);
                if (supplierRow != null)
                    suppliersDataSet.Tables[0].Rows.Remove(supplierRow);
            }
        }

        #endregion Supplier Update Functions

        #region DataSet Functions

        /// <summary>
        /// Update the suppliers data set either adding a new entry to the DataSet or updating an existing entry
        /// </summary>
        /// <param name="thisSupplier"></param>
        protected void UpdateDataSet(Supplier thisSupplier)
        {
            // First check the data set to see if an entry for this supplier already exists as if so we
            // shall update the existing entry rather than adding a new one.
            DataRow foundRow = SupplierFind(thisSupplier);
            if (foundRow != null)
            {
                // Existing supplier so just update this row
                foundRow["SupplierObject"] = thisSupplier;
                foundRow["addressline1"] = thisSupplier.AddressLine1;
                foundRow["addressline2"] = thisSupplier.AddressLine2;
                foundRow["city"] = thisSupplier.City;
                foundRow["state"] = thisSupplier.State;
                foundRow["zip"] = thisSupplier.Zip;
                foundRow["telephone"] = thisSupplier.Telephone;
                foundRow["contactname"] = thisSupplier.Contact;
                foundRow["email"] = thisSupplier.Email;
                foundRow["fax"] = thisSupplier.Fax;
            }

            else
            {
                // If we get here then no exiting entry in the DataSet so add a new one
                suppliersDataSet.Tables[0].Rows.Add(new object[] 
					{ thisSupplier
						,thisSupplier.AddressLine1
						,thisSupplier.AddressLine2
						,thisSupplier.City
						,thisSupplier.State
						,thisSupplier.Zip
						,thisSupplier.Contact
						,thisSupplier.Telephone
						,thisSupplier.Email 
						,thisSupplier.Name
						,thisSupplier.Fax
					});
            }
        }

        /// <summary>
        /// Find an existing supplier record in the DataSet and return it
        /// </summary>
        /// <param name="theSupplier"></param>
        /// <returns></returns>
        private DataRow SupplierFind(Supplier theSupplier)
        {
            foreach (DataRow row in suppliersDataSet.Tables[0].Rows)
            {
                Supplier supplier = (Supplier)row[0];
                if (supplier.SupplierID == theSupplier.SupplierID)
                    return row;
            }
            return null;
        }

        #endregion DataSet Functions

        private void ultraGridSuppliers_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Right) return;

            UIElement element = ultraGridSuppliers.DisplayLayout.UIElement.ElementFromPoint(new Point(e.X, e.Y));
            if (element == null) return;

            UltraGridCell cell = (UltraGridCell)element.GetContext(typeof(UltraGridCell));

            if (cell != null)
            {
                ultraGridSuppliers.Selected.Rows.Clear();
                cell.Row.Activate();
                cell.Row.Selected = true;
            }
        }

        private void bnNewSupplier_Click(object sender, EventArgs e)
        {
            AddSupplier();
        }

        private void bnDeleteSupplier_Click(object sender, EventArgs e)
        {
            DeleteSupplier();
        }

        private void bnEditSupplier_Click(object sender, EventArgs e)
        {
            EditSupplier();
        }

        private void ultraGridSuppliers_AfterSelectChange(object sender, AfterSelectChangeEventArgs e)
        {
            bnEditSupplier.Enabled = (ultraGridSuppliers.ActiveRow != null);
        }
    }
}
