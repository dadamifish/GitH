namespace Mi.Fish.Api.Controllers
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Mi.Fish.Application;
    using Mi.Fish.ApplicationDto;
    using Mi.Fish.Infrastructure.Results;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;

    /// <summary>
    /// 商品管理列表
    /// </summary>
    /// <seealso cref="FishControllerBase" />
    public class GoodsController : FishControllerBase
    {
        private readonly IGoodsAppService _GoodsService;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="goodsService"></param>
        public GoodsController(IGoodsAppService goodsService)
        {
            this._GoodsService = goodsService;
        }

        /// <summary>
        /// 获取自定义商品列表
        /// </summary>
        /// <returns></returns>
        [HttpGet("customerGoods")]
        [ProducesResponseType(typeof(List<GoodsOutPut>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Result), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GainCustomerGoodsList()
        {
            var result = await _GoodsService.GetGoodsList(string.Empty, 0);
            if (result.IsSuccess)
            {
                return Ok(result.Data);
            }
            return BadRequest(result.BaseResult());
        }

        /// <summary>
        /// 获取分类下商品列表
        /// </summary>
        /// <param name="categoryNo">三级分类</param>
        /// <returns></returns>
        [HttpGet("{categoryNo}/goods")]
        [ProducesResponseType(typeof(List<GoodsOutPut>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Result), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GainGoodsList(string categoryNo)
        {
            var result = await _GoodsService.GetGoodsList(categoryNo, 1);
            if (result.IsSuccess)
            {
                return Ok(result.Data);
            }
            return BadRequest(result.BaseResult());
        }

        /// <summary>
        /// 获取热门商品
        /// </summary>
        /// <returns></returns>
        [HttpGet("hotGoods")]
        [ProducesResponseType(typeof(List<GoodsOutPut>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Result), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GainHotGoodsList()
        {
            var result = await _GoodsService.GetHotGoodsList();
            if (result.IsSuccess)
            {
                return Ok(result.Data);
            }
            return BadRequest(result.BaseResult());
        }
        /// <summary>
        /// 搜索商品
        /// </summary>
        /// <param name="key">搜索关键字</param>
        /// <returns></returns>
        [HttpGet("search")]
        [ProducesResponseType(typeof(List<GoodsSearchResult>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Result), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> SearchGoodsResult([FromQuery]string key)
        {
            var result = await _GoodsService.GetGoodsSearchResult(key);
            if (result.IsSuccess)
            {
                return Ok(result.Data);
            }
            return BadRequest(result.BaseResult());
        }
    }
}