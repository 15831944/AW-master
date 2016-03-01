namespace Layton.AuditWizard.Applications
{
    partial class ApplicationsExplorerView
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
            this.applicationsTree = new Infragistics.Win.UltraWinTree.UltraTree();
            this.applicationsTreeMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.editPublisherToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ignoreAppToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.includeAppToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.aliasToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.newApplicationToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.deleteApplicationToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.newlicenseToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.editLicenseToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.propertiesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.hideAppTtoolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem2 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem3 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem4 = new System.Windows.Forms.ToolStripMenuItem();
            ((System.ComponentModel.ISupportInitialize)(this.applicationsTree)).BeginInit();
            this.applicationsTreeMenu.SuspendLayout();
            this.SuspendLayout();
            // 
            // applicationsTree
            // 
            this.applicationsTree.AllowDrop = true;
            appearance1.BackColor = System.Drawing.Color.Transparent;
            this.applicationsTree.Appearance = appearance1;
            this.applicationsTree.ContextMenuStrip = this.applicationsTreeMenu;
            this.applicationsTree.Dock = System.Windows.Forms.DockStyle.Fill;
            this.applicationsTree.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.applicationsTree.HideSelection = false;
            this.applicationsTree.Location = new System.Drawing.Point(0, 0);
            this.applicationsTree.Name = "applicationsTree";
            _override1.SelectionType = Infragistics.Win.UltraWinTree.SelectType.Extended;
            _override1.Sort = Infragistics.Win.UltraWinTree.SortType.None;
            this.applicationsTree.Override = _override1;
            scrollBarLook1.ViewStyle = Infragistics.Win.UltraWinScrollBar.ScrollBarViewStyle.Outlook2007;
            this.applicationsTree.ScrollBarLook = scrollBarLook1;
            this.applicationsTree.ScrollBounds = Infragistics.Win.UltraWinTree.ScrollBounds.ScrollToFill;
            this.applicationsTree.Size = new System.Drawing.Size(278, 418);
            this.applicationsTree.TabIndex = 2;
            this.applicationsTree.AfterSelect += new Infragistics.Win.UltraWinTree.AfterNodeSelectEventHandler(this.applicationTree_AfterSelect);
            this.applicationsTree.SelectionDragStart += new System.EventHandler(this.applicationsTree_SelectionDragStart);
            this.applicationsTree.DragLeave += new System.EventHandler(this.applicationsTree_DragLeave);
            this.applicationsTree.MouseDown += new System.Windows.Forms.MouseEventHandler(this.applicationsTree_MouseDown);
            this.applicationsTree.QueryContinueDrag += new System.Windows.Forms.QueryContinueDragEventHandler(this.applicationsTree_QueryContinueDrag);
            this.applicationsTree.BeforeExpand += new Infragistics.Win.UltraWinTree.BeforeNodeChangedEventHandler(this.applicationsTree_BeforeExpand);
            this.applicationsTree.DragOver += new System.Windows.Forms.DragEventHandler(this.applicationsTree_DragOver);
            this.applicationsTree.DragDrop += new System.Windows.Forms.DragEventHandler(this.applicationsTree_DragDrop);
            // 
            // applicationsTreeMenu
            // 
            this.applicationsTreeMenu.BackColor = System.Drawing.Color.White;
            this.applicationsTreeMenu.Font = new System.Drawing.Font("Tahoma", 8.25F);
            this.applicationsTreeMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.editPublisherToolStripMenuItem,
            this.deleteApplicationToolStripMenuItem,
            this.ignoreAppToolStripMenuItem,
            this.includeAppToolStripMenuItem,
            this.aliasToolStripMenuItem,
            this.newApplicationToolStripMenuItem,
            this.toolStripSeparator1,
            this.newlicenseToolStripMenuItem,
            this.editLicenseToolStripMenuItem,
            this.propertiesToolStripMenuItem});
            this.applicationsTreeMenu.Name = "deploymentTreeMenu";
            this.applicationsTreeMenu.ShowItemToolTips = false;
            this.applicationsTreeMenu.Size = new System.Drawing.Size(185, 230);
            this.applicationsTreeMenu.Opening += new System.ComponentModel.CancelEventHandler(this.applicationsTreeMenu_Opening);
            // 
            // editPublisherToolStripMenuItem
            // 
            this.editPublisherToolStripMenuItem.Image = global::Layton.AuditWizard.Applications.Properties.Resources.application_publisher_16;
            this.editPublisherToolStripMenuItem.Name = "editPublisherToolStripMenuItem";
            this.editPublisherToolStripMenuItem.Size = new System.Drawing.Size(184, 22);
            this.editPublisherToolStripMenuItem.Text = "Edit Publisher";
            this.editPublisherToolStripMenuItem.Click += new System.EventHandler(this.editPublisherToolStripMenuItem_Click);
            // 
            // ignoreAppToolStripMenuItem
            // 
            this.ignoreAppToolStripMenuItem.Image = global::Layton.AuditWizard.Applications.Properties.Resources.hide_application_16;
            this.ignoreAppToolStripMenuItem.Name = "ignoreAppToolStripMenuItem";
            this.ignoreAppToolStripMenuItem.Size = new System.Drawing.Size(184, 22);
            this.ignoreAppToolStripMenuItem.Text = "Ignore";
            this.ignoreAppToolStripMenuItem.ToolTipText = "Flag the selected application(s) to be ignored within AuditWizard";
            this.ignoreAppToolStripMenuItem.Click += new System.EventHandler(this.IgnoreToolStripMenuItem_Click);
            // 
            // includeAppToolStripMenuItem
            // 
            this.includeAppToolStripMenuItem.Image = global::Layton.AuditWizard.Applications.Properties.Resources.show_application_16;
            this.includeAppToolStripMenuItem.Name = "includeAppToolStripMenuItem";
            this.includeAppToolStripMenuItem.Size = new System.Drawing.Size(184, 22);
            this.includeAppToolStripMenuItem.Text = "Do Not Ignore";
            this.includeAppToolStripMenuItem.ToolTipText = "Do not ignore the selected application(s) within AuditWizard";
            this.includeAppToolStripMenuItem.Click += new System.EventHandler(this.IncludeToolStripMenuItem_Click);
            // 
            // aliasToolStripMenuItem
            // 
            this.aliasToolStripMenuItem.Image = global::Layton.AuditWizard.Applications.Properties.Resources.application_16;
            this.aliasToolStripMenuItem.Name = "aliasToolStripMenuItem";
            this.aliasToolStripMenuItem.Size = new System.Drawing.Size(184, 22);
            this.aliasToolStripMenuItem.Text = "Alias Application(s)";
            this.aliasToolStripMenuItem.Click += new System.EventHandler(this.aliasToolStripMenuItem_Click);
            // 
            // newApplicationToolStripMenuItem
            // 
            this.newApplicationToolStripMenuItem.Image = global::Layton.AuditWizard.Applications.Properties.Resources.application_add_16;
            this.newApplicationToolStripMenuItem.Name = "newApplicationToolStripMenuItem";
            this.newApplicationToolStripMenuItem.Size = new System.Drawing.Size(184, 22);
            this.newApplicationToolStripMenuItem.Text = "&New Application";
            this.newApplicationToolStripMenuItem.Click += new System.EventHandler(this.newApplicationToolStripMenuItem_Click);
            // 
            // deleteApplicationToolStripMenuItem
            // 
            this.deleteApplicationToolStripMenuItem.Image = global::Layton.AuditWizard.Applications.Properties.Resources.application_del_16;
            this.deleteApplicationToolStripMenuItem.Name = "deleteApplicationToolStripMenuItem";
            this.deleteApplicationToolStripMenuItem.Size = new System.Drawing.Size(184, 22);
            this.deleteApplicationToolStripMenuItem.Text = "&Delete Application(s)";
            this.deleteApplicationToolStripMenuItem.Click += new System.EventHandler(this.deleteApplicationToolStripMenuItem_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(181, 6);
            // 
            // newlicenseToolStripMenuItem
            // 
            this.newlicenseToolStripMenuItem.Image = global::Layton.AuditWizard.Applications.Properties.Resources.license_add_16;
            this.newlicenseToolStripMenuItem.Name = "newlicenseToolStripMenuItem";
            this.newlicenseToolStripMenuItem.Size = new System.Drawing.Size(184, 22);
            this.newlicenseToolStripMenuItem.Text = "&New License...";
            this.newlicenseToolStripMenuItem.ToolTipText = "Creates a new License for the selected application";
            this.newlicenseToolStripMenuItem.Click += new System.EventHandler(this.newlicenseToolStripMenuItem_Click);
            // 
            // editLicenseToolStripMenuItem
            // 
            this.editLicenseToolStripMenuItem.Image = global::Layton.AuditWizard.Applications.Properties.Resources.license_edit_16;
            this.editLicenseToolStripMenuItem.Name = "editLicenseToolStripMenuItem";
            this.editLicenseToolStripMenuItem.Size = new System.Drawing.Size(184, 22);
            this.editLicenseToolStripMenuItem.Text = "&Edit License...";
            this.editLicenseToolStripMenuItem.ToolTipText = "Edits the definition of the currently selected application license";
            this.editLicenseToolStripMenuItem.Click += new System.EventHandler(this.editLicenseToolStripMenuItem_Click);
            // 
            // propertiesToolStripMenuItem
            // 
            this.propertiesToolStripMenuItem.Name = "propertiesToolStripMenuItem";
            this.propertiesToolStripMenuItem.Size = new System.Drawing.Size(184, 22);
            this.propertiesToolStripMenuItem.Text = "&Properties";
            this.propertiesToolStripMenuItem.Click += new System.EventHandler(this.propertiesToolStripMenuItem_Click);
            // 
            // hideAppTtoolStripMenuItem
            // 
            this.hideAppTtoolStripMenuItem.Name = "hideAppTtoolStripMenuItem";
            this.hideAppTtoolStripMenuItem.Size = new System.Drawing.Size(179, 22);
            this.hideAppTtoolStripMenuItem.Text = "toolStripMenuItem1";
            // 
            // toolStripMenuItem2
            // 
            this.toolStripMenuItem2.Name = "toolStripMenuItem2";
            this.toolStripMenuItem2.Size = new System.Drawing.Size(179, 22);
            this.toolStripMenuItem2.Text = "toolStripMenuItem2";
            // 
            // toolStripMenuItem3
            // 
            this.toolStripMenuItem3.Name = "toolStripMenuItem3";
            this.toolStripMenuItem3.Size = new System.Drawing.Size(179, 22);
            this.toolStripMenuItem3.Text = "toolStripMenuItem3";
            // 
            // toolStripMenuItem4
            // 
            this.toolStripMenuItem4.Name = "toolStripMenuItem4";
            this.toolStripMenuItem4.Size = new System.Drawing.Size(179, 22);
            this.toolStripMenuItem4.Text = "toolStripMenuItem4";
            // 
            // ApplicationsExplorerView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.Controls.Add(this.applicationsTree);
            this.Font = new System.Drawing.Font("Verdana", 8.25F);
            this.Name = "ApplicationsExplorerView";
            this.Size = new System.Drawing.Size(278, 418);
            ((System.ComponentModel.ISupportInitialize)(this.applicationsTree)).EndInit();
            this.applicationsTreeMenu.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

		private Infragistics.Win.UltraWinTree.UltraTree applicationsTree;
		private System.Windows.Forms.ContextMenuStrip applicationsTreeMenu;
		private System.Windows.Forms.ToolStripMenuItem ignoreAppToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem includeAppToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem newlicenseToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem editLicenseToolStripMenuItem;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
		private System.Windows.Forms.ToolStripMenuItem hideAppTtoolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem2;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem3;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem4;
		private System.Windows.Forms.ToolStripMenuItem propertiesToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem newApplicationToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem aliasToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem deleteApplicationToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem editPublisherToolStripMenuItem;
    }
}
