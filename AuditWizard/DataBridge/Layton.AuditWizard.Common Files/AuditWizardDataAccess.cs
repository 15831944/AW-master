using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Globalization;
using System.Windows.Forms;

using Layton.Common.Controls;

namespace Layton.AuditWizard.Common
{
	// The 'Scope' is used by a number of different items which can be associated with either one
	// of a number of different objects such as Notes and Documents
	public enum SCOPE { Asset, Application, License, Application_Instance };

#region SmartReader Definition

	// SmartReader Class
	// =================
	//
	//    Allows more controlled reading of data from a SQL Record set avoiding null data issues and providing
	//    a degree of default handling
	//
	public sealed class SmartDataReader
	{
		private DateTime defaultDate;
		public SmartDataReader(SqlDataReader reader)
		{
			this.defaultDate = DateTime.MinValue;
			this.reader = reader;
		}

		public int GetInt32(String column)
		{
			int nColumn = reader.GetOrdinal(column);
			int data = (reader.IsDBNull(nColumn)) ? 0 : (int)reader[column];
			return data;
		}

		public short GetInt16(String column)
		{
			short data = (reader.IsDBNull(reader.GetOrdinal(column)))
								  ? (short)0 : (short)reader[column];
			return data;
		}

		public float GetFloat(String column)
		{
			float data = (reader.IsDBNull(reader.GetOrdinal(column)))
						? 0 : float.Parse(reader[column].ToString());
			return data;
		}

		public bool GetBoolean(String column)
		{
			bool data = (reader.IsDBNull(reader.GetOrdinal(column)))
									 ? false : (bool)reader[column];
			return data;
		}

		public String GetString(String column)
		{
			String data = (reader.IsDBNull(reader.GetOrdinal(column)))
								   ? null : reader[column].ToString();
			return data;
		}

		public DateTime GetDateTime(String column)
		{
			DateTime data = (reader.IsDBNull(reader.GetOrdinal(column)))
							   ? defaultDate : (DateTime)reader[column];
			return data;
		}

		public bool Read()
		{
			return this.reader.Read();
		}
		private SqlDataReader reader;
	}

#endregion SmartReader Class

#region Stored Procedure Execution Class

	public class DbStoredProcedure
	{
		SqlConnection _dbConn;
		SProcList _sprocs;  //sproc parameter info cache
		DbParameterCollection _lastParams; //used by Param()

		public DbStoredProcedure(SqlConnection theConnection)
		{
			_dbConn = theConnection;
			_sprocs = new SProcList(this);
		}

		SqlCommand NewSProc(string procName)
		{
			SqlCommand cmd = new SqlCommand(procName, _dbConn);
			cmd.CommandType = CommandType.StoredProcedure;

#if EmulateDeriveParameters   //see below for our 
                              //own DeriveParameters
            MySqlCmdBuilder.DeriveParameters(cmd);
#else
			SqlCommandBuilder.DeriveParameters(cmd);
			//SQL treats OUT params as REF params 
			//(thus requiring those parameters to be passed in)
			//if that's what you really want, remove 
			//the next three lines
			foreach (DbParameter prm in cmd.Parameters)
			{
				//make param a true OUT param
				if (prm.Direction == ParameterDirection.InputOutput)
					prm.Direction = ParameterDirection.Output;
			}
#endif

			return cmd;
		}

		SqlCommand FillParams(string procName, params object[] vals)
		{
			//get cached info (or cache if first call)
			SqlCommand cmd = _sprocs[procName];

			//fill parameter values for stored procedure call
			int i = 0;
			foreach (DbParameter prm in cmd.Parameters)
			{
				//we got info for ALL the params - only fill the INPUT params
				if (prm.Direction == ParameterDirection.Input
				 || prm.Direction == ParameterDirection.InputOutput)
					prm.Value = vals[i++];
			}

			//for subsequent calls to Param()
			_lastParams = cmd.Parameters;
			return cmd;
		}

		//handy routine if you are in control of the input.
		//but if user input, vulnerable to sql injection attack
		public DataRowCollection QueryRows(string strQry)
		{
			DataTable dt = new DataTable();
			new SqlDataAdapter(strQry, _dbConn).Fill(dt);
			return dt.Rows;
		}

		public int ExecSProc(string procName, params object[] vals)
		{
			int retVal = -1;  //some error code

			try
			{
				SqlCommand cmd = FillParams(procName, vals);
				cmd.ExecuteNonQuery();
				retVal = (int)_lastParams[0].Value;
			}
			//any special handling for SQL-generated error here
			//catch (System.Data.SqlClient.SqlException esql) {}
			catch (Exception e)
			{
				string message = e.Message;
				//handle error
			}
			finally
			{
			}
			return retVal;
		}

		public DataSet ExecSProcDS(string procName, params object[] vals)
		{
			DataSet ds = new DataSet();

			try
			{
				new SqlDataAdapter(FillParams(procName, vals)).Fill(ds);
			}
			finally
			{
			}
			return ds;
		}

		//get parameter from most recent ExecSProc
		public object Param(string param)
		{
			return _lastParams[param].Value;
		}

		class SProcList : DictionaryBase
		{
			DbStoredProcedure _db;
			public SProcList(DbStoredProcedure db)
			{ _db = db; }

			public SqlCommand this[string name]
			{
				get
				{      //read-only, "install on demand"
					if (!Dictionary.Contains(name))
						Dictionary.Add(name, _db.NewSProc(name));
					return (SqlCommand)Dictionary[name];
				}
			}
		}
	}

#endregion Stored Prodedure Execution Class

#region AuditWizardDataAccess Class

	public class AuditWizardDataAccess
	{
		#region Data

		private AuditWizardConnection _dbConnection = AuditWizardConnection.Instance;
		private string _lastError = "";

		/// <summary>This flag is used to inhibit the display of the connection box if we fail to open the 
		/// database.  This is required as we may not always have a UI to display the window</summary>
		private bool _showConnectionOnOpenError = true;

		#endregion Data

		#region Properties

		public AuditWizardConnection DatabaseConnection
		{
			get { return _dbConnection; }
		}

		// Methods
		public string ConnectionString
		{
			get { return _dbConnection.ConnectionString; }
			set { _dbConnection.ConnectionString = value; }
		}

		/// <summary>
		/// Inhibit or enable the display of the database connection form after an open error
		/// </summary>
		public bool ShowConnectionOnOpenError
		{
			get { return _showConnectionOnOpenError; }
			set { _showConnectionOnOpenError = value; }
		}

		public string LastError
		{
			get { return _lastError; }
			set { _lastError = value; }
		}

		#endregion Properties

		/// <summary>
		/// Create an open connection handling any errors by allowing the user to specify connection
		/// parameters
		/// </summary>
		/// <returns></returns>
		public SqlConnection CreateOpenConnection()
		{
			SqlConnection sqlConn = null;

			try
			{
				sqlConn = new SqlConnection(_dbConnection.ConnectionString);
				sqlConn.Open();
				return sqlConn;
			}
			
			catch (Exception ex)
			{
				// Save the exception error 
				_lastError = "Error in CreateOpenConnection : " + ex.Message;

				// If not inhibited, show the AuditWizard Database connection form
				// Application will exit if user closes form...
				if (!_showConnectionOnOpenError)
				{
					DatabaseConnectionForm connForm = new DatabaseConnectionForm();
					connForm.ShowDialog();
					sqlConn = new SqlConnection(_dbConnection.ConnectionString);
					sqlConn.Open();
				}
			}
			return sqlConn;
		}

		#region Version Table

		/// <summary>
		/// Recover the version information from the database
		/// </summary>
		/// <param name="assetID"></param>
		/// <param name="theApplication"></param>
		/// <returns></returns>
		public DataTable GetDatabaseVersion()
		{
			DataTable table = new DataTable("VERSION");

			SqlConnection conn = CreateOpenConnection();
			if (conn != null)
//			using (SqlConnection conn = CreateOpenConnection())
			{
				try
				{
					SqlCommand cmd = new SqlCommand("usp_get_version", conn);
					cmd.CommandType = CommandType.StoredProcedure;
					new SqlDataAdapter(cmd).Fill(table);
				}
				catch (SqlException ex)
				{
					this.HandleSqlError(ex);
				}
				finally
				{
					conn.Dispose();
					conn.Close();
				}
			}
			return table;
		}

		#endregion Version Table

		#region Applications tables

		/// <summary>
		/// Add a new application to the database
		/// </summary>
		/// <param name="assetID"></param>
		/// <param name="theApplication"></param>
		/// <returns></returns>
		public int ApplicationAdd(InstalledApplication theApplication)
		{
			int applicationID = 0;
			using (SqlConnection conn = CreateOpenConnection())
			{
				try
				{
					// Define parameters - note that we do not format the strings as the SP expects
					// them in their native format
					SqlParameter[] spParams = new SqlParameter[7];
					spParams[0] = new SqlParameter("@cPublisher", SqlDbType.VarChar ,255);
					spParams[0].Value = theApplication.Publisher;
					spParams[1] = new SqlParameter("@cApplication", SqlDbType.VarChar ,255);
					spParams[1].Value = theApplication.Name;
					spParams[2] = new SqlParameter("@cVersion", SqlDbType.VarChar, 255);
					spParams[2].Value = "";
					spParams[3] = new SqlParameter("@cGuid", SqlDbType.VarChar, 255);
					spParams[3].Value = "";
					spParams[4] = new SqlParameter("@nAliasedToID", SqlDbType.Int);
					spParams[4].Value = theApplication.AliasedToID;
					spParams[5] = new SqlParameter("@bUserDefined", SqlDbType.Bit);
					spParams[5].Value = theApplication.UserDefined;
					spParams[6] = new SqlParameter("@nReturnID", SqlDbType.Int);
					spParams[6].Direction = ParameterDirection.Output;

					// Execute the stored procedure
					SqlCommand cmd = new SqlCommand("usp_add_application", conn);
					cmd.CommandType = CommandType.StoredProcedure;
					cmd.Parameters.AddRange(spParams);
					cmd.ExecuteNonQuery();

					// Pickup the returned index of the inserted application (not application instance!)
					applicationID = (int)spParams[6].Value;
				}
				catch (SqlException ex)
				{
					this.HandleSqlError(ex);
				}
				finally
				{
					conn.Dispose();
					conn.Close();
				}
			}
			return applicationID;
		}


		/// <summary>
		/// Add a new application instance to the database
		/// </summary>
		/// <param name="assetID"></param>
		/// <param name="theApplication"></param>
		/// <returns></returns>
		public int ApplicationAddInstance(int assetID, ApplicationInstance theApplication, out int applicationID, out int instanceID)
		{
			applicationID = 0;
			instanceID = 0;
			using (SqlConnection conn = CreateOpenConnection())
			{
				try
				{
					// Define parameters - note that we do not format the strings as the SP expects
					// them in their native format
					SqlParameter[] spParams = new SqlParameter[9];
					spParams[0] = new SqlParameter("@nAssetID", SqlDbType.Int);
					spParams[0].Value = assetID;
					spParams[1] = new SqlParameter("@cPublisher", SqlDbType.VarChar);
					spParams[1].Value = theApplication.Publisher;
					spParams[1].Size = 255;
					spParams[2] = new SqlParameter("@cApplication", SqlDbType.VarChar);
					spParams[2].Value = theApplication.Name;
					spParams[2].Size = 255;
					spParams[3] = new SqlParameter("@cVersion", SqlDbType.VarChar);
					spParams[3].Value = theApplication.Version;
					spParams[3].Size = 255;
					spParams[4] = new SqlParameter("@cGuid", SqlDbType.VarChar);
					spParams[4].Value = theApplication.Guid;
					spParams[4].Size = 255;
					spParams[5] = new SqlParameter("@cProductID", SqlDbType.VarChar);
					spParams[5].Value = (theApplication.Serial == null) ? "" : theApplication.Serial.ProductId;
					spParams[5].Size = 255;
					spParams[6] = new SqlParameter("@cCDKey", SqlDbType.VarChar);
					spParams[6].Value = (theApplication.Serial == null) ? "" : theApplication.Serial.CdKey;
					spParams[6].Size = 255;
					spParams[7] = new SqlParameter("@nReturnApplicationID", SqlDbType.Int);
					spParams[7].Direction = ParameterDirection.Output;
					spParams[8] = new SqlParameter("@nReturnInstanceID", SqlDbType.Int);
					spParams[8].Direction = ParameterDirection.Output;

					// Execute the stored procedure
					SqlCommand cmd = new SqlCommand("usp_add_installed_application", conn);
					cmd.CommandType = CommandType.StoredProcedure;
					cmd.Parameters.AddRange(spParams);
					cmd.ExecuteNonQuery();

					// Pickup the returned index of the inserted application (not application instance!)
					applicationID = (int)spParams[7].Value;
					instanceID = (int)spParams[8].Value;
				}
				catch (SqlException ex)
				{
					this.HandleSqlError(ex);
				}
				finally
				{
					conn.Dispose();
					conn.Close();
				}
			}
			return 0;
		}

		/// <summary>
		/// Add a new application instance to the database
		/// </summary>
		/// <param name="assetID"></param>
		/// <param name="theApplication"></param>
		/// <returns></returns>
		public int OSAddInstance(int assetID, OSInstance theOS, out int applicationID, out int instanceID)
		{
			applicationID = 0;
			instanceID = 0;
			using (SqlConnection conn = CreateOpenConnection())
			{
				try
				{
					// Define parameters
					SqlParameter[] spParams = new SqlParameter[7];
					spParams[0] = new SqlParameter("@nAssetID", SqlDbType.Int);
					spParams[0].Value = assetID;
					spParams[1] = new SqlParameter("@cName", SqlDbType.VarChar ,255);
					spParams[1].Value = theOS.Name;
					spParams[2] = new SqlParameter("@cVersion", SqlDbType.VarChar ,255);
					spParams[2].Value = theOS.Version;
					spParams[3] = new SqlParameter("@cProductID", SqlDbType.VarChar ,255);
					spParams[3].Value = (theOS.Serial == null) ? "" : theOS.Serial.ProductId;
					spParams[4] = new SqlParameter("@cCDKey", SqlDbType.VarChar ,255);
					spParams[4].Value = (theOS.Serial == null) ? "" : theOS.Serial.CdKey;
					spParams[5] = new SqlParameter("@nReturnOSID", SqlDbType.Int);
					spParams[5].Direction = ParameterDirection.Output;
					spParams[6] = new SqlParameter("@nReturnInstanceID", SqlDbType.Int);
					spParams[6].Direction = ParameterDirection.Output;

					// Execute the stored procedure
					SqlCommand cmd = new SqlCommand("usp_add_installed_os", conn);
					cmd.CommandType = CommandType.StoredProcedure;
					cmd.Parameters.AddRange(spParams);
					cmd.ExecuteNonQuery();

					// Pickup the returned index of the inserted application (not application instance!)
					applicationID = (int)spParams[5].Value;
					instanceID = (int)spParams[6].Value;
				}
				catch (SqlException ex)
				{
					this.HandleSqlError(ex);
				}
				finally
				{
					conn.Dispose();
					conn.Close();
				}
			}
			return 0;
		}



		/// <summary>
		/// Flag an application as 'Ignored' in the database or clear this flag
		/// </summary>
		/// <param name="applicationID"></param>
		/// <param name="hide"></param>
		/// <returns></returns>
		public int ApplicationSetIgnored(int applicationID, bool ignore)
		{
			using (SqlConnection conn = CreateOpenConnection())
			{
				try
				{
					SqlParameter[] spParams = new SqlParameter[2];
					spParams[0] = new SqlParameter("@nApplicationID", SqlDbType.Int);
					spParams[0].Value = applicationID;
					spParams[1] = new SqlParameter("@bIgnore", SqlDbType.Bit);
					spParams[1].Value = ignore;

					// Execute the stored procedure
					SqlCommand cmd = new SqlCommand("usp_application_set_ignored", conn);
					cmd.CommandType = CommandType.StoredProcedure;
					cmd.Parameters.AddRange(spParams);
					cmd.ExecuteNonQuery();
				}
				catch (SqlException ex)
				{
					this.HandleSqlError(ex);
				}
				finally
				{
					conn.Dispose();
					conn.Close();
				}
			}
			return 0;
		}


		/// <summary>
		/// Return a count of the number of application instances in the database
		/// </summary>
		/// <returns></returns>
		public int ApplicationInstanceCount()
		{
			int count = 0;
			using (SqlConnection conn = CreateOpenConnection())
			{
				try
				{
					SqlParameter[] spParams = new SqlParameter[1];
					spParams[0] = new SqlParameter("@nReturnCount", SqlDbType.Int);
					spParams[0].Direction = ParameterDirection.ReturnValue;
					//
					SqlCommand cmd = new SqlCommand("usp_applicationinstance_getcount", conn);
					cmd.CommandType = CommandType.StoredProcedure;
					cmd.Parameters.AddRange(spParams);
					cmd.ExecuteNonQuery();
					count = (int)spParams[0].Value;
				}
				catch (SqlException e)
				{
					this.HandleSqlError(e);
				}
				finally
				{
					conn.Dispose();
					conn.Close();
				}
			}
			return count;
		}


		/// <summary>
		/// Return a list of all of the applications (for the specified publisher(s))
		/// </summary>
		/// <param name="forPublisher"></param>
		/// <param name="includeIgnored">false to not include application marked as Not-NotIgnore</param>
		/// <returns></returns>
		public DataTable GetApplications(string forPublishers, bool includeIncluded ,bool includeIgnore)
		{
			// Handle the publisher filter if specified - we need to sql format the string
			String sqlPublisherFilter = null;
			if (forPublishers == null || forPublishers == "")
				sqlPublisherFilter = "";
			else
				sqlPublisherFilter = BuildPublisherFilter(forPublishers);

			// ...then call the SP
			DataTable applicationsTable = new DataTable(TableNames.APPLICATIONS);
			using (SqlConnection conn = CreateOpenConnection())
			{
				try
				{
					SqlCommand cmd = new SqlCommand("usp_applications_enumerate", conn);
					cmd.CommandType = CommandType.StoredProcedure;
					cmd.Parameters.Add("@forPublisher", SqlDbType.VarChar);
					cmd.Parameters["@forPublisher"].Value = sqlPublisherFilter;
					cmd.Parameters.Add("@showAllPublishers", SqlDbType.Bit);
					cmd.Parameters["@showAllPublishers"].Value = (sqlPublisherFilter == "") ? 1 : 0;
					cmd.Parameters.Add("@showIncluded", SqlDbType.Bit);
					cmd.Parameters["@showIncluded"].Value = includeIncluded;
					cmd.Parameters.Add("@showIgnored", SqlDbType.Bit);
					cmd.Parameters["@showIgnored"].Value = includeIgnore;
					new SqlDataAdapter(cmd).Fill(applicationsTable);
				}
				catch (SqlException e)
				{
					this.HandleSqlError(e);
				}
				finally
				{
					conn.Dispose();
					conn.Close();
				}
				return applicationsTable;
			}
		}



		/// <summary>
		/// Return a list of all of the applications which have been aliased to another application
		/// </summary>
		/// <returns></returns>
		public DataTable GetAliasedApplications()
		{
			DataTable applicationsTable = new DataTable(TableNames.APPLICATIONS);
			using (SqlConnection conn = CreateOpenConnection())
			{
				try
				{
					SqlCommand cmd = new SqlCommand("usp_applications_enumerate_aliases", conn);
					cmd.CommandType = CommandType.StoredProcedure;
					new SqlDataAdapter(cmd).Fill(applicationsTable);
				}
				catch (SqlException e)
				{
					this.HandleSqlError(e);
				}
				finally
				{
					conn.Dispose();
					conn.Close();
				}
				return applicationsTable;
			}
		}


		/// <summary>
		/// Return a count of applications which have been aliased to the specified application
		/// </summary>
		/// <returns></returns>
		public int GetAliasCount(int applicationID)
		{
			int returnCount = 0;
			DataTable table = new DataTable(TableNames.APPLICATIONS);
			using (SqlConnection conn = CreateOpenConnection())
			{
				try
				{
					SqlParameter[] spParams = new SqlParameter[2];
					spParams[0] = new SqlParameter("@applicationId", SqlDbType.Int);
					spParams[0].Value = applicationID;
					spParams[1] = new SqlParameter("@nReturnID", SqlDbType.Int);
					spParams[1].Direction = ParameterDirection.ReturnValue;
					SqlCommand cmd = new SqlCommand("usp_application_alias_count", conn);
					cmd.CommandType = CommandType.StoredProcedure;
					cmd.Parameters.AddRange(spParams);
					new SqlDataAdapter(cmd).Fill(table);
					returnCount = (int)spParams[1].Value;
				}
				catch (SqlException e)
				{
					this.HandleSqlError(e);
				}
				finally
				{
					conn.Dispose();
					conn.Close();
				}
				return returnCount;
			}
		}


		/// <summary>
		/// Return the application definition with the specified ID
		/// </summary>
		/// <returns></returns>
		public InstalledApplication GetApplication(int applicationID)
		{
			// ...then call the SP
			InstalledApplication theApplication = null;
			//
			DataTable table = new DataTable(TableNames.APPLICATIONS);
			using (SqlConnection conn = CreateOpenConnection())
			{
				try
				{
					SqlParameter[] spParams = new SqlParameter[1];
					spParams[0] = new SqlParameter("@nApplicationId", SqlDbType.Int);
					spParams[0].Value = applicationID;
					SqlCommand cmd = new SqlCommand("usp_get_application", conn);
					cmd.CommandType = CommandType.StoredProcedure;
					cmd.Parameters.AddRange(spParams);
					new SqlDataAdapter(cmd).Fill(table);

					// ...and use the first row returned to populate the user
					if (table.Rows.Count != 0)
						theApplication = new InstalledApplication(table.Rows[0]);
				}
				catch (SqlException ex)
				{
					this.HandleSqlError(ex);
				}
				finally
				{
					conn.Dispose();
					conn.Close();
				}
			}
			return theApplication;
		}

		/// <summary>
		/// Return a list of all of the Operating Systems
		/// </summary>
		/// <returns></returns>
		public DataTable GetOperatingSystems()
		{
			DataTable dataTable = new DataTable(TableNames.APPLICATIONS);
			using (SqlConnection conn = CreateOpenConnection())
			{
				try
				{
					SqlCommand cmd = new SqlCommand("usp_os_enumerate", conn);
					cmd.CommandType = CommandType.StoredProcedure;
					new SqlDataAdapter(cmd).Fill(dataTable);
				}
				catch (SqlException e)
				{
					this.HandleSqlError(e);
				}
				finally
				{
					conn.Dispose();
					conn.Close();
				}
				return dataTable;
			}
		}



		/// <summary>
		/// Return a list of the applications that are installed on the specified asset optionally
		/// filtering this list so that only certain publishers are included
		/// </summary>
		/// <param name="forAsset"></param>
		/// <returns></returns>
		public DataTable GetInstalledOS(Asset forAsset)
		{
			// ...then call the SP
			DataTable applicationsTable = new DataTable(TableNames.APPLICATION_INSTANCES);
			using (SqlConnection conn = CreateOpenConnection())
			{

				try
				{
					SqlCommand cmd = new SqlCommand("usp_asset_getos", conn);
					cmd.CommandType = CommandType.StoredProcedure;
					cmd.Parameters.Add("@assetID", SqlDbType.Int);
					cmd.Parameters["@assetID"].Value = forAsset.AssetID;
					new SqlDataAdapter(cmd).Fill(applicationsTable);
				}
				catch (SqlException ex)
				{
					this.HandleSqlError(ex);
				}
				finally
				{
					conn.Dispose();
					conn.Close();
				}
			}
			return applicationsTable;
		}


		/// <summary>
		/// Return a list of the applications that are installed on the specified asset optionally
		/// filtering this list so that only certain publishers are included
		/// </summary>
		/// <param name="forAsset"></param>
		/// <returns></returns>
		public DataTable GetInstalledApplications(Asset forAsset, String forPublisher, bool showIncluded, bool showIgnored)
		{
			// Format any supplied publisher filter
			String sqlPublisherFilter = BuildPublisherFilter(forPublisher);

			// ...then call the SP
			DataTable applicationsTable = new DataTable(TableNames.APPLICATION_INSTANCES);
			using (SqlConnection conn = CreateOpenConnection())
			{

				try
				{
					SqlCommand cmd = new SqlCommand("usp_asset_applications_enumerate", conn);
					cmd.CommandType = CommandType.StoredProcedure;
					cmd.Parameters.Add("@assetID", SqlDbType.Int);
					cmd.Parameters["@assetID"].Value = forAsset.AssetID;
					cmd.Parameters.Add("@forPublisher", SqlDbType.VarChar);
					cmd.Parameters["@forPublisher"].Value = sqlPublisherFilter;
					cmd.Parameters.Add("@showIncluded", SqlDbType.Bit);
					cmd.Parameters["@showIncluded"].Value = showIncluded;
					cmd.Parameters.Add("@showIgnored", SqlDbType.Bit);
					cmd.Parameters["@showIgnored"].Value = showIgnored;
					//
					new SqlDataAdapter(cmd).Fill(applicationsTable);
				}
				catch (SqlException ex)
				{
					this.HandleSqlError(ex);
				}
				finally
				{
					conn.Dispose();
					conn.Close();
				}
			}
			return applicationsTable;
		}


		/// <summary>
		/// Return a list of the names of all available publishers.  This is made up from those defined in the 
		/// application configuration file and also those defined within the database
		/// </summary>
		/// <returns></returns>
		public List<string> GetAllPublisherNames()
		{
			List<string> listAvailablePublishers = new List<string>();

			// Read the publishers defined within the application configuration file
			ApplicationDefinitionsFile definitionsFile = new ApplicationDefinitionsFile();
			definitionsFile.EnumerateKeys(ApplicationDefinitionsFile.PUBLISHERS_SECTION, listAvailablePublishers);

			// ...and also add in any new publishers read from the database - we don't include None though
			DataTable publishersTable = GetPublishers("");

			// Merge any new publishers into the list
			foreach (DataRow thisRow in publishersTable.Rows)
			{
				string publisher = thisRow["_PUBLISHER"] as string;
				if ((publisher != "")
				 && (publisher != DataStrings.UNIDENIFIED_PUBLISHER)
				 && (!listAvailablePublishers.Contains(publisher)))
					listAvailablePublishers.Add(publisher);
			}

			return listAvailablePublishers;
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
			DataTable applicationsTable = new DataTable(TableNames.APPLICATIONS);

			// Before we call the stored procedure we need to handle the publisher filter if one has
			// been supplied.  
			publisherFilter = BuildPublisherFilter(publisherFilter);

			//
			using (SqlConnection conn = CreateOpenConnection())
			{
				try
				{
					SqlCommand cmd = new SqlCommand("usp_publishers_enumerate", conn);
					cmd.CommandType = CommandType.StoredProcedure;
					cmd.Parameters.Add("@publisherFilter", SqlDbType.VarChar);
					cmd.Parameters["@publisherFilter"].Value = publisherFilter;
					new SqlDataAdapter(cmd).Fill(applicationsTable);
				}
				catch (SqlException e)
				{
					this.HandleSqlError(e);
				}
				finally
				{
					conn.Dispose();
					conn.Close();
				}
			}
			return applicationsTable;
		}



		/// <summary>
		/// Delete all application instance records for the specified asset
		/// </summary>
		/// <param name="assetID"></param>
		/// <returns></returns>
		public int ApplicationDeleteOrphans()
		{
			using (SqlConnection conn = CreateOpenConnection())
			{
				try
				{

					// Execute the stored procedure
					SqlCommand cmd = new SqlCommand("usp_application_deleteorphans", conn);
					cmd.CommandType = CommandType.StoredProcedure;
					cmd.ExecuteNonQuery();
				}
				catch (SqlException ex)
				{
					this.HandleSqlError(ex);
				}
				finally
				{
					conn.Dispose();
					conn.Close();
				}
			}
			return 0;
		}



