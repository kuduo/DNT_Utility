using System;
using System.Web;
using System.Web.Security;

namespace Teein.Weibo.Common {
    /// <summary>
    /// Request操作类
    /// </summary>
    public class NTRequest {
        /// <summary>
        /// 判断当前页面是否接收到了Post请求
        /// </summary>
        /// <returns>是否接收到了Post请求</returns>
        public static bool IsPost() {
            return HttpContext.Current.Request.HttpMethod.Equals("POST");
        }

        /// <summary>
        /// 判断当前页面是否接收到了Get请求
        /// </summary>
        /// <returns>是否接收到了Get请求</returns>
        public static bool IsGet() {
            return HttpContext.Current.Request.HttpMethod.Equals("GET");
        }

        /// <summary>
        /// 返回指定的服务器变量信息
        /// </summary>
        /// <param name="key">服务器变量名</param>
        /// <returns>服务器变量信息</returns>
        public static string GetServerString(string key) {
            return HttpContext.Current.Request.ServerVariables[key] ?? string.Empty;
        }

        /// <summary>
        /// 返回上一个页面的地址
        /// </summary>
        /// <returns>上一个页面的地址</returns>
        public static string GetUrlReferrer() {
            Uri referrer = HttpContext.Current.Request.UrlReferrer;
            return referrer == null ? string.Empty : referrer.ToString();
        }

        /// <summary>
        /// 获得指定Url参数的值
        /// </summary>
        /// <param name="key">Url参数</param>
        /// <returns>Url参数的值</returns>
        public static string GetQueryString(string key) {
            return HttpContext.Current.Request.QueryString[key] ?? string.Empty;
        }

        /// <summary>
        /// 获得指定表单参数的值
        /// </summary>
        /// <param name="key">表单参数</param>
        /// <returns>表单参数的值</returns>
        public static string GetFormString(string key) {
            return HttpContext.Current.Request.Form[key] ?? string.Empty;
        }

        /// <summary>
        /// 获得Url或表单参数的值, 先判断Url参数是否为空字符串, 如为True则返回表单参数的值
        /// </summary>
        /// <param name="key">参数</param>
        /// <returns>Url或表单参数的值</returns>
        public static string GetString(string key) {
            return (HttpContext.Current.Request.QueryString[key] ?? HttpContext.Current.Request.Form[key]) ?? string.Empty;
        }

        /// <summary>
        /// 获得指定Url参数的int类型值
        /// </summary>
        /// <param name="key">Url参数</param>
        /// <param name="defValue">缺省值</param>
        /// <returns>Url参数的int类型值</returns>
        public static int GetQueryInt(string key, int defValue) {
            int result = defValue;
            if (int.TryParse(HttpContext.Current.Request.QueryString[key], out result)) return result;
            return defValue;
        }


        /// <summary>
        /// 获得指定表单参数的int类型值
        /// </summary>
        /// <param name="key">表单参数</param>
        /// <param name="defValue">缺省值</param>
        /// <returns>表单参数的int类型值</returns>
        public static int GetFormInt(string key, int defValue) {
            int result = defValue;
            if (int.TryParse(HttpContext.Current.Request.Form[key], out result)) return result;
            return defValue;
        }
        /// <summary>
        /// 获得指定Url或表单参数的int类型值, 先判断Url参数是否为缺省值, 如为True则返回表单参数的值
        /// </summary>
        /// <param name="strName">Url或表单参数</param>
        /// <param name="defValue">缺省值</param>
        /// <returns>Url或表单参数的int类型值</returns>
        public static int GetInt(string key, int defValue) {
            if (GetQueryInt(key, defValue) == defValue)
                return GetFormInt(key, defValue);
            else
                return GetQueryInt(key, defValue);
        }

        /// <summary>
        /// 获得指定Url参数的float类型值
        /// </summary>
        /// <param name="key">Url参数</param>
        /// <param name="defValue">缺省值</param>
        /// <returns>Url参数的int类型值</returns>
        public static float GetQueryFloat(string key, float defValue) {
            float result = defValue;
            if (float.TryParse(HttpContext.Current.Request.QueryString[key], out result)) return result;
            return defValue;
        }


