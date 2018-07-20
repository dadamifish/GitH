using Abp.UI;
using Newtonsoft.Json;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Mi.Fish.Common
{
    /// <summary>
    /// HttpClient扩展
    /// </summary>
    public static class HttpClientExtensions
    {
        #region private

        /// <summary>
        /// 获取键值参数
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        private static string GetParams(object param)
        {
            var type = param.GetType();
            if (type.ToString() == "System.String")
            {
                return param.ToString();
            }
            else
            {
                var sb = new StringBuilder();
                foreach (var pro in type.GetProperties())
                {
                    sb.Append("&");
                    sb.Append(pro.Name);
                    sb.Append("=");
                    var value = pro.GetValue(param, null)?.ToString();
                    sb.Append(value);
                }
                return sb.ToString().TrimStart('&');
            }
        }

        #endregion

        #region public

        /// <summary>
        /// 封装GET请求
        /// </summary>
        /// <param name="client"></param>
        /// <param name="requestUri">请求接口</param>
        /// <param name="param">参数</param>
        /// <returns></returns>
        public static Task<HttpResponseMessage> GetAsync(this HttpClient client, string requestUri, object param)
        {
            var mediaType = "application/x-www-form-urlencoded";
            if (param != null)
            {
                var parStr = GetParams(param);
                requestUri += "?" + parStr;
            }
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(mediaType));
            return client.GetAsync(requestUri);
        }

        /// <summary>
        /// 返回GET请求结果
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="requestUri">请求接口</param>
        /// <param name="param">参数</param>
        /// <param name="httpRequestHeaders">HttpRequestHeaders</param>
        /// <returns></returns>
        public static async Task<T> GetAsync<T>(string requestUri, object param, Dictionary<string, string> httpRequestHeaders = null)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    if (httpRequestHeaders != null && httpRequestHeaders.Count > 0)
                    {
                        foreach (var key in httpRequestHeaders.Keys)
                        {
                            client.DefaultRequestHeaders.Add(key, httpRequestHeaders[key]);
                        }
                    }
                    var response = await client.GetAsync(requestUri, param);
                    response.EnsureSuccessStatusCode();
                    return await response.ReadExtensionAsync<T>();
                }
            }
            catch
            {
                throw new UserFriendlyException($"连接{requestUri}接口服务失败");
            }
        }

        /// <summary>
        /// 封装POST请求
        /// </summary>
        /// <param name="client"></param>
        /// <param name="requestUri">请求接口</param>
        /// <param name="param">参数</param>
        /// <param name="mediaTypeIsJson">媒体类型,true为application/json，false为application/x-www-form-urlencoded</param>
        /// <returns></returns>
        public static Task<HttpResponseMessage> PostAsync(this HttpClient client, string requestUri, object param, bool mediaTypeIsJson = true)
        {
            var mediaType = "application/json";
            var data = string.Empty;
            if (mediaTypeIsJson)
            {
                data = JsonConvert.SerializeObject(param);
            }
            else
            {
                mediaType = "application/x-www-form-urlencoded";
                data = GetParams(param);
            }
            var content = new StringContent(data, Encoding.UTF8, mediaType);
            content.Headers.ContentType = new MediaTypeHeaderValue(mediaType);
            return client.PostAsync(requestUri, content);
        }

        /// <summary>
        /// 返回POST请求结果
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="requestUri">请求接口</param>
        /// <param name="param">参数</param>
        /// <param name="mediaTypeIsJson">媒体类型,true为application/json，false为application/x-www-form-urlencoded</param>
        /// <param name="httpRequestHeaders">HttpRequestHeaders</param>
        /// <returns></returns>
        public static async Task<T> PostAsync<T>(string requestUri, object param, bool mediaTypeIsJson = true, Dictionary<string, string> httpRequestHeaders = null)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    if (httpRequestHeaders != null && httpRequestHeaders.Count > 0)
                    {
                        foreach (var key in httpRequestHeaders.Keys)
                        {
                            client.DefaultRequestHeaders.Add(key, httpRequestHeaders[key]);
                        }
                    }
                    var response = await client.PostAsync(requestUri, param, mediaTypeIsJson);
                    response.EnsureSuccessStatusCode();
                    return await response.ReadExtensionAsync<T>();
                }
            }
            catch
            {
                throw new UserFriendlyException($"连接{requestUri}接口服务失败");
            }
        }

        /// <summary>
        /// 获取返回内容
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="content"></param>
        /// <returns></returns>
        public static async Task<T> ReadExtensionAsync<T>(this HttpResponseMessage response)
        {
            var result = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<T>(result);
        }

        #endregion
    }
}
