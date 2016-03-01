namespace Layton.AuditWizard.Network
{
    partial class ComputerTabView
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
            Infragistics.Win.Appearance appearance1 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance2 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance3 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinListView.UltraListViewSubItemColumn ultraListViewSubItemColumn1 = new Infragistics.Win.UltraWinListView.UltraListViewSubItemColumn("");
            Infragistics.Win.Appearance appearance4 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance5 = new Infragistics.Win.Appearance();
            this.ultraTabPageControl2 = new Infragistics.Win.UltraWinTabControl.UltraTabPageControl();
            this.assetListView = new Infragistics.Win.UltraWinListView.UltraListView();
            this.contextMenu1 = new Infragistics.Win.IGControls.IGContextMenu();
            ((System.ComponentModel.ISupportInitialize)(this.assetListView)).BeginInit();
            this.SuspendLayout();
            // 
            // ultraTabPageControl2
            // 
            this.ultraTabPageControl2.Location = new System.Drawing.Point(1, 23);
            this.ultraTabPageControl2.Name = "ultraTabPageControl2";
            this.ultraTabPageControl2.Size = new System.Drawing.Size(710, 354);
            // 
            // assetListView
            // 
            appearance1.BackColor = System.Drawing.Color.White;
            appearance1.BackColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(190)))), ((int)(((byte)(216)))), ((int)(((byte)(255)))));
            appearance1.BackGradientStyle = Infragistics.Win.GradientStyle.Circular;
            this.assetListView.Appearance = appearance1;
            this.assetListView.BorderStyle = Infragistics.Win.UIElementBorderStyle.None;
            this.assetListView.Dock = System.Windows.Forms.DockStyle.Fill;
            appearance2.FontData.Name = "Verdana";
            appearance2.FontData.SizeInPoints = 10F;
            appearance2.ForeColor = System.Drawing.Color.Navy;
            this.assetListView.GroupAppearance = appearance2;
            this.assetListView.GroupHeaderMargins.Bottom = 4;
            this.assetListView.GroupHeaderMargins.Top = 8;
            this.assetListView.GroupHeadersVisible = Infragistics.Win.DefaultableBoolean.True;
            this.assetListView.ItemSettings.AllowEdit = Infragistics.Win.DefaultableBoolean.False;
            this.assetListView.ItemSettings.HideSelection = false;
            appearance3.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(216)))), ((int)(((byte)(228)))), ((int)(((byte)(248)))));
            appearance3.FontData.BoldAsString = "True";
            appearance3.FontData.SizeInPoints = 10F;
            appearance3.FontData.UnderlineAsString = "False";
            appearance3.TextTrimming = Infragistics.Win.TextTrimming.None;
            this.assetListView.ItemSettings.HotTrackingAppearance = appearance3;
            this.assetListView.ItemSettings.SubItemsVisibleInToolTipByDefault = true;
            this.assetListView.Location = new System.Drawing.Point(0, 0);
            this.assetListView.MainColumn.AllowMoving = Infragistics.Win.DefaultableBoolean.False;
            this.assetListView.MainColumn.AllowSorting = Infragistics.Win.DefaultableBoolean.False;
            this.assetListView.MainColumn.DataType = typeof(string);
            this.assetListView.MainColumn.Key = "AuditedItem";
            this.assetListView.MainColumn.ShowSortIndicators = Infragistics.Win.DefaultableBoolean.False;
            this.assetListView.MainColumn.Text = "AuditedItem";
            this.assetListView.MainColumn.VisiblePositionInDetailsView = 1;
            this.assetListView.Name = "assetListView";
            this.assetListView.ScrollBarLook.ViewStyle = Infragistics.Win.UltraWinScrollBar.ScrollBarViewStyle.Office2007;
            this.assetListView.Size = new System.Drawing.Size(833, 380);
            ultraListViewSubItemColumn1.AllowMoving = Infragistics.Win.DefaultableBoolean.False;
            ultraListViewSubItemColumn1.AllowSizing = Infragistics.Win.DefaultableBoolean.False;
            ultraListViewSubItemColumn1.AllowSorting = Infragistics.Win.DefaultableBoolean.False;
            appearance4.BorderColor = System.Drawing.Color.Transparent;
            appearance4.BorderColor2 = System.Drawing.Color.Transparent;
            ultraListViewSubItemColumn1.HeaderAppearance = appearance4;
            ultraListViewSubItemColumn1.VisibleInDetailsView = Infragistics.Win.DefaultableBoolean.True;
            ultraListViewSubItemColumn1.VisibleInToolTip = Infragistics.Win.DefaultableBoolean.False;
            ultraListViewSubItemColumn1.VisiblePositionInDetailsView = 0;
            ultraListViewSubItemColumn1.Width = 32;
            this.assetListView.SubItemColumns.AddRange(new Infragistics.Win.UltraWinListView.UltraListViewSubItemColumn[] {
            ultraListViewSubItemColumn1});
            this.assetListView.TabIndex = 1;
            this.assetListView.Text = "rulesListView";
            this.assetListView.UseFlatMode = Infragistics.Win.DefaultableBoolean.True;
            this.assetListView.UseOsThemes = Infragistics.Win.DefaultableBoolean.False;
            this.assetListView.View = Infragistics.Win.UltraWinListView.UltraListViewStyle.Tiles;
            appearance5.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(216)))), ((int)(((byte)(228)))), ((int)(((byte)(248)))));
            appearance5.BackColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(157)))), ((int)(((byte)(185)))), ((int)(((byte)(235)))));
            appearance5.BackGradientStyle = Infragistics.Win.GradientStyle.GlassBottom50;
            appearance5.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(149)))), ((int)(((byte)(147)))), ((int)(((byte)(162)))));
            appearance5.BorderColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(149)))), ((int)(((byte)(147)))), ((int)(((byte)(162)))));
            appearance5.FontData.BoldAsString = "True";
            appearance5.TextTrimming = Infragistics.Win.TextTrimming.None;
            this.assetListView.ViewSettingsDetails.ColumnHeaderAppearance = appearance5;
            this.assetListView.ViewSettingsDetails.ColumnHeaderBorderStyle = Infragistics.Win.UIElementBorderStyle.None;
            this.assetListView.ViewSettingsDetails.FullRowSelect = true;
            this.assetListView.ViewSettingsIcons.ImageSize = new System.Drawing.Size(32, 32);
            this.assetListView.ViewSettingsTiles.Alignment = Infragistics.Win.UltraWinListView.ItemAlignment.TopToBottom;
            this.assetListView.ItemDoubleClick += new Infragistics.Win.UltraWinListView.ItemDoubleClickEventHandler(this.computerListView_ItemDoubleClick);
            // 
            // contextMenu1
            // 
            this.contextMenu1.ImageList = null;
            this.contextMenu1.Style = Infragistics.Win.IGControls.MenuStyle.System;
            // 
            // ComputerTabView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.assetListView);
            this.Font = new System.Drawing.Font("Verdana", 8.25F);
            this.Name = "ComputerTabView";
            this.Size = new System.Drawing.Size(833, 380);
            ((System.ComponentModel.ISupportInitialize)(this.assetListView)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

		private Infragistics.Win.UltraWinTabControl.UltraTabPageControl ultraTabPageControl2;
		private Infragistics.Win.UltraWinListView.UltraListView assetListView;
		private Infragistics.Win.IGControls.IGContextMenu contextMenu1;

	}
}
