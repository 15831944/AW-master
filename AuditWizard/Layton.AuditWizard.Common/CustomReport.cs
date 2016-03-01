using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data.SqlServerCe;
using System.Text;
using System.Data;
using System.Windows.Forms;

using Layton.AuditWizard.DataAccess;

namespace Layton.AuditWizard.Common
{
    public class CustomReport
    {
        private static readonly List<string> _updatedReportConditions = new List<string>();
        private static readonly AssetDAO lAssetDAO = new AssetDAO();
        private static readonly UserDataDefinitionsDAO lUserDataDefinitionsDAO = new UserDataDefinitionsDAO();
        private static readonly ApplicationsDAO lApplicationsDAO = new ApplicationsDAO();
        private static readonly AuditedItemsDAO lAuditedItemsDAO = new AuditedItemsDAO();
        private bool _showIncludedApplications;
        private bool _showIgnoredApplications;
        
        protected static readonly log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public CustomReport(bool showIncluded, bool showIgnored)
        {
            _showIncludedApplications = showIncluded;
            _showIgnoredApplications = showIgnored;
        }

        public DataTable CreateDataTableForCustomReport(List<string> aReportConditions)
        {
            return CreateDataTableForCustomReport(aReportConditions, new AssetDAO().GetSelectedAssets());
        }

