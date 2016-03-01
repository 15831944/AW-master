using System;
using System.Collections.Generic;
using System.Text;

namespace Layton.Common.Controls
{
	public class LaytonDataColumn
	{
		public string Name
		{
			get { return _name; }
			set { _name = value; }
		}

		public string Key
		{
			get { return _key; }
			set { _key = value; }
		}

		public object Tag
		{
			get { return _tag; }
			set { _tag = value; }
		}

		private string _name;
		private string _key;
		private object _tag;
        private Type _datatype;

        // Summary:
        //     Column's data type.
        public Type DataType 
        { 
            get { return _datatype; } 
            set { _datatype = value; } 
        }

		/// <summary>
		/// Default constructor
		/// </summary>
		public LaytonDataColumn()
		{
			_name = "";
			_key = "";
			_tag = null;
            _datatype = typeof(string);                 // Assume String type as this is the most generic
		}

		/// <summary>
		/// Over-ridden constructor
		/// </summary>
		/// <param name="name"></param>
		/// <param name="value"></param>
		public LaytonDataColumn(string name, string key)
		{
			_name = name;
			_key = key;
            _datatype = typeof(string);                 // Assume String type as this is the most generic
        }

        /// <summary>
        /// Over-ridden constructor
        /// </summary>
        /// <param name="name"></param>
        /// <param name="key">Key value (must be unique)</param>
        /// <param name="tag">Object tag</param>
        /// 
        public LaytonDataColumn(string name, string key, object tag)
        {
            _name = name;
            _key = key;
            _tag = tag;
            _datatype = typeof(string);                 // Assume String type as this is the most generic
        }

        /// <summary>
        /// Over-ridden constructor
        /// </summary>
        /// <param name="name"></param>
        /// <param name="key">Key value (must be unique)</param>
        /// <param name="tag">Object tag</param>
        /// 
        public LaytonDataColumn(string name, string key, object tag ,Type datatype)
        {
            _name = name;
            _key = key;
            _tag = tag;
            _datatype = datatype;
        }
    }
}
