using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlServerCe;
using System.Windows.Forms;

namespace Layton.AuditWizard.DataAccess
{
    public class StatisticsDAO
    {
        #region Data

        private string connectionStringCompact = null;
        protected static readonly log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private static readonly bool isDebugEnabled = logger.IsDebugEnabled;
        private bool compactDatabaseType = false;
        private Configuration config = ConfigurationManager.OpenExeConfiguration(Path.Combine(Application.StartupPath, "AuditWizardv8.exe"));

        #endregion

        public StatisticsDAO()
        {
            compactDatabaseType = Convert.ToBoolean(config.AppSettings.Settings["CompactDatabaseType"].Value);
            connectionStringCompact = "Data Source=" + Application.StartupPath + Convert.ToString(config.AppSettings.Settings["ConnectionStringCompact"].Value);
        }

        #region Private Database Helper Methods

        private string ReplaceStringQuotes(string replaceString)
        {
            if (replaceString.Length > 0)
            {
                // replace single quote with two single quotes
                replaceString.Replace("'", "''");
            }

            return replaceString;
        }

        #endregion

        #region Statistical Functions

        /// <summary>
        /// Return Audit Statistics
        /// </summary>
        /// <returns></returns>
        public DataTable AuditStatistics()
        {
            if (isDebugEnabled) logger.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " in");

            DataTable statisticsTable = new DataTable(TableNames.APPLICATION_STATISTICS);

            if (compactDatabaseType)
            {
                try
                {
                    statisticsTable.Columns.Add(new DataColumn("row"));
                    statisticsTable.Columns.Add(new DataColumn("uniqueapplications"));
                    statisticsTable.Columns.Add(new DataColumn("deployedagents"));
                    statisticsTable.Columns.Add(new DataColumn("totalapplications"));
                    statisticsTable.Columns.Add(new DataColumn("publishers"));

                    using (SqlCeConnection conn = DatabaseConnection.CreateOpenCEConnection())
                    {
                        DataRow newRow;
                        newRow = statisticsTable.NewRow();

                        object[] rowArray = new object[5];
                        rowArray[0] = "row";

                        rowArray[1] = QueryAndGetValue(conn, "SELECT COUNT(_NAME) FROM APPLICATIONS WHERE _IGNORED=0 AND _ISOS=0");
                        rowArray[2] = QueryAndGetValue(conn, "SELECT COUNT(_NAME) FROM ASSETS WHERE _AGENT_STATUS<>0");
                        rowArray[3] = QueryAndGetValue(conn, "SELECT COUNT(_APPLICATIONID) FROM APPLICATION_INSTANCES WHERE _APPLICATIONID IN (SELECT _APPLICATIONID FROM APPLICATIONS WHERE _ISOS=0 AND _IGNORED=0)");
                        rowArray[4] = QueryAndGetValue(conn, "SELECT COUNT(*) FROM (SELECT DISTINCT _PUBLISHER FROM APPLICATIONS WHERE _IGNORED = 0) t");

                        newRow.ItemArray = rowArray;
                        statisticsTable.Rows.Add(newRow);
                    }
                }
                catch (SqlCeException ex)
                {
                    Utility.DisplayErrorMessage("A database error has occurred in AuditWizard." + Environment.NewLine + Environment.NewLine +
                            "Please see the log file for further details.");
                    logger.Error("Exception in " + System.Reflection.MethodBase.GetCurrentMethod().Name, ex);
                }
                catch (Exception ex)
                {
                    Utility.DisplayErrorMessage("A database error has occurred in AuditWizard." + Environment.NewLine + Environment.NewLine +
                            "Please see the log file for further details.");

                    logger.Error("Exception in " + System.Reflection.MethodBase.GetCurrentMethod().Name, ex);
                }
            }
            else
            {
                AuditWizardDataAccess lAuditWizardDataAccess = new AuditWizardDataAccess();
                statisticsTable = lAuditWizardDataAccess.AuditStatistics();
            }

