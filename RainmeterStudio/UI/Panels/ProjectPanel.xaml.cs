using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using RainmeterStudio.Core.Model;
using RainmeterStudio.Core.Utils;
using RainmeterStudio.Interop;
using RainmeterStudio.Storage;
using RainmeterStudio.UI.Controller;
using RainmeterStudio.UI.ViewModel;

namespace RainmeterStudio.UI.Panels
{
    /// <summary>
    /// Interaction logic for SkinsPanel.xaml
    /// </summary>
    public partial class ProjectPanel : UserControl
    {
        private ProjectController _controller;
        public ProjectController Controller
        {
            get
            {
                return _controller;
            }
            set
            {
                // Unsubscribe from old controller
                if (_controller != null)
                {
                    Controller.ActiveProjectChanged -= Controller_ActiveProjectChanged;
                }

                // Set new project
                _controller = value;
                _controller.ActiveProjectChanged += Controller_ActiveProjectChanged;
                Refresh();
            }
        }

        #region Commands

        public Command SyncWithActiveViewCommand { get; private set; }

        public Command RefreshCommand { get; private set; }

        public Command ExpandAllCommand { get; private set; }

        public Command CollapseAllCommand { get; private set; }

        public Command ShowAllFilesCommand { get; private set; }

        #endregion

        /// <summary>
        /// Gets the selected tree view item
        /// </summary>
        public Tree<Reference> ActiveItem
        {
            get
            {
                var selected = treeProjectItems.SelectedItem as Tree<ReferenceViewModel>;

                if (selected == null)
                {
                    return Controller.ActiveProject.Root;
                }
                else
                {
                    return selected.Data.Reference;
                }
            }
        }

        private bool _canExpand = false;
        private bool CanExpand
        {
            get
            {
                return _canExpand;
            }
            set
            {
                _canExpand = value;

                ExpandAllCommand.NotifyCanExecuteChanged();
                CollapseAllCommand.NotifyCanExecuteChanged();
            }
        }


        public ProjectPanel()
        {
            InitializeComponent();

            SyncWithActiveViewCommand = new Command("ProjectPanel_SyncWithActiveViewCommand", SyncWithActiveView);
            RefreshCommand = new Command("ProjectPanel_RefreshCommand", Refresh);
            ExpandAllCommand = new Command("ProjectPanel_ExpandAllCommand", ExpandAll, () => _canExpand);
            CollapseAllCommand = new Command("ProjectPanel_CollapseAllCommand", CollapseAll, () => !_canExpand);
            ShowAllFilesCommand = new Command("ProjectPanel_ShowAllFilesCommand", Refresh);

            this.DataContext = this;
            Refresh();
        }

        void Controller_ActiveProjectChanged(object sender, EventArgs e)
        {
            Refresh();
        }

        private void SyncWithActiveView()
        {
            // TODO: implement
        }

        private void Refresh()
        {
            if (Controller == null || Controller.ActiveProject == null)
            {
                this.IsEnabled = false;
            }
            else
            {
                this.IsEnabled = true;

                // Get tree
                Tree<ReferenceViewModel> tree;
                if (toggleShowAllFiles.IsChecked.HasValue && toggleShowAllFiles.IsChecked.Value)
                {
                    tree = GetAllFiles();
                }
                else
                {
                    tree = GetProjectItems();
                }

                // Add tree to tree view
                treeProjectItems.Items.Clear();
                treeProjectItems.Items.Add(tree);
            }
        }

        private Tree<ReferenceViewModel> GetAllFiles()
        {
            // Get directory name
            string projectFolder = System.IO.Path.GetDirectoryName(Controller.ActiveProjectPath);

            // Get folder tree
            Tree<Reference> refTree = DirectoryHelper.GetFolderTree(projectFolder);
            refTree.Data = Controller.ActiveProject.Root.Data;

            // Remove the project file from the list
            Tree<Reference> project = refTree.First(x => DirectoryHelper.PathsEqual(x.Data.StoragePath, Controller.ActiveProjectPath));
            refTree.Remove(project);

            // Transform to reference view model and return
            return refTree.Transform<Reference, ReferenceViewModel>((node) => new Tree<ReferenceViewModel>(new ReferenceViewModel(node)));
        }

        private Tree<ReferenceViewModel> GetProjectItems()
        {
            // Get project items
            Tree<Reference> refTree = Controller.ActiveProject.Root;

            // Transform to reference view model and return
            return refTree.Transform<Reference, ReferenceViewModel>((node) => new Tree<ReferenceViewModel>(new ReferenceViewModel(node)));
        }

        private void ExpandAll()
        {
            // Get tree
            var tree = treeProjectItems.Items[0] as Tree<ReferenceViewModel>;
            if (tree == null)
                return;

            // Expand all
            tree.Apply((node) => node.Data.IsExpanded = true);

            // Set can expand property
            CanExpand = false;
        }

        private void CollapseAll()
        {
            // Get tree
            var tree = treeProjectItems.Items[0] as Tree<ReferenceViewModel>;
            if (tree == null)
                return;

            // Expand all
            tree.Apply((node) => node.Data.IsExpanded = false);

            // Set can expand property
            CanExpand = true;
        }

        void TreeViewItem_ExpandedOrCollapsed(object sender, RoutedEventArgs e)
        {
            // Get tree
            var tree = treeProjectItems.Items[0] as Tree<ReferenceViewModel>;
            if (tree == null)
                return;
            
            // We can expand if the root is not expanded
            CanExpand = (!tree.Data.IsExpanded);
        }
    }
}
