using System;
using System.Collections.Generic;
using System.Text;

namespace CtFile {
    public static class PublicConfig {
        public static string Host = "https://www.ctfile.com/";
        private static Random Ran = new Random();
        public static class DaMaTu {
            public static string Username = "";
            public static string Password = "";
        }
        /// <summary>
        /// 返回一个介于0~1之间的随机小数
        /// </summary>
        public static string RanNumber {
            get {
                return Ran.NextDouble().ToString();
            }
        }
        /// <summary>
        /// 用户名（邮箱）
        /// </summary>
        public static string Username { get; set; }

        /// <summary>
        /// 密码
        /// </summary>
        public static string Password { get; set; }

        /// <summary>
        /// Cookie
        /// </summary>
        public static string Cookie { get; set; }

        /// <summary>
        /// 登陆
        /// </summary>
        public static class Login {
            /// <summary>
            /// 登陆Url
            /// </summary>
            public static string Url = Host + "index.php";

            /// <summary>
            /// 验证登陆URL
            /// </summary>
            public static string CheckUrl = "https://home.ctfile.com/iajax.php?item=profile&action=account_info";

            /// <summary>
            /// 登陆POST数据
            /// </summary>
            public static string PostData = "item=account&action=login&task=login&ref=https%3A%2F%2Fwww.ctfile.com%2F&username={username}&password={password}&randcodeV2={vercode}";

            private static string verCodeUrl = Host + "randcodeV2_login.php?r=";
            /// <summary>
            /// 验证码
            /// </summary>
            public static string VerCodeUrl {
                get {
                    return verCodeUrl + RanNumber;
                }
            }
        }

        public static class Folder {
            public static string CreateUrl = "https://home.ctfile.com/iajax.php?item=folders";
            public static string CreatePostData = "action=folder_create&task=folder_create&folder_id={printid}&folder_name={name}&folder_description={desc}&openlink={opendc}";
            public static string ListUrl = "https://home.ctfile.com/iajax.php?item=file_act&action=file_list&folder_id=0&task=index&iDisplayLength=100&iDisplayStart={skip}";
            public static string SearchUrl = "https://home.ctfile.com/iajax.php?item=file_act&action=file_list&folder_id={folderid}&task=index&iDisplayLength=100&iDisplayStart=0&sSearch={key}";
        }

        public static class File {
            public static string GetUpUrl = "https://home.ctfile.com/iajax.php?item=files&action=index&folder_id={folderid}";
            public static string GetUserDownUrl = "https://home.ctfile.com/iajax.php?item=file_act&action=file_list&folder_id={folderid}&task=index&iDisplayLength=100&iDisplayStart={skip}";
        }

        /// <summary>
        /// 缓存目录
        /// </summary>
        public static string CacheDir = Environment.CurrentDirectory + "\\Cache";
    }
}
