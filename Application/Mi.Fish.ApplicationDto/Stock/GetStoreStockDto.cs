namespace Mi.Fish.ApplicationDto
{
    /// <summary>
    /// 
    /// </summary>
    public class GetStoreStockDto
    {
        /// <summary>
        /// 条码
        /// </summary>
        public string BarCode { get; set; }

        /// <summary>
        /// 商品名称
        /// </summary>
        public string GoodsName { get; set; }

        /// <summary>
        /// 实际库存
        /// </summary>
        public decimal StockQty { get; set; }

        /// <summary>
        /// 锁定库存
        /// </summary>
        public decimal LockStock { get; set; }
    }
}
