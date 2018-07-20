using System;
using Mi.Fish.Application.Order;
using Mi.Fish.Application.SaleMenu;
using Mi.Fish.ApplicationDto;
using Mi.Fish.Infrastructure.Results;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using Abp.AspNetCore.Mvc.Controllers;
using AutoMapper.Configuration.Conventions;

namespace Mi.Fish.Api.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    public class SaleMenuController : FishControllerBase
    {
        private readonly ISaleMenuAppService _saleMenuAppService;
        private readonly IOrderNoAppService _orderNoAppService;

        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="saleMenuAppService"></param>
        /// <param name="orderNoAppService"></param>
        public SaleMenuController(ISaleMenuAppService saleMenuAppService, IOrderNoAppService orderNoAppService)
        {
            _saleMenuAppService = saleMenuAppService;
            _orderNoAppService = orderNoAppService;
        }

        /// <summary>
        /// 获取最新订单编号
        /// </summary>
        /// <returns></returns>
        [HttpGet("OrderNo")]
        [ProducesResponseType(typeof(CreateOrderNoDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Result), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateOrderNo()
        {
            var result = await _orderNoAppService.CreateOrderNo();

            if (result.IsSuccess)
            {
                var dto = new CreateOrderNoDto();
                dto.OrderNo = result.Data;
                return CreatedAtAction(null, dto);
            }

            return BadRequest(result.BaseResult());
        }


        /// <summary>
        /// 获取当前购物车数据
        /// </summary>
        /// <returns></returns>
        [HttpGet("Meun")]
        [ProducesResponseType(typeof(SaleMenuDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Result), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetMeun()
        {
            var result = await _saleMenuAppService.GetMenu();

            if (result.IsSuccess)
            {
                return Ok(result.Data);
            }

            return BadRequest(result.BaseResult());
        }

        /// <summary>
        /// 新增商品栏
        /// </summary>
        /// <param name="goodsNo"></param>
        /// <returns></returns>
        [HttpPut("MenuItem/{goodsNo}")]
        [ProducesResponseType(typeof(SaleMenuDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Result), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> AddGoods(string goodsNo)
        {
            //System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();
            //stopwatch.Start();

            var result = await _saleMenuAppService.AddGoods(new SaleMenuInput { GoodsNo = goodsNo });

            //stopwatch.Stop();
            //TimeSpan timeSpan = stopwatch.Elapsed;

            if (result.IsSuccess) 
            {
                return Ok(result.Data);
                //return Ok($"毫秒：{timeSpan.TotalMilliseconds}");
            }

            return BadRequest(result.BaseResult());
        }

        /// <summary>
        /// 修改商品购买数量
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPatch("GoodsQty")]
        [ProducesResponseType(typeof(SaleMenuDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Result), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UpdateGoodsCount([FromBody]UpdateGoodsCountInput input)
        {
            var result = await _saleMenuAppService.UpdateGoodsCount(input);

            if (result.IsSuccess)
            {
                return Ok(result.Data);
            }

            return BadRequest(result.BaseResult());
        }

        /// <summary>
        /// 删除商品栏
        /// </summary>
        /// <param name="goodsNo">商品编号</param>
        /// <returns></returns>
        [HttpDelete("MenuItem/{goodsNo}")]
        [ProducesResponseType(typeof(SaleMenuDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Result), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> RemoveGoods(string goodsNo)
        {
            var result = await _saleMenuAppService.RemoveGoods(goodsNo);

            if (result.IsSuccess)
            {
                return Ok(result.Data);
            }

            return BadRequest(result.BaseResult());
        }

        /// <summary>
        /// 清空购物车
        /// </summary>
        /// <returns></returns>
        [HttpDelete("Menu")]
        [ProducesResponseType(typeof(SaleMenuDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Result), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> ClearMenu()
        {
            var result = await _saleMenuAppService.ClearMenu();

            if (result.IsSuccess)
            {
                return Ok(result.Data);
            }

            return BadRequest(result.BaseResult());
        }

        /// <summary>
        /// 单项打折
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPatch("SingleDiscount")]
        [ProducesResponseType(typeof(SaleMenuDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Result), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> SingleDiscount([FromBody]SingleDiscountInput input)
        {
            var result = await _saleMenuAppService.SingleDiscount(input);

            if (result.IsSuccess)
            {
                return Ok(result.Data);
            }

            return BadRequest(result.BaseResult());
        }

        /// <summary>
        /// 全部打折
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPatch("AllDiscount")]
        [ProducesResponseType(typeof(SaleMenuDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Result), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> AllDiscount([FromBody]AllDiscountInput input)
        {
            var result = await _saleMenuAppService.AllDiscount(input);

            if (result.IsSuccess)
            {
                return Ok(result.Data);
            }

            return BadRequest(result.BaseResult());
        }

        /// <summary>
        /// 订单商品销售总额（不参与折扣）
        /// </summary>
        /// <returns></returns>
        [HttpGet("ActualMenu")]
        [ProducesResponseType(typeof(ActualMenu), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Result), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetActualMenu()
        {
            var result = await _saleMenuAppService.GetActualMenu();
            if (result.IsSuccess)
            {
                return Ok(result.Data);
            }
            return BadRequest(result.BaseResult());
        }

        /// <summary>
        /// 获取当前已付款信息
        /// </summary>
        /// <returns></returns>
        [HttpGet("Pay")]
        [ProducesResponseType(typeof(SMPayDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Result), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetPay()
        {
            var result = await _saleMenuAppService.GetPay();

            if (result.IsSuccess)
            {
                return Ok(result.Data);
            }

            return BadRequest(result.BaseResult());
        }

        /// <summary>
        /// 现金付款
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost("CashPay")]
        [ProducesResponseType(typeof(SMPayDto), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(Result), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CashPay([FromBody]CashPayInput input)
        {
            var result = await _saleMenuAppService.CashPay(input);

            if (result.IsSuccess)
            {
                return Ok(result.Data);
            }

            return BadRequest(result.BaseResult());
        }

        /// <summary>
        /// 第三方付款
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost("ThirdPay")]
        [ProducesResponseType(typeof(SMPayDto), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(Result), StatusCodes.Status400BadRequest)] 
        public async Task<IActionResult> ThirdPay([FromBody]ThirdPayInput input)
        {
            var result = await _saleMenuAppService.ThirdPay(input);

            if (result.IsSuccess)
            {
                return Ok(result.Data);
            }

            return BadRequest(result.BaseResult());
        }

        /// <summary>
        /// 礼券付款
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost("CouponPay")]
        [ProducesResponseType(typeof(SMPayDto), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(Result), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CouponPay([FromBody]CouponPayInput input)
        {
            var result = await _saleMenuAppService.CouponPay(input);

            if (result.IsSuccess)
            {
                return Ok(result.Data);
            }

            return BadRequest(result.BaseResult());
        }

        /// <summary>
        /// 信用卡付款
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost("CreditCardPay")]
        [ProducesResponseType(typeof(SMPayDto), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(Result), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreditCardPay([FromBody]CreditCardPayInput input)
        {
            var result = await _saleMenuAppService.CreditCardPay(input);

            if (result.IsSuccess)
            {
                return Ok(result.Data);
            }

            return BadRequest(result.BaseResult());
        }

    }
}
