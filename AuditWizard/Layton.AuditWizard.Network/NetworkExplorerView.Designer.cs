namespace Layton.AuditWizard.Network
{
	partial class NetworkExplorerView
	{
		/// <summary> 
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary> 
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Component Designer generated code

		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			Infragistics.Win.Appearance appearance1 = new Infragistics.Win.Appearance();
			Infragistics.Win.UltraWinTree.Override _override1 = new Infragistics.Win.UltraWinTree.Override();
			Infragistics.Win.UltraWinScrollBar.ScrollBarLook scrollBarLook1 = new Infragistics.Win.UltraWinScrollBar.ScrollBarLook();
			this.networkTree = new Infragistics.Win.UltraWinTree.UltraTree();
			this.networkMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.findAssetToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.newAssetToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.deleteComputersToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.assetToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.relocateByIPMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.remoteDesktopMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.statusToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.stockMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.inUseMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.pendingDisposalMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.disposedMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.auditAgentToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.deployToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.updateConfigurationToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.startToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.stopToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.removeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.checkStatusToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.viewLogFileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.clearLogFileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.auditNowToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.reAuditDeviceToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			((System.ComponentModel.ISupportInitialize)(this.networkTree)).BeginInit();
			this.networkMenuStrip.SuspendLayout();
			this.SuspendLayout();
			// 
			// networkTree
			// 
			this.networkTree.AllowDrop = true;
			appearance1.BackColor = System.Drawing.Color.Transparent;
			this.networkTree.Appearance = appearance1;
			this.networkTree.ContextMenuStrip = this.networkMenuStrip;
			this.networkTree.Dock = System.Windows.Forms.DockStyle.Fill;
			this.networkTree.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.networkTree.HideSelection = false;
			this.networkTree.Location = new System.Drawing.Point(0, 0);
			this.networkTree.Name = "networkTree";
			_override1.SelectionType = Infragistics.Win.UltraWinTree.SelectType.Extended;
			_override1.Sort = Infragistics.Win.UltraWinTree.SortType.None;
			this.networkTree.Override = _override1;
			scrollBarLook1.ViewStyle = Infragistics.Win.UltraWinScrollBar.ScrollBarViewStyle.Outlook2007;
			this.networkTree.ScrollBarLook = scrollBarLook1;
			this.networkTree.ScrollBounds = Infragistics.Win.UltraWinTree.ScrollBounds.ScrollToFill;
			this.networkTree.Size = new System.Drawing.Size(217, 441);
			this.networkTree.TabIndex = 2;
			this.networkTree.AfterSelect += new Infragistics.Win.UltraWinTree.AfterNodeSelectEventHandler(this.networkTree_AfterSelect);
			this.networkTree.BeforeExpand += new Infragistics.Win.UltraWinTree.BeforeNodeChangedEventHandler(this.networkTree_BeforeExpand);
			this.networkTree.SelectionDragStart += new System.EventHandler(this.networkTree_SelectionDragStart);
			this.networkTree.DragDrop += new System.Windows.Forms.DragEventHandler(this.networkTree_DragDrop);
			this.networkTree.DragOver += new System.Windows.Forms.DragEventHandler(this.networkTree_DragOver);
			this.networkTree.DragLeave += new System.EventHandler(this.networkTree_DragLeave);
			this.networkTree.QueryContinueDrag += new System.Windows.Forms.QueryContinueDragEventHandler(this.applicationsTree_QueryContinueDrag);
			this.networkTree.KeyDown += new System.Windows.Forms.KeyEventHandler(this.networkTree_KeyDown);
			this.networkTree.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.networkTree_KeyPress);
			this.networkTree.MouseDown += new System.Windows.Forms.MouseEventHandler(this.networkTree_MouseDown);
			// 
			// networkMenuStrip
			// 
			this.networkMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.findAssetToolStripMenuItem,
            this.newAssetToolStripMenuItem,
            this.deleteComputersToolStripMenuItem,
            this.assetToolStripMenuItem,
            this.statusToolStripMenuItem,
            this.auditAgentToolStripMenuItem,
            this.reAuditDeviceToolStripMenuItem});
			this.networkMenuStrip.Name = "networkMenuStrip";
			this.networkMenuStrip.Size = new System.Drawing.Size(175, 158);
			this.networkMenuStrip.Opening += new System.ComponentModel.CancelEventHandler(this.networkMenuStrip_Opening);
			// 
			// findAssetToolStripMenuItem
			// 
			this.findAssetToolStripMenuItem.Image = global::Layton.AuditWizard.Network.Properties.Resources.Find_16;
			this.findAssetToolStripMenuItem.Name = "findAssetToolStripMenuItem";
			this.findAssetToolStripMenuItem.Size = new System.Drawing.Size(174, 22);
			this.findAssetToolStripMenuItem.Text = "&Find Asset";
			this.findAssetToolStripMenuItem.Click += new System.EventHandler(this.findAssetToolStripMenuItem_Click);
			// 
			// newAssetToolStripMenuItem
			// 
			this.newAssetToolStripMenuItem.Image = global::Layton.AuditWizard.Network.Properties.Resources.computer_add_16;
			this.newAssetToolStripMenuItem.Name = "newAssetToolStripMenuItem";
			this.newAssetToolStripMenuItem.Size = new System.Drawing.Size(174, 22);
			this.newAssetToolStripMenuItem.Text = "&New Asset";
			this.newAssetToolStripMenuItem.Click += new System.EventHandler(this.newAssetToolStripMenuItem_Click);
			// 
			// deleteComputersToolStripMenuItem
			// 
			this.deleteComputersToolStripMenuItem.Image = global::Layton.AuditWizard.Network.Properties.Resources.computer_delete16;
			this.deleteComputersToolStripMenuItem.Name = "deleteComputersToolStripMenuItem";
			this.deleteComputersToolStripMenuItem.Size = new System.Drawing.Size(174, 22);
			this.deleteComputersToolStripMenuItem.Text = "&Delete Asset(s)";
			this.deleteComputersToolStripMenuItem.Click += new System.EventHandler(this.deleteComputersToolStripMenuItem_Click);
			// 
			// assetToolStripMenuItem
			// 
			this.assetToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.relocateByIPMenuItem,
            this.remoteDesktopMenuItem});
			this.assetToolStripMenuItem.Image = global::Layton.AuditWizard.Network.Properties.Resources.computer16;
			this.assetToolStripMenuItem.Name = "assetToolStripMenuItem";
			this.assetToolStripMenuItem.Size = new System.Drawing.Size(174, 22);
			this.assetToolStripMenuItem.Text = "Asset";
			// 
			// relocateByIPMenuItem
			// 
			this.relocateByIPMenuItem.Name = "relocateByIPMenuItem";
			this.relocateByIPMenuItem.Size = new System.Drawing.Size(198, 22);
			this.relocateByIPMenuItem.Text = "Re-locate by IP Address";
			this.relocateByIPMenuItem.Click += new System.EventHandler(this.relocateByIPMenuItem_Click);
			// 
			// remoteDesktopMenuItem
			// 
			this.remoteDesktopMenuItem.Name = "remoteDesktopMenuItem";
			this.remoteDesktopMenuItem.Size = new System.Drawing.Size(198, 22);
			this.remoteDesktopMenuItem.Text = "Remote &Desktop";
			this.remoteDesktopMenuItem.Click += new System.EventHandler(this.remoteDesktopMenuItem_Click);
			// 
			// statusToolStripMenuItem
			// 
			this.statusToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.stockMenuItem,
            this.inUseMenuItem,
            this.pendingDisposalMenuItem,
            this.disposedMenuItem});
			this.statusToolStripMenuItem.Name = "statusToolStripMenuItem";
			this.statusToolStripMenuItem.Size = new System.Drawing.Size(174, 22);
			this.statusToolStripMenuItem.Text = "Status";
			// 
			// stockMenuItem
			// 
			this.stockMenuItem.Image = global::Layton.AuditWizard.Network.Properties.Resources.computer_stock_16;
			this.stockMenuItem.Name = "stockMenuItem";
			this.stockMenuItem.Size = new System.Drawing.Size(215, 22);
			this.stockMenuItem.Text = "Mark as \'Stock\'";
			this.stockMenuItem.Click += new System.EventHandler(this.stockMenuItem_Click);
			// 
			// inUseMenuItem
			// 
			this.inUseMenuItem.Image = global::Layton.AuditWizard.Network.Properties.Resources.computer16;
			this.inUseMenuItem.Name = "inUseMenuItem";
			this.inUseMenuItem.Size = new System.Drawing.Size(215, 22);
			this.inUseMenuItem.Text = "Mark as \'In Use\'";
			this.inUseMenuItem.Click += new System.EventHandler(this.inUseMenuItem_Click);
			// 
			// pendingDisposalMenuItem
			// 
			this.pendingDisposalMenuItem.Image = global::Layton.AuditWizard.Network.Properties.Resources.computer_pending_16;
			this.pendingDisposalMenuItem.Name = "pendingDisposalMenuItem";
			this.pendingDisposalMenuItem.Size = new System.Drawing.Size(215, 22);
			this.pendingDisposalMenuItem.Text = "Mark as \'Pending Disposal\'";
			this.pendingDisposalMenuItem.Click += new System.EventHandler(this.pendingDisposalMenuItem_Click);
			// 
			// disposedMenuItem
			// 
			this.disposedMenuItem.Image = global::Layton.AuditWizard.Network.Properties.Resources.computer_disposed_16;
			this.disposedMenuItem.Name = "disposedMenuItem";
			this.disposedMenuItem.Size = new System.Drawing.Size(215, 22);
			this.disposedMenuItem.Text = "Mark as \'Disposed\'";
			this.disposedMenuItem.Click += new System.EventHandler(this.disposedMenuItem_Click);
			// 
			// auditAgentToolStripMenuItem
			// 
			this.auditAgentToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.deployToolStripMenuItem,
            this.updateConfigurationToolStripMenuItem,
            this.startToolStripMenuItem,
            this.stopToolStripMenuItem,
            this.removeToolStripMenuItem,
            this.checkStatusToolStripMenuItem,
            this.viewLogFileToolStripMenuItem,
            this.clearLogFileToolStripMenuItem,
            this.auditNowToolStripMenuItem});
			this.auditAgentToolStripMenuItem.Image = global::Layton.AuditWizard.Network.Properties.Resources.scanner_16;
			this.auditAgentToolStripMenuItem.Name = "auditAgentToolStripMenuItem";
			this.auditAgentToolStripMenuItem.Size = new System.Drawing.Size(174, 22);
			this.auditAgentToolStripMenuItem.Text = "AuditWizard Agent";
			// 
			// deployToolStripMenuItem
			// 
			this.deployToolStripMenuItem.Image = global::Layton.AuditWizard.Network.Properties.Resources.deploy16;
			this.deployToolStripMenuItem.Name = "deployToolStripMenuItem";
			this.deployToolStripMenuItem.Size = new System.Drawing.Size(189, 22);
			this.deployToolStripMenuItem.Text = "&Deploy";
			this.deployToolStripMenuItem.ToolTipText = "Deploy the AuditAgent to the selected PC(s)";
			// 
			// updateConfigurationToolStripMenuItem
			// 
			this.updateConfigurationToolStripMenuItem.Image = global::Layton.AuditWizard.Network.Properties.Resources.agent_refresh16;
			this.updateConfigurationToolStripMenuItem.Name = "updateConfigurationToolStripMenuItem";
			this.updateConfigurationToolStripMenuItem.Size = new System.Drawing.Size(189, 22);
			this.updateConfigurationToolStripMenuItem.Text = "Update Configuration";
			this.updateConfigurationToolStripMenuItem.ToolTipText = "Update the scanner configuration for the selected AuditAgent(s)";
			this.updateConfigurationToolStripMenuItem.Click += new System.EventHandler(this.updateConfigurationToolStripMenuItem_Click);
			// 
			// startToolStripMenuItem
			// 
			this.startToolStripMenuItem.Image = global::Layton.AuditWizard.Network.Properties.Resources.start16;
			this.startToolStripMenuItem.Name = "startToolStripMenuItem";
			this.startToolStripMenuItem.Size = new System.Drawing.Size(189, 22);
			this.startToolStripMenuItem.Text = "&Start";
			this.startToolStripMenuItem.ToolTipText = "Start the AuditAgent on the selected PC(s)";
			this.startToolStripMenuItem.Click += new System.EventHandler(this.startToolStripMenuItem_Click);
			// 
			// stopToolStripMenuItem
			// 
			this.stopToolStripMenuItem.Image = global::Layton.AuditWizard.Network.Properties.Resources.stop16;
			this.stopToolStripMenuItem.Name = "stopToolStripMenuItem";
			this.stopToolStripMenuItem.Size = new System.Drawing.Size(189, 22);
			this.stopToolStripMenuItem.Text = "&Stop";
			this.stopToolStripMenuItem.ToolTipText = "Stop the AuditAgent Service on the selected PC(s)";
			this.stopToolStripMenuItem.Click += new System.EventHandler(this.stopToolStripMenuItem_Click);
			// 
			// removeToolStripMenuItem
			// 
			this.removeToolStripMenuItem.Image = global::Layton.AuditWizard.Network.Properties.Resources.remove16;
			this.removeToolStripMenuItem.Name = "removeToolStripMenuItem";
			this.removeToolStripMenuItem.Size = new System.Drawing.Size(189, 22);
			this.removeToolStripMenuItem.Text = "&Remove";
			this.removeToolStripMenuItem.ToolTipText = "Remove the AuditAgent Service from the selected PC(s)";
			this.removeToolStripMenuItem.Click += new System.EventHandler(this.removeToolStripMenuItem_Click);
			// 
			// checkStatusToolStripMenuItem
			// 
			this.checkStatusToolStripMenuItem.Image = global::Layton.AuditWizard.Network.Properties.Resources.checkstatus16;
			this.checkStatusToolStripMenuItem.Name = "checkStatusToolStripMenuItem";
			this.checkStatusToolStripMenuItem.Size = new System.Drawing.Size(189, 22);
			this.checkStatusToolStripMenuItem.Text = "Check &Status";
			this.checkStatusToolStripMenuItem.ToolTipText = "Check the Status of the AuditAgent Service on the selected PC(s)";
			this.checkStatusToolStripMenuItem.Click += new System.EventHandler(this.checkStatusToolStripMenuItem_Click);
			// 
			// viewLogFileToolStripMenuItem
			// 
			this.viewLogFileToolStripMenuItem.Image = global::Layton.AuditWizard.Network.Properties.Resources.viewlog16;
			this.viewLogFileToolStripMenuItem.Name = "viewLogFileToolStripMenuItem";
			this.viewLogFileToolStripMenuItem.Size = new System.Drawing.Size(189, 22);
			this.viewLogFileToolStripMenuItem.Text = "&View Log File";
			this.viewLogFileToolStripMenuItem.ToolTipText = "View the AuditAgent Service log file for the selected PC(s)";
			this.viewLogFileToolStripMenuItem.Click += new System.EventHandler(this.viewLogFileToolStripMenuItem_Click);
			// 
			// clearLogFileToolStripMenuItem
			// 
			this.clearLogFileToolStripMenuItem.Image = global::Layton.AuditWizard.Network.Properties.Resources.clearlog16;
			this.clearLogFileToolStripMenuItem.Name = "clearLogFileToolStripMenuItem";
			this.clearLogFileToolStripMenuItem.Size = new System.Drawing.Size(189, 22);
			this.clearLogFileToolStripMenuItem.Text = "&Clear Log File";
			this.clearLogFileToolStripMenuItem.ToolTipText = "Clear the AuditAgent Service log file on the selected PC(s)";
			this.clearLogFileToolStripMenuItem.Click += new System.EventHandler(this.clearLogFileToolStripMenuItem_Click);
			// 
			// auditNowToolStripMenuItem
			// 
			this.auditNowToolStripMenuItem.Image = global::Layton.AuditWizard.Network.Properties.Resources.request_reaudit_16;
			this.auditNowToolStripMenuItem.Name = "auditNowToolStripMenuItem";
			this.auditNowToolStripMenuItem.Size = new System.Drawing.Size(189, 22);
			this.auditNowToolStripMenuItem.Text = "&Audit Now";
			this.auditNowToolStripMenuItem.ToolTipText = "Request the AuditAgent Service to perfom an audit of the selected PC(s)";
			this.auditNowToolStripMenuItem.Click += new System.EventHandler(this.auditNowToolStripMenuItem_Click);
			// 
			// reAuditDeviceToolStripMenuItem
			// 
			this.reAuditDeviceToolStripMenuItem.Image = global::Layton.AuditWizard.Network.Properties.Resources.request_reaudit_16;
			this.reAuditDeviceToolStripMenuItem.Name = "reAuditDeviceToolStripMenuItem";
			this.reAuditDeviceToolStripMenuItem.Size = new System.Drawing.Size(174, 22);
			this.reAuditDeviceToolStripMenuItem.Text = "&Reaudit Asset";
			this.reAuditDeviceToolStripMenuItem.Click += new System.EventHandler(this.reAuditAssetToolStripMenuItem_Click);
			// 
			// NetworkExplorerView
			// 
			this.BackColor = System.Drawing.Color.White;
			this.Controls.Add(this.networkTree);
			this.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.Name = "NetworkExplorerView";
			this.Size = new System.Drawing.Size(217, 441);
			((System.ComponentModel.ISupportInitialize)(this.networkTree)).EndInit();
			this.networkMenuStrip.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion

		private Infragistics.Win.UltraWinTree.UltraTree networkTree;
		private System.Windows.Forms.ContextMenuStrip networkMenuStrip;
		private System.Windows.Forms.ToolStripMenuItem auditAgentToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem deployToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem startToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem stopToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem removeToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem checkStatusToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem viewLogFileToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem clearLogFileToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem auditNowToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem deleteComputersToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem newAssetToolStripMenuItem;
		//private System.Windows.Forms.ToolStripMenuItem assetPropertiesMenuItem;
		private System.Windows.Forms.ToolStripMenuItem assetToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem relocateByIPMenuItem;
		private System.Windows.Forms.ToolStripMenuItem remoteDesktopMenuItem;
        private System.Windows.Forms.ToolStripMenuItem findAssetToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem reAuditDeviceToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem statusToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem stockMenuItem;
		private System.Windows.Forms.ToolStripMenuItem inUseMenuItem;
		private System.Windows.Forms.ToolStripMenuItem pendingDisposalMenuItem;
		private System.Windows.Forms.ToolStripMenuItem disposedMenuItem;
		private System.Windows.Forms.ToolStripMenuItem updateConfigurationToolStripMenuItem;
	}
}
