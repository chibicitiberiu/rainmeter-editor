using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RainmeterStudio.Utils
{
    /// <summary>
    /// Linq extensions
    /// </summary>
    public static class LinqExtensions
    {
        /// <summary>
        /// Applies action on every item from the container
        /// </summary>
        /// <typeparam name="T">Enumerable type</typeparam>
        /// <param name="container">Container</param>
        /// <param name="action">Action</param>
        public static void ForEach<T> (this IEnumerable<T> container, Action<T> action)
        {
            foreach (var obj in container)
                action(obj);
        }
    }
}
