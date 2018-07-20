using System;
using System.Collections.Generic;
using System.Text;

namespace Mi.Fish.ApplicationDto
{
    /// <summary>
    /// app订单打印取餐小票实体
    /// </summary>
    public class OrdersPrintOutputDto
    {
        public OrdersPrintOutputDto()
        {
            Goods = new List<OrderMealPrint>();
            PayMents = new List<PayMent>();
        }
        /// <summary>
        /// 收银员
        /// </summary>
        public string Cashier { get; set; }
        /// <summary>
        /// 机号
        /// </summary>
        public string TerminalID { get; set; }
        /// <summary>
        /// A:销售  B:退货
        /// </summary>
        public string SaleMode { get; set; }
        /// <summary>
        /// 台号
        /// </summary>
        public string TableId { get; set; }
        /// <summary>
        /// 取餐码
        /// </summary>
        public string QrCode { get; set; }
        ///// <summary>
        /////是否是重打印
        ///// </summary>
        ////public bool IsRePrint { get; set; }
        /// <summary>
        /// POS订单号
        /// </summary>
        public string OrderNo { get; set; }
        /// <summary>
        /// App订单号
        /// </summary>
        public string ThirdTradeNo { get; set; }
        /// <summary>
        /// 订单标题  ("方特旅游App订单")
        /// </summary>
        public string Title { get; set; }
        /// <summary>
        /// 总额
        /// </summary>
        public string Amount { get; set; }
        ///// <summary>
        ///// 折扣金额
        ///// </summary>
        //public string Discount { get; set; }
        ///// <summary>
        ///// 应付款金额
        ///// </summary>
        //public string DueAmount { get; set; }
        /// <summary>
        /// 实付金额
        /// </summary>
        public string PayAmount { get; set; }
        ///// <summary>
        ///// 秒通订单号
        ///// </summary>
        //public string MuToneId { get; set; }
        /// <summary>
        /// 交易时间
        /// </summary>
        public string TradeTime { get; set; }
        /// <summary>
        /// 支付方式
        /// </summary>
        public List<PayMent> PayMents { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }
        /// <summary>
        /// 商品明细
        /// </summary>
        public List<OrderMealPrint> Goods { get; set; }

    }
    /// <summary>
    /// 打印商品明细
    /// </summary>
    public class OrderMealPrint
    {
        public OrderMealPrint()
        {
            GoodSubs = new List<GoodSub>();
        }
        /// <summary>
        /// 商品名称
        /// </summary>
        public string GoodsName { get; set; }
        ///// <summary>
        ///// 商品条码
        ///// </summary>
        //public string GoodsBarcode { get; set; }
        /// <summary>
        /// 数量
        /// </summary>
        public string Qty { get; set; }
        /// <summary>
        /// 单价
        /// </summary>
        public string Price { get; set; }
        /// <summary>
        /// 金额
        /// </summary>
        public string Amount { get; set; }

        public List<GoodSub> GoodSubs { get; set; }
    }
}
