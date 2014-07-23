using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RainmeterEditor.Model
{
    public class Property
    {
        public string Name { get; set; }

        private object _value;
        public object Value
        {
            get
            {
                return _value;
            }
            set
            {
                _value = value;

                if (ValueChanged != null)
                    ValueChanged(this, new EventArgs());
            }
        }

        public event EventHandler ValueChanged;
    }
}