        /// <summary>
        /// 获得指定表单参数的float类型值
        /// </summary>
        /// <param name="key">表单参数</param>
        /// <param name="defValue">缺省值</param>
        /// <returns>表单参数的float类型值</returns>
        public static float GetFormFloat(string key, float defValue) {
            float result = defValue;
            if (float.TryParse(HttpContext.Current.Request.Form[key], out result)) return result;
            return defValue;
        }

        /// <summary>
        /// 获得指定Url或表单参数的float类型值, 先判断Url参数是否为缺省值, 如为True则返回表单参数的值
        /// </summary>
        /// <param name="key">Url或表单参数</param>
        /// <param name="defValue">缺省值</param>
        /// <returns>Url或表单参数的int类型值</returns>
        public static float GetFloat(string key, float defValue) {
            if (GetQueryFloat(key, defValue) == defValue)
                return GetFormFloat(key, defValue);
            else
                return GetQueryFloat(key, defValue);
        }

        /// <summary>
        /// 获得当前页面客户端的IP
        /// </summary>
        /// <returns>当前页面客户端的IP</returns>
        public static string GetIP() {
            string result = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
            if (string.IsNullOrEmpty(result))
                result = HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];

            if (string.IsNullOrEmpty(result))
                result = HttpContext.Current.Request.UserHostAddress;

            if (string.IsNullOrEmpty(result))
                return "127.0.0.1";

            return result;
        }

        /// <summary>
        /// 写cookie值
        /// </summary>
        /// <param name="name">名称</param>
        /// <param name="value">值</param>
        public static void WriteCookie(string name, string value) {

            HttpCookie cookie = HttpContext.Current.Request.Cookies[name];
            if (cookie == null) {
                cookie = new HttpCookie(name);
            }
            cookie.Value = value;
            HttpContext.Current.Response.AppendCookie(cookie);
        }

        public static void WriteEncryptCookie(string name, string value) {
            FormsAuthenticationTicket ticket = new FormsAuthenticationTicket(1011, name, DateTime.Now, DateTime.Now.AddMinutes(60), true, value, FormsAuthentication.FormsCookiePath);
            string encryptTicket = FormsAuthentication.Encrypt(ticket);
            HttpCookie cookie = new HttpCookie(name, encryptTicket);
            HttpContext.Current.Response.AppendCookie(cookie);
        }

        /// <summary>
        /// 写cookie值
        /// </summary>
        /// <param name="name">名称</param>
        /// <param name="value">值</param>
        public static void WriteCookie(string name, string key, string value) {
            HttpCookie cookie = HttpContext.Current.Request.Cookies[name];
            if (cookie == null) {
                cookie = new HttpCookie(name);
            }
            cookie[key] = value;
            HttpContext.Current.Response.AppendCookie(cookie);
        }


        /// <summary>
        /// 写cookie值
        /// </summary>
        /// <param name="name">名称</param>
        /// <param name="value">值</param>
        /// <param name="value">过期时间(分钟)</param>
        public static void WriteCookie(string name, string value, int expires) {
            HttpCookie cookie = HttpContext.Current.Request.Cookies[name];
            if (cookie == null) {
                cookie = new HttpCookie(name);
            }
            cookie.Value = value;
            cookie.Expires = DateTime.Now.AddMinutes(expires);
            HttpContext.Current.Response.AppendCookie(cookie);
        }

        /// <summary>
        /// 读cookie值
        /// </summary>
        /// <param name="name">名称</param>
        /// <returns>cookie值</returns>
        public static string ReadCookie(string name) {
            if (HttpContext.Current.Request.Cookies != null && HttpContext.Current.Request.Cookies[name] != null)
                return HttpContext.Current.Request.Cookies[name].Value.ToString();

            return string.Empty;
        }


        public static string ReadEncryptCookie(string name) {
            HttpCookie cookie = HttpContext.Current.Request.Cookies[name];
            if (cookie != null) {
                FormsAuthenticationTicket ticket = FormsAuthentication.Decrypt(cookie.Value);
                return ticket.UserData;
            }
            return string.Empty;
        }

        /// <summary>
        /// 读cookie值
        /// </summary>
        /// <param name="name">名称</param>
        /// <returns>cookie值</returns>
        public static string ReadCookie(string name, string key) {
            if (HttpContext.Current.Request.Cookies != null && HttpContext.Current.Request.Cookies[name] != null && HttpContext.Current.Request.Cookies[name][key] != null)
                return HttpContext.Current.Request.Cookies[name][key].ToString();

            return string.Empty;
        }
    }
}
