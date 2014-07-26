using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace RainmeterStudio.Interop
{
    public class NativeLibrary : IDisposable
    {
        #region Imports

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr LoadLibrary(string libname);

        [DllImport("kernel32.dll", CharSet = CharSet.Ansi)]
        private static extern bool FreeLibrary(IntPtr hModule);
        
        [DllImport("kernel32.dll", CharSet = CharSet.Ansi)]
        private static extern IntPtr GetProcAddress(IntPtr hModule, string lpProcName);

        #endregion

        public string DllName { get; private set; }
        private IntPtr _handle;

        public NativeLibrary(string dllName)
        {
            // Set properties
            DllName = dllName;

            // Load library
            _handle = LoadLibrary(DllName);
            if (_handle == IntPtr.Zero)
            {
                int errorCode = Marshal.GetLastWin32Error();
                throw new BadImageFormatException(string.Format("Failed to load library (ErrorCode: {0})", errorCode));
            }
        }

        public void Dispose()
        {
            if (_handle != IntPtr.Zero)
                FreeLibrary(_handle);
        }

        public Delegate GetFunctionByName(string name, Type delegateType)
        {
            IntPtr addr = GetProcAddress(_handle, name);
            return Marshal.GetDelegateForFunctionPointer(addr, delegateType);
        }
    }
}
