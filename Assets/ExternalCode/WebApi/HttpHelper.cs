using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;

namespace Assets.ExternalCode.WebApi
{
    public static class HttpHelper
    {
        public static CookieContainer cookie = new CookieContainer();
        public static string Post(string Url, Dictionary<string, string> args)
        {
            string ret = string.Empty;
            try
            {
                string Param = string.Empty;
                foreach (var arg in args)
                {
                    Param += String.Format("{0}={1}&", arg.Key, arg.Value);
                }
                Param = Param.TrimEnd('&');
                byte[] byteArray = Encoding.Default.GetBytes(Param);
                HttpWebRequest webReq = (HttpWebRequest)WebRequest.Create(new Uri(Url));
                webReq.Method = "POST";
                webReq.ContentType = "application/x-www-form-urlencoded";
                webReq.CookieContainer = cookie;
                webReq.ContentLength = byteArray.Length;
                Stream newStream = webReq.GetRequestStream();
                newStream.Write(byteArray, 0, byteArray.Length);
                newStream.Close();
                HttpWebResponse response = (HttpWebResponse)webReq.GetResponse();
                response.Cookies = webReq.CookieContainer.GetCookies(new Uri(Url));

                StreamReader sr = new StreamReader(response.GetResponseStream(), Encoding.Default);
                ret = sr.ReadToEnd();
                sr.Close();
                response.Close();
                newStream.Close();
            }
            catch
            {
            }
            return ret;
        }
        public static string Get(string Url)
        {
            string ret = string.Empty;
            try
            {
                HttpWebRequest webReq = (HttpWebRequest)WebRequest.Create(new Uri(Url));
                webReq.Method = "GET";
                webReq.CookieContainer = cookie;
                HttpWebResponse response = (HttpWebResponse)webReq.GetResponse();
                StreamReader sr = new StreamReader(response.GetResponseStream(), Encoding.Default);
                ret = sr.ReadToEnd();
                sr.Close();
                response.Close();
            }
            catch
            {
            }
            return ret;
        }
        public static byte[] Blob(string Url, Dictionary<string, string> args)
        {
            var ret = default(byte[]);
            try
            {
                string Param = string.Empty;
                foreach (var arg in args)
                {
                    Param += String.Format("{0}={1}&", arg.Key, arg.Value);
                }
                Param = Param.TrimEnd('&');
                byte[] byteArray = Encoding.Default.GetBytes(Param);
                HttpWebRequest webReq = (HttpWebRequest)WebRequest.Create(new Uri(Url));
                webReq.Method = "POST";
                webReq.ContentType = "application/x-www-form-urlencoded";
                webReq.CookieContainer = cookie;
                webReq.ContentLength = byteArray.Length;
                Stream newStream = webReq.GetRequestStream();
                newStream.Write(byteArray, 0, byteArray.Length);
                newStream.Close();
                HttpWebResponse response = (HttpWebResponse)webReq.GetResponse();
                response.Cookies = webReq.CookieContainer.GetCookies(new Uri(Url));

                var stream = response.GetResponseStream();
                var memoryStream = new MemoryStream();
                stream.Read(ret, 0, Convert.ToInt32(stream.Length));

                stream.Close();
                response.Close();
                newStream.Close();
            }
            catch
            {
            }
            return ret;
        }
    }
}