		/// <summary>
		/// Change the publisher for the specified application
		/// </summary>
		/// <param name="theAsset"></param>
		/// <returns></returns>
		public int ApplicationUpdatePublisher(int applicationID ,string publisher)
		{
			using (SqlConnection conn = CreateOpenConnection())
			{
				try
				{
					SqlCommand cmd = new SqlCommand("usp_application_changepublisher", conn);
					cmd.CommandType = CommandType.StoredProcedure;
					cmd.Parameters.Add("@applicationID", SqlDbType.Int);
					cmd.Parameters["@applicationID"].Value = applicationID;
					cmd.Parameters.Add("@publisher", SqlDbType.VarChar);
					cmd.Parameters["@publisher"].Value = publisher;
					cmd.ExecuteNonQuery();
				}
				catch (SqlException e)
				{
					this.HandleSqlError(e);
				}
				finally
				{
					conn.Dispose();
					conn.Close();
				}
			}
			return 0;
		}




		/// <summary>
		/// Set or Clear the alias for an application
		/// </summary>
		/// <param name="theAsset"></param>
		/// <returns></returns>
		public int ApplicationSetAlias(int applicationID, int aliasApplicationID)
		{
			using (SqlConnection conn = CreateOpenConnection())
			{
				try
				{
					SqlCommand cmd = new SqlCommand("usp_application_alias", conn);
					cmd.CommandType = CommandType.StoredProcedure;
					cmd.Parameters.Add("@applicationID", SqlDbType.Int);
					cmd.Parameters["@applicationID"].Value = applicationID;
					cmd.Parameters.Add("@aliasID", SqlDbType.Int);
					cmd.Parameters["@aliasID"].Value = aliasApplicationID;
					cmd.ExecuteNonQuery();
				}
				catch (SqlException e)
				{
					this.HandleSqlError(e);
				}
				finally
				{
					conn.Dispose();
					conn.Close();
				}
			}
			return 0;
		}

#endregion Applications table

		#region Assets Table

		/// <summary>
		/// Add a new Asset to the database
		/// </summary>
		/// <param name="theAsset"></param>
		/// <returns></returns>
		public int AssetAdd(Asset theAsset)
		{
			int itemID = 0;
			using (SqlConnection conn = CreateOpenConnection())
			{
				try
				{
					SqlParameter[] spParams = new SqlParameter[15];
					spParams[0] = new SqlParameter("@cUniqueID", SqlDbType.VarChar ,255);
					spParams[0].Value =  theAsset.UniqueID;
					spParams[1] = new SqlParameter("@cName", SqlDbType.VarChar ,255);
					spParams[1].Value = theAsset.Name.ToUpper();
					spParams[2] = new SqlParameter("@nLocationID", SqlDbType.Int);
					spParams[2].Value = theAsset.LocationID;
					spParams[3] = new SqlParameter("@nDomainID", SqlDbType.Int);
					spParams[3].Value = theAsset.DomainID;
					spParams[4] = new SqlParameter("@cIPAddress", SqlDbType.VarChar ,255);
					spParams[4].Value = theAsset.IPAddress;
					spParams[5] = new SqlParameter("@cMACAddress", SqlDbType.VarChar ,255);
					spParams[5].Value = theAsset.MACAddress;
					spParams[6] = new SqlParameter("@nAssetTypeID", SqlDbType.Int);
					spParams[6].Value = theAsset.AssetTypeID;
					spParams[7] = new SqlParameter("@cMake", SqlDbType.VarChar ,255);
					spParams[7].Value = theAsset.Make;
					spParams[8] = new SqlParameter("@cModel", SqlDbType.VarChar ,255);
					spParams[8].Value = theAsset.Model;
					spParams[9] = new SqlParameter("@cSerial", SqlDbType.VarChar ,255);
					spParams[9].Value = theAsset.SerialNumber;
					spParams[10] = new SqlParameter("@nStockStatus", SqlDbType.Int);
					spParams[10].Value = (int)theAsset.StockStatus;
					spParams[11] = new SqlParameter("@nParentAssetID", SqlDbType.Int);
					spParams[11].Value = (int)theAsset.ParentAssetID;
					spParams[12] = new SqlParameter("@nSupplierID", SqlDbType.Int);
					spParams[12].Value = (int)theAsset.SupplierID;
					spParams[13] = new SqlParameter("@bAlertsEnabled", SqlDbType.Int);
					spParams[13].Value = theAsset.AlertsEnabled;
					spParams[14] = new SqlParameter("@nReturnID", SqlDbType.Int);
					spParams[14].Direction = ParameterDirection.ReturnValue;
					//
					SqlCommand cmd = new SqlCommand("usp_asset_add", conn);
					cmd.CommandType = CommandType.StoredProcedure;
					cmd.Parameters.AddRange(spParams);
					cmd.ExecuteNonQuery();
					itemID = (int)spParams[14].Value;
				}
				catch(SqlException ex)
				{
					this.HandleSqlError(ex);
				}
				finally
				{
					conn.Dispose();
					conn.Close();
				}
			}
			return itemID;
		}



		/// <summary>
		/// Flag an asset as having been audited
		/// </summary>
		/// <param name="assetID"></param>
		/// <returns></returns>
		public int AssetAudited(int assetID ,DateTime dtlastAudit)
		{
			using (SqlConnection conn = CreateOpenConnection())
			{
				try
				{
					SqlParameter[] spParams = new SqlParameter[2];
					spParams[0] = new SqlParameter("@nAssetID", SqlDbType.Int);
					spParams[0].Value = assetID;
					spParams[1] = new SqlParameter("@dtDate", SqlDbType.DateTime);
					spParams[1].Value = dtlastAudit;
					SqlCommand cmd = new SqlCommand("usp_asset_set_lastauditdate", conn);
					cmd.CommandType = CommandType.StoredProcedure;
					cmd.Parameters.AddRange(spParams);
					cmd.ExecuteNonQuery();
				}
				catch (SqlException e)
				{
					this.HandleSqlError(e);
				}
				finally
				{
					conn.Dispose();
					conn.Close();
				}
			}
			return 0;
		}



		/// <summary>
		/// Flag an asset as having been audited
		/// </summary>
		/// <param name="assetID"></param>
		/// <returns></returns>
		public DataTable AlertMonitorExclusions()
		{
			DataTable table = new DataTable(TableNames.ASSETS);
			using (SqlConnection conn = CreateOpenConnection())
			{
				try
				{
					SqlCommand cmd = new SqlCommand("usp_asset_enumerate_alerts_disabled", conn);
					cmd.CommandType = CommandType.StoredProcedure;
					new SqlDataAdapter(cmd).Fill(table);
				}
				catch (SqlException e)
				{
					this.HandleSqlError(e);
				}
				finally
				{
					conn.Dispose();
					conn.Close();
				}
			}
			return table;
		}


		/// <summary>
		/// Return a count of the number of assets in the database.  Optionally filter the 
		/// assets to include one those which have been audited and/or are not hidden
		/// </summary>
		/// <param name="auditedOnly"></param>
		/// <param name="visibleOnly"></param>
		/// <returns></returns>
		public int AssetCount(bool auditedOnly, bool visibleOnly)
		{
			int count = 0;
			using (SqlConnection conn = CreateOpenConnection())
			{
				try
				{
					SqlParameter[] spParams = new SqlParameter[3];
					spParams[0] = new SqlParameter("@bAuditedOnly", SqlDbType.Bit);
					spParams[0].Value = auditedOnly;
					spParams[0].Direction = ParameterDirection.Input;
					spParams[1] = new SqlParameter("@bVisibleOnly", SqlDbType.Bit);
					spParams[1].Direction = ParameterDirection.Input;
					spParams[1].Value = visibleOnly;
					spParams[2] = new SqlParameter("@nReturnCount", SqlDbType.Int);
					spParams[2].Direction = ParameterDirection.ReturnValue;
					//
					SqlCommand cmd = new SqlCommand("usp_asset_getcount", conn);
					cmd.CommandType = CommandType.StoredProcedure;
					cmd.Parameters.AddRange(spParams);
					cmd.ExecuteNonQuery();
					count = (int)spParams[2].Value;
				}
				catch (SqlException e)
				{
					this.HandleSqlError(e);
				}
				finally
				{
					conn.Dispose();
					conn.Close();
				}
			}
			return count;
		}



		/// <summary>
		/// Return a count of the number of licensed assets in the database.  
		/// </summary>
		/// <returns></returns>
		public int LicensedAssetCount()
		{
			int count = 0;
			using (SqlConnection conn = CreateOpenConnection())
			{
				try
				{
					SqlParameter[] spParams = new SqlParameter[1];
					spParams[0] = new SqlParameter("@nReturnCount", SqlDbType.Int);
					spParams[0].Direction = ParameterDirection.ReturnValue;
					//
					SqlCommand cmd = new SqlCommand("usp_get_licensecount", conn);
					cmd.CommandType = CommandType.StoredProcedure;
					cmd.Parameters.AddRange(spParams);
					cmd.ExecuteNonQuery();
					count = (int)spParams[0].Value;
				}
				catch (SqlException e)
				{
					this.HandleSqlError(e);
				}
				finally
				{
					conn.Dispose();
					conn.Close();
				}
			}
			return count;
		}

		/// <summary>
		/// Delete the specified asset from the database
		/// </summary>
		/// <param name="theAsset"></param>
		/// <returns></returns>
		public int AssetDelete(Asset theAsset)
		{
			if (theAsset.AssetID != 0)
			{
				using (SqlConnection conn = CreateOpenConnection())
				{
					try
					{
						new DbStoredProcedure(conn).ExecSProc("usp_asset_delete", new object[] { theAsset.AssetID });
					}
					catch (SqlException e)
					{
						this.HandleSqlError(e);
					}
					finally
					{
						conn.Dispose();
						conn.Close();
					}
				}
			}
			return 0;
		}



		/// <summary>
		/// Wraper around the following routine to return the ID of the asset identified by its
		/// name and unique ID value
		/// </summary>
		/// <returns></returns>
		public int AssetFind(Asset theAsset)
		{
			return AssetFind(theAsset.Name, theAsset.UniqueID);
		}


		/// <summary>
		/// Return the database index of the specified asset - note that this is not as easy as may 
		/// first appear as we need to be able to handle re-naming of assets on the network
		/// 
		/// This is done using the 'unique id' field.  We first check to see if there are any existing instances
		/// of this unique id and if so we will recover the name of that instance and assume that if this name
		/// does not match the current audited name that the PC has been renamed.  We can therefore rename the
		/// asset in the database.
		/// 
		/// If the unique ID is not found then we assume that we have a new asset but first we will check for 
		/// an existing asset with the same name.  This is valid if the asset has only been discovered in 
		/// which case the uniqueid field will still be blank.  If however the same named asset is found with
		/// differening unique ids then we can assume that these are actually two distinct assets - this is 
		/// only really allowed if in different domains but we shall allow it
		/// 
		/// </summary>
		/// <param name="name"></param>
		/// <returns></returns>
		public int AssetFind(string assetName, string uniqueID)
		{
			int itemId = 0;
			using (SqlConnection conn = CreateOpenConnection())
			{
				try
				{
					SqlParameter[] spParams = new SqlParameter[3];
					spParams[0] = new SqlParameter("@cName", SqlDbType.VarChar);
					spParams[0].Value = assetName;
					spParams[0].Size = 255;
					spParams[1] = new SqlParameter("@cUniqueID", SqlDbType.VarChar);
					spParams[1].Value = uniqueID;
					spParams[1].Size = 255;
					spParams[2] = new SqlParameter("@nReturnId", SqlDbType.Int);
					spParams[2].Direction = ParameterDirection.ReturnValue;
					//
					SqlCommand cmd = new SqlCommand("usp_asset_find", conn);
					cmd.CommandType = CommandType.StoredProcedure;
					cmd.Parameters.AddRange(spParams);
					cmd.ExecuteNonQuery();
					itemId = (int)spParams[2].Value;
				}
				catch (SqlException e)
				{
					this.HandleSqlError(e);
				}
				finally
				{
					conn.Dispose();
					conn.Close();
				}
			}
			return itemId;
		}


		/// <summary>
		/// Return the details of the specified asset
		/// 
		/// </summary>
		/// <param name="name"></param>
		/// <returns></returns>
		public Asset AssetGetDetails(int assetID)
		{
			DataTable table = new DataTable(TableNames.ASSETS);
			Asset returnAsset = null;

			using (SqlConnection conn = CreateOpenConnection())
			{
				try
				{
					SqlCommand cmd = new SqlCommand("usp_asset_getdetails", conn);
					cmd.CommandType = CommandType.StoredProcedure;
					cmd.Parameters.Add("@nAssetID", SqlDbType.Int);
					cmd.Parameters["@nAssetID"].Value = assetID;
					new SqlDataAdapter(cmd).Fill(table);
				}
				catch (SqlException e)
				{
					this.HandleSqlError(e);
				}
				finally
				{
					conn.Dispose();
					conn.Close();
				}
			}

			// If we were returned any rows then create an asset from that returned
			if (table.Rows.Count == 1)
				returnAsset = new Asset(table.Rows[0]);
			return returnAsset;
		}



		/// <summary>
		/// Flag the specified asset as being hidden (or clear this flag)
		/// </summary>
		/// <param name="assetID"></param>
		/// <param name="hide"></param>
		/// <returns></returns>
		public int AssetHide(int assetID, bool hide)
		{
			using (SqlConnection conn = CreateOpenConnection())
			{
				try
				{
					new DbStoredProcedure(conn).ExecSProc("usp_asset_hide", new object[] { assetID, hide });
				}
				catch (SqlException e)
				{
					this.HandleSqlError(e);
				}
				finally
				{
					conn.Dispose();
					conn.Close();
				}
			}
			return 0;
		}


		/// <summary>
		/// Flag the specified asset as being hidden (or clear this flag)
		/// </summary>
		/// <param name="assetID"></param>
		/// <param name="hide"></param>
		/// <returns></returns>
		public int AssetRequestAudit(int assetID, bool setorclear)
		{
			using (SqlConnection conn = CreateOpenConnection())
			{
				try
				{
					SqlCommand cmd = new SqlCommand("usp_asset_requestaudit", conn);
					cmd.CommandType = CommandType.StoredProcedure;
					SqlDataAdapter adapter = new SqlDataAdapter(cmd);
					cmd.Parameters.Add("@nAssetID", SqlDbType.Int);
					cmd.Parameters["@nAssetID"].Value = assetID;
					cmd.Parameters.Add("@bSetOrClear", SqlDbType.Bit);
					cmd.Parameters["@bSetOrClear"].Value = setorclear;
					cmd.ExecuteNonQuery();
				}
				catch (SqlException e)
				{
					this.HandleSqlError(e);
				}
				finally
				{
					conn.Dispose();
					conn.Close();
				}
			}
			return 0;
		}


		/// <summary>
		/// Update the definition stored for the specified asset
		/// </summary>
		/// <param name="theAsset"></param>
		/// <returns></returns>
		public int AssetUpdate(Asset theAsset)
		{
			using (SqlConnection conn = CreateOpenConnection())
			{
				try
				{
					// Set up parameters
					string domain = (theAsset.Domain == "") ? "<unknown>" : theAsset.Domain.ToUpper();
					SqlParameter[] spParams = new SqlParameter[14];
					spParams[0] = new SqlParameter("@nAssetID", SqlDbType.VarChar ,255);
					spParams[0].Value =  theAsset.AssetID;
					spParams[1] = new SqlParameter("@cName", SqlDbType.VarChar ,255);
					spParams[1].Value = theAsset.Name;
					spParams[2] = new SqlParameter("@nLocationID", SqlDbType.Int);
					spParams[2].Value = theAsset.LocationID;
					spParams[3] = new SqlParameter("@nDomainID", SqlDbType.Int);
					spParams[3].Value = theAsset.DomainID;
					spParams[4] = new SqlParameter("@cIPAddress", SqlDbType.VarChar ,255);
					spParams[4].Value = theAsset.IPAddress;
					spParams[5] = new SqlParameter("@cMACAddress", SqlDbType.VarChar ,255);
					spParams[5].Value = theAsset.MACAddress;
					spParams[6] = new SqlParameter("@nAssetTypeID", SqlDbType.Int);
					spParams[6].Value = theAsset.AssetTypeID;
					spParams[7] = new SqlParameter("@cMake", SqlDbType.VarChar ,255);
					spParams[7].Value = theAsset.Make;
					spParams[8] = new SqlParameter("@cModel", SqlDbType.VarChar ,255);
					spParams[8].Value = theAsset.Model;
					spParams[9] = new SqlParameter("@cSerial", SqlDbType.VarChar, 255);
					spParams[9].Value = theAsset.SerialNumber;
					spParams[10] = new SqlParameter("@nParentAssetID", SqlDbType.Int);
					spParams[10].Value = theAsset.ParentAssetID;
					spParams[11] = new SqlParameter("@nSupplierID", SqlDbType.Int);
					spParams[11].Value = theAsset.SupplierID;
					spParams[12] = new SqlParameter("@nStockStatus", SqlDbType.Int);
					spParams[12].Value = (int)theAsset.StockStatus;
					spParams[13] = new SqlParameter("@bAlertsEnabled", SqlDbType.Int);
					spParams[13].Value = theAsset.AlertsEnabled;
					//
					SqlCommand cmd = new SqlCommand("usp_asset_update", conn);
					cmd.CommandType = CommandType.StoredProcedure;
					cmd.Parameters.AddRange(spParams);
					cmd.ExecuteNonQuery();
				}
				catch (SqlException e)
				{
					this.HandleSqlError(e);
				}
				finally
				{
					conn.Dispose();
					conn.Close();
				}
			}
			return 0;
		}



		/// <summary>
		/// Update the Deployment Status stored for the specified asset
		/// </summary>
		/// <param name="theAsset"></param>
		/// <returns></returns>
		public int AssetUpdateDeploymentStatus(Asset theAsset)
		{
			return AssetUpdateAssetStatus(theAsset.AssetID, theAsset.AgentStatus);
		}


		/// <summary>
		/// Update the Deployment Status stored for the specified asset
		/// </summary>
		/// <param name="theAsset"></param>
		/// <returns></returns>
		public int AssetUpdateAssetStatus(int assetID, Asset.AGENTSTATUS newStatus)
		{
			using (SqlConnection conn = CreateOpenConnection())
			{
				try
				{
					new DbStoredProcedure(conn).ExecSProc("usp_asset_update_agent_status"
										, new object[] { assetID, (int)newStatus });
				}
				catch (SqlException e)
				{
					this.HandleSqlError(e);
				}
				finally
				{
					conn.Dispose();
					conn.Close();
				}
			}
			return 0;
		}



		/// <summary>
		/// Update the Stock Status stored for the specified asset
		/// </summary>
		/// <param name="theAsset"></param>
		/// <returns></returns>
		public int AssetUpdateStockStatus(int assetID, Asset.STOCKSTATUS newStatus)
		{
			using (SqlConnection conn = CreateOpenConnection())
			{
				try
				{
					new DbStoredProcedure(conn).ExecSProc("usp_asset_update_stock_status"
										, new object[] { assetID, (int)newStatus });
				}
				catch (SqlException e)
				{
					this.HandleSqlError(e);
				}
				finally
				{
					conn.Dispose();
					conn.Close();
				}
			}
			return 0;
		}



		/// <summary>
		/// Return a table containing the definitions for assets within the specified group
		/// optionally filtering out those whose status does not match that requested
		/// </summary>
		/// <returns></returns>
		public DataTable GetAssets(int forGroupID, AssetGroup.GROUPTYPE groupType ,bool applyStates)
		{
			bool showStock = false;
			bool showInUse = true;
			bool showPending = false;
			bool showDisposed = false;

			// Are we applying states?  If not then request ALL states
			if (applyStates)
			{
				// Get the current 'show' flags
				Configuration config = ConfigurationManager.OpenExeConfiguration(Path.Combine(Application.StartupPath, "Layton.Cab.Shell.exe"));
				try
				{
					showStock = Convert.ToBoolean(config.AppSettings.Settings["ShowStock"].Value);
					showInUse = Convert.ToBoolean(config.AppSettings.Settings["ShowInUse"].Value);
					showPending = Convert.ToBoolean(config.AppSettings.Settings["ShowPending"].Value);
					showDisposed = Convert.ToBoolean(config.AppSettings.Settings["ShowDisposed"].Value);
				}
				catch (Exception)
				{ }
			}

			else
			{
				showStock = showInUse = showPending = showDisposed = true;
			}

			DataTable table = new DataTable(TableNames.ASSETS);
			using (SqlConnection conn = CreateOpenConnection())
			{
				try
				{
					SqlCommand cmd = new SqlCommand("usp_asset_enumerate", conn);
					cmd.CommandType = CommandType.StoredProcedure;

					// Depending on the Group type and ID passed in determine what values to use for the location and domain IDs
					cmd.Parameters.Add("@nLocationID", SqlDbType.Int);
					cmd.Parameters["@nLocationID"].IsNullable = true;
					if (groupType == AssetGroup.GROUPTYPE.userlocation)
					{
						if (forGroupID == 0)
							cmd.Parameters["@nLocationID"].Value = DBNull.Value;
						else
							cmd.Parameters["@nLocationID"].Value = forGroupID;
					}
					else
					{
						cmd.Parameters["@nLocationID"].Value = 0;
					}
					
					cmd.Parameters.Add("@nDomainID", SqlDbType.Int);
					cmd.Parameters["@nDomainID"].IsNullable = true;
					if (groupType == AssetGroup.GROUPTYPE.domain)
					{
						if (forGroupID == 0)
							cmd.Parameters["@nDomainID"].Value = DBNull.Value;
						else
							cmd.Parameters["@nDomainID"].Value = forGroupID;
					}
					else
					{
						cmd.Parameters["@nDomainID"].Value = 0;
					}

					cmd.Parameters.Add("@showStock", SqlDbType.Bit);
					cmd.Parameters["@showStock"].Value = showStock;
					cmd.Parameters.Add("@showInUse", SqlDbType.Bit);
					cmd.Parameters["@showInUse"].Value = showInUse;
					cmd.Parameters.Add("@showPending", SqlDbType.Bit);
					cmd.Parameters["@showPending"].Value = showPending;
					cmd.Parameters.Add("@showDisposed", SqlDbType.Bit);
					cmd.Parameters["@showDisposed"].Value = showDisposed;
					new SqlDataAdapter(cmd).Fill(table);
				}
				catch (SqlException ex)
				{
					this.HandleSqlError(ex);
				}
				finally
				{
					conn.Dispose();
					conn.Close();
				}
			}
			return table;
		}



		/// <summary>
		/// Return a table containing all of the Child Assets for an asset
		/// </summary>
		/// <returns></returns>
		public DataTable EnumerateChildAssets(int assetID)
		{
			DataTable table = new DataTable(TableNames.ASSETS);
			using (SqlConnection conn = CreateOpenConnection())
			{
				try
				{
					SqlParameter[] spParams = new SqlParameter[1];
					spParams[0] = new SqlParameter("@nAssetID", SqlDbType.Int);
					spParams[0].Value = assetID;
					SqlCommand cmd = new SqlCommand("usp_childasset_enumerate", conn);
					cmd.CommandType = CommandType.StoredProcedure;
					cmd.Parameters.AddRange(spParams);
					new SqlDataAdapter(cmd).Fill(table);
				}
				catch (SqlException ex)
				{
					this.HandleSqlError(ex);
				}
				finally
				{
					conn.Dispose();
					conn.Close();
				}
			}
			return table;
		}


#endregion Assets Table

		#region Application_instance Table

		/// <summary>
		/// Return a table of the intances of the specified application
		/// </summary>
		/// <param name="applicationID"></param>
		/// <returns></returns>
		public DataTable GetApplicationInstances(int applicationID)
		{
			DataTable applicationsTable = new DataTable(TableNames.APPLICATION_INSTANCES);
			using (SqlConnection conn = CreateOpenConnection())
			{
				try
				{
					SqlCommand cmd = new SqlCommand("usp_applicationinstance_enumerate", conn);
					cmd.CommandType = CommandType.StoredProcedure;
					cmd.Parameters.Add("@applicationID", SqlDbType.Int);
					cmd.Parameters["@applicationID"].Value = applicationID;
					new SqlDataAdapter(cmd).Fill(applicationsTable);
				}
				catch (SqlException ex)
				{
					this.HandleSqlError(ex);
				}
				finally
				{
					conn.Dispose();
					conn.Close();
				}
			}
			return applicationsTable;
		}


		/// <summary>
		/// Delete all application instance records for the specified asset
		/// </summary>
		/// <param name="assetID"></param>
		/// <returns></returns>
		public int ApplicationInstanceDelete (int assetID)
		{
			using (SqlConnection conn = CreateOpenConnection())
			{
				try
				{
					SqlParameter[] spParams = new SqlParameter[1];
					spParams[0] = new SqlParameter("@forAssetID", SqlDbType.Int);
					spParams[0].Value = assetID;

					// Execute the stored procedure
					SqlCommand cmd = new SqlCommand("usp_applicationinstance_delete", conn);
					cmd.CommandType = CommandType.StoredProcedure;
					cmd.Parameters.AddRange(spParams);
					cmd.ExecuteNonQuery();
				}
				catch (SqlException ex)
				{
					this.HandleSqlError(ex);
				}
				finally
				{
					conn.Dispose();
					conn.Close();
				}
			}
			return 0;
		}


		/// <summary>
		/// Update an application instance record
		/// </summary>
		/// <param name="applicationInstance"></param>
		/// <returns></returns>
		public int ApplicationInstanceUpdate(ApplicationInstance applicationInstance)
		{
			using (SqlConnection conn = CreateOpenConnection())
			{
				try
				{
					string productSerial = (applicationInstance.Serial != null) ? applicationInstance.Serial.ProductId : "";
					string productCDKey = (applicationInstance.Serial != null) ? applicationInstance.Serial.CdKey : "";
					SqlParameter[] spParams = new SqlParameter[3];
					spParams[0] = new SqlParameter("@nInstanceID", SqlDbType.Int);
					spParams[0].Value = applicationInstance.InstanceID;
					spParams[1] = new SqlParameter("@cProductID", SqlDbType.VarChar ,255);
					spParams[1].Value = PrepareSqlString(productSerial, 255);
					spParams[2] = new SqlParameter("@cCDKey", SqlDbType.VarChar, 255);
					spParams[2].Value = PrepareSqlString(productCDKey, 255);
					
					// Execute the stored procedure
					SqlCommand cmd = new SqlCommand("usp_applicationinstance_update", conn);
					cmd.CommandType = CommandType.StoredProcedure;
					cmd.Parameters.AddRange(spParams);
					cmd.ExecuteNonQuery();
				}
				catch (SqlException ex)
				{
					this.HandleSqlError(ex);
				}
				finally
				{
					conn.Dispose();
					conn.Close();
				}
			}
			return 0;
		}




		/// <summary>
		/// Return a table of any suppoprt contracts for which alerts should be generated
		/// </summary>
		/// <param name="applicationID"></param>
		/// <returns></returns>
		public DataTable EnumerateSupportContractAlerts()
		{
			DataTable alertsTable = new DataTable(TableNames.SUPPORT_STATISTICS);
			using (SqlConnection conn = CreateOpenConnection())
			{
				try
				{
					SqlCommand cmd = new SqlCommand("usp_check_support_alerts", conn);
					cmd.CommandType = CommandType.StoredProcedure;
					new SqlDataAdapter(cmd).Fill(alertsTable);
				}
				catch (SqlException ex)
				{
					this.HandleSqlError(ex);
				}
				finally
				{
					conn.Dispose();
					conn.Close();
				}
			}
			return alertsTable;
		}

