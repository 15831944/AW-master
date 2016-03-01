using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace Layton.AuditWizard.DataAccess
{
    public class XmlDocumentEx : System.Xml.XmlDocument
    {

        //
        //    GetConfigValue
        //    ==============
        //
        //    Returns a BOOLEAN value from a config file optionally setting a default
        //    if no entry found in the XML
        //
        public int GetConfigValue(string sectionName, string settingName, bool bDefault, out bool bReturn)
        {
            string strValue;

            // Return a string value - if this fails then use the default
            if (GetConfigValue(sectionName, settingName, out strValue) != 0)
                bReturn = bDefault;
            else
                bReturn = (strValue == "yes") ? true : false;

            return 0;
        }

        //
        //    GetConfigValue
        //    ==============
        //
        //    Returns an INTEGER value from a config file optionally setting a default
        //    if no entry found in the XML
        //
        public int GetConfigValue(string sectionName, string settingName, int nDefault, out int nReturn)
        {
            string strValue;

            // Return a string value - if this fails then use the default
            if (GetConfigValue(sectionName, settingName, out strValue) != 0)
                nReturn = nDefault;
            else
                nReturn = Convert.ToInt32(strValue, 10);

            return 0;
        }


        //
        //    GetConfigValue
        //    ==============
        //
        //    Returns a STRING value from a config file optionally setting a default
        //    if no entry found in the XML
        //
        public int GetConfigValue(string sectionName, string settingName, string strDefault, out string strReturn)
        {
            if (GetConfigValue(sectionName, settingName, out strReturn) != 0)
                strReturn = strDefault;
            return 0;
        }


        // 
        //    GetConfigValue
        //    ==============
        //
        //    This generic version returns the value as a string - callers are responsible for converting
        //    to the appropriate datatype and handling default values.
        //
        private int GetConfigValue(string sectionName, string settingName, out string strReturn)
        {
            strReturn = "";

            try
            {
                // get setting node
                string xpath = String.Format("//section[@name='{0}']/setting[@name='{1}']"
                                            , sectionName
                                            , settingName);
                XmlNode node = this.DocumentElement.SelectSingleNode(xpath);

                // display value
                if (node != null)
                {
                    XmlAttribute xmlAttr = node.Attributes["value"];
                    if (xmlAttr != null)
                    {
                        strReturn = xmlAttr.Value;
                        return 0;
                    }
                }
            }
            catch (Exception)
            {
                // No handler here - we just assume that the call failed for some reason
            }
            return -1;
        }
    }
}
