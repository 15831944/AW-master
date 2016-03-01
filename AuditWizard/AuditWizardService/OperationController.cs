using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Threading;
using System.Timers;
//
using Layton.AuditWizard.Common;
using Layton.Common.Controls;
using Layton.AuditWizard.DataAccess;

namespace Layton.AuditWizard.AuditWizardService
{
#region OperationThreadData Class

	public class OperationThreadData
	{
		#region Data

		private Operation _operation;
		private Thread _thread = null;
		private DateTime _startDate = DateTime.Now;

		#endregion Data

		#region Properties

		public Operation ActiveOperation
		{
			get { return _operation; }
			set { _operation = value; }
		}

		public Thread ActiveThread
		{
			get { return _thread; }
			set { _thread = value; }
		}

		public DateTime StartDate
		{
			get { return _startDate; }
		}
		#endregion Properties

		#region Constructor

		public OperationThreadData ()
		{
		}

		#endregion Constructor

		#region Methods

		public bool IsExpired()
		{
			// Threads have a maximum life of 60 seconds
			DateTime now = DateTime.Now;
			TimeSpan ts = now - StartDate;
			return (ts.TotalSeconds > 60);
		}
		#endregion Methods
	}
#endregion OperationThreadData Class

#region OperationController Class

	public class OperationController
	{
		private AuditWizardService _service;
		private System.Timers.Timer operationTimer = new System.Timers.Timer(5000);

		/// <summary>
		/// This is the index of the last operation of which the controller is aware.  We check this
		/// periodically to determine whether we need to process new operations as it is quicker to get a single
		/// count than to return rows of data
		/// </summary>
		private int _lastOperation = 0;

		/// <summary>
		/// This is the pool of threads which we can use to perform operations
		/// </summary>
		protected List<OperationThreadData> _threadPool = new List<OperationThreadData>();

		/// <summary>
		/// This is the maximum number of threads which we will allow
		/// </summary>
		private static int MAX_THREADS = 10;

		/// <summary>
		/// This is the list of operations which have not been passed to a thread yet but have been identified
		/// as having been queued.
		/// </summary>
		OperationList _listOutstandingOperations = new OperationList();

		/// <summary>
		/// This is the primary thread for the Operations Controller.
		/// </summary>
		Thread _mainThread = null;
		
		/// <summary>This flag should be set when we want the underlying main thread to exit</summary>
		bool   _mainThreadStop = false;

		/// <summary>
		/// Constructor
		/// </summary>
		public OperationController(AuditWizardService service)
		{
			_service = service;
		}


		/// <summary>
		/// Called to start the Task
		/// </summary>
        public void Start()
        {
			LogFile ourLog = LogFile.Instance;
			ourLog.Write("Creating the Main Operations Controller Thread...", true);
			
			// The Operations Controller runs in its own thread to prevent any errors here causing issues
			// with other parts of the AuditWizard Service
			_mainThread = new Thread(new ThreadStart(OperationThreadStart));
			_mainThread.Start();
			ourLog.Write("Main Operations Controller Thread Running", true);
		}
		
		/// <summary>
		/// Called to stop the Operations Controlller
		/// </summary>
		public void Stop()
		{
			LogFile ourLog = LogFile.Instance;
			ourLog.Write("Stopping the Main Operations Controller Thread...", true);
			_mainThreadStop = true;
		}



		/// <summary>
		/// This is the main entry point for the Operatuions Controller Main Thread.
		/// We sit in this thread until we are requested to close down
		/// </summary>
		private void OperationThreadStart()
        {
			LogFile ourLog = LogFile.Instance;
			ourLog.Write("OperationThreadStart:: operations Controller Thread active...", true);

			// ...set a timer to go off every 5 seconds
			operationTimer.Elapsed += new ElapsedEventHandler(operationTimer_Elapsed);
			operationTimer.Start();

			while (!_mainThreadStop)
			{
				Thread.Sleep(2000);
			}
			
			// If we get here then the thread has been requested to close down - stop the timer first then exit
			operationTimer.Elapsed -= operationTimer_Elapsed;
			operationTimer.Stop();
			ourLog.Write("OperationThreadStart:: operations Controller Thread exited...", true);
		}