		#endregion Application_instance Table

		#region AuditedItems Table

		/// <summary>
		/// Return a table containing all of the audited items which have been declared for the 
		/// specified asset (with the specified parent)
		/// </summary>
		/// <param name="applicationID"></param>
		/// <returns></returns>
		public DataTable GetAuditedItems (int assetID ,string parentCategory ,bool all)
		{
			DataTable dataTable = new DataTable(TableNames.AUDITEDITEMS);
			using (SqlConnection conn = CreateOpenConnection())
			{
				try
				{
					SqlCommand cmd = new SqlCommand("usp_auditeditems_enumerate", conn);
					cmd.CommandType = CommandType.StoredProcedure;
					cmd.Parameters.Add("@assetID", SqlDbType.Int);
					cmd.Parameters["@assetID"].Value = assetID;
					cmd.Parameters.Add("@parentCategory", SqlDbType.VarChar, 1024);
					cmd.Parameters["@parentCategory"].Value = parentCategory;
					cmd.Parameters.Add("@bAllChildren", SqlDbType.Bit);
					cmd.Parameters["@bAllChildren"].Value = all;
					new SqlDataAdapter(cmd).Fill(dataTable);
				}
				catch (SqlException ex)
				{
					this.HandleSqlError(ex);
				}
				finally
				{
					conn.Dispose();
					conn.Close();
				}
			}
			return dataTable;
		}


		/// <summary>
		/// Return the name of the audited item with the specified database ID
		/// </summary>
		/// <param name="itemID"></param>
		/// <returns></returns>
		public string GetAuditedItemName(int itemID)
		{
			string returnValue = "";
			using (SqlConnection conn = CreateOpenConnection())
			{
				try
				{
					SqlCommand cmd = new SqlCommand("usp_auditeditem_getname", conn);
					cmd.CommandType = CommandType.StoredProcedure;
					cmd.Parameters.Add("@itemID", SqlDbType.Int);
					cmd.Parameters["@itemID"].Value = itemID;
					SqlParameter retValueParam = new SqlParameter("@retValue", SqlDbType.VarChar, 900);
					retValueParam.Direction = ParameterDirection.Output;
					cmd.Parameters.Add(retValueParam);
					cmd.ExecuteNonQuery();
					//
					if (retValueParam.Value != DBNull.Value)
						returnValue = retValueParam.Value.ToString();
				}
				catch (SqlException e)
				{
					this.HandleSqlError(e);
				}

				finally
				{
					conn.Dispose();
					conn.Close();
				}

				return returnValue;
			}
		}


		/// <summary>
		/// Return a table containing all of the audited items which have been declared for the 
		/// specified asset (with the specified parent)
		/// </summary>
		/// <param name="applicationID"></param>
		/// <returns></returns>
		public DataTable GetIconMappings()
		{
			DataTable dataTable = new DataTable(TableNames.AUDITEDITEMS);
			using (SqlConnection conn = CreateOpenConnection())
			{
				try
				{
					SqlCommand cmd = new SqlCommand("usp_auditeditems_enumerateicons", conn);
					cmd.CommandType = CommandType.StoredProcedure;
					new SqlDataAdapter(cmd).Fill(dataTable);
				}
				catch (SqlException ex)
				{
					this.HandleSqlError(ex);
				}
				finally
				{
					conn.Dispose();
					conn.Close();
				}
			}
			return dataTable;
		}


		/// <summary>
		/// Delete all audited item records for the specified asset
		/// </summary>
		/// <param name="assetID"></param>
		/// <returns></returns>
		public int AuditedItemsDelete(int assetID)
		{
			if (assetID != 0)
			{
				using (SqlConnection conn = CreateOpenConnection())
				{
					try
					{
						SqlParameter[] spParams = new SqlParameter[1];
						spParams[0] = new SqlParameter("@forAssetID", SqlDbType.Int);
						spParams[0].Value = assetID;

						// Execute the stored procedure
						SqlCommand cmd = new SqlCommand("usp_auditeditems_delete", conn);
						cmd.CommandType = CommandType.StoredProcedure;
						cmd.Parameters.AddRange(spParams);
						cmd.ExecuteNonQuery();
					}
					catch (SqlException ex)
					{
						this.HandleSqlError(ex);
					}
					finally
					{
						conn.Dispose();
						conn.Close();
					}
				}
			}
			return 0;
		}



		/// <summary>
		/// Return a table containing all of the audited items which have been declared for the 
		/// specified asset (with the specified parent)
		/// </summary>
		/// <param name="applicationID"></param>
		/// <returns></returns>
		public DataTable GetAuditedItemCategories(string parentCategory)
		{
			DataTable dataTable = new DataTable(TableNames.AUDITEDITEMS);
			using (SqlConnection conn = CreateOpenConnection())
			{
				try
				{
					SqlCommand cmd = new SqlCommand("usp_auditeditems_enumerate_categories", conn);
					cmd.CommandType = CommandType.StoredProcedure;
					cmd.Parameters.Add("@parentCategory", SqlDbType.VarChar, 1024);
					cmd.Parameters["@parentCategory"].Value = parentCategory;
					new SqlDataAdapter(cmd).Fill(dataTable);
				}
				catch (SqlException ex)
				{
					this.HandleSqlError(ex);
				}
				finally
				{
					conn.Dispose();
					conn.Close();
				}
			}
			return dataTable;
		}


		/// <summary>
		/// Return a table containing all of the audited item categories beneath the specified parent 
		/// which have a 'Value' field - that is the NAME column has a value
		/// </summary>
		/// <param name="applicationID"></param>
		/// <returns></returns>
		public DataTable GetAuditedItemCategoryNames(string parentCategory)
		{
			DataTable dataTable = new DataTable(TableNames.AUDITEDITEMS);
			using (SqlConnection conn = CreateOpenConnection())
			{
				try
				{
					SqlCommand cmd = new SqlCommand("usp_auditeditems_enumerate_category_names", conn);
					cmd.CommandType = CommandType.StoredProcedure;
					cmd.Parameters.Add("@parentCategory", SqlDbType.VarChar, 1024);
					cmd.Parameters["@parentCategory"].Value = parentCategory;
					new SqlDataAdapter(cmd).Fill(dataTable);
				}
				catch (SqlException ex)
				{
					this.HandleSqlError(ex);
				}
				finally
				{
					conn.Dispose();
					conn.Close();
				}
			}
			return dataTable;
		}


		/// <summary>
		/// Return a table containing the AUDITED VALUES for the specified Audited Data Item category/name optionally
		/// returning just the data for a specific asset
		/// </summary>
		/// <param name="applicationID"></param>
		/// <returns></returns>
		public DataTable GetAuditedItemValues(Asset forAsset ,string category ,string name)
		{
			DataTable dataTable = new DataTable(TableNames.AUDITEDITEMS);
			using (SqlConnection conn = CreateOpenConnection())
			{
				try
				{
					SqlCommand cmd = new SqlCommand("usp_auditeditems_enumerate_values", conn);
					cmd.CommandType = CommandType.StoredProcedure;
					cmd.Parameters.Add("@assetID", SqlDbType.Int);
					if (forAsset == null)
						cmd.Parameters["@assetID"].Value = 0;
					else
						cmd.Parameters["@assetID"].Value = forAsset.AssetID;
					cmd.Parameters.Add("@cCategory", SqlDbType.VarChar, 1024);
					cmd.Parameters["@cCategory"].Value = category;
					cmd.Parameters.Add("@cName", SqlDbType.VarChar, 1024);
					cmd.Parameters["@cName"].Value = name;
					new SqlDataAdapter(cmd).Fill(dataTable);
				}
				catch (SqlException ex)
				{
					this.HandleSqlError(ex);
				}
				finally
				{
					conn.Dispose();
					conn.Close();
				}
			}
			return dataTable;
		}


		/// <summary>
		/// Add a new Audited Item to the database
		/// </summary>
		/// <param name="theLicenseType"></param>
		/// <returns></returns>
		public int AuditedItemAdd(AuditedItem theItem)
		{
			int itemID = 0;
			using (SqlConnection conn = CreateOpenConnection())
			{
				try
				{
					SqlParameter[] spParams = new SqlParameter[10];
					spParams[0] = new SqlParameter("@cCategory", SqlDbType.VarChar ,1024);
					spParams[0].Value = theItem.Category;
					spParams[1] = new SqlParameter("@cName", SqlDbType.VarChar, 1024);
					spParams[1].Value = theItem.Name;
					spParams[2] = new SqlParameter("@cValue", SqlDbType.VarChar, 1024);
					spParams[2].Value = theItem.Value;
					spParams[3] = new SqlParameter("@cIcon", SqlDbType.VarChar, 255);
					spParams[3].Value = theItem.Icon;
					spParams[4] = new SqlParameter("@nAssetID", SqlDbType.Int);
					spParams[4].Value = theItem.AssetID;
					spParams[5] = new SqlParameter("@cDisplayUnits", SqlDbType.VarChar, 32);
					spParams[5].Value = theItem.DisplayUnits;
					spParams[6] = new SqlParameter("@nDataType", SqlDbType.Int);
					spParams[6].Value = theItem.Datatype;
					spParams[7] = new SqlParameter("@bHistoried", SqlDbType.Bit);
					spParams[7].Value = theItem.Historied;
					spParams[8] = new SqlParameter("@bGrouped", SqlDbType.Bit);
					spParams[8].Value = theItem.Grouped;
					spParams[9] = new SqlParameter("@nReturnID", SqlDbType.Int);
					spParams[9].Direction = ParameterDirection.ReturnValue;

					// Execute the stored procedure
					SqlCommand cmd = new SqlCommand("usp_auditeditem_add", conn);
					cmd.CommandType = CommandType.StoredProcedure;
					cmd.Parameters.AddRange(spParams);
					cmd.ExecuteNonQuery();
					itemID = (int)spParams[9].Value;
				}
				catch (SqlException e)
				{
					this.HandleSqlError(e);
				}
				finally
				{
					conn.Dispose();
					conn.Close();
				}
			}
			return itemID;
		}

#endregion

		#region Licenses Table

		/// <summary>
		/// Return a table containing all of the license which have been declared for the 
		/// specified application
		/// </summary>
		/// <param name="applicationID"></param>
		/// <returns></returns>
		public DataTable GetApplicationLicenses(int applicationID)
		{
			DataTable licensesTable = new DataTable(TableNames.LICENSES);
			using (SqlConnection conn = CreateOpenConnection())
			{
				try
				{
					SqlCommand cmd = new SqlCommand("usp_applicationlicense_enumerate", conn);
					cmd.CommandType = CommandType.StoredProcedure;
					cmd.Parameters.Add("@applicationID", SqlDbType.Int);
					cmd.Parameters["@applicationID"].Value = applicationID;
					new SqlDataAdapter(cmd).Fill(licensesTable);
				}
				catch (SqlException ex)
				{
					this.HandleSqlError(ex);
				}
				finally
				{
					conn.Dispose();
					conn.Close();
				}
			}
			return licensesTable;
		}


		/// <summary>
		/// Return a table containing all of the license which have been declared for the 
		/// specified operating system
		/// </summary>
		/// <param name="applicationID"></param>
		/// <returns></returns>
		public DataTable GetOSLicenses(int osID)
		{
			DataTable dataTable = new DataTable(TableNames.LICENSES);
			using (SqlConnection conn = CreateOpenConnection())
			{
				try
				{
					SqlCommand cmd = new SqlCommand("usp_applicationlicense_enumerate", conn);
					cmd.CommandType = CommandType.StoredProcedure;
					cmd.Parameters.Add("@osID", SqlDbType.Int);
					cmd.Parameters["@osID"].Value = osID;
					new SqlDataAdapter(cmd).Fill(dataTable);
				}
				catch (SqlException ex)
				{
					this.HandleSqlError(ex);
				}
				finally
				{
					conn.Dispose();
					conn.Close();
				}
			}
			return dataTable;
		}


		/// <summary>
		/// Return a table containing all of the license types that have been declared 
		/// </summary>
		/// <returns></returns>
		public DataTable EnumerateLicenses(LicenseType forLicenseType)
		{
			DataTable licensesTable = new DataTable(TableNames.LICENSES);
			using (SqlConnection conn = CreateOpenConnection())
			{
				try
				{
					SqlCommand cmd = new SqlCommand("usp_licenses_enumerate", conn);
					cmd.CommandType = CommandType.StoredProcedure;
					cmd.Parameters.Add("@nLicenseTypeID", SqlDbType.Int);
					cmd.Parameters["@nLicenseTypeID"].Value = (forLicenseType == null) ? 0 : forLicenseType.LicenseTypeID;
					new SqlDataAdapter(cmd).Fill(licensesTable);
				}
				catch (SqlException ex)
				{
					this.HandleSqlError(ex);
				}
				finally
				{
					conn.Dispose();
					conn.Close();
				}
			}
			return licensesTable;
		}


		/// <summary>
		/// Add a new Application License to the database
		/// </summary>
		/// <param name="theLicenseType"></param>
		/// <returns></returns>
		public int LicenseAdd(ApplicationLicense theLicense)
		{
			int itemID = 0;
			using (SqlConnection conn = CreateOpenConnection())
			{
				try
				{
					SqlParameter[] spParams = new SqlParameter[10];
					spParams[0] = new SqlParameter("@nLicenseTypeID", SqlDbType.Int);
					spParams[0].Value = theLicense.LicenseTypeID;
					spParams[1] = new SqlParameter("@nForApplicationID", SqlDbType.Int);
					spParams[1].Value = theLicense.ApplicationID;
					spParams[2] = new SqlParameter("@nCount", SqlDbType.Int);
					spParams[2].Value = theLicense.Count;
					spParams[3] = new SqlParameter("@bSupported", SqlDbType.Bit);
					spParams[3].Value = (theLicense.Supported) ? 1 : 0;
					spParams[4] = new SqlParameter("@dtSupportExpiry", SqlDbType.DateTime);
					spParams[4].Value = theLicense.SupportExpiryDate;
					spParams[5] = new SqlParameter("@nSupportAlertDays", SqlDbType.Int);
					spParams[5].Value = theLicense.SupportAlertDays;
					spParams[6] = new SqlParameter("@bSupportAlertEmail", SqlDbType.Bit);
					spParams[6].Value = (theLicense.SupportAlertEmail) ? 1 : 0;
					spParams[7] = new SqlParameter("@cSupportAlertRecipients", SqlDbType.VarChar, 1020);
					spParams[7].Value = PrepareSqlString(theLicense.SupportAlertRecipients ,1020);
					spParams[8] = new SqlParameter("@nSupplierID", SqlDbType.Int);
					spParams[8].Value = theLicense.SupplierID;
					spParams[9] = new SqlParameter("@nReturnID", SqlDbType.Int);
					spParams[9].Direction = ParameterDirection.Output;

					// Execute the stored procedure
					SqlCommand cmd = new SqlCommand("usp_license_add", conn);
					cmd.CommandType = CommandType.StoredProcedure;
					cmd.Parameters.AddRange(spParams);
					cmd.ExecuteNonQuery();

					// Pickup the returned index of the inserted license
					itemID = (int)spParams[9].Value;
				}
				catch (SqlException e)
				{
					this.HandleSqlError(e);
				}
				finally
				{
					conn.Dispose();
					conn.Close();
				}
			}
			return itemID;
		}


		/// <summary>
		/// Update the definition stored for the specified Application License
		/// </summary>
		/// <param name="theAsset"></param>
		/// <returns></returns>
		public int LicenseUpdate(ApplicationLicense theLicense)
		{
			using (SqlConnection conn = CreateOpenConnection())
			{
				try
				{
					SqlParameter[] spParams = new SqlParameter[10];
					spParams[0] = new SqlParameter("@nLicenseID", SqlDbType.Int);
					spParams[0].Value = theLicense.LicenseID;
					spParams[1] = new SqlParameter("@nLicenseTypeID", SqlDbType.Int);
					spParams[1].Value = theLicense.LicenseTypeID;
					spParams[2] = new SqlParameter("@nApplicationID", SqlDbType.Int);
					spParams[2].Value = theLicense.ApplicationID;
					spParams[3] = new SqlParameter("@nCount", SqlDbType.Int);
					spParams[3].Value = theLicense.Count;
					spParams[3] = new SqlParameter("@nCount", SqlDbType.Int);
					spParams[3].Value = theLicense.Count;
					spParams[4] = new SqlParameter("@bSupported", SqlDbType.Bit);
					spParams[4].Value = (theLicense.Supported) ? 1 : 0;
					spParams[5] = new SqlParameter("@dtSupportExpiry", SqlDbType.DateTime);
					spParams[5].Value = theLicense.SupportExpiryDate;
					spParams[6] = new SqlParameter("@nSupportAlertDays", SqlDbType.Int);
					spParams[6].Value = theLicense.SupportAlertDays;
					spParams[7] = new SqlParameter("@bSupportAlertEmail", SqlDbType.Bit);
					spParams[7].Value = (theLicense.SupportAlertEmail) ? 1 : 0;
					spParams[8] = new SqlParameter("@cSupportAlertRecipients", SqlDbType.VarChar, 1020);
					spParams[8].Value = theLicense.SupportAlertRecipients;
					spParams[9] = new SqlParameter("@nSupplierID", SqlDbType.Int);
					spParams[9].Value = theLicense.SupplierID;
					//
					SqlCommand cmd = new SqlCommand("usp_license_update", conn);
					cmd.CommandType = CommandType.StoredProcedure;
					cmd.Parameters.AddRange(spParams);
					cmd.ExecuteNonQuery();
				}
				catch (SqlException e)
				{
					this.HandleSqlError(e);
				}
				finally
				{
					conn.Dispose();
					conn.Close();
				}
			}
			return 0;
		}



		/// <summary>
		/// Delete the specified License from the database
		/// </summary>
		/// <param name="licenseID"></param>
		/// <returns></returns>
		public int LicenseDelete(ApplicationLicense theLicense)
		{
			if (theLicense.LicenseID != 0)
			{
				using (SqlConnection conn = CreateOpenConnection())
				{
					try
					{
						SqlParameter[] spParams = new SqlParameter[1];
						spParams[0] = new SqlParameter("@nLicenseID", SqlDbType.Int);
						spParams[0].Value = theLicense.LicenseID;
						//
						SqlCommand cmd = new SqlCommand("usp_license_delete", conn);
						cmd.CommandType = CommandType.StoredProcedure;
						cmd.Parameters.AddRange(spParams);
						cmd.ExecuteNonQuery();
					}
					catch (SqlException e)
					{
						this.HandleSqlError(e);
					}
					finally
					{
						conn.Dispose();
						conn.Close();
					}
				}
			}
			return 0;
		}



		/// <summary>
		/// Return a table containing the definitions for ALL Support Contracts defined within the database
		/// </summary>
		/// <returns></returns>
		public DataTable GetSupportContracts()
		{
			DataTable supportTable = new DataTable(TableNames.LICENSES);
			using (SqlConnection conn = CreateOpenConnection())
			{
				try
				{
					SqlCommand cmd = new SqlCommand("usp_supportcontract_enumerate", conn);
					cmd.CommandType = CommandType.StoredProcedure;
					SqlDataAdapter adapter = new SqlDataAdapter(cmd);
					adapter.Fill(supportTable);
				}
				catch (SqlException e)
				{
					this.HandleSqlError(e);
				}
				finally
				{
					conn.Dispose();
					conn.Close();
				}
			}
			return supportTable;
		}



#endregion Licenses Table

		#region License_Types Table
		/// <summary>
		/// Add a new License Type to the database
		/// </summary>
		/// <param name="theLicenseType"></param>
		/// <returns></returns>
		public int LicenseTypeAdd(LicenseType theLicenseType)
		{
			int itemID = 0;
			using (SqlConnection conn = CreateOpenConnection())
			{
				try
				{
					SqlParameter[] spParams = new SqlParameter[3]; 
					spParams[0] = new SqlParameter("@cName", SqlDbType.VarChar ,255);
					spParams[0].Value =  PrepareSqlString(theLicenseType.Name ,255);
					spParams[1] = new SqlParameter("@bPerPC", SqlDbType.Bit);
					spParams[1].Value =  theLicenseType.PerComputer;
					spParams[2] = new SqlParameter("@nReturnID", SqlDbType.Int);
					spParams[2].Direction = ParameterDirection.Output;
					//
					SqlCommand cmd = new SqlCommand("usp_licensetype_add", conn);
					cmd.CommandType = CommandType.StoredProcedure;
					cmd.Parameters.AddRange(spParams);
					cmd.ExecuteNonQuery();
					itemID = (int)spParams[2].Value;
				}
				catch (SqlException e)
				{
					this.HandleSqlError(e);
				}
				finally
				{
					conn.Dispose();
					conn.Close();
				}
			}
			return itemID;
		}


		/// <summary>
		/// Update the definition stored for the specified LicenseType
		/// </summary>
		/// <param name="theAsset"></param>
		/// <returns></returns>
		public int LicenseTypeUpdate(LicenseType theLicenseType)
		{
			using (SqlConnection conn = CreateOpenConnection())
			{
				try
				{
					new DbStoredProcedure(conn).ExecSProc("usp_licensetype_update", new object[] { theLicenseType.LicenseTypeID, theLicenseType.PerComputer });
				}
				catch (SqlException e)
				{
					this.HandleSqlError(e);
				}
				finally
				{
					conn.Dispose();
					conn.Close();
				}
			}
			return 0;
		}


		/// <summary>
		/// Return a table containing all of the license types that have been declared 
		/// </summary>
		/// <returns></returns>
		public DataTable EnumerateLicenseTypes()
		{
			DataTable licensetypeTable = new DataTable(TableNames.LICENSETYPES);
			using (SqlConnection conn = CreateOpenConnection())
			{
				try
				{
					SqlCommand cmd = new SqlCommand("usp_licensetype_enumerate", conn);
					cmd.CommandType = CommandType.StoredProcedure;
					new SqlDataAdapter(cmd).Fill(licensetypeTable);
				}
				catch (SqlException ex)
				{
					this.HandleSqlError(ex);
				}
				finally
				{
					conn.Dispose();
					conn.Close();
				}
			}
			return licensetypeTable;
		}


		/// <summary>
		/// Delete the specified License TYPE from the database
		/// </summary>
		/// <param name="licenseTypeID"></param>
		/// <returns></returns>
		public int LicenseTypeDelete(LicenseType theLicenseType)
		{
			if (theLicenseType.LicenseTypeID != 0)
			{
				using (SqlConnection conn = CreateOpenConnection())
				{
					try
					{
						new DbStoredProcedure(conn).ExecSProc("usp_licensetype_delete", new object[] { theLicenseType.LicenseTypeID});
					}
					catch (SqlException e)
					{
						this.HandleSqlError(e);
					}
					finally
					{
						conn.Dispose();
						conn.Close();
					}
				}
			}
			return 0;
		}




		/// <summary>
		/// Return the database index of the specified LicenseType
		/// </summary>
		/// <param name="name"></param>
		/// <returns></returns>
		public int LicenseTypeFind(string name)
		{
			int itemId = 0;
			using (SqlConnection conn = CreateOpenConnection())
			{
				try
				{
					SqlParameter[] spParams = new SqlParameter[2];
					spParams[0] = new SqlParameter("@cName", SqlDbType.VarChar);
					spParams[0].Size = 255;
					spParams[0].Value = name;
					spParams[1] = new SqlParameter("@nReturnID", SqlDbType.Int);
					spParams[1].Direction = ParameterDirection.ReturnValue;
					//
					SqlCommand cmd = new SqlCommand("usp_licensetype_find", conn);
					cmd.CommandType = CommandType.StoredProcedure;
					cmd.Parameters.AddRange(spParams);
					cmd.ExecuteNonQuery();
					itemId = (int)spParams[1].Value;
				}
				catch (SqlException e)
				{
					this.HandleSqlError(e);
				}
				finally
				{
					conn.Dispose();
					conn.Close();
				}
			}
			return itemId;
		}

#endregion License_Types Table

		#region Settings Table Functions


		public string GetSetting(string key, bool decrypt)
		{
			string returnSetting = null;

			using (SqlConnection conn = CreateOpenConnection())
			{
				try
				{
					SqlCommand cmd = new SqlCommand("usp_settings_GetSetting", conn);
					cmd.CommandType = CommandType.StoredProcedure;
					cmd.Parameters.Add("@key", SqlDbType.VarChar);
					cmd.Parameters["@key"].Value = key;
					SqlParameter retValueParam = new SqlParameter("@retValue", SqlDbType.VarChar, 255);
					retValueParam.Direction = ParameterDirection.Output;
					cmd.Parameters.Add(retValueParam);
					cmd.ExecuteNonQuery();
					//
					if (retValueParam.Value != DBNull.Value)
					{
						if (decrypt)
							returnSetting = AES.Decrypt(retValueParam.Value.ToString());
						else
							returnSetting = retValueParam.Value.ToString();
					}
				}
				catch (SqlException e)
				{
					this.HandleSqlError(e);
				}

				finally
				{
					conn.Dispose();
					conn.Close();
				}

				return returnSetting;
			}
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
			if (returnString == null)
				returnString = defaultValue;
			return returnString;
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
			if (value != "")
			{
				try
				{
					returnValue = Convert.ToBoolean(value);
				}
				catch (Exception)
				{
				}
			}
			return returnValue;
		}
		
		
		public void SetSetting(string key, string value, bool encrypt)
		{
			using (SqlConnection conn = CreateOpenConnection())
			{
				try
				{
					SqlCommand cmd = new SqlCommand("usp_settings_SetSetting", conn);
					cmd.CommandType = CommandType.StoredProcedure;
					cmd.Parameters.Add("@key", SqlDbType.VarChar);
					cmd.Parameters["@key"].Value = key;
					cmd.Parameters.Add("@value", SqlDbType.VarChar);
					if (encrypt)
					{
						cmd.Parameters["@value"].Value = AES.Encrypt(value);
					}
					else
					{
						cmd.Parameters["@value"].Value = value;
					}
					cmd.ExecuteNonQuery();
				}

				catch (SqlException e)
				{
					this.HandleSqlError(e);
				}

				finally
				{
					conn.Dispose();
					conn.Close();
				}
			}
		}
		
		public void SetSetting(string key, bool value)
		{
			SetSetting(key ,value.ToString() ,false);
		}
		

		/// <summary>
		/// Recover the current publisher filter (wrapper around the GetSetting function)
		/// </summary>
		/// <returns></returns>
		public string PublisherFilter()
		{
			return GetSetting("Publisher Filter", false);
		}


		/// <summary>
		/// Set the current publisher filter (wrapper around the SetSetting function)
		/// </summary>
		/// <returns></returns>
		public void PublisherFilter(string publisherFilter)
		{
			SetSetting("Publisher Filter", publisherFilter, false);
		}

		#endregion Settings Table Functions

		#region AuditTrail Table Functions


