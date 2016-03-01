using System;
using System.Windows.Forms;

namespace Layton.Common.Controls
{
    /// <summary>
    /// Base class that is used to represent an exterior page (welcome or
    /// completion page) within a wizard dialog.
    /// </summary>
    public class ExteriorWizardPage : WizardPage
	{
		private Label _titleLabel;
        // ==================================================================
        // Protected Fields
		// ==================================================================
        
        /// <summary>
        /// The watermark graphic.
        /// </summary>
        protected PictureBox _watermarkPicture;


        // ==================================================================
        // Public Constructors
        // ==================================================================
        
        /// <summary>
        /// Initializes a new instance of the <see cref="RenaissanceSoftware.ControlLibrary.ExteriorWizardPage">ExteriorWizardPage</see>
        /// class.
        /// </summary>
        public ExteriorWizardPage()
		{
			// This call is required by the Windows Form Designer
			InitializeComponent();
		}


        // ==================================================================
        // Private Methods
        // ==================================================================

		#region Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
            this._titleLabel = new System.Windows.Forms.Label();
            this._watermarkPicture = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this._watermarkPicture)).BeginInit();
            this.SuspendLayout();
            // 
            // _titleLabel
            // 
            this._titleLabel.BackColor = System.Drawing.Color.White;
            this._titleLabel.Location = new System.Drawing.Point(170, 13);
            this._titleLabel.Name = "_titleLabel";
            this._titleLabel.Size = new System.Drawing.Size(292, 39);
            this._titleLabel.TabIndex = 0;
            this._titleLabel.Text = "Welcome to the Sample Setup Wizard";
            // 
            // _watermarkPicture
            // 
            this._watermarkPicture.BackColor = System.Drawing.Color.White;
            this._watermarkPicture.Location = new System.Drawing.Point(0, 0);
            this._watermarkPicture.Name = "_watermarkPicture";
            this._watermarkPicture.Size = new System.Drawing.Size(164, 312);
            this._watermarkPicture.TabIndex = 1;
            this._watermarkPicture.TabStop = false;
            // 
            // ExteriorWizardPage
            // 
            this.BackColor = System.Drawing.Color.White;
            this.Controls.Add(this._watermarkPicture);
            this.Controls.Add(this._titleLabel);
            this.Font = new System.Drawing.Font("Verdana", 8.25F);
            this.Name = "ExteriorWizardPage";
            ((System.ComponentModel.ISupportInitialize)(this._watermarkPicture)).EndInit();
            this.ResumeLayout(false);

        }
		#endregion

    }
}
