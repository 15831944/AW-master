using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using System.IO;
//
using Layton.Cab.Interface;
using Layton.AuditWizard.Administration;
using Layton.AuditWizard.Common;
using Layton.AuditWizard.DataAccess;
using Layton.Common.Controls;
//
using Microsoft.Practices.CompositeUI.SmartParts;
using Microsoft.Practices.CompositeUI.EventBroker;
using Microsoft.Practices.ObjectBuilder;
using Microsoft.Practices.CompositeUI;
//
using Infragistics.Win.UltraWinTree;


namespace Layton.AuditWizard.Network
{
	public class NetworkWorkItemController : Layton.Cab.Interface.LaytonWorkItemController
	{
        UltraTree tree;

        public UltraTree Tree
        {
            get { return tree; }
            set { tree = value; }
        }
        #region CreateProcess Data
		public struct PROCESS_INFORMATION
		{
			public IntPtr hProcess;
			public IntPtr hThread;
			public uint dwProcessId;
			public uint dwThreadId;
		}

		public struct STARTUPINFO
		{
			public uint cb;
			public string lpReserved;
			public string lpDesktop;
			public string lpTitle;
			public uint dwX;
			public uint dwY;
			public uint dwXSize;
			public uint dwYSize;
			public uint dwXCountChars;
			public uint dwYCountChars;
			public uint dwFillAttribute;
			public uint dwFlags;
			public short wShowWindow;
			public short cbReserved2;
			public IntPtr lpReserved2;
			public IntPtr hStdInput;
			public IntPtr hStdOutput;
			public IntPtr hStdError;
		}

		public struct SECURITY_ATTRIBUTES
		{
			public int length;
			public IntPtr lpSecurityDescriptor;
			public bool bInheritHandle;
		}

		[DllImport("kernel32.dll")]
		static extern bool CreateProcess(string lpApplicationName, string lpCommandLine, IntPtr lpProcessAttributes, IntPtr lpThreadAttributes,
                        bool bInheritHandles, uint dwCreationFlags, IntPtr lpEnvironment, 
                        string lpCurrentDirectory, ref STARTUPINFO lpStartupInfo,out PROCESS_INFORMATION lpProcessInformation);

        #endregion CreateProcess Data	
	
        #region Data Definitions

        protected static readonly log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

		private int TrialCount = 10;
		private NetworkWorkItem workItem;
		List<Asset> _listAssets = new List<Asset>();

		/// <summary>This is the date/time that we last issued an operations request (deployment of agent etc)</summary>
		private DateTime _operationStartDate = DateTime.Today;
		
		/// <summary>
		/// This is the current publisher filter which will determine which publishers we display.  It can
		/// be updated by the PublisherFilterChanged event being fired.
		/// Note that publisher filter and the two 'show' flags below use the same event
		/// </summary>
		private String _publisherFilter = "";

		/// <summary>
		/// Flag to indicate whether or not we should be showing applications and operating systems
		/// which have been flagged as 'NotIgnore' or 'non-NotIgnore'. They can be updated by the 
		/// PublisherFilterChanged event being fired.
		/// </summary>
		private bool _showIncludedApplications = true;
		private bool _showIgnoredApplications = true;
		
		/// <summary>
		/// Flag to show if we should display computers based on domains / workgroup
		/// </summary>
		private bool _showByDomain = true;

		// We create a list of user data field categories here as this infrequently changes and can easily be refreshed
		UserDataCategoryList _userDataCategories = new UserDataCategoryList(UserDataCategory.SCOPE.Asset);

		// Asset Types also change infrequently so we can populate them during the refresh
		AssetTypeList _listAssetTypes = new AssetTypeList();

        #endregion 

        #region Data Accessors
		/// <summary>
		/// Return the list of user data categories
		/// </summary>
		public UserDataCategoryList UserDataCategories
		{
			get { return _userDataCategories; }
		}

		/// <summary>
		/// Return the list of asset types
		/// </summary>
		public AssetTypeList AssetTypes
		{
			get { return _listAssetTypes; }
		}
		

		/// <summary>
		/// Recover the filter that has been set for Publishers
		/// </summary>
		public String PublisherFilter
		{
			get { return _publisherFilter; }
			set { _publisherFilter = value; }
		}


		/// <summary>
		/// Show or hide applications that have been flagged as 'hidden' in the database
		/// </summary>
		public bool ShowIncludedApplications
		{
			get { return _showIncludedApplications; }
			set
			{
				_showIncludedApplications = value;

				// Fire an event to let everyone know about the change
				FirePublisherFilterChangedEvent();
			}
		}

		/// <summary>
		/// Show or hide applications that have been flagged as 'hidden' in the database
		/// </summary>
		public bool ShowIgnoredApplications
		{
			get { return _showIgnoredApplications; }
			set
			{
				_showIgnoredApplications = value;

				// Fire an event to let everyone know about the change
				FirePublisherFilterChangedEvent();
			}
		}


		/// <summary>
		/// Change the 'View By' style
		/// </summary>
		public bool DomainViewStyle
		{
			get { return _showByDomain; }
			set
			{
				_showByDomain = value;
				AuditWizardConfiguration configuration = new AuditWizardConfiguration();
				configuration.ShowByDomain = value;
				configuration.SaveConfiguration();

				// Refresh both the explorer and tab view to reflect this change
				RefreshView();
			}
		}

		/// <summary>
		/// Event declaration for when the View Style is changed.
		/// </summary>
		[EventPublication(CommonEventTopics.ViewStyleChanged, PublicationScope.Global)]
		public event EventHandler<ViewStyleEventArgs> ViewStyleChanged;

