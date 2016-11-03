using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace TouchHandle.myTool
{
    public static class myWinApi
    {

        [return: MarshalAs(UnmanagedType.Bool)]
        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool PostMessage(IntPtr hWnd, int Msg, int wParam, int lParam);

        [DllImport("user32.dll")]
        public static extern int MapVirtualKey(uint Ucode, uint uMapType);

        [DllImport("user32.dll")]
        public static extern void keybd_event(byte bVk, byte bScan, uint dwFlags, uint dwExtraInfo);

        [DllImport("user32.dll")]
        public static extern int SetWindowLong(IntPtr window, int index, int value);

        [DllImport("user32.dll")]
        public static extern int GetWindowLong(IntPtr window, int index);
       
    }
}
