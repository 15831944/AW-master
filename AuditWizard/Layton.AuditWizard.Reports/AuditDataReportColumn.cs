using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
//
using Layton.Common.Controls;
//
using Layton.AuditWizard.DataAccess;

namespace Layton.AuditWizard.Reports
{

    #region AuditDataReportColumnValues Class

    public class AuditDataReportColumnValue
    {
        #region Data

        private object _dataValue;
        private string _assetName;
        private int _assetID;
        private string _location;

        #endregion Data

        #region Properties

        public object DataValue
        {
            get { return _dataValue; }
            set { _dataValue = value; }
        }

        public string AssetName
        {
            get { return _assetName; }
            set { _assetName = value; }
        }

        public int AssetID
        {
            get { return _assetID; }
            set { _assetID = value; }
        }

        public string Location
        {
            get { return _location; }
            set { _location = value; }
        }

        #endregion Properties

        #region Constructors

        public AuditDataReportColumnValue()
        {
            _assetID = 0;
            _location = "";
            _assetName = "";
            _dataValue = null;
        }

        public AuditDataReportColumnValue(object dataValue, string assetName, int assetID, string location)
        {
            _dataValue = dataValue;
            _assetName = assetName;
            _assetID = assetID;
            _location = location;
        }

        #endregion Constructors

    }

    #endregion

    #region AuditDataReportColumn Class

    public class AuditDataReportColumn
    {
        public enum eFieldType { asset, os, applications, hardware, system, userdata };

        #region Data

        private eFieldType _fieldType;
        private string _fieldName;
        private string _columnLabel;

        // Values for this column stored in this list
        private List<AuditDataReportColumnValue> _listValues = new List<AuditDataReportColumnValue>();

        // Static strings to show if an application is installed or not installed
        public static string ApplicationInstalled = "Installed";
        public static string ApplicationNotInstalled = "Not Installed";

        #endregion Data

        #region Properties

        public eFieldType FieldType
        {
            get { return _fieldType; }
            set { _fieldType = value; }
        }

        public string FieldName
        {
            get { return _fieldName; }
            set { _fieldName = value; }
        }

        public string ShortFieldName
        {
            get
            {
                int index = FieldName.LastIndexOf('|');
                if (index == -1)
                    return FieldName;
                else
                    return FieldName.Substring(index + 1);
            }

        }

        public string ColumnLabel
        {
            get { return _columnLabel; }
            set { _columnLabel = value; }
        }

        public List<AuditDataReportColumnValue> Values
        {
            get { return _listValues; }
        }

        #endregion Properties

        #region Constructors

        public AuditDataReportColumn()
        {
            _fieldType = eFieldType.asset;
            _fieldName = "";
            _columnLabel = "";
        }

        public AuditDataReportColumn(eFieldType fieldType, string fieldName, string columnLabel)
        {
            _fieldType = fieldType;
            _fieldName = fieldName;
            _columnLabel = columnLabel;
        }

        #endregion Constructors

        #region Methods

        /// <summary>
        /// Return (any) AuditDataReportColumnValue object in our collection for the specified asset
        /// </summary>
        /// <param name="assetID"></param>
        /// <returns></returns>
        public AuditDataReportColumnValue GetValueForAsset(int assetID)
        {
            foreach (AuditDataReportColumnValue valueColumn in this._listValues)
            {
                if (valueColumn.AssetID == assetID)
                    return valueColumn;
            }
            return null;
        }


        #endregion Methods
    }

    #endregion AuditDataReportColumn Class

    #region AuditDataReportColumnList Class

    public class AuditDataReportColumnList : List<AuditDataReportColumn>
    {

        #region Data

        /// <summary>Cached Applications and Publishers list</summary>
        protected ApplicationPublisherList _cachedApplicationsList = null;

        /// <summary>Cached Operating Systems list</summary>
        protected InstalledOSList _cacheOperatingSystemList = null;

        /// <summary>Cached list of user defined data categories, fields and their values</summary>
        protected UserDataCategoryList _cachedUserDataList = null;

        #endregion Data

        protected Dictionary<string, string> _labelsDictionary = null;
        protected string _publisherFilter = "";
        protected bool _showIncluded = true;
        protected bool _showIgnored = false;

        #region Properties


