using System;
using System.IO;
using System.Collections.Generic;
using System.Configuration;
using System.Text;
using System.ServiceProcess;
using System.Management;
using System.ComponentModel;
using System.Net.NetworkInformation;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;

using System.Security.Principal;

using Layton.Common.Controls;

namespace Layton.AuditWizard.Common
{
	public class LaytonServiceController
	{
        protected static readonly log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        protected static bool IsDebugEnabled = logger.IsDebugEnabled;

		public enum ServiceStatus
		{
			Running = 0,
			Stopped,
			Paused,
			Busy,
			NotInstalled,
			UnableToConnect
		}

		#region DLLImport

		[DllImport("advapi32.dll", SetLastError = true)]
		public static extern IntPtr OpenSCManager(string lpMachineName, string lpSCDB, int scParameter);

		[DllImport("advapi32.dll", SetLastError = true)]
		public static extern IntPtr CreateService(IntPtr SC_HANDLE, string lpSvcName, string lpDisplayName,
		int dwDesiredAccess, int dwServiceType, int dwStartType, int dwErrorControl, string lpPathName,
		string lpLoadOrderGroup, int lpdwTagId, string lpDependencies, string lpServiceStartName, string lpPassword);

		[DllImport("advapi32.dll", SetLastError = true)]
		public static extern int StartService(IntPtr SVHANDLE, int dwNumServiceArgs, string lpServiceArgVectors);

