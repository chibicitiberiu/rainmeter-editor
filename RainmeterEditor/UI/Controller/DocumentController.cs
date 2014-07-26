using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RainmeterEditor.Business;
using RainmeterEditor.UI.Dialogs;
using RainmeterEditor.Model.Events;
using RainmeterEditor.Model;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace RainmeterEditor.UI.Controller
{
    public class DocumentController
    {
        #region Commands

        public Command DocumentCreateCommand { get; private set; }

        #endregion

        public event EventHandler<DocumentOpenedEventArgs> DocumentOpened
        {
            add
            {
                DocumentManager.Instance.DocumentOpened += value;
            }
            remove
            {
                DocumentManager.Instance.DocumentOpened -= value;
            }
        }
        public event EventHandler DocumentClosed;

        public Window OwnerWindow { get; set; }

        public DocumentController()
        {
            DocumentCreateCommand = new Command("DocumentCreateCommand", () => CreateWindow())
            {
                DisplayText = Resources.Strings.DocumentCreateCommand_DisplayText,
                Tooltip = Resources.Strings.DocumentCreateCommand_ToolTip,
                Icon = new BitmapImage(new Uri("/Resources/Icons/page_white_star_16.png", UriKind.RelativeOrAbsolute)),
                Shortcut = new KeyGesture(Key.N, ModifierKeys.Control)
            };
        }

        public void CreateWindow(DocumentFormat defaultFormat = null, string defaultPath = "")
        {
            // Show dialog
            var dialog = new CreateDocumentDialog()
            {
                Owner = OwnerWindow,
                SelectedFormat = defaultFormat,
                SelectedPath = defaultPath
            };
            bool? res = dialog.ShowDialog();

            if (!res.HasValue || !res.Value)
                return;

            var format = dialog.SelectedFormat;
            var path = dialog.SelectedPath;

            // Call manager
            DocumentManager.Instance.Create(format, path);
        }

        public void Create(DocumentFormat format, string path)
        {
            // Call manager
            DocumentManager.Instance.Create(format, path);
        }

    }
}
