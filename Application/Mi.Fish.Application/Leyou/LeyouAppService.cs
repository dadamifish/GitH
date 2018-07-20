using Abp.AutoMapper;
using Abp.Runtime.Caching;
using Mi.Fish.ApplicationDto;
using Mi.Fish.Common;
using Mi.Fish.EntityFramework;
using Mi.Fish.Infrastructure.Leyou;
using Mi.Fish.Infrastructure.Results;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Mi.Fish.Application.Leyou
{
    /// <summary>
    /// 乐游应用服务
    /// </summary>
    public class LeyouAppService : AppServiceBase, ILeyouAppService
    {
        private readonly ICacheManager _cacheManager;
        private readonly LocalDbContext _localDbContext;
        private readonly LeyouSetting _leyouSetting;

        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="localDbContext"></param>
        /// <param name="cacheManager"></param>
        /// <param name="leyouSetting"></param>
        public LeyouAppService(LocalDbContext localDbContext, ICacheManager cacheManager, IOptions<LeyouSetting> leyouSetting)
        {
            _localDbContext = localDbContext;
            _cacheManager = cacheManager;
            _leyouSetting = leyouSetting.Value;
        }

        #region private function

        /// <summary>
        /// 请求乐游Token接口
        /// </summary>
        /// <returns></returns>
        private async Task<string> GetLeyouToken()
        {
            var cache = _cacheManager.GetLeyouTokenCache();
            //乐游Token有效时间为7天
            if (cache != null)
            {
                var ts = cache.ExpirationTime - DateTime.Now;
                //提前1小时失效token
                if (ts.TotalHours > 1)
                {
                    return cache.Token;
                }
            }
            var input = new PostTokenInput();
            input.EncryPsw = _leyouSetting.Password;
            input.UserName = _leyouSetting.Account;
            var result = await HttpClientExtensions.PostAsync<PostTokenDto>(_leyouSetting.AppLink + LeyouApi.PostToken, input, false);
            if (result != null && result.Result == "0")
            {
                var tokenCache = new LeyouTokenCache();
                tokenCache.Token = result.Data.Token;
                tokenCache.ExpirationTime = DateTime.Now.AddDays(7);
                _cacheManager.SetLeyouTokenCache(tokenCache);
                return result.Data.Token;
            }
            return null;
        }

        /// <summary>
        /// 请求乐游订单接口
        /// </summary>
        /// <param name="status">状态码</param>
        /// <param name="token">token令牌</param>
        /// <returns></returns>
        private async Task<GetLeyouAppOrderDto> GetLeyouAppOrder(int status, string token)
        {
            var input = new GetLeyouAppOrderInput();
            input.OrderBeginDate = DateTime.Now.ToString("yyyy-MM-dd");
            input.OrderEndDate = DateTime.Now.ToString("yyyy-MM-dd");
            input.PageIndex = 0;
            input.PageSize = 10000;
            input.Status = status;
            input.StoreId = FishSession.StorageNo;
            input.Token = token;
            return await HttpClientExtensions.GetAsync<GetLeyouAppOrderDto>(_leyouSetting.AppLink + LeyouApi.GetAppOrder, input);
        }

        /// <summary>
        /// 从本地数据库中获取商品
        /// </summary>
        /// <returns></returns>
        private async Task<List<GetFishGoodsDto>> GetFishGoodsFromDb()
        {
            var input = new GetFishGoodsInput();
            input.StoreNo = FishSession.StorageNo;
            input.TerminalNo = FishSession.TerminalId;
            var sql = @"select StoreNo = branch_no,GoodsNo = item_no,GoodsName = sGoodsName,SalePrice = nSalePrice
from base_entry_message_allow t1
inner
join ft_v_salegoods t2 on t1.item_no = t2.sGoodsNO
where activeflag = 1 and branch_no = @StoreNo and jh = @TerminalNo";
            return await _localDbContext.ExecuteFunctionAsync<List<GetFishGoodsDto>>(sql, input);
        }

        #endregion

        #region pulic function

        /// <summary>
        /// 是否有新的订单
        /// </summary>
        /// <returns></returns>
        public async Task<Result<bool>> IsNewOrder()
        {
            //新的订单只需提示一次
            try
            {
                var token = await GetLeyouToken();

                //因乐游获取所有订单需要两个状态码（1和9），所以需请求两次接口
                var result1 = await GetLeyouAppOrder(1, token);
                var result9 = await GetLeyouAppOrder(9, token);

                var result = false;

                var count = result1.Data.List.Length + result9.Data.List.Length;

                if (count > 0)
                {
                    var maxId1 = result1.Data?.List.Max(w => w.Id) ?? "";
                    var maxId9 = result9.Data?.List.Max(w => w.Id) ?? "";

                    var cache = _cacheManager.GetLeYouNewOrderCache();
                    if (cache == null)
                    {
                        cache = new LeYouNewOrderCache();
                        cache.MaxOrderId = maxId1 + "-" + maxId9;
                        _cacheManager.SetLeYouNewOrderCache(cache);

                        result = true;
                    }
                    else
                    {
                        var ids = cache.MaxOrderId.Split('-');
                        if (ids[0] != maxId1 || ids[1] != maxId9)
                        {
                            cache.MaxOrderId = maxId1 + "-" + maxId9;
                            _cacheManager.SetLeYouNewOrderCache(cache);

                            result = true;
                        }
                    }
                }

                return Result.FromData(result);
            }
            catch (Exception exc)
            {
                return Result.Fail<bool>(exc.Message);
            }
        }

        /// <summary>
        /// 获取上传到乐游APP的商品
        /// </summary>
        /// <returns></returns>
        public async Task<Result<List<GetAppGoodsDto>>> GetAppGoods()
        {
            try
            {
                var input = new GetAppGoodsInput();
                input.StoreId = FishSession.StorageNo;
                input.Token = await GetLeyouToken();
                var list = await HttpClientExtensions.GetAsync<GetAppGoodsOutput>(_leyouSetting.AppLink + LeyouApi.GetAppGoods, input);
                var appList = list.Data.MealList.MapTo<List<GetAppGoodsDto>>();
                var posDbList = await GetFishGoodsFromDb();
                var posList = posDbList.MapTo<List<GetAppGoodsDto>>();
                //获取乐游APP可售而本地POS系统不可售商品
                var goodsNotFishList = appList.Except(posList, new GetAppGoodsDtoComparer()).ToList();
                var goods = appList.Except(goodsNotFishList, new GetAppGoodsDtoComparer()).ToList();
                goods.ForEach(w => w.FishCanSale = true);
                var result = goods.Concat(goodsNotFishList).Distinct().OrderBy(w => w.GoodsNo).ToList();
                return Result.FromData(result);
            }
            catch (Exception exc)
            {
                return Result.Fail<List<GetAppGoodsDto>>(exc.Message);
            }
        }

        /// <summary>
        /// 获取POS机未上传到乐游APP的商品
        /// </summary>
        /// <returns></returns>
        public async Task<Result<List<GetFishGoodsDto>>> GetFishGoods()
        {
            try
            {
                var appGoods = await GetAppGoods();
                var appList = appGoods.Data.MapTo<List<GetFishGoodsDto>>();
                var posList = await GetFishGoodsFromDb();
                var result = posList.Except(appList, new GetFishGoodsDtoComparer()).Distinct().ToList();
                return Result.FromData(result);
            }
            catch (Exception exc)
            {
                return Result.Fail<List<GetFishGoodsDto>>(exc.Message);
            }
        }

        /// <summary>
        /// 添加商品到乐游APP
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<Result> AddAppGoods(AddAppGoodsInput input)
        {
            try
            {
                var token = await GetLeyouToken();
                var dict = new Dictionary<string, string>();
                dict.Add("token", token);
                var apiInput = input.MapTo<AddAppGoodsApiInput>();
                apiInput.StoreId = FishSession.StorageNo;

                var result = await HttpClientExtensions.PostAsync<LeyouResult<object>>(_leyouSetting.AppLink + LeyouApi.AddAppGoods, apiInput, true, dict);
                if (result.Result == "0")
                {
                    return Result.Ok;
                }
                return Result.Fail<GetAppGoodsDto>(result.Message);
            }
            catch (Exception exc)
            {
                return Result.Fail<GetAppGoodsDto>(exc.Message);
            }
        }

        /// <summary>
        /// 从乐游APP中删除商品
        /// </summary>
        /// <param name="goodsNo"></param>
        /// <returns></returns>
        public async Task<Result> DelAppGoods(string goodsNo)
        {
            try
            {
                var token = await GetLeyouToken();
                var dict = new Dictionary<string, string>();
                dict.Add("token", token);
                var apiInput = new DelAppGoodsApiInput();
                apiInput.MealId = goodsNo.Trim();
                apiInput.StoreId = FishSession.StorageNo;

                var result = await HttpClientExtensions.PostAsync<LeyouResult<object>>(_leyouSetting.AppLink + LeyouApi.DelAppGoods, apiInput, true, dict);
                if (result.Result == "0")
                {
                    return Result.Ok;
                }
                return new Result(ResultCode.Fail, result.Message);
            }
            catch (Exception exc)
            {
                return new Result(ResultCode.Fail, exc.Message);
            }
        }

        #endregion
    }
}
