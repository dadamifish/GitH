namespace Mi.Fish.ApplicationDto
{
    using System.ComponentModel.DataAnnotations.Schema;
    public class GoodsDTO
    {
        /// <summary>
        /// 商品编号
        /// </summary>
        public string GoodsID { get; set; }
        /// <summary>
        /// 商品名称
        /// </summary>
        public string GoodsName { get; set; }
        /// <summary>
        /// 是否需要管理库存，1不需要，0需要
        /// </summary>
        public int Have_stock { get; set; }
        /// <summary>
        /// 商品类型
        /// </summary>
        public string sGoodTypeID { get; set; }
        /// <summary>
        /// 估算数量，-1表示无限，0表示无库存，
        /// </summary>
        public int guqingqty { get; set; }
        /// <summary>
        /// 成本价
        /// </summary>
        public int cost_price { get; set; }
       /// <summary>
       /// 商品主编号
       /// </summary>
        public string barcode { get; set; }
        /// <summary>
        /// 商品子编号
        /// </summary>
        public string item_subno { get; set; }
        /// <summary>
        /// 商品销售价格
        /// </summary>
        public decimal sale_price{ get; set; }
    }

    /// <summary>
    /// 商品详情
    /// </summary>
    public class GoodsOutPut
    {
        /// <summary>
        /// 商品销售价格
        /// </summary>
        public decimal SalePrice { get; set; }
        /// <summary>
        /// 商品子编号
        /// </summary>
        public string SubItemNo { get; set; }
        /// <summary>
        /// 商品主编号
        /// </summary>
        public string BarCode { get; set; }
        /// <summary>
        /// 成本价
        /// </summary>
        public int CostPrice { get; set; }
        /// <summary>
        /// 商品类型 C 套餐
        /// </summary>
        public string GoodsType { get; set; }
        /// <summary>
        /// 是否存在估清，1有0没有
        /// </summary>
        public int IsGuQing { get; set; }
        /// <summary>
        /// 是否需要管理库存，1不需要，0需要
        /// </summary>
        public int Havestork { get; set; }
        /// <summary>
        /// 商品编号
        /// </summary>
        public string GoodsNo { get; set; }
        /// <summary>
        /// 商品名称
        /// </summary>
        public string GoodsName { get; set; }
        /// <summary>
        /// 暂时不需要，套餐详情可能用到
        /// </summary>
        public string ParentId { get; set; }
    }
}
