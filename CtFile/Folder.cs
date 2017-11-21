using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text.RegularExpressions;
using System.Text;

namespace CtFile {
    /// <summary>
    /// 文件夹操作
    /// </summary>
    public static class Folder {
        /// <summary>
        /// 创建文件夹
        /// </summary>
        /// <param name="Name">文件夹名称</param>
        /// <param name="Desc">文件夹描述</param>
        /// <param name="PrintID">父文件夹ID（根目录为0）</param>
        /// <param name="OpenDC">是否开启直连（需要开通VIP）</param>
        /// <returns></returns>
        public static string Create(string Name, string Desc, int PrintID = 0, bool OpenDC = false) {
            string Data = NewC.POST_HTTP(PublicConfig.Folder.CreateUrl,
                PublicConfig.Folder.CreatePostData.Replace("{name}", Name).Replace("{desc}", Desc).Replace("{printid}", PrintID.ToString()).Replace("{opendc}", OpenDC ? "on" : "off"),
                PublicConfig.Cookie, false);
            if (Data.ToLower() != "ok" && !Data.Contains("存在")) throw new Exception($@"创建文件夹时发生错误：\r\n{Data}\r\n文件夹名称:{Name}\r\n描述:{Desc}\r\n父目录:{PrintID}\r\nCookie:{PublicConfig.Cookie}");
            return Data;
        }

        /// <summary>
        /// 通过文件名获取文件ID
        /// </summary>
        /// <param name="Name"></param>
        /// <returns></returns>
        public static int GetFolderID(string Name) {
            if (!Directory.Exists(PublicConfig.CacheDir)) Directory.CreateDirectory(PublicConfig.CacheDir);

            string CachePath = $"{PublicConfig.CacheDir}\\floder{NewC.Get_MD5(Name + PublicConfig.Username, false)}";
            if (File.Exists(CachePath)) {
                return Convert.ToInt32(File.ReadAllText(CachePath));
            }
            for (int page = 1; page <= 11; page++) {
                string Data = NewC.GET_HTTP(PublicConfig.Folder.ListUrl.Replace("{skip}", ((page - 1) * 100).ToString()), PublicConfig.Cookie, false);
                if (Data == "") return 0;
                Dictionary<string, object> Dic = JsonToDictionary.JSONToObject<Dictionary<string, object>>(Data);
                ArrayList AL = (ArrayList)Dic["aaData"];
                foreach (ArrayList item in AL) {
                    if (item[1].ToString().Contains(Name)) {
                        string ID = NewC.GetString(item[1].ToString(), "folder_id-", "\"", false, 0);
                        int OutID = 0;
                        if (!int.TryParse(ID, out OutID)) {
                            ID = NewC.GetString(item[0].ToString(), "value=\"", "\" ", false, 0);
                        }
                        int.TryParse(ID, out OutID);
                        if (OutID > 0) {
                            File.WriteAllText(CachePath, OutID.ToString());
                        }
                        return OutID;
                    }
                }
            }
            return 0;
        }


        /// <summary>
        /// 通过文件名在指定文件夹中寻找文件
        /// </summary>
        /// <param name="Name"></param>
        /// <returns></returns>
        public static string SearchFile(string FolderName, string Key, long FileLength, out string FileName) {
            FileName = "";
            double FileSizeMB = Convert.ToDouble(((double)FileLength / 1024 / 1024).ToString("0.00"));
            int ID = GetFolderID(FolderName);
            if (ID > 0) {
                string Data = NewC.GET_HTTP(PublicConfig.Folder.SearchUrl.Replace("{key}", Key).Replace("{folderid}", ID.ToString()), PublicConfig.Cookie, false);
                if (Data == "") return "";

                Dictionary<string, object> Dic = JsonToDictionary.JSONToObject<Dictionary<string, object>>(Data);
                ArrayList AL = (ArrayList)Dic["aaData"];
                foreach (ArrayList item in AL) {
                    double Size = FileSizeMB;
                    try { Size = Convert.ToDouble(item[2].ToString().Replace("MB", "").Trim()); }
                    catch { }

                    if (FileSizeMB > Size) {
                        if (FileSizeMB - Size > 1) continue;
                    }
                    else if (Size - FileSizeMB > 1) continue;

                    string Url = NewC.GetString(item[1].ToString(), "href=\"", "\">", false, 0);
                    MatchCollection Match = Regex.Matches(item[1].ToString(), @"<a.*?>(.*?)</a>", RegexOptions.IgnoreCase);
                    if (Match.Count > 0) {
                        FileName = Match[0].Groups[1].ToString();
                    }
                    return Url;
                }
            }
            return "";
        }
    }
}