        #endregion Properties

        #region Constructors

        public AuditDataReportColumnList()
        {
        }

        #endregion Constructors

        #region Methods


        /// <summary>
        /// Populate our cached lists
        /// </summary>
        public void ResetCache()
        {
            _cachedApplicationsList = null;
            _cachedUserDataList = null;
            _cacheOperatingSystemList = null;
        }

        #region Data Field Expansion Functions

        /// <summary>
        /// Populate the list of AuditDataReportColumns based on the list of fields passed in to us
        /// The problem here is that one field (say Hardware|CPU) could expand into multiple fields
        /// within the report as it is a parent category.
        /// 
        /// The purpose of this function is to build a comprehensive list of fields from that passed in
        /// expanding any categories as required and ending up with a list containing just the value fields
        /// at the terminus of each category branch
        /// </summary>
        /// <param name="fieldList"></param>		
        public void Populate(List<string> fieldList
                           , Dictionary<string, string> labelsDictionary
                           , string publisherFilter
                           , bool showIncluded
                           , bool showIgnored)
        {
            // Save the labels dictionary as we will need this during the generation of report field labels
            _labelsDictionary = labelsDictionary;
            _publisherFilter = publisherFilter;
            _showIgnored = showIgnored;
            _showIncluded = showIncluded;

            // Ensure we clear out any old columns and data values
            this.Clear();

            // Now loop through the individual fields which we are to audit and expand them as required
            foreach (string field in fieldList)
            {
                // Split this into its components as we need to know the TYPE of the field
                List<string> listParts = Utility.ListFromString(field, '|', true);

                switch (listParts[0])
                {
                    case AWMiscStrings.AssetDetails:
                        ExpandAssetDetailFieldChildren(field);
                        break;

                    case AWMiscStrings.OSNode:
                        ExpandOSFieldChildren(field);
                        break;

                    case AWMiscStrings.ApplicationsNode:
                        ExpandApplicationFieldChildren(field);
                        break;

                    case AWMiscStrings.HardwareNode:
                        ExpandHardwareFieldChildren(field);
                        break;

                    case AWMiscStrings.SystemNode:
                        ExpandHardwareFieldChildren(field);
                        break;

                    case AWMiscStrings.UserDataNode:
                        ExpandUserDataChildren(field);
                        break;

                    default:
                        break;
                }
            }

            return;
        }



        /// <summary>
        /// Expand the Children for an ASSET DETAILS based field
        /// </summary>
        /// <param name="field"></param>
        /// <param name="returnDictionary"></param>
        protected void ExpandAssetDetailFieldChildren(string field)
        {
            string baseName = field;

            // All or a specific field - if there is a delimiter then assume a specific field
            if (field.LastIndexOf("|") == -1)
            {
                // No delimiter so add ALL fields
                field = baseName + @"|" + Asset.GetAttributeName(Asset.eAttributes.assetname);
                this.Add(new AuditDataReportColumn(AuditDataReportColumn.eFieldType.asset, field, GetLabelForField(field)));
                //
                field = baseName + @"|" + Asset.GetAttributeName(Asset.eAttributes.lastaudit);
                this.Add(new AuditDataReportColumn(AuditDataReportColumn.eFieldType.asset, field, GetLabelForField(field)));
                //
                field = baseName + @"|" + Asset.GetAttributeName(Asset.eAttributes.location);
                this.Add(new AuditDataReportColumn(AuditDataReportColumn.eFieldType.asset, field, GetLabelForField(field)));
                //
                field = baseName + @"|" + Asset.GetAttributeName(Asset.eAttributes.macaddress);
                this.Add(new AuditDataReportColumn(AuditDataReportColumn.eFieldType.asset, field, GetLabelForField(field)));
                //
                field = baseName + @"|" + Asset.GetAttributeName(Asset.eAttributes.ipaddress);
                this.Add(new AuditDataReportColumn(AuditDataReportColumn.eFieldType.asset, field, GetLabelForField(field)));
                //
                field = baseName + @"|" + Asset.GetAttributeName(Asset.eAttributes.category);
                this.Add(new AuditDataReportColumn(AuditDataReportColumn.eFieldType.asset, field, GetLabelForField(field)));
                //
                field = baseName + @"|" + Asset.GetAttributeName(Asset.eAttributes.make);
                this.Add(new AuditDataReportColumn(AuditDataReportColumn.eFieldType.asset, field, GetLabelForField(field)));
                //
                field = baseName + @"|" + Asset.GetAttributeName(Asset.eAttributes.model);
                this.Add(new AuditDataReportColumn(AuditDataReportColumn.eFieldType.asset, field, GetLabelForField(field)));
                //
                field = baseName + @"|" + Asset.GetAttributeName(Asset.eAttributes.serial);
                this.Add(new AuditDataReportColumn(AuditDataReportColumn.eFieldType.asset, field, GetLabelForField(field)));
            }

            else
            {
                this.Add(new AuditDataReportColumn(AuditDataReportColumn.eFieldType.asset, field, GetLabelForField(field)));
            }
        }