            if (isDebugEnabled) logger.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " out");
            return statisticsTable;
        }



        /// <summary>
        /// Return statistics for Asset States
        /// </summary>
        /// <returns></returns>
        public DataTable AssetStatistics()
        {
            if (isDebugEnabled) logger.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " in");

            DataTable statisticsTable = new DataTable(TableNames.COMPUTER_STATISTICS);

            if (compactDatabaseType)
            {
                try
                {
                    statisticsTable.Columns.Add(new DataColumn("row"));
                    statisticsTable.Columns.Add(new DataColumn("discovered"));
                    statisticsTable.Columns.Add(new DataColumn("audited"));
                    statisticsTable.Columns.Add(new DataColumn("notaudited"));
                    statisticsTable.Columns.Add(new DataColumn("stock"));
                    statisticsTable.Columns.Add(new DataColumn("inuse"));
                    statisticsTable.Columns.Add(new DataColumn("pending"));
                    statisticsTable.Columns.Add(new DataColumn("disposed"));
                    statisticsTable.Columns.Add(new DataColumn("mostrecentaudit"));

                    using (SqlCeConnection conn = DatabaseConnection.CreateOpenCEConnection())
                    {
                        DataRow newRow;
                        newRow = statisticsTable.NewRow();

                        object[] rowArray = new object[9];
                        rowArray[0] = "row";

                        rowArray[1] = QueryAndGetValue(conn, "SELECT COUNT(*) FROM ASSETS");
                        rowArray[2] = QueryAndGetValue(conn, "SELECT COUNT(*) FROM ASSETS where  _LASTAUDIT IS NOT NULL");
                        rowArray[3] = QueryAndGetValue(conn, "SELECT COUNT(*) FROM ASSETS WHERE _LASTAUDIT IS NULL");
                        rowArray[4] = QueryAndGetValue(conn, "SELECT COUNT(*) FROM ASSETS WHERE _STOCK_STATUS = 0");
                        rowArray[5] = QueryAndGetValue(conn, "SELECT COUNT(*) FROM ASSETS WHERE _STOCK_STATUS = 1");
                        rowArray[6] = QueryAndGetValue(conn, "SELECT COUNT(*) FROM ASSETS WHERE _STOCK_STATUS = 2");
                        rowArray[7] = QueryAndGetValue(conn, "SELECT COUNT(*) FROM ASSETS WHERE _STOCK_STATUS = 3");
                        rowArray[8] = QueryAndGetValue(conn, "SELECT MAX(_LASTAUDIT) FROM ASSETS WHERE _HIDDEN=0");

                        newRow.ItemArray = rowArray;
                        statisticsTable.Rows.Add(newRow);
                    }
                }
                catch (SqlCeException ex)
                {
                    Utility.DisplayErrorMessage("A database error has occurred in AuditWizard." + Environment.NewLine + Environment.NewLine +
                            "Please see the log file for further details.");
                    logger.Error("Exception in " + System.Reflection.MethodBase.GetCurrentMethod().Name, ex);
                }
                catch (Exception ex)
                {
                    Utility.DisplayErrorMessage("A database error has occurred in AuditWizard." + Environment.NewLine + Environment.NewLine +
                            "Please see the log file for further details.");

                    logger.Error("Exception in " + System.Reflection.MethodBase.GetCurrentMethod().Name, ex);
                }
            }
            else
            {
                AuditWizardDataAccess lAuditWizardDataAccess = new AuditWizardDataAccess();
                statisticsTable = lAuditWizardDataAccess.AssetStatistics();
            }


            if (isDebugEnabled) logger.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " out");
            return statisticsTable;
        }

        private object QueryAndGetValue(SqlCeConnection conn, string commandText)
        {
            using (SqlCeCommand command = new SqlCeCommand(commandText, conn))
            {
                object result = command.ExecuteScalar();

                if (result.GetType() == typeof(Int32))
                    return Convert.ToInt32(result);

                else if (result.GetType() == typeof(DateTime))
                    return Convert.ToDateTime(result);

                else if (result.GetType() == typeof(String))
                    return Convert.ToString(result);

                else
                    return result;
            }
        }

        private object QueryAndGetValue(SqlConnection conn, string commandText)
        {
            using (SqlCommand command = new SqlCommand(commandText, conn))
            {
                object result = command.ExecuteScalar();

                if (result.GetType() == typeof(System.Int32))
                    return Convert.ToInt32(result);

                else if (result.GetType() == typeof(System.DateTime))
                    return Convert.ToDateTime(result);

                else if (result.GetType() == typeof(System.String))
                    return Convert.ToString(result);

                else
                    return result;
            }
        }

        public DataTable StatisticsAuditedAssetsDrilldown(string aRowLabel)
        {
            string sqlQuery = "SELECT _NAME as \"Asset Name\", _MAKE as Make, _MODEL as Model, _LASTAUDIT as LastAudit FROM ASSETS ";

            if (aRowLabel == "Audited")
                sqlQuery += "WHERE _LASTAUDIT IS NOT NULL ";
            else
                sqlQuery += "WHERE _LASTAUDIT IS NULL ";

            sqlQuery += String.Format("AND a._ASSETID IN ({0})", new AssetDAO().GetSelectedAssets());

            return PerformQuery(sqlQuery);
        }

        public DataTable StatisticsAuditedAssets()
        {
            DataTable dataTable = PerformQuery(
                "SELECT _NAME as Name, _MAKE as Make, _MODEL as Model, _LASTAUDIT as LastAudit " +
                "FROM ASSETS");

            DataTable myDataTable = new DataTable();

            double auditedAssets = Convert.ToDouble(dataTable.Compute("COUNT(LastAudit)", "LastAudit IS NOT NULL"));
            double unAuditedAssets = 0.0;

            foreach (DataRow foundRow in dataTable.Rows)
            {
                if (foundRow.ItemArray[4].GetType() == typeof(System.DBNull))
                    unAuditedAssets++;
            }

            DataColumn myDataColumn;

            myDataColumn = new DataColumn();
            myDataColumn.DataType = Type.GetType("System.String");
            myDataColumn.ColumnName = "Item";
            myDataTable.Columns.Add(myDataColumn);

            myDataColumn = new DataColumn();
            myDataColumn.DataType = Type.GetType("System.Int32");
            myDataColumn.ColumnName = "Total";
            myDataTable.Columns.Add(myDataColumn);

            DataRow row;

            row = myDataTable.NewRow();
            row["Item"] = "Audited";
            row["Total"] = auditedAssets;
            myDataTable.Rows.Add(row);

            row = myDataTable.NewRow();
            row["Item"] = "Unaudited";
            row["Total"] = unAuditedAssets;
            myDataTable.Rows.Add(row);

            return myDataTable;
        }

        public DataTable StatisticsComplianceOverview(string compliantIds)
        {
            double compliantCount;

            if (compliantIds == "")
                compliantCount = 0.0;
            else
                compliantCount = compliantIds.Split(',').Length;

            //aReportConditions.Remove(lDisplayResultsAsAssetRegister);
            string lSelectedAssetIds = new AssetDAO().GetSelectedAssets();

            // an empty lSelectedAssets means that all assets have been selected
            if (lSelectedAssetIds == "")
            {
                DataTable lAllAssetDataTable = new AssetDAO().GetAllAssetIds();
                foreach (DataRow assetRow in lAllAssetDataTable.Rows)
                {
                    lSelectedAssetIds += assetRow[0].ToString() + ",";
                }

                lSelectedAssetIds = lSelectedAssetIds.TrimEnd(',');
            }

            double nonCompliantCount = lSelectedAssetIds.Split(',').Length - compliantCount;

            DataTable myDataTable = new DataTable();

            DataColumn myDataColumn;

            myDataColumn = new DataColumn();
            myDataColumn.DataType = Type.GetType("System.String");
            myDataColumn.ColumnName = "Item";
            myDataTable.Columns.Add(myDataColumn);

            myDataColumn = new DataColumn();
            myDataColumn.DataType = Type.GetType("System.Int32");
            myDataColumn.ColumnName = "Total";
            myDataTable.Columns.Add(myDataColumn);

            DataRow row;

            row = myDataTable.NewRow();
            row["Item"] = "Compliant";
            row["Total"] = compliantCount;
            myDataTable.Rows.Add(row);

            row = myDataTable.NewRow();
            row["Item"] = "Non-Compliant";
            row["Total"] = nonCompliantCount;
            myDataTable.Rows.Add(row);

            return myDataTable;
        }

        public DataTable StatisticsAssetAgentVersions()
        {
            return StatisticsAssetAgentVersions(new AssetDAO().GetSelectedAssets());
        }

        public DataTable StatisticsAssetAgentVersionsAsGrid(string aAssetIds)
        {
            string commandText =
                    "select _name as \"Asset Name\", _agent_version as Item " +
                    "from assets " +
                    "where _agent_version <> '' ";

            if (aAssetIds != "")
                commandText += "AND _ASSETID IN (" + aAssetIds + ") ";

            commandText += "ORDER BY \"Asset Name\"";

            return PerformQuery(commandText);
        }

        public DataTable StatisticsAssetAgentVersions(string aAssetIds)
        {
            if (isDebugEnabled) logger.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " in");

            DataTable statisticsTable = new DataTable();
            try
            {
                string commandText =
                            "select _agent_version as Item, count(_agent_version) AS Total " +
                            "from assets " +
                            "where _agent_version <> '' ";

                if (aAssetIds != "")
                    commandText += "AND _ASSETID IN (" + aAssetIds + ") ";

                commandText += "group by _agent_version ORDER BY Total DESC";

                if (compactDatabaseType)
                {
                    using (SqlCeConnection conn = DatabaseConnection.CreateOpenCEConnection())
                    {
                        using (SqlCeCommand command = new SqlCeCommand(commandText, conn))
                        {
                            new SqlCeDataAdapter(command).Fill(statisticsTable);
                        }
                    }
                }
                else
                {
                    using (SqlConnection conn = DatabaseConnection.CreateOpenStandardConnection())
                    {
                        using (SqlCommand command = new SqlCommand(commandText, conn))
                        {
                            new SqlDataAdapter(command).Fill(statisticsTable);
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                Utility.DisplayErrorMessage("A database error has occurred in AuditWizard." + Environment.NewLine + Environment.NewLine +
                        "Please see the log file for further details.");
                logger.Error("Exception in " + System.Reflection.MethodBase.GetCurrentMethod().Name, ex);
            }
            catch (SqlCeException ex)
            {
                Utility.DisplayErrorMessage("A database error has occurred in AuditWizard." + Environment.NewLine + Environment.NewLine +
                        "Please see the log file for further details.");
                logger.Error("Exception in " + System.Reflection.MethodBase.GetCurrentMethod().Name, ex);
            }
            catch (Exception ex)
            {
                Utility.DisplayErrorMessage("A database error has occurred in AuditWizard." + Environment.NewLine + Environment.NewLine +
                        "Please see the log file for further details.");

                logger.Error("Exception in " + System.Reflection.MethodBase.GetCurrentMethod().Name, ex);
            }


            if (isDebugEnabled) logger.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " out");
            return statisticsTable;
        }

        public DataTable StatisticsAssetLocations()
        {
            return StatisticsAssetLocations(new AssetDAO().GetSelectedAssets());
        }

        public DataTable StatisticsAssetLocations(string aAssetIds)
        {
            string commandText =
                    "select l._fullname as Item, count(l._locationid) AS Total " +
                    "from locations l " +
                    "INNER JOIN assets a on (a._locationid = l._locationid) ";

            if (aAssetIds != "")
                commandText += "WHERE a._ASSETID IN (" + aAssetIds + ") ";

            commandText +=
                    "group by l._locationid, l._fullname " +
                    "ORDER BY Total DESC"; ;

            return PerformQuery(commandText);
        }

        public DataTable StatisticsAssetLocationsAsGrid(string aAssetIds)
        {
            string commandText =
                    "select a._name as \"Asset Name\", l._fullname as Item " +
                    "from locations l " +
                    "INNER JOIN assets a on (a._locationid = l._locationid) ";

            if (aAssetIds != "")
                commandText += "WHERE a._ASSETID IN (" + aAssetIds + ") ";

            commandText += "ORDER BY \"Asset Name\"";

            return PerformQuery(commandText);
        }

        public DataTable StatisticsAgentStates()
        {
            return StatisticsAgentStates(new AssetDAO().GetSelectedAssets());
        }

        /// <summary>
        /// Return statistics for Agent States
        /// </summary>
        /// <returns></returns>
        public DataTable StatisticsAgentStatesAsGrid(string aAssetIds)
        {
            string commandText =
                            "SELECT _name as \"Asset Name\", " +
                            "CASE _AGENT_STATUS " +
                                "WHEN 0 THEN 'Not Deployed' " +
                                "WHEN 1 THEN 'Deployed (Not Running)' " +
                                "WHEN 2 THEN 'Deployed (Running)' " +
                            "END as Item " +
                            "FROM ASSETS ";

            if (aAssetIds != "")
                commandText += "WHERE _ASSETID IN (" + aAssetIds + ") ";

            commandText += "ORDER BY \"Asset Name\"";

            return PerformQuery(commandText);
        }

        /// <summary>
        /// Return statistics for Agent States
        /// </summary>
        /// <returns></returns>
        public DataTable StatisticsAgentStates(string aAssetIds)
        {
            string commandText =
                            "SELECT " +
                            "CASE _AGENT_STATUS " +
                                "WHEN 0 THEN 'Not Deployed' " +
                                "WHEN 1 THEN 'Deployed (Not Running)' " +
                                "WHEN 2 THEN 'Deployed (Running)' " +
                            "END as Item, COUNT(*) AS Total " +
                            "FROM ASSETS ";

            if (aAssetIds != "")
                commandText += "WHERE _ASSETID IN (" + aAssetIds + ") ";

            commandText += "GROUP BY ASSETS._AGENT_STATUS ORDER BY Total DESC";

            return PerformQuery(commandText);
        }

        public DataTable StatisticsComplianceDrilldown(string aRowLabel)
        {
            int applicationId;
            int licenseCount;
            int installCount;
            DataRow[] rows;
            DataRow[] installRows;
            DataTable myDataTable = new DataTable();
            DataRow resultRow;

            DataColumn myDataColumn = new DataColumn();
            myDataColumn.DataType = Type.GetType("System.String");
            myDataColumn.ColumnName = "Publisher";
            myDataTable.Columns.Add(myDataColumn);

            myDataColumn = new DataColumn();
            myDataColumn.DataType = Type.GetType("System.String");
            myDataColumn.ColumnName = "Application";
            myDataTable.Columns.Add(myDataColumn);

            myDataColumn = new DataColumn();
            myDataColumn.DataType = Type.GetType("System.String");
            myDataColumn.ColumnName = "Version";
            myDataTable.Columns.Add(myDataColumn);

            myDataColumn = new DataColumn();
            myDataColumn.DataType = Type.GetType("System.String");
            myDataColumn.ColumnName = "Compliance Status";
            myDataTable.Columns.Add(myDataColumn);

            myDataColumn = new DataColumn();
            myDataColumn.DataType = Type.GetType("System.Int32");
            myDataColumn.ColumnName = "No. of Licenses";
            myDataTable.Columns.Add(myDataColumn);

            myDataColumn = new DataColumn();
            myDataColumn.DataType = Type.GetType("System.Int32");
            myDataColumn.ColumnName = "Installs";
            myDataTable.Columns.Add(myDataColumn);

            DataTable licensesDataTable = PerformQuery(
                "select a._applicationid, a._publisher, a._name, a._version, sum(l._count) as LicenseCount, a._ignored as Ignored " +
                "from applications a " +
                "left outer join licenses l on (l._applicationid = a._applicationid) " +
                "group by a._applicationid, a._publisher, a._name, a._version, a._ignored " +
                "order by LicenseCount desc");

            DataTable installsDataTable = PerformQuery(
                "SELECT ai._applicationid, COUNT(ai._applicationid) " +
                "FROM APPLICATION_INSTANCES ai " +
                "INNER JOIN ASSETS a ON (a._ASSETID = ai._ASSETID) " +
                "WHERE a._STOCK_STATUS <> 3 " +
                "GROUP BY ai._applicationid");

            DataTable notCountedLicenseDataTable = PerformQuery(
                "select l._applicationid " +
                "from license_types lt " +
                "inner join licenses l on (l._licensetypeid = lt._licensetypeid) " +
                "where lt._counted = 0");

            switch (aRowLabel)
            {
                case "Ignored":

                    try
                    {
                        foreach (DataRow ignoredRow in licensesDataTable.Select("Ignored = 1"))
                        {
                            resultRow = myDataTable.NewRow();
                            resultRow[0] = ignoredRow.ItemArray[1];
                            resultRow[1] = ignoredRow.ItemArray[2];
                            resultRow[2] = ignoredRow.ItemArray[3];
                            resultRow[3] = "Ignored";
                            resultRow[4] = ignoredRow.ItemArray[4];

                            installRows = installsDataTable.Select(String.Format("_APPLICATIONID = {0} ", (int)ignoredRow.ItemArray[0]));
                            resultRow[5] = (installRows.Length == 0) ? "0" : installRows[0].ItemArray[1];

                            //resultRow[5] = installsDataTable.Select(String.Format("_APPLICATIONID = {0} ", (int)ignoredRow.ItemArray[0]))[0].ItemArray[1];
                            myDataTable.Rows.Add(resultRow);
                        }
                    }
                    catch (Exception ex)
                    {
                        logger.Error(ex.Message);
                    }

                    break;

                case "Compliant":

                    foreach (DataRow ignoredRow in licensesDataTable.Select("Ignored = 1"))
                    {
                        licensesDataTable.Rows.Remove(ignoredRow);
                    }

                    foreach (DataRow noLicenseRow in licensesDataTable.Select("LicenseCount IS NULL"))
                    {
                        licensesDataTable.Rows.Remove(noLicenseRow);
                    }

                    foreach (DataRow licenseRow in licensesDataTable.Rows)
                    {
                        try
                        {
                            applicationId = (int)licenseRow.ItemArray[0];
                            licenseCount = (int)licenseRow.ItemArray[4];

                            rows = installsDataTable.Select(String.Format("_APPLICATIONID = {0} ", applicationId));
                            installCount = (rows.Length > 0) ? (int)rows[0].ItemArray[1] : 0;
                            rows = notCountedLicenseDataTable.Select(String.Format("_APPLICATIONID = {0} ", applicationId));

                            if ((installCount <= licenseCount) || (rows.Length > 0))
                            {
                                resultRow = myDataTable.NewRow();
                                resultRow[0] = licenseRow.ItemArray[1];
                                resultRow[1] = licenseRow.ItemArray[2];
                                resultRow[2] = licenseRow.ItemArray[3];
                                resultRow[3] = "Compliant";
                                resultRow[4] = licenseRow.ItemArray[4];

                                installRows = installsDataTable.Select(String.Format("_APPLICATIONID = {0} ", (int)licenseRow.ItemArray[0]));
                                resultRow[5] = (installRows.Length == 0) ? "0" : installRows[0].ItemArray[1];

                                //resultRow[5] = installsDataTable.Select(String.Format("_APPLICATIONID = {0} ", (int)licenseRow.ItemArray[0]))[0].ItemArray[1];
                                myDataTable.Rows.Add(resultRow);
                            }
                        }
                        catch (Exception ex)
                        {
                            logger.Error(ex.Message);
                        }
                    }
                    break;

                case "Non-Compliant":

                    foreach (DataRow ignoredRow in licensesDataTable.Select("Ignored = 1"))
                    {
                        licensesDataTable.Rows.Remove(ignoredRow);
                    }

                    foreach (DataRow noLicenseRow in licensesDataTable.Select("LicenseCount IS NULL"))
                    {
                        licensesDataTable.Rows.Remove(noLicenseRow);
                    }

                    foreach (DataRow licenseRow in licensesDataTable.Rows)
                    {
                        try
                        {
                            applicationId = (int)licenseRow.ItemArray[0];
                            licenseCount = (int)licenseRow.ItemArray[4];

                            rows = notCountedLicenseDataTable.Select(String.Format("_APPLICATIONID = {0} ", applicationId));

                            if (rows.Length > 0)
                                continue;

                            rows = installsDataTable.Select(String.Format("_APPLICATIONID = {0} ", applicationId));
                            installCount = (int)rows[0].ItemArray[1];

                            if (installCount > licenseCount)
                            {
                                resultRow = myDataTable.NewRow();
                                resultRow[0] = licenseRow.ItemArray[1];
                                resultRow[1] = licenseRow.ItemArray[2];
                                resultRow[2] = licenseRow.ItemArray[3];
                                resultRow[3] = "Non-Compliant";
                                resultRow[4] = licenseRow.ItemArray[4];

                                installRows = installsDataTable.Select(String.Format("_APPLICATIONID = {0} ", (int)licenseRow.ItemArray[0]));
                                resultRow[5] = (installRows.Length == 0) ? "0" : installRows[0].ItemArray[1];

                                //resultRow[5] = installsDataTable.Select(String.Format("_APPLICATIONID = {0} ", (int)licenseRow.ItemArray[0]))[0].ItemArray[1];
                                myDataTable.Rows.Add(resultRow);
                            }
                        }
                        catch (Exception ex)
                        {
                            logger.Error(ex.Message);
                        }
                    }
                    break;

                case "Not Defined":

                    foreach (DataRow ignoredRow in licensesDataTable.Select("LicenseCount IS NULL AND Ignored = 0"))
                    {
                        try
                        {
                            resultRow = myDataTable.NewRow();
                            resultRow[0] = ignoredRow.ItemArray[1];
                            resultRow[1] = ignoredRow.ItemArray[2];
                            resultRow[2] = ignoredRow.ItemArray[3];
                            resultRow[3] = "Not Defined";
                            resultRow[4] = ignoredRow.ItemArray[4];

                            installRows = installsDataTable.Select(String.Format("_APPLICATIONID = {0} ", (int)ignoredRow.ItemArray[0]));

                            if (installRows.Length == 0) continue;

                            resultRow[5] = installRows[0].ItemArray[1];

                            myDataTable.Rows.Add(resultRow);
                        }
                        catch (Exception ex)
                        {
                            logger.Error(ex.Message);
                        }
                    }
                    break;
            }

            myDataTable.DefaultView.Sort = "Publisher ASC";
            return myDataTable;
        }

        public DataTable StatisticsComplianceByType(string aPublisher)
        {
            return StatisticsComplianceByType(aPublisher, new AssetDAO().GetSelectedAssets());
        }

        public DataTable StatisticsComplianceByType(string aPublisher, string aAssetIds)
        {
            string whereClause = String.Empty;
            DataTable myDataTable = new DataTable();
            double compliantApps = 0.0;
            double nonCompliantApps = 0.0;
            double notDefinedApps = 0.0;
            double ignoredApps = 0.0;

            if (aPublisher != String.Empty)
                whereClause = String.Format("where a._publisher = '{0}'", aPublisher);

			// 8.3.4 - CMD - BUG#1106
			// Add _aliased_toid to the Select so that we can detect aliased applications in the 'Not Defined' counts
			//
            string commandText = String.Format(
                "SELECT a._applicationid, SUM(l._count) as LicenseCount, a._ignored as Ignored, a._aliased_toid as AliasedTo " +
                "FROM applications a " +
                "LEFT JOIN LICENSES l on (l._applicationid = a._applicationid) " +
                "{0} ", whereClause);

			commandText += "group by a._applicationid, a._ignored, a._aliased_toid order by LicenseCount desc";

            DataTable licensesDataTable = PerformQuery(commandText);

            DataRow[] rows = licensesDataTable.Select("Ignored = 1");
            ignoredApps = rows.Length;

            foreach (DataRow ignoredRow in rows)
            {
                licensesDataTable.Rows.Remove(ignoredRow);
            }

			// 8.3.4 - CMD - BUG#1106
			// Add _aliased_toid to the Select so that we can detect aliased applications in the 'Not Defined' counts
			//
			DataRow[] noLicenseRows = licensesDataTable.Select("LicenseCount IS NULL AND AliasedTo = 0");
            notDefinedApps = noLicenseRows.Length;

            foreach (DataRow noLicenseRow in noLicenseRows)
            {
                licensesDataTable.Rows.Remove(noLicenseRow);
            }

            if (aPublisher != String.Empty)
                whereClause = String.Format("where ai._publisher = '{0}'", aPublisher);

            //commandText =
            //    "select ai._applicationid, count(ai._applicationid) " +
            //    "from application_instances ai ";

            //if (aAssetIds != "")
            //    commandText += "WHERE ai._ASSETID IN (" + aAssetIds + ") ";

            //commandText += "group by ai._applicationid";

            commandText =
                "SELECT ai._applicationid, COUNT(ai._applicationid) " +
                "FROM APPLICATION_INSTANCES ai " +
                "INNER JOIN ASSETS a ON (a._ASSETID = ai._ASSETID) " +
                "WHERE a._STOCK_STATUS <> 3 ";

            if (aAssetIds != "")
                commandText += "AND ai._ASSETID IN (" + aAssetIds + ") ";

            commandText += "GROUP BY ai._applicationid";


            DataTable installsDataTable = PerformQuery(commandText);

            DataTable notCountedLicenseDataTable = PerformQuery(String.Format(
                "select l._applicationid " +
                "from license_types lt " +
                "inner join licenses l on (l._licensetypeid = lt._licensetypeid) " +
                "where lt._counted = 0", whereClause));

            foreach (DataRow licenseRow in licensesDataTable.Rows)
            {
                try
                {
                    int applicationId = (int)licenseRow.ItemArray[0];
                    int licenseCount = (int)licenseRow.ItemArray[1];

                    rows = notCountedLicenseDataTable.Select(String.Format("_APPLICATIONID = {0} ", applicationId));
                    if (rows.Length > 0)
                    {
                        compliantApps++;
                        continue;
                    }

                    rows = installsDataTable.Select(String.Format("_APPLICATIONID = {0} ", applicationId));

                    if (rows.Length > 0)
                    {
                        int installCount = (int)rows[0].ItemArray[1];

                        if (installCount <= licenseCount)
                            compliantApps++;
                        else
                            nonCompliantApps++;
                    }
                    else
                        compliantApps++;
                }
                catch (Exception ex)
                {
                    logger.Error(ex.Message);
                }
            }

            DataColumn myDataColumn = new DataColumn();
            myDataColumn.DataType = Type.GetType("System.String");
            myDataColumn.ColumnName = "Item";
            myDataTable.Columns.Add(myDataColumn);

            myDataColumn = new DataColumn();
            myDataColumn.DataType = Type.GetType("System.Int32");
            myDataColumn.ColumnName = "Total";
            myDataTable.Columns.Add(myDataColumn);

            DataRow row = myDataTable.NewRow();
            row["Item"] = "Compliant";
            row["Total"] = compliantApps;
            myDataTable.Rows.Add(row);

            row = myDataTable.NewRow();
            row["Item"] = "Non-Compliant";
            row["Total"] = nonCompliantApps;
            myDataTable.Rows.Add(row);

            row = myDataTable.NewRow();
            row["Item"] = "Ignored";
            row["Total"] = ignoredApps;
            myDataTable.Rows.Add(row);

            row = myDataTable.NewRow();
            row["Item"] = "Not Defined";
            row["Total"] = notDefinedApps;
            myDataTable.Rows.Add(row);

            return myDataTable;
        }

        public DataTable StatisticsComplianceByApplicationDrilldown(string aRowLabel)
        {
            string publisher = aRowLabel.Split('|')[0];
            string complianceState = aRowLabel.Split('|')[1];

            string whereClause = String.Empty;
            int applicationId;
            int licenseCount;
            int installCount;
            DataRow[] rows;
            DataTable myDataTable = new DataTable();
            DataRow resultRow;

            DataColumn myDataColumn;

            myDataColumn = new DataColumn();
            myDataColumn.DataType = Type.GetType("System.String");
            myDataColumn.ColumnName = "Publisher";
            myDataTable.Columns.Add(myDataColumn);

            myDataColumn = new DataColumn();
            myDataColumn.DataType = Type.GetType("System.String");
            myDataColumn.ColumnName = "Application";
            myDataTable.Columns.Add(myDataColumn);

            myDataColumn = new DataColumn();
            myDataColumn.DataType = Type.GetType("System.String");
            myDataColumn.ColumnName = "Version";
            myDataTable.Columns.Add(myDataColumn);

            myDataColumn = new DataColumn();
            myDataColumn.DataType = Type.GetType("System.String");
            myDataColumn.ColumnName = "Compliance Status";
            myDataTable.Columns.Add(myDataColumn);

            myDataColumn = new DataColumn();
            myDataColumn.DataType = Type.GetType("System.Int32");
            myDataColumn.ColumnName = "No. of Licenses";
            myDataTable.Columns.Add(myDataColumn);

            myDataColumn = new DataColumn();
            myDataColumn.DataType = Type.GetType("System.Int32");
            myDataColumn.ColumnName = "Installs";
            myDataTable.Columns.Add(myDataColumn);

            DataTable licensesDataTable = PerformQuery(String.Format(
                "select a._applicationid, a._publisher, a._name, a._version, sum(l._count) as LicenseCount, a._ignored as Ignored " +
                "from applications a " +
                "left outer join licenses l on (l._applicationid = a._applicationid) " +
                "where a._publisher = '{0}' " +
                "group by a._applicationid, a._publisher, a._name, a._version, a._ignored " +
                "order by LicenseCount desc", publisher));

            DataTable installsDataTable = PerformQuery(
                "select ai._applicationid, count(ai._applicationid) " +
                "from application_instances ai " +
                "group by ai._applicationid");

            DataTable notCountedLicenseDataTable = PerformQuery(
                "select l._applicationid " +
                "from license_types lt " +
                "inner join licenses l on (l._licensetypeid = lt._licensetypeid) " +
                "where lt._counted = 0");

            switch (complianceState)
            {
                case "Ignored":

                    foreach (DataRow ignoredRow in licensesDataTable.Select("Ignored = 1"))
                    {
                        resultRow = myDataTable.NewRow();
                        resultRow[0] = ignoredRow.ItemArray[1];
                        resultRow[1] = ignoredRow.ItemArray[2];
                        resultRow[2] = ignoredRow.ItemArray[3];
                        resultRow[3] = "Ignored";
                        resultRow[4] = ignoredRow.ItemArray[4];
                        resultRow[5] = installsDataTable.Select(String.Format("_APPLICATIONID = {0} ", (int)ignoredRow.ItemArray[0]))[0].ItemArray[1];
                        myDataTable.Rows.Add(resultRow);
                    }

                    break;

                case "Compliant":

                    foreach (DataRow ignoredRow in licensesDataTable.Select("Ignored = 1"))
                    {
                        licensesDataTable.Rows.Remove(ignoredRow);
                    }

                    foreach (DataRow noLicenseRow in licensesDataTable.Select("LicenseCount IS NULL"))
                    {
                        licensesDataTable.Rows.Remove(noLicenseRow);
                    }

                    foreach (DataRow licenseRow in licensesDataTable.Rows)
                    {
                        try
                        {
                            applicationId = (int)licenseRow.ItemArray[0];
                            licenseCount = (int)licenseRow.ItemArray[4];

                            rows = installsDataTable.Select(String.Format("_APPLICATIONID = {0} ", applicationId));

                            installCount = (rows.Length == 0) ? 0 : (int)rows[0].ItemArray[1];

                            rows = notCountedLicenseDataTable.Select(String.Format("_APPLICATIONID = {0} ", applicationId));

                            if ((installCount <= licenseCount) || (rows.Length > 0))
                            {
                                resultRow = myDataTable.NewRow();
                                resultRow[0] = licenseRow.ItemArray[1];
                                resultRow[1] = licenseRow.ItemArray[2];
                                resultRow[2] = licenseRow.ItemArray[3];
                                resultRow[3] = "Compliant";
                                resultRow[4] = licenseRow.ItemArray[4];
                                resultRow[5] = installCount;
                                //resultRow[5] = installsDataTable.Select(String.Format("_APPLICATIONID = {0} ", (int)licenseRow.ItemArray[0]))[0].ItemArray[1];
                                myDataTable.Rows.Add(resultRow);
                            }
                        }
                        catch (Exception ex)
                        {
                            logger.Error(ex.Message);
                        }
                    }
                    break;

                case "Non-Compliant":

                    foreach (DataRow ignoredRow in licensesDataTable.Select("Ignored = 1"))
                    {
                        licensesDataTable.Rows.Remove(ignoredRow);
                    }

                    foreach (DataRow noLicenseRow in licensesDataTable.Select("LicenseCount IS NULL"))
                    {
                        licensesDataTable.Rows.Remove(noLicenseRow);
                    }

                    foreach (DataRow licenseRow in licensesDataTable.Rows)
                    {
                        try
                        {
                            applicationId = (int)licenseRow.ItemArray[0];
                            licenseCount = (int)licenseRow.ItemArray[4];

                            rows = notCountedLicenseDataTable.Select(String.Format("_APPLICATIONID = {0} ", applicationId));

                            if (rows.Length > 0)
                                continue;

                            rows = installsDataTable.Select(String.Format("_APPLICATIONID = {0} ", applicationId));
                            installCount = (int)rows[0].ItemArray[1];

                            if (installCount > licenseCount)
                            {
                                resultRow = myDataTable.NewRow();
                                resultRow[0] = licenseRow.ItemArray[1];
                                resultRow[1] = licenseRow.ItemArray[2];
                                resultRow[2] = licenseRow.ItemArray[3];
                                resultRow[3] = "Non-Compliant";
                                resultRow[4] = licenseRow.ItemArray[4];
                                resultRow[5] = installsDataTable.Select(String.Format("_APPLICATIONID = {0} ", (int)licenseRow.ItemArray[0]))[0].ItemArray[1];
                                myDataTable.Rows.Add(resultRow);
                            }
                        }
                        catch (Exception ex)
                        {
                            logger.Error(ex.Message);
                        }
                    }
                    break;

                case "Not Defined":

                    foreach (DataRow ignoredRow in licensesDataTable.Select("LicenseCount IS NULL AND Ignored = 0"))
                    {
                        resultRow = myDataTable.NewRow();
                        resultRow[0] = ignoredRow.ItemArray[1];
                        resultRow[1] = ignoredRow.ItemArray[2];
                        resultRow[2] = ignoredRow.ItemArray[3];
                        resultRow[3] = "Not Defined";
                        //resultRow[4] = ignoredRow.ItemArray[4];
                        //resultRow[4] = "";

                        DataRow[] myRows = installsDataTable.Select(String.Format("_APPLICATIONID = {0} ", (int)ignoredRow.ItemArray[0]));
                        resultRow[5] = (myRows.Length == 0) ? 0 : myRows[0].ItemArray[1];
                        myDataTable.Rows.Add(resultRow);
                    }
                    break;
            }

            return myDataTable;
        }

        /// <summary>
        /// Return drilldown statistics for Applications Table
        /// </summary>
        /// <returns></returns>
        public DataTable StatisticsTopApplicationsDrilldown(string aApplicationName, string aApplicationVersion)
        {
            return PerformQuery(String.Format(
                "SELECT _PUBLISHER As Publisher, _NAME As Name, _VERSION As Version FROM APPLICATIONS " +
                "WHERE _NAME = '{0}' " +
                "AND _VERSION = '{1}'", aApplicationName, aApplicationVersion));
        }

        /// <summary>
        /// Return drilldown statistics for Asset Table
        /// </summary>
        /// <returns></returns>
        public DataTable StatisticsAssetsStatusDrilldown(string aRowLabel)
        {
            switch (aRowLabel)
            {
                case "In Stock":
                    aRowLabel = "0";
                    break;
                case "In Use":
                    aRowLabel = "1";
                    break;
                case "Pending Disposal":
                    aRowLabel = "2";
                    break;
                case "Disposed":
                    aRowLabel = "3";
                    break;
                default:
                    aRowLabel = "0";
                    break;
            }

            return PerformQuery(String.Format(
                "SELECT _NAME as \"Asset Name\", _MAKE as Make, _MODEL as Model, " +
                "case " +
                "when cast(_STOCK_STATUS as int) = 0 then 'In Stock' " +
                "when cast(_STOCK_STATUS as int) = 1 then 'In Use' " +
                "when cast(_STOCK_STATUS as int) = 2 then 'Pending Disposal' " +
                "when cast(_STOCK_STATUS as int) = 3 then 'Disposed' " +
                "else 'Unknown' end as Status " +
                "FROM ASSETS " +
                "WHERE _STOCK_STATUS = '{0}' " +
                "AND _ASSETID IN ({1})", aRowLabel, new AssetDAO().GetSelectedAssets()));
        }

        /// <summary>
        /// Return drilldown statistics
        /// </summary>
        /// <returns></returns>
        public DataTable StatisticsTopPublisherDrilldown(string aRowLabel)
        {
            string commandText = String.Format(
                    "SELECT _PUBLISHER As Publisher, _NAME As Name, _VERSION As Version FROM APPLICATIONS " +
                    "WHERE _PUBLISHER = '{0}'", aRowLabel);

            return PerformQuery(commandText);
        }

        /// <summary>
        /// Return drilldown statistics
        /// </summary>
        /// <returns></returns>
        public DataTable StatisticsSystemProcessorsDrilldown(string aRowLabel)
        {
            string commandText = String.Format(
                    "SELECT a._NAME as \"Asset Name\", a._MAKE as Make, a._MODEL as Model, ai._VALUE as Processor " +
                    "FROM AUDITEDITEMS ai, ASSETS a " +
                    "WHERE a._ASSETID = ai._ASSETID " +
                    "AND _VALUE = '{0}' " +
                    "AND a._ASSETID IN ({1})", aRowLabel, new AssetDAO().GetSelectedAssets());

            return PerformQuery(commandText);
        }

        /// <summary>
        /// Return drilldown statistics
        /// </summary>
        /// <returns></returns>
        public DataTable StatisticsAssetAuditedDrilldown(string aRowLabel)
        {
            string lWhereClause = (aRowLabel == "Audited") ? "WHERE _LASTAUDIT IS NOT NULL " : "WHERE _LASTAUDIT IS NULL ";

            string commandText = String.Format(
                    "SELECT _NAME as \"Asset Name\", _MAKE as Make, _MODEL as Model, _LASTAUDIT as LastAudit " +
                    "FROM ASSETS " +
                    "{0} " +
                    "AND _ASSETID IN ({1})", lWhereClause, new AssetDAO().GetSelectedAssets());

            return PerformQuery(commandText);
        }

        /// <summary>
        /// Return drilldown statistics
        /// </summary>
        /// <returns></returns>
        public DataTable StatisticsAssetTypesDrilldown(string aRowLabel)
        {
            string commandText = String.Format(
                    "SELECT a._NAME as \"Asset Name\", a._MAKE as Make, a._MODEL as Model, at._NAME as AssetType " +
                    "FROM ASSETS a, ASSET_TYPES at " +
                    "WHERE a._assettypeid = at._assettypeid " +
                    "AND at._NAME ='{0}' " +
                    "AND a._ASSETID IN ({1})", aRowLabel, new AssetDAO().GetSelectedAssets());

            return PerformQuery(commandText);
        }

        /// <summary>
        /// Return drilldown statistics
        /// </summary>
        /// <returns></returns>
        public DataTable StatisticsRAMCapacityDrilldown(string aRowLabel)
        {
            string lWhereClause = String.Empty;

            switch (aRowLabel)
            {
                case "0 - 500MB":
                    lWhereClause = "AND ai._VALUE > 0 AND ai._VALUE <= 500";
                    break;
                case "501MB - 1GB":
                    lWhereClause = "AND ai._VALUE > 500 AND ai._VALUE <= 1000";
                    break;
                case "1GB - 1.5GB":
                    lWhereClause = "AND ai._VALUE > 1000 AND ai._VALUE <= 1500";
                    break;
                case "1.5GB - 2GB":
                    lWhereClause = "AND ai._VALUE > 1500 AND ai._VALUE <= 2000";
                    break;
                case "2GB - 3GB":
                    lWhereClause = "AND ai._VALUE > 2000 AND ai._VALUE <= 3000";
                    break;
                case "3GB - 4GB":
                    lWhereClause = "AND ai._VALUE > 3000 AND ai._VALUE <= 4000";
                    break;
                case "4GB - 5GB":
                    lWhereClause = "AND ai._VALUE > 4000 AND ai._VALUE <= 5000";
                    break;
                case "5GB - 6GB":
                    lWhereClause = "AND ai._VALUE > 5000 AND ai._VALUE <= 6000";
                    break;
                case "6GB - 7GB":
                    lWhereClause = "AND ai._VALUE > 6000 AND ai._VALUE <= 7000";
                    break;
                case "> 7GB":
                    lWhereClause = "AND ai._VALUE > 7000";
                    break;
            }

            string commandText = String.Format(
                    "SELECT a._NAME as \"Asset Name\", a._MAKE as Make, a._MODEL as Model, ai._VALUE as \"Total RAM\" " +
                    "FROM ASSETS a, AUDITEDITEMS ai " +
                    "WHERE ai._CATEGORY='Hardware|Memory' AND ai._NAME='Total RAM' " +
                    "AND a._ASSETID = ai._ASSETID " + lWhereClause +
                    " AND a._ASSETID IN ({0})", new AssetDAO().GetSelectedAssets());

            return PerformQuery(commandText);
        }

        /// <summary>
        /// Return drilldown statistics
        /// </summary>
        /// <returns></returns>
        public DataTable StatisticsProcessorSpeedsDrilldown(string aRowLabel)
        {
            string lWhereClause = String.Empty;

            string commandText = String.Format(
                "SELECT a._NAME as \"Asset Name\", a._MAKE as Make, a._MODEL as Model, CAST(ai._VALUE AS INT) as Speed " +
                "FROM ASSETS a, AUDITEDITEMS ai " +
                "WHERE ai._CATEGORY='Hardware|CPU' AND ai._NAME='Speed' " +
                "AND a._ASSETID = ai._ASSETID " +
                "AND a._ASSETID IN ({0})", new AssetDAO().GetSelectedAssets());

            DataTable resultsTable = PerformQuery(commandText); ;

            switch (aRowLabel)
            {
                case "0 - 1GHz":
                    lWhereClause = "Speed > 0 AND Speed <= 1000";
                    break;
                case "1.0GHz - 1.5GHz":
                    lWhereClause = "Speed > 1000 AND Speed <= 1500";
                    break;
                case "1.5GHz - 2.0GHz":
                    lWhereClause = "Speed > 1500 AND Speed <= 2000";
                    break;
                case "2GHz - 2.5GHz":
                    lWhereClause = "Speed > 2000 AND Speed <= 2500";
                    break;
                case "2.5GHz - 3.0GHz":
                    lWhereClause = "Speed > 2500 AND Speed <= 3000";
                    break;
                case "3GHz - 3.5GHz":
                    lWhereClause = "Speed > 3000 AND Speed <= 3500";
                    break;
                case "3.5GHz - 4.0GHz":
                    lWhereClause = "Speed > 3500 AND Speed <= 4000";
                    break;
                case "4GHz - 4.5GHz":
                    lWhereClause = "Speed > 4000 AND Speed <= 4500";
                    break;
                case "4.5GHz - 5.0GHz":
                    lWhereClause = "Speed > 4500 AND Speed <= 5000";
                    break;
                case ">5GHz":
                    lWhereClause = "Speed > 5000";
                    break;
            }

            DataRow[] rows = resultsTable.Select(lWhereClause);
            DataTable returnTable = resultsTable.Clone();

            foreach (DataRow dataRow in rows)
            {
                returnTable.ImportRow(dataRow);
            }

            return returnTable;
        }

        public DataTable PerformCustomReportQuery(SqlConnection aConn, string aCommandText)
        {
            DataTable statisticsTable = new DataTable();
            try
            {
                if (compactDatabaseType)
                {
                    using (SqlCeConnection conn = DatabaseConnection.CreateOpenCEConnection())
                    {
                        using (SqlCeCommand command = new SqlCeCommand(aCommandText, conn))
                        {
                            new SqlCeDataAdapter(command).Fill(statisticsTable);
                        }
                    }
                }
                else
                {
                    using (SqlCommand command = new SqlCommand(aCommandText, aConn))
                    {
                        new SqlDataAdapter(command).Fill(statisticsTable);
                    }
                }
            }
            catch (SqlException ex)
            {
                Utility.DisplayErrorMessage("A database error has occurred in AuditWizard." + Environment.NewLine + Environment.NewLine +
                        "Please see the log file for further details.");
                logger.Error("Exception in " + System.Reflection.MethodBase.GetCurrentMethod().Name, ex);
            }
            catch (SqlCeException ex)
            {
                Utility.DisplayErrorMessage("A database error has occurred in AuditWizard." + Environment.NewLine + Environment.NewLine +
                        "Please see the log file for further details.");
                logger.Error("Exception in " + System.Reflection.MethodBase.GetCurrentMethod().Name, ex);
            }
            catch (Exception ex)
            {
                Utility.DisplayErrorMessage("A database error has occurred in AuditWizard." + Environment.NewLine + Environment.NewLine +
                        "Please see the log file for further details.");

                logger.Error("Exception in " + System.Reflection.MethodBase.GetCurrentMethod().Name, ex);
            }

            return statisticsTable;
        }

        /// <summary>
        /// Return drilldown statistics for Asset Table
        /// </summary>
        /// <returns></returns>
        public DataTable PerformQuery(string aCommandText)
        {
            DataTable statisticsTable = new DataTable();
            try
            {
                if (compactDatabaseType)
                {
                    using (SqlCeConnection conn = DatabaseConnection.CreateOpenCEConnection())
                    {
                        using (SqlCeCommand command = new SqlCeCommand(aCommandText, conn))
                        {
                            new SqlCeDataAdapter(command).Fill(statisticsTable);
                        }
                    }
                }
                else
                {
                    using (SqlConnection conn = DatabaseConnection.CreateOpenStandardConnection())
                    {
                        using (SqlCommand command = new SqlCommand(aCommandText, conn))
                        {
                            new SqlDataAdapter(command).Fill(statisticsTable);
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                Utility.DisplayErrorMessage("A database error has occurred in AuditWizard." + Environment.NewLine + Environment.NewLine +
                        "Please see the log file for further details.");
                logger.Error("Exception in " + System.Reflection.MethodBase.GetCurrentMethod().Name, ex);
            }
            catch (SqlCeException ex)
            {
                Utility.DisplayErrorMessage("A database error has occurred in AuditWizard." + Environment.NewLine + Environment.NewLine +
                        "Please see the log file for further details.");
                logger.Error("Exception in " + System.Reflection.MethodBase.GetCurrentMethod().Name, ex);
            }
            catch (Exception ex)
            {
                Utility.DisplayErrorMessage("A database error has occurred in AuditWizard." + Environment.NewLine + Environment.NewLine +
                        "Please see the log file for further details.");

                logger.Error("Exception in " + System.Reflection.MethodBase.GetCurrentMethod().Name, ex);
            }

            return statisticsTable;
        }


        /// <summary>
        /// Return drilldown statistics for Asset Table
        /// </summary>
        /// <returns></returns>
        public DataTable StatisticsAssetLocationDrilldown(string aRowLabel)
        {
            return PerformQuery(String.Format(
                    "SELECT a._NAME as \"Asset Name\", a._MAKE as Make, a._MODEL as Model, l._fullname as Location " +
                    "FROM ASSETS a, LOCATIONS l " +
                    "WHERE a._locationid = l._locationid " +
                    "AND l._fullname = '{0}' " +
                    "AND a._ASSETID IN ({1})", aRowLabel, new AssetDAO().GetSelectedAssets()));
        }

        /// <summary>
        /// Return drilldown statistics
        /// </summary>
        /// <returns></returns>
        public DataTable StatisticsTopPublishersDrilldown(string aRowLabel)
        {
            return PerformQuery(String.Format(
                    "SELECT _NAME as Name, _MAKE as Make, _MODEL as Model FROM ASSETS " +
                    "WHERE _locationid in (select _locationid from locations where _fullname = '{0}'", aRowLabel));
        }

        public DataTable StatisticsOverLicensedApplications()
        {
            return StatisticsOverLicensedApplications(new AssetDAO().GetSelectedAssets());
        }

        public DataTable StatisticsOverLicensedApplicationsForDashboard()
        {
            string lAssets = "";
            DataTable lAllAssetDataTable = new AssetDAO().GetAllAssetIds();

            foreach (DataRow assetRow in lAllAssetDataTable.Rows)
            {
                lAssets += assetRow[0].ToString() + ",";
            }

            return StatisticsOverLicensedApplications(lAssets.TrimEnd(','));
        }

        /// <summary>
        /// Return over compliant software applications
        /// </summary>
        /// <returns></returns>
        public DataTable StatisticsOverLicensedApplications(string aAssetIds)
        {
            DataRow[] rows;
            DataTable resultsDataTable = new DataTable();
            ApplicationsDAO lApplicationsDao = new ApplicationsDAO();
            LicensesDAO lLicenseDao = new LicensesDAO();

            string commandText =
                "SELECT a._applicationid, a._name as Application, a._version as Version, SUM(l._count) as LicenseCount " +
                "FROM licenses l " +
                "LEFT JOIN applications a on (a._applicationid = l._applicationid) " +
                "WHERE a._ignored = 0 ";

            commandText += "GROUP BY a._applicationid, a._name, a._version";

            DataTable dataTable = PerformQuery(commandText);

            DataColumn dataColumn = new DataColumn();
            dataColumn.DataType = Type.GetType("System.String");
            dataColumn.ColumnName = "Application";
            resultsDataTable.Columns.Add(dataColumn);

            dataColumn = new DataColumn();
            dataColumn.DataType = Type.GetType("System.Int32");
            dataColumn.ColumnName = "Number of Installs";
            resultsDataTable.Columns.Add(dataColumn);

            dataColumn = new DataColumn();
            dataColumn.DataType = Type.GetType("System.Int32");
            dataColumn.ColumnName = "License Declared";
            resultsDataTable.Columns.Add(dataColumn);

            commandText =
                "SELECT ai._applicationid, COUNT(ai._applicationid) " +
                "FROM application_instances ai ";

            if (aAssetIds != "")
                commandText += "WHERE ai._ASSETID IN (" + aAssetIds + ") ";

            commandText += "GROUP BY ai._applicationid";

            DataTable installsDataTable = PerformQuery(commandText);

            DataTable notCountedLicensesTable = PerformQuery(
                "SELECT l._applicationid " +
                "FROM license_types lt " +
                "INNER JOIN licenses l ON (l._licensetypeid = lt._licensetypeid) " +
                "WHERE lt._counted = 0");

            List<int> aliasedToApplicationsList = new List<int>();

            foreach (DataRow dataRow in lApplicationsDao.GetAliasedToApplications().Rows)
            {
                aliasedToApplicationsList.Add(Convert.ToInt32(dataRow[0]));
            }

            foreach (DataRow row in dataTable.Rows)
            {
                int applicationId = (int)row.ItemArray[0];
                int licenseCount = (int)row.ItemArray[3];

                rows = notCountedLicensesTable.Select(String.Format("_APPLICATIONID = {0} ", applicationId));

                // if there is an unlimited license associated with this application, we treat it as correctly licensed
                if (rows.Length > 0) continue;

                rows = installsDataTable.Select(String.Format("_APPLICATIONID = {0} ", applicationId));

                int installationCount;

                if (rows.Length == 0)
                    installationCount = 0;
                else
                    installationCount = (int)rows[0].ItemArray[1];

                // need to now check if this application is the target of any aliases
                // if so, we need to get their licenses and add to this application
                // find any applications which are aliased to this application as we also need their licenses
                bool foundNonCountedLicense = false;

                if (aliasedToApplicationsList.Contains(applicationId))
                {
                    foreach (DataRow dataRow in lApplicationsDao.GetAliasedApplicationsByApplicationId(applicationId).Rows)
                    {
                        if (foundNonCountedLicense) continue;

                        int aliasedApplicationId = Convert.ToInt32(dataRow[0]);

                        if (lLicenseDao.GetNotCountedLicenseCountByApplicationId(aliasedApplicationId) > 0)
                        {
                            // one of the aliased apps had an unlimited license - means the aliased to app is now compliant
                            foundNonCountedLicense = true;
                            continue;
                        }
                        else
                        {
                            // get all of the licenses for this aliased app and add them to the parent
                            DataTable licenseCountTable = lLicenseDao.GetLicenseCountByApplicationId(aliasedApplicationId);
                            if (licenseCountTable.Rows.Count > 0)
                            {
                                licenseCount += Convert.ToInt32(licenseCountTable.Rows[0][0]);
                            }
                        }
                    }
                }

                if (foundNonCountedLicense) continue;
                if (installationCount >= licenseCount) continue;

                string applicationName = (string)row.ItemArray[1];
                string applicationVersion = (string)row.ItemArray[2];

                DataRow newDataRow = resultsDataTable.NewRow();
                newDataRow[0] = (applicationName.EndsWith(applicationVersion) || applicationVersion == "") ? applicationName : applicationName + " (v" + applicationVersion + ")";
                newDataRow[1] = installationCount;
                newDataRow[2] = licenseCount - installationCount;

                resultsDataTable.Rows.Add(newDataRow);
            }

			resultsDataTable.DefaultView.Sort = "License Declared asc";
            return resultsDataTable;
        }

        private int PerformScalarQuery(string commandText)
        {
            int returnValue = 0;

            try
            {
                if (compactDatabaseType)
                {
                    using (SqlCeConnection conn = DatabaseConnection.CreateOpenCEConnection())
                    {
                        using (SqlCeCommand command = new SqlCeCommand(commandText, conn))
                        {
                            returnValue = Convert.ToInt32(command.ExecuteScalar());
                        }
                    }
                }
                else
                {
                    using (SqlConnection conn = DatabaseConnection.CreateOpenStandardConnection())
                    {
                        using (SqlCommand command = new SqlCommand(commandText, conn))
                        {
                            returnValue = Convert.ToInt32(command.ExecuteScalar());
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                Utility.DisplayErrorMessage("A database error has occurred in AuditWizard." + Environment.NewLine + Environment.NewLine +
                        "Please see the log file for further details.");
                logger.Error("Exception in " + System.Reflection.MethodBase.GetCurrentMethod().Name, ex);
            }
            catch (SqlCeException ex)
            {
                Utility.DisplayErrorMessage("A database error has occurred in AuditWizard." + Environment.NewLine + Environment.NewLine +
                        "Please see the log file for further details.");
                logger.Error("Exception in " + System.Reflection.MethodBase.GetCurrentMethod().Name, ex);
            }
            catch (Exception ex)
            {
                Utility.DisplayErrorMessage("A database error has occurred in AuditWizard." + Environment.NewLine + Environment.NewLine +
                        "Please see the log file for further details.");

                logger.Error("Exception in " + System.Reflection.MethodBase.GetCurrentMethod().Name, ex);
            }

            return returnValue;
        }

        public DataTable StatisticsUnderLicensedApplications()
        {
            return StatisticsUnderLicensedApplications(new AssetDAO().GetSelectedAssets());
        }

        public DataTable StatisticsUnderLicensedApplicationsForDashboard()
        {
            string lAssets = "";
            DataTable lAllAssetDataTable = new AssetDAO().GetAllAssetIds();

            foreach (DataRow assetRow in lAllAssetDataTable.Rows)
            {
                lAssets += assetRow[0].ToString() + ",";
            }

            return StatisticsUnderLicensedApplications(lAssets.TrimEnd(','));
        }

        /// <summary>
        /// Return under compliant software applications
        /// </summary>
        /// <returns></returns>
        public DataTable StatisticsUnderLicensedApplications(string aAssetIds)
        {
            DataRow[] rows;
            DataTable resultsDataTable = new DataTable();
            ApplicationsDAO lApplicationsDao = new ApplicationsDAO();
            LicensesDAO lLicenseDao = new LicensesDAO();

            string commandText =
                "SELECT a._applicationid, a._name as Application, a._version as Version, SUM(l._count) as LicenseCount " +
                "FROM licenses l " +
                "LEFT JOIN applications a on (a._applicationid = l._applicationid) " +
                "WHERE a._ignored = 0 ";

            commandText += "GROUP BY a._applicationid, a._name, a._version";

            DataTable dataTable = PerformQuery(commandText);

            DataColumn dataColumn = new DataColumn();
            dataColumn.DataType = Type.GetType("System.String");
            dataColumn.ColumnName = "Application";
            resultsDataTable.Columns.Add(dataColumn);

            dataColumn = new DataColumn();
            dataColumn.DataType = Type.GetType("System.Int32");
            dataColumn.ColumnName = "Number of Licenses";
            resultsDataTable.Columns.Add(dataColumn);

            dataColumn = new DataColumn();
            dataColumn.DataType = Type.GetType("System.Int32");
            dataColumn.ColumnName = "License Declared";
            resultsDataTable.Columns.Add(dataColumn);

            commandText =
                "select ai._applicationid, count(ai._applicationid) " +
                "from application_instances ai ";

            if (aAssetIds != "")
                commandText += "WHERE ai._ASSETID IN (" + aAssetIds + ") ";

            commandText += "group by ai._applicationid";

            DataTable installsDataTable = PerformQuery(commandText);

            DataTable notCountedLicensesTable = PerformQuery(
                "select l._applicationid " +
                "from license_types lt " +
                "inner join licenses l on (l._licensetypeid = lt._licensetypeid) " +
                "where lt._counted = 0");

            List<int> aliasedToApplicationsList = new List<int>();

            foreach (DataRow dataRow in lApplicationsDao.GetAliasedToApplications().Rows)
            {
                aliasedToApplicationsList.Add(Convert.ToInt32(dataRow[0]));
            }

            foreach (DataRow row in dataTable.Rows)
            {
                int applicationId = (int)row.ItemArray[0];
                int licenseCount = (int)row.ItemArray[3];

                rows = notCountedLicensesTable.Select(String.Format("_APPLICATIONID = {0} ", applicationId));

                // if there is an unlimited license associated with this application, we treat it as correctly licensed
                if (rows.Length > 0) continue;

                rows = installsDataTable.Select(String.Format("_APPLICATIONID = {0} ", applicationId));

                int installationCount;

                if (rows.Length == 0)
                    installationCount = 0;
                else
                    installationCount = (int)rows[0].ItemArray[1];

                // need to now check if this application is the target of any aliases
                // if so, we need to get their licenses and add to this application
                // find any applications which are aliased to this application as we also need their licenses
                bool foundNonCountedLicense = false;

                if (aliasedToApplicationsList.Contains(applicationId))
                {
                    foreach (DataRow dataRow in lApplicationsDao.GetAliasedApplicationsByApplicationId(applicationId).Rows)
                    {
                        if (foundNonCountedLicense) continue;

                        int aliasedApplicationId = Convert.ToInt32(dataRow[0]);

                        if (lLicenseDao.GetNotCountedLicenseCountByApplicationId(aliasedApplicationId) > 0)
                        {
                            // one of the aliased apps had an unlimited license - means the aliased to app is now compliant
                            foundNonCountedLicense = true;
                            continue;
                        }
                        else
                        {
                            // get all of the licenses for this aliased app and add them to the parent
                            DataTable licenseCountTable = lLicenseDao.GetLicenseCountByApplicationId(aliasedApplicationId);
                            if (licenseCountTable.Rows.Count > 0)
                            {
                                licenseCount += Convert.ToInt32(licenseCountTable.Rows[0][0]);
                            }
                        }
                    }
                }

                if (foundNonCountedLicense) continue;
                if (installationCount <= licenseCount) continue;

                string applicationName = (string)row.ItemArray[1];
                string applicationVersion = (string)row.ItemArray[2];

                DataRow newDataRow = resultsDataTable.NewRow();
                newDataRow[0] = (applicationName.EndsWith(applicationVersion) || applicationVersion == "") ? applicationName : applicationName + " (v" + applicationVersion + ")";
                newDataRow[1] = licenseCount;
                newDataRow[2] = installationCount - licenseCount;

                resultsDataTable.Rows.Add(newDataRow);
            }

            return resultsDataTable;
        }

        /// <summary>
        /// Return Top 15 compliant software applications
        /// </summary>
        /// <returns></returns>
        public DataTable StatisticsTop15CompliantApplicationsPublishersDrilldown(string aRowLabel)
        {
            return PerformQuery(String.Format(
                    "SELECT _NAME as Name, _MAKE as Make, _MODEL as Model FROM ASSETS " +
                    "WHERE _locationid in (select _locationid from locations where _fullname = '{0}'", aRowLabel));
        }

        public DataTable StatisticsSoftwareComplianceByPublisher(string aPublisher)
        {
            return StatisticsSoftwareComplianceByPublisher(aPublisher, new AssetDAO().GetSelectedAssets());
        }

        public DataTable StatisticsSoftwareComplianceByPublisher(string aPublisher, string aAssetIds)
        {
            DataRow[] rows;
            DataTable resultsDataTable = new DataTable();
            ApplicationsDAO lApplicationsDao = new ApplicationsDAO();
            LicensesDAO lLicenseDao = new LicensesDAO();

            string commandText =
                "SELECT a._applicationid, a._name AS Application, a._version AS Version, sum(l._count) AS LicenseCount " +
                "FROM LICENSES l " +
                "INNER JOIN APPLICATIONS a ON (a._applicationid = l._applicationid) " +
                "WHERE a._ignored = 0 " +
                "AND a._publisher = '" + aPublisher + "' ";

            commandText += "GROUP BY a._applicationid, a._name, a._version";

            DataTable dataTable = PerformQuery(commandText);

            DataColumn dataColumn = new DataColumn();
            dataColumn.DataType = Type.GetType("System.String");
            dataColumn.ColumnName = "Application";
            resultsDataTable.Columns.Add(dataColumn);

            dataColumn = new DataColumn();
            dataColumn.DataType = Type.GetType("System.Int32");
            dataColumn.ColumnName = "Total";
            resultsDataTable.Columns.Add(dataColumn);

            commandText =
                "SELECT ai._applicationid, COUNT(ai._applicationid) " +
                "FROM application_instances ai ";

            if (aAssetIds != "")
                commandText += "WHERE ai._ASSETID IN (" + aAssetIds + ") ";

            commandText += "GROUP BY ai._applicationid";

            DataTable installsDataTable = PerformQuery(commandText);

            DataTable notCountedLicensesTable = PerformQuery(
                "SELECT l._applicationid " +
                "FROM license_types lt " +
                "INNER JOIN licenses l ON (l._licensetypeid = lt._licensetypeid) " +
                "WHERE lt._counted = 0");

            List<int> aliasedToApplicationsList = new List<int>();

            foreach (DataRow dataRow in lApplicationsDao.GetAliasedToApplications().Rows)
            {
                aliasedToApplicationsList.Add(Convert.ToInt32(dataRow[0]));
            }

            foreach (DataRow row in dataTable.Rows)
            {
                try
                {
                    int applicationId = (int)row.ItemArray[0];
                    int licenseCount = (int)row.ItemArray[3];

                    rows = notCountedLicensesTable.Select(String.Format("_APPLICATIONID = {0} ", applicationId));

                    // if there is an unlimited license associated with this application, we treat it as correctly licensed
                    if (rows.Length > 0) continue;

                    rows = installsDataTable.Select(String.Format("_APPLICATIONID = {0} ", applicationId));

                    int installationCount;

                    if (rows.Length == 0)
                        installationCount = 0;
                    else
                        installationCount = (int)rows[0].ItemArray[1];

                    // need to now check if this application is the target of any aliases
                    // if so, we need to get their licenses and add to this application
                    // find any applications which are aliased to this application as we also need their licenses
                    bool foundNonCountedLicense = false;

                    if (aliasedToApplicationsList.Contains(applicationId))
                    {
                        foreach (DataRow dataRow in lApplicationsDao.GetAliasedApplicationsByApplicationId(applicationId).Rows)
                        {
                            if (foundNonCountedLicense) continue;

                            int aliasedApplicationId = Convert.ToInt32(dataRow[0]);

                            if (lLicenseDao.GetNotCountedLicenseCountByApplicationId(aliasedApplicationId) > 0)
                            {
                                // one of the aliased apps had an unlimited license - means the aliased to app is now compliant
                                foundNonCountedLicense = true;
                                continue;
                            }
                            else
                            {
                                // get all of the licenses for this aliased app and add them to the parent
                                DataTable licenseCountTable = lLicenseDao.GetLicenseCountByApplicationId(aliasedApplicationId);
                                if (licenseCountTable.Rows.Count > 0)
                                {
                                    licenseCount += Convert.ToInt32(licenseCountTable.Rows[0][0]);
                                }
                            }
                        }
                    }

                    if (foundNonCountedLicense) continue;
                    if (installationCount == licenseCount) continue;

                    string applicationName = (string)row.ItemArray[1];
                    string applicationVersion = (string)row.ItemArray[2];

                    DataRow newDataRow = resultsDataTable.NewRow();

                    if (applicationVersion != String.Empty)
                        newDataRow[0] = applicationName + " (v" + applicationVersion + ")";
                    else
                        newDataRow[0] = applicationName;

                    newDataRow[1] = licenseCount - installationCount;

                    resultsDataTable.Rows.Add(newDataRow);
                }
                catch (Exception ex)
                {
                    logger.Error(ex.Message);
                }
            }

            resultsDataTable.DefaultView.Sort = "Total DESC";
            return resultsDataTable;
        }

        /// <summary>
        /// Return software compliance by publisher
        /// </summary>
        /// <returns></returns>
        public DataTable StatisticsSoftwareComplianceByPublisherDrilldown(string aRowLabel)
        {
            char[] charsToTrim = { ')' };

            string[] parts = aRowLabel.Split(new string[] { "(v" }, StringSplitOptions.None);
            string applicationName = parts[0];
            string applicationVersion = parts[1].TrimEnd(charsToTrim);

            return PerformQuery(String.Format(
                    "SELECT a._NAME as Name, a._MAKE as Make, a._MODEL as Model, app._name + ' ' + app._version as Application " +
                    "FROM ASSETS a " +
                    "LEFT OUTER JOIN application_instances ai on (a._assetid = ai._assetid) " +
                    "LEFT OUTER JOIN applications app on (app._applicationid = ai._applicationid) " +
                    "WHERE app._name = '{0}' " +
                    "AND app._version = '{1}'", applicationName, applicationVersion));
        }

        /// <summary>
        /// Return drilldown statistics for Asset Table
        /// </summary>
        /// <returns></returns>
        public DataTable StatisticsAssetOSDrilldown(string aRowLabel)
        {
            return PerformQuery(String.Format(
                    "SELECT a._NAME as \"Asset Name\", a._MAKE as Make, a._MODEL as Model, ap._name + ' ' + ap._version as \"Operating System\", ai._productid as \"Serial Number\", ai._cdkey as \"CD Key\" " +
                    "FROM ASSETS a " +
                    "INNER JOIN APPLICATION_INSTANCES ai on (a._assetid = ai._assetid) " +
                    "INNER JOIN APPLICATIONS ap on (ai._applicationid = ap._applicationid) " +
                    "and ap._isos = 1 " +
                    "and ap._name + ' ' + ap._version = '{0}' " +
                    "AND a._ASSETID IN ({1})", aRowLabel, new AssetDAO().GetSelectedAssets()));
        }

        public DataTable StatisticsTopManufacturers()
        {
            return StatisticsTopManufacturers(new AssetDAO().GetSelectedAssets());
        }

        public DataTable StatisticsTopManufacturersAsGrid(string aAssetIds)
        {
            string commandText =
                        "SELECT _name as \"Asset Name\", _make as Item " +
                        "FROM assets " +
                        "WHERE _make <> '' " +
                        "AND _STOCK_STATUS <> 3 ";

            if (aAssetIds != "")
                commandText += "AND _ASSETID IN (" + aAssetIds + ") ";

            commandText += "ORDER BY \"Asset Name\"";

            return PerformQuery(commandText);
        }

        /// <summary>
        /// Return top 15 software vendors
        /// </summary>
        /// <returns></returns>
        public DataTable StatisticsTopManufacturers(string aAssetIds)
        {
            string commandText = "";

            if (compactDatabaseType)
            {
                commandText =
                    "SELECT TOP (15) _make as Item, COUNT(_make) as Total " +
                    "FROM assets " +
                    "WHERE _make <> '' " +
                    "AND _STOCK_STATUS <> 3 ";

                if (aAssetIds != "")
                    commandText += "AND _ASSETID IN (" + aAssetIds + ") ";

                commandText += "GROUP BY _make ORDER BY Total DESC";
            }
            else
            {
                commandText =
                    "SELECT TOP 15 _make as Item, COUNT(_make) as Total " +
                    "FROM assets " +
                    "WHERE _make <> '' " +
                    "AND _STOCK_STATUS <> 3 ";

                if (aAssetIds != "")
                    commandText += "AND _ASSETID IN (" + aAssetIds + ") ";

                commandText += "GROUP BY _make ORDER BY Total DESC";
            }

            return PerformQuery(commandText);
        }

        /// <summary>
        /// Return drilldown statistics 
        /// </summary>
        /// <returns></returns>
        public DataTable StatisticsTopManufacturersDrilldown(string aRowLabel)
        {
            string commandText = String.Format(
                    "SELECT _NAME as \"Asset Name\", _MAKE as Make, _MODEL as Model " +
                    "FROM ASSETS " +
                    "WHERE _MAKE = '{0}' " +
                    "AND _ASSETID IN ({1})", aRowLabel, new AssetDAO().GetSelectedAssets());

            return PerformQuery(commandText);
        }

        /// <summary>
        /// Return drilldown statistics for Asset Table
        /// </summary>
        /// <returns></returns>
        public DataTable StatisticsAgentVersionsDrilldown(string aRowLabel)
        {
            return PerformQuery(String.Format(
                    "SELECT _NAME as \"Asset Name\", _MAKE as Make, _MODEL as Model, _AGENT_VERSION as AgentVersion " +
                    "FROM ASSETS " +
                    "WHERE _AGENT_VERSION = '{0}' " +
                    "AND _ASSETID IN ({1})", aRowLabel, new AssetDAO().GetSelectedAssets()));
        }

        /// <summary>
        /// Return drilldown statistics
        /// </summary>
        /// <returns></returns>
        public DataTable StatisticsAgentDeploymentDrilldown(string aRowLabel)
        {
            string lStatus = "0";

            switch (aRowLabel)
            {
                case "Not Deployed":
                    lStatus = "0";
                    break;
                case "Deployed (Not Running)":
                    lStatus = "1";
                    break;
                case "Deployed (Running)":
                    lStatus = "2";
                    break;
            }

            return PerformQuery(String.Format(
                    "SELECT _NAME as \"Asset Name\", _MAKE as Make, _MODEL as Model, '{0}' as Status " +
                    "FROM ASSETS " +
                    "WHERE _AGENT_STATUS = '{1}' " +
                    "AND _ASSETID IN ({2})", aRowLabel, lStatus, new AssetDAO().GetSelectedAssets()));
        }

        /// <summary>
        /// Return drilldown statistics
        /// </summary>
        /// <returns></returns>
        public DataTable StatisticsOfficeVersionDrilldown(string aRowLabel)
        {
            return PerformQuery(String.Format(
                    "select as1._NAME as \"Asset Name\", as1._MAKE as Make, as1._MODEL as Model, a._name as \"MS Office Version\" " +
                    "from applications a, application_instances ai, assets as1 " +
                    "where a._name = '{0}' " +
                    "and ai._applicationid = a._applicationid " +
                    "and ai._assetid = as1._assetid " +
                    "AND as1._ASSETID IN ({1})", aRowLabel, new AssetDAO().GetSelectedAssets()));
        }

        public DataTable StatisticsAssetStates()
        {
            return StatisticsAssetStates(new AssetDAO().GetSelectedAssets(true));
        }

        /// <summary>
        /// Return statistics for Asset States
        /// </summary>
        /// <returns></returns>
        public DataTable StatisticsAssetStates(string aAssetIds)
        {
            string commandText =
                            "SELECT " +
                            "CASE _STOCK_STATUS " +
                                "WHEN 0 THEN 'In Stock' " +
                                "WHEN 1 THEN 'In Use' " +
                                "WHEN 2 THEN 'Pending Disposal' " +
                                "WHEN 3 THEN 'Disposed' " +
                            "END AS Item, COUNT(*) AS Total " +
                            "FROM ASSETS ";

            if (aAssetIds != "")
                commandText += "WHERE _ASSETID IN (" + aAssetIds + ") ";

            commandText += "GROUP BY ASSETS._STOCK_STATUS ORDER BY Total DESC";

            return PerformQuery(commandText);
        }

        public DataTable StatisticsAssetStatesAsGrid(string aAssetIds)
        {
            string commandText =
                            "SELECT _NAME as \"Asset Name\"," +
                            "CASE _STOCK_STATUS " +
                                "WHEN 0 THEN 'In Stock' " +
                                "WHEN 1 THEN 'In Use' " +
                                "WHEN 2 THEN 'Pending Disposal' " +
                                "WHEN 3 THEN 'Disposed' " +
                            "END AS Item " +
                            "FROM ASSETS ";

            if (aAssetIds != "")
                commandText += "WHERE _ASSETID IN (" + aAssetIds + ") ";

            commandText += "ORDER BY \"Asset Name\"";

            return PerformQuery(commandText);
        }

        /// <summary>
        /// Return Alert Statistics
        /// </summary>
        /// <returns></returns>
        public DataTable AlertStatistics()
        {
            if (isDebugEnabled) logger.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " in");

            DataTable statisticsTable = new DataTable(TableNames.APPLICATION_STATISTICS);
            statisticsTable.Columns.Add(new DataColumn("row"));
            statisticsTable.Columns.Add(new DataColumn("lastalert"));
            statisticsTable.Columns.Add(new DataColumn("alertstoday"));
            statisticsTable.Columns.Add(new DataColumn("alertsthisweek"));
            statisticsTable.Columns.Add(new DataColumn("alertsthismonth"));

            try
            {
                if (compactDatabaseType)
                {
                    using (SqlCeConnection conn = DatabaseConnection.CreateOpenCEConnection())
                    {
                        DataRow newRow = statisticsTable.NewRow();

                        object[] rowArray = new object[5];
                        rowArray[0] = "row";
                        rowArray[1] = QueryAndGetValue(conn, "SELECT MAX(_ALERTDATE) FROM ALERTS");
                        rowArray[2] = QueryAndGetValue(conn, "SELECT COUNT(_ALERTID) FROM ALERTS WHERE (DATEDIFF(day, GETDATE(), _ALERTDATE) = 0)");
                        rowArray[3] = QueryAndGetValue(conn, "SELECT COUNT(_ALERTID) FROM ALERTS WHERE (DATEDIFF(day, DATEADD(week, -1, GETDATE()), _ALERTDATE) >= 0)");
                        rowArray[4] = QueryAndGetValue(conn, "SELECT COUNT(_ALERTID) FROM ALERTS WHERE (DATEDIFF(day, DATEADD(month, -1, GETDATE()), _ALERTDATE) >= 0)");

                        newRow.ItemArray = rowArray;
                        statisticsTable.Rows.Add(newRow);
                    }
                }
                else
                {
                    using (SqlConnection conn = DatabaseConnection.CreateOpenStandardConnection())
                    {
                        DataRow newRow = statisticsTable.NewRow();

                        object[] rowArray = new object[5];
                        rowArray[0] = "row";
                        rowArray[1] = QueryAndGetValue(conn, "SELECT MAX(_ALERTDATE) FROM ALERTS");
                        rowArray[2] = QueryAndGetValue(conn, "SELECT COUNT(_ALERTID) FROM ALERTS WHERE (DATEDIFF(day, GETDATE(), _ALERTDATE) = 0)");
                        rowArray[3] = QueryAndGetValue(conn, "SELECT COUNT(_ALERTID) FROM ALERTS WHERE (DATEDIFF(day, DATEADD(week, -1, GETDATE()), _ALERTDATE) >= 0)");
                        rowArray[4] = QueryAndGetValue(conn, "SELECT COUNT(_ALERTID) FROM ALERTS WHERE (DATEDIFF(day, DATEADD(month, -1, GETDATE()), _ALERTDATE) >= 0)");

                        newRow.ItemArray = rowArray;
                        statisticsTable.Rows.Add(newRow);
                    }
                }
            }
            catch (SqlCeException ex)
            {
                Utility.DisplayErrorMessage("A database error has occurred in AuditWizard." + Environment.NewLine + Environment.NewLine +
                        "Please see the log file for further details.");
                logger.Error("Exception in " + System.Reflection.MethodBase.GetCurrentMethod().Name, ex);
            }
            catch (Exception ex)
            {
                Utility.DisplayErrorMessage("A database error has occurred in AuditWizard." + Environment.NewLine + Environment.NewLine +
                        "Please see the log file for further details.");

                logger.Error("Exception in " + System.Reflection.MethodBase.GetCurrentMethod().Name, ex);
            }

            if (isDebugEnabled) logger.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " out");
            return statisticsTable;
        }

        public DataTable AuditHistoryStatisticsForDashboard()
        {
            return AuditHistoryStatistics("");					// 8.4.1 CMD All Assets so do not pass an asset filter
        }

        public DataTable AuditHistoryStatistics()
        {
            return AuditHistoryStatistics(new AssetDAO().GetSelectedAssets());
        }


        /// <summary>
        /// Return Audit History Statistics
        /// </summary>
        /// <returns></returns>
        public DataTable AuditHistoryStatistics(string aAssetIds)
        {
            if (isDebugEnabled) logger.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " in");

            DataTable statisticsTable = new DataTable(TableNames.APPLICATION_STATISTICS);

            if (compactDatabaseType)
            {
                try
                {
                    statisticsTable.Columns.Add(new DataColumn("row"));
                    statisticsTable.Columns.Add(new DataColumn("notaudited"));
                    statisticsTable.Columns.Add(new DataColumn("today"));
                    statisticsTable.Columns.Add(new DataColumn("notinlast7"));
                    statisticsTable.Columns.Add(new DataColumn("notinlast14"));
                    statisticsTable.Columns.Add(new DataColumn("notinlast30"));
                    statisticsTable.Columns.Add(new DataColumn("over90days"));

                    using (SqlCeConnection conn = DatabaseConnection.CreateOpenCEConnection())
                    {
                        DataRow newRow;
                        newRow = statisticsTable.NewRow();

                        string lFilterText = "";
                        string lWhereClause;
						if (String.IsNullOrEmpty(aAssetIds))
						{
							lWhereClause = " AND _STOCK_STATUS < 3";
						}
						else
						{
							lWhereClause = " AND _STOCK_STATUS < 3 AND _ASSETID IN (" + aAssetIds + ")";
						}

                        object[] rowArray = new object[7];
                        rowArray[0] = "row";

                        lFilterText = "SELECT COUNT(_ASSETID) AS reportcount FROM ASSETS WHERE (_LASTAUDIT is NULL)";
                        lFilterText = lFilterText + lWhereClause;
                        rowArray[1] = QueryAndGetValue(conn, lFilterText);

                        lFilterText = "SELECT COUNT(_ASSETID) AS reportcount FROM ASSETS WHERE (datediff(day, _LASTAUDIT ,GETDATE()) = 0)";
                        lFilterText = lFilterText + lWhereClause;
                        rowArray[2] = QueryAndGetValue(conn, lFilterText);

                        lFilterText = "SELECT COUNT(_ASSETID) AS reportcount FROM ASSETS WHERE (datediff(day, _LASTAUDIT ,GETDATE()) > 7) AND (datediff(day, _LASTAUDIT ,GETDATE()) <= 14)";
                        lFilterText = lFilterText + lWhereClause;
                        rowArray[3] = QueryAndGetValue(conn, lFilterText);

                        lFilterText = "SELECT COUNT(_ASSETID) AS reportcount FROM ASSETS WHERE (datediff(day, _LASTAUDIT ,GETDATE()) > 14) AND (datediff(day, _LASTAUDIT ,GETDATE()) <= 30)";
                        lFilterText = lFilterText + lWhereClause;
                        rowArray[4] = QueryAndGetValue(conn, lFilterText);

                        lFilterText = "SELECT COUNT(_ASSETID) AS reportcount FROM ASSETS WHERE (datediff(day, _LASTAUDIT ,GETDATE()) > 30) AND (datediff(day, _LASTAUDIT ,GETDATE()) <= 90)";
                        lFilterText = lFilterText + lWhereClause;
                        rowArray[5] = QueryAndGetValue(conn, lFilterText);

                        lFilterText = "SELECT COUNT(_ASSETID) AS reportcount FROM ASSETS WHERE (datediff(day, _LASTAUDIT ,GETDATE()) > 90)";
                        lFilterText = lFilterText + lWhereClause;
                        rowArray[6] = QueryAndGetValue(conn, lFilterText);

                        newRow.ItemArray = rowArray;
                        statisticsTable.Rows.Add(newRow);
                    }
                }
                catch (SqlCeException ex)
                {
                    Utility.DisplayErrorMessage("A database error has occurred in AuditWizard." + Environment.NewLine + Environment.NewLine +
                            "Please see the log file for further details.");
                    logger.Error("Exception in " + System.Reflection.MethodBase.GetCurrentMethod().Name, ex);
                }
                catch (Exception ex)
                {
                    Utility.DisplayErrorMessage("A database error has occurred in AuditWizard." + Environment.NewLine + Environment.NewLine +
                            "Please see the log file for further details.");

                    logger.Error("Exception in " + System.Reflection.MethodBase.GetCurrentMethod().Name, ex);
                }
            }
            else
            {
                AuditWizardDataAccess lAuditWizardDataAccess = new AuditWizardDataAccess();
                statisticsTable = lAuditWizardDataAccess.AuditHistoryStatistics();
            }

            if (isDebugEnabled) logger.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " out");
            return statisticsTable;
        }

        /// <summary>
        /// Return statistics for the 'Support Statistics' phase
        /// </summary>
        /// <returns></returns>
        public DataTable SupportStatistics()
        {
            if (isDebugEnabled) logger.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " in");

            DataTable statisticsTable = new DataTable(TableNames.COMPUTER_STATISTICS);

            if (compactDatabaseType)
            {
                try
                {
                    statisticsTable.Columns.Add(new DataColumn("row"));
                    statisticsTable.Columns.Add(new DataColumn("expired"));
                    statisticsTable.Columns.Add(new DataColumn("expiretoday"));
                    statisticsTable.Columns.Add(new DataColumn("expirethisweek"));
                    statisticsTable.Columns.Add(new DataColumn("expirethismonth"));

                    using (SqlCeConnection conn = DatabaseConnection.CreateOpenCEConnection())
                    {
                        DataRow newRow;
                        newRow = statisticsTable.NewRow();

                        object[] rowArray = new object[5];
                        rowArray[0] = "row";
                        rowArray[1] = QueryAndGetValue(conn, "SELECT COUNT(_APPLICATIONID) FROM LICENSES WHERE _SUPPORTED=1 AND (DATEDIFF(day, GETDATE(), _SUPPORT_EXPIRES) < 0)");
                        rowArray[2] = QueryAndGetValue(conn, "SELECT COUNT(_APPLICATIONID) FROM LICENSES WHERE _SUPPORTED=1 AND (DATEDIFF(day, GETDATE(), _SUPPORT_EXPIRES) = 0)");
                        rowArray[3] = QueryAndGetValue(conn, "SELECT COUNT(_APPLICATIONID) FROM LICENSES WHERE _SUPPORTED=1 AND (DATEDIFF(day, GETDATE(), _SUPPORT_EXPIRES) > 0) AND (DATEDIFF(day, DATEADD(week, 1, GETDATE()), _SUPPORT_EXPIRES) <= 0)");
                        rowArray[4] = QueryAndGetValue(conn, "SELECT COUNT(_APPLICATIONID) FROM LICENSES WHERE _SUPPORTED=1 AND (DATEDIFF(day, GETDATE(), _SUPPORT_EXPIRES) > 7) AND (DATEDIFF(day, DATEADD(month ,1 ,GETDATE()), _SUPPORT_EXPIRES) <= 0)");

                        newRow.ItemArray = rowArray;
                        statisticsTable.Rows.Add(newRow);
                    }
                }
                catch (SqlCeException ex)
                {
                    Utility.DisplayErrorMessage("A database error has occurred in AuditWizard." + Environment.NewLine + Environment.NewLine +
                            "Please see the log file for further details.");
                    logger.Error("Exception in " + System.Reflection.MethodBase.GetCurrentMethod().Name, ex);
                }
                catch (Exception ex)
                {
                    Utility.DisplayErrorMessage("A database error has occurred in AuditWizard." + Environment.NewLine + Environment.NewLine +
                            "Please see the log file for further details.");

                    logger.Error("Exception in " + System.Reflection.MethodBase.GetCurrentMethod().Name, ex);
                }
            }
            else
            {
                AuditWizardDataAccess lAuditWizardDataAccess = new AuditWizardDataAccess();
                statisticsTable = lAuditWizardDataAccess.SupportStatistics();
            }

            if (isDebugEnabled) logger.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " out");
            return statisticsTable;
        }




        /// <summary>
        /// Return statistics for the 'Declare Licenses' phase
        /// </summary>
        /// <returns></returns>
        public DataTable DLStatistics()
        {
            if (isDebugEnabled) logger.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " in");

            DataTable statisticsTable = new DataTable(TableNames.COMPUTER_STATISTICS);

            if (compactDatabaseType)
            {
                try
                {
                    statisticsTable.Columns.Add(new DataColumn("row"));
                    statisticsTable.Columns.Add(new DataColumn("uniqueapplications"));
                    statisticsTable.Columns.Add(new DataColumn("includedapplicationinstances"));
                    statisticsTable.Columns.Add(new DataColumn("licensesdeclared"));
                    statisticsTable.Columns.Add(new DataColumn("licenseinstancecount"));

                    using (SqlCeConnection conn = DatabaseConnection.CreateOpenCEConnection())
                    {
                        DataRow newRow;
                        newRow = statisticsTable.NewRow();

                        object[] rowArray = new object[5];
                        rowArray[0] = "row";
                        rowArray[1] = QueryAndGetValue(conn, "SELECT COUNT(_NAME) FROM APPLICATIONS WHERE _ISOS=0");
                        rowArray[2] = QueryAndGetValue(conn, "SELECT COUNT(_APPLICATIONID) FROM APPLICATION_INSTANCES WHERE _APPLICATIONID IN (SELECT _APPLICATIONID FROM APPLICATIONS WHERE _IGNORED=0 AND _ISOS=0)");
                        rowArray[3] = QueryAndGetValue(conn, "SELECT COUNT(_LICENSEID) FROM LICENSES");
                        rowArray[4] = QueryAndGetValue(conn, "SELECT SUM(_COUNT) FROM LICENSES WHERE _LICENSETYPEID in (SELECT _LICENSETYPEID FROM LICENSE_TYPES WHERE _COUNTED=1)");

                        newRow.ItemArray = rowArray;
                        statisticsTable.Rows.Add(newRow);
                    }
                }
                catch (SqlCeException ex)
                {
                    Utility.DisplayErrorMessage("A database error has occurred in AuditWizard." + Environment.NewLine + Environment.NewLine +
                            "Please see the log file for further details.");
                    logger.Error("Exception in " + System.Reflection.MethodBase.GetCurrentMethod().Name, ex);
                }
                catch (Exception ex)
                {
                    Utility.DisplayErrorMessage("A database error has occurred in AuditWizard." + Environment.NewLine + Environment.NewLine +
                            "Please see the log file for further details.");

                    logger.Error("Exception in " + System.Reflection.MethodBase.GetCurrentMethod().Name, ex);
                }
            }
            else
            {
                AuditWizardDataAccess lAuditWizardDataAccess = new AuditWizardDataAccess();
                statisticsTable = lAuditWizardDataAccess.DLStatistics();
            }

            if (isDebugEnabled) logger.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " out");
            return statisticsTable;
        }


        /// <summary>
        /// Return statistics for the 'Create Actions' phase
        /// </summary>
        /// <returns></returns>
        public DataTable CAStatistics()
        {
            if (isDebugEnabled) logger.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " in");

            DataTable statisticsTable = new DataTable(TableNames.COMPUTER_STATISTICS);

            if (compactDatabaseType)
            {
                try
                {
                    using (SqlCeConnection conn = DatabaseConnection.CreateOpenCEConnection())
                    {
                        string commandText =
                            "SELECT 'row' " +
                            ",COUNT(_ACTIONID) FROM ACTIONS actioncount";

                        using (SqlCeCommand command = new SqlCeCommand(commandText, conn))
                        {
                            new SqlCeDataAdapter(command).Fill(statisticsTable);
                        }
                    }
                }
                catch (SqlCeException ex)
                {
                    Utility.DisplayErrorMessage("A database error has occurred in AuditWizard." + Environment.NewLine + Environment.NewLine +
                            "Please see the log file for further details.");
                    logger.Error("Exception in " + System.Reflection.MethodBase.GetCurrentMethod().Name, ex);
                }
                catch (Exception ex)
                {
                    Utility.DisplayErrorMessage("A database error has occurred in AuditWizard." + Environment.NewLine + Environment.NewLine +
                            "Please see the log file for further details.");

                    logger.Error("Exception in " + System.Reflection.MethodBase.GetCurrentMethod().Name, ex);
                }
            }
            else
            {
                AuditWizardDataAccess lAuditWizardDataAccess = new AuditWizardDataAccess();
                statisticsTable = lAuditWizardDataAccess.CAStatistics();
            }

            if (isDebugEnabled) logger.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " out");
            return statisticsTable;
        }


        /// <summary>
        /// Return statistics for the 'Review Actions' phase
        /// </summary>
        /// <returns></returns>
        public DataTable RAStatistics()
        {
            if (isDebugEnabled) logger.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " in");

            DataTable statisticsTable = new DataTable(TableNames.COMPUTER_STATISTICS);

            if (compactDatabaseType)
            {
                try
                {
                    statisticsTable.Columns.Add(new DataColumn("row"));
                    statisticsTable.Columns.Add(new DataColumn("reviewed"));
                    statisticsTable.Columns.Add(new DataColumn("notreviewed"));

                    using (SqlCeConnection conn = DatabaseConnection.CreateOpenCEConnection())
                    {
                        DataRow newRow;
                        newRow = statisticsTable.NewRow();

                        object[] rowArray = new object[3];
                        rowArray[0] = "row";
                        rowArray[1] = QueryAndGetValue(conn, "SELECT COUNT(_ACTIONID) FROM ACTIONS WHERE _STATUS<>0");
                        rowArray[2] = QueryAndGetValue(conn, "SELECT COUNT(_ACTIONID) FROM ACTIONS WHERE _STATUS=0");

                        newRow.ItemArray = rowArray;
                        statisticsTable.Rows.Add(newRow);
                    }
                }
                catch (SqlCeException ex)
                {
                    Utility.DisplayErrorMessage("A database error has occurred in AuditWizard." + Environment.NewLine + Environment.NewLine +
                            "Please see the log file for further details.");
                    logger.Error("Exception in " + System.Reflection.MethodBase.GetCurrentMethod().Name, ex);
                }
                catch (Exception ex)
                {
                    Utility.DisplayErrorMessage("A database error has occurred in AuditWizard." + Environment.NewLine + Environment.NewLine +
                            "Please see the log file for further details.");

                    logger.Error("Exception in " + System.Reflection.MethodBase.GetCurrentMethod().Name, ex);
                }
            }
            else
            {
                AuditWizardDataAccess lAuditWizardDataAccess = new AuditWizardDataAccess();
                statisticsTable = lAuditWizardDataAccess.RAStatistics();
            }

            if (isDebugEnabled) logger.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " out");
            return statisticsTable;
        }


        /// <summary>
        /// Return the top applications count from the database
        /// </summary>
        /// <param name="recordCount"></param>
        /// <returns></returns>
        public DataTable StatisticsTopApplications(int recordCount, String publisherFilter)
        {
            if (isDebugEnabled) logger.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " in");

            AuditWizardDataAccess lAuditWizardDataAccess = new AuditWizardDataAccess();

            // Before we call the stored procedure we need to handle the publisher filter if one has
            // been supplied.  
            String sqlPublisherFilter = lAuditWizardDataAccess.BuildPublisherFilter(publisherFilter);

            // Create the data table
            DataTable statisticsTable = new DataTable(TableNames.APPLICATION_STATISTICS);

            if (compactDatabaseType)
            {
                try
                {
                    using (SqlCeConnection conn = DatabaseConnection.CreateOpenCEConnection())
                    {
                        string commandText =
                            "SELECT TOP (@recordCount) APPLICATIONS._NAME, COUNT(*) t " +
                            "FROM APPLICATION_INSTANCES, APPLICATIONS " +
                            "WHERE APPLICATION_INSTANCES._APPLICATIONID = APPLICATIONS._APPLICATIONID " +
                            "AND APPLICATIONS._ISOS = 0 " +
                            "AND APPLICATIONS._IGNORED = 0";

                        if (sqlPublisherFilter != "")
                            commandText += "AND APPLICATIONS._PUBLISHER IN (@publisherFilter) ";

                        commandText +=
                            "GROUP BY APPLICATIONS._NAME " +
                            "ORDER BY t DESC";

                        using (SqlCeCommand command = new SqlCeCommand(commandText, conn))
                        {
                            if (sqlPublisherFilter != "")
                                command.Parameters.AddWithValue("@publisherFilter", sqlPublisherFilter);

                            command.Parameters.AddWithValue("@recordCount", recordCount);

                            new SqlCeDataAdapter(command).Fill(statisticsTable);
                        }
                    }
                }
                catch (SqlCeException ex)
                {
                    Utility.DisplayErrorMessage("A database error has occurred in AuditWizard." + Environment.NewLine + Environment.NewLine +
                            "Please see the log file for further details.");
                    logger.Error("Exception in " + System.Reflection.MethodBase.GetCurrentMethod().Name, ex);
                }
                catch (Exception ex)
                {
                    Utility.DisplayErrorMessage("A database error has occurred in AuditWizard." + Environment.NewLine + Environment.NewLine +
                            "Please see the log file for further details.");

                    logger.Error("Exception in " + System.Reflection.MethodBase.GetCurrentMethod().Name, ex);
                }
            }
            else
            {
                statisticsTable = lAuditWizardDataAccess.StatisticsTopApplications(recordCount, publisherFilter);
            }

            if (isDebugEnabled) logger.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " out");
            return statisticsTable;
        }

        public DataTable StatisticsTopPublishers(bool showIncluded, bool showIgnored)
        {
            return StatisticsTopPublishers(showIncluded, showIgnored, new AssetDAO().GetSelectedAssets());
        }

        public DataTable StatisticsTopPublishersAsGrid(bool showIncluded, bool showIgnored, string aAssetIds)
        {
            string commandText = String.Empty;

            if (compactDatabaseType)
            {
                commandText =
                        "SELECT TOP (15) ap._PUBLISHER as Item, COUNT(*) AS Total " +
                        "FROM ASSETS a " +
                        "LEFT JOIN APPLICATION_INSTANCES ai ON (ai._ASSETID = a._ASSETID) " +
                        "LEFT JOIN APPLICATIONS ap ON (ap._APPLICATIONID = ai._APPLICATIONID) ";
            }
            else
            {
                commandText =
                        "SELECT TOP 15 ap._PUBLISHER as Item, COUNT(*) AS Total " +
                        "FROM ASSETS a " +
                        "LEFT JOIN APPLICATION_INSTANCES ai ON (ai._ASSETID = a._ASSETID) " +
                        "LEFT JOIN APPLICATIONS ap ON (ap._APPLICATIONID = ai._APPLICATIONID) ";
            }

            if (showIncluded && !showIgnored)
                commandText += "WHERE ap._IGNORED = 0 AND a._STOCK_STATUS <> 3 ";

            else if (!showIncluded && showIgnored)
                commandText += "WHERE ap._IGNORED = 1 AND a._STOCK_STATUS <> 3 ";

            else
                commandText += "WHERE a._STOCK_STATUS <> 3 ";

            if (aAssetIds != "")
                commandText += "AND a._ASSETID IN (" + aAssetIds + ") ";

            commandText += "GROUP BY ap._PUBLISHER ORDER BY Total DESC";

            return PerformQuery(commandText);
        }

        /// <summary>
        /// Return the top publishers for applications from the database
        /// </summary>
        /// <param name="recordCount"></param>
        /// <returns></returns>
        public DataTable StatisticsTopPublishers(bool showIncluded, bool showIgnored, string aAssetIds)
        {
            string commandText = String.Empty;

            if (compactDatabaseType)
            {
                commandText =
                    "SELECT TOP (15) ap._PUBLISHER as Item, COUNT(*) AS Total " +
                    "FROM ASSETS a " +
                    "LEFT JOIN APPLICATION_INSTANCES ai ON (ai._ASSETID = a._ASSETID) " +
                    "LEFT JOIN APPLICATIONS ap ON (ap._APPLICATIONID = ai._APPLICATIONID) ";
            }
            else
            {
                commandText =
                    "SELECT TOP 15 ap._PUBLISHER as Item, COUNT(*) AS Total " +
                    "FROM ASSETS a " +
                    "LEFT JOIN APPLICATION_INSTANCES ai ON (ai._ASSETID = a._ASSETID) " +
                    "LEFT JOIN APPLICATIONS ap ON (ap._APPLICATIONID = ai._APPLICATIONID) ";
            }

            if (showIncluded && !showIgnored)
                commandText += "WHERE ap._IGNORED = 0 AND a._STOCK_STATUS <> 3 ";

            else if (!showIncluded && showIgnored)
                commandText += "WHERE ap._IGNORED = 1 AND a._STOCK_STATUS <> 3 ";

            else
                commandText += "WHERE a._STOCK_STATUS <> 3 ";

            if (aAssetIds != "")
                commandText += "AND a._ASSETID IN (" + aAssetIds + ") ";

            commandText += "AND ap._PUBLISHER IS NOT NULL ";

            commandText += "GROUP BY ap._PUBLISHER ORDER BY Total DESC";

            return PerformQuery(commandText);
        }

        public DataTable StatisticsMsOffice(int recordCount, bool showIncluded, bool showIgnored)
        {
            return StatisticsMsOffice(recordCount, showIncluded, showIgnored, new AssetDAO().GetSelectedAssets());
        }

        public DataTable StatisticsMsOfficeAsGrid(int recordCount, bool showIncluded, bool showIgnored, string aAssetIds)
        {
            string commandText = String.Format(
                "SELECT a._name as \"Asset Name\", ap._NAME as Item " +
                "FROM ASSETS a " +
                "LEFT JOIN APPLICATION_INSTANCES ai ON (ai._ASSETID = a._ASSETID) " +
                "LEFT JOIN APPLICATIONS ap ON (ap._APPLICATIONID = ai._APPLICATIONID) " +
                "WHERE ap._ISOS = 0  " +
                "AND a._STOCK_STATUS <> 3 " +
                "AND ap._NAME LIKE 'Microsoft Office %' ", recordCount);

            if (showIncluded && !showIgnored)
                commandText += "AND ap._IGNORED = 0 ";

            else if (!showIncluded && showIgnored)
                commandText += "AND ap._IGNORED = 1 ";

            //else
            //    commandText += "AND ap._IGNORED = 1 "; 

            if (aAssetIds != "")
                commandText += "AND a._ASSETID IN (" + aAssetIds + ") ";

            commandText += "ORDER BY \"Asset Name\", Item";

            return PerformQuery(commandText);
        }

        /// <summary>
        /// Return the MS Office usage counts from the database
        /// </summary>
        /// <param name="recordCount"></param>
        /// <returns></returns>
        public DataTable StatisticsMsOffice(int recordCount, bool showIncluded, bool showIgnored, string aAssetIds)
        {
            string commandText = String.Format(
                "SELECT ap._NAME as Item, COUNT(*) AS Total " +
                "FROM ASSETS a " +
                "LEFT JOIN APPLICATION_INSTANCES ai ON (ai._ASSETID = a._ASSETID) " +
                "LEFT JOIN APPLICATIONS ap ON (ap._APPLICATIONID = ai._APPLICATIONID) " +
                "WHERE ap._ISOS = 0  " +
                "AND a._STOCK_STATUS <> 3 " +
                "AND ap._NAME LIKE 'Microsoft Office %' ", recordCount);

            if (showIncluded && !showIgnored)
                commandText += "AND ap._IGNORED = 0 ";

            else if (!showIncluded && showIgnored)
                commandText += "AND ap._IGNORED = 1 ";

            if (aAssetIds != "")
                commandText += "AND a._ASSETID IN (" + aAssetIds + ") ";

            commandText += "GROUP BY ap._NAME ORDER BY Total DESC";

            return PerformQuery(commandText);
        }

        /// <summary>
        /// Return the top publishers for applications from the database
        /// </summary>
        /// <param name="recordCount"></param>
        /// <returns></returns>
        public DataTable StatisticsTopComputers(int recordCount, String publisherFilter)
        {
            if (isDebugEnabled) logger.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " in");

            // Create the data table
            DataTable statisticsTable = new DataTable(TableNames.APPLICATION_STATISTICS);

            if (compactDatabaseType)
            {
                statisticsTable = null; ;
            }
            else
            {
                AuditWizardDataAccess lAuditWizardDataAccess = new AuditWizardDataAccess();
                statisticsTable = lAuditWizardDataAccess.StatisticsTopComputers(recordCount, publisherFilter);
            }

            if (isDebugEnabled) logger.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " out");
            return statisticsTable;
        }

        public DataTable StatisticsTopAuditedItem(int recordCount, string category, string item)
        {
            return StatisticsTopAuditedItem(recordCount, category, item, new AssetDAO().GetSelectedAssets());
        }

        public DataTable StatisticsTopAuditedItemAsGrid(int recordCount, string category, string item, string aAssetIds)
        {
            string commandText = "";

            if (compactDatabaseType)
            {
                commandText = String.Format(
                    "SELECT TOP ({0}) a._name as \"Asset Name\", ai._VALUE as Item " +
                    "FROM ASSETS a " +
                    "INNER JOIN AUDITEDITEMS ai on (ai._ASSETID = a._ASSETID) " +
                    "WHERE ai._CATEGORY = '{1}' " +
                    "AND ai._NAME = '{2}' ", recordCount, category, item);
            }
            else
            {
                commandText = String.Format(
                    "SELECT TOP {0} a._name as \"Asset Name\", ai._VALUE as Item " +
                    "FROM ASSETS a " +
                    "INNER JOIN AUDITEDITEMS ai on (ai._ASSETID = a._ASSETID) " +
                    "WHERE ai._CATEGORY = '{1}' " +
                    "AND ai._NAME = '{2}' ", recordCount, category, item);
            }

            if (aAssetIds != "")
                commandText += "AND a._ASSETID IN (" + aAssetIds + ") ";

            commandText += "ORDER BY \"Asset Name\", Item";

            return PerformQuery(commandText);
        }

        /// <summary>
        /// Return the top processors count from the database
        /// </summary>
        /// <param name="recordCount"></param>
        /// <returns></returns>
        public DataTable StatisticsTopAuditedItem(int recordCount, string category, string item, string aAssetIds)
        {
            string commandText = "";

            if (compactDatabaseType)
            {
                commandText = String.Format(
                    "SELECT TOP ({0}) ai._VALUE as Item, COUNT(ai._VALUE) AS Total " +
                    "FROM ASSETS a " +
                    "INNER JOIN AUDITEDITEMS ai on (ai._ASSETID = a._ASSETID) " +
                    "WHERE ai._CATEGORY = '{1}' " +
                    "AND ai._NAME = '{2}' ", recordCount, category, item);
            }
            else
            {
                commandText = String.Format(
                    "SELECT TOP {0} ai._VALUE as Item, COUNT(ai._VALUE) AS Total " +
                    "FROM ASSETS a " +
                    "INNER JOIN AUDITEDITEMS ai on (ai._ASSETID = a._ASSETID) " +
                    "WHERE ai._CATEGORY = '{1}' " +
                    "AND ai._NAME = '{2}' ", recordCount, category, item);
            }

            if (aAssetIds != "")
                commandText += "AND a._ASSETID IN (" + aAssetIds + ") ";

            commandText += "GROUP BY ai._VALUE ORDER BY Total DESC";

            return PerformQuery(commandText);
        }

        public DataTable StatisticsTopOS()
        {
            return StatisticsTopOS(new AssetDAO().GetSelectedAssets());
        }

        public DataTable StatisticsTopOSAsGrid(string aAssetIds)
        {
            string commandText =
                "SELECT a._name as \"Asset Name\", ap._NAME + ' ' + ap._VERSION as Item " +
                "FROM ASSETS a " +
                "LEFT JOIN APPLICATION_INSTANCES ai ON (ai._ASSETID = a._ASSETID) " +
                "LEFT JOIN APPLICATIONS ap ON (ap._APPLICATIONID = ai._APPLICATIONID) " +
                "WHERE ap._ISOS = 1 " +
                "AND a._STOCK_STATUS <> 3 ";

            if (aAssetIds != "")
                commandText += "AND a._ASSETID IN (" + aAssetIds + ") ";

            commandText += "ORDER BY \"Asset Name\"";

            return PerformQuery(commandText);
        }

        /// <summary>
        /// Return the top Operating systems used from the database
        /// </summary>
        /// <param name="recordCount"></param>
        /// <returns></returns>
        public DataTable StatisticsTopOS(string aAssetIds)
        {
            string commandText =
                "SELECT ap._NAME + ' ' + ap._VERSION as Item, COUNT(*) AS Total " +
                "FROM ASSETS a " +
                "LEFT JOIN APPLICATION_INSTANCES ai ON (ai._ASSETID = a._ASSETID) " +
                "LEFT JOIN APPLICATIONS ap ON (ap._APPLICATIONID = ai._APPLICATIONID) " +
                "WHERE ap._ISOS = 1 " +
                "AND a._STOCK_STATUS <> 3 ";

            if (aAssetIds != "")
                commandText += "AND a._ASSETID IN (" + aAssetIds + ") ";

            commandText +=
                "GROUP BY ap._NAME, ap._VERSION ORDER BY Total DESC";

            return PerformQuery(commandText);
        }

        public DataTable StatisticsTopProcessorSpeeds()
        {
            return StatisticsTopProcessorSpeeds(new AssetDAO().GetSelectedAssets());
        }

        public DataTable StatisticsTopProcessorSpeedsAsGrid(string aAssetIds)
        {
            string commandText =
                            "SELECT a._name as \"Asset Name\", ai._VALUE " +
                            "FROM ASSETS a " +
                            "INNER JOIN AUDITEDITEMS ai on (ai._ASSETID = a._ASSETID) " +
                            "WHERE ai._CATEGORY = 'Hardware|CPU' " +
                            "AND ai._NAME = 'Speed' " +
                            "AND a._STOCK_STATUS <> 3 ";

            if (aAssetIds != "")
                commandText += "AND a._ASSETID IN (" + aAssetIds + ") ";

            commandText += "ORDER BY \"Asset Name\"";

            return PerformQuery(commandText);
        }

        /// <summary>
        /// Return the top Processor Speeds used from the database
        /// </summary>
        /// <param name="recordCount"></param>
        /// <returns></returns>
        public DataTable StatisticsTopProcessorSpeeds(string aAssetIds)
        {
            string commandText =
                            "SELECT t.range as Item, count(*) AS Total " +
                            "FROM " +
                            "(select case " +
                            "when cast(ai._VALUE as int) >=0 and cast(ai._VALUE as int) <=1000 then '0 - 1GHz' " +
                            "when cast(ai._VALUE as int) >1000 and cast(ai._VALUE as int) <=1500 then '1.0GHz - 1.5GHz' " +
                            "when cast(ai._VALUE as int) >1500 and cast(ai._VALUE as int) <=2000 then '1.5GHz - 2.0GHz' " +
                            "when cast(ai._VALUE as int) >2000 and cast(ai._VALUE as int) <=2500 then '2GHz - 2.5GHz' " +
                            "when cast(ai._VALUE as int) >2500 and cast(ai._VALUE as int) <=3000 then '2.5GHz - 3.0GHz' " +
                            "when cast(ai._VALUE as int) >3000 and cast(ai._VALUE as int) <=3500 then '3GHz - 3.5GHz' " +
                            "when cast(ai._VALUE as int) >3500 and cast(ai._VALUE as int) <=4000 then '3.5GHz - 4.0GHz' " +
                            "when cast(ai._VALUE as int) >4000 and cast(ai._VALUE as int) <=4500 then '4GHz - 4.5GHz' " +
                            "when cast(ai._VALUE as int) >4500 and cast(ai._VALUE as int) <=5000 then '4.5GHz - 5.0GHz' " +
                            "else '>5GHz' end as range " +
                            "FROM ASSETS a " +
                            "INNER JOIN AUDITEDITEMS ai on (ai._ASSETID = a._ASSETID) " +
                            "WHERE ai._CATEGORY = 'Hardware|CPU' " +
                            "AND ai._NAME = 'Speed' " +
                            "AND a._STOCK_STATUS <> 3 ";

            if (aAssetIds != "")
                commandText += "AND a._ASSETID IN (" + aAssetIds + ") ";

            commandText += ") t GROUP BY t.range ORDER BY Total DESC";
            return PerformQuery(commandText);
        }

        public DataTable StatisticsDefaultBrowser()
        {
            return StatisticsDefaultBrowser(new AssetDAO().GetSelectedAssets());
        }

        public DataTable StatisticsDefaultBrowser(string aAssetIds)
        {
            string commandText =
                "SELECT SUBSTRING(_CATEGORY, 10, LEN(_CATEGORY) - 9) as Browser, COUNT(SUBSTRING(_CATEGORY, 10, LEN(_CATEGORY) - 9)) as Total " +
                "FROM AUDITEDITEMS " +
                "WHERE _NAME = 'Default Browser' " +
                "AND _VALUE = 'Yes'";

            if (aAssetIds != "")
                commandText += "AND _ASSETID IN (" + aAssetIds + ") ";

            commandText += "GROUP BY _CATEGORY";

            return PerformQuery(commandText);
        }

        public DataTable StatisticsDefaultBrowserDrilldown(string aRowLabel)
        {
            string cmdText = String.Format(
                "SELECT a._NAME as \"Asset Name\", a._MAKE as Make, a._MODEL as Model, '{0}' as \"Default Browser\" " +
                "FROM ASSETS a " +
                "INNER JOIN AUDITEDITEMS ai ON (ai._ASSETID = a._ASSETID) " +
                "WHERE _CATEGORY = 'Internet|{0}' " +
                "AND ai._NAME = 'Default Browser' " +
                "AND ai._VALUE = 'Yes' ", aRowLabel);

            cmdText += String.Format("AND a._ASSETID IN ({0})", new AssetDAO().GetSelectedAssets());

            return PerformQuery(cmdText);
        }

        public DataTable StatisticsAssetsAudited()
        {
            return StatisticsAssetsAudited(new AssetDAO().GetSelectedAssets());
        }

        public DataTable StatisticsAssetsAuditedAsGrid(string aAssetIds)
        {
            string commandText =
                "SELECT _name as \"Asset Name\", _LASTAUDIT " +
                "FROM ASSETS ";

            if (aAssetIds != "")
                commandText += "WHERE _ASSETID IN (" + aAssetIds + ") ";

            commandText += "ORDER BY \"Asset Name\"";

            return PerformQuery(commandText);
        }

        /// <summary>
        /// Return the top Processor Speeds used from the database
        /// </summary>
        /// <returns></returns>
        public DataTable StatisticsAssetsAudited(string aAssetIds)
        {
            string commandText =
                "SELECT t.range as Item, count(*) AS Total FROM " +
                "(select case " +
                "when _LASTAUDIT is not null then 'Audited' " +
                "else 'Not Audited' end " +
                "as range " +
                "FROM ASSETS";

            if (aAssetIds != "")
                commandText += " WHERE _ASSETID IN (" + aAssetIds + ") ";

            commandText += ") t GROUP BY t.range ORDER BY Total DESC";

            return PerformQuery(commandText);
        }

        public DataTable StatisticsTopMemoryCapacity()
        {
            return StatisticsTopMemoryCapacity(new AssetDAO().GetSelectedAssets());
        }

        public DataTable StatisticsTopMemoryCapacityAsGrid(string aAssetIds)
        {
            string commandText =
                            "SELECT a._name as \"Asset Name\", ai._VALUE as \"Total RAM\" " +
                            "FROM ASSETS a " +
                            "INNER JOIN AUDITEDITEMS ai on (ai._ASSETID = a._ASSETID) " +
                            "WHERE ai._CATEGORY = 'Hardware|Memory' " +
                            "AND ai._NAME = 'Total RAM' " +
                            "AND a._STOCK_STATUS <> 3 ";

            if (aAssetIds != "")
                commandText += "AND a._ASSETID IN (" + aAssetIds + ") ";

            commandText += "ORDER BY \"Asset Name\"";

            return PerformQuery(commandText);
        }

        /// <summary>
        /// Return the top memory capacities used from the database
        /// </summary>
        /// <param name="aAssetIds"></param>
        /// <returns></returns>
        public DataTable StatisticsTopMemoryCapacity(string aAssetIds)
        {
            string commandText =
                            "SELECT t.range as Item, count(*) AS Total " +
                            "FROM " +
                            "(select case " +
                            "when cast(ai._VALUE as int) >=0 and cast(ai._VALUE as int) <=500 then '0 - 500MB' " +
                            "when cast(ai._VALUE as int) >500 and cast(ai._VALUE as int) <=1000 then '501MB - 1GB' " +
                            "when cast(ai._VALUE as int) >1000 and cast(ai._VALUE as int) <=1500 then '1GB - 1.5GB' " +
                            "when cast(ai._VALUE as int) >1500 and cast(ai._VALUE as int) <=2000 then '1.5GB - 2GB' " +
                            "when cast(ai._VALUE as int) >2000 and cast(ai._VALUE as int) <=3000 then '2GB - 3GB' " +
                            "when cast(ai._VALUE as int) >3000 and cast(ai._VALUE as int) <=4000 then '3GB - 4GB' " +
                            "when cast(ai._VALUE as int) >4000 and cast(ai._VALUE as int) <=5000 then '4GB - 5GB' " +
                            "when cast(ai._VALUE as int) >5000 and cast(ai._VALUE as int) <=6000 then '5GB - 6GB' " +
                            "when cast(ai._VALUE as int) >6000 and cast(ai._VALUE as int) <=7000 then '6GB - 7GB' " +
                            "else '> 7GB' end as range " +
                            "FROM ASSETS a " +
                            "INNER JOIN AUDITEDITEMS ai on (ai._ASSETID = a._ASSETID) " +
                            "WHERE ai._CATEGORY='Hardware|Memory' " +
                            "AND ai._NAME = 'Total RAM' " +
                            "AND a._STOCK_STATUS <> 3 ";

            if (aAssetIds != "")
                commandText += "AND a._ASSETID IN (" + aAssetIds + ") ";

            commandText += ") t GROUP BY t.range ORDER BY Total DESC";

            return PerformQuery(commandText);
        }

        public DataTable StatisticsAssetsByType()
        {
            return StatisticsAssetsByType(new AssetDAO().GetSelectedAssets());
        }

        public DataTable StatisticsAssetsByTypeAsGrid(string aAssetIds)
        {
            string commandText =
                    "SELECT a._name as \"Asset Name\", at._NAME as Item " +
                    "FROM ASSET_TYPES at " +
                    "LEFT JOIN ASSETS a ON (a._ASSETTYPEID = at._ASSETTYPEID) " +
                    "WHERE a._STOCK_STATUS <> 3 ";

            if (aAssetIds != "")
                commandText += "AND a._ASSETID IN (" + aAssetIds + ")";

            commandText += "ORDER BY \"Asset Name\"";

            return PerformQuery(commandText);
        }

        /// <summary>
        /// Return statistics for Assets By Type 
        /// </summary>
        /// <returns></returns>
        public DataTable StatisticsAssetsByType(string aAssetIds)
        {
            string commandText =
                    "SELECT at._NAME as Item, COUNT(*) as Total " +
                    "FROM ASSET_TYPES at " +
                    "LEFT JOIN ASSETS a ON (a._ASSETTYPEID = at._ASSETTYPEID) " +
                    "WHERE a._STOCK_STATUS <> 3 ";

            if (aAssetIds != "")
                commandText += "AND a._ASSETID IN (" + aAssetIds + ") ";

            commandText += "GROUP BY at._NAME ORDER BY Total DESC";

            return PerformQuery(commandText);
        }

        public DataTable StatisticsSupportExpiryDate()
        {
            return StatisticsSupportExpiryDate(new AssetDAO().GetSelectedAssets());
        }

        public DataTable StatisticsSupportExpiryDateAsGrid(string aAssetIds)
        {
            string commandText =
                "SELECT ap._name as \"Application Name\", _SUPPORT_EXPIRES " +
                "FROM LICENSES l " +
                "LEFT JOIN APPLICATIONS ap on (ap._APPLICATIONID = l._APPLICATIONID) " +
                "LEFT JOIN APPLICATION_INSTANCES ai on (ai._APPLICATIONID = l._APPLICATIONID) " +
                "LEFT JOIN ASSETS a on (a._ASSETID = ai._ASSETID) " +
                "WHERE l._SUPPORTED = 1 ";

            if (aAssetIds != "")
                commandText += "AND a._ASSETID IN (" + aAssetIds + ") ";

            commandText += "ORDER BY \"Application Name\"";

            return PerformQuery(commandText);
        }

        /// <summary>
        /// Return statistics for Support Expiry Date 
        /// </summary>
        /// <returns></returns>
        public DataTable StatisticsSupportExpiryDate(string aAssetIds)
        {
            string commandText =
                "SELECT t.range as Item, count(*) AS Total FROM " +
                "(select case " +
                "when ( DATEDIFF(day, GETDATE(), _SUPPORT_EXPIRES) < 0 ) then 'Support Expired' " +
                "when ( DATEDIFF(day, GETDATE(), _SUPPORT_EXPIRES) = 0 ) then 'Expires Today' " +
                "when ( DATEDIFF(day, GETDATE(), _SUPPORT_EXPIRES) > 0 and DATEDIFF(day, GETDATE(), _SUPPORT_EXPIRES) <= 7 ) then 'Expires Within One Week' " +
                "when ( DATEDIFF(day, GETDATE(), _SUPPORT_EXPIRES) > 7 and DATEDIFF(day, GETDATE(), _SUPPORT_EXPIRES) <= 30 ) then 'Expires Between One Week and One Month' " +
                "when ( DATEDIFF(day, GETDATE(), _SUPPORT_EXPIRES) > 30 and DATEDIFF(day, GETDATE(), _SUPPORT_EXPIRES) <= 180 ) then 'Expires Between One Month and Six Months' " +
                "when ( DATEDIFF(day, GETDATE(), _SUPPORT_EXPIRES) > 180 and DATEDIFF(day, GETDATE(), _SUPPORT_EXPIRES) <= 365 ) then 'Expires Between Six Months and One Year' " +
                "else 'Expires In Over a Year' end " +
                "as range " +
                "FROM LICENSES l " +
                "LEFT JOIN APPLICATION_INSTANCES ai on (ai._APPLICATIONID = l._APPLICATIONID) " +
                "WHERE l._SUPPORTED = 1 ";

            if (aAssetIds != "")
                commandText += "AND ai._ASSETID IN (" + aAssetIds + ") ";

            commandText += ") t GROUP BY t.range";

            return PerformQuery(commandText);
        }

        /// <summary>
        /// Return statistics for Support Expiry Date 
        /// </summary>
        /// <returns></returns>
        public DataTable StatisticsSupportExpiryDateDrilldown(string aRowLabel)
        {
            string lWhereClause = String.Empty;

            switch (aRowLabel)
            {
                case "Support Expired":
                    lWhereClause = "AND DATEDIFF(day, GETDATE(), l._SUPPORT_EXPIRES) < 0";
                    break;
                case "Expires Today":
                    lWhereClause = "AND DATEDIFF(day, GETDATE(), _SUPPORT_EXPIRES) = 0";
                    break;
                case "Expires Within One Week":
                    lWhereClause = "AND DATEDIFF(day, GETDATE(), _SUPPORT_EXPIRES) > 0 and DATEDIFF(day, GETDATE(), _SUPPORT_EXPIRES) <= 7";
                    break;
                case "Expires Between One Week and One Month":
                    lWhereClause = "AND DATEDIFF(day, GETDATE(), _SUPPORT_EXPIRES) > 7 and DATEDIFF(day, GETDATE(), _SUPPORT_EXPIRES) <= 30";
                    break;
                case "Expires Between One Month and Six Months":
                    lWhereClause = "AND DATEDIFF(day, GETDATE(), _SUPPORT_EXPIRES) > 30 and DATEDIFF(day, GETDATE(), _SUPPORT_EXPIRES) <= 180";
                    break;
                case "Expires Between Six Months and One Year":
                    lWhereClause = "AND DATEDIFF(day, GETDATE(), _SUPPORT_EXPIRES) > 180 and DATEDIFF(day, GETDATE(), _SUPPORT_EXPIRES) <= 365";
                    break;
                case "Expires In Over a Year":
                    lWhereClause = "AND DATEDIFF(day, GETDATE(), l._SUPPORT_EXPIRES) > 365";
                    break;
            }

            string commandText = String.Format(
                    "SELECT a._PUBLISHER as Publisher, a._NAME as Name, a._VERSION as Version, l._SUPPORT_EXPIRES as ExpiryDate, DATEDIFF(day, GETDATE(), l._SUPPORT_EXPIRES) as DaysUntilExpiry " +
                    "FROM LICENSES l, APPLICATIONS a " +
                    "WHERE l._SUPPORTED = 1 " +
                    "AND a._APPLICATIONID = l._APPLICATIONID " +
                    "{0}", lWhereClause);

            return PerformQuery(commandText);
        }
        /// <summary>
        /// Return statistics for the 'Support Statistics' phase
        /// </summary>
        /// <returns></returns>
        public DataTable SupportStatisticsAsset()
        {
            if (isDebugEnabled) logger.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " in");

            DataTable statisticsTable = new DataTable(TableNames.COMPUTER_STATISTICS);

            if (compactDatabaseType)
            {
                try
                {
                    statisticsTable.Columns.Add(new DataColumn("row"));
                    statisticsTable.Columns.Add(new DataColumn("expired"));
                    statisticsTable.Columns.Add(new DataColumn("expiretoday"));
                    statisticsTable.Columns.Add(new DataColumn("expirethisweek"));
                    statisticsTable.Columns.Add(new DataColumn("expirethismonth"));

                    using (SqlCeConnection conn = DatabaseConnection.CreateOpenCEConnection())
                    {
                        DataRow newRow;
                        newRow = statisticsTable.NewRow();

                        object[] rowArray = new object[5];
                        rowArray[0] = "row";
                        rowArray[1] = QueryAndGetValue(conn, "SELECT COUNT(_ASSETID) FROM ASSET_SUPPORTCONTRACT WHERE _ALERTFLAG=1 AND (DATEDIFF(day, GETDATE(), _SUPPORT_EXPIRY) < 0)");
                        rowArray[2] = QueryAndGetValue(conn, "SELECT COUNT(_ASSETID) FROM ASSET_SUPPORTCONTRACT WHERE _ALERTFLAG=1 AND (DATEDIFF(day, GETDATE(), _SUPPORT_EXPIRY) = 0)");
                        rowArray[3] = QueryAndGetValue(conn, "SELECT COUNT(_ASSETID) FROM ASSET_SUPPORTCONTRACT WHERE _ALERTFLAG=1 AND (DATEDIFF(day, GETDATE(), _SUPPORT_EXPIRY) > 0) AND (DATEDIFF(day, DATEADD(week, 1, GETDATE()), _SUPPORT_EXPIRY) <= 0)");
                        rowArray[4] = QueryAndGetValue(conn, "SELECT COUNT(_ASSETID) FROM ASSET_SUPPORTCONTRACT WHERE _ALERTFLAG=1 AND (DATEDIFF(day, GETDATE(), _SUPPORT_EXPIRY) > 7) AND (DATEDIFF(day, DATEADD(month ,1 ,GETDATE()), _SUPPORT_EXPIRY) <= 0)");

                        newRow.ItemArray = rowArray;
                        statisticsTable.Rows.Add(newRow);
                    }
                }
                catch (SqlCeException ex)
                {
                    Utility.DisplayErrorMessage("A database error has occurred in AuditWizard." + Environment.NewLine + Environment.NewLine +
                            "Please see the log file for further details.");
                    logger.Error("Exception in " + System.Reflection.MethodBase.GetCurrentMethod().Name, ex);
                }
                catch (Exception ex)
                {
                    Utility.DisplayErrorMessage("A database error has occurred in AuditWizard." + Environment.NewLine + Environment.NewLine +
                            "Please see the log file for further details.");

                    logger.Error("Exception in " + System.Reflection.MethodBase.GetCurrentMethod().Name, ex);
                }
            }
            else
            {
                AuditWizardDataAccess lAuditWizardDataAccess = new AuditWizardDataAccess();
                statisticsTable = lAuditWizardDataAccess.SupportStatisticsAsset();
            }

            if (isDebugEnabled) logger.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " out");
            return statisticsTable;
        }

        #endregion

        #region Report Functions



        /// <summary>
        /// Purge the Internet table given a date before which items should be discarded
        /// </summary>
        /// <param name="purgeBeforeDate"></param>
        /// <returns></returns>
        public DataTable GetInternetHistory(DateTime startDate, DateTime endDate, string urlFilter)
        {
            if (isDebugEnabled) logger.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " in");

            DataTable internetTable = new DataTable(TableNames.APPLICATIONS);

            try
            {
                string commandText =
                    "SELECT AUDITEDITEMS.*, ASSETS._NAME AS ASSETNAME, LOCATIONS._FULLNAME AS FULLLOCATIONNAME FROM AUDITEDITEMS " +
                    "LEFT JOIN ASSETS ON (AUDITEDITEMS._ASSETID = ASSETS._ASSETID) " +
                    "LEFT JOIN LOCATIONS ON (ASSETS._LOCATIONID = LOCATIONS._LOCATIONID) " +
                    "WHERE (AUDITEDITEMS._ASSETID <> 0) ";

                if (urlFilter != String.Empty)
                {
                    commandText +=
                    "AND (AUDITEDITEMS._CATEGORY LIKE 'Internet|Internet Explorer|History|%' AND (SUBSTRING(AUDITEDITEMS._CATEGORY, 29, 100)) LIKE @strUrl) ";
                }

                commandText +=
                    "AND (AUDITEDITEMS._CATEGORY LIKE 'Internet|Internet Explorer|History|%')";

                string lAssetIds = new AssetDAO().GetSelectedAssets();

                if (lAssetIds != "")
                    commandText += " AND ASSETS._ASSETID IN (" + lAssetIds + ") ";

                if (compactDatabaseType)
                {
                    using (SqlCeConnection conn = DatabaseConnection.CreateOpenCEConnection())
                    {
                        using (SqlCeCommand command = new SqlCeCommand(commandText, conn))
                        {
                            if (startDate != DateTime.MinValue && endDate != DateTime.MinValue)
                            {
                                command.Parameters.AddWithValue("@cStartDate", startDate);
                                command.Parameters.AddWithValue("@cEndDate", endDate);
                            }

                            if (urlFilter != String.Empty)
                                command.Parameters.AddWithValue("@strUrl", urlFilter);

                            new SqlCeDataAdapter(command).Fill(internetTable);
                        }
                    }
                }
                else
                {
                    using (SqlConnection conn = DatabaseConnection.CreateOpenStandardConnection())
                    {
                        SqlCommand command = new SqlCommand(commandText, conn);

                        if (startDate != DateTime.MinValue && endDate != DateTime.MinValue)
                        {
                            command.Parameters.AddWithValue("@cStartDate", startDate);
                            command.Parameters.AddWithValue("@cEndDate", endDate);
                        }

                        if (urlFilter != String.Empty)
                            command.Parameters.AddWithValue("@strUrl", urlFilter);

                        new SqlDataAdapter(command).Fill(internetTable);
                    }
                }
            }
            catch (SqlException ex)
            {
                Utility.DisplayErrorMessage("A database error has occurred in AuditWizard." + Environment.NewLine + Environment.NewLine +
                        "Please see the log file for further details.");
                logger.Error("Exception in " + System.Reflection.MethodBase.GetCurrentMethod().Name, ex);
            }
            catch (SqlCeException ex)
            {
                Utility.DisplayErrorMessage("A database error has occurred in AuditWizard." + Environment.NewLine + Environment.NewLine +
                        "Please see the log file for further details.");
                logger.Error("Exception in " + System.Reflection.MethodBase.GetCurrentMethod().Name, ex);
            }
            catch (Exception ex)
            {
                Utility.DisplayErrorMessage("A database error has occurred in AuditWizard." + Environment.NewLine + Environment.NewLine +
                        "Please see the log file for further details.");

                logger.Error("Exception in " + System.Reflection.MethodBase.GetCurrentMethod().Name, ex);
            }

            if (isDebugEnabled) logger.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " out");
            return internetTable;
        }

        public DataTable ComputersDiscovered()
        {
            /*rowArray[1] = QueryAndGetValue(conn, "SELECT COUNT(*) FROM ASSETS");
            rowArray[2] = QueryAndGetValue(conn, "SELECT COUNT(*) FROM ASSETS where  _LASTAUDIT IS NOT NULL");
            rowArray[3] = QueryAndGetValue(conn, "SELECT COUNT(*) FROM ASSETS WHERE _LASTAUDIT IS NULL");
            rowArray[4] = QueryAndGetValue(conn, "SELECT COUNT(*) FROM ASSETS WHERE _STOCK_STATUS = 0");
            rowArray[5] = QueryAndGetValue(conn, "SELECT COUNT(*) FROM ASSETS WHERE _STOCK_STATUS = 1");
            rowArray[6] = QueryAndGetValue(conn, "SELECT COUNT(*) FROM ASSETS WHERE _STOCK_STATUS = 2");
            rowArray[7] = QueryAndGetValue(conn, "SELECT COUNT(*) FROM ASSETS WHERE _STOCK_STATUS = 3");
            rowArray[8] = QueryAndGetValue(conn, "SELECT MAX(_LASTAUDIT) FROM ASSETS WHERE _HIDDEN=0");*/


            string commandText =
                "SELECT _NAME as Name, _MAKE as Make, _MODEL as Model FROM ASSETS";

            return PerformQuery(commandText);
        }

        public DataTable ComputersAudited()
        {
            string commandText =
                "SELECT _NAME as Name, _MAKE as Make, _MODEL as Model, _LASTAUDIT as LastAudit FROM ASSETS " +
                "WHERE _LASTAUDIT IS NOT NULL";

            return PerformQuery(commandText);
        }

        public DataTable ComputerLastAudit()
        {
            string commandText = "";

            if (compactDatabaseType)
            {
                commandText =
                    "SELECT TOP (1) _NAME as Name, _MAKE as Make, _MODEL as Model, _LASTAUDIT as \"Last Audit\" FROM ASSETS " +
                    "WHERE _HIDDEN = 0 ORDER BY _LASTAUDIT DESC";
            }
            else
            {
                commandText =
                    "SELECT TOP 1 _NAME as Name, _MAKE as Make, _MODEL as Model, _LASTAUDIT as \"Last Audit\" FROM ASSETS " +
                    "WHERE _HIDDEN = 0 ORDER BY _LASTAUDIT DESC";
            }

            return PerformQuery(commandText);
        }

        public DataTable AssetsInStock()
        {
            string commandText =
                "SELECT _NAME as Name, _MAKE as Make, _MODEL as Model FROM ASSETS " +
                "WHERE _STOCK_STATUS = 0";

            return PerformQuery(commandText);
        }


        public DataTable AssetsInUse()
        {
            string commandText =
                "SELECT _NAME as Name, _MAKE as Make, _MODEL as Model FROM ASSETS " +
                "WHERE _STOCK_STATUS = 1";

            return PerformQuery(commandText);
        }

        public DataTable AssetsPending()
        {
            string commandText =
                "SELECT _NAME as Name, _MAKE as Make, _MODEL as Model FROM ASSETS " +
                "WHERE _STOCK_STATUS = 2";

            return PerformQuery(commandText);
        }

        public DataTable AssetsDisposed()
        {
            string commandText =
                "SELECT _NAME as Name, _MAKE as Make, _MODEL as Model FROM ASSETS " +
                "WHERE _STOCK_STATUS = 3";

            return PerformQuery(commandText);
        }

        public DataTable AgentsDeployed()
        {
            string commandText =
                "SELECT _NAME as Name, _MAKE as Make, _MODEL as Model, _AGENT_VERSION as \"Agent Version\" FROM ASSETS " +
                "WHERE _AGENT_STATUS <> 0";

            return PerformQuery(commandText);
        }

        public DataTable UniqueApplications()
        {
            string commandText =
                "SELECT DISTINCT _PUBLISHER As Publisher, _NAME As Name, _VERSION As Version " +
                "FROM APPLICATIONS " +
                "WHERE _IGNORED = 0 AND _ISOS = 0";

            return PerformQuery(commandText);
        }

        public DataTable TotalApplications()
        {
            string commandText =
                "SELECT DISTINCT _PUBLISHER As Publisher, _NAME As Name, _VERSION As Version " +
                "FROM APPLICATIONS a " +
                "LEFT JOIN APPLICATION_INSTANCES ai on (ai._applicationid = a._applicationid) " +
                "WHERE a._ISOS = 0 " +
                "AND a._IGNORED = 0";

            return PerformQuery(commandText);
        }

        public DataTable AuditedToday()
        {
            string commandText =
                "SELECT _NAME as Name, _MAKE as Make, _MODEL as Model, _LASTAUDIT as LastAudit FROM ASSETS " +
				"WHERE _STOCK_STATUS < 3 AND (datediff(day, _LASTAUDIT ,GETDATE()) = 0)";

            return PerformQuery(commandText);
        }

        public DataTable NotAudited7()
        {
            string commandText =
                "SELECT _NAME as Name, _MAKE as Make, _MODEL as Model, _LASTAUDIT as LastAudit FROM ASSETS " +
				"WHERE _STOCK_STATUS < 3 AND (datediff(day, _LASTAUDIT ,GETDATE()) > 7) AND (datediff(day, _LASTAUDIT ,GETDATE()) <= 14)";

            return PerformQuery(commandText);
        }

        public DataTable NotAudited14()
        {
            string commandText =
                "SELECT _NAME as Name, _MAKE as Make, _MODEL as Model, _LASTAUDIT as LastAudit FROM ASSETS " +
				"WHERE _STOCK_STATUS < 3 AND (datediff(day, _LASTAUDIT ,GETDATE()) > 14) AND (datediff(day, _LASTAUDIT ,GETDATE()) <= 30)";

            DataTable lDataTable = PerformQuery(commandText);
            foreach (DataRow row in lDataTable.Rows)
            {
                //row["LastAudit"] = Convert.ToDateTime(row[3]).ToString("yyyy-MM-dd HH:mm");
                //row["LastAudit"] = "123";
            }

            return lDataTable;
        }

        public DataTable NotAudited30()
        {
            string commandText =
                "SELECT _NAME as Name, _MAKE as Make, _MODEL as Model, _LASTAUDIT as LastAudit FROM ASSETS " +
				"WHERE _STOCK_STATUS < 3 AND (datediff(day, _LASTAUDIT ,GETDATE()) > 30) AND (datediff(day, _LASTAUDIT ,GETDATE()) <= 90)";

            return PerformQuery(commandText);
        }

        public DataTable NotAudited90()
        {
            string commandText =
                "SELECT _NAME as Name, _MAKE as Make, _MODEL as Model, _LASTAUDIT as LastAudit FROM ASSETS " +
				"WHERE _STOCK_STATUS < 3 AND (datediff(day, _LASTAUDIT ,GETDATE()) > 90)";

            return PerformQuery(commandText);
        }

        public DataTable NotAudited()
        {
            string commandText =
                "SELECT _NAME as Name, _MAKE as Make, _MODEL as Model, _LASTAUDIT as LastAudit FROM ASSETS " +
				"WHERE _STOCK_STATUS < 3 AND (_LASTAUDIT is NULL)";

            return PerformQuery(commandText);
        }

        public DataTable LastAlert()
        {
            string commandText = "";

            if (compactDatabaseType)
            {
                commandText =
                    "SELECT TOP (1)" +
                    "CASE " +
                    "WHEN _TYPE = 0 THEN 'Support Expiry' " +
                    "WHEN _TYPE = 1 THEN 'Install/Uninstall' " +
                    "WHEN _TYPE = 2 THEN 'Hardware' " +
                    "WHEN _TYPE = 3 THEN 'Alert Monitor' " +
                    "ELSE 'Unknown Type' END AS Type, " +
                    "CASE " +
                    "WHEN _CATEGORY = 0 THEN 'Expired' " +
                    "WHEN _CATEGORY = 1 THEN 'Added' " +
                    "WHEN _CATEGORY = 2 THEN 'Deleted' " +
                    "WHEN _CATEGORY = 3 THEN 'Changed' " +
                    "ELSE 'Unknown Type' END AS Category,  _MESSAGE as Message, _FiELD1 as Field1, " +
                   "_FIELD2 as Field2, _ALERTDATE as AlertDate, _ASSETNAME as AssetName, _ALERTNAME as AlertName " +
                   "FROM ALERTS " +
                   "ORDER BY _ALERTDATE DESC";
            }
            else
            {
                commandText =
                    "SELECT TOP 1 " +
                    "CASE " +
                    "WHEN _TYPE = 0 THEN 'Support Expiry' " +
                    "WHEN _TYPE = 1 THEN 'Install/Uninstall' " +
                    "WHEN _TYPE = 2 THEN 'Hardware' " +
                    "WHEN _TYPE = 3 THEN 'Alert Monitor' " +
                    "ELSE 'Unknown Type' END AS Type, " +
                    "CASE " +
                    "WHEN _CATEGORY = 0 THEN 'Expired' " +
                    "WHEN _CATEGORY = 1 THEN 'Added' " +
                    "WHEN _CATEGORY = 2 THEN 'Deleted' " +
                    "WHEN _CATEGORY = 3 THEN 'Changed' " +
                    "ELSE 'Unknown Type' END AS Category,  _MESSAGE as Message, _FiELD1 as Field1, " +
                    "_FIELD2 as Field2, _ALERTDATE as AlertDate, _ASSETNAME as AssetName, _ALERTNAME as AlertName " +
                    "FROM ALERTS " +
                    "ORDER BY _ALERTDATE DESC";
            }

            return PerformQuery(commandText);
        }

        public DataTable AlertsToday()
        {
            string commandText =
                "SELECT " +
                "CASE " +
                "WHEN _TYPE = 0 THEN 'Support Expiry' " +
                "WHEN _TYPE = 1 THEN 'Install/Uninstall' " +
                "WHEN _TYPE = 2 THEN 'Hardware' " +
                "WHEN _TYPE = 3 THEN 'Alert Monitor' " +
                "ELSE 'Unknown Type' END AS Type, " +
                "CASE " +
                "WHEN _CATEGORY = 0 THEN 'Expired' " +
                "WHEN _CATEGORY = 1 THEN 'Added' " +
                "WHEN _CATEGORY = 2 THEN 'Deleted' " +
                "WHEN _CATEGORY = 3 THEN 'Changed' " +
                "ELSE 'Unknown Type' END AS Category,  _MESSAGE as Message, _FiELD1 as Field1, " +
                "_FIELD2 as Field2, _ALERTDATE as AlertDate, _ASSETNAME as AssetName, _ALERTNAME as AlertName " +
                "FROM ALERTS " +
                "WHERE (DATEDIFF(day , GETDATE(), _ALERTDATE) = 0)";

            return PerformQuery(commandText);
        }

        public DataTable AlertsThisWeek()
        {
            string commandText =
                "SELECT " +
                "CASE " +
                "WHEN _TYPE = 0 THEN 'Support Expiry' " +
                "WHEN _TYPE = 1 THEN 'Install/Uninstall' " +
                "WHEN _TYPE = 2 THEN 'Hardware' " +
                "WHEN _TYPE = 3 THEN 'Alert Monitor' " +
                "ELSE 'Unknown Type' END AS Type, " +
                "CASE " +
                "WHEN _CATEGORY = 0 THEN 'Expired' " +
                "WHEN _CATEGORY = 1 THEN 'Added' " +
                "WHEN _CATEGORY = 2 THEN 'Deleted' " +
                "WHEN _CATEGORY = 3 THEN 'Changed' " +
                "ELSE 'Unknown Type' END AS Category,  _MESSAGE as Message, _FiELD1 as Field1, " +
                "_FIELD2 as Field2, _ALERTDATE as AlertDate, _ASSETNAME as AssetName, _ALERTNAME as AlertName " +
                "FROM ALERTS " +
                "WHERE (DATEDIFF(day , DATEADD(week ,1 ,GETDATE()), _ALERTDATE) <= 0)";

            return PerformQuery(commandText);
        }

        public DataTable AlertsThisMonth()
        {
            string commandText =
                "SELECT " +
                "CASE " +
                "WHEN _TYPE = 0 THEN 'Support Expiry' " +
                "WHEN _TYPE = 1 THEN 'Install/Uninstall' " +
                "WHEN _TYPE = 2 THEN 'Hardware' " +
                "WHEN _TYPE = 3 THEN 'Alert Monitor' " +
                "ELSE 'Unknown Type' END AS Type, " +
                "CASE " +
                "WHEN _CATEGORY = 0 THEN 'Expired' " +
                "WHEN _CATEGORY = 1 THEN 'Added' " +
                "WHEN _CATEGORY = 2 THEN 'Deleted' " +
                "WHEN _CATEGORY = 3 THEN 'Changed' " +
                "ELSE 'Unknown Type' END AS Category,  _MESSAGE as Message, _FiELD1 as Field1, " +
                "_FIELD2 as Field2, _ALERTDATE as AlertDate, _ASSETNAME as AssetName, _ALERTNAME as AlertName " +
                "FROM ALERTS " +
                "WHERE (DATEDIFF(day, DATEADD(month ,1 ,GETDATE()), _ALERTDATE) <= 0)";

            return PerformQuery(commandText);
        }

        public DataTable SupportExpired()
        {
            string commandText =
                "SELECT a._publisher as Publisher, a._name as Application, a._version as Version, l._support_expires as SupportExpiryDate " +
                "FROM LICENSES l " +
                "INNER JOIN APPLICATIONS a ON (a._applicationid = l._applicationid) " +
                "WHERE _SUPPORTED = 1 AND (DATEDIFF(day, GETDATE(), _SUPPORT_EXPIRES) < 0)";

            return PerformQuery(commandText);
        }

        public DataTable SupportExpireToday()
        {
            string commandText =
                "SELECT a._publisher as Publisher, a._name as Application, a._version as Version, l._support_expires as SupportExpiryDate " +
                "FROM LICENSES l " +
                "INNER JOIN APPLICATIONS a ON (a._applicationid = l._applicationid) " +
                "WHERE _SUPPORTED = 1 AND (DATEDIFF(day, GETDATE(), _SUPPORT_EXPIRES) = 0)";

            return PerformQuery(commandText);
        }

        public DataTable SupportExpireThisWeek()
        {
            string commandText =
                "SELECT a._publisher as Publisher, a._name as Application, a._version as Version, l._support_expires as SupportExpiryDate " +
                "FROM LICENSES l " +
                "INNER JOIN APPLICATIONS a ON (a._applicationid = l._applicationid) " +
                "WHERE _SUPPORTED=1 AND (DATEDIFF(day, GETDATE(), _SUPPORT_EXPIRES) > 0) AND (DATEDIFF(day, DATEADD(week, 1, GETDATE()), _SUPPORT_EXPIRES) <= 0)";

            return PerformQuery(commandText);
        }

        public DataTable SupportExpireThisMonth()
        {
            string commandText =
                "SELECT a._publisher as Publisher, a._name as Application, a._version as Version, l._support_expires as SupportExpiryDate " +
                "FROM LICENSES l " +
                "INNER JOIN APPLICATIONS a ON (a._applicationid = l._applicationid) " +
                "WHERE _SUPPORTED=1 AND (DATEDIFF(day, GETDATE(), _SUPPORT_EXPIRES) > 7) AND (DATEDIFF(day, DATEADD(month ,1 ,GETDATE()), _SUPPORT_EXPIRES) <= 0)";

            return PerformQuery(commandText);
        }

        public DataTable SupportExpiredAsset()
        {
            
            string commandText =
                "SELECT ASSETS._NAME AS Name, ASSET_SUPPORTCONTRACT._CONTRACT_NUMBER AS ContractNumber, SUPPLIERS._NAME AS Supplier, ASSET_SUPPORTCONTRACT._SUPPORT_EXPIRY AS SupportExpiryDate " +

                "FROM   ASSET_SUPPORTCONTRACT INNER JOIN ASSETS ON ASSET_SUPPORTCONTRACT._ASSETID = ASSETS._ASSETID INNER JOIN SUPPLIERS ON ASSET_SUPPORTCONTRACT._SUPPLIERID = SUPPLIERS._SUPPLIERID " +

                "WHERE  ASSET_SUPPORTCONTRACT._ALERTFLAG = 1 AND (DATEDIFF(day, GETDATE(), ASSET_SUPPORTCONTRACT._SUPPORT_EXPIRY) < 0)";                

            return PerformQuery(commandText);
                  
    }

        public DataTable SupportExpireTodayAsset()
        {
            string commandText =
                "SELECT ASSETS._NAME AS Name,ASSET_SUPPORTCONTRACT._CONTRACT_NUMBER AS ContractNumber,SUPPLIERS._NAME AS Supplier,ASSET_SUPPORTCONTRACT._SUPPORT_EXPIRY AS SupportExpiryDate " +

                "FROM   ASSET_SUPPORTCONTRACT INNER JOIN ASSETS ON ASSET_SUPPORTCONTRACT._ASSETID = ASSETS._ASSETID INNER JOIN SUPPLIERS ON ASSET_SUPPORTCONTRACT._SUPPLIERID = SUPPLIERS._SUPPLIERID " +

                "WHERE  _ALERTFLAG = 1 AND (DATEDIFF(day, GETDATE(), _SUPPORT_EXPIRY) = 0)";

            return PerformQuery(commandText);
        }

        public DataTable SupportExpireThisWeekAsset()
        {
            string commandText =
                "SELECT ASSETS._NAME AS Name,ASSET_SUPPORTCONTRACT._CONTRACT_NUMBER AS ContractNumber,SUPPLIERS._NAME AS Supplier,ASSET_SUPPORTCONTRACT._SUPPORT_EXPIRY AS SupportExpiryDate " +

                "FROM   ASSET_SUPPORTCONTRACT INNER JOIN ASSETS ON ASSET_SUPPORTCONTRACT._ASSETID = ASSETS._ASSETID INNER JOIN SUPPLIERS ON ASSET_SUPPORTCONTRACT._SUPPLIERID = SUPPLIERS._SUPPLIERID " +

                "WHERE  _ALERTFLAG = 1 AND (DATEDIFF(day, GETDATE(), _SUPPORT_EXPIRY) > 0) AND (DATEDIFF(day, DATEADD(week, 1, GETDATE()), _SUPPORT_EXPIRY) <= 0)";
    

            return PerformQuery(commandText);
        }

        public DataTable SupportExpireThisMonthAsset()
        {
            string commandText =
                "SELECT ASSETS._NAME AS Name,ASSET_SUPPORTCONTRACT._CONTRACT_NUMBER AS ContractNumber,SUPPLIERS._NAME AS Supplier,ASSET_SUPPORTCONTRACT._SUPPORT_EXPIRY AS SupportExpiryDate " +

                "FROM   ASSET_SUPPORTCONTRACT INNER JOIN ASSETS ON ASSET_SUPPORTCONTRACT._ASSETID = ASSETS._ASSETID INNER JOIN SUPPLIERS ON ASSET_SUPPORTCONTRACT._SUPPLIERID = SUPPLIERS._SUPPLIERID " +

                "WHERE  _ALERTFLAG = 1 AND (DATEDIFF(day, GETDATE(), _SUPPORT_EXPIRY) > 7) AND (DATEDIFF(day, DATEADD(month ,1 ,GETDATE()), _SUPPORT_EXPIRY) <= 0)";
    
            return PerformQuery(commandText);
        }



        public DataTable ApplicationCompliance()
        {
            string commandText =
                "SELECT a._publisher as Publisher, a._name as Application, a._version as Version, l._support_expires as SupportExpiryDate " +
                "FROM LICENSES l " +
                "INNER JOIN APPLICATIONS a ON (a._applicationid = l._applicationid) " +
                "WHERE _SUPPORTED=1 AND (DATEDIFF(day, GETDATE(), _SUPPORT_EXPIRES) > 7) AND (DATEDIFF(day, DATEADD(month ,1 ,GETDATE()), _SUPPORT_EXPIRES) <= 0)";

            return PerformQuery(commandText);
        }

        public DataTable StatisticsServerData(int serverID)
        {
            string commandText = String.Format(
                "select _assetid, _category, _name, _value, _display_units " +
                "from auditeditems " +
                "where _assetid = {0} " +
                "and _category like ('Hardware|Disk Drives|%')", serverID);

            return PerformQuery(commandText);
        }

        public DataTable UniqueDriveLettersForAsset(int assetID)
        {
            string commandText = String.Format(
                "select distinct  _assetid, _category " +
                "from auditeditems " +
                "where _assetid = {0} " +
                "and _category like ('Hardware|Disk Drives|%')", assetID);

            return PerformQuery(commandText);
        }

        public DataTable GetServers()
        {
            string commandText =
                "select a._name, a._assetid " +
                "from asset_types at " +
                "inner join assets a on (a._assettypeid = at._assettypeid) " +
                "where at._name = 'Server' " +
                "or at._name = 'Domain Controller' " +
                "order by a._name";

            return PerformQuery(commandText);
        }

        public DataTable CheckNewPrinterLevels(int assetID, string assetName)
        {
            int supplyLevel;
            string supplyName;
            DataTable resultsDataTable = new DataTable();
            DataColumn dataColumn;

            string lPrinterLevel = new SettingsDAO().GetSetting("NewsFeedPrinter", false);

            lPrinterLevel = (lPrinterLevel == "") ? "25" : lPrinterLevel;
            double printerThreshold = Convert.ToDouble(lPrinterLevel);

            dataColumn = new DataColumn();
            dataColumn.DataType = Type.GetType("System.String");
            dataColumn.ColumnName = "Asset";
            resultsDataTable.Columns.Add(dataColumn);

            dataColumn = new DataColumn();
            dataColumn.DataType = Type.GetType("System.String");
            dataColumn.ColumnName = "Supply Name";
            resultsDataTable.Columns.Add(dataColumn);

            dataColumn = new DataColumn();
            dataColumn.DataType = Type.GetType("System.Int32");
            dataColumn.ColumnName = "Level";
            resultsDataTable.Columns.Add(dataColumn);

            DataTable supplyLevelsDataTable = PerformQuery(String.Format(
                "select _category, _value " +
                "from auditeditems " +
                "where _assetid = {0} " +
                "and _name = 'Supply Level'", assetID));

            foreach (DataRow supplyRow in supplyLevelsDataTable.Rows)
            {
                supplyLevel = Convert.ToInt32(supplyRow.ItemArray[1]);

                if (supplyLevel >= printerThreshold)
                    continue;

                supplyName = supplyRow.ItemArray[0].ToString();
                supplyName = supplyName.Substring(supplyName.LastIndexOf('|') + 1);

                DataRow newDataRow = resultsDataTable.NewRow();
                newDataRow[0] = assetName;
                newDataRow[1] = supplyName;
                newDataRow[2] = supplyLevel;

                resultsDataTable.Rows.Add(newDataRow);
            }

            return resultsDataTable;
        }

        public DataTable CheckForNewLicenseViolations(int assetID)
        {
            DataRow[] rows;
            int installationCount;
            int applicationId;
            int licenseCount;
            string applicationName;
            string applicationVersion;
            double licensePercentUsed;

            DataTable dataTable = PerformQuery(
                "select a._applicationid, a._name as Application, a._version as Version, sum(l._count) as LicenseCount " +
                "from licenses l " +
                "left outer join applications a on (a._applicationid = l._applicationid) " +
                "where a._ignored = 0 " +
                "group by a._applicationid, a._name, a._version");

            DataTable resultsDataTable = new DataTable();
            DataColumn dataColumn;

            dataColumn = new DataColumn();
            dataColumn.DataType = Type.GetType("System.String");
            dataColumn.ColumnName = "Application";
            resultsDataTable.Columns.Add(dataColumn);

            dataColumn = new DataColumn();
            dataColumn.DataType = Type.GetType("System.String");
            dataColumn.ColumnName = "Version";
            resultsDataTable.Columns.Add(dataColumn);

            dataColumn = new DataColumn();
            dataColumn.DataType = Type.GetType("System.Int32");
            dataColumn.ColumnName = "Percentage";
            resultsDataTable.Columns.Add(dataColumn);

            DataTable installsDataTable = PerformQuery(String.Format(
                "select ai._applicationid, count(ai._applicationid) " +
                "from application_instances ai " +
				"left join assets on assets._assetid = ai._assetid " +
				"where assets._stock_status < 3 " + 
                //"where ai._assetid = {0} " +
                "group by ai._applicationid", assetID));

            DataTable notCountedLicensesTable = PerformQuery(
                "select l._applicationid " +
                "from license_types lt " +
                "inner join licenses l on (l._licensetypeid = lt._licensetypeid) " +
                "where lt._counted = 0");

            foreach (DataRow row in dataTable.Rows)
            {
                applicationId = (int)row.ItemArray[0];
                licenseCount = (int)row.ItemArray[3];

                rows = notCountedLicensesTable.Select(String.Format("_APPLICATIONID = {0} ", applicationId));

                if (rows.Length > 0)
                    continue;

                rows = installsDataTable.Select(String.Format("_APPLICATIONID = {0} ", applicationId));

                if (rows.Length == 0)
                    continue;

                installationCount = (int)rows[0].ItemArray[1];

                licensePercentUsed = licenseCount == 0 ? 1.0 : (double)installationCount / licenseCount;

                string lLicenseLevel = new SettingsDAO().GetSetting("NewsFeedLicenses", false);

                lLicenseLevel = (lLicenseLevel == "") ? "100" : lLicenseLevel;
                double licenseThreshold = Convert.ToDouble(lLicenseLevel);

                licenseThreshold = licenseThreshold / 100;

                if (licensePercentUsed <= licenseThreshold)
                    continue;

                applicationName = (string)row.ItemArray[1];
                applicationVersion = (string)row.ItemArray[2];

                DataRow newDataRow = resultsDataTable.NewRow();
                newDataRow[0] = applicationName;
                newDataRow[1] = applicationVersion;
                newDataRow[2] = licensePercentUsed * 100;

                resultsDataTable.Rows.Add(newDataRow);
            }

            return resultsDataTable;
        }

        public DataTable GetAuditedItems(string whereClause)
        {
            return PerformQuery(String.Format("SELECT _assetid, _category, _name, _value FROM auditeditems WHERE {0}", whereClause));
        }

        public DataTable GetCompliantAssets(string assetIds)
        {
            return PerformQuery(String.Format(
                "SELECT _NAME as Name, _MAKE as Make, _MODEL as Model, _LASTAUDIT as LastAudit FROM ASSETS " +
                "WHERE _ASSETID IN ({0})", assetIds));
        }

        public DataTable GetAuditedItemValue(string assetIds)
        {
            string commandText = "SELECT * FROM AUDITEDITEMS WHERE _ASSETID IN (" + assetIds + ")";
            return PerformQuery(commandText);
        }

        public DataTable GetParentAssetTypes()
        {
            string commandText = "SELECT _name FROM asset_types WHERE _parentid IS NULL";
            return PerformQuery(commandText);
        }

        public DataTable GetTopLevelDashboardInventoryItems(string aAssetCategory, int aLocationId)
        {
            string commandText = String.Format(
                "SELECT at._name, count(*) as Total " +
                "FROM asset_types at " +
                "INNER JOIN assets a ON (a._assettypeid = at._assettypeid) " +
                "WHERE at._parentid IN (SELECT _assettypeid FROM asset_types where _name = '{0}') " +
                "AND a._STOCK_STATUS <> 3 ", aAssetCategory);

            if (aLocationId != -1)
                commandText += String.Format("AND a._locationid = {0} ", aLocationId);

            commandText += "GROUP BY at._name ORDER BY Total DESC";

            return PerformQuery(commandText);
        }

        public DataTable GetDashboardInventoryAssets(string aAssetMake, string aAssetTypeName, string aAssetTypeParent, int aLocationId)
        {
            aAssetMake = (aAssetMake == "<no value>") ? "" : aAssetMake;

            string commandText = String.Format(
                "SELECT a._name as \"Asset Name\", a._model as Model, l._fullname as \"Location\" " +
                "FROM asset_types at " +
                "INNER JOIN assets a ON (a._assettypeid = at._assettypeid) " +
                "INNER JOIN locations l ON (l._locationid = a._locationid) " +
                "WHERE a._make = '{0}' " +
                "AND at._name = '{1}' " +
                "AND a._STOCK_STATUS <> 3 " +
                "AND at._parentid IN (SELECT _ASSETTYPEID FROM ASSET_TYPES WHERE _NAME = '{2}')", aAssetMake, aAssetTypeName, aAssetTypeParent);

            if (aLocationId != -1)
                commandText += "AND a._locationid = " + aLocationId;

            commandText += "ORDER BY Location DESC, a._name ASC";

            return PerformQuery(commandText);
        }

        public DataTable GetDashboardInventoryItems(string aAssetType, int aLocationId)
        {
            string commandText = String.Format(
                "SELECT CASE a._make WHEN '' THEN '<no value>' ELSE a._make END, count(a._make) as Total " +
                "FROM asset_types at " +
                "INNER JOIN assets a ON (a._assettypeid = at._assettypeid) " +
                "WHERE at._name = '{0}' " +
                "AND a._STOCK_STATUS <> 3 ", aAssetType);

            if (aLocationId != -1)
                commandText += "AND a._locationid = " + aLocationId;

            commandText += "GROUP BY a._make ORDER BY Total DESC, a._make ASC";

            return PerformQuery(commandText);
        }

        public DataTable GetDashboardPeripheralInventoryItems(int aLocationId)
        {
            string commandText = "";

            if (compactDatabaseType)
            {
                commandText =
                    "SELECT TOP (10) at._name, count(at._name) as Total " +
                    "FROM asset_types at " +
                    "INNER JOIN assets a ON (a._assettypeid = at._assettypeid) " +
                    "WHERE at._parentid IN (SELECT _assettypeid FROM ASSET_TYPES WHERE _NAME = 'Peripherals') " +
                    "AND a._STOCK_STATUS <> 3 ";

                if (aLocationId != -1)
                    commandText += "AND a._locationid = " + aLocationId;

                commandText += "GROUP BY at._name ORDER BY Total DESC, at._name ASC";
            }
            else
            {
                commandText =
                    "SELECT TOP 10 at._name, count(at._name) as Total " +
                    "FROM asset_types at " +
                    "INNER JOIN assets a ON (a._assettypeid = at._assettypeid) " +
                    "WHERE at._parentid IN (SELECT _assettypeid FROM ASSET_TYPES WHERE _NAME = 'Peripherals') " +
                    "AND a._STOCK_STATUS <> 3 ";

                if (aLocationId != -1)
                    commandText += "AND a._locationid = " + aLocationId;

                commandText += "GROUP BY at._name ORDER BY Total DESC, at._name ASC";
            }

            return PerformQuery(commandText);
        }

        #endregion Report Functions
    }
}
