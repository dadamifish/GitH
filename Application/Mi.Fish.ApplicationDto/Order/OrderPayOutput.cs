using System;
using System.Collections.Generic;
using System.Text;

namespace Mi.Fish.ApplicationDto
{
    /// <summary>
    /// 付款输出参数
    /// </summary>
    public class OrderPayOutput
    {
        /// <summary>
        /// 是否完成支付
        /// </summary>
        public bool IsComplete { get; set; }

        /// <summary>
        /// 付款信息
        /// </summary>
        public OrderPayInfo OrderPayInfo { get; set; }
    }
}
