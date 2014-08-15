using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using RainmeterStudio.UI.Controller;
using RainmeterStudio.UI.ViewModel;

namespace RainmeterStudio.UI.Dialogs
{
    /// <summary>
    /// Interaction logic for CreateDocumentDialog.xaml
    /// </summary>
    public partial class CreateDocumentDialog : Window
    {
        private DocumentController _documentController;

        /// <summary>
        /// Gets or sets the currently selected file format
        /// </summary>
        public DocumentTemplateViewModel SelectedTemplate
        {
            get
            {
                return listTemplates.SelectedItem as DocumentTemplateViewModel;
            }
            set
            {
                listTemplates.SelectedItem = value;
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
            }
        }

        /// <summary>
        /// Creates a new instance of CreateDocumentDialog
        /// </summary>
        public CreateDocumentDialog(DocumentController docCtrl)
        {
            InitializeComponent();
            _documentController = docCtrl;

            PopulateFormats();
            Validate();
        }

        private void PopulateFormats()
        {
            listTemplates.ItemsSource = _documentController.DocumentTemplates;
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

        private void Validate()
        {
            bool res = true;

            res &= !String.IsNullOrWhiteSpace(textPath.Text);
            res &= !textPath.Text.Intersect(System.IO.Path.GetInvalidFileNameChars()).Any();
            res &= (listTemplates.SelectedItem != null);

            buttonCreate.IsEnabled = res;
        }

        private void listFormats_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Validate();
        }
    }
}
