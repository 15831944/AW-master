using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
//

namespace Layton.AuditWizard.DataAccess
{
	#region AuditedItem Class

	/// <summary>
	/// The 'AuditedItem' refers to a single item read from an Audit Data File.  This item may be either
	/// a hardware or a system element
	/// </summary>
	public class AuditedItem
	{
#region enumerations
		public enum eDATATYPE { text ,numeric ,date ,ipaddress };
#endregion enumerations

#region Data
		private Int64		_itemID;
		private int			_assetID;
		private string		_category;
		private string		_name;
		private string		_value;
		private string		_icon;
		private string		_displayUnits;
		private eDATATYPE	_datatype;
		private bool		_historied;
		private bool		_grouped;

        protected static readonly log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
 
#endregion Data

#region Properties

		/// <summary>This is the database index of the audited item</summary>
        public Int64 ItemID
		{
		  get { return _itemID; }
		  set { _itemID = value; }
		}

		/// <summary>This is the database index of the asset to which this item is related</summary>
		public int AssetID
		{
		  get { return _assetID; }
		  set { _assetID = value; }
		}

		/// <summary>Type string - this is the full path to the item audited</summary>
		public string Category
		{
			get { return _category; }
			set { _category = value; }
		}

		/// <summary>Name of the audited data item (for example 'CPU Type')</summary>
		public string Name
		{
			get { return _name; }
			set { _name = value; }
		}

		/// <summary>Value of the above named audited data item</summary>
		public string Value
		{
			get { return _value; }
			set { _value = value; }
		}

		/// <summary>Textual units representation displayed with the data value - for example 'Mhz'</summary>
		public string DisplayUnits
		{
			get { return _displayUnits; }
			set { _displayUnits = value; }
		}

		/// <summary>Name of the icon file to use with this item</summary>
		public string Icon
		{
			get { return _icon; }
			set { _icon = value; }
		}

		/// <summary>Datatype of the value audited - see above for enumeration</summary>
		public eDATATYPE Datatype
		{
			get { return _datatype; }
			set { _datatype = value; }
		}

		/// <summary>Determines whether or not this item requires history to be generated</summary>
		public bool Historied
		{
			get { return _historied; }
			set { _historied = value; }
		}

		/// <summary>Determines whether or not this item is Grouped - grouped prevents the item
		/// from being expanded in a tree view and forces items of the same name to be grouped together 
		/// in list views</summary>
		public bool Grouped
		{
			get { return _grouped; }
			set { _grouped = value; }
		}

#endregion Properties

#region Constructor

        public AuditedItem()
        {
        }

		public AuditedItem(string category, string name, string value, string units, eDATATYPE datatype ,bool historied)
		{
			_itemID = 0;
			_assetID = 0;
			_category = category;
			_name = name;
			_value = value;
			_displayUnits = units;
			_datatype = datatype;
			_historied = historied;
			_grouped = false;
		}

		public AuditedItem(int itemID, int assetID, string category, string name, string value, string units, eDATATYPE datatype, bool historied)
		{
			_itemID = itemID;
			_assetID = assetID;
			_category = category;
			_name = name;
			_value = value;
			_displayUnits = units;
			_datatype = datatype;
			_historied = historied;
			_grouped = false;
		}

