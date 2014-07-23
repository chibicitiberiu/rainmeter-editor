using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RainmeterEditor.Business;
using RainmeterEditor.UI.Dialogs;
using RainmeterEditor.Model.Events;
using RainmeterEditor.Model;

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

        public void Create()
        {
            // Show dialog
            var dialog = new CreateDocumentDialog();
            bool? res = dialog.ShowDialog();

            if (!res.HasValue || !res.Value)
                return;

            var format = dialog.SelectedFormat;
            var path = dialog.SelectedPath;

            // Call manager
            DocumentManager.Instance.Create(format, path);
        }
    }
}