		/// <summary>
		/// Event declaration for when the Publisher Filter is changed.
		/// </summary>
		[EventPublication(CommonEventTopics.PublisherFilterChanged, PublicationScope.Global)]
		public event EventHandler<PublisherFilterEventArgs> PublisherFilterChanged;

		/// <summary>
		/// Invoke the 'Filter Publishers' form to change which publishers are filtered
		/// </summary>
		public void FilterPublishers()
		{
			FormFilterPublishers form = new FormFilterPublishers();
			if (form.ShowDialog() == System.Windows.Forms.DialogResult.OK)
			{
				// Update our copy of the filter
				_publisherFilter = form.PublisherFilter;

				// OK - Fire an event to indicate to any interested parties that the list of 
				// publisher filters has changed
				FirePublisherFilterChangedEvent();
			}
		}


		/// <summary>
		/// Refresh the current ACTIVE views
		/// </summary>
		protected void RefreshView()
		{
			// Refresh the User Data Field Categories and asset types
			_userDataCategories.Populate();
			_listAssetTypes.Populate();

			// Refresh the view itself
			ILaytonView activeExplorerView = (ILaytonView)WorkItem.RootWorkItem.Workspaces[WorkspaceNames.ExplorerWorkspace].ActiveSmartPart;
			ILaytonView activeTabView = (ILaytonView)workItem.RootWorkItem.Workspaces[WorkspaceNames.TabWorkspace].ActiveSmartPart;

			if (activeExplorerView == WorkItem.ExplorerView)
			{
				activeExplorerView.RefreshView();
				activeTabView.RefreshView();
			}
		}

		/// <summary>
		/// Get the current license count
		/// </summary>
		public int LicenseCount
		{
			get
			{
				Layton.Cab.Interface.LaytonProductKey key = WorkItem.RootWorkItem.Items[Layton.Cab.Interface.MiscStrings.ProductKey] as Layton.Cab.Interface.LaytonProductKey;
				return (key.IsTrial) ? TrialCount : key.AssetCount;
			}
		}

        #endregion

        #region Constructor

		public NetworkWorkItemController(WorkItem workItem) : base(workItem)
		{
            tree = new UltraTree();
			this.workItem = workItem as NetworkWorkItem;
			
			// We need to pull the publisher filter list from the database
            SettingsDAO lwDataAccess = new SettingsDAO();
			_publisherFilter = lwDataAccess.GetPublisherFilter();

			// Pull the last used view style from the AuditWizard configuration
			AuditWizardConfiguration configuration = new AuditWizardConfiguration();
			DomainViewStyle = configuration.ShowByDomain;
		}
        #endregion

		/// <summary>
		/// Each time we activate the network tab we automatically update it
		/// </summary>
		public override void ActivateWorkItem()
		{
			base.ActivateWorkItem();
			RefreshView();
		}

#region Event Handlers

		/// <summary>
		/// This event handler fires to notify interested parties that the View Style has been changed
		/// </summary>
		protected void FireViewStyleChangedEvent()
		{
			if (ViewStyleChanged != null)
				ViewStyleChanged(this, new ViewStyleEventArgs(_showByDomain));
		}



		/// <summary>
		/// This event fiores to inform all interested parties that a change has been made to the publisher filter
		/// </summary>
		protected void FirePublisherFilterChangedEvent()
		{
			// If the settings were changed from this tab then we should fire an event to let all other
			// tabs know of the change - if however we are not the active tab then we have been informed
			// of the change ourselves and do not need to pass it on.
			if (PublisherFilterChanged != null)
			{
				// relay the filter change if we are the active explorer view and hence we have changed
				// the filter ourselves
				ILaytonView activeExplorerView = (ILaytonView)WorkItem.RootWorkItem.Workspaces[WorkspaceNames.ExplorerWorkspace].ActiveSmartPart;
				if (activeExplorerView == WorkItem.ExplorerView)
					PublisherFilterChanged(this, new PublisherFilterEventArgs(_publisherFilter, _showIncludedApplications, _showIgnoredApplications));
			}
		}