		[DllImport("advapi32.dll", SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool ControlService(IntPtr hService, int dwControl, ref SERVICE_STATUS lpServiceStatus);

		[DllImport("advapi32.dll", SetLastError = true)]
		public static extern IntPtr OpenService(IntPtr SCHANDLE, string lpSvcName, int dwNumServiceArgs);

		[DllImport("advapi32.dll", SetLastError = true)]
		public static extern int DeleteService(IntPtr SVHANDLE);

		[DllImport("advapi32.dll", SetLastError = true)]
		public static extern void CloseServiceHandle(IntPtr SCHANDLE);

		[DllImport("kernel32.dll", SetLastError = true)]
		public static extern int GetLastError();

		[DllImport("Mpr.dll", EntryPoint = "WNetGetLastError", SetLastError = true)]
		public static extern int WNetGetLastError(ref int lpError, string lpErrorBuf, int nErrorBufSize, string lpNameBuf, int nNameBufSize);

		[DllImport("Mpr.dll", SetLastError = true)]
		private static extern int WNetAddConnection2(NETRESOURCE lpNetResource, string lpPassword, string lpUsername, System.UInt32 dwFlags);

		[DllImport("Mpr.dll", SetLastError = true)]
		private static extern int WNetCancelConnection2(string remoteResource, System.UInt32 dwFlags, bool forceClose);

		[StructLayout(LayoutKind.Sequential)]
		public class NETRESOURCE
		{
			public int dwScope = 0;
			public int dwType = 0;
			public int dwDisplayType = 0;
			public int dwUsage = 0;
			public string lpLocalName = null;
			public string lpRemoteName = null;
			public string lpComment = null;
			public string lpProvider = null;
		};

		[StructLayout(LayoutKind.Sequential)]
		public struct SERVICE_STATUS
		{
			public uint serviceType;
			public uint currentState;
			public uint controlsAccepted;
			public uint win32ExitCode;
			public uint serviceSpecificExitCode;
			public uint checkPoint;
			public uint waitHint;
		};

		#endregion DLLImport
	
		#region Constants declaration.

		private static int STOP = 0x00000001;
		private static int SC_MANAGER_CREATE_SERVICE = 0x0002;
		private static int SERVICE_WIN32_OWN_PROCESS = 0x00000010;
		private static int SERVICE_ERROR_NORMAL = 0x00000001;
		private static int STANDARD_RIGHTS_REQUIRED = 0xF0000;
		private static int SERVICE_QUERY_CONFIG = 0x0001;
		private static int SERVICE_CHANGE_CONFIG = 0x0002;
		private static int SERVICE_QUERY_STATUS = 0x0004;
		private static int SERVICE_ENUMERATE_DEPENDENTS = 0x0008;
		private static int SERVICE_START = 0x0010;
		private static int SERVICE_STOP = 0x0020;
		private static int SERVICE_PAUSE_CONTINUE = 0x0040;
		private static int SERVICE_INTERROGATE = 0x0080;
		private static int SERVICE_USER_DEFINED_CONTROL = 0x0100;
		private static int SERVICE_AUTO_START = 0x00000002;
		private static int GENERIC_WRITE = 0x40000000;
		private static int SERVICE_ALL_ACCESS = (STANDARD_RIGHTS_REQUIRED |
							SERVICE_QUERY_CONFIG |
							SERVICE_CHANGE_CONFIG |
							SERVICE_QUERY_STATUS |
							SERVICE_ENUMERATE_DEPENDENTS |
							SERVICE_START |
							SERVICE_STOP |
							SERVICE_PAUSE_CONTINUE |
							SERVICE_INTERROGATE |
							SERVICE_USER_DEFINED_CONTROL);
		private static int ERROR_SERVICE_NOT_EXISTS = 1060;
		#endregion Constants declaration.

		/// <summary>This is the fully qualified path to the executable file which implements the service </summary>
		private string _serviceExecutable;

		/// <summary>This is the name of the computer on which the service to be controlled resides</summary>
		private string _computerName;

		/// <summary>This is the name that will be given to the service</summary>
		private string _serviceName;

		/// <summary>This is the desription that will be given to the service</summary>
		private string _serviceDisplayName;

		/// <summary>These are the credentials (if any) that will be used to connect to the remote computer
		/// and/or install services</summary>
		private string _username;
		private string _password;

		/// <summary>This is the last WIN32 error code returned</summary>
		protected int _lastWin32Error = 0;

		/// <summary>
		/// This flag is set if we are to use a 'Ping' function to determine if a client can be connected to 
		/// before actually trying to connect.
		/// </summary>
		private bool _usePingToEstablishConnectivity = true;

		/// <summary>
		/// Constructor
		/// </summary>
		public string ServiceExecutable
		{
			get { return _serviceExecutable; }
			set { _serviceExecutable = value; } 
		}

		public string ComputerName
		{
			get { return _computerName; }
			set { _computerName = value; }
		}

		public string ServiceName
		{
			get { return _serviceName; }
			set { _serviceName = value; }
		}

		public string ServiceDisplayName
		{
			get { return _serviceDisplayName; }
			set { _serviceDisplayName = value; }
		}

		public bool UsePingToEstablishConnectivity
		{
			get { return _usePingToEstablishConnectivity; }
			set { _usePingToEstablishConnectivity = value; }
		}

		/// <summary>Set the credentials to use for all subsequent requests.  Pass a NULL or blank username
		/// to remove any existing credentials</summary>
		///
		/// <param name="username"></param>
		/// <param name="password"></param>
		public void SetCredentials(string username, string password)
		{ 
			_username = username; 
			_password = password; 
		}
		

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="serviceExecutable"></param>
		/// <param name="serviceName"></param>
		/// <param name="computerName"></param>
		public LaytonServiceController(string serviceExecutable, string serviceName, string serviceDisplayName ,string computerName)
		{
			_serviceExecutable = serviceExecutable;
			_serviceName = serviceName;
			_serviceDisplayName = serviceDisplayName;
			_computerName = computerName;
			_username = null;
			_password = null;

			// Pick up the default status of the 'Use Ping to Establish Connectivity' flag from the framework
			// configuration file
			Configuration config = ConfigurationManager.OpenExeConfiguration(Path.Combine(Application.StartupPath, "AuditWizardv8.exe"));
			try
			{
				_usePingToEstablishConnectivity = Convert.ToBoolean(config.AppSettings.Settings["PingConnections"].Value);
			}
			catch (Exception)
			{
				_usePingToEstablishConnectivity = true;
			}		
		}

		public LaytonServiceController() : this("" ,"" ,"" ,"")
		{
		}


		/// <summary>
		/// This method installs and runs the service in the service control manager.
		/// </summary>
		/// <returns>True if the process went through successfully. False if there was any error.</returns>
		/// 
		/// Any errors encountered here are thrown as Win32Exceptions
		/// 
		public virtual void Install()
		{
			int errorNumber = 0;

			// Start by opening the SC Manager
			IntPtr scmHandle = OpenSCManager(_computerName, null, SC_MANAGER_CREATE_SERVICE);
			if (scmHandle.ToInt32() != 0)
			{
				// Install the service
				IntPtr serviceHandle = CreateService(scmHandle
													, _serviceName
													, _serviceDisplayName
													, SERVICE_ALL_ACCESS
													, SERVICE_WIN32_OWN_PROCESS
													, SERVICE_AUTO_START
													, SERVICE_ERROR_NORMAL
													, this._serviceExecutable
													, null, 0, null
													, _username
													, _password);
				if (serviceHandle.ToInt32() != 0)
				{
                    long result = LsaUtility.SetRight(_username, "SeServiceLogonRight", _computerName);

					//SERVICE_DESCRIPTION sdBuf;
					//sdBuf.lpDescription = _serviceDescription;

					// Set the service description if specified
					//ChangeServiceConfig2(serviceHandle, SERVICE_CONFIG_DESCRIPTION, &sdBuf);
					CloseServiceHandle(serviceHandle);
				}
				else
				{
					errorNumber = Marshal.GetLastWin32Error();
				}
				CloseServiceHandle(scmHandle);
			}
			else
			{
				errorNumber = Marshal.GetLastWin32Error();
			}

			// If we have an error then throw it as an exception
			if (errorNumber != 0)
				ThrowWin32Error(errorNumber);
		}



		/// <summary>
		/// This method uninstalls the service from the service conrol manager.
		/// </summary>
		public virtual bool Remove()
		{
            bool isRemoved = false;
			int errorNumber = 0;

			// Open a connection to the remote computers IPC$ share
			// Open the Service Control Manager on the (remote) computer
			IntPtr scmHandle = OpenSCManager(_computerName, null, GENERIC_WRITE);
			if (scmHandle.ToInt32() != 0)
			{
				int DELETE = 0x10000;
				IntPtr serviceHandle = OpenService(scmHandle, _serviceName, DELETE);
				if (serviceHandle.ToInt32() != 0)
				{
					int status = DeleteService(serviceHandle);
					isRemoved = (status != 0);
					if (!isRemoved)
						errorNumber = Marshal.GetLastWin32Error();

					// Close the service handle
					CloseServiceHandle(serviceHandle);
				}
				CloseServiceHandle(scmHandle);
			}

			// If we have an error then throw it as an exception
			if (errorNumber != 0)
				ThrowWin32Error(errorNumber);

			return isRemoved;
		}


		/// <summary>
		/// Start the specified service
		/// </summary>
		public void Start()
		{
			int errorNumber = 0;

			// Start by opening the SC Manager
			IntPtr scmHandle = OpenSCManager(_computerName, null, SC_MANAGER_CREATE_SERVICE);
			if (scmHandle.ToInt32() != 0)
			{
				// get the service
				IntPtr serviceHandle = OpenService(scmHandle, _serviceName, SERVICE_ALL_ACCESS);
				if (serviceHandle.ToInt32() != 0)
				{
					// start the service
					StartService(serviceHandle, 0, null);
					CloseServiceHandle(serviceHandle);
				}
				else
				{
					errorNumber = Marshal.GetLastWin32Error();
				}
				CloseServiceHandle(scmHandle);
			}
			else
			{
				errorNumber = Marshal.GetLastWin32Error();
			}

			// If we have an error then throw it as an exception
			if (errorNumber != 0)
				ThrowWin32Error(errorNumber);
		}


		/// <summary>
		/// Stop the specified Windows Service
		/// </summary>
		public void Stop()
		{
			int errorNumber = 0;

			// For some reason this fails if we use alternate credentials so we will remove any specified username
			// and password for now and restore when we have finished
			string saveUsername = _username;
			string savePassword = _password;
			SetCredentials("" ,"");
			
			// Open a connection to the remote computers IPC$ share
			int result = OpenRemoteConnection();
			if (result == 0)
			{
				// Start by opening the SC Manager
				IntPtr scmHandle = OpenSCManager(_computerName, null, SC_MANAGER_CREATE_SERVICE);
				if (scmHandle.ToInt32() != 0)
				{
					// get the service
					IntPtr serviceHandle = OpenService(scmHandle, _serviceName, SERVICE_ALL_ACCESS);
					if (serviceHandle.ToInt32() != 0)
					{
						// start the service
						SERVICE_STATUS status = new SERVICE_STATUS();
						if (!ControlService(serviceHandle, STOP, ref status))
							errorNumber = Marshal.GetLastWin32Error();
						CloseServiceHandle(serviceHandle);
					}
					else
					{
						errorNumber = Marshal.GetLastWin32Error();
					}
					CloseServiceHandle(scmHandle);
				}
				else
				{
					errorNumber = Marshal.GetLastWin32Error();
				}

				// close the remote connection again
				CloseRemoteConnection();
			}

			// reset (any) saved credentials
			SetCredentials(saveUsername ,savePassword);
			
			// If we have an error then throw it as an exception
			if (errorNumber != 0)
				ThrowWin32Error(errorNumber);
		}


		/// <summary>
		/// Check the status of the service on the remote computer
		/// </summary>
		/// <param name="computerName"></param>
		/// <returns></returns>
		public virtual ServiceStatus CheckStatus()
		{
			ServiceStatus status = ServiceStatus.UnableToConnect;
			
			// If requested first 'ping' the remote computer to see if it is turned on
			// If we do not get a response then assume that the PC is unreachable and return that
			if (_usePingToEstablishConnectivity)
			{
				try
				{
					Ping pinger = new Ping();
					PingReply pingReply = pinger.Send(_computerName);
					if (pingReply.Status != IPStatus.Success)
						return status;
				}
				catch (Exception ex)
				{
					string exceptionText;
					if (ex.InnerException is PingException)
						exceptionText = ((PingException)ex.InnerException).Message;
					else
						exceptionText = ex.Message;

					LogFile ourLog = LogFile.Instance;
					string message = String.Format("An exception occurred pinging the remote host {0}, the exception text was {1}.  The host is considered to be offline", _computerName, exceptionText);
					ourLog.Write(message, true);
					return status;	
				}
			}

			// First try and connect to the Service controller and return a generic status if
			// we cannot 
			try
			{
				ServiceController agentController = new ServiceController(this._serviceName);
				if (_computerName != "")
					agentController.MachineName = _computerName;

				ServiceControllerStatus agentStatus = agentController.Status;
				if (agentStatus == ServiceControllerStatus.Running)
					status = ServiceStatus.Running;
				else if (agentStatus == ServiceControllerStatus.Stopped)
					status = ServiceStatus.Stopped;
				else if (agentStatus == ServiceControllerStatus.Paused)
					status = ServiceStatus.Paused;
				else
					status = ServiceStatus.Busy;
			}

			catch(Exception ex) 
			{
				// Check for an error of 'Service does not exist' and return this as not installed
				Win32Exception win32Error = (Win32Exception) ex.InnerException;
				_lastWin32Error = win32Error.NativeErrorCode;
				if (win32Error.NativeErrorCode == ERROR_SERVICE_NOT_EXISTS)
				{
					status = ServiceStatus.NotInstalled;
				}
				else
				{
					LogFile ourLog = LogFile.Instance;
					string message = String.Format("An exception occurred in LaytonServiceController::CheckStatus ,exception text is {0}, Native Error Code is {1}", ex.Message, win32Error.NativeErrorCode);
					ourLog.Write(message, true);
				}
			}
			return status;
		}


		/// <summary>
		/// Copy a list of files to the remote computer
		/// </summary>
		/// <param name="listFiles">List of FULLY QUALIFIED file names to copy</param>
		/// <returns></returns>
		/// 
		/// This function throws an exception with the error text if a copy fails
		/// 
		public void CopyFilesToRemote(List<string> listFiles)
		{
			LogFile ourLog = LogFile.Instance;

            if (IsDebugEnabled)
                logger.Debug("CopyFilesToRemote in");

			// Open a connection to the remote computers IPC$ share
            //int result = OpenRemoteConnection();

            int result = 0;

			if (result != 0)
			{
				string error = "CopyFilesToRemote - Error code [" + result.ToString() + "] returned from OpenRemoteConnection";
				ourLog.Write(error, true);
                logger.Debug("error in CopyFilesToRemote"); 
				throw new Exception(error);
			}
			else
			{
                // check for 64-bit versions first
                string destinationPath = "\\\\" + _computerName + "\\ADMIN$\\syswow64\\";
                
                if (!Directory.Exists(destinationPath))
				    destinationPath = "\\\\" + _computerName + "\\ADMIN$\\system32\\";

                logger.Debug("destination path is : " + destinationPath);

				// copy all the specified files to the system32 folder on the remote machine
                try
                {
                    foreach (String thisFile in listFiles)
                    {
                        string filename = Path.GetFileName(thisFile);
                        string destinationFile = Path.Combine(destinationPath, filename);

                        File.Copy(thisFile, destinationFile, true);

                        if (!File.Exists(destinationFile))
                        {
                            string error = "No exception was thrown but the file does not exist";
                            throw new Exception(error);
                        }
                    }
                }
                catch (Exception e)
                {
                    string error = "Failed to copy file to remote path: " + destinationPath + ".  Reason:  " + e.Message;
                    ourLog.Write(error, true);
                    throw new Exception(error, e);
                }
                //finally
                //{
                //    // close the remote connection
                //    CloseRemoteConnection();
                //}
			}
		}

		/// <summary>
		/// Restart the service
		/// </summary>
		public void RestartService()
		{
			if (CheckStatus() == LaytonServiceController.ServiceStatus.Running)
			{
				using (new WaitCursor())
				{
					Stop();

					// Wait for it to stop
					for (int index = 0; index < 5; index++)
					{
						if (CheckStatus() == LaytonServiceController.ServiceStatus.Stopped)
							break;
						Thread.Sleep(1000);
					}

					// Start it again
					Start();
				}
			}
		}


		/// <summary>
		/// Delete remote files - note that this is protected so that we do not allow deletion of critical
		/// files too easily...
		/// </summary>
		/// <param name="listFiles"></param>
		protected void DeleteRemoteFiles(List<string> listFiles)
		{
			// Open a connection to the remote computers IPC$ share
			int result = OpenRemoteConnection();

			// ...if sucessfull then delete the files on the remote computer
			if (result == 0)
			{
                // check for 64-bit versions first
                string destinationPath = "\\\\" + _computerName + "\\ADMIN$\\syswow64\\";

                if (!Directory.Exists(destinationPath))
                    destinationPath = "\\\\" + _computerName + "\\ADMIN$\\system32\\";

				// delete all the client files from the system32 folder on the remote machine
				try
				{
					foreach (string thisFile in listFiles)
					{
						File.Delete(destinationPath + thisFile);
					}
				}
				catch (Exception)
				{
					CloseRemoteConnection();
					return;
				}

				// close the remote connection again
				CloseRemoteConnection();
			}
		}



		/// <summary>
		/// View the contents of the specified remote file
		/// </summary>
		/// <param name="filename"></param>
		protected void ViewRemoteFile(string filename)
		{
			// Open a connection to the remote computers IPC$ share
			int result = OpenRemoteConnection();
			if (result == 0)
			{
				string destinationFile = String.Empty;
				try
				{
                    // check for 64-bit versions first
                    string destinationPath = "\\\\" + _computerName + "\\ADMIN$\\syswow64\\";

                    if (Directory.Exists(destinationPath))
                        destinationFile = "\\\\" + _computerName + "\\ADMIN$\\syswow64\\" + filename;
                    else
                        destinationFile = "\\\\" + _computerName + "\\ADMIN$\\system32\\" + filename;

					if (File.Exists(destinationFile))
						System.Diagnostics.Process.Start(destinationFile);
					else
						throw new Exception("The log file does not exist.");
				}
				catch (Exception e)
				{
					throw new Exception("Failed to read the remote file : " + destinationFile + ".  Reason:  " + e.Message, e);
				}

				// close the remote connection again
				CloseRemoteConnection();
			}
		}



		/// <summary>
		/// Opens a connection to the remote computer
		/// </summary>
		/// <param name="computerName"></param>
		/// <returns></returns>
		public int OpenRemoteConnection()
        {
            uint ERROR_EXISTING_CONNECTION = 1219;
			int connResult;
			NETRESOURCE remoteConn = new NETRESOURCE();
			remoteConn.lpRemoteName = "\\\\" + _computerName + "\\IPC$";
            connResult = WNetAddConnection2(remoteConn, _password, _username, 0);
            if (connResult == ERROR_EXISTING_CONNECTION)
                connResult = CloseRemoteConnection();
			return connResult;
		}

		/// <summary>
		/// Close a previously established remote connection
		/// </summary>
		/// <returns></returns>
		public int CloseRemoteConnection()
		{
			string remoteResource = "\\\\" + _computerName + "\\IPC$";

			const int dwFlags = 0;
			int err = 0;
			err = WNetCancelConnection2(remoteResource, dwFlags, true);
			if ((err != 0) && (err != 2250))				//2250 = connection does not exist
				return err;
			return 0;
		}


		/// <summary>
		/// Construct and throw Win32Exception from the supplied error 
		/// </summary>
		/// <param name="errorNumber"></param>
		protected void ThrowWin32Error(int errorNumber)
		{
			if (errorNumber != 0)
			{
				Win32Exception eWin32 = new Win32Exception(errorNumber);
				throw new Exception(eWin32.Message);
			}
		}

	}
}
