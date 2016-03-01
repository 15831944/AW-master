using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Microsoft.Practices.CompositeUI.SmartParts;
using Microsoft.Practices.CompositeUI;
using Microsoft.Practices.ObjectBuilder;
using Infragistics.Practices.CompositeUI.WinForms;
using Infragistics.Win.UltraWinExplorerBar;
using Infragistics.Win.UltraWinTabControl;

namespace AuditWizardv8
{
    [SmartPart]
    public partial class LaytonSettingsFormShell : Form
    {
        private LaytonSettingsFormPresenter presenter;

        public LaytonSettingsFormShell()
        {
            InitializeComponent();
            settingsTabWorkspace.Style = Infragistics.Win.UltraWinTabControl.UltraTabControlStyle.Wizard;
            settingsTabWorkspace.TabPageMargins.Bottom = 4;
            settingsTabWorkspace.TabPageMargins.Left = 4;
            settingsTabWorkspace.TabPageMargins.Right = 4;
            settingsTabWorkspace.TabPageMargins.Top = 4;
            
            try
            {
                // set the form's icon
                this.Icon = new Icon(Properties.Settings.Default.appIcon);
            }
            catch
            {
                // use the existing default icon
            }
        }

        public UltraTabWorkspace TabWorkspace
        {
            get { return settingsTabWorkspace; }
        }

        [CreateNew]
        public LaytonSettingsFormPresenter Presenter
        {
            set 
            { 
                presenter = value;
                presenter.View = this;
                presenter.Initialize();
            }
            get { return presenter; }
        }

        public void AddListItem(Image image, string title, UltraTab tab)
        {
            if (!settingsExplorerBar.Groups[0].Items.Exists(title))
            {
                UltraExplorerBarItem item = new UltraExplorerBarItem(title);
                item.Text = title;
                item.Key = title;
                item.Tag = tab;
                item.Settings.AppearancesLarge.Appearance.Image = image;
                this.settingsExplorerBar.Groups[0].Items.Add(item);
            }
        }

        public void SetPanelSize(Size panelSize)
        {
            this.Width = 137 + panelSize.Width;
            this.Height = 40 + panelSize.Height;
        }

        private void settingsExplorerBar_ActiveItemChanged(object sender, ItemEventArgs e)
        {
            UltraTab tab = (UltraTab)e.Item.Tag;
            TabWorkspace.SelectedTab = tab;
        }
    }
}