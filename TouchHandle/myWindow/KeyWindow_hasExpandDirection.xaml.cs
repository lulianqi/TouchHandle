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
using System.Windows.Threading;
using TouchHandle.myTool;

namespace TouchHandle.myWindow
{
    /// <summary>
    /// KeyWindow_Original.xaml 的交互逻辑
    /// </summary>
    public partial class KeyWindow_hasExpandDirection : myControl.myBaseKeyWindow
    {
        public KeyWindow_hasExpandDirection()
        {
            InitializeComponent();
        }

        DispatcherTimer myFormSizeTimer = new DispatcherTimer(DispatcherPriority.Background);
        private Dictionary<int, Ellipse> movingEllipses = new Dictionary<int, Ellipse>();
        private int ellipseSize = 30;
        private float sizeFactor = 0.5f;

        /// <summary>
        /// FormSize Tick Start
        /// </summary>
        public void myFormSizeTickStart()
        {
            myFormSizeTimer.Interval = TimeSpan.FromSeconds(1);
            myFormSizeTimer.Tick += myFormSizeTimer_Tick;
            myFormSizeTimer.Start();
        }

        void myFormSizeTimer_Tick(object sender, EventArgs e)
        {
            if(sizeFactor != myRuntimeConfiguration.myRuntimesizeFactor)
            {
                adjustFormSize(myRuntimeConfiguration.myRuntimesizeFactor/sizeFactor);
                sizeFactor = myRuntimeConfiguration.myRuntimesizeFactor;
                this.AdaptPosition(new Point(SystemParameters.PrimaryScreenWidth, SystemParameters.PrimaryScreenHeight));
            }
        }

        /// <summary>
        /// set window on top and no activate
        /// </summary>
        public void setWindow()
        {
            WindowInteropHelper wndHelper = new WindowInteropHelper(this);
            IntPtr wpfHwnd = wndHelper.Handle;
            myWinApi.SetWindowLong(wpfHwnd, MessageIdentifier.GWL_EXSTYLE, myWinApi.GetWindowLong(wpfHwnd, MessageIdentifier.GWL_EXSTYLE) | MessageIdentifier.WS_EX_NOACTIVATE);
            this.Topmost = true;
        }

        /// <summary>
        /// load key image key value
        /// </summary>
        private void loadMyKey()
        {
            key_up.myKeyVaule = KeyInterop.VirtualKeyFromKey(myRuntimeConfiguration.myUserKey_Up);
            key_down.myKeyVaule = KeyInterop.VirtualKeyFromKey(myRuntimeConfiguration.myUserKey_Down);
            key_left.myKeyVaule = KeyInterop.VirtualKeyFromKey(myRuntimeConfiguration.myUserKey_Left);
            key_right.myKeyVaule = KeyInterop.VirtualKeyFromKey(myRuntimeConfiguration.myUserKey_Right);
            key_ok.myKeyVaule = KeyInterop.VirtualKeyFromKey(myRuntimeConfiguration.myUserKey_Ok);
            key_A.myKeyVaule = KeyInterop.VirtualKeyFromKey(myRuntimeConfiguration.myUserKey_A);
            key_B.myKeyVaule = KeyInterop.VirtualKeyFromKey(myRuntimeConfiguration.myUserKey_B);
            key_C.myKeyVaule = KeyInterop.VirtualKeyFromKey(myRuntimeConfiguration.myUserKey_C);
            key_D.myKeyVaule = KeyInterop.VirtualKeyFromKey(myRuntimeConfiguration.myUserKey_D);

            key_leftAndUp.myKeyVaule = KeyInterop.VirtualKeyFromKey(myRuntimeConfiguration.myUserKey_Left);
            key_leftAndUp.myKeyVauleExpand = KeyInterop.VirtualKeyFromKey(myRuntimeConfiguration.myUserKey_Up);
            key_upAndRight.myKeyVaule = KeyInterop.VirtualKeyFromKey(myRuntimeConfiguration.myUserKey_Up);
            key_upAndRight.myKeyVauleExpand = KeyInterop.VirtualKeyFromKey(myRuntimeConfiguration.myUserKey_Right);
            key_rightAndDown.myKeyVaule = KeyInterop.VirtualKeyFromKey(myRuntimeConfiguration.myUserKey_Right);
            key_rightAndDown.myKeyVauleExpand = KeyInterop.VirtualKeyFromKey(myRuntimeConfiguration.myUserKey_Down);
            key_downAndLeft.myKeyVaule = KeyInterop.VirtualKeyFromKey(myRuntimeConfiguration.myUserKey_Down);
            key_downAndLeft.myKeyVauleExpand = KeyInterop.VirtualKeyFromKey(myRuntimeConfiguration.myUserKey_Left);
        }


        private void myChangeContorSize(Image yourImage,float mySizeFactor)
        {
            yourImage.Height = yourImage.Height * mySizeFactor;
            yourImage.Width = yourImage.Width * mySizeFactor;
        }

