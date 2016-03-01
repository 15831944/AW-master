using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Text;
using System.Windows.Forms;

namespace Layton.Common.Controls
{
	public partial class ShadedImageForm : Form
	{
		public void SetBitmap(Bitmap footerBitmap)
		{
			footerPictureBox.Image = footerBitmap;
		}

		public ShadedImageForm()
		{
			InitializeComponent();
		}

		private void ShadedImageForm_Paint(object sender, PaintEventArgs e)
		{
			LinearGradientBrush brush = new LinearGradientBrush(new Point(0, 0), new Point(0, 100), Color.FromArgb(228, 228, 235), Color.White);
			e.Graphics.FillRectangle(brush, new Rectangle(0, 0, Width, 100));
		}
	}
}