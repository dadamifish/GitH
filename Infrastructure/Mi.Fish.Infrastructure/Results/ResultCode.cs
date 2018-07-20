using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Mi.Fish.Infrastructure.Results
{
    /// <summary>
    /// 返回结果状态码
    /// </summary>
    public enum ResultCode
    {
        Undefined = 0,

        /// <summary>
        /// 成功
        /// </summary>
        [DisplayName("成功")]
        Ok = 1,

        /// <summary>
        /// 失败
        /// </summary>
        [DisplayName("失败")]
        Fail = 100,

        /// <summary>
        /// 参数验证失败
        /// </summary>
        [DisplayName("参数验证失败")]
        ParameterFailed = 101,

        /// <summary>
        /// 失败后刷新
        /// </summary>
        [DisplayName("失败后刷新")]
        FailAndRefresh = 102,

        /// <summary>
        /// 资源不存在
        /// </summary>
        [DisplayName("资源不存在")]
        NotFound = 200,

        /// <summary>
        /// 登录已失效
        /// </summary>
        [DisplayName("登录已失效")]
        Expired = 201,

        /// <summary>
        /// 授权失败
        /// </summary>
        [DisplayName("授权失败")]
        Unauthorized = 401,

        /// <summary>
        /// 未授权，需代理授权
        /// </summary>
        [DisplayName("需代理授权")]
        Proxyauthorized=407,
    }
}
