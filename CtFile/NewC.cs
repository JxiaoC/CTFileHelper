using RestSharp;
using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Drawing;
using System.IO;
using System.Net.Security;

namespace CtFile {
    public static class NewC {
        /// <summary>
        /// GET数据
        /// </summary>
        /// <param name="url"></param>
        /// <param name="Cookie"></param>
        /// <param name="retError"></param>
        /// <returns></returns>
        public static string GET_HTTP(string url, string Cookie, bool retError) {
            try {
                return GetHtmlForRestClient(url, Cookie);
            }
            catch (Exception ee) {
                string str = ee.ToString();
                if (retError) {
                    return "Error(错误)::" + str;
                }
                else {
                    return "";
                }
            }
        }

        /// <summary>
        /// POST数据
        /// </summary>
        /// <param name="url"></param>
        /// <param name="Cookie"></param>
        /// <param name="retError"></param>
        /// <returns></returns>
        public static string POST_HTTP(string url, string PostData, string Cookie, bool retError) {
            try {
                return PostHtmlForRestClient(url, PostData, Cookie);
            }
            catch (Exception ee) {
                string str = ee.ToString();
                if (retError) {
                    return "Error(错误)::" + str;
                }
                else {
                    return "";
                }
            }
        }

        /// <summary>
        /// 上传文件
        /// </summary>
        /// <param name="url"></param>
        /// <param name="FilePath"></param>
        /// <param name="Cookie"></param>
        /// <param name="retError"></param>
        /// <returns></returns>
        public static string UpFile_HTTP(string url, string FilePath, string Cookie, bool retError) {
            try {
                return UpFileForRestClient(url, FilePath, Cookie);
            }
            catch (Exception ee) {
                string str = ee.ToString();
                if (retError) {
                    return "Error(错误)::" + str;
                }
                else {
                    return "";
                }
            }
        }

        private static string GetHtmlForRestClient(string Url, string Cookie) {
            var client = new RestClient(Url);
            var request = new RestRequest(Method.GET);
            request.AddHeader("cache-control", "no-cache");
            string[] temp0 = Cookie.Split(';');
            for (int i = 0; i < temp0.Length; i++) {
                try {
                    string[] temp1 = temp0[i].Split('=');
                    request.AddCookie(temp1[0].Trim().ToString(), temp1[1].Trim().ToString());
                }
                catch { }
            }
            IRestResponse response = client.Execute(request);
            return response.Content;
        }

        private static string PostHtmlForRestClient(string Url, string PostData, string Cookie) {
            var client = new RestClient(Url);
            var request = new RestRequest(Method.POST);
            request.AddHeader("content-type", "application/x-www-form-urlencoded");
            request.AddHeader("cache-control", "no-cache");
            string[] temp0 = Cookie.Split(';');
            for (int i = 0; i < temp0.Length; i++) {
                try {
                    string[] temp1 = temp0[i].Split('=');
                    request.AddCookie(temp1[0].Trim().ToString(), temp1[1].Trim().ToString());
                }
                catch { }
            }
            request.AddParameter("application/x-www-form-urlencoded", PostData, ParameterType.RequestBody);
            IRestResponse response = client.Execute(request);
            return response.Content;
        }

        private static string UpFileForRestClient(string Url, string FilePath, string Cookie) {
            var client = new RestClient(Url);
            var request = new RestRequest(Method.POST);
            request.AddHeader("content-type", "application/x-www-form-urlencoded");
            request.AddHeader("cache-control", "no-cache");
            request.AddFile("file", FilePath, "application/octet-stream");
            string[] temp0 = Cookie.Split(';');
            for (int i = 0; i < temp0.Length; i++) {
                try {
                    string[] temp1 = temp0[i].Split('=');
                    request.AddCookie(temp1[0].Trim().ToString(), temp1[1].Trim().ToString());
                }
                catch { }
            }
            IRestResponse response = client.Execute(request);
            return response.Content;
        }

    

