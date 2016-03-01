using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlServerCe;
using System.IO;
using System.Windows.Forms;


namespace Layton.AuditWizard.DataAccess
{
    public class ApplicationsDAO
    {
        #region Data

        protected static readonly log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private static readonly bool isDebugEnabled = logger.IsDebugEnabled;
        private bool isCompactDatabase = false;
        private Configuration config = ConfigurationManager.OpenExeConfiguration(Path.Combine(Application.StartupPath, "AuditWizardv8.exe"));

        #endregion

        public ApplicationsDAO()
        {
            isCompactDatabase = Convert.ToBoolean(config.AppSettings.Settings["CompactDatabaseType"].Value);
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

        #region Applications tables

        /// <summary>
        /// Add a new application to the database
        /// </summary>
        /// <param name="aApplication"></param>
        /// <returns></returns>
        public int ApplicationAdd(InstalledApplication aApplication)
        {
            if (isDebugEnabled) logger.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " in");

            int lApplicationID = 0;
            string lOriginalPublisherName = aApplication.Publisher;
            aApplication.Publisher = RationalisePublisherNames(aApplication.Publisher);

            try
            {
                if (isCompactDatabase)
                {
                    using (SqlCeConnection conn = DatabaseConnection.CreateOpenCEConnection())
                    {
                        // first check if there is a match for this application publisher, name and version
                        string commandText =
                            "SELECT min(_APPLICATIONID) FROM APPLICATIONS " +
                            "WHERE _NAME = @cApplication AND _PUBLISHER = @cPublisher AND _VERSION = @cVersion";

                        using (SqlCeCommand commandReturnValue = new SqlCeCommand(commandText, conn))
                        {
                            commandReturnValue.Parameters.AddWithValue("@cApplication", aApplication.Name);
                            commandReturnValue.Parameters.AddWithValue("@cPublisher", aApplication.Publisher);
                            commandReturnValue.Parameters.AddWithValue("@cVersion", aApplication.Version);

                            object result = commandReturnValue.ExecuteScalar();

                            if ((result != null) && (result.GetType() != typeof(DBNull)))
                                return Convert.ToInt32(result);
                        }

                        // no match so need to insert a new record
                        commandText =
                            "INSERT INTO APPLICATIONS " +
                            "(_NAME ,_VERSION ,_PUBLISHER ,_GUID ,_ISOS ,_ALIASED_TOID ,_USER_DEFINED) " +
                            "VALUES (@cApplication, @cVersion, @cPublisher ,@cGuid ,0 ,@nAliasedToID ,@bUserDefined)";

                        SqlCeParameter[] spParams = new SqlCeParameter[6];

                        spParams[0] = new SqlCeParameter("@cPublisher", aApplication.Publisher);
                        spParams[1] = new SqlCeParameter("@cApplication", aApplication.Name);
                        spParams[2] = new SqlCeParameter("@cVersion", aApplication.Version);
                        spParams[3] = new SqlCeParameter("@cGuid", String.Empty);
                        spParams[4] = new SqlCeParameter("@nAliasedToID", aApplication.AliasedToID);
                        spParams[5] = new SqlCeParameter("@bUserDefined", Convert.ToInt32(aApplication.UserDefined));

                        using (SqlCeCommand command = new SqlCeCommand(commandText, conn))
                        {
                            command.Parameters.AddRange(spParams);
                            command.ExecuteNonQuery();
                        }

                        // then get id of newly inserted record
                        using (SqlCeCommand command = new SqlCeCommand("SELECT @@IDENTITY", conn))
                        {
                            lApplicationID = Convert.ToInt32(command.ExecuteScalar());
                        }

                        // if lOriginalPublisherName != aApplication.Publisher it means we have encountered an aliased publisher
                        // in this case we need to enter a record into the link table to allow us to revert the publisher
                        // if the user chooses to undo the alias at a later date
                        if (lOriginalPublisherName != aApplication.Publisher)
                        {
                            new PublisherAliasAppDAO().InsertNewApplication(lApplicationID, lOriginalPublisherName);
                        }
                    }
                }
                else
                {
                    using (SqlConnection conn = DatabaseConnection.CreateOpenStandardConnection())
                    {
                        // first check if there is a match for this application publisher, name and version
                        string commandText =
                            "SELECT min(_APPLICATIONID) FROM APPLICATIONS " +
                            "WHERE _NAME = @cApplication AND _PUBLISHER = @cPublisher AND _VERSION = @cVersion";

                        using (SqlCommand commandReturnValue = new SqlCommand(commandText, conn))
                        {
                            commandReturnValue.Parameters.AddWithValue("@cApplication", aApplication.Name);
                            commandReturnValue.Parameters.AddWithValue("@cPublisher", aApplication.Publisher);
                            commandReturnValue.Parameters.AddWithValue("@cVersion", aApplication.Version);

                            object result = commandReturnValue.ExecuteScalar();

                            if ((result != null) && (result.GetType() != typeof(DBNull)))
                                return Convert.ToInt32(result);
                        }

                        // no match so need to insert a new record
                        commandText =
                            "INSERT INTO APPLICATIONS " +
                            "(_NAME ,_VERSION ,_PUBLISHER ,_GUID ,_ISOS ,_ALIASED_TOID ,_USER_DEFINED) " +
                            "VALUES (@cApplication, @cVersion, @cPublisher ,@cGuid ,0 ,@nAliasedToID ,@bUserDefined)";

                        SqlParameter[] spParams = new SqlParameter[6];

                        spParams[0] = new SqlParameter("@cPublisher", aApplication.Publisher);
                        spParams[1] = new SqlParameter("@cApplication", aApplication.Name);
                        spParams[2] = new SqlParameter("@cVersion", aApplication.Version);
                        spParams[3] = new SqlParameter("@cGuid", String.Empty);
                        spParams[4] = new SqlParameter("@nAliasedToID", aApplication.AliasedToID);
                        spParams[5] = new SqlParameter("@bUserDefined", Convert.ToInt32(aApplication.UserDefined));

                        using (SqlCommand command = new SqlCommand(commandText, conn))
                        {
                            command.Parameters.AddRange(spParams);
                            command.ExecuteNonQuery();
                        }

                        // then get id of newly inserted record
                        using (SqlCommand command = new SqlCommand("SELECT @@IDENTITY", conn))
                        {
                            lApplicationID = Convert.ToInt32(command.ExecuteScalar());
                        }

                        // if lOriginalPublisherName != aApplication.Publisher it means we have encountered an aliased publisher
                        // in this case we need to enter a record into the link table to allow us to revert the publisher
                        // if the user chooses to undo the alias at a later date
                        if (lOriginalPublisherName != aApplication.Publisher)
                        {
                            new PublisherAliasAppDAO().InsertNewApplication(lApplicationID, lOriginalPublisherName);
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


            if (isDebugEnabled) logger.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " out with id of : " + lApplicationID);
            return lApplicationID;
        }


        /// <summary>
        /// Add a new application instance to the database
        /// </summary>
        /// <returns></returns>
        public void ApplicationAddInstance(int aAssetID, ApplicationInstance aApplication, out int applicationID, out int instanceID)
        {
            if (isDebugEnabled) logger.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " in");

            applicationID = 0;
            instanceID = 0;

            string lOriginalPublisherName = aApplication.Publisher;
            aApplication.Publisher = RationalisePublisherNames(aApplication.Publisher);
            object result;

            try
            {
                int lReturnedApplicationID = -1;
                if (isCompactDatabase)
                {
                    using (SqlCeConnection conn = DatabaseConnection.CreateOpenCEConnection())
                    {
                        string commandText =
                            "SELECT min(_APPLICATIONID) " +
                            "FROM APPLICATIONS " +
                            "WHERE _NAME = @cName " +
                            "AND _VERSION = @cVersion";

                        using (SqlCeCommand command = new SqlCeCommand(commandText, conn))
                        {
                            command.Parameters.AddWithValue("@cName", aApplication.Name);
                            command.Parameters.AddWithValue("@cVersion", aApplication.Version);
                            result = command.ExecuteScalar();
                        }

                        if ((result != null) && (result.GetType() != typeof(DBNull)))
                        {
                            lReturnedApplicationID = Convert.ToInt32(result);
                        }
                        else
                        {
                            // inserting a new record into the APPLICATIONS table
                            commandText =
                                "INSERT INTO APPLICATIONS " +
                                "(_NAME ,_VERSION ,_PUBLISHER ,_GUID ,_ISOS ,_ALIASED_TOID ,_USER_DEFINED) " +
                                "VALUES (@cApplication, @cVersion, @cPublisher, @cGuid, 0, 0, 0)";

                            SqlCeParameter[] spParams = new SqlCeParameter[4];

                            spParams[0] = new SqlCeParameter("@cPublisher", aApplication.Publisher);
                            spParams[1] = new SqlCeParameter("@cApplication", aApplication.Name);
                            spParams[2] = new SqlCeParameter("@cVersion", aApplication.Version);
                            spParams[3] = new SqlCeParameter("@cGuid", aApplication.Guid);

                            using (SqlCeCommand insertCommand = new SqlCeCommand(commandText, conn))
                            {
                                insertCommand.Parameters.AddRange(spParams);
                                insertCommand.ExecuteNonQuery();
                            }

                            // then get id of newly inserted record
                            using (SqlCeCommand commandReturnValue = new SqlCeCommand("SELECT @@IDENTITY", conn))
                            {
                                lReturnedApplicationID = Convert.ToInt32(commandReturnValue.ExecuteScalar());
                            }

                            // if lOriginalPublisherName != aApplication.Publisher it means we have encountered an aliased publisher
                            // in this case we need to enter a record into the link table to allow us to revert the publisher
                            // if the user chooses to undo the alias at a later date
                            if (lOriginalPublisherName != aApplication.Publisher)
                            {
                                new PublisherAliasAppDAO().InsertNewApplication(lReturnedApplicationID, lOriginalPublisherName);
                            }
                        }

                        applicationID = lReturnedApplicationID;
                        instanceID =
                            AddApplicationInstanceLocal(aAssetID, aApplication.Serial.ProductId, aApplication.Serial.CdKey, conn, lReturnedApplicationID);

                    }
                }
                else
                {
                    using (SqlConnection conn = DatabaseConnection.CreateOpenStandardConnection())
                    {
                        string commandText =
                            "SELECT min(_APPLICATIONID) " +
                            "FROM APPLICATIONS " +
                            "WHERE _NAME = @cName " +
                            "AND _VERSION = @cVersion";

                        using (SqlCommand command = new SqlCommand(commandText, conn))
                        {
                            command.Parameters.AddWithValue("@cName", aApplication.Name);
                            command.Parameters.AddWithValue("@cVersion", aApplication.Version);
                            result = command.ExecuteScalar();
                        }

                        if ((result != null) && (result.GetType() != typeof(DBNull)))
                        {
                            lReturnedApplicationID = Convert.ToInt32(result);
                        }
                        else
                        {
                            // the following code is needed to fix a legacy issue:
                            // in versions > 8.1.0, the publisher names were reconciled to ensure no close duplicates existed.
                            // it is possible that an application exists in the table from before this time and have the old publisher name:
                            // i.e. < 8.1.0 would insert Microsoft Corporation, > 8.1.0 would change this to Microsoft Corporation, Inc
                            // so, if we don't find (e.g.) Application A, publisher Microsoft Corporation, Inc it could mean that it actually
                            // doesn't exist *or* it exists but as Application A, publisher Microsoft Corporation
                            // so, re-run the query using the original publisher name to see if that gives us a match

                            commandText = String.Format(
                            "SELECT min(_APPLICATIONID) " +
                            "FROM APPLICATIONS " +
                            "WHERE _NAME = '{0}' " +
                            "AND _PUBLISHER = '{1}' " +
                            "AND _VERSION = '{2}'", aApplication.Name, lOriginalPublisherName, aApplication.Version);

                            using (SqlCommand command = new SqlCommand(commandText, conn))
                            {
                                result = command.ExecuteScalar();
                            }

                            if ((result != null) && (result.GetType() != typeof(DBNull)))
                            {
                                lReturnedApplicationID = Convert.ToInt32(result);
                            }
                            else
                            {
                                commandText =
                                "INSERT INTO APPLICATIONS " +
                                "(_NAME ,_VERSION ,_PUBLISHER ,_GUID ,_ISOS ,_ALIASED_TOID ,_USER_DEFINED) " +
                                "VALUES (@cApplication, @cVersion, @cPublisher, @cGuid, 0, 0, 0)";

                                SqlParameter[] spParams = new SqlParameter[4];

                                spParams[0] = new SqlParameter("@cPublisher", aApplication.Publisher);
                                spParams[1] = new SqlParameter("@cApplication", aApplication.Name);
                                spParams[2] = new SqlParameter("@cVersion", aApplication.Version);
                                spParams[3] = new SqlParameter("@cGuid", aApplication.Guid);

                                using (SqlCommand insertCommand = new SqlCommand(commandText, conn))
                                {
                                    insertCommand.Parameters.AddRange(spParams);
                                    insertCommand.ExecuteNonQuery();
                                }

                                // then get id of newly inserted record
                                using (SqlCommand commandReturnValue = new SqlCommand("SELECT @@IDENTITY", conn))
                                {
                                    lReturnedApplicationID = Convert.ToInt32(commandReturnValue.ExecuteScalar());
                                }
                            }
                        }

                        applicationID = lReturnedApplicationID;
                        instanceID =
                            AddApplicationInstanceLocal(aAssetID, aApplication.Serial.ProductId, aApplication.Serial.CdKey, conn, lReturnedApplicationID);

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


            if (isDebugEnabled) logger.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " out with applicationID of : " + applicationID + " and instance id of : " + instanceID);
        }

        public string RationalisePublisherNames(string aPublisherName)
        {
            string lPublisherName = new PublisherAliasDAO().GetAliasedPublisherName(aPublisherName);

            if (lPublisherName == null)
                return aPublisherName;
            else
                return lPublisherName;
        }

        public int AddApplicationInstanceLocal(int aAssetID, string aProductID, string aCDKey, SqlConnection conn, int lReturnedApplicationID)
        {
            int lExistingApplicationInstance = -1;

            try
            {
                string commandText =
                        "SELECT _INSTANCEID FROM APPLICATION_INSTANCES " +
                        "WHERE _ASSETID = @nAssetID AND _APPLICATIONID = @nApplicationID";

                using (SqlCommand commandReturnValue = new SqlCommand(commandText, conn))
                {
                    commandReturnValue.Parameters.AddWithValue("@nApplicationID", lReturnedApplicationID);
                    commandReturnValue.Parameters.AddWithValue("@nAssetID", aAssetID);

                    object result = commandReturnValue.ExecuteScalar();

                    if ((result != null) && (result.GetType() != typeof(DBNull)))
                        lExistingApplicationInstance = Convert.ToInt32(commandReturnValue.ExecuteScalar());
                }

                // if we didn't find a match then we need to insert
                if (lExistingApplicationInstance == -1)
                {
                    int lBaseApplicationId = 0;
                    int aliasedToId = 0;

                    // need to check first for pre-aliased applications
                    // if this application has already been aliased to another, we need to get the aliased application id
                    // and assign it to this lReturnedApplicationID. The current lReturnedApplicationID becomes the base application id.
                    commandText =
                        String.Format(
                        "SELECT _ALIASED_TOID " +
                        "FROM APPLICATIONS " +
                        "WHERE _APPLICATIONID = {0}", lReturnedApplicationID);

                    using (SqlCommand command = new SqlCommand(commandText, conn))
                    {
                        aliasedToId = (int)command.ExecuteScalar();
                    }

                    // if we found an aliased id, we need to assign it as the current application id.
                    // the original application id is stored in the base application id
                    if (aliasedToId != 0)
                    {
                        lBaseApplicationId = lReturnedApplicationID;
                        lReturnedApplicationID = aliasedToId;
                    }

                    commandText =
                        "INSERT INTO APPLICATION_INSTANCES (_APPLICATIONID, _BASE_APPLICATIONID, _ASSETID, _PRODUCTID, _CDKEY) " +
                        "VALUES (@nApplicationID, @nBaseApplicationID, @nAssetID, @cSqlProductID, @cSqlCDKey)";

                    SqlParameter[] spParams = new SqlParameter[5];
                    spParams[0] = new SqlParameter("@nApplicationID", lReturnedApplicationID);
                    spParams[1] = new SqlParameter("@nBaseApplicationID", lBaseApplicationId);
                    spParams[2] = new SqlParameter("@nAssetID", aAssetID);
                    spParams[3] = new SqlParameter("@cSqlProductID", aProductID);
                    spParams[4] = new SqlParameter("@cSqlCDKey", aCDKey);

                    using (SqlCommand command = new SqlCommand(commandText, conn))
                    {
                        command.Parameters.AddRange(spParams);
                        command.ExecuteNonQuery();
                    }

                    using (SqlCommand commandReturnValue = new SqlCommand("SELECT @@IDENTITY", conn))
                    {
                        lExistingApplicationInstance = Convert.ToInt32(commandReturnValue.ExecuteScalar());
                    }
                }
                else
                {
                    string updateCommandText = String.Empty;

                    if (aProductID != String.Empty && aCDKey != String.Empty)
                    {
                        // both productID and CDKey
                        updateCommandText =
                            "UPDATE APPLICATION_INSTANCES SET " +
                            "_PRODUCTID = '" + aProductID +
                            "', _CDKEY = '" + aCDKey + "'";
                    }

                    else if (aProductID != String.Empty && aCDKey == String.Empty)
                    {
                        // only productID
                        updateCommandText =
                            "UPDATE APPLICATION_INSTANCES SET " +
                            "_PRODUCTID = '" + aProductID + "'";
                    }

                    else if (aProductID == String.Empty && aCDKey != String.Empty)
                    {
                        // only CDKey
                        updateCommandText =
                            "UPDATE APPLICATION_INSTANCES SET " +
                            "', _CDKEY = '" + aCDKey + "'";
                    }

                    if (updateCommandText != String.Empty)
                    {
                        updateCommandText += " WHERE _INSTANCEID = '" + lExistingApplicationInstance + "'";

                        using (SqlCommand command = new SqlCommand(updateCommandText, conn))
                        {
                            command.ExecuteNonQuery();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error("Error in AddApplicationInstanceLocal()", ex);
                throw ex;
            }

            return lExistingApplicationInstance;
        }

        public int AddApplicationInstanceLocal(int aAssetID, string aProductID, string aCDKey, SqlCeConnection conn, int lReturnedApplicationID)
        {
            int lExistingApplicationInstance = -1;

            try
            {
                string commandText =
                        "SELECT _INSTANCEID FROM APPLICATION_INSTANCES " +
                        "WHERE _ASSETID = @nAssetID AND _APPLICATIONID = @nApplicationID";

                using (SqlCeCommand commandReturnValue = new SqlCeCommand(commandText, conn))
                {
                    commandReturnValue.Parameters.AddWithValue("@nApplicationID", lReturnedApplicationID);
                    commandReturnValue.Parameters.AddWithValue("@nAssetID", aAssetID);
                    object result = commandReturnValue.ExecuteScalar();

                    if ((result != null) && (result.GetType() != typeof(DBNull)))
                        lExistingApplicationInstance = Convert.ToInt32(commandReturnValue.ExecuteScalar());
                }

                // if we didn't find a match then we need to insert
                if (lExistingApplicationInstance == -1)
                {
                    int lBaseApplicationId = 0;
                    int aliasedToId = 0;

                    // need to check first for pre-aliased applications
                    // if this application has already been aliased to another, we need to get the aliased application id
                    // and assign it to this lReturnedApplicationID. The current lReturnedApplicationID becomes the base application id.
                    commandText =
                        String.Format(
                        "SELECT _ALIASED_TOID " +
                        "FROM APPLICATIONS " +
                        "WHERE _APPLICATIONID = {0}", lReturnedApplicationID);

                    using (SqlCeCommand command = new SqlCeCommand(commandText, conn))
                    {
                        aliasedToId = (int)command.ExecuteScalar();
                    }

                    // if we found an aliased id, we need to assign it as the current application id.
                    // the original application id is stored in the base application id
                    if (aliasedToId != 0)
                    {
                        lBaseApplicationId = lReturnedApplicationID;
                        lReturnedApplicationID = aliasedToId;
                    }

                    commandText =
                        "INSERT INTO APPLICATION_INSTANCES (_APPLICATIONID, _BASE_APPLICATIONID, _ASSETID, _PRODUCTID, _CDKEY) " +
                        "VALUES (@nApplicationID, @nBaseApplicationID, @nAssetID, @cSqlProductID, @cSqlCDKey)";

                    SqlCeParameter[] spParams = new SqlCeParameter[5];
                    spParams[0] = new SqlCeParameter("@nApplicationID", lReturnedApplicationID);
                    spParams[1] = new SqlCeParameter("@nBaseApplicationID", lBaseApplicationId);
                    spParams[2] = new SqlCeParameter("@nAssetID", aAssetID);
                    spParams[3] = new SqlCeParameter("@cSqlProductID", aProductID);
                    spParams[4] = new SqlCeParameter("@cSqlCDKey", aCDKey);

                    using (SqlCeCommand command = new SqlCeCommand(commandText, conn))
                    {
                        command.Parameters.AddRange(spParams);
                        command.ExecuteNonQuery();
                    }

                    using (SqlCeCommand commandReturnValue = new SqlCeCommand("SELECT @@IDENTITY", conn))
                    {
                        lExistingApplicationInstance = Convert.ToInt32(commandReturnValue.ExecuteScalar());
                    }
                }
                else
                {
                    string updateCommandText = String.Empty;

                    if (aProductID != String.Empty && aCDKey != String.Empty)
                    {
                        // both productID and CDKey
                        updateCommandText =
                            "UPDATE APPLICATION_INSTANCES SET " +
                            "_PRODUCTID = '" + aProductID +
                            "', _CDKEY = '" + aCDKey + "'";
                    }

                    else if (aProductID != String.Empty && aCDKey == String.Empty)
                    {
                        // only productID
                        updateCommandText =
                            "UPDATE APPLICATION_INSTANCES SET " +
                            "_PRODUCTID = '" + aProductID + "'";
                    }

                    else if (aProductID == String.Empty && aCDKey != String.Empty)
                    {
                        // only CDKey
                        updateCommandText =
                            "UPDATE APPLICATION_INSTANCES SET " +
                            "', _CDKEY = '" + aCDKey + "'";
                    }

                    if (updateCommandText != String.Empty)
                    {
                        updateCommandText += " WHERE _INSTANCEID = '" + lExistingApplicationInstance + "'";

                        using (SqlCeCommand command = new SqlCeCommand(updateCommandText, conn))
                        {
                            command.ExecuteNonQuery();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error("Error in AddApplicationInstanceLocal()", ex);
                throw ex;
            }

            return lExistingApplicationInstance;
        }

        /// <summary>
        /// Add a new application instance to the database
        /// </summary>
        /// <param name="assetID"></param>
        /// <param name="theApplication"></param>
        /// <returns></returns>
        public int OSAddInstance(int aAssetID, OSInstance aOS, out int applicationID, out int instanceID)
        {
            if (isDebugEnabled) logger.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " in");

            if (isCompactDatabase)
            {
                applicationID = 0;
                instanceID = 0;

                try
                {

                    int lReturnedOSID = -1;

                    // see if this OS exists
                    string commandText =
                                "SELECT min(_APPLICATIONID) FROM APPLICATIONS WHERE _NAME = @cName AND _VERSION = @cVersion AND _ISOS = 1";

                    using (SqlCeConnection conn = DatabaseConnection.CreateOpenCEConnection())
                    {
                        using (SqlCeCommand command = new SqlCeCommand(commandText, conn))
                        {
                            command.Parameters.AddWithValue("@cName", aOS.Name);
                            command.Parameters.AddWithValue("@cVersion", aOS.Version);
                            object result = command.ExecuteScalar();

                            if (result.GetType() == typeof(DBNull))
                            {
                                // haven't found a result so insert a new one
                                aOS.Name = ReplaceStringQuotes(aOS.Name);
                                aOS.Version = ReplaceStringQuotes(aOS.Version);

                                commandText =
                                    "INSERT INTO APPLICATIONS (_NAME ,_VERSION ,_PUBLISHER ,_GUID ,_ISOS) " +
                                    "VALUES (@Name, @Version, @Publisher, @Guid, @IsOS)";

                                SqlCeParameter[] spParams = new SqlCeParameter[5];
                                spParams[0] = new SqlCeParameter("@Name", aOS.Name);
                                spParams[1] = new SqlCeParameter("@Version", aOS.Version);
                                spParams[2] = new SqlCeParameter("@Publisher", "Microsoft Corporation, Inc.");
                                spParams[3] = new SqlCeParameter("@Guid", String.Empty);
                                spParams[4] = new SqlCeParameter("@IsOS", 1);

                                using (SqlCeCommand command1 = new SqlCeCommand(commandText, conn))
                                {
                                    command1.Parameters.AddRange(spParams);
                                    command1.ExecuteNonQuery();
                                }

                                // then get id of newly inserted record
                                using (SqlCeCommand commandReturnValue = new SqlCeCommand("SELECT @@IDENTITY", conn))
                                {
                                    lReturnedOSID = Convert.ToInt32(commandReturnValue.ExecuteScalar());
                                }
                            }
                            else
                            {
                                lReturnedOSID = Convert.ToInt32(result);
                            }

                            applicationID = lReturnedOSID;
                            instanceID = AddApplicationInstanceLocal(aAssetID, aOS.Serial.ProductId, aOS.Serial.CdKey, conn, lReturnedOSID);
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
                lAuditWizardDataAccess.OSAddInstance(aAssetID, aOS, out applicationID, out instanceID);
            }

            if (isDebugEnabled) logger.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " out with applicationID of : " + applicationID + " and instance id of : " + instanceID);
            return 0;
        }



        /// <summary>
        /// Flag an application as 'Ignored' in the database or clear this flag
        /// </summary>
        /// <param name="applicationID"></param>
        /// <param name="hide"></param>
        /// <returns></returns>
        public int ApplicationSetIgnored(int aApplicationID, bool aIgnore)
        {
            if (isDebugEnabled) logger.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " in.");

            if (isCompactDatabase)
            {
                try
                {
                    string commandText =
                        "UPDATE APPLICATIONS SET _IGNORED = @bIgnore WHERE _APPLICATIONID = @nApplicationID";

                    SqlCeParameter[] spParams = new SqlCeParameter[2];
                    spParams[0] = new SqlCeParameter("@nApplicationID", aApplicationID);
                    spParams[1] = new SqlCeParameter("@bIgnore", Convert.ToInt32(aIgnore));

                    using (SqlCeCommand command = new SqlCeCommand(commandText, DatabaseConnection.OpenCeConnection()))
                    {
                        command.Parameters.AddRange(spParams);
                        int rowsUpdated = command.ExecuteNonQuery();
                        if (isDebugEnabled) logger.Debug("updated " + rowsUpdated + " row(s)");
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
                lAuditWizardDataAccess.ApplicationSetIgnored(aApplicationID, aIgnore);
            }

            if (isDebugEnabled) logger.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " out.");
            return 0;
        }


        /// <summary>
        /// Return a count of the number of application instances in the database
        /// </summary>
        /// <returns></returns>
        public int ApplicationInstanceCount()
        {
            if (isDebugEnabled) logger.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " in.");

            int lCount = 0;

            if (isCompactDatabase)
            {
                using (SqlCeConnection conn = DatabaseConnection.CreateOpenCEConnection())
                {
                    try
                    {
                        string commandText =
                            "SELECT COUNT(*) FROM APPLICATION_INSTANCES " +
                            "LEFT JOIN APPLICATIONS ON (APPLICATIONS._APPLICATIONID = APPLICATION_INSTANCES._APPLICATIONID) " +
                            "WHERE APPLICATIONS._IGNORED = 1 " +
                            "AND APPLICATIONS._ISOS = 0";

                        using (SqlCeCommand command = new SqlCeCommand(commandText, conn))
                        {
                            lCount = Convert.ToInt32(command.ExecuteScalar());
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
                lCount = lAuditWizardDataAccess.ApplicationInstanceCount();
            }

            if (isDebugEnabled) logger.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " out with count : " + lCount);
            return lCount;
        }

        /// <summary>
        /// Return a count of the number of application instances in the database
        /// </summary>
        /// <returns></returns>
        public int ApplicationCount()
        {
            if (isDebugEnabled) logger.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " in.");

            int lCount = 0;

            if (isCompactDatabase)
            {
                using (SqlCeConnection conn = DatabaseConnection.CreateOpenCEConnection())
                {
                    try
                    {
                        string commandText =
                            "SELECT COUNT(*) FROM APPLICATIONS " +
                            "WHERE _ISOS = 0 AND _ALIASED_TOID = 0";

                        using (SqlCeCommand command = new SqlCeCommand(commandText, conn))
                        {
                            lCount = Convert.ToInt32(command.ExecuteScalar());
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
                //AuditWizardDataAccess lAuditWizardDataAccess = new AuditWizardDataAccess();
                //lCount = lAuditWizardDataAccess.ApplicationInstanceCount();
            }

            if (isDebugEnabled) logger.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " out with count : " + lCount);
            return lCount;
        }

        /// <summary>
        /// Return a table containing all of the Alerts have been triggered since the specified date
        /// </summary>
        /// <returns></returns>
        public List<string> EnumerateApplicationNames()
        {
            if (isDebugEnabled) logger.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " in");

            List<string> lApplicationNames = new List<string>();

            if (isCompactDatabase)
            {
                try
                {
                    using (SqlCeConnection conn = DatabaseConnection.CreateOpenCEConnection())
                    {
                        string commandText =
                            "SELECT _NAME FROM APPLICATIONS " +
                            "WHERE _ISOS = 0 AND _ALIASED_TOID = 0";

                        using (SqlCeCommand command = new SqlCeCommand(commandText, conn))
                        {
                            using (SqlCeDataReader reader = command.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    lApplicationNames.Add(reader.GetString(0));
                                }
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
                //AuditWizardDataAccess lAuditWizardDataAccess = new AuditWizardDataAccess();
                //alertsTable = lAuditWizardDataAccess.EnumerateAlerts(aSinceDate);
            }

            if (isDebugEnabled) logger.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " out");
            return lApplicationNames;
        }

        public DataTable GetApplications(string aForPublishers, bool aIncludeIncluded, bool aIncludeIgnored)
        {
            return GetApplications(aForPublishers, aIncludeIncluded, aIncludeIgnored, false);
        }

        public DataTable GetApplications(string aForPublishers, bool aIncludeIncluded, bool aIncludeIgnored, bool aShowOS)
        {
            if (isDebugEnabled) logger.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " in");

            DataTable applicationsTable = new DataTable(TableNames.APPLICATIONS);

            try
            {
                string whereClause = String.Empty;

                string sqlPublisherFilter = string.IsNullOrEmpty(aForPublishers) ? "%" : aForPublishers;

                foreach (string publisher in sqlPublisherFilter.Split(';'))
                {
                    if (whereClause == String.Empty)
                        whereClause += "_PUBLISHER LIKE ('" + publisher + "')";
                    else
                        whereClause += " OR _PUBLISHER LIKE ('" + publisher + "')";
                }

                string commandText = String.Format(
                    "SELECT APPLICATIONS.* FROM APPLICATIONS " +
                    "WHERE ({0}) ", whereClause);

                if (!aShowOS)
                    commandText += "AND _ISOS = 0 ";

                commandText += String.Format
                    ("AND _ALIASED_TOID = 0 AND (_IGNORED = {0} OR _IGNORED = {1}) " +
                    "ORDER BY APPLICATIONS._PUBLISHER, APPLICATIONS._NAME", Convert.ToInt16(!aIncludeIncluded), Convert.ToInt16(aIncludeIgnored));

                if (isCompactDatabase)
                {
                    using (SqlCeConnection conn = DatabaseConnection.CreateOpenCEConnection())
                    {
                        SqlCeCommand command = new SqlCeCommand(commandText, conn);
                        new SqlCeDataAdapter(command).Fill(applicationsTable);
                    }
                }
                else
                {
                    using (SqlConnection conn = DatabaseConnection.CreateOpenStandardConnection())
                    {
                        SqlCommand command = new SqlCommand(commandText, conn);
                        new SqlDataAdapter(command).Fill(applicationsTable);
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
            return applicationsTable;
        }

        public DataTable GetApplicationIdsByPublisherName(string aPublisher)
        {
            if (isDebugEnabled) logger.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " in");

            DataTable applicationsTable = new DataTable(TableNames.APPLICATIONS);

            try
            {
                string cmdText = String.Format("SELECT _APPLICATIONID FROM APPLICATIONS WHERE _PUBLISHER = '{0}'", aPublisher);

                if (isCompactDatabase)
                {
                    using (SqlCeConnection conn = DatabaseConnection.CreateOpenCEConnection())
                    {
                        SqlCeCommand command = new SqlCeCommand(cmdText, conn);
                        new SqlCeDataAdapter(command).Fill(applicationsTable);
                    }
                }
                else
                {
                    using (SqlConnection conn = DatabaseConnection.CreateOpenStandardConnection())
                    {
                        SqlCommand command = new SqlCommand(cmdText, conn);
                        new SqlDataAdapter(command).Fill(applicationsTable);
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
            return applicationsTable;
        }

        public DataTable GetApplicationsByPublisher(string aPublisher, bool aShowIncluded, bool aShowIgnored)
        {
            if (isDebugEnabled) logger.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " in");

            DataTable applicationsTable = new DataTable(TableNames.APPLICATIONS);

            try
            {
                string cmdText =
                       String.Format(
                       "SELECT _APPLICATIONID, _PUBLISHER, _NAME, _VERSION, _IGNORED, _ALIASED_TOID, _USER_DEFINED, _ASSIGNED_FILEID " +
                       "FROM APPLICATIONS " +
                       "WHERE _PUBLISHER = '{0}' " +
                       "AND _ISOS = 0 " +
                       "AND _ALIASED_TOID = 0 ", aPublisher);

                if (aShowIncluded && !aShowIgnored)
                {
                    cmdText += "AND _IGNORED = 0 ";
                }
                else if (!aShowIncluded && aShowIgnored)
                {
                    cmdText += "AND _IGNORED = 1 ";
                }

                cmdText += "ORDER BY APPLICATIONS._PUBLISHER, APPLICATIONS._NAME";

                if (isCompactDatabase)
                {
                    using (SqlCeConnection conn = DatabaseConnection.CreateOpenCEConnection())
                    {
                        SqlCeCommand command = new SqlCeCommand(cmdText, conn);
                        new SqlCeDataAdapter(command).Fill(applicationsTable);
                    }
                }
                else
                {
                    using (SqlConnection conn = DatabaseConnection.CreateOpenStandardConnection())
                    {
                        SqlCommand command = new SqlCommand(cmdText, conn);
                        new SqlDataAdapter(command).Fill(applicationsTable);
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
            return applicationsTable;
        }

        /// <summary>
        /// Return a list of all of the applications (for the specified publisher(s))
        /// </summary>
        /// <param name="forPublisher"></param>
        /// <param name="includeIgnored">false to not include application marked as Not-NotIgnore</param>
        /// <returns></returns>
        //public DataTable GetApplications(string aForPublishers, bool aIncludeIncluded, bool aIncludeIgnored)
        //{
        //    if (isDebugEnabled) logger.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " in");

        //    AuditWizardDataAccess lAuditWizardDataAccess = new AuditWizardDataAccess();
        //    DataTable applicationsTable = new DataTable(TableNames.APPLICATIONS);

        //    if (compactDatabaseType)
        //    {
        //        try
        //        {
        //            string commandText = String.Empty;
        //            String sqlPublisherFilter = null;

        //            if (aForPublishers == null || aForPublishers == "")
        //                sqlPublisherFilter = "";
        //            else
        //                sqlPublisherFilter = lAuditWizardDataAccess.BuildPublisherFilter(aForPublishers);

        //            commandText =
        //                "SELECT APPLICATIONS._APPLICATIONID, COUNT(APPLICATION_INSTANCES._ASSETID) " +
        //                "FROM APPLICATIONS, APPLICATION_INSTANCES, ASSETS " +
        //                "WHERE APPLICATIONS._APPLICATIONID = APPLICATION_INSTANCES._APPLICATIONID " +
        //                "AND APPLICATION_INSTANCES._ASSETID = ASSETS._ASSETID ";

        //            if (aForPublishers == null || aForPublishers == "")
        //                commandText += "AND _ISOS = 0 AND _ALIASED_TOID = 0 ";
        //            else
        //                commandText += "AND _PUBLISHER in (' + @forPublisher + ') AND _ISOS = 0 AND _ALIASED_TOID = 0 ";

        //            if (aIncludeIncluded && !aIncludeIgnored)
        //                commandText += "AND _IGNORED = 0 ";
        //            else if (!aIncludeIncluded && aIncludeIgnored)
        //                commandText += "AND _IGNORED = 1 ";

        //            commandText += "GROUP BY APPLICATIONS._APPLICATIONID";

        //            using (SqlCeConnection conn = DatabaseConnection.CreateOpenCEConnection())
        //            {
        //                SqlCeCommand cmd = new SqlCeCommand("APPLICATIONS", conn);
        //                cmd.CommandType = CommandType.TableDirect;
        //                new SqlCeDataAdapter(cmd).Fill(applicationsTable);
        //                applicationsTable.Columns.Add("INSTALLCOUNT", typeof(int));

        //                SqlCeCommand command = new SqlCeCommand(commandText, conn);

        //                if (aForPublishers != String.Empty)
        //                    command.Parameters.AddWithValue("@forPublisher", aForPublishers);

        //                using (SqlCeDataReader reader = command.ExecuteReader())
        //                {
        //                    while (reader.Read())
        //                    {
        //                        int lApplicationId = reader.GetInt32(0);
        //                        int lCount = reader.GetInt32(1);

        //                        DataRow[] customerRow = applicationsTable.Select("_APPLICATIONID = " + lApplicationId);
        //                        customerRow[0]["INSTALLCOUNT"] = lCount;
        //                    }
        //                }
        //            }

        //            // create a temp table to hold results
        //        }
        //        catch (SqlCeException ex)
        //        {
        //            Utility.DisplayErrorMessage("A database error has occurred in AuditWizard." + Environment.NewLine + Environment.NewLine +
        //                    "Please see the log file for further details.");
        //            logger.Error("Exception in " + System.Reflection.MethodBase.GetCurrentMethod().Name, ex);
        //        }
        //        catch (Exception ex)
        //        {
        //            Utility.DisplayErrorMessage("A database error has occurred in AuditWizard." + Environment.NewLine + Environment.NewLine +
        //                    "Please see the log file for further details.");

        //            logger.Error("Exception in " + System.Reflection.MethodBase.GetCurrentMethod().Name, ex);
        //        }
        //    }
        //    else
        //    {
        //        applicationsTable = lAuditWizardDataAccess.GetApplications(aForPublishers, aIncludeIncluded, aIncludeIgnored);
        //    }

        //    if (isDebugEnabled) logger.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " out");
        //    return applicationsTable;
        //}



        /// <summary>
        /// Return a list of all of the applications which have been aliased to another application
        /// </summary>
        /// <returns></returns>
        public DataTable GetAliasedApplications()
        {
            string cmdText =
                "SELECT A._PUBLISHER + ' | ' + A._NAME + ' | ' + A._VERSION as \"Original Application\", " +
                "B._PUBLISHER + ' | ' + B._NAME + ' | ' + B._VERSION as \"Aliased To Application\" " +
                "FROM APPLICATIONS A " +
                "LEFT JOIN APPLICATIONS B ON (A._ALIASED_TOID = B._APPLICATIONID) " +
                "WHERE A._ALIASED_TOID <> 0 " +
                "ORDER BY A._PUBLISHER, A._NAME";

            return PerformQuery(cmdText);
        }

        public DataTable GetAliasedApplicationsByApplicationId(int aApplicationId)
        {
            string cmdText = "SELECT _APPLICATIONID FROM APPLICATIONS WHERE _ALIASED_TOID = " + aApplicationId;
            return PerformQuery(cmdText);
        }

        public DataTable GetAliasedToApplications()
        {
            string cmdText = "SELECT DISTINCT _ALIASED_TOID FROM APPLICATIONS WHERE _ALIASED_TOID <> 0";
            return PerformQuery(cmdText);
        }

        /// <summary>
        /// Return a count of applications which have been aliased to the specified application
        /// </summary>
        /// <returns></returns>
        public int GetAliasCount(int applicationID)
        {
            if (isDebugEnabled) logger.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " in");

            int lReturnCount = 0;

            if (isCompactDatabase)
            {
                try
                {
                    using (SqlCeConnection conn = DatabaseConnection.CreateOpenCEConnection())
                    {
                        string commandText =
                            "SELECT COUNT(*) FROM APPLICATIONS WHERE _ALIASED_TOID = @applicationID";

                        using (SqlCeCommand command = new SqlCeCommand(commandText, conn))
                        {
                            command.Parameters.AddWithValue("@applicationID", applicationID);
                            lReturnCount = Convert.ToInt32(command.ExecuteScalar());
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
                lReturnCount = lAuditWizardDataAccess.GetAliasCount(applicationID);
            }

            if (isDebugEnabled) logger.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " out with count : " + lReturnCount);
            return lReturnCount;
        }

        public DataTable SelectApplicationIdByPublisherName(string aPublisherName)
        {
            if (isDebugEnabled) logger.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " in");

            string commandText = String.Format(
                "SELECT _APPLICATIONID " +
                "FROM APPLICATIONS  " +
                "WHERE _PUBLISHER = '{0}'", aPublisherName);

            if (isDebugEnabled) logger.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " out");

            return PerformQuery(commandText); ;
        }

        public DataTable GetApplicationNameAndVersionById(int aApplicationId)
        {
            if (isDebugEnabled) logger.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " in");

            string commandText = String.Format(
                "SELECT _NAME, _VERSION " +
                "FROM APPLICATIONS  " +
                "WHERE _APPLICATIONID = {0}", aApplicationId);

            if (isDebugEnabled) logger.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " out");

            return PerformQuery(commandText); ;
        }


        /// <summary>
        /// Return the application definition with the specified ID
        /// </summary>
        /// <returns></returns>
        public InstalledApplication GetApplication(int applicationID)
        {
            InstalledApplication lApplication = null;
            DataTable table = new DataTable(TableNames.APPLICATIONS);

            if (isCompactDatabase)
            {
                try
                {
                    using (SqlCeConnection conn = DatabaseConnection.CreateOpenCEConnection())
                    {
                        string commandText =
                            "SELECT APPLICATIONS.* " +
                            "FROM APPLICATIONS " +
                            "WHERE _APPLICATIONID = @nApplicationID";

                        using (SqlCeCommand command = new SqlCeCommand(commandText, conn))
                        {
                            command.Parameters.AddWithValue("@nApplicationID", applicationID);
                            new SqlCeDataAdapter(command).Fill(table);
                        }

                        // ...and use the first row returned to populate the user
                        if (table.Rows.Count != 0)
                            lApplication = new InstalledApplication(table.Rows[0]);
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
                lApplication = lAuditWizardDataAccess.GetApplication(applicationID);
            }

            if (isDebugEnabled) logger.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " out");
            return lApplication;
        }

        public DataTable GetOperatingSystemsLite()
        {
            string cmdText =
                "SELECT APPLICATIONS._APPLICATIONID ,APPLICATIONS._NAME ,APPLICATIONS._VERSION " +
                "FROM APPLICATIONS " +
                "WHERE _ISOS = 1 " +
                "ORDER BY APPLICATIONS._APPLICATIONID";

            return PerformQuery(cmdText);
        }

        /// <summary>
        /// Return a list of all of the Operating Systems
        /// </summary>
        /// <returns></returns>
        public DataTable GetOperatingSystems()
        {
            if (isDebugEnabled) logger.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " in");
            DataTable dataTable = new DataTable(TableNames.APPLICATIONS);

            if (isCompactDatabase)
            {

                try
                {
                    using (SqlCeConnection conn = DatabaseConnection.CreateOpenCEConnection())
                    {
                        string commandText =
                            "SELECT APPLICATIONS._APPLICATIONID ,APPLICATIONS._NAME ,APPLICATIONS._VERSION, INSTALLCOUNTS.installCount " +
                            "FROM APPLICATIONS " +
                            "LEFT OUTER JOIN " +
                                "(SELECT APPLICATIONS._APPLICATIONID ,COUNT(APPLICATIONS._APPLICATIONID) INSTALLCOUNT " +
                                    "FROM APPLICATIONS LEFT OUTER JOIN APPLICATION_INSTANCES S ON (APPLICATIONS._APPLICATIONID = S._APPLICATIONID) " +
                                    "WHERE _ISOS = 1 " +
                                    "GROUP BY APPLICATIONS._APPLICATIONID) INSTALLCOUNTS  " +
                            "ON APPLICATIONS._APPLICATIONID = INSTALLCOUNTS._APPLICATIONID " +
                            "WHERE _ISOS = 1 " +
                            "ORDER BY APPLICATIONS._APPLICATIONID";

                        using (SqlCeCommand command = new SqlCeCommand(commandText, conn))
                        {
                            new SqlCeDataAdapter(command).Fill(dataTable);
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
                dataTable = lAuditWizardDataAccess.GetOperatingSystems();
            }

            if (isDebugEnabled) logger.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " out");
            return dataTable;
        }

        /// <summary>
        /// Return a list of the applications that are installed on the specified asset optionally
        /// filtering this list so that only certain publishers are included
        /// </summary>
        /// <param name="forAsset"></param>
        /// <returns></returns>
        public DataTable GetInstalledOS(Asset aAsset)
        {
            if (isDebugEnabled) logger.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " in");

            DataTable applicationsTable = new DataTable(TableNames.APPLICATION_INSTANCES);

            if (isCompactDatabase)
            {

                try
                {
                    using (SqlCeConnection conn = DatabaseConnection.CreateOpenCEConnection())
                    {
                        string commandText =
                            "SELECT APPLICATION_INSTANCES._INSTANCEID ,APPLICATION_INSTANCES._APPLICATIONID ,APPLICATION_INSTANCES._PRODUCTID ,APPLICATION_INSTANCES._CDKEY " +
                            ",APPLICATIONS._NAME ,APPLICATIONS._VERSION " +
                            ",ASSETS._ASSETID ,ASSETS._NAME AS ASSETNAME " +
                            ",ASSET_TYPES._ICON AS ASSETICON " +
                            "FROM APPLICATION_INSTANCES " +
                            "LEFT JOIN APPLICATIONS ON (APPLICATION_INSTANCES._APPLICATIONID = APPLICATIONS._APPLICATIONID) " +
                            "LEFT JOIN ASSETS ON (APPLICATION_INSTANCES._ASSETID = ASSETS._ASSETID) " +
                            "LEFT JOIN ASSET_TYPES ON (ASSETS._ASSETTYPEID = ASSET_TYPES._ASSETTYPEID) " +
                            "WHERE APPLICATION_INSTANCES._ASSETID = @assetID AND APPLICATIONS._ISOS = 1";

                        using (SqlCeCommand command = new SqlCeCommand(commandText, conn))
                        {
                            command.Parameters.AddWithValue("@assetID", aAsset.AssetID);
                            new SqlCeDataAdapter(command).Fill(applicationsTable);
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
                applicationsTable = lAuditWizardDataAccess.GetInstalledOS(aAsset);
            }

            if (isDebugEnabled) logger.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " out");
            return applicationsTable;
        }

        /// <summary>
        /// Return a list of the applications that are installed on the specified asset optionally
        /// filtering this list so that only certain publishers are included
        /// </summary>
        /// <param name="aAsset"></param>
        /// <returns></returns>
        public DataTable GetInstalledApplications(Asset aAsset, string aPublisher, bool aShowIncluded, bool aShowIgnored)
        {
            if (isDebugEnabled) logger.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " in");
            AuditWizardDataAccess lAuditWizardDataAccess = new AuditWizardDataAccess();

            // ...then call the SP
            DataTable applicationsTable = new DataTable(TableNames.APPLICATION_INSTANCES);

            if (isCompactDatabase)
            {
                // Format any supplied publisher filter
                string sqlPublisherFilter = lAuditWizardDataAccess.BuildPublisherFilter(aPublisher);

                try
                {
                    using (SqlCeConnection conn = DatabaseConnection.CreateOpenCEConnection())
                    {
                        string commandText =
                            "SELECT APPLICATION_INSTANCES._INSTANCEID ,APPLICATION_INSTANCES._APPLICATIONID ,APPLICATION_INSTANCES._ASSETID ,APPLICATION_INSTANCES._PRODUCTID ,APPLICATION_INSTANCES._CDKEY " +
                            ",APPLICATION_INSTANCES._SUPPORT_ALERTEMAIL " +
                            ",APPLICATIONS._NAME ,APPLICATIONS._VERSION ,APPLICATIONS._PUBLISHER ,APPLICATIONS._GUID ,APPLICATIONS._ISOS ,APPLICATIONS._IGNORED ,APPLICATIONS._ALIASED_TOID ,APPLICATIONS._USER_DEFINED " +
                            ",ASSETS._NAME AS ASSETNAME " +
                            ",ASSET_TYPES._ICON AS ASSETICON " +
                            ",LOCATIONS._FULLNAME AS FULLLOCATIONNAME " +
                            "FROM APPLICATION_INSTANCES " +
                            "LEFT JOIN APPLICATIONS ON (APPLICATION_INSTANCES._APPLICATIONID = APPLICATIONS._APPLICATIONID) " +
                            "LEFT JOIN ASSETS ON (APPLICATION_INSTANCES._ASSETID = ASSETS._ASSETID) " +
                            "LEFT JOIN ASSET_TYPES ON (ASSETS._ASSETTYPEID=ASSET_TYPES._ASSETTYPEID) " +
                            "LEFT JOIN LOCATIONS ON (ASSETS._LOCATIONID = LOCATIONS._LOCATIONID) " +
                            "WHERE APPLICATION_INSTANCES._ASSETID = @assetID " +
                            "AND APPLICATIONS._ISOS = 0";

                        if (!aShowIncluded && aShowIgnored)
                            commandText += " AND APPLICATIONS._IGNORED = 1";

                        else if (aShowIncluded && !aShowIgnored)
                            commandText += " AND APPLICATIONS._IGNORED = 0";

                        if (sqlPublisherFilter != String.Empty)
                            commandText += " AND APPLICATIONS._PUBLISHER IN (" + sqlPublisherFilter + ")";

                        commandText += " ORDER BY APPLICATIONS._PUBLISHER, APPLICATIONS._NAME";

                        using (SqlCeCommand command = new SqlCeCommand(commandText, conn))
                        {
                            command.Parameters.AddWithValue("@assetID", aAsset.AssetID);
                            new SqlCeDataAdapter(command).Fill(applicationsTable);
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
                applicationsTable = lAuditWizardDataAccess.GetInstalledApplications(aAsset, aPublisher, aShowIncluded, aShowIgnored);
            }

            if (isDebugEnabled) logger.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " out");
            return applicationsTable;
        }

        public DataTable GetAllPublisherNamesAsDataTable(string aPublisherFilter)
        {
            return GetAllPublisherNamesAsDataTable(aPublisherFilter, true, false);
        }

        public int GetCountIncludedAppsForPublisher(string aPublisherName)
        {
            int lCount = 0;

            try
            {
                string cmdText =
                    "SELECT COUNT(_APPLICATIONID) " +
                    "FROM APPLICATIONS " +
                    "WHERE _PUBLISHER = @publisher " +
                    "AND _IGNORED = 0";

                if (isCompactDatabase)
                {
                    using (SqlCeConnection conn = DatabaseConnection.CreateOpenCEConnection())
                    {
                        using (SqlCeCommand command = new SqlCeCommand(cmdText, conn))
                        {
                            command.Parameters.AddWithValue("@publisher", aPublisherName);
                            lCount = Convert.ToInt32(command.ExecuteScalar());
                        }
                    }
                }
                else
                {
                    using (SqlConnection conn = DatabaseConnection.CreateOpenStandardConnection())
                    {
                        using (SqlCommand command = new SqlCommand(cmdText, conn))
                        {
                            command.Parameters.AddWithValue("@publisher", aPublisherName);
                            lCount = Convert.ToInt32(command.ExecuteScalar());
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

            return lCount;
        }

        public List<string> CheckApplicationIsAnAlias(List<int> applicationIds)
        {
            List<string> aliasedApps = new List<string>();

            foreach (int applicationId in applicationIds)
            {
                DataTable dt = GetAliasedApplicationsByApplicationId(applicationId);

                if (dt.Rows.Count > 0)
                {
                    DataTable aliasedAppDataTable = GetApplicationNameAndVersionById(applicationId);
                    string name = aliasedAppDataTable.Rows[0][0].ToString();
                    string version = aliasedAppDataTable.Rows[0][1].ToString();

                    if (version == "" || name.EndsWith(version))
                        aliasedApps.Add(name);
                    else
                        aliasedApps.Add(name + " (" + version + ")");
                }
            }

            return aliasedApps;
        }

        public void UpdatePublisherName(string originalPublisherName, string newPublisherName)
        {
            string cmdText = 
                String.Format("UPDATE APPLICATIONS SET _PUBLISHER = '{0}' WHERE _PUBLISHER = '{1}'", newPublisherName, originalPublisherName);
            
            ExecuteNonQuery(cmdText);
        }

        public void DeleteApplications(List<int> applicationIds)
        {
            foreach (int applicationId in applicationIds)
            {
                string cmdText = "DELETE FROM DOCUMENTS WHERE _SCOPE = 1 AND _PARENTID = " + applicationId;
                ExecuteNonQuery(cmdText);

                cmdText = "DELETE FROM DOCUMENTS WHERE _SCOPE = 2 AND _PARENTID IN " +
                          "(SELECT _LICENSEID FROM LICENSES WHERE _APPLICATIONID = " + applicationId + ")";
                ExecuteNonQuery(cmdText);

                cmdText = "DELETE FROM NOTES WHERE _SCOPE = 1 AND _PARENTID = " + applicationId;
                ExecuteNonQuery(cmdText);

                cmdText = "DELETE FROM NOTES WHERE _SCOPE = 2 AND _PARENTID IN " +
                          "(SELECT _LICENSEID FROM LICENSES WHERE _APPLICATIONID = " + applicationId + ")";
                ExecuteNonQuery(cmdText);

                cmdText = "DELETE FROM LICENSES WHERE _APPLICATIONID = " + applicationId;
                ExecuteNonQuery(cmdText);

                cmdText = "DELETE FROM USERDATA_VALUES WHERE _PARENTTYPE = 1 AND _PARENTID = " + applicationId;
                ExecuteNonQuery(cmdText);

                cmdText = "DELETE FROM APPLICATION_INSTANCES WHERE _APPLICATIONID = " + applicationId;
                ExecuteNonQuery(cmdText);

                cmdText = "DELETE FROM APPLICATIONS WHERE _APPLICATIONID = " + applicationId;
                ExecuteNonQuery(cmdText);
            }
        }

        /// <summary>
        /// Return a list of the names of all available publishers.  This is made up from those defined in the 
        /// application configuration file and also those defined within the database
        /// </summary>
        /// <returns></returns>
        public DataTable GetAllPublisherNamesAsDataTable(string aPublisherFilter, bool showIncluded, bool showIgnored)
        {
            if (isDebugEnabled) logger.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " in");

            DataTable dt = new DataTable();
            string whereClause = "";

            string cmdText =
                "SELECT DISTINCT _PUBLISHER FROM APPLICATIONS ";

            if (aPublisherFilter != "")
            {
                foreach (string publisher in aPublisherFilter.Split(';'))
                {
                    if (whereClause == "")
                        whereClause += "WHERE _PUBLISHER LIKE ('" + publisher + "') ";
                    else
                        whereClause += "OR _PUBLISHER LIKE ('" + publisher + "') ";
                }

                cmdText += whereClause;
            }

            if (showIncluded && !showIgnored)
            {
                if (cmdText.Contains("WHERE"))
                    cmdText += "AND _IGNORED = 0 ";
                else
                    cmdText += "WHERE _IGNORED = 0 ";
            }
            else if (!showIncluded && showIgnored)
            {
                if (cmdText.Contains("WHERE"))
                    cmdText += "AND _IGNORED = 1 ";
                else
                    cmdText += "WHERE _IGNORED = 1 ";
            }

            if (cmdText.Contains("WHERE"))
                cmdText += "AND _ALIASED_TOID = 0 ";
            else
                cmdText += "WHERE _ALIASED_TOID = 0 ";

            cmdText += "ORDER BY _PUBLISHER";

            if (isCompactDatabase)
            {
                using (SqlCeConnection conn = DatabaseConnection.CreateOpenCEConnection())
                {
                    using (SqlCeCommand command = new SqlCeCommand(cmdText, conn))
                    {
                        new SqlCeDataAdapter(command).Fill(dt);
                    }
                }
            }
            else
            {
                using (SqlConnection conn = DatabaseConnection.CreateOpenStandardConnection())
                {
                    using (SqlCommand command = new SqlCommand(cmdText, conn))
                    {
                        new SqlDataAdapter(command).Fill(dt);
                    }
                }
            }

            if (isDebugEnabled) logger.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " out");
            return dt;
        }

        public DataTable GetAllOSPublisherNamesAsDataTable()
        {
            if (isDebugEnabled) logger.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " in");

            DataTable dt = new DataTable();

            string commandText =
                "SELECT DISTINCT _PUBLISHER FROM APPLICATIONS ORDER BY _PUBLISHER";

            if (isCompactDatabase)
            {
                using (SqlCeConnection conn = DatabaseConnection.CreateOpenCEConnection())
                {
                    using (SqlCeCommand command = new SqlCeCommand(commandText, conn))
                    {
                        new SqlCeDataAdapter(command).Fill(dt);
                    }
                }
            }
            else
            {
                using (SqlConnection conn = DatabaseConnection.CreateOpenStandardConnection())
                {
                    using (SqlCommand command = new SqlCommand(commandText, conn))
                    {
                        new SqlDataAdapter(command).Fill(dt);
                    }
                }
            }

            if (isDebugEnabled) logger.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " out");
            return dt;
        }

        public DataTable GetAllPublisherNames()
        {
            if (isDebugEnabled) logger.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " in");

            string cmdText =
                "SELECT DISTINCT _PUBLISHER FROM APPLICATIONS ORDER BY _PUBLISHER";

            return PerformQuery(cmdText);
        }



        /// <summary>
        /// Return a list of publishers, optionally filtering this list to only include those
        /// publishers defined within the filter string
        /// </summary>
        /// <param name="publisherFilter">A semi-colon delimited list of publisher names
        /// If an empty string then no filtering will take place</param>
        /// <returns></returns>
        public DataTable GetPublishers(String publisherFilter)
        {
            if (isDebugEnabled) logger.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " in");

            AuditWizardDataAccess lAuditWizardDataAccess = new AuditWizardDataAccess();
            DataTable applicationsTable = new DataTable(TableNames.APPLICATIONS);

            if (isCompactDatabase)
            {
                // Before we call the stored procedure we need to handle the publisher filter if one has
                // been supplied.  
                publisherFilter = lAuditWizardDataAccess.BuildPublisherFilter(publisherFilter);

                try
                {
                    using (SqlCeConnection conn = DatabaseConnection.CreateOpenCEConnection())
                    {
                        string commandText = "SELECT DISTINCT(_PUBLISHER) FROM APPLICATIONS";

                        if (publisherFilter != "")
                            commandText += " WHERE _PUBLISHER IN ('" + publisherFilter + "')";

                        using (SqlCeCommand command = new SqlCeCommand(commandText, conn))
                        {
                            new SqlCeDataAdapter(command).Fill(applicationsTable);
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
                applicationsTable = lAuditWizardDataAccess.GetPublishers(publisherFilter);
            }

            if (isDebugEnabled) logger.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " out");
            return applicationsTable;
        }



        /// <summary>
        /// Delete all application instance records for the specified asset
        /// </summary>
        /// <returns></returns>
        public int ApplicationDeleteOrphans()
        {
            if (isDebugEnabled) logger.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " in");

            try
            {
                string commandText =
                            "DELETE FROM APPLICATIONS " +
                            "WHERE _APPLICATIONID NOT IN (SELECT DISTINCT _APPLICATIONID FROM APPLICATION_INSTANCES) " +
                            "AND _APPLICATIONID NOT IN (SELECT DISTINCT _APPLICATIONID FROM LICENSES) " +
                            "AND _APPLICATIONID NOT IN (SELECT DISTINCT _APPLICATIONID FROM ACTIONS) " +
                            "AND _APPLICATIONID NOT IN (SELECT DISTINCT _ALIASED_TOID FROM APPLICATIONS) " +
                    //"AND _APPLICATIONID NOT IN (SELECT DISTINCT _PARENTID FROM USERDATA_VALUES WHERE _PARENTTYPE = 1) " +
                            "AND _ALIASED_TOID = 0 " +
                            "AND _ASSIGNED_FILEID = 0";

                if (isCompactDatabase)
                {
                    using (SqlCeConnection conn = DatabaseConnection.CreateOpenCEConnection())
                    {
                        using (SqlCeCommand command = new SqlCeCommand(commandText, conn))
                        {
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
                            command.ExecuteNonQuery();
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
            return 0;
        }

        /// <summary>
        /// Change the publisher for the specified application
        /// </summary>
        /// <param name="theAsset"></param>
        /// <returns></returns>
        public int ApplicationUpdatePublisher(int aApplicationID, string aPublisher)
        {
            if (isDebugEnabled) logger.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " in with id : " + aApplicationID);

            if (isCompactDatabase)
            {
                try
                {
                    using (SqlCeConnection conn = DatabaseConnection.CreateOpenCEConnection())
                    {
                        string commandText =
                            "UPDATE APPLICATIONS SET _PUBLISHER = @publisher WHERE _APPLICATIONID = @applicationID";

                        using (SqlCeCommand command = new SqlCeCommand(commandText, conn))
                        {
                            command.Parameters.AddWithValue("@publisher", aPublisher);
                            command.Parameters.AddWithValue("@applicationID", aApplicationID);

                            command.ExecuteNonQuery();
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
                lAuditWizardDataAccess.ApplicationUpdatePublisher(aApplicationID, aPublisher);
            }

            if (isDebugEnabled) logger.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " out");
            return 0;
        }

        /// <summary>
        /// Set or Clear the alias for an application
        /// </summary>
        /// <param name="theAsset"></param>
        /// <returns></returns>
        public void ApplicationSetAlias(int aApplicationID, int aAliasApplicationID)
        {
            if (isDebugEnabled) logger.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " in");

            try
            {
                if (isCompactDatabase)
                {
                    using (SqlCeConnection conn = DatabaseConnection.CreateOpenCEConnection())
                    {
                        if (aAliasApplicationID == 0)
                        {
                            // *un-aliasing an application*
                            // reset the application aliasing, reset the old application id and remove the base app id value
                            string commandText = String.Format(
                                "UPDATE APPLICATION_INSTANCES " +
                                "SET _APPLICATIONID = {0}, _BASE_APPLICATIONID = 0 " +
                                "WHERE _BASE_APPLICATIONID = {0} AND " +
                                "_APPLICATIONID IN (SELECT _ALIASED_TOID " +
                                "FROM APPLICATIONS " +
                                "WHERE _APPLICATIONID = {0})", aApplicationID);

                            using (SqlCeCommand command = new SqlCeCommand(commandText, conn))
                            {
                                command.ExecuteNonQuery();
                            }

                            // remove the aliased reference in the APPLICATIONS table
                            commandText = String.Format(
                                "UPDATE APPLICATIONS " +
                                "SET _ALIASED_TOID = 0 " +
                                "WHERE _APPLICATIONID = {0}", aApplicationID);

                            using (SqlCeCommand command = new SqlCeCommand(commandText, conn))
                            {
                                command.ExecuteNonQuery();
                            }
                        }
                        else
                        {
                            // *aliasing an application*
                            // check first that this application isn't already an alias for another application
                            string commandText = String.Format(
                                "SELECT COUNT(*) " +
                                "FROM APPLICATIONS " +
                                "WHERE _ALIASED_TOID = {0}", aApplicationID);

                            using (SqlCeCommand command = new SqlCeCommand(commandText, conn))
                            {
                                if (Convert.ToInt32(command.ExecuteScalar()) != 0)
                                    return;
                            }

                            // define the relationship between the application and aliased application in the APPLICATIONS table
                            commandText = String.Format(
                                "UPDATE APPLICATIONS " +
                                "SET _ALIASED_TOID = {0} " +
                                "WHERE _APPLICATIONID = {1}", aAliasApplicationID, aApplicationID);

                            using (SqlCeCommand command = new SqlCeCommand(commandText, conn))
                            {
                                command.ExecuteNonQuery();
                            }

                            // update the APPLICATIONS_INSTANCES table so that each instance of the application points to the aliased app
                            commandText = String.Format(
                                    "UPDATE APPLICATION_INSTANCES " +
                                    "SET _BASE_APPLICATIONID = _APPLICATIONID, _APPLICATIONID = {0} " +
                                    "WHERE _APPLICATIONID = {1}", aAliasApplicationID, aApplicationID);

                            using (SqlCeCommand command = new SqlCeCommand(commandText, conn))
                            {
                                command.ExecuteNonQuery();
                            }
                        }
                    }
                }
                else
                {
                    using (SqlConnection conn = DatabaseConnection.CreateOpenStandardConnection())
                    {
                        if (aAliasApplicationID == 0)
                        {
                            // *un-aliasing an application*
                            // reset the application aliasing, reset the old application id and remove the base app id value
                            string commandText = String.Format(
                                "UPDATE APPLICATION_INSTANCES " +
                                "SET _APPLICATIONID = {0}, _BASE_APPLICATIONID = 0 " +
                                "WHERE _BASE_APPLICATIONID = {0} AND " +
                                "_APPLICATIONID IN (SELECT _ALIASED_TOID " +
                                "FROM APPLICATIONS " +
                                "WHERE _APPLICATIONID = {0})", aApplicationID);

                            using (SqlCommand command = new SqlCommand(commandText, conn))
                            {
                                command.ExecuteNonQuery();
                            }

                            // remove the aliased reference in the APPLICATIONS table
                            commandText = String.Format(
                                "UPDATE APPLICATIONS " +
                                "SET _ALIASED_TOID = 0 " +
                                "WHERE _APPLICATIONID = {0}", aApplicationID);

                            using (SqlCommand command = new SqlCommand(commandText, conn))
                            {
                                command.ExecuteNonQuery();
                            }
                        }
                        else
                        {
                            // *aliasing an application*
                            // check first that this application isn't already an alias for another application
                            string commandText = String.Format(
                                "SELECT COUNT(*) " +
                                "FROM APPLICATIONS " +
                                "WHERE _ALIASED_TOID = {0}", aApplicationID);

                            using (SqlCommand command = new SqlCommand(commandText, conn))
                            {
                                if (Convert.ToInt32(command.ExecuteScalar()) != 0)
                                    return;
                            }

                            // define the relationship between the application and aliased application in the APPLICATIONS table
                            commandText = String.Format(
                                "UPDATE APPLICATIONS " +
                                "SET _ALIASED_TOID = {0} " +
                                "WHERE _APPLICATIONID = {1}", aAliasApplicationID, aApplicationID);

                            using (SqlCommand command = new SqlCommand(commandText, conn))
                            {
                                command.ExecuteNonQuery();
                            }

                            // update the APPLICATIONS_INSTANCES table so that each instance of the application points to the aliased app
                            commandText = String.Format(
                                    "UPDATE APPLICATION_INSTANCES " +
                                    "SET _BASE_APPLICATIONID = _APPLICATIONID, _APPLICATIONID = {0} " +
                                    "WHERE _APPLICATIONID = {1}", aAliasApplicationID, aApplicationID);

                            using (SqlCommand command = new SqlCommand(commandText, conn))
                            {
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
        }

        public DataTable GetOSPickerValues(string columnName)
        {
            string commandText = String.Format(
                "SELECT DISTINCT {0} FROM APPLICATIONS WHERE _ISOS = 1 ORDER BY {0}", columnName);

            return PerformQuery(commandText);
        }

        public string GetCustomOSValues(string aColumnName, int aAssetId)
        {
            string commandText = String.Format(
                "SELECT {0} " +
                "FROM APPLICATIONS a " +
                "LEFT JOIN APPLICATION_INSTANCES ai ON (ai._applicationid = a._applicationid) " +
                "WHERE ai._assetid = {1} " +
                "AND a._isos = 1", aColumnName, aAssetId);

            return PerformScalarQuery(commandText);
        }

        public DataTable SelectApplicationByPublisherName(string aPublisherName)
        {
            string commandText = String.Format(
                "SELECT _APPLICATIONID, _PUBLISHER, _NAME, _VERSION, _ALIASED_TOID " +
                "FROM APPLICATIONS " +
                "WHERE _PUBLISHER = '{0}'", aPublisherName);

            return PerformQuery(commandText);
        }

        public int SelectAliasedToIdByApplicationId(int aApplicationId)
        {
            int lAliasedToId = -1;

            string commandText = String.Format(
                "SELECT _ALIASED_TOID " +
                "FROM APPLICATIONS " +
                "WHERE _APPLICATIONID = {0}", aApplicationId);

            try
            {
                if (isCompactDatabase)
                {
                    using (SqlCeConnection conn = DatabaseConnection.CreateOpenCEConnection())
                    {
                        using (SqlCeCommand command = new SqlCeCommand(commandText, conn))
                        {
                            object result = command.ExecuteScalar();
                            if ((result != null) && (result.GetType() != typeof(DBNull)))
                            {
                                lAliasedToId = Convert.ToInt32(result);
                            }
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
                            {
                                lAliasedToId = Convert.ToInt32(result);
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

            return lAliasedToId;
        }

        public int SelectIdByPublisherNameVersion(string aPublisherName, string aApplicationName, string aApplicationVersion)
        {
            int lApplicationId = -1;

            string commandText =
                "SELECT _APPLICATIONID " +
                "FROM APPLICATIONS " +
                "WHERE _PUBLISHER = @publisher AND _NAME = @name AND _VERSION = @version";

            try
            {
                if (isCompactDatabase)
                {
                    using (SqlCeConnection conn = DatabaseConnection.CreateOpenCEConnection())
                    {
                        using (SqlCeCommand command = new SqlCeCommand(commandText, conn))
                        {
                            command.Parameters.AddWithValue("@publisher", aPublisherName);
                            command.Parameters.AddWithValue("@name", aApplicationName);
                            command.Parameters.AddWithValue("@version", aApplicationVersion);

                            object result = command.ExecuteScalar();
                            if ((result != null) && (result.GetType() != typeof(DBNull)))
                            {
                                lApplicationId = Convert.ToInt32(result);
                            }
                        }
                    }
                }
                else
                {
                    using (SqlConnection conn = DatabaseConnection.CreateOpenStandardConnection())
                    {
                        using (SqlCommand command = new SqlCommand(commandText, conn))
                        {
                            command.Parameters.AddWithValue("@publisher", aPublisherName);
                            command.Parameters.AddWithValue("@name", aApplicationName);
                            command.Parameters.AddWithValue("@version", aApplicationVersion);

                            object result = command.ExecuteScalar();
                            if ((result != null) && (result.GetType() != typeof(DBNull)))
                            {
                                lApplicationId = Convert.ToInt32(result);
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

            return lApplicationId;
        }

        public void UpdateAliasedPublishers(string aAliasedPublisherName, int aApplicationId)
        {
            if (isDebugEnabled) logger.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " in");

            string lCommandText = String.Format(
                "UPDATE APPLICATIONS " +
                "SET _PUBLISHER = '{0}' " +
                "WHERE _APPLICATIONID = {1}", aAliasedPublisherName, aApplicationId);

            try
            {
                if (isCompactDatabase)
                {
                    using (SqlCeConnection conn = DatabaseConnection.CreateOpenCEConnection())
                    {
                        using (SqlCeCommand command = new SqlCeCommand(lCommandText, conn))
                        {
                            command.ExecuteNonQuery();
                        }
                    }
                }
                else
                {
                    using (SqlConnection conn = DatabaseConnection.CreateOpenStandardConnection())
                    {
                        using (SqlCommand command = new SqlCommand(lCommandText, conn))
                        {
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
        }

        private DataTable PerformQuery(string aCommandText)
        {
            if (isDebugEnabled) logger.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " in");

            DataTable statisticsTable = new DataTable();
            try
            {
                if (isCompactDatabase)
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

        public DataTable GetLicenseKeysByPublisher(string publisherName, bool showIncluded, bool showIgnored)
        {
            string commandText = String.Format(
                "select ast._name as \"Installed On\", a._name \"Application Name\", a._version as \"Version\", ai._productid as \"Serial Number\", ai._cdkey as \"CD Key\" " +
                "from applications a " +
                "left join application_instances ai on (ai._applicationid = a._applicationid) " +
                "inner join assets ast on (ast._assetid = ai._assetid) " +
                "where a._publisher = '{0}' " +
                "and ast._stock_status <> 3 ", publisherName);

            if (showIncluded && !showIgnored)
                commandText += "and a._ignored = 0 ";

            else if (!showIncluded && showIgnored)
                commandText += "and a._ignored = 1 ";

            string lAssetIds = new AssetDAO().GetSelectedAssets();

            if (lAssetIds != "")
                commandText += "and ast._ASSETID IN (" + lAssetIds + ") ";

            commandText += "group by ast._name, a._name, a._version, ai._productid, ai._cdkey";

            return PerformQuery(commandText);
        }

        public void RevertPublisherOfAliasedApplications(string aOriginalPublisherName)
        {
            if (isDebugEnabled) logger.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " in");

            string commandText = String.Format(
                "UPDATE APPLICATIONS " +
                "SET _PUBLISHER = '{0}' " +
                "WHERE _APPLICATIONID in ( " +
                "SELECT pap._APPLICATIONID " +
                "FROM PUBLISHER_ALIAS pa " +
                "INNER JOIN PUBLISHER_ALIAS_APP pap on (pap._PUBLISHER_ALIAS_ID = pa._PUBLISHER_ALIAS_ID) " +
                "WHERE pa._ORIG_PUBLISHER = '{0}')", aOriginalPublisherName);

            ExecuteNonQuery(commandText);

            if (isDebugEnabled) logger.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " out");
        }

        public void DeleteByApplicationId(int aApplicationId)
        {
            string commandText = String.Format(
                "DELETE FROM APPLICATIONS " +
                "WHERE _APPLICATIONID = {0}", aApplicationId);

            ExecuteNonQuery(commandText);
        }

        public DataTable GetAllApplicationsByPublisher(string aPublisherName, bool showIncluded, bool showIgnored)
        {
            string commandText =
                "SELECT _publisher + '|' + _name, _version " +
                "FROM APPLICATIONS ";

            if (aPublisherName != "")
                commandText += String.Format("WHERE _publisher = '{0}' ", aPublisherName);

            if (showIncluded && !showIgnored)
            {
                if (aPublisherName == "")
                    commandText += "WHERE _IGNORED = 0 ";
                else
                    commandText += "AND _IGNORED = 0 ";
            }

            else if (!showIncluded && showIgnored)
            {
                if (aPublisherName == "")
                    commandText += "WHERE _IGNORED = 1 ";
                else
                    commandText += "AND _IGNORED = 1 ";
            }

            commandText += "ORDER BY _publisher";

            return PerformQuery(commandText);
        }

        public DataTable PopulateApplicationsTableForCustomReport()
        {
            string commandText =
                "SELECT ai._assetid, ap._publisher, ap._name, ap._version " +
                "FROM APPLICATIONS ap " +
                "INNER JOIN APPLICATION_INSTANCES ai ON (ai._applicationid = ap._applicationid) " +
                "GROUP BY ai._assetid, ap._publisher, ap._name, ap._version";

            return PerformQuery(commandText);
        }

        private void ExecuteNonQuery(string aCommandText)
        {
            try
            {
                if (isCompactDatabase)
                {
                    using (SqlCeConnection conn = DatabaseConnection.CreateOpenCEConnection())
                    {
                        using (SqlCeCommand command = new SqlCeCommand(aCommandText, conn))
                        {
                            command.ExecuteNonQuery();
                        }
                    }
                }
                else
                {
                    using (SqlConnection conn = DatabaseConnection.CreateOpenStandardConnection())
                    {
                        using (SqlCommand command = new SqlCommand(aCommandText, conn))
                        {
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
        }

        private string PerformScalarQuery(string commandText)
        {
            string returnValue = String.Empty;

            try
            {
                if (isCompactDatabase)
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

        #endregion Applications table
    }
}
