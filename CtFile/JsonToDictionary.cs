using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Web.Script.Serialization;
using System.Data;
namespace CtFile {
    public static class JsonToDictionary {
        /// <summary>
        /// Json格式转换成键值对，键值对中的Key需要区分大小写
        /// </summary>
        /// <param name="JsonData">需要转换的Json文本数据</param>
        /// <returns></returns>
        public static Dictionary<string, object> ToDictionary(string JsonData) {
            object Data = null;
            Dictionary<string, object> Dic = new Dictionary<string, object>();
            if (JsonData.StartsWith("[")) {
                //如果目标直接就为数组类型，则将会直接输出一个Key为List的List<Dictionary<string, object>>集合
                //使用示例List<Dictionary<string, object>> ListDic = (List<Dictionary<string, object>>)Dic["List"];
                List<Dictionary<string, object>> List = new List<Dictionary<string, object>>();
                MatchCollection ListMatch = Regex.Matches(JsonData, @"{[\s\S]+?}");//使用正则表达式匹配出JSON数组
                foreach (Match ListItem in ListMatch) {
                    List.Add(ToDictionary(ListItem.ToString()));//递归调用
                }
                Data = List;
                Dic.Add("List", Data);
            }
            else {
                MatchCollection Match = Regex.Matches(JsonData, @"""(.+?)"": {0,1}(\[[\s\S]+?\]|null|true|false|"".*?""|-{0,1}\d*)");//使用正则表达式匹配出JSON数据中的键与值
                foreach (Match item in Match) {
                    try {
                        if (item.Groups[2].ToString().StartsWith("[")) {
                            //如果目标是数组，将会输出一个Key为当前Json的List<Dictionary<string, object>>集合
                            //使用示例List<Dictionary<string, object>> ListDic = (List<Dictionary<string, object>>)Dic["Json中的Key"];
                            List<Dictionary<string, object>> List = new List<Dictionary<string, object>>();
                            MatchCollection ListMatch = Regex.Matches(item.Groups[2].ToString(), @"{[\s\S]+?}");//使用正则表达式匹配出JSON数组
                            foreach (Match ListItem in ListMatch) {
                                List.Add(ToDictionary(ListItem.ToString()));//递归调用
                            }
                            Data = List;
                        }
                        else if (item.Groups[2].ToString().ToLower() == "null") Data = null;//如果数据为null(字符串类型),直接转换成null
                        else if (item.Groups[2].ToString() == "\"\"") Data = "";
                        else {
                            if (item.Groups[2].ToString() == "true") Data = true;
                            else if (item.Groups[2].ToString() == "false") Data = false;
                            else {
                                Data = ToGB2312(item.Groups[2].ToString());
                                if (Data.ToString().StartsWith("\"")) {
                                    Data = Data.ToString().Substring(1, Data.ToString().Length - 1);
                                }
                                if (Data.ToString().EndsWith("\"")) {
                                    Data = Data.ToString().Substring(0, Data.ToString().Length - 1);
                                }
                            }
                        }
                        Dic.Add(item.Groups[1].ToString(), Data);
                    }
                    catch { }
                }
            }
            return Dic;
        }

        public static Dictionary<string, object> NewToDictionary(string JsonData) {
            object Data = null;
            Dictionary<string, object> Dic = new Dictionary<string, object>();
            if (JsonData.StartsWith("[")) {
                //如果目标直接就为数组类型，则将会直接输出一个Key为List的List<Dictionary<string, object>>集合
                //使用示例List<Dictionary<string, object>> ListDic = (List<Dictionary<string, object>>)Dic["List"];
                List<Dictionary<string, object>> List = new List<Dictionary<string, object>>();
                MatchCollection ListMatch = Regex.Matches(JsonData, @"{[\s\S]+?]}]");//使用正则表达式匹配出JSON数组
                foreach (Match ListItem in ListMatch) {
                    List.Add(NewToDictionary(ListItem.ToString()));//递归调用
                }
                Data = List;
                Dic.Add("List", Data);
            }
            else {
                MatchCollection Match = Regex.Matches(JsonData, @"""(.+?)"": {0,1}(\[[\s\S]+?\]|null|true|false|"".*?""|-{0,1}\d*)");//使用正则表达式匹配出JSON数据中的键与值
                foreach (Match item in Match) {
                    try {
                        if (item.Groups[2].ToString().StartsWith("[")) {
                            //如果目标是数组，将会输出一个Key为当前Json的List<Dictionary<string, object>>集合
                            //使用示例List<Dictionary<string, object>> ListDic = (List<Dictionary<string, object>>)Dic["Json中的Key"];
                            List<Dictionary<string, object>> List = new List<Dictionary<string, object>>();
                            MatchCollection ListMatch = Regex.Matches(item.Groups[2].ToString(), @"{[\s\S]+?}");//使用正则表达式匹配出JSON数组
                            foreach (Match ListItem in ListMatch) {
                                List.Add(NewToDictionary(ListItem.ToString()));//递归调用
                            }
                            Data = List;
                        }
                        else if (item.Groups[2].ToString().ToLower() == "null") Data = null;//如果数据为null(字符串类型),直接转换成null
                        else if (item.Groups[2].ToString() == "\"\"") Data = "";
                        else {
                            if (item.Groups[2].ToString() == "true") Data = true;
                            else if (item.Groups[2].ToString() == "false") Data = false;
                            else {
                                Data = ToGB2312(item.Groups[2].ToString());
                                if (Data.ToString().StartsWith("\"")) {
                                    Data = Data.ToString().Substring(1, Data.ToString().Length - 1);
                                }
                                if (Data.ToString().EndsWith("\"")) {
                                    Data = Data.ToString().Substring(0, Data.ToString().Length - 1);
                                }
                            }
                        }
                        Dic.Add(item.Groups[1].ToString(), Data);
                    }
                    catch { }
                }
            }
            return Dic;
        }

        public static Dictionary<string, object> ToDictionaryForJavaScriptSerializer(string jsonData) {
            //实例化JavaScriptSerializer类的新实例
            JavaScriptSerializer jss = new JavaScriptSerializer();
            try {
                //将指定的 JSON 字符串转换为 Dictionary<string, object> 类型的对象
                return jss.Deserialize<Dictionary<string, object>>(jsonData);
            }
            catch (Exception ex) {
                throw new Exception(ex.Message);
            }
        }

        public static T JSONToObject<T>(string jsonText) {
            JavaScriptSerializer jss = new JavaScriptSerializer();
            try {
                return jss.Deserialize<T>(jsonText);
            }
            catch (Exception ex) {
                throw new Exception("JSONHelper.JSONToObject(): " + ex.Message);
            }
        }

        /// <summary> 
        /// 数据表转键值对集合 www.2cto.com  
        /// 把DataTable转成 List集合, 存每一行 
        /// 集合中放的是键值对字典,存每一列 
        /// </summary> 
        /// <param name="dt">数据表</param> 
        /// <returns>哈希表数组</returns> 
        public static List<Dictionary<string, object>> DataTableToList(DataTable dt) {
            List<Dictionary<string, object>> list = new List<Dictionary<string, object>>();

            foreach (DataRow dr in dt.Rows) {
                Dictionary<string, object> dic = new Dictionary<string, object>();
                foreach (DataColumn dc in dt.Columns) {
                    dic.Add(dc.ColumnName, dr[dc.ColumnName]);
                }
                list.Add(dic);
            }
            return list;
        }


        /// <summary> 
        /// 数据表转JSON 
        /// </summary> 
        /// <param name="dataTable">数据表</param> 
        /// <returns>JSON字符串</returns> 
        public static string DataTableToJSON(DataTable dt) {
            try {
                return ObjectToJSON(DataTableToList(dt));
            }
            catch { return ""; }
        }

        /// <summary> 
        /// 对象转JSON 
        /// </summary> 
        /// <param name="obj">对象</param> 
        /// <returns>JSON格式的字符串</returns> 
        public static string ObjectToJSON(object obj) {
            JavaScriptSerializer jss = new JavaScriptSerializer();
            try {
                return jss.Serialize(obj);
            }
            catch (Exception ex) {

                throw new Exception("JSONHelper.ObjectToJSON(): " + ex.Message);
            }
        }
        #region 将Unicode编码转换为汉字字符串
        /// <summary>
        /// 将Unicode编码转换为汉字字符串
        /// </summary>
        /// <param name="str">Unicode编码字符串</param>
        /// <returns>汉字字符串</returns>
        static string ToGB2312(string str) {
            MatchCollection mc = Regex.Matches(str, @"\\u([\w]{2})([\w]{2})", RegexOptions.Compiled | RegexOptions.IgnoreCase);
            byte[] bts = new byte[2];
            foreach (Match m in mc) {
                bts[0] = (byte)int.Parse(m.Groups[2].Value, System.Globalization.NumberStyles.HexNumber);
                bts[1] = (byte)int.Parse(m.Groups[1].Value, System.Globalization.NumberStyles.HexNumber);
                str = str.Replace(m.ToString(), Encoding.Unicode.GetString(bts));
            }
            return str;
        }
        #endregion
    }
}