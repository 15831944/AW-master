using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Drawing;
using System.Data;
using System.IO;
using System.Text;
using System.Windows.Forms;
using Microsoft.Practices.CompositeUI.SmartParts;
using Layton.Cab.Interface;
using Microsoft.Practices.CompositeUI;
using Microsoft.Practices.CompositeUI.Utility;
using Microsoft.Practices.ObjectBuilder;
//
using Infragistics.Win;
using Infragistics.Win.UltraWinDataSource;
using Infragistics.Win.UltraWinGrid;
//
using Layton.AuditWizard.Common;
using Layton.AuditWizard.DataAccess;
//
using PickerSample;

namespace Layton.AuditWizard.Administration
{
    [SmartPart]
    public partial class ToolsTabView : UserControl, ILaytonView, IAdministrationView
    {
        private LaytonWorkItem workItem;
		ToolsTabViewPresenter presenter;

        [InjectionConstructor]
		public ToolsTabView([ServiceDependency] WorkItem workItem)
        {
            this.workItem = workItem as LaytonWorkItem;
            InitializeComponent();
        }

		[CreateNew]
		public ToolsTabViewPresenter Presenter
		{
			set { presenter = value; presenter.View = this; presenter.Initialize(); }
			get { return presenter; }
		}

        public void RefreshViewSinglePublisher()
        {
        }

		/// <summary>
		/// Refresh the current view
		/// </summary>
		public void RefreshView()
		{
			base.Refresh();
		}


		/// <summary>
		/// Called as this tab is activated to ensure that we display the latest possible data
		/// This function comes from the IAdministrationView Interface
		/// </summary>
		public void Activate()
		{
			InitializeTools();		
		}

		/// <summary>
		/// save function for the IAdministrationView Interface
		/// </summary>
		public void Save()
		{
			SaveToolsTab();
		}

        public LaytonWorkItem WorkItem
        {
            get { return workItem; }
        }


		private void ToolsTabView_Load(object sender, EventArgs e)
		{
            InitializeTools();
		}

        private void ToolsTabView_Leave(object sender, EventArgs e)
        {
        }

		/// <summary>
		/// Initialize this tab
		/// </summary>
		private void InitializeTools()
		{
			SettingsDAO lwDataAccess = new SettingsDAO();
			string remoteDesktop = lwDataAccess.GetSetting("RemoteDesktopCommand", false);
			if (remoteDesktop == "")
				remoteDesktop = Path.Combine(System.Environment.GetFolderPath(Environment.SpecialFolder.System) ,"mstsc.exe /v:%A");
			tbRemoteDesktop.Text = remoteDesktop;
		}


		/// <summary>
		/// Save any data entered on the tools tab
		/// </summary>
		private void SaveToolsTab()
		{
			SettingsDAO lwDataAccess = new SettingsDAO();
			lwDataAccess.SetSetting("RemoteDesktopCommand", tbRemoteDesktop.Text ,false);
		}

		private void bnBrowse_Click_1(object sender, EventArgs e)
		{
			if (this.openFileDialog1.ShowDialog() == DialogResult.OK)
				tbRemoteDesktop.Text = openFileDialog1.FileName;
		}

        private void btnUpdateSettings_Click(object sender, EventArgs e)
        {
            SaveToolsTab();
            btnUpdateSettings.Enabled = false;
        }

        private void Text_Changed(object sender, EventArgs e)
        {
            if (tbRemoteDesktop.Text.Length > 0)
                btnUpdateSettings.Enabled = true;
            else
                btnUpdateSettings.Enabled = false;
        }
    }
}
