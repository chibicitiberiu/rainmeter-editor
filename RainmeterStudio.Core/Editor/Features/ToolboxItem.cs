using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RainmeterStudio.Core.Editor.Features
{
    public class ToolboxItem
    {
        public string Name { get; private set; }

        public ToolboxItem(string name)
        {
            Name = name;
        }
    }
}
