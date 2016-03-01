using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration;
using System.Data;
using Layton.AuditWizard.DataAccess;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace DBUtility
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {                
                
                //Read settings from app config file
                AppConfigFileHandler objAppConfigFileHandler = new AppConfigFileHandler(@"AuditWizardv8.exe.config");
                KeyValueConfigurationCollection ApplicationSettings = objAppConfigFileHandler.AppSettings;

                SqlConnectionStringBuilder cb = new SqlConnectionStringBuilder();

                bool bIsCompactDatabase = Convert.ToBoolean(ApplicationSettings["CompactDatabaseType"].Value);

                if (bIsCompactDatabase)
                {
                    string strConnectionStringCompact = "Data Source=" + Application.StartupPath + Convert.ToString(ApplicationSettings["ConnectionStringCompact"].Value +
                   "; Max Database Size = 1024; Max Buffer Size = 2048");

                   string strCreateQuery = "CREATE TABLE ASSET_SUPPORTCONTRACT (_CONTRACT_NUMBER nvarchar(25) NOT NULL, " +
                   "_CONTRACT_VALUE int NOT NULL , _SUPPORT_EXPIRY datetime NOT NULL , _ALERTFLAG  bit NOT NULL DEFAULT ((0)) , " +
                   "_NOOFDAYS int NOT NULL , _ALERTBYEMAIL  bit NOT NULL DEFAULT ((0)) , _NOTES nvarchar(200) NULL , _ASSETID int NOT NULL  PRIMARY KEY," +
                   "_SUPPLIERID int NOT NULL)";
                   DatabaseUtils objDatabaseUtils = new DatabaseUtils();
                   objDatabaseUtils.SetConnectionStringCompact(strConnectionStringCompact);
                   objDatabaseUtils.ExecuteCEQuery(strCreateQuery);
                }
                else
                {
                    bool bIsWindowsAuthentication = Convert.ToBoolean(ApplicationSettings["ConnectionStringExpressIntegratedSecurity"].Value);
                    cb.DataSource = ApplicationSettings["ConnectionStringExpressDataSource"].Value;
                    cb.InitialCatalog = ApplicationSettings["ConnectionStringExpressInitialCatalog"].Value;
                    string strCreateQuery = "CREATE TABLE [dbo].[ASSET_SUPPORTCONTRACT] ([_CONTRACT_NUMBER] [varchar](25) NOT NULL, " +
                    "[_CONTRACT_VALUE] [int] NOT NULL , [_SUPPORT_EXPIRY] [datetime] NOT NULL , [_ALERTFLAG]  [bit] NOT NULL DEFAULT ((0)) , " +
                    "[_NOOFDAYS] [int] NOT NULL , [_ALERTBYEMAIL]  [bit] NOT NULL DEFAULT ((0)) , [_NOTES] [varchar](200) NULL , [_ASSETID] [int] NOT NULL ," +
                    "[_SUPPLIERID] [int] NOT NULL, CONSTRAINT [PK_ASSET_SUPPORTCONTRACT] PRIMARY KEY CLUSTERED ( [_ASSETID] ASC ) ON [PRIMARY] ) ON [PRIMARY]";



                    if (bIsWindowsAuthentication)
                    {
                        string strConnectionString = cb.ToString() + ";Integrated Security=SSPI";
                        DatabaseUtils objDatabaseUtils = new DatabaseUtils();
                        objDatabaseUtils.SetConnectionString(strConnectionString);
                        objDatabaseUtils.ExceuteQuery(strCreateQuery);
                        objDatabaseUtils.ExecuteStoredProcedureCreateScript(Application.StartupPath + @"\db\addedprocedures.sql");                        
                    }
                    else
                    {

                        cb.UserID = ApplicationSettings["ConnectionStringExpressUserID"].Value;
                        cb.Password = AES.Decrypt(ApplicationSettings["ConnectionStringExpressPassword"].Value);
                        string strConnectionString = cb.ToString();
                        DatabaseUtils objDatabaseUtils = new DatabaseUtils();
                        objDatabaseUtils.SetConnectionString(strConnectionString);
                        objDatabaseUtils.ExceuteQuery(strCreateQuery);
                        objDatabaseUtils.ExecuteStoredProcedureCreateScript(Application.StartupPath + @"\db\addedprocedures.sql");

                    }
                }
            }
            catch (Exception Ex)
            {
                Console.WriteLine(Ex.Message.ToString());
                throw Ex;
            }
        }
    }
}
