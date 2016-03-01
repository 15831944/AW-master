using System;
using System.Collections.Generic;
using System.Text;
using System.Data.OleDb;
using System.Data;
using System.Data.SqlServerCe;
using System.Data.SqlClient;
using System.Xml;
using System.IO;

namespace DBUtility
{
    public class DatabaseUtils
    {
        #region Membre Variables

        public static string m_strConnectionString = string.Empty;
        public static string m_strConnectionStringCompact =string.Empty;
                    
        public string m_strErrorMessage;
        public SqlCommand m_objSqlCommand;

        #endregion

        #region Constructor
        public DatabaseUtils()
        {
            m_objSqlCommand = new SqlCommand();
            m_strErrorMessage = string.Empty;
        }
        #endregion

        #region Public Methods

        #region Set connection string
        /// <summary>
        /// Set connection string
        /// </summary>
        /// <param name="strConnectionString"></param>
        public void SetConnectionString(string strConnectionString)
        {
            m_strConnectionString = strConnectionString;
        }
        public void SetConnectionStringCompact(string strConnectionString)
        {
            m_strConnectionStringCompact = strConnectionString;
        }
        #endregion

        #region Get Records
        /// <summary>
        /// Get Record
        /// </summary>
        /// <param name="strTableName">string type value representing the table name</param>
        /// <param name="strQuery">string type value representing the query to execute</param>
        /// <returns>DataSet type object contains the query result on success,null otherwise</returns>
        public DataSet GetRecords(string strTableName, string strQuery)
        {
            DataSet objDataSet = new DataSet();
            SqlConnection objSqlConnection=null;
            try
            {
                objSqlConnection = new SqlConnection(m_strConnectionString);
                SqlCommand objSqlCommand = new SqlCommand(strQuery, objSqlConnection);
                SqlDataAdapter adapter = new SqlDataAdapter();

                adapter.SelectCommand = objSqlCommand;
                adapter.Fill(objDataSet);

            }
            catch (Exception Ex)
            {
                m_strErrorMessage = Ex.Message;
                throw new Exception(Ex.Message + "Inner Exception details:" + Ex.InnerException.ToString());
            }
            finally
            {
                objSqlConnection.Close();
            }
            return objDataSet;

        }
        #endregion

        #region Execute Query
        /// <summary>
        ///  Execute Query
        /// </summary>
        /// <param name="strQuery">string type value representing the query to execute</param>
        /// <returns>true if query executed successfully,false otherwise</returns>
        public bool ExceuteQuery(string strQuery)
        {
            bool bResult = false;
            SqlConnection objSqlConnection = null;
            try
            {
                objSqlConnection = new SqlConnection(m_strConnectionString);
                objSqlConnection.Open();
                m_objSqlCommand.Connection = objSqlConnection;
                m_objSqlCommand.CommandType = CommandType.Text;
                m_objSqlCommand.CommandText = strQuery;
                if (m_objSqlCommand.ExecuteNonQuery() > 0)
                {
                    bResult = true;
                }

            }
            catch (Exception Ex)
            {
                Console.WriteLine(Ex.Message.ToString());
                m_strErrorMessage = Ex.Message;
                throw new Exception(Ex.Message + "Inner Exception details:" + Ex.InnerException.ToString());

            }
            finally
            {
                objSqlConnection.Close();
            }
            return bResult;
        }

        #endregion

        #region Execute Return StoredProcedure
        /// <summary>
        /// Execute ReturnStored Procedure
        /// </summary>
        /// <param name="strStoredProcedure">string type value representing the name of the stored procedure</param>
        /// <returns>If the Operation is Success It Return 1 Otherwise -1</returns>
        public int ExecuteReturnStoredProcedure(string strStoredProcedure)
        {
            int iResult = -1;
            SqlConnection objSqlConnection=null;
            try
            {
                objSqlConnection = new SqlConnection(m_strConnectionString);
                objSqlConnection.Open();
                m_objSqlCommand.Connection = objSqlConnection;
                m_objSqlCommand.CommandType = CommandType.StoredProcedure;
                m_objSqlCommand.CommandText = strStoredProcedure;
                iResult = m_objSqlCommand.ExecuteNonQuery();
                return iResult;
            }
            catch (Exception Ex)
            {
                m_strErrorMessage = Ex.ToString();
                throw new Exception(Ex.Message + "Inner Exception details:" + Ex.InnerException.ToString());
            }
            finally
            {
                objSqlConnection.Close();
            }
        }
        #endregion

        #region Execute scalar
        /// <summary>
        /// Execute scalar
        /// </summary>
        /// <param name="strQuery">string type value representing the query to execute</param>
        /// <returns>object type value representing the query result on success,null otherwise</returns>
        public object ExcecuteScalar(string strQuery)
        {
            object objValue = null;
            SqlConnection objSqlConnection=null;

