using System;
using System.Collections;

namespace Layton.AuditWizard.DataAccess
{
	/// <summary>
	/// Summary description for SimpleElements.
	/// </summary>
	public class XmlSimpleElements : CollectionBase 
	{
		public XmlSimpleElements() 
		{
		}
 
		public void Add(XmlSimpleElement se) 
		{
			this.List.Add(se);
		}

		public XmlSimpleElement Item(int index) 
		{
			return (XmlSimpleElement) this.List[index];
		}

		public Object[] ToArray() 
		{
			Object[] ar = new Object[this.List.Count];
			for (int i=0; i < this.List.Count; i++) 
			{
				ar[i] = this.List[i];
			}
			return ar;
		}
	}
}