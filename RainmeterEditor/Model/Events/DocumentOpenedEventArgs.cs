﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RainmeterEditor.Model.Events
{
    public class DocumentOpenedEventArgs : EventArgs
    {
        public IDocumentEditor Editor { get; private set; }

        public DocumentOpenedEventArgs(IDocumentEditor editor)
        {
            Editor = editor;
        }
    }
}
