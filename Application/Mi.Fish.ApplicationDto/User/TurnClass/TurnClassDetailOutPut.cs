using System;
using System.Collections.Generic;
using System.Text;

namespace Mi.Fish.ApplicationDto
{
    /// <summary>
    /// 交班明细
    /// </summary>
    public class TurnClassDetailOutPut
    {
        /// <summary>
        /// 收银员工号
        /// </summary>
        public string CashierNo { get; set; }

        ///// <summary>
        ///// 当前当班人单号
        ///// </summary>
        //public string CurrentDealNo { get; set; }
        /// <summary>
        /// 班次
        /// </summary>
        public  string Shift { get; set; }

        /// <summary>
        /// 打印时间
        /// </summary>
        public string PrintTime { get; set; }

        /// <summary>
        /// 当班开始时间
        /// </summary>
        public string ShiftStartTime { get; set; }

        /// <summary>
        /// 当班结束时间
        /// </summary>
        public string ShiftEndTime { get; set; }

        /// <summary>
        /// 开单张数
        /// </summary>
        public int SaleCount { get; set; }

        /// <summary>
        /// 当班钱箱余额
        /// </summary>
        public string ShiftBalanceMoney { get; set; }

        /// <summary>
        /// 实销金额(人民币)
        /// </summary>
        public string RealSaleMoney { get; set; }


        /// <summary>
        /// 应交金额列表
        /// </summary>
        public List<string> ListAmountReceivable { get; set; }


    }

    /// <summary>
    /// 应交金额
    /// </summary>
    public class AmountReceivable
    {
        /// <summary>
        /// (应收金额)人民币金额
        /// </summary>
        public string RmbSaleMoney { get; set; }

        /// <summary>
        /// (应收金额)信用卡金额
        /// </summary>
        public string CreditCardMoney { get; set; }

        /// <summary>
        /// (应收金额)金卡金额
        /// </summary>
        public string GoldMoney { get; set; }
        /// <summary>
        /// (应收金额)方特App金额
        /// </summary>
        public string AppMoney { get; set; }
        /// <summary>
        /// (应收金额)微信金额
        /// </summary>
        public string WxMoney { get; set; }

        /// <summary>
        /// (应收金额)支付宝金额
        /// </summary>
        public string AliMoney { get; set; }

        /// <summary>
        /// (应收金额)秒通金额
        /// </summary>
        public string MutonMoney { get; set; }
        /// <summary>
        /// (应收金额)方特卡
        /// </summary>
        public string FateCardSaleMoney { get; set; }

        /// <summary>
        /// (应收金额)二维码
        /// </summary>
        public string QRcodeMoney { get; set; }

        /// <summary>
        /// (应收金额)礼券
        /// </summary>
        public string CouponsMoney { get; set; }

        /// <summary>
        /// (应收金额)代金券
        /// </summary>
        public string VoucherMoney { get; set; }

        /// <summary>
        /// (应收金额)微信外
        /// </summary>
        public string WxOutMoney { get; set; }

        /// <summary>
        /// (应收金额)支付宝外
        /// </summary>
        public string AliOutMoney { get; set; }
    }
}
