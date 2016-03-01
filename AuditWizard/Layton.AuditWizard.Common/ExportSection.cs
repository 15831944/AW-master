using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
//
using Infragistics.Win;
using Layton.AuditWizard.DataAccess;

namespace Layton.AuditWizard.Common
{
	#region ExportSection Class

	public class ExportSection
	{
		public enum eSectionType { header ,subheader ,footer }

		#region data
		
		private eSectionType	_sectionType;
		private FormattedText	_formattedText;
		#endregion data
		
		#region Properties
		public eSectionType		SectionType
		{
			get { return _sectionType; }
			set { _sectionType = value; }
		}
		
		public FormattedText FormattedText
		{
			get { return _formattedText; }
			set { _formattedText = value; }
		}
		
		#endregion Properties
		
		#region XML Strings for Exporting
		
		protected const string V_SECTION_TYPE = "Type";
		protected const string S_REPORT_SECTION_FORMAT  = "FormattedText";
	
		#endregion XML Strings for Exporting

		#region Constructor

		public ExportSection ()
		{
			_sectionType = eSectionType.header;
			_formattedText = new FormattedText();
		}

		public ExportSection (eSectionType sectiontype ,FormattedText formattedText)
		{
			_sectionType = sectiontype;
			_formattedText = formattedText;
		}
		
		#endregion Constructor
		
		#region Methods

		public int SaveToXml(XmlTextWriterEx writer)
		{
			writer.WriteSetting(V_SECTION_TYPE, ((int)_sectionType).ToString());
			
			// Save the formatted text object in its own section
			writer.StartSection(S_REPORT_SECTION_FORMAT);		
			_formattedText.SaveToXml(writer);
			writer.EndSection();
			
			return 0;
		}


		/// <summary>
		/// Load this object definition from XML
		/// </summary>
		/// <param name="reader"></param>
		/// <returns></returns>
		public int LoadFromXml(XmlSimpleElement reader)
		{
			foreach (XmlSimpleElement childElement in reader.ChildElements)
			{
				switch (childElement.TagName)
				{
					case V_SECTION_TYPE:
						_sectionType = (eSectionType)childElement.TextAsInt;
						break;

					// If we find the FormattedText section then let that object load itself
					case S_REPORT_SECTION_FORMAT:
						_formattedText.LoadFromXml(childElement);
						break;

					default:
						break;
				}
			}
			
			return 0;
		}

		
		#endregion Mathod
	}

	#endregion ExportSection Class

	#region ExportSectionList Class
	
	public class ExportSectionList : List<ExportSection>
	{

		/// <summary>
		/// return the first section in the report with the specified type
		/// </summary>
		/// <param name="sectionType"></param>
		/// <returns></returns>
		public ExportSection GetSection(ExportSection.eSectionType sectionType)
		{
			foreach (ExportSection reportSection in this)
			{
				if (reportSection.SectionType == sectionType)
					return reportSection;
			}

			return null;
		}
	
	}
	
	#endregion ExportSectionList Class


}
