using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace RainmeterStudio.Model
{
    public class Property : INotifyPropertyChanged
    {
        /// <summary>
        /// Gets or sets the name of the property
        /// </summary>
        public string Name { get; private set; }

        private object _value;

        /// <summary>
        /// Gets or sets the value of the property
        /// </summary>
        public object Value
        {
            get
            {
                return _value;
            }
            set
            {
                _value = value;

                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("Value"));
            }
        }
        
        /// <summary>
        /// Triggered when the value changes
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Initializes this property
        /// </summary>
        /// <param name="name">Name of the property</param>
        /// <param name="value">Value of the property</param>
        public Property(string name, object value = null)
        {
            Name = name;
            Value = value;
        }
    }
}
