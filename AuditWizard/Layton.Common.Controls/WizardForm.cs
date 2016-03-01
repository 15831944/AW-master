
using System;
using System.Collections;
using System.Drawing;
using System.Windows.Forms;

namespace Layton.Common.Controls
{
    /// <summary>
    /// Used to identify the various buttons that may appear within a wizard
    /// dialog.  
    /// </summary>
    [Flags]		
    public enum WizardButton
    {
        /// <summary>
        /// Identifies the <b>Back</b> button.
        /// </summary>
        Back           = 0x00000001,
        
        /// <summary>
        /// Identifies the <b>Next</b> button.
        /// </summary>
        Next           = 0x00000002,
        
        /// <summary>
        /// Identifies the <b>Finish</b> button.
        /// </summary>
        Finish         = 0x00000004,
        
        /// <summary>
        /// Identifies the disabled <b>Finish</b> button.
        /// </summary>
        DisabledFinish = 0x00000008,
    }
    
    /// <summary>
    /// Represents a wizard dialog.
    /// </summary>
    public class WizardForm : Form
	{
        // ==================================================================
        // Public Constants
        // ==================================================================

        /// <summary>
        /// Used by a page to indicate to this wizard that the next page
        /// should be activated when either the Back or Next buttons are
        /// pressed.
        /// </summary>
        public const string NextPage = "";

        /// <summary>
        /// Used by a page to indicate to this wizard that the selected page
        /// should remain active when either the Back or Next buttons are
        /// pressed.
        /// </summary>
        public const string NoPageChange = null;
	
	
        // ==================================================================
        // Private Fields
        // ==================================================================
        
        /// <summary>
        /// Array of wizard pages.
        /// </summary>
        private ArrayList _pages = new ArrayList();
        
        /// <summary>
        /// Index of the selected page; -1 if no page selected.
        /// </summary>
        private int _selectedIndex = -1;


        // ==================================================================
        // Protected Fields
        // ==================================================================
        
        /// <summary>
        /// The Back button.
        /// </summary>
        protected Button _backButton;

        /// <summary>
        /// The Next button.
        /// </summary>
        protected Button _extButton;

        /// <summary>
        /// The Cancel button.
        /// </summary>
        protected Button _cancelButton;

        /// <summary>
        /// The Finish button.
        /// </summary>
        protected Button _finishButton;

        /// <summary>
        /// The separator between the buttons and the content.
        /// </summary>
        protected GroupBox _separator;


        // ==================================================================
        // Public Constructors
        // ==================================================================
        
        /// <summary>
        /// Initializes a new instance of the <see cref="RenaissanceSoftware.ControlLibrary.WizardForm">WizardForm</see>
        /// class.
        /// </summary>
        public WizardForm()
		{
			// Required for Windows Form Designer support
			InitializeComponent();

            // Ensure Finish and Next buttons are positioned similarly
			_finishButton.Location = _extButton.Location;
		}


        // ==================================================================
        // Private Methods
        // ==================================================================
        
