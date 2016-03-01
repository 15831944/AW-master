using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Threading;
using System.Windows.Forms;
using Infragistics.Win.UltraWinEditors;
using Infragistics.Win.UltraWinMaskedEdit;
using Infragistics.Win.UltraWinTabControl;
using Layton.AuditWizard.Common;
using Layton.AuditWizard.DataAccess;
using Layton.Common.Controls;

namespace Layton.AuditWizard.Applications
{
	public partial class FormApplicationProperties : ShadedImageForm
	{
		private readonly InstalledApplication _installedApplication;
		private readonly UserDataCategoryList _listCategories = new UserDataCategoryList(UserDataCategory.SCOPE.Application);
		private PickListList _listPickLists;
		private const int labelWidth = 100;
		private const int textWidth = 250;

		public FormApplicationProperties(InstalledApplication application)
		{
            notesControl = new NotesControl(application);
            documentsControl = new DocumentsControl(application);
			InitializeComponent();
			_installedApplication = application;

			// Populate the tabs
			InitializeGeneralTab();
			InitializeNotesTab();
			InitializeDocumentsTab();

			// Initialize User Defined Data Tabe for this application
			InitializeUserDefinedData();
		}

		protected void InitializeGeneralTab()
		{
			// Populate the window
			tbPublisher.Text = _installedApplication.Publisher;
			tbName.Text = _installedApplication.Name;
		    tbVersion.Text = _installedApplication.Version;

			// Determine if any applications have been aliased to this application as if so we cannot
			// alias this application to any other.
			ApplicationsDAO lwDataAccess = new ApplicationsDAO();
			if (lwDataAccess.GetAliasCount(_installedApplication.ApplicationID) != 0)
			{
                lblAliasDescription.Enabled = false;
                lblAliasedTo.Enabled = false;
                tbAliasedApplication.Enabled = false;
                bnBrowseAlias.Enabled = false;
			}
		}

		protected void InitializeNotesTab()
		{
			notesControl.LoadNotes(SCOPE.Application, _installedApplication.ApplicationID);
		}


		protected void InitializeDocumentsTab()
		{
			documentsControl.LoadDocuments(SCOPE.Application, _installedApplication.ApplicationID);
		}

		#region General Tab Functions

		/// <summary>
		/// Called when we click on the button to select a different publisher
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void bnSetPublisher_Click(object sender, EventArgs e)
		{
			FormSelectPublisher selectPublisher = new FormSelectPublisher(true);
			if (selectPublisher.ShowDialog() == DialogResult.OK)
			{
				tbPublisher.Text = selectPublisher.SelectedPublisher;

				// Ensure that this publisher is in the publisher filter otherwise this application could disapp[ear
				SettingsDAO lwDataAccess = new SettingsDAO();
				string publisherFilter = lwDataAccess.GetPublisherFilter();
				if ((publisherFilter != "") && (!publisherFilter.Contains(tbPublisher.Text)))
				{
					publisherFilter = publisherFilter + ";" + tbPublisher.Text;
					lwDataAccess.SetPublisherFilter(publisherFilter);
				}
			}
		}

		#endregion General Tab Functions


		#region User Defined Data Functions

		protected void InitializeUserDefinedData()
		{
			_listCategories.Populate();

			// Suspend layout of the tab control until we have finished
			dataTabControl.SuspendLayout();

			// Create a tab for each category which is pertinent for this asset
			foreach (UserDataCategory category in _listCategories)
			{
				category.GetValuesFor(_installedApplication.ApplicationID);
				AddCategory(category);
			}

			// Ensure that the first tab is selected
			dataTabControl.Tabs[0].Selected = true;

			// Resume layout
			dataTabControl.ResumeLayout();
		}

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
		/// <param name="dataField"></param>
		protected void CreateBooleanField(UserDataField dataField, out Label label, out Control control)
		{
			// Create the label;
			label = new Label();
			label.Text = dataField.Name;
			label.Size = new System.Drawing.Size(labelWidth, 13);

			// Create a combobox with True and False values
			ComboBox combo = new ComboBox();
			control = combo;
			combo.DropDownStyle = ComboBoxStyle.DropDownList;
			combo.Items.Add("True");
			combo.Items.Add("False");
		
			// Set its value
			combo.SelectedIndex = (dataField.BooleanValue(_installedApplication.ApplicationID)) ? 0 : 1;
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

            try
            {
                DateTime value = Convert.ToDateTime(dataField.GetValueFor(_installedApplication.ApplicationID));
                nullableDateTime.Value = value;
            }
            catch (FormatException)
            {
                nullableDateTime.Value = null;
            }

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

            try
            {
                currencyEditor.Value = Convert.ToDecimal(dataField.GetValueFor(_installedApplication.ApplicationID));
            }
            catch (FormatException)
            {
                currencyEditor.Value = 0;
            }
        }

