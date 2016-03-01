using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace Layton.NetworkDiscovery
{
    public partial class IpAddressRangeForm : Form
    {
        private bool _cancelClicked;

        public IpAddressRangeForm()
        {
            InitializeComponent();
            _cancelClicked = false;
        }

        public IpAddressRangeForm(string aStartIPAddress, string aEndIPAddress)
        {
            InitializeComponent();
            _cancelClicked = false;
            startIpAddress.Text = aStartIPAddress;
            endIpAddress.Text = aEndIPAddress;
        }

        public string StartAddress
        {
            get { return startIpAddress.ToString(); }
            set { startIpAddress.Text = value; }
        }

        public string EndAddress
        {
            get { return endIpAddress.ToString(); }
            set { endIpAddress.Text = value; }
        }

        private void okButton_Click(object sender, EventArgs e)
        {
        }

        private void IpAddressRangeForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (_cancelClicked) return;

            byte[] startBytes = startIpAddress.GetAddressBytes();
            byte[] endBytes = endIpAddress.GetAddressBytes();
            for (int i = 0; i < 4; i++)
            {
                if (startBytes[i] > endBytes[i])
                {
                    e.Cancel = true;
                    MessageBox.Show("The Start Address must be smaller than the End Address", "Invalid IP Range", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                    return;
                }
                if (startBytes[i] < endBytes[i])
                {
                    e.Cancel = false;
                    return;
                }
            }
        }

        private void IpAddressRangeForm_Paint(object sender, PaintEventArgs e)
        {
            LinearGradientBrush brush = new LinearGradientBrush(new Point(0, 0), new Point(0, 100), Color.FromArgb(228, 228, 235), Color.White);
            e.Graphics.FillRectangle(brush, new Rectangle(0, 0, Width, 100));
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            _cancelClicked = true;
        }
    }
}