		/// <summary>
		/// Add a new Asset to the database
		/// </summary>
		/// <param name="theAsset"></param>
		/// <returns></returns>
		public int AuditTrailAdd(AuditTrailEntry ate)
		{
			int itemID = 0;
			using (SqlConnection conn = CreateOpenConnection())
			{
				try
				{
					SqlParameter[] spParams = new SqlParameter[9];
					spParams[0] = new SqlParameter("@nAssetID", SqlDbType.Int);
					spParams[0].Value = ate.AssetID;
					spParams[1] = new SqlParameter("@cUsername", SqlDbType.VarChar);
					spParams[1].Size = 255;
					spParams[1].Value = PrepareSqlString(ate.Username ,255);
					spParams[2] = new SqlParameter("@dtDate", SqlDbType.DateTime);
					spParams[2].Value = new System.Data.SqlTypes.SqlDateTime(ate.Date);
					spParams[3] = new SqlParameter("@nClass", SqlDbType.Int);
					spParams[3].Value = ate.Class;
					spParams[4] = new SqlParameter("@nType", SqlDbType.Int);
					spParams[4].Value = ate.Type;
					spParams[5] = new SqlParameter("@cKey", SqlDbType.VarChar);
					spParams[5].Size = 255;
					spParams[5].Value = PrepareSqlString(ate.Key ,255);
					spParams[6] = new SqlParameter("@cOldValue", SqlDbType.VarChar);
					spParams[6].Size = 255;
					spParams[6].Value = PrepareSqlString(ate.OldValue ,255);
					spParams[7] = new SqlParameter("@cNewValue", SqlDbType.VarChar);
					spParams[7].Size = 255;
					spParams[7].Value = PrepareSqlString(ate.NewValue ,255);
					spParams[8] = new SqlParameter("@nReturnID", SqlDbType.Int);
					spParams[8].Direction = ParameterDirection.Output;
					//
					SqlCommand cmd = new SqlCommand("usp_audittrail_add", conn);
					cmd.CommandType = CommandType.StoredProcedure;
					cmd.Parameters.AddRange(spParams);
					cmd.ExecuteNonQuery();
					itemID = (int)spParams[8].Value;
				}
				catch (SqlException ex)
				{
					this.HandleSqlError(ex);
				}
				finally
				{
					conn.Dispose();
					conn.Close();
				}
			}
			return itemID;
		}



		/// <summary>
		/// Delete the specified Audit TRail Entry from the database
		/// </summary>
		/// <param name="ate">The audit trail entry to remove</param>
		/// <returns></returns>
		public int AuditTrailDelete(AuditTrailEntry ate)
		{
			if (ate.AuditTrailID != 0)
			{
				using (SqlConnection conn = CreateOpenConnection())
				{
					try
					{
						new DbStoredProcedure(conn).ExecSProc("usp_audittrail_delete", new object[] { ate.AuditTrailID });
					}
					catch (SqlException e)
					{
						this.HandleSqlError(e);
					}
					finally
					{
						conn.Dispose();
						conn.Close();
					}
				}
			}
			return 0;
		}

		/// <summary>
		/// Return a list of all of the audit trail entries
		/// </summary>
		/// <returns></returns>
		public DataTable GetAuditTrailEntries (int requiredClass)
		{
			DataTable ateTable = new DataTable(TableNames.AUDITTRAIL);
			using (SqlConnection conn = CreateOpenConnection())
			{
				try
				{
					SqlParameter[] spParams = new SqlParameter[1];
					spParams[0] = new SqlParameter("@nClass", SqlDbType.Int);
					spParams[0].Value = requiredClass;
					//
					SqlCommand cmd = new SqlCommand("usp_audittrail_enumerate", conn);
					cmd.CommandType = CommandType.StoredProcedure;
					cmd.Parameters.AddRange(spParams);
					new SqlDataAdapter(cmd).Fill(ateTable);
				}
				catch (SqlException e)
				{
					this.HandleSqlError(e);
				}
				finally
				{
					conn.Dispose();
					conn.Close();
				}
				return ateTable;
			}
		}



		/// <summary>
		/// Return a list of all of the audit trail entries which are audit history records for
		/// the specified asset
		/// </summary>
		/// <returns></returns>
		public DataTable GetAssetAuditHistory(Asset forAsset, DateTime startDate ,DateTime endDate)
		{
			DataTable ateTable = new DataTable(TableNames.AUDITTRAIL);
			using (SqlConnection conn = CreateOpenConnection())
			{
				try
				{
					SqlParameter[] spParams = new SqlParameter[3];
					spParams[0] = new SqlParameter("@nAssetID", SqlDbType.Int);
					spParams[0].Value = forAsset.AssetID;
					spParams[1] = new SqlParameter("@dtStartDate", SqlDbType.DateTime);
					spParams[1].IsNullable = true;
					if ((startDate == null) || (startDate.Ticks == 0))
						spParams[1].Value = DBNull.Value;
					else
						spParams[1].Value = startDate;
					//
					spParams[2] = new SqlParameter("@dtEndDate", SqlDbType.DateTime);
					spParams[2].IsNullable = true;
					if ((endDate == null) || (endDate.Ticks == 0))
						spParams[2].Value = DBNull.Value;
					else
						spParams[2].Value = endDate;
					
					//
					SqlCommand cmd = new SqlCommand("usp_audittrail_getassethistory", conn);
					cmd.CommandType = CommandType.StoredProcedure;
					cmd.Parameters.AddRange(spParams);
					new SqlDataAdapter(cmd).Fill(ateTable);
				}
				catch (SqlException e)
				{
					this.HandleSqlError(e);
				}
				finally
				{
					conn.Dispose();
					conn.Close();
				}
				return ateTable;
			}
		}




		/// <summary>
		/// Return a list of all of the audit trail entries which are audit history records for
		/// the specified asset
		/// </summary>
		/// <returns></returns>
		public DataTable GetAssetLastAuditDate(Asset forAsset, int days ,bool hasBeenAudited)
		{
			DataTable ateTable = new DataTable(TableNames.AUDITTRAIL);
			using (SqlConnection conn = CreateOpenConnection())
			{
				try
				{
					SqlParameter[] spParams = new SqlParameter[3];
					spParams[0] = new SqlParameter("@nAssetID", SqlDbType.Int);
					spParams[0].Value = forAsset.AssetID;
					spParams[1] = new SqlParameter("@nDays", SqlDbType.Int);
					spParams[1].Value = days;
					spParams[2] = new SqlParameter("@bAudited", SqlDbType.Bit);
					spParams[2].Value = hasBeenAudited;
					//
					SqlCommand cmd = new SqlCommand("usp_audittrail_lastauditdate", conn);
					cmd.CommandType = CommandType.StoredProcedure;
					cmd.Parameters.AddRange(spParams);
					new SqlDataAdapter(cmd).Fill(ateTable);
				}
				catch (SqlException e)
				{
					this.HandleSqlError(e);
				}
				finally
				{
					conn.Dispose();
					conn.Close();
				}
				return ateTable;
			}
		}



		/// <summary>
		/// Purge the Audit Trail records from the database
		/// </summary>
		/// <returns></returns>
		public int AuditTrailPurge(DateTime dtPurge)
		{
			int purgeCount = 0;
			using (SqlConnection conn = CreateOpenConnection())
			{
				try
				{
					SqlParameter[] spParams = new SqlParameter[2];
					spParams[0] = new SqlParameter("@dtPurgeDate", SqlDbType.DateTime);
					spParams[0].Value = dtPurge;
					spParams[1] = new SqlParameter("@nReturnCount", SqlDbType.Int);
					spParams[1].Direction = ParameterDirection.ReturnValue;
					//
					SqlCommand cmd = new SqlCommand("usp_audittrail_purge", conn);
					cmd.CommandType = CommandType.StoredProcedure;
					cmd.Parameters.AddRange(spParams);
					cmd.ExecuteNonQuery();
					purgeCount = (int)spParams[1].Value;
				}
				catch (SqlException e)
				{
					this.HandleSqlError(e);
				}
				finally
				{
					conn.Dispose();
					conn.Close();
				}
			}
			return purgeCount;
		}

#endregion AuditTrail functions

		#region Users Table

		/// <summary>
		/// Return a table containing all of the Users which have been declared 
		/// </summary>
		/// <returns></returns>
		public DataTable GetUsers ()
		{
			DataTable usersTable = new DataTable(TableNames.USERS);
			using (SqlConnection conn = CreateOpenConnection())
			{
				try
				{
					SqlCommand cmd = new SqlCommand("usp_users_enumerate", conn);
					cmd.CommandType = CommandType.StoredProcedure;
					new SqlDataAdapter(cmd).Fill(usersTable);
				}
				catch (SqlException ex)
				{
					this.HandleSqlError(ex);
				}
				finally
				{
					conn.Dispose();
					conn.Close();
				}
			}
			return usersTable;
		}


		/// <summary>
		/// Add a new Application License to the database
		/// </summary>
		/// <param name="theLicenseType"></param>
		/// <returns></returns>
		public int UserAdd(User theUser)
		{
			int itemID = 0;
			using (SqlConnection conn = CreateOpenConnection())
			{
				try
				{
					SqlParameter[] spParams = new SqlParameter[6];
					spParams[0] = new SqlParameter("@cLogin", SqlDbType.VarChar ,255);
					spParams[0].Value = theUser.Logon;
					spParams[1] = new SqlParameter("@cFirstName", SqlDbType.VarChar ,255);
					spParams[1].Value = theUser.FirstName;
					spParams[2] = new SqlParameter("@cLastName", SqlDbType.VarChar ,255);
					spParams[2].Value = theUser.LastName;
					spParams[3] = new SqlParameter("@nAccessLevel", SqlDbType.Int);
					spParams[3].Value = (int)theUser.AccessLevel;
					spParams[4] = new SqlParameter("@nRootLocation", SqlDbType.Int);
					spParams[4].Value = theUser.RootLocationID;
					spParams[5] = new SqlParameter("@nReturnId", SqlDbType.Int);
					spParams[5].Direction = ParameterDirection.Output;

					// Execute the stored procedure
					SqlCommand cmd = new SqlCommand("[usp_add_user]", conn);
					cmd.CommandType = CommandType.StoredProcedure;
					cmd.Parameters.AddRange(spParams);
					cmd.ExecuteNonQuery();

					// Pickup the returned index of the inserted license
					itemID = (int)spParams[5].Value;
				}
				catch (SqlException e)
				{
					this.HandleSqlError(e);
				}
				finally
				{
					conn.Dispose();
					conn.Close();
				}
			}
			return itemID;
		}




		/// <summary>
		/// Set the password for the specified user
		/// </summary>
		/// <param name="theLicenseType"></param>
		/// <returns></returns>
		public void UserSetPassword(int userId ,string password)
		{
			// ..then update the user
			using (SqlConnection conn = CreateOpenConnection())
			{
				try
				{
					SqlParameter[] spParams = new SqlParameter[2];
					spParams[0] = new SqlParameter("@nUserId", SqlDbType.Int);
					spParams[0].Value = userId;
					spParams[1] = new SqlParameter("@cPassword", SqlDbType.VarChar, 255);
					spParams[1].Value = AES.Encrypt(password);

					// Execute the stored procedure
					SqlCommand cmd = new SqlCommand("[usp_user_setpassword]", conn);
					cmd.CommandType = CommandType.StoredProcedure;
					cmd.Parameters.AddRange(spParams);
					cmd.ExecuteNonQuery();
				}
				catch (SqlException e)
				{
					this.HandleSqlError(e);
				}
				finally
				{
					conn.Dispose();
					conn.Close();
				}
			}
		}




		/// <summary>
		/// Check the username/password pair and recover the full details of the logged in user (if any)
		/// </summary>
		/// <returns></returns>
		public User UserCheckPassword(string username ,string password)
		{
			User loggedUser = null;

			using (SqlConnection conn = CreateOpenConnection())
			{
				try
				{
					SqlParameter[] spParams = new SqlParameter[3];
					spParams[0] = new SqlParameter("@cLogin", SqlDbType.VarChar, 255);
					spParams[0].Value = username;
					spParams[1] = new SqlParameter("@cPassword", SqlDbType.VarChar, 255);
					spParams[1].Value = (password == "") ? "" : AES.Encrypt(password);
					spParams[2] = new SqlParameter("@nReturnID", SqlDbType.Int);
					spParams[2].Direction = ParameterDirection.Output;
					//
					SqlCommand cmd = new SqlCommand("usp_user_checkpassword", conn);
					cmd.CommandType = CommandType.StoredProcedure;
					cmd.Parameters.AddRange(spParams);
					cmd.ExecuteNonQuery();

					// Check the return to see if valid or not - if valid we need to return the full 
					// details for this user
					bool IsValid = ((int)spParams[2].Value != 0);
					if (IsValid)
						loggedUser = GetUserDetails((int)spParams[2].Value);
				}
				catch (SqlException e)
				{
					this.HandleSqlError(e);
				}
				finally
				{
					conn.Dispose();
					conn.Close();
				}
			}

			return loggedUser;
		}



		/// <summary>
		/// Return full details of the user with the specified ID
		/// </summary>
		/// <returns></returns>
		public User GetUserDetails (int userId)
		{
			User user = null;
			//
			DataTable usersTable = new DataTable(TableNames.USERS);
			using (SqlConnection conn = CreateOpenConnection())
			{
				try
				{
					SqlParameter[] spParams = new SqlParameter[1];
					spParams[0] = new SqlParameter("@nUserId", SqlDbType.Int);
					spParams[0].Value = userId;
					SqlCommand cmd = new SqlCommand("usp_user_getdetails", conn);
					cmd.CommandType = CommandType.StoredProcedure;
					cmd.Parameters.AddRange(spParams);
					new SqlDataAdapter(cmd).Fill(usersTable);

					// ...and use the first row returned to populate the user
					if (usersTable.Rows.Count != 0)
						user = new User(usersTable.Rows[0]);
				}
				catch (SqlException ex)
				{
					this.HandleSqlError(ex);
				}
				finally
				{
					conn.Dispose();
					conn.Close();
				}
			}
			return user;
		}


		/// <summary>
		/// Update the definition stored for the specified User
		/// </summary>
		/// <param name="theAsset"></param>
		/// <returns></returns>
		public int UserUpdate(User theUser)
		{
			using (SqlConnection conn = CreateOpenConnection())
			{
				try
				{
					SqlParameter[] spParams = new SqlParameter[6];
					spParams[0] = new SqlParameter("@nUserID", SqlDbType.Int);
					spParams[0].Value = theUser.UserID;
					spParams[1] = new SqlParameter("@cLogin", SqlDbType.VarChar, 255);
					spParams[1].Value = theUser.Logon;
					spParams[2] = new SqlParameter("@cFirstName", SqlDbType.VarChar, 255);
					spParams[2].Value = theUser.FirstName;
					spParams[3] = new SqlParameter("@cLastName", SqlDbType.VarChar, 255);
					spParams[3].Value = theUser.LastName;
					spParams[4] = new SqlParameter("@nAccessLevel", SqlDbType.Int);
					spParams[4].Value = (int)theUser.AccessLevel;
					spParams[5] = new SqlParameter("@nRootLocation", SqlDbType.Int);
					spParams[5].Value = theUser.RootLocationID;
					//
					SqlCommand cmd = new SqlCommand("usp_user_update", conn);
					cmd.CommandType = CommandType.StoredProcedure;
					cmd.Parameters.AddRange(spParams);
					cmd.ExecuteNonQuery();
				}
				catch (SqlException e)
				{
					this.HandleSqlError(e);
				}
				finally
				{
					conn.Dispose();
					conn.Close();
				}
			}
			return 0;
		}



		/// <summary>
		/// Delete the specified User from the database
		/// </summary>
		/// <param name="licenseID"></param>
		/// <returns></returns>
		public int UserDelete(User theUser)
		{
			// We cannot delete the last administrator so we had better ensure that if this is 
			// an administrator that there is at least 1 other
			if (theUser.AccessLevel == User.ACCESSLEVEL.administrator)
			{
				int administratorCount = UserAdministratorCount();
				if (administratorCount <= 1)
					return -1;
			}

			if (theUser.UserID != 0)
			{
				using (SqlConnection conn = CreateOpenConnection())
				{
					try
					{
						SqlParameter[] spParams = new SqlParameter[1];
						spParams[0] = new SqlParameter("@nUserID", SqlDbType.Int);
						spParams[0].Value = theUser.UserID;
						//
						SqlCommand cmd = new SqlCommand("usp_user_delete", conn);
						cmd.CommandType = CommandType.StoredProcedure;
						cmd.Parameters.AddRange(spParams);
						cmd.ExecuteNonQuery();
					}
					catch (SqlException e)
					{
						this.HandleSqlError(e);
					}
					finally
					{
						conn.Dispose();
						conn.Close();
					}
				}
			}
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
			bool bEnabled = false;
			using (SqlConnection conn = CreateOpenConnection())
			{
				try
				{
					SqlParameter[] spParams = new SqlParameter[1];
					spParams[0] = new SqlParameter("@bEnabled", SqlDbType.Bit);
					spParams[0].Direction = ParameterDirection.Output;
					//
					SqlCommand cmd = new SqlCommand("usp_get_security", conn);
					cmd.CommandType = CommandType.StoredProcedure;
					cmd.Parameters.AddRange(spParams);
					cmd.ExecuteNonQuery();
					bEnabled = (bool)spParams[0].Value;
				}
				catch (SqlException e)
				{
					this.HandleSqlError(e);
				}
				finally
				{
					conn.Dispose();
					conn.Close();
				}
			}

			return bEnabled;
		}


		/// <summary>
		/// This function sets whether or not security is enabled
		/// </summary>
		/// <param name="enable"></param>
		public void SecurityStatus(bool enable)
		{
			using (SqlConnection conn = CreateOpenConnection())
			{
				try
				{
					SqlParameter[] spParams = new SqlParameter[1];
					spParams[0] = new SqlParameter("@bEnable", SqlDbType.Bit);
					spParams[0].Value = enable;
					//
					SqlCommand cmd = new SqlCommand("usp_set_security", conn);
					cmd.CommandType = CommandType.StoredProcedure;
					cmd.Parameters.AddRange(spParams);
					cmd.ExecuteNonQuery();
				}
				catch (SqlException e)
				{
					this.HandleSqlError(e);
				}
				finally
				{
					conn.Dispose();
					conn.Close();
				}
			}
		}

		#endregion Users Table

		#region Alerts Table


		/// <summary>
		/// Return a table containing all of the Alerts have been triggered since the specified date
		/// </summary>
		/// <returns></returns>
		public DataTable EnumerateAlerts(DateTime sinceDate)
		{
			DataTable alertsTable = new DataTable(TableNames.ALERTS);
			using (SqlConnection conn = CreateOpenConnection())
			{
				try
				{
					SqlCommand cmd = new SqlCommand("usp_alert_enumerate", conn);
					cmd.CommandType = CommandType.StoredProcedure;
					cmd.Parameters.Add("@dtSinceDate", SqlDbType.DateTime);
					cmd.Parameters["@dtSinceDate"].Value = sinceDate;
					new SqlDataAdapter(cmd).Fill(alertsTable);
				}
				catch (SqlException ex)
				{
					this.HandleSqlError(ex);
				}
				finally
				{
					conn.Dispose();
					conn.Close();
				}
			}
			return alertsTable;
		}



		/// <summary>
		/// Add a new Alert to the database
		/// </summary>
		/// <param name="theAlert"></param>
		/// <returns></returns>
		public int AlertAdd(Alert theAlert)
		{
			int itemID = 0;
			LogFile ourLog = LogFile.Instance;

			using (SqlConnection conn = CreateOpenConnection())
			{
				try
				{
					SqlParameter[] spParams = new SqlParameter[8];
					spParams[0] = new SqlParameter("@nType", SqlDbType.Int);
					spParams[0].Value = (int)theAlert.Type;
					spParams[1] = new SqlParameter("@nCategory", SqlDbType.Int);
					spParams[1].Value = (int)theAlert.Category;
					spParams[2] = new SqlParameter("@cMessage", SqlDbType.VarChar, 510);
					spParams[2].Value = theAlert.Message;
					spParams[3] = new SqlParameter("@cField1", SqlDbType.VarChar, 1020);
					spParams[3].Value = theAlert.Field1;
					spParams[4] = new SqlParameter("@cField2", SqlDbType.VarChar, 1020);
					spParams[4].Value = theAlert.Field2;
					spParams[5] = new SqlParameter("@cAssetName", SqlDbType.VarChar, 255);
					spParams[5].Value = theAlert.AssetName;
					spParams[6] = new SqlParameter("@cAlertName", SqlDbType.VarChar, 255);
					spParams[6].Value = theAlert.AlertName;
					spParams[7] = new SqlParameter("@nReturnID", SqlDbType.Int);
					spParams[7].Direction = ParameterDirection.ReturnValue;

					// Execute the stored procedure
					SqlCommand cmd = new SqlCommand("usp_alert_add", conn);
					cmd.CommandType = CommandType.StoredProcedure;
					cmd.Parameters.AddRange(spParams);
					cmd.ExecuteNonQuery();
					itemID = (int)spParams[7].Value;
				}
				catch (SqlException e)
				{
					ourLog.Write("Exception adding an alert, the message was " + e.Message, true);
					this.HandleSqlError(e);
				}
				finally
				{
					conn.Dispose();
					conn.Close();
				}
			}
			return itemID;
		}


		/// <summary>
		/// Update the last alert date for the specified alert
		/// </summary>
		/// <param name="theAsset"></param>
		/// <returns></returns>
		public int AlertDateUpdate(Alert theAlert)
		{
			using (SqlConnection conn = CreateOpenConnection())
			{
				try
				{
					SqlParameter[] spParams = new SqlParameter[2];
					spParams[0] = new SqlParameter("@nAlertID", SqlDbType.Int);
					spParams[0].Value = theAlert.AlertID;
					spParams[1] = new SqlParameter("@dtAlertDate", SqlDbType.DateTime);
					spParams[1].Value = theAlert.AlertedOnDate;
					//
					SqlCommand cmd = new SqlCommand("usp_alert_update", conn);
					cmd.CommandType = CommandType.StoredProcedure;
					cmd.Parameters.AddRange(spParams);
					cmd.ExecuteNonQuery();
				}
				catch (SqlException e)
				{
					this.HandleSqlError(e);
				}
				finally
				{
					conn.Dispose();
					conn.Close();
				}
			}
			return 0;
		}



		/// <summary>
		/// Dismiss the specified Alert - this does not actually delete the alert - just marks it as dismissed
		/// </summary>
		/// <param name="licenseID"></param>
		/// <returns></returns>
		public int AlertSetStatus(Alert theAlert)
		{
			if (theAlert.AlertID != 0)
			{
				using (SqlConnection conn = CreateOpenConnection())
				{
					try
					{
						SqlParameter[] spParams = new SqlParameter[2];
						spParams[0] = new SqlParameter("@nAlertID", SqlDbType.Int);
						spParams[0].Value = theAlert.AlertID;
						spParams[0] = new SqlParameter("@nStatus", SqlDbType.Int);
						spParams[0].Value = (int)theAlert.Status;
						//
						SqlCommand cmd = new SqlCommand("usp_alert_set_status", conn);
						cmd.CommandType = CommandType.StoredProcedure;
						cmd.Parameters.AddRange(spParams);
						cmd.ExecuteNonQuery();
					}
					catch (SqlException e)
					{
						this.HandleSqlError(e);
					}
					finally
					{
						conn.Dispose();
						conn.Close();
					}
				}
			}
			return 0;
		}


		/// <summary>
		/// Delete the specified Alert from the database
		/// </summary>
		/// <param name="licenseID"></param>
		/// <returns></returns>
		public int AlertDelete(Alert theAlert)
		{
			if (theAlert.AlertID != 0)
			{
				using (SqlConnection conn = CreateOpenConnection())
				{
					try
					{
						SqlParameter[] spParams = new SqlParameter[1];
						spParams[0] = new SqlParameter("@nAlertID", SqlDbType.Int);
						spParams[0].Value = theAlert.AlertID;
						//
						SqlCommand cmd = new SqlCommand("usp_alert_delete", conn);
						cmd.CommandType = CommandType.StoredProcedure;
						cmd.Parameters.AddRange(spParams);
						cmd.ExecuteNonQuery();
					}
					catch (SqlException e)
					{
						this.HandleSqlError(e);
					}
					finally
					{
						conn.Dispose();
						conn.Close();
					}
				}
			}
			return 0;
		}




		/// <summary>
		/// Purge Alerts from the database
		/// </summary>
		/// <param name="dtBefore"></param>
		/// <returns></returns>
		public int AlertPurge(DateTime dtBeforeDate)
		{
			using (SqlConnection conn = CreateOpenConnection())
			{
				try
				{
					SqlParameter[] spParams = new SqlParameter[1];
					spParams[0] = new SqlParameter("@dtPurgeDate", SqlDbType.DateTime);
					spParams[0].Value = dtBeforeDate;
					//
					SqlCommand cmd = new SqlCommand("usp_alert_purge", conn);
					cmd.CommandType = CommandType.StoredProcedure;
					cmd.Parameters.AddRange(spParams);
					cmd.ExecuteNonQuery();
				}
				catch (SqlException e)
				{
					this.HandleSqlError(e);
				}
				finally
				{
					conn.Dispose();
					conn.Close();
				}
			}
			return 0;
		}

		#endregion Alerts Table

		#region Actions Table


		/// <summary>
		/// Return a table containing all of the Actions that have been defined
		/// </summary>
		/// <returns></returns>
		public DataTable EnumerateActions()
		{
			DataTable actionTable = new DataTable(TableNames.ACTIONS);
			using (SqlConnection conn = CreateOpenConnection())
			{
				try
				{
					SqlCommand cmd = new SqlCommand("usp_action_enumerate", conn);
					cmd.CommandType = CommandType.StoredProcedure;
					new SqlDataAdapter(cmd).Fill(actionTable);
				}
				catch (SqlException ex)
				{
					this.HandleSqlError(ex);
				}
				finally
				{
					conn.Dispose();
					conn.Close();
				}
			}
			return actionTable;
		}


		/// <summary>
		/// Add a new Action to the database
		/// </summary>
		/// <param name="theAlert"></param>
		/// <returns></returns>
		public int ActionAdd(Action theAction)
		{
			int itemID = 0;
			using (SqlConnection conn = CreateOpenConnection())
			{
				try
				{
					SqlParameter[] spParams = new SqlParameter[6];
					spParams[0] = new SqlParameter("@nType", SqlDbType.Int);
					spParams[0].Value = (int)theAction.ActionType;
					spParams[1] = new SqlParameter("@nApplicationID", SqlDbType.Int);
					spParams[1].Value = (int)theAction.ApplicationID;
					spParams[2] = new SqlParameter("@nStatus", SqlDbType.Int);
					spParams[2].Value = (int)theAction.Status;
					spParams[3] = new SqlParameter("@cAssets", SqlDbType.VarChar, 5000);
					spParams[3].Value = PrepareSqlString(theAction.AssociatedAssets ,5000);
					spParams[4] = new SqlParameter("@cNotes", SqlDbType.VarChar, 1020);
					spParams[4].Value = PrepareSqlString(theAction.Notes, 1020);
					spParams[5] = new SqlParameter("@nReturnID", SqlDbType.Int);
					spParams[5].Direction = ParameterDirection.ReturnValue;

					// Execute the stored procedure
					SqlCommand cmd = new SqlCommand("usp_action_add", conn);
					cmd.CommandType = CommandType.StoredProcedure;
					cmd.Parameters.AddRange(spParams);
					cmd.ExecuteNonQuery();
					itemID = (int)spParams[5].Value;
				}
				catch (SqlException e)
				{
					this.HandleSqlError(e);
				}
				finally
				{
					conn.Dispose();
					conn.Close();
				}
			}
			return itemID;
		}


