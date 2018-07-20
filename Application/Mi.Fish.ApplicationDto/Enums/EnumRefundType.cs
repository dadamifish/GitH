using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Mi.Fish.ApplicationDto
{
    /// <summary>
    /// 退款类型
    /// </summary>
    public enum EnumRefundType
    {
        /// <summary>
        /// 金额可以退给客户
        /// </summary>
        [Description("可退类型")]
        Allowed =1,
        /// <summary>
        /// 金额不可退给客户
        /// </summary>
        [Description("不可退类型")]
        NotAllowed=2,
        /// <summary>
        /// 不允许退款类型
        /// </summary>
        [Description("不退类型")]
        NoRefund=3
    }
}
