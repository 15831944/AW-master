using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using Layton.AuditWizard.DataAccess;

namespace Layton.AuditWizard.Common
{
    public class ComplianceReport
    {
        private bool _showIncludedApplications;
        private bool _showIgnoredApplications;
        protected static readonly log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public string CreateDataTableForComplianceReport(List<string> aReportConditions, bool aShowIncludedApps, bool aShowIgnoredApps)
        {
            _showIncludedApplications = aShowIncludedApps;
            _showIgnoredApplications = aShowIgnoredApps;

            return GetCompliantAssetIds(aReportConditions, new AssetDAO().GetSelectedAssets());
        }

        public string CreateDataTableForComplianceReport(List<string> aReportConditions, bool aShowIncludedApps, bool aShowIgnoredApps, string aAssetIds)
        {
            _showIncludedApplications = aShowIncludedApps;
            _showIgnoredApplications = aShowIgnoredApps;

            return GetCompliantAssetIds(aReportConditions, aAssetIds);
        }

        private string CreateUnionSqlStatement(string aReportCondition, string aAssetIds, int aReferenceId)
        {
            string[] lConditionArray = aReportCondition.Split('|');

            // APPLICATIONS
            if (aReportCondition.StartsWith("Applications|"))
            {
                string lPublisher;
                string lVersion = "";
                aReportCondition = aReportCondition.Substring(13);

                lPublisher = aReportCondition.Substring(0, aReportCondition.IndexOf('|'));

                string lApplicationName = aReportCondition.Substring(aReportCondition.LastIndexOf('|') + 1);

                if (lApplicationName.Contains("(v"))
                {
                    lVersion = lApplicationName.Substring(lApplicationName.LastIndexOf("(v") + 2, lApplicationName.LastIndexOf(")") - lApplicationName.LastIndexOf("(v") - 2);
                    lApplicationName = lApplicationName.Replace(" (v" + lVersion + ")", String.Empty);
                }

                return String.Format(
                    "SELECT ai._assetid AS ASSETID, ap._name + ' ' + ap._version AS VALUE, {0} AS REFID " +
                    "FROM APPLICATIONS ap " +
                    "INNER JOIN APPLICATION_INSTANCES ai ON (ai._applicationid = ap._applicationid) " +
                    "WHERE _ASSETID IN ({1}) " +
                    "AND ap._publisher = '{2}' " +
                    "AND ap._name = '{3}' " +
                    "AND ap._version = '{4}'", aReferenceId, aAssetIds, lPublisher.Replace("'", "''"), lApplicationName.Replace("'", "''"), lVersion);

            }
            // USER-DEFINED DATA
            if (aReportCondition.StartsWith("UserData|"))
            {
                return String.Format(
                    "select uv._parentid AS ASSETID, uv._value AS VALUE, {0} AS REFID " +
                    "from userdata_definitions ud " +
                    "left join userdata_values uv on (uv._userdefid = ud._userdefid) " +
                    "where ud._name = '{1}' " +
                    "and ud._parentid in (select _userdefid from userdata_definitions where _name = '{2}') " +
                    "and uv._parentid IN ({3})", aReferenceId, lConditionArray[2], lConditionArray[1], aAssetIds);
            }

            // ASSET DETAILS
            else if (aReportCondition.StartsWith("Asset Details|"))
            {
                string columnName = "";

                switch (aReportCondition.Split('|')[1])
                {
                    case "Asset Name":
                        columnName = "_name";
                        break;
                    case "Location":
                        columnName = "_locationid";
                        break;
                    case "Date of last Audit":
                        columnName = "_lastaudit";
                        break;
                    case "IP Address":
                        columnName = "_ipaddress";
                        break;
                    case "MAC Address":
                        columnName = "_macaddress";
                        break;
                    case "Make":
                        columnName = "_make";
                        break;
                    case "Model":
                        columnName = "_model";
                        break;
                    case "Serial Number":
                        columnName = "_serial_number";
                        break;
                    case "Category":
                        columnName = "_category";
                        break;
                    case "Type":
                        columnName = "_type";
                        break;
                    case "Asset Tag":
                        columnName = "_assettag";
                        break;
                }
                switch (columnName)
                {
                    case "_category":
                        return String.Format(
                            "SELECT a._ASSETID AS ASSETID, at1._NAME AS VALUE, {0} AS REFID " +
                            "FROM ASSET_TYPES at " +
                            "INNER JOIN ASSETS a ON (a._ASSETTYPEID = at._ASSETTYPEID) " +
                            "INNER JOIN ASSET_TYPES at1 ON at._PARENTID = at1._ASSETTYPEID " +
                            "WHERE a._ASSETID IN ({1})", aReferenceId, aAssetIds);
                    case "_type":
                        return String.Format(
                            "SELECT a._ASSETID AS ASSETID, at._NAME AS VALUE, {0} AS REFID " +
                            "FROM ASSET_TYPES at " +
                            "INNER JOIN ASSETS a ON (a._ASSETTYPEID = at._ASSETTYPEID) " +
                            "WHERE a._ASSETID IN ({1})", aReferenceId, aAssetIds);
                    case "_locationid":
                        return String.Format(
                            "SELECT a._ASSETID AS ASSETID, l._fullname AS VALUE, {0} AS REFID " +
                            "FROM LOCATIONS l " +
                            "INNER JOIN ASSETS a on (a._locationid = l._locationid) " +
                            "WHERE a._ASSETID IN ({1})", aReferenceId, aAssetIds);
                    case "_lastaudit":
                        return String.Format(
                            "SELECT _ASSETID AS ASSETID, CONVERT(nvarchar(30), _lastaudit, 20) AS VALUE, {0} AS REFID " +
                            "FROM ASSETS " +
                            "WHERE _ASSETID IN ({1})", aReferenceId, aAssetIds);
                    default:
                        return String.Format(
                            "SELECT _ASSETID AS ASSETID, {0} AS VALUE, {1} AS REFID " +
                            "FROM ASSETS " +
                            "WHERE _ASSETID IN ({2})", columnName, aReferenceId, aAssetIds);
                }
            }
            // OPERATING SYSTEMS
            else if (aReportCondition.StartsWith("Operating Systems|"))
            {
                string columnName = "";

                switch (lConditionArray[1])
                {
                    case "Family":
                        columnName = "_name";
                        break;
                    case "Version":
                        columnName = "_version";
                        break;
                    case "CD Key":
                        columnName = "_cdkey";
                        break;
                    case "Serial Number":
                        columnName = "_productid";
                        break;
                }

                return String.Format(
                    "SELECT ai._assetid AS ASSETID, {0} AS VALUE, {1} AS REFID " +
                    "FROM APPLICATIONS a " +
                    "LEFT JOIN APPLICATION_INSTANCES ai ON (ai._applicationid = a._applicationid) " +
                    "WHERE ai._assetid IN ({2}) " +
                    "AND a._isos = 1", columnName, aReferenceId, aAssetIds);
            }
            // SYSTEM PATCHES
            //else if (aReportCondition.StartsWith("System|Patches|"))
            //{
            //}
            // if we have reached here must be dealing with AUDITEDITEMS
            else
            {
                int lConditionLength = lConditionArray.Length;
                string lName = lConditionArray[lConditionLength - 1];
                string lCategory = aReportCondition.Replace(lName, "");
                lCategory = lCategory.TrimEnd('|');

                return String.Format(
                    "SELECT _assetid AS ASSETID, _value AS VALUE, {0} AS REFID " +
                    "FROM auditeditems " +
                    "WHERE _assetid IN ({1}) " +
                    "AND _category = '{2}' " +
                    "AND _name = '{3}'", aReferenceId, aAssetIds, lCategory, lName);
            }
        }

