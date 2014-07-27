using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using RainmeterStudio.Interop;
using RainmeterStudio.Model;
using RainmeterStudio.Storage;
using RainmeterStudio.UI.Controller;
using RainmeterStudio.Utils;

namespace RainmeterStudio.UI
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
                // Unsubscribe from old project
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

        private Command _syncWithActiveViewCommand;
        public Command SyncWithActiveViewCommand
        {
            get
            {
                if (_syncWithActiveViewCommand == null)
                {
                    _syncWithActiveViewCommand = new Command("ProjectPanel_SyncWithActiveViewCommand", SyncWithActiveView);
                }
                return _syncWithActiveViewCommand;
            }
        }

        private Command _refreshCommand;
        public Command RefreshCommand
        {
            get
            {
                if (_refreshCommand == null)
                {
                    _refreshCommand = new Command("ProjectPanel_RefreshCommand", SyncWithActiveView)
                    {
                        Shortcut = new KeyGesture(Key.F5)
                    };
                }
                return _refreshCommand;
            }
        }

        private Command _expandAllCommand;
        public Command ExpandAllCommand
        {
            get
            {
                if (_expandAllCommand == null)
                {
                    _expandAllCommand = new Command("ProjectPanel_ExpandAllCommand", SyncWithActiveView);
                }
                return _expandAllCommand;
            }
        }

        private Command _collapseAllCommand;
        public Command CollapseAllCommand
        {
            get
            {
                if (_collapseAllCommand == null)
                {
                    _collapseAllCommand = new Command("ProjectPanel_CollapseAllCommand", SyncWithActiveView);
                }
                return _collapseAllCommand;
            }
        }

        private Command _showAllFilesCommand;
        public Command ShowAllFilesCommand
        {
            get
            {
                if (_showAllFilesCommand == null)
                {
                    _showAllFilesCommand = new Command("ProjectPanel_ShowAllFilesCommand", SyncWithActiveView);
                }
                return _showAllFilesCommand;
            }
        }

        public ProjectPanel()
        {
            InitializeComponent();

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

                // Display all files in the project directory
                if (toggleShowAllFiles.IsChecked.HasValue && toggleShowAllFiles.IsChecked.Value)
                {
                    string projectFolder = System.IO.Path.GetDirectoryName(Controller.ActiveProjectPath);
                    var tree = DirectoryHelper.GetFolderTree(projectFolder);
                    tree.Data = Controller.ActiveProject.Root.Data;
            
                    treeProjectItems.Items.Clear();
                    treeProjectItems.Items.Add(tree);
                }

                // Display only the project items
                else
                {
                    treeProjectItems.Items.Clear();
                    treeProjectItems.Items.Add(Controller.ActiveProject.Root);
                }
            }
        }

        private void toggleShowAllFiles_Checked(object sender, RoutedEventArgs e)
        {
            Refresh();
        }

        private void toggleShowAllFiles_Unchecked(object sender, RoutedEventArgs e)
        {
            Refresh();
        }
    }
}
