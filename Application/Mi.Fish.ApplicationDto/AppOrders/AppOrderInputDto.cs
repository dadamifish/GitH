using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Mi.Fish.ApplicationDto
{
    /// <summary>
    /// App订单转POS订单实体
    /// </summary>
    public class AppOrderInputDto
    {
        /// <summary>
        /// 订单号，App订单号
        /// </summary>
        [Required(ErrorMessage ="订单编号不能为空")]
        public string OrderId { get; set; }
        /// <summary>
        /// 商品Id
        /// </summary>
        [Required(ErrorMessage = "商品Id不能为空")]
        public string MealId { get; set; }
        /// <summary>
        /// 数量
        /// </summary>
        [Required(ErrorMessage = "数量不能为空")]
        public string Qty { get; set; }
        /// <summary>
        /// 单价
        /// </summary>
        [Required(ErrorMessage = "单价不能为空")]
        public string Price { get; set; }
        /// <summary>
        /// 取餐时间
        /// </summary>
        [Required(ErrorMessage = "取餐时间不能为空")]
        public string BookingTime { get; set; }
        /// <summary>
        /// 状态
        /// </summary>
        [Required(ErrorMessage = "状态不能为空")]
        public AppOrderStatus Status { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }
    }
}
