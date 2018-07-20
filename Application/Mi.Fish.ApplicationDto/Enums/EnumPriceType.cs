using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Mi.Fish.ApplicationDto
{

    /// <summary>
    /// 价格类型 
    /// </summary>
    public enum EnumPriceType
    {
        /// <summary>
        /// 实际价格
        /// </summary>
        [Description("实际价格")]
        Real = 0,

        /// <summary>
        /// 默认价格
        /// </summary>
        [Description("默认价格")]
        Default = 1

    }
}
