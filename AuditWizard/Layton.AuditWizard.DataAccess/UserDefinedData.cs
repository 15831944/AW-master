using System;
using System.Collections.Generic;
using System.Data;
using System.Xml.Serialization;

namespace Layton.AuditWizard.DataAccess
{
    #region UserDataCategory

    public class UserDataCategory : List<UserDataField>, IComparable<UserDataCategory>
    {
        #region Data

        protected static readonly log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>This is the scope of the User Defined Data Field Category</summary>
        public enum SCOPE { Any = -1, any = -1, Asset = 0, asset = 0, Application = 1, application = 1, NonInteractiveAsset }

        private int _categoryID;
        private string _name;
        private int _appliesTo;
        private int _tabOrder;
        private string _icon;
        private string _appliesToName;
        private SCOPE _scope;
        private object _tag;

        // We store asset types so that the 'Applies To' function can function effeciently
        AssetTypeList _listAssetTypes;

        #endregion Data

        #region Properties

        public int CategoryID
        {
            get { return _categoryID; }
            set { _categoryID = value; }
        }

        /// <summary>
        /// This is the name of the category
        /// </summary>
        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        /// <summary>
        /// This is ID of the asset type or asset category to which this user defined category is associated
        /// </summary>
        public int AppliesTo
        {
            get { return _appliesTo; }
        }

        public string AppliesToName
        {
            get { return _appliesToName; }
        }


        /// <summary>
        /// This is the order in which the Categories will be displayed either within AuditWizard or within
        /// an interactive scanner.
        /// </summary>
        public int TabOrder
        {
            get { return _tabOrder; }
            set { _tabOrder = value; }
        }

        /// <summary>
        /// This is the icon which will be displayed for the category
        /// </summary>
        public string Icon
        {
            get { return _icon; }
            set { _icon = value; }
        }


        public void AppliesToDetails(int appliesToId, string appliesToName)
        {
            _appliesTo = appliesToId;
            _appliesToName = appliesToName;
        }

        public SCOPE Scope
        {
            get { return _scope; }
            set { _scope = value; }
        }

        public string ScopeAsString
        {
            get
            {
                switch ((int)_scope)
                {
                    case -1:
                        return "All";
                    case 0:
                        return "Asset";
                    case 1:
                        return "Application";
                    case 2:
                        return "Asset Non-Interactive";
                    default:
                        return "Unknown Scope";
                }
            }
        }

        public object Tag
        {
            get { return _tag; }
            set { _tag = value; }
        }

        #endregion Properties

        #region Constructor

        public UserDataCategory()
        {
            _categoryID = 0;
            _name = "";
            _appliesTo = 0;
            _appliesToName = "";
            _tabOrder = -1;
            _icon = "userdata.png";
            _scope = SCOPE.Asset;
            _tag = null;
        }

        public UserDataCategory(UserDataCategory other)
        {
            _categoryID = other._categoryID;
            _name = other._name;
            _appliesTo = other._appliesTo;
            _appliesToName = other._appliesToName;
            _tabOrder = other._tabOrder;
            _icon = other._icon;
            _scope = other._scope;
            _tag = other._tag;

            // Copy the associated fields also
            foreach (UserDataField field in other)
            {
                this.Add(field);
            }
        }

        public UserDataCategory(SCOPE scope)
            : this()
        {
            _scope = scope;
        }

        public UserDataCategory(DataRow dataRow)
            : this()
        {
            try
            {
                _categoryID = (int)dataRow["_USERDEFID"];
                _name = (string)dataRow["_NAME"];
                _appliesTo = (int)dataRow["_APPLIESTO"];
                _tabOrder = (int)dataRow["_TABORDER"];
                _appliesTo = (int)dataRow["_APPLIESTO"];
                _scope = (UserDataCategory.SCOPE)((int)dataRow["_SCOPE"]);

                if (dataRow.IsNull("ASSETTYPENAME"))
                    _appliesToName = "";
                else
                    _appliesToName = (string)dataRow["ASSETTYPENAME"];


                // Only over-write the default icon if we actually had something specified
                string icon = (string)dataRow["_ICON"];
                if (icon != "")
                    _icon = icon;
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message);
                Utility.DisplayErrorMessage("Exception occurred creating an USERDATACATEGORY Object, please check database schema.  The message was " + ex.Message);
            }
        }


