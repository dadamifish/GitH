using Mi.Fish.Application.SaleDetail;
using Mi.Fish.ApplicationDto;
using Mi.Fish.Infrastructure.Results;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace Mi.Fish.Api.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    public class SaleDetailController : FishControllerBase
    {
        private readonly ISaleDetailAppService _saleDetailAppService;

        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="saleDetailAppService"></param>
        public SaleDetailController(ISaleDetailAppService saleDetailAppService)
        {
            _saleDetailAppService = saleDetailAppService;
        }

        /// <summary>
        /// 获取收银员当前班次销售明细
        /// </summary>
        /// <param name="input">the input</param>
        /// <returns></returns>
        [HttpGet("Cashier")]
        [ProducesResponseType(typeof(List<GetSaleDetailOutput>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Result), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(Result), StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetCashierSaleDetail([FromQuery]GetSaleDetailInput input)
        {
            var result = await _saleDetailAppService.GetCashierSaleDetail(input);

            if (result.IsSuccess)
            {
                return Ok(result.Data);
            }

            if (result.Code == ResultCode.Unauthorized)
            {
                return Unauthorized();
            }

            return BadRequest(result.BaseResult());
        }

        /// <summary>
        /// 获取SaleType枚举值
        /// </summary>
        /// <returns></returns>
        [HttpGet("SaleType")]
        [ProducesResponseType(typeof(EnumResult<SaleType>), StatusCodes.Status200OK)]
        public IActionResult GetSaleType()
        {
            return Ok(new EnumResult<SaleType>());
        }
    }
}
