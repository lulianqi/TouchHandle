using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using TouchHandle.myControl;
using TouchHandle.myTool;

namespace TouchHandle.myWindow
{
    /// <summary>
    /// FunctionKey.xaml 的交互逻辑
    /// </summary>
    public partial class FunctionKey : myBaseKeyWindow
    {
        public FunctionKey()
        {
            InitializeComponent();
        }

        private void loadMyKey()
        {
            key_A.myKeyVaule = KeyInterop.VirtualKeyFromKey(myRuntimeConfiguration.myUserKey_A);
            key_B.myKeyVaule = KeyInterop.VirtualKeyFromKey(myRuntimeConfiguration.myUserKey_B);
            key_C.myKeyVaule = KeyInterop.VirtualKeyFromKey(myRuntimeConfiguration.myUserKey_C);
            key_D.myKeyVaule = KeyInterop.VirtualKeyFromKey(myRuntimeConfiguration.myUserKey_D);
        }

        private void FunctionKey_Loaded(object sender, RoutedEventArgs e)
        {
            myFormWay = "right";
            myLoad();
            mySetPosition();
            myTickStart();
            loadMyKey();
        }
    }
}
