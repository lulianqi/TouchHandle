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


namespace TouchHandle.myTool
{
    static class myExtensionMethods
    {
        /*
        /// <summary>
        /// 获取文本url编码值
        /// </summary>
        /// <param name="tb">文本框</param>
        /// <returns>文本url编码值</returns>
        public static string myText(this TextBox tb)
        {
            return System.Web.HttpUtility.UrlEncode(tb.Text);
        }

        /// <summary>
        /// 添加键值，若遇到已有key则覆盖
        /// </summary>
        /// <param name="dc">Dictionary</param>
        /// <param name="yourKeyValuePair">KeyValuePair</param>
        public static void myAdd(this Dictionary<string, string> dc, KeyValuePair<string,string> yourKeyValuePair)
        {
            if(dc.ContainsKey(yourKeyValuePair.Key))
            {
                dc[yourKeyValuePair.Key] = yourKeyValuePair.Value;
            }
            else
            {
                dc.Add(yourKeyValuePair.Key, yourKeyValuePair.Value);
            }
        }

        /// <summary>
        /// 添加键值，若遇到已有key则覆盖
        /// </summary>
        /// <param name="dc">Dictionary</param>
        /// <param name="yourKey">Key</param>
        /// <param name="yourValue">Value</param>
        public static void myAdd(this Dictionary<string, string> dc, string yourKey,string yourValue)
        {
            if (dc.ContainsKey(yourKey))
            {
                dc[yourKey] = yourValue;
            }
            else
            {
                dc.Add(yourKey, yourValue);
            }
        }
         * */

        /// <summary>
        /// set the window position with a point
        /// </summary>
        /// <param name="yourWindow">your Window</param>
        /// <param name="yourPosition">your Position</param>
        public static void mySetWindowPosition(this Window yourWindow, Point yourPosition)
        {
            yourWindow.Left = yourPosition.X;
            yourWindow.Top = yourPosition.Y;
        }

        /// <summary>
        /// 添加键值，若遇到已有key则覆盖
        /// </summary>
        /// <param name="dc">Dictionary</param>
        /// <param name="yourKey">Key</param>
        /// <param name="yourValue">Value</param>
        public static void myAdd(this Dictionary<string, Key> dc, string yourKey, Key yourValue)
        {
            if (dc.ContainsKey(yourKey))
            {
                dc[yourKey] = yourValue;
            }
            else
            {
                dc.Add(yourKey, yourValue);
            }
        }
    }
}