        /// <summary>
        /// Create a label and input control for a Numeric field
        /// </summary>
        protected void CreateNumberField(UserDataField dataField, out Label label, out Control control)
        {
            // Create the label;
            label = new Label { Text = dataField.Name, AutoSize = true, Size = new Size(labelWidth, 13) };

            NumericUpDown nup = new NumericUpDown { TextAlign = HorizontalAlignment.Left, Width = 110 };
            control = nup;
            nup.Maximum = Decimal.MaxValue;
            nup.Minimum = Decimal.MinValue;

            //NumericUpDown numericUpDown = (NumericUpDown)dataField.Tag;
            try
            {
                nup.Value = Convert.ToDecimal(dataField.GetValueFor(_installedApplication.ApplicationID));
            }
            catch (FormatException)
            {
                nup.Value = 0;
            }
        }


        /// <summary>
        /// Create a label and input control for a Boolean field
        /// </summary>
        protected void CreatePicklistField(UserDataField dataField, out Label label, out Control control)
        {
            // Do we already have the picklists read?  If not then do so now
            _listPickLists = new PickListList();
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

            int index = combo.FindStringExact(dataField.GetValueFor(_installedApplication.ApplicationID));
            combo.SelectedIndex = (index != -1) ? index : -1;
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
            textbox.Text = dataField.GetValueFor(_installedApplication.ApplicationID);
        }


		/// <summary>
		/// Create a label and input control for a textual field
		/// </summary>
		/// <param name="dataField"></param>
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

			// Set the current value
			textbox.Text = dataField.GetValueFor(_installedApplication.ApplicationID);
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


		/// <summary>
		/// Called as we exit from this form to save any changes made to the user defined data for
		/// this application.
		/// </summary>
		protected void SaveUserDefinedData()
		{
			// Loop through the user data categories and fields and recover their values
			foreach (UserDataCategory category in _listCategories)
			{
				foreach (UserDataField dataField in category)
				{
					UpdateUserDataFieldValue(dataField);
				}
			}
		}




		/// <summary>
		/// Update a specific user data field value noting that the control used by the field is stored
		/// as the tag for the field so we can simply iterate through the fields
		/// </summary>
		/// <param name="field"></param>
		/// <param name="control"></param>
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
			if (newValue != dataField.GetValueFor(_installedApplication.ApplicationID))
				dataField.SetValueFor(_installedApplication.ApplicationID, newValue, true);
		}



		#endregion User Defined Data Functions

		/// <summary>
		/// Called as we click the OK button - save the definition back to the database
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void bnOK_Click(object sender, EventArgs e)
		{
			if (_installedApplication.Publisher != tbPublisher.Text)
			{
                string oldPublisherName = _installedApplication.Publisher;
				ApplicationsDAO lwDataAccess = new ApplicationsDAO();
				lwDataAccess.ApplicationUpdatePublisher(_installedApplication.ApplicationID, tbPublisher.Text);
				_installedApplication.Publisher = tbPublisher.Text;

                AuditTrailEntry ate = new AuditTrailEntry();
                ate.Date = DateTime.Now;
                ate.Class = AuditTrailEntry.CLASS.application_changes;
                ate.Type = AuditTrailEntry.TYPE.changed;
                ate.Key = _installedApplication.Name + "|Publisher";
                ate.AssetID = 0;
                ate.AssetName = "";
                ate.OldValue = oldPublisherName;
                ate.NewValue = tbPublisher.Text;
                ate.Username = System.Environment.UserName;
                new AuditTrailDAO().AuditTrailAdd(ate);
			}

			// Save any changes made to the user defined data field values
			SaveUserDefinedData();
		}


		/// <summary>
		/// Called to allow the user to select an application to which this application should be aliased
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void bnBrowseAlias_Click(object sender, EventArgs e)
		{
			FormSelectPublishersAndApplications form = new FormSelectPublishersAndApplications("", "");
			form.SelectionType = SelectApplicationsControl.eSelectionType.singleapplication;
			form.ShowIgnored = true;
			form.ShowIncluded = true;

			if (form.ShowDialog() == DialogResult.OK)
			{
				// Set the alias for this application
				ApplicationsDAO lwDataAccess = new ApplicationsDAO();
				lwDataAccess.ApplicationSetAlias(_installedApplication.ApplicationID, form.SelectedApplication().ApplicationID);
				tbAliasedApplication.Text = form.SelectedApplication().Name;
			}
		}
	}
}