            try
            {
                objSqlConnection = new SqlConnection(m_strConnectionString);
                objSqlConnection.Open();
                SqlCommand objSqlCommand = new SqlCommand(strQuery, objSqlConnection);
                objValue = objSqlCommand.ExecuteScalar();
            }
            catch (Exception Ex)
            {
                m_strErrorMessage = Ex.Message;
                throw new Exception(Ex.Message + "Inner Exception details:" + Ex.InnerException.ToString());
            }
            finally
            {
                objSqlConnection.Close();
            }
            return objValue;
        }

        #endregion

        #region Execute scalar SP

        public object ExcecuteScalarSP(string strStoredProcedureName)
        {
            object objValue = null;
            SqlConnection objSqlConnection=null;
            try
            {
                objSqlConnection = new SqlConnection(m_strConnectionString);
                objSqlConnection.Open();
                SqlCommand objSqlCommand = new SqlCommand(strStoredProcedureName, objSqlConnection);
                objSqlCommand.CommandType = CommandType.StoredProcedure;
                objValue = objSqlCommand.ExecuteScalar();
            }
            catch (Exception Ex)
            {
                m_strErrorMessage = Ex.Message;
                throw new Exception(Ex.Message + "Inner Exception details:" + Ex.InnerException.ToString());
            }
            finally
            {
                objSqlConnection.Close();
            }
            return objValue;
        }

        #endregion


        #region Add parameter to the sql command
        /// <summary>
        /// Add parameter
        /// </summary>
        /// <param name="strParameterName">Name of the parameter</param>
        /// <param name="objParameterValue">value of the parameter</param>
        /// <returns></returns>
        public bool AddSQLParameter(string strParameterName, object objParameterValue)
        {
            bool bResult = false;
            try
            {
                m_objSqlCommand.Parameters.Add(new SqlParameter(strParameterName, objParameterValue));
                bResult = true;
            }
            catch (Exception Ex)
            {
                m_strErrorMessage = Ex.ToString();
                throw new Exception(Ex.Message + "Inner Exception details:" + Ex.InnerException.ToString());
            }
            return bResult;
        }
        #endregion

        #region Add parameters to the sql command
        /// <summary>
        /// Add parameter
        /// </summary>
        /// <param name="strParameterName">Name of the parameter</param>
        /// <param name="objParameterValue">value of the parameter</param>
        /// <returns></returns>
        public bool AddSQLParameters(SqlParameter[] Parameters)
        {
            bool bResult=false;
            try
            {
                m_objSqlCommand.Parameters.AddRange(Parameters);
                bResult = true;
            }
            catch (Exception objException)
            {
                m_strErrorMessage = objException.ToString();
            }
            return bResult;
        }
        #endregion

        #region Clear all Parameters
        /// <summary>
        /// Clear SQL Parameters
        /// </summary>
        /// <returns></returns>
        public bool ClearAllSQLParameters()
        {
            bool bResult = false;
            try
            {
                m_objSqlCommand.Parameters.Clear();
                bResult = true;
            }
            catch (Exception Ex)
            {
                throw new Exception(Ex.Message + "Inner Exception details:" + Ex.InnerException.ToString());
            }
            return bResult;
        }
        #endregion

        #region Get Error Message
        public string GetErrorMessage()
        {
            return m_strErrorMessage; ;
        }
        
        #endregion

        #region Execute Sql Transaction
        /// <summary>
        /// Execute Sql Transaction
        /// </summary>
        /// <param name="strQuery">string type value representing the query to excecute</param>
        /// <returns>1 if transaction completed successfully,0 if transaction not completed successfully but rollbacked sucessfully</returns>
        public int ExecuteSqlTransaction(string strQuery)
        {
            using (SqlConnection objSqlConnection = new SqlConnection(m_strConnectionString))
            {
                int iTransactionResult = 0;
                objSqlConnection.Open();

                SqlTransaction objSqlTransaction=objSqlConnection.BeginTransaction();
                SqlCommand objSqlCommand = new SqlCommand(strQuery, objSqlConnection, objSqlTransaction);

                try
                {
                   
                    objSqlCommand.ExecuteNonQuery();
                    // Attempt to commit the transaction.
                    objSqlTransaction.Commit();
                    iTransactionResult =1;
                }
                catch (Exception)
                {
                    // Attempt to roll back the transaction.
                    try
                    {
                        objSqlTransaction.Rollback();
                        iTransactionResult = 0;
                    }
                    catch (Exception Ex)
                    {
                        m_strErrorMessage = Ex.Message;
                        throw new Exception("Transaction rollback failed" + Ex.InnerException);
                    }
                }
                return iTransactionResult;
            }
        }
        #endregion