		/// <summary>
		/// Called as the Operation timer expired
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		void operationTimer_Elapsed(object sender, ElapsedEventArgs e)
        {	
			// Temparily disable the timer as we don't want it stacking up if we are busy
			operationTimer.Elapsed -= operationTimer_Elapsed;
			operationTimer.Stop();

			// Now check the operations
			CheckOperations();

			// Re-enable the timer
			operationTimer.Elapsed += new ElapsedEventHandler(operationTimer_Elapsed);
			operationTimer.Start();
		}


		/// <summary>
		/// This function is called periodically by the expiry of the refresh timer to check for 
		/// and process any outstanding operations
		/// </summary>
		protected void CheckOperations()
		{
			LogFile ourLog = LogFile.Instance;
			try
			{
				// Do we have any items left in our 'outstanding queue' - If not check to see if anymore
				// have been queued
				if (_listOutstandingOperations.Count == 0)
				{
					// OK no operations left in the queue but have new ones been queued in the database?
                    OperationsDAO lwDataAccess = new OperationsDAO();
					int lastOperation = lwDataAccess.OperationGetLastIndex();
					if (lastOperation > _lastOperation)
					{
						// New Operations have been queued - recover them
						_listOutstandingOperations.Populate(Operation.OPERATION.any, Operation.STATUS.none);

						// Update the last operation index - the returned operations are ordered by their ID
						if (_listOutstandingOperations.Count > 0)
						{
							Operation operation = _listOutstandingOperations[_listOutstandingOperations.Count - 1];
							_lastOperation = operation.OperationID;
						}
					}
				}
					
				// Check for threads which need to be killed off as they have timed out
				CheckTimedOutOperations();

				// Now loop round checking any outstanding operations unless and until we are returned false
				bool status;
				do
				{
					status = ProcessOperations();
				}
				while (status == true);
			}
			
			catch (Exception ex)
			{
				ourLog.Write("CheckOperations:: An exception has occurred, error text: " + ex.Message ,true);
				return;
			}
		}


		/// <summary>
		/// This function checks each entry in the thread pool and aborts any threads which have exceeded 
		/// the maximum time permitted for a thread to complete.  
		/// </summary>
		protected void CheckTimedOutOperations()
		{
			foreach (OperationThreadData operationThread in _threadPool)
			{
				if (operationThread.IsExpired())
				{
					Operation operation = operationThread.ActiveOperation;
					operation.Status = Operation.STATUS.complete_error;
					operation.ErrorText = "Error: Operation has timed out";

					// Update the Operation itself
					OperationComplete(operationThread);
					
					// Abort the thread
					Thread thread = operationThread.ActiveThread;
					thread.Abort();
				}
			}
		}



		/// <summary>
		/// Process the current outstanding list of operations
		/// </summary>
		protected bool ProcessOperations()
		{
			// If no more operations simply return false now
			if (_listOutstandingOperations.Count == 0)
				return false;

			// Create a log file
			LogFile ourLog = LogFile.Instance;

			// We use a separate thread for each upto a maximum of 10 threads 
			// If we have exceeded the thread limit then we need to wait for one to become availab;e
			if (_threadPool.Count >= MAX_THREADS)
				return false;

			// We have a spare entry in the Thread pool - allocate it and pass the operation to the thread to perform
			OperationThreadData operationThreadData = new OperationThreadData();
			operationThreadData.ActiveOperation = _listOutstandingOperations[0];
			operationThreadData.ActiveOperation.Status = Operation.STATUS.pending;
			ourLog.Write("Processing Operation for " + operationThreadData.ActiveOperation.AssetName, true);

			// remove the operation from the list now that we have allocated it to a thread
			_listOutstandingOperations.RemoveAt(0);

			// Add the OperationThreadData to the thread pool
			_threadPool.Add(operationThreadData);

			// Mark the operation as 'Pending' in the database
			OperationsDAO lwDataAccess = new OperationsDAO();
			lwDataAccess.OperationUpdate(operationThreadData.ActiveOperation);

			// Create the actual Windows Thread and start it passing the OperationThreadData object as a parameter
			BackgroundWorker operationThread = new System.ComponentModel.BackgroundWorker();
			operationThread.WorkerReportsProgress = false;
			operationThread.DoWork += new System.ComponentModel.DoWorkEventHandler(operationThread_DoWork);
			operationThread.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(operationThread_WorkerCompleted);
			operationThread.RunWorkerAsync(operationThreadData);

			//Thread thread = new Thread(new ParameterizedThreadStart(PerformOperation));
			//thread
			//thread.Start(operationThread);
			return true;
		}


