using System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;

namespace Mi.Fish.Api.JwtBearer
{
    /// <summary>
    /// 必要参数类
    /// </summary>
    public class AuthRequirement : IAuthorizationRequirement
    {
        /// <summary>
        /// 无权限action
        /// </summary>
        public string DeniedAction { get; set; }
        /// <summary>
        /// 认证授权类型
        /// </summary>
        public string ClaimType { internal get; set; }
        /// <summary>
        /// 发行人
        /// </summary>
        public string Issuer { get; set; }
        /// <summary>
        /// 订阅人
        /// </summary>
        public string Audience { get; set; }
        /// <summary>
        /// 签名
        /// </summary>
        public SigningCredentials SigningCredentials { get; set; }

        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="deniedAction"></param>
        /// <param name="claimType"></param>
        /// <param name="audience"></param>
        /// <param name="issuer"></param>
        /// <param name="signingCredentials"></param>
        public AuthRequirement(string deniedAction, string claimType, string issuer, string audience, SigningCredentials signingCredentials)
        {
            ClaimType = claimType;
            DeniedAction = deniedAction;
            Issuer = issuer;
            Audience = audience;
            SigningCredentials = signingCredentials;
        }
    }
}
