using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.IO;

namespace CtFile {
    public class Login {
        string UserCachePath = "";
        public Login(string Username, string Password) {
            PublicConfig.Username = Username;
            if (!Directory.Exists(PublicConfig.CacheDir)) Directory.CreateDirectory(PublicConfig.CacheDir);

            UserCachePath = $"{PublicConfig.CacheDir}\\{NewC.Get_MD5(Username, false)}";
            if (File.Exists(UserCachePath)) {
                string Cookie = File.ReadAllText(UserCachePath, Encoding.UTF8);
                if (IsLogin(Cookie)) {
                    //已经登陆过了
                    PublicConfig.Cookie = Cookie;
                    Console.WriteLine("登录还未失效：" + Username);
                    return;
                }
            }

            if (PublicConfig.DaMaTu.Username == string.Empty || PublicConfig.DaMaTu.Password == string.Empty) {
                throw new Exception("验证码识别平台账号未初始化。\r\n请先设置验证码识别平台的账号密码：PublicConfig.DaMaTu.Username与PublicConfig.DaMaTu.Password\r\n验证码识别平台：dama2.com");
            }
            string[] VerCodeAndCookie = GetVerCode();
            LoginCtFile(Username, Password, VerCodeAndCookie[0], VerCodeAndCookie[1]);
        }

        private string[] GetVerCode() {
            string Cookie;
            byte[] imageData = NewC.Get_Vercode(PublicConfig.Login.VerCodeUrl, PublicConfig.Login.VerCodeUrl, false, new CookieContainer(), out Cookie);
            return new string[] { DaMa.Dama.Get(imageData), Cookie };
        }

        private void LoginCtFile(string Username, string Password, string VerCode, string Cookie) {
            Console.WriteLine($"开始登录网盘：{Username}, Code：{VerCode}");
            int NowTime = NewC.ConvertDateTimeInt(DateTime.Now);
            string Data;
            CookieContainer RetCookie = NewC.POST_SITE_DATA_Cookie(PublicConfig.Login.Url, PublicConfig.Login.Url, "", "", false, false, NewC.CookieToCookieContainer(Cookie, PublicConfig.Host),
                PublicConfig.Login.PostData.Replace("{username}", Username).Replace("{password}", Password).Replace("{vercode}", VerCode),
                false, out Data);
            if (Data == "ok") {
                PublicConfig.Cookie = RetCookie.GetCookieHeader(new Uri(PublicConfig.Host));
                File.WriteAllText(UserCachePath, PublicConfig.Cookie, Encoding.UTF8);
            }
            else {
                throw new Exception($"登陆失败,{Data}");
            }
        }

        private bool IsLogin(string Cookie) {
            string Data = NewC.GET_HTTP(PublicConfig.Login.CheckUrl, Cookie, false);
            if (Data.Contains("注册会员后再继续使用本功能") || Data.Contains("请先 登录")) {
                return false;
            }
            else return true;
        }
    }
}