		#region Thread Processing Functions

		/// <summary>
		/// This function is called from within the Operations Thread to perform the desired operation
		/// </summary>
		/// <param name="operationThread"></param>
		private void operationThread_DoWork(object sender, DoWorkEventArgs e)
		{
			// Recover the OperationThreadData and Operation objects from that passed in to us
			OperationThreadData operationThreadData = e.Argument as OperationThreadData;
			Operation operation = operationThreadData.ActiveOperation;

			// Before we can perform any operation we need to ensure that we have the latest status for
			// any AuditAgent on the target computer - If we fail to get the status then we can go no further
			Asset.AGENTSTATUS assetStatus;
			string message = "Success";
			if (UpdateCurrentAgentStatus(operation.AssetName, operation.AssetID, out assetStatus, out message) != 0)
			{
				operation.Status = Operation.STATUS.complete_error;
				operation.ErrorText = message;
			}

			else
			{
				// Switch to handle the Operation itself
				switch ((int)operation.OperationType)
				{
					case (int)Operation.OPERATION.checkstatus:
						CheckStatus(operationThreadData, assetStatus);
						break;
					case (int)Operation.OPERATION.clearagentlog:
						ClearAgentLog(operationThreadData, assetStatus);
						break;
					case (int)Operation.OPERATION.deployagent:
						DeployAgent(operationThreadData, assetStatus);
						break;
					case (int)Operation.OPERATION.reaudit:
						Reaudit(operationThreadData, assetStatus);
						break;
					case (int)Operation.OPERATION.removeagent:
						RemoveAgent(operationThreadData, assetStatus);
						break;
					case (int)Operation.OPERATION.startagent:
						StartAgent(operationThreadData, assetStatus);
						break;
					case (int)Operation.OPERATION.stopagent:
						StopAgent(operationThreadData, assetStatus);
						break;
					case (int)Operation.OPERATION.updateconfiguration:
						UpdateAgentConfiguration(operationThreadData, assetStatus);
						break;
					default:
						break;
				}
			}

			// Show that the thread is complete - we pause just a little to give the operation a little time to complete
			Thread.Sleep(1000);

			// and exit
			e.Result = operationThreadData;
			return;
		}


		/// <summary>
		/// Called as the operation thread exits - this is in the context of the main thread though so no 
		/// threading issues
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void operationThread_WorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
		{
			LogFile ourLog = LogFile.Instance;
			ourLog.Write("operationThread_WorkerCompleted Start" ,true);
			OperationThreadData operationThreadData = e.Result as OperationThreadData;
			OperationComplete(operationThreadData);
			ourLog.Write("operationThread_WorkerCompleted Start" ,true);
		}


	    /// <summary>
	    /// Called to perform a 'Check Status' Operation on the specified PC
	    /// </summary>
	    /// <param name="operationThread"></param>
	    /// <param name="assetStatus"></param>
	    protected bool CheckStatus(OperationThreadData operationThread, Asset.AGENTSTATUS assetStatus)
		{
			LogFile ourLog = LogFile.Instance;

			// This is the text which we shall display for the operation
			string message = "Success, AgentStatus is " + Asset.TranslateDeploymentStatus(assetStatus);

			// We have actually already performed the sttaus check so need not do it again!
			// Get the operation to be performed
			Operation operation = operationThread.ActiveOperation;
			operation.Status = Operation.STATUS.complete_success;
			operation.ErrorText = message;

			// Return now
			return true;
		}


		/// <summary>
		/// Called to perform a 'Clear Agent Log' Operation on the specified PC
		/// </summary>
		/// <param name="operationThread"></param>
		protected bool ClearAgentLog(OperationThreadData operationThread, Asset.AGENTSTATUS assetStatus)
		{
			// This is the status of the operation
			bool success = true;

			// This is the text which we shall display for the operation
			string message = "Success";

			// Get the operation to be performed
			Operation operation = operationThread.ActiveOperation;

			try
			{
				// Get the Agent Service Controller passing it the name of the computer
				AuditAgentServiceController agentServiceController = new AuditAgentServiceController(operation.AssetName);

				// Use it to get the current status of the asset
				success = agentServiceController.ClearLogFile();
				if (!success)
					message = "An error occurred while clearing the AuditAgent log file";
			}
			catch (Exception e)
			{
				success = false;
				message = "Error: An Exception occurred while clearing the AuditAgent log file, the error text was " + e.Message;
			}

			// Update the Operation object with the overall status and completion message
			operation.Status = (success) ? Operation.STATUS.complete_success : Operation.STATUS.complete_error;
			operation.ErrorText = message;

			// Return now
			return success;
		}


