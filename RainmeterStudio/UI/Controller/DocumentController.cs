using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RainmeterStudio.Business;
using RainmeterStudio.UI.Dialogs;
using RainmeterStudio.Model.Events;
using RainmeterStudio.Model;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace RainmeterStudio.UI.Controller
{
    public class DocumentController
    {
        #region Commands

        public Command _documentCreateCommand;
        public Command DocumentCreateCommand
        {
            get
            {
                if (_documentCreateCommand == null)
                {
                    _documentCreateCommand = new Command("DocumentCreateCommand", () => CreateWindow())
                    {
                        DisplayText = Resources.Strings.DocumentCreateCommand_DisplayText,
                        Tooltip = Resources.Strings.DocumentCreateCommand_ToolTip,
                        Icon = new BitmapImage(new Uri(Resources.Icons.DocumentCreateCommand_Icon, UriKind.RelativeOrAbsolute)),
                        Shortcut = new KeyGesture(Key.N, ModifierKeys.Control)
                    };
                }

                return _documentCreateCommand;
            }
        }

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
        }

        public void CreateWindow(DocumentTemplate defaultFormat = null, string defaultPath = "")
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

        public void Create(DocumentTemplate format, string path)
        {
            // Call manager
            DocumentManager.Instance.Create(format, path);
        }

    }
}
