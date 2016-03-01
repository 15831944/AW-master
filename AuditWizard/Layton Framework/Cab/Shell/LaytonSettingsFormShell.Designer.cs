namespace AuditWizardv8
{
    partial class LaytonSettingsFormShell
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
            Infragistics.Win.Appearance appearance1 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinExplorerBar.UltraExplorerBarGroup ultraExplorerBarGroup1 = new Infragistics.Win.UltraWinExplorerBar.UltraExplorerBarGroup();
            Infragistics.Win.Appearance appearance2 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance3 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance4 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance5 = new Infragistics.Win.Appearance();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.settingsExplorerBar = new Infragistics.Win.UltraWinExplorerBar.UltraExplorerBar();
            this.settingsTabWorkspace = new Infragistics.Practices.CompositeUI.WinForms.UltraTabWorkspace();
            this.tabViewControlPage = new Infragistics.Win.UltraWinTabControl.UltraTabSharedControlsPage();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.settingsExplorerBar)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.settingsTabWorkspace)).BeginInit();
            this.settingsTabWorkspace.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitContainer1
            // 
            this.splitContainer1.BackColor = System.Drawing.Color.Silver;
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.splitContainer1.IsSplitterFixed = true;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.settingsExplorerBar);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.settingsTabWorkspace);
            this.splitContainer1.Size = new System.Drawing.Size(497, 353);
            this.splitContainer1.SplitterDistance = 120;
            this.splitContainer1.SplitterWidth = 1;
            this.splitContainer1.TabIndex = 1;
            // 
            // settingsExplorerBar
            // 
            appearance1.BackColor = System.Drawing.Color.White;
            appearance1.BackColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(228)))), ((int)(((byte)(228)))), ((int)(((byte)(235)))));
            appearance1.BackGradientStyle = Infragistics.Win.GradientStyle.GlassTop50;
            this.settingsExplorerBar.Appearance = appearance1;
            this.settingsExplorerBar.BorderStyle = Infragistics.Win.UIElementBorderStyle.None;
            this.settingsExplorerBar.Dock = System.Windows.Forms.DockStyle.Fill;
            appearance2.BackColor = System.Drawing.Color.Transparent;
            appearance2.BackColor2 = System.Drawing.Color.Transparent;
            ultraExplorerBarGroup1.ItemSettings.AppearancesLarge.Appearance = appearance2;
            ultraExplorerBarGroup1.Settings.HeaderVisible = Infragistics.Win.DefaultableBoolean.False;
            ultraExplorerBarGroup1.Text = "Settings";
            this.settingsExplorerBar.Groups.AddRange(new Infragistics.Win.UltraWinExplorerBar.UltraExplorerBarGroup[] {
            ultraExplorerBarGroup1});
            appearance3.BackColor = System.Drawing.Color.Transparent;
            appearance3.BackColor2 = System.Drawing.Color.Transparent;
            this.settingsExplorerBar.ItemSettings.AppearancesLarge.Appearance = appearance3;
            this.settingsExplorerBar.Location = new System.Drawing.Point(0, 0);
            this.settingsExplorerBar.Name = "settingsExplorerBar";
            this.settingsExplorerBar.Size = new System.Drawing.Size(120, 353);
            this.settingsExplorerBar.Style = Infragistics.Win.UltraWinExplorerBar.UltraExplorerBarStyle.Listbar;
            this.settingsExplorerBar.TabIndex = 0;
            this.settingsExplorerBar.UseFlatMode = Infragistics.Win.DefaultableBoolean.False;
            this.settingsExplorerBar.UseOsThemes = Infragistics.Win.DefaultableBoolean.True;
            this.settingsExplorerBar.ViewStyle = Infragistics.Win.UltraWinExplorerBar.UltraExplorerBarViewStyle.Office2007;
            this.settingsExplorerBar.ItemClick += new Infragistics.Win.UltraWinExplorerBar.ItemClickEventHandler(this.settingsExplorerBar_ActiveItemChanged);
            // 
            // settingsTabWorkspace
            // 
            appearance4.BackColor = System.Drawing.Color.White;
            appearance4.BackColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(228)))), ((int)(((byte)(228)))), ((int)(((byte)(235)))));
            appearance4.BackGradientStyle = Infragistics.Win.GradientStyle.GlassBottom50;
            appearance4.BorderColor = System.Drawing.Color.Transparent;
            this.settingsTabWorkspace.Appearance = appearance4;
            appearance5.Image = global::AuditWizardv8.Properties.Resources.settings_corner;
            this.settingsTabWorkspace.ClientAreaAppearance = appearance5;
            this.settingsTabWorkspace.Controls.Add(this.tabViewControlPage);
            this.settingsTabWorkspace.Dock = System.Windows.Forms.DockStyle.Fill;
            this.settingsTabWorkspace.Location = new System.Drawing.Point(0, 0);
            this.settingsTabWorkspace.Name = "settingsTabWorkspace";
            this.settingsTabWorkspace.SharedControlsPage = this.tabViewControlPage;
            this.settingsTabWorkspace.Size = new System.Drawing.Size(376, 353);
            this.settingsTabWorkspace.TabIndex = 0;
            this.settingsTabWorkspace.ViewStyle = Infragistics.Win.UltraWinTabControl.ViewStyle.Office2007;
            // 
            // tabViewControlPage
            // 
            this.tabViewControlPage.Location = new System.Drawing.Point(1, 20);
            this.tabViewControlPage.Name = "tabViewControlPage";
            this.tabViewControlPage.Size = new System.Drawing.Size(374, 332);
            // 
            // LaytonSettingsFormShell
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(216)))), ((int)(((byte)(228)))), ((int)(((byte)(248)))));
            this.ClientSize = new System.Drawing.Size(497, 353);
            this.Controls.Add(this.splitContainer1);
            this.Font = new System.Drawing.Font("Verdana", 8.25F);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "LaytonSettingsFormShell";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Settings";
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.settingsExplorerBar)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.settingsTabWorkspace)).EndInit();
            this.settingsTabWorkspace.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainer1;
        private Infragistics.Practices.CompositeUI.WinForms.UltraTabWorkspace settingsTabWorkspace;
        private Infragistics.Win.UltraWinTabControl.UltraTabSharedControlsPage tabViewControlPage;
        private Infragistics.Win.UltraWinExplorerBar.UltraExplorerBar settingsExplorerBar;
    }
}