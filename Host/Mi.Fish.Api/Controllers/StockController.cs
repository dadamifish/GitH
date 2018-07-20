using Mi.Fish.Application.Stock;
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
    public class StockController : FishControllerBase
    {
        private readonly IStockAppService _stockAppService;

        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="stockAppService"></param>
        public StockController(IStockAppService stockAppService)
        {
            _stockAppService = stockAppService;
        }

        /// <summary>
        /// 获取门店库存
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpGet("StoreStock")]
        [ProducesResponseType(typeof(List<GetStoreStockDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Result), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetStoreStock([FromQuery] GetStoreStockInput input)
        {
            var result = await _stockAppService.GetStoreStock(input);

            if (result.IsSuccess)
            {
                return Ok(result.Data);
            }

            return BadRequest(result.BaseResult());
        }
    }
}
