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

namespace TouchHandle.myWindow
{
    /// <summary>
    /// SetKeyValue.xaml 的交互逻辑
    /// </summary>
    public partial class SetKeyValue : Window
    {
        public SetKeyValue(ref Key yourKey)
        {
            InitializeComponent();
            //yourKey = Key.B;
        }

        public SetKeyValue()
        {
            InitializeComponent();
        }

        MainWindow myMainWindow;

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            myMainWindow = (MainWindow)this.Owner;
            myTagImage.Source = myMainWindow.nowSetKeyImage;
            myMainWindow.isKeySetCancel = true;
            tb_setKey.Focus();
            lb_info1.Content = "当前设置键值为：" + myMainWindow.nowKey.ToString();
        }

        private void tb_setKey_KeyDown(object sender, KeyEventArgs e)
        {
           
        }

        private void tb_setKey_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            myMainWindow.nowKey = e.Key;
            lb_info1.Content ="该键将设置为："+ e.Key.ToString();
            e.Handled = true;
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
           
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            myMainWindow.isKeySetCancel = false;
            this.Close();
        }
    }
}
