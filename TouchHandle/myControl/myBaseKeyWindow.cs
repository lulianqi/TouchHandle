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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using TouchHandle.myTool;

namespace TouchHandle.myControl
{
    /// <summary>
    /// 按照步骤 1a 或 1b 操作，然后执行步骤 2 以在 XAML 文件中使用此自定义控件。
    ///
    /// 步骤 1a) 在当前项目中存在的 XAML 文件中使用该自定义控件。
    /// 将此 XmlNamespace 特性添加到要使用该特性的标记文件的根 
    /// 元素中: 
    ///
    ///     xmlns:MyNamespace="clr-namespace:TouchHandle.myControl"
    ///
    ///
    /// 步骤 1b) 在其他项目中存在的 XAML 文件中使用该自定义控件。
    /// 将此 XmlNamespace 特性添加到要使用该特性的标记文件的根 
    /// 元素中: 
    ///
    ///     xmlns:MyNamespace="clr-namespace:TouchHandle.myControl;assembly=TouchHandle.myControl"
    ///
    /// 您还需要添加一个从 XAML 文件所在的项目到此项目的项目引用，
    /// 并重新生成以避免编译错误: 
    ///
    ///     在解决方案资源管理器中右击目标项目，然后依次单击
    ///     “添加引用”->“项目”->[浏览查找并选择此项目]
    ///
    ///
    /// 步骤 2)
    /// 继续操作并在 XAML 文件中使用控件。
    ///
    ///     <MyNamespace:myBaseKeyWindow/>
    ///
    /// </summary>
    public class myBaseKeyWindow : Window
    {
        static myBaseKeyWindow()
        {
            //DefaultStyleKeyProperty.OverrideMetadata(typeof(myBaseKeyWindow), new FrameworkPropertyMetadata(typeof(myBaseKeyWindow)));
        }

        #region myVar

        DispatcherTimer myPositionTimer = new DispatcherTimer(DispatcherPriority.Background);
        public Point windowRect;
        public string myFormWay ="left";
        public bool isChangeWidth = false;

        #endregion


        #region myFunction

        public void AdaptPosition(Point yourRectangle)
        {
            this.Topmost = false;
            this.Topmost = true;
            if (myFormWay == "left")
            {
                this.mySetWindowPosition(new Point(20, yourRectangle.Y - 20 - this.Height));
            }
            else if (myFormWay == "right")
            {
                this.mySetWindowPosition(new Point(yourRectangle.X - 20 - this.Width, yourRectangle.Y - 20 - this.Height));
            }
            else
            {
                this.mySetWindowPosition(new Point(20, yourRectangle.Y - 20 - this.Height));
            }
            if(isChangeWidth)
            {
                this.Width = yourRectangle.X-30;
            }
        }

        #endregion

        public void myLoad()
        {
            WindowInteropHelper wndHelper = new WindowInteropHelper(this);
            IntPtr wpfHwnd = wndHelper.Handle;
            myWinApi.SetWindowLong(wpfHwnd, MessageIdentifier.GWL_EXSTYLE, myWinApi.GetWindowLong(wpfHwnd, MessageIdentifier.GWL_EXSTYLE) | MessageIdentifier.WS_EX_NOACTIVATE);
            this.Topmost = true;
        }

        public void mySetWindowEx()
        {
            this.Background = null;
            this.AllowsTransparency = true;
            this.WindowStyle = WindowStyle.None;
        }

        public void mySetPosition()
        {
            try
            { 
                windowRect = new Point(SystemParameters.PrimaryScreenWidth, SystemParameters.PrimaryScreenHeight);
            }
            catch(Exception ex)
            {
                windowRect = new Point(1024, 768);
                MessageBox.Show(ex.Message);
            }
            AdaptPosition(windowRect);
        }

        public void myTickStart()
        {
            myPositionTimer.Interval = TimeSpan.FromSeconds(2);
            myPositionTimer.Tick += myPositionTimer_Tick;
            myPositionTimer.Start();
        }




        private void myPositionTimer_Tick(object sender, EventArgs e)
        {
            if(windowRect.X!=SystemParameters.PrimaryScreenWidth||windowRect.Y!=SystemParameters.PrimaryScreenHeight)
            {
                windowRect = new Point(SystemParameters.PrimaryScreenWidth, SystemParameters.PrimaryScreenHeight);
                AdaptPosition(windowRect);
            }
        }
    }
}
