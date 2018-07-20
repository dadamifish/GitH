using System;
using System.Collections.Generic;
using System.Text;
using Abp.AutoMapper;

namespace Mi.Fish.ApplicationDto
{

    /// <summary>
    /// 订单打印
    /// </summary>
    [AutoMap(typeof(OrdersPrintDto))]
    public class PrintInfo
    {
        /// <summary>
        /// 机号
        /// </summary>
         public string TerminalID { get; set; }
        /// <summary>
        /// 收银员
        /// </summary>
         public string Cashier { get; set; }
        /// <summary>
        /// 销售模式
        /// </summary>
        public string SaleMode { get; set; }
        /// <summary>
        /// 台号
        /// </summary>
        public string TableId { get; set; }
        /// <summary>
        /// 总额
        /// </summary>
        public  decimal Amount { get; set; }
        /// <summary>
        /// 折扣
        /// </summary>
        public decimal Discount { get; set; }
        /// <summary>
        /// 应付
        /// </summary>
        public decimal PayableAmount { get; set; }
        /// <summary>
        /// 实付
        /// </summary>
        public decimal PayAmount { get; set; }
        /// <summary>
        /// 找零
        /// </summary>
        public decimal GiveAmount { get; set; }
        /// <summary>
        /// 订单号
        /// </summary>

        public string OrderNo { get; set; }
        /// <summary>
        /// 支付订单号
        /// </summary>
        public string ThirdTradeNo { get; set; }
        /// <summary>
        /// 交易时间
        /// </summary>
        public DateTime TradeTime { get; set; }

        /// <summary>
        /// 支付方式
        /// </summary>
        public List<PayMentWay> PayMents { get; set; }

        /// <summary>
        /// 商品信息
        /// </summary>
        public List<GoodsInfo> Menus { get; set; }
    }

    /// <summary>
    /// 商品
    /// </summary>
    [AutoMap(typeof(Good))]
    public class GoodsInfo
    {
        /// <summary>
        /// 商品名称
        /// </summary>
        public string GoodsName { get; set; }

        /// <summary>
        /// 单价
        /// </summary>
        public decimal SalePrice { get; set; }

        /// <summary>
        /// 数量
        /// </summary>
        public int Qty { get; set; }

        /// <summary>
        /// 金额
        /// </summary>
        public decimal PayAmount { get; set; }

        /// <summary>
        /// 套餐信息
        /// </summary>
        public List<MealSubInfo> MealSubs { get; set; }

    }

    /// <summary>
    /// 套餐信息
    /// </summary>
    [AutoMap(typeof(GoodSub))]
    public class MealSubInfo
    {

        /// <summary>
        /// 商品名称
        /// </summary>
        public string GoodsName { get; set; }

        /// <summary>
        /// 单价
        /// </summary>
        public decimal SalePrice { get; set; }

        /// <summary>
        /// 数量
        /// </summary>
        public int Qty { get; set; }

        /// <summary>
        /// 金额
        /// </summary>
        public decimal PayAmount { get; set; }

    }


    /// <summary>
    /// 支付信息
    /// </summary>
    [AutoMap(typeof(PayMent))]
    public class PayMentWay
    {
        /// <summary>
        /// 支付名称
        /// </summary>
        public string PayName { get; set; }

        /// <summary>
        /// 支付金额
        /// </summary>
        public decimal Amount { get; set; }
    }

    /// <summary>
    /// 厨打产品
    /// </summary>
    public class GoodsCook
    {
        /// <summary>
        /// 产品编号
        /// </summary>
        public string GoodsNo { get; set; }

        /// <summary>
        /// 产品名称
        /// </summary>
        public string GoodsName { get; set; }

        /// <summary>
        /// 数量
        /// </summary>
        public int Qty { get; set; }
    }

    /// <summary>
    /// 厨打参数
    /// </summary>
    public class GoodsCookPrint
    {
        /// <summary>
        /// 产品编号
        /// </summary>
        public string GoodsNo { get; set; }

        /// <summary>
        /// 打印机名称
        /// </summary>
        public string PrintName { get; set; }
    }

}
