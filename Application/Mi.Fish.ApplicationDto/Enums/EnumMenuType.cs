using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;

namespace Mi.Fish.ApplicationDto
{
    /// <summary>
    /// 菜单操作类型 
    /// </summary>
    public enum EnumMenuType
    {
        /// <summary>
        /// 增加菜品
        /// </summary>
        [Description("增加菜品")]
        Add = 0,

        /// <summary>
        /// 增加数量
        /// </summary>
        [Description("增加数量")]
        AddQty = 1,


        /// <summary>
        /// 减少数量
        /// </summary>
        [Description("减少数量")]
        ReduceQty = 2,


        /// <summary>
        /// 修改数量
        /// </summary>
        [Description("修改数量")] 
        UpdateQty = 3,

        /// <summary>
        /// 删除
        /// </summary>
        [Description("删除")]
        Delete = 4,


        /// <summary>
        /// 清空
        /// </summary>
        [Description("清空")]
        Empty = 5,


        /// <summary>
        /// 单项折扣
        /// </summary>
        [Description("单项折扣")]
        SingleDiscount = 6,


        /// <summary>
        /// 全部折扣
        /// </summary>
        [Description("全部折扣")]
        AllDiscount = 7,

    }
}