		/// <summary>
		/// Update the specified action
		/// </summary>
		/// <param name="theAsset"></param>
		/// <returns></returns>
		public int ActionUpdate(Action theAction)
		{
			using (SqlConnection conn = CreateOpenConnection())
			{
				try
				{
					SqlParameter[] spParams = new SqlParameter[4];
					spParams[0] = new SqlParameter("@nActionID", SqlDbType.Int);
					spParams[0].Value = theAction.ActionID;
					spParams[1] = new SqlParameter("@nStatus", SqlDbType.Int);
					spParams[1].Value = (int)theAction.Status;
					spParams[2] = new SqlParameter("@cAssets", SqlDbType.VarChar, 5000);
					spParams[2].Value = PrepareSqlString(theAction.AssociatedAssets, 5000);
					spParams[3] = new SqlParameter("@cNotes", SqlDbType.VarChar, 1020);
					spParams[3].Value = PrepareSqlString(theAction.Notes, 1020);
					//
					SqlCommand cmd = new SqlCommand("usp_action_update", conn);
					cmd.CommandType = CommandType.StoredProcedure;
					cmd.Parameters.AddRange(spParams);
					cmd.ExecuteNonQuery();
				}
				catch (SqlException e)
				{
					this.HandleSqlError(e);
				}
				finally
				{
					conn.Dispose();
					conn.Close();
				}
			}
			return 0;
		}



		/// <summary>
		/// Delete the specified Action from the database
		/// </summary>
		/// <param name="licenseID"></param>
		/// <returns></returns>
		public int ActionDelete(Action theAction)
		{
			if (theAction.ActionID != 0)
			{
				using (SqlConnection conn = CreateOpenConnection())
				{
					try
					{
						SqlParameter[] spParams = new SqlParameter[1];
						spParams[0] = new SqlParameter("@nActionID", SqlDbType.Int);
						spParams[0].Value = theAction.ActionID;
						//
						SqlCommand cmd = new SqlCommand("usp_action_delete", conn);
						cmd.CommandType = CommandType.StoredProcedure;
						cmd.Parameters.AddRange(spParams);
						cmd.ExecuteNonQuery();
					}
					catch (SqlException e)
					{
						this.HandleSqlError(e);
					}
					finally
					{
						conn.Dispose();
						conn.Close();
					}
				}
			}
			return 0;
		}


		#endregion Action Table

		#region Suppliers Table


		/// <summary>
		/// Return a table containing all of the Alerts have been triggered since the specified date
		/// </summary>
		/// <returns></returns>
		public DataTable EnumerateSuppliers()
		{
			DataTable supplierTable = new DataTable(TableNames.ALERTS);
			using (SqlConnection conn = CreateOpenConnection())
			{
				try
				{
					SqlCommand cmd = new SqlCommand("usp_supplier_enumerate", conn);
					cmd.CommandType = CommandType.StoredProcedure;
					new SqlDataAdapter(cmd).Fill(supplierTable);
				}
				catch (SqlException ex)
				{
					this.HandleSqlError(ex);
				}
				finally
				{
					conn.Dispose();
					conn.Close();
				}
			}
			return supplierTable;
		}


		/// <summary>
		/// Add a new Supplier to the database
		/// </summary>
		/// <param name="theAlert"></param>
		/// <returns></returns>
		public int SupplierAdd(Supplier theSupplier)
		{
			int itemID = 0;

			using (SqlConnection conn = CreateOpenConnection())
			{
				try
				{
					SqlParameter[] spParams = new SqlParameter[13];
					spParams[0] = new SqlParameter("@cName", SqlDbType.VarChar, 255);
					spParams[0].Value = PrepareSqlString(theSupplier.Name, 255);
					spParams[1] = new SqlParameter("@cAddress1", SqlDbType.VarChar, 255);
					spParams[1].Value = PrepareSqlString(theSupplier.AddressLine1, 255);
					spParams[2] = new SqlParameter("@cAddress2", SqlDbType.VarChar, 255);
					spParams[2].Value = PrepareSqlString(theSupplier.AddressLine2, 255);
					spParams[3] = new SqlParameter("@cCity", SqlDbType.VarChar, 255);
					spParams[3].Value = PrepareSqlString(theSupplier.City, 255);
					spParams[4] = new SqlParameter("@cState", SqlDbType.VarChar, 255);
					spParams[4].Value = PrepareSqlString(theSupplier.State, 255);
					spParams[5] = new SqlParameter("@cZip", SqlDbType.VarChar, 64);
					spParams[5].Value = PrepareSqlString(theSupplier.Zip, 64);
					spParams[6] = new SqlParameter("@cTelephone", SqlDbType.VarChar, 64);
					spParams[6].Value = PrepareSqlString(theSupplier.Telephone, 64);
					spParams[7] = new SqlParameter("@cContactName", SqlDbType.VarChar, 255);
					spParams[7].Value = PrepareSqlString(theSupplier.Contact, 255);
					spParams[8] = new SqlParameter("@cContactEmail", SqlDbType.VarChar, 255);
					spParams[8].Value = PrepareSqlString(theSupplier.Email, 255);
					spParams[9] = new SqlParameter("@cWWW", SqlDbType.VarChar, 255);
					spParams[9].Value = PrepareSqlString(theSupplier.WWW, 255);
					spParams[10] = new SqlParameter("@cFax", SqlDbType.VarChar, 255);
					spParams[10].Value = PrepareSqlString(theSupplier.Fax, 255);
					spParams[11] = new SqlParameter("@cNotes", SqlDbType.VarChar, 1020);
					spParams[11].Value = PrepareSqlString(theSupplier.Notes, 1020);
					spParams[12] = new SqlParameter("@nReturnID", SqlDbType.Int);
					spParams[12].Direction = ParameterDirection.ReturnValue;

					// Execute the stored procedure
					SqlCommand cmd = new SqlCommand("usp_supplier_add", conn);
					cmd.CommandType = CommandType.StoredProcedure;
					cmd.Parameters.AddRange(spParams);
					cmd.ExecuteNonQuery();
					itemID = (int)spParams[12].Value;
				}
				catch (SqlException e)
				{
					this.HandleSqlError(e);
				}
				finally
				{
					conn.Dispose();
					conn.Close();
				}
			}
			return itemID;
		}


		/// <summary>
		/// Update the Supplier Information
		/// </summary>
		/// <param name="theAsset"></param>
		/// <returns></returns>
		public int SupplierUpdate(Supplier theSupplier)
		{
			using (SqlConnection conn = CreateOpenConnection())
			{
				try
				{
					SqlParameter[] spParams = new SqlParameter[13];
					spParams[0] = new SqlParameter("@nSupplierID", SqlDbType.Int);
					spParams[0].Value = theSupplier.SupplierID;
					spParams[1] = new SqlParameter("@cName", SqlDbType.VarChar, 255);
					spParams[1].Value = theSupplier.Name;
					spParams[2] = new SqlParameter("@cAddress1", SqlDbType.VarChar, 255);
					spParams[2].Value = theSupplier.AddressLine1;
					spParams[3] = new SqlParameter("@cAddress2", SqlDbType.VarChar, 255);
					spParams[3].Value = theSupplier.AddressLine2;
					spParams[4] = new SqlParameter("@cCity", SqlDbType.VarChar, 255);
					spParams[4].Value = theSupplier.City;
					spParams[5] = new SqlParameter("@cState", SqlDbType.VarChar, 255);
					spParams[5].Value = theSupplier.State;
					spParams[6] = new SqlParameter("@cZip", SqlDbType.VarChar, 64);
					spParams[6].Value = theSupplier.Zip;
					spParams[7] = new SqlParameter("@cTelephone", SqlDbType.VarChar, 64);
					spParams[7].Value = theSupplier.Telephone;
					spParams[8] = new SqlParameter("@cContactName", SqlDbType.VarChar, 255);
					spParams[8].Value = theSupplier.Contact;
					spParams[9] = new SqlParameter("@cContactEmail", SqlDbType.VarChar, 255);
					spParams[9].Value = theSupplier.Email;
					spParams[10] = new SqlParameter("@cWWW", SqlDbType.VarChar, 255);
					spParams[10].Value = theSupplier.WWW;
					spParams[11] = new SqlParameter("@cFax", SqlDbType.VarChar, 255);
					spParams[11].Value = theSupplier.Fax;
					spParams[12] = new SqlParameter("@cNotes", SqlDbType.VarChar, 1020);
					spParams[12].Value = theSupplier.Notes;
					//
					SqlCommand cmd = new SqlCommand("usp_supplier_update", conn);
					cmd.CommandType = CommandType.StoredProcedure;
					cmd.Parameters.AddRange(spParams);
					cmd.ExecuteNonQuery();
				}
				catch (SqlException e)
				{
					this.HandleSqlError(e);
				}
				finally
				{
					conn.Dispose();
					conn.Close();
				}
			}
			return 0;
		}



		/// <summary>
		/// Delete the specified Supplier from the database
		/// </summary>
		/// <param name="supplierID"></param>
		/// <returns></returns>
		public int SupplierDelete(Supplier theSupplier)
		{
			if (theSupplier.SupplierID != 0)
			{
				using (SqlConnection conn = CreateOpenConnection())
				{
					try
					{
						SqlParameter[] spParams = new SqlParameter[1];
						spParams[0] = new SqlParameter("@nSupplierID", SqlDbType.Int);
						spParams[0].Value = theSupplier.SupplierID;
						//
						SqlCommand cmd = new SqlCommand("usp_supplier_delete", conn);
						cmd.CommandType = CommandType.StoredProcedure;
						cmd.Parameters.AddRange(spParams);
						cmd.ExecuteNonQuery();
					}
					catch (SqlException e)
					{
						this.HandleSqlError(e);
					}
					finally
					{
						conn.Dispose();
						conn.Close();
					}
				}
			}
			return 0;
		}


		/// <summary>
		/// Return the database index of the specified Supplier Record
		/// </summary>
		/// <param name="name"></param>
		/// <returns></returns>
		public int SupplierFind(string name)
		{
			int itemId = 0;
			using (SqlConnection conn = CreateOpenConnection())
			{
				try
				{
					SqlParameter[] spParams = new SqlParameter[2];
					spParams[0] = new SqlParameter("@cName", SqlDbType.VarChar);
					spParams[0].Size = 255;
					spParams[0].Value = name;
					spParams[1] = new SqlParameter("@nReturnID", SqlDbType.Int);
					spParams[1].Direction = ParameterDirection.ReturnValue;
					//
					SqlCommand cmd = new SqlCommand("usp_supplier_find", conn);
					cmd.CommandType = CommandType.StoredProcedure;
					cmd.Parameters.AddRange(spParams);
					cmd.ExecuteNonQuery();
					itemId = (int)spParams[1].Value;
				}
				catch (SqlException e)
				{
					this.HandleSqlError(e);
				}
				finally
				{
					conn.Dispose();
					conn.Close();
				}
			}
			return itemId;
		}

#endregion Suppliers Table

		#region User Defined Data Definitions


		/// <summary>
		/// Return a table containing all of the User Defined Data Field Definitions (of the specified type)
		/// </summary>
		/// <returns></returns>
		public DataTable EnumerateUserDataDefinitions(UserDataCategory.SCOPE scope)
		{
			DataTable uddTable = new DataTable(TableNames.UDDD);
			using (SqlConnection conn = CreateOpenConnection())
			{
				try
				{
					SqlParameter[] spParams = new SqlParameter[1];
					spParams[0] = new SqlParameter("@nScope", SqlDbType.Int);
					spParams[0].Value = (int)scope;
					SqlCommand cmd = new SqlCommand("usp_udd_enumerate", conn);
					cmd.CommandType = CommandType.StoredProcedure;
					cmd.Parameters.AddRange(spParams);
					new SqlDataAdapter(cmd).Fill(uddTable);
				}
				catch (SqlException ex)
				{
					this.HandleSqlError(ex);
				}
				finally
				{
					conn.Dispose();
					conn.Close();
				}
			}
			return uddTable;
		}



		/// <summary>
		/// Add a new User Data Category Definition to the database
		/// </summary>
		/// <param name="theAlert"></param>
		/// <returns></returns>
		public int UserDataCategoryAdd(UserDataCategory category)
		{
			int itemID = 0;

			using (SqlConnection conn = CreateOpenConnection())
			{
				try
				{
					SqlParameter[] spParams = new SqlParameter[11];
					spParams[0] = new SqlParameter("@cName", SqlDbType.VarChar, 255);
					spParams[0].Value = category.Name;
					spParams[1] = new SqlParameter("@nParentID", SqlDbType.Int);	// Parent set to null for category
					spParams[1].IsNullable = true;
					spParams[1].Value = DBNull.Value;
					spParams[2] = new SqlParameter("@bIsMandatory", SqlDbType.Bit);
					spParams[2].Value = 0;
					spParams[3] = new SqlParameter("@nType", SqlDbType.Int);
					spParams[3].Value = 0;
					spParams[4] = new SqlParameter("@nAppliesTo" ,SqlDbType.Int);
					spParams[4].Value = category.AppliesTo;
					spParams[5] = new SqlParameter("@cValue1", SqlDbType.VarChar, 255);
					spParams[5].Value = "";
					spParams[6] = new SqlParameter("@cValue2", SqlDbType.VarChar, 255);
					spParams[6].Value = "";
					spParams[7] = new SqlParameter("@nTabOrder", SqlDbType.Int);
					spParams[7].Value = category.TabOrder;
					spParams[8] = new SqlParameter("@cIcon", SqlDbType.VarChar, 255);
					spParams[8].Value = category.Icon;
					spParams[9] = new SqlParameter("@nScope", SqlDbType.Int);
					spParams[9].Value = (int)category.Scope;
					spParams[10] = new SqlParameter("@nReturnID", SqlDbType.Int);
					spParams[10].Direction = ParameterDirection.Output;

					// Execute the stored procedure
					SqlCommand cmd = new SqlCommand("usp_udd_add", conn);
					cmd.CommandType = CommandType.StoredProcedure;
					cmd.Parameters.AddRange(spParams);
					cmd.ExecuteNonQuery();
					itemID = (int)spParams[10].Value;
				}
				catch (SqlException e)
				{
					this.HandleSqlError(e);
				}
				finally
				{
					conn.Dispose();
					conn.Close();
				}
			}
			return itemID;
		}
		
		
		
		/// <summary>
		/// Add a new User Data Field Definition to the database
		/// </summary>
		/// <param name="theAlert"></param>
		/// <returns></returns>
		public int UserDataFieldAdd(UserDataField field)
		{
			int itemID = 0;

			using (SqlConnection conn = CreateOpenConnection())
			{
				try
				{
					SqlParameter[] spParams = new SqlParameter[11];
					spParams[0] = new SqlParameter("@cName", SqlDbType.VarChar, 255);
					spParams[0].Value = field.Name;
					spParams[1] = new SqlParameter("@nParentID", SqlDbType.Int);
					spParams[1].Value = field.ParentID;
					spParams[2] = new SqlParameter("@bIsMandatory", SqlDbType.Bit);
					spParams[2].Value = (field.IsMandatory) ? 1 : 0;
					spParams[3] = new SqlParameter("@nType", SqlDbType.Int);
					spParams[3].Value = (int)field.Type;
					spParams[4] = new SqlParameter("@nAppliesTo", SqlDbType.Int);
					spParams[4].Value = 0;
					spParams[5] = new SqlParameter("@cValue1", SqlDbType.VarChar, 255);
					spParams[5].Value = field.Value1;
					spParams[6] = new SqlParameter("@cValue2", SqlDbType.VarChar, 255);
					spParams[6].Value = field.Value2;
					spParams[7] = new SqlParameter("@nTabOrder", SqlDbType.Int);
					spParams[7].Value = field.TabOrder;
					spParams[8] = new SqlParameter("@cIcon", SqlDbType.VarChar, 255);
					spParams[8].Value = "";
					spParams[9] = new SqlParameter("@nScope", SqlDbType.Int);
					spParams[9].Value = (int)field.ParentScope;
					spParams[10] = new SqlParameter("@nReturnID", SqlDbType.Int);
					spParams[10].Direction = ParameterDirection.Output;

					// Execute the stored procedure
					SqlCommand cmd = new SqlCommand("usp_udd_add", conn);
					cmd.CommandType = CommandType.StoredProcedure;
					cmd.Parameters.AddRange(spParams);
					cmd.ExecuteNonQuery();
					itemID = (int)spParams[10].Value;
				}
				catch (SqlException e)
				{
					this.HandleSqlError(e);
				}
				finally
				{
					conn.Dispose();
					conn.Close();
				}
			}
			return itemID;
		}


		/// <summary>
		/// Update the definintion for a User Data Category
		/// </summary>
		/// <param name="category"></param>
		/// <returns></returns>
		public int UserDataCategoryUpdate (UserDataCategory category)
		{
			using (SqlConnection conn = CreateOpenConnection())
			{
				try
				{
					SqlParameter[] spParams = new SqlParameter[10];
					spParams[0] = new SqlParameter("@nUserDefID", SqlDbType.Int);
					spParams[0].Value = category.CategoryID;
					spParams[1] = new SqlParameter("@nCategoryID", SqlDbType.Int);
					spParams[1].IsNullable = true;
					spParams[1].Value = DBNull.Value;
					spParams[2] = new SqlParameter("@cName", SqlDbType.VarChar, 255);
					spParams[2].Value = category.Name;
					spParams[3] = new SqlParameter("@bIsMandatory", SqlDbType.Bit);
					spParams[3].Value = 0;
					spParams[4] = new SqlParameter("@nType", SqlDbType.Int);
					spParams[4].Value = 0;
					spParams[5] = new SqlParameter("@nAppliesTo", SqlDbType.Int);
					spParams[5].Value = category.AppliesTo;
					spParams[6] = new SqlParameter("@cValue1", SqlDbType.VarChar, 255);
					spParams[6].Value = "";
					spParams[7] = new SqlParameter("@cValue2", SqlDbType.VarChar, 255);
					spParams[7].Value = "";
					spParams[8] = new SqlParameter("@nTabOrder", SqlDbType.Int);
					spParams[8].Value = category.TabOrder;
					spParams[9] = new SqlParameter("@cIcon", SqlDbType.VarChar, 255);
					spParams[9].Value = category.Icon;
					//
					SqlCommand cmd = new SqlCommand("usp_udd_update", conn);
					cmd.CommandType = CommandType.StoredProcedure;
					cmd.Parameters.AddRange(spParams);
					cmd.ExecuteNonQuery();
				}
				catch (SqlException e)
				{
					this.HandleSqlError(e);
				}
				finally
				{
					conn.Dispose();
					conn.Close();
				}
			}
			return 0;
		}



		/// <summary>
		/// Update the definintion for a User Data Field
		/// </summary>
		/// <param name="category"></param>
		/// <returns></returns>
		public int UserDataFieldUpdate(UserDataField field)
		{
			using (SqlConnection conn = CreateOpenConnection())
			{
				try
				{
					SqlParameter[] spParams = new SqlParameter[10];
					spParams[0] = new SqlParameter("@nUserDefID", SqlDbType.Int);
					spParams[0].Value = field.FieldID;
					spParams[1] = new SqlParameter("@nCategoryID", SqlDbType.Int);
					spParams[1].Value = field.ParentID;
					spParams[2] = new SqlParameter("@cName", SqlDbType.VarChar, 255);
					spParams[2].Value = field.Name;
					spParams[3] = new SqlParameter("@bIsMandatory", SqlDbType.Bit);
					spParams[3].Value = (field.IsMandatory) ? 1 : 0;
					spParams[4] = new SqlParameter("@nType", SqlDbType.Int);
					spParams[4].Value = (int)field.Type;
					spParams[5] = new SqlParameter("@nAppliesTo", SqlDbType.Int);
					spParams[5].Value = 0;
					spParams[6] = new SqlParameter("@cValue1", SqlDbType.VarChar, 255);
					spParams[6].Value = field.Value1;
					spParams[7] = new SqlParameter("@cValue2", SqlDbType.VarChar, 255);
					spParams[7].Value = field.Value2;
					spParams[8] = new SqlParameter("@nTabOrder", SqlDbType.Int);
					spParams[8].Value = field.TabOrder;
					spParams[9] = new SqlParameter("@cIcon", SqlDbType.VarChar, 255);
					spParams[9].Value = "";
					//
					SqlCommand cmd = new SqlCommand("usp_udd_update", conn);
					cmd.CommandType = CommandType.StoredProcedure;
					cmd.Parameters.AddRange(spParams);
					cmd.ExecuteNonQuery();
				}
				catch (SqlException e)
				{
					this.HandleSqlError(e);
				}
				finally
				{
					conn.Dispose();
					conn.Close();
				}
			}
			return 0;
		}



		/// <summary>
		/// Delete the specified User Data Category / Field from the database
		/// Note that it is the responsibility of the caller to ensure that the referential integrity of the
		/// database is retained - i.e. do not delete items that are still being referenced
		/// </summary>
		/// <param name="licenseID"></param>
		/// <returns></returns>
		public int UserDataDefinitionDelete(int userDefID)
		{
			int references = 0;

			// The delete code will perform a sanity check to ensure that we do not
			// delete user defined categories/fields which are still referenced so we simply delete it now
			using (SqlConnection conn = CreateOpenConnection())
			{
				try
				{
					SqlParameter[] spParams = new SqlParameter[2];
					spParams[0] = new SqlParameter("@nUserDataID", SqlDbType.Int);
					spParams[0].Value = userDefID;
					spParams[1] = new SqlParameter("@nReferences", SqlDbType.Int);
					spParams[1].Direction = ParameterDirection.ReturnValue;
					//
					SqlCommand cmd = new SqlCommand("usp_udd_delete", conn);
					cmd.CommandType = CommandType.StoredProcedure;
					cmd.Parameters.AddRange(spParams);
					cmd.ExecuteNonQuery();
					references = (int)spParams[1].Value;
				}
				catch (SqlException e)
				{
					this.HandleSqlError(e);
				}
				finally
				{
					conn.Dispose();
					conn.Close();
				}
			}
			return references;
		}



		/// <summary>
		/// Return a table containing all of the values for the specified User Defined Data Category
		/// for the specified item
		/// </summary>
		/// <returns></returns>
		public DataTable EnumerateUserDataValues (UserDataCategory.SCOPE scope ,int parentID ,int categoryID)
		{
			DataTable table = new DataTable(TableNames.UDDD);
			using (SqlConnection conn = CreateOpenConnection())
			{
				try
				{
					SqlParameter[] spParams = new SqlParameter[3];
					spParams[0] = new SqlParameter("@nParentType", SqlDbType.Int);
					spParams[0].Value = (int)scope;
					spParams[1] = new SqlParameter("@nParentID", SqlDbType.Int);
					spParams[1].Value = parentID;
					spParams[2] = new SqlParameter("@nCategoryID", SqlDbType.Int);
					spParams[2].Value = categoryID;
					SqlCommand cmd = new SqlCommand("usp_userdata_getvalues", conn);
					cmd.CommandType = CommandType.StoredProcedure;
					cmd.Parameters.AddRange(spParams);
					new SqlDataAdapter(cmd).Fill(table);
				}
				catch (SqlException ex)
				{
					this.HandleSqlError(ex);
				}
				finally
				{
					conn.Dispose();
					conn.Close();
				}
			}
			return table;
		}



		/// <summary>
		/// Delete the specified User Data Category / Field from the database
		/// Note that it is the responsibility of the caller to ensure that the referential integrity of the
		/// database is retained - i.e. do not delete items that are still being referenced
		/// </summary>
		/// <param name="licenseID"></param>
		/// <returns></returns>
		public int UserDataUpdateValue(UserDataCategory.SCOPE scope ,int parentID ,int fieldID ,string value)
		{
			int returnID = 0;

			using (SqlConnection conn = CreateOpenConnection())
			{
				try
				{
					SqlParameter[] spParams = new SqlParameter[5];
					spParams[0] = new SqlParameter("@nScope", SqlDbType.Int);
					spParams[0].Value = (int)scope;
					spParams[1] = new SqlParameter("@nParentID", SqlDbType.Int);
					spParams[1].Value = parentID;
					spParams[2] = new SqlParameter("@nUserDefID", SqlDbType.Int);
					spParams[2].Value = fieldID;
					spParams[3] = new SqlParameter("@cValue", SqlDbType.VarChar ,1020);
					spParams[3].Value = value;
					spParams[4] = new SqlParameter("@nReturnID", SqlDbType.Int);
					spParams[4].Direction = ParameterDirection.Output;
					//
					SqlCommand cmd = new SqlCommand("usp_userdata_update_value", conn);
					cmd.CommandType = CommandType.StoredProcedure;
					cmd.Parameters.AddRange(spParams);
					cmd.ExecuteNonQuery();
					returnID = (int)spParams[4].Value;
				}
				catch (SqlException e)
				{
					this.HandleSqlError(e);
				}
				finally
				{
					conn.Dispose();
					conn.Close();
				}
			}
			return returnID;
		}


		/// <summary>
		/// Return a table containing all of the values for the specified User Defined Data Field
		/// </summary>
		/// <returns></returns>
		public DataTable EnumerateUserDataFieldValues(int fieldID)
		{
			DataTable table = new DataTable(TableNames.UDDD);
			using (SqlConnection conn = CreateOpenConnection())
			{
				try
				{
					SqlParameter[] spParams = new SqlParameter[1];
					spParams[0] = new SqlParameter("@nFieldID", SqlDbType.Int);
					spParams[0].Value = (int)fieldID;
					SqlCommand cmd = new SqlCommand("usp_userdatafield_getvalues", conn);
					cmd.CommandType = CommandType.StoredProcedure;
					cmd.Parameters.AddRange(spParams);
					new SqlDataAdapter(cmd).Fill(table);
				}
				catch (SqlException ex)
				{
					this.HandleSqlError(ex);
				}
				finally
				{
					conn.Dispose();
					conn.Close();
				}
			}
			return table;
		}

		#endregion User Defined data Definitions
	
		#region Asset Types Table

		/// <summary>
		/// Return a table containing all of the Asset Types which have been defined
		/// </summary>
		/// <returns></returns>
		public DataTable EnumerateAssetTypes()
		{
			DataTable table = new DataTable(TableNames.ASSET_TYPES);
			using (SqlConnection conn = CreateOpenConnection())
			{
				try
				{
					SqlCommand cmd = new SqlCommand("usp_assettype_enumerate", conn);
					cmd.CommandType = CommandType.StoredProcedure;
					new SqlDataAdapter(cmd).Fill(table);
				}
				catch (SqlException ex)
				{
					this.HandleSqlError(ex);
				}
				finally
				{
					conn.Dispose();
					conn.Close();
				}
			}
			return table;
		}


		/// <summary>
		/// Add a new Asset Type to the database
		/// </summary>
		/// <param name="theAlert"></param>
		/// <returns></returns>
		public int AssetTypeAdd(AssetType assettype)
		{
			int itemID = 0;

			using (SqlConnection conn = CreateOpenConnection())
			{
				try
				{
					SqlParameter[] spParams = new SqlParameter[5];
					spParams[0] = new SqlParameter("@cName", SqlDbType.VarChar, 255);
					spParams[0].Value = assettype.Name;
					spParams[1] = new SqlParameter("@nParentID", SqlDbType.Int);
					spParams[1].IsNullable = true;
					if (assettype.ParentID == 0)
						spParams[1].Value = DBNull.Value;
					else
						spParams[1].Value = assettype.ParentID;
					spParams[2] = new SqlParameter("@bAuditable", SqlDbType.Bit);
					spParams[2].Value = assettype.Auditable;
					spParams[3] = new SqlParameter("@cIcon", SqlDbType.VarChar, 255);
					spParams[3].Value = assettype.Icon;
					spParams[4] = new SqlParameter("@nReturnID", SqlDbType.Int);
					spParams[4].Direction = ParameterDirection.Output;

					// Execute the stored procedure
					SqlCommand cmd = new SqlCommand("usp_assettype_add", conn);
					cmd.CommandType = CommandType.StoredProcedure;
					cmd.Parameters.AddRange(spParams);
					cmd.ExecuteNonQuery();
					itemID = (int)spParams[4].Value;
				}
				catch (SqlException e)
				{
					this.HandleSqlError(e);
				}
				finally
				{
					conn.Dispose();
					conn.Close();
				}
			}
			return itemID;
		}