		/// <summary>
		/// Called to perform a 'Deploy Agent' Operation on the specified PC
		/// </summary>
		/// <param name="operationThread"></param>
		protected bool DeployAgent(OperationThreadData operationThread, Asset.AGENTSTATUS assetStatus)
		{
			// This is the status of the operation
			bool success = true;

			// This is the text which we shall display for the operation
			string message = "Success";

			// Get the operation to be performed
			Operation operation = operationThread.ActiveOperation;

			try
			{
				// Get the Agent Service Controller passing it the name of the computer
				AuditAgentServiceController agentServiceController = new AuditAgentServiceController(operation.AssetName);

				// Use it to get the current status of the asset

				// If already started, flag this
				if (assetStatus == Asset.AGENTSTATUS.Running)
				{
					message = "The AuditWizard Agent Service is already running";
				}

				else
				{
					// If the agent has not been deployed to this computer previously then do so now
					if (assetStatus == Asset.AGENTSTATUS.notdeployed)
					{
						// Install the Agent Service (throws an exception on error)
						agentServiceController.Install();
						assetStatus = Asset.AGENTSTATUS.deployed;
						AssetDAO lwDataAccess = new AssetDAO();
						lwDataAccess.AssetUpdateAssetStatus(operation.AssetID, assetStatus);				
					}

					// If the status is now deployed then we can start the Agent
					if (assetStatus == Asset.AGENTSTATUS.deployed)
					{
						// Start the agent (throws an exception on error)
						agentServiceController.Start();
					}
				}
			}			
			
			catch (Exception e)
			{
				success = false;
				message = "Error: An Exception occurred while Deploying the AuditAgent, the error text was " + e.Message;
			}

			// Update the Operation object with the overall status and completion message
			operation.Status = (success) ? Operation.STATUS.complete_success : Operation.STATUS.complete_error;
			operation.ErrorText = message;

			// Return now
			return success;
		}


		/// <summary>
		/// Called to perform a 'Reaudit' Operation on the specified PC
		/// </summary>
		/// <param name="operationThread"></param>
		protected bool Reaudit(OperationThreadData operationThread, Asset.AGENTSTATUS assetStatus)
		{
			// This is the status of the operation
			bool success = true;

			// This is the text which we shall display for the operation
			string message = "Success";

			// Get the operation to be performed
			Operation operation = operationThread.ActiveOperation;

			// If the AuditAgent is not active then we fail to do a re-audit as it must be running
			if (assetStatus != Asset.AGENTSTATUS.Running)
			{
				success = false;
				message = "The AuditAgent is not currently active";
			}

			else
			{
				try
				{
					// Get the Agent Service Controller passing it the name of the computer
					AuditAgentServiceController agentServiceController = new AuditAgentServiceController(operation.AssetName);

					// Use it to request a re-audit of the asset
					success = (agentServiceController.RequestReaudit() == 0);
					if (!success)
						message = "An error occurred while requesting a re-audit";
				}
				catch (Exception e)
				{
					success = false;
					message = "Error: An Exception occurred while requesting a re-audit, the error text was " + e.Message;
				}
			}

			// Update the Operation object with the overall status and completion message
			operation.Status = (success) ? Operation.STATUS.complete_success : Operation.STATUS.complete_error;
			operation.ErrorText = message;

			// Return now
			return success;
		}