        /// 获取验证码
        /// </summary>
        ///<param name="url">需要获取的URL页面</param>
        ///<param name="referer_post">来路页面</param>
        ///<param name="keepalive">编码格式  true=GB2312  false=UTF-8</param>
        ///<param name="CookieContainer_Post">Cookies</param>
        ///<param name="cookies">返回的cookies</param>
        /// <returns></returns>
        public static byte[] Get_Vercode(string url, string referer_post, bool keepalive, CookieContainer Cookie, out string cookies) {
            Uri HttpPostUrl = new Uri(url);
            HttpWebRequest reqp;
            Random r = new Random();
            double num = r.NextDouble();
            HttpPostUrl = new Uri(url);
            reqp = ((HttpWebRequest)(WebRequest.Create(HttpPostUrl)));
            reqp.Referer = referer_post;
            //reqp.Headers.Add("cookie", Cookie);
            reqp.CookieContainer = Cookie;
            HttpWebResponse resP = ((HttpWebResponse)(reqp.GetResponse()));
            Bitmap bmp = new Bitmap(resP.GetResponseStream());
            byte[] stream = BitmapToBytes(bmp);
            resP.Close();
            cookies = reqp.CookieContainer.GetCookieHeader(new Uri(url));
            return stream;
        }

        /// <summary>
        /// 获取访问页面产生的Cookie
        /// </summary>
        /// <param name="url"></param>
        /// <param name="CookieContainer_Post"></param>
        /// <param name="cookies"></param>
        /// <returns></returns>
        public static string Get_Cookie(string url) {
            Uri HttpPostUrl = new Uri(url);
            HttpWebRequest reqp;
            Random r = new Random();
            double num = r.NextDouble();
            HttpPostUrl = new Uri(url);
            reqp = ((HttpWebRequest)(WebRequest.Create(HttpPostUrl)));
            reqp.Referer = url;
            HttpWebResponse resP = ((HttpWebResponse)(reqp.GetResponse()));
            StreamReader reader = new StreamReader(resP.GetResponseStream(), Encoding.UTF8);
            string respHTML = reader.ReadToEnd();
            resP.Close();
            return reqp.CookieContainer.GetCookieHeader(new Uri(url)); ;
        }

        /// <summary>
        /// Stream转换为byte[]
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        public static byte[] BitmapToBytes(Bitmap BitReturn) {
            MemoryStream ms = new MemoryStream();
            BitReturn.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
            return ms.GetBuffer();
        }