        /// <summary>
        /// Expand the Children for an OS based field
        /// </summary>
        /// <param name="field"></param>
        /// <param name="returnDictionary"></param>
        protected void ExpandOSFieldChildren(string osField)
        {
            // If we have not already done so populate the cached list of Operating Systems
            if (_cacheOperatingSystemList == null)
                _cacheOperatingSystemList = new InstalledOSList();

            // All or a specific field - if there is a delimiter then assume a specific field
            if (osField.LastIndexOf("|") == -1)
            {
                // No delimiter so add ALL OS fields
                string field = osField + @"|" + OSInstance.GetAttributeName(OSInstance.eAttributes.family);
                this.Add(new AuditDataReportColumn(AuditDataReportColumn.eFieldType.os, field, GetLabelForField(field)));
                //
                field = osField + @"|" + OSInstance.GetAttributeName(OSInstance.eAttributes.fullname);
                this.Add(new AuditDataReportColumn(AuditDataReportColumn.eFieldType.os, field, GetLabelForField(field)));
                //
                field = osField + @"|" + OSInstance.GetAttributeName(OSInstance.eAttributes.cdkey);
                this.Add(new AuditDataReportColumn(AuditDataReportColumn.eFieldType.os, field, GetLabelForField(field)));
				//
				field = osField + @"|" + OSInstance.GetAttributeName(OSInstance.eAttributes.serial);
				this.Add(new AuditDataReportColumn(AuditDataReportColumn.eFieldType.os, field, GetLabelForField(field)));
			}

            else
            {
                this.Add(new AuditDataReportColumn(AuditDataReportColumn.eFieldType.os, osField, GetLabelForField(osField)));
            }
        } 
        /// <summary>
        /// Expand the Children for an ASSET DETAILS based field
        /// </summary>
        /// <param name="field"></param>
        /// <param name="returnDictionary"></param>
        protected void ExpandApplicationFieldChildren(string field)
        {
            // Split the field into its parts again
            List<string> listParts = Utility.ListFromString(field, '|', true);

            // If we have not already done so populate the cached list of Applications
            if (_cachedApplicationsList == null)
                _cachedApplicationsList = new ApplicationPublisherList(_publisherFilter, _showIncluded, _showIgnored);

            // OK - the string is formatted as Applications | Publisher | Application
            // Therefore we know if we have 1 part include all applications, 2 parts then this is a Publisher 
            // 3 parts it is an application
            if (listParts.Count == 1)
            {
                // get a list of ALL Publishers and applications first
                AddApplications(_cachedApplicationsList);
            }

            else if (listParts.Count == 2)
            {
                string publisher = listParts[1];
                ApplicationPublisher thisPublisher = _cachedApplicationsList.FindPublisher(publisher);
                ApplicationPublisherList listPublishers = new ApplicationPublisherList();
                listPublishers.Add(thisPublisher);
                AddApplications(listPublishers);
            }

            else
            {
                this.Add(new AuditDataReportColumn(AuditDataReportColumn.eFieldType.applications, field, GetLabelForField(field)));
            }
        }


