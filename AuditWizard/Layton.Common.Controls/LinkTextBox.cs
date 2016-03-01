using System;
using System.ComponentModel;
using System.Windows.Forms;

namespace Layton.Common.Controls
{
  /// <summary>
  /// LinkTextBox extends TextBox with LinkType property which
  /// creates a clickable Hyperlink when user is not editing the text.
  /// </summary>
  public class LinkTextBox : TextBox 
  {

    #region Private Fields

    private LinkLabel llLinkLabel;    
    private LinkTypes ltLinkType = LinkTypes.None;

    private bool bLinkClicked;

    #endregion

    #region Properties

    [DefaultValue(LinkTypes.None)]
    public LinkTypes LinkType {
      set { 
        this.ltLinkType = value; 
        if (value == LinkTypes.None) {
          SwitchToEditMode(true);
        } else {
          SwitchToEditMode(false);
          FillLinkData();
        }
      }
      get { return this.ltLinkType; }
    }

    #endregion

    #region Constructor

    public LinkTextBox() {
    
      // create LinkLabel, add it to controls array, 
      // position it so that it exactly overlaps 
      // the text in the text box and
      // add event handler so we can correct strange tab behavior

      llLinkLabel = new LinkLabel();
      
      this.Controls.Add(llLinkLabel);                 

      llLinkLabel.AutoSize = true;
      llLinkLabel.Left = -1;
      llLinkLabel.Top = 1;
      llLinkLabel.LinkClicked += new LinkLabelLinkClickedEventHandler(ll_LinkClicked);
      llLinkLabel.Visible = true;
      llLinkLabel.Text = this.Text;      
      
      llLinkLabel.GotFocus += new EventHandler(llLinkLabel_GotFocus);
      llLinkLabel.MouseDown += new MouseEventHandler(llLinkLabel_MouseDown);

    }


    #endregion

    #region Focus overrides
        
    protected override void OnGotFocus(EventArgs e) {
      
      base.OnGotFocus(e);

      // when control gets focus and we have active LinkType switch to edit mode
      if (ltLinkType != LinkTypes.None) this.SwitchToEditMode(true);

    }

    protected override void OnLostFocus(System.EventArgs e) {      

      base.OnLostFocus(e);		      
      
      // when control gets focus and we have active LinkType switch to clickable mode
      if (ltLinkType != LinkTypes.None) this.SwitchToEditMode(false);

    }

    protected override void OnTextChanged(EventArgs e) {
      
      base.OnTextChanged (e);

      // when TextBox's Text changes, copy that data to LinkLabel
      if (ltLinkType != LinkTypes.None) {      
        FillLinkData();
      }

    }         


    #endregion

    #region Click Handling

    /// <summary>
    /// Switch to Edit mode or to Clickable mode.
    /// </summary>
    /// <param name="_bEditMode">Edit mode = true, Clickable mode = false</param>
    protected void SwitchToEditMode(bool _bEditMode) {      

      // edit mode only means that LinkLabel is not visible
      llLinkLabel.Visible = !_bEditMode;      

    }    

    /// <summary>
    /// Copy information from TextBox to LinkLabel.
    /// </summary>
    private void FillLinkData() {

      // copy the text
      llLinkLabel.Text = this.Text;      

      // figure out if we need mailto: or http:// link
      string sLinkType = "";

      switch (ltLinkType) {
        case LinkTypes.Http: 
          if (this.Text.ToLower().IndexOf(@"http://") < 0 && this.Text.ToLower().IndexOf(@"https://") < 0) {
            sLinkType = @"http://";
          }
          break;
        case LinkTypes.Ftp:
          if (this.Text.ToLower().IndexOf(@"ftp://") < 0) {
            sLinkType = @"ftp://";
          }
          break;
        case LinkTypes.Email:
          if (this.Text.ToLower().IndexOf("mailto:") < 0) {
            sLinkType = "mailto:";
          }
          break;
      }

      // clear old links and create a new one
      llLinkLabel.Links.Clear();
      llLinkLabel.Links.Add(0, llLinkLabel.Text.Length, sLinkType + this.Text);

    }

    /// <summary>
    /// Try to "execute" supplied link. Throws ArgumentException if it fails.
    /// </summary>
    private void UseHyperlink() {
      
      try {

        if (llLinkLabel.Links.Count > 0) {
          string sLink = llLinkLabel.Links[0].LinkData.ToString();
          System.Diagnostics.Process.Start(sLink);
        }

      } catch (Exception ex) {

        throw new ArgumentException("Link error.", ex);        

      }

    }

    #endregion

    #region Link Activation

    /// <summary>
    /// Use the Hyperlink if user clicked on a LinkLabel.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ll_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e) {
      if (ltLinkType != LinkTypes.None) UseHyperlink();      
    }
                                            
    /// <summary>
    /// If user clicked in the TextBox with Control key pressed, user HyperLink.
    /// </summary>
    /// <param name="e"></param>           
    protected override void OnMouseDown(MouseEventArgs e) {
      if (ltLinkType != LinkTypes.None) {
        if (e.Button == MouseButtons.Left && (Control.ModifierKeys & Keys.Control) == Keys.Control) {
          UseHyperlink();
        } else {
          base.OnMouseDown(e);
        }
      } else {
        base.OnMouseDown(e);
      }
    }

    #endregion

    #region Focus Handling

    private void llLinkLabel_GotFocus(object sender, EventArgs e) {      
      // if control got focus with tab and not because user clicked a link
      // the transfer focus to TextBox and clear the flag
      if (!bLinkClicked) {
        this.Focus();
        bLinkClicked = false;
      }
    }
    
    private void llLinkLabel_MouseDown(object sender, MouseEventArgs e) {
      // remember that user clicked on the label, so we can correct the focus of a label
      bLinkClicked = true;
    }


    #endregion

  }

  #region LinkTypes Enum

  /// <summary>
  /// Types of Hyperlinks that LinkTextBox supports.
  /// </summary>
  public enum LinkTypes {
    None,     // act as a regular TextBox
    Http,     // act as a http:// or https:// hyperlink
    Ftp,      // act as a ftp:// hyperlink
    Email     // act as a mailto: hyperlink
  }

  #endregion

}