        public DataTable CreateDataTableForCustomReport(List<string> aReportConditions, string aAssetIds)
        {
            DataTable lResultsDataTable = new DataTable();
            bool bPatchesIncluded = false;

            try
            {
                Cursor.Current = Cursors.WaitCursor;
                bool lDisplayAsAssetRegister = false;

                // aReportConditions contains a list of all criteria and a list of required assets
                // loop through each asset and get the data for each column
                // need to build a DataTable with the required columns
                string lDisplayResultsAsAssetRegister = "";

                foreach (string lReportCondition in aReportConditions)
                {
                    if (lDisplayResultsAsAssetRegister == "")
                    {
                        if (lReportCondition.StartsWith("ASSET_REGISTER:"))
                        {
                            lDisplayResultsAsAssetRegister = lReportCondition;
                            break;
                        }
                    }
                }

                // an empty lSelectedAssets means that all assets have been selected
                if (aAssetIds == "")
                {
                    aAssetIds = new AssetDAO().GetAllAssetIdsAsString();
                }

                if (lDisplayResultsAsAssetRegister != "")
                    lDisplayAsAssetRegister = Convert.ToBoolean(lDisplayResultsAsAssetRegister.Substring(15));

                // if all children of a parent node are selected, we will only see the parent here
                // i.e. Hardware|CPU means we have selected all children of that root
                // we need to loop through each of the fields now to handle this case

                _updatedReportConditions.Clear();

                foreach (string lReportCondition in aReportConditions)
                {
                    List<string> listParts = Utility.ListFromString(lReportCondition, '|', true);

                    switch (listParts[0])
                    {
                        case AWMiscStrings.AssetDetails:
                            ExpandAssetDetailFieldChildren(lReportCondition);
                            break;

                        case AWMiscStrings.OSNode:
                            ExpandOSFieldChildren(lReportCondition);
                            break;

                        case AWMiscStrings.ApplicationsNode:
                            ExpandApplicationFieldChildren(lReportCondition);
                            break;

                        case AWMiscStrings.HardwareNode:
                            ExpandAuditedItemsChildren(lReportCondition);
                            break;

                        case AWMiscStrings.SystemNode:
                            ExpandAuditedItemsChildren(lReportCondition);
                            break;

                        case AWMiscStrings.UserDataNode:
                            ExpandUserDataChildren(lReportCondition);
                            break;

                        default:
                            break;
                    }
                }

                DataRow[] rows;
                string lCurrentReportCondition;

                if (!lDisplayAsAssetRegister)
                {
                    lResultsDataTable = BuildResultsTableStandard(_updatedReportConditions);
                    object[] lNewRowArray = new object[_updatedReportConditions.Count];

                    DataTable lUnionResultsDataTable = RunCustomUnionStatement(aAssetIds);
                    //object[] lNewRowArray = new object[lUnionResultsDataTable.Rows.Count];

                    foreach (string lAssetId in aAssetIds.Split(','))
                    {
                        for (int i = 0; i < _updatedReportConditions.Count; i++)
                        {
                            lCurrentReportCondition = _updatedReportConditions[i];

                            rows = lUnionResultsDataTable.Select(String.Format("ASSETID = {0} AND REFID = {1}", lAssetId, i));

                            // deal with applications differently
                            if (lCurrentReportCondition.StartsWith("Applications|"))
                                lNewRowArray[i] = (rows.Length == 0) ? "Not Installed" : "Installed";                            
                            else
                                lNewRowArray[i] = (rows.Length == 0) ? "" : rows[0].ItemArray[1].ToString();
                            if (lCurrentReportCondition.StartsWith("System|Patches"))
                            {
                                bPatchesIncluded = true;
                            }
                        }
                                                
                        if (bPatchesIncluded)
                        {

                            string strAssetName = lNewRowArray[0].ToString();                            
                            int iColCount = lResultsDataTable.Columns.Count;
                            object[] lRowArray = new object[iColCount];
                            for (int j = 1; j < lNewRowArray.Length; j = j + 4)
                            {
                                for (int i = 0; i < iColCount-2; i++)
                                {                                    
                                    lRowArray[i+2] = lNewRowArray[i + j].ToString();
                                }
                                lRowArray[0] = strAssetName;
                                string strPatchItem = _updatedReportConditions[j].ToString();
                                String [] lReportConditions = strPatchItem.Split('|');
                                strPatchItem = lReportConditions[lReportConditions.Length - 3] + "|" + lReportConditions[lReportConditions.Length - 2];
                                lRowArray[1] = strPatchItem;

                                //check before adding applies to this asset
                                bool bInsert=false;
                                for (int i = 2; i < iColCount ; i++)
                                {
                                    if (lRowArray[i].ToString() != "")
                                    {
                                        bInsert = true;
                                        break;
                                    }
                                }
                                if (bInsert)
                                {
                                    lResultsDataTable.Rows.Add(lRowArray);
                                }
                            }                            
                        }
                        else
                        {
                            lResultsDataTable.Rows.Add(lNewRowArray);
                        }
                    }
                }
                else
                {
                    lResultsDataTable = new DataTable();
                    lResultsDataTable.Columns.Add("Asset Name", typeof(string));
                    lResultsDataTable.Columns.Add("Field Name", typeof(string));
                    lResultsDataTable.Columns.Add("Value", typeof(string));                    

                    DataTable lUnionResultsDataTable = RunCustomUnionStatement(aAssetIds);

                    foreach (string lAssetId in aAssetIds.Split(','))
                    {
                        string lAssetName = lAssetDAO.ConvertIdListToNames(lAssetId, ';');                        
                        bPatchesIncluded = false;
                        bool bItemRowAdded = false;
                        int iCount = 0;
                        List<object[]> listupdatedPatch = new List<object[]>();

                        for (int i = 0; i < _updatedReportConditions.Count; i++)
                        {
                            object[] lNewRowArray = new object[3];
                            lCurrentReportCondition = _updatedReportConditions[i];
                            rows = lUnionResultsDataTable.Select(String.Format("ASSETID = {0} AND REFID = {1}", lAssetId, i));

                            lNewRowArray[0] = lAssetName;
                            lNewRowArray[1] = lCurrentReportCondition.Split('|')[lCurrentReportCondition.Split('|').Length - 1];

                            // deal with applications differently
                            if (lCurrentReportCondition.StartsWith("Applications|"))
                                lNewRowArray[2] = (rows.Length == 0) ? "Not Installed" : "Installed";
                            else
                                lNewRowArray[2] = (rows.Length == 0) ? "" : rows[0].ItemArray[1].ToString();

                            if (lCurrentReportCondition.StartsWith("System|Patches"))
                            {
                                bPatchesIncluded = true;
                            }

                            if (bPatchesIncluded)
                            {
                                object[] NewRowArray = new object[3];
                                NewRowArray[0] = lAssetName;
                                NewRowArray[1] = "Item";                                
                                String[] lReportConditions = lCurrentReportCondition.Split('|');
                                string strPatchItem = lReportConditions[lReportConditions.Length - 3] + "|" + lReportConditions[lReportConditions.Length - 2];
                                NewRowArray[2] = strPatchItem;                       

                                if (!bItemRowAdded)
                                {
                                    //lResultsDataTable.Rows.Add(NewRowArray);
                                    listupdatedPatch.Add(NewRowArray);
                                    bItemRowAdded=true;
                                }
                                iCount++;                        
                                //lResultsDataTable.Rows.Add(lNewRowArray);
                                listupdatedPatch.Add(lNewRowArray);
                                
                                
                                if (iCount == 4)
                                {
                                    //We had the details for a patch check it is associated with the asset
                                    bItemRowAdded = false;
                                    iCount = 0;
                                    bool bInsert = false;
                                    for (int k = 1; k < listupdatedPatch.Count-1;k++ )
                                    {
                                       if (listupdatedPatch[k][2].ToString() != "")
                                        {
                                            bInsert = true;
                                            break;
                                        }
                                                                            
                                    }
                                    if (bInsert)
                                    {
                                        foreach (object[] objItem in listupdatedPatch)
                                        {
                                            lResultsDataTable.Rows.Add(objItem);
                                        }
                                        listupdatedPatch.Clear();
                                    }
                                    else
                                    {
                                        listupdatedPatch.Clear();
                                    }
                                }
                                
                            }
                            else
                            {
                                lResultsDataTable.Rows.Add(lNewRowArray);
                            }
                        }
                    }

                    lResultsDataTable.DefaultView.Sort = "Asset Name ASC";
                }

            }
            catch (OutOfMemoryException ex)
            {
                logger.Error(ex.Message);
                MessageBox.Show(
                    "An out of memory exception has occured.  You may have too many items in your Custom Report." + Environment.NewLine + Environment.NewLine +
                    "Please reduce the number of assets and/or report fields and re-run the query.",
                    "AuditWizard",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Exclamation);
            }
            catch (System.Data.SqlClient.SqlException ex)
            {
                if (ex.Number == 701 || ex.Number == 8623)
                {
                    MessageBox.Show(
				   "An SQL Exception has occured. You may have too many items in your Custom Report." + Environment.NewLine + Environment.NewLine +
                   "Please reduce the number of assets and/or report fields and re-run the query.",
                   "AuditWizard",
                   MessageBoxButtons.OK,
                   MessageBoxIcon.Exclamation);
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message);
            }
            finally
            {
                Cursor.Current = Cursors.Default;
            }

            return lResultsDataTable;
        }

        

