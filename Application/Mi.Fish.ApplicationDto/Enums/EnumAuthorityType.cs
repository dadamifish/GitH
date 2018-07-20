using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Mi.Fish.ApplicationDto
{
    /// <summary>
    /// 功能菜单权限
    /// </summary>
    public enum EnumAuthorityType
    {
        /// <summary>
        /// 查询交易记录
        /// </summary>
        [DisplayName("查询交易记录")]
        SearchSaleGrant=2,
        /// <summary>
        /// 前台设置
        /// </summary>
        [DisplayName("前台设置")]
        SetGrant = 4,
        /// <summary>
        /// 退单权限
        /// </summary>
        [DisplayName("退单权限")]
        BackGood=9,
        /// <summary>
        /// 重打印
        /// </summary>
        [DisplayName("重打印")]
        RePrint = 12,
        ///// <summary>
        ///// 开钱箱
        ///// </summary>
        //[DisplayName("开钱箱")]
        //OpenBox = 14,

        /// <summary>
        /// 单项打折
        /// </summary>
        [DisplayName("单项打折")]
        SingleDiscount = 17,

        /// <summary>
        /// 全部打折
        /// </summary>
        [DisplayName("全部打折")]
        AllDiscount = 18,
        /// <summary>
        /// 修改订单
        /// </summary>
        [DisplayName("修改订单")]
        EditOrder=23,
        /// <summary>
        /// 撤单
        /// </summary>
        [DisplayName("撤单")]
        Revoke=25,
    }
}
