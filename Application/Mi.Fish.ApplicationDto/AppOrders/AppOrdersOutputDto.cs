using System;
using System.Collections.Generic;
using System.Text;

namespace Mi.Fish.ApplicationDto
{
    public class AppOrdersOutputDto :AppResult<AppOrdersList>
    {
    
    }

    public class AppOrdersList
    {
        public AppOrdersList()
        {
            List = new List<AppOrderMain>();
        }
        public List<AppOrderMain> List { get; set; }
    }

    public class AppOrderMain
    {
        public AppOrderMain()
        {
            MealDetailList = new List<AppOrderDetail>();
        }
        public string Status { get; set; }
        public string Id { get; set; }
        public string MealTime { get; set; }
        public string Remark { get; set; }
        public string GetMealType { get; set; }
        public string MealTimeValue { get; set; }

        public List<AppOrderDetail> MealDetailList { get; set; }
    }

    public class AppOrderDetail
    {
        public string OrderId { get; set; }
        public string AssignedDiningStoreStoreId { get; set; }
        public string AssignedDiningMealMealName { get; set; }
        public string AssignedDiningMealMealId { get; set; }
        public string BookingTime { get; set; }
        public string Quantity { get; set; }
        public string Price { get; set; }
        public string AssignedDiningOrderTotalMoney { get; set; }
        public string AssignedDiningOrderInputTime { get; set; }
        public string Status { get; set; }
        public string AssignedDiningMealId { get; set; }
        public string Id { get; set; }
    }
}