		/// <summary>
		/// SetTabView
		/// ==========
		/// 
		/// This function is called when we have changed the item selected in the main explorer view 
		/// It ensures that the correct view (and item) is selected in the tab view.
		/// </summary>
		/// <param name="displayedNode"></param>
		/// <param name="itemType">Underlying type of the object that is being displayed</param>
		/// <param name="subObject">Used when displaying a sub-item such as user defined data category</param>
		public void SetTabView(UltraTreeNode displayedNode, TreeSelectionEventArgs.ITEMTYPE itemType ,object subObject)
		{
			try
			{
				// If we have selected a group then ensure that the GroupTabView is being displayed and request the 
				// group tab view to display the expansion of the selected group
				if (displayedNode.Tag is AssetGroup)
				{
					AssetGroup selectedGroup = displayedNode.Tag as AssetGroup;
					ILaytonView tabView = WorkItem.Items[Layton.Cab.Interface.ViewNames.MainTabView] as ILaytonView;
					if (tabView != null)
					{
						base.SetTabView(tabView);
						((GroupTabView)tabView).Presenter.Show(selectedGroup);
					}
				}

				else if (displayedNode.Tag is Asset)
				{
					// Have we selected the Asset ITSELF?
					if (itemType == TreeSelectionEventArgs.ITEMTYPE.asset)
					{
                        //ILaytonView tabView = WorkItem.Items[Properties.Settings.Default.ComputerTabView] as ILaytonView;
                        ILaytonView tabView = WorkItem.Items[Properties.Settings.Default.SummaryTabView] as ILaytonView;
						if (tabView != null)
						{
							base.SetTabView(tabView);
                            ((SummaryTabView)tabView).Presenter.Tree = tree;
                            
                            
                            ((SummaryTabView)tabView).Presenter.Show(displayedNode);
							//((ComputerTabView)tabView).Presenter.Show(displayedNode);
						}
					}

					// Have we selected the Asset Summary?
					else if (itemType == TreeSelectionEventArgs.ITEMTYPE.asset_summary)
					{
						ILaytonView tabView = WorkItem.Items[Properties.Settings.Default.SummaryTabView] as ILaytonView;
						if (tabView != null)
						{
							base.SetTabView(tabView);
							((SummaryTabView)tabView).Presenter.Show(displayedNode);
						}
					}

					// Have we selected the Asset Applications?
					else if (itemType == TreeSelectionEventArgs.ITEMTYPE.asset_applications)
					{
						ILaytonView tabView = WorkItem.Items[Properties.Settings.Default.ApplicationInstancesTabView] as ILaytonView;
						if (tabView != null)
						{
							base.SetTabView(tabView);
							((ApplicationInstancesTabView)tabView).Presenter.Show(displayedNode ,itemType);
						}
					}

					// Have we selected audited asset data (hardware or system information)
					else if ((itemType == TreeSelectionEventArgs.ITEMTYPE.asset_auditdata_category)
					||		 (itemType == TreeSelectionEventArgs.ITEMTYPE.asset_auditdata)
					||		 (itemType == TreeSelectionEventArgs.ITEMTYPE.asset_filesystem))
					{
						ILaytonView tabView = WorkItem.Items[Properties.Settings.Default.AuditedDataTabView] as ILaytonView;
						if (tabView != null)
						{
							base.SetTabView(tabView);
							((AuditedDataTabView)tabView).Presenter.Show(displayedNode ,itemType);
						}
					}

					// Have we selected asset history data 
					else if (itemType == TreeSelectionEventArgs.ITEMTYPE.asset_history)
					{
						ILaytonView tabView = WorkItem.Items[Properties.Settings.Default.HistoryTabView] as ILaytonView;
						if (tabView != null)
						{
							base.SetTabView(tabView);
							((HistoryTabView)tabView).Presenter.Show(displayedNode);
						}
					}					
				}

				// Displaying UserData so display the UserDefinedDataTabView
				else if (displayedNode.Tag is UserDataCategory)
				{
					UserDataCategory category = displayedNode.Tag as UserDataCategory;

					ILaytonView tabView = WorkItem.Items[Properties.Settings.Default.UserDefinedDataTabView] as ILaytonView;
					if (tabView != null)
					{
						base.SetTabView(tabView);
						((UserDefinedDataTabView)tabView).Presenter.Show(displayedNode, category);
					}
				}

				// When displaying FileSystem information we use the AuditedDataTabView				
				else if (displayedNode.Tag is FileSystemFolder)
				{
					ILaytonView tabView = WorkItem.Items[Properties.Settings.Default.AuditedDataTabView] as ILaytonView;
					if (tabView != null)
					{
						base.SetTabView(tabView);
						((AuditedDataTabView)tabView).Presenter.Show(displayedNode ,itemType);
					}
				}
				
				
				// Handle the 'ALL ASSETS' branch
				else if (displayedNode.Tag is AllAssets)
				{
					// Have we selected the Applications branch beneath 'All Assets'?
					if ((itemType == TreeSelectionEventArgs.ITEMTYPE.asset_applications)
					||	(itemType == TreeSelectionEventArgs.ITEMTYPE.asset_publisher)
					||  (itemType == TreeSelectionEventArgs.ITEMTYPE.asset_application)
					||  (itemType == TreeSelectionEventArgs.ITEMTYPE.asset_os))
					{
						ILaytonView tabView = WorkItem.Items[Properties.Settings.Default.ApplicationInstancesTabView] as ILaytonView;
						if (tabView != null)
						{
							base.SetTabView(tabView);
							((ApplicationInstancesTabView)tabView).Presenter.Show(displayedNode ,itemType);
						}
					}

					// Have we selected audited asset data (hardware or system information)
					else if ((itemType == TreeSelectionEventArgs.ITEMTYPE.asset_auditdata_category)
					||		 (itemType == TreeSelectionEventArgs.ITEMTYPE.asset_auditdata))
					{
						ILaytonView tabView = WorkItem.Items[Properties.Settings.Default.AuditedDataTabView] as ILaytonView;
						if (tabView != null)
						{
							base.SetTabView(tabView);
							((AuditedDataTabView)tabView).Presenter.Show(displayedNode, itemType);
						}
					}			
				}
			}

			catch (Exception ex)
			{
				//MessageBox.Show("Exception in SetTabView, message is " + ex.Message);
                logger.Error(ex.Message);
			}
		}


		/// <summary>
		/// This is the handler for the GLOBAL PublisherFilterChangeEvent which is fired when 
		/// the Publisher Filter has been updated elsewhere in the application.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		[EventSubscription(CommonEventTopics.PublisherFilterChanged)]
		public void PublisherFilterChangedHandler(object sender, PublisherFilterEventArgs e)
		{
			// Simply update our internal filters with those specified
			_publisherFilter = e.PublisherFilter;
			_showIncludedApplications = e.ViewIncludedApplications;
			_showIgnoredApplications = e.ViewIgnoredApplications;

			// ...and force a refresh if we are the active explorer view
			ILaytonView activeExplorerView = (ILaytonView)WorkItem.RootWorkItem.Workspaces[WorkspaceNames.ExplorerWorkspace].ActiveSmartPart;
			ILaytonView activeTabView =		 (ILaytonView)workItem.RootWorkItem.Workspaces[WorkspaceNames.TabWorkspace].ActiveSmartPart;
			if (activeExplorerView == WorkItem.ExplorerView)
			{
				Trace.Write("Refreshing Network Views\n");
				activeExplorerView.RefreshView();
				activeTabView.RefreshView();
			}
		}

#endregion Event Handlers

#region Deployment Functions

