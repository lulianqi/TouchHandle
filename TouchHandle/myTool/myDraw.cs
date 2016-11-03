using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace TouchHandle.myTool
{
    public static class myDraw
    {

        private struct RECT
        {
            public int Left;        // x position of upper-left corner
            public int Top;         // y position of upper-left corner
            public int Right;       // x position of lower-right corner
            public int Bottom;      // y position of lower-right corner
            public RECT(int a, int b, int c, int d)
            {
                Left = a;
                Top = b;
                Right = c;
                Bottom = d;
            }
        }

        public static Graphics myGrap;
        public static Pen myPen;

        //返回与指定字符创相匹配的窗口类名或窗口名的最顶层窗口的窗口句柄
        [DllImport("user32.dll", SetLastError = true)]
        private static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        //返回指定窗口的边框矩形的尺寸。该尺寸以相对于屏幕坐标左上角的屏幕坐标给出
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool GetWindowRect(IntPtr hwnd, out RECT lpRect);

        //检索一指定窗口的客户区域或整个屏幕的显示设备上下文环境的句柄，以后可以在GDI函数中使用该句柄来在设备上下文环境中绘图
        [DllImport("user32.dll")]
        private static extern IntPtr GetDC(IntPtr hWnd);

        //创建一个具有指定颜色的逻辑刷子
        [DllImport("gdi32.dll", EntryPoint = "CreateSolidBrush", SetLastError = true)]
        private static extern IntPtr CreateSolidBrush(int crColor);

        //用指定的画刷填充矩形，此函数包括矩形的左上边界，但不包括矩形的右下边界
        [DllImport("user32.dll")]
        private static extern int FillRect(IntPtr hDC, [In] ref RECT lprc, IntPtr hbr);

        //功能该函数画一个矩形，用当前的画笔画矩形轮廓，用当前画刷进行填充
        //[DllImport("gdi32.dll")]
        //private static extern bool Rectangle(IntPtr hdc, int nLeftRect, int nTopRect, int nRightRect, int nBottomRect);

        //该函数选择一对象到指定的设备上下文环境中，该新对象替换先前的相同类型的对象
        [DllImport("gdi32.dll", ExactSpelling = true, PreserveSig = true, SetLastError = true)]
        private static extern IntPtr SelectObject(IntPtr hdc, IntPtr hgdiobj);

        //该函数检索预定义的备用笔、刷子、字体或者调色板的句柄
        [DllImport("gdi32.dll")]
        private static extern IntPtr GetStockObject(int fnObject);


        public static void drawMyGraph(Point[] drawPoints, Color yourColor)
        {

        }

        public static void drawMyRect(Rectangle yourRectangle, Pen yourPen)
        {
            if (myGrap != null)
            {
                myGrap.DrawRectangle(yourPen, yourRectangle);
            }
        }

        public static void drawEllipse(Rectangle yourRectangle, Pen yourPen)
        {
            if (myGrap != null)
            {
                myGrap.DrawEllipse(yourPen, yourRectangle);
            }
        }

        public static void drawMyPolygon(Point[] yourPoints, Pen yourPen)
        {
            if (myGrap != null)
            {
                myGrap.DrawPolygon(yourPen, yourPoints);
            }
        }

        public static void drawMyFillRect(Rectangle yourRectangle, Color yourColor)
        {
            RECT myRect = new RECT(yourRectangle.X, yourRectangle.Y, yourRectangle.Width, yourRectangle.Height);
            FillRect(GetDC(IntPtr.Zero), ref myRect, CreateSolidBrush((int)ColorTranslator.ToWin32(yourColor)));
        }
    }
}
