using System;
using System.IO;
using System.Collections;
using System.Collections.Specialized;
using System.Xml;


namespace Layton.AuditWizard.DataAccess
{
    public class XmlParser
    {
        private XmlTextReader Reader;
        private Stack elements;
        private XmlSimpleElement currentElement;
        private XmlSimpleElement rootElement;

        public XmlParser()
        {
            elements = new Stack();
            currentElement = null;
        }

        public XmlSimpleElement Parse(XmlTextReader reader)
        {
            XmlSimpleElement se = null;
            this.Reader = reader;

            while (!Reader.EOF)
            {
                Reader.Read();

                switch (Reader.NodeType)
                {
                    case XmlNodeType.Element:
                        // create a new SimpleElement
                        se = new XmlSimpleElement(Reader.LocalName);
                        currentElement = se;
                        if (elements.Count == 0)
                        {
                            rootElement = se;
                            elements.Push(se);
                        }
                        else
                        {
                            XmlSimpleElement parent = (XmlSimpleElement)elements.Peek();
                            parent.ChildElements.Add(se);

                            // don't push empty elements onto the stack
                            //if (Reader.IsEmptyElement) // ends with "/>"
                            //    break;
                            //else
                            //    elements.Push(se);
                            if (!Reader.IsEmptyElement)
                                elements.Push(se);
                        }

                        if (Reader.HasAttributes)
                        {
                            while (Reader.MoveToNextAttribute())
                            {
                                currentElement.setAttribute(Reader.Name, Reader.Value);
                            }
                        }
                        break;

                    case XmlNodeType.Attribute:
                        se.setAttribute(Reader.Name, Reader.Value);
                        break;

                    case XmlNodeType.EndElement:
                        //pop the top element 
                        elements.Pop();
                        break;

                    case XmlNodeType.Text:
                        currentElement.Text = Reader.Value;
                        break;

                    case XmlNodeType.CDATA:
                        currentElement.Text = Reader.Value;
                        break;

                    default:
                        // ignore
                        break;
                }
            }

            return rootElement;
        }
    }
}