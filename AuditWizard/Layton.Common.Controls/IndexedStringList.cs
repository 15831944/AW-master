using System;
using System.Collections.Generic;
using System.Text;

namespace Layton.Common.Controls
{

#region IndexedStringListEntry class

	public class IndexedStringListEntry
	{
		#region Data
		
		private string _text;
		private int _value;
		private object _tag;

		#endregion Data

		#region Properties

		/// <summary>
		/// Recover the text value
		/// </summary>
		public string Text
		{
			get { return _text; }
			set { _text = value; }
		}

		/// <summary>
		/// Recover the numeric index value
		/// </summary>
		public int Value
		{
			get { return _value; }
			set { _value = value; }
		}
		
		/// <summary>Recover any associated tag
		/// 
		/// </summary>
		public object Tag
		{
			get { return _tag; }
			set { _tag = value; }
		}

		#endregion Properties

#region Constructor

		public IndexedStringListEntry()
		{
			_text = "";
			_value = 0;
			_tag = null;
		}

		public IndexedStringListEntry(string text, int value, Object tag)
		{
			_text = text;
			_value = value;
			_tag = tag;
		}

#endregion

		/// <summary>
		/// Override the ToString method to return the text stored in this object
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			return _text;
		}
	}
#endregion IndexedStringListEntry class

#region IndexStringList class

	/// <summary>
	/// This class implements a 2 dimensional list where each item in the list has a numeric
	/// and a string value. This is most useful when for example a list of strings together with
	/// their value needs to be stored say for Scanner Operating Modes and their associated enum
	/// value.
	/// </summary>
	public class IndexedStringList : List<IndexedStringListEntry>
	{

	}
#endregion IndexStringList class
}
