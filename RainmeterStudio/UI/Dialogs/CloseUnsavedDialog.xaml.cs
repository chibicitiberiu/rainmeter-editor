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
using System.Windows.Shapes;
using RainmeterStudio.Core.Model;

namespace RainmeterStudio.UI.Dialogs
{
    /// <summary>
    /// Interaction logic for CloseUnsavedDialog.xaml
    /// </summary>
    public partial class CloseUnsavedDialog : Window
    {
        /// <summary>
        /// Displays the dialog and returns the result
        /// </summary>
        /// <param name="unsavedDocuments">List of unsaved documents</param>
        /// <returns>Dialog result</returns>
        public static bool? ShowDialog(IEnumerable<IDocument> unsavedDocuments)
        {
            var dialog = new CloseUnsavedDialog(unsavedDocuments);
            return dialog.ShowDialog();
        }

        /// <summary>
        /// Displays the dialog and returns the result
        /// </summary>
        /// <param name="owner">Owner window</param>
        /// <param name="unsavedDocuments">List of unsaved documents</param>
        /// <returns>Dialog result</returns>
        public static bool? ShowDialog(Window owner, IEnumerable<IDocument> unsavedDocuments)
        {
            var dialog = new CloseUnsavedDialog(unsavedDocuments);
            dialog.Owner = owner;
            return dialog.ShowDialog();
        }

        /// <summary>
        /// Displays the dialog and returns the result
        /// </summary>
        /// <param name="owner">Owner window</param>
        /// <param name="unsavedDocuments">List of unsaved documents</param>
        /// <returns>Dialog result</returns>
        public static bool? ShowDialog(Window owner, params IDocument[] unsavedDocuments)
        {
            var dialog = new CloseUnsavedDialog(unsavedDocuments);
            dialog.Owner = owner;
            return dialog.ShowDialog();
        }

        /// <summary>
        /// Initializes the dialog
        /// </summary>
        /// <param name="unsavedDocuments">List of unsaved documents</param>
        public CloseUnsavedDialog(IEnumerable<IDocument> unsavedDocuments)
        {
            InitializeComponent();

            textFiles.Inlines.AddRange(unsavedDocuments.SelectMany(GetInlines));
        }

        private IEnumerable<Inline> GetInlines(IDocument doc)
        {
            var folder = System.IO.Path.GetDirectoryName(doc.Reference.StoragePath);
            
            yield return new Run(folder)
            {
                Foreground = Brushes.DarkGray
            };

            yield return new Run(doc.Reference.Name)
            {
                FontWeight = FontWeights.Bold
            };
        }

        private void buttonSave_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }

        private void buttonDoNotSave_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private void buttonCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = null;
            Close();
        }
    }
}
