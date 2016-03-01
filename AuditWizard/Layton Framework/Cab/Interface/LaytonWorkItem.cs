using Microsoft.Practices.CompositeUI.SmartParts;
using Microsoft.Practices.CompositeUI;
using Infragistics.Practices.CompositeUI.WinForms;

namespace Layton.Cab.Interface
{
    public abstract class LaytonWorkItem : WorkItem
    {
        // Hold the properties for this WorkItem
        private string title;
        private string description;
        private System.Drawing.Image image;

        // Expose all the necessary critical LaytonWorkItem objects
        public virtual ILaytonView ExplorerView { get { return Items[ViewNames.MainExplorerView] as ILaytonView; } }
        public virtual ILaytonView TabView { get { return Items[ViewNames.MainTabView] as ILaytonView; } }
        public virtual ILaytonView SettingsView { get { return Items[ViewNames.SettingsTabView] as ILaytonView; } }
        public virtual LaytonWorkItemController Controller { get { return Items[ControllerNames.WorkItemController] as LaytonWorkItemController; } }
        public virtual LaytonToolbarsController ToolbarsController { get { return Items[ControllerNames.ToolbarsController] as LaytonToolbarsController; } }

        /// <summary>
        /// The text that will be displayed for this module in the ExplorerWorkspace's GroupTab
        /// </summary>
        public virtual string Title
        {
            get { return title; }
            set { this.title = value; }
        }

        /// <summary>
        /// The description (or ToolTip) that will be displayed for this module in the ExplorerWorkspace's GroupTab
        /// </summary>
        public virtual string Description
        {
            get { return description; }
            set { this.description = value; }
        }

        /// <summary>
        /// The 32x32 image that will be displayed in the ExplorerWorkspaces's GroupTab
        /// </summary>
        public virtual System.Drawing.Image Image 
        {
            get { return image; }
            set { this.image = value; } 
        }
    }
}