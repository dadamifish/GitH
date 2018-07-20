using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Mi.Fish.ApplicationDto
{
    /// <summary>
    /// 支付方式枚举
    /// </summary>
    public enum PayType
    {
        /// <summary>
        /// 扫码付
        /// </summary>
        [Description("扫码付")]
        Scan = 0,

        /// <summary>
        /// 现金支付
        /// </summary>
        [Description("现金支付")]
        Cash = 1,
        
        /// <summary>
        /// 现金找零
        /// </summary>
        [Description("找零")]
        Change=2,

        /// <summary>
        /// 银联支付
        /// </summary>
        [Description("银联支付")]
        Unionpay = 5,

        ///// <summary>
        ///// 一卡通支付
        ///// </summary>
        //[Description("一卡通支付")]
        //IcCard = 93,

        /// <summary>
        /// 微信支付
        /// </summary>
        [Description("微信支付")]
        Wx = 120,


        /// <summary>
        /// 支付宝支付
        /// </summary>
        [Description("支付宝支付")]
        Ali = 130,


        /// <summary>
        /// 秒通支付
        /// </summary>
        [Description("秒通支付")]
        Mutone = 112,

        /// <summary>
        /// 方特支付
        /// </summary>
        [Description("方特支付")]
        Ft = 113,

        /// <summary>
        /// 方特App接单支付
        /// </summary>
        [Description("方特App接单支付")]
        FtApp = 105,

        /// <summary>
        /// 微信(外)
        /// </summary>
        [Description("微信(外)")]
        WxWai = 992,

        /// <summary>
        /// 支付宝(外)
        /// </summary>
        [Description("支付宝(外)")]
        AliWai = 993,

        /// <summary>
        /// 礼券
        /// </summary>
        [Description("礼券")]
        Coupons = 8,

        /// <summary>
        /// 代金券
        /// </summary>
        [Description("代金券")]
        CashCoupon = 84,
    }
}
