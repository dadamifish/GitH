using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using Abp.Runtime.Caching;
using Abp.Runtime.Caching.Redis;
using Mi.Fish.ApplicationDto;


namespace Mi.Fish.Application
{
    public static class CacheManagerExtensions
    {
        public const string AreaInfoKey = "AreaInfoKey";
        public static ITypedCache<string, AreaInfo> AreaInfoCache(this ICacheManager cacheManager)
        {
            return cacheManager.GetCache<string, AreaInfo>(AreaInfoKey);
        }

        public const string PayInfoKey = "PayInfoKey";
        public static ITypedCache<string, PayInfo> PayInfoCache(this ICacheManager cacheManager)
        {
            return cacheManager.GetCache<string, PayInfo>(PayInfoKey);
        }

        #region 用户数据缓存

        private const string UserDataKey = "UserDataKey";


        /// <summary>
        /// 设置用户信息
        /// </summary>
        /// <param name="cacheManager"></param>
        /// <param name="userId"></param>
        /// <param name="userData"></param>
        public static void SetUserDataCache(this ICacheManager cacheManager, string userId, UserData userData)
        {
            cacheManager.GetCache<string, UserData>(UserDataKey).Set(userId, userData, TimeSpan.FromDays(2));
        }

        /// <summary>
        /// 移除用户信息
        /// </summary>
        /// <param name="cacheManager"></param>
        /// <param name="userId"></param>
        public static void RemoveUserDataCacheByUserId(this ICacheManager cacheManager, string userId)
        {
            cacheManager.GetCache<string, UserData>(UserDataKey).Remove(userId);
        }

        /// <summary>
        /// 获取用户信息
        /// </summary>
        /// <param name="cacheManager"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public static UserData GetUserDataCacheByUserId(this ICacheManager cacheManager, string userId)
        {
            return cacheManager.GetCache<string, UserData>(UserDataKey).GetOrDefault(userId);
        }


        private const string UserTokenKey = "UserTokenKey";

        /// <summary>
        /// 获取用户Token
        /// </summary>
        /// <param name="cacheManager"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public static string GetUserTokenCache(this ICacheManager cacheManager, string userId)
        {
            return cacheManager.GetCache<string, string>(UserTokenKey).GetOrDefault(userId);
        }

        /// <summary>
        /// 设置用户Token
        /// </summary>
        /// <param name="cacheManager"></param>
        /// <param name="userId"></param>
        /// <param name="token"></param>
        public static void SetUserTokenCache(this ICacheManager cacheManager, string userId, string token)
        {
            cacheManager.GetCache<string, string>(UserTokenKey).Set(userId, token, TimeSpan.FromHours(2));
        }

        /// <summary>
        /// 移除用户Token
        /// </summary>
        /// <param name="cacheManager"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public static void RemoveUserTokenCache(this ICacheManager cacheManager, string userId)
        {
            cacheManager.GetCache<string, string>(UserTokenKey).Remove(userId);
        }

        #endregion

        #region SaleMenuCache

        private const string SaleMenuKey = "SaleMenuKey";

        /// <summary>
        /// 获取销售菜单缓存
        /// </summary>
        /// <param name="cacheManager"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public static List<SaleMenuCache> GetSaleMenuCache(this ICacheManager cacheManager, string userId)
        {
            return cacheManager.GetCache<string, List<SaleMenuCache>>(SaleMenuKey).GetOrDefault(userId);
        }

        /// <summary>
        /// 设置销售菜单缓存
        /// </summary>
        /// <param name="cacheManager"></param>
        /// <param name="userId"></param>
        /// <param name="cache"></param>
        public static void SetSaleMenuCache(this ICacheManager cacheManager, string userId, List<SaleMenuCache> cache)
        {
            cacheManager.GetCache<string, List<SaleMenuCache>>(SaleMenuKey).Set(userId, cache, TimeSpan.FromHours(4));
        }


        /// <summary>
        /// 删除销售菜单缓存
        /// </summary>
        /// <param name="cacheManager"></param>
        /// <param name="userId"></param>
        public static void RemoveSaleMenuCache(this ICacheManager cacheManager, string userId)
        {
            cacheManager.GetCache<string, List<SaleMenuCache>>(SaleMenuKey).Remove(userId);
        }


        private const string PayDataKey = "PayDataKey";

        /// <summary>
        /// 获取支付缓存
        /// </summary>
        /// <param name="cacheManager"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public static List<OrderPayInfo> GetPayDataCache(this ICacheManager cacheManager, string userId)
        {
            return cacheManager.GetCache<string, List<OrderPayInfo>>(PayDataKey).GetOrDefault(userId);
        }

        /// <summary>
        /// 设置支付缓存
        /// </summary>
        /// <param name="cacheManager"></param>
        /// <param name="userId"></param>
        /// <param name="cache"></param>
        public static void SetPayDataCache(this ICacheManager cacheManager, string userId, List<OrderPayInfo> cache)
        {
            cacheManager.GetCache<string, List<OrderPayInfo>>(PayDataKey).Set(userId, cache, TimeSpan.FromHours(4));
        }

        #endregion

        #region LeyouTokenCache

        private const string LeyouTokenKey = "LeyouTokenKey";

