using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Mi.Fish.Application.AppGoods;
using Mi.Fish.ApplicationDto;
using Mi.Fish.Infrastructure.Results;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Mi.Fish.Api.Controllers
{
    /// <summary>
    /// App商品对接接口
    /// </summary>
    public class AppGoodsController : FishControllerBase
    {
        private readonly IAppGoodsAppService _appGoodsAppService;
        public AppGoodsController(IAppGoodsAppService appGoodsAppService)
        {
            _appGoodsAppService = appGoodsAppService;
        }

        /// <summary>
        /// 获取app已添加商品
        /// </summary>
        /// <returns></returns>
        [HttpGet("GetAppGoods")]
        [ProducesResponseType(typeof(List<MealListOutputDto>),StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Result),StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetAppGoods()
        {
            Result<List<MealListOutputDto>> result = await _appGoodsAppService.GetAppGoodsAsync();
            if (result.IsSuccess)
            {
                return Ok(result.Data);
            }
            else
            {
                return BadRequest(result.BaseResult());
            }
        }

        /// <summary>
        /// 获取POS系统 未上传到app的商品
        /// </summary>
        /// <returns></returns>
        [HttpGet("GetFishGoods")]
        [ProducesResponseType(typeof(List<MealListOutputDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Result), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult>GetFishGoods()
        {
            Result<List<MealListOutputDto>> result = await _appGoodsAppService.GetFishGoodsAsync();
            if (result.IsSuccess)
            {
                return Ok(result.Data);
            }
            else
            {
                return BadRequest(result.BaseResult());
            }
        }

        /// <summary>
        /// 向app中添加商品
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpFisht]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(Result), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateAppGoods([FromBody]AddAppGoodsInputDto input)
        {
            Result result = await _appGoodsAppService.CreateAppGoodsAsync(input);
            if (result.IsSuccess)
            {
                return CreatedAtAction(nameof(GetAppGoods), null);
                //return Ok();
            }
            else
            {
                return BadRequest(result.BaseResult());
            }
        }

        /// <summary>
        ///  删除app已添加的商品
        /// </summary>
        /// <param name="mealId">商品编码</param>
        /// <returns></returns>
        [HttpDelete]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(Result), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> DeleteAppGoods(string mealId)
        {
            Result result = await _appGoodsAppService.DeleteAppGoodsAsync(mealId);
            if (result.IsSuccess)
            {
                return NoContent();
            }
            else
            {
                return BadRequest(result.BaseResult());
            }
        }
    }
}