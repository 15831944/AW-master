namespace Layton.Cab.Shell
{
    partial class LaytonRegistrationForm
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
			this.label2 = new System.Windows.Forms.Label();
			this.label4 = new System.Windows.Forms.Label();
			this.label6 = new System.Windows.Forms.Label();
			this.licensingTextBox = new System.Windows.Forms.TextBox();
			this.continueButton = new System.Windows.Forms.Button();
			this.label1 = new System.Windows.Forms.Label();
			this.label5 = new System.Windows.Forms.Label();
			this.companyNameTextBox = new System.Windows.Forms.TextBox();
			this.companyIdTextBox = new System.Windows.Forms.TextBox();
			this.productKeyTextBox = new System.Windows.Forms.TextBox();
			this.registerButton = new System.Windows.Forms.Button();
			this.getKeyButton = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// label2
			// 
			this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.label2.BackColor = System.Drawing.Color.Transparent;
			this.label2.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label2.ForeColor = System.Drawing.Color.White;
			this.label2.Location = new System.Drawing.Point(392, 86);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(207, 31);
			this.label2.TabIndex = 11;
			this.label2.Text = "Enter your license information below if you wish to register the product:";
			// 
			// label4
			// 
			this.label4.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.label4.AutoSize = true;
			this.label4.BackColor = System.Drawing.Color.Transparent;
			this.label4.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label4.ForeColor = System.Drawing.Color.White;
			this.label4.Location = new System.Drawing.Point(392, 126);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(96, 14);
			this.label4.TabIndex = 13;
			this.label4.Text = "Company Name:";
			// 
			// label6
			// 
			this.label6.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.label6.AutoSize = true;
			this.label6.BackColor = System.Drawing.Color.Transparent;
			this.label6.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label6.ForeColor = System.Drawing.Color.White;
			this.label6.Location = new System.Drawing.Point(392, 275);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(94, 14);
			this.label6.TabIndex = 16;
			this.label6.Text = "License Details:";
			// 
			// licensingTextBox
			// 
			this.licensingTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.licensingTextBox.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(228)))), ((int)(((byte)(228)))), ((int)(((byte)(235)))));
			this.licensingTextBox.Location = new System.Drawing.Point(395, 292);
			this.licensingTextBox.Multiline = true;
			this.licensingTextBox.Name = "licensingTextBox";
			this.licensingTextBox.ReadOnly = true;
			this.licensingTextBox.Size = new System.Drawing.Size(204, 46);
			this.licensingTextBox.TabIndex = 17;
			// 
			// continueButton
			// 
			this.continueButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.continueButton.BackColor = System.Drawing.Color.Lavender;
			this.continueButton.Location = new System.Drawing.Point(540, 357);
			this.continueButton.Name = "continueButton";
			this.continueButton.Size = new System.Drawing.Size(75, 23);
			this.continueButton.TabIndex = 18;
			this.continueButton.Text = "Continue";
			this.continueButton.UseVisualStyleBackColor = false;
			this.continueButton.Click += new System.EventHandler(this.continueButton_Click);
			// 
			// label1
			// 
			this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.label1.AutoSize = true;
			this.label1.BackColor = System.Drawing.Color.Transparent;
			this.label1.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label1.ForeColor = System.Drawing.Color.White;
			this.label1.Location = new System.Drawing.Point(392, 176);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(75, 14);
			this.label1.TabIndex = 19;
			this.label1.Text = "Company ID:";
			// 
			// label5
			// 
			this.label5.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.label5.AutoSize = true;
			this.label5.BackColor = System.Drawing.Color.Transparent;
			this.label5.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label5.ForeColor = System.Drawing.Color.White;
			this.label5.Location = new System.Drawing.Point(392, 225);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(76, 14);
			this.label5.TabIndex = 20;
			this.label5.Text = "Product Key:";
			// 
			// companyNameTextBox
			// 
			this.companyNameTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.companyNameTextBox.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(228)))), ((int)(((byte)(228)))), ((int)(((byte)(235)))));
			this.companyNameTextBox.Location = new System.Drawing.Point(395, 143);
			this.companyNameTextBox.Name = "companyNameTextBox";
			this.companyNameTextBox.Size = new System.Drawing.Size(204, 20);
			this.companyNameTextBox.TabIndex = 21;
			// 
			// companyIdTextBox
			// 
			this.companyIdTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.companyIdTextBox.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(228)))), ((int)(((byte)(228)))), ((int)(((byte)(235)))));
			this.companyIdTextBox.Location = new System.Drawing.Point(395, 193);
			this.companyIdTextBox.Name = "companyIdTextBox";
			this.companyIdTextBox.Size = new System.Drawing.Size(204, 20);
			this.companyIdTextBox.TabIndex = 22;
			// 
			// productKeyTextBox
			// 
			this.productKeyTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.productKeyTextBox.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(228)))), ((int)(((byte)(228)))), ((int)(((byte)(235)))));
			this.productKeyTextBox.Location = new System.Drawing.Point(395, 242);
			this.productKeyTextBox.Name = "productKeyTextBox";
			this.productKeyTextBox.Size = new System.Drawing.Size(204, 20);
			this.productKeyTextBox.TabIndex = 23;
			// 
			// registerButton
			// 
			this.registerButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.registerButton.BackColor = System.Drawing.Color.Lavender;
			this.registerButton.Location = new System.Drawing.Point(459, 357);
			this.registerButton.Name = "registerButton";
			this.registerButton.Size = new System.Drawing.Size(75, 23);
			this.registerButton.TabIndex = 24;
			this.registerButton.Text = "Register";
			this.registerButton.UseVisualStyleBackColor = false;
			this.registerButton.Click += new System.EventHandler(this.registerButton_Click);
			// 
			// getKeyButton
			// 
			this.getKeyButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.getKeyButton.BackColor = System.Drawing.Color.Lavender;
			this.getKeyButton.Location = new System.Drawing.Point(381, 357);
			this.getKeyButton.Name = "getKeyButton";
			this.getKeyButton.Size = new System.Drawing.Size(72, 23);
			this.getKeyButton.TabIndex = 25;
			this.getKeyButton.Text = "Get Key";
			this.getKeyButton.UseVisualStyleBackColor = false;
			this.getKeyButton.Click += new System.EventHandler(this.getKeyButton_Click);
			// 
			// LaytonRegistrationForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackColor = System.Drawing.SystemColors.Control;
			this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
			this.ClientSize = new System.Drawing.Size(633, 392);
			this.Controls.Add(this.getKeyButton);
			this.Controls.Add(this.registerButton);
			this.Controls.Add(this.productKeyTextBox);
			this.Controls.Add(this.companyIdTextBox);
			this.Controls.Add(this.companyNameTextBox);
			this.Controls.Add(this.label5);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.continueButton);
			this.Controls.Add(this.licensingTextBox);
			this.Controls.Add(this.label6);
			this.Controls.Add(this.label4);
			this.Controls.Add(this.label2);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
			this.Name = "LaytonRegistrationForm";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Product Registration";
			this.TransparencyKey = System.Drawing.SystemColors.Control;
			this.Shown += new System.EventHandler(this.LaytonRegistrationForm_Shown);
			this.ResumeLayout(false);
			this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox licensingTextBox;
        private System.Windows.Forms.Button continueButton;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox companyNameTextBox;
        private System.Windows.Forms.TextBox companyIdTextBox;
        private System.Windows.Forms.TextBox productKeyTextBox;
        private System.Windows.Forms.Button registerButton;
        private System.Windows.Forms.Button getKeyButton;
    }
}