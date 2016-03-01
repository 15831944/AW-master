using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Threading;
using System.Windows.Forms;
using Infragistics.Win;
using Infragistics.Win.UltraWinEditors;
using Infragistics.Win.UltraWinMaskedEdit;
using Infragistics.Win.UltraWinTabControl;
using Layton.AuditWizard.Administration;
using Layton.AuditWizard.DataAccess;
using Layton.Cab.Interface;
using Layton.Common.Controls;
using Microsoft.Practices.CompositeUI;
using Microsoft.Practices.CompositeUI.SmartParts;
using Microsoft.Practices.ObjectBuilder;
using Infragistics.Win.UltraWinTree;

namespace Layton.AuditWizard.Network
{
    [SmartPart]
    public partial class SummaryTabView : UserControl, ILaytonView
    {
        private SummaryTabViewPresenter _presenter;
        private readonly LaytonWorkItem workItem;
        private Asset _asset;
        UltraTree tree;
        private bool bApply;
        SupportContract m_objSupportContract;

        public UltraTree Tree
        {           
            
            get { return tree; }
            set { tree = value; }
        }
        public Asset SelectedAsset
        {
            get { return _asset; }
            set { _asset = value; }
        }        
        private readonly AssetTypeList _assetTypes = new AssetTypeList();
        private readonly UserDataCategoryList _listCategories = new UserDataCategoryList(UserDataCategory.SCOPE.Asset);
        private static int labelWidth = 100;
        private static int textWidth = 450;
        private readonly PickListList _listPickLists = new PickListList();
        private readonly AssetTypeList _listAssetTypes = new AssetTypeList();

        // Constructor
        [InjectionConstructor]
        public SummaryTabView([ServiceDependency] WorkItem workItem)
        {
            this.workItem = workItem as LaytonWorkItem;
            InitializeComponent();
            InitializeUserDefinedCategoryTabs();
            _listAssetTypes.Populate();
            tree = new UltraTree();
            bApply = false;
            m_objSupportContract = new SupportContract();

            //LoadForm();
        }
        public SummaryTabView()
        {
        }

        [CreateNew]
        public SummaryTabViewPresenter Presenter
        {
            set
            {
                _presenter = value;
                _presenter.View = this;
                _presenter.Initialize();
            }
            get
            {
                return _presenter;
            }
        }

        public void RefreshView()
        {
            _presenter.Initialize();
            InitializeUserDefinedCategoryTabs();
            FillUserDefinedData();
            base.Refresh();
        }

        public LaytonWorkItem WorkItem
        {
            get { return workItem; }
        }

        public void Display(Asset theAsset)        
		{           
            _asset = theAsset;

            lbAssetNameText.Text = theAsset.Name;
            lblCategory.Text = theAsset.TypeAsString;
            lblMake.Text = theAsset.Make;
            lblModel.Text = theAsset.Model;
            lblAssetTag.Text = theAsset.AssetTag;
            lbSerialNumber.Text = theAsset.SerialNumber;
            //
            lblIPAddress.Text = theAsset.IPAddress;
            lblMacAddress.Text = theAsset.MACAddress;
            lblDomain.Text = theAsset.Domain;

            lblDateOfLastAudit.Text = String.Empty;
            lblScannerType.Text = String.Empty;
            lblName.Text = String.Empty;
            lblExtendedName.Text = String.Empty;
            lblProductID.Text = String.Empty;
            lblCDKey.Text = String.Empty;

            if (theAsset.LastAudit != DateTime.MinValue)
            {

                lblScannerType.Text = theAsset.AgentVersion;
                lblDateOfLastAudit.Text = theAsset.LastAuditDateString;
            }

            // Is this an auditable device?
            if (theAsset.Auditable)
            {
                // But OS information we need to read from the database
                ApplicationsDAO lwDataAccess = new ApplicationsDAO();
                DataTable osTable = lwDataAccess.GetInstalledOS(theAsset);

                gbOS.Visible = true;

                // If we have an OS then list it
                if (osTable.Rows.Count > 0)
                {
                    OSInstance ourOS = new OSInstance(osTable.Rows[0]);
                    lblName.Text = ourOS.Name;
                    lblExtendedName.Text = ourOS.Name + " " + ourOS.Version;
                    lblProductID.Text = ourOS.Serial.ProductId;
                    lblCDKey.Text = ourOS.Serial.CdKey;

					// 8.3.4 - CMD
					// We may also be able to display the system type (32 or 64 bit OS) however this would be held in the Processor Hardware data 
					// if it is available.
					AuditedItem systemTypeItem = theAsset.AuditedItems.FindItemByName("Hardware|CPU" ,"System Type");
					if (systemTypeItem != null)
					{
						lblSystemType.Text = systemTypeItem.Value;
					}
					else
					{
						lblSystemType.Text = "";
					}
                }
                else
                {
                    lblName.Text = String.Empty;
                    lblExtendedName.Text = String.Empty;
                    lblProductID.Text = String.Empty;
                    lblCDKey.Text = String.Empty;
					lblSystemType.Text = "";
				}
            }
            else
            {
                gbOS.Visible = false;
            }

            LoadForm();
        }

