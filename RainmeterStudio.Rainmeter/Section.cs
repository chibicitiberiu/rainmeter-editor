using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace RainmeterStudio.Rainmeter
{
    public abstract class Section : Group
    {
        #region Imports

        [DllImport("Rainmeter.dll", CallingConvention = CallingConvention.Cdecl)]
        static extern bool Section_GetName(out string result, Int32 handle);

        [DllImport("Rainmeter.dll", CallingConvention = CallingConvention.Cdecl)]
        static extern bool Section_GetOriginalName(out string result, Int32 handle);

        [DllImport("Rainmeter.dll", CallingConvention = CallingConvention.Cdecl)]
        static extern bool Section_HasDynamicVariables(out bool result, Int32 handle);

        [DllImport("Rainmeter.dll", CallingConvention = CallingConvention.Cdecl)]
        static extern bool Section_SetDynamicVariables(Int32 handle, bool value);

        [DllImport("Rainmeter.dll", CallingConvention = CallingConvention.Cdecl)]
        static extern bool Section_ResetUpdateCounter(Int32 handle);

        [DllImport("Rainmeter.dll", CallingConvention = CallingConvention.Cdecl)]
        static extern bool Section_GetUpdateCounter(out int result, Int32 handle);

        [DllImport("Rainmeter.dll", CallingConvention = CallingConvention.Cdecl)]
        static extern bool Section_GetUpdateDivider(out int result, Int32 handle);

        [DllImport("Rainmeter.dll", CallingConvention = CallingConvention.Cdecl)]
        static extern bool Section_GetOnUpdateAction(out string result, Int32 handle);

        [DllImport("Rainmeter.dll", CallingConvention = CallingConvention.Cdecl)]
        static extern bool Section_DoUpdateAction(Int32 handle);

        [DllImport("Rainmeter.dll", CallingConvention = CallingConvention.Cdecl)]
        static extern bool Section_Destroy(Int32 handle);

        #endregion

        protected Section(Int32 handle)
            : base(handle)
        {
        }

        ~Section()
        {
            Section_Destroy(Handle);
        }

        /// <summary>
        /// Gets the name of the section
        /// </summary>
        public string Name
        {
            get
            {
                string name;

                if (!Section_GetName(out name, Handle))
                    throw new ExternalException("Get name failed.");

                return name;
            }
        }

        /// <summary>
        /// Gets the original name of the section
        /// </summary>
        public string OriginalName
        {
            get
            {
                string name;

                if (!Section_GetOriginalName(out name, Handle))
                    throw new ExternalException("Get original name failed.");

                return name;
            }
        }

        /// <summary>
        /// Gets a value indicating if this section has dynamic variables
        /// </summary>
        public bool HasDynamicVariables
        {
            get
            {
                bool result;

                if (!Section_HasDynamicVariables(out result, Handle))
                    throw new ExternalException("Get dynamic variables has failed.");

                return result;
            }
            set
            {
                if (!Section_SetDynamicVariables(Handle, value))
                    throw new ExternalException("Set dynamic variables has failed.");
            }
        }

        /// <summary>
        /// Resets the update counter
        /// </summary>
        public void ResetUpdateCounter()
        {
            if (!Section_ResetUpdateCounter(Handle))
                throw new ExternalException("Reset update counter has failed.");
        }

        /// <summary>
        /// Gets the update counter
        /// </summary>
        public int UpdateCounter
        {
            get
            {
                int result;

                if (!Section_GetUpdateCounter(out result, Handle))
                    throw new ExternalException("Get update counter has failed.");

                return result;
            }
        }

        /// <summary>
        /// Gets the update divider
        /// </summary>
        public int UpdateDivider
        {
            get
            {
                int result;

                if (!Section_GetUpdateDivider(out result, Handle))
                    throw new ExternalException("Get update divider has failed.");

                return result;
            }
        }

        /// <summary>
        /// Gets the update divider
        /// </summary>
        public string OnUpdateAction
        {
            get
            {
                string result;

                if (!Section_GetOnUpdateAction(out result, Handle))
                    throw new ExternalException("Get on update action has failed.");

                return result;
            }
        }

        /// <summary>
        /// Executes the update action
        /// </summary>
        public void DoUpdateAction()
        {
            if (!Section_DoUpdateAction(Handle))
                throw new ExternalException("Do update action has failed.");
        }
    }
}
