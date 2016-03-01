using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Globalization;
using System.Web;

namespace Layton.Common.Controls
{

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

	
	/// <summary>
	/// This is a generic base class for any application which wants to implement a database
	/// based around Microsoft SQL Server.
	/// </summary>
	public class SqlDatabase
	{
		// A general purpose 'tracing/progress' event.  Note that the caller must ensure that the event 
		// is cleared once it is no longer in scope.
		public delegate bool TrackProgress(ProgressDetails progressDetails);
		public event TrackProgress ShowProgress = null;

#region data declarations

		/// <summary>
		/// Physical name of the SQL server with the ActiveHelp database.
		/// </summary>
		protected string _serverName = "(local)";

		/// <summary>
		/// Name of the database to connect to.
		/// </summary>
		protected string _databaseName = "";

		/// <summary>
		/// True, to use Windows Authtentication.
		/// </summary>
		protected bool _useTrustedConnection = true;

		/// <summary>
		/// Username of the database if using SQL Authentication.
		/// </summary>
		protected string _serverUserName;

		/// <summary>
		/// Password of the <see cref="_serverUserName"/> if using SQL Authentication.
		/// </summary>
		protected string _serverPassword;

		/// <summary>
		/// Connection string used to connect to the ActiveHelp database.
		/// </summary>
		protected string _connStr = "";

		/// <summary>
		/// Latest error description.
		/// </summary>
		protected string _error;

		/// <summary>This string recovers and stores the folder in which the executable is running</summary>
		protected string _applicationFolder = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);

#endregion data declarations

#region Data accessors

		/// <summary>
		/// Return the connection string as currently defined
		/// </summary>
		public string ConnectionString
		{
			get { return _connStr; }
		}

		/// <summary>
		/// True if using Windows Authentication, otherwise set to false for SQL Authentication.
		/// </summary>
		public bool UseTrustedConnection
		{
			get { return _useTrustedConnection; }
			set
			{
				_useTrustedConnection = value;
				UpdateConnectionString();
			}
		}


		/// <summary>
		/// Gets or Sets the name of the SQL Server to connect to.
		/// </summary>
		public string ServerName
		{
			get { return _serverName; }
			set
			{
				_serverName = value;
				UpdateConnectionString();
			}
		}

		/// <summary>
		/// Gets the name of the database we are connecting to.
		/// </summary>
		public string DatabaseName
		{
			get { return _databaseName; }
			set
			{
				_databaseName = value;
				UpdateConnectionString();
			}
		}


		/// <summary>
		/// Gets or Sets the name of the SQL Server to connect to.  You must have
		/// <see cref="UseTrustedConnection"/> set to false.
		/// </summary>
		/// 
		public string ServerUserName
		{
			get { return _serverUserName; }
			set
			{
				_serverUserName = value;
				UpdateConnectionString();
			}
		}

		/// <summary>
		/// Gets or Sets the password of the user connecting to the SQL Server.  You must have
		/// <see cref="UseTrustedConnection"/> set to false.
		/// </summary>
		/// 
		public string ServerPassword
		{
			get { return _serverPassword; }
			set
			{
				_serverPassword = value;
				UpdateConnectionString();
			}
		}
		
			
		/// <summary>
		/// Generates the connection string by using the following parameters:
		/// <see cref="_useTrustedConnection"/>, <see cref="_serverName"/>, <see cref="_databaseName"/>, <see cref="_serverUserName"/>, <see cref="_serverPassword"/>
		/// </summary>
		private void UpdateConnectionString()
		{
			if (_useTrustedConnection)
			{
				_connStr = "Server=" + _serverName;
				if (_databaseName != "")
					_connStr = _connStr + ";Database=" + _databaseName;
				_connStr += ";Trusted_Connection=Yes;";
			}
			else
			{
				_connStr = "Server=" + _serverName;
				if (_databaseName != "")
					_connStr = _connStr + ";Database=" + _databaseName;
				_connStr = _connStr + ";User Id=" + _serverUserName + ";password=" + _serverPassword;
			}
		}

