//*******************************************************//
//                                                       //
//CSharp.Net Data Potection Application Custom Action Lib//
// Copyright(c) 2014-2015 Spectra Logic Corporation.     //
//                                                       //
//*******************************************************//
using System;
using System.Windows.Forms;

namespace DataProtectionApplication.SpectraCustomAction
{
    public class WindowWrapper : IWin32Window
    {
        public WindowWrapper(IntPtr handle)
        {
            _hwnd = handle;
        }

        public IntPtr Handle
        {
            get { return _hwnd; }
        }

        private readonly IntPtr _hwnd;
    }
}
