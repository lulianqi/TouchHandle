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
using System.Windows.Navigation;
using System.Windows.Shapes;

using TouchHandle.myTool;

namespace TouchHandle.myControl
{
    /// <summary>
    /// myTouchKey.xaml 的交互逻辑
    /// </summary>
    public partial class myTouchKey : Image
    {
        public myTouchKey()
        {
            InitializeComponent();
        }

        public int myKeyVaule { get; set; }
        public int myKeyVauleExpand { get; set; }
        public int myTriggerMode { get; set; }

        public ImageSource myInImageSource { get; set; }

        public ImageSource myOutImageSource { get; set; }


        public void mySendKey(int yourKey, bool isKeyDown)
        {

            int btScancode = 0;
            btScancode = myWinApi.MapVirtualKey((uint)yourKey, 0);
            if (isKeyDown)
            {
                myWinApi.keybd_event((byte)yourKey, (byte)btScancode, 0, 0);
            }
            else
            {
                myWinApi.keybd_event((byte)yourKey, (byte)btScancode, MessageIdentifier.KEYEVENTF_KEYUP, 0);
            }

        }

        private void Image_Loaded(object sender, RoutedEventArgs e)
        {
            switch(myTriggerMode)
            {
                case 1:
                    this.TouchDown += myTouchKey_TouchDown;
                    this.TouchUp += myTouchKey_TouchUp;
                    break;
                case 2:
                    this.TouchEnter += myTouchKey_TouchEnter;
                    this.TouchLeave += myTouchKey_TouchLeave;
                    break;
                case 3:
                    this.PreviewDragEnter += myTouchKey_PreviewDragEnter;
                    this.PreviewDragLeave += myTouchKey_PreviewDragLeave;
                    break;
                default:
                    this.TouchEnter += myTouchKey_TouchEnter;
                    this.TouchLeave += myTouchKey_TouchLeave;
                    break;

            }        
        }

        void myTouchKey_PreviewDragEnter(object sender, DragEventArgs e)
        {
            mySendKey(myKeyVaule, true);
            if(myKeyVauleExpand!=0)
            {
                mySendKey(myKeyVauleExpand, true);
            }
        }
        void myTouchKey_PreviewDragLeave(object sender, DragEventArgs e)
        {
            mySendKey(myKeyVaule, false);
            if (myKeyVauleExpand != 0)
            {
                mySendKey(myKeyVauleExpand, false);
            }
        }


        void myTouchKey_TouchEnter(object sender, TouchEventArgs e)
        {
            mySendKey(myKeyVaule, true);
            if (myKeyVauleExpand != 0)
            {
                mySendKey(myKeyVauleExpand, true);
            }
            if(myInImageSource!=null)
            {
                this.Source = myInImageSource;
            }
        }
         void myTouchKey_TouchLeave(object sender, TouchEventArgs e)
        {
            mySendKey(myKeyVaule, false);
            if (myKeyVauleExpand != 0)
            {
                mySendKey(myKeyVauleExpand, false);
            }
            if(myOutImageSource!=null)
            {
                this.Source = myOutImageSource;
            }
        }

        void myTouchKey_TouchDown(object sender, TouchEventArgs e)
        {
            mySendKey(myKeyVaule, true);
            if (myKeyVauleExpand != 0)
            {
                mySendKey(myKeyVauleExpand, true);
            }
        }
        void myTouchKey_TouchUp(object sender, TouchEventArgs e)
        {
            mySendKey(myKeyVaule, false);
            if (myKeyVauleExpand != 0)
            {
                mySendKey(myKeyVauleExpand, false);
            }
        }

        

        //private void Image_TouchDown(object sender, TouchEventArgs e)
        //{
        //    mySendKey(myKeyVaule, true);
        //}

        //private void Image_TouchUp(object sender, TouchEventArgs e)
        //{
        //    mySendKey(myKeyVaule, false);
        //}

        //private void Image_TouchEnter(object sender, TouchEventArgs e)
        //{
        //    mySendKey(myKeyVaule, true);
        //}

        //private void Image_TouchLeave(object sender, TouchEventArgs e)
        //{
        //    mySendKey(myKeyVaule, false);
        //}



    }
}
