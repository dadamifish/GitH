namespace Mi.Fish.ApplicationDto
{
    /// <summary>
    /// 
    /// </summary>
    public class GetStoreStockInput
    {
        /// <summary>
        /// 商品名称或条码
        /// </summary>
        public string Key { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public class GetStoreStockSqlInput: GetStoreStockInput
    {
        /// <summary>
        /// 门店号
        /// </summary>
        public string StoreNo { get; set; }

        /// <summary>
        /// 设备号
        /// </summary>
        public string TerminalNo { get; set; }
    }
}
