using System;
using System.Windows.Forms;

namespace Layton.Common.Controls
{
    /// <summary>
    /// Base class that is used to represent an exterior page (welcome or
    /// completion page) within a wizard dialog.
    /// </summary>
    public class ExteriorImageWizardPage : WizardPage
	{
		protected PictureBox pbTitle;
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
        /// Initializes a new instance of the <see cref="RenaissanceSoftware.ControlLibrary.ExteriorImageWizardPage">ExteriorImageWizardPage</see>
        /// class.
        /// </summary>
		public ExteriorImageWizardPage()
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
            this._watermarkPicture = new System.Windows.Forms.PictureBox();
            this.pbTitle = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this._watermarkPicture)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbTitle)).BeginInit();
            this.SuspendLayout();
            // 
            // _watermarkPicture
            // 
            this._watermarkPicture.BackColor = System.Drawing.Color.White;
            this._watermarkPicture.Location = new System.Drawing.Point(0, 0);
            this._watermarkPicture.Name = "_watermarkPicture";
            this._watermarkPicture.Size = new System.Drawing.Size(165, 310);
            this._watermarkPicture.TabIndex = 1;
            this._watermarkPicture.TabStop = false;
            // 
            // pbTitle
            // 
            this.pbTitle.Location = new System.Drawing.Point(180, 20);
            this.pbTitle.Name = "pbTitle";
            this.pbTitle.Size = new System.Drawing.Size(300, 55);
            this.pbTitle.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.pbTitle.TabIndex = 3;
            this.pbTitle.TabStop = false;
            // 
            // ExteriorImageWizardPage
            // 
            this.BackColor = System.Drawing.Color.White;
            this.Controls.Add(this.pbTitle);
            this.Controls.Add(this._watermarkPicture);
            this.Font = new System.Drawing.Font("Verdana", 8.25F);
            this.Name = "ExteriorImageWizardPage";
            ((System.ComponentModel.ISupportInitialize)(this._watermarkPicture)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbTitle)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }
		#endregion

    }
}