		/// <summary>
		/// Called to perform a 'Remove Agent' Operation on the specified PC
		/// </summary>
		/// <param name="operationThread"></param>
		protected bool RemoveAgent(OperationThreadData operationThread, Asset.AGENTSTATUS assetStatus)
		{
			// This is the status of the operation
			bool success = true;

			// This is the text which we shall display for the operation
			string message = "Success";

			// Get the operation to be performed
			Operation operation = operationThread.ActiveOperation;

			// If the AuditAgent is not deployed then just log this - it isn't an error as such but there is
			// no point removing the agent if it is not deployed
			if (assetStatus == Asset.AGENTSTATUS.notdeployed)
			{
				message = "The AuditAgent was not deployed";
			}

			else
			{
				try
				{
					// Get the Agent Service Controller passing it the name of the computer
					AuditAgentServiceController agentServiceController = new AuditAgentServiceController(operation.AssetName);

					// If the AuditAgent is RUNNING then we need to stop it before we can remove it
					if (assetStatus == Asset.AGENTSTATUS.Running)
					{
						success = StopAgent(operationThread ,assetStatus);
						Thread.Sleep(2000);
					}

					// Now remove the AudutAgent
					if (success)
					{
						// Use it to get the current status of the asset
						success = agentServiceController.Remove();
						message = (success) ? "Success" : "An error occurred while removing the AuditAgent Service";
					}
				}
				catch (Exception e)
				{
					success = false;
					message = "Error: An Exception occurred while removing the AuditAgent Service, the error text was " + e.Message;
				}
			}

			// Update the Operation object with the overall status and completion message
			operation.Status = (success) ? Operation.STATUS.complete_success : Operation.STATUS.complete_error;
			operation.ErrorText = message;

			// Return now
			return success;
		}


		/// <summary>
		/// Called to perform a 'Start Agent' Operation on the specified PC
		/// </summary>
		/// <param name="operationThread"></param>
		protected bool StartAgent(OperationThreadData operationThread, Asset.AGENTSTATUS assetStatus)
		{
			// This is the status of the operation
			bool success = true;

			// This is the text which we shall display for the operation
			string message = "Success";

			// Get the operation to be performed
			Operation operation = operationThread.ActiveOperation;

			// To Start the agent it must be deployed, if not then this is an error.
			if (assetStatus == Asset.AGENTSTATUS.notdeployed)
			{
				success = false;
				message = "The AuditAgent has not been deployed";
			}

			// Already running is not an error but don't try and start it again
			else if (assetStatus == Asset.AGENTSTATUS.Running)
			{
				message = "The AuditAgent was already active";
			}

			else
			{
				try
				{
					// Get the Agent Service Controller passing it the name of the computer
					AuditAgentServiceController agentServiceController = new AuditAgentServiceController(operation.AssetName);

					// Use it to get the current status of the asset
					agentServiceController.Start();
				}
				catch (Exception e)
				{
					success = false;
					message = "Error: An Exception occurred while starting the AuditAgent, the error text was " + e.Message;
				}
			}
			
			// Update the Operation object with the overall status and completion message
			operation.Status = (success) ? Operation.STATUS.complete_success : Operation.STATUS.complete_error;
			operation.ErrorText = message;

			// Return now
			return success;
		}



		/// <summary>
		/// Called to perform a 'Stop Agent' Operation on the specified PC
		/// </summary>
		/// <param name="operationThread"></param>
		protected bool StopAgent(OperationThreadData operationThread, Asset.AGENTSTATUS assetStatus)
		{
			// This is the status of the operation
			bool success = true;

			// This is the text which we shall display for the operation
			string message = "Success";

			// Get the operation to be performed
			Operation operation = operationThread.ActiveOperation;

			// To STOP the agent it must be deployed, if not then this is an error.
			if (assetStatus == Asset.AGENTSTATUS.notdeployed)
			{
				success = false;
				message = "The AuditAgent has not been deployed";
			}

			// Already STOPPED is not an error but don't try and start it again
			else if (assetStatus == Asset.AGENTSTATUS.deployed)
			{
				message = "The AuditAgent was already stopped";
			}

			else
			{
				try
				{
					// Get the Agent Service Controller passing it the name of the computer
					AuditAgentServiceController agentServiceController = new AuditAgentServiceController(operation.AssetName);

					// Use it to get the current status of the asset
					agentServiceController.Stop();
				}
				catch (Exception e)
				{
					success = false;
					message = "Error: An Exception occurred while stopping the AuditAgent, the error text was " + e.Message;
				}
			}

			// Update the Operation object with the overall status and completion message
			operation.Status = (success) ? Operation.STATUS.complete_success : Operation.STATUS.complete_error;
			operation.ErrorText = message;

			// Return now
			return success;
		}


