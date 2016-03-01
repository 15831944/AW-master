namespace Layton.AuditWizard.Administration
{
	partial class FormAlertTrigger
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormAlertTrigger));
            Infragistics.Win.Appearance appearance2 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance4 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance3 = new Infragistics.Win.Appearance();
            this.tbName = new System.Windows.Forms.TextBox();
            this.ultraLabel3 = new Infragistics.Win.Misc.UltraLabel();
            this.bnCancel = new System.Windows.Forms.Button();
            this.bnOK = new System.Windows.Forms.Button();
            this.selectedFields = new Layton.AuditWizard.Common.SelectAuditedDataFieldsControl();
            this.ultraLabel1 = new Infragistics.Win.Misc.UltraLabel();
            this.cbConditions = new Infragistics.Win.UltraWinEditors.UltraComboEditor();
            this.tbValue = new System.Windows.Forms.TextBox();
            this.ultraLabel2 = new Infragistics.Win.Misc.UltraLabel();
            this.ultraLabel4 = new Infragistics.Win.Misc.UltraLabel();
            ((System.ComponentModel.ISupportInitialize)(this.footerPictureBox)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cbConditions)).BeginInit();
            this.SuspendLayout();
            // 
            // footerPictureBox
            // 
            this.footerPictureBox.Image = global::Layton.AuditWizard.Administration.Properties.Resources.alert_trigger_create_corner;
            this.footerPictureBox.Location = new System.Drawing.Point(334, 395);
            this.footerPictureBox.Size = new System.Drawing.Size(447, 120);
            // 
            // tbName
            // 
            this.tbName.BackColor = System.Drawing.SystemColors.Window;
            this.tbName.Location = new System.Drawing.Point(140, 26);
            this.tbName.Name = "tbName";
            this.tbName.ReadOnly = true;
            this.tbName.Size = new System.Drawing.Size(292, 21);
            this.tbName.TabIndex = 23;
            // 
            // ultraLabel3
            // 
            appearance1.BackColor = System.Drawing.Color.Transparent;
            this.ultraLabel3.Appearance = appearance1;
            this.ultraLabel3.Location = new System.Drawing.Point(16, 29);
            this.ultraLabel3.Name = "ultraLabel3";
            this.ultraLabel3.Size = new System.Drawing.Size(117, 23);
            this.ultraLabel3.TabIndex = 22;
            this.ultraLabel3.Text = "Name of Alert:";
            // 
            // bnCancel
            // 
            this.bnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.bnCancel.Location = new System.Drawing.Point(670, 354);
            this.bnCancel.Name = "bnCancel";
            this.bnCancel.Size = new System.Drawing.Size(87, 23);
            this.bnCancel.TabIndex = 21;
            this.bnCancel.Text = "&Cancel";
            this.bnCancel.UseVisualStyleBackColor = true;
            // 
            // bnOK
            // 
            this.bnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.bnOK.Location = new System.Drawing.Point(575, 354);
            this.bnOK.Name = "bnOK";
            this.bnOK.Size = new System.Drawing.Size(87, 23);
            this.bnOK.TabIndex = 20;
            this.bnOK.Text = "&Add";
            this.bnOK.UseVisualStyleBackColor = true;
            this.bnOK.Click += new System.EventHandler(this.bnOK_Click);
            // 
            // selectedFields
            // 
            this.selectedFields.AlertableItemsOnly = true;
            this.selectedFields.AllowExpandApplications = false;
            this.selectedFields.AllowInternetSelection = true;
            this.selectedFields.Location = new System.Drawing.Point(16, 95);
            this.selectedFields.Name = "selectedFields";
            this.selectedFields.NodeStyle = Infragistics.Win.UltraWinTree.NodeStyle.Standard;
            this.selectedFields.SelectedItems = ((System.Collections.Generic.List<string>)(resources.GetObject("selectedFields.SelectedItems")));
            this.selectedFields.Size = new System.Drawing.Size(222, 401);
            this.selectedFields.TabIndex = 24;
            // 
            // ultraLabel1
            // 
            appearance2.BackColor = System.Drawing.Color.Transparent;
            this.ultraLabel1.Appearance = appearance2;
            this.ultraLabel1.Location = new System.Drawing.Point(16, 66);
            this.ultraLabel1.Name = "ultraLabel1";
            this.ultraLabel1.Size = new System.Drawing.Size(222, 23);
            this.ultraLabel1.TabIndex = 25;
            this.ultraLabel1.Text = "Select the field on which to trigger...";
            // 
            // cbConditions
            // 
            this.cbConditions.DropDownStyle = Infragistics.Win.DropDownStyle.DropDownList;
            this.cbConditions.Location = new System.Drawing.Point(266, 95);
            this.cbConditions.Name = "cbConditions";
            this.cbConditions.Size = new System.Drawing.Size(211, 22);
            this.cbConditions.TabIndex = 26;
            this.cbConditions.ValueChanged += new System.EventHandler(this.cbConditions_ValueChanged);
            // 
            // tbValue
            // 
            this.tbValue.Enabled = false;
            this.tbValue.Location = new System.Drawing.Point(498, 96);
            this.tbValue.Name = "tbValue";
            this.tbValue.Size = new System.Drawing.Size(258, 21);
            this.tbValue.TabIndex = 27;
            // 
            // ultraLabel2
            // 
            appearance4.BackColor = System.Drawing.Color.Transparent;
            this.ultraLabel2.Appearance = appearance4;
            this.ultraLabel2.Location = new System.Drawing.Point(266, 66);
            this.ultraLabel2.Name = "ultraLabel2";
            this.ultraLabel2.Size = new System.Drawing.Size(222, 23);
            this.ultraLabel2.TabIndex = 28;
            this.ultraLabel2.Text = "...then select a condition";
            // 
            // ultraLabel4
            // 
            appearance3.BackColor = System.Drawing.Color.Transparent;
            this.ultraLabel4.Appearance = appearance3;
            this.ultraLabel4.Location = new System.Drawing.Point(498, 66);
            this.ultraLabel4.Name = "ultraLabel4";
            this.ultraLabel4.Size = new System.Drawing.Size(229, 23);
            this.ultraLabel4.TabIndex = 29;
            this.ultraLabel4.Text = "...then select a value to check against";
            // 
            // FormAlertTrigger
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(782, 515);
            this.Controls.Add(this.ultraLabel4);
            this.Controls.Add(this.ultraLabel2);
            this.Controls.Add(this.tbValue);
            this.Controls.Add(this.cbConditions);
            this.Controls.Add(this.ultraLabel1);
            this.Controls.Add(this.selectedFields);
            this.Controls.Add(this.tbName);
            this.Controls.Add(this.ultraLabel3);
            this.Controls.Add(this.bnCancel);
            this.Controls.Add(this.bnOK);
            this.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "FormAlertTrigger";
            this.Text = "Create Alert Trigger";
            this.Load += new System.EventHandler(this.FormAlertTrigger_Load);
            this.Controls.SetChildIndex(this.footerPictureBox, 0);
            this.Controls.SetChildIndex(this.bnOK, 0);
            this.Controls.SetChildIndex(this.bnCancel, 0);
            this.Controls.SetChildIndex(this.ultraLabel3, 0);
            this.Controls.SetChildIndex(this.tbName, 0);
            this.Controls.SetChildIndex(this.selectedFields, 0);
            this.Controls.SetChildIndex(this.ultraLabel1, 0);
            this.Controls.SetChildIndex(this.cbConditions, 0);
            this.Controls.SetChildIndex(this.tbValue, 0);
            this.Controls.SetChildIndex(this.ultraLabel2, 0);
            this.Controls.SetChildIndex(this.ultraLabel4, 0);
            ((System.ComponentModel.ISupportInitialize)(this.footerPictureBox)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cbConditions)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.TextBox tbName;
		private Infragistics.Win.Misc.UltraLabel ultraLabel3;
		private System.Windows.Forms.Button bnCancel;
		private System.Windows.Forms.Button bnOK;
		private Layton.AuditWizard.Common.SelectAuditedDataFieldsControl selectedFields;
		private Infragistics.Win.Misc.UltraLabel ultraLabel1;
		private Infragistics.Win.UltraWinEditors.UltraComboEditor cbConditions;
		private System.Windows.Forms.TextBox tbValue;
		private Infragistics.Win.Misc.UltraLabel ultraLabel2;
		private Infragistics.Win.Misc.UltraLabel ultraLabel4;
	}
}