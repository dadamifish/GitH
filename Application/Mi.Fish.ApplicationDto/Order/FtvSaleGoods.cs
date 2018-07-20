using System;
using System.Collections.Generic;
using System.Text;

namespace Mi.Fish.ApplicationDto
{
    public class FtvSaleGoods
    {

        /// <summary>
        /// 产品编号
        /// </summary>
       public string sGoodsNO { get; set; }
        
        /// <summary>
        /// 产品名称
        /// </summary>
        public string sGoodsName { get; set; }
        
        /// <summary>
        /// 产品售价
        /// </summary>
        public decimal nRealSalePrice { get; set; }

        /// <summary>
        /// 产品默认价格
        /// </summary>
        public decimal nSalePrice { get; set; }

        /// <summary>
        /// 产品类型 
        /// </summary>
        public string  sGoodTypeID { get; set; }

        /// <summary>
        /// 条形码
        /// </summary>
        public string sMainBarcode { get; set; }

        /// <summary>
        /// 套餐子产品父级编号 
        /// </summary>
        public string master_no { get; set; }

        /// <summary>
        /// 是否判断实时库存
        /// </summary>
        public int have_stock { get; set; }

        /// <summary>
        /// 是否为套餐子产品
        /// </summary>
        public bool IsMealSub { get; set; }
    }
}
