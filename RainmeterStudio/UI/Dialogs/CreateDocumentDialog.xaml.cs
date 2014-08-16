using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using RainmeterStudio.Core.Utils;
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
        /// Creates a new instance of CreateDocumentDialog
        /// </summary>
        public CreateDocumentDialog(DocumentController docCtrl)
        {
            InitializeComponent();
            _documentController = docCtrl;

            listTemplates.ItemsSource = _documentController.DocumentTemplates.OrderBy(x => x.DisplayText);

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

        private void Validate()
        {
            bool res = true;

            res &= (listTemplates.SelectedItem != null);
            res &= PathHelper.IsFileNameValid(SelectedName);

            buttonCreate.IsEnabled = res;
        }

        private void listFormats_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Validate();
        }

        private void textName_TextChanged(object sender, TextChangedEventArgs e)
        {
            Validate();
        }
    }
}