		/// <summary>
		/// Returns the last error that occurred.
		/// </summary>
		public string ErrorDescription
		{
			get { return _error; }
		}


#endregion Data accessors

#region Database Helper Functions
		//
		//    PrepareSqlString
		//    ================
		//
		//    Pre-process a string to be passed to a SQL command to ensure that it does
		//    not contain any illegal or invalid characters.
		//	  This includes finding single quotes in a string and replaces with SQL safe
		//    equivalent of two quotes. Also surrounds string with single quotes ready for insertion in a query.
		//
		protected string PrepareSqlString(string strInputString, int nMaxLen)
		{
			string newString = "\'";

			// get string length and scan it
			int nLen = strInputString.Length;
			int nNewLen = 0;

			for (int n = 0; n < nLen; n++)
			{
				// are we checking length?
				if ((nMaxLen != 0)
				&& ((nNewLen + 1) > nMaxLen))
					break;

				// is it a single quote ?
				if (strInputString[n] == '\'')
				{
					// ok - replace with two quotes
					newString += "\'\'";
				}
				else
				{
					newString += strInputString[n];
				}
				nNewLen++;
			}

			// finish off with a quote
			newString += '\'';
			return newString;
		}
		// End of PrepareSqlString



		/// <summary>
		/// Wrapper function around the ShowProgress Delegate so that we can detremine whether or not
		/// a delegate event handler has been set up.
		/// </summary>
		/// <param name="progressDetails"></param>
		/// <returns></returns>
		protected int TryShowProgress(ProgressDetails progressDetails)
		{
			if (this.ShowProgress != null)
				this.ShowProgress(progressDetails);
			return 0;
		}


		protected void HandleSqlError(SqlException exception)
		{
			_error = exception.Message;
			throw exception;
			//string message = "An SQL Exception has occurred.  reason is : " + _error;
			//MessageBox.Show(message, "SQL Exception");
		}

		/// <summary>
		/// ExecuteNonQuery
		/// ===============
		/// 
		/// Simple wrapper around the existing ExecuteNonQuery SQL command with full exception handling
		/// and reporting
		/// </summary>
		/// <param name="query">The query to be executed</param>
		/// <returns>true if successfully executed, false otherwise</returns>
		/// 
		protected bool ExecuteNonQuery(String query)
		{
			bool returnStatus = false;
			try
			{
				using (SqlConnection conn = new SqlConnection(_connStr))
				{
					conn.Open();
					SqlCommand myCommand = new SqlCommand(query, conn);
					myCommand.ExecuteNonQuery();
					returnStatus = true;
				}
			}

			// Catch all exceptions here and set the error text before returning the error
			catch (SqlException e)
			{
				HandleSqlError(e);

			}
			return returnStatus;
		}

		//
		//    InsertRow
		//    =========
		//
		//    Insert a new row into the specified table specifying the fields and their values
		//    returns the identity of the newly inserted field.
		//
		protected int InsertRow(string strTable, string strFields, string strValues, bool identity)
		{
			int insertedRow = 0;

			try
			{
				// Insert the new record
				using (SqlConnection conn = new SqlConnection(_connStr))
				{
					String query = String.Format("Insert into {0} ({1}) values ({2})", strTable, strFields, strValues);
					conn.Open();
					SqlCommand myCommand = new SqlCommand(query, conn);
					myCommand.ExecuteNonQuery();

					// ...get the ID of the last inserted record
					if (identity)
					{
						myCommand.CommandText = "select @@IDENTITY";
						SqlDataReader myReader = myCommand.ExecuteReader();
						if (myReader.Read())
						{
							object obValue = myReader.GetValue(0);
							insertedRow = Convert.ToInt32(obValue.ToString());
						}
						myReader.Close();
					}
				}
			}
			catch (SqlException e)
			{
				HandleSqlError(e);
			}

			return insertedRow;
		}

		/// <summary>
		/// Delete the row with the specified index from the specified table
		/// </summary>
		/// <param name="strTable"></param>
		/// <param name="nIndex"></param>
		protected void DeleteRow(string strTable, string strColumn, object value)
		{
			try
			{
				// Now delete the item itself
				String query;
				if (value is string)
					query = String.Format("DELETE FROM {0} WHERE {1}={2}", strTable, strColumn, PrepareSqlString((string)value, 0));
				else
					query = String.Format("DELETE FROM {0} WHERE {1}={2}", strTable, strColumn, value);
				ExecuteNonQuery(query);
			}

			// Catch all exceptions here and set the error text before returning the error
			catch (SqlException e)
			{
				HandleSqlError(e);
			}

			return;
		}

#endregion Database Helper Functions

	}
}