        private DataTable RunCustomUnionStatement(string aSelectedAssetIds, List<string> aReportConditions)
        {
            string lUnionStatement = "";
            string lReportCondition = "";
            StringBuilder sb = new StringBuilder();

            for (int i = 0; i < aReportConditions.Count; i++)
            {
                //assetFilter.Substring(assetFilter.IndexOf("_category"))
                lReportCondition = aReportConditions[i].Substring(aReportConditions[i].IndexOf("_category"));

                lReportCondition =
                    lReportCondition.Substring(13, lReportCondition.IndexOf(" and _name", 12) - 14)
                    + "|" + lReportCondition.Substring(lReportCondition.IndexOf(" and _name = '") + 14,
                    lReportCondition.IndexOf("and _value") - (lReportCondition.IndexOf(" and _name = '") + 16));

                sb.Append(CreateUnionSqlStatement(lReportCondition, aSelectedAssetIds, i) + " UNION ALL ");
            }

            lUnionStatement = sb.ToString();
            lUnionStatement = lUnionStatement.Substring(0, lUnionStatement.Length - 11);
            return new AssetDAO().RunCustomUnionStatement(lUnionStatement);
        }

        public DataTable CreateComplianceGrid(string complianceStatus, string _lastCompliantIds, List<string> _lastComplianceFilterConditions)
        {
            return CreateComplianceGrid(complianceStatus, _lastCompliantIds, _lastComplianceFilterConditions, new AssetDAO().GetSelectedAssets());
        }

