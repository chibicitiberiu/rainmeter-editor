using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RainmeterStudio.Rainmeter.DataTypes
{
    public abstract class Bang
    {
        /// <summary>
        /// Argument info
        /// </summary>
        public class ArgumentInfo
        {
            public string Name { get; set; }
            public Type DataType { get; set; }

            public ArgumentInfo(string name, Type dataType)
            {
                Name = name;
                DataType = dataType;
            }
        }

        /// <summary>
        /// Gets the function name of the bang
        /// </summary>
        public abstract string FunctionName { get; }

        /// <summary>
        /// Gets the list of arguments
        /// </summary>
        public abstract IEnumerable<ArgumentInfo> Arguments { get; }

        /// <summary>
        /// Executes the bang
        /// </summary>
        public void Execute(params object[] args)
        {

        }

        /// <summary>
        /// Gets the string representation of this bang
        /// </summary>
        /// <returns>String representation</returns>
        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();
            builder.AppendFormat("!{0}", FunctionName);

            foreach (var arg in Arguments.Select(x => x.ToString()))
            {
                if (arg.Any(Char.IsWhiteSpace))
                    builder.AppendFormat(@" ""{0}""", arg);

                else builder.AppendFormat(" {0}", arg);
            }

            return builder.ToString();
        }
    }
}
