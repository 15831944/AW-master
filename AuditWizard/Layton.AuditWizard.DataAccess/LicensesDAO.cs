using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlServerCe;
using System.IO;
using System.Windows.Forms;

namespace Layton.AuditWizard.DataAccess
{
    public class LicensesDAO
    {
        #region Data

        private string connectionStringCompact = null;
        protected static readonly log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private static readonly bool isDebugEnabled = logger.IsDebugEnabled;
        private bool compactDatabaseType = false;
        private Configuration config = ConfigurationManager.OpenExeConfiguration(Path.Combine(Application.StartupPath, "AuditWizardv8.exe"));

        #endregion

        public LicensesDAO()
        {
            compactDatabaseType = Convert.ToBoolean(config.AppSettings.Settings["CompactDatabaseType"].Value);
            connectionStringCompact = "Data Source=" + Application.StartupPath + Convert.ToString(config.AppSettings.Settings["ConnectionStringCompact"].Value);
        }

        #region Licenses Table

        /// <summary>
        /// Return a table containing all of the license which have been declared for the 
        /// specified application
        /// </summary>
        /// <param name="applicationID"></param>
        /// <returns></returns>
        public DataTable GetApplicationLicenses()
        {
            string commandText =
                        "SELECT LICENSES._LICENSEID, LICENSES._LICENSETYPEID ,LICENSES._COUNT " +
                        ",LICENSES._APPLICATIONID ,LICENSES._SUPPORTED, LICENSES._SUPPORT_EXPIRES ,LICENSES._SUPPORT_ALERTDAYS " +
                        ",LICENSES._SUPPORT_ALERTBYEMAIL ,LICENSES._SUPPORT_ALERTRECIPIENTS " +
                        ",LICENSES._SUPPLIERID " +
                        ",LICENSE_TYPES._NAME AS LICENSE_TYPES_NAME ,LICENSE_TYPES._COUNTED " +
                        ",APPLICATIONS._NAME AS APPLICATION_NAME " +
                        ",SUPPLIERS._NAME AS SUPPLIER_NAME " +
                        "FROM LICENSES " +
                            "LEFT JOIN LICENSE_TYPES ON (LICENSES._LICENSETYPEID = LICENSE_TYPES._LICENSETYPEID) " +
                            "LEFT JOIN APPLICATIONS ON (LICENSES._APPLICATIONID = APPLICATIONS._APPLICATIONID) " +
                            "LEFT JOIN SUPPLIERS ON (LICENSES._SUPPLIERID = SUPPLIERS._SUPPLIERID)";

            return PerformQuery(commandText);
        }

