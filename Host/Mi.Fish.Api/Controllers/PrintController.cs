using Mi.Fish.Application.Print;
using Mi.Fish.ApplicationDto;
using Mi.Fish.Infrastructure.Results;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Abp.AutoMapper;

namespace Mi.Fish.Api.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    public class PrintController : FishControllerBase
    {
        private readonly IPrintAppService _printAppService;

        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="printAppService"></param>
        public PrintController(IPrintAppService printAppService)
        {
            _printAppService = printAppService;
        }

        /// <summary>
        /// 订单重打印
        /// </summary>
        /// <returns></returns>
        [HttpGet("RePrint/{orderNo}")]
        [ProducesResponseType(typeof(OrdersPrintDto), 200)]
        [ProducesResponseType(typeof(Result), 400)]
        public async Task<IActionResult> RePrint([FromRoute]string orderNo)
        {
            var input = new RePrintInput();
            input.OrderNo = orderNo;

            var result = await _printAppService.RePrint(input);

            if (result.IsSuccess)
            {
                var data = result.Data.MapTo<OrdersPrintDto>();
                return Ok(data);
            }

            return BadRequest(result.BaseResult());
        }
    }
}
