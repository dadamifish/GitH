using Mi.Fish.Common;

namespace Mi.Fish.ApplicationDto
{
    public class FishOrdersOutputDto
    {
        /// <summary>
        /// 订单Id
        /// </summary>
        public string OrderId { get; set; }
        /// <summary>
        /// 门店Id
        /// </summary>
        public string StoreId { get; set; }
        /// <summary>
        /// 商品Id
        /// </summary>
        public string MealId { get; set; }
        /// <summary>
        /// 商品名称
        /// </summary>
        public string MealName { get; set; }
        /// <summary>
        /// 就餐时间
        /// </summary>
        public string BookingTime { get; set; }
        /// <summary>
        /// 数量
        /// </summary>
        public string Qty { get; set; }
        /// <summary>
        /// 单价
        /// </summary>
        public string Price { get; set; }
        /// <summary>
        /// 状态
        /// </summary>
        public AppOrderStatus status { get; set; }
        /// <summary>
        /// 状态描述
        /// </summary>
        public string statusDesc => status.DisplayName();
        /// <summary>
        /// 取餐类型
        /// </summary>
        public string GetMealType { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }
    }
}
