namespace Layton.AuditWizard.Common
{
    partial class FormEditReportSchedule
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
            this.bnEditSchedule = new System.Windows.Forms.Button();
            this.bnEditLocations = new System.Windows.Forms.Button();
            this.bnOK = new System.Windows.Forms.Button();
            this.bnClose = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.tbSchedule = new System.Windows.Forms.TextBox();
            this.tbLocations = new System.Windows.Forms.TextBox();
            this.tbTitle = new System.Windows.Forms.TextBox();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // bnEditSchedule
            // 
            this.bnEditSchedule.Location = new System.Drawing.Point(512, 27);
            this.bnEditSchedule.Name = "bnEditSchedule";
            this.bnEditSchedule.Size = new System.Drawing.Size(87, 23);
            this.bnEditSchedule.TabIndex = 0;
            this.bnEditSchedule.Text = "Edit";
            this.bnEditSchedule.UseVisualStyleBackColor = true;
            this.bnEditSchedule.Click += new System.EventHandler(this.bnEditSchedule_Click);
            // 
            // bnEditLocations
            // 
            this.bnEditLocations.Location = new System.Drawing.Point(512, 105);
            this.bnEditLocations.Name = "bnEditLocations";
            this.bnEditLocations.Size = new System.Drawing.Size(87, 23);
            this.bnEditLocations.TabIndex = 1;
            this.bnEditLocations.Text = "Edit";
            this.bnEditLocations.UseVisualStyleBackColor = true;
            this.bnEditLocations.Click += new System.EventHandler(this.bnEditLocations_Click);
            // 
            // bnOK
            // 
            this.bnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.bnOK.Location = new System.Drawing.Point(423, 218);
            this.bnOK.Name = "bnOK";
            this.bnOK.Size = new System.Drawing.Size(87, 23);
            this.bnOK.TabIndex = 6;
            this.bnOK.Text = "OK";
            this.bnOK.UseVisualStyleBackColor = true;
            this.bnOK.Click += new System.EventHandler(this.bnOK_Click);
            // 
            // bnClose
            // 
            this.bnClose.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.bnClose.Location = new System.Drawing.Point(527, 218);
            this.bnClose.Name = "bnClose";
            this.bnClose.Size = new System.Drawing.Size(87, 23);
            this.bnClose.TabIndex = 7;
            this.bnClose.Text = "Close";
            this.bnClose.UseVisualStyleBackColor = true;
            this.bnClose.Click += new System.EventHandler(this.bnClose_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.tbLocations);
            this.groupBox1.Controls.Add(this.tbSchedule);
            this.groupBox1.Controls.Add(this.bnEditSchedule);
            this.groupBox1.Controls.Add(this.bnEditLocations);
            this.groupBox1.Location = new System.Drawing.Point(15, 52);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(605, 150);
            this.groupBox1.TabIndex = 9;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Current settings";
            // 
            // tbSchedule
            // 
            this.tbSchedule.BackColor = System.Drawing.Color.White;
            this.tbSchedule.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.tbSchedule.Location = new System.Drawing.Point(21, 27);
            this.tbSchedule.Multiline = true;
            this.tbSchedule.Name = "tbSchedule";
            this.tbSchedule.ReadOnly = true;
            this.tbSchedule.Size = new System.Drawing.Size(474, 65);
            this.tbSchedule.TabIndex = 6;
            // 
            // tbLocations
            // 
            this.tbLocations.BackColor = System.Drawing.Color.White;
            this.tbLocations.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.tbLocations.Location = new System.Drawing.Point(21, 105);
            this.tbLocations.Multiline = true;
            this.tbLocations.Name = "tbLocations";
            this.tbLocations.ReadOnly = true;
            this.tbLocations.Size = new System.Drawing.Size(474, 21);
            this.tbLocations.TabIndex = 7;
            // 
            // tbTitle
            // 
            this.tbTitle.BackColor = System.Drawing.Color.White;
            this.tbTitle.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.tbTitle.Location = new System.Drawing.Point(15, 18);
            this.tbTitle.Multiline = true;
            this.tbTitle.Name = "tbTitle";
            this.tbTitle.ReadOnly = true;
            this.tbTitle.Size = new System.Drawing.Size(605, 22);
            this.tbTitle.TabIndex = 10;
            // 
            // FormEditReportSchedule
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(632, 254);
            this.Controls.Add(this.tbTitle);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.bnClose);
            this.Controls.Add(this.bnOK);
            this.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormEditReportSchedule";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Edit Report Schedule";
            this.Load += new System.EventHandler(this.FormEditReportSchedule_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button bnEditSchedule;
        private System.Windows.Forms.Button bnEditLocations;
        private System.Windows.Forms.Button bnOK;
        private System.Windows.Forms.Button bnClose;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.TextBox tbLocations;
        private System.Windows.Forms.TextBox tbSchedule;
        private System.Windows.Forms.TextBox tbTitle;
    }
}