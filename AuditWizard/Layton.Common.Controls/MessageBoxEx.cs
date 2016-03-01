using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Layton.Common.Controls
{
    public enum MessageBoxResult { Yes, YesToAll, No, NoToAll, Cancel }

    public partial class MessageBoxEx : Form
    {
        // Internal values
        MessageBoxResult lastResult = MessageBoxResult.Cancel;
        MessageBoxResult result = MessageBoxResult.Cancel;
        
        /// <summary>
        /// The default constructor for MessageBoxEx.
        /// </summary>
        public MessageBoxEx()
        {
            InitializeComponent();
        }

        #region Properties
        public String LabelText
        {
            get { return this.labelBody.Text; }
            set
            {
                this.labelBody.Text = value;
                UpdateSize();
            }
        }

        public MessageBoxResult Result
        {
            get { return this.result; }
            set { this.result = value; }
        }
        
        #endregion

#region Public Methods
        /// <summary>
        /// This is a direct replacement for the standard ShowDialog except it will handle
		/// the case where a previous call has request either yestoall or notoall
        /// </summary>
        /// <returns></returns>
        public new MessageBoxResult ShowDialog()
        {
            result = MessageBoxResult.Cancel;
            if (lastResult == MessageBoxResult.NoToAll)
            {
                result = MessageBoxResult.No;
            }
            else if (lastResult == MessageBoxResult.YesToAll)
            {
                result = MessageBoxResult.Yes;
            }
            else
            {
                base.ShowDialog();
            }
            return result;
        }

        public MessageBoxResult ShowDialog(String label, string title)
        {
            this.Text = title;
            LabelText = label;
            return ShowDialog();
        }
        #endregion

        #region Private Methods

        /// <summary>
        /// This call updates the size of the window based on certain factors,
        /// such as if an icon is present, and the size of label.
        /// </summary>
        private void UpdateSize()
        {
            int newWidth = labelBody.Size.Width + 40;

            // Add the width of the icon, and some padding.
            if (pictureBoxIcon.Image != null)
            {
                newWidth += pictureBoxIcon.Width + 20;
                labelBody.Location = new Point(118, labelBody.Location.Y);
            }
            else
            {
                labelBody.Location = new Point(12, labelBody.Location.Y);
            }
            if (newWidth >= 440)
            {
                this.Width = newWidth;
            }
            else
            {
                this.Width = 440;
            }

            int newHeight = labelBody.Size.Height + 100;
            if (newHeight >= 200)
            {
                this.Height = newHeight;
            }
            else
            {
                this.Height = 200;
            }
        }

        #endregion

        private void buttonYes_Click(object sender, EventArgs e)
        {
            result = MessageBoxResult.Yes;
            lastResult = MessageBoxResult.Yes;
            DialogResult = DialogResult.Yes;
        }

        private void buttonYestoAll_Click(object sender, EventArgs e)
        {
            result = MessageBoxResult.Yes;
            lastResult = MessageBoxResult.YesToAll;
            DialogResult = DialogResult.Yes;
        }

        private void buttonNo_Click(object sender, EventArgs e)
        {
            result = MessageBoxResult.No;
            lastResult = MessageBoxResult.No;
            DialogResult = DialogResult.No;
        }

        private void buttonNotoAll_Click(object sender, EventArgs e)
        {
            result = MessageBoxResult.No;
            lastResult = MessageBoxResult.NoToAll;
            DialogResult = DialogResult.No;
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            result = MessageBoxResult.Cancel;
            lastResult = MessageBoxResult.Cancel;
            DialogResult = DialogResult.Cancel;
        }
    }
}