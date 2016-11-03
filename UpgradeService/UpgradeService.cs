using SharpCompress.Common;
using SharpCompress.Reader;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Windows.Forms;
using UpgradeService.myTool;

namespace UpgradeService
{
    public partial class UpgradeService : Form
    {
        public UpgradeService()
        {
            InitializeComponent();
        }

        public struct myVersionState
        {
            public bool isNewVersion;
            public string ver_url;
            public string ver_desc;
            public string ver_code;
            public string ver_type;
        }

        string applicationPath = System.Environment.CurrentDirectory;        //默认脚本路径
        myVersionState nowVersion = new myVersionState();                    //版本状态
        bool isAlive = true;                                                 //被升级应用是否正在运行
        bool isShow = true;                                                  //是否显示升级窗口
        bool isNeedDownload = true;                                          //升级文件是否已经下载
        //string upgradeMode = "0";                                            //0：标识默认状态； 1：标识用户发起升级 2：标识静默升级

        /// <summary>
        /// i will down load the file by Async
        /// </summary>
        /// <param name="sourceUrl">file url</param>
        /// <param name="yourFilePath">file path to sava</param>
        public void downloadFiles(string sourceUrl, string yourFilePath)
        {
            label_info.Text = "正在下载···";
            using (WebClient client = new WebClient())
            {
                client.DownloadFileAsync(new Uri(sourceUrl), yourFilePath);
                client.DownloadProgressChanged += client_DownloadProgressChanged;
                client.DownloadFileCompleted += client_DownloadFileCompleted;
            }
        }

        void client_DownloadFileCompleted(object sender, AsyncCompletedEventArgs e)
        {
            if (!isAlive)
            {
                myReplace();
                startMain(applicationPath + "\\TouchHandle.exe");
            }
            else
            {
                myini.IniWriteValue("UpgradeState", "isNeedDownload","false"  ,applicationPath + "\\config\\version.thd");
                this.Close();
            }
        }

        void client_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            Action<DownloadProgressChangedEventArgs> OnChange = myProceChange;
            OnChange.Invoke(e);
        }


        private void myProceChange(DownloadProgressChangedEventArgs e)
        {
            //pictureBox_load.Image = imageList_downLoad.Images[e.ProgressPercentage];

            System.Reflection.Assembly asm = System.Reflection.Assembly.GetEntryAssembly();
            System.IO.Stream imgStream = asm.GetManifestResourceStream("UpgradeService.myResources.frame_" + e.ProgressPercentage + ".png");
            pictureBox_load.Image = System.Drawing.Image.FromStream(imgStream);
            pictureBox_load.Update();
        }

      

        private void UpgradeService_Load(object sender, EventArgs e)
        {
            pictureBox_decompression.Parent = pictureBox_load;
            label_info.Parent = pictureBox_load;

            nowVersion.isNewVersion = (myini.IniReadValue("VersionState", "isNew", applicationPath + "\\config\\version.thd") == "true");
            nowVersion.ver_code = myini.IniReadValue("VersionState", "code", applicationPath + "\\config\\version.thd");
            nowVersion.ver_desc = myini.IniReadValue("VersionState", "desc", applicationPath + "\\config\\version.thd");
            nowVersion.ver_type = myini.IniReadValue("VersionState", "type", applicationPath + "\\config\\version.thd");
            nowVersion.ver_url = myini.IniReadValue("VersionState", "url", applicationPath + "\\config\\version.thd");

            isAlive = (myini.IniReadValue("UpgradeState", "isAlive", applicationPath + "\\config\\version.thd") == "true");
            isShow = (myini.IniReadValue("UpgradeState", "isShow", applicationPath + "\\config\\version.thd") == "true");
            isNeedDownload = (myini.IniReadValue("UpgradeState", "isNeedDownload", applicationPath + "\\config\\version.thd") == "true");

            if (nowVersion.isNewVersion)
            {
                //是否显示升级界面
                if(!isShow)
                {
                    this.Hide();
                    this.ShowInTaskbar = false;
                    this.WindowState = FormWindowState.Minimized;
                }
                //是否需要下载
                if (isNeedDownload)
                {
                     downloadFiles(nowVersion.ver_url, applicationPath + "\\upgrade\\updata.bin");    
                }
                else
                {
                    myReplace();
                    startMain(applicationPath + "\\TouchHandle.exe");
                }
            }
            else
            {
                myTool.ErrorLog.PutInLog("ID:00116 没有必要的启动");
                Environment.Exit(0);
            }
        }

        public void myReplace()
        {
            try
            {
                pictureBox_decompression.Image = imageList_downLoad.Images[0];
                pictureBox_decompression.Update();
                label_info.Text = "正在安装···";
                label_info.Update();
                myDecompression(applicationPath + "\\upgrade\\updata.bin", applicationPath);
                label_info.Text = "升级成功···";
                pictureBox_decompression.Image = imageList_downLoad.Images[1];
                pictureBox_decompression.Update();
                myini.IniWriteValue("VersionState", "isNew", "false", applicationPath + "\\config\\version.thd");
            }
            catch(Exception ex)
            {
                myini.IniWriteValue("VersionState", "isNew", "false", applicationPath + "\\config\\version.thd");
                myTool.ErrorLog.PutInLogEx(ex.Message);
            }
        }

        public void startMain(string appName)
        {
            try
            {
                if (!OpenPress(appName, ""))
                {
                    myTool.ErrorLog.PutInLog("ID:00141 主应用程序丢失");
                }
                this.Close();
            }
            catch(Exception ex)
            {
                myTool.ErrorLog.PutInLogEx(ex.Message);
                Environment.Exit(0);
            }
        }

        /// <summary>
        /// 启动其他程序
        /// </summary>
        /// <param name="FileName">需要启动的外部程序名称</param>
        public static bool OpenPress(string FileName, string Arguments)
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

        public void myDecompression(string souceData, string yourfilePath)
        {

            using (Stream stream = File.OpenRead(souceData))
            {
                var reader = ReaderFactory.Open(stream);
                while (reader.MoveToNextEntry())
                {
                    if (!reader.Entry.IsDirectory)
                    {
                        Console.WriteLine(reader.Entry.FilePath);
                        reader.WriteEntryToDirectory(yourfilePath, ExtractOptions.ExtractFullPath | ExtractOptions.Overwrite);
                    }
                }
            }

        }



        public void GetFrames(string pPath, string pSavedPath) 
        {
           //eg:GetFrames(applicationPath + "\\souce\\down", applicationPath + "\\souce\\xx");
           Image gif = Image.FromFile(pPath); 
           FrameDimension fd = new FrameDimension(gif.FrameDimensionsList[0]); 
           //获取帧数(gif图片可能包含多帧，其它格式图片一般仅一帧) 
           int count = gif.GetFrameCount(fd); 
           //以Jpeg格式保存各帧 
           for (int i = 0; i < count; i++) 
           {
               gif.SelectActiveFrame(fd, i);
               gif.Save(pSavedPath + "\\frame_" + i + ".png", ImageFormat.Png); 

           }
       }


    }
}
