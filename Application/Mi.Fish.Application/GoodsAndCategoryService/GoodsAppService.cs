namespace Mi.Fish.Application
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Threading.Tasks;
    using Mi.Fish.ApplicationDto;
    using Abp.AutoMapper;
    using Abp.Runtime.Caching;
    using Mi.Fish.EntityFramework;
    using Mi.Fish.Infrastructure.Results;

    public class GoodsAppService : AppServiceBase, IGoodsAppService
    {
        private readonly ICacheManager _cacheManager;
        private readonly LocalDbContext _context;
        public GoodsAppService(ICacheManager cacheManager, LocalDbContext context)
        {
            this._cacheManager = cacheManager;
            this._context = context;
        }
        private Result<T> GainDefaultResult<T>(UserData userinfo)
        {
            if (string.IsNullOrEmpty(userinfo.StorageNo) || string.IsNullOrEmpty(userinfo.TerminalID))
            {
                return new Result<T>(ResultCode.Expired, default(T), "登录已经失效");
            }
            return new Result<T>(ResultCode.Ok, default(T));
        }

        /// <summary>
        /// 获取商品列表
        /// </summary>
        /// <param name="categoryNo">三级分类</param>
        /// <param name="type">0自定义商品列表</param>
        /// <returns></returns>
        public async Task<Result<List<GoodsOutPut>>> GetGoodsList(string categoryNo, int type)
        {
            var result = new List<GoodsOutPut>();
            try
            {
                var query = string.Empty;
                var category = categoryNo;

                string userid = FishSession.UserId;
                var userinfo = _cacheManager.GetUserDataCacheByUserId(userid);
                var defaultResult = GainDefaultResult<List<GoodsOutPut>>(userinfo);
                if (defaultResult.Code == ResultCode.Expired)
                {
                    return defaultResult;
                }
                if (string.IsNullOrEmpty(userinfo.StorageNo) || string.IsNullOrEmpty(userinfo.TerminalID))
                {
                    return new Result<List<GoodsOutPut>>(ResultCode.Expired, new List<GoodsOutPut>());
                }
                var key = userid + "_" + categoryNo + "_" + type;
                result = _cacheManager.GetGoodsCache(key);
                if (result == null)
                {
                    result = new List<GoodsOutPut>();
                }
                else
                {
                    return Result.FromData(result);
                }
                Logger.Info("开始获取商品信息。storeno:" + userinfo.StorageNo + ",terminalid:" + userinfo.TerminalID + ",type:" + type + ",category:" + category);
                //0 代表自定义
                if (type == 0)
                {
                    query = ApplicationConsts.QueryCustomerGoodsList;
                    object obj = new
                    {
                        storageno = userinfo.StorageNo,
                        terminalid = userinfo.TerminalID,
                    };
                    var goodList = await _context.ExecuteFunctionAsync<List<GoodsDTO>>(query, obj);
                    result = goodList.MapTo<List<GoodsOutPut>>();
                }
                else
                {
                    query = ApplicationConsts.QueryGoodsListByCategory;
                    object obj = new
                    {
                        storageno = userinfo.StorageNo,
                        terminalid = userinfo.TerminalID,
                        categoy = category,
                    };
                    var goodList = await _context.ExecuteFunctionAsync<List<GoodsDTO>>(query, obj);
                    result = goodList.MapTo<List<GoodsOutPut>>();
                }
                _cacheManager.SetGoodsCache(key, result);
            }
            catch (Exception ex)
            {
                Logger.Error("通过分类或者类型获取商品失败", ex);
            }
            return Result.FromData(result);
        }
        public async Task<Result<List<GoodsOutPut>>> GetHotGoodsList()
        {
            var result = new List<GoodsOutPut>();
            try
            {
                Logger.Info("开始获取热门商品");
                result = _cacheManager.GetGoodsCache(FishSession.UserId);
                if (result == null)
                {
                    result = new List<GoodsOutPut>();
                }
                else
                {
                    return Result.FromData(result);
                }
                var query = ApplicationConsts.QueryHotGoodsList;
                var goodList = await _context.ExecuteFunctionAsync<List<GoodsDTO>>(query, new object());
                result = goodList.MapTo<List<GoodsOutPut>>();
                _cacheManager.SetGoodsCache(FishSession.UserId, result);
            }
            catch (Exception ex)
            {
                Logger.Error("获取热门商品失败", ex);
            }
            return Result.FromData(result);
        }

        public async Task<Result<List<GoodsSearchResult>>> GetGoodsSearchResult(string key)
        {
            var result = new Result<List<GoodsSearchResult>>(ResultCode.Ok, new List<GoodsSearchResult>());
            var query = ApplicationConsts.QueryGoodsByKey;
            try
            {
                Logger.Info("开始搜索商品，key=" + key);
                string userid = FishSession.UserId;
                var userinfo = _cacheManager.GetUserDataCacheByUserId(userid);
                var defaultResult = GainDefaultResult<List<GoodsSearchResult>>(userinfo);
                if (defaultResult.Code == ResultCode.Expired)
                {
                    return defaultResult;
                }
                var obj = new
                {
                    storageno = userinfo.StorageNo,
                    terminalid = userinfo.TerminalID,
                    Key = "",
                };
                if (!String.IsNullOrWhiteSpace(key))
                {
                    if (long.TryParse(key.Trim(), out long barCode))
                    {
                        query += " and a.sMainBarcode like @Key";
                    }
                    else
                    {
                        query += " and a.sGoodsName like @Key";
                    }
                    key = "%" + key + "%";
                    obj = new
                    {
                        storageno = userinfo.StorageNo,
                        terminalid = userinfo.TerminalID,
                        Key = key,
                    };
                }
                var goodsResult = await _context.ExecuteFunctionAsync<List<GoodsSearchResult>>(query, obj);
                result = Result.FromData(goodsResult);
            }
            catch (Exception ex)
            {
                Logger.Error("商品搜索失败", ex);
                result = Result.Fail<List<GoodsSearchResult>>(ex.Message);
            }
            return result;
        }
    }
}
