using System;
using System.Collections.Generic;
using System.Linq;
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
using RainmeterEditor.Model.Events;
using RainmeterEditor.UI.Controller;
using Xceed.Wpf.AvalonDock.Layout;

namespace RainmeterEditor.UI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private DocumentController documentController;

        private IEnumerable<Command> Commands
        {
            get
            {
                yield return documentController.DocumentCreateCommand;
            }
        }

        public MainWindow()
        {
            InitializeComponent();

            documentController = new DocumentController();
            documentController.OwnerWindow = this;
            documentController.DocumentOpened += documentController_DocumentOpened;

            foreach (var c in Commands)
            {
                Resources.Add(c.Name, c);

                if (c.Shortcut != null)
                    InputBindings.Add(new KeyBinding(c, c.Shortcut));
            }
        }

        void documentController_DocumentOpened(object sender, DocumentOpenedEventArgs e)
        {
            // Spawn a new window
            LayoutDocument document = new LayoutDocument();
            document.Content = e.Editor.EditorUI;
            document.Title = e.Editor.Title;
            document.Closing += document_Closing;

            documentPane.Children.Add(document);
            documentPane.SelectedContentIndex = documentPane.IndexOf(document);
        }

        void document_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            switch (MessageBox.Show("Are you sure?", "", MessageBoxButton.YesNoCancel, MessageBoxImage.Question))
            {
                case MessageBoxResult.Yes:
                    break;

                case MessageBoxResult.No:
                    break;

                default:
                    e.Cancel = true;
                    return;
            }
        }
    }
}