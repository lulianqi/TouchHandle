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
    /// yourSuggestions.xaml 的交互逻辑
    /// </summary>
    public partial class yourSuggestions : Window
    {
        public yourSuggestions()
        {
            InitializeComponent();
        }

        MainWindow myMainWindow;
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            myMainWindow = (MainWindow)this.Owner;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if(TextBox_Suggestions.Text=="")
            {
                MessageBox.Show("您还未描述任何意见或建议", "抱歉", MessageBoxButton.OK, MessageBoxImage.Asterisk);
                return;
            }
            myMainWindow.mySuggestionsMess = "联系人： " + TextBox_Connection.Text + "\r\n" + "建议：  " + TextBox_Suggestions.Text;
            this.Close();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (TextBox_Suggestions.Text == "")
            {
                myMainWindow.mySuggestionsMess = "";
            }
            else
            {
                myMainWindow.mySuggestionsMess = "联系人： " + TextBox_Connection.Text + "\r\n" + "建议：  " + TextBox_Suggestions.Text;
            }
        }

    }
}
