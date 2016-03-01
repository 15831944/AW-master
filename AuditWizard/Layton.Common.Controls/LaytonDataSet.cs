using System;
using System.Collections.Generic;
using System.Text;

namespace Layton.Common.Controls
{
	public class LaytonDataSet : List<LaytonDataRow>
	{
		/// <summary>This is the internal list of columns that have been added to the dataset</summary>
		protected List<LaytonDataColumn> _listColumns = new List<LaytonDataColumn>();

		private string _dataSetTitle;

		/// <summary>
		/// Set the title for this dataset
		/// </summary>
		public string DataSetTitle
		{
			get { return _dataSetTitle; }
			set { _dataSetTitle = value; }
		}

		public List<LaytonDataColumn> Columns
		{
			get { return _listColumns; }
		}

		/// <summary>
		/// Add a new data column to the dataset
		/// </summary>
		/// <param name="newColumn"></param>
		public void AddColumn(LaytonDataColumn newColumn)
		{
			_listColumns.Add(newColumn);
			return;
		}


		/// <summary>
		/// Delete all colimns from the dataset
		/// </summary>
		public void DeleteAllColumns()
		{
			_listColumns.Clear();
		}


		/// <summary>
		/// Delete all rows of data
		/// </summary>
		public void DeleteAllRows()
		{
			this.Clear();
		}

		/// <summary>
		/// Clear the dataset - this clears all columns and rows
		/// </summary>
		new public void Clear()
		{
			base.Clear();
			DeleteAllColumns();
		}


		/// <summary>
		/// Return a count of the number of columns defined
		/// </summary>
		/// <returns></returns>
		public int ColumnCount()
		{
			return _listColumns.Count;
		}


		/// <summary>
		/// Returns the list of column names
		/// </summary>
		/// <returns></returns>
		public List<string> ColumnNames()
		{
			List<string> columnNames = new List<string>();
			foreach (LaytonDataColumn theColumn in _listColumns)
			{
				columnNames.Add(theColumn.Name);
			}
			return columnNames;
		}

		/// <summary>
		/// Find a Data Row in the collection given it's key and return it
		/// </summary>
		/// <param name="key">The key to look for</param>
		/// <returns>The Row if found, null otherwise</returns>		
		public LaytonDataRow FindRow (string key)
		{
			LaytonDataRow returnRow = null;
			foreach (LaytonDataRow row in this)
			{
				if (row.Key == key)
				{
					returnRow = row;
					break;
				}
			}

			return returnRow;
		}

		/// <summary>
		/// Find a column in the collection given it's key and return it
		/// </summary>
		/// <param name="key">The key to look for</param>
		/// <returns>The column if found, null otherwise</returns>		
		public LaytonDataColumn FindColumn(string key)
		{
			LaytonDataColumn returnColumn = null;
			foreach (LaytonDataColumn column in _listColumns)
			{
				if (column.Key == key)
				{
					returnColumn = column;
					break;
				}
			}

			return returnColumn;
		}


		/// <summary>
		/// Find a column in the collection and return it's index
		/// </summary>
		/// <param name="column">The LaytonDataColumn object to find</param>
		/// <returns>index of the column, -1 if not found</returns>		
		public int FindColumn(LaytonDataColumn theColumn)
		{
			for (int isub=0; isub<_listColumns.Count; isub++)
			{
				if (_listColumns[isub].Key == theColumn.Key)
					return isub;
			}

			return -1;
		}


		public LaytonDataRow AddRow(string key)
		{
			LaytonDataRow newRow = new LaytonDataRow(this ,key);
			this.Add(newRow);
			return newRow;
		}
	}





}
