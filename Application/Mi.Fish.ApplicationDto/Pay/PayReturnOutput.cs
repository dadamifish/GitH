using System;
using System.Collections.Generic;
using System.Text;

namespace Mi.Fish.ApplicationDto
{
    /// <summary>
    /// 微信退款输出参数
    /// </summary>
    public class PayReturnOutput
    {

        /// <summary>
        /// 实际退款金额
        /// </summary>

        public decimal RefundFee { get; set; }

        /// <summary>
        /// 餐饮退款支付订单号
        /// </summary>

        public string PayReturnOrderNo { get; set; }

        /// <summary>
        /// 餐饮支付订单号
        /// </summary>

        public string PayOrderNo { get; set; }

        /// <summary>
        /// 第三方支付退款订单号
        /// </summary>

        public string ReturnTradeNo { get; set; }

    }
}