        /// <summary>
        /// 获取乐游Token缓存
        /// </summary>
        /// <param name="cacheManager"></param>
        /// <returns></returns>
        public static LeyouTokenCache GetLeyouTokenCache(this ICacheManager cacheManager)
        {
            return cacheManager.GetCache<string, LeyouTokenCache>(LeyouTokenKey).GetOrDefault(LeyouTokenKey);
        }

        /// <summary>
        /// 设置乐游Token缓存
        /// </summary>
        /// <param name="cacheManager"></param>
        /// <param name="tokenCache"></param>
        public static void SetLeyouTokenCache(this ICacheManager cacheManager, LeyouTokenCache tokenCache)
        {
            cacheManager.GetCache<string, LeyouTokenCache>(LeyouTokenKey).Set(LeyouTokenKey, tokenCache);
        }

        #endregion

        #region LeYouNewOrderCache

        private const string LeYouNewOrderCache = "LeYouNewOrderCache";

        /// <summary>
        /// 获取乐游新订单缓存
        /// </summary>
        /// <param name="cacheManager"></param>
        /// <returns></returns>
        public static LeYouNewOrderCache GetLeYouNewOrderCache(this ICacheManager cacheManager)
        {
            return cacheManager.GetCache<string, LeYouNewOrderCache>(LeYouNewOrderCache).GetOrDefault(LeYouNewOrderCache);
        }

        /// <summary>
        /// 设置乐游新订单缓存
        /// </summary>
        /// <param name="cacheManager"></param>
        /// <param name="cache"></param>
        public static void SetLeYouNewOrderCache(this ICacheManager cacheManager, LeYouNewOrderCache cache)
        {
            cacheManager.GetCache<string, LeYouNewOrderCache>(LeYouNewOrderCache).Set(LeYouNewOrderCache, cache);
        }

        #endregion

        #region NewOrderCache

        private const string NewOrderKey = "NewOrderKey";

        /// <summary>
        /// 获取新的订单
        /// </summary>
        /// <param name="cacheManager"></param>
        /// <returns></returns>
        public static NewOrderCache GetNewOrderCache(this ICacheManager cacheManager, string storeId)
        {
            return cacheManager.GetCache<string, NewOrderCache>(NewOrderKey).GetOrDefault(storeId);
        }

        /// <summary>
        /// 设置新的订单
        /// </summary>
        /// <param name="cacheManager"></param>
        /// <param name="cache"></param>
        public static void SetNewOrderCache(this ICacheManager cacheManager, string storeId, NewOrderCache cache)
        {
            cacheManager.GetCache<string, NewOrderCache>(NewOrderKey).Set(storeId, cache);
        }

        #endregion

        #region tokenUserKey
        /// <summary>
        /// 用户功能授权缓存
        /// </summary>
        /// <param name="cacheManager"></param>
        /// <param name="userId">用户ID</param>
        /// <param name="type">功能类型</param>
        /// <param name="data">true false</param>
        /// <param name="ExpireTime"></param>
        public static void SetUserAuthority(this ICacheManager cacheManager, string userId, EnumAuthorityType type, bool data, int ExpireTime = 5)
        {
            SetUserAuthority(cacheManager, string.Concat(userId, "_", type.ToString()), data, ExpireTime);
        }
        public static bool GetUserAuthority(this ICacheManager cacheManager, string userId, EnumAuthorityType type)
        {
            string key = string.Concat(userId, "_", type.ToString());
            return GetUserAuthority(cacheManager, key);
        }
        public static void SetUserAuthority(this ICacheManager cacheManager, string key, bool data, int ExpireTime)
        {
            cacheManager.GetCache<string, bool>(key).Set(key, data, TimeSpan.FromMinutes(ExpireTime));
        }

        public static bool GetUserAuthority(this ICacheManager cacheManager, string key)
        {
            return cacheManager.GetCache<string, bool>(key).GetOrDefault(key);
        }

        public static void RemoveUserAuthority(this ICacheManager cacheManager, string userId, EnumAuthorityType type)
        {
            string key = string.Concat(userId, "_", type.ToString());
            cacheManager.GetCache<string, bool>(key).Remove(key);
        }
        #endregion

        #region Category Key

        private const string CategoryKey = "FishCategory";
        public static void SetCategoryCache(this ICacheManager cacheManager, string key, CategoryDetailOutPut cache)
        {
            cacheManager.GetCache<String, CategoryDetailOutPut>(CategoryKey).Set(key, cache, TimeSpan.FromMinutes(5));
        }

        public static CategoryDetailOutPut GetCategoryCache(this ICacheManager cacheManager, string key)
        {
            return cacheManager.GetCache<String, CategoryDetailOutPut>(CategoryKey).GetOrDefault(key);
        }
        #endregion

        #region Goods Key
        private const string GoodsKey = "FishGoodsKey";
        public static void SetGoodsCache(this ICacheManager cacheManager, string key, List<GoodsOutPut> cache)
        {
            cacheManager.GetCache<String, List<GoodsOutPut>>(GoodsKey).Set(key, cache, TimeSpan.FromMinutes(5));
        }

        public static List<GoodsOutPut> GetGoodsCache(this ICacheManager cacheManager, string key)
        {
            return cacheManager.GetCache<String, List<GoodsOutPut>>(GoodsKey).GetOrDefault(key);
        }
        #endregion

    }

}
