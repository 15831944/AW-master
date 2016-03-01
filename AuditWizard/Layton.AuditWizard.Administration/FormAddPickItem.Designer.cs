namespace Layton.AuditWizard.Administration
{
    partial class FormAddPickItem
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
            this.tbPickItemName = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.bnCancel = new System.Windows.Forms.Button();
            this.lblTitle = new System.Windows.Forms.Label();
            this.bnOK = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.footerPictureBox)).BeginInit();
            this.SuspendLayout();
            // 
            // footerPictureBox
            // 
            this.footerPictureBox.Image = global::Layton.AuditWizard.Administration.Properties.Resources.pickitem_add_corner;
            this.footerPictureBox.Location = new System.Drawing.Point(0, 120);
            // 
            // tbPickItemName
            // 
            this.tbPickItemName.Location = new System.Drawing.Point(89, 42);
            this.tbPickItemName.Name = "tbPickItemName";
            this.tbPickItemName.Size = new System.Drawing.Size(343, 21);
            this.tbPickItemName.TabIndex = 19;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.BackColor = System.Drawing.Color.Transparent;
            this.label2.Location = new System.Drawing.Point(14, 45);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(45, 13);
            this.label2.TabIndex = 18;
            this.label2.Text = "Name:";
            // 
            // bnCancel
            // 
            this.bnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.bnCancel.Location = new System.Drawing.Point(345, 91);
            this.bnCancel.Name = "bnCancel";
            this.bnCancel.Size = new System.Drawing.Size(87, 23);
            this.bnCancel.TabIndex = 16;
            this.bnCancel.Text = "&Cancel";
            this.bnCancel.UseVisualStyleBackColor = true;
            // 
            // lblTitle
            // 
            this.lblTitle.AutoSize = true;
            this.lblTitle.BackColor = System.Drawing.Color.Transparent;
            this.lblTitle.Location = new System.Drawing.Point(14, 9);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(279, 13);
            this.lblTitle.TabIndex = 17;
            this.lblTitle.Text = "Please specify a unique name for this Pick Item";
            // 
            // bnOK
            // 
            this.bnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.bnOK.Location = new System.Drawing.Point(251, 91);
            this.bnOK.Name = "bnOK";
            this.bnOK.Size = new System.Drawing.Size(87, 23);
            this.bnOK.TabIndex = 15;
            this.bnOK.Text = "&OK";
            this.bnOK.UseVisualStyleBackColor = true;
            this.bnOK.Click += new System.EventHandler(this.bnOK_Click);
            // 
            // FormAddPickItem
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(447, 240);
            this.Controls.Add(this.tbPickItemName);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.bnCancel);
            this.Controls.Add(this.lblTitle);
            this.Controls.Add(this.bnOK);
            this.Name = "FormAddPickItem";
            this.Text = "New Pick Item";
            this.Controls.SetChildIndex(this.footerPictureBox, 0);
            this.Controls.SetChildIndex(this.bnOK, 0);
            this.Controls.SetChildIndex(this.lblTitle, 0);
            this.Controls.SetChildIndex(this.bnCancel, 0);
            this.Controls.SetChildIndex(this.label2, 0);
            this.Controls.SetChildIndex(this.tbPickItemName, 0);
            ((System.ComponentModel.ISupportInitialize)(this.footerPictureBox)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox tbPickItemName;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button bnCancel;
        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.Button bnOK;
    }
}