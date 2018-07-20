using System;
using System.Collections.Generic;
using System.Text;

namespace Mi.Fish.ApplicationDto
{
    public class AppTokenOutputDto:AppResult<TokenDto>
    {
    }

    public class TokenDto
    {
        /// <summary>
        /// Token
        /// </summary>
        public string Token { get; set;}
        /// <summary>
        /// 刷新token有效时间，分钟 
        /// </summary>
        public int TokenValidTime { get; set; }
    }
}
