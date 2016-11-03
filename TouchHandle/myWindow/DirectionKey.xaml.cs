using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using TouchHandle.myTool;


namespace TouchHandle.myWindow
{
    /// <summary>
    /// DirectionKey.xaml 的交互逻辑
    /// </summary>
    public partial class DirectionKey : myControl.myBaseKeyWindow
    {
        public DirectionKey()
        {
            InitializeComponent();
        }
       

        #region myVar

        #endregion

        #region myFunction

        #endregion
        
        #region myAction
        private void loadMyImage()
        {

        }

        private void loadMyKey()
        {
            key_up.myKeyVaule = KeyInterop.VirtualKeyFromKey(myRuntimeConfiguration.myUserKey_Up);
            key_down.myKeyVaule = KeyInterop.VirtualKeyFromKey(myRuntimeConfiguration.myUserKey_Down);
            key_left.myKeyVaule = KeyInterop.VirtualKeyFromKey(myRuntimeConfiguration.myUserKey_Left);
            key_right.myKeyVaule = KeyInterop.VirtualKeyFromKey(myRuntimeConfiguration.myUserKey_Right);
            key_ok.myKeyVaule = KeyInterop.VirtualKeyFromKey(myRuntimeConfiguration.myUserKey_Ok);
        }

        public void setWindow()
        {
            WindowInteropHelper wndHelper = new WindowInteropHelper(this);
            IntPtr wpfHwnd = wndHelper.Handle;
            myWinApi.SetWindowLong(wpfHwnd, MessageIdentifier.GWL_EXSTYLE, myWinApi.GetWindowLong(wpfHwnd, MessageIdentifier.GWL_EXSTYLE) | MessageIdentifier.WS_EX_NOACTIVATE);
            this.Topmost = true;
        }

        #endregion

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            myFormWay = "left";
            setWindow();
            loadMyKey();
            mySetPosition();
            myTickStart();
        }

    }
}