		/// <summary>
		/// Update the specified Asset Type
		/// </summary>
		/// <param name="theAsset"></param>
		/// <returns></returns>
		public int AssetTypeUpdate(AssetType assettype)
		{
			using (SqlConnection conn = CreateOpenConnection())
			{
				try
				{
					SqlParameter[] spParams = new SqlParameter[5];
					spParams[0] = new SqlParameter("@nAssetTypeID", SqlDbType.Int);
					spParams[0].Value = assettype.AssetTypeID;
					spParams[1] = new SqlParameter("@cName", SqlDbType.VarChar, 255);
					spParams[1].Value = assettype.Name;
					spParams[2] = new SqlParameter("@nParentID", SqlDbType.Int);
					spParams[2].IsNullable = true;
					if (assettype.ParentID == 0)
						spParams[2].Value = DBNull.Value;
					else
						spParams[2].Value = assettype.ParentID;
					spParams[3] = new SqlParameter("@cIcon", SqlDbType.VarChar, 255);
					spParams[3].Value = assettype.Icon;
					spParams[4] = new SqlParameter("@nReturnID", SqlDbType.Int);
					spParams[4].Direction = ParameterDirection.ReturnValue;
					//
					SqlCommand cmd = new SqlCommand("usp_assettype_update", conn);
					cmd.CommandType = CommandType.StoredProcedure;
					cmd.Parameters.AddRange(spParams);
					cmd.ExecuteNonQuery();
				}
				catch (SqlException e)
				{
					this.HandleSqlError(e);
				}
				finally
				{
					conn.Dispose();
					conn.Close();
				}
			}
			return 0;
		}




		/// <summary>
		/// Delete the specified AssetType from the database
		/// </summary>
		/// <param name="licenseID"></param>
		/// <returns></returns>
		public int AssetTypeDelete(AssetType assetType)
		{
			int references = 0;

			// The asset type delete code will perform a sanity check to ensure that we do not
			// delete asset types which are still referenced so we simply delete it now
			using (SqlConnection conn = CreateOpenConnection())
			{
				try
				{
					SqlParameter[] spParams = new SqlParameter[2];
					spParams[0] = new SqlParameter("@nAssetTypeID", SqlDbType.Int);
					spParams[0].Value = assetType.AssetTypeID;
					spParams[1] = new SqlParameter("@nReferences", SqlDbType.Int);
					spParams[1].Direction = ParameterDirection.ReturnValue;
					//
					SqlCommand cmd = new SqlCommand("usp_assettype_delete", conn);
					cmd.CommandType = CommandType.StoredProcedure;
					cmd.Parameters.AddRange(spParams);
					cmd.ExecuteNonQuery();
					references = (int)spParams[1].Value;
				}
				catch (SqlException e)
				{
					this.HandleSqlError(e);
				}
				finally
				{
					conn.Dispose();
					conn.Close();
				}
			}
			return references;
		}


		#endregion Asset Types Table

		#region PickList Table


		/// <summary>
		/// Return a table containing all of the PickLists and PickItem entries
		/// </summary>
		/// <returns></returns>
		public DataTable EnumeratePickLists()
		{
			DataTable table = new DataTable(TableNames.UDDD);
			using (SqlConnection conn = CreateOpenConnection())
			{
				try
				{
					SqlCommand cmd = new SqlCommand("usp_picklist_enumerate", conn);
					cmd.CommandType = CommandType.StoredProcedure;
					new SqlDataAdapter(cmd).Fill(table);
				}
				catch (SqlException ex)
				{
					this.HandleSqlError(ex);
				}
				finally
				{
					conn.Dispose();
					conn.Close();
				}
			}
			return table;
		}

		/// <summary>
		/// Return a table containing all of the PickItem entries for the specfiied picklist
		/// </summary>
		/// <returns></returns>
		public DataTable EnumeratePickItems(int picklist)
		{
			DataTable table = new DataTable(TableNames.UDDD);
			using (SqlConnection conn = CreateOpenConnection())
			{
				try
				{
					SqlCommand cmd = new SqlCommand("usp_pickitem_enumerate", conn);
					SqlParameter[] spParams = new SqlParameter[1];
					spParams[0] = new SqlParameter("@nPicklistID", SqlDbType.Int);
					spParams[0].Value = picklist;
					cmd.CommandType = CommandType.StoredProcedure;
					cmd.Parameters.AddRange(spParams);
					new SqlDataAdapter(cmd).Fill(table);
				}
				catch (SqlException ex)
				{
					this.HandleSqlError(ex);
				}
				finally
				{
					conn.Dispose();
					conn.Close();
				}
			}
			return table;
		}



		/// <summary>
		/// Add a new PickList to the database
		/// </summary>
		/// <param name="theAlert"></param>
		/// <returns></returns>
		public int PickListAdd(PickList pickList)
		{
			int itemID = 0;

			using (SqlConnection conn = CreateOpenConnection())
			{
				try
				{
					SqlParameter[] spParams = new SqlParameter[3];
					spParams[0] = new SqlParameter("@cName", SqlDbType.VarChar, 255);
					spParams[0].Value = pickList.Name;
					spParams[1] = new SqlParameter("@nParentID", SqlDbType.Int);
					spParams[1].IsNullable = true;
					spParams[1].Value = DBNull.Value;
					spParams[2] = new SqlParameter("@nReturnID", SqlDbType.Int);
					spParams[2].Direction = ParameterDirection.Output;

					// Execute the stored procedure
					SqlCommand cmd = new SqlCommand("usp_picklist_add", conn);
					cmd.CommandType = CommandType.StoredProcedure;
					cmd.Parameters.AddRange(spParams);
					cmd.ExecuteNonQuery();
					itemID = (int)spParams[2].Value;
				}
				catch (SqlException e)
				{
					this.HandleSqlError(e);
				}
				finally
				{
					conn.Dispose();
					conn.Close();
				}
			}
			return itemID;
		}



		/// <summary>
		/// Add a new PickItem to the database
		/// </summary>
		/// <param name="theAlert"></param>
		/// <returns></returns>
		public int PickItemAdd(PickItem pickItem)
		{
			int itemID = 0;

			using (SqlConnection conn = CreateOpenConnection())
			{
				try
				{
					SqlParameter[] spParams = new SqlParameter[3];
					spParams[0] = new SqlParameter("@cName", SqlDbType.VarChar, 255);
					spParams[0].Value = pickItem.Name;
					spParams[1] = new SqlParameter("@nParentID", SqlDbType.Int);
					spParams[1].Value = pickItem.ParentID;
					spParams[2] = new SqlParameter("@nReturnID", SqlDbType.Int);
					spParams[2].Direction = ParameterDirection.Output;

					// Execute the stored procedure
					SqlCommand cmd = new SqlCommand("usp_picklist_add", conn);
					cmd.CommandType = CommandType.StoredProcedure;
					cmd.Parameters.AddRange(spParams);
					cmd.ExecuteNonQuery();
					itemID = (int)spParams[2].Value;
				}
				catch (SqlException e)
				{
					this.HandleSqlError(e);
				}
				finally
				{
					conn.Dispose();
					conn.Close();
				}
			}
			return itemID;
		}


		/// <summary>
		/// Update the definintion for a PickList
		/// </summary>
		/// <param name="category"></param>
		/// <returns></returns>
		public int PickListUpdate(PickList pickList)
		{
			using (SqlConnection conn = CreateOpenConnection())
			{
				try
				{
					SqlParameter[] spParams = new SqlParameter[2];
					spParams[0] = new SqlParameter("@nPicklistID", SqlDbType.Int);
					spParams[0].Value = pickList.PicklistID;
					spParams[1] = new SqlParameter("@cName", SqlDbType.VarChar, 255);
					spParams[1].Value = pickList.Name;
					//
					SqlCommand cmd = new SqlCommand("usp_picklist_update", conn);
					cmd.CommandType = CommandType.StoredProcedure;
					cmd.Parameters.AddRange(spParams);
					cmd.ExecuteNonQuery();
				}
				catch (SqlException e)
				{
					this.HandleSqlError(e);
				}
				finally
				{
					conn.Dispose();
					conn.Close();
				}
			}
			return 0;
		}



		/// <summary>
		/// Update the definintion for a PickItem
		/// </summary>
		/// <param name="category"></param>
		/// <returns></returns>
		public int PickItemUpdate(PickItem pickItem)
		{
			using (SqlConnection conn = CreateOpenConnection())
			{
				try
				{
					SqlParameter[] spParams = new SqlParameter[2];
					spParams[0] = new SqlParameter("@nPicklistID", SqlDbType.Int);
					spParams[0].Value = pickItem.PickItemID;
					spParams[1] = new SqlParameter("@cName", SqlDbType.VarChar, 255);
					spParams[1].Value = pickItem.Name;
					//
					SqlCommand cmd = new SqlCommand("usp_picklist_update", conn);
					cmd.CommandType = CommandType.StoredProcedure;
					cmd.Parameters.AddRange(spParams);
					cmd.ExecuteNonQuery();
				}
				catch (SqlException e)
				{
					this.HandleSqlError(e);
				}
				finally
				{
					conn.Dispose();
					conn.Close();
				}
			}
			return 0;
		}



		/// <summary>
		/// Delete the specified PickList from the database
		/// Note that it is the responsibility of the caller to ensure that the referential integrity of the
		/// database is retained - i.e. do not delete PickLists that are still being referenced
		/// </summary>
		/// <param name="licenseID"></param>
		/// <returns></returns>
		public int PickListDelete(int index)
		{
			// The delete code will perform a sanity check to ensure that we do not
			// delete user defined categories/fields which are still referenced so we simply delete it now
			using (SqlConnection conn = CreateOpenConnection())
			{
				try
				{
					SqlParameter[] spParams = new SqlParameter[1];
					spParams[0] = new SqlParameter("@nPicklistID", SqlDbType.Int);
					spParams[0].Value = index;
					//
					SqlCommand cmd = new SqlCommand("usp_picklist_delete", conn);
					cmd.CommandType = CommandType.StoredProcedure;
					cmd.Parameters.AddRange(spParams);
					cmd.ExecuteNonQuery();
				}
				catch (SqlException e)
				{
					this.HandleSqlError(e);
				}
				finally
				{
					conn.Dispose();
					conn.Close();
				}
			}
			return 0;
		}


		#endregion User Defined data Definitions

		#region Notes Table

		/// <summary>
		/// Return a table containing all of the Notes defined for an asset
		/// </summary>
		/// <returns></returns>
		public DataTable EnumerateNotes(Asset forAsset)
		{
			return EnumerateNotes(SCOPE.Asset ,forAsset.AssetID);
		}



		/// <summary>
		/// This function is the base function for returning notes for a specific item or the specified scope
		/// and database ID.  It is called from the public specific functions
		/// </summary>
		/// <param name="scope"></param>
		/// <param name="parentID"></param>
		/// <returns></returns>
		public DataTable EnumerateNotes (SCOPE scope ,int parentID)
		{
			DataTable table = new DataTable(TableNames.NOTES);
			using (SqlConnection conn = CreateOpenConnection())
			{
				try
				{
					SqlParameter[] spParams = new SqlParameter[2];
					spParams[0] = new SqlParameter("@nScope", SqlDbType.Int);
					spParams[0].Value = (int)scope;
					spParams[1] = new SqlParameter("@nParentID", SqlDbType.Int);
					spParams[1].Value = parentID;
					//
					SqlCommand cmd = new SqlCommand("usp_notes_enumerate", conn);
					cmd.CommandType = CommandType.StoredProcedure;
					cmd.Parameters.AddRange(spParams);
					new SqlDataAdapter(cmd).Fill(table);
				}
				catch (SqlException ex)
				{
					this.HandleSqlError(ex);
				}
				finally
				{
					conn.Dispose();
					conn.Close();
				}
			}
			return table;
		}


		/// <summary>
		/// Add a new Note to the database
		/// </summary>
		/// <param name="theAlert"></param>
		/// <returns></returns>
		public int NoteAdd(Note note)
		{
			int itemID = 0;

			using (SqlConnection conn = CreateOpenConnection())
			{
				try
				{
					SqlParameter[] spParams = new SqlParameter[6];
					spParams[0] = new SqlParameter("@dtDate", SqlDbType.DateTime);
					spParams[0].Value = note.DateOfNote;
					spParams[1] = new SqlParameter("@nScope", SqlDbType.Int);
					spParams[1].Value = (int)note.Scope;
					spParams[2] = new SqlParameter("@nParentID", SqlDbType.Int);
					spParams[2].Value = note.ParentID;
					spParams[3] = new SqlParameter("@cText", SqlDbType.VarChar, 5000);
					spParams[3].Value = note.Text;
					spParams[4] = new SqlParameter("@cUser", SqlDbType.VarChar, 255);
					spParams[4].Value = note.User;
					spParams[5] = new SqlParameter("@nReturnID", SqlDbType.Int);
					spParams[5].Direction = ParameterDirection.Output;

					// Execute the stored procedure
					SqlCommand cmd = new SqlCommand("usp_note_add", conn);
					cmd.CommandType = CommandType.StoredProcedure;
					cmd.Parameters.AddRange(spParams);
					cmd.ExecuteNonQuery();
					itemID = (int)spParams[5].Value;
				}
				catch (SqlException e)
				{
					this.HandleSqlError(e);
				}
				finally
				{
					conn.Dispose();
					conn.Close();
				}
			}
			return itemID;
		}


		/// <summary>
		/// Update the specified Note
		/// </summary>
		/// <param name="theAsset"></param>
		/// <returns></returns>
		public int NoteUpdate(Note note)
		{
			using (SqlConnection conn = CreateOpenConnection())
			{
				try
				{
					SqlParameter[] spParams = new SqlParameter[2];
					spParams[0] = new SqlParameter("@nNoteID", SqlDbType.Int);
					spParams[0].Value = note.NoteID;
					spParams[1] = new SqlParameter("@cText", SqlDbType.VarChar, 5000);
					spParams[1].Value = note.Text;
					//
					SqlCommand cmd = new SqlCommand("usp_note_update", conn);
					cmd.CommandType = CommandType.StoredProcedure;
					cmd.Parameters.AddRange(spParams);
					cmd.ExecuteNonQuery();
				}
				catch (SqlException e)
				{
					this.HandleSqlError(e);
				}
				finally
				{
					conn.Dispose();
					conn.Close();
				}
			}
			return 0;
		}




		/// <summary>
		/// Delete the specified Note from the database
		/// </summary>
		/// <returns></returns>
		public int NoteDelete(Note note)
		{
			using (SqlConnection conn = CreateOpenConnection())
			{
				try
				{
					SqlParameter[] spParams = new SqlParameter[1];
					spParams[0] = new SqlParameter("@nNoteID", SqlDbType.Int);
					spParams[0].Value = note.NoteID;
					//
					SqlCommand cmd = new SqlCommand("usp_note_delete", conn);
					cmd.CommandType = CommandType.StoredProcedure;
					cmd.Parameters.AddRange(spParams);
					cmd.ExecuteNonQuery();
				}
				catch (SqlException e)
				{
					this.HandleSqlError(e);
				}
				finally
				{
					conn.Dispose();
					conn.Close();
				}
			}
			return 0;
		}

		#endregion Notes Table

		#region Documents Table

		/// <summary>
		/// Return a table containing all of the Documents defined for an asset
		/// </summary>
		/// <returns></returns>
		public DataTable EnumerateDocuments(Asset forAsset)
		{
			return EnumerateDocuments(SCOPE.Asset, forAsset.AssetID);
		}

		/// <summary>
		/// Return a table containing all of the Documents defined for an application
		/// </summary>
		/// <returns></returns>
		public DataTable EnumerateDocuments(InstalledApplication forApplication)
		{
			return EnumerateDocuments(SCOPE.Application, forApplication.ApplicationID);
		}

		/// <summary>
		/// Return a table containing all of the Documents defined for an application license
		/// </summary>
		/// <returns></returns>
		public DataTable EnumerateDocuments(ApplicationLicense forLicense)
		{
			return EnumerateDocuments(SCOPE.License, forLicense.LicenseID);
		}



		/// <summary>
		/// This function is the base function for returning documents for a specific item or the specified scope
		/// and database ID.  It is called from the public specific functions
		/// </summary>
		/// <param name="scope"></param>
		/// <param name="parentID"></param>
		/// <returns></returns>
		public DataTable EnumerateDocuments(SCOPE scope, int parentID)
		{
			DataTable table = new DataTable(TableNames.DOCUMENTS);
			using (SqlConnection conn = CreateOpenConnection())
			{
				try
				{
					SqlParameter[] spParams = new SqlParameter[2];
					spParams[0] = new SqlParameter("@nScope", SqlDbType.Int);
					spParams[0].Value = (int)scope;
					spParams[1] = new SqlParameter("@nParentID", SqlDbType.Int);
					spParams[1].Value = parentID;
					//
					SqlCommand cmd = new SqlCommand("usp_documents_enumerate", conn);
					cmd.CommandType = CommandType.StoredProcedure;
					cmd.Parameters.AddRange(spParams);
					new SqlDataAdapter(cmd).Fill(table);
				}
				catch (SqlException ex)
				{
					this.HandleSqlError(ex);
				}
				finally
				{
					conn.Dispose();
					conn.Close();
				}
			}
			return table;
		}


		/// <summary>
		/// Add a new Document to the database
		/// </summary>
		/// <param name="document">The document to add</param>
		/// <returns></returns>
		public int DocumentAdd(Document document)
		{
			int itemID = 0;

			using (SqlConnection conn = CreateOpenConnection())
			{
				try
				{
					SqlParameter[] spParams = new SqlParameter[5];
					spParams[0] = new SqlParameter("@nScope", SqlDbType.Int);
					spParams[0].Value = (int)document.Scope;
					spParams[1] = new SqlParameter("@nParentID", SqlDbType.Int);
					spParams[1].Value = document.ParentID;
					spParams[2] = new SqlParameter("@cName", SqlDbType.VarChar, 255);
					spParams[2].Value = document.Name;
					spParams[3] = new SqlParameter("@cPath", SqlDbType.VarChar, 255);
					spParams[3].Value = document.Path;
					spParams[4] = new SqlParameter("@nReturnID", SqlDbType.Int);
					spParams[4].Direction = ParameterDirection.Output;

					// Execute the stored procedure
					SqlCommand cmd = new SqlCommand("usp_document_add", conn);
					cmd.CommandType = CommandType.StoredProcedure;
					cmd.Parameters.AddRange(spParams);
					cmd.ExecuteNonQuery();
					itemID = (int)spParams[4].Value;
				}
				catch (SqlException e)
				{
					this.HandleSqlError(e);
				}
				finally
				{
					conn.Dispose();
					conn.Close();
				}
			}
			return itemID;
		}


		/// <summary>
		/// Update the specified Document
		/// </summary>
		/// <param name="theAsset"></param>
		/// <returns></returns>
		public int DocumentUpdate(Document document)
		{
			using (SqlConnection conn = CreateOpenConnection())
			{
				try
				{
					SqlParameter[] spParams = new SqlParameter[2];
					spParams[0] = new SqlParameter("@nDocumentID", SqlDbType.Int);
					spParams[0].Value = document.DocumentID;
					spParams[1] = new SqlParameter("@cPath", SqlDbType.VarChar, 255);
					spParams[1].Value = document.Path;
					//
					SqlCommand cmd = new SqlCommand("usp_Document_update", conn);
					cmd.CommandType = CommandType.StoredProcedure;
					cmd.Parameters.AddRange(spParams);
					cmd.ExecuteNonQuery();
				}
				catch (SqlException e)
				{
					this.HandleSqlError(e);
				}
				finally
				{
					conn.Dispose();
					conn.Close();
				}
			}
			return 0;
		}


		/// <summary>
		/// Delete the specified Document from the database
		/// </summary>
		/// <returns></returns>
		public int DocumentDelete(Document document)
		{
			using (SqlConnection conn = CreateOpenConnection())
			{
				try
				{
					SqlParameter[] spParams = new SqlParameter[1];
					spParams[0] = new SqlParameter("@nDocumentID", SqlDbType.Int);
					spParams[0].Value = document.DocumentID;
					//
					SqlCommand cmd = new SqlCommand("usp_Document_delete", conn);
					cmd.CommandType = CommandType.StoredProcedure;
					cmd.Parameters.AddRange(spParams);
					cmd.ExecuteNonQuery();
				}
				catch (SqlException e)
				{
					this.HandleSqlError(e);
				}
				finally
				{
					conn.Dispose();
					conn.Close();
				}
			}
			return 0;
		}

		#endregion Documents Table

		#region Operations Table

		/// <summary>
		/// Return a table containing all of the Operations defined
		/// </summary>
		/// <returns></returns>
		public DataTable EnumerateOperations(Operation.OPERATION operationType ,Operation.STATUS status)
		{
			DataTable table = new DataTable(TableNames.OPERATIONS);
			using (SqlConnection conn = CreateOpenConnection())
			{
				try
				{
					SqlParameter[] spParams = new SqlParameter[2];
					spParams[0] = new SqlParameter("@nOperation", SqlDbType.Int);
					spParams[0].Value = (int)operationType;
					spParams[1] = new SqlParameter("@nStatus", SqlDbType.Int);
					spParams[1].Value = (int)status;
					//
					SqlCommand cmd = new SqlCommand("usp_operations_enumerate", conn);
					cmd.CommandType = CommandType.StoredProcedure;
					cmd.Parameters.AddRange(spParams);
					new SqlDataAdapter(cmd).Fill(table);
				}
				catch (SqlException ex)
				{
					this.HandleSqlError(ex);
				}
				finally
				{
					conn.Dispose();
					conn.Close();
				}
			}
			return table;
		}


		/// <summary>
		/// Add a new Operation to the database
		/// </summary>
		/// <param name="operation">The Operation to add</param>
		/// <returns></returns>
		public int OperationAdd(Operation operation)
		{
			int itemID = 0;

			using (SqlConnection conn = CreateOpenConnection())
			{
				try
				{
					SqlParameter[] spParams = new SqlParameter[3];
					spParams[0] = new SqlParameter("@nOperation", SqlDbType.Int);
					spParams[0].Value = (int)operation.OperationType;
					spParams[1] = new SqlParameter("@nAssetID", SqlDbType.Int);
					spParams[1].Value = operation.AssetID;
					spParams[2] = new SqlParameter("@nReturnID", SqlDbType.Int);
					spParams[2].Direction = ParameterDirection.Output;

					// Execute the stored procedure
					SqlCommand cmd = new SqlCommand("usp_Operation_add", conn);
					cmd.CommandType = CommandType.StoredProcedure;
					cmd.Parameters.AddRange(spParams);
					cmd.ExecuteNonQuery();
					itemID = (int)spParams[2].Value;
				}
				catch (SqlException e)
				{
					this.HandleSqlError(e);
				}
				finally
				{
					conn.Dispose();
					conn.Close();
				}
			}
			return itemID;
		}


		/// <summary>
		/// Update the specified Operation
		/// </summary>
		/// <param name="theAsset"></param>
		/// <returns></returns>
		public int OperationUpdate(Operation operation)
		{
			using (SqlConnection conn = CreateOpenConnection())
			{
				try
				{
					SqlParameter[] spParams = new SqlParameter[4];
					spParams[0] = new SqlParameter("@nOperationID", SqlDbType.Int);
					spParams[0].Value = operation.OperationID;
					spParams[1] = new SqlParameter("@dtEndDate", SqlDbType.DateTime);
					spParams[1].IsNullable = true;
					if (operation.EndDate.Ticks == 0)
						spParams[1].Value = DBNull.Value;
					else
						spParams[1].Value = operation.EndDate;
					spParams[2] = new SqlParameter("@nStatus", SqlDbType.Int);
					spParams[2].Value = (int)operation.Status;
					spParams[3] = new SqlParameter("@cErrorText", SqlDbType.VarChar ,510);
					spParams[3].Value = operation.ErrorText;
					//
					SqlCommand cmd = new SqlCommand("usp_operation_update", conn);
					cmd.CommandType = CommandType.StoredProcedure;
					cmd.Parameters.AddRange(spParams);
					cmd.ExecuteNonQuery();
				}
				catch (SqlException e)
				{
					this.HandleSqlError(e);
				}
				finally
				{
					conn.Dispose();
					conn.Close();
				}
			}
			return 0;
		}


		/// <summary>
		/// Delete the specified Operation from the database
		/// </summary>
		/// <returns></returns>
		public int OperationDelete(Operation operation)
		{
			using (SqlConnection conn = CreateOpenConnection())
			{
				try
				{
					SqlParameter[] spParams = new SqlParameter[1];
					spParams[0] = new SqlParameter("@nOperationID", SqlDbType.Int);
					spParams[0].Value = operation.OperationID;
					//
					SqlCommand cmd = new SqlCommand("usp_Operation_delete", conn);
					cmd.CommandType = CommandType.StoredProcedure;
					cmd.Parameters.AddRange(spParams);
					cmd.ExecuteNonQuery();
				}
				catch (SqlException e)
				{
					this.HandleSqlError(e);
				}
				finally
				{
					conn.Dispose();
					conn.Close();
				}
			}
			return 0;
		}



		/// <summary>
		/// Get the database index of the last operation in the database
		/// </summary>
		/// <returns></returns>
		public int OperationGetLastIndex()
		{
			int returnID = 0;

			using (SqlConnection conn = CreateOpenConnection())
			{
				try
				{
					SqlParameter[] spParams = new SqlParameter[1];
					spParams[0] = new SqlParameter("@nReturnID", SqlDbType.Int);
					spParams[0].Direction = ParameterDirection.Output;

					// Execute the stored procedure
					SqlCommand cmd = new SqlCommand("usp_Operation_lastindex", conn);
					cmd.CommandType = CommandType.StoredProcedure;
					cmd.Parameters.AddRange(spParams);
					cmd.ExecuteNonQuery();
					returnID = (int)spParams[0].Value;
				}
				catch (SqlException e)
				{
					this.HandleSqlError(e);
				}
				finally
				{
					conn.Dispose();
					conn.Close();
				}
			}
			return returnID;
		}

		#endregion Operations Table

		#region File Systems Table


		/// <summary>
		/// Return a table containing all of the Folders that have been audited for an asset
		/// </summary>
		/// <returns></returns>
		public DataTable EnumerateFileSystemFolders(int assetID)
		{
			DataTable table = new DataTable(TableNames.FS_FOLDERS);
			using (SqlConnection conn = CreateOpenConnection())
			{
				try
				{
					SqlParameter[] spParams = new SqlParameter[1];
					spParams[0] = new SqlParameter("@nAssetID", SqlDbType.Int);
					spParams[0].Value = assetID;
					SqlCommand cmd = new SqlCommand("usp_fs_folder_enumerate", conn);
					cmd.CommandType = CommandType.StoredProcedure;
					cmd.Parameters.AddRange(spParams);
					new SqlDataAdapter(cmd).Fill(table);
				}
				catch (SqlException ex)
				{
					this.HandleSqlError(ex);
				}
				finally
				{
					conn.Dispose();
					conn.Close();
				}
			}
			return table;
		}