        private void LoadForm()
        {
            tbParentLocation.Text = _asset.Location;
            tbAssetName.Text = _asset.Name;
            tbSerialNumber.Text = _asset.SerialNumber;
            tbAssetTag.Text = _asset.AssetTag;
            cbOverwriteData.Checked = _asset.OverwriteData;

            _assetTypes.Populate();

            // Recover the category of the current asset so that we can select it correctly in the combo
            AssetType assetType = _assetTypes.FindByName(_asset.TypeAsString);
            if (assetType == null)
                assetType = _assetTypes[0];

            AssetType assetCategory = _assetTypes.GetParent(assetType);

            // We now need to add the Asset Type categories to the combo box
            cbAssetCategories.BeginUpdate();

            cbAssetCategories.Items.Clear();

            AssetTypeList categories = _assetTypes.EnumerateCategories();
            foreach (AssetType category in categories)
            {
                cbAssetCategories.Items.Add(category);
            }

            int index = cbAssetCategories.Items.IndexOf(assetCategory);
            cbAssetCategories.SelectedIndex = (index != -1) ? index : 0;

            cbAssetCategories.EndUpdate();

            FillAssetTypes(assetCategory);
            FillStockStatuses(_asset.StockStatus);

            // ...and select the asset type for the asset
            index = cbAssetTypes.Items.IndexOf(assetType);
            cbAssetTypes.SelectedIndex = (index != -1) ? index : 0;

            FillAssetMakes(assetType.AssetTypeID);
            FillAssetModels(assetType.AssetTypeID);

            InitializeNotesTab();
            PopulateSuppliers();
            InitializeDocumentsTab();

            FillUserDefinedData();
            //Added by Sojan E John KTS Infotech
            ClearControlsSupportContract();
            FillControlsOnLoad();
            FillSupportContractComboBox();
            FillSuppliersComboBox();
            InitialiseSupportContractTab();
            
        }

        private void HideNotApplicableTabs()
        {
            foreach (UltraTab tab in dataTabControl.Tabs)
            {
                tab.Visible = true;

                UserDataCategory category = (UserDataCategory) tab.Tag;

                if (category == null)
                    continue;

                if (!CategoryAppliesTo(category, _asset))
                    tab.Visible = false;
            }
        }

        public bool CategoryAppliesTo(UserDataCategory category, Asset asset)
        {
            // Is this category specific to an asset type?
            if (category.AppliesTo != 0)
            {
                // OK - does the category apply specifically to this asset type?
                if (category.AppliesTo != asset.AssetTypeID)
                {
                    // No - we need to get the parent category of this asset type and check that also then
                    AssetType parentType = _listAssetTypes.FindByName(asset.TypeAsString);
                    if ((parentType == null) || (category.AppliesTo != parentType.ParentID))
                        return false;
                }

            }

            // User data category applies to this type of asset so return true
            return true;
        }

        private void FillUserDefinedData()
        {
            foreach (UserDataCategory category in _listCategories)
            {
                foreach (UserDataField userDataField in category)
                {
                    switch ((int)userDataField.Type)
                    {
                        case (int)UserDataField.FieldType.Number:

                            NumericUpDown numericUpDown = (NumericUpDown)userDataField.Tag;
                            try
                            {
                                numericUpDown.Value = Convert.ToDecimal(userDataField.GetValueFor(_asset.AssetID));
                            }
                            catch (FormatException)
                            {
                                numericUpDown.Value = 0;
                            }

                            break;

                        case (int)UserDataField.FieldType.Picklist:
                            ComboBox combo = (ComboBox)userDataField.Tag;
                            int index = combo.FindStringExact(userDataField.GetValueFor(_asset.AssetID));
                            combo.SelectedIndex = (index != -1) ? index : -1;
                            break;

                        case (int)UserDataField.FieldType.Date:

                            NullableDateTimePicker dateTimePicker = (NullableDateTimePicker)userDataField.Tag;
                            try
                            {
                                DateTime value = Convert.ToDateTime(userDataField.GetValueFor(_asset.AssetID));
                                dateTimePicker.Value = value;
                                dateTimePicker.CustomFormat = "yyyy-MM-dd";
                            }
                            catch (FormatException)
                            {
                                dateTimePicker.Value = null;
                            }

                            break;

                        case (int)UserDataField.FieldType.Currency:

                            UltraCurrencyEditor currencyEditor = (UltraCurrencyEditor)userDataField.Tag;
                            try
                            {
                                currencyEditor.Value = Convert.ToDecimal(userDataField.GetValueFor(_asset.AssetID));
                            }
                            catch (FormatException)
                            {
                                currencyEditor.Value = 0;
                            }

                            break;

                        default:
                            TextBox textBox = (TextBox)userDataField.Tag;
                            textBox.Text = userDataField.GetValueFor(_asset.AssetID);
                            break;
                    }
                }
            }

            HideNotApplicableTabs();
        }

