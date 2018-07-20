using Mi.Fish.Application.Leyou;
using Mi.Fish.ApplicationDto;
using Mi.Fish.Infrastructure.Results;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Mi.Fish.Api.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    public class LeyouController : FishControllerBase
    {
        private readonly ILeyouAppService _leyouAppService;

        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="leyouAppService"></param>
        public LeyouController(ILeyouAppService leyouAppService)
        {
            _leyouAppService = leyouAppService;
        }

        /// <summary>
        /// 是否有新的乐游订单
        /// </summary>
        /// <returns></returns>
        [HttpGet("IsNewOrder")]
        [ProducesResponseType(typeof(IsNewOrderDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Result), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> IsNewOrder()
        {
            var result = await _leyouAppService.IsNewOrder();

            if (result.IsSuccess)
            {
                var dto = new IsNewOrderDto();
                if (result.Data)
                {
                    dto.Flag = true;
                }
                return Ok(dto);
            }

            return BadRequest(result.BaseResult());
        }

        /// <summary>
        /// 获取上传到乐游APP的商品
        /// </summary>
        /// <returns></returns>
        [HttpGet("AppGoods")]
        [ProducesResponseType(typeof(List<GetAppGoodsDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Result), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetAppGoods()
        {
            var result = await _leyouAppService.GetAppGoods();

            if (result.IsSuccess)
            {
                return Ok(result.Data);
            }

            return BadRequest(result.BaseResult());
        }

        /// <summary>
        /// 获取POS机未上传到乐游APP的商品
        /// </summary>
        /// <returns></returns>
        [HttpGet("FishGoods")]
        [ProducesResponseType(typeof(List<GetFishGoodsDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Result), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetFishGoods()
        {
            var result = await _leyouAppService.GetFishGoods();

            if (result.IsSuccess)
            {
                return Ok(result.Data);
            }

            return BadRequest(result.BaseResult());
        }

        /// <summary>
        /// 添加商品到乐游APP
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost("AppGoods")]
        [ProducesResponseType(typeof(Result), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(Result), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> AddAppGoods([FromBody] AddAppGoodsInput input)
        {
            var result = await _leyouAppService.AddAppGoods(input);

            if (result.IsSuccess)
            {
                return CreatedAtAction(null, null);
            }

            return BadRequest(result.BaseResult());
        }

        /// <summary>
        /// 从乐游APP中删除商品
        /// </summary>
        /// <param name="goodsNo">商品编号</param>
        /// <returns></returns>
        [HttpDelete("AppGoods/{goodsNo}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(Result), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> DelAppGoods(string goodsNo)
        {
            var result = await _leyouAppService.DelAppGoods(goodsNo);

            if (result.IsSuccess)
            {
                return NoContent();
            }

            return BadRequest(result.BaseResult());
        }
    }
}
