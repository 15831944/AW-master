using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.DirectoryServices;
using System.DirectoryServices.ActiveDirectory;
using System.Text;

namespace Layton.NetworkDiscovery
{
	public class ActiveDirectoryNetworkDiscovery : NetworkDiscovery
	{
        private bool isComplete = false;
        private bool isStarted = false;
        private List<string[]> computers = new List<string[]>();
		private List<string> domains = new List<string>();
		private List<string> _domainsToDiscover = new List<string>();
		private NameValueCollection _adSettings = null;

		/// <summary>This field contains (any) custom LDAP string specified by the user</summary>
		private string _customLdapString = "";

		public ActiveDirectoryNetworkDiscovery()
		{
		}

		public ActiveDirectoryNetworkDiscovery(NameValueCollection adSettings)
		{
			_adSettings = adSettings;

			// Check for a domain list
			foreach (string key in adSettings.AllKeys)
			{
				if (key == "DomainList")
					_domainsToDiscover = Utility.ListFromString(adSettings[key] ,';');

				else if (key == "CustomLdapString")
					_customLdapString = adSettings[key];
			}
		}

		/// <summary>
		/// Allow a custom LDAP string to be passed in here rather than the function trying to determine
		/// the current or 'default' LDAP string
		/// </summary>
		public string CustomLdapString
		{
			set { _customLdapString = value; }
		}

        public override bool HasDiscoverStarted
        {
            get { return isStarted; }
        }

        public override bool IsDiscoverComplete
        {
            get { return isComplete; }
        }

        public override List<string> DomainList
        {
            get { return domains; }
        }

        public override List<string[]> ComputerList
        {
            get { return computers; }
        }

        public override bool CanRunInOwnThread
        {
            get { return false; }
        }


		/// <summary>
		/// Initiate the Active Directory Discovery process
		/// </summary>
		public override void Start()
		{
			isStarted = true;

			try
			{
				// Construct a Directory Searcher to extract all current Domain Directory Partitions
				// Connect to RootDSE to extra directory configuration information
				DirectoryEntry RootDSE = new DirectoryEntry("LDAP://rootDSE");
			
				// Retrieve the Configuration Naming Context from RootDSE
				string LdapConfigNC = (string)RootDSE.Properties["configurationNamingContext"].Value;
			
				// Connect to the Configuration Naming Context
				DirectoryEntry configSearchRoot = new DirectoryEntry("LDAP://" + LdapConfigNC);

				// Search for all partitions where the NetBIOSName is set.
				DirectorySearcher configSearch = new DirectorySearcher(configSearchRoot);
				configSearch.Filter = ("(NETBIOSName=*)");

				// Configure search to return dnsroot and ncname attributes
				configSearch.PropertiesToLoad.Add("dnsroot");
				configSearch.PropertiesToLoad.Add("ncname");
				SearchResultCollection forestPartitionList = configSearch.FindAll();

				// Loop through each returned domain in the result collection
				foreach (SearchResult domainPartition in forestPartitionList)
				{
					// domainName like "domain.com". ncName like "DC=domain,DC=com"
					string domainName = domainPartition.Properties["dnsroot"][0].ToString();
					string ncName = domainPartition.Properties["ncname"][0].ToString();

					// Have we specified specific domains to discover and if so is this one of them?
					if ((_domainsToDiscover.Count != 0) && (!_domainsToDiscover.Contains(domainName)))
						continue;

					// Create new LdapSearchRoot using domain and naming context returned above
					string DomainLdapServer = "LDAP://" + domainName + "/" + ncName;

					// Set the root to be this server. Removed "LDAP://". Duplicated from above.
					DirectoryEntry domainSearchRoot = new DirectoryEntry(DomainLdapServer);

					// Set a Directory searcher to return all Computers in the AD
					DirectorySearcher domainSearch = new DirectorySearcher(domainSearchRoot);
					domainSearch.Filter = "(objectClass=computer)";

					// Add "name" to the list of attributes returned by the search
					domainSearch.PropertiesToLoad.Add("name");

					// Invoke the find
					SearchResultCollection domainComputerList = domainSearch.FindAll();

					// Enumerate over each returned domain.
					foreach (SearchResult domainComputer in domainComputerList)
					{
						string computerName = domainComputer.Properties["name"][0].ToString();

						// ...and add these to our lists if not already present
						computers.Add(new string[] { computerName, domainName });
					}
				}
			}

			catch (Exception)
			{
				// Ignore any exceptions here as they most likely mean that we failed to connect to the LDAP
				// server as there is nothing that we can do about that.
			}

			isComplete = true;
			FireNetworkDiscoveryUpdate(new DiscoveryUpdateEventArgs("", "", computers.Count));
		}



		/// <summary>
		/// Enumerate the domains in this forest
		/// </summary>
		/// <returns></returns>
		public List<string>	EnumerateDomains()
		{
			List<string> domainList = new List<string>();
			Forest currentForest = Forest.GetCurrentForest();
			DomainCollection myDomains = currentForest.Domains;

			foreach (Domain objDomain in myDomains)
			{
				domainList.Add(objDomain.Name);
			}
		    return domainList;
		}
	}
}
