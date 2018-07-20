using System.Collections.Generic;

namespace Mi.Fish.ApplicationDto
{
    #region input

    /// <summary>
    /// 
    /// </summary>
    public class GetLeyouAppOrderInput
    {
        /// <summary>
        /// 门店编号
        /// </summary>
        public string StoreId { get; set; }

        /// <summary>
        /// 订单状态
        /// </summary>
        public int Status { get; set; }

        /// <summary>
        /// 分页大小
        /// </summary>
        public int PageSize { get; set; }

        /// <summary>
        /// 当前页
        /// </summary>
        public int PageIndex { get; set; }

        /// <summary>
        /// 订单开始日期
        /// </summary>
        public string OrderBeginDate { get; set; }

        /// <summary>
        /// 订单截止日期
        /// </summary>
        public string OrderEndDate { get; set; }

        /// <summary>
        /// Token令牌
        /// </summary>
        public string Token { get; set; }
    }

    #endregion

    #region dto

    /// <summary>
    /// 
    /// </summary>
    public class GetLeyouAppOrderOutput
    {
        /// <summary>
        /// 
        /// </summary>
        public OerderDetail[] List { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public class OerderDetail
    {
        /// <summary>
        /// 
        /// </summary>
        public string AssignedCustomerId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string AssignedCustomerCustName { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string TotalMoney { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string QrCode { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Status { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Paymode { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string PayTime { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string InputTime { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string CustPhone { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string MealTime { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Remark { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string MealCode { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string GetMealType { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string MealTimeValue { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public DiningOrderDetailList[] MealDetailList { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public class DiningOrderDetailList
    {
        /// <summary>
        /// 
        /// </summary>
        public string OrderId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string AssignedDiningStoreParkId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string AssignedDiningStoreStoreId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string AssignedDiningStoreDiningName { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string AssignedDiningMealMealName { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string AssignedDiningMealMealId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string BookingTime { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Quantity { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Price { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string AssignedDiningOrderTotalMoney { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string AssignedDiningOrderInputTime { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Status { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string AssignedDiningOrderQRCode { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string DiningMealPicUrl { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string AssignedDiningMealId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string AssignedDiningMealCanSale { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string AssignedDiningMealCanBooking { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string AssignedDiningMealMaxBookingNum { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string AssignedDiningMealIsDelete { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string AssignedDiningMealDiscount { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Id { get; set; }
    }


    /// <summary>
    /// 
    /// </summary>
    public class GetLeyouAppOrderDto : LeyouResult<GetLeyouAppOrderOutput>
    {

    }

    /// <summary>
    /// 
    /// </summary>
    public class IsNewOrderDto
    {
        /// <summary>
        /// 
        /// </summary>
        public bool Flag { get; set; }
    }

    #endregion

    #region cache

    /// <summary>
    /// 
    /// </summary>
    public class LeYouNewOrderCache
    {
        /// <summary>
        /// 
        /// </summary>
        public string MaxOrderId { get; set; }

    }

    #endregion
}
