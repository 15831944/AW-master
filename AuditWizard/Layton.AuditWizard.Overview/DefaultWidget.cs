using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace Layton.AuditWizard.Overview
{
    public partial class DefaultWidget : UserControl
    {
		public static string _widgetName = "default";
		
        public DefaultWidget()
        {
            InitializeComponent();
        }

		public virtual string WidgetName()
		{ return _widgetName; }
        
        public virtual void RefreshWidget()
        {
		}
    }
}
