using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Mi.Fish.ApplicationDto
{
	/// <summary>
	/// 扫码支付参数
	/// </summary>
    public class SacnPayInput
    {
        /// <summary>
        /// 支付方式【暂时只支持 微信 支付宝 秒通 方特】
        /// </summary>
        public PayType PayType { get; set; }

        /// <summary>
        /// 支付码
        /// </summary>
        [Required]
		public string AuthCode { get; set; }

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