        //public DataTable CreateDataTableForCustomReport(List<string> aReportConditions, string aAssetIds)
        //{
        //    DataTable lResultsDataTable = new DataTable();

        //    try
        //    {
        //        Cursor.Current = Cursors.WaitCursor;
        //        bool lDisplayAsAssetRegister = false;

        //        // aReportConditions contains a list of all criteria and a list of required assets
        //        // loop through each asset and get the data for each column
        //        // need to build a DataTable with the required columns
        //        string lDisplayResultsAsAssetRegister = "";

        //        foreach (string lReportCondition in aReportConditions)
        //        {
        //            if (lDisplayResultsAsAssetRegister == "")
        //            {
        //                if (lReportCondition.StartsWith("ASSET_REGISTER:"))
        //                {
        //                    lDisplayResultsAsAssetRegister = lReportCondition;
        //                    break;
        //                }
        //            }
        //        }

        //        // an empty lSelectedAssets means that all assets have been selected
        //        if (aAssetIds == "")
        //        {
        //            aAssetIds = new AssetDAO().GetAllAssetIdsAsString(); ;
        //        }

        //        if (lDisplayResultsAsAssetRegister != "")
        //            lDisplayAsAssetRegister = Convert.ToBoolean(lDisplayResultsAsAssetRegister.Substring(15));

        //        // if all children of a parent node are selected, we will only see the parent here
        //        // i.e. Hardware|CPU means we have selected all children of that root
        //        // we need to loop through each of the fields now to handle this case

        //        _updatedReportConditions.Clear();

        //        foreach (string lReportCondition in aReportConditions)
        //        {
        //            List<string> listParts = Utility.ListFromString(lReportCondition, '|', true);

        //            switch (listParts[0])
        //            {
        //                case AWMiscStrings.AssetDetails:
        //                    ExpandAssetDetailFieldChildren(lReportCondition);
        //                    break;

        //                case AWMiscStrings.OSNode:
        //                    ExpandOSFieldChildren(lReportCondition);
        //                    break;

        //                case AWMiscStrings.ApplicationsNode:
        //                    ExpandApplicationFieldChildren(lReportCondition);
        //                    break;

        //                case AWMiscStrings.HardwareNode:
        //                    ExpandAuditedItemsChildren(lReportCondition);
        //                    break;

        //                case AWMiscStrings.SystemNode:
        //                    ExpandAuditedItemsChildren(lReportCondition);
        //                    break;

        //                case AWMiscStrings.UserDataNode:
        //                    ExpandUserDataChildren(lReportCondition);
        //                    break;

