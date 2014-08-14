using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using RainmeterStudio.Business;
using RainmeterStudio.Core.Documents;
using RainmeterStudio.UI.Controller;

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
        public DocumentTemplate SelectedTemplate
        {
            get
            {
                return listFormats.SelectedItem as DocumentTemplate;
            }
            set
            {
                listFormats.SelectedItem = value;
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

            PopulateCategories();
            RepopulateFormats();
            Validate();
        }

        private void PopulateCategories()
        {
            listCategories.ItemsSource = _documentController.DocumentTemplates
                .Select(template => template.Category)
                .Where(cat => cat != null)
                .Distinct()
                .Concat(new[] { "All" });

            listCategories.SelectedIndex = listCategories.Items.Count - 1;
        }

        private void RepopulateFormats()
        {
            if (Object.Equals(listCategories.SelectedItem, "All"))
                listFormats.ItemsSource = _documentController.DocumentTemplates;

            else
                listFormats.ItemsSource = _documentController.DocumentTemplates.Where(x => Object.Equals(x.Category, listCategories.SelectedItem));
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
            res &= (listFormats.SelectedItem != null);

            buttonCreate.IsEnabled = res;
        }

        private void listCategories_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            RepopulateFormats();
        }
    }
}