        /// <summary>
        /// Add this User Data Category to the database (or possibly update an existing item)
        /// </summary>
        /// <returns></returns>
        public int Add()
        {
            UserDataDefinitionsDAO lwDataAccess = new UserDataDefinitionsDAO();
            if (CategoryID == 0)
                _categoryID = lwDataAccess.UserDataCategoryAdd(this);
            else
                lwDataAccess.UserDataCategoryUpdate(this);
            return 0;
        }


        public bool Delete()
        {
            UserDataDefinitionsDAO lwDataAccess = new UserDataDefinitionsDAO();

            foreach (UserDataField dataField in this)
            {
                if (!lwDataAccess.UserDataDefinitionFieldDelete(dataField.FieldID))
                    return false;
            }

            return lwDataAccess.UserDataDefinitionCategoryDelete(CategoryID);
        }

        #endregion Constructor

        #region Methods

        /// <summary>
        /// Does this user data category apply to this asset?
        /// </summary>
        /// <param name="asset"></param>
        /// <returns>true if yes, false otherwise</returns>
        public bool CategoryAppliesTo(Asset asset)
        {
            if (_listAssetTypes == null)
            {
                _listAssetTypes = new AssetTypeList();
                _listAssetTypes.Populate();
            }

            // Is this category specific to an asset type?
            if (AppliesTo != 0)
            {
                // OK - does the category apply specifically to this asset type?
                if (AppliesTo != asset.AssetTypeID)
                {
                    // No - we need to get the parent category of this asset type and check that also then
                    AssetType parentType = _listAssetTypes.FindByName(asset.TypeAsString);
                    if ((parentType == null) || (_appliesTo != parentType.ParentID))
                        return false;
                }
            }

            // User data category applies to this type of asset so return true
            return true;
        }


        /// <summary>
        /// Recover the values for the fields within this category for the specified asset
        /// </summary>
        /// <param name="asset"></param>
        public void GetValuesFor(int itemID)
        {
            UserDataDefinitionsDAO lwDataAccess = new UserDataDefinitionsDAO();
            DataTable table = lwDataAccess.EnumerateUserDataValues(_scope, itemID, _categoryID);

            foreach (DataRow row in table.Rows)
            {
                // Get the value and ID of the field
                string value = (string)row["_VALUE"];
                int userDefID = (int)row["_USERDEFID"];
                SetValueFor(itemID, userDefID, value, false);
            }
        }

        /// <summary>
        /// Set the field with the specified ID to the specified value
        /// </summary>
        /// <param name="userDefID"></param>
        /// <param name="value"></param>
        public void SetValueFor(int itemID, int userDefID, string value, bool updateDatabase)
        {
            foreach (UserDataField field in this)
            {
                // Is this the specific field/
                if (field.FieldID == userDefID)
                {
                    field.SetValueFor(itemID, value, updateDatabase);
                    break;
                }
            }
        }



        /// <summary>
        /// Find and return a UsedDataField given it's name
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public UserDataField FindField(string name)
        {
            foreach (UserDataField field in this)
            {
                if (field.Name == name)
                    return field;
            }

            return null;
        }


