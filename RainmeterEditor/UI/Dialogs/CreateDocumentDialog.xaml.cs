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
using RainmeterEditor.Business;
using RainmeterEditor.Model;

namespace RainmeterEditor.UI.Dialogs
{
    /// <summary>
    /// Interaction logic for CreateDocumentDialog.xaml
    /// </summary>
    public partial class CreateDocumentDialog : Window
    {
        /// <summary>
        /// Gets or sets the currently selected file format
        /// </summary>
        public DocumentFormat SelectedFormat
        {
            get
            {
                return listFormats.SelectedItem as DocumentFormat;
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
        public CreateDocumentDialog()
        {
            InitializeComponent();

            PopulateFormats();
        }

        private void PopulateFormats()
        {
            listFormats.ItemsSource = DocumentManager.Instance.DocumentFormats;
            CollectionView view = (CollectionView)CollectionViewSource.GetDefaultView(listFormats.ItemsSource);
            view.GroupDescriptions.Add(new PropertyGroupDescription("Category"));
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
    }
}
