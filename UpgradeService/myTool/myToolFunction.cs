using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace UpgradeService.myTool
{
    class myToolFunction
    {
    }

    /// <summary>
    /// put in error message in file
    /// </summary>
    class ErrorLog
    {
        #region 内部成员
        private static string FilePath = System.IO.Path.GetDirectoryName(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName) + "\\log\\" + DateTime.Now.ToString("yyyy.MM.dd")+"upgrade" + ".txt";
        private static bool isUsing = false;                                                                                                            //this log path may using where you call him
        private static List<string> unHandleLogs = new List<string>();     
        #endregion

        /// <summary>
        /// here i will save the log not handle 
        /// </summary>
        private static void savaUnHandleLogs()
        {
            if (unHandleLogs.Count > 0)
            {
                if (isUsing == false)
                {
                    isUsing = true;
                    FileStream fs;
                    if (!File.Exists(FilePath))
                    {
                        fs = new FileStream(FilePath, FileMode.Create, FileAccess.Write);//创建写入文件        
                    }
                    else
                    {
                        fs = new FileStream(FilePath, FileMode.Append, FileAccess.Write);
                    }
                    StreamWriter sw = new StreamWriter(fs);
                    foreach (string tempLog in unHandleLogs)
                    {
                        sw.WriteLine(tempLog);
                    }
                    unHandleLogs.Clear();
                    sw.Close();
                    fs.Close();
                    isUsing = false;
                }
            }
        }

        /// <summary>
        /// get now line
        /// </summary>
        /// <returns>line</returns>
        public static int GetLineNum(int skipFrames)
        {
            System.Diagnostics.StackTrace st = new System.Diagnostics.StackTrace(skipFrames, true);
            return st.GetFrame(0).GetFileLineNumber();
        }


        /// <summary>
        /// get now file
        /// </summary>
        /// <returns>file</returns>
        public static string GetCurSourceFileName(int skipFrames)
        {
            System.Diagnostics.StackTrace st = new System.Diagnostics.StackTrace(skipFrames, true);
            return st.GetFrame(0).GetFileName();
        }

        /// <summary>
        /// i will pu int log in the file with any other message 
        /// </summary>
        /// <param name="errorMessage"> your message</param>
        public static void PutInLog(string errorMessage)
        {
            if (isUsing == false)
            {
                isUsing = true;
                if (!File.Exists(FilePath))
                {
                    FileStream fs = new FileStream(FilePath, FileMode.Create, FileAccess.Write);//创建写入文件 
                    StreamWriter sw = new StreamWriter(fs);
                    sw.WriteLine(errorMessage);//开始写入值
                    sw.Close();
                    fs.Close();
                }
                else
                {
                    FileStream fs = new FileStream(FilePath, FileMode.Append, FileAccess.Write);
                    StreamWriter sw = new StreamWriter(fs);
                    sw.WriteLine("++++++++++++++++++++");
                    sw.WriteLine(errorMessage);//开始写入值
                    sw.Close();
                    fs.Close();
                }
                isUsing = false;
            }
            else
            {
                unHandleLogs.Add(errorMessage);
            }
        }

        /// <summary>
        /// i will pu int log in the file with the error code Position
        /// </summary>
        /// <param name="errorMessage">your message</param>
        public static void PutInLogEx(string errorMessage)
        {
            errorMessage = "ErrorFile: " + GetCurSourceFileName(2) + "\r\n" + "ErrorLine: " + GetLineNum(2) + "\r\n" + errorMessage;
            if (isUsing == false)
            {
                isUsing = true;
                if (!File.Exists(FilePath))
                {
                    FileStream fs = new FileStream(FilePath, FileMode.Create, FileAccess.Write);//创建写入文件 
                    StreamWriter sw = new StreamWriter(fs);
                    sw.WriteLine(errorMessage);//开始写入值
                    sw.Close();
                    fs.Close();
                }
                else
                {
                    FileStream fs = new FileStream(FilePath, FileMode.Append, FileAccess.Write);
                    StreamWriter sw = new StreamWriter(fs);
                    sw.WriteLine("++++++++++++++++++++");
                    sw.WriteLine(errorMessage);//开始写入值
                    sw.Close();
                    fs.Close();
                }
                isUsing = false;
            }
            else
            {
                unHandleLogs.Add(errorMessage);
            }
        }

        /// <summary>
        ///  i will pu int log in the file with the error code Position and you can set skipFrames
        /// </summary>
        /// <param name="errorMessage"> your message</param>
        /// <param name="skipFrames">skipFrames</param>
        public static void PutInLogEx(string errorMessage, int skipFrames)
        {
            errorMessage = "ErrorFile: " + GetCurSourceFileName(skipFrames) + "\r\n" + "ErrorLine: " + GetLineNum(skipFrames) + "\r\n" + errorMessage;
            if (isUsing == false)
            {
                isUsing = true;
                if (!File.Exists(FilePath))
                {
                    FileStream fs = new FileStream(FilePath, FileMode.Create, FileAccess.Write);//创建写入文件 
                    StreamWriter sw = new StreamWriter(fs);
                    sw.WriteLine(errorMessage);//开始写入值
                    sw.Close();
                    fs.Close();
                }
                else
                {
                    FileStream fs = new FileStream(FilePath, FileMode.Append, FileAccess.Write);
                    StreamWriter sw = new StreamWriter(fs);
                    sw.WriteLine("++++++++++++++++++++");
                    sw.WriteLine(errorMessage);//开始写入值
                    sw.Close();
                    fs.Close();
                }
                isUsing = false;
            }
            else
            {
                unHandleLogs.Add(errorMessage);
            }
        }

        /// <summary>
        /// when you close your application you can call me to deal with the log you not put in the file 
        /// </summary>
        public static void closeLog()
        {
            savaUnHandleLogs();
        }
    }

    /// <summary>
    /// get or set the ini file
    /// </summary>
    static class myini
    {
        [DllImport("kernel32")]
        private static extern long WritePrivateProfileString(string section, string key, string val, string filePath);
        [DllImport("kernel32")]
        private static extern int GetPrivateProfileString(string section, string key, string def, StringBuilder retVal, int size, string filePath);

        public static void IniWriteValue(string Section, string Key, string Value, string filepath)//对ini文件进行写操作的函数
        {
            try
            {
                WritePrivateProfileString(Section, Key, Value, filepath);
            }
            catch (Exception ex)
            {
                ErrorLog.PutInLogEx("ID:D314  " + ex.Message,3);
            }
        }

        public static string IniReadValue(string Section, string Key, string filepath)//对ini文件进行读操作的函数
        {
            StringBuilder temp = new StringBuilder(255);
            try
            {
                int i = GetPrivateProfileString(Section, Key, "", temp, 255, filepath);
            }
            catch (Exception ex)
            {
                ErrorLog.PutInLogEx("ID:D327  " + ex.Message,3);
            }
            return temp.ToString();
        }
    }
}
