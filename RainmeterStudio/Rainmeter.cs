using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using RainmeterStudio.Interop;

namespace RainmeterStudio
{
    class Rainmeter
    {
        #region Imports

        [DllImport("Rainmeter.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr Rainmeter_Initialize();

        [DllImport("Rainmeter.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern void Rainmeter_Finalize(IntPtr handle);

        #endregion

        private static Rainmeter _instance = null;

        /// <summary>
        /// Gets the single instance of this class
        /// </summary>
        public static Rainmeter Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new Rainmeter();

                return _instance;
            }
        }

        private IntPtr _handle;

        #region Constructor, finalizer

        private Rainmeter()
        {
            _handle = Rainmeter_Initialize();

            if (_handle == IntPtr.Zero)
                throw new Exception("Failed to initialize native library.");
        }

        ~Rainmeter()
        {
            Rainmeter_Finalize(_handle);
        }

        #endregion

        public IntPtr Handle { get { return _handle; } }
    }
}
