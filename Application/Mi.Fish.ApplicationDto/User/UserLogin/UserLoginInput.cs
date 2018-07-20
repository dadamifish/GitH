using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Mi.Fish.ApplicationDto
{
    /// <summary>
    /// 
    /// </summary>
    public class UserLoginInput
    {
        /// <summary>
        /// 门店号
        /// </summary>
        [Required(ErrorMessage = "门店号不能为空")]
        public string StorageNo { get; set; }

        /// <summary>
        /// 机号
        /// </summary>
        [Required(ErrorMessage = "机号不能为空")]
        public string FishNo { get; set; }

        /// <summary>
        /// 工号
        ///</summary>
        [Required(ErrorMessage = "工号不能为空")]
        public string CashierNo { get; set; }

        /// <summary>
        /// 密码
        /// </summary>
        [Required(ErrorMessage = "密码不能为空")]
        [DataType(DataType.Password)]
        public string CashierPwd { get; set; }

        /// <summary>
        /// 当班金额
        /// </summary>
        [Required(ErrorMessage = "当班金额不能为空")]
        [Range(0, double.MaxValue,ErrorMessage ="当班金额不小于0")]
        public decimal StartAmount { get; set; }

        /// <summary>
        /// 班次
        /// </summary>
        [Required(ErrorMessage = "班次不能为空")]
        [Range(1, 10,ErrorMessage ="班次范围1~10")]
        public int Shift { get; set; }


    }
}
