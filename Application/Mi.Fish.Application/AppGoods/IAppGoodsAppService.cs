using Abp.Application.Services;
using Mi.Fish.ApplicationDto;
using Mi.Fish.Infrastructure.Results;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Mi.Fish.Application.AppGoods
{
    public interface IAppGoodsAppService:IApplicationService
    {
        /// <summary>
        /// 获取app已添加商品
        /// </summary>
        /// <returns></returns>
        Task<Result<List<MealListOutputDto>>> GetAppGoodsAsync();
        /// <summary>
        /// 向app中添加商品
        /// </summary>
        /// <param name="input"></param>

        /// <returns></returns>
        Task<Result> CreateAppGoodsAsync(AddAppGoodsInputDto input);
        /// <summary>
        /// 获取pos系统未上传到app的商品
        /// </summary>
        /// <returns></returns>
        Task<Result<List<MealListOutputDto>>> GetFishGoodsAsync();
        /// <summary>
        /// 删除app商品
        /// </summary>
        /// <param name="mealId">商品Id</param>
        /// <returns></returns>
        Task<Result> DeleteAppGoodsAsync(string mealId);
    }
}
