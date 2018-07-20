using System;
using System.Collections.Generic;
using System.Text;

namespace Mi.Fish.ApplicationDto
{
    /// <summary>
    /// 微信支付参数
    /// </summary>
    public class WxMicroPayDto
    {
        public string result{ get; set; }
        public string appid{ get; set; }
        public string mch_id{ get; set; }
        public string device_info{ get; set; }
        public string nonce_str{ get; set; }
        public string sign{ get; set; }
        public string result_code{ get; set; }
        public string err_code{ get; set; }
        public string err_code_des{ get; set; }
        public string openid{ get; set; }
        public string is_subscribe{ get; set; }
        public string trade_type{ get; set; }
        public string bank_type{ get; set; }
        public string fee_type{ get; set; }
        public string total_fee{ get; set; }
        public string cash_fee_type{ get; set; }
        public string cash_fee{ get; set; }
        public string coupon_fee{ get; set; }
        public string transaction_id{ get; set; }
        public string out_trade_no{ get; set; }
        public string attach{ get; set; }
        public string time_end{ get; set; }

        public string trade_state{ get; set; }

        /// <summary>
        /// 商户退款单号
        /// </summary>
        public string out_refund_no{ get; set; }
        /// <summary>
        /// 微信退款单号
        /// </summary>
        public string refund_id{ get; set; }
        /// <summary>
        /// 现金退款金额
        /// </summary>
        public string cash_refund_fee{ get; set; }

        /// <summary>
        /// 退款金额
        /// </summary>
        public string refund_fee{ get; set; }

    }
}
