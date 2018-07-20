using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Mi.Fish.ApplicationDto
{
    /// <summary>
    /// 单据模式
    /// </summary>
    public enum SaleMode
    {
        /// <summary>
        /// 销售
        /// </summary>
        [DisplayName("销售")]
        A=0,
        /// <summary>
        /// 退单
        /// </summary>
        [DisplayName("退单")]
        B=2
    }
}
