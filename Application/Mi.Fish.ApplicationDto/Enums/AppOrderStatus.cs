using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Mi.Fish.ApplicationDto
{
    /// <summary>
    /// App下单订单状态
    /// </summary>
    public enum AppOrderStatus
    {
        /// <summary>
        /// 等候取餐-目前无用
        /// </summary>
        [DisplayName("等候取餐-目前无用")]
        WaitTake=-5,
        /// <summary>
        /// 待接单
        /// </summary>
        [DisplayName("待接单")]
        Paid=1,
        /// <summary>
        /// 已取餐
        /// </summary>
        [DisplayName("已取餐")]
        Finished=3,
        /// <summary>
        /// 已接单
        /// </summary>
        [DisplayName("已接单")]
        Taked=8,
        /// <summary>
        /// 待接单
        /// </summary>
        [DisplayName("待接单")]
        Arrive=9,
        /// <summary>
        /// 待取餐
        /// </summary>
        [DisplayName("待取餐")]
        DiningComplete=10
    }
}
