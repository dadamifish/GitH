using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
namespace Mi.Fish.ApplicationDto
{
    /// <summary>
    /// 销售类型
    /// </summary>
    public enum SaleType
    {
        /// <summary>
        /// 全部
        /// </summary>
        [DisplayName("全部")]
        [Display(Order = 1)]
        All = -1,

        /// <summary>
        /// 销售
        /// </summary>
        [DisplayName("销售")]
        Sale = 0,

        /// <summary>
        /// 退货
        /// </summary>
        [DisplayName("退货")]
        Return = 2,
    }

    /// <summary>
    /// 
    /// </summary>
    public class GetSaleDetailInput
    {
        /// <summary>
        /// 商品条码
        /// </summary>
        public string BarCode { get; set; }

        /// <summary>
        /// 商品名称
        /// </summary>
        public string GoodsName { get; set; }

        /// <summary>
        /// 销售类型
        /// </summary>
        [Required(ErrorMessage = "销售类型不允许为空")]
        public SaleType SaleType { get; set; }

        /// <summary>
        /// 销售单号
        /// </summary>
        public string SaleNo { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public class GetSaleDetailProcInput
    {
        /// <summary>
        /// 收银员编号
        /// </summary>
        public string CashierNO { get; set; }

        /// <summary>
        /// 商品编号
        /// </summary>
        public string item_no { get; set; }

        /// <summary>
        /// 商品名称
        /// </summary>
        public string item_name { get; set; }

        /// <summary>
        /// 销售类型
        /// </summary>
        public string sell_way { get; set; }

        /// <summary>
        /// 当班开始时间
        /// </summary>
        public DateTime oper_date { get; set; }

        /// <summary>
        /// 销售单号
        /// </summary>
        public string flow_no { get; set; }
    }
}
