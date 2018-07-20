using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Mi.Fish.Application;
using Mi.Fish.ApplicationDto;
using Mi.Fish.Infrastructure.Results;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Mi.Fish.Api.Controllers
{
    /// <summary>
    /// 订单获取
    /// </summary>
    public class OrderDetailController : FishControllerBase
    {
        private readonly IOrderDetailService _orderDetailService;
        /// <summary>
        /// octr
        /// </summary>
        /// <param name="orderDetailService"></param>
        public OrderDetailController(IOrderDetailService orderDetailService)
        {
            this._orderDetailService = orderDetailService;
        }

        /// <summary>
        /// 获取订单详情
        /// </summary>
        /// <param name="orderNo">订单号</param>
        /// <returns></returns>
        [HttpGet("OrderDetail/{orderNo}")]
        [ProducesResponseType(typeof(OrderDetailOutput), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Result), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GainOrderDetail(string orderNo)
        {
            var result = await _orderDetailService.GetOrderDetail(orderNo);
            if (result.IsSuccess)
            {
                return Ok(result.Data);
            }
            return BadRequest(result.BaseResult());
        }


        /// <summary>
        /// 获取最近十笔已完成单号
        /// </summary>
        /// <returns></returns>
        /// <response code="200">请求成功</response>
        /// <response code="400">请求失败</response>
        [HttpGet("LastSaleList")]
        [ProducesResponseType(typeof(List<OrderSaleListOutPut>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Result), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetLastSaleList()
        {
            var result = await _orderDetailService.GetLastSaleList();
            if (result.IsSuccess)
            {
                return Ok(result.Data);
            }
            return BadRequest(result.BaseResult());
        }

        /// <summary>
        /// 整单退单
        /// </summary>
        /// <param name="orderNo">订单号</param>
        /// <returns></returns>
        [HttpPost("OrderRefund/{orderNo}")]
        [ProducesResponseType(typeof(BaseOrderRefundOutput), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Result), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> OrderRefund(string orderNo)
        {
            var ip = GetUserIp();
            var result = await _orderDetailService.OrderRefund(orderNo, ip);
            if (result.IsSuccess)
            {
                return Ok(result);
            }
            return BadRequest(result.BaseResult());
        }

        /// <summary>
        /// 订单检查是否可直接退款，IsRefund=true可直接退款，false需要确认后强制退款
        /// </summary>
        /// <param name="orderNo">订单号</param>
        /// <returns></returns>
        [HttpPost("OrderRefundCheck/{orderNo}")]
        [ProducesResponseType(typeof(OrderRefundOutput), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Result), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> OrderRefundCheck(string orderNo)
        {
            var result = await _orderDetailService.OrderRefundCheck(orderNo);
            if (result.IsSuccess)
            {
                return Ok(result);
            }
            return BadRequest(result.BaseResult());
        }
    }
}