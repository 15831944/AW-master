using System;
using System.Collections.Generic;
using System.Text;

namespace Layton.AuditWizard.DataAccess
{
	public class RemoteCredentials
	{
		private String _remoteHost;
		private String _domain;
		private String _username;
		private String _password;

		public String Password
		{
			get { return _password; }
			set { _password = value; }
		}

		public String Domain
		{
			get { return _domain; }
			set { _domain = value; }
		}

		public String Username
		{
			get { return _username; }
			set { ParseUserName(value);	}
		}

		public String RemoteHost
		{
			get { return _remoteHost; }
			set { _remoteHost = value; }
		}

		public bool IsLocalComputer ()
		{
			return (RemoteHost == "" || RemoteHost == "localhost" || RemoteHost == Environment.MachineName);
		}


		public RemoteCredentials(String remoteHost, String username, String password)
		{
			_remoteHost = remoteHost;
			ParseUserName(username);

			// Save the password
			_password = password;
		}

		private void ParseUserName(String username)
		{
			// Now the username noting that this may be split into a domain and user
			if (username.Contains(@"\"))
			{
				String[] userParts = username.Split('\\');
				_username = userParts[1];
				_domain = userParts[0];
			}
			else
			{
				_username = username;
				String currentUser = System.Security.Principal.WindowsIdentity.GetCurrent().Name;
				String[] userParts = currentUser.Split('\\');
				_domain = userParts[0];
			}


		}
	}
}
