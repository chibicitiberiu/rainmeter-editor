using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using RainmeterStudio.Business;
using RainmeterStudio.Core.Documents;
using RainmeterStudio.Core.Model;
using RainmeterStudio.UI.Controller;

namespace RainmeterStudio.UI.Dialogs
{
    /// <summary>
    /// Interaction logic for CreateProjectDialog.xaml
    /// </summary>
    public partial class CreateProjectDialog : Window
    {
        #region Commands

        private Command _createCommand;
        public Command CreateCommand
        {
            get
            {
                if (_createCommand == null)
                    _createCommand = new Command("CreateCommand", Create, Validate);

                return _createCommand;
            }
        }

        private Command _cancelCommand;
        public Command CancelCommand
        {
            get
            {
                if (_cancelCommand == null)
                    _cancelCommand = new Command("CancelCommand", Cancel);

                return _cancelCommand;
            }
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the currently selected file format
        /// </summary>
        public IDocumentTemplate SelectedTemplate
        {
            get
            {
                return listTemplates.SelectedItem as IDocumentTemplate;
            }
            set
            {
                listTemplates.SelectedItem = value;
            }
        }

        /// <summary>
        /// Gets or sets the path
        /// </summary>
        public string SelectedName
        {
            get
            {
                return textName.Text;
            }
            set
            {
                textName.Text = value;
            }
        }

        /// <summary>
        /// Gets or sets the path
        /// </summary>
        public string SelectedLocation
        {
            get
            {
                return textLocation.Text;
            }
            set
            {
                textLocation.Text = value;
            }
        }

        /// <summary>
        /// Gets or sets the path
        /// </summary>
        public string SelectedPath
        {
            get
            {
                return textPath.Text;
            }
            set
            {
                textPath.Text = value;
                _pathUserSet = true;
            }
        }

        #endregion

        #region Private fields

        private bool _pathUserSet = false;
        private bool _ignoreNextChange = false;

        #endregion

        public CreateProjectDialog(ProjectController projectController)
        {
            InitializeComponent();

            // Add event handlers
            textLocation.AddHandler(TextBoxBase.TextChangedEvent, new TextChangedEventHandler(textLocation_TextChanged));
            textPath.AddHandler(TextBoxBase.TextChangedEvent, new TextChangedEventHandler(textPath_TextChanged));

            // Set data context
            DataContext = this;

            // Populate templates
            listTemplates.ItemsSource = projectController.ProjectTemplates.OrderBy(x => x.DisplayText);

            // Focus on name textbox
            textName.Focus();
        }

        private void Create()
        {
            DialogResult = true;
            Close();
        }

        private void Cancel()
        {
            DialogResult = false;
            Close();
        }

        private bool Validate()
        {
            bool res = true;
            res &= !String.IsNullOrWhiteSpace(textPath.Text);
            res &= (listTemplates.SelectedItem != null);
            return res;
        }

        private void UpdatePath()
        {
            if (!_pathUserSet)
            {
                // Start with location
                string path = textLocation.Text;

                try
                {
                    // Combine with project directory
                    if (checkCreateDirectory.IsChecked.HasValue && checkCreateDirectory.IsChecked.Value)
                        path = System.IO.Path.Combine(path, textName.Text);

                    // Combine with project file name
                    path = System.IO.Path.Combine(path, textName.Text + ".rsproj");

                    // Set new value
                    _ignoreNextChange = true;
                    textPath.Text = path;
                }
                catch (ArgumentException)
                {
                }
            }
        }

        private void textName_TextChanged(object sender, TextChangedEventArgs e)
        {
            UpdatePath();
        }

        private void textLocation_TextChanged(object sender, TextChangedEventArgs e)
        {
            UpdatePath();
        }

        private void textPath_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (_ignoreNextChange)
            {
                _ignoreNextChange = false;
            }
            else
            {
                _pathUserSet = true;
            }
        }

        private void checkCreateDirectory_CheckChanged(object sender, RoutedEventArgs e)
        {
            UpdatePath();
        }

        private void listTemplates_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
        }
    }
}
