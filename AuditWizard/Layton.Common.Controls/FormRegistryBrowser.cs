using System;
using System.Drawing;
using System.Windows.Forms;
using Microsoft.Win32;

namespace Layton.Common.Controls
{
    public partial class FormRegistryBrowser : AWForm
    {
        public FormRegistryBrowser(bool hKeyLocalMachineOnly)
        {
            InitializeForm(hKeyLocalMachineOnly);
        }

        public string RegKey { get; set; }
        public string RegValue { get; set; }

        private void InitializeForm(bool hKeyLocalMachineOnly)
        {
            InitializeComponent();

            if (hKeyLocalMachineOnly)
            {
                AddNodes(Registry.LocalMachine);
                tvRegistry.SelectedNode = tvRegistry.Nodes[0];
            }
            else
            {
                AddNodes(Registry.ClassesRoot);
                AddNodes(Registry.CurrentConfig);
                AddNodes(Registry.CurrentUser);
                AddNodes(Registry.LocalMachine);
                AddNodes(Registry.Users);

                tvRegistry.SelectedNode = tvRegistry.Nodes[3];
            }
        }

        private void AddNodes(RegistryKey key)
        {
            ImageList list = new ImageList();
            list.Images.Add("one", Properties.Resources.add16);
            TreeNode rootNode = new TreeNode(key.Name) { Tag = key, ImageKey = "one"};
            tvRegistry.Nodes.Add(rootNode);

            if (Registry.LocalMachine.SubKeyCount > 0) rootNode.Nodes.Add("");
        }

        private void DisplayValues(RegistryKey rkey)
        {
            lvItems.Items.Clear();

            string[] subKeys = rkey.GetValueNames();

            foreach (String vstr in subKeys)
            {
                ListViewItem item1 = new ListViewItem { Text = vstr == "" ? "Default" : vstr };
                lvItems.Items.Add(item1);
            }
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            RegKey = "";
            RegValue = "";

            if (tvRegistry.SelectedNode != null) RegKey = tvRegistry.SelectedNode.FullPath;
            if (lvItems.SelectedItems.Count > 0) RegValue = lvItems.SelectedItems[0].Text;

            Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void tvRegistry_BeforeExpand(object sender, TreeViewCancelEventArgs e)
        {
            try
            {
                Cursor.Current = Cursors.WaitCursor;

                RegistryKey expandingKey = e.Node.Tag as RegistryKey;

                if (expandingKey == null) return;

                e.Node.Nodes.Clear();

                foreach (string subKey in expandingKey.GetSubKeyNames())
                {
                    TreeNode subKeyNode = new TreeNode(subKey);
                    try
                    {
                        RegistryKey subKeyNodeTag = expandingKey.OpenSubKey(subKey);
                        subKeyNode.Tag = subKeyNodeTag;

                        if (subKeyNodeTag != null && subKeyNodeTag.SubKeyCount > 0)
                            subKeyNode.Nodes.Add("");

                        e.Node.Nodes.Add(subKeyNode);
                    }
                    catch (System.Security.SecurityException)
                    {
                        subKeyNode.Text += " (access denied)";
                        subKeyNode.ForeColor = Color.Gray;
                        e.Node.Nodes.Add(subKeyNode);
                    }
                }
            }
            finally
            {
                Cursor.Current = Cursors.Default;
            }
        }

        private void tvRegistry_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (e.Node.Tag != null)
            {
                DisplayValues(e.Node.Tag as RegistryKey);
                btnOk.Enabled = lvItems.SelectedItems.Count == 1;
            }
        }

        private void lvItems_SelectedIndexChanged(object sender, EventArgs e)
        {
            btnOk.Enabled = lvItems.SelectedItems.Count == 1;
        }
    }
}