        /// <summary>
        /// Add applications from a list of publishers and applications to our list of displayed fields
        /// </summary>
        /// <param name="listPublishers"></param>
        /// <param name="returnDictionary"></param>
        protected void AddApplications(ApplicationPublisherList listPublishers)
        {
            // Add the publishers to the tree 
            foreach (ApplicationPublisher thePublisher in listPublishers)
            {
                // Add the Applications beneath this publisher
                foreach (InstalledApplication theApplication in thePublisher)
                {
                    string field = AWMiscStrings.ApplicationsNode + "|" + thePublisher.Name + "|" + theApplication.Name;
                    this.Add(new AuditDataReportColumn(AuditDataReportColumn.eFieldType.applications, field, GetLabelForField(field)));
                }
            }
        }




        /// <summary>
        /// Expand the Children for a Hardware based field
        /// </summary>
        /// <param name="field"></param>
        /// <param name="returnDictionary"></param>
        protected void ExpandHardwareFieldChildren(string field)
        {
            AuditedItemsDAO lwDataAccess = new AuditedItemsDAO();

            // Get the child CATEGORIES of this field (if any)
            DataTable categoryTable = lwDataAccess.GetAuditedItemCategories(field);

            // OK there were child CATEGORIES - recurse as we are only actually interested in VALUES beneath 
            // the categories, not the categories themselves
            foreach (DataRow row in categoryTable.Rows)
            {
                string category = (string)row["_CATEGORY"];
                ExpandHardwareFieldChildren(category);
            }

            // Now check for any NAMES within this category
            DataTable namesTable = lwDataAccess.GetAuditedItemCategoryNames(field);

            // Add any NAMES into the report
            foreach (DataRow row in namesTable.Rows)
            {
                string category = (string)row["_CATEGORY"];
                string name = (string)row["_NAME"];
                string icon = (string)row["_ICON"];

                string fieldName = category + "|" + name;
                this.Add(new AuditDataReportColumn(AuditDataReportColumn.eFieldType.hardware
                                    , fieldName
                                    , GetLabelForField(fieldName)));

            }

            // If there were NO SUB-CATEGORIES and NO NAMES then simply add the field itself as it be a NAME
            // If we have no return data then just add the item itself to the dictionary
            if ((categoryTable.Rows.Count == 0) && (namesTable.Rows.Count == 0))
            {
                this.Add(new AuditDataReportColumn(AuditDataReportColumn.eFieldType.hardware, field, GetLabelForField(field)));
                return;
            }
        }


        /// <summary>
        /// Expansion of a field which is based on User Defined Data Fields
        /// </summary>
        /// <param name="field"></param>
        /// <param name="returnDictionary"></param>
        protected void ExpandUserDataChildren(string field)
        {
            // If we have not already done so create the cache for User Defined Data
            if (_cachedUserDataList == null)
            {
                _cachedUserDataList = new UserDataCategoryList(UserDataCategory.SCOPE.Asset);
                _cachedUserDataList.Populate();
            }

            // The User data Field is formatted as User Data | Category | Field
            // Split the field into its parts again
            List<string> listParts = Utility.ListFromString(field, '|', true);

            // If we have 3 parts then this is easiest as this is an explic field which we just add it
            if (listParts.Count == 3)
            {
                this.Add(new AuditDataReportColumn(AuditDataReportColumn.eFieldType.userdata, field, GetLabelForField(field)));
                return;
            }

            // Ok we are either adding ALL User Data Field for ALL categories or all for a single category		
            string requiredCategory = "";
            if (listParts.Count == 2)
                requiredCategory = listParts[1];

            //				
            foreach (UserDataCategory category in _cachedUserDataList)
            {
                if ((requiredCategory == "" || requiredCategory == category.Name)
                && (category.Scope == UserDataCategory.SCOPE.Asset))
                {
                    foreach (UserDataField dataField in category)
                    {
                        bool alreadyExists = false;

                        field = AWMiscStrings.UserDataNode + "|" + category.Name + "|" + dataField.Name;

                        foreach (AuditDataReportColumn reportColumn in this)
                        {
                            if (reportColumn.FieldName == field)
                            {
                                alreadyExists = true;
                                break;
                            }
                        }

                        if (!alreadyExists)
                            this.Add(new AuditDataReportColumn(AuditDataReportColumn.eFieldType.userdata, field, GetLabelForField(field)));
                    }
                }
            }
        }

        #endregion Data Field Expansion Functions


        #region Report Data Generation Functions

