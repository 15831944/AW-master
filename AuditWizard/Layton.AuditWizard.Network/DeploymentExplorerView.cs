using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using Microsoft.Practices.CompositeUI.SmartParts;
using Microsoft.Practices.CompositeUI.EventBroker;
using Microsoft.Practices.ObjectBuilder;
using Microsoft.Practices.CompositeUI;
using Infragistics.Win.UltraWinTree;
using Layton.Cab.Interface;

namespace Layton.Deviceshield.Deployment
{
    [SmartPart]
    public partial class DeploymentExplorerView : UserControl, ILaytonView
    {
        private WorkItem workItem;
        private DeploymentExplorerViewPresenter presenter;
        private UltraTreeNode rootNode;

        [Microsoft.Practices.ObjectBuilder.InjectionConstructor]
        public DeploymentExplorerView([ServiceDependency] WorkItem workItem) 
        {
            InitializeComponent();
            this.workItem = workItem;
            this.deploymentTree.Override.SelectionType = SelectType.ExtendedAutoDrag;
            this.Paint += new PaintEventHandler(DeploymentExplorerView_Paint);
        }

        [CreateNew]
        public DeploymentExplorerViewPresenter Presenter
        {
            set
            {
                presenter = value;
                presenter.View = this;
                presenter.Initialize();
                rootNode.Selected = true;
            }
        }

        public LaytonWorkItem WorkItem
        {
            get { return workItem as LaytonWorkItem; }
        }

        public void RefreshView()
        {
            presenter.Initialize();
            base.Refresh();
        }

        public void Clear()
        {
            deploymentTree.Nodes[EntireNetworkNode.Key].Nodes.Clear();
        }

        public UltraTreeNode EntireNetworkNode
        {
            get
            {
                if (rootNode == null)
                {
                    rootNode = deploymentTree.Nodes.Add(MiscStrings.EntireNetwork, MiscStrings.EntireNetwork);
                    rootNode.LeftImages.Add(Properties.Resources.entire_network16);
                }
                return rootNode; 
            }
        }

        void DeploymentExplorerView_Paint(object sender, PaintEventArgs e)
        {
            Image bgImg = Properties.Resources.ghosted_deployment_icon;
            e.Graphics.DrawImage(bgImg, (this.Width - bgImg.Width), (this.Height - bgImg.Height));
        }

        #region Event declarations and handlers

        /// <summary>
        /// Event declaration for when a computer node has been selected in the Deployment tree view.
        /// </summary>
        [EventPublication(EventTopics.ComputerSelectionChanged, PublicationScope.WorkItem)]
        public event EventHandler<ComputerEventArgs> ComputerSelectionChanged;

        /// <summary>
        /// Event declaration for when a group node has been selected in the Deployment tree view.
        /// </summary>
        [EventPublication(EventTopics.GroupSelectionChanged, PublicationScope.WorkItem)]
        public event EventHandler<GroupEventArgs> GroupSelectionChanged;

        private void deploymentTree_AfterSelect(object sender, SelectEventArgs e)
        {
            if (e.NewSelections.Count > 0)
            {
                UltraTreeNode node = e.NewSelections[0];

                // check if the 'Entire Network' node is selected
                if (node.Key == MiscStrings.EntireNetwork)
                {
                    List<ComputerGroup> group = new List<ComputerGroup>();
                    group.Add(presenter.GetGroup(node.Key));
                    if (GroupSelectionChanged != null)
                        GroupSelectionChanged(this, new GroupEventArgs(group));
                }
                else if (node.Parent == EntireNetworkNode)
                {
                    // fire GroupSelectionChanged event
                    List<ComputerGroup> groups = new List<ComputerGroup>();
                    foreach (UltraTreeNode groupNode in e.NewSelections)
                    {
                        ComputerGroup group = presenter.GetGroup(groupNode.Key);
                        groups.Add(group);
                    }
                    if (GroupSelectionChanged != null)
                        GroupSelectionChanged(this, new GroupEventArgs(groups));
                }
                else
                {
                    List<Computer> computers = new List<Computer>();
                    foreach (UltraTreeNode compNode in e.NewSelections)
                    {
                        Computer computer = presenter.GetComputer(compNode.Parent.Key, compNode.Key);
                        computers.Add(computer);
                    }
                    // fire ComnputerSelectionChanged event
                    if (ComputerSelectionChanged != null)
                        ComputerSelectionChanged(this, new ComputerEventArgs(computers));
                }
            }
        }

        #endregion

        #region Context Menu Handlers

        private void deployToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ((DeploymentWorkItemController)WorkItem.Controller).Deploy();
        }

        private void startToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ((DeploymentWorkItemController)WorkItem.Controller).Start();
        }

        private void stopToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ((DeploymentWorkItemController)WorkItem.Controller).Stop();
        }

        private void removeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ((DeploymentWorkItemController)WorkItem.Controller).Remove();
        }

        private void checkStatusMenuItem_Click(object sender, EventArgs e)
        {
            ((DeploymentWorkItemController)WorkItem.Controller).CheckStatus();
        }

        private void viewClientLogToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ((DeploymentWorkItemController)WorkItem.Controller).ViewLogFile();
        }

        private void clearClientLogToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ((DeploymentWorkItemController)WorkItem.Controller).ClearLogFile();
        }

        #endregion

        #region Drag and Drop Support

        private void deploymentTree_SelectionDragStart(object sender, EventArgs e)
        {
            UltraTreeNode node = deploymentTree.SelectedNodes[0];
            if (node.Text != MiscStrings.EntireNetwork && node.Parent != EntireNetworkNode)
            {
                deploymentTree.DoDragDrop(deploymentTree.SelectedNodes, DragDropEffects.Move); 
            }
        }

        private void deploymentTree_DragDrop(object sender, DragEventArgs e)
        {
            //Get the position of the mouse in the tree, as opposed to form coords
            Point currentPoint = deploymentTree.PointToClient(new Point(e.X, e.Y));

            //Get the node the mouse is over.
            UltraTreeNode node = deploymentTree.GetNodeFromPoint(currentPoint);

            if (node != null && node.Parent == EntireNetworkNode)
            {
                ((DeploymentWorkItemController)WorkItem.Controller).ChangeGroup(presenter.GetGroup(node.Text));
            }
            else
            {
                MessageBox.Show("Computers can only be dragged to a Domain", "Invalid Operation", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void deploymentTree_DragOver(object sender, DragEventArgs e)
        {
            //Get the position of the mouse in the tree, as opposed to form coords
            Point currentPoint = deploymentTree.PointToClient(new Point(e.X, e.Y));

            //Get the node the mouse is over.
            UltraTreeNode node = deploymentTree.GetNodeFromPoint(currentPoint);

            //Make sure the mouse is over a node
            if (node != null && node.Parent == EntireNetworkNode)
            {
                // allow dropping on this node
                e.Effect = DragDropEffects.Move;
            }
            else
            {
                e.Effect = DragDropEffects.None;
            }
        }

        #endregion

        private void deploymentTree_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                //Get the position of the mouse
                Point currentPoint = new Point(e.X, e.Y);

                //Get the node the mouse is over.
                UltraTreeNode node = deploymentTree.GetNodeFromPoint(currentPoint);

                if (node != null)
                {
                    if (!deploymentTree.SelectedNodes.Contains(node))
                    {
                        deploymentTree.SelectedNodes.Clear();
                        node.Selected = true;
                    }
                }
            }
        }
    }
}