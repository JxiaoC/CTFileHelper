using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Net;
using System.Collections;
using System.Threading;
using System.Windows.Forms;

namespace CtFile {
    public class UpFile {
        /// <summary>
        /// 文件大小
        /// </summary>
        public long Size { get; set; }

        /// <summary>
        /// 目前已上传文件大小
        /// </summary>
        public long NowSize { get; set; }

        /// <summary>
        /// 目前上传进度
        /// </summary>
        public double NowProgress { get; set; }

        string _FolderName = "";
        string _FilePath = "";
        string _SaveName = "";
        public string DownUrl = "";
        public bool IsComplete = false;
        /// <summary>
        /// 上传文件
        /// </summary>
        /// <param name="FilePath">本地文件路径</param>
        /// <param name="FolderName">城通文件夹名称</param>
        /// <returns></returns>
        public void Up(string FilePath, string FolderName, string SaveName = "") {
            _FolderName = FolderName;
            _FilePath = FilePath;
            _SaveName = SaveName;
            new Thread(new ThreadStart(Thread_Start)).Start();
        }

        void Thread_Start() {
            try {
                string UpUrl = "";
                int FolderID = Folder.GetFolderID(_FolderName);
                string Html = NewC.GET_HTTP(PublicConfig.File.GetUpUrl.Replace("{folderid}", FolderID.ToString()), PublicConfig.Cookie, false);
                UpUrl = NewC.GetString(Html, "initUpload('", "',", false, 0);
                FileInfo Info = new FileInfo(_FilePath);
                string Name = (_SaveName == "" ? Info.Name : _SaveName);
                Name = FilterName(Name);
                string Data = _UpFile(UpUrl, _FilePath, Name, Info.Length);
                DownUrl = GetFileDownUrl(Convert.ToInt32(Data), FolderID);
            }
            catch { }
            IsComplete = true;
        }

        private string _UpFile(string address, string fileNamePath, string saveName, long FileLength) {
            string returnValue = "";     // 要上传的文件 
            FileStream fs = new FileStream(fileNamePath, FileMode.Open, FileAccess.Read);
            BinaryReader r = new BinaryReader(fs);     //时间戳 
            string strBoundary = "----WebKitFormBoundary" + DateTime.Now.Ticks.ToString("x");
            byte[] boundaryBytes = Encoding.ASCII.GetBytes("\r\n--" + strBoundary + "\r\n");     //请求头部信息 
            StringBuilder sb = new StringBuilder();
            sb.Append(string.Format(@"--{0}
Content-Disposition: form-data; name=""name""

{2}
--{0}
Content-Disposition: form-data; name=""filesize""

{1}
--{0}
Content-Disposition: form-data; name=""file""; filename=""{2}""
Content-Type: application/x-msdownload

", strBoundary, FileLength, saveName));
            string strPostHeader = sb.ToString();
            byte[] postHeaderBytes = Encoding.UTF8.GetBytes(strPostHeader);     // 根据uri创建HttpWebRequest对象 
            HttpWebRequest httpReq = (HttpWebRequest)WebRequest.Create(new Uri(address));
            httpReq.Timeout = int.MaxValue;
            httpReq.ReadWriteTimeout = int.MaxValue;
            httpReq.Method = "POST";     //对发送的数据不使用缓存 
            httpReq.AllowWriteStreamBuffering = false;     //设置获得响应的超时时间（300秒） 
            httpReq.ContentType = @"multipart/form-data; boundary=" + strBoundary;
            httpReq.Accept = "*/*";
            httpReq.Headers.Add("Accept-Language", "zh-CN");
            httpReq.Headers.Add("Accept-Encoding", "gzip,deflate");
            httpReq.UserAgent = "Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Maxthon/4.4.7.3000 Chrome/30.0.1599.101 Safari/537.36";
            httpReq.Referer = "https://home.ctfile.com/";
            httpReq.KeepAlive = true;
            long length = fs.Length + postHeaderBytes.Length + boundaryBytes.Length;
            long fileLength = fs.Length;
            Size = fileLength;
            httpReq.ContentLength = length;
            try {
                int bufferLength = 40960;
                byte[] buffer = new byte[bufferLength]; //已上传的字节数 
                long offset = 0;         //开始上传时间 
                DateTime startTime = DateTime.Now;
                int size = r.Read(buffer, 0, bufferLength);
                Stream postStream = httpReq.GetRequestStream();         //发送请求头部消息 
                postStream.Write(postHeaderBytes, 0, postHeaderBytes.Length);
                while (size > 0) {
                    postStream.Write(buffer, 0, size);
                    offset += size;
                    NowSize += size;
                    NowProgress = (double)NowSize / Size * 100;
                    size = r.Read(buffer, 0, bufferLength);
                }
                //Console.CursorTop++;
                //添加尾部的时间戳 
                postStream.Write(boundaryBytes, 0, boundaryBytes.Length);
                postStream.Close();         //获取服务器端的响应 
                WebResponse webRespon = httpReq.GetResponse();
                Stream s = webRespon.GetResponseStream();
                //读取服务器端返回的消息
                StreamReader sr = new StreamReader(s);
                String sReturnString = sr.ReadToEnd();
                s.Close();
                sr.Close();
                return sReturnString;
            }
            catch (Exception ee) {
                returnValue = ee.ToString();
            }
            finally {
                fs.Close();
                r.Close();
            }
            return returnValue;
        }

        /// <summary>
        /// 获取文件下载地址
        /// </summary>
        /// <param name="FileID"></param>
        /// <param name="FolderID"></param>
        /// <returns></returns>
        public static string GetFileDownUrl(int FileID, int FolderID) {
            if (!Directory.Exists(PublicConfig.CacheDir)) Directory.CreateDirectory(PublicConfig.CacheDir);

            string CachePath = $"{PublicConfig.CacheDir}\\floder{NewC.Get_MD5(FileID.ToString(), false)}";
            if (File.Exists(CachePath)) {
                return File.ReadAllText(CachePath);
            }
            for (int page = 1; page <= 11; page++) {
                string Data = NewC.GET_HTTP(PublicConfig.File.GetUserDownUrl.Replace("{skip}", ((page - 1) * 100).ToString()).Replace("{folderid}", FolderID.ToString()), PublicConfig.Cookie, false);
                if (Data == "") return FileID.ToString();
                Dictionary<string, object> Dic = JsonToDictionary.JSONToObject<Dictionary<string, object>>(Data);
                ArrayList AL = (ArrayList)Dic["aaData"];
                foreach (ArrayList item in AL) {
                    if (item[1].ToString().Contains(FileID.ToString())) {
                        string Url = NewC.GetString(item[1].ToString(), "href=\"", "\">", false, 0);
                        File.WriteAllText(CachePath, Url);
                        return Url;
                    }
                }
            }
            return FileID.ToString();
        }

        /// <summary>
        /// 替换非法关键词
        /// </summary>
        /// <param name="Name"></param>
        /// <returns></returns>
        private static string FilterName(string Name) {
            if (File.Exists(Application.StartupPath + "\\flist.txt")) {
                string List = File.ReadAllText(Application.StartupPath + "\\flist.txt", Encoding.UTF8);
                foreach (var item in List.Split('_')) {
                    Name = Name.Replace(item, Substitutes(item));
                }
            }
            return Name;
        }

        /// <summary>
        /// 返回相同长度的x作为替换
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        private static string Substitutes(string item) {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < item.Length; i++) {
                sb.Append("x");
            }
            return sb.ToString();
        }
    }
}