		/// <summary>
		/// Called to return a list of the computers that have been selected in either the tree or list view
		/// of the network tab depending on which view currently has focus
		/// </summary>
		/// <returns></returns>
		protected List<Asset> GetSelectedAssets()
		{
			List<Asset> listAssets = new List<Asset>();

			// Ensure that this is the correct view before trying to use it
			if (WorkItem.RootWorkItem.Workspaces[WorkspaceNames.ExplorerWorkspace].ActiveSmartPart is NetworkExplorerView)
			{
				NetworkExplorerView explorerView = (NetworkExplorerView)WorkItem.RootWorkItem.Workspaces[WorkspaceNames.ExplorerWorkspace].ActiveSmartPart;

				// The tab view may be either the GroupTabView or one of the other Computer views - we only
				// deal with the group tab view
				GroupTabView tabView = null;
				if (workItem.RootWorkItem.Workspaces[WorkspaceNames.TabWorkspace].ActiveSmartPart is GroupTabView)
					tabView = (GroupTabView)workItem.RootWorkItem.Workspaces[WorkspaceNames.TabWorkspace].ActiveSmartPart;

				if (explorerView.ContainsFocus)
				{
					listAssets = explorerView.GetSelectedAssets();
				}
				else if (tabView != null && tabView.ContainsFocus)
				{
					listAssets = tabView.GetSelectedAssets();
				}
			}
			return listAssets;
		}

		#region Deploy the AuditAgent Functions

		/// <summary>
		/// Deploy the remote audit agent to the computers currently selected within the Network view in either the
		/// Explorer or Tab views
		/// </summary>
		public void DeployAgents(string selectedAgentFileName)
		{
			// Get the list of selected computers
			_listAssets = GetSelectedAssets();

			// Call the common routine to perform the actual work.
			Deploy(selectedAgentFileName);
		}


		/// <summary>
		/// Deploy the audit agent client to the supplied List of <see cref="Asset"/> objects
		/// </summary>
        /// <param name="listComputers">List of computers to deploy the client to</param>
		public void DeployAgents(List<Asset> listComputers)
		{
			// Get the list of selected computers from that passed in and save
			_listAssets = listComputers;

            // Do we have any asset to act on - if not report this and exit
            if (_listAssets.Count == 0)
            {
                MessageBox.Show("There were no computers selected to be deployed.  Please try again.", "Deploy Error");
                return;
            }

            // Set the start date/time for the operation
            _operationStartDate = DateTime.Now;

            // We will pend the operation by adding it to the Operations queue in the database
            // The AuditWizard service works on this queue
            foreach (Asset asset in _listAssets)
            {
                Operation newOperation = new Operation(asset.AssetID, Operation.OPERATION.deployagent);
                newOperation.Add();
            }

            // Tell the user the operation has been pended or display the operations log
            if (Properties.Settings.Default.AlwaysShowOperationsLog)
                OperationsLog();
            else
                MessageBox.Show("The Deployment of the AuditAgent has been queued and will be actioned by the AuditWizard Service\n\nYou can check its progress by viewing the Operations Log", "Operations Queued", MessageBoxButtons.OK, MessageBoxIcon.Information);
		}
        
		/// <summary>
		/// Deploys the AuditWizard Audit Agent client to the computers held in the _listAssets object
		///
		/// The operation itself will not be performed synchronously - rather it is passed to the AuditWizard
		/// service to actually action.  Results can be viewed in the Operations Log
		/// </summary>
        protected void Deploy(string selectedFileName)
		{
            // Do we have any asset to act on - if not report this and exit
            if (_listAssets.Count == 0)
            {
                MessageBox.Show("There were no computers selected to be deployed.  Please try again.", "Deploy Error");
                return;
            }

			// Set the start date/time for the operation
			_operationStartDate = DateTime.Now;

			// Write the Agent Ini File to the Agent folder as this will combine the scanner configuration
			// with data taken from the Application Definitions File (publisher mappings etc)
            string agentIniFileName = Path.Combine(Application.StartupPath, AuditAgentStrings.AuditAgentFilesPath + "\\" + AuditAgentStrings.AuditAgentIni);
            
            try
            {
                File.Copy(selectedFileName, agentIniFileName, true);
            }
            catch (Exception ex)
            {
                // JML TODO log these errors and handle specific exceptions
                MessageBox.Show("Error : Failed to write the scanner configuration file, error was : " + ex.Message, "Deploy Error");
                return;
            }

			// We will pend the operation by adding it to the Operations queue in the database
			// The AuditWizard service works on this queue
			if (_listAssets.Count != 0)
			{
				foreach (Asset asset in _listAssets)
				{
					Operation newOperation = new Operation(asset.AssetID, Operation.OPERATION.deployagent);
					newOperation.Add();
				}

				// Tell the user the operation has been pended or display the operations log
				if (Properties.Settings.Default.AlwaysShowOperationsLog)
					OperationsLog();			
				else
					MessageBox.Show("The Deployment of the AuditAgent has been queued and will be actioned by the AuditWizard Service\n\nYou can check its progress by viewing the Operations Log", "Operations Queued", MessageBoxButtons.OK,  MessageBoxIcon.Information);
			}
		}

		#endregion AuditAgent Deploy Functions

		#region Start the AuditAgent Functions

