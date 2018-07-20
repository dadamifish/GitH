using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;

namespace Mi.Fish.ApplicationDto
{
    /// <summary>
    /// 特价类型
    /// </summary>
    public enum EnumSpecialType
    {

        /// <summary>
        /// 普通
        /// </summary>
        [Description("O")]
        Normal = 0,

        /// <summary>
        /// 单品打折类型
        /// </summary>
        [Description("K")]
        SingleDiscount = 1,

        /// <summary>
        /// 全部打折类型
        /// </summary>
        [Description("L")]
        AllDiscount = 2
    }
}
