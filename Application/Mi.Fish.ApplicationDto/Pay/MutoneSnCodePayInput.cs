using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Mi.Fish.ApplicationDto
{
    /// <summary>
    /// 秒通四位码支付输入参数
    /// </summary>
    public class MutoneSnCodePayInput
    {

        /// <summary>
        /// 四位码
        /// </summary>
        [Required]
        public string SnCode { get; set; }

        /// <summary>
        /// 餐饮支付订单号
        /// </summary>
        [Required]
        public string PayOrderNo { get; set; }

        /// <summary>
        /// 支付总金额
        /// </summary>
        [Required]
        public decimal PayMoney { get; set; }

        /// <summary>
        /// 商品名称[格式如： 冰红茶X1;绿茶X2]
        /// </summary>
        [Required]
        public string GoodsName { get; set; }



    }
}
