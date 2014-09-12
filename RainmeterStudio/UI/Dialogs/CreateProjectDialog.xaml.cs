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
using Microsoft.Win32;
using RainmeterStudio.Business;
using RainmeterStudio.Core.Editor;
using RainmeterStudio.Core.Model;
using RainmeterStudio.Core.Utils;
using RainmeterStudio.Properties;
using RainmeterStudio.Resources;
using RainmeterStudio.UI.Controller;
using RainmeterStudio.UI.ViewModel;

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
        public IProjectTemplate SelectedTemplate
        {
            get
            {
                var item = listTemplates.SelectedItem as ProjectTemplateViewModel;

                if (item != null)
                    return item.Template;

                return null;
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
        /// Gets the selected path
        /// </summary>
        public string SelectedPath
        {
            get
            {
                string path = SelectedLocation;

                if (checkCreateDirectory.IsChecked.HasValue && checkCreateDirectory.IsChecked.Value)
                    path = System.IO.Path.Combine(path, SelectedName);

                return System.IO.Path.Combine(path, SelectedName + ".rsproj");
            }
        }

        #endregion

        #region Private fields

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
            string location = Settings.Default.Project_SavedLocation;

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
            res &= PathHelper.IsFileNameValid(textName.Text);
            res &= PathHelper.IsPathValid(textLocation.Text);

            buttonCreate.IsEnabled = res;
        }

        private void textName_TextChanged(object sender, TextChangedEventArgs e)
        {
            Validate();
        }

        private void textLocation_TextChanged(object sender, TextChangedEventArgs e)
        {
            Validate();
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
                    Settings.Default.Project_SavedLocation = SelectedLocation;
                }
            }
        }

        private void buttonBrowse_Click(object sender, RoutedEventArgs e)
        {
            // Show dialog
            SaveFileDialog dialog = new SaveFileDialog();
            dialog.Title = Strings.CreateProjectDialog_Browse_Title;
            dialog.AddExtension = true;
            dialog.Filter = Strings.Dialog_FileType_Project + "|*.rsproj|" + Strings.Dialog_FileType_AllFiles + "|*.*";
            dialog.InitialDirectory = SelectedLocation;
            dialog.FileName = SelectedName;
            bool? res = dialog.ShowDialog();

            if (res.HasValue && res.Value)
            {
                SelectedName = System.IO.Path.GetFileNameWithoutExtension(dialog.FileName);
                SelectedLocation = System.IO.Path.GetDirectoryName(dialog.FileName);
            }
        }
    }
}
