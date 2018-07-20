using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Mi.Fish.ApplicationDto
{
    #region input

    /// <summary>
    /// 
    /// </summary>
    public class RePrintInput
    {
        /// <summary>
        /// 订单号
        /// </summary>
        [Required]
        public string OrderNo { get; set; }
    }

    #endregion

    #region dto

    /// <summary>
    /// 
    /// </summary>
    public class RePrintDto
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
        /// 0：销售、2：退货
        /// </summary>
        public string SaleMode { get; set; }

        /// <summary>
        /// 商品集合
        /// </summary>
        public List<RePrintGoodsDto> Goods { get; set; }

        /// <summary>
        /// 应付
        /// </summary>
        public decimal PayAbleAmount { get; set; }

        /// <summary>
        /// 支付方式
        /// </summary>
        public List<PayMent> PayMents { get; set; }

        /// <summary>
        /// 实付
        /// </summary>
        public decimal PayAmount { get; set; }

        /// <summary>
        /// 找零
        /// </summary>
        public decimal ReturnMoney { get; set; }

        /// <summary>
        /// 单号
        /// </summary>
        public string OrderNo { get; set; }

        /// <summary>
        /// 第三方支付交易订单号
        /// </summary>
        public string ThirdTradeNo { get; set; }

        /// <summary>
        /// 交易时间
        /// </summary>
        public string TradeTime { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public class RePrintSaleDto
    {
        /// <summary>
        /// 交易时间
        /// </summary>
        public DateTime TradeTime { get; set; }

        /// <summary>
        /// 收银员
        /// </summary>
        public string Cashier { get; set; }

        /// <summary>
        /// 应付金额
        /// </summary>
        public decimal PayPrice { get; set; }

        /// <summary>
        /// 销售模式
        /// 0：销售、2：退货
        /// </summary>
        public string SaleMode { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public class RePrintGoodsDto
    {
        /// <summary>
        /// 商品名称
        /// </summary>
        public string GoodsName { get; set; }

        /// <summary>
        /// 数量
        /// </summary>
        public decimal Qty { get; set; }

        /// <summary>
        /// 金额
        /// </summary>
        public decimal PayPrice { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public class PaySqlDto
    {
        /// <summary>
        /// 支付类型编号
        /// </summary>
        public string PayTypeId { get; set; }

        /// <summary>
        /// 支付类型名称
        /// </summary>
        public string PayType { get; set; }

        /// <summary>
        /// 第三方订单号
        /// </summary>
        public string TradeNo { get; set; }

        /// <summary>
        /// 金额
        /// </summary>
        public decimal PayPrice { get; set; }
    }

    #endregion
}
