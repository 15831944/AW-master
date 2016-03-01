using System;
using System.Collections.Generic;
using System.Text;
using Infragistics.Win;
using Infragistics.Win.UltraWinExplorerBar;

namespace AuditWizardv8
{
    public class NavigationOverflowButtonFilter : IUIElementCreationFilter
    {
        public void AfterCreateChildElements(UIElement parent)
        {
        }

        public bool BeforeCreateChildElements(UIElement parent)
        {
            if (parent is NavigationOverflowButtonAreaUIElement)
            {
                return true;
            }
            return false;
        }
    }
}
