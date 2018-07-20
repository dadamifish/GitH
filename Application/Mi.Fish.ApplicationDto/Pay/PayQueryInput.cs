using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Mi.Fish.ApplicationDto
{
    /// <summary>
    /// 支付查询输入参数
    /// </summary>
    public class PayQueryInput
    {
        /// <summary>
        /// 商户或第三方交易单号(秒通、乐游传第三方订单号  微信、支付宝传餐饮订单号)
        /// </summary>
        [Required]
        public string TradeNo { get; set; }

        /// <summary>
        /// 支付总金额
        /// </summary>
        [Required]
        [Range(0.01,double.MaxValue)]
        public decimal PayMoney { get; set; }



    }
}
