using System;
using System.ComponentModel.DataAnnotations;

namespace Mi.Fish.ApplicationDto
{
    /// <summary>
    /// 订单报表状态
    /// </summary>
    public enum OrderReportStatus
    {
        /// <summary>
        /// 全部
        /// </summary>
        [Display(Name = "全部", Order = 1)]
        All = -1,

        /// <summary>
        /// 已支付
        /// </summary>
        [Display(Name = "已支付")]
        Paid = 0,

        /// <summary>
        /// 已出单
        /// </summary>
        [Display(Name = "已出单")]
        Commited = 1,

        /// <summary>
        /// 已退单
        /// </summary>
        [Display(Name = "已退单")]
        Return = 2,
    }

    /// <summary>
    /// 
    /// </summary>
    public class GetOrderReportInput
    {
        /// <summary>
        /// 订单号
        /// </summary>
        public string OrderNo { get; set; }

        /// <summary>
        /// 订单状态
        /// </summary>
        [Required(ErrorMessage = "订单状态不允许为空")]
        public OrderReportStatus OrderStatus { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public class GetOrderReportSqlInput
    {
        /// <summary>
        /// 门店编号
        /// </summary>
        public string StoreNo { get; set; }

        private string _orderNo;
        /// <summary>
        /// 订单号
        /// </summary>
        public string OrderNo
        {
            get
            {
                return _orderNo;
            }
            set
            {
                _orderNo = "%" + value + "%";
            }
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public class GetOrderReportDto
    {
        /// <summary>
        /// 订单号
        /// </summary>
        public string OrderNo { get; set; }

        /// <summary>
        /// 商品名称
        /// </summary>
        public string GoodsName { get; set; }

        /// <summary>
        /// 支付方式
        /// </summary>
        public string PayType { get; set; }

        /// <summary>
        /// 交易时间
        /// </summary>
        public string TradeTime { get; set; }

        /// <summary>
        /// 餐桌号
        /// </summary>
        public string TableNum { get; set; }

        /// <summary>
        /// 购买数量
        /// </summary>
        public decimal Qty { get; set; }

        /// <summary>
        /// 销售价格
        /// </summary>
        public decimal SalePrice { get; set; }

        /// <summary>
        /// 支付金额
        /// </summary>
        public decimal PayPrice { get; set; }

        /// <summary>
        /// 订单状态
        /// </summary>
        public string OrderStatus { get; set; }

        /// <summary>
        /// 交易状态
        /// </summary>
        public string TradeStatus { get; set; }

        /// <summary>
        /// 交易类型
        /// </summary>
        public string TradeType { get; set; }

        /// <summary>
        /// 打印状态
        /// </summary>
        public string PrintStatus { get; set; }

        /// <summary>
        /// 项目序号
        /// </summary>
        public decimal ItemNo { get; set; }
    }
}