		/// <summary>
		/// Start the AuditWizard Audit Agent on the selected Asset(s)
		/// </summary>
		public void Start()
		{
			// Set the start date/time for the operation
			_operationStartDate = DateTime.Now;

			// Get the list of selected computers
			_listAssets = GetSelectedAssets();

			// We will pend the operation by adding it to the Operations queue in the database
			// The AuditWizard service works on this queue
			if (_listAssets.Count != 0)
			{
				foreach (Asset asset in _listAssets)
				{
					Operation newOperation = new Operation(asset.AssetID, Operation.OPERATION.startagent);
					newOperation.Add();
				}

				// Tell the user the operation has been pended or display the operations log
				if (Properties.Settings.Default.AlwaysShowOperationsLog)
					OperationsLog();
				else
					MessageBox.Show("The AuditAgent Start Request has been queued and will be actioned by the AuditWizard Service\n\nYou can check its progress by viewing the Operations Log", "Operations Queued", MessageBoxButtons.OK, MessageBoxIcon.Information);
			}
		}

#endregion

		#region Stop AuditAgent Functions

		/// <summary>
		/// Attempt to stop and closedown the AuditWizard audit agent running on the selected computer(s)
		/// </summary>
		public void Stop()
		{
			// Set the start date/time for the operation
			_operationStartDate = DateTime.Now;

			// Get the list of selected computers
			_listAssets = GetSelectedAssets();

			// We will pend the operation by adding it to the Operations queue in the database
			// The AuditWizard service works on this queue
			if (_listAssets.Count != 0)
			{
				foreach (Asset asset in _listAssets)
				{
					Operation newOperation = new Operation(asset.AssetID, Operation.OPERATION.stopagent);
					newOperation.Add();
				}

				// Tell the user the operation has been pended or display the operations log
				if (Properties.Settings.Default.AlwaysShowOperationsLog)
					OperationsLog();
				else
					MessageBox.Show("The AuditAgent Stop Request has been queued and will be actioned by the AuditWizard Service\n\nYou can check its progress by viewing the Operations Log", "Operations Queued", MessageBoxButtons.OK, MessageBoxIcon.Information);
			}
		}

		#endregion Stop AuditAgent functions

		/// <summary>
		/// Called to remove the AuditWizard Audit Agent from the selected Asset(s)
		/// </summary>
		public void Remove()
		{
			// Set the start date/time for the operation
			_operationStartDate = DateTime.Now;

			// Get the list of selected computers
			_listAssets = GetSelectedAssets();

			// We will pend the operation by adding it to the Operations queue in the database
			// The AuditWizard service works on this queue
			if (_listAssets.Count != 0)
			{
				foreach (Asset asset in _listAssets)
				{
					Operation newOperation = new Operation(asset.AssetID, Operation.OPERATION.removeagent);
					newOperation.Add();
				}

				// Tell the user the operation has been pended or display the operations log
				if (Properties.Settings.Default.AlwaysShowOperationsLog)
					OperationsLog();
				else
					MessageBox.Show("The AuditAgent Remove Request has been queued and will be actioned by the AuditWizard Service\n\nYou can check its progress by viewing the Operations Log", "Operations Queued", MessageBoxButtons.OK, MessageBoxIcon.Information);
			}
		}



		/// <summary>
		/// Called to request a re-audit of the selected List of <see cref="Asset"/> objects
		/// </summary>
		public void RequestReaudit()
		{
			// Set the start date/time for the operation
			_operationStartDate = DateTime.Now;

			// Get the list of selected computers
			_listAssets = GetSelectedAssets();

			// We will pend the operation by adding it to the Operations queue in the database
			// The AuditWizard service works on this queue
			if (_listAssets.Count != 0)
			{
				foreach (Asset asset in _listAssets)
				{
					Operation newOperation = new Operation(asset.AssetID, Operation.OPERATION.reaudit);
					newOperation.Add();
				}

				// Tell the user the operation has been pended or display the operations log
				if (Properties.Settings.Default.AlwaysShowOperationsLog)
					OperationsLog();
				else
					MessageBox.Show("The AuditAgent Start Request has been queued and will be actioned by the AuditWizard Service\n\nYou can check its progress by viewing the Operations Log", "Operations Queued", MessageBoxButtons.OK, MessageBoxIcon.Information);
			}
		}


		/// <summary>
		/// Check the current deployment status of the AuditWizard agent on the specified computer(s)
		/// </summary>
		public void CheckStatus()
		{
			// Set the start date/time for the operation
			_operationStartDate = DateTime.Now;

			// Get the list of selected computers
			_listAssets = GetSelectedAssets();

			// We will pend the operation by adding it to the Operations queue in the database
			// The AuditWizard service works on this queue
			if (_listAssets.Count != 0)
			{
				foreach (Asset asset in _listAssets)
				{
					Operation newOperation = new Operation(asset.AssetID, Operation.OPERATION.checkstatus);
					newOperation.Add();
				}

				// Tell the user the operation has been pended or display the operations log
				if (Properties.Settings.Default.AlwaysShowOperationsLog)
					OperationsLog();
				else
					MessageBox.Show("The Request to check the status of the AuditWizard Agent has been queued and will be actioned by the AuditWizard Service\n\nYou can check its progress by viewing the Operations Log", "Operations Queued", MessageBoxButtons.OK, MessageBoxIcon.Information);
			}
		}



		/// <summary>
		/// Clear the AuditWizard Agent Log file on the remote computer
		/// </summary>
		public void ClearLogFile()
		{
			// Set the start date/time for the operation
			_operationStartDate = DateTime.Now;

			// Get the list of selected computers
			_listAssets = GetSelectedAssets();

			// We will pend the operation by adding it to the Operations queue in the database
			// The AuditWizard service works on this queue
			if (_listAssets.Count != 0)
			{
				foreach (Asset asset in _listAssets)
				{
					Operation newOperation = new Operation(asset.AssetID, Operation.OPERATION.startagent);
					newOperation.Add();
				}

				// Tell the user the operation has been pended or display the operations log
				if (Properties.Settings.Default.AlwaysShowOperationsLog)
					OperationsLog();
				else
					MessageBox.Show("The Request to clear the log file for the selected AuditAgent(s) has been queued and will be actioned by the AuditWizard Service\n\nYou can check its progress by viewing the Operations Log", "Operations Queued", MessageBoxButtons.OK, MessageBoxIcon.Information);
			}
		}



