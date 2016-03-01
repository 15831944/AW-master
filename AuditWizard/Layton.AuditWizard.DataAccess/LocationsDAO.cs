using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlServerCe;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace Layton.AuditWizard.DataAccess
{
    public class LocationsDAO
    {
        #region Data

        private string connectionStringCompact = null;
        protected static readonly log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private static readonly bool isDebugEnabled = logger.IsDebugEnabled;
        private bool compactDatabaseType = false;
        private Configuration config = ConfigurationManager.OpenExeConfiguration(Path.Combine(Application.StartupPath, "AuditWizardv8.exe"));

        #endregion

        public LocationsDAO()
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

        #region Locations Table

        /// <summary>
        /// Return a table containing all of the Groups which have been defined
        /// </summary>
        /// <returns></returns>
        public int SetOrganization(string organizationName)
        {
            if (isDebugEnabled) logger.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " in");

            if (compactDatabaseType)
            {
                using (SqlCeConnection conn = DatabaseConnection.CreateOpenCEConnection())
                {
                    try
                    {
                        string commandText =
                            "UPDATE LOCATIONS SET _FULLNAME = @cOrganization, _NAME = @cOrganization WHERE _PARENTID is NULL";

                        using (SqlCeCommand command = new SqlCeCommand(commandText, conn))
                        {
                            command.Parameters.AddWithValue("@cOrganization", organizationName);
                            command.ExecuteNonQuery();
                        }

                        commandText =
                            "UPDATE DOMAINS SET _NAME = @cOrganization WHERE _PARENTID is NULL";

                        using (SqlCeCommand command = new SqlCeCommand(commandText, conn))
                        {
                            command.Parameters.AddWithValue("@cOrganization", organizationName);
                            command.ExecuteNonQuery();
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
            }
            else
            {
                AuditWizardDataAccess lAuditWizardDataAccess = new AuditWizardDataAccess();
                lAuditWizardDataAccess.SetOrganization(organizationName);
            }

            if (isDebugEnabled) logger.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " out");
            return 0;
        }

        /// <summary>
        /// Return a table containing all of the Groups which have been defined
        /// </summary>
        /// <returns></returns>
        public int UpdateRootDomain(string organizationName)
        {
            if (isDebugEnabled) logger.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " in");

            try
            {
                string commandText = "UPDATE DOMAINS SET _NAME = @cOrganization WHERE _PARENTID is NULL";

                if (compactDatabaseType)
                {
                    using (SqlCeConnection conn = DatabaseConnection.CreateOpenCEConnection())
                    {
                        using (SqlCeCommand command = new SqlCeCommand(commandText, conn))
                        {
                            command.Parameters.AddWithValue("@cOrganization", organizationName);
                            command.ExecuteNonQuery();
                        }
                    }
                }
                else
                {
                    using (SqlConnection conn = DatabaseConnection.CreateOpenStandardConnection())
                    {
                        using (SqlCommand command = new SqlCommand(commandText, conn))
                        {
                            command.Parameters.AddWithValue("@cOrganization", organizationName);
                            command.ExecuteNonQuery();
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
            return 0;
        }


        /// <summary>
        /// Return a table containing all of the Groups which have been defined
        /// </summary>
        /// <returns></returns>
        public DataTable GetGroups(AssetGroup aParentGroup)
        {
            if (isDebugEnabled) logger.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " in");

            DataTable table = new DataTable(TableNames.LOCATIONS);

            if (compactDatabaseType)
            {
                try
                {
                    using (SqlCeConnection conn = DatabaseConnection.CreateOpenCEConnection())
                    {
                        if (aParentGroup.GroupType == AssetGroup.GROUPTYPE.userlocation)
                        {
                            string commandText =
                                "SELECT _LOCATIONID, _FULLNAME, _NAME ,_PARENTID ,_START_IPADDRESS ,_END_IPADDRESS ,_HIDDEN " +
                                "FROM LOCATIONS ";

                            if (aParentGroup.GroupID > 0)
                            {
                                commandText += "WHERE _PARENTID = @nParentID ";
                            }
                            else
                            {
                                commandText += "WHERE _PARENTID IS NULL ";
                            }

                            commandText += "ORDER BY _NAME";

                            SqlCeParameter[] spParams = new SqlCeParameter[1];
                            spParams[0] = new SqlCeParameter("@nParentID", aParentGroup.GroupID);

                            using (SqlCeCommand command = new SqlCeCommand(commandText, conn))
                            {
                                command.Parameters.AddRange(spParams);
                                new SqlCeDataAdapter(command).Fill(table);
                            }
                        }
                        else
                        {
                            string commandText =
                                "SELECT _DOMAINID, _NAME ,_PARENTID ,_HIDDEN " +
                                "FROM DOMAINS ";

                            if (aParentGroup.GroupID > 0)
                            {
                                commandText += "WHERE _PARENTID = @nParentID ";
                            }
                            else
                            {
                                commandText += "WHERE _PARENTID IS NULL ";
                            }

                            commandText += "ORDER BY _NAME";

                            SqlCeParameter[] spParams = new SqlCeParameter[1];
                            spParams[0] = new SqlCeParameter("@nParentID", aParentGroup.GroupID);

                            using (SqlCeCommand command = new SqlCeCommand(commandText, conn))
                            {
                                command.Parameters.AddRange(spParams);
                                new SqlCeDataAdapter(command).Fill(table);
                            }
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
                table = lAuditWizardDataAccess.GetGroups(aParentGroup);
            }

            if (isDebugEnabled) logger.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " out");
            return table;
        }

        public bool IsRootLocation(int aLocationId)
        {
            string cmdText =
                    "SELECT _PARENTID " +
                    "FROM LOCATIONS WHERE _LOCATIONID = " + aLocationId;

            return PerformScalarQuery(cmdText) == String.Empty;
        }

        public DataTable GetRootLocation()
        {
            if (isDebugEnabled) logger.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " in");

            DataTable table = new DataTable();

            try
            {
                string commandText =
                    "SELECT _LOCATIONID, _FULLNAME, _NAME, _PARENTID, _START_IPADDRESS, _END_IPADDRESS, _HIDDEN " +
                    "FROM LOCATIONS WHERE _PARENTID IS NULL ORDER BY _NAME";

                if (compactDatabaseType)
                {
                    using (SqlCeConnection conn = DatabaseConnection.CreateOpenCEConnection())
                    {
                        using (SqlCeCommand command = new SqlCeCommand(commandText, conn))
                        {
                            new SqlCeDataAdapter(command).Fill(table);
                        }
                    }
                }
                else
                {
                    using (SqlConnection conn = DatabaseConnection.CreateOpenStandardConnection())
                    {
                        using (SqlCommand command = new SqlCommand(commandText, conn))
                        {
                            new SqlDataAdapter(command).Fill(table);
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
            return table;
        }

        public DataTable GetLocationByParentId(int aParentId)
        {
            if (isDebugEnabled) logger.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " in");

            DataTable table = new DataTable();

            try
            {
                string commandText =
                    "SELECT _LOCATIONID, _FULLNAME, _NAME, _PARENTID, _START_IPADDRESS, _END_IPADDRESS, _HIDDEN " +
                    "FROM LOCATIONS WHERE _PARENTID = @nParentId ORDER BY _NAME";

                if (compactDatabaseType)
                {
                    using (SqlCeConnection conn = DatabaseConnection.CreateOpenCEConnection())
                    {
                        SqlCeParameter[] spParams = new SqlCeParameter[1];
                        spParams[0] = new SqlCeParameter("@nParentId", aParentId);

                        using (SqlCeCommand command = new SqlCeCommand(commandText, conn))
                        {
                            command.Parameters.AddRange(spParams);
                            new SqlCeDataAdapter(command).Fill(table);
                        }
                    }
                }
                else
                {
                    using (SqlConnection conn = DatabaseConnection.CreateOpenStandardConnection())
                    {
                        SqlParameter[] spParams = new SqlParameter[1];
                        spParams[0] = new SqlParameter("@nParentId", aParentId);

                        using (SqlCommand command = new SqlCommand(commandText, conn))
                        {
                            command.Parameters.AddRange(spParams);
                            new SqlDataAdapter(command).Fill(table);
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
            return table;
        }

        public DataTable GetLocationById(int aGroupId)
        {
            if (isDebugEnabled) logger.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " in");

            DataTable table = new DataTable();

            try
            {
                string commandText =
                    "SELECT _LOCATIONID, _FULLNAME, _NAME, _PARENTID, _START_IPADDRESS, _END_IPADDRESS, _HIDDEN " +
                    "FROM LOCATIONS WHERE _LOCATIONID = @nLocationId ORDER BY _NAME";

                if (compactDatabaseType)
                {
                    using (SqlCeConnection conn = DatabaseConnection.CreateOpenCEConnection())
                    {
                        SqlCeParameter[] spParams = new SqlCeParameter[1];
                        spParams[0] = new SqlCeParameter("@nLocationId", aGroupId);

                        using (SqlCeCommand command = new SqlCeCommand(commandText, conn))
                        {
                            command.Parameters.AddRange(spParams);
                            new SqlCeDataAdapter(command).Fill(table);
                        }
                    }
                }
                else
                {
                    using (SqlConnection conn = DatabaseConnection.CreateOpenStandardConnection())
                    {
                        SqlParameter[] spParams = new SqlParameter[1];
                        spParams[0] = new SqlParameter("@nLocationId", aGroupId);

                        using (SqlCommand command = new SqlCommand(commandText, conn))
                        {
                            command.Parameters.AddRange(spParams);
                            new SqlDataAdapter(command).Fill(table);
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
            return table;
        }

        public List<string> GetChildLocationIds(string aParentLocationIds)
        {
            List<string> groups = new List<string>();

            if (aParentLocationIds == String.Empty)
                return groups;

            foreach (string groupId in aParentLocationIds.Split(','))
            {
                groups.Add(groupId);
                GetChildLocationIds(groupId, groups);
            }

            groups.Sort();
            return groups;
        }

        private void GetChildLocationIds(string aParentLocationId, List<string> groups)
        {
            string childId;

            string cmdText = "SELECT _LOCATIONID FROM LOCATIONS WHERE _PARENTID = " + aParentLocationId;
            DataTable dt = PerformQuery(cmdText);

            foreach (DataRow row in dt.Rows)
            {
                childId = row[0].ToString();
                groups.Add(childId);
                GetChildLocationIds(childId, groups);
            }
        }

        /// <summary>
        /// Add a new Group to the database
        /// </summary>
        /// <param name="aGroup"></param>
        /// <returns></returns>
        public int GroupAdd(AssetGroup aGroup)
        {
            if (isDebugEnabled) logger.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " in");
            int lItemID = 0;

            // If the item passed in already exists then we should update rather than add
            if (aGroup.GroupID != 0)
            {
                GroupUpdate(aGroup);
                UpdateChildLocationsFullName(aGroup.GroupID);

                // quick fix here as the root domain is not updated
                // if this the parent location then the domain needs to be updated to match
                if (aGroup.GroupType == AssetGroup.GROUPTYPE.userlocation && aGroup.ParentID == 0)
                    UpdateRootDomain(aGroup.Name);

                return aGroup.GroupID;
            }

            try
            {
                string commandText = String.Format("SELECT _LOCATIONID FROM LOCATIONS WHERE _FULLNAME = '{0}'", aGroup.FullName);

                if (compactDatabaseType)
                {
                    using (SqlCeConnection conn = DatabaseConnection.CreateOpenCEConnection())
                    {
                        if (aGroup.GroupType == AssetGroup.GROUPTYPE.userlocation)
                        {
                            using (SqlCeCommand command = new SqlCeCommand(commandText, conn))
                            {
                                //command.Parameters.AddWithValue("@cFullName", aGroup.FullName);
                                object result = command.ExecuteScalar();

                                if ((result != null) && (result.GetType() != typeof(DBNull)))
                                    lItemID = (int)result;
                            }

                            if (lItemID == 0)
                            {
                                commandText = String.Format(
                                    "INSERT INTO LOCATIONS " +
                                    "(_FULLNAME, _NAME ,_PARENTID ,_START_IPADDRESS ,_END_IPADDRESS ,_HIDDEN) " +
                                    "VALUES " +
                                    "('{0}', '{1}', @nParentID, '{2}', '{3}', {4})",
                                    aGroup.FullName,
                                    aGroup.Name,
                                    aGroup.StartIP,
                                    aGroup.EndIP,
                                    0);

                                SqlCeParameter[] spParams = new SqlCeParameter[1];

                                if (aGroup.ParentID == 0)
                                    spParams[0] = new SqlCeParameter("@nParentID", DBNull.Value);
                                else
                                    spParams[0] = new SqlCeParameter("@nParentID", aGroup.ParentID);

                                using (SqlCeCommand command = new SqlCeCommand(commandText, conn))
                                {
                                    command.Parameters.AddRange(spParams);
                                    command.ExecuteNonQuery();
                                }

                                // then get id of newly inserted record
                                using (SqlCeCommand command = new SqlCeCommand("SELECT @@IDENTITY", conn))
                                {
                                    lItemID = Convert.ToInt32(command.ExecuteScalar());
                                }
                            }
                        }

                        else
                        {
                            commandText = String.Format(
                                "INSERT INTO DOMAINS " +
                                "(_NAME ,_PARENTID ,_HIDDEN) " +
                                "VALUES " +
                                "('{0}', @nParentID, {1})",
                                aGroup.Name,
                                0);

                            SqlCeParameter[] spParams = new SqlCeParameter[1];

                            if (aGroup.ParentID == 0)
                                spParams[0] = new SqlCeParameter("@nParentID", DBNull.Value);
                            else
                                spParams[0] = new SqlCeParameter("@nParentID", aGroup.ParentID);

                            using (SqlCeCommand command = new SqlCeCommand(commandText, conn))
                            {
                                command.Parameters.AddRange(spParams);
                                command.ExecuteNonQuery();
                            }

                            // then get id of newly inserted record
                            using (SqlCeCommand command = new SqlCeCommand("SELECT @@IDENTITY", conn))
                            {
                                lItemID = Convert.ToInt32(command.ExecuteScalar());
                            }
                        }
                    }
                }
                else
                {
                    using (SqlConnection conn = DatabaseConnection.CreateOpenStandardConnection())
                    {
                        if (aGroup.GroupType == AssetGroup.GROUPTYPE.userlocation)
                        {
                            using (SqlCommand command = new SqlCommand(commandText, conn))
                            {
                                //command.Parameters.AddWithValue("@cFullName", aGroup.FullName);
                                object result = command.ExecuteScalar();

                                if ((result != null) && (result.GetType() != typeof(DBNull)))
                                    lItemID = (int)result;
                            }

                            if (lItemID == 0)
                            {
                                commandText = String.Format(
                                    "INSERT INTO LOCATIONS " +
                                    "(_FULLNAME, _NAME ,_PARENTID ,_START_IPADDRESS ,_END_IPADDRESS ,_HIDDEN) " +
                                    "VALUES " +
                                    "('{0}', '{1}', @nParentID, '{2}', '{3}', {4})",
                                    aGroup.FullName,
                                    aGroup.Name,
                                    aGroup.StartIP,
                                    aGroup.EndIP,
                                    0);

                                SqlParameter[] spParams = new SqlParameter[1];

                                if (aGroup.ParentID == 0)
                                    spParams[0] = new SqlParameter("@nParentID", DBNull.Value);
                                else
                                    spParams[0] = new SqlParameter("@nParentID", aGroup.ParentID);

                                using (SqlCommand command = new SqlCommand(commandText, conn))
                                {
                                    command.Parameters.AddRange(spParams);
                                    command.ExecuteNonQuery();
                                }

                                // then get id of newly inserted record
                                using (SqlCommand command = new SqlCommand("SELECT @@IDENTITY", conn))
                                {
                                    lItemID = Convert.ToInt32(command.ExecuteScalar());
                                }
                            }
                        }
                        else
                        {
                            commandText = String.Format(
                                "INSERT INTO DOMAINS " +
                                "(_NAME ,_PARENTID ,_HIDDEN) " +
                                "VALUES " +
                                "('{0}', @nParentID, {1})",
                                aGroup.Name,
                                0);

                            SqlParameter[] spParams = new SqlParameter[1];

                            if (aGroup.ParentID == 0)
                                spParams[0] = new SqlParameter("@nParentID", DBNull.Value);
                            else
                                spParams[0] = new SqlParameter("@nParentID", aGroup.ParentID);

                            using (SqlCommand command = new SqlCommand(commandText, conn))
                            {
                                command.Parameters.AddRange(spParams);
                                command.ExecuteNonQuery();
                            }

                            // then get id of newly inserted record
                            using (SqlCommand command = new SqlCommand("SELECT @@IDENTITY", conn))
                            {
                                lItemID = Convert.ToInt32(command.ExecuteScalar());
                            }
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
            return lItemID;
        }

        public void UpdateChildLocationsFullName(int aGroupId)
        {
            // Bug #609
            // Ensure we update the _FULLNAME column to match the new _NAME
            foreach (DataRow row in GetLocationByParentId(aGroupId).Rows)
            {
                AssetGroup childGroup = new AssetGroup(row, AssetGroup.GROUPTYPE.userlocation);
                AssetGroup origGroup = childGroup;

                string newFullName = childGroup.Name;

                while (childGroup.ParentID != 0)
                {
                    childGroup = new AssetGroup(GetLocationById(childGroup.ParentID).Rows[0], AssetGroup.GROUPTYPE.userlocation);
                    newFullName = childGroup.Name + "\\" + newFullName;
                }

                origGroup.FullName = newFullName;
                GroupUpdate(origGroup);

                UpdateChildLocationsFullName(origGroup.GroupID);
            }
        }

        /// <summary>
        /// Update the specified group
        /// </summary>
        /// <param name="aGroup"></param>
        /// <returns></returns>
        public int GroupUpdate(AssetGroup aGroup)
        {
            if (isDebugEnabled) logger.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " in");

            int lItemId = 0;

            // If the item passed in does not already exists then we need to add not update
            if (aGroup.GroupID == 0)
                return GroupAdd(aGroup);

            lItemId = aGroup.GroupID;

            try
            {
                if (compactDatabaseType)
                {
                    using (SqlCeConnection conn = DatabaseConnection.CreateOpenCEConnection())
                    {
                        if (aGroup.GroupType == AssetGroup.GROUPTYPE.userlocation)
                        {
                            string commandText = String.Format(
                                "UPDATE LOCATIONS SET " +
                                "_FULLNAME = '{0}', " +
                                "_NAME = '{1}', " +
                                "_PARENTID = @nParentID, " +
                                "_START_IPADDRESS = '{2}', " +
                                "_END_IPADDRESS	= '{3}', " +
                                "_HIDDEN = {4} " +
                                "WHERE " +
                                "_LOCATIONID = {5}",
                                aGroup.FullName,
                                aGroup.Name,
                                aGroup.StartIP,
                                aGroup.EndIP,
                                0,
                                aGroup.GroupID);

                            SqlCeParameter[] spParams = new SqlCeParameter[1];

                            if (aGroup.ParentID == 0)
                                spParams[0] = new SqlCeParameter("@nParentID", DBNull.Value);
                            else
                                spParams[0] = new SqlCeParameter("@nParentID", aGroup.ParentID);

                            using (SqlCeCommand command = new SqlCeCommand(commandText, conn))
                            {
                                command.Parameters.AddRange(spParams);
                                command.ExecuteNonQuery();
                            }
                        }

                        else
                        {
                            string commandText = String.Format(
                                "UPDATE DOMAINS  SET " +
                                "_NAME = '{0}', " +
                                "_PARENTID = @nParentID, " +
                                "_HIDDEN = {1} " +
                                "WHERE " +
                                "_DOMAINID = {2}",
                                aGroup.Name,
                                0,
                                aGroup.GroupID);

                            SqlCeParameter[] spParams = new SqlCeParameter[1];

                            if (aGroup.ParentID == 0)
                                spParams[0] = new SqlCeParameter("@nParentID", DBNull.Value);
                            else
                                spParams[0] = new SqlCeParameter("@nParentID", aGroup.ParentID);

                            using (SqlCeCommand command = new SqlCeCommand(commandText, conn))
                            {
                                command.Parameters.AddRange(spParams);
                                command.ExecuteNonQuery();
                            }
                        }
                    }
                }
                else
                {
                    using (SqlConnection conn = DatabaseConnection.CreateOpenStandardConnection())
                    {
                        if (aGroup.GroupType == AssetGroup.GROUPTYPE.userlocation)
                        {
                            string commandText = String.Format(
                                "UPDATE LOCATIONS SET " +
                                "_FULLNAME = '{0}', " +
                                "_NAME = '{1}', " +
                                "_PARENTID = @nParentID, " +
                                "_START_IPADDRESS = '{2}', " +
                                "_END_IPADDRESS	= '{3}', " +
                                "_HIDDEN = {4} " +
                                "WHERE " +
                                "_LOCATIONID = {5}",
                                aGroup.FullName,
                                aGroup.Name,
                                aGroup.StartIP,
                                aGroup.EndIP,
                                0,
                                aGroup.GroupID);

                            SqlParameter[] spParams = new SqlParameter[1];

                            if (aGroup.ParentID == 0)
                                spParams[0] = new SqlParameter("@nParentID", DBNull.Value);
                            else
                                spParams[0] = new SqlParameter("@nParentID", aGroup.ParentID);

                            using (SqlCommand command = new SqlCommand(commandText, conn))
                            {
                                command.Parameters.AddRange(spParams);
                                command.ExecuteNonQuery();
                            }
                        }

                        else
                        {
                            string commandText = String.Format(
                                "UPDATE DOMAINS  SET " +
                                "_NAME = '{0}', " +
                                "_PARENTID = @nParentID, " +
                                "_HIDDEN = {1} " +
                                "WHERE " +
                                "_DOMAINID = {2}",
                                aGroup.Name,
                                0,
                                aGroup.GroupID);

                            SqlParameter[] spParams = new SqlParameter[1];

                            if (aGroup.ParentID == 0)
                                spParams[0] = new SqlParameter("@nParentID", DBNull.Value);
                            else
                                spParams[0] = new SqlParameter("@nParentID", aGroup.ParentID);

                            using (SqlCommand command = new SqlCommand(commandText, conn))
                            {
                                command.Parameters.AddRange(spParams);
                                command.ExecuteNonQuery();
                            }
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
            return lItemId;
        }

        /// <summary>
        /// Delete the specified Group from the database - note it is the callers responsibility to
        /// ensure that the referential integrity is maintained - that is no assets refer to this group and
        /// there are no children of the group which would be left orphaned
        /// </summary>
        /// <param name="licenseID"></param>
        /// <returns></returns>
        public int GroupDelete(AssetGroup aGroup)
        {
            if (isDebugEnabled) logger.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " in");
            int references = 0;

            if (compactDatabaseType)
            {
                SqlCeTransaction transaction = null;


                int lParentId = -1;

                using (SqlCeConnection conn = DatabaseConnection.CreateOpenCEConnection())
                {
                    try
                    {
                        transaction = conn.BeginTransaction();

                        if (aGroup.GroupType == AssetGroup.GROUPTYPE.userlocation)
                        {
                            string commandText =
                                "SELECT _PARENTID FROM LOCATIONS WHERE _LOCATIONID = @nLocationID";

                            using (SqlCeCommand command = new SqlCeCommand(commandText, conn))
                            {
                                command.Parameters.AddWithValue("@nLocationID", aGroup.GroupID);

                                object result = command.ExecuteScalar();

                                if ((result != null) && (result.GetType() != typeof(DBNull)))
                                    lParentId = (int)result;
                            }

                            if (lParentId != -1)
                            {
                                commandText =
                                    "UPDATE ASSETS SET _LOCATIONID = @parentLocationID WHERE _LOCATIONID = @nLocationID";

                                using (SqlCeCommand command = new SqlCeCommand(commandText, conn))
                                {
                                    command.Parameters.AddWithValue("@nLocationID", aGroup.GroupID);
                                    command.Parameters.AddWithValue("@parentLocationID", lParentId);

                                    command.ExecuteNonQuery();
                                }

                                commandText = "DELETE FROM LOCATIONS WHERE _LOCATIONID = @nLocationID";

                                using (SqlCeCommand command = new SqlCeCommand(commandText, conn))
                                {
                                    command.Parameters.AddWithValue("@nLocationID", aGroup.GroupID);
                                    command.ExecuteNonQuery();
                                }
                            }
                        }
                        else
                        {
                            string commandText =
                                "UPDATE ASSETS SET _DOMAINID=0 WHERE _DOMAINID = @nDomainID";

                            using (SqlCeCommand command = new SqlCeCommand(commandText, conn))
                            {
                                command.Parameters.AddWithValue("@nDomainID", aGroup.GroupID);
                                command.ExecuteNonQuery();
                            }

                            commandText = "DELETE FROM DOMAINS WHERE _DOMAINID = @nDomainID";

                            using (SqlCeCommand command = new SqlCeCommand(commandText, conn))
                            {
                                command.Parameters.AddWithValue("@nDomainID", aGroup.GroupID);
                                command.ExecuteNonQuery();
                            }
                        }

                        transaction.Commit();
                    }
                    catch (SqlCeException ex)
                    {
                        transaction.Rollback();

                        Utility.DisplayErrorMessage("A database error has occurred in AuditWizard." + Environment.NewLine + Environment.NewLine +
                                "Please see the log file for further details.");
                        logger.Error("Exception in " + System.Reflection.MethodBase.GetCurrentMethod().Name, ex);
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();

                        Utility.DisplayErrorMessage("A database error has occurred in AuditWizard." + Environment.NewLine + Environment.NewLine +
                                "Please see the log file for further details.");

                        logger.Error("Exception in " + System.Reflection.MethodBase.GetCurrentMethod().Name, ex);
                    }
                }
            }
            else
            {
                AuditWizardDataAccess lAuditWizardDataAccess = new AuditWizardDataAccess();
                references = lAuditWizardDataAccess.GroupDelete(aGroup);
            }

            if (isDebugEnabled) logger.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " out");
            return references;
        }

        /// <summary>
        /// Return a table containing all of the LOCATIONS which have been defined
        /// </summary>
        /// <returns></returns>
        public DataTable GetAllLocationsLight()
        {
            if (isDebugEnabled) logger.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " in");

            DataTable table = new DataTable(TableNames.LOCATIONS);

            if (compactDatabaseType)
            {
                try
                {
                    using (SqlCeConnection conn = DatabaseConnection.CreateOpenCEConnection())
                    {
                        string commandText =
                        "SELECT _LOCATIONID, _FULLNAME FROM LOCATIONS";

                        using (SqlCeCommand command = new SqlCeCommand(commandText, conn))
                        {
                            new SqlCeDataAdapter(command).Fill(table);
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
                table = lAuditWizardDataAccess.GetAllLocations();
            }

            if (isDebugEnabled) logger.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " out");
            return table;
        }

        private DataTable GetAllLocationIds()
        {
            string commandText = "SELECT _LOCATIONID FROM LOCATIONS";
            return PerformQuery(commandText);
        }

        public string GetSelectedLocations()
        {
            string lGroups = new LocationSettingsDAO().GetSetting("LocationFilterGroups");
            //StringBuilder sb;

            //if (lGroups == "")
            //{
            //    sb = new StringBuilder();
            //    DataTable dt = GetAllLocationIds();
            //    foreach (DataRow assetRow in dt.Rows)
            //    {
            //        sb.Append(assetRow[0].ToString() + ",");
            //    }

            //    lGroups = sb.ToString().TrimEnd(',');
            //}

            return lGroups;
        }

        /// <summary>
        /// Return a table containing all of the LOCATIONS which have been defined
        /// </summary>
        /// <returns></returns>
        public DataTable GetAllLocations()
        {
            if (isDebugEnabled) logger.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " in");

            DataTable table = new DataTable(TableNames.LOCATIONS);

            if (compactDatabaseType)
            {
                try
                {
                    using (SqlCeConnection conn = DatabaseConnection.CreateOpenCEConnection())
                    {
                        string commandText =
                        "SELECT _LOCATIONID, _FULLNAME, _NAME ,_PARENTID ,_START_IPADDRESS ,_END_IPADDRESS ,_HIDDEN " +
                        "FROM LOCATIONS";

                        using (SqlCeCommand command = new SqlCeCommand(commandText, conn))
                        {
                            new SqlCeDataAdapter(command).Fill(table);
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
                table = lAuditWizardDataAccess.GetAllLocations();
            }

            if (isDebugEnabled) logger.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " out");
            return table;
        }


        /// <summary>
        /// Return the id of the specified Location given its full name
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public int LocationFind(string aFullname)
        {
            if (isDebugEnabled) logger.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " in");
            int lItemId = 0;

            if (compactDatabaseType)
            {
                try
                {
                    using (SqlCeConnection conn = DatabaseConnection.CreateOpenCEConnection())
                    {
                        string commandText =
                            "SELECT _LOCATIONID FROM LOCATIONS WHERE _FULLNAME=@cFullName";

                        SqlCeParameter[] spParams = new SqlCeParameter[1];
                        spParams[0] = new SqlCeParameter("@cFullName", aFullname);

                        using (SqlCeCommand command = new SqlCeCommand(commandText, conn))
                        {
                            command.Parameters.AddRange(spParams);
                            object result = command.ExecuteScalar();

                            if ((result != null) && (result.GetType() != typeof(DBNull)))
                                lItemId = (int)result;
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
                lItemId = lAuditWizardDataAccess.LocationFind(aFullname);
            }

            if (isDebugEnabled) logger.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " out");
            return lItemId;
        }


        /// <summary>
        /// Return the database index of the specified Location given its full name
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public int LocationFindByName(string aName)
        {
            if (isDebugEnabled) logger.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " in");
            int lItemId = 0;

            string commandText = String.Format("SELECT _LOCATIONID FROM LOCATIONS WHERE _NAME='{0}'", aName);

            try
            {
                if (compactDatabaseType)
                {
                    using (SqlCeConnection conn = DatabaseConnection.CreateOpenCEConnection())
                    {
                        using (SqlCeCommand command = new SqlCeCommand(commandText, conn))
                        {
                            object result = command.ExecuteScalar();

                            if ((result != null) && (result.GetType() != typeof(DBNull)))
                                lItemId = (int)result;
                        }
                    }
                }
                else
                {
                    using (SqlConnection conn = DatabaseConnection.CreateOpenStandardConnection())
                    {
                        using (SqlCommand command = new SqlCommand(commandText, conn))
                        {
                            object result = command.ExecuteScalar();

                            if ((result != null) && (result.GetType() != typeof(DBNull)))
                                lItemId = (int)result;
                        }
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
            return lItemId;
        }

        public DataTable GetLocationPickerValues()
        {
            string commandText =
                "SELECT DISTINCT l._fullname " +
                "FROM LOCATIONS l " +
                "INNER JOIN ASSETS a on (a._locationid = l._locationid)";

            return PerformQuery(commandText);
        }

        public DataTable GetCompliantLocationValues(string assetValue)
        {
            string commandText = String.Format(
                "SELECT a._assetid " +
                "FROM LOCATIONS l " +
                "INNER JOIN ASSETS a on (a._locationid = l._locationid) " +
                "WHERE l._fullname = {0}", assetValue);

            return PerformQuery(commandText);
        }

        private DataTable PerformQuery(string aCommandText)
        {
            if (isDebugEnabled) logger.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " in");

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


            if (isDebugEnabled) logger.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " out");
            return statisticsTable;
        }

        private string PerformScalarQuery(string commandText)
        {
            string returnValue = String.Empty;

            try
            {
                if (compactDatabaseType)
                {
                    using (SqlCeConnection conn = DatabaseConnection.CreateOpenCEConnection())
                    {
                        using (SqlCeCommand command = new SqlCeCommand(commandText, conn))
                        {
                            returnValue = command.ExecuteScalar().ToString();
                        }
                    }
                }
                else
                {
                    using (SqlConnection conn = DatabaseConnection.CreateOpenStandardConnection())
                    {
                        using (SqlCommand command = new SqlCommand(commandText, conn))
                        {
                            returnValue = command.ExecuteScalar().ToString();
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

        #endregion Locations Table
    }
}
