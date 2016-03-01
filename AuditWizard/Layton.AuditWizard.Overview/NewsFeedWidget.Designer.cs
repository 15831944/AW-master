using Infragistics.Win.UltraWinGrid;

namespace Layton.AuditWizard.Overview
{
    partial class NewsFeedWidget
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

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
            this.components = new System.ComponentModel.Container();
            Infragistics.Win.Appearance lGridAppearance = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance1 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance lHeaderAppearance = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance lRowAppearance = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance lSelectedRowAppearance = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinScrollBar.ScrollBarLook scrollBarLook1 = new Infragistics.Win.UltraWinScrollBar.ScrollBarLook();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.bnEditNewsFeed = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.ultraGridNewsFeed = new Infragistics.Win.UltraWinGrid.UltraGrid();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ultraGridNewsFeed)).BeginInit();
            this.SuspendLayout();
            // 
            // splitContainer1
            // 
            this.splitContainer1.BackColor = System.Drawing.Color.White;
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.IsSplitterFixed = true;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.BackColor = System.Drawing.Color.NavajoWhite;
            this.splitContainer1.Panel1.Controls.Add(this.bnEditNewsFeed);
            this.splitContainer1.Panel1.Controls.Add(this.label1);
            this.splitContainer1.Panel1.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.splitContainer1.Panel1MinSize = 19;
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.BackColor = System.Drawing.Color.White;
            this.splitContainer1.Panel2.Controls.Add(this.ultraGridNewsFeed);
            this.splitContainer1.Size = new System.Drawing.Size(297, 319);
            this.splitContainer1.SplitterDistance = 21;
            this.splitContainer1.TabIndex = 0;
            // 
            // bnEditNewsFeed
            // 
            this.bnEditNewsFeed.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.bnEditNewsFeed.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.bnEditNewsFeed.BackColor = System.Drawing.Color.NavajoWhite;
            this.bnEditNewsFeed.BackgroundImage = global::Layton.AuditWizard.Overview.Properties.Resources.process_info;
            this.bnEditNewsFeed.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.bnEditNewsFeed.FlatAppearance.BorderSize = 0;
            this.bnEditNewsFeed.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.bnEditNewsFeed.ForeColor = System.Drawing.Color.Transparent;
            this.bnEditNewsFeed.Location = new System.Drawing.Point(271, 3);
            this.bnEditNewsFeed.Name = "bnEditNewsFeed";
            this.bnEditNewsFeed.Size = new System.Drawing.Size(24, 17);
            this.bnEditNewsFeed.TabIndex = 9;
            this.toolTip1.SetToolTip(this.bnEditNewsFeed, "Configure News Feed Settings");
            this.bnEditNewsFeed.UseVisualStyleBackColor = false;
            this.bnEditNewsFeed.Click += new System.EventHandler(this.bnEditNewsFeed_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.BackColor = System.Drawing.Color.NavajoWhite;
            this.label1.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(3, 4);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(185, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "Live Application News Feed";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // ultraGridNewsFeed
            // 
            lGridAppearance.BackColor = System.Drawing.Color.White;
            this.ultraGridNewsFeed.DisplayLayout.Appearance = lGridAppearance;
            this.ultraGridNewsFeed.DisplayLayout.AutoFitStyle = Infragistics.Win.UltraWinGrid.AutoFitStyle.ExtendLastColumn;
            this.ultraGridNewsFeed.DisplayLayout.Override.AllowColMoving = Infragistics.Win.UltraWinGrid.AllowColMoving.NotAllowed;
            this.ultraGridNewsFeed.DisplayLayout.Override.AllowColSwapping = Infragistics.Win.UltraWinGrid.AllowColSwapping.NotAllowed;
            this.ultraGridNewsFeed.DisplayLayout.Override.AllowRowFiltering = Infragistics.Win.DefaultableBoolean.True;
            this.ultraGridNewsFeed.DisplayLayout.Override.AllowRowSummaries = Infragistics.Win.UltraWinGrid.AllowRowSummaries.False;
            this.ultraGridNewsFeed.DisplayLayout.Override.AllowUpdate = Infragistics.Win.DefaultableBoolean.False;
            this.ultraGridNewsFeed.DisplayLayout.Override.BorderStyleCell = Infragistics.Win.UIElementBorderStyle.None;
            this.ultraGridNewsFeed.DisplayLayout.Override.CellPadding = 3;
            appearance1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(246)))), ((int)(((byte)(252)))), ((int)(((byte)(245)))));
            this.ultraGridNewsFeed.DisplayLayout.Override.FilterRowAppearance = appearance1;
            this.ultraGridNewsFeed.DisplayLayout.Override.FilterUIType = Infragistics.Win.UltraWinGrid.FilterUIType.FilterRow;
            lHeaderAppearance.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(209)))), ((int)(((byte)(26)))));
            this.ultraGridNewsFeed.DisplayLayout.Override.HeaderAppearance = lHeaderAppearance;
            this.ultraGridNewsFeed.DisplayLayout.Override.HeaderClickAction = Infragistics.Win.UltraWinGrid.HeaderClickAction.SortMulti;
            this.ultraGridNewsFeed.DisplayLayout.Override.MergedCellStyle = Infragistics.Win.UltraWinGrid.MergedCellStyle.Never;
            lRowAppearance.BackColor = System.Drawing.Color.White;
            lRowAppearance.BorderAlpha = Infragistics.Win.Alpha.Transparent;
            lRowAppearance.BorderColor = System.Drawing.Color.White;
            lRowAppearance.ForeColor = System.Drawing.Color.DimGray;
            this.ultraGridNewsFeed.DisplayLayout.Override.RowAppearance = lRowAppearance;
            this.ultraGridNewsFeed.DisplayLayout.Override.RowSelectors = Infragistics.Win.DefaultableBoolean.True;
            lSelectedRowAppearance.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(238)))), ((int)(((byte)(243)))), ((int)(((byte)(223)))));
            lSelectedRowAppearance.ForeColor = System.Drawing.Color.Black;
            this.ultraGridNewsFeed.DisplayLayout.Override.SelectedRowAppearance = lSelectedRowAppearance;
            this.ultraGridNewsFeed.DisplayLayout.Override.SelectTypeCol = Infragistics.Win.UltraWinGrid.SelectType.None;
            this.ultraGridNewsFeed.DisplayLayout.Override.SelectTypeRow = Infragistics.Win.UltraWinGrid.SelectType.Single;
            this.ultraGridNewsFeed.DisplayLayout.RowConnectorStyle = Infragistics.Win.UltraWinGrid.RowConnectorStyle.None;
            scrollBarLook1.ViewStyle = Infragistics.Win.UltraWinScrollBar.ScrollBarViewStyle.Outlook2007;
            this.ultraGridNewsFeed.DisplayLayout.ScrollBarLook = scrollBarLook1;
            this.ultraGridNewsFeed.DisplayLayout.ScrollBounds = Infragistics.Win.UltraWinGrid.ScrollBounds.ScrollToFill;
            this.ultraGridNewsFeed.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ultraGridNewsFeed.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ultraGridNewsFeed.Location = new System.Drawing.Point(0, 0);
            this.ultraGridNewsFeed.Name = "ultraGridNewsFeed";
            this.ultraGridNewsFeed.Size = new System.Drawing.Size(297, 294);
            this.ultraGridNewsFeed.TabIndex = 0;
            // 
            // NewsFeedWidget
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 14F);
            this.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.Controls.Add(this.splitContainer1);
            this.Font = new System.Drawing.Font("Calibri", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Name = "NewsFeedWidget";
            this.Size = new System.Drawing.Size(297, 319);
            this.VisibleChanged += new System.EventHandler(this.NewsFeedWidget_VisibleChanged);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel1.PerformLayout();
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.ultraGridNewsFeed)).EndInit();
            this.ResumeLayout(false);

		}

		#endregion

        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.Label label1;
        private Infragistics.Win.UltraWinGrid.UltraGrid ultraGridNewsFeed;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.Button bnEditNewsFeed;







    }
}
