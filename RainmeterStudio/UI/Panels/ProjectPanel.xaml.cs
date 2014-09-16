using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using RainmeterStudio.Core.Model;
using RainmeterStudio.Core.Utils;
using RainmeterStudio.UI.Controller;
using RainmeterStudio.UI.ViewModel;

namespace RainmeterStudio.UI.Panels
{
    /// <summary>
    /// Interaction logic for SkinsPanel.xaml
    /// </summary>
    public partial class ProjectPanel : UserControl
    {
        private ProjectController _projectController;

        public ProjectController ProjectController
        {
            get
            {
                return _projectController;
            }
            set
            {
                // Unsubscribe from old controller
                if (_projectController != null)
                {
                    ProjectController.ActiveProjectChanged -= Controller_ActiveProjectChanged;
                }

                // Set new project
                _projectController = value;
                _projectController.ActiveProjectChanged += Controller_ActiveProjectChanged;
                Refresh();
            }
        }

        public DocumentController DocumentController { get; set; }

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
        public Reference ActiveItem
        {
            get
            {
                var selected = treeProjectItems.SelectedItem as Tree<ReferenceViewModel>;

                if (selected == null)
                {
                    return ProjectController.ActiveProject.Root;
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
            // Clear current items
            treeProjectItems.Items.Clear();

            // No project
            if (ProjectController == null || ProjectController.ActiveProject == null)
            {
                this.IsEnabled = false;
            }
            else
            {
                this.IsEnabled = true;

                // Get tree
                Reference refTree;

                if (toggleShowAllFiles.IsChecked.HasValue && toggleShowAllFiles.IsChecked.Value)
                {
                    // Get directory name
                    string projectFolder = System.IO.Path.GetDirectoryName(ProjectController.ActiveProjectPath);

                    // Get folder tree
                    refTree = DirectoryHelper.GetFolderTree(projectFolder);
                }
                else
                {
                    refTree = ProjectController.ActiveProject.Root;
                }

                // Add tree to tree view
                treeProjectItems.Items.Add(new ReferenceViewModel(refTree));
            }
        }

        private void ExpandAll()
        {
            // Get tree
            var tree = treeProjectItems.Items[0] as ReferenceViewModel;
            if (tree == null)
                return;

            // Expand all
            tree.TreeExpand(true);

            // Set can expand property
            CanExpand = false;
        }

        private void CollapseAll()
        {
            // Get tree
            var tree = treeProjectItems.Items[0] as ReferenceViewModel;
            if (tree == null)
                return;

            // Expand all
            tree.TreeExpand(false);

            // Set can expand property
            CanExpand = true;
        }

        void TreeViewItem_ExpandedOrCollapsed(object sender, RoutedEventArgs e)
        {
            // Get tree
            var tree = treeProjectItems.Items[0] as ReferenceViewModel;
            if (tree == null)
                return;
            
            // We can expand if the root is not expanded
            CanExpand = (!tree.IsExpanded);
        }

        private void TreeViewItem_PreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            var treeViewItem = sender as TreeViewItem;
            var referenceViewModel = treeViewItem.Header as ReferenceViewModel;

            if (referenceViewModel != null)
            {
                treeViewItem.ContextMenu = new ContextMenu();
                treeViewItem.ContextMenu.ItemsSource = GetContextMenuItems(referenceViewModel.Reference);
            }
        }

        private void TreeViewItem_PreviewMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var treeViewItem = sender as TreeViewItem;
            var refViewModel = treeViewItem.Header as ReferenceViewModel;

            if (refViewModel != null)
            {
                Command command = GetDefaultCommand(refViewModel.Reference);
                command.Execute(refViewModel.Reference);
            }
        }

        /// <summary>
        /// Gets the default command (double click) for a specific reference
        /// </summary>
        /// <param name="reference">The reference</param>
        /// <returns>The command</returns>
        public Command GetDefaultCommand(Reference reference)
        {
            switch (reference.TargetKind)
            {
                case ReferenceTargetKind.File:
                    return DocumentController.DocumentOpenCommand;

                case ReferenceTargetKind.Project:
                    return DocumentController.DocumentOpenCommand;

                case ReferenceTargetKind.Directory:
                    return null; // TODO: expand command

                default:
                    return null;
            }
        }

        private MenuItem GetMenuItem(Command cmd, Reference reference)
        {
            var icon = new Image();
            icon.Source = cmd.Icon;
            icon.Width = 16;
            icon.Height = 16;

            var menuItem = new MenuItem();
            menuItem.DataContext = cmd;
            menuItem.Style = Application.Current.TryFindResource("CommandContextMenuItemStyle") as Style;
            menuItem.Icon = icon;
            menuItem.CommandParameter = reference;

            if (GetDefaultCommand(reference) == cmd)
                menuItem.FontWeight = FontWeights.Bold;

            return menuItem;
        }

        public IEnumerable<UIElement> GetContextMenuItems(Reference @ref)
        {
            if (@ref.TargetKind == ReferenceTargetKind.File || @ref.TargetKind == ReferenceTargetKind.Project)
            {
                yield return GetMenuItem(DocumentController.DocumentOpenCommand, @ref);
            }
            if (@ref.TargetKind == ReferenceTargetKind.Directory || @ref.TargetKind == ReferenceTargetKind.Project)
            {
                // TODO: expand command
            }

            yield return new Separator();

            if (@ref.TargetKind != ReferenceTargetKind.Project)
            {
                yield return GetMenuItem(ProjectController.ProjectItemCutCommand, @ref);
                yield return GetMenuItem(ProjectController.ProjectItemCopyCommand, @ref);

                if (@ref.TargetKind == ReferenceTargetKind.Directory)
                    yield return GetMenuItem(ProjectController.ProjectItemPasteCommand, @ref);
            }

            yield return GetMenuItem(ProjectController.ProjectItemRenameCommand, @ref);

            if (@ref.TargetKind != ReferenceTargetKind.Project)
                yield return GetMenuItem(ProjectController.ProjectItemDeleteCommand, @ref);

            yield return new Separator();

            if (@ref.TargetKind == ReferenceTargetKind.Directory)
                yield return GetMenuItem(ProjectController.ProjectItemOpenInExplorerCommand, @ref);
            else
                yield return GetMenuItem(ProjectController.ProjectItemOpenContainingFolderCommand, @ref);
        }
    }
}
