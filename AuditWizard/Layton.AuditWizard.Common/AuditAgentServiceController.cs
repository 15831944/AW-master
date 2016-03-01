using System;
using System.Collections.Generic;
using System.ServiceProcess;
using System.Management;
using System.ComponentModel;
using System.Configuration;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;
using System.Text;
//
using Layton.Common.Controls;
using Layton.AuditWizard.DataAccess;

namespace Layton.AuditWizard.Common
{
	/// <summary>
	/// This class implements a specific version of the generic LaytonServiceController to 
	/// control the AuditWizard Audit Agent Service
	/// </summary>
	public class AuditAgentServiceController : LaytonServiceController
	{
		/// <summary>This is a custom command which we will pass to the AuditAgent when we want it to 
		/// run an audit of the computer immediately irrespective of any re-audit intervals set.</summary>
		private static int AGENT_REQUEST_REAUDIT = 128;

		/// <summary>
		/// Constant strings needeed to identify the agent
		/// </summary>
		private const string AuditWizardAgentServiceName = "AuditWizard Agent";
		private const string AuditWizardAgentServiceDisplayName = "AuditWizard Agent";
		private const string AuditWizardAgentFilesPath	= "AuditWizard Agent";
		private const string AuditWizardAgentExe			= "AuditAgent.exe";
		private const string AuditWizardAgentIni			= "AuditAgent.xml";
		private const string AuditWizardAgentLogFile		= "AuditAgent.log";

		/// <summary>
		/// The main constructor simply takes the computer name and fills in the other parameters
		/// </summary>
		/// <param name="computerName"></param>
		public AuditAgentServiceController(string computerName)
		{
			// Ensure that we configure the base class as we have not called it's extended constructor
			ServiceName = AuditWizardAgentServiceName;
			ServiceDisplayName = AuditWizardAgentServiceDisplayName;
            //ServiceExecutable = @"%SystemRoot%" + "\\system32\\" + AuditWizardAgentExe;	
            ComputerName = computerName;

            // check for 64bit machines
            string destinationPath = "\\\\" + computerName + "\\ADMIN$\\syswow64\\";

            if (Directory.Exists(destinationPath))
                ServiceExecutable = @"%SystemRoot%" + "\\syswow64\\" + AuditWizardAgentExe;
            else
                ServiceExecutable = @"%SystemRoot%" + "\\system32\\" + AuditWizardAgentExe;			

			// Recover any credentials that we will be using
			string username = null;
			string password = null;
			GetCredentials(ref username ,ref password);

			// Set any credentials that we will be using later
			SetCredentials(username ,password);
		}

		
		/// <summary>
		/// Over-ride the base class Install function so that we can do more of the tasks in one go
		/// </summary>
		/// <returns></returns>
		public override void Install ()
		{
			LogFile ourLog = LogFile.Instance;
			ourLog.Write("AuditAgentServiceController::Install", true);

			// First lets see iwhat the current status of the servie is
			LaytonServiceController.ServiceStatus serviceStatus = CheckStatus();

			// If we cannot connect then throw this as an exception
			if (serviceStatus == ServiceStatus.UnableToConnect)
				ThrowWin32Error(_lastWin32Error);

			// If the service shows any sign of being installed then remove it first
			if (serviceStatus != ServiceStatus.NotInstalled)
				Remove();

			// Construct the list of files that the agent will need to be copied to the remote computer
			string sourcePath = Path.Combine(Application.StartupPath, AuditAgentStrings.AuditAgentFilesPath);
			List<string> listFiles = new List<string>();
			listFiles.Add(Path.Combine(sourcePath ,AuditWizardAgentExe));
			listFiles.Add(Path.Combine(sourcePath ,AuditWizardAgentIni));

			// Copy the files to the remote Asset
			// The caller should catch any exceptions thrown as we do not want to continue after a failure
			CopyFilesToRemote(listFiles);

			// Now install the service
			base.Install();
		}


		/// <summary>
		/// RequestReaudit
		/// ==============
		/// 
		/// Request a re-audit of the specified computer
		/// 
		/// </summary>
		public int RequestReaudit()
		{
			try
			{
				ServiceController agentController = new ServiceController(AuditWizardAgentServiceName, ComputerName);
				agentController.ExecuteCommand(AGENT_REQUEST_REAUDIT);
				return 0;
			}
			catch (Exception)
			{
				return -1;
			}
		}


