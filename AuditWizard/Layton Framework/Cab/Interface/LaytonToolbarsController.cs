using Infragistics.Win.UltraWinToolbars;
using Microsoft.Practices.CompositeUI;

namespace Layton.Cab.Interface
{
    public abstract class LaytonToolbarsController : Controller
    {
        new LaytonWorkItem WorkItem { get { return WorkItem; } }
        public abstract RibbonTab RibbonTab { get; }
        public abstract void Initialize();
        public abstract void UpdateTools();
    }
}