		/// <summary>
		/// Called to allow us to recover and then view the audit agent log file from the remote computer
		/// </summary>
		public void ViewLogFile()
		{
			// Get the list of selected computers
			_listAssets = GetSelectedAssets();

			// ...and iterate through the list
			if (_listAssets.Count > 0)
			{
				try
				{
					AuditAgentServiceController agentServiceController = new AuditAgentServiceController(_listAssets[0].Name);
					agentServiceController.ViewLogFile();
				}
				catch (Exception e)
				{
					MessageBox.Show(
                        "Error opening the log file on " + _listAssets[0].Name + Environment.NewLine + Environment.NewLine + e.Message, 
                        "Error Opening Log File", 
                        MessageBoxButtons.OK, 
                        MessageBoxIcon.Information);
				}
			}
		}

		#region Update the AuditAgent Configuration Functions

		/// <summary>
		/// Update the AuditWizard Audit Agent Configuration on the selected Asset(s)
		/// </summary>
        public void UpdateAgentConfiguration(string selectedFileName)
		{
            // Get the list of selected computers
            _listAssets = GetSelectedAssets();

            // Do we have any asset to act on - if not report this and exit
            if (_listAssets.Count == 0)
            {
                MessageBox.Show("There were no computers selected to be deployed.  Please try again.", "Deploy Error");
                return;
            }

            // Set the start date/time for the operation
            _operationStartDate = DateTime.Now;

            try
            {
                File.Copy(selectedFileName, Path.Combine(AuditAgentStrings.AuditAgentFilesPath, AuditAgentStrings.AuditAgentIni), true);
            }
            catch (FileNotFoundException ex)
            {
                logger.Error(ex.Message);

                MessageBox.Show("An error has occurred whilst updating the configuration, please see the log file for further information.",
                    "AuditWizard", MessageBoxButtons.OK, MessageBoxIcon.Error);

                return;
            }
            catch (IOException ex)
            {
                logger.Error(ex.Message);

                MessageBox.Show("An error has occurred whilst updating the configuration, please see the log file for further information.",
                    "AuditWizard", MessageBoxButtons.OK, MessageBoxIcon.Error);

                logger.Error(ex.Message);
                return;
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message);

                MessageBox.Show("An error has occurred whilst updating the configuration, please see the log file for further information.",
                    "AuditWizard", MessageBoxButtons.OK, MessageBoxIcon.Error);

                // JML TODO log this exception
                return;

            }

			// We will pend the operation by adding it to the Operations queue in the database
			// The AuditWizard service works on this queue
			if (_listAssets.Count != 0)
			{
				foreach (Asset asset in _listAssets)
				{
					Operation newOperation = new Operation(asset.AssetID, Operation.OPERATION.updateconfiguration);
					newOperation.Add();
				}

				// Tell the user the operation has been pended or display the operations log
				if (Properties.Settings.Default.AlwaysShowOperationsLog)
					OperationsLog();
				else
					MessageBox.Show("The AuditAgent Update Request has been queued and will be actioned by the AuditWizard Service\n\nYou can check its progress by viewing the Operations Log", "Operations Queued", MessageBoxButtons.OK, MessageBoxIcon.Information);
			}
		}

		#endregion Update the AuditAgent Configuration Functions


		/// <summary>
		/// Called to re-locate the selected assets based on their IP addresses
		/// </summary>
		public void RelocateByIPAddress()
		{
			int relocated = 0;
			
			// Get the list of selected computers
			_listAssets = GetSelectedAssets();
			
			// ...and populate a list of all locations defined
			LocationsDAO lwDataAccess = new LocationsDAO();
			DataTable table = lwDataAccess.GetAllLocations();
			AssetGroupList listGroups = new AssetGroupList();

			foreach (DataRow row in table.Rows)
			{
				AssetGroup group = new AssetGroup(row, AssetGroup.GROUPTYPE.userlocation);
				listGroups.Add(group);
			}
		
			// Loop through the assets and determine the best match location
			foreach (Asset asset in _listAssets)
			{
				// Loop through the groups and see if we can find a good match for the IP address of the asset (if any)
				AssetGroup matchedGroup = listGroups.FindByIP(asset.IPAddress);
				if ((matchedGroup == null) || (matchedGroup.GroupID == asset.LocationID))
					continue;
				
				// A match was found that is different to the current location so update this asset
				asset.LocationID = matchedGroup.GroupID;
				asset.Update();
				relocated++;
			}

            string message = (relocated == 1) ? "1 asset has been relocated based on IP address" : 
                string.Format("{0} asset(s) have been relocated based on their IP address", relocated);

            DesktopAlert.ShowDesktopAlert(message);

			// Refresh the display if ANY assets have been re-located
			if (relocated != 0)
				RefreshView();				
		}