        //                default:
        //                    break;
        //            }
        //        }

        //        DataRow[] rows;
        //        string lCurrentReportCondition;

        //        if (!lDisplayAsAssetRegister)
        //        {
        //            lResultsDataTable = BuildResultsTableStandard(_updatedReportConditions);
        //            object[] lNewRowArray = new object[_updatedReportConditions.Count];

        //            DataTable lUnionResultsDataTable = RunCustomUnionStatement(aAssetIds);

        //            foreach (string lAssetId in aAssetIds.Split(','))
        //            {
        //                for (int i = 0; i < _updatedReportConditions.Count; i++)
        //                {
        //                    lCurrentReportCondition = _updatedReportConditions[i];
        //                    rows = lUnionResultsDataTable.Select(String.Format("ASSETID = {0} AND REFID = {1}", lAssetId, i));

        //                    // deal with applications differently
        //                    if (lCurrentReportCondition.StartsWith("Applications|"))
        //                        lNewRowArray[i] = (rows.Length == 0) ? "Not Installed" : "Installed";
        //                    else
        //                        lNewRowArray[i] = (rows.Length == 0) ? "" : rows[0].ItemArray[1].ToString();
        //                }

        //                lResultsDataTable.Rows.Add(lNewRowArray);
        //            }
        //        }
        //        else
        //        {
        //            lResultsDataTable = new DataTable();
        //            lResultsDataTable.Columns.Add("Asset Name", typeof(string));
        //            lResultsDataTable.Columns.Add("Field Name", typeof(string));
        //            lResultsDataTable.Columns.Add("Value", typeof(string));

        //            object[] lNewRowArray = new object[3];

        //            DataTable lUnionResultsDataTable = RunCustomUnionStatement(aAssetIds);

        //            foreach (string lAssetId in aAssetIds.Split(','))
        //            {
        //                string lAssetName = lAssetDAO.ConvertIdListToNames(lAssetId, ';');

        //                for (int i = 0; i < _updatedReportConditions.Count; i++)
        //                {
        //                    lCurrentReportCondition = _updatedReportConditions[i];
        //                    rows = lUnionResultsDataTable.Select(String.Format("ASSETID = {0} AND REFID = {1}", lAssetId, i));

        //                    lNewRowArray[0] = lAssetName;
        //                    lNewRowArray[1] = lCurrentReportCondition.Split('|')[lCurrentReportCondition.Split('|').Length - 1];

        //                    // deal with applications differently
        //                    if (lCurrentReportCondition.StartsWith("Applications|"))
        //                        lNewRowArray[2] = (rows.Length == 0) ? "Not Installed" : "Installed";
        //                    else
        //                        lNewRowArray[2] = (rows.Length == 0) ? "" : rows[0].ItemArray[1].ToString();

        //                    lResultsDataTable.Rows.Add(lNewRowArray);
        //                }
        //            }

        //            lResultsDataTable.DefaultView.Sort = "Asset Name ASC";
        //        }

        //    }
        //    catch (OutOfMemoryException ex)
        //    {
        //        logger.Error(ex.Message);
        //        MessageBox.Show(
        //            "You have exceeded the maximum number of items a Custom Report can display." + Environment.NewLine + Environment.NewLine +
        //            "Please reduce the number of assets and/or report fields and re-run the query.",
        //            "AuditWizard",
        //            MessageBoxButtons.OK,
        //            MessageBoxIcon.Exclamation);
        //    }
        //    catch (System.Data.SqlClient.SqlException ex)
        //    {
        //        if (ex.Number == 701 || ex.Number == 8623)
        //        {
        //            MessageBox.Show(
        //           "You have exceeded the maximum number of items a Custom Report can display." + Environment.NewLine + Environment.NewLine +
        //           "Please reduce the number of assets and/or report fields and re-run the query.",
        //           "AuditWizard",
        //           MessageBoxButtons.OK,
        //           MessageBoxIcon.Exclamation);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        logger.Error(ex.Message);
        //    }
        //    finally
        //    {
        //        Cursor.Current = Cursors.Default;
        //    }

        //    return lResultsDataTable;
        //}

