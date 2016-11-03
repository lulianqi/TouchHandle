using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading;

namespace TouchHandle.myTool
{
    class myVaneLife
    {
        public static string vaneUrl;
        public static string vaneApp_key;
        public static string vaneApp_secret;

        private static string vaneChannel_tag = "MicroControlUpgrade";
        private static string vaneEpId = "MicroControl00001";
        private static string vaneDevId="20B624B4075AD1AF075DAAE567065E61";

        private static Thread myTaskThread;
        private static List<string[]> vaneTaskList = new List<string[]>();

        /// <summary>
        /// MD5计算
        /// </summary>
        /// <param name="data">加密数据</param>
        /// <returns>加密结果</returns>
        public static string createMD5Key(string data)
        {
            byte[] result = Encoding.UTF8.GetBytes(data);
            MD5 md5 = new MD5CryptoServiceProvider();
            byte[] output = md5.ComputeHash(result);
            return BitConverter.ToString(output).Replace("-", "");
        }


        /// <summary>
        /// 生成测试数据
        /// </summary>
        /// <param name="testData">用例数据</param>
        /// <returns>测试数据</returns>
        /// 
        public static string myCreatSendData(string testData)
        {
            Hashtable myDataTable = new Hashtable();
            StringBuilder myStrBld = new StringBuilder();
            string tempSign = "";

            #region 填装数据
            string[] sArray = testData.Split('&');
            if (testData == "")
            {
            }
            else
            {
                foreach (string tempStr in sArray)
                {
                    int myBreak = tempStr.IndexOf('=');
                    if (myBreak == -1)
                    {
                        return "can't find =";
                    }
                    myDataTable.Add(tempStr.Substring(0, myBreak), tempStr.Substring(myBreak + 1));
                }
            }
            //foreach (DictionaryEntry de in publicValue)
            //change here
            myDataTable.Add("key", vaneApp_key);
            //myDataTable.Add("v", myReceiveData.vaneV);
            myDataTable.Add("timestamp", DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss"));
            #endregion

            #region 生成Sign
            ArrayList akeys = new ArrayList(myDataTable.Keys);
            akeys.Sort();
            //myStrBld.Append(myReceiveData.vaneApp_secret);
            foreach (string tempKey in akeys)
            {
                myStrBld.Append(tempKey + myDataTable[tempKey]);
            }
            myStrBld.Append(vaneApp_secret);
            tempSign = createMD5Key(myStrBld.ToString());
            #endregion

            #region 组合数据
            myStrBld.Remove(0, myStrBld.Length);
            //change here
            myStrBld.Append("signature=" + tempSign);
            foreach (DictionaryEntry de in myDataTable)
            {
                //myStrBld.Append("&" + de.Key + "=" + de.Value);
                //对每次参数进行url编码
                myStrBld.Append("&" + de.Key + "=" + System.Web.HttpUtility.UrlEncode((de.Value).ToString()));
            }
            return myStrBld.ToString();
            #endregion

        }


        /// <summary>
        /// i can find the value you need in the json
        /// </summary>
        /// <param name="yourTarget">the key you want get</param>
        /// <param name="yourSouce">the json Souce</param>
        /// <returns>back value</returns>
        public static string myFindSaveParameter(string yourTarget, string yourSouce)
        {
            string tempTarget = "\"" + yourTarget + "\"";
            string myJsonBack;
            if (!yourSouce.Contains(tempTarget))
            {
                return null;
            }

            //i will start
            try
            {
                if (yourSouce.StartsWith("["))
                {
                    JArray jAObj = (JArray)JsonConvert.DeserializeObject(yourSouce);
                    for (int i = 0; i < jAObj.Count; i++)
                    {
                        JObject jObj = (JObject)jAObj[i];
                        if (jObj[yourTarget] != null)
                        {
                            myJsonBack = jObj[yourTarget].ToString();
                            return myJsonBack;
                        }
                    }
                    return null ;
                }
                else if (yourSouce.StartsWith("{"))
                {
                    JObject jObj = (JObject)JsonConvert.DeserializeObject(yourSouce);
                    if (jObj[yourTarget] == null)
                    {
                        return null;
                    }
                    else
                    {
                        myJsonBack = jObj[yourTarget].ToString();
                    }
                    return myJsonBack;
                }
            }

            catch (Exception ex)
            {
                ErrorLog.PutInLog("ID:0243 " + ex.Message);
            }
            return null;
        }

        /// <summary>
        /// 启动其他程序
        /// </summary>
        /// <param name="FileName">需要启动的外部程序名称</param>
        public static bool OpenPress(string FileName, string Arguments)
        {
            try
            {
                System.Diagnostics.Process pro = new System.Diagnostics.Process();
                if (System.IO.File.Exists(FileName))
                {
                    pro.StartInfo.FileName = FileName;
                    pro.StartInfo.Arguments = Arguments;
                    pro.Start();
                    return true;
                }
                return false;
            }
            catch(Exception ex)
            {
                myTool.ErrorLog.PutInLogEx(ex.Message);
                return false;
            }
        }

        public static bool OpenPress()
        {
            return OpenPress(System.IO.Path.GetDirectoryName(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName) + "\\UpgradeService.exe", "");
        }

        /// <summary>
        /// init the vanelife
        /// </summary>
        /// <param name="yourKey">your app Key</param>
        /// <param name="yourSecret">your app Secret</param>
        /// <param name="yourUrl">your sever url</param>
        public static void intVanelife(string yourKey, string yourSecret, string yourUrl)
        {
            vaneUrl = yourUrl;
            vaneApp_secret = yourSecret;
            vaneApp_key = yourKey;

            if (myTaskThread == null)
            {
                myTaskThread = new Thread(new ThreadStart(doTask));
                myTaskThread.IsBackground = true;
                //myTaskThread.SetApartmentState(ApartmentState.STA);
                myTaskThread.Start();
            }
        }

        public static void checkNew(string yourVersion, string yourVerFilePath)
        {
            myVersionState tempVer=new myVersionState();
            //继续强制升级
            if (myini.IniReadValue("VersionState", "isNew", yourVerFilePath) == "true")
            {
                myini.IniWriteValue("UpgradeState", "isAlive", "false", yourVerFilePath);
                myini.IniWriteValue("UpgradeState", "isShow", "false", yourVerFilePath);
                if (OpenPress())
                {
                    Environment.Exit(0);
                }
                else
                {
                    System.Windows.MessageBox.Show("升级模块错误，请重新下载应用！", "发现错误", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Stop);
                    myTool.ErrorLog.PutInLog("ID:224 升级服务无法启动");
                }
            }
            if (myVaneLife.checkUpdgrade(yourVersion, ref tempVer))
            {
                //可选升级
                if (tempVer.ver_type == "0")
                {
                    myini.IniWriteValue("VersionState", "isNew", "true", yourVerFilePath);
                    myini.IniWriteValue("VersionState", "type", "0", yourVerFilePath);
                    myini.IniWriteValue("VersionState", "code", tempVer.ver_code, yourVerFilePath);
                    myini.IniWriteValue("VersionState", "desc", tempVer.ver_desc, yourVerFilePath);
                    myini.IniWriteValue("VersionState", "url", tempVer.ver_url, yourVerFilePath);

                    myini.IniWriteValue("UpgradeState", "isNeedDownload", "true", yourVerFilePath);
                    myini.IniWriteValue("UpgradeState", "isAlive", "false", yourVerFilePath);
                    myini.IniWriteValue("UpgradeState", "isShow", "true", yourVerFilePath);

                    if(System.Windows.MessageBox.Show("您的应用目前已有更新，是否更新！", "版本信息", System.Windows.MessageBoxButton.OKCancel, System.Windows.MessageBoxImage.Information)== System.Windows.MessageBoxResult.OK)
                    {
                        if (OpenPress())
                        {
                            //System.Windows.Application.Current.Shutdown();
                            Environment.Exit(0);
                        }
                        else
                        {
                            System.Windows.MessageBox.Show("升级模块错误，请重新下载应用！", "发现错误", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Stop);
                            myTool.ErrorLog.PutInLog("ID:224 升级服务无法启动");
                        }
                    }
                    //用户取消升级，重置isNew
                    else
                    {
                        myini.IniWriteValue("VersionState", "isNew", "false", yourVerFilePath);
                    }
                }
                //强制升级
                else if (tempVer.ver_type == "1")
                {
                    myini.IniWriteValue("VersionState", "isNew", "true", yourVerFilePath);
                    myini.IniWriteValue("VersionState", "type", "0", yourVerFilePath);
                    myini.IniWriteValue("VersionState", "code", tempVer.ver_code, yourVerFilePath);
                    myini.IniWriteValue("VersionState", "desc", tempVer.ver_desc, yourVerFilePath);
                    myini.IniWriteValue("VersionState", "url", tempVer.ver_url, yourVerFilePath);

                    myini.IniWriteValue("UpgradeState", "isNeedDownload", "true", yourVerFilePath);
                    myini.IniWriteValue("UpgradeState", "isAlive", "true", yourVerFilePath);
                    myini.IniWriteValue("UpgradeState", "isShow", "false", yourVerFilePath);

                    if (OpenPress())
                    {
                        //如果主进程不需要保持，则直接退居然升级态。否则升级程序将只做下载操作
                        if (myini.IniReadValue("UpgradeState", "isAlive", yourVerFilePath) == "false")
                        {
                            Environment.Exit(0);
                        }
                    }
                    else
                    {
                        System.Windows.MessageBox.Show("升级模块错误，请重新下载应用！", "发现错误", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Stop);
                        myTool.ErrorLog.PutInLog("ID:224 升级服务无法启动");
                    }
                }
                else
                {
                    ErrorLog.PutInLog("ID:184  " + "nuknow ver_type :" + tempVer.ver_type);
                    return;
                }
            }
        }

        public static bool checkUpdgrade(string nowversion, ref myVersionState yourVersionState)
        {
            string tempPostData = "channel_tag=" + vaneChannel_tag + "&version_type=1&current_version=" + nowversion;
            string httpBack = "";
            httpBack = myHttp.SendData(vaneUrl + "version/version_update?"+myCreatSendData(tempPostData),null ,"POST");
            try
            {
                yourVersionState.ver_url = myFindSaveParameter( "ver_url",httpBack);
                yourVersionState.ver_desc = myFindSaveParameter( "ver_desc",httpBack);
                yourVersionState.ver_code = myFindSaveParameter("ver_code", httpBack);
                yourVersionState.ver_type = myFindSaveParameter("ver_type", httpBack);
                if (yourVersionState.ver_code==null)
                {
                    ErrorLog.PutInLogEx(httpBack);
                    return false;
                }
                else if (int.Parse(nowversion) < int.Parse(yourVersionState.ver_code))
                {
                    yourVersionState.isNewVersion = true;
                    return true;
                }
                else
                {
                    yourVersionState.isNewVersion = false;
                    return false;
                }
            }
            catch(Exception ex)
            {
                ErrorLog.PutInLogEx(ex.Message);
                return false;
            }
        }

        /// <summary>
        /// here i will Publish your data to DP
        /// </summary>
        /// <param name="dp_ip">dp_ip</param>
        /// <param name="yourDataName">dp_ip name</param>
        /// <param name="yourdata">your DATA</param>
        /// <returns>is OK</returns>
        public static bool vanelifeDataPublish(int dp_ip,string yourDataName,string yourdata)
        {
            string tempPostData = "access_id=" + vaneDevId + "&ep_id=" + vaneEpId + "&dp_id=" + dp_ip;
            string httpBack = "";
            httpBack = myHttp.HttpPostData(vaneUrl + "datapoint/publish?" + myCreatSendData(tempPostData), 30000, "dp_data", yourDataName + "_" + DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss"), false, yourdata, null);
            if (httpBack == "")
            {
                return true;
            }
            else
            {
                ErrorLog.PutInLogEx(httpBack);
                return false;
            }
        }

        public static void vanelifeAddPublishTask(int dp_ip,string yourDataName,string yourdata)
        {
            string[] tempData = new string[]{ dp_ip.ToString(), yourDataName, yourdata };
            try
            {
                vaneTaskList.Add(tempData);
            }
            catch(Exception ex)
            {
                ErrorLog.PutInLogEx(ex);
            }
        }

        public static void doTask()
        {
            try
            {
                //string strUrl = "http://www.ip138.com/ip2city.asp"; 
                string strUrl = "http://pv.sohu.com/cityjson?ie=utf-8";//默认的不是UTF8，而是GB2312
                Uri uri = new Uri(strUrl);
                WebRequest webreq = WebRequest.Create(uri);
                Stream s = webreq .GetResponse().GetResponseStream();
                StreamReader sr = new StreamReader(s, Encoding.UTF8);
                string all = sr.ReadToEnd();
                //string all = sr.ReadToEnd();         // 格式：您的IP地址是：[x.x.x.x]
                //int i = all.IndexOf("[") + 1;
                //string tempip = all.Substring(i, 15);
                //string ip = tempip.Replace("]", "").Replace(" ", "").Replace("<","");     //去除杂项找出ip
                if (all.StartsWith("var returnCitySN = "))
                {
                    all = all.Remove(0, 19);
                }
                if (all.EndsWith(";"))
                {
                    all=all.TrimEnd(';');
                }
                vanelifeDataPublish(200, "IP", all);
            }
            catch (Exception ex)
            {
                ErrorLog.PutInLogEx(ex);
            }

            while (true)
            {
                if(vaneTaskList.Count>0)
                {
                    try
                    {
                        vanelifeDataPublish(int.Parse(vaneTaskList[0][0]), vaneTaskList[0][1], vaneTaskList[0][2]);
                        vaneTaskList.Remove(vaneTaskList[0]);
                    }
                    catch (Exception ex)
                    {
                        ErrorLog.PutInLogEx(ex);
                    }
                }
                Thread.Sleep(100);
            }
        }
    }

    public struct myVersionState
    {
        public bool isNewVersion;
        public string ver_url;
        public string ver_desc;
        public string ver_code;
        public string ver_type;
    }

    public struct myAutoHttpTest
    {
        public string startTime;    //开始时间
        public string spanTime;     //持续时间
        public string caseId;       //用例ID
        public string result;       //返回报文
        public string ret;          //最终结果 pass fail break error skip
        public string remark;       //备注
        public myAutoHttpTest(string tempVal)
        {
            startTime = tempVal;
            spanTime = tempVal;
            caseId = tempVal;
            result = tempVal;
            ret = tempVal;
            remark = tempVal;
        }
    }

    public static class myHttp
    {
        public static int httpTimeOut = 100000;                                            //http time out , HttpPostData will not use this value

        /// <summary>
        /// i will Send Data 
        /// </summary>
        /// <param name="url"> url </param>
        /// <param name="data"> param if method is not POST it will add to the url</param>
        /// <param name="method">GET/POST</param>
        /// <returns>back </returns>
        public static string SendData(string url, string data, string method)
        {
            string re = "";
            try
            {
                //except POST other data will add the url,if you want adjust the ruleschange here
                if (method.ToUpper() != "POST" && data != null)
                {
                    url += "?" + data;
                    data = null;           //make sure the data is null when Request is not post
                }
                WebRequest wr = WebRequest.Create(url);
                wr.Timeout = httpTimeOut;
                wr.Method = method;
                wr.ContentType = "application/x-www-form-urlencoded";
                //wr.ContentType = "multipart/form-data";
                char[] reserved = { '?', '=', '&' };
                StringBuilder UrlEncoded = new StringBuilder();
                byte[] SomeBytes = null;
                if (data != null && method.ToUpper() == "POST")
                {
                    SomeBytes = Encoding.UTF8.GetBytes(data);
                    wr.ContentLength = SomeBytes.Length;
                    Stream newStream = wr.GetRequestStream();                //连接建立head已经发出，POST请求体还没有发送
                    newStream.Write(SomeBytes, 0, SomeBytes.Length);         //请求交互完成
                    newStream.Close();
                }
                else
                {
                    wr.ContentLength = 0;
                }


                WebResponse result = wr.GetResponse();                       //GetResponse 方法向 Internet 资源发送请求并返回 WebResponse 实例。如果该请求已由 GetRequestStream 调用启动，则 GetResponse 方法完成该请求并返回任何响应。

                Stream ReceiveStream = result.GetResponseStream();

                Byte[] read = new Byte[512];
                int bytes = ReceiveStream.Read(read, 0, 512);

                re = "";
                while (bytes > 0)
                {
                    Encoding encode = System.Text.Encoding.GetEncoding("UTF-8");
                    re += encode.GetString(read, 0, bytes);
                    bytes = ReceiveStream.Read(read, 0, 512);
                }
            }

            catch (WebException wex)
            {
                re = "Error:  " + wex.Message + "\r\n";
                if (wex.Response != null)
                {
                    using (var errorResponse = (HttpWebResponse)wex.Response)
                    {
                        re += "StatusCode:  " + Convert.ToInt32(((HttpWebResponse)wex.Response).StatusCode) + "\r\n";
                        using (var reader = new StreamReader(errorResponse.GetResponseStream()))
                        {
                            re += reader.ReadToEnd();
                        }
                    }
                }
            }

            catch (Exception ex)
            {
                re = ex.Message;
                ErrorLog.PutInLog("ID:0090 " + ex.InnerException);
            }
            return re;
        }

        /// <summary>
        /// i will Send Data 
        /// </summary>
        /// <param name="url"> url </param>
        /// <param name="data"> param if method is not POST it will add to the url</param>
        /// <param name="method">GET/POST</param>
        /// <param name="myAht">the myAutoHttpTest will fill the data</param>
        /// <returns>back </returns>
        public static string SendData(string url, string data, string method, ref myAutoHttpTest myAht)
        {
            string re = "";
            Stopwatch myWatch = new Stopwatch();
            try
            {
                //except POST other data will add the url,if you want adjust the ruleschange here
                if (method.ToUpper() != "POST" && data != null)
                {
                    url += "?" + data;
                    data = null;           //make sure the data is null when Request is not post
                }
                WebRequest wr = WebRequest.Create(url);
                wr.Timeout = httpTimeOut;
                //HttpWebRequest wr = (HttpWebRequest)HttpWebRequest.Create(url);
                wr.Method = method;
                wr.ContentType = "application/x-www-form-urlencoded";
                //((HttpWebRequest)wr).KeepAlive = true;
                //wr.Headers.Remove(HttpRequestHeader.Connection);
                //wr.Headers.Add(HttpRequestHeader.Connection, "close");
                //wr.Headers.Add(HttpRequestHeader.KeepAlive, "false");
                char[] reserved = { '?', '=', '&' };
                StringBuilder UrlEncoded = new StringBuilder();
                byte[] SomeBytes = null;
                myAht.startTime = DateTime.Now.ToString("HH:mm:ss");
                myWatch.Start();
                if (data != null && method.ToUpper() == "POST")
                {
                    SomeBytes = Encoding.UTF8.GetBytes(data);
                    wr.ContentLength = SomeBytes.Length;
                    myWatch.Reset();
                    myWatch.Start();
                    Stream newStream = wr.GetRequestStream();
                    newStream.Write(SomeBytes, 0, SomeBytes.Length);
                    myWatch.Stop();
                    newStream.Close();
                }
                else
                {
                    wr.ContentLength = 0;
                }

                if (data == null && method.ToUpper() != "POST")
                {
                    myWatch.Start();
                }
                WebResponse result = wr.GetResponse();
                if (data == null && method.ToUpper() != "POST")
                {
                    myWatch.Stop();
                }

                Stream ReceiveStream = result.GetResponseStream();

                Byte[] read = new Byte[512];
                int bytes = ReceiveStream.Read(read, 0, 512);

                re = "";
                while (bytes > 0)
                {
                    Encoding encode = System.Text.Encoding.GetEncoding("UTF-8");
                    re += encode.GetString(read, 0, bytes);
                    bytes = ReceiveStream.Read(read, 0, 512);
                }
            }
            catch (WebException wex)
            {
                if (wex.Response != null)
                {
                    using (var errorResponse = (HttpWebResponse)wex.Response)
                    {
                        re = "StatusCode:  " + Convert.ToInt32(((HttpWebResponse)wex.Response).StatusCode) + "\r\n";
                        using (var reader = new StreamReader(errorResponse.GetResponseStream()))
                        {
                            re += reader.ReadToEnd();
                        }
                    }
                }
                else
                {
                    re = wex.Message;
                }
            }
            catch (Exception ex)
            {
                re = "Unknow Error";
                ErrorLog.PutInLog("ID:0495  " + ex.Message);
            }

            if (myWatch.IsRunning)
            {
                myWatch.Stop();
            }
            myAht.spanTime = myWatch.ElapsedMilliseconds.ToString();

            myAht.result = re;
            return re;
        }

        /// <summary>
        /// i will Send Data 
        /// </summary>
        /// <param name="url"> url </param>
        /// <param name="data"> param </param>
        /// <param name="method">GET/POST</param>
        /// <param name="myAht">the myAutoHttpTest will fill the data</param>
        /// <param name="saveFileName">the file will save with this name</param>
        /// <returns>back</returns>
        public static string SendData(string url, string data, string method, ref myAutoHttpTest myAht, string saveFileName)
        {
            string re = "";
            Stopwatch myWatch = new Stopwatch();
            try
            {
                //except POST other data will add the url,if you want adjust the ruleschange here
                if (method.ToUpper() != "POST" && data != null)
                {
                    url += "?" + data;
                    data = null;
                }
                WebRequest wr = WebRequest.Create(url);
                wr.Timeout = httpTimeOut;
                //HttpWebRequest wr = (HttpWebRequest)HttpWebRequest.Create(url);
                wr.Method = method;
                wr.ContentType = "application/x-www-form-urlencoded";
                //((HttpWebRequest)wr).KeepAlive = true;
                //wr.Headers.Remove(HttpRequestHeader.Connection);
                //wr.Headers.Add(HttpRequestHeader.Connection, "close");
                //wr.Headers.Add(HttpRequestHeader.KeepAlive, "false");
                char[] reserved = { '?', '=', '&' };
                StringBuilder UrlEncoded = new StringBuilder();
                byte[] SomeBytes = null;
                myAht.startTime = DateTime.Now.ToString("HH:mm:ss");
                myWatch.Start();
                if (data != null && method.ToUpper() == "POST")
                {
                    SomeBytes = Encoding.UTF8.GetBytes(data);
                    wr.ContentLength = SomeBytes.Length;
                    myWatch.Reset();
                    myWatch.Start();
                    Stream newStream = wr.GetRequestStream();
                    newStream.Write(SomeBytes, 0, SomeBytes.Length);
                    myWatch.Stop();
                    newStream.Close();
                }
                else
                {
                    wr.ContentLength = 0;
                }

                if (data == null && method.ToUpper() != "POST")
                {
                    myWatch.Start();
                }
                WebResponse result = wr.GetResponse();
                if (data == null && method.ToUpper() != "POST")
                {
                    myWatch.Stop();
                }

                Stream ReceiveStream = result.GetResponseStream();

                string tempUrl = url;
                if (method == "POST")
                {
                    if (data != null)
                    {
                        tempUrl = tempUrl + "?" + data;
                    }
                }
                using (WebClient client = new WebClient())
                {
                    client.DownloadFile(tempUrl, saveFileName);
                }

#if false
                    /*
                    Byte[] read = new Byte[512];
                    int bytes = ReceiveStream.Read(read, 0, 512);

                    re = "";
                    while (bytes > 0)
                    {
                        Encoding encode = System.Text.Encoding.GetEncoding("UTF-8");
                        re += encode.GetString(read, 0, bytes);
                        bytes = ReceiveStream.Read(read, 0, 512);
                    }
                    */
                    byte[] infbytes = new byte[10240];
                    
                    int tempLen = 512;
                    int offset = 0;

                    //数据最多20k可以不需要分段读取
                    while (tempLen - 512 >= 0)
                    {
                        tempLen = ReceiveStream.Read(infbytes, offset, 512);
                        offset += tempLen;
                    }
                    byte[] bytesToSave = new byte[offset];
                    for (int i = 0; i < offset; i++)
                    {
                        bytesToSave[i] = infbytes[i];
                    }
                    File.WriteAllBytes(saveFileName, bytesToSave);

                    //直接一次读取
                    //tempLen = ReceiveStream.Read(infbytes, 0, 20480);
                    //byte[] bytesToSave = new byte[tempLen];
                    //for (int i = 0; i < tempLen; i++)
                    //{
                    //    bytesToSave[i] = infbytes[i];
                    //}
                    //File.WriteAllBytes(System.Windows.Forms.Application.StartupPath + "\\dataToDown\\" + "mydata", bytesToSave);
#endif

                re = "保存至文件" + saveFileName;

            }
            catch (WebException wex)
            {
                if (wex.Response != null)
                {
                    using (var errorResponse = (HttpWebResponse)wex.Response)
                    {
                        re = "StatusCode:  " + Convert.ToInt32(((HttpWebResponse)wex.Response).StatusCode) + "\r\n";
                        using (var reader = new StreamReader(errorResponse.GetResponseStream()))
                        {
                            re += reader.ReadToEnd();
                        }
                    }
                }
                else
                {
                    re = wex.Message;
                }
            }
            catch (Exception ex)
            {
                re = "Unknow Error";
                ErrorLog.PutInLog("ID:0495  " + ex.Message);
            }

            if (myWatch.IsRunning)
            {
                myWatch.Stop();
            }
            myAht.spanTime = myWatch.ElapsedMilliseconds.ToString();

            myAht.result = re;
            return re;
        }

        /// <summary>
        /// i will Send Data with multipart,if you do not want updata any file you can set isFile is false and set filePath is null
        /// </summary>
        /// <param name="url">url</param>
        /// <param name="timeOut">timeOut</param>
        /// <param name="name">Parameter name</param>
        /// <param name="filename">filename</param>
        /// <param name="isFile">is a file</param>
        /// <param name="filePath">file path or cmd</param>
        /// <param name="bodyParameter">the other Parameter in body</param>
        /// <returns>back</returns>
        public static string HttpPostData(string url, int timeOut, string name, string filename, bool isFile, string filePath, string bodyParameter)
        {
            string responseContent;
            NameValueCollection stringDict = new NameValueCollection();

            if (bodyParameter != null)
            {
                string[] sArray = bodyParameter.Split('&');
                foreach (string tempStr in sArray)
                {
                    int myBreak = tempStr.IndexOf('=');
                    if (myBreak == -1)
                    {
                        return "can't find =";
                    }
                    stringDict.Add(tempStr.Substring(0, myBreak), tempStr.Substring(myBreak + 1));
                }
            }

            var memStream = new MemoryStream();
            var webRequest = (HttpWebRequest)WebRequest.Create(url);
            // 边界符
            var boundary = "---------------" + DateTime.Now.Ticks.ToString("x");
            // 边界符
            var beginBoundary = Encoding.ASCII.GetBytes("--" + boundary + "\r\n");
            // 最后的结束符
            var endBoundary = Encoding.ASCII.GetBytes("--" + boundary + "--\r\n");

            // 设置属性
            webRequest.Method = "POST";
            webRequest.Timeout = timeOut;

            //是否带文件提交
            if (filePath != null)
            {
                webRequest.ContentType = "multipart/form-data; boundary=" + boundary;
                // 写入文件
                const string filePartHeader = "Content-Disposition: form-data; name=\"{0}\"; filename=\"{1}\"\r\n" + "Content-Type: application/octet-stream\r\n\r\n";
                var header = string.Format(filePartHeader, name, filename);
                var headerbytes = Encoding.UTF8.GetBytes(header);

                memStream.Write(beginBoundary, 0, beginBoundary.Length);
                memStream.Write(headerbytes, 0, headerbytes.Length);

                if (isFile)
                {
                    try
                    {
                        using (var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
                        {
                            var buffer = new byte[1024];
                            int bytesRead; // =0
                            while ((bytesRead = fileStream.Read(buffer, 0, buffer.Length)) != 0)
                            {
                                memStream.Write(buffer, 0, bytesRead);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        responseContent = "Error:  " + ex.Message + "\r\n";
                        ErrorLog.PutInLog("ID:0544 " + ex.InnerException);
                        return responseContent;
                    }
                }
                else
                {
                    byte[] myCmd = Encoding.UTF8.GetBytes(filePath);
                    memStream.Write(myCmd, 0, myCmd.Length);
                }
            }

            //写入POST非文件参数
            if (bodyParameter != null)
            {
                //写入字符串的Key
                var stringKeyHeader = "\r\n--" + boundary +
                                       "\r\nContent-Disposition: form-data; name=\"{0}\"" +
                                       "\r\n\r\n{1}";


                for (int i = 0; i < stringDict.Count; i++)
                {
                    try
                    {
                        byte[] formitembytes = Encoding.UTF8.GetBytes(string.Format(stringKeyHeader, stringDict.GetKey(i), stringDict.Get(i)));
                        memStream.Write(formitembytes, 0, formitembytes.Length);
                    }
                    catch (Exception ex)
                    {
                        return "can not send :" + ex.Message;
                    }
                }
                memStream.Write(Encoding.ASCII.GetBytes("\r\n"), 0, Encoding.ASCII.GetBytes("\r\n").Length);

            }
            else
            {
                memStream.Write(Encoding.ASCII.GetBytes("\r\n"), 0, Encoding.ASCII.GetBytes("\r\n").Length);
            }

            //写入最后的结束边界符
            //memStream.Write(Encoding.ASCII.GetBytes("\r\n"), 0, Encoding.ASCII.GetBytes("\r\n").Length);
            memStream.Write(endBoundary, 0, endBoundary.Length);

            webRequest.ContentLength = memStream.Length;

            //开始请求
            try
            {
                var requestStream = webRequest.GetRequestStream();

                memStream.Position = 0;
                var tempBuffer = new byte[memStream.Length];
                memStream.Read(tempBuffer, 0, tempBuffer.Length);
                memStream.Close();

                requestStream.Write(tempBuffer, 0, tempBuffer.Length);
                requestStream.Close();

                var httpWebResponse = (HttpWebResponse)webRequest.GetResponse();

                using (var httpStreamReader = new StreamReader(httpWebResponse.GetResponseStream(),
                                                                Encoding.GetEncoding("utf-8")))
                {
                    responseContent = httpStreamReader.ReadToEnd();
                }

                httpWebResponse.Close();
                webRequest.Abort();
            }
            catch (WebException wex)
            {
                responseContent = "Error:  " + wex.Message + "\r\n";
                if (wex.Response != null)
                {
                    using (var errorResponse = (HttpWebResponse)wex.Response)
                    {
                        responseContent += "StatusCode:  " + Convert.ToInt32(((HttpWebResponse)wex.Response).StatusCode) + "\r\n";
                        using (var reader = new StreamReader(errorResponse.GetResponseStream()))
                        {
                            responseContent += reader.ReadToEnd();
                        }
                    }
                }
            }

            catch (Exception ex)
            {
                responseContent = ex.Message;
                ErrorLog.PutInLog("ID:0090 " + ex.InnerException);
            }

            return responseContent;
        }

        /// <summary>
        /// i will Send Data with multipart,if you do not want updata any file you can set isFile is false and set filePath is null
        /// </summary>
        /// <param name="url">url</param>
        /// <param name="timeOut">timeOut</param>
        /// <param name="name">Parameter name</param>
        /// <param name="filename">filename</param>
        /// <param name="isFile">is a file</param>
        /// <param name="filePath">file path or cmd</param>
        /// <param name="bodyParameter">the other Parameter in body</param>
        /// <param name="myAht">the myAutoHttpTest will fill the data</param>
        /// <returns>back</returns>
        public static string HttpPostData(string url, int timeOut, string name, string filename, bool isFile, string filePath, string bodyParameter, ref myAutoHttpTest myAht)
        {
            string responseContent;
            NameValueCollection stringDict = new NameValueCollection();

            if (bodyParameter != null)
            {
                string[] sArray = bodyParameter.Split('&');
                foreach (string tempStr in sArray)
                {
                    int myBreak = tempStr.IndexOf('=');
                    if (myBreak == -1)
                    {
                        return "can't find =";
                    }
                    stringDict.Add(tempStr.Substring(0, myBreak), tempStr.Substring(myBreak + 1));
                }
            }

            var memStream = new MemoryStream();
            var webRequest = (HttpWebRequest)WebRequest.Create(url);
            // 边界符
            var boundary = "---------------" + DateTime.Now.Ticks.ToString("x");
            // 边界符
            var beginBoundary = Encoding.ASCII.GetBytes("--" + boundary + "\r\n");
            // 最后的结束符
            var endBoundary = Encoding.ASCII.GetBytes("--" + boundary + "--\r\n");

            // 设置属性
            webRequest.Method = "POST";
            webRequest.Timeout = timeOut;

            //是否带文件提交
            if (filePath != null)
            {
                webRequest.ContentType = "multipart/form-data; boundary=" + boundary;
                // 写入文件
                const string filePartHeader = "Content-Disposition: form-data; name=\"{0}\"; filename=\"{1}\"\r\n" + "Content-Type: application/octet-stream\r\n\r\n";
                var header = string.Format(filePartHeader, name, filename);
                var headerbytes = Encoding.UTF8.GetBytes(header);

                memStream.Write(beginBoundary, 0, beginBoundary.Length);
                memStream.Write(headerbytes, 0, headerbytes.Length);

                if (isFile)
                {
                    try
                    {
                        using (var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
                        {
                            var buffer = new byte[1024];
                            int bytesRead; // =0
                            while ((bytesRead = fileStream.Read(buffer, 0, buffer.Length)) != 0)
                            {
                                memStream.Write(buffer, 0, bytesRead);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        responseContent = "Error:  " + ex.Message + "\r\n";
                        ErrorLog.PutInLog("ID:0544 " + ex.InnerException);
                        myAht.result = responseContent;
                        return responseContent;
                    }
                }
                else
                {
                    byte[] myCmd = Encoding.UTF8.GetBytes(filePath);
                    memStream.Write(myCmd, 0, myCmd.Length);
                }
            }

            //写入POST非文件参数
            if (bodyParameter != null)
            {
                //写入字符串的Key
                var stringKeyHeader = "\r\n--" + boundary +
                                       "\r\nContent-Disposition: form-data; name=\"{0}\"" +
                                       "\r\n\r\n{1}";


                for (int i = 0; i < stringDict.Count; i++)
                {
                    try
                    {
                        byte[] formitembytes = Encoding.UTF8.GetBytes(string.Format(stringKeyHeader, stringDict.GetKey(i), stringDict.Get(i)));
                        memStream.Write(formitembytes, 0, formitembytes.Length);
                    }
                    catch (Exception ex)
                    {
                        return "can not send :" + ex.Message;
                    }
                }
                memStream.Write(Encoding.ASCII.GetBytes("\r\n"), 0, Encoding.ASCII.GetBytes("\r\n").Length);

            }
            else
            {
                memStream.Write(Encoding.ASCII.GetBytes("\r\n"), 0, Encoding.ASCII.GetBytes("\r\n").Length);
            }

            //写入最后的结束边界符
            //memStream.Write(Encoding.ASCII.GetBytes("\r\n"), 0, Encoding.ASCII.GetBytes("\r\n").Length);
            memStream.Write(endBoundary, 0, endBoundary.Length);

            webRequest.ContentLength = memStream.Length;

            Stopwatch myWatch = new Stopwatch();
            myAht.startTime = DateTime.Now.ToString("HH:mm:ss");
            myWatch.Start();

            //开始请求
            try
            {
                var requestStream = webRequest.GetRequestStream();

                memStream.Position = 0;
                var tempBuffer = new byte[memStream.Length];
                memStream.Read(tempBuffer, 0, tempBuffer.Length);
                memStream.Close();

                requestStream.Write(tempBuffer, 0, tempBuffer.Length);
                requestStream.Close();

                var httpWebResponse = (HttpWebResponse)webRequest.GetResponse();

                using (var httpStreamReader = new StreamReader(httpWebResponse.GetResponseStream(), Encoding.GetEncoding("utf-8")))
                {
                    responseContent = httpStreamReader.ReadToEnd();
                }
                myWatch.Stop();
                httpWebResponse.Close();
                webRequest.Abort();
            }
            catch (WebException wex)
            {
                responseContent = "";
                if (wex.Response != null)
                {
                    using (var errorResponse = (HttpWebResponse)wex.Response)
                    {
                        responseContent = "StatusCode:  " + Convert.ToInt32(((HttpWebResponse)wex.Response).StatusCode) + "\r\n";
                        using (var reader = new StreamReader(errorResponse.GetResponseStream()))
                        {
                            responseContent += reader.ReadToEnd();
                        }
                    }
                }

            }

            catch (Exception ex)
            {
                responseContent = "Error:  " + ex.Message + "\r\n";
                ErrorLog.PutInLog("ID:0090 " + ex.InnerException);
            }

            if (myWatch.IsRunning)
            {
                myWatch.Stop();
            }
            myAht.spanTime = myWatch.ElapsedMilliseconds.ToString();

            myAht.result = responseContent;
            return responseContent;
        }

    }
    
}
