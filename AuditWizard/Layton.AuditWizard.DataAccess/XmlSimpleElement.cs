using System;
using System.Collections;
using System.Collections.Specialized;
using System.Text;

namespace Layton.AuditWizard.DataAccess
{
   public class XmlSimpleElement 
   {
      private String tagName;
      private String text;
      private StringDictionary attributes;
      private XmlSimpleElements childElements;

      public XmlSimpleElement(String tagName) 
	  {
         this.tagName = tagName;
         attributes = new StringDictionary();
         childElements = new XmlSimpleElements();
         this.text="";
      }

      public String TagName 
	  {
         get { return tagName; }
         set { this.tagName = value; }
      }

	   public string Text
	   {
		   get { return text; }
		   set { this.text = value; }
	   }

	   public bool TextAsBoolean
	   {
		   get { return (text == "true") ? true : false; }
		   set { this.text = (value == true) ? "true" : "false"; }
	   }

	   public int TextAsInt
	   {
		   get { return (Convert.ToInt32(text)); }
		   set { this.text = (Convert.ToString(value)); }
	   }

      public XmlSimpleElements ChildElements 
	  {
         get { return this.childElements; }
      }

      public StringDictionary Attributes 
	  {
         get { return this.attributes; }
      }

      public String Attribute(String name) 
	  {
         return (String) attributes[name];
      }

      public void setAttribute(String name, String value) 
	  {
         attributes.Add(name, value);
      }

	   public bool Contains (string name)
	   {
		   foreach (XmlSimpleElement element in this.childElements)
		   {
			   if (element.tagName == name)
				   return true;
		   }
		   return false;
	   }

	   public XmlSimpleElement FindChildElement(string name)
	   {
		   foreach (XmlSimpleElement element in this.childElements)
		   {
			   if (element.tagName == name)
				   return element;
		   }
		   return null;
	   }

	   public string ChildElementText(string name)
	   {
		   string returnValue = "";
		   XmlSimpleElement element = FindChildElement(name);
		   if (element != null)
			   returnValue = element.Text;
		   return returnValue;
	   }

	   public int ChildElementTextAsInt(string name)
	   {
		   int returnValue = 0;
		   XmlSimpleElement element = FindChildElement(name);
		   if (element != null)
			   returnValue = element.TextAsInt;
		   return returnValue;
	   }
   }
}
