using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

namespace TouchHandle.myTool
{
    public static class myHook
    {
        public class Win32Api
        {

            [StructLayout(LayoutKind.Sequential)]
            public class POINT
            {
                public int x;
                public int y;
            }

            [StructLayout(LayoutKind.Sequential)]
            public class MouseHookStruct
            {
                public POINT pt;
                public int hwnd;
                public int wHitTestCode;
                public int dwExtraInfo;
            }

            public delegate int HookProc(int nCode, IntPtr wParam, IntPtr lParam);

            //安装钩子
            [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]

            public static extern int SetWindowsHookEx(int idHook, HookProc lpfn, IntPtr hInstance, int threadId);

            //卸载钩子

            [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]

            public static extern bool UnhookWindowsHookEx(int idHook);

            //调用下一个钩子

            [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]

            public static extern int CallNextHookEx(int idHook, int nCode, IntPtr wParam, IntPtr lParam);

        }

        public class MouseHook
        {

            private Point point;
            private int mouseClickState;

            private Point Point
            {
                get
                {
                    return point;
                }
                set
                {
                    if (point != value)
                    {
                        point = value;
                        if (MouseMoveEvent != null)
                        {
                            var e = new MouseEventArgs(MouseButtons.None, 0, point.X, point.Y, 0);
                            MouseMoveEvent(this, e);
                        }
                    }
                }
            }

            private int hHook;
            public const int WH_MOUSE_LL = 14;
            public Win32Api.HookProc hProc;

            public MouseHook()
            {
                this.Point = new Point();
            }

            public int SetHook()
            {
                hProc = new Win32Api.HookProc(MouseHookProc);
                hHook = Win32Api.SetWindowsHookEx(WH_MOUSE_LL, hProc, IntPtr.Zero, 0);
                return hHook;
            }

            public void UnHook()
            {
                Win32Api.UnhookWindowsHookEx(hHook);
            }

            private int MouseHookProc(int nCode, IntPtr wParam, IntPtr lParam)
            {
                Win32Api.MouseHookStruct MyMouseHookStruct = (Win32Api.MouseHookStruct)Marshal.PtrToStructure(lParam, typeof(Win32Api.MouseHookStruct));
                if (nCode < 0)
                {
                    return Win32Api.CallNextHookEx(hHook, nCode, wParam, lParam);
                    //return 0;
                }

                else
                {
                    this.Point = new Point(MyMouseHookStruct.pt.x, MyMouseHookStruct.pt.y);

                    if ((int)wParam == MessageIdentifier.WM_LBUTTONDOWN)
                    {
                        if (MouseClickEvent != null)
                        {
                            var e = new MouseEventArgs(MouseButtons.None, 0, point.X, point.Y, 0);
                            MouseClickEvent(this, e);
                        }
                    }
                    return Win32Api.CallNextHookEx(hHook, nCode, wParam, lParam);
                    //return 0;
                }

            }

            //委托+事件（把钩到的消息封装为事件，由调用者处理）
            public delegate void MouseMoveHandler(object sender, MouseEventArgs e);
            public event MouseMoveHandler MouseMoveEvent;

            public delegate void MouseClickHandler(object sender, MouseEventArgs e);
            public event MouseClickHandler MouseClickEvent;

        }
    }
}