		/// <summary>
		/// Return a table containing all of the Files that have been audited for an asset
		/// </summary>
		/// <returns></returns>
		public DataTable EnumerateFileSystemFiles(int assetID)
		{
			DataTable table = new DataTable(TableNames.FS_FILES);
			using (SqlConnection conn = CreateOpenConnection())
			{
				try
				{
					SqlParameter[] spParams = new SqlParameter[1];
					spParams[0] = new SqlParameter("@nAssetID", SqlDbType.Int);
					spParams[0].Value = assetID;
					SqlCommand cmd = new SqlCommand("usp_fs_file_enumerate", conn);
					cmd.CommandType = CommandType.StoredProcedure;
					cmd.Parameters.AddRange(spParams);
					new SqlDataAdapter(cmd).Fill(table);
				}
				catch (SqlException ex)
				{
					this.HandleSqlError(ex);
				}
				finally
				{
					conn.Dispose();
					conn.Close();
				}
			}
			return table;
		}



		/// <summary>
		/// Return a table containing all of the Files that have been assigned
		/// </summary>
		/// <returns></returns>
		public DataTable EnumerateFileSystemFileAssignments()
		{
			DataTable table = new DataTable(TableNames.FS_FILES);
			using (SqlConnection conn = CreateOpenConnection())
			{
				try
				{
					SqlCommand cmd = new SqlCommand("usp_fs_file_enumerate_assignments", conn);
					cmd.CommandType = CommandType.StoredProcedure;
					new SqlDataAdapter(cmd).Fill(table);
				}
				catch (SqlException ex)
				{
					this.HandleSqlError(ex);
				}
				finally
				{
					conn.Dispose();
					conn.Close();
				}
			}
			return table;
		}


		/// <summary>
		/// Return a table containing all of the Files which match the file specification supplied
		/// </summary>
		/// <returns></returns>
		public DataTable EnumerateFileSystemFiles(string filter)
		{
			DataTable table = new DataTable(TableNames.FS_FILES);
			using (SqlConnection conn = CreateOpenConnection())
			{
				try
				{
					SqlParameter[] spParams = new SqlParameter[1];
					spParams[0] = new SqlParameter("@cFilter", SqlDbType.VarChar ,255);
					spParams[0].Value = filter;
					SqlCommand cmd = new SqlCommand("usp_fs_file_report", conn);
					cmd.CommandType = CommandType.StoredProcedure;
					cmd.Parameters.AddRange(spParams);
					new SqlDataAdapter(cmd).Fill(table);
				}
				catch (SqlException ex)
				{
					this.HandleSqlError(ex);
				}
				finally
				{
					conn.Dispose();
					conn.Close();
				}
			}
			return table;
		}

		/// <summary>
		/// Add a new File System Folder to the database
		/// </summary>
		/// <returns></returns>
		public int FileSystemFolder_Add(FileSystemFolder folder)
		{
			int itemID = 0;
			using (SqlConnection conn = CreateOpenConnection())
			{
				try
				{
					SqlParameter[] spParams = new SqlParameter[4];
					spParams[0] = new SqlParameter("@cName", SqlDbType.VarChar ,255);
					spParams[0].Value = folder.Name;
					spParams[1] = new SqlParameter("@nParentID", SqlDbType.Int);
					spParams[1].Value = folder.ParentID;
					spParams[2] = new SqlParameter("@nAssetID", SqlDbType.Int);
					spParams[2].Value = folder.AssetID;
					spParams[3] = new SqlParameter("@nReturnID", SqlDbType.Int);
					spParams[3].Direction = ParameterDirection.Output;

					// Execute the stored procedure
					SqlCommand cmd = new SqlCommand("usp_fs_folder_add", conn);
					cmd.CommandType = CommandType.StoredProcedure;
					cmd.Parameters.AddRange(spParams);
					cmd.ExecuteNonQuery();
					itemID = (int)spParams[3].Value;
				}
				catch (SqlException e)
				{
					this.HandleSqlError(e);
				}
				finally
				{
					conn.Dispose();
					conn.Close();
				}
			}
			return itemID;
		}


		/// <summary>
		/// Add a new File System File to the database
		/// </summary>
		/// <returns></returns>
		public int FileSystemFile_Add(FileSystemFile file)
		{
			int itemID = 0;
			using (SqlConnection conn = CreateOpenConnection())
			{
				try
				{
					SqlParameter[] spParams = new SqlParameter[16];
					spParams[0] = new SqlParameter("@cName", SqlDbType.VarChar, 255);
					spParams[0].Value = file.Name;
					spParams[1] = new SqlParameter("@nParentID", SqlDbType.Int);
					spParams[1].Value = file.ParentID;
					spParams[2] = new SqlParameter("@nAssetID", SqlDbType.Int);
					spParams[2].Value = file.AssetID;
					spParams[3] = new SqlParameter("@nSize", SqlDbType.Int);
					spParams[3].Value = file.Size;
					
					// Creation date - set to dbnull if none
					spParams[4] = new SqlParameter("@dtCreated", SqlDbType.DateTime);
					spParams[4].IsNullable = true;
					if (file.CreatedDateTime.Ticks == 0)
						spParams[4].Value = DBNull.Value;
					else
						spParams[4].Value = file.CreatedDateTime;
					
					// Modified Time - set to dbnull if none
					spParams[5] = new SqlParameter("@dtModified", SqlDbType.DateTime);
					spParams[5].IsNullable = true;
					if (file.ModifiedDateTime.Ticks == 0)
						spParams[5].Value = DBNull.Value;
					else
						spParams[5].Value = file.ModifiedDateTime;

					// Last Accessed Time - set to dbnull if none
					spParams[6] = new SqlParameter("@dtLastAccessed", SqlDbType.DateTime);
					spParams[6].IsNullable = true;
					if (file.LastAccessedDateTime.Ticks == 0)
						spParams[6].Value = DBNull.Value;
					else
						spParams[6].Value = file.LastAccessedDateTime;
					//
					spParams[7] = new SqlParameter("@cPublisher", SqlDbType.VarChar, 255);
					spParams[7].Value = file.Publisher;
					spParams[8] = new SqlParameter("@cProductName", SqlDbType.VarChar, 255);
					spParams[8].Value = file.ProductName;
					spParams[9] = new SqlParameter("@cDescription", SqlDbType.VarChar, 255);
					spParams[9].Value = file.Description;
					spParams[10] = new SqlParameter("@cPVersion1", SqlDbType.VarChar, 255);
					spParams[10].Value = file.ProductVersion1;
					spParams[11] = new SqlParameter("@cPVersion2", SqlDbType.VarChar, 255);
					spParams[11].Value = file.ProductVersion2;
					spParams[12] = new SqlParameter("@cFVersion1", SqlDbType.VarChar, 255);
					spParams[12].Value = file.FileVersion1;
					spParams[13] = new SqlParameter("@cFVersion2", SqlDbType.VarChar, 255);
					spParams[13].Value = file.FileVersion2;
					spParams[14] = new SqlParameter("@cFilename", SqlDbType.VarChar, 255);
					spParams[14].Value = file.OriginalFileName;
					spParams[15] = new SqlParameter("@nReturnID", SqlDbType.Int);
					spParams[15].Direction = ParameterDirection.Output;

					// Execute the stored procedure
					SqlCommand cmd = new SqlCommand("usp_fs_file_add", conn);
					cmd.CommandType = CommandType.StoredProcedure;
					cmd.Parameters.AddRange(spParams);
					cmd.ExecuteNonQuery();
					itemID = (int)spParams[15].Value;
				}
				catch (SqlException e)
				{
					this.HandleSqlError(e);
				}
				finally
				{
					conn.Dispose();
					conn.Close();
				}
			}
			return itemID;
		}




		/// <summary>
		/// Clean all of the FileSystem records for the specified asset
		/// </summary>
		/// <returns></returns>
		public int FileSystemFolder_Clean(int assetID)
		{
			using (SqlConnection conn = CreateOpenConnection())
			{
				try
				{
					SqlParameter[] spParams = new SqlParameter[1];
					spParams[0] = new SqlParameter("@nAssetID", SqlDbType.Int);
					spParams[0].Value = assetID;

					// Execute the stored procedure
					SqlCommand cmd = new SqlCommand("usp_fs_clean", conn);
					cmd.CommandType = CommandType.StoredProcedure;
					cmd.Parameters.AddRange(spParams);
					cmd.ExecuteNonQuery();
				}
				catch (SqlException e)
				{
					this.HandleSqlError(e);
				}
				finally
				{
					conn.Dispose();
					conn.Close();
				}
			}
			return 0;
		}



		/// <summary>
		/// Assign (or unassign) a File System File 
		/// </summary>
		/// <returns></returns>
		public int FileSystemFile_Assign(FileSystemFile file)
		{
			int itemID = 0;
			using (SqlConnection conn = CreateOpenConnection())
			{
				try
				{
					SqlParameter[] spParams = new SqlParameter[6];
					spParams[0] = new SqlParameter("@cName", SqlDbType.VarChar, 255);
					spParams[0].Value = file.Name;
					spParams[1] = new SqlParameter("@cPublisher", SqlDbType.VarChar, 255);
					spParams[1].Value = file.Publisher;
					spParams[2] = new SqlParameter("@cProductName", SqlDbType.VarChar, 255);
					spParams[2].Value = file.ProductName;
					spParams[3] = new SqlParameter("@cFVersion1", SqlDbType.VarChar, 255);
					spParams[3].Value = file.FileVersion1;
					spParams[4] = new SqlParameter("@nApplicationID", SqlDbType.Int);
					spParams[4].Value = file.AssignApplicationID;
					spParams[5] = new SqlParameter("@nReturnID", SqlDbType.Int);
					spParams[5].Direction = ParameterDirection.Output;

					// Execute the stored procedure
					SqlCommand cmd = new SqlCommand("usp_fs_file_assign", conn);
					cmd.CommandType = CommandType.StoredProcedure;
					cmd.Parameters.AddRange(spParams);
					cmd.ExecuteNonQuery();
				}
				catch (SqlException e)
				{
					this.HandleSqlError(e);
				}
				finally
				{
					conn.Dispose();
					conn.Close();
				}
			}
			return itemID;
		}

		#endregion File Systems (FS_FOLDER / FS_FILE) Table

		#region Statistical Functions

		/// <summary>
		/// Return Audit Statistics
		/// </summary>
		/// <returns></returns>
		public DataTable AuditStatistics()
		{
			DataTable statisticsTable = new DataTable(TableNames.APPLICATION_STATISTICS);
			using (SqlConnection conn = CreateOpenConnection())
			{
				try
				{
					SqlCommand cmd = new SqlCommand("usp_audit_statistics", conn);
					cmd.CommandType = CommandType.StoredProcedure;
					new SqlDataAdapter(cmd).Fill(statisticsTable);
				}
				catch (SqlException e)
				{
					this.HandleSqlError(e);
				}
				finally
				{
					conn.Dispose();
					conn.Close();
				}
			}
			return statisticsTable;
		}



		/// <summary>
		/// Return statistics for Asset States
		/// </summary>
		/// <returns></returns>
		public DataTable AssetStatistics()
		{
			DataTable statisticsTable = new DataTable(TableNames.COMPUTER_STATISTICS);
			using (SqlConnection conn = CreateOpenConnection())
			{
				try
				{
					SqlCommand cmd = new SqlCommand("usp_statistics_assets", conn);
					cmd.CommandType = CommandType.StoredProcedure;
					new SqlDataAdapter(cmd).Fill(statisticsTable);
				}
				catch (SqlException e)
				{
					this.HandleSqlError(e);
				}
				finally
				{
					conn.Dispose();
					conn.Close();
				}
			}
			return statisticsTable;
		}



		/// <summary>
		/// Return statistics for Asset States
		/// </summary>
		/// <returns></returns>
		public DataTable StatisticsAssetStates()
		{
			DataTable statisticsTable = new DataTable(TableNames.COMPUTER_STATISTICS);
			using (SqlConnection conn = CreateOpenConnection())
			{
				try
				{
					SqlCommand cmd = new SqlCommand("usp_statistics_asset_states", conn);
					cmd.CommandType = CommandType.StoredProcedure;
					new SqlDataAdapter(cmd).Fill(statisticsTable);
				}
				catch (SqlException e)
				{
					this.HandleSqlError(e);
				}
				finally
				{
					conn.Dispose();
					conn.Close();
				}
			}
			return statisticsTable;
		}

		/// <summary>
		/// Return Alert Statistics
		/// </summary>
		/// <returns></returns>
		public DataTable AlertStatistics()
		{
			DataTable statisticsTable = new DataTable(TableNames.APPLICATION_STATISTICS);
			using (SqlConnection conn = CreateOpenConnection())
			{
				try
				{
					SqlCommand cmd = new SqlCommand("usp_statistics_alerts", conn);
					cmd.CommandType = CommandType.StoredProcedure;
					new SqlDataAdapter(cmd).Fill(statisticsTable);
				}
				catch (SqlException e)
				{
					this.HandleSqlError(e);
				}
				finally
				{
					conn.Dispose();
					conn.Close();
				}
			}
			return statisticsTable;
		}


		/// <summary>
		/// Return Audit History Statistics
		/// </summary>
		/// <returns></returns>
		public DataTable AuditHistoryStatistics()
		{
			DataTable statisticsTable = new DataTable(TableNames.APPLICATION_STATISTICS);
			using (SqlConnection conn = CreateOpenConnection())
			{
				try
				{
					SqlCommand cmd = new SqlCommand("usp_statistics_auditdates", conn);
					cmd.CommandType = CommandType.StoredProcedure;
					new SqlDataAdapter(cmd).Fill(statisticsTable);
				}
				catch (SqlException e)
				{
					this.HandleSqlError(e);
				}
				finally
				{
					conn.Dispose();
					conn.Close();
				}
			}
			return statisticsTable;
		}


		/// <summary>
		/// Return statistics for the 'Support Statistics' phase
		/// </summary>
		/// <returns></returns>
		public DataTable SupportStatistics()
		{
			DataTable statisticsTable = new DataTable(TableNames.COMPUTER_STATISTICS);
			using (SqlConnection conn = CreateOpenConnection())
			{
				try
				{
					SqlCommand cmd = new SqlCommand("usp_support_statistics", conn);
					cmd.CommandType = CommandType.StoredProcedure;
					new SqlDataAdapter(cmd).Fill(statisticsTable);
				}
				catch (SqlException e)
				{
					this.HandleSqlError(e);
				}
				finally
				{
					conn.Dispose();
					conn.Close();
				}
			}
			return statisticsTable;
		}




		/// <summary>
		/// Return statistics for the 'Declare Licenses' phase
		/// </summary>
		/// <returns></returns>
		public DataTable DLStatistics()
		{
			DataTable statisticsTable = new DataTable(TableNames.COMPUTER_STATISTICS);
			using (SqlConnection conn = CreateOpenConnection())
			{
				try
				{
					SqlCommand cmd = new SqlCommand("usp_statistics_dl", conn);
					cmd.CommandType = CommandType.StoredProcedure;
					new SqlDataAdapter(cmd).Fill(statisticsTable);
				}
				catch (SqlException e)
				{
					this.HandleSqlError(e);
				}
				finally
				{
					conn.Dispose();
					conn.Close();
				}
			}
			return statisticsTable;
		}


		/// <summary>
		/// Return statistics for the 'Create Actions' phase
		/// </summary>
		/// <returns></returns>
		public DataTable CAStatistics()
		{
			DataTable statisticsTable = new DataTable(TableNames.COMPUTER_STATISTICS);
			using (SqlConnection conn = CreateOpenConnection())
			{
				try
				{
					SqlCommand cmd = new SqlCommand("usp_statistics_ca", conn);
					cmd.CommandType = CommandType.StoredProcedure;
					new SqlDataAdapter(cmd).Fill(statisticsTable);
				}
				catch (SqlException e)
				{
					this.HandleSqlError(e);
				}
				finally
				{
					conn.Dispose();
					conn.Close();
				}
			}
			return statisticsTable;
		}


		/// <summary>
		/// Return statistics for the 'Review Actions' phase
		/// </summary>
		/// <returns></returns>
		public DataTable RAStatistics()
		{
			DataTable statisticsTable = new DataTable(TableNames.COMPUTER_STATISTICS);
			using (SqlConnection conn = CreateOpenConnection())
			{
				try
				{
					SqlCommand cmd = new SqlCommand("usp_statistics_ra", conn);
					cmd.CommandType = CommandType.StoredProcedure;
					new SqlDataAdapter(cmd).Fill(statisticsTable);
				}
				catch (SqlException e)
				{
					this.HandleSqlError(e);
				}
				finally
				{
					conn.Dispose();
					conn.Close();
				}
			}
			return statisticsTable;
		}


		/// <summary>
		/// Return the top applications count from the database
		/// </summary>
		/// <param name="recordCount"></param>
		/// <returns></returns>
		public DataTable StatisticsTopApplications(int recordCount, String publisherFilter)
		{

			// Before we call the stored procedure we need to handle the publisher filter if one has
			// been supplied.  
			String sqlPublisherFilter = BuildPublisherFilter(publisherFilter);

			// Create the data table
			DataTable statisticsTable = new DataTable(TableNames.APPLICATION_STATISTICS);

			// ...and run the query
			using (SqlConnection conn = CreateOpenConnection())
			{
				try
				{
					SqlCommand cmd = new SqlCommand("usp_statistics_topapplications", conn);
					cmd.CommandType = CommandType.StoredProcedure;
					cmd.Parameters.Add("@resultCount", SqlDbType.Int);
					cmd.Parameters["@resultCount"].Value = recordCount;
					cmd.Parameters.Add("@publisherFilter", SqlDbType.VarChar);
					cmd.Parameters["@publisherFilter"].Value = sqlPublisherFilter;
					new SqlDataAdapter(cmd).Fill(statisticsTable);
				}
				catch (SqlException e)
				{
					this.HandleSqlError(e);
				}
				finally
				{
					conn.Dispose();
					conn.Close();
				}
			}
			return statisticsTable;
		}


		/// <summary>
		/// Return the top publishers for applications from the database
		/// </summary>
		/// <param name="recordCount"></param>
		/// <returns></returns>
		public DataTable StatisticsTopPublishers(int recordCount, String publisherFilter)
		{

			// Before we call the stored procedure we need to handle the publisher filter if one has
			// been supplied.  
			String sqlPublisherFilter = BuildPublisherFilter(publisherFilter);

			// Create the data table
			DataTable statisticsTable = new DataTable(TableNames.APPLICATION_STATISTICS);
			//
			using (SqlConnection conn = CreateOpenConnection())
			{
				try
				{
					SqlCommand cmd = new SqlCommand("usp_statistics_toppublishers", conn);
					cmd.CommandType = CommandType.StoredProcedure;
					cmd.Parameters.Add("@resultCount", SqlDbType.Int);
					cmd.Parameters["@resultCount"].Value = recordCount;
					cmd.Parameters.Add("@publisherFilter", SqlDbType.VarChar);
					cmd.Parameters["@publisherFilter"].Value = sqlPublisherFilter;
					new SqlDataAdapter(cmd).Fill(statisticsTable);
				}
				catch (SqlException e)
				{
					this.HandleSqlError(e);
				}
				finally
				{
					conn.Dispose();
					conn.Close();
				}
			}
			return statisticsTable;
		}


		/// <summary>
		/// Return the MS Office usage counts from the database
		/// </summary>
		/// <param name="recordCount"></param>
		/// <returns></returns>
		public DataTable StatisticsMsOffice(int recordCount)
		{
			// Create the data table
			DataTable statisticsTable = new DataTable(TableNames.APPLICATION_STATISTICS);
			//
			using (SqlConnection conn = CreateOpenConnection())
			{
				try
				{
					SqlCommand cmd = new SqlCommand("usp_statistics_msoffice", conn);
					cmd.CommandType = CommandType.StoredProcedure;
					cmd.Parameters.Add("@resultCount", SqlDbType.Int);
					cmd.Parameters["@resultCount"].Value = recordCount;
					new SqlDataAdapter(cmd).Fill(statisticsTable);
				}
				catch (SqlException e)
				{
					this.HandleSqlError(e);
				}
				finally
				{
					conn.Dispose();
					conn.Close();
				}
			}
			return statisticsTable;
		}

		/// <summary>
		/// Return the top publishers for applications from the database
		/// </summary>
		/// <param name="recordCount"></param>
		/// <returns></returns>
		public DataTable StatisticsTopComputers(int recordCount, String publisherFilter)
		{
			// Before we call the stored procedure we need to handle the publisher filter if one has
			// been supplied.  
			String sqlPublisherFilter = BuildPublisherFilter(publisherFilter);

			// Create the data table
			DataTable statisticsTable = new DataTable(TableNames.APPLICATION_STATISTICS);
			//
			using (SqlConnection conn = CreateOpenConnection())
			{
				try
				{
					SqlCommand cmd = new SqlCommand("usp_statistics_topcomputers", conn);
					cmd.CommandType = CommandType.StoredProcedure;
					cmd.Parameters.Add("@resultCount", SqlDbType.Int);
					cmd.Parameters["@resultCount"].Value = recordCount;
					cmd.Parameters.Add("@publisherFilter", SqlDbType.VarChar);
					cmd.Parameters["@publisherFilter"].Value = sqlPublisherFilter;
					new SqlDataAdapter(cmd).Fill(statisticsTable);
				}
				catch (SqlException e)
				{
					this.HandleSqlError(e);
				}
				finally
				{
					conn.Dispose();
					conn.Close();
				}
			}
			return statisticsTable;
		}



		/// <summary>
		/// Return the top processors count from the database
		/// </summary>
		/// <param name="recordCount"></param>
		/// <returns></returns>
		public DataTable StatisticsTopAuditedItem (int recordCount, string category ,string item)
		{
			// Create the data table
			DataTable statisticsTable = new DataTable(TableNames.APPLICATION_STATISTICS);

			// ...and run the query
			using (SqlConnection conn = CreateOpenConnection())
			{
				try
				{
					SqlCommand cmd = new SqlCommand("usp_statistics_auditeditems", conn);
					cmd.CommandType = CommandType.StoredProcedure;
					cmd.Parameters.Add("@resultCount", SqlDbType.Int);
					cmd.Parameters["@resultCount"].Value = recordCount;
					//
					cmd.Parameters.Add("@cCategory", SqlDbType.VarChar ,255);
					cmd.Parameters["@cCategory"].Value = category;
					//
					cmd.Parameters.Add("@cName", SqlDbType.VarChar, 255);
					cmd.Parameters["@cName"].Value = item;
					new SqlDataAdapter(cmd).Fill(statisticsTable);
				}
				catch (SqlException e)
				{
					this.HandleSqlError(e);
				}
				finally
				{
					conn.Dispose();
					conn.Close();
				}
			}
			return statisticsTable;
		}



		/// <summary>
		/// Return the top Operating systems used from the database
		/// </summary>
		/// <param name="recordCount"></param>
		/// <returns></returns>
		public DataTable StatisticsTopOS(int recordCount)
		{
			// Create the data table
			DataTable statisticsTable = new DataTable(TableNames.APPLICATION_STATISTICS);

			// ...and run the query
			using (SqlConnection conn = CreateOpenConnection())
			{
				try
				{
					SqlCommand cmd = new SqlCommand("usp_statistics_topos", conn);
					cmd.CommandType = CommandType.StoredProcedure;
					cmd.Parameters.Add("@resultCount", SqlDbType.Int);
					cmd.Parameters["@resultCount"].Value = recordCount;
					new SqlDataAdapter(cmd).Fill(statisticsTable);
				}
				catch (SqlException e)
				{
					this.HandleSqlError(e);
				}
				finally
				{
					conn.Dispose();
					conn.Close();
				}
			}
			return statisticsTable;
		}


		/// <summary>
		/// Return the top Processor Speeds used from the database
		/// </summary>
		/// <param name="recordCount"></param>
		/// <returns></returns>
		public DataTable StatisticsTopProcessorSpeeds()
		{
			// Create the data table
			DataTable statisticsTable = new DataTable(TableNames.APPLICATION_STATISTICS);

			// ...and run the query
			using (SqlConnection conn = CreateOpenConnection())
			{
				try
				{
					SqlCommand cmd = new SqlCommand("[usp_statistics_topprocessorspeeds]", conn);
					cmd.CommandType = CommandType.StoredProcedure;
					new SqlDataAdapter(cmd).Fill(statisticsTable);
				}
				catch (SqlException e)
				{
					this.HandleSqlError(e);
				}
				finally
				{
					conn.Dispose();
					conn.Close();
				}
			}
			return statisticsTable;
		}


		/// <summary>
		/// Return the top memory capacities used from the database
		/// </summary>
		/// <param name="recordCount"></param>
		/// <returns></returns>
		public DataTable StatisticsTopMemoryCapacity()
		{
			// Create the data table
			DataTable statisticsTable = new DataTable(TableNames.APPLICATION_STATISTICS);

			// ...and run the query
			using (SqlConnection conn = CreateOpenConnection())
			{
				try
				{
					SqlCommand cmd = new SqlCommand("usp_statistics_topmemorycapacity", conn);
					cmd.CommandType = CommandType.StoredProcedure;
					new SqlDataAdapter(cmd).Fill(statisticsTable);
				}
				catch (SqlException e)
				{
					this.HandleSqlError(e);
				}
				finally
				{
					conn.Dispose();
					conn.Close();
				}
			}
			return statisticsTable;
		}
		

		/// <summary>
		/// Return statistics for Assets By Type 
		/// </summary>
		/// <returns></returns>
		public DataTable StatisticsAssetsByType()
		{
			DataTable statisticsTable = new DataTable(TableNames.COMPUTER_STATISTICS);
			using (SqlConnection conn = CreateOpenConnection())
			{
				try
				{
					SqlCommand cmd = new SqlCommand("usp_statistics_assettypes", conn);
					cmd.CommandType = CommandType.StoredProcedure;
					new SqlDataAdapter(cmd).Fill(statisticsTable);
				}
				catch (SqlException e)
				{
					this.HandleSqlError(e);
				}
				finally
				{
					conn.Dispose();
					conn.Close();
				}
			}
			return statisticsTable;
		}

		#endregion

		#region Locations / Domains Table


		/// <summary>
		/// Return a table containing all of the Groups which have been defined
		/// </summary>
		/// <returns></returns>
		public int SetOrganization(string organizationName)
		{
			using (SqlConnection conn = CreateOpenConnection())
			{
				try
				{
					SqlParameter[] spParams = new SqlParameter[1];
					spParams[0] = new SqlParameter("@cOrganization", SqlDbType.VarChar ,255);
					spParams[0].Value = organizationName;
					SqlCommand cmd = new SqlCommand("usp_set_organization", conn);
					cmd.CommandType = CommandType.StoredProcedure;
					cmd.Parameters.AddRange(spParams);
					cmd.ExecuteNonQuery();
				}
				catch (SqlException ex)
				{
					this.HandleSqlError(ex);
				}
				finally
				{
					conn.Dispose();
					conn.Close();
				}
			}
			return 0;
		}


		/// <summary>
		/// Return a table containing all of the Groups which have been defined
		/// </summary>
		/// <returns></returns>
		public DataTable GetGroups(AssetGroup parentGroup)
		{
			DataTable table = new DataTable(TableNames.LOCATIONS);
			using (SqlConnection conn = CreateOpenConnection())
			{
				try
				{
					if (parentGroup.GroupType == AssetGroup.GROUPTYPE.userlocation)
					{
						SqlParameter[] spParams = new SqlParameter[2];
						spParams[0] = new SqlParameter("@nParentID", SqlDbType.Int);
						spParams[0].Value = parentGroup.GroupID;
						spParams[1] = new SqlParameter("@bShowAll", SqlDbType.Bit);
						spParams[1].Value = 1;
						SqlCommand cmd = new SqlCommand("usp_locations_enumerate", conn);
						cmd.CommandType = CommandType.StoredProcedure;
						cmd.Parameters.AddRange(spParams);
						new SqlDataAdapter(cmd).Fill(table);
					}
					else
					{
						SqlParameter[] spParams = new SqlParameter[2];
						spParams[0] = new SqlParameter("@nParentID", SqlDbType.Int);
						spParams[0].Value = parentGroup.GroupID;
						spParams[1] = new SqlParameter("@bShowAll", SqlDbType.Bit);
						spParams[1].Value = 1;
						SqlCommand cmd = new SqlCommand("usp_domains_enumerate", conn);
						cmd.CommandType = CommandType.StoredProcedure;
						cmd.Parameters.AddRange(spParams);
						new SqlDataAdapter(cmd).Fill(table);
					}
				}
				catch (SqlException ex)
				{
					this.HandleSqlError(ex);
				}
				finally
				{
					conn.Dispose();
					conn.Close();
				}
			}
			return table;
		}