        public DataTable CreateComplianceGrid(string complianceStatus, string _lastCompliantIds, List<string> _lastComplianceFilterConditions, string aAssetIds)
        {
            DataTable complianceDataTable = new DataTable();
            string compliantIds = _lastCompliantIds;
            DataRow[] rows;
            AssetDAO lAssetDAO = new AssetDAO();

            DataColumn myDataColumn;

            myDataColumn = new DataColumn();
            myDataColumn.DataType = Type.GetType("System.String");
            myDataColumn.ColumnName = "AssetName";
            complianceDataTable.Columns.Add(myDataColumn);

            if (complianceStatus == "Non-Compliant")
            {
                // an empty lSelectedAssets means that all assets have been selected
                if (aAssetIds == "")
                {
                    aAssetIds = new AssetDAO().GetAllAssetIdsAsString();
                }

                StringBuilder sb = new StringBuilder();

                DataTable nonCompliantIds = new AssetDAO().GetNonCompliantAssetIds(compliantIds, aAssetIds);

                foreach (DataRow compliantRow in nonCompliantIds.Rows)
                {
                    sb.Append(compliantRow.ItemArray[0].ToString() + ",");
                }

                compliantIds = sb.ToString().TrimEnd(',');
            }

            if (compliantIds != "")
            {
                object[] lNewRowArray = new object[_lastComplianceFilterConditions.Count + 1];
                string lCurrentReportCondition = "";
                DataTable lUnionResultsDataTable = RunCustomUnionStatement(compliantIds, _lastComplianceFilterConditions);
                string t;
                int k = 1;

                foreach (string filterCondition in _lastComplianceFilterConditions)
                {
                    t = filterCondition;
                    myDataColumn = new DataColumn();
                    myDataColumn.DataType = Type.GetType("System.String");
                    myDataColumn.ColumnName = t.Substring(t.IndexOf("_name = ") + 9, t.IndexOf("and _value") - t.IndexOf("_name = ") - 11);

                    while (complianceDataTable.Columns.Contains(myDataColumn.ColumnName))
                    {
                        myDataColumn.ColumnName += "_" + k.ToString();
                        k++;
                    }

                    complianceDataTable.Columns.Add(myDataColumn);
                }

                DataTable lAssetNameDataTable = lAssetDAO.GetAssetNamesByIds(compliantIds);

                foreach (string lAssetId in compliantIds.Split(','))
                {
                    lNewRowArray[0] = lAssetNameDataTable.Select("_ASSETID = " + lAssetId)[0].ItemArray[1].ToString();
                    for (int i = 0; i < _lastComplianceFilterConditions.Count; i++)
                    {
                        lCurrentReportCondition = _lastComplianceFilterConditions[i];
                        lCurrentReportCondition = lCurrentReportCondition.Substring(lCurrentReportCondition.IndexOf("_category"));

                        rows = lUnionResultsDataTable.Select(String.Format("ASSETID = {0} AND REFID = {1}", lAssetId, i));

                        // deal with applications differently
                        if (lCurrentReportCondition.StartsWith("_category = 'Applications|"))
                        {
                            lNewRowArray[i + 1] = (rows.Length == 0) ? "Not Installed" : "Installed";
                        }
                        else
                            lNewRowArray[i + 1] = (rows.Length == 0) ? "" : rows[0].ItemArray[1].ToString();
                    }

                    complianceDataTable.Rows.Add(lNewRowArray);
                }
            }

            return complianceDataTable;
        }

