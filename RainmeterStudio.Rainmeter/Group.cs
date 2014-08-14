using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace RainmeterStudio.Rainmeter
{
    /// <summary>
    /// Represents a group
    /// </summary>
    /// <remarks>
    /// Skins, meters, and measures can be categorized into groups to allow 
    /// easier control with group bangs. For example, the !HideMeterGroup
    /// bang may be used to hide multiple meters in a single bang (compared 
    /// to !HideMeter statements for each meter).
    /// </remarks>
    public abstract class Group
    {
        #region Imports

        [DllImport("Rainmeter.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern bool Group_BelongsToGroup(out bool result, Int32 handle, string group);

        [DllImport("Rainmeter.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern bool Group_Destroy(Int32 handle);

        #endregion

        /// <summary>
        /// Gets or sets the associated handle of this object
        /// </summary>
        protected Int32 Handle { get; private set; }

        /// <summary>
        /// Tests if belongs to a group
        /// </summary>
        /// <param name="group">Group name</param>
        /// <returns>True if belongs</returns>
        public bool BelongsToGroup(string group)
        {
            bool result;
            
            if (!Group_BelongsToGroup(out result, Handle, group))
                throw new ExternalException("Belongs to group failed.");

            return result;
        }

        /// <summary>
        /// Initializes this group
        /// </summary>
        /// <param name="handle">The handle</param>
        protected Group(Int32 handle)
        {
            Handle = handle;
        }

        /// <summary>
        /// Finalizer
        /// </summary>
        ~Group()
        {
            Group_Destroy(Handle);
        }
    }
}
