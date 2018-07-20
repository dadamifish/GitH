using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Mi.Fish.ApplicationDto
{

    /// <summary>
    /// 第三方支付退款输入参数
    /// </summary>
    public class PayReturnInput
    {
        /// <summary>
        /// 全部退标志 True 全部退 False 允许部分退
        /// </summary>
        public bool AllReturn { get; set; }

        /// <summary>
        /// 退款金额
        /// </summary>
        [Required]
        public decimal ReturnAmount { get; set; }

        /// <summary>
        /// 退款支付方式
        /// </summary>
        [Required]
        public PayType PayType { get; set; }


        /// <summary>
        /// 原单小票单号
        /// </summary>
        [Required]
        public string OriginalOrderNo { get; set; }


    }
}