		/// <summary>
		/// Called as a thread completes its designated operation.  We need to remove the thread from
		/// the thread pool and exit the thread
		/// </summary>
		/// <param name="operationThread"></param>
		protected void OperationComplete(OperationThreadData operationThread)
		{
			LogFile ourLog = LogFile.Instance;
			ourLog.Write("OperationComplete Start" ,true);
			try
			{
				// Update the operation in the database
				Operation operation = operationThread.ActiveOperation;
				operation.EndDate = DateTime.Now;
				operation.Update();

				// Remove this entry from the thread pool
				_threadPool.Remove(operationThread);

				// If the operation was successful and it affected the AuditAgent status then ensure that
				// we now update the status
				if ((operation.Status == Operation.STATUS.complete_success)
				&&  (operation.OperationType != Operation.OPERATION.checkstatus)
				&&  (operation.OperationType != Operation.OPERATION.clearagentlog))
				{
					string message;
					Asset.AGENTSTATUS assetStatus;
					UpdateCurrentAgentStatus(operation.AssetName, operation.AssetID, out assetStatus, out message);
				}
			}
			catch (Exception)
			{
			
			}
			ourLog.Write("OperationComplete End" ,true);
			return;
		}



		/// <summary>
		/// Called to return and update the current status of the specified (named) asset
		/// </summary>
		/// <param name="assetName"></param>
		/// <param name="assetStatus"></param>
		/// <param name="message"></param>
		protected int UpdateCurrentAgentStatus(string assetName, int assetID ,out Asset.AGENTSTATUS assetStatus, out string message)
		{
			// Get the Agent Service Controller passing it the name of the computer
			AuditAgentServiceController agentServiceController = new AuditAgentServiceController(assetName);

			// Use it to get the current status of the asset
			LaytonServiceController.ServiceStatus serviceStatus = agentServiceController.CheckStatus();
			assetStatus = Asset.AGENTSTATUS.notdeployed;

			switch (serviceStatus)
			{
				case LaytonServiceController.ServiceStatus.Running:
					assetStatus = Asset.AGENTSTATUS.Running;
					break;

				case LaytonServiceController.ServiceStatus.Stopped:
					assetStatus = Asset.AGENTSTATUS.deployed;
					break;

				case LaytonServiceController.ServiceStatus.NotInstalled:
					assetStatus = Asset.AGENTSTATUS.notdeployed;
					break;

				case LaytonServiceController.ServiceStatus.UnableToConnect:
					message = "Error: Could not connect to remote computer, please check that it is turned on";
					return -1;

				default:
					message = "Error: An Invalid or Unknown Status was returned";
					return -1;
			}

			// Update the status of the computer in the database if we were successful
			message = Asset.TranslateDeploymentStatus(assetStatus);
			AssetDAO lwDataAccess = new AssetDAO();
			lwDataAccess.AssetUpdateAssetStatus(assetID, assetStatus);

			return 0;
		}


		/// <summary>
		/// Called to perform an 'Update Agent configuration' Operation on the specified PC
		/// </summary>
		/// <param name="operationThread"></param>
		protected bool UpdateAgentConfiguration(OperationThreadData operationThread, Asset.AGENTSTATUS assetStatus)
		{
			// This is the status of the operation
			bool success = true;

			// This is the text which we shall display for the operation
			string message = "Success";

			// Get the operation to be performed
			Operation operation = operationThread.ActiveOperation;

			// To update the agent it must be deployed, if not then this is an error.
			if (assetStatus == Asset.AGENTSTATUS.notdeployed)
			{
				success = false;
				message = "The AuditAgent has not been deployed";
			}

			else
			{
				try
				{
					// Get the Agent Service Controller passing it the name of the computer
					AuditAgentServiceController agentServiceController = new AuditAgentServiceController(operation.AssetName);

					// Use it to get the current status of the asset
					agentServiceController.Update();
				}

				catch (Exception e)
				{
					success = false;
					message = "Error: An Exception occurred while starting the AuditAgent, the error text was " + e.Message;
				}
			}

			// Update the Operation object with the overall status and completion message
			operation.Status = (success) ? Operation.STATUS.complete_success : Operation.STATUS.complete_error;
			operation.ErrorText = message;

			// Return now
			return success;
		}


		#endregion Thread Processing Functions

	}




#endregion OperationController Class
}
