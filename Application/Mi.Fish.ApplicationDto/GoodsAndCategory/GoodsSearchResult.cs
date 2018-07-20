namespace Mi.Fish.ApplicationDto
{
    /// <summary>
    /// 商品搜索结果
    /// </summary>
    public class GoodsSearchResult
    {
        /// <summary>
        /// 商品码
        /// </summary>
        public string ItemNo { get; set; }
        /// <summary>
        /// 商品条码
        /// </summary>
        public string GoodsNo { get; set; }
        /// <summary>
        /// 商品名称
        /// </summary>
        public string GoodsName { get; set; }
    }
}
