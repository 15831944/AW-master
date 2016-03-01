using System;
using System.Configuration;
using System.Data;
using System.Data.SqlServerCe;
using System.Data.SqlClient;
using System.IO;
using System.Windows.Forms;

namespace Layton.AuditWizard.DataAccess
{
    public class NewsFeedDAO
    {
        #region Data

        private string connectionStringCompact = null;
        protected static readonly log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private static readonly bool isDebugEnabled = logger.IsDebugEnabled;
        private bool compactDatabaseType = false;
        private Configuration config = ConfigurationManager.OpenExeConfiguration(Path.Combine(Application.StartupPath, "AuditWizardv8.exe"));

        #endregion

        public NewsFeedDAO()
        {
            compactDatabaseType = Convert.ToBoolean(config.AppSettings.Settings["CompactDatabaseType"].Value);
            connectionStringCompact = "Data Source=" + Application.StartupPath + Convert.ToString(config.AppSettings.Settings["ConnectionStringCompact"].Value);
        }

        #region Helper Methods

        /// <summary>
        /// Return drilldown statistics for Asset Table
        /// </summary>
        /// <returns></returns>
        private DataTable PerformQuery(string aCommandText)
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

        #endregion

        public void PurgeNewsFeed()
        {
            try
            {
                if (compactDatabaseType)
                {
                    string commandText = String.Format(
                        "DELETE " +
                        "FROM NEWS_FEED " +
                        "WHERE ID NOT IN (SELECT TOP (250) ID FROM NEWS_FEED ORDER BY news_date DESC)");

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
                    string commandText = String.Format(
                        "DELETE " +
                        "FROM NEWS_FEED " +
                        "WHERE ID NOT IN (SELECT TOP 250 ID FROM NEWS_FEED ORDER BY news_date DESC)");

                    using (SqlConnection conn = DatabaseConnection.CreateOpenStandardConnection())
                    {
                        using (SqlCommand command = new SqlCommand(commandText, conn))
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

        //public void PurgeNewsFeed(int days)
        //{
        //    string commandText = String.Format(
        //        "delete " +
        //        "from news_feed " +
        //        "where (datediff(day, news_date, getdate()) >= {0})", days);

        //    try
        //    {
        //        if (compactDatabaseType)
        //        {
        //            using (SqlCeConnection conn = DatabaseConnection.CreateOpenCEConnection())
        //            {
        //                using (SqlCeCommand command = new SqlCeCommand(commandText, conn))
        //                {
        //                    command.ExecuteNonQuery();
        //                }
        //            }
        //        }
        //        else
        //        {
        //            using (SqlConnection conn = DatabaseConnection.CreateOpenStandardConnection())
        //            {
        //                using (SqlCommand command = new SqlCommand(commandText, conn))
        //                {
        //                    command.ExecuteNonQuery();
        //                }
        //            }
        //        }
        //    }
        //    catch (SqlException ex)
        //    {
        //        Utility.DisplayErrorMessage("A database error has occurred in AuditWizard." + Environment.NewLine + Environment.NewLine +
        //                "Please see the log file for further details.");
        //        logger.Error("Exception in " + System.Reflection.MethodBase.GetCurrentMethod().Name, ex);
        //    }
        //    catch (SqlCeException ex)
        //    {
        //        Utility.DisplayErrorMessage("A database error has occurred in AuditWizard." + Environment.NewLine + Environment.NewLine +
        //                "Please see the log file for further details.");
        //        logger.Error("Exception in " + System.Reflection.MethodBase.GetCurrentMethod().Name, ex);
        //    }
        //    catch (Exception ex)
        //    {
        //        Utility.DisplayErrorMessage("A database error has occurred in AuditWizard." + Environment.NewLine + Environment.NewLine +
        //                "Please see the log file for further details.");

        //        logger.Error("Exception in " + System.Reflection.MethodBase.GetCurrentMethod().Name, ex);
        //    }
        //}

        public void AddNewsFeed(int priority, string newsText)
        {
            string commandText = String.Format(
                "INSERT INTO NEWS_FEED " +
                "(news_text, news_date, priority) " +
                "VALUES (@newsText, @newsDate, @newsPriority)", newsText, DateTime.Now, priority);

            try
            {
                if (compactDatabaseType)
                {
                    using (SqlCeConnection conn = DatabaseConnection.CreateOpenCEConnection())
                    {
                        SqlCeParameter[] spParams = new SqlCeParameter[3];
                        spParams[0] = new SqlCeParameter("@newsText", newsText);
                        spParams[1] = new SqlCeParameter("@newsDate", DateTime.Now);
                        spParams[2] = new SqlCeParameter("@newsPriority", priority);

                        using (SqlCeCommand command = new SqlCeCommand(commandText, conn))
                        {
                            command.Parameters.AddRange(spParams);
                            command.ExecuteNonQuery();
                        }
                    }
                }
                else
                {
                    using (SqlConnection conn = DatabaseConnection.CreateOpenStandardConnection())
                    {
                        SqlParameter[] spParams = new SqlParameter[3];
                        spParams[0] = new SqlParameter("@newsText", newsText);
                        spParams[1] = new SqlParameter("@newsDate", DateTime.Now);
                        spParams[2] = new SqlParameter("@newsPriority", priority);

                        using (SqlCommand command = new SqlCommand(commandText, conn))
                        {
                            command.Parameters.AddRange(spParams);
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

        public DataTable GetNewsFeed()
        {
            return PerformQuery(
                "SELECT news_date as Date, news_text as Item " +
                "FROM NEWS_FEED");
        }

        public void CheckTablePresent()
        {
            string commandText =
                "SELECT COUNT(*) FROM INFORMATION_SCHEMA.TABLES " +
                "WHERE TABLE_NAME = 'NEWS_FEED'";

            if (PerformScalarQuery(commandText) == 0)
            {
                commandText =
                    "CREATE TABLE NEWS_FEED ( " +
                    "id int IDENTITY(1,1) NOT NULL , " +
                    "news_text nvarchar(500) NOT NULL , " +
                    "news_date datetime NOT NULL , " +
                    "priority int NOT NULL )";

                PerformQuery(commandText);

                commandText = "ALTER TABLE NEWS_FEED ADD CONSTRAINT PK_NEWS_FEED PRIMARY KEY (id)";
                PerformQuery(commandText);
            }
        }
    }
}
