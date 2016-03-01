using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
//
using Infragistics.Win.UltraWinTree;
//
using Layton.AuditWizard.DataAccess;
using Layton.Common.Controls;

namespace Layton.AuditWizard.Common
{
	public partial class FormSelectComputers : ShadedImageForm
	{
		public string SelectedComputers
		{
			get 
			{ 
				string selectedComputers = "";
				foreach (string name in clbComputers.CheckedItems)
				{
					if (selectedComputers == "")
						selectedComputers = name;
					else
						selectedComputers = selectedComputers + ";" + name;
				}
				return selectedComputers;
			}
		}

		public FormSelectComputers(InstalledApplication installedApplication)
		{
			InitializeComponent();

			// Load the data into the form - note that we are loading the computers on which the application
			// has been installed and not the entire list of computers
			foreach (ApplicationInstance instance in installedApplication.Instances)
			{
				clbComputers.Items.Add(instance.InstalledOnComputer);
			}
		}

	}
}