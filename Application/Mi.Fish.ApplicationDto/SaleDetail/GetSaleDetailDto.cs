namespace Mi.Fish.ApplicationDto
{
    /// <summary>
    /// 
    /// </summary>
    public class GetSaleDetailDto
    {
        /// <summary>
        /// 单号
        /// </summary>
        public string flow_no { get; set; }

        /// <summary>
        /// 序号
        /// </summary>
        public string flow_id { get; set; }

        /// <summary>
        /// 货号
        /// </summary>
        public string item_subno { get; set; }

        /// <summary>
        /// 条码
        /// </summary>
        public string barcode { get; set; }

        /// <summary>
        /// 商品名称
        /// </summary>
        public string item_name { get; set; }

        /// <summary>
        /// 销售数量
        /// </summary>
        public decimal sale_qnty { get; set; }

        /// <summary>
        /// 商品原价
        /// </summary>
        public string source_price { get; set; }

        /// <summary>
        /// 销售价格
        /// </summary>
        public string sale_price { get; set; }

        /// <summary>
        /// 支付金额
        /// </summary>
        public decimal sale_money { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public class GetSaleDetailOutput
    {
        /// <summary>
        /// 单号
        /// </summary>
        public string SaleNo { get; set; }

        /// <summary>
        /// 序号
        /// </summary>
        public string Sort { get; set; }

        /// <summary>
        /// 货号
        /// </summary>
        public string GoodsCode { get; set; }

        /// <summary>
        /// 条码
        /// </summary>
        public string BarCode { get; set; }

        /// <summary>
        /// 商品名称
        /// </summary>
        public string GoodsName { get; set; }

        /// <summary>
        /// 销售数量
        /// </summary>
        public decimal Qty { get; set; }

        /// <summary>
        /// 商品原价
        /// </summary>
        public string GoodsPrice { get; set; }

        /// <summary>
        /// 销售价格
        /// </summary>
        public string SalePrice { get; set; }

        /// <summary>
        /// 支付金额
        /// </summary>
        public decimal PayPrice { get; set; }
    }
}