		/// <summary>
		/// Called to allow us to remote desktop to the remote computer
		/// </summary>
		public void RemoteDesktop()
		{
			// Get the list of selected computers
			_listAssets = GetSelectedAssets();

			// ...and iterate through the list
			if (_listAssets.Count != 1)
				return;
				
			// get the remote desktop command from the configuration file				
			Configuration config = ConfigurationManager.OpenExeConfiguration(Path.Combine(Application.StartupPath, "AuditWizardv8.exe"));

			// Now try and read the product license key details from it
			try
			{		
				// get the remote desktop command from the database
				SettingsDAO lwDataAccess = new SettingsDAO();
				string remoteDesktop = lwDataAccess.GetSetting("RemoteDesktopCommand" ,false);
				if (remoteDesktop == "")
					return;
									
				// ...and the asset from that selected
				Asset asset = _listAssets[0];
				
				// Perform any substitutions
				remoteDesktop = remoteDesktop.ToUpper();
				remoteDesktop = remoteDesktop.Replace(@"%SYSTEMROOT%", System.Environment.GetEnvironmentVariable(@"%SYSTEMROOT%"));
				remoteDesktop = remoteDesktop.Replace(@"%I" ,asset.IPAddress);
				remoteDesktop = remoteDesktop.Replace(@"%A", asset.Name);
				
				// Now run the command
				STARTUPINFO si = new STARTUPINFO();
				PROCESS_INFORMATION pi = new PROCESS_INFORMATION();

				CreateProcess(null 
							, remoteDesktop
							, IntPtr.Zero
							, IntPtr.Zero
							, false
							, 0
							, IntPtr.Zero
							, null
							, ref si
							, out pi);
			}
			catch (Exception ex)
			{
				MessageBox.Show("Failed to launch the Remote Desktop Session, the error was " + ex.Message ,"Remote Desktop Error");
			}
		}


		#region Asset State Functions


		/// <summary>
		/// Set the selected Assets as 'In Stock'
		/// </summary>
		public void SetAssetStock()
		{
			List<Asset> listComputers = GetSelectedAssets();
			SetAssetState(listComputers, Asset.STOCKSTATUS.stock);
		}


		/// <summary>
		/// Set the selected Assets as 'In Use'
		/// </summary>
		public void SetAssetInUse()
		{
			List<Asset> listComputers = GetSelectedAssets();
			SetAssetState(listComputers, Asset.STOCKSTATUS.inuse);
		}


		/// <summary>
		/// Set the selected Assets as 'Pending Disposal'
		/// </summary>
		public void SetAssetPending()
		{
			List<Asset> listComputers = GetSelectedAssets();
			SetAssetState(listComputers, Asset.STOCKSTATUS.pendingdisposal);
		}


		/// <summary>
		/// Set the selected Assets as 'Disposed'
		/// </summary>
		public void SetAssetDisposed()
		{
			List<Asset> listComputers = GetSelectedAssets();
			SetAssetState(listComputers, Asset.STOCKSTATUS.disposed);
		}

		/// <summary>
		/// Hide the selected computer(s) within the database
		/// </summary>
		public void SetAssetState(List<Asset> listAssets ,Asset.STOCKSTATUS newStatus)
		{
			// Iterate through the selected assets and change the state for each
			foreach (Asset asset in listAssets)
			{
				asset.UpdateStockStatus(newStatus);
			}
			RefreshView();
		}

		
		#endregion Asset State Functions


		/// <summary>
		/// Display the Operations Log Form
		/// </summary>
		public void OperationsLog()
		{
            AuditWizardServiceController serviceController = new AuditWizardServiceController();
            if (serviceController.CheckStatus() != LaytonServiceController.ServiceStatus.Running)
            {
                if (MessageBox.Show("The AuditWizard service is currently stopped and should be started to process any outstanding operations." +
                                Environment.NewLine + Environment.NewLine +
                                "Would you like to configure the service now?"
                                , "AuditWizard Service"
                                , MessageBoxButtons.YesNo
                                , MessageBoxIcon.Question) == DialogResult.Yes)
                {

                    FormAuditWizardServiceControl serviceForm = new FormAuditWizardServiceControl();
                    if (serviceForm.ShowDialog() != DialogResult.OK)
                        return;
                }
            }

			FormOperationsLog form = new FormOperationsLog();
			form.StartDateTime = _operationStartDate.AddMinutes(-1);
			form.ShowDialog();
			RefreshView();
		}

		/// <summary>
		/// Check the current deployment status of the AuditWizard agent on the specified computer(s)
		/// </summary>
		public void UploadData()
		{
            List<WorkItem> workItemList = (List<WorkItem>)workItem.RootWorkItem.WorkItems.FindByType(typeof(NetworkWorkItem));
            NetworkWorkItem netDiscWorkItem = workItemList[0] as NetworkWorkItem;

		    if (netDiscWorkItem != null)
		    {
		        NetworkExplorerView explorerView = (NetworkExplorerView)netDiscWorkItem.ExplorerView;
                explorerView.Clear();
		    }

            RefreshView();

			FormUploadAudits form = new FormUploadAudits(LicenseCount);
			form.ShowDialog();

			// ...and force a refresh
			RefreshView();
		}


		#endregion Deployment Functions

		public void ChangeGroup(List<Asset> listDroppedAssets ,AssetGroup newGroup)
		{
			foreach (Asset asset in listDroppedAssets)
			{
				if (newGroup.GroupType == AssetGroup.GROUPTYPE.userlocation)
				{
					asset.LocationID = newGroup.GroupID;
					asset.Location = newGroup.Name;
				}
				else
				{
					asset.DomainID = newGroup.GroupID;
					asset.Domain = newGroup.Name;
				}
				asset.Update();
			}

			// Update the view
			RefreshView();
		}



		/// <summary>
		/// Add a new asset to the database
		/// </summary>
		public void AddAsset(AssetGroup toGroup)
		{
			// Create a new asset in this group
			Asset asset = new Asset();
			asset.Name = "<New Asset>";

			// Are there any assets in this group now?  If so set the new asset to be their sibling
			if (toGroup.Assets.Count != 0)
			{
				asset.SetAsSibling(toGroup.Assets[0]);
			}
			else
			{
				if (_showByDomain)
				{
					asset.Location = "";
					asset.LocationID = 1;
					asset.Domain = toGroup.FullName;
					asset.DomainID = toGroup.GroupID;
				}
				else
				{
					asset.Location = toGroup.FullName;
					asset.LocationID = toGroup.GroupID;
					asset.Domain = "";
					asset.DomainID = 1;
				}
			}
			
			// ...and allow us to define its characteristics
			FormAddAsset form = new FormAddAsset(asset);
			if (form.ShowDialog() == DialogResult.OK)
				RefreshView();
		}



