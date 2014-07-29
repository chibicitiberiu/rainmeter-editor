using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace RainmeterStudio.Model
{
    public interface IDocument : INotifyPropertyChanged
    {
        Reference Reference { get; }
        bool IsDirty { get; set; }
    }
}