        /// <summary>
        /// Over0-ride ToString to return the name of the category
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return Name;
        }

        #endregion Methods

        // We sort based on tab order
        public int CompareTo(UserDataCategory obj)
        {
            return _tabOrder.CompareTo(obj._tabOrder);
        }

    }

    #endregion UserDataCategory

    #region UserDataCategoryList

    public class UserDataCategoryList : List<UserDataCategory>
    {
        private UserDataCategory.SCOPE _scope = UserDataCategory.SCOPE.Asset;
        private bool _populated = false;

        public UserDataCategoryList()
        {
        }

        public UserDataCategory.SCOPE Scope
        {
            get { return _scope; }
            set { this.Clear(); _scope = value; }
        }

        public UserDataCategoryList(UserDataCategory.SCOPE scope)
        {
            _scope = scope;
        }

        /// <summary>
        /// Populate the list with all currently defined user defined data categories and associated fields
        /// </summary>
        /// <returns></returns>
        public int Populate()
        {
            // Ensure that the list is empty initially
            Clear();

            UserDataDefinitionsDAO lwDataAccess = new UserDataDefinitionsDAO();
            DataTable table = lwDataAccess.EnumerateUserDataDefinitions(_scope);

            // Iterate through the returned table and add to our internal list
            Clear();
            foreach (DataRow thisRow in table.Rows)
            {
                AddItem(thisRow);
            }

            // Flag as having populated the list
            _populated = true;

            return Count;
        }


        /// <summary>
        /// Add an item to the list given a data row.
        /// </summary>
        /// <param name="row"></param>
        public void AddItem(DataRow row)
        {
            // Is this a category or a field?
            bool isCategory = row.IsNull("_PARENTID");
            if (isCategory)
                AddCategory(row);
            else
                AddField(row);
        }


        /// <summary>
        /// Add a new User Defined Data Type to the list
        /// </summary>
        /// <param name="row"></param>
        public void AddCategory(DataRow row)
        {
            // First of all do we already have a definition for this category?
            // If we do then we simply return as no more to do
            UserDataCategory category = FindCategory((string)row["_NAME"]);
            if (category != null)
                return;

            // No so add it
            category = new UserDataCategory(row);
            this.Add(category);
        }


        /// <summary>
        /// Add a new User Defined data field (to an existing category)
        /// </summary>
        /// <param name="row"></param>
        public void AddField(DataRow row)
        {
            // Can we find a definition for the associated category?  
            // If not then we fail as we cannot add a field to a non-existant category
            UserDataCategory category = FindCategory((int)row["_PARENTID"]);
            if (category == null)
                throw new Exception("Invalid User Defined Data Type with index [" + ((int)row["_PARENTID"]).ToString() + "] Associated with field [" + ((int)row["_USERDEFID"]).ToString() + "]");
            UserDataField field = new UserDataField(row);
            category.Add(field);
        }



        /// <summary>
        /// Find and return a Used Defined Data Type given it's name
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public UserDataCategory FindCategory(string name)
        {
            foreach (UserDataCategory category in this)
            {
                if (category.Name == name)
                    return category;
            }

            return null;
        }


        /// <summary>
        /// Find and return a Used Defined Data Type given it's index
        /// </summary>
        /// <param name="categoryID">Index of the category to find</param>
        /// <returns></returns>
        public UserDataCategory FindCategory(int categoryID)
        {
            foreach (UserDataCategory category in this)
            {
                if (category.CategoryID == categoryID)
                    return category;
            }

            return null;
        }
    }

    #endregion UserDataCategoryList

    #region UserDataField

    public class UserDataField : IComparable<UserDataField>
    {
        public enum FieldCase { Any, Upper, Lower, Title };

        // Bug #639
        public enum FieldType
        {
            Text = 0, text = 0, 
            Number = 1, number = 1,
            Picklist = 2, picklist = 2,
            Environment = 3, environment = 3, 
            Registry = 4, registry = 4, 
            Date = 5, date = 5, 
            Currency = 6, currency = 6
        };

        #region Data

        protected static readonly log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private int _fieldID;
        private string _name;
        private int _parentID;
        private bool _isMandatory;
        private FieldType _type;
        private string _value1;
        private string _value2;
        private int _tabOrder;
        private object _tag;
        private bool _populated;
        private UserDataCategory.SCOPE _parentScope;
        private string _userDateFieldCase;

        // Static dictionary which can be used to determine what field types are supported
        static SerializableDictionary<FieldType, string> _fieldTypes = new SerializableDictionary<FieldType, string>();

        // Static dictionary of field input cases
        static SerializableDictionary<FieldCase, string> _fieldCase = new SerializableDictionary<FieldCase, string>();

        // This dictionary is used to store values for this UserDataField for asset(s)
        SerializableDictionary<int, string> _listCurrentValues = new SerializableDictionary<int, string>();

        #endregion Data

        #region Properties

        public int FieldID
        {
            get { return _fieldID; }
            set { _fieldID = value; }
        }

        /// <summary>
        /// This is the name of the field
        /// </summary>
        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        /// <summary>
        /// This is the category of the alert
        /// </summary>
        public bool IsCategory
        {
            get { return (ParentID == 0); }
        }

        /// <summary>
        /// This is the ID of the Parent CATEGORY
        /// </summary>
        public int ParentID
        {
            get { return _parentID; }
            set { _parentID = value; }
        }

        /// <summary>
        /// This returns a flag to indicate whether or not this field is mandatory or not
        /// </summary>
        public bool IsMandatory
        {
            get { return _isMandatory; }
            set { _isMandatory = value; }
        }


        /// <summary>
        /// The case in which text strings will be entered
        /// </summary>
        public FieldType Type
        {
            get { return _type; }
            set
            {
                _type = value;
                if (_type == FieldType.Number)
                {
                    _value1 = "0";
                    _value2 = "999";
                }
                else
                {
                    _value1 = "";
                    _value2 = "";
                }
            }
        }

        public string TypeAsString
        {
            get { return _fieldTypes[_type]; }
        }

        /// <summary>
        /// This is the order in which the Categories will be displayed either within AuditWizard or within
        /// an interactive scanner.
        /// </summary>
        public int TabOrder
        {
            get { return _tabOrder; }
            set { _tabOrder = value; }
        }


        /// <summary>
        /// Return a dictionary containing the possible types of user data field supported
        /// </summary>
        /// <returns></returns>
        public SerializableDictionary<FieldType, string> FieldTypes
        {
            get { return _fieldTypes; }
        }


        /// <summary>
        /// Return a dictionary of the possible input cases
        /// </summary>
        /// <returns></returns>
        public SerializableDictionary<FieldCase, string> FieldInputCases
        {
            get { return _fieldCase; }
        }


        /// <summary>
        /// Return the maximum permitted text string length, 0 is no limit
        /// </summary>
        public int Length
        {
            get { return GetNumericValue(Value1, 0); }
            set { Value1 = value.ToString(); }
        }


        /// <summary>
        /// Field Case is encoded into Value2
        /// </summary>
        public string InputCase
        {
            //get { return (FieldCase)GetNumericValue(Value2, (int)FieldCase.any); }
            get { return Value2; }
            set { Value2 = value.ToString(); }
        }

        /// <summary>
        /// Field Case is encoded into Value2
        /// </summary>
        public string UserDataInputCase
        {
            get { return Convert.ToString(GetNumericValue(Value2, (int)FieldCase.Any)); }
            set { Value2 = _userDateFieldCase; }
            //set { Value2 = value.ToString(); }
        }


        /// <summary>
        /// Return a text version of the current field input case
        /// </summary>
        public string InputCaseAsString
        {
            //get { return _fieldCase[InputCase]; }
            get { return InputCase; }
        }


        /// <summary>
        /// Return the Minimum numeric value permitted, if not specified return 0;
        /// </summary>
        public int MinimumValue
        {
            get { return GetNumericValue(Value1, 0); }
            set { Value1 = value.ToString(); }
        }

        /// <summary>
        /// Return the Maximum numeric value permitted, if not specified return 0;
        /// </summary>
        public int MaximumValue
        {
            get { return GetNumericValue(Value2, 0); }
            set { Value2 = value.ToString(); }
        }


        /// <summary>
        /// Accessor for Picklist (stored in value1)
        /// </summary>
        public string Picklist
        {
            get { return Value1; }
            set { Value1 = value; }
        }

        /// <summary>
        /// Accessor for Environment Variable (stored in value2)
        /// </summary>
        public string EnvironmentVariable
        {
            get { return Value2; }
            set { Value2 = value; }
        }


        /// <summary>
        /// Accessor for Registry Key (stored in value1)
        /// </summary>
        public string RegistryKey
        {
            get { return Value1; }
            set { Value1 = value; }
        }

        /// <summary>
        /// Accessor for Value1
        /// </summary>
        public string Value1
        {
            get { return _value1; }
            set { _value1 = value; }
        }

        /// <summary>
        /// Accessor for Value2 
        /// </summary>
        public string Value2
        {
            get { return _value2; }
            set { _value2 = value; }
        }

        /// <summary>
        /// Accessor for Registry Value (stored in value2)
        /// </summary>
        public string RegistryValue
        {
            get { return Value2; }
            set { Value2 = value; }
        }

        /// <summary>
        /// Gelper function to convert a string to a numeric handling defaults
        /// </summary>
        /// <param name="valueString"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        protected int GetNumericValue(string valueString, int defaultValue)
        {
            int returnValue = defaultValue;
            try
            {
                if (valueString == "")
                    valueString = defaultValue.ToString();
                returnValue = Convert.ToInt32(valueString);
            }
            catch (Exception)
            { }

            return returnValue;
        }

        /// <summary>
        /// Tag field to store what-ever you like and associate with this field
        /// </summary>
        public object Tag
        {
            get { return _tag; }
            set { _tag = value; }
        }


        public UserDataCategory.SCOPE ParentScope
        {
            get { return _parentScope; }
            set { _parentScope = value; }
        }

        #region Current Value Accessors

        public SerializableDictionary<int, string> ListCurrentValues
        {
            get { return _listCurrentValues; }
        }

        /// <summary>
        /// Return the numeric value
        /// </summary>
        public decimal NumericValue(int assetID)
        {
            // Recover (any) value specified for this asset
            string value = GetValueFor(assetID);
            return (value == AWMiscStrings.NoValueFound) ? 0 : Convert.ToDecimal(value);
        }

        public void NumericValue(int assetID, int value)
        {
            string stringValue = value.ToString();
            SetValueFor(assetID, stringValue, false);
        }


        /// <summary>
        /// Set/Get Boolean value
        /// </summary>
        public bool BooleanValue(int assetID)
        {
            // Recover (any) value specified for this asset
            string value = GetValueFor(assetID);
            return (value == "True");
        }

        public void BooleanValue(int assetID, bool value)
        {
            SetValueFor(assetID, (value) ? "True" : "False", false);
        }

        public string DateValue(int assetID)
        {
            return GetValueFor(assetID);
        }

        public void DateValue(int assetID, string value)
        {
            SetValueFor(assetID, value, false);
        }


        #endregion Current Value Accessors

        #endregion Properties

        #region Constructor

        public UserDataField()
        {
            try
            {
                _fieldID = 0;
                _name = "";
                _parentID = 0;
                _isMandatory = false;
                _type = FieldType.Text;
                _value1 = "";
                _value2 = "";
                _tabOrder = 0;
                _parentScope = UserDataCategory.SCOPE.Asset;

                // Initialize the field types dictionary
                _fieldTypes.Clear();
                _fieldTypes.Add(FieldType.Text, "Text");
                _fieldTypes.Add(FieldType.Number, "Numeric");
                _fieldTypes.Add(FieldType.Picklist, "PickList");
                //_fieldTypes.Add(FieldType.boolean, "Boolean");
                _fieldTypes.Add(FieldType.Environment, "Environment Variable");
                _fieldTypes.Add(FieldType.Registry, "Registry Key");
                _fieldTypes.Add(FieldType.Date, "Date");
                _fieldTypes.Add(FieldType.Currency, "Currency");
                //
                _fieldCase.Clear();
                _fieldCase.Add(FieldCase.Any, "any");
                _fieldCase.Add(FieldCase.Lower, "lower");
                _fieldCase.Add(FieldCase.Title, "TitleCase");
                _fieldCase.Add(FieldCase.Upper, "UPPER");
                //
                _listCurrentValues.Clear();
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message);
            }
        }

        public UserDataField(DataRow dataRow)
            : this()
        {
            try
            {
                _fieldID = (int)dataRow["_USERDEFID"];
                _name = (string)dataRow["_NAME"];
                _parentID = (int)dataRow["_PARENTID"];
                _isMandatory = (bool)dataRow["_ISMANDATORY"];
                _type = (FieldType)dataRow["_TYPE"];
                _value1 = (string)dataRow["_VALUE1"];
                _value2 = (string)dataRow["_VALUE2"];
                _tabOrder = (int)dataRow["_TABORDER"];
                _parentScope = (UserDataCategory.SCOPE)dataRow["_SCOPE"];
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message);
                Utility.DisplayErrorMessage("Exception occurred creating a USERDATAFIELD Object, please check database schema.  The message was " + ex.Message);
            }
        }


        /// <summary>
        /// Copy constructor
        /// </summary>
        /// <param name="other"></param>
        public UserDataField(UserDataField other)
        {
            _fieldID = other._fieldID;
            _name = other._name;
            _parentID = other._parentID;
            _isMandatory = other._isMandatory;
            _type = other._type;
            _value1 = other._value1;
            _value2 = other._value2;
            _tabOrder = other._tabOrder;
            _parentScope = other._parentScope;
        }


        /// <summary>
        /// Equality test - check to see if this instance matches another
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            if (obj != null && obj.GetType().Equals(this.GetType()))
            {
                UserDataField other = obj as UserDataField;

                if ((object)other != null)
                {
                    bool equality;
                    equality = other._fieldID == _fieldID
                            && other._name == _name
                            && other._parentID == _parentID
                            && other._isMandatory == _isMandatory
                            && other._type == _type
                            && other._value1 == _value1
                            && other._value2 == _value2
                            && other._parentScope == _parentScope
                            && other._tabOrder == _tabOrder;
                    return equality;
                }
            }
            return base.Equals(obj);
        }


        #endregion Constructor

        public override string ToString()
        {
            return Name;
        }

        // We sort based on tab order
        public int CompareTo(UserDataField obj)
        {
            return _tabOrder.CompareTo(obj._tabOrder);
        }

        #region Methods


        /// <summary>
        /// Add this User Data Field to the database (or possibly update an existing item)
        /// </summary>
        /// <returns></returns>
        public int Add()
        {
            UserDataDefinitionsDAO lUserDataDefinitionsDAO = new UserDataDefinitionsDAO();
            if (FieldID == 0)
                _fieldID = lUserDataDefinitionsDAO.UserDataFieldAdd(this);
            else
            {
                // editing a user data field
                lUserDataDefinitionsDAO.UserDataFieldUpdate(this);
                lUserDataDefinitionsDAO.UpdateUserDataValueParentType(FieldID, (int)ParentScope);
            }
            return 0;
        }

        public int Update()
        {
            return Add();
        }

        /// <summary>
        /// Delete the current User Defined Field / category from the database
        /// </summary>
        public bool Delete()
        {
            return new UserDataDefinitionsDAO().UserDataDefinitionFieldDelete(_fieldID);
        }



        /// <summary>
        /// Format a descriptiove string for this data field
        /// </summary>
        /// <returns></returns>
        public string Description()
        {
            List<string> fields = new List<string>();

            // field type?
            switch ((int)_type)
            {
                case (int)FieldType.Text:

                    switch (InputCase)
                    {
                        case "upper":
                            fields.Add("Upper Case");
                            break;
                        case "lower":
                            fields.Add("Lower Case");
                            break;
                        case "title":
                            fields.Add("Title Case");
                            break;
                    }
                    break;

                case (int)FieldType.Number:
                    break;

                case (int)FieldType.Picklist:
                    fields.Add("Picklist " + Picklist);
                    break;

                //case (int)FieldType.boolean:
                //	break;

                case (int)FieldType.Environment:
                    fields.Add("Environment Variable '" + this.EnvironmentVariable + "'");
                    break;

                case (int)FieldType.Registry:
                    fields.Add(RegistryKey + "\\" + RegistryValue);
                    break;

                case (int)FieldType.Date:
                	break;

                case (int)FieldType.Currency:
                    break;

                default:
                    break;
            }

            // Interactive or non-interactive
            if (ParentScope == UserDataCategory.SCOPE.Asset)
                fields.Add("Interactive");

            // Mandatory or optional 
            if (IsMandatory)
                fields.Add("Mandatory");

            // now concatenate all the options, adding commas where necessary
            string description = "";
            foreach (string phrase in fields)
            {
                if (description != "")
                    description += ", ";
                description += phrase;
            }

            return description;
        }


        /// <summary>
        /// Add a value for this data field for the specified asset
        /// </summary>
        /// <param name="itemID"></param>
        /// <param name="value"></param>
        /// <param name="updateDatabase"></param>
        public void SetValueFor(int itemID, string value, bool updateDatabase)
        {
            if (_listCurrentValues.ContainsKey(itemID))
                _listCurrentValues[itemID] = value;
            else
                _listCurrentValues.Add(itemID, value);

            // Update the database also if requested
            if (updateDatabase)
            {
                UserDataDefinitionsDAO lwDataAccess = new UserDataDefinitionsDAO();
                lwDataAccess.UserDataUpdateValue(_parentScope, itemID, _fieldID, value);
            }
        }



        /// <summary>
        /// Recover ALL stored values for this field
        /// </summary>
        public void PopulateValues()
        {
            UserDataDefinitionsDAO lwDataAccess = new UserDataDefinitionsDAO();
            DataTable table = lwDataAccess.EnumerateUserDataFieldValues(_fieldID);

            foreach (DataRow row in table.Rows)
            {
                // Get the value and ID of the field
                int assetID = (int)row["_PARENTID"];
                string value = (string)row["_VALUE"];
                SetValueFor(assetID, value, false);
            }

            _populated = true;
        }

        /// <summary>
        /// Return (any) stored values for this asset
        /// </summary>
        /// <param name="itemID"></param>
        /// <returns></returns>
        public string GetValueFor(int itemID)
        {
            PopulateValues();
            return _listCurrentValues.ContainsKey(itemID) ? _listCurrentValues[itemID] : "";
        }

        #endregion Methods

        public class SerializableDictionary<TKey, TValue> : Dictionary<TKey, TValue>, IXmlSerializable
        {

            #region IXmlSerializable Members

            public System.Xml.Schema.XmlSchema GetSchema()
            {
                return null;
            }



            public void ReadXml(System.Xml.XmlReader reader)
            {
                XmlSerializer keySerializer = new XmlSerializer(typeof(TKey));
                XmlSerializer valueSerializer = new XmlSerializer(typeof(TValue));

                bool wasEmpty = reader.IsEmptyElement;
                reader.Read();

                if (wasEmpty)
                    return;

                while (reader.NodeType != System.Xml.XmlNodeType.EndElement)
                {
                    reader.ReadStartElement("item");
                    reader.ReadStartElement("key");
                    TKey key = (TKey)keySerializer.Deserialize(reader);
                    reader.ReadEndElement();
                    reader.ReadStartElement("value");
                    TValue value = (TValue)valueSerializer.Deserialize(reader);
                    reader.ReadEndElement();
                    this.Add(key, value);
                    reader.ReadEndElement();
                    reader.MoveToContent();
                }

                reader.ReadEndElement();
            }

            public void WriteXml(System.Xml.XmlWriter writer)
            {

                XmlSerializer keySerializer = new XmlSerializer(typeof(TKey));
                XmlSerializer valueSerializer = new XmlSerializer(typeof(TValue));

                foreach (TKey key in this.Keys)
                {
                    writer.WriteStartElement("item");
                    writer.WriteStartElement("key");
                    keySerializer.Serialize(writer, key);
                    writer.WriteEndElement();

                    writer.WriteStartElement("value");

                    TValue value = this[key];
                    valueSerializer.Serialize(writer, value);
                    writer.WriteEndElement();
                    writer.WriteEndElement();
                }
            }
        }
            #endregion

    }

    #endregion UserDataField

    #region UserDataCategories

    public class UserDataCategories
    {
        #region Data

        /// <summary>Name assigned to this alert definition</summary>
        private string _name;

        /// <summary>The list of alert triggers</summary>
        private UserDataCategory _userFields = new UserDataCategory();

        #endregion Data

        #region Properties

        /// <summary>Name of the Alert Definition</summary>
        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        public UserDataCategory UserFields
        {
            get { return _userFields; }
            set { _userFields = value; }
        }

        #endregion Properties

        #region Constructor

        public UserDataCategories()
        {
        }

        #endregion Constructor
    }

    #endregion


}
