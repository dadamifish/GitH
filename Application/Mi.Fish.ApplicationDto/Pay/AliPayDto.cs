using System;
using System.Collections.Generic;
using System.Text;

namespace Mi.Fish.ApplicationDto
{
    /// <summary>
    /// 支付宝支付参数
    /// </summary>
    public class AliPayDto
    {
        public Alipay_trade_pay_response alipay_trade_pay_response;

        public Alipay_trade_pay_response alipay_trade_query_response;

        public Alipay_trade_pay_response alipay_trade_refund_response;
        public string sign { get; set; }
    }

    public class Alipay_trade_pay_response
    {
        public string out_trade_no { get; set; }
        public string trade_no { get; set; }
        public string total_amount { get; set; }
        public string trade_status { get; set; }
        public string refund_fee { get; set; }
        public string code { get; set; }
        public string msg { get; set; }
        public string sub_code { get; set; }
        public string sub_msg { get; set; }
    }
}
