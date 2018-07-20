namespace Mi.Fish.Api.Controllers
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Mi.Fish.Application.AppOrders;
    using Mi.Fish.ApplicationDto;
    using Mi.Fish.Infrastructure.Results;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;

    /// <summary>
    /// App订单对接
    /// </summary>
    public class AppOrdersController : FishControllerBase
    {
        private readonly IAppOrdersAppService _appOrdersAppService;
        public AppOrdersController(IAppOrdersAppService appOrdersAppService)
        {
            _appOrdersAppService = appOrdersAppService;
        }
        /// <summary>
        /// App订单查询，订单状态选项
        /// </summary>
        /// <returns></returns>
        [HttpGet("appStatus")]
        [ProducesResponseType(typeof(EnumResult<AppOrderStatus>), StatusCodes.Status200OK)]
        public IActionResult GetAppStatus()
        {
            return Ok(new EnumResult<AppOrderStatus>(AppOrderStatus.Paid, AppOrderStatus.Taked, AppOrderStatus.DiningComplete, AppOrderStatus.Finished));
        }

        /// <summary>
        /// 获取App订单
        /// </summary>
        /// <param name="status">订单状态</param>
        /// <returns></returns>
        [HttpGet("orderStatus/{status}")]
        [ProducesResponseType(typeof(List<FishOrdersOutputDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Result), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetAppOrders(AppOrderStatus status)
        {
            if (status == AppOrderStatus.WaitTake)
            {
                return BadRequest(new Result(ResultCode.ParameterFailed,"参数错误"));
            }
            Result<List<FishOrdersOutputDto>> result = await _appOrdersAppService.GetAppOrdersAsync(status);
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
        /// App订单转Fish单
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(typeof(CreateOrderNoDto), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(Result), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateFishOrder([FromBody]AppOrderInputDto input)
        {
            Result<CreateOrderNoDto> result = await _appOrdersAppService.CreateFishOrdersAsync(input);
            if (result.IsSuccess)
            {
                return CreatedAtAction(null, result.Data);
            }
            else
            {
                return BadRequest(result.BaseResult());
            }
        }
        /// <summary>
        /// App订单，退单接口
        /// </summary>
        /// <param name="appOrderNo">App订单号</param>
        /// <returns></returns>
        [HttpDelete("{appOrderNo}")]
        [ProducesResponseType(typeof(CreateOrderNoDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Result), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> DeleteFishOrder(string appOrderNo)
        {
            Result<CreateOrderNoDto> result = await _appOrdersAppService.DeleteOrdersAsync(appOrderNo);
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
        /// 获取订单打印信息
        /// </summary>
        /// <param name="orderNo">销售单号</param>
        /// <returns></returns>
        [HttpGet("{orderNo}/printInfo")]//"{orderNo}/printInfo/{isRePrint?}"
        [ProducesResponseType(typeof(OrdersPrintDto),StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Result), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetPrintInfo(string orderNo)
        {
            Result<OrdersPrintDto> result = await _appOrdersAppService.PrintInfoAsync(orderNo, 0);
            if(result.IsSuccess)
            {
                return Ok(result.Data);
            }
            else
            {
                return BadRequest(result.BaseResult());
            }
        }
    }
}