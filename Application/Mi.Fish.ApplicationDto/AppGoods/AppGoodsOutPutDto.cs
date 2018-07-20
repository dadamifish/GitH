using System;
using System.Collections.Generic;
using System.Text;

namespace Mi.Fish.ApplicationDto
{
    /// <summary>
    /// app接口获取 商品数据 Dto
    /// </summary>
    public class AppGoodsOutputDto:AppResult<AppGoodsList>
    {

    }

    public class AppGoodsList
    {
        public AppGoodsList()
        {
            MealList = new List<MealList>();
        }
        public List<MealList> MealList { get; set; }
    }

    public class MealList
    {
        public string MealId { get; set; }
        public string AssignedDiningStoreId { get; set; }
        public string MealName { get; set; }
        public string Price { get; set; }
    }


    public class MealListOutputDto
    {
        /// <summary>
        /// 门店Id
        /// </summary>
        public string StoreId { get; set; }
        /// <summary>
        /// 商品Id
        /// </summary>
        public string MealId { get; set; }
        /// <summary>
        /// 商品名称
        /// </summary>
        public string MealName { get; set; }
        /// <summary>
        /// 单价
        /// </summary>
        public string Price { get; set; }
    }

    public class MealListOutputDtoEquality : IEqualityComparer<MealListOutputDto>
    {
        public bool Equals(MealListOutputDto x, MealListOutputDto y)
        {
            if(x.StoreId==y.StoreId && x.MealId == y.MealId)
            {
                return true;
            }
            return false;
        }

        public int GetHashCode(MealListOutputDto obj)
        {
            if(obj == null)
            {
                return 0;
            }
            return obj.ToString().GetHashCode();
        }
    }
}
