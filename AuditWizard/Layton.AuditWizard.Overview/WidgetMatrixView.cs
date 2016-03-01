using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using Infragistics.Win.Layout;

//using Layton.AuditWizard

namespace Layton.AuditWizard.Overview
{
    public partial class WidgetMatrixView : UserControl
    {
        public int MaxWidgets = 8;
        private int rowCount = 2;
        private int columnCount = 2;

		/// <summary>This is a list of the widgets being displayed at this time</summary>
		List<DefaultWidget> _listWidgets = new List<DefaultWidget>();

        public WidgetMatrixView()
        {
            InitializeComponent();
			_listWidgets.Clear();
        }

		/// <summary>
		/// Called to add a new widget to the collection, note that we may have to add a new row or column
		/// to the display in order that the widget can be displayed
		/// </summary>
		/// <param name="widget"></param>
		public bool Add(DefaultWidget widget)
		{			
			// we have a free space in the grid so add this widget into the grid 
			_listWidgets.Add(widget);
			this.Controls.Add(widget);
			layoutManager.SetIncludeInLayout(widget ,true);
			
			// Re-Layout the widgets
            if (_listWidgets.Count == 4)
                LayoutWidgets();

			return true;
		}
		


		/// <summary>
		/// Called to remove a widget from the collection, note that we may have to delete rows/columns
		/// from the display to clean up after this widget is removed
		/// </summary>
		/// <param name="widget"></param>
		public bool Remove(DefaultWidget widgetToRemove)
		{
			// Remove the control from the form
			this.Controls.Remove(widgetToRemove);
			
			// ...and from our internal list
			_listWidgets.Remove(widgetToRemove);
			
			// We have lost one item - if the column count is greater than the row count then reduce the columns
			// otherwise reduce the row to keep our required shape.
			if (columnCount > rowCount)
				columnCount--;
			else
				rowCount--;

			// Re-Layout the widgets
			LayoutWidgets();
			
			return true;
		}
		
		
		protected void LayoutWidgets()
		{
			layoutManager.SuspendLayout();
			
			// Populate the gridbag constraints for all of the controls first
			foreach (DefaultWidget widget in _listWidgets)
			{
				layoutManager.ResetGridBagConstraint(widget);
			}
						
			// Now relay out the controls as we need to give them new GridBagConstraint
			int row = 1;
			int column = 1;
            foreach (DefaultWidget widget in _listWidgets)
            {
                layoutManager.SetGridBagConstraint(widget, new GridBagConstraint(column, row, 1, 1, 0, 0, AnchorType.Center, FillType.Both, new Insets()));
                if (column < columnCount)
                {
                    column++;
                }
                else
                {
                    column = 1;
                    row++;
                }
            }

			layoutManager.ResumeLayout();
			layoutManager.PerformLayout();

            //RefreshWidgets();
		}		
		
				
		public void RefreshWidgets()
		{
			foreach (DefaultWidget widget in _listWidgets)
			{
				widget.RefreshWidget();
			}		
		}

        public int RowCount
        {
            get { return rowCount; }
        }

        public int ColumnCount
        {
            get { return columnCount; }
        }
    }
}
