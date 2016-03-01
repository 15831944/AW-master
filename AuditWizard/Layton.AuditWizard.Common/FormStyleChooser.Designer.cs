namespace Layton.AuditWizard.Common
{
	partial class FormStyleChooser
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
            Infragistics.Win.Appearance appearance3 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance4 = new Infragistics.Win.Appearance();
            Infragistics.Win.ValueListItem valueListItem106 = new Infragistics.Win.ValueListItem();
            Infragistics.Win.ValueListItem valueListItem107 = new Infragistics.Win.ValueListItem();
            Infragistics.Win.ValueListItem valueListItem108 = new Infragistics.Win.ValueListItem();
            Infragistics.Win.Appearance appearance5 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance7 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance2 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance6 = new Infragistics.Win.Appearance();
            this.foreColorPicker = new Infragistics.Win.UltraWinEditors.UltraColorPicker();
            this.fontName = new Infragistics.Win.UltraWinEditors.UltraFontNameEditor();
            this.cbUnderline = new Infragistics.Win.UltraWinEditors.UltraCheckEditor();
            this.cbBold = new Infragistics.Win.UltraWinEditors.UltraCheckEditor();
            this.cbAlignment = new Infragistics.Win.UltraWinEditors.UltraComboEditor();
            this.ultraLabel1 = new Infragistics.Win.Misc.UltraLabel();
            this.ultraLabel2 = new Infragistics.Win.Misc.UltraLabel();
            this.ultraLabel3 = new Infragistics.Win.Misc.UltraLabel();
            this.labelPreview = new Infragistics.Win.Misc.UltraLabel();
            this.ultraLabel6 = new Infragistics.Win.Misc.UltraLabel();
            this.nupFontSize = new System.Windows.Forms.NumericUpDown();
            this.bnCancel = new System.Windows.Forms.Button();
            this.bnOK = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.footerPictureBox)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.foreColorPicker)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.fontName)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cbAlignment)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nupFontSize)).BeginInit();
            this.SuspendLayout();
            // 
            // footerPictureBox
            // 
            this.footerPictureBox.Image = global::Layton.AuditWizard.Common.Properties.Resources.textstyle_corner;
            this.footerPictureBox.Location = new System.Drawing.Point(2, 250);
            this.footerPictureBox.Size = new System.Drawing.Size(447, 125);
            // 
            // foreColorPicker
            // 
            this.foreColorPicker.Color = System.Drawing.Color.DarkBlue;
            this.foreColorPicker.Location = new System.Drawing.Point(101, 31);
            this.foreColorPicker.Name = "foreColorPicker";
            this.foreColorPicker.Size = new System.Drawing.Size(168, 22);
            this.foreColorPicker.TabIndex = 10;
            this.foreColorPicker.Text = "DarkBlue";
            this.foreColorPicker.ColorChanged += new System.EventHandler(this.style_Changed);
            // 
            // fontName
            // 
            this.fontName.Location = new System.Drawing.Point(101, 58);
            this.fontName.Name = "fontName";
            this.fontName.Size = new System.Drawing.Size(168, 22);
            this.fontName.TabIndex = 11;
            this.fontName.Text = "Verdana";
            this.fontName.ValueChanged += new System.EventHandler(this.style_Changed);
            // 
            // cbUnderline
            // 
            appearance3.BackColor = System.Drawing.Color.Transparent;
            this.cbUnderline.Appearance = appearance3;
            this.cbUnderline.BackColor = System.Drawing.Color.Transparent;
            this.cbUnderline.BackColorInternal = System.Drawing.Color.Transparent;
            this.cbUnderline.Location = new System.Drawing.Point(101, 85);
            this.cbUnderline.Name = "cbUnderline";
            this.cbUnderline.Size = new System.Drawing.Size(83, 20);
            this.cbUnderline.TabIndex = 12;
            this.cbUnderline.Text = "Underline";
            this.cbUnderline.CheckedChanged += new System.EventHandler(this.style_Changed);
            // 
            // cbBold
            // 
            appearance4.BackColor = System.Drawing.Color.Transparent;
            this.cbBold.Appearance = appearance4;
            this.cbBold.BackColor = System.Drawing.Color.Transparent;
            this.cbBold.BackColorInternal = System.Drawing.Color.Transparent;
            this.cbBold.Location = new System.Drawing.Point(198, 85);
            this.cbBold.Name = "cbBold";
            this.cbBold.Size = new System.Drawing.Size(140, 20);
            this.cbBold.TabIndex = 13;
            this.cbBold.Text = "Bold";
            this.cbBold.CheckedChanged += new System.EventHandler(this.style_Changed);
            // 
            // cbAlignment
            // 
            this.cbAlignment.DropDownStyle = Infragistics.Win.DropDownStyle.DropDownList;
            valueListItem106.DataValue = 0;
            valueListItem106.DisplayText = "Left";
            valueListItem107.DataValue = 1;
            valueListItem107.DisplayText = "Right";
            valueListItem108.DataValue = 2;
            valueListItem108.DisplayText = "Center";
            this.cbAlignment.Items.AddRange(new Infragistics.Win.ValueListItem[] {
            valueListItem106,
            valueListItem107,
            valueListItem108});
            this.cbAlignment.Location = new System.Drawing.Point(101, 111);
            this.cbAlignment.Name = "cbAlignment";
            this.cbAlignment.Size = new System.Drawing.Size(168, 22);
            this.cbAlignment.TabIndex = 14;
            this.cbAlignment.ValueChanged += new System.EventHandler(this.style_Changed);
            // 
            // ultraLabel1
            // 
            appearance5.BackColor = System.Drawing.Color.Transparent;
            this.ultraLabel1.Appearance = appearance5;
            this.ultraLabel1.Location = new System.Drawing.Point(14, 35);
            this.ultraLabel1.Name = "ultraLabel1";
            this.ultraLabel1.Size = new System.Drawing.Size(80, 23);
            this.ultraLabel1.TabIndex = 15;
            this.ultraLabel1.Text = "Text Color:";
            // 
            // ultraLabel2
            // 
            appearance7.BackColor = System.Drawing.Color.Transparent;
            this.ultraLabel2.Appearance = appearance7;
            this.ultraLabel2.Location = new System.Drawing.Point(14, 62);
            this.ultraLabel2.Name = "ultraLabel2";
            this.ultraLabel2.Size = new System.Drawing.Size(80, 23);
            this.ultraLabel2.TabIndex = 16;
            this.ultraLabel2.Text = "Font:";
            // 
            // ultraLabel3
            // 
            appearance2.BackColor = System.Drawing.Color.Transparent;
            this.ultraLabel3.Appearance = appearance2;
            this.ultraLabel3.Location = new System.Drawing.Point(14, 111);
            this.ultraLabel3.Name = "ultraLabel3";
            this.ultraLabel3.Size = new System.Drawing.Size(80, 23);
            this.ultraLabel3.TabIndex = 17;
            this.ultraLabel3.Text = "Alignment:";
            // 
            // labelPreview
            // 
            this.labelPreview.Location = new System.Drawing.Point(14, 168);
            this.labelPreview.Name = "labelPreview";
            this.labelPreview.Size = new System.Drawing.Size(421, 33);
            this.labelPreview.TabIndex = 18;
            this.labelPreview.Text = "Your text will look like this";
            // 
            // ultraLabel6
            // 
            appearance6.BackColor = System.Drawing.Color.Transparent;
            this.ultraLabel6.Appearance = appearance6;
            this.ultraLabel6.Location = new System.Drawing.Point(292, 62);
            this.ultraLabel6.Name = "ultraLabel6";
            this.ultraLabel6.Size = new System.Drawing.Size(47, 23);
            this.ultraLabel6.TabIndex = 20;
            this.ultraLabel6.Text = "Size:";
            // 
            // nupFontSize
            // 
            this.nupFontSize.Location = new System.Drawing.Point(345, 59);
            this.nupFontSize.Maximum = new decimal(new int[] {
            36,
            0,
            0,
            0});
            this.nupFontSize.Minimum = new decimal(new int[] {
            4,
            0,
            0,
            0});
            this.nupFontSize.Name = "nupFontSize";
            this.nupFontSize.Size = new System.Drawing.Size(66, 21);
            this.nupFontSize.TabIndex = 21;
            this.nupFontSize.Value = new decimal(new int[] {
            4,
            0,
            0,
            0});
            this.nupFontSize.ValueChanged += new System.EventHandler(this.style_Changed);
            // 
            // bnCancel
            // 
            this.bnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.bnCancel.Location = new System.Drawing.Point(345, 207);
            this.bnCancel.Name = "bnCancel";
            this.bnCancel.Size = new System.Drawing.Size(87, 23);
            this.bnCancel.TabIndex = 58;
            this.bnCancel.Text = "&Cancel";
            this.bnCancel.UseVisualStyleBackColor = true;
            // 
            // bnOK
            // 
            this.bnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.bnOK.Location = new System.Drawing.Point(251, 207);
            this.bnOK.Name = "bnOK";
            this.bnOK.Size = new System.Drawing.Size(87, 23);
            this.bnOK.TabIndex = 57;
            this.bnOK.Text = "&OK";
            this.bnOK.UseVisualStyleBackColor = true;
            this.bnOK.Click += new System.EventHandler(this.bnOK_Click);
            // 
            // FormStyleChooser
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 13F);
            this.ClientSize = new System.Drawing.Size(449, 370);
            this.Controls.Add(this.bnCancel);
            this.Controls.Add(this.bnOK);
            this.Controls.Add(this.nupFontSize);
            this.Controls.Add(this.ultraLabel6);
            this.Controls.Add(this.labelPreview);
            this.Controls.Add(this.ultraLabel3);
            this.Controls.Add(this.ultraLabel2);
            this.Controls.Add(this.ultraLabel1);
            this.Controls.Add(this.cbAlignment);
            this.Controls.Add(this.cbBold);
            this.Controls.Add(this.foreColorPicker);
            this.Controls.Add(this.cbUnderline);
            this.Controls.Add(this.fontName);
            this.DoubleBuffered = true;
            this.Font = new System.Drawing.Font("Verdana", 8.25F);
            this.Name = "FormStyleChooser";
            this.Text = "Style Chooser";
            this.Load += new System.EventHandler(this.FormStyleChooser_Load);
            this.Controls.SetChildIndex(this.fontName, 0);
            this.Controls.SetChildIndex(this.cbUnderline, 0);
            this.Controls.SetChildIndex(this.foreColorPicker, 0);
            this.Controls.SetChildIndex(this.footerPictureBox, 0);
            this.Controls.SetChildIndex(this.cbBold, 0);
            this.Controls.SetChildIndex(this.cbAlignment, 0);
            this.Controls.SetChildIndex(this.ultraLabel1, 0);
            this.Controls.SetChildIndex(this.ultraLabel2, 0);
            this.Controls.SetChildIndex(this.ultraLabel3, 0);
            this.Controls.SetChildIndex(this.labelPreview, 0);
            this.Controls.SetChildIndex(this.ultraLabel6, 0);
            this.Controls.SetChildIndex(this.nupFontSize, 0);
            this.Controls.SetChildIndex(this.bnOK, 0);
            this.Controls.SetChildIndex(this.bnCancel, 0);
            ((System.ComponentModel.ISupportInitialize)(this.footerPictureBox)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.foreColorPicker)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.fontName)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cbAlignment)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nupFontSize)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

		}

		#endregion

		private Infragistics.Win.UltraWinEditors.UltraColorPicker foreColorPicker;
		private Infragistics.Win.UltraWinEditors.UltraFontNameEditor fontName;
		private Infragistics.Win.UltraWinEditors.UltraCheckEditor cbUnderline;
		private Infragistics.Win.UltraWinEditors.UltraCheckEditor cbBold;
		private Infragistics.Win.UltraWinEditors.UltraComboEditor cbAlignment;
		private Infragistics.Win.Misc.UltraLabel ultraLabel1;
		private Infragistics.Win.Misc.UltraLabel ultraLabel2;
		private Infragistics.Win.Misc.UltraLabel ultraLabel3;
		private Infragistics.Win.Misc.UltraLabel labelPreview;
		private Infragistics.Win.Misc.UltraLabel ultraLabel6;
		private System.Windows.Forms.NumericUpDown nupFontSize;
		private System.Windows.Forms.Button bnCancel;
		private System.Windows.Forms.Button bnOK;
	}
}
