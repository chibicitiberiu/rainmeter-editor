using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RainmeterEditor.Model
{
    public interface IDocument
    {
        string Name { get; }
        string FilePath { get; }
    }
}