		/// <summary>
		/// Find an asset in the database
		/// </summary>
		public void FindAsset()
		{
			NetworkExplorerView explorerView = (NetworkExplorerView)workItem.ExplorerView;
			FormFindAsset form = new FormFindAsset(explorerView.GetDisplayedTree);
			form.ShowDialog();
		}


		/// <summary>
		/// Called as we change the check state for 'View 'Stock' assets
		/// </summary>
		public void ShowStockAssets(bool show)
		{
			Configuration config = ConfigurationManager.OpenExeConfiguration(Path.Combine(Application.StartupPath, "AuditWizardv8.exe"));
			config.AppSettings.Settings["ShowStock"].Value = show.ToString();
			config.Save();
			RefreshView();
		}


		/// <summary>
		/// Called as we change the check state for 'View 'In Use' assets
		/// </summary>
		public void ShowInUseAssets(bool show)
		{
			Configuration config = ConfigurationManager.OpenExeConfiguration(Path.Combine(Application.StartupPath, "AuditWizardv8.exe"));
			config.AppSettings.Settings["ShowInUse"].Value = show.ToString();
			config.Save();
			RefreshView();
		}


		/// <summary>
		/// Called as we change the check state for 'View 'Pending' assets
		/// </summary>
		public void ShowPendingAssets(bool show)
		{
			Configuration config = ConfigurationManager.OpenExeConfiguration(Path.Combine(Application.StartupPath, "AuditWizardv8.exe"));
			config.AppSettings.Settings["ShowPending"].Value = show.ToString();
			config.Save();
			RefreshView();
		}


		/// <summary>
		/// Called as we change the check state for 'View 'Disposed' assets
		/// </summary>
		public void ShowDisposedAssets(bool show)
		{
			Configuration config = ConfigurationManager.OpenExeConfiguration(Path.Combine(Application.StartupPath, "AuditWizardv8.exe"));
			config.AppSettings.Settings["ShowDisposed"].Value = show.ToString();
			config.Save();
			RefreshView();
		}

		/// <summary>
		/// Delete the selected computer(s) within the database
		/// </summary>
		public void DeleteAsset()
		{
			// Get the list of selected computers and delete each
			List<Asset> listComputers = GetSelectedAssets();
			DeleteAsset(listComputers);
		}

		/// <summary>
		/// Hide the selected computer(s) within the database
		/// </summary>
		public void DeleteAsset(List<Asset> listAssets)
		{
			// Have we selected a group?  If so we will delete all of the computers within that group
			// after we confirm the operation as it is quite widespread
			if (MessageBox.Show("Are you sure that you want to delete the selected asset(s), this operation is not reversible"
							  , "Confirm Delete"
							  , MessageBoxButtons.YesNo
							  , MessageBoxIcon.Question
							  ,MessageBoxDefaultButton.Button2) != DialogResult.Yes)
					return;
	
			// OK go off and delete the computers then
			foreach (Asset asset in listAssets)
			{
				asset.Delete();
			}

			RefreshView();
		}


#region Export Menu Functions



		/// <summary>
		/// Export to PDF format 
		/// We call the appropriate grid view to handle this request themselves
		/// </summary>
		public void ExportToPDF()
		{
			ILaytonView tabView = (ILaytonView)WorkItem.RootWorkItem.Workspaces[WorkspaceNames.TabWorkspace].ActiveSmartPart;
			if (tabView is GroupTabView)
				((GroupTabView)tabView).ExportToPDF();

			else if (tabView is ApplicationInstancesTabView)
				((ApplicationInstancesTabView)tabView).ExportToPDF();
		}


		/// <summary>
		/// Export to XLS format 
		/// We call the appropriate grid view to handle this request themselves
		/// </summary>
		public void ExportToXLS()
		{
			ILaytonView tabView = (ILaytonView)WorkItem.RootWorkItem.Workspaces[WorkspaceNames.TabWorkspace].ActiveSmartPart;
			if (tabView is GroupTabView)
				((GroupTabView)tabView).ExportToXLS();

			else if (tabView is ApplicationInstancesTabView)
				((ApplicationInstancesTabView)tabView).ExportToXLS();
		}


		/// <summary>
		/// Export to XPS format 
		/// We call the appropriate grid view to handle this request themselves
		/// </summary>
		public void ExportToXPS()
		{
			ILaytonView tabView = (ILaytonView)WorkItem.RootWorkItem.Workspaces[WorkspaceNames.TabWorkspace].ActiveSmartPart;
			if (tabView is GroupTabView)
				((GroupTabView)tabView).ExportToXPS();

			else if (tabView is ApplicationInstancesTabView)
				((ApplicationInstancesTabView)tabView).ExportToXPS();
		}

		#endregion Export Menu Functions

		/// <summary>
		/// Does this user data category apply to this asset?
		/// </summary>
		/// <param name="asset"></param>
		/// <returns>true if yes, false otherwise</returns>
		public bool CategoryAppliesTo(UserDataCategory category, Asset asset)
		{
			// Is this category specific to an asset type?
			if (category.AppliesTo != 0)
			{
				// OK - does the category apply specifically to this asset type?
				if (category.AppliesTo != asset.AssetTypeID)
				{
					// No - we need to get the parent category of this asset type and check that also then
					AssetType parentType = AssetTypes.FindByName(asset.TypeAsString);
					if ((parentType == null) || (category.AppliesTo != parentType.ParentID))
						return false;
				}

			}

			// User data category applies to this type of asset so return true
			return true;
		}
	}
}
