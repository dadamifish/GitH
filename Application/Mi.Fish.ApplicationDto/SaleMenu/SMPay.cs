using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace Mi.Fish.ApplicationDto
{
    #region enum

    /// <summary>
    /// 现金支付类型
    /// </summary>
    public enum CashPayType
    {
        /// <summary>
        /// 现金
        /// </summary>
        [Display(Name = "现金")]
        Cash = 1,

        /// <summary>
        /// 代金券
        /// </summary>
        [Display(Name = "代金券")]
        CashCoupon = 84,

        /// <summary>
        /// 支付宝外
        /// </summary>
        [Display(Name = "支付宝外")]
        AliWai = 993,

        /// <summary>
        /// 微信外
        /// </summary>
        [Display(Name = "微信外")]
        WxWai = 992
    }

    /// <summary>
    /// 第三方支付类型
    /// </summary>
    public enum ThirdPayType
    {
        /// <summary>
        /// 微信
        /// </summary>
        [Display(Name = "微信")]
        Wx = 120,

        /// <summary>
        /// 支付宝
        /// </summary>
        [Display(Name = "支付宝")]
        Ali = 130,

        /// <summary>
        /// 秒通
        /// </summary>
        [Display(Name = "秒通")]
        Mutone = 112,

        /// <summary>
        /// 方特
        /// </summary>
        [Display(Name = "方特")]
        Ft = 113
    }

    /// <summary>
    /// 其他支付类型
    /// </summary>
    public enum OtherPayType
    {
        /// <summary>
        /// 礼券
        /// </summary>
        [Display(Name = "礼券")]
        Coupons = 8,

        /// <summary>
        /// 信用卡
        /// </summary>
        [Display(Name = "信用卡")]
        Unionpay = 5
    }

    /// <summary>
    /// 金额类型 
    /// </summary>
    public enum EnumAmountType
    {
        /// <summary>
        /// 优惠金额
        /// </summary>
        [Display(Name = "优惠金额")]
        DiscountAmount,

        /// <summary>
        /// 订单总额
        /// </summary>
        [Display(Name = "订单总额")]
        TotalAmount,

        /// <summary>
        /// 应付金额
        /// </summary>
        [Display(Name = "应付金额")]
        PayableAmount,

        /// <summary>
        /// 已付金额
        /// </summary>
        [Display(Name = "已付金额")]
        PaidAmount,

        /// <summary>
        /// 找零金额
        /// </summary>
        [Display(Name = "找零金额")]
        GiveAmount
    }

    #endregion

        #region input

        /// <summary>
        /// 现金支付(现在、代金券、微信外、支付宝外)
        /// </summary>
    public class CashPayInput
    {
        /// <summary>
        /// 台号
        /// </summary>
        public string TableNo { get; set; }

        /// <summary>
        /// 支付金额
        /// </summary>
        [Range(0.00, 99999999, ErrorMessage = "支付金额不能小于0")]
        [Required(ErrorMessage = "支付金额不能为空")]
        public decimal PayAmount { get; set; }

        /// <summary>
        /// 现金支付类型
        /// </summary>
        public CashPayType CashType { get; set; }
    }

    /// <summary>
    /// 第三方支付(秒通、乐游、微信、支付宝)
    /// </summary>
    public class ThirdPayInput
    {
        /// <summary>
        /// 台号
        /// </summary>
        public string TableNo { get; set; }

        /// <summary>
        /// 支付金额
        /// </summary>
        [Range(0.00, 99999999, ErrorMessage = "支付金额不能小于0")]
        [Required(ErrorMessage = "支付金额不能为空")]
        public decimal PayAmount { get; set; }

        /// <summary>
        /// 餐饮支付订单号
        /// </summary>
        [Required(ErrorMessage = "餐饮支付订单号不能为空")]
        public string PayOrderNo { get; set; }

        /// <summary>
        /// 第三方支付订单号
        /// </summary>
        [Required(ErrorMessage = "第三方支付订单号不能为空")]
        public string TradeNo { get; set; }

        /// <summary>
        /// 第三方支付类型
        /// </summary>
        public ThirdPayType ThirdPay { get; set; }
    }

    /// <summary>
    /// 礼券支付
    /// </summary>
    public class CouponPayInput
    {
        /// <summary>
        /// 台号
        /// </summary>
        public string TableNo { get; set; }

        /// <summary>
        /// 支付金额
        /// </summary>
        [Range(0.01, 99999999, ErrorMessage = "支付金额需大于0")]
        [Required(ErrorMessage = "支付金额不能为空")]
        public decimal PayAmount { get; set; }

        /// <summary>
        /// 礼券
        /// </summary>
        [Required(ErrorMessage = "礼券不能为空")]
        public List<string> TicketVolumes { get; set; }

    }

    /// <summary>
    /// 信用卡支付
    /// </summary>
    public class CreditCardPayInput
    {
        /// <summary>
        /// 台号
        /// </summary>
        public string TableNo { get; set; }

        /// <summary>
        /// 支付金额
        /// </summary>
        [Range(0.01, 99999999, ErrorMessage = "支付金额需大于0")]
        [Required(ErrorMessage = "支付金额不能为空")]
        public decimal PayAmount { get; set; }

        /// <summary>
        /// 信用卡号
        /// </summary>
        public string CardNo { get; set; }

    }

    #endregion

    #region dto

    /// <summary>
    /// 支付返回结果
    /// </summary>
    public class SMPayDto
    {
        /// <summary>
        /// 是否完成支付
        /// </summary>
        public bool IsComplete { get; set; }

        /// <summary>
        /// 完成支付返回新的单号
        /// </summary>
        public string NewestOrderNo { get; set; }

        /// <summary>
        /// 付款信息
        /// </summary>
        public List<SMPayInfo> PayInfos { get; set; }

        /// <summary>
        /// 打印信息
        /// </summary>
        public OrdersPrintDto PrintInfo { get; set; }
    }

    /// <summary>
    /// 支付信息
    /// </summary>
    public class SMPayInfo
    {
        public string PayName { get; set; }

        public decimal Amount { get; set; }
    }

    #endregion

}
