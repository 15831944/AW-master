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
using Layton.Common.Controls;

namespace Layton.AuditWizard.Common
{
	public partial class FormStyleChooser : Layton.Common.Controls.ShadedImageForm
	{
		#region Data

		private FormattedText _formattedText;
		
		#endregion Data
		
		#region Properties
		
		public FormattedText FormattedText
		{
			get { return _formattedText; }
		}
		
		#endregion Properties
		
		#region Constructor
		
		public FormStyleChooser(FormattedText formattedText)
		{
			InitializeComponent();

			_formattedText = formattedText;
				
			// Initialize the text alignment box
			cbAlignment.Items.Clear();
			cbAlignment.Items.Add(Infragistics.Win.HAlign.Left, "Left");
			cbAlignment.Items.Add(Infragistics.Win.HAlign.Right, "Right");
			cbAlignment.Items.Add(Infragistics.Win.HAlign.Center, "Center");
			
			// Pick up the alignment
			cbAlignment.SelectedIndex = 0;
		}

		#endregion Constructor

		#region Methods
		
		/// <summary>
		/// Called as we load this form to set the initial values based on the object passed in
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void FormStyleChooser_Load(object sender, EventArgs e)
		{
			// Select the correct alignment from that passed through to us
			foreach (ValueListItem item in cbAlignment.Items)
			{
				if (((int)_formattedText.HorizontalAlignment) == (int)item.DataValue)
				{
					cbAlignment.SelectedItem = item;
					break;
				}
			}
			
			// Set foreground and background color
			foreColorPicker.Color = _formattedText.ForeColor;
			//backColorPicker.Color = _formattedText.BackColor;
			
			// Bold and Underline
			cbBold.Checked = (_formattedText.FontData.Bold == DefaultableBoolean.True);
			cbUnderline.Checked = (_formattedText.FontData.Underline == DefaultableBoolean.True);
			
			// Font name and size
			fontName.Text = _formattedText.FontData.Name;
			nupFontSize.Value = (int)_formattedText.FontData.SizeInPoints;

			// Set the preview box			
			ResetPreview();
		}

		/// <summary>
		/// Called to make the preview label look like the settings currently chosen
		/// </summary>
		private void ResetPreview()
		{
			labelPreview.Appearance.ForeColor = foreColorPicker.Color;
			labelPreview.Appearance.FontData.Bold = (cbBold.Checked) ? DefaultableBoolean.True : DefaultableBoolean.False;
			labelPreview.Appearance.FontData.Underline = (cbUnderline.Checked) ? DefaultableBoolean.True : DefaultableBoolean.False;
			labelPreview.Appearance.FontData.Name = fontName.Text;
			labelPreview.Appearance.FontData.SizeInPoints = (int)nupFontSize.Value;
			labelPreview.Appearance.TextHAlign = (Infragistics.Win.HAlign)cbAlignment.SelectedItem.DataValue;
		}
		
		
		private void style_Changed(object sender, EventArgs e)
		{
			ResetPreview();
		}


		/// <summary>
		/// Called as we click OK to exit from this form - save any format changes
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void bnOK_Click(object sender, EventArgs e)
		{
			_formattedText.HorizontalAlignment = (HAlign)cbAlignment.SelectedItem.DataValue;
			
			// Set color
			_formattedText.ForeColor = foreColorPicker.Color;
			
			// Bold and Underline
			_formattedText.FontData.Bold = (cbBold.Checked) ? DefaultableBoolean.True : DefaultableBoolean.False;
			_formattedText.FontData.Underline = (cbUnderline.Checked) ? DefaultableBoolean.True : DefaultableBoolean.False;
			
			// Font name and size
			_formattedText.FontData.Name = fontName.Text;
			_formattedText.FontData.SizeInPoints = (int)nupFontSize.Value;
		}

		#endregion Methods
	}
}