        private void InitializeUserDefinedCategoryTabs()
        {
            _listCategories.Populate();
            _listPickLists.Populate();

            dataTabControl.SuspendLayout();

            List<UltraTab> tabsToRemove = new List<UltraTab>();

            // remove existing UDD tabs (if any)
            foreach (UltraTab tab in dataTabControl.Tabs)
            {
                if (tab.Text != "Summary" && tab.Text != "Basic Information" && tab.Text != "Supplier" && tab.Text != "Support Contract" && tab.Text != "Notes" && tab.Text != "Documents")
                    tabsToRemove.Add(tab);
            }

            foreach (UltraTab tab in tabsToRemove)
            {
                dataTabControl.Tabs.Remove(tab);
            }

            // Create a tab for each category which is pertinent for this asset
            foreach (UserDataCategory category in _listCategories)
            {
                //category.GetValuesFor(_asset.AssetID);
                AddCategory(category);
            }

            // Resume layout
            dataTabControl.ResumeLayout();
        }

        /// <summary>
        /// Add a new tab to the tab control for the specified category and add controls to the tab
        /// for the fields within the category
        /// </summary>
        /// <param name="category"></param>
        protected void AddCategory(UserDataCategory category)
        {
            UltraTab selectedTab = dataTabControl.SelectedTab;

            // Create the objects required to add a tab
            UltraTab newTab = new UltraTab();
            newTab.Tag = category;
            newTab.Key = "UDD:" + category.Name;
            newTab.Text = category.Name;
            newTab.ToolTipText = "Display User Defined Data for " + category.Name;
            newTab.Appearance.Image = IconMapping.LoadIcon(category.Icon, IconMapping.Iconsize.Small);

            // Add the new tab to the control
            dataTabControl.Tabs.Add(newTab);



            if (selectedTab != null)
            {
                foreach (UltraTab tab in dataTabControl.Tabs)
                {
                    if (tab.Key != selectedTab.Key) continue;

                    dataTabControl.SelectedTab = tab;
                    break;
                }
            }

            // Create a Table Layout Panel which will fit on the tab
            TableLayoutPanel tableLayoutPanel = new TableLayoutPanel();
            tableLayoutPanel.Anchor = ((AnchorStyles.Top | AnchorStyles.Bottom) | AnchorStyles.Left) | AnchorStyles.Right;
            tableLayoutPanel.Location = new Point(10, 10);
            tableLayoutPanel.Padding = new Padding(10);
            tableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 30F));
            tableLayoutPanel.TabIndex = 0;
            tableLayoutPanel.RowCount = 12;
            tableLayoutPanel.ColumnCount = 2;
            tableLayoutPanel.Dock = DockStyle.Fill;

            GroupBox gb = new GroupBox
                           {
                               Text = category.Name,
                               Location = new Point(12, 20),
                               Size = new Size(800, 400),
                               Padding = new Padding(15, 15, 15, 15)
                           };

            gb.Controls.Add(tableLayoutPanel);
            newTab.TabPage.Controls.Add(gb);

            Size containerSize = newTab.TabPage.Size;
            Size tableSize = new Size(containerSize.Width - 20, containerSize.Height - 20);
            tableLayoutPanel.Size = tableSize;
            tableLayoutPanel.AutoScroll = true;

            // Get the list of fields which form this category 
            List<Control> listControls = new List<Control>();
            foreach (UserDataField dataField in category)
            {
                Label label;
                Control control;

                switch ((int)dataField.Type)
                {
                    case (int)UserDataField.FieldType.Number:
                        CreateNumberField(dataField, out label, out control);
                        break;

                    case (int)UserDataField.FieldType.Picklist:
                        CreatePicklistField(dataField, out label, out control);
                        break;

                    case (int)UserDataField.FieldType.Date:
                        CreateDateField(dataField, out label, out control);
                        break;

                    case (int)UserDataField.FieldType.Currency:
                        CreateCurrencyField(dataField, out label, out control);
                        break;

                    case (int)UserDataField.FieldType.Registry:
                        CreateEnvironmentOrRegistryField(dataField, out label, out control);
                        break;

                    case (int)UserDataField.FieldType.Environment:
                        CreateEnvironmentOrRegistryField(dataField, out label, out control);
                        break;

                    default:
                        CreateTextField(dataField, out label, out control);
                        break;
                }

                // Add the controls to the panel
                listControls.Add(label);
                control.Tag = dataField;
                listControls.Add(control);

                // Ensure that the user data field knows about its display control
                dataField.Tag = control;
            }

            // Suspend Layout of the table as we add the rows
            tableLayoutPanel.SuspendLayout();