        private string GetCompliantAssetIds(List<string> aReportConditions, string aAssetIds)
        {
            DataTable resultsTable = new DataTable();
            string lReturnedIds = "";
            try
            {
                // an empty lSelectedAssets means that all assets have been selected
                if (aAssetIds == "")
                {
                    aAssetIds = new AssetDAO().GetAllAssetIdsAsString();
                }

                string lBooleanCondition = "";
                string lFilterCondition = "";
                string lSQLString = "SELECT _ASSETID FROM ASSETS WHERE (";

                // the user could have picked any of the audited categories
                // parse each filter condition and find the data
                foreach (string filterCondition in aReportConditions)
                {
                    string otherCompliantIds = "";

                    if (!filterCondition.StartsWith("1ST"))
                    {
                        lBooleanCondition = filterCondition.Substring(0, filterCondition.IndexOf("_category"));
                        lSQLString += (lBooleanCondition == "And") ? ") " + lBooleanCondition + " (" : lBooleanCondition + " ";
                    }

                    lFilterCondition = filterCondition.Substring(filterCondition.IndexOf("_category"));

                    // APPLICATIONS
                    if (lFilterCondition.StartsWith("_category = 'Applications|"))
                    {
                        otherCompliantIds = HandleApplications(lFilterCondition, aAssetIds);
                    }
                    // USER-DEFINED DATA
                    else if (lFilterCondition.StartsWith("_category = 'UserData|"))
                    {
                        otherCompliantIds = HandleUserData(lFilterCondition);
                    }
                    // ASSET DETAILS
                    else if (lFilterCondition.StartsWith("_category = 'Asset Details'"))
                    {
                        otherCompliantIds = HandleAssetDetailsData(lFilterCondition);
                    }
                    // OPERATING SYSTEMS
                    else if (lFilterCondition.StartsWith("_category = 'Operating Systems'"))
                    {
                        otherCompliantIds = HandleOSData(lFilterCondition);
                    }
                    // SYSTEM PATCHES
                    else if (lFilterCondition.StartsWith("_category = 'System|Patches|"))
                    {
                        otherCompliantIds = HandlePatchesData(lFilterCondition);
                    }
                    // AUDITEDITEMS
                    else
                    {
                        otherCompliantIds = HandleAuditedItemsData(lFilterCondition);
                    }

                    otherCompliantIds = (otherCompliantIds == "") ? "''" : otherCompliantIds;
                    lSQLString += String.Format("_ASSETID IN ({0}) ", otherCompliantIds);
                }

                lSQLString = lSQLString.Trim();

                // if we have only included Or statements we'll need to add a trailing bracket here
                if (CountOccurencesOfChar(lSQLString, '(') > CountOccurencesOfChar(lSQLString, ')'))
                    lSQLString += ")";

                lSQLString += String.Format(" AND _ASSETID IN ({0})", aAssetIds);

                DataTable lDataTable = new StatisticsDAO().PerformQuery(lSQLString);


                foreach (DataRow compliantRow in lDataTable.Rows)
                {
                    if (!lReturnedIds.Contains(compliantRow.ItemArray[0].ToString()))
                        lReturnedIds += compliantRow.ItemArray[0].ToString() + ",";
                }

                lReturnedIds = lReturnedIds.TrimEnd(',');
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message);
            }

            return lReturnedIds;
        }

        private int CountOccurencesOfChar(string instance, char c)
        {
            int result = 0;
            foreach (char curChar in instance)
            {
                if (c == curChar)
                {
                    ++result;
                }
            }
            return result;
        }

        private string HandleAuditedItemsData(string applicationFilter)
        {
            DataTable resultsTable;
            string newIds = String.Empty;

            resultsTable = new StatisticsDAO().GetAuditedItems(applicationFilter);

            //rows = resultsTable.Select(applicationFilter + " and _assetid in (" + compliantIds + ")");

            foreach (DataRow compliantRow in resultsTable.Rows)
            {
                if (!newIds.Contains(compliantRow.ItemArray[0].ToString()))
                    newIds += compliantRow.ItemArray[0].ToString() + ",";
            }

            return newIds.TrimEnd(',');
        }

