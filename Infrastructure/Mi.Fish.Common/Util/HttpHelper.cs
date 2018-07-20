using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Net;
using System.IO;

namespace Mi.Fish.Common
{
    public class HttpHelper
    {

        /// <summary>
        /// Http POST
        /// </summary>
        /// <param name="url"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public static string GetPostString(string url, string data)
        {
            Encoding encode = System.Text.Encoding.GetEncoding("utf-8");
            byte[] arrB = encode.GetBytes(data);
            HttpWebRequest myReq = (HttpWebRequest)WebRequest.Create(url);
            myReq.Method = "POST";
            myReq.ContentType = "application/x-www-form-urlencoded";
            myReq.ContentLength = arrB.Length;
            Stream outStream = myReq.GetRequestStream();
            outStream.Write(arrB, 0, arrB.Length);
            outStream.Close();


            //接收HTTP做出的响应
            WebResponse myResp = myReq.GetResponse();
            Stream ReceiveStream = myResp.GetResponseStream();
            StreamReader readStream = new StreamReader(ReceiveStream, encode);
            Char[] read = new Char[256];
            int count = readStream.Read(read, 0, 256);
            string str = null;
            while (count > 0)
            {
                str += new String(read, 0, count);
                count = readStream.Read(read, 0, 256);
            }
            readStream.Close();
            myResp.Close();

            return str;
        }

        /// <summary>
        /// 调用方特旅游接口 返回json 数据
        /// </summary>
        /// <param name="url">接口地址+参数</param>
        /// <returns></returns>
        public static string getAppApi(string url)
        {
            string html = "";
            try
            {
                HttpWebRequest req = WebRequest.Create(url) as HttpWebRequest;
                req.ContentType = "multipart/form-data";
                req.Accept = "*/*";
                req.UserAgent = "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.1; SV1; .NET CLR 2.0.50727)";
                req.Timeout = 40000;//40秒连接不成功就中断 
                req.Method = "GET";

                HttpWebResponse response = req.GetResponse() as HttpWebResponse;
                using (StreamReader sr = new StreamReader(response.GetResponseStream(), Encoding.UTF8))
                {
                    html = sr.ReadToEnd();
                }
            }
            catch (WebException ex)
            {
                html = "";
            }
            return html;
        }

    }
}
