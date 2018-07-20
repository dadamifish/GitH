namespace Mi.Fish.Infrastructure.Leyou
{
    /// <summary>
    /// 乐游API
    /// </summary>
    public static class LeyouApi
    {
        /// <summary>
        /// 获取乐游Token
        /// </summary>
        public const string PostToken = "api/DinnerSystemSynchronization/Login";

        /// <summary>
        /// 获取乐游APP订单
        /// </summary>
        public const string GetAppOrder = "api/DinnerSystemSynchronization/QueryOrderListForMealSystem";

        /// <summary>
        /// 获取乐游APP商品
        /// </summary>
        public const string GetAppGoods = "api/DinnerSystemSynchronization/QuerySynDiningMeal";

        /// <summary>
        /// 上传本地POS商品到乐游APP
        /// </summary>
        public const string AddAppGoods = "api/DinnerSystemSynchronization/SynDiningMeal";

        /// <summary>
        /// 删除已上传到乐游APP的商品
        /// </summary>
        public const string DelAppGoods = "api/DinnerSystemSynchronization/DelSynDiningMeal";
    }
}
