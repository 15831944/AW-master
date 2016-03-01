using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Data;
using System.Windows.Forms;

namespace Layton.AuditWizard.Common
{
	#region Operation Class

	/// <summary>
	/// This class represents a single operation with AuditWizard which is to be performed by an external
	/// AuditWizard service.  Typical operations would be Deploying, Starting and Stopping AuditAgents
	/// </summary>
	public class Operation
	{
		#region Data
		/// <summary>This enumeration defines the operations which may be performed</summary>
		public enum OPERATION { any = -1, deployagent ,startagent ,stopagent ,removeagent ,clearagentlog ,reaudit ,checkstatus};

		/// <summary>This enumeration lists the possible states which an Operation may be in</summary>
		public enum STATUS	{ any = -1, none ,pending, active ,complete_success ,complete_error };

		private int			_OperationID;
		private OPERATION	_operation;
		private DateTime	_startDate;
		private DateTime	_endDate;
		private int			_assetID;
		private string		_assetName;
		private STATUS		_status;
		private string		_errorText;

#endregion Data

		#region Properties

		public int OperationID
		{
			get { return _OperationID; }
			set { _OperationID = value; }
		}

		public OPERATION OperationType
		{
			get { return _operation; }
			set { _operation = value; }
		}

		public DateTime StartDate
		{
			get { return _startDate; }
			set { _startDate = value; }
		}

		public DateTime EndDate
		{
			get { return _endDate; }
			set { _endDate = value; }
		}

		public int AssetID
		{
			get { return _assetID; }
			set { _assetID = value; }
		}

		public STATUS Status
		{
			get { return _status; }
			set { _status = value; }
		}

		public string ErrorText
		{
			get { return _errorText; }
			set { _errorText = value; }
		}

		public string AssetName
		{
			get { return _assetName; }
		}

		public string OperationAsString
		{
			get
			{
				string value = "";
				switch ((int)_operation)
				{
					case (int)OPERATION.any: value = "Any"; break;
					case (int)OPERATION.checkstatus: value = "Check AuditAgent Status"; break;
					case (int)OPERATION.clearagentlog: value = "Clear AuditAgent Log File"; break;
					case (int)OPERATION.deployagent: value = "Deploy AuditAgent"; break;
					case (int)OPERATION.reaudit: value = "Reaudit Asset"; break;
					case (int)OPERATION.removeagent: value = "Remove AuditAgent"; break;
					case (int)OPERATION.startagent: value = "Start AuditAgent"; break;
					case (int)OPERATION.stopagent: value = "Stop AuditAgent"; break;
				}
				return value;
			}
		}

		public string StatusAsString
		{
			get
			{
				string value = "";
				switch ((int)_status)
				{
					case (int)STATUS.any: value = "Any"; break;
					case (int)STATUS.active: value = "Active"; break;
					case (int)STATUS.complete_error: value = "Completed with Error"; break;
					case (int)STATUS.complete_success: value = "Completed Successfully"; break;
					case (int)STATUS.none: value = "Not Processed"; break;
					case (int)STATUS.pending: value = "Processing..."; break;
				}
				return value;
			}
		}

		#endregion Properties

		#region Constructors

		public Operation()
		{
			_OperationID = 0;
			_operation = OPERATION.deployagent;
			_assetID = 0;
			_assetName = "";
			_startDate = new DateTime(0);
			_endDate = new DateTime(0);
			_status = STATUS.none;
			_errorText = "";
		}

		public Operation(int assetID, OPERATION operation) : this()
		{
			_operation = operation;
			_assetID = assetID;
			_startDate = DateTime.Now;
			_status = STATUS.pending;
		}

		public Operation(DataRow dataRow)
		{
			try
			{
				this.OperationID =	(int)dataRow["_OPERATIONID"];
				this.OperationType = (OPERATION)dataRow["_OPERATION"];
				this.StartDate =	(DateTime)dataRow["_START_DATE"];
				if (!dataRow.IsNull("_END_DATE"))
					this.EndDate =	(DateTime)dataRow["_END_DATE"];
				this.AssetID =		(int)dataRow["_ASSETID"];
				this._assetName =	(string)dataRow["ASSETNAME"];
				this.Status =		(STATUS)dataRow["_STATUS"];
				this.ErrorText =	(string)dataRow["_ERRORTEXT"];
			}
			catch (Exception ex)
			{
				System.Windows.Forms.MessageBox.Show("Exception occured creating an OPERATION Object, please check database schema.  The message was " + ex.Message);
			}
		}
#endregion Constructors

		#region Methods

		/// <summary>
		/// Add a new Operation to the database (or possibly update an existing item)
		/// </summary>
		/// <returns></returns>
		public int Add()
		{
			AuditWizardDataAccess lwDataAccess = new AuditWizardDataAccess();
			if (OperationID == 0)
				OperationID = lwDataAccess.OperationAdd(this);
			else
				lwDataAccess.OperationUpdate(this);
			return 0;
		}


		/// <summary>
		/// Update an existing Operation in the database (or possibly add a new one)
		/// </summary>
		/// <returns></returns>
		public int Update()
		{
			return Add();
		}

		/// <summary>
		/// Delete this Operation from the database
		/// </summary>
		/// <returns></returns>
		public int Delete()
		{
			// First remove the reference to the Operation from the database
			AuditWizardDataAccess lwDataAccess = new AuditWizardDataAccess();
			lwDataAccess.OperationDelete(this);
			return 0;
		}

		#endregion Methods
	}

	#endregion Operation Class

	#region OperationList Class

	public class OperationList : List<Operation>
	{
		public int Populate(Operation.OPERATION operationType ,Operation.STATUS status)
		{
			// Ensure that the list is empty initially
			this.Clear();

			AuditWizardDataAccess lwDataAccess = new AuditWizardDataAccess();
			DataTable table = lwDataAccess.EnumerateOperations(operationType, status);

			// Iterate through the returned rows in the table and create AssetType objects for each
			foreach (DataRow row in table.Rows)
			{
				AddOperation(row);
			}
			return this.Count;
		}


		/// <summary>
		/// Add a new Operation to the list given a database row
		/// </summary>
		/// <param name="row"></param>
		/// <returns></returns>
		public Operation AddOperation(DataRow row)
		{
			// Create the assettype object
			Operation Operation = new Operation(row);
			this.Add(Operation);
			return Operation;
		}
	}

	#endregion OperationList Class

}
