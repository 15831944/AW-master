namespace Layton.AuditWizard.Common
{
    partial class FormTasks
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
            Infragistics.Win.Appearance appearance24 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance25 = new Infragistics.Win.Appearance();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormTasks));
            Infragistics.Win.Appearance appearance26 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance27 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance28 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance29 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance30 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance31 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance32 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance33 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance34 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance35 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance36 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance37 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance38 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance39 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance40 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance41 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinScrollBar.ScrollBarLook scrollBarLook1 = new Infragistics.Win.UltraWinScrollBar.ScrollBarLook();
            Infragistics.Win.Appearance appearance42 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance43 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance44 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance45 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance46 = new Infragistics.Win.Appearance();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.bnAdd = new System.Windows.Forms.Button();
            this.ultraGridTaskSchedules = new Infragistics.Win.UltraWinGrid.UltraGrid();
            this.bnDeleteSchedule = new System.Windows.Forms.Button();
            this.bnEditSchedule = new System.Windows.Forms.Button();
            this.bnDeleteAllSchedules = new System.Windows.Forms.Button();
            this.bnClose = new System.Windows.Forms.Button();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ultraGridTaskSchedules)).BeginInit();
            this.SuspendLayout();
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.bnAdd);
            this.groupBox2.Controls.Add(this.ultraGridTaskSchedules);
            this.groupBox2.Controls.Add(this.bnDeleteSchedule);
            this.groupBox2.Controls.Add(this.bnEditSchedule);
            this.groupBox2.Controls.Add(this.bnDeleteAllSchedules);
            this.groupBox2.Location = new System.Drawing.Point(12, 12);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(854, 517);
            this.groupBox2.TabIndex = 6;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Add, view, edit and delete existing tasks";
            // 
            // bnAdd
            // 
            this.bnAdd.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.bnAdd.Location = new System.Drawing.Point(575, 482);
            this.bnAdd.Name = "bnAdd";
            this.bnAdd.Size = new System.Drawing.Size(87, 21);
            this.bnAdd.TabIndex = 14;
            this.bnAdd.Text = "Add";
            this.bnAdd.UseVisualStyleBackColor = true;
            this.bnAdd.Click += new System.EventHandler(this.bnAdd_Click);
            // 
            // ultraGridTaskSchedules
            // 
            appearance24.BackColor = System.Drawing.Color.White;
            appearance24.BackColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(221)))), ((int)(((byte)(221)))), ((int)(((byte)(221)))));
            appearance24.BackGradientStyle = Infragistics.Win.GradientStyle.Vertical;
            appearance24.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(21)))), ((int)(((byte)(27)))), ((int)(((byte)(85)))));
            this.ultraGridTaskSchedules.DisplayLayout.AddNewBox.Appearance = appearance24;
            this.ultraGridTaskSchedules.DisplayLayout.AddNewBox.BorderStyle = Infragistics.Win.UIElementBorderStyle.None;
            appearance25.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(21)))), ((int)(((byte)(27)))), ((int)(((byte)(85)))));
            appearance25.ImageBackground = ((System.Drawing.Image)(resources.GetObject("appearance25.ImageBackground")));
            appearance25.ImageBackgroundAlpha = Infragistics.Win.Alpha.UseAlphaLevel;
            appearance25.ImageBackgroundStretchMargins = new Infragistics.Win.ImageBackgroundStretchMargins(6, 3, 6, 3);
            appearance25.ImageBackgroundStyle = Infragistics.Win.ImageBackgroundStyle.Stretched;
            this.ultraGridTaskSchedules.DisplayLayout.AddNewBox.ButtonAppearance = appearance25;
            this.ultraGridTaskSchedules.DisplayLayout.AddNewBox.ButtonConnectorColor = System.Drawing.Color.FromArgb(((int)(((byte)(21)))), ((int)(((byte)(27)))), ((int)(((byte)(85)))));
            this.ultraGridTaskSchedules.DisplayLayout.AddNewBox.ButtonStyle = Infragistics.Win.UIElementButtonStyle.FlatBorderless;
            appearance26.BackColor = System.Drawing.Color.White;
            this.ultraGridTaskSchedules.DisplayLayout.Appearance = appearance26;
            this.ultraGridTaskSchedules.DisplayLayout.AutoFitStyle = Infragistics.Win.UltraWinGrid.AutoFitStyle.ResizeAllColumns;
            this.ultraGridTaskSchedules.DisplayLayout.BorderStyle = Infragistics.Win.UIElementBorderStyle.None;
            appearance27.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(21)))), ((int)(((byte)(27)))), ((int)(((byte)(85)))));
            appearance27.FontData.Name = "Trebuchet MS";
            appearance27.FontData.SizeInPoints = 9F;
            appearance27.ForeColor = System.Drawing.Color.White;
            appearance27.TextHAlignAsString = "Right";
            this.ultraGridTaskSchedules.DisplayLayout.CaptionAppearance = appearance27;
            this.ultraGridTaskSchedules.DisplayLayout.CaptionVisible = Infragistics.Win.DefaultableBoolean.False;
            this.ultraGridTaskSchedules.DisplayLayout.FixedHeaderOffImage = ((System.Drawing.Image)(resources.GetObject("ultraGridTaskSchedules.DisplayLayout.FixedHeaderOffImage")));
            this.ultraGridTaskSchedules.DisplayLayout.FixedHeaderOnImage = ((System.Drawing.Image)(resources.GetObject("ultraGridTaskSchedules.DisplayLayout.FixedHeaderOnImage")));
            this.ultraGridTaskSchedules.DisplayLayout.FixedRowOffImage = ((System.Drawing.Image)(resources.GetObject("ultraGridTaskSchedules.DisplayLayout.FixedRowOffImage")));
            this.ultraGridTaskSchedules.DisplayLayout.FixedRowOnImage = ((System.Drawing.Image)(resources.GetObject("ultraGridTaskSchedules.DisplayLayout.FixedRowOnImage")));
            appearance28.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(216)))), ((int)(((byte)(228)))), ((int)(((byte)(248)))));
            appearance28.BackColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(157)))), ((int)(((byte)(185)))), ((int)(((byte)(235)))));
            appearance28.BackGradientStyle = Infragistics.Win.GradientStyle.Vertical;
            appearance28.FontData.BoldAsString = "True";
            appearance28.FontData.Name = "Verdana";
            appearance28.FontData.SizeInPoints = 10F;
            appearance28.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(127)))), ((int)(((byte)(177)))));
            this.ultraGridTaskSchedules.DisplayLayout.GroupByBox.Appearance = appearance28;
            this.ultraGridTaskSchedules.DisplayLayout.GroupByBox.BandLabelBorderStyle = Infragistics.Win.UIElementBorderStyle.None;
            this.ultraGridTaskSchedules.DisplayLayout.GroupByBox.ButtonBorderStyle = Infragistics.Win.UIElementBorderStyle.None;
            appearance29.BackColor = System.Drawing.Color.Transparent;
            appearance29.FontData.Name = "Verdana";
            this.ultraGridTaskSchedules.DisplayLayout.GroupByBox.PromptAppearance = appearance29;
            this.ultraGridTaskSchedules.DisplayLayout.Override.AllowRowFiltering = Infragistics.Win.DefaultableBoolean.True;
            this.ultraGridTaskSchedules.DisplayLayout.Override.BorderStyleCell = Infragistics.Win.UIElementBorderStyle.None;
            this.ultraGridTaskSchedules.DisplayLayout.Override.BorderStyleHeader = Infragistics.Win.UIElementBorderStyle.None;
            this.ultraGridTaskSchedules.DisplayLayout.Override.BorderStyleRow = Infragistics.Win.UIElementBorderStyle.None;
            appearance30.BackColor = System.Drawing.Color.Transparent;
            this.ultraGridTaskSchedules.DisplayLayout.Override.CardAreaAppearance = appearance30;
            appearance31.BorderColor = System.Drawing.Color.Transparent;
            appearance31.FontData.Name = "Verdana";
            this.ultraGridTaskSchedules.DisplayLayout.Override.CellAppearance = appearance31;
            appearance32.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(21)))), ((int)(((byte)(27)))), ((int)(((byte)(85)))));
            appearance32.ImageBackground = ((System.Drawing.Image)(resources.GetObject("appearance32.ImageBackground")));
            appearance32.ImageBackgroundStretchMargins = new Infragistics.Win.ImageBackgroundStretchMargins(6, 3, 6, 3);
            appearance32.ImageBackgroundStyle = Infragistics.Win.ImageBackgroundStyle.Stretched;
            this.ultraGridTaskSchedules.DisplayLayout.Override.CellButtonAppearance = appearance32;
            this.ultraGridTaskSchedules.DisplayLayout.Override.CellClickAction = Infragistics.Win.UltraWinGrid.CellClickAction.RowSelect;
            this.ultraGridTaskSchedules.DisplayLayout.Override.CellPadding = 4;
            appearance33.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(248)))), ((int)(((byte)(251)))));
            this.ultraGridTaskSchedules.DisplayLayout.Override.FilterCellAppearance = appearance33;
            appearance34.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(21)))), ((int)(((byte)(27)))), ((int)(((byte)(85)))));
            appearance34.ImageBackground = ((System.Drawing.Image)(resources.GetObject("appearance34.ImageBackground")));
            appearance34.ImageBackgroundStretchMargins = new Infragistics.Win.ImageBackgroundStretchMargins(6, 3, 6, 3);
            this.ultraGridTaskSchedules.DisplayLayout.Override.FilterClearButtonAppearance = appearance34;
            appearance35.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(251)))), ((int)(((byte)(251)))), ((int)(((byte)(251)))));
            appearance35.BackColorAlpha = Infragistics.Win.Alpha.Opaque;
            this.ultraGridTaskSchedules.DisplayLayout.Override.FilterRowPromptAppearance = appearance35;
            this.ultraGridTaskSchedules.DisplayLayout.Override.FilterUIType = Infragistics.Win.UltraWinGrid.FilterUIType.FilterRow;
            appearance36.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(221)))), ((int)(((byte)(236)))), ((int)(((byte)(255)))));
            appearance36.BackColor2 = System.Drawing.Color.White;
            appearance36.BackGradientStyle = Infragistics.Win.GradientStyle.None;
            appearance36.FontData.BoldAsString = "True";
            appearance36.FontData.Name = "Verdana";
            appearance36.FontData.SizeInPoints = 8F;
            appearance36.ForeColor = System.Drawing.Color.DimGray;
            appearance36.ImageHAlign = Infragistics.Win.HAlign.Center;
            appearance36.ImageVAlign = Infragistics.Win.VAlign.Middle;
            appearance36.TextHAlignAsString = "Center";
            this.ultraGridTaskSchedules.DisplayLayout.Override.HeaderAppearance = appearance36;
            appearance37.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(246)))), ((int)(((byte)(252)))), ((int)(((byte)(255)))));
            this.ultraGridTaskSchedules.DisplayLayout.Override.RowAlternateAppearance = appearance37;
            appearance38.BorderColor = System.Drawing.Color.LightGray;
            this.ultraGridTaskSchedules.DisplayLayout.Override.RowAppearance = appearance38;
            appearance39.BackColor = System.Drawing.Color.White;
            this.ultraGridTaskSchedules.DisplayLayout.Override.RowSelectorAppearance = appearance39;
            this.ultraGridTaskSchedules.DisplayLayout.Override.RowSelectors = Infragistics.Win.DefaultableBoolean.True;
            appearance40.BorderColor = System.Drawing.Color.Transparent;
            appearance40.ForeColor = System.Drawing.Color.Black;
            this.ultraGridTaskSchedules.DisplayLayout.Override.SelectedCellAppearance = appearance40;
            appearance41.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(238)))), ((int)(((byte)(243)))), ((int)(((byte)(223)))));
            appearance41.BorderColor = System.Drawing.Color.Transparent;
            appearance41.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(21)))), ((int)(((byte)(27)))), ((int)(((byte)(85)))));
            appearance41.ImageBackgroundStretchMargins = new Infragistics.Win.ImageBackgroundStretchMargins(1, 1, 1, 4);
            appearance41.ImageBackgroundStyle = Infragistics.Win.ImageBackgroundStyle.Stretched;
            this.ultraGridTaskSchedules.DisplayLayout.Override.SelectedRowAppearance = appearance41;
            appearance42.ImageBackgroundStretchMargins = new Infragistics.Win.ImageBackgroundStretchMargins(2, 4, 2, 4);
            appearance42.ImageBackgroundStyle = Infragistics.Win.ImageBackgroundStyle.Stretched;
            scrollBarLook1.Appearance = appearance42;
            appearance43.ImageBackground = ((System.Drawing.Image)(resources.GetObject("appearance43.ImageBackground")));
            appearance43.ImageBackgroundStretchMargins = new Infragistics.Win.ImageBackgroundStretchMargins(3, 2, 3, 2);
            scrollBarLook1.AppearanceHorizontal = appearance43;
            appearance44.ImageBackground = ((System.Drawing.Image)(resources.GetObject("appearance44.ImageBackground")));
            appearance44.ImageBackgroundStretchMargins = new Infragistics.Win.ImageBackgroundStretchMargins(2, 3, 2, 3);
            appearance44.ImageBackgroundStyle = Infragistics.Win.ImageBackgroundStyle.Stretched;
            scrollBarLook1.AppearanceVertical = appearance44;
            appearance45.ImageBackground = ((System.Drawing.Image)(resources.GetObject("appearance45.ImageBackground")));
            appearance45.ImageBackgroundStretchMargins = new Infragistics.Win.ImageBackgroundStretchMargins(0, 2, 0, 1);
            scrollBarLook1.TrackAppearanceHorizontal = appearance45;
            appearance46.ImageBackground = ((System.Drawing.Image)(resources.GetObject("appearance46.ImageBackground")));
            appearance46.ImageBackgroundStretchMargins = new Infragistics.Win.ImageBackgroundStretchMargins(2, 0, 1, 0);
            appearance46.ImageBackgroundStyle = Infragistics.Win.ImageBackgroundStyle.Stretched;
            scrollBarLook1.TrackAppearanceVertical = appearance46;
            this.ultraGridTaskSchedules.DisplayLayout.ScrollBarLook = scrollBarLook1;
            this.ultraGridTaskSchedules.DisplayLayout.ScrollBounds = Infragistics.Win.UltraWinGrid.ScrollBounds.ScrollToFill;
            this.ultraGridTaskSchedules.Font = new System.Drawing.Font("Verdana", 8.25F);
            this.ultraGridTaskSchedules.Location = new System.Drawing.Point(10, 34);
            this.ultraGridTaskSchedules.Name = "ultraGridTaskSchedules";
            this.ultraGridTaskSchedules.Size = new System.Drawing.Size(838, 442);
            this.ultraGridTaskSchedules.TabIndex = 7;
            this.ultraGridTaskSchedules.Text = "Grid Caption Area";
            this.ultraGridTaskSchedules.UseOsThemes = Infragistics.Win.DefaultableBoolean.False;
            this.ultraGridTaskSchedules.AfterSelectChange += new Infragistics.Win.UltraWinGrid.AfterSelectChangeEventHandler(this.ultraGridTaskSchedules_AfterSelectChange);
            this.ultraGridTaskSchedules.InitializeRow += new Infragistics.Win.UltraWinGrid.InitializeRowEventHandler(this.ultraGridReportSchedules_InitializeRow);
            this.ultraGridTaskSchedules.DoubleClickRow += new Infragistics.Win.UltraWinGrid.DoubleClickRowEventHandler(this.ultraGridTaskSchedules_DoubleClickRow);
            // 
            // bnDeleteSchedule
            // 
            this.bnDeleteSchedule.Enabled = false;
            this.bnDeleteSchedule.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.bnDeleteSchedule.Location = new System.Drawing.Point(761, 482);
            this.bnDeleteSchedule.Name = "bnDeleteSchedule";
            this.bnDeleteSchedule.Size = new System.Drawing.Size(87, 21);
            this.bnDeleteSchedule.TabIndex = 6;
            this.bnDeleteSchedule.Text = "Delete";
            this.bnDeleteSchedule.UseVisualStyleBackColor = true;
            this.bnDeleteSchedule.Click += new System.EventHandler(this.bnDeleteSchedule_Click);
            // 
            // bnEditSchedule
            // 
            this.bnEditSchedule.Enabled = false;
            this.bnEditSchedule.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.bnEditSchedule.Location = new System.Drawing.Point(668, 482);
            this.bnEditSchedule.Name = "bnEditSchedule";
            this.bnEditSchedule.Size = new System.Drawing.Size(87, 21);
            this.bnEditSchedule.TabIndex = 5;
            this.bnEditSchedule.Text = "Edit";
            this.bnEditSchedule.UseVisualStyleBackColor = true;
            this.bnEditSchedule.Click += new System.EventHandler(this.bnEditSchedule_Click);
            // 
            // bnDeleteAllSchedules
            // 
            this.bnDeleteAllSchedules.Location = new System.Drawing.Point(14, 482);
            this.bnDeleteAllSchedules.Name = "bnDeleteAllSchedules";
            this.bnDeleteAllSchedules.Size = new System.Drawing.Size(179, 21);
            this.bnDeleteAllSchedules.TabIndex = 13;
            this.bnDeleteAllSchedules.Text = "Delete All Expired Tasks";
            this.bnDeleteAllSchedules.UseVisualStyleBackColor = true;
            this.bnDeleteAllSchedules.Click += new System.EventHandler(this.bnDeleteAllSchedules_Click);
            // 
            // bnClose
            // 
            this.bnClose.Location = new System.Drawing.Point(791, 547);
            this.bnClose.Name = "bnClose";
            this.bnClose.Size = new System.Drawing.Size(75, 23);
            this.bnClose.TabIndex = 10;
            this.bnClose.Text = "Close";
            this.bnClose.UseVisualStyleBackColor = true;
            this.bnClose.Click += new System.EventHandler(this.bnClose_Click);
            // 
            // FormTasks
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(892, 581);
            this.Controls.Add(this.bnClose);
            this.Controls.Add(this.groupBox2);
            this.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormTasks";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Manage Tasks";
            this.Load += new System.EventHandler(this.FormTasks_Load);
            this.groupBox2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.ultraGridTaskSchedules)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox2;
        private Infragistics.Win.UltraWinGrid.UltraGrid ultraGridTaskSchedules;
        private System.Windows.Forms.Button bnDeleteSchedule;
        private System.Windows.Forms.Button bnEditSchedule;
        private System.Windows.Forms.Button bnDeleteAllSchedules;
        private System.Windows.Forms.Button bnClose;
        private System.Windows.Forms.Button bnAdd;
    }
}