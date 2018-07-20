using System;
using System.Collections.Generic;
using System.Text;

namespace Mi.Fish.ApplicationDto
{
    /// <summary>
    /// 获取秒通四位码接口返回类
    /// </summary>
    public class MutoneSnCodeOutput
    {
        /// <summary>
        /// 四位码
        /// </summary>
        public string SnCode { get; set; }

        /// <summary>
        /// 餐饮支付订单号
        /// </summary>

        public string PayOrderNo { get; set; }

    }
}