            // Now add the controls as pairs to the table
            int row = 0;
            int column = 0;
            for (int isub = 0; isub < listControls.Count; isub += 2)
            {
                // Get the controls
                Label label = (Label)listControls[isub];
                Control control = listControls[isub + 1];

                // Ensure that the controls will align at the top, left of their cell
                label.Anchor = AnchorStyles.Left | AnchorStyles.None;
                label.Location = new Point(0, 0);
                label.TextAlign = ContentAlignment.MiddleCenter;
                label.Padding = new Padding(5);
                control.Anchor = AnchorStyles.Left | AnchorStyles.None;
                control.Padding = new Padding(5);
                control.Location = new Point(0, 0);

                // Are we at the end of the rows?  If so reset to row 0 but now start at column 3
                if (row >= tableLayoutPanel.RowCount)
                {
                    row = 0;
                    column = 3;
                }

                // Add the data to the row
                tableLayoutPanel.Controls.Add(label, column, row);
                tableLayoutPanel.Controls.Add(control, column + 1, row);

                // next row
                row++;
            }

            // Resume and perform the layout
            tableLayoutPanel.ResumeLayout();
            tableLayoutPanel.PerformLayout();
        }

        /// <summary>
        /// Create a label and input control for a Boolean field
        /// </summary>
        protected void CreateBooleanField(UserDataField dataField, out Label label, out Control control)
        {
            // Create the label;
            label = new Label { Text = dataField.Name, Size = new Size(labelWidth, 13) };

            // Create a combobox with True and False values
            ComboBox combo = new ComboBox();
            control = combo;
            combo.DropDownStyle = ComboBoxStyle.DropDownList;
            combo.Items.Add("True");
            combo.Items.Add("False");
        }


        /// <summary>
        /// Create a label and input control for a Date field
        /// </summary>
        protected void CreateDateField(UserDataField dataField, out Label label, out Control control)
        {
            // Create the label;
            label = new Label { Text = dataField.Name, AutoSize = true, Size = new Size(labelWidth, 13) };

            // Create a Date/Time Picker
            NullableDateTimePicker nullableDateTime = new NullableDateTimePicker();

            nullableDateTime.Value = null;
            nullableDateTime.Format = DateTimePickerFormat.Custom;
            nullableDateTime.CustomFormat = "yyyy-MM-dd";
            nullableDateTime.Width = 110;

            control = nullableDateTime;
        }

        /// <summary>
        /// Create a label and input control for a Currency field
        /// </summary>
        protected void CreateCurrencyField(UserDataField dataField, out Label label, out Control control)
        {
            // Create the label;
            label = new Label { Text = dataField.Name, AutoSize = true, Size = new Size(labelWidth, 13) };

            // Create a Currency control
            UltraCurrencyEditor currencyEditor = new UltraCurrencyEditor();
            currencyEditor.Value = 0;
            currencyEditor.MaskClipMode = MaskMode.Raw;
            currencyEditor.PromptChar = ' ';
            currencyEditor.Width = 110;
            control = currencyEditor;
        }


        /// <summary>
        /// Create a label and input control for a Numeric field
        /// </summary>
        protected void CreateNumberField(UserDataField dataField, out Label label, out Control control)
        {
            // Create the label;
            label = new Label { Text = dataField.Name, AutoSize = true, Size = new Size(labelWidth, 13) };

            NumericUpDown nup = new NumericUpDown { TextAlign = HorizontalAlignment.Left, Width = 110 };
            nup.Maximum = Decimal.MaxValue;
            nup.Minimum = Decimal.MinValue;
            control = nup;
        }


        /// <summary>
        /// Create a label and input control for a Boolean field
        /// </summary>
        protected void CreatePicklistField(UserDataField dataField, out Label label, out Control control)
        {
            // Do we already have the picklists read?  If not then do so now
            _listPickLists.Populate();

            // Create the label;
            label = new Label { Text = dataField.Name, AutoSize = true, Size = new Size(labelWidth, 13) };

            // Create a combobox with values taken from the picklist
            ComboBox combo = new ComboBox();
            control = combo;
            combo.DropDownStyle = ComboBoxStyle.DropDownList;

            // Get the picklist which is in use and add the pickitems to the combo
            PickList picklist = _listPickLists.FindPickList(dataField.Picklist);
            if (picklist != null)
            {
                int maxWidth = 0;
                foreach (PickItem pickitem in picklist)
                {
                    if ((pickitem.Name.Length * 7) > maxWidth)
                    {
                        maxWidth = pickitem.Name.Length * 7;
                    }
                    combo.Items.Add(pickitem);
                }

                combo.Width = maxWidth + 25;
            }
        }


        /// <summary>
        /// Create a label and input control for a textual field
        /// </summary>
        protected void CreateTextField(UserDataField dataField, out Label label, out Control control)
        {
            // Create the label;
            label = new Label();
            label.Text = dataField.Name;
            label.AutoSize = true;
            label.Size = new Size(labelWidth, 13);

            // Create a textbox and set any requirements
            TextBox textbox = new TextBox();
            control = textbox;
            textbox.Size = new Size(textWidth, 20);
            textbox.Tag = dataField.FieldID;

            switch (dataField.InputCase)
            {
                case "any":
                    break;

                case "lower":
                    textbox.CharacterCasing = CharacterCasing.Lower;
                    break;

                case "upper":
                    textbox.CharacterCasing = CharacterCasing.Upper;
                    break;

                case "title":
                    textbox.LostFocus += textBox_TextChanged;
                    break;
            }
        }

        /// <summary>
        /// Create a label and input control for a textual field
        /// </summary>
        protected void CreateEnvironmentOrRegistryField(UserDataField dataField, out Label label, out Control control)
        {
            // Create the label;
            label = new Label { Text = dataField.Name, AutoSize = true, Size = new Size(labelWidth, 13) };

            // Create a textbox and set any requirements
            TextBox textbox = new TextBox();
            control = textbox;
            textbox.Size = new Size(textWidth, 20);
            textbox.Tag = dataField.FieldID;
            textbox.Enabled = false;
        }

        /// <summary>
        /// Called when the text is changed in a Text Box which has Title Case Enforced
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void textBox_TextChanged(object sender, EventArgs e)
        {
            // Get the textbox 
            TextBox textbox = (TextBox)sender;

            // Disable the textchanged event while we play with the text to prevent recursion
            textbox.LostFocus -= textBox_TextChanged;

            // Get the text and ensure that it is title case
            string text = textbox.Text;
            CultureInfo cultureInfo = Thread.CurrentThread.CurrentCulture;
            TextInfo textInfo = cultureInfo.TextInfo;
            text = textInfo.ToTitleCase(text);
            textbox.Text = text;

            // Re-establish the TextChanged event
            textbox.LostFocus += new EventHandler(textBox_TextChanged);
        }

        private void PopulateSuppliers()
        {
            Supplier lSupplier = new Supplier(new SuppliersDAO().SelectSupplierByID(_asset.SupplierID).Rows[0]);

            lbSupplierNameText.Text = lSupplier.Name;

            lbAddress1.Text = lSupplier.AddressLine1;
            lbAddress2.Text = lSupplier.AddressLine2;
            lbCity.Text = lSupplier.City;
            lbRegion.Text = lSupplier.State;
            lbPostalCode.Text = lSupplier.Zip;
            lbTelephone.Text = lSupplier.Telephone;
            lbContactName.Text = lSupplier.Contact;
            lbContactEmail.Text = lSupplier.Email;
            lbSupplierWWW.Text = lSupplier.WWW;
            lbFax.Text = lSupplier.Fax;
        }

        #region Basic Properties Tab Handlers

        private void BasicPropertiesTab_Leave(object sender, EventArgs e)
        {
            SaveBasicProperties();

            lblCategory.Text = _asset.TypeAsString;
            lblMake.Text = _asset.Make;
            lblModel.Text = _asset.Model;
            lbSerialNumber.Text = _asset.SerialNumber;
            lblAssetTag.Text = _asset.AssetTag;
        }

        private void SaveUserDefinedData()
        {
            foreach (UserDataCategory category in _listCategories)
            {
                if (category.CategoryAppliesTo(_asset))
                {
                    foreach (UserDataField dataField in category)
                    {
                        UpdateUserDataFieldValue(dataField);
                    }
                }
            }
        }

        /// <summary>
        /// Update a specific user data field value 
        /// </summary>
        private void UpdateUserDataFieldValue(UserDataField dataField)
        {
            string newValue = "";
            Control control = dataField.Tag as Control;

            // The data value depends on the field type
            switch ((int)dataField.Type)
            {
                case (int)UserDataField.FieldType.Number:
                    NumericUpDown nup = (NumericUpDown)control;
                    if (nup != null) newValue = nup.Value.ToString();
                    break;

                case (int)UserDataField.FieldType.Picklist:
                    ComboBox comboPicklist = (ComboBox)control;
                    if (comboPicklist != null) newValue = (comboPicklist.SelectedIndex == -1) ? "" : comboPicklist.Text;
                    break;

                case (int)UserDataField.FieldType.Text:
                    TextBox textbox = (TextBox)control;
                    if (textbox != null) newValue = textbox.Text;
                    break;

                case (int)UserDataField.FieldType.Environment:
                    TextBox textbox1 = (TextBox)control;
                    if (textbox1 != null) newValue = textbox1.Text;
                    break;

                case (int)UserDataField.FieldType.Registry:
                    TextBox textbox2 = (TextBox)control;
                    if (textbox2 != null) newValue = textbox2.Text;
                    break;

                case (int)UserDataField.FieldType.Date:
                    NullableDateTimePicker dateTimePicker = (NullableDateTimePicker)control;
                    if (dateTimePicker != null) newValue = dateTimePicker.Text;
                    break;

                case (int)UserDataField.FieldType.Currency:
                    UltraCurrencyEditor currencyEditor = (UltraCurrencyEditor)control;
                    if (currencyEditor != null) newValue = currencyEditor.Value.ToString();
                    break;

                default:
                    newValue = "";
                    break;
            }

            // Has the value changed from that currently stored?
            if (newValue != dataField.GetValueFor(_asset.AssetID))
                dataField.SetValueFor(_asset.AssetID, newValue, true);
        }

        private void SaveBasicProperties()
        {
            Asset oldAsset = new Asset(_asset);

            // Recove the asset type - note that this is what-ever is selected in the type and we
            // ignore the asset category as this is simply a way of getting to the asset type
            if (cbAssetTypes != null)
            {
                if (cbAssetTypes.SelectedItem != null)
                {
                    _asset.AssetTypeID = (cbAssetTypes.SelectedItem as AssetType).AssetTypeID;
                    _asset.TypeAsString = (cbAssetTypes.SelectedItem as AssetType).Name;
                }
            }

            // Make, Model, Serial Number (cbAssetTypes.SelectedItem as AssetType)
            lbAssetNameText.Text = tbAssetName.Text;
            _asset.Name = tbAssetName.Text;
            _asset.Make = cbMake.Text;
            _asset.Model = cbModel.Text;
            _asset.SerialNumber = tbSerialNumber.Text;
            _asset.AssetTag = tbAssetTag.Text;

            // Stock Status
            _asset.StockStatus = (Asset.STOCKSTATUS)cbStatus.SelectedItem.DataValue;

            // overwrite function
            _asset.OverwriteData = cbOverwriteData.Checked;

            // Update the asset
            _asset.Update(oldAsset);
        }

        /// <summary>
        /// Called as we change the selected asset category - display a list of available asset types
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// 
        private void cbAssetCategories_SelectedIndexChanged(object sender, EventArgs e)
        {
            AssetType selectedCategory = cbAssetCategories.SelectedItem as AssetType;
            FillAssetTypes(selectedCategory);

            if (cbAssetTypes.Items.Count > 0)
                cbAssetTypes.SelectedIndex = 0;
        }


        /// <summary>
        /// Populate the asset types combo box for the selected asset category
        /// </summary>
        /// <param name="category"></param>
        private void FillAssetTypes(AssetType category)
        {
            cbAssetTypes.BeginUpdate();
            cbAssetTypes.Items.Clear();
            AssetTypeList subTypes = _assetTypes.EnumerateChildren(category.AssetTypeID);
            //
            foreach (AssetType subType in subTypes)
            {
                cbAssetTypes.Items.Add(subType);
            }
            //
            cbAssetTypes.EndUpdate();
        }



        /// <summary>
        /// Populate the asset status combo box
        /// </summary>
        /// <param name="category"></param>
        private void FillStockStatuses(Asset.STOCKSTATUS currentStatus)
        {
            cbStatus.BeginUpdate();
            cbStatus.Items.Clear();

            Dictionary<Asset.STOCKSTATUS, string> stockStatuses = Asset.StockStatuses();
            foreach (KeyValuePair<Asset.STOCKSTATUS, string> kvp in stockStatuses)
            {
                ValueListItem vli = cbStatus.Items.Add(kvp.Key, kvp.Value);
                if (currentStatus == kvp.Key)
                    cbStatus.SelectedItem = vli;
            }
            cbStatus.EndUpdate();
        }

        private void FillAssetMakes(int assetTypeId)
        {
            cbMake.BeginUpdate();
            cbMake.Items.Clear();

            foreach (DataRow row in new AssetDAO().GetAssetMakesByType(assetTypeId).Rows)
            {
                cbMake.Items.Add(row[0].ToString());
            }

            if (!cbMake.Items.Contains(_asset.Make))
                cbMake.Items.Add(_asset.Make);


            cbMake.SelectedItem = _asset.Make;
            cbMake.EndUpdate();
        }

        private void FillAssetModels(int assetTypeId)
        {
            cbModel.BeginUpdate();
            cbModel.Items.Clear();

            foreach (DataRow row in new AssetDAO().GetAssetModelsByType(assetTypeId).Rows)
            {
                cbModel.Items.Add(row[0].ToString());
            }

            if (!cbModel.Items.Contains(_asset.Model))
                cbModel.Items.Add(_asset.Model);

            cbModel.SelectedItem = _asset.Model;
            cbModel.EndUpdate();
        }


        #endregion Basic Properties Tab Handlers

        #region Note Handlers

        private void InitializeNotesTab()
        {
            notesControl.LoadNotes(SCOPE.Asset, _asset.AssetID);
        }
        #endregion Note Handlers

        #region Supplier Handlers

        private void bnSelectSupplier_Click(object sender, EventArgs e)
        {
            FormSelectSupplier form = new FormSelectSupplier();
            if (form.ShowDialog() == DialogResult.OK)
            {
                if (form.SupplierName != String.Empty)
                {
                    _asset.SupplierID = new SuppliersDAO().SupplierFind(form.SupplierName);
                    new AssetDAO().AssetUpdate(_asset);
                    PopulateSuppliers();
                }
            }
        }

        //private void InitializeSuppliersTab()
        //{
        //    supplierControl.LoadSuppliers(_asset.SupplierID, _asset.SupplierName);
        //}

        #endregion Supplier Handlers

        #region Document Tab Handlers


        private void InitializeDocumentsTab()
        {
            documentsControl1.LoadDocuments(SCOPE.Asset, _asset.AssetID);
        }
        #endregion Document Tab Handlers

        private void dataTabControl_Leave(object sender, EventArgs e)
        {
            SaveUserDefinedData();
        }

        private void dataTabControl_SelectedTabChanged(object sender, SelectedTabChangedEventArgs e)
        {
            if (e.Tab != null)
            {
                lbAddUserData.Visible = e.Tab.Key.StartsWith("UDD");
                tbFocus.Focus();
            }
        }

        private void buttonOK_Click(object sender, EventArgs e)
        {

            AssetDAO objAsseDAO = new AssetDAO();
            SelectedNodesCollection objSelectednode = tree.SelectedNodes;            
            if (buttonOK.Text == "Apply" && bApply ==true)
            {
                int iCount = objSelectednode.Count;                
                for (int i = 0; i < iCount; i++)
                {
                    string strTag = objSelectednode[i].Tag.ToString();
                    if (GetSupportContractDetails(strTag))
                    {
                        objAsseDAO.AddSupportContract(m_objSupportContract);
                    }
                }
                tree.SelectedNodes.Clear();
                ClearControlsSupportContract();
                FillControlsOnLoad();
                FillSupportContractComboBox();
                FillSuppliersComboBox();
                checkBoxSupportContract.Checked = false;
                InitialiseSupportContractTab();
                MessageBox.Show("Support Contract Successfully Added for the Selected Assets.", "AuditWizard", MessageBoxButtons.OK, MessageBoxIcon.Information);
                buttonOK.Text = "OK";
                bApply = false;

            }

            else if (MessageBox.Show("Do you wish to Deploy Support Contract to other Assets?\nTo deploy, highlight required assets in Network View using Ctrl key", "AuditWizard", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                buttonOK.Text = "Apply";
                bApply = true;
                GetSupportContractDetails();
            }
            else
            {
                if (GetSupportContractDetails())
                {
                    objAsseDAO.AddSupportContract(m_objSupportContract);

                    ClearControlsSupportContract();
                    FillControlsOnLoad();
                    FillSupportContractComboBox();
                    FillSuppliersComboBox();
                    checkBoxSupportContract.Checked = false;
                    InitialiseSupportContractTab();

                }
                MessageBox.Show("Support Contract Successfully Added", "AuditWizard", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            
                                    
        }

        private void FillSuppliersComboBox()
        {
            
            comboBoxSupplier.BeginUpdate();
            comboBoxSupplier.Items.Clear();
            foreach (DataRow row in new SuppliersDAO().EnumerateSuppliers().Rows)
            {
                comboBoxSupplier.Items.Add(row[1].ToString());
            }
            if (comboBoxSupplier.Text == "")                      
            {
                comboBoxSupplier.SelectedIndex = 0;
            }            
            comboBoxSupplier.EndUpdate();

        }

        private void FillSupportContractComboBox()
        {
             
            comboBoxContractNumber.BeginUpdate();
            comboBoxContractNumber.Items.Clear();
            comboBoxContractNumber.Items.Add("< Add New >");
            foreach (DataRow row in new AssetDAO().EnumerateSupportContract().Rows)
            {
                if(!comboBoxContractNumber.Items.Contains((row[0].ToString())))
                {
                    comboBoxContractNumber.Items.Add(row[0].ToString());
                }
            }
            //string strContractNumber = new AssetDAO().GetContractNumberByAssetID(_asset.AssetID);

            //if (comboBoxContractNumber.Items.Contains(strContractNumber))
            //{
            //    comboBoxContractNumber.SelectedItem = strContractNumber;
            //}
            if(comboBoxContractNumber.Text == "")
            {
                comboBoxContractNumber.SelectedIndex = 0;
            }
            comboBoxContractNumber.EndUpdate();

        }

        private void FillControlsOnLoad()
        {
            foreach (DataRow row in new AssetDAO().GetSupportContractDetailsByID(_asset.AssetID).Rows)
                {
                    comboBoxContractNumber.Text = row[0].ToString();
                    textBoxContractValue.Text = row[1].ToString();
                    int iSupplierID = Convert.ToInt32(row[2]);
                    foreach (DataRow supplierRow in new SuppliersDAO().SelectSupplierByID(iSupplierID).Rows)
                    {
                        comboBoxSupplier.Text = supplierRow[1].ToString();
                    }
                    dateTimePickerExpiryDate.Value = Convert.ToDateTime(row[3]);
                    checkBoxAlert.Checked = Convert.ToBoolean(row[4]);
                    textBoxExpireWithin.Text = row[5].ToString();
                    checkBoxEmail.Checked = Convert.ToBoolean(row[6].ToString());
                    textBoxNotes.Text = row[7].ToString();

                    // Display the alert if the support has lareday expired - or is about to
                    TimeSpan ts = DateTime.Today - Convert.ToDateTime(row[3]);
                    this.panelSupportExpired.Visible = (ts.Days >= 0);
                    lblSupportExpired.Text = (ts.Days == 0) ? "ALERT - this support contract expires today." : (ts.Days > 0) ? "ALERT - this support contract has expired!" : "";
                
                }
            if (checkBoxAlert.Checked==false)
            {
                this.panelSupportExpired.Visible = false;
            }
        }    

        
        private void InitialiseSupportContractTab()
        {
            if (checkBoxSupportContract.Checked == false)
            {
                comboBoxContractNumber.Enabled = false;
                textBoxContractValue.Enabled = false;
                comboBoxSupplier.Enabled = false;
                textBoxExpireWithin.Enabled = false;
                textBoxNotes.Enabled = false;
                checkBoxAlert.Enabled = false;
                checkBoxEmail.Enabled = false;
                buttonOK.Enabled = false;
                buttonDelete.Enabled = false;
                buttonCancel.Enabled = false;
                dateTimePickerExpiryDate.Enabled = false;
            }
            else
            {
                comboBoxContractNumber.Enabled = true;
                textBoxContractValue.Enabled = true;
                comboBoxSupplier.Enabled = true;
                textBoxExpireWithin.Enabled = true;
                textBoxNotes.Enabled = true;
                checkBoxAlert.Enabled = true;
                checkBoxEmail.Enabled = true;
                buttonOK.Enabled = true;
                buttonDelete.Enabled = true;
                buttonCancel.Enabled = true;
                dateTimePickerExpiryDate.Enabled = true;
            }
        }

        private void ClearControlsSupportContract()
        {
            comboBoxContractNumber.Text = "";
            textBoxContractValue.Clear();
            comboBoxSupplier.Text = ""; ;
            textBoxExpireWithin.Clear();
            textBoxNotes.Clear();
            checkBoxAlert.Checked = false;
            checkBoxEmail.Checked = false;            
            dateTimePickerExpiryDate.Value=DateTime.Now;
        }

        private void checkBoxSupportContract_CheckedChanged(object sender, EventArgs e)
        {
            InitialiseSupportContractTab();
        }
        private bool GetSupportContractDetails(string strAssetName)
        {
            
            int iAssetID = new AssetDAO().AssetIDByAssetName(strAssetName);
            if (iAssetID > 0)
            {               

                m_objSupportContract.AssetID = iAssetID;                    
                return true;
            }
            else
            {
                    return false;
            }            
            
        }

        private bool GetSupportContractDetails()
        {
            string strContractNumber = comboBoxContractNumber.Text.ToString();
            string strContractValue = textBoxContractValue.Text.ToString();
            string strDays = textBoxExpireWithin.Text.ToString();
            if (strContractNumber != "" && strContractNumber != "< Add New >")
            {
                                
                    m_objSupportContract.ContractNumber = strContractNumber;
                    if (strContractValue != "")
                    {
                        m_objSupportContract.ContractValue = Convert.ToInt32(strContractValue);
                    }
                    m_objSupportContract.Supplier = comboBoxSupplier.Text.ToString();
                    m_objSupportContract.ExpiryDate = Convert.ToDateTime(dateTimePickerExpiryDate.Text.ToString());
                    m_objSupportContract.AlertFlag = checkBoxAlert.Checked;
                    m_objSupportContract.AlertByEmail = checkBoxEmail.Checked;
                    if (strDays != "")
                    {
                        m_objSupportContract.NoOfDays = Convert.ToInt32(strDays);    
                    }
                    m_objSupportContract.AssetID = _asset.AssetID;
                    m_objSupportContract.Notes = textBoxNotes.Text.ToString();
                    return true;               
            }
            else
            {
                MessageBox.Show("Please Enter a Contract Number.", "Audit Wizard");
                return false;
            }
        }


        private void buttonDelete_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Do you want to delete the Support Contract?", "AuditWizard", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
               AssetDAO objAsseDAO = new AssetDAO();
               objAsseDAO.DeleteSupportContract(_asset.AssetID);
               checkBoxSupportContract.Checked = false;
               ClearControlsSupportContract();
               FillSupportContractComboBox();
               FillSuppliersComboBox();
               MessageBox.Show("Support Contract Successfully Deleted", "AuditWizard", MessageBoxButtons.OK, MessageBoxIcon.Information);
               this.panelSupportExpired.Visible = false;
            }
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            buttonOK.Text = "OK";
            if (tree.SelectedNodes.Count > 0)
            {
                tree.SelectedNodes.Clear();
            }
            ClearControlsSupportContract();
            FillControlsOnLoad();
            FillSupportContractComboBox();
            FillSuppliersComboBox();
            checkBoxSupportContract.Checked = false;
            InitialiseSupportContractTab();
        }

        private void textBoxContractValue_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar != '\b')
            {
                e.Handled = !char.IsDigit(e.KeyChar);
            }

        }

        private void textBoxExpireWithin_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar != '\b')
            {
                e.Handled = !char.IsDigit(e.KeyChar);
            }
        }                   
               
                 
        
    }
}