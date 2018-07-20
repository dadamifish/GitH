using Mi.Fish.Application;
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
    public class OrderController : FishControllerBase
    {
        private readonly IOrderAppService _orderAppService;

        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="orderAppService"></param>
        public OrderController(IOrderAppService orderAppService)
        {
            _orderAppService = orderAppService;
        }

        /// <summary>
        /// 获取自助点餐订单报表
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpGet("OrderReport")]
        [ProducesResponseType(typeof(List<GetOrderReportDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Result), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetOrderReport([FromQuery]GetOrderReportInput input)
        {
            var result = await _orderAppService.GetOrderReport(input);

            if (result.IsSuccess)
            {
                return Ok(result.Data);
            }

            return BadRequest(result.BaseResult());
        }

        /// <summary>
        /// 获取OrderReportStatus枚举值
        /// </summary>
        /// <returns></returns>
        [HttpGet("OrderReportStatus")]
        [ProducesResponseType(typeof(EnumResult<OrderReportStatus>), StatusCodes.Status200OK)]
        public IActionResult GetOrderReportStatus()
        {
            return Ok(new EnumResult<OrderReportStatus>());
        }

        /// <summary>
        /// 获取最新自助点餐订单
        /// </summary>
        /// <returns></returns>
        [HttpGet("NewOrder")]
        [ProducesResponseType(typeof(List<GetNewOrderDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Result), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetNewOrder()
        {
            var result = await _orderAppService.GetNewOrder();

            if (result.IsSuccess)
            {
                return Ok(result.Data);
            }

            return BadRequest(result.BaseResult());
        }
    }
}
