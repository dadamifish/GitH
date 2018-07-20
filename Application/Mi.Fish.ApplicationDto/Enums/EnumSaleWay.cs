using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;

namespace Mi.Fish.ApplicationDto
{
    /// <summary>
    /// 销售方式
    /// </summary>
    public enum EnumSaleWay
    {

        /// <summary>
        /// 销售
        /// </summary>
        [DisplayName("销售")]
        Sale = 0,

        /// <summary>
        /// 退货
        /// </summary>
        [DisplayName("退货")]
        Refund = 2,



    }
}
