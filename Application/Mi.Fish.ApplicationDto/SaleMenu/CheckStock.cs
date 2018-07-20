using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Mi.Fish.ApplicationDto
{
    #region input

    /// <summary>
    /// 库存判断
    /// </summary>
    public class CheckStockInput
    {
        /// <summary>
        /// 商品编号
        /// </summary>
        [Required(ErrorMessage = "商品编号不能为空")]
        public string GoodsNo { get; set; }

        /// <summary>
        /// 库存数量
        /// </summary>
        public int? Qty { get; set; }
    }

    /// <summary>
    /// 订单库存检验
    /// </summary>
    public class CheckOrderInput
    {
        /// <summary>
        /// 订单编号
        /// </summary>
        [Required(ErrorMessage = "订单编号不能为空")]
        public string OrderNo { get; set; }
    }


    #endregion

    #region dto

    /// <summary>
    /// 实时库存
    /// </summary>
    public class ProcNowStock
    {
        /// <summary>
        /// 实时库存
        /// </summary>
        public decimal nStockQty { get; set; }
    }

    /// <summary>
    /// 估清库存
    /// </summary>
    public class SqlVirtualStock
    {
        /// <summary>
        /// HavaStock字段不能判断是否允许估清库存，返回值总为0
        /// </summary>
        public int HavaStock { get; set; }

        /// <summary>
        /// 估清库存
        /// </summary>
        public int VirtualStock { get; set; }
    }

    /// <summary>
    /// 库存返回参数
    /// </summary>
    public class GetRealStockDto
    {
        /// <summary>
        /// 商品编号
        /// </summary>
        public string GoodsNo { get; set; }

        /// <summary>
        /// 库存状态
        /// </summary>
        public bool HasStock { get; set; }

        /// <summary>
        /// 实时库存
        /// </summary>
        public decimal Stock { get; set; }

        /// <summary>
        /// 返回结果提示信息
        /// </summary>
        public string Message { get; set; }
    }

    /// <summary>
    /// 下单前检验订单所有菜品库存
    /// </summary>
    public class GetOrderStockDto
    {
        /// <summary>
        /// 订单编号
        /// </summary>
        public string OrderNo { get; set; }

        /// <summary>
        /// 库存状态
        /// </summary>
        public bool HasStock { get; set; }

        /// <summary>
        /// 返回结果提示信息
        /// </summary>
        public string Message { get; set; }

    }

    #endregion
}