        private string HandlePatchesData(string applicationFilter)
        {
            string newIds = String.Empty;
            string category;
            string name;
            DataTable resultsDataTable = new DataTable();
            AuditedItemsDAO lAuditedItemsDAO = new AuditedItemsDAO();

            string af = applicationFilter;

            category = af.Substring(13, af.IndexOf("and _name") - 15);
            name = af.Substring(af.IndexOf("and _name") + 13, af.IndexOf("and _value") - (af.IndexOf("and _name = '") + 15));

            resultsDataTable = lAuditedItemsDAO.GetCompliantAssetsIdsForPatch(category + "|" + name);

            foreach (DataRow compliantRow in resultsDataTable.Rows)
            {
                if (!newIds.Contains(compliantRow.ItemArray[0].ToString()))
                    newIds += compliantRow.ItemArray[0].ToString() + ",";
            }

            return newIds.TrimEnd(',');
        }

        private string HandleOSData(string applicationFilter)
        {
            string assetValue;
            DataTable resultsDataTable = new DataTable();
            ApplicationInstanceDAO lApplicationInstanceDAO = new ApplicationInstanceDAO();
            string newIds = String.Empty;
            string complianceCriteria = String.Empty;

            string assetCriteria = applicationFilter.Substring(45, applicationFilter.IndexOf("and _value") - 47);

            switch (assetCriteria)
            {
                case "Family":
                    complianceCriteria = "_name";
                    break;
                case "Version":
                    complianceCriteria = "_version";
                    break;
                case "CD Key":
                    complianceCriteria = "_cdkey";
                    break;
                case "Serial Number":
                    complianceCriteria = "_productid";
                    break;
            }

            assetValue = applicationFilter.Substring(applicationFilter.LastIndexOf("=") + 2);
            resultsDataTable = lApplicationInstanceDAO.GetCompliantOSAssetIds(complianceCriteria, assetValue);

            foreach (DataRow compliantRow in resultsDataTable.Rows)
            {
                if (!newIds.Contains(compliantRow.ItemArray[0].ToString()))
                    newIds += compliantRow.ItemArray[0].ToString() + ",";
            }

            return newIds.TrimEnd(',');
        }

        private string HandleAssetDetailsData(string assetFilter)
        {
            string assetValue;
            string assetCriteria;
            DataTable resultsDataTable = new DataTable();
            AssetDAO lAssetDAO = new AssetDAO();
            AssetTypesDAO lAssetTypesDAO = new AssetTypesDAO();
            LocationsDAO lLocationsDAO = new LocationsDAO();
            string newIds = String.Empty;
            string complianceCriteria = String.Empty;

            //foreach (string assetFilter in applicationFilters)
            //{
            assetCriteria = assetFilter.Substring(41, assetFilter.IndexOf("and _value") - 43);

            switch (assetCriteria)
            {
                case "Asset Name":
                    complianceCriteria = "_name";
                    break;
                case "Location":
                    complianceCriteria = "_locationid";
                    break;
                case "Date of last Audit":
                    complianceCriteria = "_lastaudit";
                    break;
                case "IP Address":
                    complianceCriteria = "_ipaddress";
                    break;
                case "MAC Address":
                    complianceCriteria = "_macaddress";
                    break;
                case "Make":
                    complianceCriteria = "_make";
                    break;
                case "Model":
                    complianceCriteria = "_model";
                    break;
                case "Serial Number":
                    complianceCriteria = "_serial_number";
                    break;
                case "Category":
                    complianceCriteria = "_category";
                    break;
                case "Type":
                    complianceCriteria = "_type";
                    break;
                case "Asset Tag":
                    complianceCriteria = "_assettag";
                    break;
            }

            if (complianceCriteria == "_category")
            {
                assetValue = assetFilter.Substring(assetFilter.LastIndexOf("=") + 2);
                resultsDataTable = lAssetTypesDAO.GetCompliantAssetCategoriesValue(assetValue);
            }
            else if (complianceCriteria == "_type")
            {
                assetValue = assetFilter.Substring(assetFilter.LastIndexOf("=") + 2);
                resultsDataTable = lAssetTypesDAO.GetCompliantAssetTypesValue(assetValue);
            }
            else if (complianceCriteria == "_locationid")
            {
                assetValue = assetFilter.Substring(assetFilter.LastIndexOf("=") + 2);
                resultsDataTable = lLocationsDAO.GetCompliantLocationValues(assetValue);
            }
            else if (complianceCriteria == "_lastaudit")
            {
                if (assetFilter.IndexOf("and _value") != -1)
                    resultsDataTable =
                        lAssetDAO.GetCompliantAssetValueForLastAudit(assetFilter.Substring(assetFilter.IndexOf("and _value") + 11));
            }
            else
            {
                assetValue = assetFilter.Substring(assetFilter.LastIndexOf("=") + 2);
                //resultsDataTable = lAssetDAO.GetCompliantAssetValue(complianceCriteria, assetValue, compliantIds);
                resultsDataTable = lAssetDAO.GetCompliantAssetValue(complianceCriteria, assetValue);
            }

            foreach (DataRow compliantRow in resultsDataTable.Rows)
            {
                if (!newIds.Contains(compliantRow.ItemArray[0].ToString()))
                    newIds += compliantRow.ItemArray[0].ToString() + ",";
            }

            newIds = newIds.TrimEnd(',');
            return newIds;
        }

