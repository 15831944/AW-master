using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlServerCe;
using System.Windows.Forms;

namespace Layton.AuditWizard.DataAccess
{
    public class SettingsDAO
    {
        #region Data

        private string _connectionStringCompact;
        protected static readonly log4net.ILog Logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private static readonly bool IsDebugEnabled = Logger.IsDebugEnabled;
        private readonly bool _compactDatabaseType;
        private readonly Configuration _config = ConfigurationManager.OpenExeConfiguration(Path.Combine(Application.StartupPath, "AuditWizardv8.exe"));

        #endregion

        public SettingsDAO()
        {
            _compactDatabaseType = Convert.ToBoolean(_config.AppSettings.Settings["CompactDatabaseType"].Value);
            _connectionStringCompact = "Data Source=" + Application.StartupPath + Convert.ToString(_config.AppSettings.Settings["ConnectionStringCompact"].Value);
        }

        #region Settings Table

        public string GetSetting(string key, bool decrypt)
        {
            if (IsDebugEnabled) Logger.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " in");

            string returnSetting = String.Empty;

            try
            {
                const string commandText = "SELECT _VALUE FROM SETTINGS WHERE _KEY = @key";

                if (_compactDatabaseType)
                {
                    using (SqlCeConnection conn = DatabaseConnection.CreateOpenCEConnection())
                    {
                        using (SqlCeCommand command = new SqlCeCommand(commandText, conn))
                        {
                            command.Parameters.AddWithValue("@key", key);

                            object result = command.ExecuteScalar();
                            if ((result != null) && (result.GetType() != typeof(DBNull)))
                            {
                                string resultValue = Convert.ToString(result);
                                returnSetting = (decrypt) ? AES.Decrypt(resultValue) : resultValue;
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
                            command.Parameters.AddWithValue("@key", key);

                            object result = command.ExecuteScalar();
                            if ((result != null) && (result.GetType() != typeof(DBNull)))
                            {
                                string resultValue = Convert.ToString(result);
                                returnSetting = (decrypt) ? AES.Decrypt(resultValue) : resultValue;
                            }
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                Utility.DisplayErrorMessage("A database error has occurred in AuditWizard." + Environment.NewLine + Environment.NewLine +
                        "Please see the log file for further details.");
                Logger.Error("Exception in " + System.Reflection.MethodBase.GetCurrentMethod().Name, ex);
            }
            catch (SqlCeException ex)
            {
                Utility.DisplayErrorMessage("A database error has occurred in AuditWizard." + Environment.NewLine + Environment.NewLine +
                        "Please see the log file for further details.");
                Logger.Error("Exception in " + System.Reflection.MethodBase.GetCurrentMethod().Name, ex);
            }
            catch (Exception ex)
            {
                Utility.DisplayErrorMessage("A database error has occurred in AuditWizard." + Environment.NewLine + Environment.NewLine +
                        "Please see the log file for further details.");

                Logger.Error("Exception in " + System.Reflection.MethodBase.GetCurrentMethod().Name, ex);
            }

            if (IsDebugEnabled) Logger.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " out with setting : " + returnSetting);
            return returnSetting;
        }


        /// <summary>
        /// Recover a setting which is textual specifying a default value
        /// </summary>
        /// <param name="key"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public string GetSettingAsString(string key, string defaultValue)
        {
            string returnString = GetSetting(key, false);
            return (returnString == null) ? defaultValue : returnString;
        }


        /// <summary>
        /// Recover a setting which is textual specifying a default value
        /// </summary>
        /// <param name="key"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public bool GetSettingAsBoolean(string key, bool defaultValue)
        {
            bool returnValue = defaultValue;

            string value = GetSetting(key, false);
            if (value != String.Empty)
            {
                try
                {
                    returnValue = Convert.ToBoolean(value);
                }
                catch (FormatException)
                {
                    // we can ignore a format exception and return default value
                }
                catch (Exception ex)
                {
                    // log this error and return default value
                    Logger.Error(ex.Message);
                }
            }
            return returnValue;
        }


        public void SetSetting(string key, string value, bool encrypt)
        {
            if (IsDebugEnabled) Logger.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " in");

            if (value == null)
                value = String.Empty;

            int lCount = 0;

            try
            {
                string commandText = "SELECT COUNT(*) FROM SETTINGS WHERE _KEY = @key";

                if (_compactDatabaseType)
                {
                    using (SqlCeConnection conn = DatabaseConnection.CreateOpenCEConnection())
                    {
                        using (SqlCeCommand command = new SqlCeCommand(commandText, conn))
                        {
                            command.Parameters.AddWithValue("@key", key);
                            lCount = Convert.ToInt32(command.ExecuteScalar());
                        }

                        commandText = (lCount == 0)
                                          ? "INSERT INTO SETTINGS (_KEY,_VALUE) VALUES (@key ,@value)"
                                          : "UPDATE SETTINGS SET _VALUE = @value WHERE _KEY = @key";

                        using (SqlCeCommand command = new SqlCeCommand(commandText, conn))
                        {
                            command.Parameters.AddWithValue("@key", key);
                            command.Parameters.AddWithValue("@value", encrypt ? AES.Encrypt(value) : value);
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
                            command.Parameters.AddWithValue("@key", key);
                            lCount = Convert.ToInt32(command.ExecuteScalar());
                        }

                        commandText = (lCount == 0)
                                          ? "INSERT INTO SETTINGS (_KEY,_VALUE) VALUES (@key ,@value)"
                                          : "UPDATE SETTINGS SET _VALUE = @value WHERE _KEY = @key";

                        using (SqlCommand command = new SqlCommand(commandText, conn))
                        {
                            command.Parameters.AddWithValue("@key", key);
                            command.Parameters.AddWithValue("@value", encrypt ? AES.Encrypt(value) : value);
                            command.ExecuteNonQuery();
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                Utility.DisplayErrorMessage("A database error has occurred in AuditWizard." + Environment.NewLine + Environment.NewLine +
                        "Please see the log file for further details.");
                Logger.Error("Exception in " + System.Reflection.MethodBase.GetCurrentMethod().Name, ex);
            }
            catch (SqlCeException ex)
            {
                Utility.DisplayErrorMessage("A database error has occurred in AuditWizard." + Environment.NewLine + Environment.NewLine +
                        "Please see the log file for further details.");
                Logger.Error("Exception in " + System.Reflection.MethodBase.GetCurrentMethod().Name, ex);
            }
            catch (Exception ex)
            {
                Utility.DisplayErrorMessage("A database error has occurred in AuditWizard." + Environment.NewLine + Environment.NewLine +
                        "Please see the log file for further details.");

                Logger.Error("Exception in " + System.Reflection.MethodBase.GetCurrentMethod().Name, ex);
            }


            if (IsDebugEnabled) Logger.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " out");
        }

        public void SetSetting(string key, bool value)
        {
            SetSetting(key, value.ToString(), false);
        }


        /// <summary>
        /// Recover the current publisher filter (wrapper around the GetSetting function)
        /// </summary>
        /// <returns></returns>
        public string GetPublisherFilter()
        {
            return GetSetting("Publisher Filter", false);
        }


        /// <summary>
        /// Set the current publisher filter (wrapper around the SetSetting function)
        /// </summary>
        /// <returns></returns>
        public void SetPublisherFilter(string publisherFilter)
        {
            SetSetting("Publisher Filter", publisherFilter, false);
        }

        /// <summary>
        /// Updates the settings column, changing column type to ntext
        /// </summary>
        public void AlterValueColumnType()
        {
            string cmdText;

            try
            {
                if (_compactDatabaseType)
                {
                    using (SqlCeConnection conn = DatabaseConnection.CreateOpenCEConnection())
                    {
                        // drop the default first
                        cmdText = "ALTER TABLE SETTINGS ALTER COLUMN _VALUE DROP DEFAULT";

                        using (SqlCeCommand command = new SqlCeCommand(cmdText, conn))
                        {
                            try
                            {
                                command.ExecuteNonQuery();
                            }
                            catch (SqlCeException ex)
                            {
                                // a SqlCeException can just be logged as a DEBUG error
                                Logger.Debug(ex.Message);
                            }
                        }

                        // alter the column type
                        cmdText = "ALTER TABLE SETTINGS ALTER COLUMN _VALUE ntext NOT NULL";

                        using (SqlCeCommand command = new SqlCeCommand(cmdText, conn))
                        {
                            try
                            {
                                command.ExecuteNonQuery();
                            }
                            catch (SqlCeException ex)
                            {
                                // a SqlCeException can just be logged as a DEBUG error - likely to be because column is already ntext
                                Logger.Debug(ex.Message);
                            }
                        }

                        // re-establish the default value
                        cmdText = "ALTER TABLE SETTINGS ADD DEFAULT ('') FOR _VALUE";

                        using (SqlCeCommand command = new SqlCeCommand(cmdText, conn))
                        {
                            try
                            {
                                command.ExecuteNonQuery();
                            }
                            catch (SqlCeException ex)
                            {
                                // a SqlCeException can just be logged as a DEBUG error
                                Logger.Debug(ex.Message);
                            }
                        }
                    }
                }
                else
                {
                    string constraintName = "";
                    cmdText = "SELECT name FROM dbo.sysobjects WHERE type = 'D' AND name LIKE 'DF__SETTINGS___VALUE%'";

                    using (SqlConnection conn = DatabaseConnection.CreateOpenStandardConnection())
                    {
                        using (SqlCommand command = new SqlCommand(cmdText, conn))
                        {
                            object result = command.ExecuteScalar();
                            if ((result != null) && (result.GetType() != typeof(DBNull)))
                            {
                                constraintName = Convert.ToString(result);
                            }
                        }

                        if (constraintName != "")
                        {
                            cmdText = "ALTER TABLE SETTINGS DROP CONSTRAINT " + constraintName;

                            using (SqlCommand command = new SqlCommand(cmdText, conn))
                            {
                                command.ExecuteNonQuery();
                            }
                        }

                        cmdText = "ALTER TABLE SETTINGS ALTER COLUMN _VALUE ntext NOT NULL";

                        using (SqlCommand command = new SqlCommand(cmdText, conn))
                        {
                            try
                            {
                                command.ExecuteNonQuery();
                            }
                            catch (SqlException ex)
                            {
                                // a SqlCeException can just be logged as a DEBUG error - likely to be because column is already ntext
                                Logger.Debug(ex.Message);
                            }
                        }

                        cmdText = "ALTER TABLE SETTINGS ADD DEFAULT ('') FOR _VALUE";

                        using (SqlCommand command = new SqlCommand(cmdText, conn))
                        {
                            try
                            {
                                command.ExecuteNonQuery();
                            }
                            catch (SqlException ex)
                            {
                                // a SqlCeException can just be logged as a DEBUG error
                                Logger.Debug(ex.Message);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Debug((ex.Message));
            }
        }

        #endregion Settings Table
    }
}
