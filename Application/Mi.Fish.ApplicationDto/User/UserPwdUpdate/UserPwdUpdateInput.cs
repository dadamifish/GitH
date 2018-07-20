using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Mi.Fish.ApplicationDto
{
    /// <summary>
    /// 
    /// </summary>
    public class UserPwdUpdateInput
    {
        /// <summary>
        /// 原密码
        /// </summary>
        [Required(ErrorMessage = "原密码不能为空")]
        [DataType(DataType.Password)]
        public string OldPwd { get; set; }

        /// <summary>
        /// 新密码
        /// </summary>
        [Required(ErrorMessage = "新密码不能为空")]
        [DataType(DataType.Password)]
        public string NewPwd { get; set; }

        /// <summary>
        /// 确认密码
        /// </summary>
        [Required(ErrorMessage = "确认密码不能为空")]
        [DataType(DataType.Password)]
        [Compare("NewPwd",ErrorMessage = "新密码与确认密码不一致")]
        public string ConfirmPwd { get; set; }
    }
}
