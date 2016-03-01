using System;
using System.Collections.Generic;
using System.Text;

namespace Layton.AuditWizard.Administration
{
	/// <summary>
	/// This interface is used by the tab views in the applications module as they all need to save their
	/// data when the user moves off the tab.
	/// </summary>
	public interface IAdministrationView
	{
		void Save();
		void Activate();
	}
}
