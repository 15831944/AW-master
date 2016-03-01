using System;
using System.Configuration;
using System.Data;
using System.Data.SqlServerCe;
using System.IO;
using System.Windows.Forms;

namespace Layton.AuditWizard.DataAccess
{
    public class UsersDAO
    {
        #region Data

        private string connectionStringCompact = null;
        protected static readonly log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private static readonly bool isDebugEnabled = logger.IsDebugEnabled;
        private bool compactDatabaseType = false;
        private Configuration config = ConfigurationManager.OpenExeConfiguration(Path.Combine(Application.StartupPath, "AuditWizardv8.exe"));

        #endregion

        public UsersDAO()
        {
            compactDatabaseType = Convert.ToBoolean(config.AppSettings.Settings["CompactDatabaseType"].Value);
            connectionStringCompact = "Data Source=" + Application.StartupPath + Convert.ToString(config.AppSettings.Settings["ConnectionStringCompact"].Value);
        }

        #region Users Table

        /// <summary>
        /// Return a table containing all of the Users which have been declared 
        /// </summary>
        /// <returns></returns>
        public DataTable GetUsers()
        {
            if (isDebugEnabled) logger.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " in");
            DataTable usersTable = new DataTable(TableNames.USERS);

            if (compactDatabaseType)
            {
                try
                {
                    using (SqlCeConnection conn = DatabaseConnection.CreateOpenCEConnection())
                    {
                        string commandText =
                            "SELECT USERS._USERID ,USERS._LOGIN ,USERS._FIRSTNAME ,USERS._LASTNAME ,USERS._ACCESSLEVEL ,USERS._ROOTLOCATION, " +
			                "LOCATIONS._FULLNAME ,LOCATIONS._NAME " +
		                    "FROM USERS " +
		                    "LEFT JOIN LOCATIONS ON (USERS._ROOTLOCATION = LOCATIONS._LOCATIONID)";

                        using (SqlCeCommand command = new SqlCeCommand(commandText, conn))
                        {
                            new SqlCeDataAdapter(command).Fill(usersTable);
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
                usersTable = lAuditWizardDataAccess.GetUsers();
            }

            if (isDebugEnabled) logger.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " out");
            return usersTable;
        }


        /// <summary>
        /// Add a new Application License to the database
        /// </summary>
        /// <param name="theLicenseType"></param>
        /// <returns></returns>
        public int UserAdd(User aUser)
        {
            if (isDebugEnabled) logger.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " in");
            int lItemID = 0;

            if (compactDatabaseType)
            {
                try
                {
                    int lUserCount = 0;

                    using (SqlCeConnection conn = DatabaseConnection.CreateOpenCEConnection())
                    {
                        string commandText =
                            "SELECT COUNT(_USERID) FROM USERS WHERE _LOGIN = @cLogin";

                        using (SqlCeCommand command = new SqlCeCommand(commandText, conn))
                        {
                            command.Parameters.AddWithValue("@cLogin", aUser.Logon);
                            lUserCount = Convert.ToInt32(command.ExecuteScalar());
                        }

                        if (lUserCount == 0)
                        {
                            commandText =
                                "INSERT INTO USERS " +
                                "(_LOGIN, _FIRSTNAME, _LASTNAME, _ACCESSLEVEL, _ROOTLOCATION) " +
                                "VALUES " +
                                "(@cLogin, @cFirstName, @cLastName, @nAccessLevel, @nRootLocation)";

                            SqlCeParameter[] spParams = new SqlCeParameter[5];
                            spParams[0] = new SqlCeParameter("@cLogin", aUser.Logon);
                            spParams[1] = new SqlCeParameter("@cFirstName", aUser.FirstName);
                            spParams[2] = new SqlCeParameter("@cLastName", aUser.LastName);
                            spParams[3] = new SqlCeParameter("@nAccessLevel", (int)aUser.AccessLevel);
                            spParams[4] = new SqlCeParameter("@nRootLocation", aUser.RootLocationID);

                            using (SqlCeCommand command = new SqlCeCommand(commandText, conn))
                            {
                                command.Parameters.AddRange(spParams);
                                command.ExecuteNonQuery();
                            }

                            using (SqlCeCommand commandReturnValue = new SqlCeCommand("SELECT @@IDENTITY", conn))
                            {
                                lItemID = Convert.ToInt32(commandReturnValue.ExecuteScalar());
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
                lItemID = lAuditWizardDataAccess.UserAdd(aUser);
            }

            if (isDebugEnabled) logger.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " out");
            return lItemID;
        }




        /// <summary>
        /// Set the password for the specified user
        /// </summary>
        /// <param name="theLicenseType"></param>
        /// <returns></returns>
        public void UserSetPassword(int aUserId, string aPassword)
        {
            if (isDebugEnabled) logger.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " in");

            if (compactDatabaseType)
            {
                try
                {
                    using (SqlCeConnection conn = DatabaseConnection.CreateOpenCEConnection())
                    {
                        string commandText =
                            "UPDATE USERS SET _PASSWORD = @cPassword WHERE _USERID = @nUserID";

                        SqlCeParameter[] spParams = new SqlCeParameter[2];
                        spParams[0] = new SqlCeParameter("@nUserId", aUserId);
                        spParams[1] = new SqlCeParameter("@cPassword", AES.Encrypt(aPassword));

                        using (SqlCeCommand command = new SqlCeCommand(commandText, conn))
                        {
                            command.Parameters.AddRange(spParams);
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
                lAuditWizardDataAccess.UserSetPassword(aUserId, aPassword);
            }

            if (isDebugEnabled) logger.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " out");
        }




        /// <summary>
        /// Check the username/password pair and recover the full details of the logged in user (if any)
        /// </summary>
        /// <returns></returns>
        public User UserCheckPassword(string aUsername, string aPassword)
        {
            if (isDebugEnabled) logger.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " in");
            User loggedUser = null;

            if (compactDatabaseType)
            {
                try
                {
                    using (SqlCeConnection conn = DatabaseConnection.CreateOpenCEConnection())
                    {
                        string commandText =
                            "SELECT _USERID FROM USERS WHERE _LOGIN = @cLogin AND _PASSWORD = @cPassword";

                        SqlCeParameter[] spParams = new SqlCeParameter[2];
                        spParams[0] = new SqlCeParameter("@cLogin", aUsername);
                        spParams[1] = new SqlCeParameter("@cPassword", AES.Encrypt(aPassword));

                        using (SqlCeCommand command = new SqlCeCommand(commandText, conn))
                        {
                            command.Parameters.AddRange(spParams);

                            object result = command.ExecuteScalar();

                            if ((result != null) && (result.GetType() != typeof(DBNull)))
                                loggedUser = GetUserDetails((int)result);
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
                loggedUser = lAuditWizardDataAccess.UserCheckPassword(aUsername, aPassword);
            }

            if (isDebugEnabled) logger.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " out");
            return loggedUser;
        }



        /// <summary>
        /// Return full details of the user with the specified ID
        /// </summary>
        /// <returns></returns>
        public User GetUserDetails(int aUserId)
        {
            if (isDebugEnabled) logger.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " in");
            User user = null;
            DataTable usersTable = new DataTable(TableNames.USERS);

            if (compactDatabaseType)
            {
                try
                {
                    using (SqlCeConnection conn = DatabaseConnection.CreateOpenCEConnection())
                    {
                        string commandText =
                            "SELECT USERS._USERID ,USERS._LOGIN ,USERS._FIRSTNAME ,USERS._LASTNAME ,USERS._ACCESSLEVEL ,USERS._ROOTLOCATION " +
			                ",LOCATIONS._FULLNAME ,LOCATIONS._NAME " +
			                "FROM USERS " +
			                "LEFT JOIN LOCATIONS ON (USERS._ROOTLOCATION = LOCATIONS._LOCATIONID) " +
			                "WHERE _USERID = @nUserId";

                        using (SqlCeCommand command = new SqlCeCommand(commandText, conn))
                        {
                            command.Parameters.AddWithValue("@nUserId", aUserId);
                            new SqlCeDataAdapter(command).Fill(usersTable);
                        }

                        // ...and use the first row returned to populate the user
                        if (usersTable.Rows.Count != 0)
                            user = new User(usersTable.Rows[0]);
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
                user = lAuditWizardDataAccess.GetUserDetails(aUserId);
            }

            if (isDebugEnabled) logger.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " out");
            return user;
        }


        /// <summary>
        /// Update the definition stored for the specified User
        /// </summary>
        /// <param name="theAsset"></param>
        /// <returns></returns>
        public int UserUpdate(User aUser)
        {
            if (isDebugEnabled) logger.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " in");

            if (compactDatabaseType)
            {
                try
                {
                    using (SqlCeConnection conn = DatabaseConnection.CreateOpenCEConnection())
                    {
                        string commandText =
                            "UPDATE USERS SET " +
                            "_LOGIN = @cLogin ,_FIRSTNAME = @cFirstName ,_LASTNAME = @cLastName " + 
                            ",_ACCESSLEVEL = @nAccessLevel ,_ROOTLOCATION = @nRootLocation " +
					        "WHERE _USERID = @nUserID";

                        SqlCeParameter[] spParams = new SqlCeParameter[6];
                        spParams[0] = new SqlCeParameter("@nUserID", aUser.UserID);
                        spParams[1] = new SqlCeParameter("@cLogin", aUser.Logon);
                        spParams[2] = new SqlCeParameter("@cFirstName", aUser.FirstName);
                        spParams[3] = new SqlCeParameter("@cLastName", aUser.LastName);
                        spParams[4] = new SqlCeParameter("@nAccessLevel", (int)aUser.AccessLevel);
                        spParams[5] = new SqlCeParameter("@nRootLocation", aUser.RootLocationID);

                        using (SqlCeCommand command = new SqlCeCommand(commandText, conn))
                        {
                            command.Parameters.AddRange(spParams);
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
                lAuditWizardDataAccess.UserUpdate(aUser);
            }

            if (isDebugEnabled) logger.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " out");
            return 0;
        }



        /// <summary>
        /// Delete the specified User from the database
        /// </summary>
        /// <param name="licenseID"></param>
        /// <returns></returns>
        public int UserDelete(User aUser)
        {
            if (isDebugEnabled) logger.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " in");

            if (compactDatabaseType)
            {
                // We cannot delete the last administrator so we had better ensure that if this is 
                // an administrator that there is at least 1 other
                if (aUser.AccessLevel == User.ACCESSLEVEL.administrator)
                {
                    int administratorCount = UserAdministratorCount();
                    if (administratorCount <= 1)
                        return -1;
                }

                if (aUser.UserID != 0)
                {
                    using (SqlCeConnection conn = DatabaseConnection.CreateOpenCEConnection())
                    {
                        try
                        {
                            string commandText =
                                "DELETE FROM USERS WHERE _USERID = @nUserID";

                            using (SqlCeCommand command = new SqlCeCommand(commandText, conn))
                            {
                                command.Parameters.AddWithValue("@nUserID", aUser.UserID);
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
            }
            else
            {
                AuditWizardDataAccess lAuditWizardDataAccess = new AuditWizardDataAccess();
                lAuditWizardDataAccess.UserDelete(aUser);
            }

            if (isDebugEnabled) logger.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " out");
            return 0;
        }


        /// <summary>
        /// Return a count of the number of administrators defined in the database
        /// </summary>
        /// <returns></returns>
        public int UserAdministratorCount()
        {
            DataTable usersTable = GetUsers();
            int administratorCount = 0;
            foreach (DataRow thisRow in usersTable.Rows)
            {
                User.ACCESSLEVEL accessLevel = (User.ACCESSLEVEL)thisRow["_ACCESSLEVEL"];
                if (accessLevel == User.ACCESSLEVEL.administrator)
                    administratorCount++;
            }
            return administratorCount;
        }


        /// <summary>
        /// Return the current security status
        /// </summary>
        /// <returns></returns>
        public bool SecurityStatus()
        {
            if (isDebugEnabled) logger.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " in");

            bool bEnabled = false;

            if (compactDatabaseType)
            {
                using (SqlCeConnection conn = DatabaseConnection.CreateOpenCEConnection())
                {
                    try
                    {
                        SettingsDAO settingsDAO = new SettingsDAO();
                        bEnabled = (settingsDAO.GetSetting("SecurityEnabled", false) == "True");
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
                bEnabled = lAuditWizardDataAccess.SecurityStatus();
            }

            if (isDebugEnabled) logger.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " out");
            return bEnabled;
        }


        /// <summary>
        /// This function sets whether or not security is enabled
        /// </summary>
        /// <param name="enable"></param>
        public void SecurityStatus(bool enable)
        {
            if (isDebugEnabled) logger.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " in");

            if (compactDatabaseType)
            {                
                try
                {
                    using (SqlCeConnection conn = DatabaseConnection.CreateOpenCEConnection())
                    {
                        SettingsDAO settingsDAO = new SettingsDAO();
                        settingsDAO.SetSetting("SecurityEnabled", enable);
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
                lAuditWizardDataAccess.SecurityStatus(enable);
            }

            if (isDebugEnabled) logger.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " out");
        }

        #endregion Users Table
    }
}
