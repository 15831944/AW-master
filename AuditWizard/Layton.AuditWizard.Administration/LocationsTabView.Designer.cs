namespace Layton.AuditWizard.Administration
{
	partial class LocationsTabView
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
            Infragistics.Win.UltraWinTree.Override _override1 = new Infragistics.Win.UltraWinTree.Override();
            Infragistics.Win.Appearance appearance3 = new Infragistics.Win.Appearance();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(LocationsTabView));
            Infragistics.Win.Appearance appearance2 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance1 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance14 = new Infragistics.Win.Appearance();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.locationsTree = new Infragistics.Win.UltraWinTree.UltraTree();
            this.cmsTvLocations = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.miNewLocation = new System.Windows.Forms.ToolStripMenuItem();
            this.miDeleteLocation = new System.Windows.Forms.ToolStripMenuItem();
            this.miLocationProperties = new System.Windows.Forms.ToolStripMenuItem();
            this.locationsList = new Infragistics.Win.UltraWinListView.UltraListView();
            this.cmsLvLocations = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.miLvNewItem = new System.Windows.Forms.ToolStripMenuItem();
            this.miLvDeleteItem = new System.Windows.Forms.ToolStripMenuItem();
            this.miLvProperties = new System.Windows.Forms.ToolStripMenuItem();
            this.headerLabel = new Infragistics.Win.Misc.UltraLabel();
            this.headerGroupBox = new Infragistics.Win.Misc.UltraGroupBox();
            this.dataColumn1 = new System.Data.DataColumn();
            this.dataColumn5 = new System.Data.DataColumn();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.locationsTree)).BeginInit();
            this.cmsTvLocations.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.locationsList)).BeginInit();
            this.cmsLvLocations.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.headerGroupBox)).BeginInit();
            this.headerGroupBox.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 80);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.locationsTree);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.locationsList);
            this.splitContainer1.Size = new System.Drawing.Size(817, 520);
            this.splitContainer1.SplitterDistance = 271;
            this.splitContainer1.SplitterWidth = 5;
            this.splitContainer1.TabIndex = 7;
            // 
            // locationsTree
            // 
            this.locationsTree.ContextMenuStrip = this.cmsTvLocations;
            this.locationsTree.DisplayStyle = Infragistics.Win.UltraWinTree.UltraTreeDisplayStyle.WindowsVista;
            this.locationsTree.Dock = System.Windows.Forms.DockStyle.Fill;
            this.locationsTree.FullRowSelect = true;
            this.locationsTree.HideSelection = false;
            this.locationsTree.Location = new System.Drawing.Point(0, 0);
            this.locationsTree.Name = "locationsTree";
            appearance3.Image = global::Layton.AuditWizard.Administration.Properties.Resources.location_16;
            _override1.NodeAppearance = appearance3;
            this.locationsTree.Override = _override1;
            this.locationsTree.Size = new System.Drawing.Size(271, 520);
            this.locationsTree.TabIndex = 0;
            this.locationsTree.AfterSelect += new Infragistics.Win.UltraWinTree.AfterNodeSelectEventHandler(this.locationsTree_AfterSelect);
            this.locationsTree.MouseDown += new System.Windows.Forms.MouseEventHandler(this.locationsTree_MouseDown);
            this.locationsTree.MouseUp += new System.Windows.Forms.MouseEventHandler(this.locationsTree_MouseUp);
            this.locationsTree.BeforeExpand += new Infragistics.Win.UltraWinTree.BeforeNodeChangedEventHandler(this.locationsTree_BeforeExpand);
            this.locationsTree.DoubleClick += new System.EventHandler(this.locationsTree_DoubleClick);
            // 
            // cmsTvLocations
            // 
            this.cmsTvLocations.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.miNewLocation,
            this.miDeleteLocation,
            this.miLocationProperties});
            this.cmsTvLocations.Name = "contextMenuStrip1";
            this.cmsTvLocations.Size = new System.Drawing.Size(160, 70);
            this.cmsTvLocations.Opening += new System.ComponentModel.CancelEventHandler(this.cmsTvLocations_Opening);
            // 
            // miNewLocation
            // 
            this.miNewLocation.Image = ((System.Drawing.Image)(resources.GetObject("miNewLocation.Image")));
            this.miNewLocation.Name = "miNewLocation";
            this.miNewLocation.Size = new System.Drawing.Size(159, 22);
            this.miNewLocation.Text = "&New Location";
            this.miNewLocation.Click += new System.EventHandler(this.miNewLocation_Click);
            // 
            // miDeleteLocation
            // 
            this.miDeleteLocation.Image = ((System.Drawing.Image)(resources.GetObject("miDeleteLocation.Image")));
            this.miDeleteLocation.Name = "miDeleteLocation";
            this.miDeleteLocation.Size = new System.Drawing.Size(159, 22);
            this.miDeleteLocation.Text = "&Delete Location";
            this.miDeleteLocation.Click += new System.EventHandler(this.miDeleteLocation_Click);
            // 
            // miLocationProperties
            // 
            this.miLocationProperties.Image = ((System.Drawing.Image)(resources.GetObject("miLocationProperties.Image")));
            this.miLocationProperties.Name = "miLocationProperties";
            this.miLocationProperties.Size = new System.Drawing.Size(159, 22);
            this.miLocationProperties.Text = "&Properties";
            this.miLocationProperties.Click += new System.EventHandler(this.miLocationProperties_Click);
            // 
            // locationsList
            // 
            appearance2.TextHAlignAsString = "Left";
            this.locationsList.Appearance = appearance2;
            this.locationsList.ContextMenuStrip = this.cmsLvLocations;
            this.locationsList.Dock = System.Windows.Forms.DockStyle.Fill;
            this.locationsList.ItemSettings.AllowEdit = Infragistics.Win.DefaultableBoolean.False;
            appearance1.Image = global::Layton.AuditWizard.Administration.Properties.Resources.location_16;
            this.locationsList.ItemSettings.Appearance = appearance1;
            this.locationsList.Location = new System.Drawing.Point(0, 0);
            this.locationsList.Name = "locationsList";
            this.locationsList.Size = new System.Drawing.Size(541, 520);
            this.locationsList.TabIndex = 0;
            this.locationsList.Text = "locations";
            this.locationsList.View = Infragistics.Win.UltraWinListView.UltraListViewStyle.Details;
            this.locationsList.KeyDown += new System.Windows.Forms.KeyEventHandler(this.ulvLocations_KeyDown);
            this.locationsList.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.locationsList_KeyPress);
            this.locationsList.ToolTipDisplaying += new Infragistics.Win.UltraWinListView.ToolTipDisplayingEventHandler(this.locationsList_ToolTipDisplaying);
            this.locationsList.ItemDoubleClick += new Infragistics.Win.UltraWinListView.ItemDoubleClickEventHandler(this.locationsList_ItemDoubleClick);
            this.locationsList.MouseDown += new System.Windows.Forms.MouseEventHandler(this.ulvUserData_MouseDown);
            // 
            // cmsLvLocations
            // 
            this.cmsLvLocations.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.miLvNewItem,
            this.miLvDeleteItem,
            this.miLvProperties});
            this.cmsLvLocations.Name = "contextMenuStrip1";
            this.cmsLvLocations.Size = new System.Drawing.Size(160, 70);
            this.cmsLvLocations.Opening += new System.ComponentModel.CancelEventHandler(this.cmsLvLocations_Opening);
            // 
            // miLvNewItem
            // 
            this.miLvNewItem.Image = ((System.Drawing.Image)(resources.GetObject("miLvNewItem.Image")));
            this.miLvNewItem.Name = "miLvNewItem";
            this.miLvNewItem.Size = new System.Drawing.Size(159, 22);
            this.miLvNewItem.Text = "&New Location";
            this.miLvNewItem.Click += new System.EventHandler(this.miLvNewItem_Click);
            // 
            // miLvDeleteItem
            // 
            this.miLvDeleteItem.Image = ((System.Drawing.Image)(resources.GetObject("miLvDeleteItem.Image")));
            this.miLvDeleteItem.Name = "miLvDeleteItem";
            this.miLvDeleteItem.Size = new System.Drawing.Size(159, 22);
            this.miLvDeleteItem.Text = "&Delete Location";
            this.miLvDeleteItem.Click += new System.EventHandler(this.miLvDeleteItem_Click);
            // 
            // miLvProperties
            // 
            this.miLvProperties.Image = ((System.Drawing.Image)(resources.GetObject("miLvProperties.Image")));
            this.miLvProperties.Name = "miLvProperties";
            this.miLvProperties.Size = new System.Drawing.Size(159, 22);
            this.miLvProperties.Text = "Properties";
            this.miLvProperties.Click += new System.EventHandler(this.miLvProperties_Click);
            // 
            // headerLabel
            // 
            this.headerLabel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            appearance14.BackColor = System.Drawing.Color.Transparent;
            appearance14.Image = global::Layton.AuditWizard.Administration.Properties.Resources.location_72;
            appearance14.ImageVAlign = Infragistics.Win.VAlign.Middle;
            appearance14.TextHAlignAsString = "Center";
            appearance14.TextVAlignAsString = "Middle";
            this.headerLabel.Appearance = appearance14;
            this.headerLabel.AutoSize = true;
            this.headerLabel.Font = new System.Drawing.Font("Verdana", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.headerLabel.ImageSize = new System.Drawing.Size(72, 72);
            this.headerLabel.Location = new System.Drawing.Point(7, 5);
            this.headerLabel.Name = "headerLabel";
            this.headerLabel.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.headerLabel.Size = new System.Drawing.Size(241, 72);
            this.headerLabel.TabIndex = 5;
            this.headerLabel.Text = "User Locations";
            // 
            // headerGroupBox
            // 
            this.headerGroupBox.Controls.Add(this.headerLabel);
            this.headerGroupBox.Dock = System.Windows.Forms.DockStyle.Top;
            this.headerGroupBox.Location = new System.Drawing.Point(0, 0);
            this.headerGroupBox.Name = "headerGroupBox";
            this.headerGroupBox.Size = new System.Drawing.Size(817, 80);
            this.headerGroupBox.TabIndex = 6;
            this.headerGroupBox.ViewStyle = Infragistics.Win.Misc.GroupBoxViewStyle.Office2007;
            // 
            // dataColumn1
            // 
            this.dataColumn1.ColumnName = "dataobject";
            this.dataColumn1.DataType = typeof(object);
            // 
            // dataColumn5
            // 
            this.dataColumn5.ColumnName = "dataobject";
            this.dataColumn5.DataType = typeof(object);
            // 
            // LocationsTabView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.headerGroupBox);
            this.Font = new System.Drawing.Font("Verdana", 8.25F);
            this.Name = "LocationsTabView";
            this.Size = new System.Drawing.Size(817, 600);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.locationsTree)).EndInit();
            this.cmsTvLocations.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.locationsList)).EndInit();
            this.cmsLvLocations.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.headerGroupBox)).EndInit();
            this.headerGroupBox.ResumeLayout(false);
            this.headerGroupBox.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

		private Infragistics.Win.Misc.UltraLabel headerLabel;
		private Infragistics.Win.Misc.UltraGroupBox headerGroupBox; 
		private System.Data.DataColumn dataColumn1;
		private System.Data.DataColumn dataColumn5;
		private System.Windows.Forms.ContextMenuStrip cmsTvLocations;
		private System.Windows.Forms.ToolStripMenuItem miNewLocation;
		private System.Windows.Forms.ToolStripMenuItem miDeleteLocation;
		private System.Windows.Forms.ToolStripMenuItem miLocationProperties;
		private System.Windows.Forms.ContextMenuStrip cmsLvLocations;
		private System.Windows.Forms.ToolStripMenuItem miLvNewItem;
		private System.Windows.Forms.ToolStripMenuItem miLvDeleteItem;
		private System.Windows.Forms.SplitContainer splitContainer1;
		private Infragistics.Win.UltraWinTree.UltraTree locationsTree;
		private Infragistics.Win.UltraWinListView.UltraListView locationsList;
		private System.Windows.Forms.ToolStripMenuItem miLvProperties;
    }
}