        #region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
            this._backButton = new System.Windows.Forms.Button();
            this._extButton = new System.Windows.Forms.Button();
            this._cancelButton = new System.Windows.Forms.Button();
            this._finishButton = new System.Windows.Forms.Button();
            this._separator = new System.Windows.Forms.GroupBox();
            this.SuspendLayout();
            // 
            // _backButton
            // 
            this._backButton.Location = new System.Drawing.Point(210, 304);
            this._backButton.Name = "_backButton";
            this._backButton.Size = new System.Drawing.Size(62, 21);
            this._backButton.TabIndex = 8;
            this._backButton.Text = "< &Back";
            this._backButton.Click += new System.EventHandler(this.OnClickBack);
            // 
            // _extButton
            // 
            this._extButton.Location = new System.Drawing.Point(272, 304);
            this._extButton.Name = "_extButton";
            this._extButton.Size = new System.Drawing.Size(63, 21);
            this._extButton.TabIndex = 9;
            this._extButton.Text = "&Next >";
            this._extButton.Click += new System.EventHandler(this.OnClickNext);
            // 
            // _cancelButton
            // 
            this._cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this._cancelButton.Location = new System.Drawing.Point(343, 304);
            this._cancelButton.Name = "_cancelButton";
            this._cancelButton.Size = new System.Drawing.Size(63, 21);
            this._cancelButton.TabIndex = 11;
            this._cancelButton.Text = "Cancel";
            this._cancelButton.Click += new System.EventHandler(this.OnClickCancel);
            // 
            // _finishButton
            // 
            this._finishButton.Location = new System.Drawing.Point(8, 304);
            this._finishButton.Name = "_finishButton";
            this._finishButton.Size = new System.Drawing.Size(63, 21);
            this._finishButton.TabIndex = 10;
            this._finishButton.Text = "&Finish";
            this._finishButton.Visible = false;
            this._finishButton.Click += new System.EventHandler(this.OnClickFinish);
            // 
            // _separator
            // 
            this._separator.Location = new System.Drawing.Point(0, 291);
            this._separator.Name = "_separator";
            this._separator.Size = new System.Drawing.Size(416, 1);
            this._separator.TabIndex = 7;
            this._separator.TabStop = false;
            // 
            // WizardForm
            // 
            this.AcceptButton = this._extButton;
            this.AutoScaleBaseSize = new System.Drawing.Size(6, 14);
            this.CancelButton = this._cancelButton;
            this.ClientSize = new System.Drawing.Size(497, 360);
            this.Controls.Add(this._backButton);
            this.Controls.Add(this._extButton);
            this.Controls.Add(this._cancelButton);
            this.Controls.Add(this._finishButton);
            this.Controls.Add(this._separator);
            this.Font = new System.Drawing.Font("Verdana", 8.25F);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "WizardForm";
            this.ShowInTaskbar = false;
            this.ResumeLayout(false);

        }
		#endregion

        /// <summary>
        /// Activates the page at the specified index in the page array.
        /// </summary>
        /// <param name="newIndex">
        /// Index of new page to be selected.
        /// </param>
        private void ActivatePage ( int newIndex )
        {
            // Ensure the index is valid
            if( newIndex < 0 || newIndex >= _pages.Count )
                throw new ArgumentOutOfRangeException();

            // Deactivate the current page if applicable
            WizardPage currentPage = null;
            if( _selectedIndex != -1 )
            {
                currentPage = (WizardPage)_pages[ _selectedIndex ];
                if( !currentPage.OnKillActive() )
                    return;
            }

            WizardPage newPage = null;

            do
            {
                // Ensure the index is valid
                if( newIndex < 0 || newIndex >= _pages.Count )
                    throw new ArgumentOutOfRangeException();

                // Activate the new page
                newPage = (WizardPage)_pages[ newIndex ];

                if (newPage.Enabled && newPage.OnSetActive() )
                {
                    // ok we can move to the next/previous page
                }
                else
                {
                    newPage = null;
                    if (_selectedIndex > newIndex)
                        newIndex--;
                    else
                        newIndex++;
                }
            }
            while (newPage == null);

            // Update state
            _selectedIndex = newIndex;
            if( currentPage != null )
                currentPage.Visible = false;
            newPage.Visible = true;
            newPage.Focus();
        }

        /// <summary>
        /// Handles the Click event for the Back button.
        /// </summary>
        private void OnClickBack( object sender, EventArgs e )
        {
            // Ensure a page is currently selected
            if( _selectedIndex != -1 )
            {
                // Inform selected page that the Back button was clicked
                string pageName = ((WizardPage)_pages[
                    _selectedIndex ]).OnWizardBack();
                switch( pageName )
                {
                    // Do nothing
                    case NoPageChange:
                        break;
                        
                    // Activate the next appropriate page
                    case NextPage:
                        if ( _selectedIndex - 1 >= 0 )
                            ActivatePage(_selectedIndex - 1);
                            break;

                    // Activate the specified page if it exists
                    default:
                        foreach( WizardPage page in _pages )
                        {
                            if( page.Name == pageName )
                                ActivatePage( _pages.IndexOf( page ) );
                        }
                        break;
                }
            }
        }

        /// <summary>
        /// Handles the Click event for the Cancel button.
        /// </summary>
		protected virtual void OnClickCancel(object sender, EventArgs e)
        {
            // Close wizard
			DialogResult = DialogResult.Cancel;
        }

