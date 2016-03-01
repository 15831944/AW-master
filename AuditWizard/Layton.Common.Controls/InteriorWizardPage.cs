//
// InteriorWizardPage.cs
//
// Copyright (C) 2002-2002 Steven M. Soloff (mailto:s_soloff@bellsouth.net)
// All rights reserved.
//

using System;
using System.Windows.Forms;

namespace Layton.Common.Controls
{
    /// <summary>
    /// Base class that is used to represent an interior page within a
    /// wizard dialog.
    /// </summary>
	public class InteriorWizardPage : WizardPage
	{
        // ==================================================================
        // Protected Fields
        // ==================================================================
	    
        /// <summary>
        /// The title label.
        /// </summary>
        protected Label _titleLabel;
        
        /// <summary>
        /// The subtitle label.
        /// </summary>
        protected Label _subtitleLabel;

        /// <summary>
        /// The header panel.
        /// </summary>
        protected Panel _headerPanel;
        
        /// <summary>
        /// The header graphic.
        /// </summary>
        protected PictureBox _headerPicture;
        
        /// <summary>
        /// The header/body separator.
        /// </summary>
        protected GroupBox _headerSeparator;


        // ==================================================================
        // Public Constructors
        // ==================================================================
        
        /// <summary>
        /// Initializes a new instance of the <see cref="RenaissanceSoftware.ControlLibrary.InteriorWizardPage">InteriorWizardPage</see>
        /// class.
        /// </summary>
        public InteriorWizardPage()
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
            this._headerSeparator = new System.Windows.Forms.GroupBox();
            this._headerPanel = new System.Windows.Forms.Panel();
            this._titleLabel = new System.Windows.Forms.Label();
            this._subtitleLabel = new System.Windows.Forms.Label();
            this._headerPicture = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this._headerPicture)).BeginInit();
            this.SuspendLayout();
            // 
            // _headerSeparator
            // 
            this._headerSeparator.Dock = System.Windows.Forms.DockStyle.Top;
            this._headerSeparator.Location = new System.Drawing.Point(0, 58);
            this._headerSeparator.Name = "_headerSeparator";
            this._headerSeparator.Size = new System.Drawing.Size(500, 2);
            this._headerSeparator.TabIndex = 3;
            this._headerSeparator.TabStop = false;
            // 
            // _headerPanel
            // 
            this._headerPanel.BackColor = System.Drawing.Color.White;
            this._headerPanel.Dock = System.Windows.Forms.DockStyle.Top;
            this._headerPanel.Location = new System.Drawing.Point(0, 0);
            this._headerPanel.Name = "_headerPanel";
            this._headerPanel.Size = new System.Drawing.Size(500, 58);
            this._headerPanel.TabIndex = 0;
            // 
            // _titleLabel
            // 
            this._titleLabel.BackColor = System.Drawing.Color.White;
            this._titleLabel.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._titleLabel.Location = new System.Drawing.Point(20, 10);
            this._titleLabel.Name = "_titleLabel";
            this._titleLabel.Size = new System.Drawing.Size(410, 13);
            this._titleLabel.TabIndex = 1;
            this._titleLabel.Text = "Sample Header Title";
            // 
            // _subtitleLabel
            // 
            this._subtitleLabel.BackColor = System.Drawing.Color.White;
            this._subtitleLabel.Location = new System.Drawing.Point(41, 25);
            this._subtitleLabel.Name = "_subtitleLabel";
            this._subtitleLabel.Size = new System.Drawing.Size(389, 26);
            this._subtitleLabel.TabIndex = 2;
            this._subtitleLabel.Text = "Sample header subtitle will help a user complete a certain task in the Sample wiz" +
                "ard by clarifying the task in some way.";
            // 
            // _headerPicture
            // 
            this._headerPicture.BackColor = System.Drawing.Color.White;
            this._headerPicture.Location = new System.Drawing.Point(443, 5);
            this._headerPicture.Name = "_headerPicture";
            this._headerPicture.Size = new System.Drawing.Size(49, 49);
            this._headerPicture.TabIndex = 4;
            this._headerPicture.TabStop = false;
            // 
            // InteriorWizardPage
            // 
            this.Controls.Add(this._headerPicture);
            this.Controls.Add(this._subtitleLabel);
            this.Controls.Add(this._titleLabel);
            this.Controls.Add(this._headerSeparator);
            this.Controls.Add(this._headerPanel);
            this.Font = new System.Drawing.Font("Verdana", 8.25F);
            this.Name = "InteriorWizardPage";
            ((System.ComponentModel.ISupportInitialize)(this._headerPicture)).EndInit();
            this.ResumeLayout(false);

        }
		#endregion
    }
}