        /// <summary>
        /// Return a table containing all of the license which have been declared for the 
        /// specified application
        /// </summary>
        /// <param name="applicationID"></param>
        /// <returns></returns>
        public DataTable GetApplicationLicenses(int applicationID)
        {
            //if (isDebugEnabled) logger.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " in");

            DataTable licensesTable = new DataTable(TableNames.LICENSES);

            if (compactDatabaseType)
            {
                try
                {
                    //using (SqlCeConnection conn = DatabaseConnection.CreateOpenCEConnection())
                    //{
                        string commandText =
                            "SELECT LICENSES._LICENSEID, LICENSES._LICENSETYPEID ,LICENSES._COUNT " +
			                ",LICENSES._APPLICATIONID ,LICENSES._SUPPORTED, LICENSES._SUPPORT_EXPIRES ,LICENSES._SUPPORT_ALERTDAYS " +
			                ",LICENSES._SUPPORT_ALERTBYEMAIL ,LICENSES._SUPPORT_ALERTRECIPIENTS " +
			                ",LICENSES._SUPPLIERID " +
			                ",LICENSE_TYPES._NAME AS LICENSE_TYPES_NAME ,LICENSE_TYPES._COUNTED " +
			                ",APPLICATIONS._NAME AS APPLICATION_NAME " +
                            ",SUPPLIERS._NAME AS SUPPLIER_NAME " +
	                        "FROM LICENSES " +
		                        "LEFT JOIN LICENSE_TYPES ON (LICENSES._LICENSETYPEID = LICENSE_TYPES._LICENSETYPEID) " +
		                        "LEFT JOIN APPLICATIONS ON (LICENSES._APPLICATIONID = APPLICATIONS._APPLICATIONID) " +
		                        "LEFT JOIN SUPPLIERS ON (LICENSES._SUPPLIERID = SUPPLIERS._SUPPLIERID) " +
	                        "WHERE LICENSES._APPLICATIONID = @applicationID";

                        using (SqlCeCommand command = new SqlCeCommand(commandText, DatabaseConnection.OpenCeConnection()))
                        {
                            command.Parameters.AddWithValue("@applicationID", applicationID);
                            new SqlCeDataAdapter(command).Fill(licensesTable);
                        }
                    //}
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
                licensesTable = lAuditWizardDataAccess.GetApplicationLicenses(applicationID);
            }

            //if (isDebugEnabled) logger.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " out");
            return licensesTable;
        }

        public DataTable GetLicenseCountByApplicationId(int aApplicationId)
        {
            string cmdText =
                "SELECT SUM(l._count) " +
                "FROM LICENSES l " +
                "INNER JOIN APPLICATIONS a ON (a._applicationid = l._applicationid) " +
                "WHERE a._ignored = 0 " +
                "AND a._applicationid = " + aApplicationId + " " +
                "GROUP BY a._applicationid";

            return PerformQuery(cmdText);
        }

        public int GetNotCountedLicenseCountByApplicationId(int aApplicationId)
        {
            string cmdText = 
                "SELECT COUNT(*) " +
                "FROM license_types lt  " +
                "INNER JOIN licenses l ON (l._licensetypeid = lt._licensetypeid)  " +
                "WHERE lt._counted = 0 " +
                "AND l._applicationid = " + aApplicationId;

            return PerformScalarQuery(cmdText);
        }

        /// <summary>
        /// Return a table containing all of the license which have been declared for the 
        /// specified operating system
        /// </summary>
        /// <returns></returns>
        public DataTable GetOSLicenses(int osID)
        {
            if (isDebugEnabled) logger.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " in");

            DataTable dataTable = new DataTable(TableNames.LICENSES);

            if (compactDatabaseType)
            {
                try
                {
                    using (SqlCeConnection conn = DatabaseConnection.CreateOpenCEConnection())
                    {
                        string commandText =
                            "SELECT LICENSES._LICENSEID, LICENSES._LICENSETYPEID ,LICENSES._COUNT " +
                            ",LICENSES._APPLICATIONID ,LICENSES._SUPPORTED, LICENSES._SUPPORT_EXPIRES ,LICENSES._SUPPORT_ALERTDAYS " +
                            ",LICENSES._SUPPORT_ALERTBYEMAIL ,LICENSES._SUPPORT_ALERTRECIPIENTS " +
                            ",LICENSES._SUPPLIERID " +
                            ",LICENSE_TYPES._NAME AS LICENSE_TYPES_NAME ,LICENSE_TYPES._COUNTED " +
                            ",APPLICATIONS._NAME AS APPLICATION_NAME " +
                            ", SUPPLIERS._NAME AS SUPPLIER_NAME " +
                            "FROM LICENSES " +
                                "LEFT JOIN LICENSE_TYPES ON (LICENSES._LICENSETYPEID = LICENSE_TYPES._LICENSETYPEID) " +
                                "LEFT JOIN APPLICATIONS ON (LICENSES._APPLICATIONID = APPLICATIONS._APPLICATIONID) " +
                                "LEFT JOIN SUPPLIERS ON (LICENSES._SUPPLIERID = SUPPLIERS._SUPPLIERID) " +
                            "WHERE LICENSES._APPLICATIONID = @applicationID";

                        using (SqlCeCommand command = new SqlCeCommand(commandText, conn))
                        {
                            command.Parameters.AddWithValue("@applicationID", osID);
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
                dataTable = lAuditWizardDataAccess.GetOSLicenses(osID);
            }

            if (isDebugEnabled) logger.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " out");
            return dataTable;
        }


        /// <summary>
        /// Return a table containing all of the license types that have been declared 
        /// </summary>
        /// <returns></returns>
        public DataTable EnumerateLicenses(LicenseType aLicenseType)
        {
            if (isDebugEnabled) logger.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " in");

            DataTable licensesTable = new DataTable(TableNames.LICENSES);

            if (compactDatabaseType)
            {
                try
                {
                    using (SqlCeConnection conn = DatabaseConnection.CreateOpenCEConnection())
                    {
                        string commandText =
                            "SELECT _LICENSEID ,_LICENSETYPEID ,_APPLICATIONID ,_COUNT " +
			                "FROM LICENSES";

                        if (aLicenseType != null)
                            commandText += " WHERE _LICENSETYPEID = @nLicenseTypeID";

                        using (SqlCeCommand command = new SqlCeCommand(commandText, conn))
                        {
                            if (aLicenseType != null)
                                command.Parameters.AddWithValue("@nLicenseTypeID", aLicenseType.LicenseTypeID);

                            new SqlCeDataAdapter(command).Fill(licensesTable);
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
                licensesTable = lAuditWizardDataAccess.EnumerateLicenses(aLicenseType);
            }

            if (isDebugEnabled) logger.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " out");
            return licensesTable;
        }


        /// <summary>
        /// Add a new Application License to the database
        /// </summary>
        /// <returns></returns>
        public int LicenseAdd(ApplicationLicense aLicense)
        {
            if (isDebugEnabled) logger.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " in");

            int lItemID = 0;

            if (compactDatabaseType)
            {
                try
                {
                    using (SqlCeConnection conn = DatabaseConnection.CreateOpenCEConnection())
                    {
                        string commandText =
                            "INSERT INTO LICENSES " +
		                    "(_LICENSETYPEID ,_APPLICATIONID ,_COUNT ,_SUPPORTED ,_SUPPORT_EXPIRES " +
		                    ",_SUPPORT_ALERTDAYS ,_SUPPORT_ALERTBYEMAIL ,_SUPPORT_ALERTRECIPIENTS ,_SUPPLIERID) " +
	                        "VALUES " +
		                    "(@nLicenseTypeID, @nForApplicationID, @nCount, @bSupported " +
                            ",@dtSupportExpiry, @nSupportAlertDays, @bSupportAlertEmail, @cSupportAlertRecipients " +
		                    ",@nSupplierID)";

                        SqlCeParameter[] spParams = new SqlCeParameter[9];
                        spParams[0] = new SqlCeParameter("@nLicenseTypeID", aLicense.LicenseTypeID);
                        spParams[1] = new SqlCeParameter("@nForApplicationID", aLicense.ApplicationID);
                        spParams[2] = new SqlCeParameter("@nCount", aLicense.Count);
                        spParams[3] = new SqlCeParameter("@bSupported", Convert.ToInt32(aLicense.Supported));
                        spParams[4] = new SqlCeParameter("@dtSupportExpiry", aLicense.SupportExpiryDate);
                        spParams[5] = new SqlCeParameter("@nSupportAlertDays", aLicense.SupportAlertDays);
                        spParams[6] = new SqlCeParameter("@bSupportAlertEmail", Convert.ToInt32(aLicense.SupportAlertEmail));
                        spParams[7] = new SqlCeParameter("@cSupportAlertRecipients", aLicense.SupportAlertRecipients);
                        spParams[8] = new SqlCeParameter("@nSupplierID", aLicense.SupplierID);

                        using (SqlCeCommand command = new SqlCeCommand(commandText, conn))
                        {
                            command.Parameters.AddRange(spParams);
                            command.ExecuteNonQuery();
                        }

                        // then get id of newly inserted record
                        using (SqlCeCommand commandReturnValue = new SqlCeCommand("SELECT @@IDENTITY", conn))
                        {
                            lItemID = Convert.ToInt32(commandReturnValue.ExecuteScalar());
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
                lItemID = lAuditWizardDataAccess.LicenseAdd(aLicense);
            }

            if (isDebugEnabled) logger.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " out with id : " + lItemID);
            return lItemID;
        }


        /// <summary>
        /// Update the definition stored for the specified Application License
        /// </summary>
        /// <returns></returns>
        public int LicenseUpdate(ApplicationLicense aLicense)
        {
            if (isDebugEnabled) logger.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " in");

            if (compactDatabaseType)
            {
                try
                {
                    using (SqlCeConnection conn = DatabaseConnection.CreateOpenCEConnection())
                    {
                        string commandText =
                            "UPDATE LICENSES SET _LICENSETYPEID = @nLicenseTypeID " + 
						    ",_APPLICATIONID = @nApplicationID " +
						    ",_COUNT = @nCount " +
						    ",_SUPPORTED = @bSupported " +
						    ",_SUPPORT_EXPIRES = @dtSupportExpiry " +
						    ",_SUPPORT_ALERTDAYS = @nSupportAlertDays " +
						    ",_SUPPORT_ALERTBYEMAIL = @bSupportAlertEmail " +
						    ",_SUPPORT_ALERTRECIPIENTS = @cSupportAlertRecipients " +
                            ",_SUPPLIERID = @nSupplierID " +
					        "WHERE _LICENSEID = @nLicenseID";

                        SqlCeParameter[] spParams = new SqlCeParameter[10];
                        spParams[0] = new SqlCeParameter("@nLicenseID", aLicense.LicenseID);
                        spParams[1] = new SqlCeParameter("@nLicenseTypeID", aLicense.LicenseTypeID);
                        spParams[2] = new SqlCeParameter("@nApplicationID", aLicense.ApplicationID);
                        spParams[3] = new SqlCeParameter("@nCount", aLicense.Count);
                        spParams[3] = new SqlCeParameter("@nCount", aLicense.Count);
                        spParams[4] = new SqlCeParameter("@bSupported", Convert.ToInt32(aLicense.Supported));
                        spParams[5] = new SqlCeParameter("@dtSupportExpiry", aLicense.SupportExpiryDate);
                        spParams[6] = new SqlCeParameter("@nSupportAlertDays", aLicense.SupportAlertDays);
                        spParams[7] = new SqlCeParameter("@bSupportAlertEmail", Convert.ToInt32( aLicense.SupportAlertEmail));
                        spParams[8] = new SqlCeParameter("@cSupportAlertRecipients", aLicense.SupportAlertRecipients);
                        spParams[9] = new SqlCeParameter("@nSupplierID", aLicense.SupplierID);

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
                lAuditWizardDataAccess.LicenseUpdate(aLicense);
            }

            if (isDebugEnabled) logger.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " out");
            return 0;
        }



        /// <summary>
        /// Delete the specified License from the database
        /// </summary>
        /// <param name="licenseID"></param>
        /// <returns></returns>
        public int LicenseDelete(ApplicationLicense aLicense)
        {
            if (isDebugEnabled) logger.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " in with id : " + aLicense.LicenseID);

            if (compactDatabaseType)
            {
                if (aLicense.LicenseID != 0)
                {
                    using (SqlCeConnection conn = DatabaseConnection.CreateOpenCEConnection())
                    {
                        try
                        {
                            string commandText =
                                "DELETE FROM LICENSES WHERE _LICENSEID = @nLicenseID";

                            using (SqlCeCommand command = new SqlCeCommand(commandText, conn))
                            {
                                command.Parameters.AddWithValue("@nLicenseID", aLicense.LicenseID);
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
                lAuditWizardDataAccess.LicenseDelete(aLicense);
            }

            if (isDebugEnabled) logger.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " out");
            return 0;
        }



        /// <summary>
        /// Return a table containing the definitions for ALL Support Contracts defined within the database
        /// </summary>
        /// <returns></returns>
        public DataTable GetSupportContracts()
        {
            if (isDebugEnabled) logger.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " in");

            DataTable supportTable = new DataTable(TableNames.LICENSES);

            if (compactDatabaseType)
            {
                try
                {
                    using (SqlCeConnection conn = DatabaseConnection.CreateOpenCEConnection())
                    {
                        string commandText =
                            "SELECT LICENSES._SUPPORT_EXPIRES,LICENSES._SUPPORT_ALERTDAYS ,LICENSES._SUPPORT_ALERTBYEMAIL " +
		                    ",APPLICATIONS._NAME " +
	                        "FROM LICENSES " +
                            "LEFT JOIN APPLICATIONS ON (LICENSES._APPLICATIONID = APPLICATIONS._APPLICATIONID) " +
	                        "WHERE LICENSES._SUPPORTED = 1";

                        using (SqlCeCommand command = new SqlCeCommand(commandText, conn))
                        {
                            new SqlCeDataAdapter(command).Fill(supportTable);
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
                supportTable = lAuditWizardDataAccess.GetSupportContracts();
            }

            if (isDebugEnabled) logger.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " out");
            return supportTable;
        }

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
            int returnValue = -1;

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

        #endregion Licenses Table
    }
}
