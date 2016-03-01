namespace Layton.Deviceshield.Deployment
{
    partial class DeploymentExplorerView
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
            Infragistics.Win.UltraWinScrollBar.ScrollBarLook scrollBarLook1 = new Infragistics.Win.UltraWinScrollBar.ScrollBarLook();
            this.deploymentTree = new Infragistics.Win.UltraWinTree.UltraTree();
            this.deploymentTreeMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.deployToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.startToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.stopToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.removeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.checkStatusMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.viewClientLogToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.clearClientLogToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            ((System.ComponentModel.ISupportInitialize)(this.deploymentTree)).BeginInit();
            this.deploymentTreeMenu.SuspendLayout();
            this.SuspendLayout();
            // 
            // deploymentTree
            // 
            this.deploymentTree.AllowDrop = true;
            appearance1.BackColor = System.Drawing.Color.Transparent;
            this.deploymentTree.Appearance = appearance1;
            this.deploymentTree.ContextMenuStrip = this.deploymentTreeMenu;
            this.deploymentTree.Dock = System.Windows.Forms.DockStyle.Fill;
            this.deploymentTree.HideSelection = false;
            this.deploymentTree.Location = new System.Drawing.Point(0, 0);
            this.deploymentTree.Name = "deploymentTree";
            scrollBarLook1.ViewStyle = Infragistics.Win.UltraWinScrollBar.ScrollBarViewStyle.Office2007;
            this.deploymentTree.ScrollBarLook = scrollBarLook1;
            this.deploymentTree.Size = new System.Drawing.Size(209, 312);
            this.deploymentTree.TabIndex = 0;
            this.deploymentTree.AfterSelect += new Infragistics.Win.UltraWinTree.AfterNodeSelectEventHandler(this.deploymentTree_AfterSelect);
            this.deploymentTree.SelectionDragStart += new System.EventHandler(this.deploymentTree_SelectionDragStart);
            this.deploymentTree.MouseDown += new System.Windows.Forms.MouseEventHandler(this.deploymentTree_MouseDown);
            this.deploymentTree.DragOver += new System.Windows.Forms.DragEventHandler(this.deploymentTree_DragOver);
            this.deploymentTree.DragDrop += new System.Windows.Forms.DragEventHandler(this.deploymentTree_DragDrop);
            // 
            // deploymentTreeMenu
            // 
            this.deploymentTreeMenu.BackColor = System.Drawing.Color.White;
            this.deploymentTreeMenu.Font = new System.Drawing.Font("Tahoma", 8.25F);
            this.deploymentTreeMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.deployToolStripMenuItem,
            this.startToolStripMenuItem,
            this.stopToolStripMenuItem,
            this.removeToolStripMenuItem,
            this.checkStatusMenuItem,
            this.toolStripSeparator1,
            this.viewClientLogToolStripMenuItem,
            this.clearClientLogToolStripMenuItem});
            this.deploymentTreeMenu.Name = "deploymentTreeMenu";
            this.deploymentTreeMenu.ShowItemToolTips = false;
            this.deploymentTreeMenu.Size = new System.Drawing.Size(161, 164);
            // 
            // deployToolStripMenuItem
            // 
            this.deployToolStripMenuItem.Image = global::Layton.Deviceshield.Deployment.Properties.Resources.deploy16;
            this.deployToolStripMenuItem.Name = "deployToolStripMenuItem";
            this.deployToolStripMenuItem.Size = new System.Drawing.Size(160, 22);
            this.deployToolStripMenuItem.Text = "&Deploy...";
            this.deployToolStripMenuItem.ToolTipText = "Installs the Client on the Selected Computers";
            this.deployToolStripMenuItem.Click += new System.EventHandler(this.deployToolStripMenuItem_Click);
            // 
            // startToolStripMenuItem
            // 
            this.startToolStripMenuItem.Image = global::Layton.Deviceshield.Deployment.Properties.Resources.start16;
            this.startToolStripMenuItem.Name = "startToolStripMenuItem";
            this.startToolStripMenuItem.Size = new System.Drawing.Size(160, 22);
            this.startToolStripMenuItem.Text = "&Start...";
            this.startToolStripMenuItem.ToolTipText = "Starts the Client on the Selected Remote Computers";
            this.startToolStripMenuItem.Click += new System.EventHandler(this.startToolStripMenuItem_Click);
            // 
            // stopToolStripMenuItem
            // 
            this.stopToolStripMenuItem.Image = global::Layton.Deviceshield.Deployment.Properties.Resources.stop16;
            this.stopToolStripMenuItem.Name = "stopToolStripMenuItem";
            this.stopToolStripMenuItem.Size = new System.Drawing.Size(160, 22);
            this.stopToolStripMenuItem.Text = "&Stop...";
            this.stopToolStripMenuItem.ToolTipText = "Stops the Client on the Selected Remote Computers";
            this.stopToolStripMenuItem.Click += new System.EventHandler(this.stopToolStripMenuItem_Click);
            // 
            // removeToolStripMenuItem
            // 
            this.removeToolStripMenuItem.Image = global::Layton.Deviceshield.Deployment.Properties.Resources.remove16;
            this.removeToolStripMenuItem.Name = "removeToolStripMenuItem";
            this.removeToolStripMenuItem.Size = new System.Drawing.Size(160, 22);
            this.removeToolStripMenuItem.Text = "&Remove...";
            this.removeToolStripMenuItem.ToolTipText = "Removes the Client from the Selected Computers";
            this.removeToolStripMenuItem.Click += new System.EventHandler(this.removeToolStripMenuItem_Click);
            // 
            // checkStatusMenuItem
            // 
            this.checkStatusMenuItem.Image = global::Layton.Deviceshield.Deployment.Properties.Resources.checkstatus16;
            this.checkStatusMenuItem.Name = "checkStatusMenuItem";
            this.checkStatusMenuItem.Size = new System.Drawing.Size(160, 22);
            this.checkStatusMenuItem.Text = "&Check Status";
            this.checkStatusMenuItem.Click += new System.EventHandler(this.checkStatusMenuItem_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(157, 6);
            // 
            // viewClientLogToolStripMenuItem
            // 
            this.viewClientLogToolStripMenuItem.Image = global::Layton.Deviceshield.Deployment.Properties.Resources.viewlog16;
            this.viewClientLogToolStripMenuItem.Name = "viewClientLogToolStripMenuItem";
            this.viewClientLogToolStripMenuItem.Size = new System.Drawing.Size(160, 22);
            this.viewClientLogToolStripMenuItem.Text = "&View Client Log";
            this.viewClientLogToolStripMenuItem.ToolTipText = "Opens the Client Log File on the Selected Computer";
            this.viewClientLogToolStripMenuItem.Click += new System.EventHandler(this.viewClientLogToolStripMenuItem_Click);
            // 
            // clearClientLogToolStripMenuItem
            // 
            this.clearClientLogToolStripMenuItem.Image = global::Layton.Deviceshield.Deployment.Properties.Resources.clearlog16;
            this.clearClientLogToolStripMenuItem.Name = "clearClientLogToolStripMenuItem";
            this.clearClientLogToolStripMenuItem.Size = new System.Drawing.Size(160, 22);
            this.clearClientLogToolStripMenuItem.Text = "&Clear Client Log";
            this.clearClientLogToolStripMenuItem.ToolTipText = "Deletes the Client Log File on the Selected Computer";
            this.clearClientLogToolStripMenuItem.Click += new System.EventHandler(this.clearClientLogToolStripMenuItem_Click);
            // 
            // DeploymentExplorerView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.Controls.Add(this.deploymentTree);
            this.Name = "DeploymentExplorerView";
            this.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.Size = new System.Drawing.Size(209, 312);
            ((System.ComponentModel.ISupportInitialize)(this.deploymentTree)).EndInit();
            this.deploymentTreeMenu.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private Infragistics.Win.UltraWinTree.UltraTree deploymentTree;
        private System.Windows.Forms.ContextMenuStrip deploymentTreeMenu;
        private System.Windows.Forms.ToolStripMenuItem deployToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem startToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem stopToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem removeToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem viewClientLogToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem clearClientLogToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem checkStatusMenuItem;
    }
}
