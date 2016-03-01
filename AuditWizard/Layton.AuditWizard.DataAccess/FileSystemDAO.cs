using System;
using System.Configuration;
using System.Data;
using System.Data.SqlServerCe;
using System.Data.SqlClient;
using System.IO;
using System.Windows.Forms;

namespace Layton.AuditWizard.DataAccess
{
    public class FileSystemDAO
    {
        #region Data

        private string connectionStringCompact = null;
        protected static readonly log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private static readonly bool isDebugEnabled = logger.IsDebugEnabled;
        private bool isCompactDatabase = false;
        private Configuration config = ConfigurationManager.OpenExeConfiguration(Path.Combine(Application.StartupPath, "AuditWizardv8.exe"));

        #endregion

        public FileSystemDAO()
        {
            isCompactDatabase = Convert.ToBoolean(config.AppSettings.Settings["CompactDatabaseType"].Value);
            connectionStringCompact = "Data Source=" + Application.StartupPath + Convert.ToString(config.AppSettings.Settings["ConnectionStringCompact"].Value);
        }

        #region File Systems Table


        /// <summary>
        /// Return a table containing all of the Folders that have been audited for an asset
        /// </summary>
        /// <returns></returns>
        public DataTable EnumerateFileSystemFolders(int assetID)
        {
            if (isDebugEnabled) logger.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " in");

            DataTable table = new DataTable(TableNames.FS_FOLDERS);

