using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Practices.CompositeUI;

namespace Layton.AuditWizard.Overview
{
	public class WidgetManager
	{
		private List<DefaultWidget> _listWidgets = new List<DefaultWidget>();
		private WidgetMatrixView _widgetView;

        public WidgetManager(WidgetMatrixView widgetView, [ServiceDependency] WorkItem workItem)
		{
			_widgetView = widgetView;
			
			// Add all known widgets to our internal list here
			_listWidgets.Add(new NewsFeedWidget());
			_listWidgets.Add(new InventoryWidget());
            _listWidgets.Add(new ApplicationComplianceWidget(workItem));
            _listWidgets.Add(new ServerDataWidget());
		}


		/// <summary>
		/// Add a widget to the display
		/// </summary>
		/// <param name="widgetName"></param>
		/// <returns></returns>
		public bool DisplayWidget(string widgetName)
		{
			DefaultWidget widget = GetWidget(widgetName);
			if (widget == null)
				return false;
				
			// OK so this is a valid widget - however is it already displayed?  
			// If not then we should add it
			if (!IsWidgetDisplayed(widgetName))
				_widgetView.Add(widget);
				
			return true;
		}



		/// <summary>
		/// Remove a widget from the display
		/// </summary>
		/// <param name="widgetName"></param>
		/// <returns></returns>
		public bool RemoveWidget(string widgetName)
		{
			DefaultWidget widget = GetWidget(widgetName);
			if (widget == null)
				return false;

			// OK so this is a valid widget - however is it already displayed?  
			// If so then we should remove it
			if (IsWidgetDisplayed(widgetName))
				_widgetView.Remove(widget);
				
			return true;
		}
		
		/// <summary>
		/// Return the named widget if any or null if not found
		/// </summary>
		/// <param name="name"></param>
		/// <returns></returns>
		public DefaultWidget GetWidget (string name)
		{
			foreach (DefaultWidget widget in _listWidgets)
			{
				if (widget.WidgetName() == name)
					return widget;
			}
			return null;
		}
		
		
		/// <summary>
		/// Is the current named widget displayed?
		/// </summary>
		/// <param name="name"></param>
		/// <returns></returns>
		public bool IsWidgetDisplayed(string name)
		{
			foreach (DefaultWidget widget in _listWidgets)
			{
				string widgetName = widget.WidgetName();
				if (widget.WidgetName() == name) 
				{
					if (_widgetView.Controls.Contains(widget))
						return true;
				}
			}
			return false;
		}
		
		public List<string> GetWidgetNames(bool displayedOnly)
		{
			List<string> names = new List<string>();
			foreach (DefaultWidget widget in _listWidgets)
			{
				if ((!displayedOnly) || IsWidgetDisplayed(widget.WidgetName()))
					names.Add(widget.WidgetName());
			}
			return names;
		}

        public List<DefaultWidget> GetAllWidgets()
        {
            return _listWidgets;
        }
	}
}
