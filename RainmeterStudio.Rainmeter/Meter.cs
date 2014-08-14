using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RainmeterStudio.Rainmeter
{
    public abstract class Meter : Group
    {
        public Meter(int handle)
            : base(handle)
        {
        }
    }
}
