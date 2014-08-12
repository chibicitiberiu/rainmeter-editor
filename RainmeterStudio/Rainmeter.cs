using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using RainmeterStudio.Interop;

namespace RainmeterStudio
{
    class RainmeterContext   
    {
        #region Imports

        [DllImport("Rainmeter.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr Rainmeter_Initialize();

        [DllImport("Rainmeter.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern void Rainmeter_Finalize(IntPtr handle);

        #endregion

        private static RainmeterContext _instance = null;

        /// <summary>
        /// Gets the single instance of this class
        /// </summary>
        public static RainmeterContext Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new RainmeterContext();

                return _instance;
            }
        }

        private IntPtr _handle;

        #region Constructor, finalizer

        private RainmeterContext()
        {
            _handle = Rainmeter_Initialize();

            if (_handle == IntPtr.Zero)
                throw new Exception("Failed to initialize native library.");
        }

        ~RainmeterContext()
        {
            Rainmeter_Finalize(_handle);
        }

        #endregion

        public IntPtr Handle { get { return _handle; } }
    }
}
