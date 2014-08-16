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
using RainmeterStudio.Core.Utils;
using RainmeterStudio.Properties;
using RainmeterStudio.UI.Controller;

namespace RainmeterStudio.UI.Dialogs
{
    /// <summary>
    /// Interaction logic for CreateProjectDialog.xaml
    /// </summary>
    public partial class CreateProjectDialog : Window
    {
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
        private ProjectController _projectController;

        #endregion

        /// <summary>
        /// Initializes the create project dialog
        /// </summary>
        /// <param name="projectController">Project controller</param>
        public CreateProjectDialog(ProjectController projectController)
        {
            InitializeComponent();

            _projectController = projectController;

            // Add event handlers
            textLocation.AddHandler(TextBoxBase.TextChangedEvent, new TextChangedEventHandler(textLocation_TextChanged));

            // Populate controls
            listTemplates.ItemsSource = projectController.ProjectTemplates.OrderBy(x => x.DisplayText);

            textLocation.ItemsSource = GetRecentLocations().OrderBy(x => x);

            textLocation.Text = GetLocation();

            Validate();

            // Focus on name textbox
            textName.Focus();
        }

        private string GetLocation()
        {
            // Get setting
            string location = Settings.Default.CreateProjectDialog_SavedLocation;

            // No location provided, use default
            if (String.IsNullOrEmpty(location))
                return System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "Rainmeter Studio Projects");

            return location;
        }

        private IEnumerable<string> GetRecentLocations()
        {
            return Settings.Default.CreateProjectDialog_RecentLocations
                .Split(new[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
        }

        private void Validate()
        {
            bool res = true;
            res &= (listTemplates.SelectedItem != null); 
            res &= !String.IsNullOrWhiteSpace(textPath.Text);
            res &= PathHelper.IsPathValid(textPath.Text);
            
            buttonCreate.IsEnabled = res;
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

                try
                {
                    textLocation.Text = System.IO.Path.GetDirectoryName(textPath.Text);
                }
                catch { }
            }

            Validate();
        }

        private void checkCreateDirectory_CheckChanged(object sender, RoutedEventArgs e)
        {
            UpdatePath();
        }

        private void listTemplates_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Validate();
        }

        private void buttonCreate_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }

        private void buttonCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            // Save settings
            if (DialogResult.HasValue && DialogResult.Value)
            {
                // Save recent locations
                IEnumerable<string> recentLocations = GetRecentLocations();
                if (!recentLocations.Contains(SelectedLocation))
                {
                    if (recentLocations.Count() > 5)
                        recentLocations = recentLocations.Skip(1);

                    recentLocations = recentLocations.Append(SelectedLocation);
                }

                Settings.Default.CreateProjectDialog_RecentLocations = recentLocations.Aggregate((first, second) => first + "|" + second);

                // Save location
                if (checkLocationDefault.IsChecked.HasValue && checkLocationDefault.IsChecked.Value)
                {
                    Settings.Default.CreateProjectDialog_SavedLocation = SelectedLocation;
                }
            }
        }
    }
}
