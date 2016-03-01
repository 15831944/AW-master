using System;
using System.Collections.Generic;
using System.DirectoryServices;
using Layton.AuditWizard.DataAccess;
using PickerSample;

namespace Layton.NetworkDiscovery
{
    public class LdapNetworkDiscovery : NetworkDiscovery
    {
        private ADPicker computerPicker;
        private bool isComplete = false;
        private bool isStarted = false;
        private List<string[]> computers = new List<string[]>();
        private List<string> domains = new List<string>();
        protected static readonly log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        
        public LdapNetworkDiscovery()
        {
            computerPicker = new PickerSample.ADPicker();
            computerPicker.DownlevelFilter = PickerSample.adPickerDownlevelFilter.DSOP_DOWNLEVEL_FILTER_COMPUTERS;
            computerPicker.Options = PickerSample.adPickerOptions.DSOP_FLAG_MULTISELECT;
            computerPicker.ReturnFormat = PickerSample.adPickerReturnFormat.ADsPath;
            computerPicker.ScopeFormat = PickerSample.adPickerScopeFormat.DSOP_SCOPE_FLAG_WANT_PROVIDER_LDAP;
            computerPicker.ScopeType = ((PickerSample.adPickerScopeType)((((((((PickerSample.adPickerScopeType.DSOP_SCOPE_TYPE_UPLEVEL_JOINED_DOMAIN | PickerSample.adPickerScopeType.DSOP_SCOPE_TYPE_DOWNLEVEL_JOINED_DOMAIN)
                        | PickerSample.adPickerScopeType.DSOP_SCOPE_TYPE_ENTERPRISE_DOMAIN)
                        | PickerSample.adPickerScopeType.DSOP_SCOPE_TYPE_GLOBAL_CATALOG)
                        | PickerSample.adPickerScopeType.DSOP_SCOPE_TYPE_EXTERNAL_UPLEVEL_DOMAIN)
                        | PickerSample.adPickerScopeType.DSOP_SCOPE_TYPE_EXTERNAL_DOWNLEVEL_DOMAIN)
                        | PickerSample.adPickerScopeType.DSOP_SCOPE_TYPE_USER_ENTERED_UPLEVEL_SCOPE)
                        | PickerSample.adPickerScopeType.DSOP_SCOPE_TYPE_USER_ENTERED_DOWNLEVEL_SCOPE)));
            computerPicker.UplevelFilter = PickerSample.adPickerUplevelFilter.DSOP_FILTER_COMPUTERS;
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

        public override void Start()
        {
            try
            {
                isStarted = true;
                List<string> returnValues = new List<string>();

                SettingsDAO lSettingsDAO = new SettingsDAO();

                // are we using a custom string or presenting the AD dialog?
                if (lSettingsDAO.GetSettingAsBoolean("UseADCustomString", false))
                {
                    string lCustomStrings = lSettingsDAO.GetSettingAsString("ADCustomString", String.Empty);

                    if (lCustomStrings != String.Empty)
                    {
                        foreach (string lCustomString in lCustomStrings.Split('|'))
                        {
                            if (lCustomString != String.Empty)
                            {
                                DirectoryEntry ent = new DirectoryEntry(lCustomString);
                                DirectorySearcher searcher = new DirectorySearcher(ent);
                                searcher.Filter = "(objectClass=Computer)";

                                foreach (SearchResult result in searcher.FindAll())
                                {
                                    if (!returnValues.Contains(result.Path))
                                        returnValues.Add(result.Path);
                                }
                            }
                        }
                    }
                }
                else
                {
                    // displays the "Select Computers" dialog to the user
                    try
                    {
                        computerPicker.ShowDialog();

                        foreach (string ldapString in computerPicker.ReturnValues)
                        {
                            returnValues.Add(ldapString);
                        }

                    }
                    catch
                    {
                        // the dialog throws an exception if no directories are found...just ignore it
                    }
                }

                //if (computerPicker.ReturnValues != null)
                if (returnValues.Count > 0)
                {
                    string computerName = "";
                    string domainName = "";
                    bool domainFound = false;

                    foreach (string ldapString in returnValues)
                    {
                        // remove the LDAP address from the returning string
                        string ldapFilters = ldapString.Substring(ldapString.IndexOf("CN="));

                        // now split the string into the sub-strings
                        string[] filterStrings = ldapFilters.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                        foreach (string filter in filterStrings)
                        {
                            if (filter.StartsWith("CN=", StringComparison.OrdinalIgnoreCase))
                            {
                                string pcName = filter.Remove(0, 3);
                                if (pcName.Length > 0 && !pcName.Equals("Computers", StringComparison.OrdinalIgnoreCase))
                                {
                                    computerName = pcName;
                                }
                            }
                            else if (filter.StartsWith("DC=", StringComparison.OrdinalIgnoreCase))
                            {
                                string domain = filter.Remove(0, 3);
                                if (domain.Length > 0 && !domain.Equals("local", StringComparison.OrdinalIgnoreCase) && !domainFound)
                                {
                                    domainName = domain.ToUpper();
                                    domainFound = true;
                                }
                            }
                        }
                        // first check if this is a new domain
                        if (domainName != "" && !domains.Contains(domainName))
                        {
                            domains.Add(domainName);
                        }
                        // add the computer to the list along with its domain name
                        if (computerName != "")
                        {
                            computers.Add(new string[] { computerName, domainName });
                        }

                        domainFound = false;
                    }
                }

                isComplete = true;
                FireNetworkDiscoveryUpdate(new DiscoveryUpdateEventArgs(computers.Count.ToString(), "Computer", computers.Count, 0));
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message);
                Utility.DisplayApplicationErrorMessage(String.Format(
                    "AuditWizard encountered an error during Network Discovery. The error message is:{0}{1}{2}", 
                    Environment.NewLine, Environment.NewLine, ex.Message));

                isComplete = true;
            }
        }
    }
}
