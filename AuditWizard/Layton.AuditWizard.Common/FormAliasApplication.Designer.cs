namespace Layton.AuditWizard.Common
{
    partial class FormAliasApplication
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
            this.bnAlias = new System.Windows.Forms.Button();
            this.bnClose = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.listBoxApps = new System.Windows.Forms.ListBox();
            this.label2 = new System.Windows.Forms.Label();
            this.selectTarget = new Layton.AuditWizard.Common.SelectApplicationsControl();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // bnAlias
            // 
            this.bnAlias.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.bnAlias.Location = new System.Drawing.Point(584, 622);
            this.bnAlias.Name = "bnAlias";
            this.bnAlias.Size = new System.Drawing.Size(75, 23);
            this.bnAlias.TabIndex = 18;
            this.bnAlias.Text = "Alias";
            this.bnAlias.UseVisualStyleBackColor = true;
            this.bnAlias.Click += new System.EventHandler(this.bnAlias_Click);
            // 
            // bnClose
            // 
            this.bnClose.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.bnClose.Location = new System.Drawing.Point(665, 622);
            this.bnClose.Name = "bnClose";
            this.bnClose.Size = new System.Drawing.Size(75, 23);
            this.bnClose.TabIndex = 20;
            this.bnClose.Text = "Close";
            this.bnClose.UseVisualStyleBackColor = true;
            this.bnClose.Click += new System.EventHandler(this.bnClose_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(13, 154);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(500, 13);
            this.label1.TabIndex = 21;
            this.label1.Text = "Select an application from the list below to which these applications should be a" +
                "liased:";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.listBoxApps);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.selectTarget);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Location = new System.Drawing.Point(12, 13);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(728, 592);
            this.groupBox1.TabIndex = 23;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Choose Target Applications";
            // 
            // listBoxApps
            // 
            this.listBoxApps.FormattingEnabled = true;
            this.listBoxApps.Location = new System.Drawing.Point(10, 61);
            this.listBoxApps.Name = "listBoxApps";
            this.listBoxApps.Size = new System.Drawing.Size(709, 82);
            this.listBoxApps.TabIndex = 23;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(13, 34);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(301, 13);
            this.label2.TabIndex = 22;
            this.label2.Text = "You have chosen to alias the following applications:";
            // 
            // selectTarget
            // 
            this.selectTarget.BackColor = System.Drawing.Color.White;
            this.selectTarget.Font = new System.Drawing.Font("Verdana", 8.25F);
            this.selectTarget.Location = new System.Drawing.Point(10, 174);
            this.selectTarget.Name = "selectTarget";
            this.selectTarget.SelectionType = Layton.AuditWizard.Common.SelectApplicationsControl.eSelectionType.all;
            this.selectTarget.Size = new System.Drawing.Size(709, 399);
            this.selectTarget.TabIndex = 17;
            // 
            // FormAliasApplication
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(752, 657);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.bnClose);
            this.Controls.Add(this.bnAlias);
            this.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormAliasApplication";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Alias Applications";
            this.Load += new System.EventHandler(this.FormAliasApplication_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private SelectApplicationsControl selectTarget;
        private System.Windows.Forms.Button bnAlias;
        private System.Windows.Forms.Button bnClose;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ListBox listBoxApps;
    }
}