        private void myChangeContorSize(Viewbox yourViewbox,  float mySizeFactor)
        {
            yourViewbox.Height = yourViewbox.Height * mySizeFactor;
            yourViewbox.Width = yourViewbox.Width * mySizeFactor;
        }

        private void myChangeContorSize(Control yourControl, float mySizeFactor)
        {
            yourControl.Height = yourControl.Height * mySizeFactor;
            yourControl.Width = yourControl.Width * mySizeFactor;
        }

        private void myChangeWindowSize(Window yourControl, float mySizeFactor)
        {
            yourControl.Height = yourControl.Height * mySizeFactor;
        }

        /// <summary>
        /// change window size
        /// </summary>
        /// <param name="mySizeFactor">your size factor</param>
        private void adjustFormSize(float mySizeFactor)
        {
            myChangeContorSize(ViewboxDirection, mySizeFactor);
            myChangeContorSize(ViewboxFunction, mySizeFactor);
            myChangeWindowSize(this,mySizeFactor);
        }


        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            sizeFactor = myRuntimeConfiguration.myRuntimesizeFactor;
            adjustFormSize(sizeFactor);
            myFormWay = "left";
            setWindow();
            loadMyKey();
            mySetPosition();
            myTickStart();
            myFormSizeTickStart();
        }

        #region Crid_Move
        private void CridMain_TouchEnter(object sender, TouchEventArgs e)
        {
            Ellipse ellipse = new Ellipse();
            ellipse.Width = ellipseSize;
            ellipse.Height = ellipseSize;
            ellipse.Stroke = Brushes.White;
            ellipse.Fill = new SolidColorBrush(Color.FromRgb(0, 191, 255));
            TouchPoint touchPoint = e.GetTouchPoint(CridMain);
            ellipse.HorizontalAlignment = HorizontalAlignment.Left;
            ellipse.VerticalAlignment = VerticalAlignment.Top;
            ellipse.Margin = new Thickness(touchPoint.Bounds.Left - ellipseSize / 2, touchPoint.Bounds.Top - ellipseSize / 2, 0, 0);
            movingEllipses[e.TouchDevice.Id] = ellipse;
            CridMain.Children.Add(ellipse);
        }

        private void CridMain_TouchLeave(object sender, TouchEventArgs e)
        {
            Ellipse ellipse = movingEllipses[e.TouchDevice.Id];
            CridMain.Children.Remove(ellipse);
        }

        private void CridMain_TouchMove(object sender, TouchEventArgs e)
        {
            Ellipse ellipse = movingEllipses[e.TouchDevice.Id];
            TouchPoint touchPoint = e.GetTouchPoint(CridMain);
            ellipse.Margin = new Thickness(touchPoint.Bounds.Left - ellipseSize / 2, touchPoint.Bounds.Top - ellipseSize / 2, 0, 0);
        } 
        #endregion

        #region Canvas_Move
        //private void CanvasDirection_TouchMove(object sender, TouchEventArgs e)
        //{
        //    Ellipse ellipse = movingEllipses[e.TouchDevice.Id];
        //    TouchPoint touchPoint = e.GetTouchPoint(CanvasDirection);
        //    Canvas.SetTop(ellipse, touchPoint.Bounds.Top);
        //    Canvas.SetLeft(ellipse, touchPoint.Bounds.Left);
        //    //if (touchPoint.Bounds.Top > ViewboxDirection.Height - 2 || touchPoint.Bounds.Left > ViewboxDirection.Width - 2 || touchPoint.Bounds.Top <2 || touchPoint.Bounds.Left <2)
        //    //{
        //    //    CanvasDirection.Children.Remove(ellipse);
        //    //    //movingEllipses.Remove(e.TouchDevice.Id);
        //    //}
        //}


        //private void CanvasDirection_TouchEnter(object sender, TouchEventArgs e)
        //{
        //    Ellipse ellipse = new Ellipse();
        //    ellipse.Width = ellipseSize;
        //    ellipse.Height = ellipseSize;
        //    ellipse.Stroke = Brushes.White;
        //    ellipse.Fill = new SolidColorBrush(Color.FromRgb(0, 191, 255));
        //    TouchPoint touchPoint = e.GetTouchPoint(CanvasDirection);
        //    Canvas.SetTop(ellipse, touchPoint.Bounds.Top);
        //    Canvas.SetLeft(ellipse, touchPoint.Bounds.Left);
        //    movingEllipses[e.TouchDevice.Id] = ellipse;
        //    CanvasDirection.Children.Add(ellipse);
        //}

        //private void CanvasDirection_TouchLeave(object sender, TouchEventArgs e)
        //{
        //    Ellipse ellipse = movingEllipses[e.TouchDevice.Id];
        //    CanvasDirection.Children.Remove(ellipse);
        //}
        #endregion
        

        
    }
}
