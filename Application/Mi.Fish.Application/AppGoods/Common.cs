using Mi.Fish.ApplicationDto;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Mi.Fish.Application
{
    public class Common
    {
        /// <summary>
        /// 获取 token
        /// </summary>
        /// <returns></returns>
        public static async Task<string> GetAppTokenAsync(string url,string userName,string encryPsw)
        {
            url = url + "api/DinnerSystemSynchronization/Login";
            encryPsw = Md5Hash(encryPsw).ToUpper();
            string data = string.Concat("{\"username\":\"", userName, "\",\"encryPsw\":\"", encryPsw, "\"}");
            using (HttpClient http = new HttpClient())
            {
                StringContent content = new StringContent(data, Encoding.UTF8, "application/json");
                var response = await http.PostAsync(url, content);
                string result = await response.Content.ReadAsStringAsync();
                AppTokenOutputDto token = JsonConvert.DeserializeObject<AppTokenOutputDto>(result);
                return token.Data.Token;
            }
        }
        /// <summary>
        /// 32位MD5加密
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        private static string Md5Hash(string input)
        {
            MD5CryptoServiceProvider md5Hasher = new MD5CryptoServiceProvider();
            byte[] data = md5Hasher.ComputeHash(Encoding.Default.GetBytes(input));
            StringBuilder sBuilder = new StringBuilder();
            for (int i = 0; i < data.Length; i++)
            {
                sBuilder.Append(data[i].ToString("x2"));
            }
            return sBuilder.ToString();
        }
    }
}