        /// <summary>
        /// Handles the Click event for the Finish button.
        /// </summary>
        protected virtual void OnClickFinish( object sender, EventArgs e )
        {
            // Ensure a page is currently selected
            if( _selectedIndex != -1 )
            {
                // Inform selected page that the Finish button was clicked
                WizardPage page = (WizardPage)_pages[ _selectedIndex ];
                if( page.OnWizardFinish() )
                {
                    // Deactivate page and close wizard
                    if( page.OnKillActive() )
                        DialogResult = DialogResult.OK;
                }
            }
        }

        /// <summary>
        /// Handles the Click event for the Next button.
        /// </summary>
        private void OnClickNext( object sender, EventArgs e )
        {
            // Ensure a page is currently selected
            if( _selectedIndex != -1 )
            {
                // Inform selected page that the Next button was clicked
                string pageName = ((WizardPage)_pages[_selectedIndex]).OnWizardNext();
                switch( pageName )
                {
                    // Do nothing
                    case NoPageChange:
                        break;

                    // Activate the next appropriate page
                    case NextPage:
                        if( _selectedIndex + 1 < _pages.Count )
                            ActivatePage( _selectedIndex + 1 );
                        break;

                    // Activate the specified page if it exists
                    default:
                        foreach( WizardPage page in _pages )
                        {
                            if( page.Name == pageName )
                                ActivatePage( _pages.IndexOf( page ) );
                        }
                        break;
                }
            }
        }


        // ==================================================================
        // Protected Methods
        // ==================================================================
        
        /// <seealso cref="System.Windows.Forms.Control.OnControlAdded">
        /// System.Windows.Forms.Control.OnControlAdded
        /// </seealso>
        protected override void OnControlAdded( ControlEventArgs e )
        {
            // Invoke base class implementation
            base.OnControlAdded( e );
            
            // Set default properties for all WizardPage instances added to this form
            WizardPage page = e.Control as WizardPage;
            if( page != null )
            {
                page.Visible = false;
                page.Location = new Point( 0, 0 );
                page.Size = new Size( Width, _separator.Location.Y );
                _pages.Add( page );
//                if( _selectedIndex == -1 )
//                  _selectedIndex = 0;
            }
        }

        /// <seealso cref="System.Windows.Forms.Form.OnLoad">
        /// System.Windows.Forms.Form.OnLoad
        /// </seealso>
        protected override void OnLoad( EventArgs e )
        {
            // Invoke base class implementation
            base.OnLoad( e );
            
            // Activate the first page in the wizard
            if( _pages.Count > 0 )
                ActivatePage( 0 );
        }


        // ==================================================================
        // Public Methods
        // ==================================================================
        
        /// <summary>
        /// Sets the text in the Finish button.
        /// </summary>
        /// <param name="text">
        /// Text to be displayed on the Finish button.
        /// </param>
        public void SetFinishText( string text )
        {
            // Set the Finish button text
            _finishButton.Text = text;
        }
        
        /// <summary>
        /// Enables or disables the Back, Next, or Finish buttons in the
        /// wizard.
        /// </summary>
        /// <param name="flags">
        /// A set of flags that customize the function and appearance of the
        /// wizard buttons.  This parameter can be a combination of any
        /// value in the <c>WizardButton</c> enumeration.
        /// </param>
        /// <remarks>
        /// Typically, you should call <c>SetWizardButtons</c> from
        /// <c>WizardPage.OnSetActive</c>.  You can display a Finish or a
        /// Next button at one time, but not both.
        /// </remarks>
        public void SetWizardButtons( WizardButton flags )
        {
            // Enable/disable and show/hide buttons appropriately
            _backButton.Enabled =
                (flags & WizardButton.Back) == WizardButton.Back;
            _extButton.Enabled =
                (flags & WizardButton.Next) == WizardButton.Next;
            _extButton.Visible =
                (flags & WizardButton.Finish) == 0 &&
                (flags & WizardButton.DisabledFinish) == 0;
            _finishButton.Enabled =
                (flags & WizardButton.DisabledFinish) == 0;
            _finishButton.Visible =
                (flags & WizardButton.Finish) == WizardButton.Finish ||
                (flags & WizardButton.DisabledFinish) == WizardButton.DisabledFinish;
                
            // Set the AcceptButton depending on whether or not the Finish
            // button is visible or not
            AcceptButton = _finishButton.Visible ? _finishButton :
                _extButton;
        }
    }
}
