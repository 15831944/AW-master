using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using Infragistics.Win;
//
using Layton.AuditWizard.DataAccess;

namespace Layton.AuditWizard.Common
{
	public class FormattedText
	{
		#region Data

		private string		_rawText = "";
		private string		_formattedText = "";
		private FontData	_fontData = new FontData();
		private HAlign		_hAlignment = HAlign.Left;
		private VAlign		_vAlignment = VAlign.Middle;
		private Color		_foreColor = Color.Black;
		//private Color		_backColor = Color.White;

		#endregion Data

		#region XML Strings for Exporting

		protected const string V_SECTION_TEXT		= "RawText";
		protected const string V_SECTION_HALIGN		= "Halign";
		protected const string V_SECTION_VALIGN		= "Valign";
		protected const string V_SECTION_FORECOLOR	= "ForeColor";
		//protected const string V_SECTION_BACKCOLOR	= "BackColor";
		
		// We only save selected attributes of FontData namely FontName and Size, Bold and Underline
		protected const string V_SECTION_FONTDATA_NAME = "FontDataName";
		protected const string V_SECTION_FONTDATA_SIZE = "FontDataSize";
		protected const string V_SECTION_FONTDATA_UNDERLINE = "FontDataUnderline";
		protected const string V_SECTION_FONTDATA_BOLD = "FontDataBold";
		
		#endregion XML Strings for Exporting
		
		#region Properties
		
		public string RawText
		{
		  get { return _rawText; }
		  set { _rawText = value; }
		}
		
		public string GetFormattedText
		{
		  get { return _formattedText; }
		}
		
		public FontData FontData
		{
		  get { return _fontData; }
		  set { _fontData = value; }
		}
		
		public HAlign HorizontalAlignment
		{
		  get { return _hAlignment; }
		  set { _hAlignment = value; }
		}
		
		public VAlign VerticalAlignment
		{
		  get { return _vAlignment; }
		  set { _vAlignment = value; }
		}
		
		public Color ForeColor
		{
		  get { return _foreColor; }
		  set { _foreColor = value; }
		}
		
		//public Color BackColor
		//{
		//  get { return _backColor; }
		//  set { _backColor = value; }
		//}
				
		#endregion Properties
		
		#region Constructor
		
		public FormattedText()
		{
			_rawText = "";
			_formattedText = "";
			_fontData = new FontData();
			_fontData.Name = "Verdana";
			_fontData.SizeInPoints = 10;
			_foreColor = Color.Black;
			//_backColor = Color.White;
			_hAlignment = HAlign.Left;
			_vAlignment = VAlign.Top;
		}
		
		public FormattedText (string rawText) 
			: this()
		{
			_rawText = rawText;
		}
		
		public FormattedText (string rawText ,FontData fontData ,Color foreColor ,HAlign hAlign ,VAlign vAlign) 
			: this()
		{
			_rawText = rawText;
			_fontData = fontData;
			_foreColor = foreColor;
			_hAlignment = hAlign;
			_vAlignment = vAlign;
		}		
		
		#endregion Constructor
		
		#region Methods		
		
		/// <summary>
		/// Create the formatted string by substituting any occurences of the first string with the second
		/// </summary>
		/// <param name="substitutions"></param>
		public string FormatUsingSubstitutions(Dictionary<string ,string> substitutions)
		{
			_formattedText = _rawText;
			foreach (KeyValuePair<string ,string> kvp in substitutions)
			{
				_formattedText = _formattedText.Replace(kvp.Key ,kvp.Value);
			}
			return _formattedText;
		}
		
		
		/// <summary>
		/// Called to save the attributes for this object to an XML file
		/// </summary>
		/// <param name="writer"></param>
		/// <returns></returns>
		public int SaveToXml(XmlTextWriterEx writer)
		{
			writer.WriteSetting(V_SECTION_TEXT, _rawText);
			writer.WriteSetting(V_SECTION_HALIGN, ((int)_hAlignment).ToString());
			writer.WriteSetting(V_SECTION_VALIGN, ((int)_vAlignment).ToString());
			writer.WriteSetting(V_SECTION_FORECOLOR, _foreColor.Name);
			//writer.WriteSetting(V_SECTION_BACKCOLOR, _backColor.ToString());
			//
			writer.WriteSetting(V_SECTION_FONTDATA_NAME, _fontData.Name);
			writer.WriteSetting(V_SECTION_FONTDATA_SIZE, _fontData.SizeInPoints.ToString());
			writer.WriteSetting(V_SECTION_FONTDATA_UNDERLINE, (_fontData.Underline == DefaultableBoolean.True) ? "Y" : "N");
			writer.WriteSetting(V_SECTION_FONTDATA_BOLD, (_fontData.Bold == DefaultableBoolean.True) ? "Y" : "N");
			
			return 0;
		}


		/// <summary>
		/// Load this object from XML
		/// </summary>
		/// <param name="reader"></param>
		/// <returns></returns>
		public int LoadFromXml(XmlSimpleElement reader)
		{
			foreach (XmlSimpleElement childElement in reader.ChildElements)
			{
				switch (childElement.TagName)
				{
					case V_SECTION_TEXT:
						_rawText = childElement.Text;
						break;

					case V_SECTION_HALIGN:
						_hAlignment = (HAlign)childElement.TextAsInt;
						break;

					case V_SECTION_VALIGN:
						_vAlignment = (VAlign)childElement.TextAsInt;
						break;

					case V_SECTION_FORECOLOR:
						_foreColor = Color.FromName(childElement.Text);
						break;

					//case V_SECTION_BACKCOLOR:
					//    _backColor = Color.FromName(childElement.Text);
					//    break;

					case V_SECTION_FONTDATA_SIZE:
						_fontData.SizeInPoints = childElement.TextAsInt;
						break;

					case V_SECTION_FONTDATA_NAME:
						_fontData.Name = childElement.Text;
						break;

					case V_SECTION_FONTDATA_UNDERLINE:
						_fontData.Underline = (childElement.TextAsBoolean) ? DefaultableBoolean.True : DefaultableBoolean.False;
						break;

					case V_SECTION_FONTDATA_BOLD:
						_fontData.Bold = (childElement.TextAsBoolean) ? DefaultableBoolean.True : DefaultableBoolean.False;
						break;

					default:
						break;
				}
			}
			return 0;
		}

		#endregion Methods
		
	}
}
