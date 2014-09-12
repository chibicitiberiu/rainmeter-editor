using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RainmeterStudio.Core.Editor.Features
{
    public interface ICustomDocumentTitleProvider
    {
        string Title { get; }
        event EventHandler TitleChanged;
    }
}
