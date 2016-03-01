using System;
using System.Windows.Forms;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Security;
using System.Text;
using Microsoft.Win32;

namespace Layton.Common.Controls
{
	public class RegistryAccess
	{
		private string _lastErrorText;

		public string LastErrorText
		{
			get { return _lastErrorText; }
			set { _lastErrorText = value; }
		}

		/// <summary>
		/// This function is called to return the specified registry root key, optionally connecting
		/// to a remote computer registry
		/// </summary>
		/// <param name="rootKey"></param>
		/// <param name="remoteHost">Name of the Computer to connect to, NULL if the local Computer</param>
		/// <returns>The registry key if successful, null otherwise</returns>
		public int OpenRegistry(RegistryKey rootKey, String remoteHost, out RegistryKey openKey)
		{
			// Assume that the open will fail!
			openKey = null;

			// If we have no remote credentials or the credentials relate to the local computer then 
			// simply return the specified root key as we need no special processing for local registries
			if (remoteHost == "" || remoteHost == "localhost" || remoteHost == Environment.MachineName)
			{
				openKey = rootKey;
				return 0;
			}

			// We allow access to the remote hives but must map the supplied rootKey with the hive
			// as we canot pass the key to the remote registry function
			RegistryHive remoteHive;
			if (rootKey == Registry.LocalMachine)
				remoteHive = RegistryHive.LocalMachine;
			else if (rootKey == Registry.CurrentUser)
				remoteHive = RegistryHive.CurrentUser;
			else if (rootKey == Registry.ClassesRoot)
				remoteHive = RegistryHive.ClassesRoot;
			else if (rootKey == Registry.CurrentConfig)
				remoteHive = RegistryHive.CurrentConfig;
			else
				return -1;

			// OK try and open the remote registry key
			try
			{
				openKey = RegistryKey.OpenRemoteBaseKey(remoteHive, remoteHost);
			}
			catch (IOException)
			{
				LastErrorText = "Failed to find the Computer [" + remoteHost + "]";
				return -1;
			}
			catch (SecurityException)
			{
				// Security Exception indicates insufficient privilege
				LastErrorText = "Failed to connect to the system registry on Computer [" + remoteHost + "] - A Security exception was thrown";
				return -2;
			}
			catch (Exception ex)
			{
				// anything else - 
				LastErrorText = "Failed to connect to the system registry on Computer [" + remoteHost + "] - The error was [" + ex.Message + "]";
				return -3;
			}

			return 0;
		}
	}
}
