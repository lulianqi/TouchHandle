using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
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
using TouchHandle.myWindow;

namespace TouchHandle
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        #region myVar
        public static string myVersion = "2";
        public static string myVanelifeUserInfo = "";
        public string mySuggestionsMess = "";
        bool isFirstLoad = true;                                         //是否为窗体首次加载
        Thread myThreadDirectionForm;
        Thread myThreadFunctionForm;
        Thread myThreadForm;
        public ImageSource nowSetKeyImage;
        public myKeyMap runKeyMap = new myKeyMap();                       //用于解析xml数据文件的封装
        //用list<object> 也可以，只不过数据源变化后，要调用﻿﻿﻿lv.Items.Refresh();
        public static ObservableCollection<KeyMapValue> myKeyMapValue;    //用于填充ListView是数据源（lv_keyVaule）
        public string FilePath = System.IO.Path.GetDirectoryName(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName);      //应用启动目录
        public Key nowKey;                                                //真在被设置的key的值临时存储
        public string nowSetIndexValue = "";                              //真在被设置的key的索引
        public bool isKeySetCancel = false;                               //是否被用户取消设置
        public int KeyMapListIndex = 0;                                   //默认被选中的配置文件
        public int previousKeyMapListIndex = 0;                           //上一个默认被选中的配置文件
        TextBox TextBoxEditNowName = new TextBox();                       //namelist编辑控件 
        bool isNowNameListEdit = true;                                    //namelist是否将要进行编辑
        bool isRefreshKeyMapNameList = false;                             //是否真正刷新 ComboBox_KeyMapList         
        #endregion

        #region myFunction

        /// <summary>
        /// i will change KeyName to his Description
        /// </summary>
        /// <param name="yourKeyName">your Key Name</param>
        /// <returns>his Description</returns>
        public string getKeyNameDescription(string yourKeyName)
        {
            switch(yourKeyName)
            {
                case "key_up":
                    return "方向键上";
                    //break;
                case "key_down":
                    return "方向键下";
                    //break;
                case "key_left":
                    return "方向键左";
                    //break;
                case "key_right":
                    return "方向键右";
                    //break;
                case "key_ok":
                    return "确认按键";
                    //break;
                case "key_A":
                    return "功能键A";
                    //break;
                case "key_B":
                    return "功能键B";
                    //break;
                case "key_C":
                    return "功能键C";
                    //break;
                case "key_D":
                    return "功能键D";
                    //break;
                default:
                    return yourKeyName;
            }
        }

        #endregion

        #region myAction

        /// <summary>
        /// here i will chech the new version in server
        /// </summary>
        public void checkVersion()
        {
            //myVaneLife.intVanelife("D6E08D9849AC1843DDC7C6EFB1752CF3", "9DD0D09051A6FC8E5C9897E5601F101E", "http://api.vanelife.com/v1/");
            myVaneLife.intVanelife("AA5D937522978133CB78F571FAE20C8C", "574ECEB51E4ACDA242DC032270150653", "http://api.vanelife.com/v1/");
            myVaneLife.checkNew(myVersion, FilePath + "\\config\\version.thd");
        }
             

        /// <summary>
        /// load all set file or set value
        /// </summary>
        public void loadSet()
        {
            myKeyMapValue = new ObservableCollection<KeyMapValue>();
            loadSaveIni();

            loadKeyVaulesInXml();
            loadMyUserKey();
            loadMyKeyMapValue();
        }

        /// <summary>
        /// load VaneLife Service
        /// </summary>
        public void loadVaneLifeSv()
        {
            //初始化vanelife服务，同时检查新版本
            Thread myCheckVersionThread = new Thread(new ThreadStart(checkVersion));
            myCheckVersionThread.IsBackground = true;
            myCheckVersionThread.SetApartmentState(ApartmentState.STA);
            myCheckVersionThread.Start();

            //检查新的安装，并上传用户信息
            string tempUserInfo = myini.IniReadValue("MainLog", "UserInfo", FilePath + "\\config\\app_log");
            switch (tempUserInfo)
            {
                case "null":
                    myini.IniWriteValue("MainLog", "UserInfo", Environment.OSVersion.ToString(), FilePath + "\\config\\app_log");
                    myVaneLife.vanelifeAddPublishTask(1, "NewUser", Environment.OSVersion.ToString());
                    myVaneLife.vanelifeAddPublishTask(2, "LOAD","New Start " + DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss"));
                    break;
                default:
                    break;
            }

            //检查是否存在未完成的消息上报
            if (myini.IniReadValue("MainLog", "isNeedUp", FilePath + "\\config\\app_log") == "true")
            {
                myVaneLife.vanelifeAddPublishTask(2, "LOAD", myini.IniReadValue("MainLog", "Message", FilePath + "\\config\\app_log"));
                myini.IniWriteValue("MainLog", "isNeedUp", "false", FilePath + "\\config\\app_log");
            }

            //记录用户本次启动APP的时间
            myVanelifeUserInfo = "Start " + DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss") + "  -  ";
            myini.IniWriteValue("MainLog", "Message", myVanelifeUserInfo , FilePath + "\\config\\app_log");
            myini.IniWriteValue("MainLog", "isNeedUp", "true", FilePath + "\\config\\app_log");
        }

        /// <summary>
        /// set window for his contor
        /// </summary>
        public void setWindow()
        {
            //ComboBox_KeyMapList
            lv_keyVaule.DataContext = myKeyMapValue;
            ComboBox_KeyMapList.ItemsSource = runKeyMap.myListMapNames;
            ComboBox_KeyMapList.SelectedValuePath = "Key";
            ComboBox_KeyMapList.DisplayMemberPath = "Value";
            ComboBox_KeyMapList.SelectedIndex = KeyMapListIndex;//请务必不要改变此处与设置lv_keyVaule的默认绑定的顺序
            //SliderSizeChange
            SliderSizeChange.Value = (double)(myRuntimeConfiguration.myRuntimesizeFactor);
            //TextBoxEditNowName
            TextBoxEditNowName.Width = ComboBox_KeyMapList.Width;
            TextBoxEditNowName.Height = ComboBox_KeyMapList.Height;
            TextBoxEditNowName.Margin = ComboBox_KeyMapList.Margin;
            TextBoxEditNowName.VerticalAlignment = VerticalAlignment.Top;
            TextBoxEditNowName.HorizontalAlignment = HorizontalAlignment.Left;
        }

        /// <summary>
        /// sava my ini file
        /// </summary>
        public void saveConfig()
        {
            myini.IniWriteValue("MainWindow", "sizevalue", myRuntimeConfiguration.myRuntimesizeFactor.ToString(), FilePath + "\\config\\seting.ini");
        }

        /// <summary>
        /// load my ini file
        /// </summary>
        public void loadSaveIni()
        {
            //sizevalue
            try
            {
                myRuntimeConfiguration.myRuntimesizeFactor = float.Parse(myini.IniReadValue("MainWindow", "sizevalue", FilePath + "\\config\\seting.ini"));
            }
            catch (Exception ex)
            {
                ErrorLog.PutInLogEx(ex.Message);
                myRuntimeConfiguration.myRuntimesizeFactor = 1;
                myini.IniWriteValue("MainWindow", "sizevalue", "1", FilePath + "\\config\\seting.ini");
            }
            //KeyMapListIndex
            try
            {
                KeyMapListIndex = int.Parse(myini.IniReadValue("MainWindow", "KeyMapListIndex", FilePath + "\\config\\seting.ini"));
            }
            catch (Exception ex)
            {
                ErrorLog.PutInLogEx(ex.Message);
                KeyMapListIndex = 0;
                myini.IniWriteValue("MainWindow", "KeyMapListIndex", "0", FilePath + "\\config\\seting.ini");
            }
        }

        /// <summary>
        /// load xml key map
        /// </summary>
        public void loadKeyVaulesInXml()
        {
            string keyMapPath = FilePath + @"\keyValue\keyValueData.xml";
            if (!File.Exists(keyMapPath))
            {
                MessageBox.Show("未发现键值配置文件,将为您加载了默认配置", "STOP");
               
            }
            else if (!runKeyMap.LoadFile(keyMapPath))
            {
                MessageBox.Show("键值配置文件异常,将为您加载了默认配置", "STOP");
            }
            else if (runKeyMap.loadKeyMaps())
            {
                ComboBox_KeyMapList.SelectionChanged += ComboBox_KeyMapList_SelectionChanged;
            }
            else
            {
                MessageBox.Show("键值配置文件异常,将为您加载了默认配置", "Warning");
            }

            if (runKeyMap.myKeyMaps == null || runKeyMap.myKeyMaps.Count==0)
            {
                bt_editNowName.Visibility = Visibility.Hidden;
                ComboBox_KeyMapList.Visibility = Visibility.Hidden;
            }

        }

        public void saveKeyVaulesInXml()
        {
            if (runKeyMap.myKeyMaps.Count > 0)
            {
                runKeyMap.savaNowKeyMap(previousKeyMapListIndex, ((KeyValuePair<int, string>)(ComboBox_KeyMapList.Items[previousKeyMapListIndex])).Value);
                runKeyMap.saveAllKeyXml();
            }
        }

        /// <summary>
        /// load key value in RuntimeConfiguration
        /// </summary>
        public void loadMyUserKey()
        {
            myRuntimeConfiguration.myUserKey_Up = Key.Up;
            myRuntimeConfiguration.myUserKey_Down = Key.Down;
            myRuntimeConfiguration.myUserKey_Left = Key.Left;
            myRuntimeConfiguration.myUserKey_Right = Key.Right;
            myRuntimeConfiguration.myUserKey_Ok = Key.Enter;
            myRuntimeConfiguration.myUserKey_A = Key.A;
            myRuntimeConfiguration.myUserKey_B = Key.B;
            myRuntimeConfiguration.myUserKey_C = Key.C;
            myRuntimeConfiguration.myUserKey_D = Key.D;
        }

        /// <summary>
        /// load key vaule in KeyMap
        /// </summary>
        public void loadMyKeyMapValue()
        {
            myKeyMapValue.Clear();
            myKeyMapValue.Add(new KeyMapValue("key_up", "方向键上", myRuntimeConfiguration.myUserKey_Up.ToString()));
            myKeyMapValue.Add(new KeyMapValue("key_down", "方向键下", myRuntimeConfiguration.myUserKey_Down.ToString()));
            myKeyMapValue.Add(new KeyMapValue("key_left", "方向键左", myRuntimeConfiguration.myUserKey_Left.ToString()));
            myKeyMapValue.Add(new KeyMapValue("key_right", "方向键右", myRuntimeConfiguration.myUserKey_Right.ToString()));
            myKeyMapValue.Add(new KeyMapValue("key_ok", "确认按键", myRuntimeConfiguration.myUserKey_Ok.ToString()));
            myKeyMapValue.Add(new KeyMapValue("key_A", "功能键A", myRuntimeConfiguration.myUserKey_A.ToString()));
            myKeyMapValue.Add(new KeyMapValue("key_B", "功能键B", myRuntimeConfiguration.myUserKey_B.ToString()));
            myKeyMapValue.Add(new KeyMapValue("key_C", "功能键C", myRuntimeConfiguration.myUserKey_C.ToString()));
            myKeyMapValue.Add(new KeyMapValue("key_D", "功能键D", myRuntimeConfiguration.myUserKey_D.ToString()));
            //myKeyMapValue.Add(new { Key = "1", Value = "1" });
            //myKeyMapValue.Add(new { Key = "1", Value = "1" });
            //myKeyMapValue.Add(new { Key = "1", Value = "1" });
        }

        public void createKeyWindowEx()
        {
            myThreadDirectionForm = new Thread(new ThreadStart(newDirectionKey));
            myThreadFunctionForm = new Thread(new ThreadStart(newFunctionKey));
            myThreadDirectionForm.IsBackground = true;
            myThreadDirectionForm.SetApartmentState(ApartmentState.STA);
            myThreadDirectionForm.Start();
            //myKeyForms.Add(myThreadDirectionForm);
            myThreadFunctionForm.IsBackground = true;
            myThreadFunctionForm.SetApartmentState(ApartmentState.STA);
            myThreadFunctionForm.Start();
            //myKeyForms.Add(myThreadFunctionForm);
            this.WindowState = WindowState.Minimized;
        }

        public void createKeyWindow()
        {
            myThreadForm = new Thread(new ThreadStart(newWindowKey));
            myThreadForm.IsBackground = true;
            myThreadForm.SetApartmentState(ApartmentState.STA);
            myThreadForm.Start();
            this.WindowState = WindowState.Minimized;
        }


        public void updataMyKeyMapValue(string yourIndexValue, Key yourKey)
        {
            for (int i = 0; i < myKeyMapValue.Count; i++)
            {
                if (myKeyMapValue[i].IndexValue == yourIndexValue)
                {
                    myKeyMapValue[i].Value = yourKey.ToString();
                    lv_keyVaule.Items.Refresh();
                    break;
                }
            }
        }

        private void newFunctionKey()
        {
            DirectionKey myDirectionKey = new DirectionKey();
            myDirectionKey.ShowDialog();
        }

        private void newDirectionKey()
        {
            FunctionKey myFunctionKey = new FunctionKey();
            myFunctionKey.myFormWay = "right";
            myFunctionKey.ShowDialog();
        }

        private void newWindowKey()
        {
            //KeyWindow_Original myFunctionKey = new KeyWindow_Original();
            KeyWindow_hasExpandDirection myFunctionKey = new KeyWindow_hasExpandDirection();
            myFunctionKey.isChangeWidth = true;
            myFunctionKey.ShowDialog();
        }
        #endregion

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            loadSet();
            setWindow();
            isFirstLoad = false;

            loadVaneLifeSv();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            saveConfig();
            saveKeyVaulesInXml();

            //记录用户本次关闭APP的时间
            myVanelifeUserInfo += "End " + DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss");
            myini.IniWriteValue("MainLog", "Message", myVanelifeUserInfo , FilePath + "\\config\\app_log");
        }

        private void Button_SetKey_Click(object sender, RoutedEventArgs e)
        {
            nowSetKeyImage = (((Image)(((Button)sender).Content))).Source;
            nowKey = myRuntimeConfiguration.myUserKey_Up;
            SetKeyValue mySetKeyValue = new SetKeyValue();
            mySetKeyValue.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            mySetKeyValue.Owner = this;
            if(mySetKeyValue.ShowDialog() == false)
            {
                if (isKeySetCancel || nowKey==Key.None)
                {
                    return;
                }
                else
                {
                    switch (((Button)sender).Name)
                    {
                        case "key_up":
                            myRuntimeConfiguration.myUserKey_Up = nowKey;
                            nowSetIndexValue = "key_up";
                            break;
                        case "key_down":
                            myRuntimeConfiguration.myUserKey_Down = nowKey;
                            nowSetIndexValue = "key_down";
                            break;
                        case "key_left":
                            myRuntimeConfiguration.myUserKey_Left = nowKey;
                            nowSetIndexValue = "key_left";
                            break;
                        case "key_right":
                            myRuntimeConfiguration.myUserKey_Right = nowKey;
                            nowSetIndexValue = "key_right";
                            break;
                        case "key_ok":
                            myRuntimeConfiguration.myUserKey_Ok = nowKey;
                            nowSetIndexValue = "key_ok";
                            break;
                        case "key_A":
                            myRuntimeConfiguration.myUserKey_A = nowKey;
                            nowSetIndexValue = "key_A";
                            break;
                        case "key_B":
                            myRuntimeConfiguration.myUserKey_B = nowKey;
                            nowSetIndexValue = "key_B";
                            break;
                        case "key_C":
                            myRuntimeConfiguration.myUserKey_C = nowKey;
                            nowSetIndexValue = "key_C";
                            break;
                        case "key_D":
                            myRuntimeConfiguration.myUserKey_D = nowKey;
                            nowSetIndexValue = "key_D";
                            break;
                        case "null":
                            break;
                        default:
                            break;

                    }
                    updataMyKeyMapValue(nowSetIndexValue, nowKey);
                }
            }
        }

        private void bt_editNowName_Click(object sender, RoutedEventArgs e)
        {
            if (isNowNameListEdit)
            {
                Image_editNowName.Source = new BitmapImage(new Uri(@"myResources/nameList_save.png", UriKind.Relative)); 
                Grid_Main.Children.Add(TextBoxEditNowName);
                isNowNameListEdit = false;
                TextBoxEditNowName.Text = ((KeyValuePair<int, string>)(ComboBox_KeyMapList.SelectedItem)).Value;
                TextBoxEditNowName.Focus();
            }
            else
            {
                runKeyMap.myListMapNames[ComboBox_KeyMapList.SelectedIndex]= TextBoxEditNowName.Text;
                ComboBox_KeyMapList.Items.Refresh();
                isRefreshKeyMapNameList = true;
                ComboBox_KeyMapList.SelectedIndex = previousKeyMapListIndex;
                isRefreshKeyMapNameList = false;
                Image_editNowName.Source = new BitmapImage(new Uri(@"myResources/nameList_edit.png", UriKind.Relative)); 
                Grid_Main.Children.Remove(TextBoxEditNowName);
                isNowNameListEdit = true;
            }
        }


        private void bt_startHandel_Click(object sender, RoutedEventArgs e)
        {
            if (myThreadForm == null)
            {
                createKeyWindow();
            }
            else
            {
                if (myThreadForm.ThreadState != ThreadState.Stopped && myThreadForm.ThreadState != ThreadState.Aborted)
                {
                      myThreadForm.Abort();
                }
                else
                {
                    createKeyWindow();
                }
            }
        }

        private void bt_openSuggestions(object sender, RoutedEventArgs e)
        {
            yourSuggestions mySuggestions = new yourSuggestions();
            mySuggestions.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            mySuggestions.Owner = this;
            if(mySuggestions.ShowDialog() == false)
            {
                if(mySuggestionsMess!="")
                {
                    myVaneLife.vanelifeAddPublishTask(199, "Suggestion", mySuggestionsMess);
                }
            }
        }

        private void SliderSizeChange_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (SliderSizeChange.IsInitialized)
            {
                myRuntimeConfiguration.myRuntimesizeFactor =(float)(Math.Round(SliderSizeChange.Value, 2));
            }
        }

        private void ComboBox_KeyMapList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ComboBox_KeyMapList.SelectedIndex!=-1 && !isRefreshKeyMapNameList)
            { 
                if (!isFirstLoad)
                {
                    runKeyMap.savaNowKeyMap(previousKeyMapListIndex, ((KeyValuePair<int, string>)(ComboBox_KeyMapList.Items[previousKeyMapListIndex])).Value);
                }
           
                foreach(KeyValuePair<string,Key> tempKeyValuePair in runKeyMap.myKeyMaps[ComboBox_KeyMapList.SelectedIndex])
                {
                    switch (tempKeyValuePair.Key)
                    {
                        case "key_up":
                            myRuntimeConfiguration.myUserKey_Up = tempKeyValuePair.Value;
                            break;
                        case "key_down":
                            myRuntimeConfiguration.myUserKey_Down = tempKeyValuePair.Value;
                            break;
                        case "key_left":
                            myRuntimeConfiguration.myUserKey_Left = tempKeyValuePair.Value;
                            break;
                        case "key_right":
                            myRuntimeConfiguration.myUserKey_Right = tempKeyValuePair.Value;
                            break;
                        case "key_ok":
                            myRuntimeConfiguration.myUserKey_Ok = tempKeyValuePair.Value;
                            break;
                        case "key_A":
                            myRuntimeConfiguration.myUserKey_A = tempKeyValuePair.Value;
                            break;
                        case "key_B":
                            myRuntimeConfiguration.myUserKey_B = tempKeyValuePair.Value;
                            break;
                        case "key_C":
                            myRuntimeConfiguration.myUserKey_C = tempKeyValuePair.Value;
                            break;
                        case "key_D":
                            myRuntimeConfiguration.myUserKey_D = tempKeyValuePair.Value;
                            break;
                        default:
                            myTool.ErrorLog.PutInLogEx(tempKeyValuePair.Key + "未发现匹配项");
                            break;
                    }
                }
                loadMyKeyMapValue();
                previousKeyMapListIndex = ComboBox_KeyMapList.SelectedIndex;
            }
        }

        
    
    }
}