		/// <summary>
		/// Update
		/// ======
		/// 
		/// Update the scanner configuration file on the specified computer
		/// </summary>
		/// <param name="computerName"></param>
		public void Update()
		{
			string sourcePath = Path.Combine(Application.StartupPath, AuditAgentStrings.AuditAgentFilesPath);
			List<string> listFiles = new List<string>();
			listFiles.Add(Path.Combine(sourcePath, AuditWizardAgentIni));

			// Copy the files to the remote Asset
			// The caller should catch any exceptions thrown as we do not want to continue after a failure
			CopyFilesToRemote(listFiles);
		}


		/// <summary>
		/// Check the status of the AuditAgent on the remote computer
		/// </summary>
		/// <param name="computerName"></param>
		/// <returns></returns>
		public ServiceStatus CheckStatus(string computerName)
		{
			return base.CheckStatus();
		}


		/// <summary>
		/// Over-ride the Remove function so that we can delete the agent files as part of the
		/// removal process
		/// </summary>
		/// <returns></returns>
		public override bool Remove()
		{
			// Call the base class implementation to actually remove the service
			if (base.Remove())
			{
				DeleteClientFiles();
				return true;
			}
			else
			{
				return false;
			}
		}


		/// <summary>
		/// Delete the files which were previously deployed to the remote computer
		/// </summary>
		/// <param name="computerName"></param>
		private void DeleteClientFiles()
		{
			OpenRemoteConnection();

            // check for 64-bit versions first
            string destinationPath = "\\\\" + ComputerName + "\\ADMIN$\\syswow64\\";

            if (!Directory.Exists(destinationPath))
                destinationPath = "\\\\" + ComputerName + "\\ADMIN$\\system32\\";

			// delete all the client files from the system32 folder on the remote machine
			try
			{
				File.Delete(destinationPath + AuditWizardAgentExe);
				File.Delete(destinationPath + AuditWizardAgentIni);

				try
				{
					File.Delete(destinationPath + AuditWizardAgentLogFile);
				}
				catch
				{
					//...ignore...just log file
				}
			}
			catch (Exception)
			{
				CloseRemoteConnection();
				//throw new Exception("Failed to remove client files from remote path: " + destinationPath + "\nReason:  " + e.Message, e);
			}
			CloseRemoteConnection();
		}




		/// <summary>
		/// Clear the contents of the audit agent log file
		/// </summary>
		/// <param name="computerName"></param>
		public bool ClearLogFile()
		{
            string destinationFile = String.Empty;

            // check for 64-bit versions first
            string destinationPath = "\\\\" + ComputerName + "\\ADMIN$\\syswow64\\";

            if (Directory.Exists(destinationPath))
                destinationFile = "\\\\" + ComputerName + "\\ADMIN$\\syswow64\\" + AuditAgentStrings.ClientLogFile;
            else
                destinationFile = "\\\\" + ComputerName + "\\ADMIN$\\system32\\" + AuditAgentStrings.ClientLogFile;

			if (File.Exists(destinationFile))
			{
				File.Delete(destinationFile);
				return true;
			}
			else
			{
				MessageBox.Show("The AuditWizard Agent log file for computer " + ComputerName + " could not be found at " + destinationFile + ".  The Audit Agent may not be running.");
				return false;
			}
		}


		/// <summary>
		/// View the Agent Log File
		/// </summary>
		/// <param name="computerName"></param>
		public void ViewLogFile()
		{
			base.ViewRemoteFile(AuditWizardAgentLogFile);
		}

		/// <summary>
		/// Determine the username and password to use for the agent if any
		/// We will return null for both if we are using the local system account
		/// </summary>
		protected void GetCredentials(ref string username, ref string password)
		{
			Configuration config = ConfigurationManager.OpenExeConfiguration(Path.Combine(Application.StartupPath, "AuditWizardv8.exe"));
			bool runAsSystem = Convert.ToBoolean(config.AppSettings.Settings["agent_runassystem"].Value);
			if (!runAsSystem)
			{
				username = config.AppSettings.Settings["agent_username"].Value;
				password = config.AppSettings.Settings["agent_password"].Value;
				if (password != "")
					password = AES.Decrypt(password);

				// If either the username or password is blank then allow the user to specify it
				if (username == "" || password == "")
				{
					FormAskInput2 form = new FormAskInput2("Please enter the credentials under which the AutoAgent Service will run"
														 , "Logon As"
														 , "Username"
														 , "Password"
														 , username
														 , password);
					form.SetPasswordField2(true);			// Set field 2 to be a password field
					if (form.ShowDialog() != DialogResult.OK)
						return;

					// OK - pick up the username and password specified
					username = form.Value1Entered();
					password = form.Value2Entered();
				}
			}
		}
	}
}
