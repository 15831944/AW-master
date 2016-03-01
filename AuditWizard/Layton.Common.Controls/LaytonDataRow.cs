using System;
using System.Collections.Generic;
using System.Text;

namespace Layton.Common.Controls
{
	public class LaytonDataRow
	{
		/// <summary>Get the unique key for this row</summary>
		public string Key
		{
			get { return _key; }
			set { _key = value; }
		}
		/// <summary>Get the Tag value for this row</summary>
		public object Tag
		{
			get { return _tag; }
			set { _tag = value; }
		}

		/// <summary>The parent data set of which this row is part</summary>
		protected LaytonDataSet _parentSet;

		/// <summary>Unique key by which we can find this row again</summary>
		protected string _key;

		/// <summary>The column values for this row</summary>
        private List<object> _values;

		/// <summary>Each value may have a tag associated with it</summary>
        private List<object> _valuesTags;

		/// <summary>User defined tag object per row</summary>
		private object _tag;

		/// <summary>
		/// Primary constructor
		/// </summary>
		/// <param name="parentSet">Parent DataSet</param>
		/// <param name="key">Unique key for this row</param>
		public LaytonDataRow(LaytonDataSet parentSet ,string key)
		{
			_parentSet = parentSet;
			_key = key;

			// Allocate and initialize the values list as we know how many columns we have
            _values = new List<object>(parentSet.ColumnCount());
            _valuesTags = new List<object>(parentSet.ColumnCount());
			//
			for (int isub = 0; isub < parentSet.ColumnCount(); isub++)
			{
                LaytonDataColumn column = _parentSet.Columns[isub];

                if (column.DataType == typeof(string) || column.DataType == typeof(DateTime))
                    _values.Add("");
                else
                    _values.Add(0);
				_valuesTags.Add(0);
			}
		}

		/// <summary>
		/// Set the value for the specified column
		/// </summary>
		/// <param name="column">LaytonDataColumn to identify the column in question</param>
		/// <param name="value"></param>
        public void SetValue(LaytonDataColumn column, object value)
		{
			// Try and find the column in the dataset
			int index = _parentSet.FindColumn(column);
			if (index != -1)
				_values[index] = value;
		}

		/// <summary>
		/// Set the value for the specified column
		/// </summary>
		/// <param name="column"></param>
		/// <param name="value"></param>
        public void SetValue(int column, object value)
		{
			if (column < 0 || column >= _values.Count)
				return;
			_values[column] = value;
		}


		/// <summary>
		/// Set the value for the specified column Tag
		/// </summary>
		/// <param name="column">LaytonDataColumn to identify the column in question</param>
		/// <param name="value"></param>
        public void SetValueTag(LaytonDataColumn column, object value)
		{
			// Try and find the column in the dataset
			int index = _parentSet.FindColumn(column);
			if (index != -1)
				_valuesTags[index] = value;
		}


		/// <summary>
		/// Set the value tag for the specified column
		/// </summary>
		/// <param name="column"></param>
		/// <param name="valuetag"></param>
		public void SetValueTag(int column, object valuetag)
		{
			if (column < 0 || column >= _values.Count)
				return;
			_valuesTags[column] = valuetag;
		}


		/// <summary>
		/// This function is called to increment a value held in this row at the specified column position
		/// If the value is not an integer then this function will fail and will return 0.
		/// </summary>
		/// <param name="column"></param>
		/// <returns></returns>
		public int IncrementValue(int column)
		{
			// Increment any existing value - assuming that the existing value is an INT
			int newCount = 1;
			try
			{
                object intObj = GetValue(column);
                if (intObj is int)
                {
                    newCount = ((int)intObj) + 1;
					SetValue(column, newCount);
                }
			}
			catch (Exception)
			{
				return 0;
			}

			return newCount;
		}

		/// <summary>
		/// This function is called to increment a value held in this row at the specified column position
		/// If the value is not an integer then this function will fail and will return 0.
		/// </summary>
		/// <param name="column"></param>
		/// <returns></returns>
		public int IncrementValue(LaytonDataColumn column)
		{
			// Try and find the column in the dataset
			int index = _parentSet.FindColumn(column);
			if (index != -1)
				return IncrementValue(index);
			return 0;
		}

		/// <summary>
		/// Return the value for the specified column
		/// </summary>
		/// <param name="column"></param>
		/// <returns></returns>
        public object GetValue(int column)
		{
			if (column < 0 || column >= _values.Count)
				return null;
			return _values[column];
		}


		/// <summary>
		/// return the the value for the specified column
		/// </summary>
		/// <param name="column">LaytonDataColumn to identify the column in question</param>
		public object GetValue(LaytonDataColumn column)
		{
			// Try and find the column in the dataset
			int index = _parentSet.FindColumn(column);
			if (index != -1)
				return GetValue(index);
			return "";
		}


		/// <summary>
		/// Return the tag value for the specified column 
		/// </summary>
		/// <param name="column"></param>
		/// <returns></returns>
        public object GetValueTag(int column)
		{
			if (column < 0 || column >= _values.Count)
				return null;
			return _valuesTags[column];
		}


		/// <summary>
		/// return the the value for the specified column
		/// </summary>
		/// <param name="column">LaytonDataColumn to identify the column in question</param>
        public object GetValueTag(LaytonDataColumn column)
		{
			// Try and find the column in the dataset
			int index = _parentSet.FindColumn(column);
			if (index != -1)
				return GetValueTag(index);
			return "";
		}

	}


}
