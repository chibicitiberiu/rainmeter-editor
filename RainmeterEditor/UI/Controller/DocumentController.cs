using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RainmeterEditor.Business;
using RainmeterEditor.UI.Dialogs;
using RainmeterEditor.Model.Events;
using RainmeterEditor.Model;
using System.Windows;

namespace RainmeterEditor.UI.Controller
{
    public class DocumentController
    {
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
        
        public DocumentController()
        {
        }

        public void Create(Window parent = null, DocumentFormat defaultFormat = null, string defaultPath = "")
        {
            // Show dialog
            var dialog = new CreateDocumentDialog()
            {
                Owner = parent,
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