        #region Execute Sql Transaction
        /// <summary>
        /// Execute Sql Transaction
        /// </summary>
        /// <param name="strQuery">string type value representing the query to excecute</param>
        /// <returns>1 if transaction completed successfully,0 if transaction not completed successfully but rollbacked sucessfully</returns>
        public bool ExecuteSqlTransaction(string strQuery1,string strQuery2,string strQuery3)
        {
            using (SqlConnection objSqlConnection1 = new SqlConnection(m_strConnectionString))
            {
                try
                {
                    SqlConnection objSqlConnection2 = new SqlConnection(m_strConnectionString);
                    SqlConnection objSqlConnection3 = new SqlConnection(m_strConnectionString);

                    objSqlConnection1.Open();
                    objSqlConnection2.Open();
                    objSqlConnection3.Open();

                    SqlTransaction objSqlTransaction1 = objSqlConnection1.BeginTransaction();
                    SqlTransaction objSqlTransaction2 = objSqlConnection2.BeginTransaction();
                    SqlTransaction objSqlTransaction3 = objSqlConnection3.BeginTransaction();

                    SqlCommand objSqlCommand1 = new SqlCommand(strQuery1, objSqlConnection1, objSqlTransaction1);
                    SqlCommand objSqlCommand2 = new SqlCommand(strQuery2, objSqlConnection2, objSqlTransaction2);
                    SqlCommand objSqlCommand3 = new SqlCommand(strQuery3, objSqlConnection3, objSqlTransaction3);

                    try
                    {
                        objSqlCommand3.CommandTimeout = 600;
                        objSqlCommand3.ExecuteNonQuery();
                        objSqlTransaction3.Commit();
                    }
                    catch (Exception Ex)
                    {
                        objSqlTransaction3.Rollback();
                        m_strErrorMessage = Ex.Message;
                        throw new Exception("An error occured while excecuting the query.But  rollbacked successfully" + Ex.InnerException);
                    }

                    try
                    {
                        objSqlCommand1.CommandTimeout = 600;
                        objSqlCommand1.ExecuteNonQuery();
                    }
                    catch (Exception Ex)
                    {

                        objSqlTransaction1.Rollback();
                        m_strErrorMessage = Ex.Message;
                        throw new Exception("An error occured while excecuting the query.But  rollbacked successfully" + Ex.InnerException);

                    }
                    try
                    {
                        objSqlCommand2.CommandTimeout = 600;
                        objSqlCommand2.ExecuteNonQuery();
                    }
                    catch (Exception Ex)
                    {
                        objSqlTransaction2.Rollback();
                        m_strErrorMessage = Ex.Message;
                        throw new Exception("An error occured while excecuting the query.But  rollbacked successfully" + Ex.InnerException);
                    }

                    objSqlTransaction1.Commit();
                    objSqlTransaction2.Commit();
                    return true;
                }
                catch (Exception Ex)
                {
                    throw new Exception("An error occured while processing the transaction(commit or rollback failed)", Ex.InnerException);
                }
            }
        }
        #endregion

        #region Compact SQL Server database
        public static bool CompactDatabase(string strDatabaseName)
        {
            bool bResult = false;
            SqlConnection objSqlConnection = null;
            try
            {
                objSqlConnection = new SqlConnection(m_strConnectionString);
                SqlCommand objSqlCommand = new SqlCommand();
                objSqlConnection.Open();
                objSqlCommand.Connection = objSqlConnection;
                objSqlCommand.CommandTimeout = 600;
                objSqlCommand.CommandType = CommandType.Text;
                objSqlCommand.CommandText = string.Format("DBCC SHRINKDATABASE(N'{0}' )", strDatabaseName);
                if (objSqlCommand.ExecuteNonQuery() > 0)
                {
                    bResult = true;
                }
            }
            catch (Exception Ex)
            {
                throw new Exception("An error occured while compacting the database",Ex.InnerException);
            }
            finally
            {
                objSqlConnection.Close();
            }
            return bResult;
        }
        #endregion

        public void ExecuteStoredProcedureCreateScript(string filename)
        {
            SqlConnection objSqlConnection = null;
            try
            {
                objSqlConnection = new SqlConnection(m_strConnectionString);
                objSqlConnection.Open();
                m_objSqlCommand.Connection = objSqlConnection;
                m_objSqlCommand.CommandType = CommandType.Text;
                FileInfo file = new FileInfo(filename);
                string scriptText = file.OpenText().ReadToEnd();

                string[] splitter = new string[] { "\r\nGO\r\n" };
                string[] commandTexts = scriptText.Split(splitter, StringSplitOptions.RemoveEmptyEntries);
                foreach (string commandText in commandTexts)
                {
                    using (SqlCommand command = new SqlCommand(commandText, objSqlConnection))
                    {
                        command.ExecuteNonQuery();
                    }
                }                
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message.ToString());
                throw ex;
            }
            finally
            {
                objSqlConnection.Close();
            }
        }

        public void ExecuteCEQuery(string strQuery)
        {
            SqlCeConnection connection = null;
            try
            {
                connection = new SqlCeConnection(m_strConnectionStringCompact);
                connection.Open();
                SqlCeCommand command = new SqlCeCommand(strQuery, connection);
                command.ExecuteNonQuery();

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message.ToString());
                throw ex;
            }

            finally
            {
                connection.Dispose();
                connection.Close();
            }
        }

        #endregion
    }
}