            try
            {
                string commandText =
                        "SELECT FS_FOLDERS.[_FOLDERID] " +
                        ", FS_FOLDERS.[_NAME] " +
                        ", FS_FOLDERS.[_PARENTID] " +
                        ", FS_FOLDERS.[_ASSETID] " +
                        "FROM FS_FOLDERS " +
                        "WHERE FS_FOLDERS._ASSETID = @nAssetID " +
                        "ORDER BY FS_FOLDERS.[_FOLDERID]";

                if (isCompactDatabase)
                {
                    SqlCeParameter[] spParams = new SqlCeParameter[1];
                    spParams[0] = new SqlCeParameter("@nAssetID", assetID);

                    using (SqlCeConnection conn = DatabaseConnection.CreateOpenCEConnection())
                    {
                        using (SqlCeCommand command = new SqlCeCommand(commandText, conn))
                        {
                            command.Parameters.AddRange(spParams);
                            new SqlCeDataAdapter(command).Fill(table);
                        }
                    }
                }
                else
                {
                    SqlParameter[] spParams = new SqlParameter[1];
                    spParams[0] = new SqlParameter("@nAssetID", assetID);

                    using (SqlConnection conn = DatabaseConnection.CreateOpenStandardConnection())
                    {
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



        /// <summary>
        /// Return a table containing all of the Files that have been audited for an asset
        /// </summary>
        /// <returns></returns>
        public DataTable EnumerateFileSystemFiles(int assetID)
        {
            if (isDebugEnabled) logger.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " in");

            DataTable table = new DataTable(TableNames.FS_FILES);

            try
            {
                string commandText =
                               "SELECT FS_FILES.* " +
                               ",FS_FOLDERS._NAME AS PARENTNAME " +
                               "FROM FS_FILES " +
                               "LEFT JOIN FS_FOLDERS ON (FS_FILES._PARENTID = FS_FOLDERS._FOLDERID) " +
                               "WHERE FS_FILES._ASSETID = @nAssetID " +
                               "ORDER BY FS_FILES._PARENTID";

                if (isCompactDatabase)
                {
                    using (SqlCeConnection conn = DatabaseConnection.CreateOpenCEConnection())
                    {
                        SqlCeParameter[] spParams = new SqlCeParameter[1];
                        spParams[0] = new SqlCeParameter("@nAssetID", assetID);

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
                        spParams[0] = new SqlParameter("@nAssetID", assetID);

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



        /// <summary>
        /// Return a table containing all of the Files that have been assigned
        /// </summary>
        /// <returns></returns>
        public DataTable EnumerateFileSystemFileAssignments()
        {
            if (isDebugEnabled) logger.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " in");

            DataTable table = new DataTable(TableNames.FS_FILES);

            string commandText =
                               "SELECT FS_FILES.* " +
                               ",FS_FOLDERS._NAME AS PARENTNAME " +
                               "FROM FS_FILES " +
                               "LEFT JOIN FS_FOLDERS ON (FS_FILES._PARENTID = FS_FOLDERS._FOLDERID) " +
                               "WHERE FS_FILES._ASSETID = 0 " +
                               "ORDER BY FS_FILES._PARENTID";
            try
            {
                if (isCompactDatabase)
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


        /// <summary>
        /// Return a table containing all of the Files which match the file specification supplied
        /// </summary>
        /// <returns></returns>
        public DataTable EnumerateFileSystemFiles(string aFilter)
        {
            if (isDebugEnabled) logger.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " in");

            DataTable table = new DataTable(TableNames.FS_FILES);
            string commandText = String.Empty;

            try
            {
                if (isCompactDatabase)
                {
                    using (SqlCeConnection conn = DatabaseConnection.CreateOpenCEConnection())
                    {
                        if (aFilter == String.Empty)
                        {
                            commandText =
                                "SELECT FS_FILES.* " +
                                ",FS_FOLDERS._NAME AS PARENTNAME " +
                                ",ASSETS._NAME AS ASSETNAME " +
                                ",LOCATIONS._FULLNAME AS FULLLOCATIONNAME " +
                                "FROM FS_FILES " +
                                "LEFT JOIN FS_FOLDERS ON (FS_FILES._PARENTID = FS_FOLDERS._FOLDERID) " +
                                "LEFT JOIN ASSETS ON (FS_FILES._ASSETID = ASSETS._ASSETID) " +
                                "LEFT JOIN LOCATIONS ON (ASSETS._LOCATIONID = LOCATIONS._LOCATIONID) " +
                                "ORDER BY FS_FILES._PARENTID";
                        }
                        else
                        {
                            commandText =
                                "SELECT FS_FILES.* " +
                                ",FS_FOLDERS._NAME AS PARENTNAME " +
                                ",ASSETS._NAME AS ASSETNAME " +
                                ",LOCATIONS._FULLNAME AS FULLLOCATIONNAME " +
                                "FROM FS_FILES " +
                                "LEFT JOIN FS_FOLDERS ON (FS_FILES._PARENTID = FS_FOLDERS._FOLDERID) " +
                                "LEFT JOIN ASSETS ON (FS_FILES._ASSETID = ASSETS._ASSETID) " +
                                "LEFT JOIN LOCATIONS ON (ASSETS._LOCATIONID = LOCATIONS._LOCATIONID) " +
                                "WHERE FS_FILES._NAME LIKE @cFilter " +
                                "ORDER BY FS_FILES._PARENTID";
                        }

                        using (SqlCeCommand command = new SqlCeCommand(commandText, conn))
                        {
                            if (aFilter != String.Empty)
                                command.Parameters.AddWithValue("@cFilter", aFilter);

                            new SqlCeDataAdapter(command).Fill(table);
                        }
                    }
                }
                else
                {
                    using (SqlConnection conn = DatabaseConnection.CreateOpenStandardConnection())
                    {
                        if (aFilter == String.Empty)
                        {
                            commandText =
                                "SELECT FS_FILES.* " +
                                ",FS_FOLDERS._NAME AS PARENTNAME " +
                                ",ASSETS._NAME AS ASSETNAME " +
                                ",LOCATIONS._FULLNAME AS FULLLOCATIONNAME " +
                                "FROM FS_FILES " +
                                "LEFT JOIN FS_FOLDERS ON (FS_FILES._PARENTID = FS_FOLDERS._FOLDERID) " +
                                "LEFT JOIN ASSETS ON (FS_FILES._ASSETID = ASSETS._ASSETID) " +
                                "LEFT JOIN LOCATIONS ON (ASSETS._LOCATIONID = LOCATIONS._LOCATIONID) " +
                                "ORDER BY FS_FILES._PARENTID";
                        }
                        else
                        {
                            commandText =
                                "SELECT FS_FILES.* " +
                                ",FS_FOLDERS._NAME AS PARENTNAME " +
                                ",ASSETS._NAME AS ASSETNAME " +
                                ",LOCATIONS._FULLNAME AS FULLLOCATIONNAME " +
                                "FROM FS_FILES " +
                                "LEFT JOIN FS_FOLDERS ON (FS_FILES._PARENTID = FS_FOLDERS._FOLDERID) " +
                                "LEFT JOIN ASSETS ON (FS_FILES._ASSETID = ASSETS._ASSETID) " +
                                "LEFT JOIN LOCATIONS ON (ASSETS._LOCATIONID = LOCATIONS._LOCATIONID) " +
                                "WHERE FS_FILES._NAME LIKE @cFilter " +
                                "ORDER BY FS_FILES._PARENTID";
                        }

                        using (SqlCommand command = new SqlCommand(commandText, conn))
                        {
                            if (aFilter != String.Empty)
                                command.Parameters.AddWithValue("@cFilter", aFilter);

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

        /// <summary>
        /// Add a new File System Folder to the database
        /// </summary>
        /// <returns></returns>
        public int FileSystemFolder_Add(FileSystemFolder aFolder)
        {
            if (isDebugEnabled) logger.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " in");

            int lItemID = 0;

            try
            {
                string commandText =
                               "INSERT INTO FS_FOLDERS " +
                               "(_NAME ,_PARENTID ,_ASSETID) " +
                               "VALUES " +
                               "(@cName, @nParentID, @nAssetID)";

                if (isCompactDatabase)
                {
                    using (SqlCeConnection conn = DatabaseConnection.CreateOpenCEConnection())
                    {
                        SqlCeParameter[] spParams = new SqlCeParameter[3];
                        spParams[0] = new SqlCeParameter("@cName", aFolder.Name);
                        spParams[1] = new SqlCeParameter("@nParentID", aFolder.ParentID);
                        spParams[2] = new SqlCeParameter("@nAssetID", aFolder.AssetID);

                        using (SqlCeCommand command = new SqlCeCommand(commandText, conn))
                        {
                            command.Parameters.AddRange(spParams);
                            command.ExecuteNonQuery();
                        }

                        using (SqlCeCommand command = new SqlCeCommand("SELECT @@IDENTITY", conn))
                        {
                            lItemID = Convert.ToInt32(command.ExecuteScalar());
                        }
                    }
                }
                else
                {
                    using (SqlConnection conn = DatabaseConnection.CreateOpenStandardConnection())
                    {
                        SqlParameter[] spParams = new SqlParameter[3];
                        spParams[0] = new SqlParameter("@cName", aFolder.Name);
                        spParams[1] = new SqlParameter("@nParentID", aFolder.ParentID);
                        spParams[2] = new SqlParameter("@nAssetID", aFolder.AssetID);

                        using (SqlCommand command = new SqlCommand(commandText, conn))
                        {
                            command.Parameters.AddRange(spParams);
                            command.ExecuteNonQuery();
                        }

                        using (SqlCommand command = new SqlCommand("SELECT @@IDENTITY", conn))
                        {
                            lItemID = Convert.ToInt32(command.ExecuteScalar());
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


        /// <summary>
        /// Add a new File System File to the database
        /// </summary>
        /// <returns></returns>
        public int FileSystemFile_Add(FileSystemFile aFile)
        {
            if (isDebugEnabled) logger.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " in");

            int lItemID = 0;

            try
            {
                if (isCompactDatabase)
                {
                    using (SqlCeConnection conn = DatabaseConnection.CreateOpenCEConnection())
                    {
                        string commandText =
                               "INSERT INTO FS_FILES " +
                               "(_NAME ,_PARENTID ,_ASSETID ,_SIZE ,_CREATED_DATE ,_MODIFIED_DATE ,_LASTACCESSED_DATE " +
                               ",_PUBLISHER ,_PRODUCTNAME ,_DESCRIPTION ,_PRODUCT_VERSION1 ,_PRODUCT_VERSION2 ,_FILE_VERSION1 " +
                               ",_FILE_VERSION2 ,_ORIGINAL_FILENAME) " +
                               "VALUES " +
                               "(@cName ,@nParentID ,@nAssetID ,@nSize ,@dtCreated ,@dtModified ,@dtLastAccessed ,@cPublisher " +
                               ",@cProductName ,@cDescription ,@cPVersion1 ,@cPVersion2 ,@cFVersion1 ,@cFVersion2 ,@cFilename )";

                        SqlCeParameter[] spParams = new SqlCeParameter[15];
                        spParams[0] = new SqlCeParameter("@cName", aFile.Name);
                        spParams[1] = new SqlCeParameter("@nParentID", aFile.ParentID);
                        spParams[2] = new SqlCeParameter("@nAssetID", aFile.AssetID);
                        spParams[3] = new SqlCeParameter("@nSize", aFile.Size);

                        if (aFile.CreatedDateTime.Ticks == 0)
                            spParams[4] = new SqlCeParameter("@dtCreated", DBNull.Value);
                        else
                            spParams[4] = new SqlCeParameter("@dtCreated", aFile.CreatedDateTime);

                        if (aFile.ModifiedDateTime.Ticks == 0)
                            spParams[5] = new SqlCeParameter("@dtModified", DBNull.Value);
                        else
                            spParams[5] = new SqlCeParameter("@dtModified", aFile.ModifiedDateTime);

                        if (aFile.LastAccessedDateTime.Ticks == 0)
                            spParams[6] = new SqlCeParameter("@dtLastAccessed", DBNull.Value);
                        else
                            spParams[6] = new SqlCeParameter("@dtLastAccessed", aFile.LastAccessedDateTime);

                        spParams[7] = new SqlCeParameter("@cPublisher", aFile.Publisher);
                        spParams[8] = new SqlCeParameter("@cProductName", aFile.ProductName);
                        spParams[9] = new SqlCeParameter("@cDescription", aFile.Description);
                        spParams[10] = new SqlCeParameter("@cPVersion1", aFile.ProductVersion1);
                        spParams[11] = new SqlCeParameter("@cPVersion2", aFile.ProductVersion2);
                        spParams[12] = new SqlCeParameter("@cFVersion1", aFile.FileVersion1);
                        spParams[13] = new SqlCeParameter("@cFVersion2", aFile.FileVersion2);
                        spParams[14] = new SqlCeParameter("@cFilename", aFile.OriginalFileName);

                        using (SqlCeCommand command = new SqlCeCommand(commandText, conn))
                        {
                            command.Parameters.AddRange(spParams);
                            command.ExecuteNonQuery();
                        }

                        using (SqlCeCommand command = new SqlCeCommand("SELECT @@IDENTITY", conn))
                        {
                            lItemID = Convert.ToInt32(command.ExecuteScalar());
                        }

                        commandText =
                            "SELECT min(_FILEID) FROM FS_FILES " +
                            "WHERE _NAME = @cName " +
                            "AND _PUBLISHER = @cPublisher " +
                            "AND _PRODUCTNAME = @cProductName " +
                            "AND _FILE_VERSION1 = @cFVersion1 " +
                            "AND _ASSETID = 0";

                        int lAssignmentId = -1;

                        using (SqlCeCommand command = new SqlCeCommand(commandText, conn))
                        {
                            command.Parameters.AddWithValue("@cName", aFile.Name);
                            command.Parameters.AddWithValue("@cPublisher", aFile.Publisher);
                            command.Parameters.AddWithValue("@cProductName", aFile.ProductName);
                            command.Parameters.AddWithValue("@cFVersion1", aFile.FileVersion1);

                            object result = command.ExecuteScalar();
                            if ((result != null) && (result.GetType() != typeof(DBNull)))
                            {
                                lAssignmentId = (int)result;

                            }
                        }

                        if (lAssignmentId != -1)
                        {
                            commandText =
                                "SELECT _ASSIGN_APPLICATIONID FROM FS_FILES WHERE _FILEID = @nAssignment";

                            int lAssignApplicationID = -1;

                            using (SqlCeCommand command = new SqlCeCommand(commandText, conn))
                            {
                                command.Parameters.AddWithValue("@nAssignment", lAssignmentId);

                                object result = command.ExecuteScalar();

                                if ((result != null) && (result.GetType() != typeof(DBNull)))
                                    lAssignApplicationID = Convert.ToInt32(result);
                            }

                            if (lAssignApplicationID != -1)
                            {
                                commandText =
                                    "UPDATE FS_FILES " +
                                    "SET _ASSIGN_APPLICATIONID = @nAssignment " +
                                    "WHERE _FILEID = @nReturnID";

                                using (SqlCeCommand command = new SqlCeCommand(commandText, conn))
                                {
                                    command.Parameters.AddWithValue("@nReturnID", lItemID);
                                    command.Parameters.AddWithValue("@nAssignment", lAssignApplicationID);

                                    command.ExecuteNonQuery();
                                }

                                commandText = String.Format(
                                    "SELECT COUNT(*) " +
                                    "FROM APPLICATION_INSTANCES " +
                                    "WHERE _ASSETID = {0} and _APPLICATIONID = {1}", aFile.AssetID, lAssignApplicationID);

                                int lResult = -1;

                                using (SqlCeCommand command = new SqlCeCommand(commandText, conn))
                                {
                                    lResult = Convert.ToInt32(command.ExecuteScalar());
                                }
                                if (lResult == 0)
                                {
                                    // if it doesn't already exist, insert a record into the application_instances table
                                    int lBaseApplicationId = 0;
                                    int aliasedToId = 0;

                                    // need to check first for pre-aliased applications
                                    // if this application has already been aliased to another, we need to get the aliased application id
                                    // and assign it to this lReturnedApplicationID. The current lReturnedApplicationID becomes the base application id.
                                    commandText =
                                        String.Format(
                                        "SELECT _ALIASED_TOID " +
                                        "FROM APPLICATIONS " +
                                        "WHERE _APPLICATIONID = {0}", lAssignApplicationID);

                                    using (SqlCeCommand command = new SqlCeCommand(commandText, conn))
                                    {
                                        aliasedToId = (int)command.ExecuteScalar();
                                    }

                                    // if we found an aliased id, we need to assign it as the current application id.
                                    // the original application id is stored in the base application id
                                    if (aliasedToId != 0)
                                    {
                                        lBaseApplicationId = lAssignApplicationID;
                                        lAssignApplicationID = aliasedToId;
                                    }

                                    commandText =
                                        "INSERT INTO APPLICATION_INSTANCES (_APPLICATIONID, _BASE_APPLICATIONID, _ASSETID, _PRODUCTID, _CDKEY) " +
                                        "VALUES (@nApplicationID, @nBaseApplicationID, @nAssetID, '', '')";

                                    using (SqlCeCommand command = new SqlCeCommand(commandText, conn))
                                    {
                                        command.Parameters.AddWithValue("@nApplicationID", lAssignApplicationID);
                                        command.Parameters.AddWithValue("@nBaseApplicationID", lBaseApplicationId);
                                        command.Parameters.AddWithValue("@nAssetID", aFile.AssetID);

                                        command.ExecuteNonQuery();
                                    }
                                }
                            }
                        }
                    }
                }
                else
                {
                    using (SqlConnection conn = DatabaseConnection.CreateOpenStandardConnection())
                    {
                        string commandText =
                               "INSERT INTO FS_FILES " +
                               "(_NAME ,_PARENTID ,_ASSETID ,_SIZE ,_CREATED_DATE ,_MODIFIED_DATE ,_LASTACCESSED_DATE " +
                               ",_PUBLISHER ,_PRODUCTNAME ,_DESCRIPTION ,_PRODUCT_VERSION1 ,_PRODUCT_VERSION2 ,_FILE_VERSION1 " +
                               ",_FILE_VERSION2 ,_ORIGINAL_FILENAME) " +
                               "VALUES " +
                               "(@cName ,@nParentID ,@nAssetID ,@nSize ,@dtCreated ,@dtModified ,@dtLastAccessed ,@cPublisher " +
                               ",@cProductName ,@cDescription ,@cPVersion1 ,@cPVersion2 ,@cFVersion1 ,@cFVersion2 ,@cFilename )";

                        SqlParameter[] spParams = new SqlParameter[15];
                        spParams[0] = new SqlParameter("@cName", aFile.Name);
                        spParams[1] = new SqlParameter("@nParentID", aFile.ParentID);
                        spParams[2] = new SqlParameter("@nAssetID", aFile.AssetID);
                        spParams[3] = new SqlParameter("@nSize", aFile.Size);

                        if (aFile.CreatedDateTime.Ticks == 0)
                            spParams[4] = new SqlParameter("@dtCreated", DBNull.Value);
                        else
                            spParams[4] = new SqlParameter("@dtCreated", aFile.CreatedDateTime);

                        if (aFile.ModifiedDateTime.Ticks == 0)
                            spParams[5] = new SqlParameter("@dtModified", DBNull.Value);
                        else
                            spParams[5] = new SqlParameter("@dtModified", aFile.ModifiedDateTime);

                        if (aFile.LastAccessedDateTime.Ticks == 0)
                            spParams[6] = new SqlParameter("@dtLastAccessed", DBNull.Value);
                        else
                            spParams[6] = new SqlParameter("@dtLastAccessed", aFile.LastAccessedDateTime);

                        spParams[7] = new SqlParameter("@cPublisher", aFile.Publisher);
                        spParams[8] = new SqlParameter("@cProductName", aFile.ProductName);
                        spParams[9] = new SqlParameter("@cDescription", aFile.Description);
                        spParams[10] = new SqlParameter("@cPVersion1", aFile.ProductVersion1);
                        spParams[11] = new SqlParameter("@cPVersion2", aFile.ProductVersion2);
                        spParams[12] = new SqlParameter("@cFVersion1", aFile.FileVersion1);
                        spParams[13] = new SqlParameter("@cFVersion2", aFile.FileVersion2);
                        spParams[14] = new SqlParameter("@cFilename", aFile.OriginalFileName);

                        using (SqlCommand command = new SqlCommand(commandText, conn))
                        {
                            command.Parameters.AddRange(spParams);
                            command.ExecuteNonQuery();
                        }

                        using (SqlCommand command = new SqlCommand("SELECT @@IDENTITY", conn))
                        {
                            lItemID = Convert.ToInt32(command.ExecuteScalar());
                        }

                        commandText =
                            "SELECT min(_FILEID) FROM FS_FILES " +
                            "WHERE _NAME = @cName " +
                            "AND _PUBLISHER = @cPublisher " +
                            "AND _PRODUCTNAME = @cProductName " +
                            "AND _FILE_VERSION1 = @cFVersion1 " +
                            "AND _ASSETID = 0";

                        int lAssignmentId = -1;

                        using (SqlCommand command = new SqlCommand(commandText, conn))
                        {
                            command.Parameters.AddWithValue("@cName", aFile.Name);
                            command.Parameters.AddWithValue("@cPublisher", aFile.Publisher);
                            command.Parameters.AddWithValue("@cProductName", aFile.ProductName);
                            command.Parameters.AddWithValue("@cFVersion1", aFile.FileVersion1);

                            object result = command.ExecuteScalar();
                            if ((result != null) && (result.GetType() != typeof(DBNull)))
                            {
                                lAssignmentId = (int)result;

                            }
                        }

                        if (lAssignmentId != -1)
                        {
                            commandText =
                                "SELECT _ASSIGN_APPLICATIONID FROM FS_FILES WHERE _FILEID = @nAssignment";

                            int lAssignApplicationID = -1;

                            using (SqlCommand command = new SqlCommand(commandText, conn))
                            {
                                command.Parameters.AddWithValue("@nAssignment", lAssignmentId);

                                object result = command.ExecuteScalar();

                                if ((result != null) && (result.GetType() != typeof(DBNull)))
                                    lAssignApplicationID = Convert.ToInt32(result);
                            }

                            if (lAssignApplicationID != -1)
                            {
                                commandText =
                                    "UPDATE FS_FILES " +
                                    "SET _ASSIGN_APPLICATIONID = @nAssignment " +
                                    "WHERE _FILEID = @nReturnID";

                                using (SqlCommand command = new SqlCommand(commandText, conn))
                                {
                                    command.Parameters.AddWithValue("@nReturnID", lItemID);
                                    command.Parameters.AddWithValue("@nAssignment", lAssignApplicationID);

                                    command.ExecuteNonQuery();
                                }

                                commandText = String.Format(
                                    "SELECT COUNT(*) " +
                                    "FROM APPLICATION_INSTANCES " +
                                    "WHERE _ASSETID = {0} and _APPLICATIONID = {1}", aFile.AssetID, lAssignApplicationID);

                                int lResult = -1;

                                using (SqlCommand command = new SqlCommand(commandText, conn))
                                {
                                    lResult = Convert.ToInt32(command.ExecuteScalar());
                                }
                                if (lResult == 0)
                                {
                                    // if it doesn't already exist, insert a record into the application_instances table
                                    int lBaseApplicationId = 0;
                                    int aliasedToId = 0;

                                    // need to check first for pre-aliased applications
                                    // if this application has already been aliased to another, we need to get the aliased application id
                                    // and assign it to this lReturnedApplicationID. The current lReturnedApplicationID becomes the base application id.
                                    commandText =
                                        String.Format(
                                        "SELECT _ALIASED_TOID " +
                                        "FROM APPLICATIONS " +
                                        "WHERE _APPLICATIONID = {0}", lAssignApplicationID);

                                    using (SqlCommand command = new SqlCommand(commandText, conn))
                                    {
                                        aliasedToId = (int)command.ExecuteScalar();
                                    }

                                    // if we found an aliased id, we need to assign it as the current application id.
                                    // the original application id is stored in the base application id
                                    if (aliasedToId != 0)
                                    {
                                        lBaseApplicationId = lAssignApplicationID;
                                        lAssignApplicationID = aliasedToId;
                                    }

                                    commandText =
                                        "INSERT INTO APPLICATION_INSTANCES (_APPLICATIONID, _BASE_APPLICATIONID, _ASSETID, _PRODUCTID, _CDKEY) " +
                                        "VALUES (@nApplicationID, @nBaseApplicationID, @nAssetID, '', '')";

                                    using (SqlCommand command = new SqlCommand(commandText, conn))
                                    {
                                        command.Parameters.AddWithValue("@nApplicationID", lAssignApplicationID);
                                        command.Parameters.AddWithValue("@nBaseApplicationID", lBaseApplicationId);
                                        command.Parameters.AddWithValue("@nAssetID", aFile.AssetID);

                                        command.ExecuteNonQuery();
                                    }
                                }
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




        /// <summary>
        /// Clean all of the FileSystem records for the specified asset
        /// </summary>
        /// <returns></returns>
        public int FileSystemFolder_Clean(int assetID)
        {
            if (isDebugEnabled) logger.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " in");

            if (isCompactDatabase)
            {
                SqlCeTransaction transaction = null;

                using (SqlCeConnection conn = DatabaseConnection.CreateOpenCEConnection())
                {
                    try
                    {
                        transaction = conn.BeginTransaction();

                        string commandText =
                               "DELETE FROM FS_FILES WHERE _ASSETID = @nAssetID";

                        using (SqlCeCommand command = new SqlCeCommand(commandText, conn))
                        {
                            command.Parameters.AddWithValue("@nAssetID", assetID);
                            command.ExecuteNonQuery();
                        }

                        commandText =
                            "DELETE FROM FS_FOLDERS WHERE _ASSETID = @nAssetID";

                        using (SqlCeCommand command = new SqlCeCommand(commandText, conn))
                        {
                            command.Parameters.AddWithValue("@nAssetID", assetID);
                            command.ExecuteNonQuery();
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
                lAuditWizardDataAccess.FileSystemFolder_Clean(assetID);
            }

            if (isDebugEnabled) logger.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " out");
            return 0;
        }



        /// <summary>
        /// Assign (or unassign) a File System File 
        /// </summary>
        /// <returns></returns>
        public int FileSystemFile_Assign(FileSystemFile aFile)
        {
            if (isDebugEnabled) logger.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " in");

            int lItemID = 0;
            try
            {
                if (isCompactDatabase)
                {
                    using (SqlCeConnection conn = DatabaseConnection.CreateOpenCEConnection())
                    {
                        string commandText = String.Empty;
                        int lFileAssignmentId = -1;
                        int lAssignedAppliationId = -1;

                        if (aFile.AssignApplicationID == 0)
                        {
                            commandText =
                                "SELECT MIN(_FILEID) FROM FS_FILES " +
                                "WHERE _NAME = @cName " +
                                "AND _PUBLISHER = @cPublisher " +
                                "AND _PRODUCTNAME = @cProductName " +
                                "AND _FILE_VERSION1 = @cFVersion1 " +
                                "AND _ASSETID = 0";

                            SqlCeParameter[] spParams = new SqlCeParameter[4];
                            spParams[0] = new SqlCeParameter("@cName", aFile.Name);
                            spParams[1] = new SqlCeParameter("@cPublisher", aFile.Publisher);
                            spParams[2] = new SqlCeParameter("@cProductName", aFile.ProductName);
                            spParams[3] = new SqlCeParameter("@cFVersion1", aFile.FileVersion1);

                            using (SqlCeCommand command = new SqlCeCommand(commandText, conn))
                            {
                                command.Parameters.AddRange(spParams);

                                object result = command.ExecuteScalar();
                                if ((result != null) && (result.GetType() != typeof(DBNull)))
                                    lFileAssignmentId = (int)result;
                            }

                            if (lFileAssignmentId != -1)
                            {
                                commandText =
                                    "UPDATE FS_FILES SET _ASSIGN_APPLICATIONID = 0 " +
                                    "WHERE _NAME = @cName " +
                                    "AND _PUBLISHER = @cPublisher " +
                                    "AND _PRODUCTNAME = @cProductName " +
                                    "AND _FILE_VERSION1 = @cFVersion1";

                                using (SqlCeCommand command = new SqlCeCommand(commandText, conn))
                                {
                                    SqlCeParameter[] spParams1 = new SqlCeParameter[4];
                                    spParams1[0] = new SqlCeParameter("@cName", aFile.Name);
                                    spParams1[1] = new SqlCeParameter("@cPublisher", aFile.Publisher);
                                    spParams1[2] = new SqlCeParameter("@cProductName", aFile.ProductName);
                                    spParams1[3] = new SqlCeParameter("@cFVersion1", aFile.FileVersion1);

                                    command.Parameters.AddRange(spParams1);
                                    command.ExecuteNonQuery();
                                }

                                commandText =
                                    "DELETE FROM FS_FILES WHERE _FILEID = @fileAssignmentID";

                                using (SqlCeCommand command = new SqlCeCommand(commandText, conn))
                                {
                                    command.Parameters.AddWithValue("@fileAssignmentID", lFileAssignmentId);
                                    command.ExecuteNonQuery();
                                }

                                commandText =
                                    "SELECT MIN(_APPLICATIONID) FROM APPLICATIONS " +
                                    "WHERE _ASSIGNED_FILEID = @fileAssignmentID";

                                using (SqlCeCommand command = new SqlCeCommand(commandText, conn))
                                {
                                    command.Parameters.AddWithValue("@fileAssignmentID", lFileAssignmentId);
                                    object result = command.ExecuteScalar();

                                    if ((result != null) && (result.GetType() != typeof(DBNull)))
                                        lAssignedAppliationId = (int)result;
                                }

                                if (lAssignedAppliationId != -1)
                                {
                                    commandText =
                                        "DELETE FROM APPLICATION_INSTANCES WHERE _APPLICATIONID = @nAssignedApplicationID";

                                    using (SqlCeCommand command = new SqlCeCommand(commandText, conn))
                                    {
                                        command.Parameters.AddWithValue("@nAssignedApplicationID", lAssignedAppliationId);
                                        command.ExecuteNonQuery();
                                    }

                                    commandText =
                                        "DELETE FROM APPLICATIONS WHERE _APPLICATIONID = @nAssignedApplicationID";

                                    using (SqlCeCommand command = new SqlCeCommand(commandText, conn))
                                    {
                                        command.Parameters.AddWithValue("@nAssignedApplicationID", lAssignedAppliationId);
                                        command.ExecuteNonQuery();
                                    }
                                }
                            }
                        }
                        else
                        {
                            commandText =
                                "INSERT INTO FS_FILES " +
                                "(_NAME, _PUBLISHER,_PRODUCTNAME, _FILE_VERSION1) " +
                                "VALUES " +
                                "(@cName, @cPublisher, @cProductName, @cFVersion1)";

                            SqlCeParameter[] spParams = new SqlCeParameter[4];
                            spParams[0] = new SqlCeParameter("@cName", aFile.Name);
                            spParams[1] = new SqlCeParameter("@cPublisher", aFile.Publisher);
                            spParams[2] = new SqlCeParameter("@cProductName", aFile.ProductName);
                            spParams[3] = new SqlCeParameter("@cFVersion1", aFile.FileVersion1);

                            using (SqlCeCommand command = new SqlCeCommand(commandText, conn))
                            {
                                command.Parameters.AddRange(spParams);
                                command.ExecuteNonQuery();
                            }

                            using (SqlCeCommand command = new SqlCeCommand("SELECT @@IDENTITY", conn))
                            {
                                lItemID = Convert.ToInt32(command.ExecuteScalar());
                            }

                            commandText =
                                "UPDATE APPLICATIONS SET _ASSIGNED_FILEID = @nReturnID WHERE _APPLICATIONID = @nApplicationID";

                            using (SqlCeCommand command = new SqlCeCommand(commandText, conn))
                            {
                                command.Parameters.AddWithValue("@nReturnID", lItemID);
                                command.Parameters.AddWithValue("@nApplicationID", aFile.AssignApplicationID);

                                command.ExecuteNonQuery();
                            }

                            commandText =
                                "UPDATE FS_FILES SET _ASSIGN_APPLICATIONID = @nApplicationID " +
                                "WHERE _NAME = @cName " +
                                "AND _PUBLISHER = @cPublisher " +
                                "AND _PRODUCTNAME = @cProductName " +
                                "AND _FILE_VERSION1 = @cFVersion1";

                            using (SqlCeCommand command = new SqlCeCommand(commandText, conn))
                            {
                                SqlCeParameter[] spParams1 = new SqlCeParameter[4];
                                spParams1[0] = new SqlCeParameter("@cName", aFile.Name);
                                spParams1[1] = new SqlCeParameter("@cPublisher", aFile.Publisher);
                                spParams1[2] = new SqlCeParameter("@cProductName", aFile.ProductName);
                                spParams1[3] = new SqlCeParameter("@cFVersion1", aFile.FileVersion1);

                                command.Parameters.AddRange(spParams1);
                                command.Parameters.AddWithValue("@nApplicationID", aFile.AssignApplicationID);

                                command.ExecuteNonQuery();
                            }

                            commandText =
                                "SELECT _ASSETID FROM FS_FILES WHERE _ASSIGN_APPLICATIONID = @nApplicationID";

                            using (SqlCeCommand command = new SqlCeCommand(commandText, conn))
                            {
                                command.Parameters.AddWithValue("@nApplicationID", aFile.AssignApplicationID);

                                using (SqlCeDataReader reader = command.ExecuteReader())
                                {
                                    while (reader.Read())
                                    {
                                        int lAssetId = reader.GetInt32(0);

                                        if (lAssetId != 0)
                                        {
                                            ApplicationsDAO applicationDAO = new ApplicationsDAO();
                                            applicationDAO.AddApplicationInstanceLocal(lAssetId, String.Empty, String.Empty, conn, aFile.AssignApplicationID);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                else
                {
                    using (SqlConnection conn = DatabaseConnection.CreateOpenStandardConnection())
                    {
                        string commandText = String.Empty;
                        int lFileAssignmentId = -1;
                        int lAssignedAppliationId = -1;

                        if (aFile.AssignApplicationID == 0)
                        {
                            commandText =
                                "SELECT MIN(_FILEID) FROM FS_FILES " +
                                "WHERE _NAME = @cName " +
                                "AND _PUBLISHER = @cPublisher " +
                                "AND _PRODUCTNAME = @cProductName " +
                                "AND _FILE_VERSION1 = @cFVersion1 " +
                                "AND _ASSETID = 0";

                            SqlParameter[] spParams = new SqlParameter[4];
                            spParams[0] = new SqlParameter("@cName", aFile.Name);
                            spParams[1] = new SqlParameter("@cPublisher", aFile.Publisher);
                            spParams[2] = new SqlParameter("@cProductName", aFile.ProductName);
                            spParams[3] = new SqlParameter("@cFVersion1", aFile.FileVersion1);

                            using (SqlCommand command = new SqlCommand(commandText, conn))
                            {
                                command.Parameters.AddRange(spParams);

                                object result = command.ExecuteScalar();
                                if ((result != null) && (result.GetType() != typeof(DBNull)))
                                    lFileAssignmentId = (int)result;
                            }

                            if (lFileAssignmentId != -1)
                            {
                                commandText =
                                    "UPDATE FS_FILES SET _ASSIGN_APPLICATIONID = 0 " +
                                    "WHERE _NAME = @cName " +
                                    "AND _PUBLISHER = @cPublisher " +
                                    "AND _PRODUCTNAME = @cProductName " +
                                    "AND _FILE_VERSION1 = @cFVersion1";

                                using (SqlCommand command = new SqlCommand(commandText, conn))
                                {
                                    SqlParameter[] spParams1 = new SqlParameter[4];
                                    spParams1[0] = new SqlParameter("@cName", aFile.Name);
                                    spParams1[1] = new SqlParameter("@cPublisher", aFile.Publisher);
                                    spParams1[2] = new SqlParameter("@cProductName", aFile.ProductName);
                                    spParams1[3] = new SqlParameter("@cFVersion1", aFile.FileVersion1);

                                    command.Parameters.AddRange(spParams1);
                                    command.ExecuteNonQuery();
                                }

                                commandText =
                                    "DELETE FROM FS_FILES WHERE _FILEID = @fileAssignmentID";

                                using (SqlCommand command = new SqlCommand(commandText, conn))
                                {
                                    command.Parameters.AddWithValue("@fileAssignmentID", lFileAssignmentId);
                                    command.ExecuteNonQuery();
                                }

                                commandText =
                                    "SELECT MIN(_APPLICATIONID) FROM APPLICATIONS " +
                                    "WHERE _ASSIGNED_FILEID = @fileAssignmentID";

                                using (SqlCommand command = new SqlCommand(commandText, conn))
                                {
                                    command.Parameters.AddWithValue("@fileAssignmentID", lFileAssignmentId);
                                    object result = command.ExecuteScalar();

                                    if ((result != null) && (result.GetType() != typeof(DBNull)))
                                        lAssignedAppliationId = (int)result;
                                }

                                if (lAssignedAppliationId != -1)
                                {
                                    commandText =
                                        "DELETE FROM APPLICATION_INSTANCES WHERE _APPLICATIONID = @nAssignedApplicationID";

                                    using (SqlCommand command = new SqlCommand(commandText, conn))
                                    {
                                        command.Parameters.AddWithValue("@nAssignedApplicationID", lAssignedAppliationId);
                                        command.ExecuteNonQuery();
                                    }

                                    commandText =
                                        "DELETE FROM APPLICATIONS WHERE _APPLICATIONID = @nAssignedApplicationID";

                                    using (SqlCommand command = new SqlCommand(commandText, conn))
                                    {
                                        command.Parameters.AddWithValue("@nAssignedApplicationID", lAssignedAppliationId);
                                        command.ExecuteNonQuery();
                                    }
                                }
                            }
                        }
                        else
                        {
                            commandText =
                                "INSERT INTO FS_FILES " +
                                "(_NAME, _PUBLISHER,_PRODUCTNAME, _FILE_VERSION1) " +
                                "VALUES " +
                                "(@cName, @cPublisher, @cProductName, @cFVersion1)";

                            SqlParameter[] spParams = new SqlParameter[4];
                            spParams[0] = new SqlParameter("@cName", aFile.Name);
                            spParams[1] = new SqlParameter("@cPublisher", aFile.Publisher);
                            spParams[2] = new SqlParameter("@cProductName", aFile.ProductName);
                            spParams[3] = new SqlParameter("@cFVersion1", aFile.FileVersion1);

                            using (SqlCommand command = new SqlCommand(commandText, conn))
                            {
                                command.Parameters.AddRange(spParams);
                                command.ExecuteNonQuery();
                            }

                            using (SqlCommand command = new SqlCommand("SELECT @@IDENTITY", conn))
                            {
                                lItemID = Convert.ToInt32(command.ExecuteScalar());
                            }

                            commandText =
                                "UPDATE APPLICATIONS SET _ASSIGNED_FILEID = @nReturnID WHERE _APPLICATIONID = @nApplicationID";

                            using (SqlCommand command = new SqlCommand(commandText, conn))
                            {
                                command.Parameters.AddWithValue("@nReturnID", lItemID);
                                command.Parameters.AddWithValue("@nApplicationID", aFile.AssignApplicationID);

                                command.ExecuteNonQuery();
                            }

                            commandText =
                                "UPDATE FS_FILES SET _ASSIGN_APPLICATIONID = @nApplicationID " +
                                "WHERE _NAME = @cName " +
                                "AND _PUBLISHER = @cPublisher " +
                                "AND _PRODUCTNAME = @cProductName " +
                                "AND _FILE_VERSION1 = @cFVersion1";

                            using (SqlCommand command = new SqlCommand(commandText, conn))
                            {
                                SqlParameter[] spParams1 = new SqlParameter[4];
                                spParams1[0] = new SqlParameter("@cName", aFile.Name);
                                spParams1[1] = new SqlParameter("@cPublisher", aFile.Publisher);
                                spParams1[2] = new SqlParameter("@cProductName", aFile.ProductName);
                                spParams1[3] = new SqlParameter("@cFVersion1", aFile.FileVersion1);

                                command.Parameters.AddRange(spParams1);
                                command.Parameters.AddWithValue("@nApplicationID", aFile.AssignApplicationID);

                                command.ExecuteNonQuery();
                            }

                            commandText =
                                "SELECT _ASSETID FROM FS_FILES WHERE _ASSIGN_APPLICATIONID = @nApplicationID";

                            using (SqlCommand command = new SqlCommand(commandText, conn))
                            {
                                command.Parameters.AddWithValue("@nApplicationID", aFile.AssignApplicationID);

                                using (SqlDataReader reader = command.ExecuteReader())
                                {
                                    while (reader.Read())
                                    {
                                        int lAssetId = reader.GetInt32(0);

                                        if (lAssetId != 0)
                                        {
                                            ApplicationsDAO applicationDAO = new ApplicationsDAO();
                                            applicationDAO.AddApplicationInstanceLocal(lAssetId, String.Empty, String.Empty, conn, aFile.AssignApplicationID);
                                        }
                                    }
                                }
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

        /// <summary>
        /// Delete all orphaned file references
        /// </summary>
        /// <param name="assetID"></param>
        /// <returns></returns>
        public int FileSystemDeleteOrphans(int aAssetID)
        {
            if (isDebugEnabled) logger.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " in");

            string commandText = String.Format(
                "select ai._instanceid from application_instances ai " +
                "left join applications a on (ai._applicationid = a._applicationid) " +
                "where ai._assetid = {0} " +
                "and _assigned_fileid <> 0 " +
                "and ai._applicationid not in (select _assign_applicationid from fs_files where _assetid = {0})", aAssetID);

            try
            {
                if (isCompactDatabase)
                {
                    using (SqlCeConnection conn = DatabaseConnection.CreateOpenCEConnection())
                    {
                        using (SqlCeCommand command = new SqlCeCommand(commandText, conn))
                        {
                            using (SqlCeDataReader reader = command.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    new ApplicationInstanceDAO().ApplicationInstanceDeleteByInstanceId(reader.GetInt32(0));
                                }
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
                            using (SqlDataReader reader = command.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    new ApplicationInstanceDAO().ApplicationInstanceDeleteByInstanceId(reader.GetInt32(0));
                                }
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
            return 0;
        }

        #endregion File Systems (FS_FOLDER / FS_FILE) Table
    }
}
