using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RainmeterEditor.Model;

namespace RainmeterEditor.Business
{
    public class ProjectManager
    {
        public Project ActiveProject { get; protected set; }

        public void Open() { }
        public void Close() { }
    }
}
