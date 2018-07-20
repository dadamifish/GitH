using System;

namespace Mi.Fish.ApplicationDto
{
    /// <summary>
    /// 
    /// </summary>
    public class PostTokenInput
    {
        /// <summary>
        /// 账号
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// 密码
        /// </summary>
        public string EncryPsw { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public class PostTokenOutput
    {
        /// <summary>
        /// token
        /// </summary>
        public string Token { get; set; }

        /// <summary>
        /// 有效时间(7天)
        /// </summary>
        public int TokenValidTime { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public class PostTokenDto : LeyouResult<PostTokenOutput>
    {

    }

    /// <summary>
    /// 乐游Token缓存
    /// </summary>
    public class LeyouTokenCache
    {
        /// <summary>
        /// 令牌
        /// </summary>
        public string Token { get; set; }

        /// <summary>
        /// 过期时间
        /// </summary>
        public DateTime ExpirationTime { get; set; }
    }
}
