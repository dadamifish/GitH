using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Abp.AutoMapper;

namespace Mi.Fish.ApplicationDto
{
    #region GetAppGoods

    /// <summary>
    /// 
    /// </summary>
    public class GetAppGoodsInput
    {
        /// <summary>
        /// 分店编号
        /// </summary>
        public string StoreId { get; set; }

        /// <summary>
        /// token
        /// </summary>
        public string Token { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public class GetAppGoodsOutput : LeyouResult<GetAppGoodsMealList>
    {

    }

    /// <summary>
    /// 
    /// </summary>
    public class GetAppGoodsMealList
    {
        /// <summary>
        /// 
        /// </summary>
        public List<GetAppGoodsMeal> MealList { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public class GetAppGoodsMeal
    {
        /// <summary>
        /// 商品编号
        /// </summary>
        public string MealId { get; set; }

        /// <summary>
        /// 门店编号
        /// </summary>
        public string AssignedDiningStoreId { get; set; }

        /// <summary>
        /// 商品名称
        /// </summary>
        public string MealName { get; set; }

        /// <summary>
        /// 商品价格
        /// </summary>
        public string Price { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public class GetAppGoodsDto
    {
        /// <summary>
        /// 门店编号
        /// </summary>
        public string StoreNo { get; set; }

        /// <summary>
        /// 商品编号
        /// </summary>
        public string GoodsNo { get; set; }

        /// <summary>
        /// 商品名称
        /// </summary>
        public string GoodsName { get; set; }

        /// <summary>
        /// 销售单价
        /// </summary>
        public string SalePrice { get; set; }

        /// <summary>
        /// POS可售商品是否包含
        /// </summary>
        public bool FishCanSale { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    [AutoMap(typeof(GetAppGoodsDto))]
    public class GetFishGoodsDto
    {
        /// <summary>
        /// 门店编号
        /// </summary>
        public string StoreNo { get; set; }

        /// <summary>
        /// 商品编号
        /// </summary>
        public string GoodsNo { get; set; }

        /// <summary>
        /// 商品名称
        /// </summary>
        public string GoodsName { get; set; }

        /// <summary>
        /// 销售单价
        /// </summary>
        public string SalePrice { get; set; }
    }

    #endregion

    #region GetFishGoods

    /// <summary>
    /// 
    /// </summary>
    public class GetFishGoodsInput
    {
        /// <summary>
        /// 门店编号
        /// </summary>
        public string StoreNo { get; set; }

        /// <summary>
        /// 设备编号
        /// </summary>
        public string TerminalNo { get; set; }
    }

    #endregion

    #region AddAppGoods

    /// <summary>
    /// 
    /// </summary>
    public class AddAppGoodsInput
    {
        /// <summary>
        /// 商品编码
        /// </summary>
        [Required(ErrorMessage = "商品编码不能为空")]
        public string GoodsNo { get; set; }

        /// <summary>
        /// 商品名称
        /// </summary>
        [Required(ErrorMessage = "商品名称不能为空")]
        public string GoodsName { get; set; }

        /// <summary>
        /// 商品售价
        /// </summary>
        [Required(ErrorMessage = "商品售价不能小等于0"), Range(0, double.MaxValue)]
        public double SalePrice { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public class AddAppGoodsApiInput
    {
        /// <summary>
        /// 商品编码
        /// </summary>
        public string MealId { get; set; }

        /// <summary>
        /// 商品名称
        /// </summary>
        public string MealName { get; set; }

        /// <summary>
        /// 商品单价
        /// </summary>
        public double Price { get; set; }

        /// <summary>
        /// 门店编码
        /// </summary>
        public string StoreId { get; set; }
    }

    #endregion

    #region DelAppGoods

    /// <summary>
    /// 
    /// </summary>
    public class DelAppGoodsApiInput
    {
        /// <summary>
        /// 商品编号
        /// </summary>
        public string MealId { get; set; }

        /// <summary>
        /// 门店编号
        /// </summary>
        public string StoreId { get; set; }
    }

    #endregion

    #region Linq

    /// <summary>
    /// 
    /// </summary>
    public class GetAppGoodsDtoComparer : IEqualityComparer<GetAppGoodsDto>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public bool Equals(GetAppGoodsDto x, GetAppGoodsDto y)
        {
            if (x.GoodsNo == y.GoodsNo && x.StoreNo == y.StoreNo)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public int GetHashCode(GetAppGoodsDto obj)
        {
            return obj.GoodsNo.GetHashCode();
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public class GetFishGoodsDtoComparer : IEqualityComparer<GetFishGoodsDto>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public bool Equals(GetFishGoodsDto x, GetFishGoodsDto y)
        {
            if (x.GoodsNo == y.GoodsNo && x.StoreNo == y.StoreNo)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public int GetHashCode(GetFishGoodsDto obj)
        {
            return obj.GoodsNo.GetHashCode();
        }
    }

    #endregion
}
