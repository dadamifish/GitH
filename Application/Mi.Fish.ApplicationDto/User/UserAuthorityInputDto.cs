using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Mi.Fish.ApplicationDto
{
    public class UserAuthorityInputDto
    {
        /// <summary>
        /// 用户账号
        /// </summary>
        [Required]
        public string UserId { get; set; }
        /// <summary>
        /// 用户密码
        /// </summary>
        [Required]
        public string PWD { get; set; }
        /// <summary>
        /// 授权类型
        /// </summary>
        [Required]
        public EnumAuthorityType AuthorityType { get; set; }
    }

    public class Judge
    {
        public bool result { get; set; }
    }
}
