using System;
using System.Collections.Generic;
using System.Text;

namespace Mi.Fish.ApplicationDto
{
    /// <summary>
    /// app 接口返回信息结构
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class AppResult<T>
    {
        /// <summary>
        /// 标示代码 0:正确
        /// </summary>
        public string Result { get; set; }
        /// <summary>
        /// 返回消息描述
        /// </summary>
        public string Message { get; set; }
        /// <summary>
        /// 返回数据
        /// </summary>
        public T Data { get; set; }
        /// <summary>
        /// 服务器时间
        /// </summary>
        public DateTime Time { get; set; }
    }
}