        public void GenerateReportData(AssetList cachedAssetList)
        {
            // Iterate through each of the fields defined in the DataSet and recover a list of values for that field
            // Note we then have to filter any values which are for assets not included in the report
            foreach (AuditDataReportColumn reportColumn in this)
            {
                GenerateReportFieldValues(cachedAssetList, reportColumn);
            }
        }


        /// <summary>
        /// This function is responsible for generating the actual data which will be displayed by the report
        /// We need to create one value for each asset per field in the report
        /// </summary>
        protected void GenerateReportFieldValues(AssetList cachedAssetList, AuditDataReportColumn reportColumn)
        {
            // Recovery of the data values depends on the type of value
            switch (reportColumn.FieldType)
            {
                case AuditDataReportColumn.eFieldType.asset:
                    GetAssetDetailFieldValues(cachedAssetList, reportColumn);
                    break;

                case AuditDataReportColumn.eFieldType.os:
                    GetOSFieldValues(cachedAssetList, reportColumn);
                    break;

                case AuditDataReportColumn.eFieldType.applications:
                    GetApplicationFieldValues(cachedAssetList, reportColumn);
                    break;

                case AuditDataReportColumn.eFieldType.hardware:
                    GetHardwareFieldValues(cachedAssetList, reportColumn);
                    break;

                case AuditDataReportColumn.eFieldType.system:
                    GetHardwareFieldValues(cachedAssetList, reportColumn);
                    break;

                case AuditDataReportColumn.eFieldType.userdata:
                    GetUserDataFieldValues(cachedAssetList, reportColumn);
                    break;

                default:
                    break;
            }
        }



        /// <summary>
        /// Gets all available data values for the specied asset detail field
        /// </summary>
        /// <param name="reportColumn"></param>
        protected void GetAssetDetailFieldValues(AssetList cachedAssetList, AuditDataReportColumn reportColumn)
        {
            List<string> fieldParts = Utility.ListFromString(reportColumn.FieldName, '|', true);

            // We need to get the values for the specific asset details field for each of the assets 
            // specified in the assets list
            foreach (Asset asset in cachedAssetList)
            {
                string value = asset.GetAttributeValue(reportColumn.ShortFieldName);
                reportColumn.Values.Add(new AuditDataReportColumnValue(value, asset.Name, asset.AssetID, asset.Location));
            }
        }



        /// <summary>
        /// Gets all available data values for the specied Operating System field
        /// </summary>
        /// <param name="reportColumn"></param>
        protected void GetOSFieldValues(AssetList cachedAssetList, AuditDataReportColumn reportColumn)
        {
            List<string> fieldParts = Utility.ListFromString(reportColumn.FieldName, '|', true);

            // We need to get the values for the specific asset details field for each of the assets 
            // specified in the assets list
            foreach (Asset asset in cachedAssetList)
            {
                // Find (any) OS entry for this asset
                OSInstance osInstance = _cacheOperatingSystemList.GetInstanceForAsset(asset.AssetID);
                string value = (osInstance == null) ? AWMiscStrings.NoValueFound : osInstance.GetAttributeValue(reportColumn.ShortFieldName);

                // Add the column to the result set
                reportColumn.Values.Add(new AuditDataReportColumnValue(value, asset.Name, asset.AssetID, asset.Location));
            }
        }


        /// <summary>
        /// Gets all available data values for the specifed APPLICATION field.  In this case we need to know 
        /// for each asset whether or not the application is installed.
        /// </summary>
        /// <param name="reportColumn"></param>
        protected void GetApplicationFieldValues(AssetList cachedAssetList, AuditDataReportColumn reportColumn)
        {
            try
            {
                List<string> fieldParts = Utility.ListFromString(reportColumn.FieldName, '|', true);
                string publisherName = fieldParts[1];
                string applicationName = fieldParts[2];

                // The application name is formatted as Applications | Publisher | Application - we need to find this specific
                // application in our internal cached list
                ApplicationPublisher publisher = _cachedApplicationsList.FindPublisher(publisherName);
                InstalledApplication application = publisher.FindApplication(applicationName);

                // Now loop through the Assets and determine which does have the application and which does not
                // We store the value 'Installed' or 'Not Installed' for the value for each asset
                foreach (Asset asset in cachedAssetList)
                {
                    string value = (application.FindInstance(asset.AssetID) == null) ? AuditDataReportColumn.ApplicationNotInstalled : AuditDataReportColumn.ApplicationInstalled;
                    reportColumn.Values.Add(new AuditDataReportColumnValue(value, asset.Name, asset.AssetID, asset.Location));
                }
            }
            catch (Exception)
            {
            }
        }


