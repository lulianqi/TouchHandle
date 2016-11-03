using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouchHandle.myTool
{
    class MessageIdentifier
    {
        #region 鼠标标识
        public const int WM_MOUSEFIRST = 0x0200;
        public const int WM_MOUSEMOVE = 0x0200;
        public const int WM_LBUTTONDOWN = 0x0201;
        public const int WM_LBUTTONUP = 0x0202;
        public const int WM_LBUTTONDBLCLK = 0x0203;
        public const int WM_RBUTTONDOWN = 0x0204;
        public const int WM_RBUTTONUP = 0x0205;
        public const int WM_RBUTTONDBLCLK = 0x0206;
        public const int WM_MBUTTONDOWN = 0x0207;
        public const int WM_MBUTTONUP = 0x0208;
        public const int WM_MBUTTONDBLCLK = 0x0209;
        #endregion

        #region 按键事件
        public const int KEYEVENTF_KEYUP = 0x0002;
        #endregion

        #region SendMessage参数
        public const int WM_KEYDOWN = 0X100;
        public const int WM_KEYUP = 0X101;
        public const int WM_SYSCHAR = 0X106;
        public const int WM_SYSKEYUP = 0X105;
        public const int WM_SYSKEYDOWN = 0X104;
        public const int WM_CHAR = 0X102;
        #endregion

        #region 键盘方向
        public const int VK_UP = 0X26;
        public const int VK_DOWN = 0X28;
        public const int VK_LEFT = 0X25;
        public const int VK_RIGHT = 0X27;
        #endregion

        #region 键盘

        public const int VK_SPACE = 0X20;
        public const int VK_RETURN = 0X0D;
        public const int VK_SHIFT = 0X10;

        public const int VK_Z = 0X5A;
        public const int VK_X = 0X58;
        #endregion

        //ShowWindow参数
        public const int GWL_EXSTYLE = -20;
        public const int WS_EX_NOACTIVATE = 0x8000000;
        //SendMessage参数
        
    }
}