        private string HandleUserData(string applicationFilter)
        {
            string parentDefinition;
            string userDataName;
            string userDataValue;
            string newIds = String.Empty;
            DataTable resultsDataTable = new DataTable();
            UserDataDefinitionsDAO lUserDataDefinitionsDAO = new UserDataDefinitionsDAO();

            string uf = applicationFilter;

            userDataName = uf.Substring(uf.IndexOf("and _name = '") + 13, uf.IndexOf("and _value") - (uf.IndexOf("and _name = '") + 15));
            userDataValue = uf.Substring(uf.LastIndexOf("=") + 2);
            parentDefinition = uf.Substring(22, uf.IndexOf("and _name =") - 24);

            resultsDataTable = lUserDataDefinitionsDAO.GetCompliantUserData(userDataName, parentDefinition, userDataValue);

            foreach (DataRow compliantRow in resultsDataTable.Rows)
            {
                if (!newIds.Contains(compliantRow.ItemArray[0].ToString()))
                    newIds += compliantRow.ItemArray[0].ToString() + ",";
            }

            return newIds.TrimEnd(',');
        }

        private string HandleApplications(string applicationFilter, string compliantIds)
        {
            DataTable applicationInstancesDataTable = new ApplicationInstanceDAO().GetAllInstances(_showIncludedApplications, _showIgnoredApplications);
            string publisher = String.Empty;
            string application = String.Empty;
            string version = String.Empty;
            bool installed = true;
            DataRow[] rows;
            List<string> compliantIdList = Utility.ListFromString(compliantIds, ',', true);
            List<string> newCompliantIdList = new List<string>();
            string newIds = String.Empty;

            publisher = applicationFilter.Substring(26, applicationFilter.IndexOf("' and _name") - 26);
            application = applicationFilter.Substring(applicationFilter.IndexOf("_name = ") + 9, applicationFilter.IndexOf("and _value") - applicationFilter.IndexOf("_name = ") - 11);
            installed = applicationFilter.Contains("==");

            // check if there is a specific version number to check for this application
            if (applicationFilter.Contains("(v"))
            {
                version = applicationFilter.Substring(applicationFilter.LastIndexOf("(v") + 2, applicationFilter.LastIndexOf(")") - applicationFilter.LastIndexOf("(v") - 2);
                application = application.Replace(" (v" + version + ")", String.Empty);
            }

            string id;

            for (int i = 0; i < compliantIdList.Count; i++)
            {
                id = compliantIdList[i];
                rows = applicationInstancesDataTable.Select(
                    String.Format("_ASSETID = {0} AND _PUBLISHER = '{1}' AND _NAME = '{2}' AND _VERSION = '{3}'",
                    id, publisher.Replace("'", "''"), application.Replace("'", "''"), version.Replace("'", "''")));

                if (installed)
                {
                    if (rows.Length > 0)
                        newCompliantIdList.Add(id);
                }
                else
                {
                    if (rows.Length == 0)
                        newCompliantIdList.Add(id);
                }
            }

            compliantIdList.Clear();
            compliantIdList.InsertRange(0, newCompliantIdList);
            newCompliantIdList.Clear();

            foreach (string compliantId in compliantIdList)
            {
                if (!newIds.Contains(compliantId))
                    newIds += compliantId + ",";
            }

            return newIds.TrimEnd(',');
        }
    }
}
