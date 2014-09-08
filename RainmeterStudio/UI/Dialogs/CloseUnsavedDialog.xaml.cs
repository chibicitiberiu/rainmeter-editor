using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using RainmeterStudio.Core.Utils;

namespace RainmeterStudio.UI.Dialogs
{
    public enum CloseUnsavedDialogResult
    {
        Unset,
        Save,
        DoNotSave,
        Cancel
    }
    
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
        public static CloseUnsavedDialogResult ShowDialog(IEnumerable<IDocument> unsavedDocuments)
        {
            var dialog = new CloseUnsavedDialog(unsavedDocuments);
            dialog.ShowDialog();
            return dialog.SaveDialogResult;
        }

        /// <summary>
        /// Displays the dialog and returns the result
        /// </summary>
        /// <param name="owner">Owner window</param>
        /// <param name="unsavedDocuments">List of unsaved documents</param>
        /// <returns>Dialog result</returns>
        public static CloseUnsavedDialogResult ShowDialog(Window owner, IEnumerable<IDocument> unsavedDocuments)
        {
            var dialog = new CloseUnsavedDialog(unsavedDocuments);
            dialog.ShowDialog();
            return dialog.SaveDialogResult;
        }

        /// <summary>
        /// Displays the dialog and returns the result
        /// </summary>
        /// <param name="owner">Owner window</param>
        /// <param name="unsavedDocuments">List of unsaved documents</param>
        /// <returns>Dialog result</returns>
        public static CloseUnsavedDialogResult ShowDialog(Window owner, params IDocument[] unsavedDocuments)
        {
            var dialog = new CloseUnsavedDialog(unsavedDocuments);
            dialog.ShowDialog();
            return dialog.SaveDialogResult;
        }

        /// <summary>
        /// Gets the 'close unsaved' dialog result
        /// </summary>
        public CloseUnsavedDialogResult SaveDialogResult { get; private set; }

        /// <summary>
        /// Initializes the dialog
        /// </summary>
        /// <param name="unsavedDocuments">List of unsaved documents</param>
        public CloseUnsavedDialog(IEnumerable<IDocument> unsavedDocuments)
        {
            InitializeComponent();

            SaveDialogResult = CloseUnsavedDialogResult.Unset;
            unsavedDocuments.ForEach(d => listUnsavedDocuments.Items.Add(d));
        }

        private void buttonSave_Click(object sender, RoutedEventArgs e)
        {
            SaveDialogResult = CloseUnsavedDialogResult.Save;
            Close();
        }

        private void buttonDoNotSave_Click(object sender, RoutedEventArgs e)
        {
            SaveDialogResult = CloseUnsavedDialogResult.DoNotSave;
            Close();
        }

        private void buttonCancel_Click(object sender, RoutedEventArgs e)
        {
            SaveDialogResult = CloseUnsavedDialogResult.Cancel;
            Close();
        }
    }
}
