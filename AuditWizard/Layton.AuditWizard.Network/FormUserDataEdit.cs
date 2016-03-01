using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Text;
using System.Threading;
using System.Windows.Forms;
// 
using Infragistics.Win.UltraWinTabControl;
using Infragistics.Win.Misc;
using Infragistics.Win.Layout;

//
using Layton.AuditWizard.Common;
using Layton.AuditWizard.DataAccess;
using Layton.Common.Controls;

namespace Layton.AuditWizard.Network
{
	public partial class FormUserDataEdit : Layton.Common.Controls.ShadedImageForm
	{
		private Asset _asset;
		private UserDataCategory _category;
		private PickListList _listPickLists = null;
		UserDataCategoryList _userDataCategories = new UserDataCategoryList(UserDataCategory.SCOPE.Asset);

		public FormUserDataEdit(Asset asset ,UserDataCategory category)
		{
			InitializeComponent();
			_asset = asset;
			_category = category;
		}

		private void FormUserDataEdit_Load(object sender, EventArgs e)
		{
			// Read the picklist definitions
			_listPickLists = new PickListList();
			_listPickLists.Populate();

			// Read ALL user data categories
			_userDataCategories.Populate();

			// Suspend layout of the tab control until we have finished
			dataTabControl.SuspendLayout();

			// Create a tab for each category which is pertinent for this asset
			foreach (UserDataCategory category in _userDataCategories)
			{
				// Is this category applicable to this asset?  
				// If so add a new tab to the control
				if (category.CategoryAppliesTo(_asset))
				{
					category.GetValuesFor(_asset.AssetID);
					AddCategory(category);
				}
			}

			// Ensure that the first tab is selected
			dataTabControl.Tabs[0].Selected = true;

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
			// Create the objects required to add a tab
			UltraTab newTab = new UltraTab();
			newTab.Tag = category;
			Infragistics.Win.Appearance newAppearance = new Infragistics.Win.Appearance();

			// Add the new tab to the control
			this.dataTabControl.Tabs.Add(newTab);

			// Set the tab appearance
			newAppearance.Image = IconMapping.LoadIcon(category.Icon, IconMapping.Iconsize.Small);
			newTab.Appearance = newAppearance;
			newTab.Text = category.Name;

			// Create a Table Layout Panel which will fit on the tab
			TableLayoutPanel tableLayoutPanel = new TableLayoutPanel();
			// 
			// tableLayoutPanel
			// 
			tableLayoutPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			//tableLayoutPanel.CellBorderStyle = System.Windows.Forms.TableLayoutPanelCellBorderStyle.Single;
			tableLayoutPanel.Location = new System.Drawing.Point(10, 10);
			tableLayoutPanel.Padding = new System.Windows.Forms.Padding(5);
			tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
			tableLayoutPanel.TabIndex = 0;

			// How many rows do we need?  We will try and limit to 12 rows but we may need to increase this depending on 
			// how many fields there are.
			if (category.Count > 30)
			{
				tableLayoutPanel.RowCount = (category.Count / 2);
			}
			else
			{
				tableLayoutPanel.RowCount = 12;
			}

			// Either one or two sets of columns
			if (tableLayoutPanel.RowCount <= 12)
			{
				tableLayoutPanel.ColumnCount = 2;
				tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 30F));
				tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 70F));
			}
			else
			{
				tableLayoutPanel.ColumnCount = 5;
				tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 15F));
				tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 30F));
				tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 10F));
				tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 15F));
				tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 30F));
			}

				
			// Add this to the tab
			newTab.TabPage.Controls.Add(tableLayoutPanel);
			Size containerSize = newTab.TabPage.Size;
			Size tableSize = new Size(containerSize.Width - 20, containerSize.Height - 20);
			tableLayoutPanel.Size = tableSize;


			// Get the list of fields which form this category 
			List<Control> listControls = new List<Control>();
			foreach (UserDataField dataField in category)
			{
				Label label = null;
				Control control = null;

				switch ((int)dataField.Type)
				{
					//case (int)UserDataField.FieldType.boolean:
					//	CreateBooleanField(dataField, out label, out control);
					//	break;

					//case (int)UserDataField.FieldType.date:
					//	CreateDateField(dataField, out label, out control);
					//	break;

					case (int)UserDataField.FieldType.Number:
						CreateNumberField(dataField, out label, out control);
						break;

					case (int)UserDataField.FieldType.Picklist:
						CreatePicklistField(dataField, out label, out control);
						break;

					case (int)UserDataField.FieldType.Text:
						CreateTextField(dataField, out label, out control);
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
			for (int isub=0; isub<listControls.Count; isub+=2 )
			{
				// Get the controls
				Label label = (Label)listControls[isub];
				Control control = listControls[isub+1];

				// Ensure that the controls will align at the top, left of their cell
				label.Anchor = AnchorStyles.Left | AnchorStyles.Top;
				label.Location = new Point(0, 0);
				control.Anchor = AnchorStyles.Left | AnchorStyles.Top;
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

			// Create a combobox with True and False values
			ComboBox combo = new ComboBox();
			control = combo;
			combo.DropDownStyle = ComboBoxStyle.DropDownList;
			combo.Items.Add("True");
			combo.Items.Add("False");

			// Set its value
			combo.SelectedIndex = (dataField.BooleanValue(_asset.AssetID)) ? 0 : 1;
		}


		/// <summary>
		/// Create a label and input control for a Date field
		/// </summary>
		/// <param name="dataField"></param>
		protected void CreateDateField(UserDataField dataField, out Label label, out Control control)
		{
			// Create the label;
			label = new Label();
			label.Text = dataField.Name;
			label.AutoSize = true;

			// Create a Date/Time Picker
			NullableDateTimePicker nullableDateTime = new NullableDateTimePicker();
			nullableDateTime.Value = null;
			control = nullableDateTime;

			// Set its value
			if (dataField.DateValue(_asset.AssetID) != "")
				nullableDateTime.Value = Convert.ToDateTime(dataField.DateValue(_asset.AssetID));
		}


		/// <summary>
		/// Create a label and input control for a Numeric field
		/// </summary>
		/// <param name="dataField"></param>
		protected void CreateNumberField(UserDataField dataField, out Label label, out Control control)
		{
			// Create the label;
			label = new Label();
			label.Text = dataField.Name;

			// Create a NumericUpDown field and set any limits
            NumericUpDown nup = new NumericUpDown();
            control = nup;
            nup.Minimum = dataField.MinimumValue;
            nup.Maximum = dataField.MaximumValue;

            // Set its value
            decimal lValue = dataField.NumericValue(_asset.AssetID);

            if (lValue > nup.Maximum)
                nup.Value = nup.Maximum;
            else
                nup.Value = lValue;    
		}

		/// <summary>
		/// Create a label and input control for a Boolean field
		/// </summary>
		/// <param name="dataField"></param>
		protected void CreatePicklistField(UserDataField dataField, out Label label, out Control control)
		{
			// Do we already have the picklists read?  If not then do so now
			if (_listPickLists == null)
			{
				_listPickLists = new PickListList();
				_listPickLists.Populate();
			}

			// Create the label;
			label = new Label();
			label.Text = dataField.Name;
			label.AutoSize = true;

			// Create a combobox with values taken from the picklist
			ComboBox combo = new ComboBox();
			control = combo;
			combo.DropDownStyle = ComboBoxStyle.DropDownList;

			// Get the picklist which is in use and add the pickitems to the combo
			PickList picklist = _listPickLists.FindPickList(dataField.Picklist);
			if (picklist != null)
			{
				foreach (PickItem pickitem in picklist)
				{
					combo.Items.Add(pickitem);
				}
			}

			// Set the current selection (if any)
			int index = combo.FindStringExact(dataField.GetValueFor(_asset.AssetID));
			if (index != -1)
				combo.SelectedIndex = index;
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

			// Create a textbox and set any requirements
			TextBox textbox = new TextBox();
			control = textbox;

			// Do we have a maximum length specified
			if (dataField.Length != 0)
				textbox.MaxLength = dataField.Length;

			// What about case?
            //switch ((int)dataField.InputCase)
            //{
            //    case (int)UserDataField.FieldCase.any:
            //        break;

            //    case (int)UserDataField.FieldCase.lower:
            //        textbox.CharacterCasing = CharacterCasing.Lower;
            //        break;

            //    case (int)UserDataField.FieldCase.upper:
            //        textbox.CharacterCasing = CharacterCasing.Upper;
            //        break;

            //    case (int)UserDataField.FieldCase.title:
            //        textbox.TextChanged += new EventHandler(textBox_TextChanged);
            //        break;
            //}

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
                    textbox.LostFocus += new EventHandler(textBox_TextChanged);
                    break;
            }

			// Set the current value
			textbox.Text = dataField.GetValueFor(_asset.AssetID);
            textbox.Width = 300;

            if ((dataField.Type == UserDataField.FieldType.Environment) || (dataField.Type == UserDataField.FieldType.Registry))
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

		/// <summary>
		/// Called as we click to exit from this form - we need to save any changes made to the 
		/// user defined data field values back to the database.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void bnOK_Click(object sender, EventArgs e)
		{
			// Loop through the user data categories and fields and recover their values
			foreach (UserDataCategory category in _userDataCategories)
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
		/// <param name="field"></param>
		/// <param name="control"></param>
		private void UpdateUserDataFieldValue(UserDataField dataField)
		{
			string newValue; 
			Control control = dataField.Tag as Control;

			// The data value depends on the field type
			switch ((int)dataField.Type)
			{
				//case (int)UserDataField.FieldType.boolean:
				//    ComboBox comboBoolean = (ComboBox)control;
				//    newValue = (comboBoolean.SelectedIndex == -1) ? "" : comboBoolean.SelectedText;
				//    break;

				//case (int)UserDataField.FieldType.date:
				//    NullableDateTimePicker nullableDateTime = (NullableDateTimePicker)control;
				//    newValue = (nullableDateTime.Value == null) ? null : nullableDateTime.Value.ToString(); 
				//    break;

				case (int)UserDataField.FieldType.Number:
					NumericUpDown nup = (NumericUpDown)control;
					newValue = nup.Value.ToString();
					break;

				case (int)UserDataField.FieldType.Picklist:
					ComboBox comboPicklist = (ComboBox)control;
					newValue = (comboPicklist.SelectedIndex == -1) ? "" : comboPicklist.Text;
					break;

				case (int)UserDataField.FieldType.Text:
					TextBox textbox = (TextBox)control;
					newValue = textbox.Text;
					break;

                case (int)UserDataField.FieldType.Environment:
                    TextBox textbox1 = (TextBox)control;
                    newValue = textbox1.Text;
                    break;

                case (int)UserDataField.FieldType.Registry:
                    TextBox textbox2 = (TextBox)control;
                    newValue = textbox2.Text;
                    break;

				default:
					newValue = "";
					break;
			}

			// Has the value changed from that currently stored?
			if (newValue != dataField.GetValueFor(_asset.AssetID))
				dataField.SetValueFor(_asset.AssetID ,newValue ,true);
		}
	}
}