        #region 时间戳转为C#格式时间
        /// <summary>
        /// 时间戳转为C#格式时间
        /// </summary>
        /// <param name=”timeStamp”></param>
        /// <returns></returns>
        public static DateTime GetTime(string timeStamp) {
            DateTime dtStart = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
            long lTime = long.Parse(timeStamp + "0000000");
            TimeSpan toNow = new TimeSpan(lTime); return dtStart.Add(toNow);
        }
        #endregion
        #region DateTime时间格式转换为Unix时间戳格式
        /// <summary>
        /// DateTime时间格式转换为Unix时间戳格式
        /// </summary>
        /// <param name=”time”></param>
        /// <returns></returns>
        public static int ConvertDateTimeInt(System.DateTime time) {
            System.DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1));
            return (int)(time - startTime).TotalSeconds;
        }
        #endregion

        #region Post发送数据包

        private static bool CheckValidationResult(object sender, System.Security.Cryptography.X509Certificates.X509Certificate certificate, System.Security.Cryptography.X509Certificates.X509Chain chain, SslPolicyErrors errors) {
            return true; //总是接受   
        }
        /// <summary>
        /// 发送数据包,并且返回Cookies到Form1中的User_Cookies;
        /// </summary>
        /// <param name="URL_Post">POST请求的页面地址是什么？</param>
        /// <param name="Referer_Post">请求来路是什么？</param>
        /// <param name="Accept_Post">返回的数据格式是什么?缺省值:*/*</param>
        /// <param name="UserAgent_Post">请求的浏览器头部是什么？缺省值:Mozilla/4.0 (compatible; MSIE 8.0; Windows NT 6.0)</param>
        /// <param name="KeepAlive_Post">是否保持在线状态?</param>
        /// <param name="encoding">请求的格式是什么？true=GB2312,false=UTF-8</param>
        /// <param name="CookieContainer_Post">cookies是哪个？</param>
        /// <param name="Post_Data_String">POST发送的数据是什么？</param>
        /// /// <param name="retError">是否显示错误提示(Flase的话错误的时候返回空值)</param>
        /// <param name="msg">返回的网页源代码</param>
        /// <returns></returns>
        //xiaoc.POST_SITE_DATA_Cookie("http://www.jingainian.com/Login.aspx", "application/x-www-form-urlencoded", "application/x-www-form-urlencoded", "*/*", "Mozilla/4.0 (compatible; MSIE 8.0; Windows NT 6.0)", true, false, User_Cookies,
        public static CookieContainer POST_SITE_DATA_Cookie(string URL_Post, string Referer_Post, string Accept_Post, string UserAgent_Post, bool KeepAlive_Post, bool encoding, CookieContainer CookieContainer_Post, string Post_Data_String, bool retError, out string msg) {
            try {
                Uri httpUrl = new Uri(URL_Post);
                HttpWebRequest req = null;
                if (URL_Post.StartsWith("https", StringComparison.OrdinalIgnoreCase)) {
                    ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(CheckValidationResult);
                    req = ((HttpWebRequest)(WebRequest.Create(httpUrl)));
                    req.ProtocolVersion = HttpVersion.Version10;
                }
                else {
                    req = ((HttpWebRequest)(WebRequest.Create(httpUrl)));
                }

                req.Method = "POST";
                req.ContentType = "application/x-www-form-urlencoded";
                req.Referer = Referer_Post;
                if (Accept_Post == string.Empty) Accept_Post = "*/*";
                req.Accept = Accept_Post;
                if (UserAgent_Post == string.Empty) UserAgent_Post = "Mozilla/4.0 (compatible; MSIE 8.0; Windows NT 6.0)";
                req.UserAgent = UserAgent_Post;
                req.KeepAlive = KeepAlive_Post;
                req.CookieContainer = CookieContainer_Post;
                if (encoding) {
                    byte[] bytesData = Encoding.Default.GetBytes(Post_Data_String);

                    req.ContentLength = bytesData.Length;
                    using (Stream postStream = req.GetRequestStream()) {
                        postStream.Write(bytesData, 0, bytesData.Length);
                    }
                    HttpWebResponse res = ((HttpWebResponse)(req.GetResponse()));
                    using (StreamReader reader = new StreamReader(res.GetResponseStream(), Encoding.Default)) {
                        string respHTML = reader.ReadToEnd();
                        res.Close();
                        msg = respHTML;
                        return req.CookieContainer;
                    }
                }
                else {
                    byte[] bytesData = Encoding.GetEncoding("UTF-8").GetBytes(Post_Data_String);
                    req.ContentLength = bytesData.Length;
                    using (Stream postStream = req.GetRequestStream()) {
                        postStream.Write(bytesData, 0, bytesData.Length);
                    }
                    HttpWebResponse res = ((HttpWebResponse)(req.GetResponse()));
                    using (StreamReader reader = new StreamReader(res.GetResponseStream(), Encoding.GetEncoding("UTF-8"))) {
                        string respHTML = reader.ReadToEnd();
                        res.Close();
                        msg = respHTML;
                        return req.CookieContainer;
                    }
                }
            }
            catch (WebException ee) {
                WebResponse wr = ee.Response;
                Stream st = wr.GetResponseStream();
                StreamReader sr = new StreamReader(st, Encoding.Default);
                string sError = sr.ReadToEnd();
                sr.Close();
                st.Close();
                string str = ee.Message.ToString();
                if (retError) {
                    msg = "Error(错误)::" + str;
                    return null;
                }
                else {
                    msg = "";
                    return null;
                }
            }
        }
        public static CookieContainer POST_SITE_DATA_Cookie(string URL_Post, string Referer_Post, string Accept_Post, string UserAgent_Post, bool KeepAlive_Post, bool encoding, string CookieContainer_Post, string Post_Data_String, bool retError, out string msg) {
            try {
                Uri httpUrl = new Uri(URL_Post);
                HttpWebRequest req = null;
                if (URL_Post.StartsWith("https", StringComparison.OrdinalIgnoreCase)) {
                    ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(CheckValidationResult);
                    req = ((HttpWebRequest)(WebRequest.Create(httpUrl)));
                    req.ProtocolVersion = HttpVersion.Version10;
                }
                else {
                    req = ((HttpWebRequest)(WebRequest.Create(httpUrl)));
                }

                req.Method = "POST";
                req.ContentType = "application/x-www-form-urlencoded";
                req.Referer = Referer_Post;
                if (Accept_Post == string.Empty) Accept_Post = "*/*";
                req.Accept = Accept_Post;
                if (UserAgent_Post == string.Empty) UserAgent_Post = "Mozilla/4.0 (compatible; MSIE 8.0; Windows NT 6.0)";
                req.UserAgent = UserAgent_Post;
                req.KeepAlive = KeepAlive_Post;
                req.Headers.Add("cookie", CookieContainer_Post);
                if (encoding) {
                    byte[] bytesData = Encoding.Default.GetBytes(Post_Data_String);

                    req.ContentLength = bytesData.Length;
                    using (Stream postStream = req.GetRequestStream()) {
                        postStream.Write(bytesData, 0, bytesData.Length);
                    }
                    HttpWebResponse res = ((HttpWebResponse)(req.GetResponse()));
                    using (StreamReader reader = new StreamReader(res.GetResponseStream(), Encoding.Default)) {
                        string respHTML = reader.ReadToEnd();
                        res.Close();
                        msg = respHTML;
                        return req.CookieContainer;
                    }
                }
                else {
                    byte[] bytesData = Encoding.GetEncoding("UTF-8").GetBytes(Post_Data_String);
                    req.ContentLength = bytesData.Length;
                    using (Stream postStream = req.GetRequestStream()) {
                        postStream.Write(bytesData, 0, bytesData.Length);
                    }
                    HttpWebResponse res = ((HttpWebResponse)(req.GetResponse()));
                    using (StreamReader reader = new StreamReader(res.GetResponseStream(), Encoding.GetEncoding("UTF-8"))) {
                        string respHTML = reader.ReadToEnd();
                        res.Close();
                        if (respHTML.Contains("超时")) {
                        }
                        msg = respHTML;
                        return req.CookieContainer;
                    }
                }
            }
            catch (Exception ee) {
                string str = ee.Message.ToString();
                if (retError) {
                    msg = "Error(错误)::" + str;
                    return null;
                }
                else {
                    msg = "";
                    return null;
                }
            }
        }
        /// <summary>
        /// 发送数据包,并且返回Cookies到Form1中的User_Cookies;
        /// </summary>
        /// <param name="URL_Post">POST请求的页面地址是什么？</param>
        /// <param name="Referer_Post">请求来路是什么？</param>
        /// <param name="Accept_Post">返回的数据格式是什么?缺省值:*/*</param>
        /// <param name="UserAgent_Post">请求的浏览器头部是什么？缺省值:Mozilla/4.0 (compatible; MSIE 8.0; Windows NT 6.0)</param>
        /// <param name="KeepAlive_Post">是否保持在线状态?</param>
        /// <param name="encoding">请求的格式是什么？true=GB2312,false=UTF-8</param>
        /// <param name="CookieContainer_Post">cookies是哪个？</param>
        /// <param name="Post_Data_String">POST发送的数据是什么？</param>
        /// <param name="retError">是否显示错误提示(Flase的话错误的时候返回空值)</param>
        /// <param name="msg">返回的网页源代码</param>
        /// <param name="cookie">返回的文本类cookie</param>
        /// <returns></returns>
        public static CookieContainer POST_SITE_DATA_Cookie(string URL_Post, string Referer_Post, string Accept_Post, string UserAgent_Post, bool KeepAlive_Post, bool encoding, CookieContainer CookieContainer_Post, string Post_Data_String, bool retError, out string msg, out string cookie) {
            try {
                Uri httpUrl = new Uri(URL_Post);
                HttpWebRequest req = null;
                if (URL_Post.StartsWith("https", StringComparison.OrdinalIgnoreCase)) {
                    ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(CheckValidationResult);
                    req = ((HttpWebRequest)(WebRequest.Create(httpUrl)));
                    req.ProtocolVersion = HttpVersion.Version10;
                }
                else {
                    req = ((HttpWebRequest)(WebRequest.Create(httpUrl)));
                }

                req.Method = "POST";
                req.ContentType = "application/x-www-form-urlencoded";
                req.Referer = Referer_Post;
                if (Accept_Post == string.Empty) Accept_Post = "*/*";
                req.Accept = Accept_Post;
                if (UserAgent_Post == string.Empty) UserAgent_Post = "Mozilla/4.0 (compatible; MSIE 8.0; Windows NT 6.0)";
                req.UserAgent = UserAgent_Post;
                req.KeepAlive = KeepAlive_Post;
                req.CookieContainer = CookieContainer_Post;
                if (encoding) {
                    byte[] bytesData = Encoding.Default.GetBytes(Post_Data_String);

                    req.ContentLength = bytesData.Length;
                    using (Stream postStream = req.GetRequestStream()) {
                        postStream.Write(bytesData, 0, bytesData.Length);
                    }
                    HttpWebResponse res = ((HttpWebResponse)(req.GetResponse()));
                    using (StreamReader reader = new StreamReader(res.GetResponseStream(), Encoding.Default)) {
                        string cookies = "";
                        string respHTML = reader.ReadToEnd();
                        foreach (var item in res.Cookies) {
                            cookies += item + ";";
                        }
                        cookie = cookies;
                        res.Close();
                        msg = respHTML;
                        return req.CookieContainer;
                    }
                }
                else {
                    byte[] bytesData = Encoding.GetEncoding("UTF-8").GetBytes(Post_Data_String);
                    req.ContentLength = bytesData.Length;
                    using (Stream postStream = req.GetRequestStream()) {
                        postStream.Write(bytesData, 0, bytesData.Length);
                    }
                    HttpWebResponse res = ((HttpWebResponse)(req.GetResponse()));
                    using (StreamReader reader = new StreamReader(res.GetResponseStream(), Encoding.GetEncoding("UTF-8"))) {
                        string cookies = "";
                        string respHTML = reader.ReadToEnd();
                        foreach (var item in res.Cookies) {
                            cookies += item + ";";
                        }
                        cookie = cookies;
                        res.Close();
                        msg = respHTML;
                        return req.CookieContainer;
                    }
                }
            }
            catch (Exception ee) {
                string str = ee.Message.ToString();
                if (retError) {
                    msg = "Error(错误)::" + str;
                    cookie = "";
                    return null;
                }
                else {
                    msg = "";
                    cookie = "";
                    return null;
                }
            }
        }

        #endregion

        #region Cookie转换为CookieContainer
        /// <summary>
        /// Cookie转换为CookieContainer
        /// </summary>
        /// <param name="cookie">需要转换的文本cookie</param>
        /// <param name="url">针对于哪个域名(例：.bilibili.com)</param>
        /// <returns></returns>
        public static CookieContainer CookieToCookieContainer(string cookie, string url) {
            CookieContainer c = new CookieContainer();
            string[] temp0 = cookie.Split(';');
            for (int i = 0; i < temp0.Length; i++) {
                try {
                    string[] temp1 = temp0[i].Split('=');
                    Cookie ck = new Cookie(temp1[0].Trim().ToString(), temp1[1].Trim().ToString());
                    ck.Domain = new Uri(url).Host;
                    c.Add(ck);
                }
                catch { }
            }
            return c;
        }
        #endregion

        #region 字符串MD5加密
        //字符串加密解密
        /// <summary>
        /// 获取MD5加密后的加密数据
        /// </summary>
        /// <param name="str">你想要把什么字符串进行MD5加密呢?</param>
        /// <param name="code">你想要使用16位MD5还是32位呢?true=16位,false=32位</param>
        /// <returns></returns>
        public static string Get_MD5(string str, bool code) {
            if (code) //16位MD5加密（取32位加密的9~25字符） 
            {
                return System.Web.Security.FormsAuthentication.HashPasswordForStoringInConfigFile(str, "MD5").ToLower().Substring(8, 16);
            }
            else//32位加密 
            {
                return System.Web.Security.FormsAuthentication.HashPasswordForStoringInConfigFile(str, "MD5").ToLower();
            }
        }
        #endregion


        /// <summary>
        /// 获取字符串中指定的数据
        /// </summary>
        /// <param name="shuju">你要从哪个字符串中获取数据呢?</param>
        /// <param name="tou">要从哪个字符串开始获取呢？</param>
        /// <param name="wei">要获取到哪个字符串结束呢？</param>
        /// <param name="pd">获取的结果要包含用来判断开始和结束的字符串么？true=包含,false=不包含</param>
        /// <param name="weiyi">获取的结果是否要增加或者减少呢？减少5个就输入-5，增加5个就输入5，不增加不减少输入0就OK</param>
        /// <returns></returns>
        public static string GetString(string shuju, string tou, string wei, bool pd, int weiyi = 0) {
            //C#获取字符串中需要的部分。
            //作者:小C
            //使用时请保留此版权说明,谢谢！
            //使用说明:第一个参数:需要从某个文件、变量获取字符串;
            //第二个参数:需要获取的字符串开头;
            //第三个参数:需要获取的字符串结尾;
            //第四个参数:True=获取的字符串中包含字符串开头和结尾,False=获取的字符串中不包含开头和结尾;
            //第五个参数:多获取指定数量的字符.
            //------------=-------------奇怪的分割线------------=-------------
            try {
                if (tou == string.Empty) tou = "!!@xiaoC@!!@xiaoC@!!";
                if (wei == string.Empty) wei = "@xiaoC@!!@xiaoC@!!@xiaoC@";
                shuju = "!!@xiaoC@!!@xiaoC@!!" + shuju + "@xiaoC@!!@xiaoC@!!@xiaoC@";
                string linshi = "";
                if (pd == true) {
                    string kaishi;
                    kaishi = shuju.Substring(shuju.IndexOf(tou), shuju.Length - shuju.IndexOf(tou));
                    linshi = kaishi.Substring(0, kaishi.IndexOf(wei) + weiyi + wei.Length);
                }
                else {
                    string kaishi;
                    kaishi = shuju.Substring(shuju.IndexOf(tou), shuju.Length - shuju.IndexOf(tou));
                    linshi = kaishi.Substring(0, kaishi.IndexOf(wei) + weiyi + wei.Length);
                    if (tou.Length >= wei.Length) {
                        linshi = (linshi.Replace(tou, "").Replace(wei, "")).Trim();
                    }
                    else {
                        linshi = (linshi.Replace(wei, "").Replace(tou, "")).Trim();
                    }
                }
                string str = linshi.Replace("@xiaoC@!!@xiaoC@!!@xiaoC@", "").Replace("!!@xiaoC@!!@xiaoC@!!", "");
                return str;
            }
            catch (Exception ee) {
                return "";
            }
        }
    }
}