        /// <summary>
        /// Gets all available data values for the specifed HARDWARE field.  In this case we need to know 
        /// for each asset the value for the specified hardware or system field.
        /// </summary>
        /// <param name="reportColumn"></param>
        protected void GetHardwareFieldValues(AssetList cachedAssetList, AuditDataReportColumn reportColumn)
        {
            AuditedItemsDAO lwDataAccess = new AuditedItemsDAO();

            // The field name held in the report column combines the field catagory and name with the name last
            // We need to split these for the purpose of passing to the database function
            int index = reportColumn.FieldName.LastIndexOf('|');
            string category = reportColumn.FieldName.Substring(0, index);
            string name = reportColumn.FieldName.Substring(index + 1);

            // We need to get a list of values specified for this hardware field noting that we return ALL audited values
            // as we cannot filter by asset at this time.  The data is therefore returned is the form AssetID / Value			
            DataTable table = lwDataAccess.GetAuditedItemValues(null, category, name);

            // Move this data into a Dictionary to make it easier to access
            Dictionary<int, string> listFieldValues = new Dictionary<int, string>();

            foreach (DataRow row in table.Rows)
            {
                int assetID = (int)row["_ASSETID"];
                string value = (string)row["_VALUE"];

                // Just in case we get duplicate values filter them out here
                if (!listFieldValues.ContainsKey(assetID))
                    listFieldValues.Add(assetID, value);
            }

            // Now loop through the Assets and determine which does have the application and which does not
            // We store the value 'Installed' or 'Not Installed' for the value for each asset
            foreach (Asset asset in cachedAssetList)
            {
                string value;

                // Do we have a value for this asset?  If not store "<no value>"
                if (listFieldValues.ContainsKey(asset.AssetID))
                    value = listFieldValues[asset.AssetID];
                else
                    value = AWMiscStrings.NoValueFound;

                // Add the column to the result set
                reportColumn.Values.Add(new AuditDataReportColumnValue(value, asset.Name, asset.AssetID, asset.Location));
            }
        }


        /// <summary>
        /// Gets all available data values for the specifed USER DATA field.  In this case we need to know 
        /// for each asset any value specified for the supplied User Data field.
        /// </summary>
        /// <param name="reportColumn"></param>
        protected void GetUserDataFieldValues(AssetList cachedAssetList, AuditDataReportColumn reportColumn)
        {
            // the user data field name is formatted as User Data | Category | Field
            List<string> fieldParts = Utility.ListFromString(reportColumn.FieldName, '|', true);
            string categoryName = fieldParts[1];
            string fieldName = fieldParts[2];
            //
            UserDataCategory userDataCategory = _cachedUserDataList.FindCategory(categoryName);
            if (userDataCategory == null)
                return;
            //
            UserDataField userDataField = userDataCategory.FindField(fieldName);
            if (userDataField == null)
                return;

            // Query the database for the possible values for this user defined data field
            userDataField.PopulateValues();

            // ...now loop through the assets and get the fiekld value for each where available		
            foreach (Asset asset in cachedAssetList)
            {
                string value = userDataField.GetValueFor(asset.AssetID);
                reportColumn.Values.Add(new AuditDataReportColumnValue(value, asset.Name, asset.AssetID, asset.Location));
            }

        }

        #endregion Report Data Generation Functions


        /// <summary>
        /// return the appropriate label for the specified field  by first matching against the list of
        /// labels previously defined (if any) and if there is a match returning that otherwise returning the
        /// last element of the field which is delimited by pipe characters
        /// </summary>
        /// <param name="field"></param>
        /// <returns></returns>
        protected string GetLabelForField(string field)
        {
            if (_labelsDictionary.ContainsKey(field))
                return _labelsDictionary[field];

            int index = field.LastIndexOf("|");
            if (index == -1)
                return field;
            else
                return field.Substring(index + 1);
        }


        #endregion Methods

    }

    #endregion AuditDataReportColumnList Class

}