		public AuditedItem(DataRow dataRow)
		{
			try
			{
				// We may be creating a category or an item so should determine which here
				if (dataRow.Table.Columns.Contains("_AUDITEDITEMID"))
				{
					this.ItemID = Convert.ToInt64(dataRow["_AUDITEDITEMID"]);
					this.AssetID = (int)dataRow["_ASSETID"];
					this.Category = (string)dataRow["_CATEGORY"];
					this.Name = (string)dataRow["_NAME"];
					this.DisplayUnits = (string)dataRow["_DISPLAY_UNITS"];
					this.Icon = (string)dataRow["_ICON"];
					this.Datatype = (eDATATYPE)((int)dataRow["_DATATYPE"]);
					this.Historied = (bool)dataRow["_HISTORIED"];
					this.Grouped = (bool)dataRow["_GROUPED"];
					this.Value = (string)dataRow["_VALUE"];
				}
				else
				{
					this.Category = (string)dataRow["_CATEGORY"];
					this.Icon = (string)dataRow["_ICON"];
				}
			}
			catch (Exception ex)
			{
                logger.Error(ex.Message);
				Utility.DisplayErrorMessage("Exception occurred creating an AUDITEDITEM Object, please check database schema.  The message was " + ex.Message);
			}
		}

#endregion Constructor

#region Methods
		public static eDATATYPE TranslateDatatype(string dataType)
		{
			switch (dataType)
			{
				case "text":
					return eDATATYPE.text;
				case "numeric":
					return eDATATYPE.numeric;
				case "date":
					return eDATATYPE.date;
				case "ipaddress":
					return eDATATYPE.ipaddress;
				default:
					return eDATATYPE.text;
			}
		}


		public string ShortCategory()
		{
			int delimiter = _category.LastIndexOf('|');
			if (delimiter == -1)
			{
				return _category;
			}
			else
			{
				return _category.Substring(delimiter + 1);
			}
		}


#endregion Methods
	}
	#endregion AuditedItem Class

	#region AuditedItemList Class

	public class AuditedItemList : List<AuditedItem>
	{
		public AuditedItemList()
		{
		}

		/// <summary>
		/// This constructor takes a data table with a list of asset groups which will be added
		/// </summary>
		/// <param name="locations"></param>
		public AuditedItemList(DataTable dataTable)
		{
			foreach (DataRow row in dataTable.Rows)
			{
				this.Add(new AuditedItem(row));
			}
		}

		public List<AuditedItem> GetItemsInCategory(string categoryName)
		{
			List<AuditedItem> listItems = new List<AuditedItem>();

			foreach (AuditedItem item in this)
			{
				if (item.Category == categoryName)
					listItems.Add(item);
			}
			return listItems;
		}



		/// <summary>
		/// This function returns entries for the immediate children of the specified category.  This is most often
		/// used with grouping where the category could be say 'System|Local User Accounts' and we want to return single 
		/// entries for each Local User Account
		/// </summary>
		/// <param name="categoryName"></param>
		/// <returns></returns>
		public List<string> GetChildrenInCategory(string categoryName)
		{
			List<string> listChildNames = new List<string>();

			// Loop through the audited items and find any children of this category - note we only add 
			foreach (AuditedItem item in this)
			{
				if (item.Category.StartsWith(categoryName))
				{
					if (!listChildNames.Contains(item.Category))
						listChildNames.Add(item.Category);
				}
			}
			return listChildNames;
		}


		/// <summary>
		/// This function determines whether or not items beneath the specified category should be grouped
		/// It does this by analysing the first entry which begins with the category name specified.
		/// </summary>
		/// <param name="categoryName"></param>
		/// <returns></returns>
		public bool IsGrouped(string categoryName)
		{
			// Count the number of separators in the parent category
			int count1 = Utility.CharCount(categoryName ,"|");

			// Now loop through the sub-items found
			foreach (AuditedItem item in this)
			{
				if (item.Category.StartsWith(categoryName))
				{
					// Get the number of delimiters in the suspect item
					int count2 = Utility.CharCount(item.Category, "|");

					// To be a child of the specified category, the string must have 1 additonal separator
					if (count2 == (count1 + 1))
						return item.Grouped;
				}
			}
			return false;
		}



		/// <summary>
		/// Search the list for an item with the specified category and name
		/// </summary>
		/// <param name="category">Full category string to search for</param>
		/// <param name="name">Name of the item within the specified category to search for</param>
		/// <returns></returns>
		public AuditedItem FindItemByName(string category, string name)
		{
			foreach (AuditedItem item in this)
			{
				if ((item.Name == name) && (item.Category == category))
					return item;
			}

			return null;
		}
	}

	#endregion AuditedItemList Class


}