        private static DataTable RunCustomUnionStatement(string aSelectedAssetIds)
        {
            StringBuilder sb = new StringBuilder();

            for (int i = 0; i < _updatedReportConditions.Count; i++)
            {
                string lReportCondition = _updatedReportConditions[i];
                sb.Append(CreateUnionSqlStatement(lReportCondition, aSelectedAssetIds, i) + " UNION ALL ");
            }

            string lUnionStatement = sb.ToString();
            lUnionStatement = lUnionStatement.Substring(0, lUnionStatement.Length - 11);

            return new AssetDAO().RunCustomUnionStatement(lUnionStatement);
        }

        private static string CreateUnionSqlStatement(string aReportCondition, string aAssetIds, int aReferenceId)
        {
            string[] lConditionArray = aReportCondition.Split('|');

            // APPLICATIONS
            if (aReportCondition.StartsWith("Applications|"))
            {
                string lVersion = "";
                aReportCondition = aReportCondition.Substring(13);

                string lPublisher = aReportCondition.Substring(0, aReportCondition.IndexOf('|'));
                string lApplicationName = aReportCondition.Substring(aReportCondition.LastIndexOf('|') + 1);

                if (lApplicationName.Contains("(v"))
                {
                    lVersion = lApplicationName.Substring(lApplicationName.LastIndexOf("(v") + 2, lApplicationName.LastIndexOf(")") - lApplicationName.LastIndexOf("(v") - 2);
                    lApplicationName = lApplicationName.Replace(" (v" + lVersion + ")", String.Empty);
                }

                return String.Format(
                    "SELECT ai._assetid AS ASSETID, ap._name AS VALUE, {0} AS REFID " +
                    "FROM APPLICATIONS ap " +
                    "INNER JOIN APPLICATION_INSTANCES ai ON (ai._applicationid = ap._applicationid) " +
                    "WHERE _ASSETID IN ({1}) " +
                    "AND ap._publisher = '{2}' " +
                    "AND ap._name = '{3}' " +
                    "AND ap._version = '{4}'", aReferenceId, aAssetIds, lPublisher.Replace("'", "''"), lApplicationName.Replace("'", "''"), lVersion);

            }
            // USER-DEFINED DATA
            else if (aReportCondition.StartsWith("UserData|"))
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
                    "FROM AUDITEDITEMS " +
                    "WHERE _assetid IN ({1}) " +
                    "AND _category = '{2}' " +
                    "AND _name = '{3}'", aReferenceId, aAssetIds, lCategory, lName);
            }
        }

        private void ExpandAssetDetailFieldChildren(string aReportCondition)
        {
            // if the string has one component it means the user has selected the whole node
            string[] lFieldArray = aReportCondition.Split('|');

            if (lFieldArray.Length == 1)
            {
                _updatedReportConditions.Add("Asset Details|Asset Name");
                _updatedReportConditions.Add("Asset Details|Location");
                //_updatedReportConditions.Add("Asset Details|Domain");
                _updatedReportConditions.Add("Asset Details|Date of last Audit");
                //_updatedReportConditions.Add("Asset Details|Stock Status");
                //_updatedReportConditions.Add("Asset Details|Supplier Name");
                _updatedReportConditions.Add("Asset Details|IP Address");
                _updatedReportConditions.Add("Asset Details|MAC Address");
                _updatedReportConditions.Add("Asset Details|Category");
                _updatedReportConditions.Add("Asset Details|Make");
                _updatedReportConditions.Add("Asset Details|Model");
                _updatedReportConditions.Add("Asset Details|Serial Number");
            }
            else
            {
                _updatedReportConditions.Add(aReportCondition);
            }
        }

        private void ExpandOSFieldChildren(string aReportCondition)
        {
            // if the string has one component it means the user has selected the whole node
            string[] lFieldArray = aReportCondition.Split('|');

            if (lFieldArray.Length == 1)
            {
                _updatedReportConditions.Add("Operating Systems|Family");
                _updatedReportConditions.Add("Operating Systems|Version");
                _updatedReportConditions.Add("Operating Systems|CD Key");
                _updatedReportConditions.Add("Operating Systems|Serial Number");
            }
            else
            {
                _updatedReportConditions.Add(aReportCondition);
            }
        }

        private void ExpandUserDataChildren(string aReportCondition)
        {
            // if the string has two components it means the user has selected the whole node
            string[] lFieldArray = aReportCondition.Split('|');

            // length of one is the whole User Data tree
            if (lFieldArray.Length == 1)
            {
                DataTable lParentDataTable = lUserDataDefinitionsDAO.GetParentUserDataCategories();

                foreach (DataRow lParentCategory in lParentDataTable.Rows)
                {
                    ExpandUserDataForParent(lParentCategory[0].ToString());
                }
            }

            // length of two is the whole User Data parent category
            else if (lFieldArray.Length == 2)
            {
                ExpandUserDataForParent(lFieldArray[1]);
            }

            // otherwise just the individual value
            else
            {
                _updatedReportConditions.Add(aReportCondition);
            }
        }

        private void ExpandUserDataForParent(string aParentName)
        {
            DataTable userDataChildren = lUserDataDefinitionsDAO.GetUserDataNamesByParent(aParentName);

            foreach (DataRow userDataChildRow in userDataChildren.Rows)
            {
                _updatedReportConditions.Add(userDataChildRow[0].ToString());
            }
        }

        private void ExpandApplicationFieldChildren(string aReportCondition)
        {
            string lApp = "";

            // count of one component means the user has selected the whole node (All Applications)
            // count of two components means the user has selected the whole publisher 
            // count of three is an individual application and we don't need to do anything here
            string[] lFieldArray = aReportCondition.Split('|');

            DataTable dataTable;

            if (lFieldArray.Length == 1)
            {
                dataTable = lApplicationsDAO.GetAllApplicationsByPublisher("", _showIncludedApplications, _showIgnoredApplications);

                foreach (DataRow lApplicationRow in dataTable.Rows)
                {
                    lApp = lApplicationRow[0].ToString();
                    lApp = (lApplicationRow[0].ToString() == "") ? lApp : lApp + " (v" + lApplicationRow[1] + ")";

                    _updatedReportConditions.Add("Applications|" + lApp);
                }
            }
            else if (lFieldArray.Length == 2)
            {
                dataTable = lApplicationsDAO.GetAllApplicationsByPublisher(lFieldArray[1], _showIncludedApplications, _showIgnoredApplications);

                foreach (DataRow lApplicationRow in dataTable.Rows)
                {
                    lApp = lApplicationRow[0].ToString();
                    lApp = (lApplicationRow[1].ToString() == "") ? lApp : lApp + " (v" + lApplicationRow[1] + ")";

                    _updatedReportConditions.Add("Applications|" + lApp);
                }
            }
            else
            {
                _updatedReportConditions.Add(aReportCondition);
            }
        }

        private void ExpandAuditedItemsChildren(string aReportCondition)
        {
            // for this parent node run the following query:
            DataTable childrenOfParentNode = lAuditedItemsDAO.GetChildrenFromParent(aReportCondition);

            if (childrenOfParentNode.Rows.Count == 0)
            {
                // if the number of rows returned is zero then we know this parent has no children

                // we can then run the following query to get all of the values for this node
                DataTable childNodeValues = lAuditedItemsDAO.GetValuesForChildNode(aReportCondition);

                // if that returns no rows then we know we had the child node all along so just add it
                if (childNodeValues.Rows.Count == 0)
                {
                    _updatedReportConditions.Add(aReportCondition);
                }
                // if there are rows then add them to the list
                else
                {
                    foreach (DataRow row in childNodeValues.Rows)
                    {
                        _updatedReportConditions.Add(row[0].ToString());
                    }
                }
            }
            else
            {
                // if the number of rows IS NOT zero then we know that the user has chosen a parent of a parent
                // in this case we use recursion to pass this value back to this method
                foreach (DataRow row in childrenOfParentNode.Rows)
                {
                    ExpandAuditedItemsChildren(row[0].ToString());
                }
            }
        }

        private DataTable BuildResultsTableStandard(List<string> aReportConditions)
        {
            DataTable lResultsDataTable = new DataTable();

            bool bPatch=false;

            foreach (string lReportCondition in aReportConditions)
            {
                string[] lConditionArray = lReportCondition.Split('|');
                int i = 1;

                if (lReportCondition.StartsWith("System|Patches"))
                {
                    while (!lResultsDataTable.Columns.Contains("Item"))
                    {
                        lResultsDataTable.Columns.Add("Item", typeof(string));
                    }
                }
                string columnName = lConditionArray[lConditionArray.Length - 1];

                while (lResultsDataTable.Columns.Contains(columnName))
                {
                    if (lReportCondition.StartsWith("System|Patches"))
                    {
                        bPatch = true;   
                    }
                    columnName = lConditionArray[lConditionArray.Length - 1];
                    columnName += "_" + i;
                    i++;
                }
                if (!bPatch)
                {
                    lResultsDataTable.Columns.Add(columnName, typeof(string));
                }
                bPatch = false;
            }

            return lResultsDataTable;
        }
    }
}
