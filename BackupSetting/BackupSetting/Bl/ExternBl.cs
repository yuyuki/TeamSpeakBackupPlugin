using System;
using System.Runtime.InteropServices;

namespace BackupSetting.Bl
{
    public static class ExternBl
    {
        #region  Public Method

        [DllImport("user32.dll")]
        public static extern int SetForegroundWindow(IntPtr hwnd);

        #endregion
    }
}