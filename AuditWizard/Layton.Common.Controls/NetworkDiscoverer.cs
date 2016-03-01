using System;
using System.DirectoryServices;
using System.Net;
using System.Net.Sockets;
using ServerEnumerator;

namespace Layton.Common.Controls
{
	public class DiscoveredItem
	{
		public DiscoveredItem ()
		{}

		public DiscoveredItem (string strName ,string strParent ,string strIPAddress)
		{ _Name = strName; _Parent = (strParent == "") ? "<none>" : strParent; _IPAddress = strIPAddress; }

		public string Name
		{
			get { return _Name; }
			set { _Name = value; }
		}

		public string Parent
		{
			get { return _Parent; }
			set { _Parent = value; }
		}

		public string IPAddress
		{
			get { return _IPAddress; }
			set { _IPAddress = value; }
		}

		public override string ToString()
		{
			string strText = String.Format("Name: {0}   Location: {1}   IP Address: {2}", _Name, _Parent, _IPAddress);
			return strText;
		}

		private string _Name;
        private string _Parent;
        private string _IPAddress;
	}


	/// <summary>
	/// Summary description for NetworkDiscoverer.
	/// </summary>
	abstract public class NetworkDiscoverer
	{
		// This delegate can be used to return progress information to the caller
		public delegate bool TrackProgress(ProgressDetails progressDetails);
		public event TrackProgress ShowProgress = null;

		// Abstract class the actually performs the discovery process
		public abstract int Discover ();

		// Gets the IP address of an entry
		public string IpAddress	(string strName)
		{
			string strIpAddress = "";
			try 
			{
                IPHostEntry hostInfo = Dns.GetHostByName(strName);
				if (hostInfo.AddressList.Length != 0)
					strIpAddress = hostInfo.AddressList[0].ToString();
			}
			catch(SocketException)
			{
			}
			return strIpAddress;
		}



		// 
		//    Add
		//    ===
		//
		//    Adds a 'new' entry to the list of discovered assets.
		//
		public bool Add	(DiscoveredItem newitem)
		{
			if (newitem.IPAddress.Length == 0)
				newitem.IPAddress = IpAddress(newitem.Name);
		
			// ..and log the progress
			ProgressDetails progressDetails = new ProgressDetails(ProgressDetails.eState.success, newitem.ToString());
			progressDetails.Tag = newitem;
			bool bCancelled = !TryShowProgress(progressDetails);
			return bCancelled;
		}


		protected bool TryShowProgress(ProgressDetails progressDetails)
		{
			if (this.ShowProgress != null)
				return this.ShowProgress(progressDetails);
			return true;
		}
	}


	//
	// NetworkDiscovererActiveDirectory
	// ================================
	//
	public class NetworkDiscovererActiveDirectory : NetworkDiscoverer
	{
		// Constructor - takes the LDAP Server name asan input

		//
		//    Discover
		//    ========
		//
		//    Base class over-ride to perform the actual work
		//
		public override int Discover	()
		{
			try
			{
				// Identify the 'default' AD/LDAP Server
				DirectoryEntry defaultServer = new DirectoryEntry("LDAP://rootDSE");
				string strLdapServer = (string)defaultServer.Properties["defaultNamingContext"].Value;
				DirectoryEntry mySearchRoot = new DirectoryEntry("LDAP://" + strLdapServer);

				// Create a 'DirectoryEntry' object to search.		
				DirectorySearcher myDirectorySearcher = new DirectorySearcher(mySearchRoot);
				myDirectorySearcher.Filter = ("(objectClass=computer)");

				// Iterate through (any) results
				foreach(SearchResult resEnt in myDirectorySearcher.FindAll())
				{
					// Get the 'DirectoryEntry' that corresponds to 'mySearchResult'.
					DirectoryEntry myDirectoryEntry = resEnt.GetDirectoryEntry();
					string strComputer = myDirectoryEntry.Name.ToString();

					// strip off the 'CN=' if it exists
					if (strComputer.StartsWith("CN="))
						strComputer = strComputer.Remove(0 ,3);

					// What OU is this computer in though?
					string strParent = myDirectoryEntry.Path;
					strParent = GetOU(strParent);

					// ...and add this to our list
					string thisIp = IpAddress(strComputer);
					DiscoveredItem item = new DiscoveredItem(strComputer ,strParent ,thisIp);
					bool bCancelled = Add(item);
					if (bCancelled)
						break;
				}
			}
			catch(Exception)
			{				
				TryShowProgress(new ProgressDetails(ProgressDetails.eState.failure, "Failed to locate/connect to the Active Directory Server"));
				return -1;						// Server probably doesn't exist
			}

			return 0;
		}


		//
		//    GetOU
		//    =====
		//
		//    Iterate through the string supplied looking for OU's.  Note that they arrive
		//    in reverse order so we need to sort that
		//
		protected string GetOU (string strParent)
		{
			string strLocation = "";
			int nStart = strParent.Length-1;
			int nEnd = 0;
			int nCount = nStart - nEnd - 1;
			int nPos = strParent.LastIndexOf("OU=" ,nStart ,nCount);
			while (nPos != -1)
			{
				int nTokenStart = nPos + 3;
				int nTokenEnd = strParent.IndexOf("," ,nTokenStart);

				// find the delimiting ',' or end of the string
				string strToken;
				if (nTokenEnd == -1)
				{
					strToken = strParent.Substring(nTokenStart ,nTokenEnd - nTokenStart + 1);
				}
				else
				{
					strToken = strParent.Substring(nTokenStart ,nTokenEnd - nTokenStart);
				}
				
				if (strLocation.Length != 0)
					strLocation += "\\";
				strLocation += strToken;

				// ...and see if there any more OU's in the search string
				nPos = strParent.LastIndexOf("OU=" ,nPos ,nPos - 1);
			}

			return strLocation;
		}
	}

	//
	// NetworkDiscovererNetBios
	// ========================
	//
	public class NetworkDiscovererNetBios : NetworkDiscoverer
	{

		// Constructor

		//
		//    Discover
		//    ========
		//
		//    Base class over-ride to perform the actual work
		//
		public override int Discover ()
		{
			try
			{
				ServerEnum servers = new ServerEnum(ResourceScope.RESOURCE_GLOBALNET
					,ResourceType.RESOURCETYPE_DISK
					,ResourceUsage.RESOURCEUSAGE_ALL
					,ResourceDisplayType.RESOURCEDISPLAYTYPE_SERVER);

				// The data is returned in an array list with the name encapsulated in the form
				// domain\\PC which we have to split and add to our own list
				foreach	(Server server in servers)
				{
					string strComputer = server.Name.ToString();
					string strDomain = server.Domain.ToString();

					// Skip any where we have no domain
					if (strDomain == "")
						continue;
				
					// ...and add this to our list
					string thisIp = IpAddress(strComputer);
					DiscoveredItem item = new DiscoveredItem(strComputer ,strDomain ,thisIp);
					bool bCancelled = Add(item);
					if (bCancelled)
						break;
				}
			}
			catch(Exception ex)
			{
				return -1;						// Server probably doesn't exist
			}

			return 0;
		}
	}
}
