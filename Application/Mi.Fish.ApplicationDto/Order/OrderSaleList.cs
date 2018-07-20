using System;
using System.Collections.Generic;
using System.Text;

namespace Mi.Fish.ApplicationDto
{
    /// <summary>
    /// 获取最近十笔销售单号
    /// </summary>
    public class OrderSaleListDto
    {
        /// <summary>
        /// 销售单号
        /// </summary>
        public string flow_no { get; set; }

    }

    public class OrderSaleListOutPut
    {
        /// <summary>
        /// 销售单号
        /// </summary>
        public string FlowNo { get; set; }

    }
}