		/// <summary>
		/// Add a new Group to the database
		/// </summary>
		/// <param name="group"></param>
		/// <returns></returns>
		public int GroupAdd(AssetGroup group)
		{
			int itemID = 0;
			
			// If the item passed in already exists then we should update rather than add
			if (group.GroupID != 0)
				return GroupUpdate(group);

			// Add a new location
			using (SqlConnection conn = CreateOpenConnection())
			{
				try
				{
					if (group.GroupType == AssetGroup.GROUPTYPE.userlocation)
					{
						SqlParameter[] spParams = new SqlParameter[7];
						spParams[0] = new SqlParameter("@cFullName", SqlDbType.VarChar, 255);
						spParams[0].Value = group.FullName;
						spParams[1] = new SqlParameter("@cName", SqlDbType.VarChar, 255);
						spParams[1].Value = group.Name;
						spParams[2] = new SqlParameter("@nParentID", SqlDbType.Int);
						spParams[2].IsNullable = true;
						if (group.ParentID == 0)
							spParams[2].Value = DBNull.Value;
						else
							spParams[2].Value = group.ParentID;
						spParams[3] = new SqlParameter("@cStartIP", SqlDbType.VarChar, 255);
						spParams[3].Value = group.StartIP;
						spParams[4] = new SqlParameter("@cEndIP", SqlDbType.VarChar, 255);
						spParams[4].Value = group.EndIP;
						spParams[5] = new SqlParameter("@bHidden", SqlDbType.Bit);
						spParams[5].Value = false;
						spParams[6] = new SqlParameter("@nReturnID", SqlDbType.Int);
						spParams[6].Direction = ParameterDirection.Output;

						// Execute the stored procedure
						SqlCommand cmd = new SqlCommand("usp_location_add", conn);
						cmd.CommandType = CommandType.StoredProcedure;
						cmd.Parameters.AddRange(spParams);
						cmd.ExecuteNonQuery();
						itemID = (int)spParams[6].Value;
					}

					else
					{
						SqlParameter[] spParams = new SqlParameter[4];
						spParams[0] = new SqlParameter("@cName", SqlDbType.VarChar, 255);
						spParams[0].Value = group.Name;
						spParams[1] = new SqlParameter("@nParentID", SqlDbType.Int);
						spParams[1].IsNullable = true;
						if (group.ParentID == 0)
							spParams[1].Value = DBNull.Value;
						else
							spParams[1].Value = group.ParentID;
						spParams[2] = new SqlParameter("@bHidden", SqlDbType.Bit);
						spParams[2].Value = false;
						spParams[3] = new SqlParameter("@nReturnID", SqlDbType.Int);
						spParams[3].Direction = ParameterDirection.Output;

						// Execute the stored procedure
						SqlCommand cmd = new SqlCommand("usp_domain_add", conn);
						cmd.CommandType = CommandType.StoredProcedure;
						cmd.Parameters.AddRange(spParams);
						cmd.ExecuteNonQuery();
						itemID = (int)spParams[3].Value; 
					}
				}
				catch (SqlException e)
				{
					this.HandleSqlError(e);
				}
				finally
				{
					conn.Dispose();
					conn.Close();
				}
			}
			return itemID;
		}


		/// <summary>
		/// Update the specified group
		/// </summary>
		/// <param name="location"></param>
		/// <returns></returns>
		public int GroupUpdate(AssetGroup group)
		{
			// If the item passed in does not already exists then we need to add not update
			if (group.GroupID == 0)
				return GroupAdd(group);

			// OK Update 
			using (SqlConnection conn = CreateOpenConnection())
			{
				try
				{
					if (group.GroupType == AssetGroup.GROUPTYPE.userlocation)
					{
						SqlParameter[] spParams = new SqlParameter[7];
						spParams[0] = new SqlParameter("@nLocationID", SqlDbType.Int);
						spParams[0].Value = group.GroupID;
						spParams[1] = new SqlParameter("@cFullName", SqlDbType.VarChar, 255);
						spParams[1].Value = group.FullName;
						spParams[2] = new SqlParameter("@cName", SqlDbType.VarChar, 255);
						spParams[2].Value = group.Name;
						spParams[3] = new SqlParameter("@nParentID", SqlDbType.Int);
						spParams[3].IsNullable = true;
						if (group.ParentID == 0)
							spParams[3].Value = DBNull.Value;
						else
							spParams[3].Value = group.ParentID;
						spParams[4] = new SqlParameter("@cStartIP", SqlDbType.VarChar, 255);
						spParams[4].Value = group.StartIP;
						spParams[5] = new SqlParameter("@cEndIP", SqlDbType.VarChar, 255);
						spParams[5].Value = group.EndIP;
						spParams[6] = new SqlParameter("@bHidden", SqlDbType.Bit);
						spParams[6].Value = false;
						//
						SqlCommand cmd = new SqlCommand("usp_location_update", conn);
						cmd.CommandType = CommandType.StoredProcedure;
						cmd.Parameters.AddRange(spParams);
						cmd.ExecuteNonQuery();
					}

					else
					{
						SqlParameter[] spParams = new SqlParameter[4];
						spParams[0] = new SqlParameter("@nDomainID", SqlDbType.Int);
						spParams[0].Value = group.GroupID;
						spParams[1] = new SqlParameter("@cName", SqlDbType.VarChar, 255);
						spParams[1].Value = group.Name;
						spParams[2] = new SqlParameter("@nParentID", SqlDbType.Int);
						spParams[2].IsNullable = true;
						if (group.ParentID == 0)
							spParams[2].Value = DBNull.Value;
						else
							spParams[2].Value = group.ParentID;
						//
						spParams[3] = new SqlParameter("@bHidden", SqlDbType.Bit);
						spParams[3].Value = false;
						SqlCommand cmd = new SqlCommand("usp_domain_update", conn);
						cmd.CommandType = CommandType.StoredProcedure;
						cmd.Parameters.AddRange(spParams);
						cmd.ExecuteNonQuery();
					}
				}
				catch (SqlException e)
				{
					this.HandleSqlError(e);
				}
				finally
				{
					conn.Dispose();
					conn.Close();
				}
			}
			return group.GroupID;
		}




		/// <summary>
		/// Delete the specified Group from the database - note it is the callers responsibility to
		/// ensure that the referential integrity is maintained - that is no assets refer to this group and
		/// there are no children of the group which would be left orphaned
		/// </summary>
		/// <param name="licenseID"></param>
		/// <returns></returns>
		public int GroupDelete(AssetGroup group)
		{
			int references = 0;

			// The delete code will perform a sanity check to ensure that we do not
			// delete groups which are still referenced so we simply delete it now
			using (SqlConnection conn = CreateOpenConnection())
			{
				try
				{
					if (group.GroupType == AssetGroup.GROUPTYPE.userlocation)
					{
						SqlParameter[] spParams = new SqlParameter[2];
						spParams[0] = new SqlParameter("@nLocationID", SqlDbType.Int);
						spParams[0].Value = group.GroupID;
						spParams[1] = new SqlParameter("@nReferences", SqlDbType.Int);
						spParams[1].Direction = ParameterDirection.ReturnValue;
						//
						SqlCommand cmd = new SqlCommand("usp_location_delete", conn);
						cmd.CommandType = CommandType.StoredProcedure;
						cmd.Parameters.AddRange(spParams);
						cmd.ExecuteNonQuery();
						references = (int)spParams[1].Value;
					}
					else
					{
						SqlParameter[] spParams = new SqlParameter[2];
						spParams[0] = new SqlParameter("@nDomainID", SqlDbType.Int);
						spParams[0].Value = group.GroupID;
						spParams[1] = new SqlParameter("@nReferences", SqlDbType.Int);
						spParams[1].Direction = ParameterDirection.ReturnValue;
						//
						SqlCommand cmd = new SqlCommand("usp_domain_delete", conn);
						cmd.CommandType = CommandType.StoredProcedure;
						cmd.Parameters.AddRange(spParams);
						cmd.ExecuteNonQuery();
						references = (int)spParams[1].Value;
					}
				}
				catch (SqlException e)
				{
					this.HandleSqlError(e);
				}
				finally
				{
					conn.Dispose();
					conn.Close();
				}
			}
			return references;
		}



		/// <summary>
		/// Return a table containing all of the LOCATIONS which have been defined
		/// </summary>
		/// <returns></returns>
		public DataTable GetAllLocations()
		{
			DataTable table = new DataTable(TableNames.LOCATIONS);
			using (SqlConnection conn = CreateOpenConnection())
			{
				try
				{
					SqlCommand cmd = new SqlCommand("usp_locations_enumerate_all", conn);
					cmd.CommandType = CommandType.StoredProcedure;
					new SqlDataAdapter(cmd).Fill(table);
				}
				catch (SqlException ex)
				{
					this.HandleSqlError(ex);
				}
				finally
				{
					conn.Dispose();
					conn.Close();
				}
			}
			return table;
		}


		/// <summary>
		/// Return the database index of the specified Location given its full name
		/// </summary>
		/// <param name="name"></param>
		/// <returns></returns>
		public int LocationFind(string fullname)
		{
			int itemId = 0;
			using (SqlConnection conn = CreateOpenConnection())
			{
				try
				{
					SqlParameter[] spParams = new SqlParameter[2];
					spParams[0] = new SqlParameter("@cFullName", SqlDbType.VarChar ,510);
					spParams[0].Value = fullname;
					spParams[1] = new SqlParameter("@nReturnID", SqlDbType.Int);
					spParams[1].Direction = ParameterDirection.ReturnValue;
					//
					SqlCommand cmd = new SqlCommand("usp_location_find", conn);
					cmd.CommandType = CommandType.StoredProcedure;
					cmd.Parameters.AddRange(spParams);
					cmd.ExecuteNonQuery();
					itemId = (int)spParams[1].Value;
				}
				catch (SqlException e)
				{
					this.HandleSqlError(e);
				}
				finally
				{
					conn.Dispose();
					conn.Close();
				}
			}
			return itemId;
		}



		#endregion Groups Table

		#region Database Maintenance Functions

		/// <summary>
		/// Return a table containing all of the Groups which have been defined
		/// </summary>
		/// <returns></returns>
		public int DatabasePurge (DatabaseSettings settings)
		{
			int rowsPurged = 0;

			// First we need to check for the different types of purging and call each in turn
			if (settings.HistoryPurgeUnits != DatabaseSettings.PURGEUNITS.never)
			{
				DateTime purgeBeforeDate = GetPurgeDate(settings.HistoryPurge ,settings.HistoryPurgeUnits);
				rowsPurged += DatabasePurgeAuditTrail(purgeBeforeDate);
			}

			// Now purge the Internet data 
			if (settings.InternetPurgeUnits != DatabaseSettings.PURGEUNITS.never)
			{
				DateTime purgeBeforeDate = GetPurgeDate(settings.InternetPurge ,settings.InternetPurgeUnits);
				rowsPurged += DatabasePurgeInternet(purgeBeforeDate);
			}

			// Now purge the Audited Assets
			if (settings.AssetPurgeUnits != DatabaseSettings.PURGEUNITS.never)
			{
				DateTime purgeBeforeDate = GetPurgeDate(settings.AssetPurge, settings.AssetPurgeUnits);
				rowsPurged += DatabasePurgeAssets(purgeBeforeDate);
			}

			// Now purge the Operations - we only store operations for 7 days irrespective any other settings
			DateTime purgeOperationsBeforeDate = GetPurgeDate(7, DatabaseSettings.PURGEUNITS.days);
			DatabasePurgeOperations(purgeOperationsBeforeDate);

			return rowsPurged;
		}


		/// <summary>
		/// Purge the AuditTrail table given a date before which items should be discarded
		/// </summary>
		/// <param name="purgeBeforeDate"></param>
		/// <returns></returns>
		public int DatabasePurgeAuditTrail(DateTime purgeBeforeDate)
		{
			using (SqlConnection conn = CreateOpenConnection())
			{
				try
				{
					SqlParameter[] spParams = new SqlParameter[1];
					spParams[0] = new SqlParameter("@dtPurgeDate", SqlDbType.DateTime);
					spParams[0].Value = purgeBeforeDate;
					SqlCommand cmd = new SqlCommand("usp_audittrail_purge", conn);
					cmd.CommandType = CommandType.StoredProcedure;
					cmd.Parameters.AddRange(spParams);
					cmd.ExecuteNonQuery();
				}
				catch (SqlException ex)
				{
					this.HandleSqlError(ex);
				}
				finally
				{
					conn.Dispose();
					conn.Close();
				}
			}
			return 0;
		}


		

		/// <summary>
		/// Purge the Internet table given a date before which items should be discarded
		/// </summary>
		/// <param name="purgeBeforeDate"></param>
		/// <returns></returns>
		public int DatabasePurgeInternet(DateTime purgeBeforeDate)
		{
			using (SqlConnection conn = CreateOpenConnection())
			{
				try
				{
					SqlParameter[] spParams = new SqlParameter[1];
					spParams[0] = new SqlParameter("@dtPurgeDate", SqlDbType.DateTime);
					spParams[0].Value = purgeBeforeDate;
					SqlCommand cmd = new SqlCommand("usp_internet_purge", conn);
					cmd.CommandType = CommandType.StoredProcedure;
					cmd.Parameters.AddRange(spParams);
					cmd.ExecuteNonQuery();
				}
				catch (SqlException ex)
				{
					this.HandleSqlError(ex);
				}
				finally
				{
					conn.Dispose();
					conn.Close();
				}
			}
			return 0;
		}


		

		/// <summary>
		/// Purge unaudited assets given a date before which items should be discarded
		/// </summary>
		/// <param name="purgeBeforeDate"></param>
		/// <returns></returns>
		public int DatabasePurgeAssets(DateTime purgeBeforeDate)
		{
			using (SqlConnection conn = CreateOpenConnection())
			{
				try
				{
					SqlParameter[] spParams = new SqlParameter[1];
					spParams[0] = new SqlParameter("@dtPurgeDate", SqlDbType.DateTime);
					spParams[0].Value = purgeBeforeDate;
					SqlCommand cmd = new SqlCommand("usp_unaudited_assets_purge", conn);
					cmd.CommandType = CommandType.StoredProcedure;
					cmd.Parameters.AddRange(spParams);
					cmd.ExecuteNonQuery();
				}
				catch (SqlException ex)
				{
					this.HandleSqlError(ex);
				}
				finally
				{
					conn.Dispose();
					conn.Close();
				}
			}
			return 0;
		}


		/// <summary>
		/// Purge the Operations table given a date before which items should be discarded
		/// </summary>
		/// <param name="purgeBeforeDate"></param>
		/// <returns></returns>
		public int DatabasePurgeOperations (DateTime purgeBeforeDate)
		{
			using (SqlConnection conn = CreateOpenConnection())
			{
				try
				{
					SqlParameter[] spParams = new SqlParameter[1];
					spParams[0] = new SqlParameter("@dtPurgeDate", SqlDbType.DateTime);
					spParams[0].Value = purgeBeforeDate;
					SqlCommand cmd = new SqlCommand("usp_operations_purge", conn);
					cmd.CommandType = CommandType.StoredProcedure;
					cmd.Parameters.AddRange(spParams);
					cmd.ExecuteNonQuery();
				}
				catch (SqlException ex)
				{
					this.HandleSqlError(ex);
				}
				finally
				{
					conn.Dispose();
					conn.Close();
				}
			}
			return 0;
		}


		/// <summary>
		/// Calculate the date before records should be purged given the units and count
		/// </summary>
		/// <param name="?"></param>
		/// <returns></returns>
		protected DateTime GetPurgeDate(int count, DatabaseSettings.PURGEUNITS units)
		{
			DateTime purgeBeforeDate = DateTime.Now;
			switch ((int)units)
			{
				case (int)DatabaseSettings.PURGEUNITS.days:
					purgeBeforeDate.AddDays(-count);
					break;
				case (int)DatabaseSettings.PURGEUNITS.months:
					purgeBeforeDate.AddMonths(-count);
					break;
				case (int)DatabaseSettings.PURGEUNITS.years:
					purgeBeforeDate.AddDays(-count);
					break;
			}
			return purgeBeforeDate;
		}

		#endregion Database Maintenance Functions

		#region Report Functions



		/// <summary>
		/// Purge the Internet table given a date before which items should be discarded
		/// </summary>
		/// <param name="purgeBeforeDate"></param>
		/// <returns></returns>
		public DataTable GetInternetHistory(DateTime startDate ,DateTime endDate ,string urlFilter)
		{
			DataTable internetTable = new DataTable(TableNames.APPLICATIONS);
			using (SqlConnection conn = CreateOpenConnection())
			{
				try
				{
					SqlParameter[] spParams = new SqlParameter[3];
					spParams[0] = new SqlParameter("@dtStartDate", SqlDbType.DateTime);
					spParams[0].IsNullable = true;
					if ((startDate == null) || (startDate.Ticks == 0))
						spParams[0].Value = DBNull.Value;
					else
						spParams[0].Value = startDate;
					//
					spParams[1] = new SqlParameter("@dtEndDate", SqlDbType.DateTime);
					spParams[1].IsNullable = true;
					if ((endDate == null) || (endDate.Ticks == 0))
						spParams[1].Value = DBNull.Value;
					else
						spParams[1].Value = endDate;
					//
					spParams[2] = new SqlParameter("@strUrl", SqlDbType.VarChar, 510);
					spParams[2].Value = urlFilter;
					//
					SqlCommand cmd = new SqlCommand("usp_internet_getdata", conn);
					cmd.CommandType = CommandType.StoredProcedure;
					cmd.Parameters.AddRange(spParams);
					new SqlDataAdapter(cmd).Fill(internetTable);
				}
				catch (SqlException ex)
				{
					this.HandleSqlError(ex);
				}
				finally
				{
					conn.Dispose();
					conn.Close();
				}
			}
			return internetTable;
		}
		
		
		#endregion Report Functions
		
		#region Scanner Definitions Table


		/// <summary>
		/// Return a table containing all of the Scanner Definitions which have been created
		/// </summary>
		/// <returns></returns>
		public DataTable EnumerateScanners(string scannerName)
		{
			DataTable dataTable = new DataTable(TableNames.ALERTS);
			using (SqlConnection conn = CreateOpenConnection())
			{
				try
				{
					SqlParameter[] spParams = new SqlParameter[1];
					spParams[0] = new SqlParameter("@cName", SqlDbType.VarChar, 255);
					spParams[0].Value = scannerName;
					SqlCommand cmd = new SqlCommand("usp_scanner_enumerate", conn);
					cmd.CommandType = CommandType.StoredProcedure;
					cmd.Parameters.AddRange(spParams);
					new SqlDataAdapter(cmd).Fill(dataTable);
				}
				catch (SqlException ex)
				{
					this.HandleSqlError(ex);
				}
				finally
				{
					conn.Dispose();
					conn.Close();
				}
			}
			return dataTable;
		}


		/// <summary>
		/// Add a new Scanner Definition to the database
		/// </summary>
		/// <param name="theAlert"></param>
		/// <returns></returns>
		public int ScannerAdd(int scannerID ,string scannerName ,MemoryStream xmlStream)
		{
			int itemID = 0;
			if (scannerID != 0)
				return ScannerUpdate(scannerID, scannerName, xmlStream);
			
			// Get the actual data ouit of the memory stream as an ASCII formatted string 
			string xmlText = System.Text.ASCIIEncoding.ASCII.GetString(xmlStream.ToArray());
			
			using (SqlConnection conn = CreateOpenConnection())
			{
				try
				{
					SqlParameter[] spParams = new SqlParameter[3];
					spParams[0] = new SqlParameter("@cName", SqlDbType.VarChar, 255);
					spParams[0].Value = scannerName;
					spParams[1] = new SqlParameter("@cXML", SqlDbType.Text, (int)xmlText.Length);
					spParams[1].Value = xmlText;
					spParams[2] = new SqlParameter("@nReturnID", SqlDbType.Int);
					spParams[2].Direction = ParameterDirection.Output;

					// Execute the stored procedure
					SqlCommand cmd = new SqlCommand("usp_scanner_add", conn);
					cmd.CommandType = CommandType.StoredProcedure;
					cmd.Parameters.AddRange(spParams);
					cmd.ExecuteNonQuery();
					itemID = (int)spParams[2].Value;
				}
				catch (SqlException e)
				{
					this.HandleSqlError(e);
				}
				finally
				{
					conn.Dispose();
					conn.Close();
				}
			}
			return itemID;
		}


		/// <summary>
		/// Update a Scanner Definition in the database
		/// </summary>
		/// <param name="theAlert"></param>
		/// <returns></returns>
		public int ScannerUpdate(int scannerID, string scannerName, MemoryStream xmlStream)
		{
			int itemID = 0;
			
			using (SqlConnection conn = CreateOpenConnection())
			{
				try
				{
					SqlParameter[] spParams = new SqlParameter[3];
					spParams[0] = new SqlParameter("@nScannerID", SqlDbType.Int);
					spParams[0].Value = scannerID;
					spParams[1] = new SqlParameter("@cName", SqlDbType.VarChar, 255);
					spParams[1].Value = scannerName;
					spParams[2] = new SqlParameter("@cXML", SqlDbType.Text, (int)xmlStream.Length);
					spParams[2].Value = xmlStream;

					// Execute the stored procedure
					SqlCommand cmd = new SqlCommand("usp_scanner_update", conn);
					cmd.CommandType = CommandType.StoredProcedure;
					cmd.Parameters.AddRange(spParams);
					cmd.ExecuteNonQuery();
				}
				catch (SqlException e)
				{
					this.HandleSqlError(e);
				}
				finally
				{
					conn.Dispose();
					conn.Close();
				}
			}
			return itemID;
		}



		/// <summary>
		/// Delete the specified Scanner Definition from the database
		/// </summary>
		/// <param name="licenseID"></param>
		/// <returns></returns>
		public int ScannerDelete(int scannerID)
		{
			using (SqlConnection conn = CreateOpenConnection())
			{
				try
				{
					SqlParameter[] spParams = new SqlParameter[1];
					spParams[0] = new SqlParameter("@nScannerD", SqlDbType.Int);
					spParams[0].Value = scannerID;
					//
					SqlCommand cmd = new SqlCommand("usp_scanner_delete", conn);
					cmd.CommandType = CommandType.StoredProcedure;
					cmd.Parameters.AddRange(spParams);
					cmd.ExecuteNonQuery();
				}
				catch (SqlException e)
				{
					this.HandleSqlError(e);
				}
				finally
				{
					conn.Dispose();
					conn.Close();
				}
			}

			return 0;
		}






		#endregion Alerts Table
		
		#region Helper Functions

		/// <summary>
		/// This function takes a semi-colon delimited string of publisher names and returns
		/// a comma-delimited string where each entry is SQL complient
		/// </summary>
		/// <param name="delimitedFilter">Input ; delimited list</param>
		/// <returns>Comma delimited string</returns>
		protected String BuildPublisherFilter(String delimitedFilter)
		{
			String returnFilter = "";
			if (delimitedFilter != "")
			{
				String[] publishers = delimitedFilter.Split(';');

				foreach (String publisher in publishers)
				{
					String sqlPublisher = PrepareSqlString(publisher, 255);
					if (returnFilter == "")
						returnFilter = sqlPublisher;
					else
						returnFilter = returnFilter + "," + sqlPublisher;
				}
			}

			return returnFilter;
		}

		
		/// <summary>
		/// Helper function to delete the specified row from the specified table
		/// </summary>
		/// <param name="strTable"></param>
		/// <param name="strColumn"></param>
		/// <param name="value"></param>
		protected void DeleteRow(string strTable, string strColumn, object value)
		{
			try
			{
				string query;
				if (value is string)
				{
					query = string.Format("DELETE FROM {0} WHERE {1}={2}", strTable, strColumn, this.PrepareSqlString((string)value, 0));
				}
				else
				{
					query = string.Format("DELETE FROM {0} WHERE {1}={2}", strTable, strColumn, value);
				}
				this.ExecuteNonQuery(query);
			}
			catch (SqlException e)
			{
				this.HandleSqlError(e);
			}
		}

		/// <summary>
		/// Helper function to execute a SQL query which returns no results
		/// </summary>
		/// <param name="query"></param>
		/// <returns></returns>
		protected bool ExecuteNonQuery(string query)
		{
			bool returnStatus = false;
			using (SqlConnection conn = CreateOpenConnection())
			{
				try
				{
					new SqlCommand(query, conn).ExecuteNonQuery();
					returnStatus = true;
				}
				catch (SqlException e)
				{
					this.HandleSqlError(e);
				}
				finally
				{
					conn.Dispose();
					conn.Close();
				}
			}

			return returnStatus;
		}

		
		/// <summary>
		/// Helper function to handle an exception or other error which occurs during the
		/// execution of an SQL command
		/// </summary>
		/// <param name="exception"></param>
		protected void HandleSqlError(SqlException exception)
		{
			this._lastError = exception.Message;
			LogFile ourLog = LogFile.Instance;
			ourLog.Write("An SQL Exception has occured.  The error was : " + exception.Message ,true);
		}



		/// <summary>
		/// Helper Function - Insert a row into the specified table
		/// </summary>
		/// <param name="strTable"></param>
		/// <param name="strFields"></param>
		/// <param name="strValues"></param>
		/// <param name="identity"></param>
		/// <returns></returns>
		protected int InsertRow(string strTable, string strFields, string strValues, bool identity)
		{
			int insertedRow = 0;
			using (SqlConnection conn = CreateOpenConnection())
			{
				try
				{
					string query = string.Format("Insert into {0} ({1}) values ({2})", strTable, strFields, strValues);
					SqlCommand myCommand = new SqlCommand(query, conn);
					myCommand.ExecuteNonQuery();
					if (identity)
					{
						myCommand.CommandText = "select @@IDENTITY";
						SqlDataReader myReader = myCommand.ExecuteReader();
						if (myReader.Read())
						{
							insertedRow = Convert.ToInt32(myReader.GetValue(0).ToString());
						}
						myReader.Close();
					}
				}
				catch (SqlException e)
				{
					this.HandleSqlError(e);
				}
				finally
				{
					conn.Dispose();
					conn.Close();
				}
			}
			return insertedRow;
		}



		/// <summary>
		/// Helper Function - Prepare a character string so that it can be inserted safely into
		/// an SQL query
		/// </summary>
		/// <param name="strInputString"></param>
		/// <param name="nMaxLen"></param>
		/// <returns></returns>
		protected string PrepareSqlString(string strInputString, int nMaxLen)
		{
			string newString = "'";
			int nLen = strInputString.Length;
			int nNewLen = 0;
			for (int n = 0; n < nLen; n++)
			{
				if ((nMaxLen != 0) && ((nNewLen + 1) > nMaxLen))
				{
					break;
				}
				if (strInputString[n] == '\'')
				{
					newString = newString + "''";
				}
				else
				{
					newString = newString + strInputString[n];
				}
				nNewLen++;
			}
			return (newString + '\'');
		}

#endregion Helper Functions
	}

#endregion AuditWizardDataAccess Class

}

