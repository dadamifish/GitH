namespace Mi.Fish.ApplicationDto
{
    using System.Collections.Generic;

    /// <summary>
    /// 退款详情
    /// </summary>
    public class OrderRefundInfo
    {
        /// <summary>
        /// 支付类型
        /// </summary>
        public string PayType { get; set; }
        /// <summary>
        /// 退款金额
        /// </summary>
        public decimal PayAmount { get; set; }

        /// <summary>
        /// 退款类型
        /// </summary>
        public EnumRefundType RefundType { get; set; }
    }

    /// <summary>
    /// 退单详情
    /// </summary>
    public class BaseOrderRefundOutput
    {
        /// <summary>
        /// 退单单号
        /// </summary>
        public string OrderNo { get; set; }
        /// <summary>
        /// 退款提示
        /// </summary>
        public string Message { get; set; }
        /// <summary>
        /// 退款详情
        /// </summary>
        public List<OrderRefundInfo> OrderRefundList { get; set; }
    }
    /// <summary>
    /// 退款详情
    /// </summary>
    public class OrderRefundOutput : BaseOrderRefundOutput
    {
        /// <summary>
        /// 是否可以直接退款
        /// </summary>
        public bool IsRefund { get; set; }
    }
}
