using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Abp.Runtime.Caching;
using Castle.Core.Logging;
using Mi.Fish.ApplicationDto;
using Mi.Fish.EntityFramework;
using Mi.Fish.Infrastructure;
using Mi.Fish.Infrastructure.Results;
using Newtonsoft.Json;

namespace Mi.Fish.Application.AppGoods
{
    public class AppGoodsAppService : AppServiceBase, IAppGoodsAppService
    {
        private readonly LocalDbContext _localDbContext;
        private readonly ICacheManager _cacheManager;
        public AppGoodsAppService(LocalDbContext localDbContext,ICacheManager cacheManager)
        {
            _localDbContext = localDbContext;
            _cacheManager = cacheManager;
        }

        /// <summary>
        /// 获取app已添加数据
        /// </summary>
        /// <returns></returns>
        public async Task<Result<List<MealListOutputDto>>> GetAppGoodsAsync()
        {
            string userId = FishSession.UserId;
            var userData = _cacheManager.GetUserDataCacheByUserId(userId);
            string storeId = userData.StorageNo;
            string token = await Common.GetAppTokenAsync(userData.AppLink, userData.LeyouAccount, userData.LeyouPwd);
            string url = userData.AppLink + $"api/DinnerSystemSynchronization/QuerySynDiningMeal?storeId={storeId}&token={token}";

            using (HttpClient http = new HttpClient())
            {
                var response = await http.GetAsync(url);
                string json = await response.Content.ReadAsStringAsync();
                AppGoodsOutputDto app = JsonConvert.DeserializeObject<AppGoodsOutputDto>(json);
                List<MealListOutputDto> list = AutoMapper.Mapper.Map<List<MealListOutputDto>>(app.Data.MealList);
                return Result.FromData(list);
            }
        }
        /// <summary>
        /// 获取Fish系统未上传到app的商品
        /// </summary>
        /// <returns></returns>
        public async Task<Result<List<MealListOutputDto>>> GetFishGoodsAsync()
        {
            string userId = FishSession.UserId;
            var userData = _cacheManager.GetUserDataCacheByUserId(userId);
            string storeId = userData.StorageNo;        //门店编码
            string terminalID = userData.TerminalID;              //机号

            Result<List<MealListOutputDto>> appMealList = await GetAppGoodsAsync();
            string sql = " select branch_no as StoreId,item_no as MealId,sGoodsName as MealName,nSalePrice as Price " +
                " from dbo.base_entry_message_allow a inner join ft_v_salegoods b on a.item_no=b.sGoodsNO " +
                " where branch_no=@storeId and jh=@terminalID and activeflag=1 ";
            object obj = new { storeId, terminalID };
            List<MealListOutputDto> posMealList = await _localDbContext.ExecuteFunctionAsync<List<MealListOutputDto>>(sql, obj);

            if (appMealList != null && appMealList.Data.Count > 0)
            {
                return Result.FromData(posMealList.Except<MealListOutputDto>(appMealList.Data, new MealListOutputDtoEquality()).ToList());
            }
            else
            {
                return Result.FromData(posMealList);
            }
        }
        /// <summary>
        /// 向app 添加商品
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<Result> CreateAppGoodsAsync(AddAppGoodsInputDto input)
        {
            string userId = FishSession.UserId;
            var userData = _cacheManager.GetUserDataCacheByUserId(userId);
            string storeId = userData.StorageNo;
            string token = await Common.GetAppTokenAsync(userData.AppLink, userData.LeyouAccount, userData.LeyouPwd);
            string url = userData.AppLink + "api/DinnerSystemSynchronization/SynDiningMeal";
            AddAppGoodsToAppInputDto appInput = AutoMapper.Mapper.Map<AddAppGoodsToAppInputDto>(input);
            appInput.StoreId = storeId;

            using (HttpClient http = new HttpClient())
            {
                string data = JsonConvert.SerializeObject(appInput);
                http.DefaultRequestHeaders.Add("token", token);
                StringContent content = new StringContent(data, Encoding.UTF8, "application/json");
                var response = await http.PostAsync(url, content);
                string result = await response.Content.ReadAsStringAsync();

                AppResult<string> appResult = JsonConvert.DeserializeObject<AppResult<string>>(result);
                if (appResult.Result == "0")
                {
                    return new Result(ResultCode.Ok);
                }
                else
                {
                    return new Result(ResultCode.Fail);
                }
            }
        }
        /// <summary>
        /// 删除app商品
        /// </summary>
        /// <param name="mealId">商品编码</param>
        /// <returns></returns>
        public async Task<Result> DeleteAppGoodsAsync(string mealId)
        {
            string userId = FishSession.UserId;
            var userData = _cacheManager.GetUserDataCacheByUserId(userId);
            string storeId = userData.StorageNo;
            string url = userData.AppLink + "api/DinnerSystemSynchronization/DelSynDiningMeal";
            string token = await Common.GetAppTokenAsync(userData.AppLink, userData.LeyouAccount, userData.LeyouPwd);
            string data = string.Concat("{storeId:\"", storeId, "\",\"mealId\":\"", mealId, "\"}");

            using (HttpClient http = new HttpClient())
            {
                http.DefaultRequestHeaders.Add("token", token);
                StringContent content = new StringContent(data, Encoding.UTF8, "application/json");
                var response = await http.PostAsync(url, content);
                string result = await response.Content.ReadAsStringAsync();

                AppResult<string> appresult = JsonConvert.DeserializeObject<AppResult<string>>(result);
                if (appresult.Result == "0")
                {
                    return new Result(ResultCode.Ok);
                }
                else
                {
                    return new Result(ResultCode.Fail);
                }
            }
        }
    }
}
