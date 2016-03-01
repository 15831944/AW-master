using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Text;
using System.Windows.Forms;
//
using Layton.AuditWizard.DataAccess;
using Layton.Common.Controls;

namespace Layton.AuditWizard.Common
{
	public class AuditWizardServiceController : LaytonServiceController
	{

		/// <summary>
		/// Constant strings needeed to identify the agent
		/// </summary>
		public const string AuditWizardServiceName			= "AuditWizardService";
		public const string AuditWizardServiceDisplayName	= "AuditWizardService";
		public const string AuditWizardServiceExecutable	= "AuditWizardService.exe";
        //public const string AuditWizardServiceLog = "AuditWizardService.txt";
        public const string AuditWizardServiceLog = "aw_svc.log";

		/// <summary>
		/// The main constructor simply takes the computer name and fills in the other parameters
		/// </summary>
		/// <param name="computerName"></param>
		public AuditWizardServiceController()
		{
			// Ensure that we configure the base class as we have not called it's extended constructor
			ServiceName = AuditWizardServiceName;
			ServiceDisplayName = AuditWizardServiceDisplayName;
			ServiceExecutable = Path.Combine(Application.StartupPath, AuditWizardServiceExecutable);
			ComputerName = System.Environment.MachineName;

			// Recover any credentials that we will be using
//			ResetCredentials();
		}

		/// <summary>
		/// Populate the login credentials used by the service
		/// </summary>
		public void ResetCredentials()
		{
			string username = null;
			string password = null;
			GetCredentials(ref username, ref password);

			// Set any credentials that we will be using later
			SetCredentials(username, password);
		}

		/// <summary>
		/// Determine the username and password to use for the uploader service if any
		/// We will return null for both if we are using the local system account
		/// </summary>
		protected void GetCredentials(ref string username, ref string password)
		{
			Configuration config = ConfigurationManager.OpenExeConfiguration(Path.Combine(Application.StartupPath, "AuditWizardv8.exe"));
			bool runAsSystem = Convert.ToBoolean(config.AppSettings.Settings["auditwizardservice_runassystem"].Value);
			if (!runAsSystem)
			{
				username = config.AppSettings.Settings["auditwizardservice_username"].Value;
				password = config.AppSettings.Settings["auditwizardservice_password"].Value;
				if (password != "")
					password = AES.Decrypt(password);

				// If either the username or password is blank then allow the user to specify it
				//if (username == "" || password == "")
				//{
				//    FormAskInput2 form = new FormAskInput2("Please enter the credentials under which the AuditWizard Service will run"
				//                                         , "Logon As"
				//                                         , "Username"
				//                                         , "Password"
				//                                         , username
				//                                         , password);
				//    form.SetPasswordField2(true);			// Set field 2 to be a password field
				//    if (form.ShowDialog() != DialogResult.OK)
				//        return;

				//    // OK - pick up the username and password specified
				//    username = form.Value1Entered();
				//    password = form.Value2Entered();
				//}
			}
		}

	}
}
