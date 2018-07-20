using System;
using System.Collections.Generic;

namespace Mi.Fish.ApplicationDto
{
    /// <summary>
    /// 
    /// </summary>
    public class GetNewOrderOutput
    {
        /// <summary>
        /// 分店编号
        /// </summary>
        public string StoreNo { get; set; }

        /// <summary>
        /// 订单号
        /// </summary>
        public string OrderNo { get; set; }

        /// <summary>
        /// 餐桌号
        /// </summary>
        public string TableNum { get; set; }

        /// <summary>
        /// 支付金额
        /// </summary>
        public decimal PayPrice { get; set; }

        /// <summary>
        /// 订单交易时间
        /// </summary>
        public string TradeTime { get; set; }

        /// <summary>
        /// 订单创建时间
        /// </summary>
        public string CteateTime { get; set; }

        /// <summary>
        /// 商品编号
        /// </summary>
        public string GoodsId { get; set; }

        /// <summary>
        /// 商品名称
        /// </summary>
        public string GoodsName { get; set; }

        /// <summary>
        /// 商品条码
        /// </summary>
        public string BarCode { get; set; }

        /// <summary>
        /// 购买数量
        /// </summary>
        public decimal Qty { get; set; }

        /// <summary>
        /// 销售价格
        /// </summary>
        public decimal SalePrice { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public class GetNewOrderDto
    {
        /// <summary>
        /// 分店编号
        /// </summary>
        public string StoreNo { get; set; }

        /// <summary>
        /// 订单号
        /// </summary>
        public string OrderNo { get; set; }

        /// <summary>
        /// 餐桌号
        /// </summary>
        public string TableNum { get; set; }

        /// <summary>
        /// 订单交易时间
        /// </summary>
        public string TradeTime { get; set; }

        /// <summary>
        /// 订单创建时间
        /// </summary>
        public string CteateTime { get; set; }

        /// <summary>
        /// 支付金额
        /// </summary>
        public decimal PayPrice { get; set; }

        /// <summary>
        /// 购买商品
        /// </summary>
        public List<GetNewOrderGoods> Goods { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public class GetNewOrderGoods
    {
        /// <summary>
        /// 商品编号
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// 商品名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 商品条码
        /// </summary>
        public string BarCode { get; set; }

        /// <summary>
        /// 购买数量
        /// </summary>
        public decimal Qty { get; set; }

        /// <summary>
        /// 销售价格
        /// </summary>
        public decimal SalePrice { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public class NewOrderCache
    {
        /// <summary>
        /// 当前最大订单号
        /// </summary>
        public string MaxOrderNo { get; set; }

        /// <summary>
        /// 当前订单缓存
        /// </summary>
        public List<GetNewOrderDto> Data { get; set; }
    }
}
