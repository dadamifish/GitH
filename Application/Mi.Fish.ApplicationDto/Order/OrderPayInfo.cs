using System;
using System.Collections.Generic;
using System.Text;

namespace Mi.Fish.ApplicationDto
{
    /// <summary>
    /// 订单已支付金额
    /// </summary>
    public class OrderPayInfo
    {
        /// <summary>
        /// 订单编号
        /// </summary>
        public string OrderNo { get; set; }

        /// <summary>
        /// 台号
        /// </summary>
        public string TableNo { get; set; }

        /// <summary>
        /// 现金
        /// </summary>
        public decimal CashAmount { get; set; }

        /// <summary>
        /// 信用卡
        /// </summary>
        public decimal CreditCardAmount { get; set; }

        /// <summary>
        /// 礼券
        /// </summary>
        public decimal TicketVolumeAmount { get; set; }

        /// <summary>
        /// 代金券
        /// </summary>
        public decimal VoucherAmount { get; set; }

        /// <summary>
        /// 微信（外）
        /// </summary>
        public decimal WXWaiAmount { get; set; }

        /// <summary>
        /// 支付宝（外）
        /// </summary>
        public decimal ALiPayWaiAmount { get; set; }

        /// <summary>
        /// 微信
        /// </summary>
        public decimal WXAmount { get; set; }

        /// <summary>
        /// 支付宝
        /// </summary>
        public decimal ALiPayAmount { get; set; }

        /// <summary>
        /// 方特扫描
        /// </summary>
        public decimal LeYouScanAmount { get; set; }

        /// <summary>
        /// 秒通
        /// </summary>
        public decimal MuTonAmount { get; set; }

        /// <summary>
        /// 已付金额
        /// </summary>
        public decimal PaidAmount
        {
            get
            {
                return CashAmount + CreditCardAmount + TicketVolumeAmount + VoucherAmount  + WXWaiAmount + 
                       ALiPayWaiAmount + WXAmount + ALiPayAmount + LeYouScanAmount + MuTonAmount;
            }
        }

        /// <summary>
        /// 订单优惠金额
        /// </summary>
        public decimal DiscountAmount { get; set; }

        /// <summary>
        /// 订单总额
        /// </summary>
        public decimal TotalAmount { get; set; }

        /// <summary>
        /// 应付金额
        /// </summary>
        public decimal PayableAmount
        {
            get
            {
                if (TotalAmount >= PaidAmount + DiscountAmount)
                    return TotalAmount - (PaidAmount + DiscountAmount);
                else
                    return 0;
            }
        }

        /// <summary>
        /// 找零金额
        /// </summary>
        public decimal GiveAmount
        {

            get
            {
                if (TotalAmount <= PaidAmount + DiscountAmount)
                    return PaidAmount + DiscountAmount - TotalAmount;
                else
                    return 0;
            }
        }
    }
}
