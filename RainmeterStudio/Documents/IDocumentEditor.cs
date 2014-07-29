using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using RainmeterStudio.Model;

namespace RainmeterStudio.Documents
{
    public interface IDocumentEditor
    {
        IDocument AttachedDocument { get; }
        UIElement EditorUI { get; }
    }
}
