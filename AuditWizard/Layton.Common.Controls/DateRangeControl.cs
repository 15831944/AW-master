using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace Layton.Common.Controls
{
    public partial class DateRangeControl : UserControl
    {
        public DateRangeControl()
        {
            InitializeComponent();
            startDateCombo.Value = DateTime.Today;
            endDateCombo.Value = DateTime.Today;
        }

        public DateTime StartDate
        {
            get { return ((DateTime)startDateCombo.Value).Date; }
            set { startDateCombo.Value = value; }
        }

        public DateTime EndDate
        {
            get { return ((DateTime)endDateCombo.Value).Date.AddSeconds(24*60*60 - 1); }
            set { endDateCombo.Value = value; }
        }
    }
}
