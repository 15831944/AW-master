using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;

namespace Layton.AuditWizard.DataAccess
{
    public class XmlTextWriterEx : System.Xml.XmlTextWriter
    {
        public XmlTextWriterEx(TextWriter w)
            : base(w)
        { }

        public XmlTextWriterEx(Stream w, Encoding encoding)
            : base(w ,encoding)
        { }

        public XmlTextWriterEx(string filename, Encoding encoding)
            : base(filename, encoding)
        { }

        public void StartSection(string sectionName)
        {
			WriteStartElement(sectionName);
            //WriteStartElement("section");
            //WriteAttributeString("name", sectionName);
        }


        public void WriteSetting(string valueName, string strValue)
        {
			WriteStartElement(valueName);
			WriteString(strValue);
            //WriteAttributeString("name", valueName);
            //WriteAttributeString("value", strValue);
            WriteEndElement();
        }

        public void WriteSetting(string valueName, bool bValue)
        {
			WriteStartElement(valueName);
			WriteValue(bValue);
			WriteEndElement();
		}

        public void WriteSetting(string valueName, int nValue)
        {
			WriteStartElement(valueName);
			WriteValue(nValue);
			WriteEndElement();
		}

        public void EndSection()
        {
            WriteEndElement();
        }
    }
}
