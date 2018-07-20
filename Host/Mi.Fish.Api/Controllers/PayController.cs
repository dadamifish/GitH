using Mi.Fish.Application;
using Mi.Fish.ApplicationDto;
using Mi.Fish.Infrastructure.Results;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Mi.Fish.Api.Controllers
{

	/// <summary>
	/// 第三方支付接口
	/// </summary>
    public class PayController : FishControllerBase
	{

		private readonly IPayAppService _payAppService;
		/// <summary>
		/// 
		/// </summary>
		/// <param name="payAppService"></param>
		public PayController(IPayAppService payAppService)
		{
			_payAppService = payAppService;
		}



        /// <summary>
        /// 获取秒通四位码接口
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [ProducesResponseType(typeof(MutoneSnCodeOutput), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Result), StatusCodes.Status400BadRequest)]
        [HttpGet("MutoneSnCode")]
        public async Task<IActionResult> MutoneSnCode([FromQuery] GetMutoneSnCodePayInput input)
        {
            var result = await _payAppService.GetMutoneSnCode(input);

            if (result.IsSuccess)
            {
                return Ok(result.Data);
            }

            return BadRequest(new Result(result.Code, result.Message));
        }


        /// <summary>
        /// 秒通四位码支付接口
        /// </summary>
        /// <returns></returns>
        [ProducesResponseType(typeof(PayOutput), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(Result), StatusCodes.Status400BadRequest)]
        [HttpPost("MutoneSnCode")]
        public async Task<IActionResult> MutoneSnCode([FromBody]MutoneSnCodePayInput input)
        {
            var result = await _payAppService.MutoneSnCodePay(input);

            if (result.IsSuccess)
            {
                return CreatedAtAction(nameof(MutoneSnCode), result.Data);
            }

            return BadRequest(new Result(result.Code, result.Message));
        }


        /// <summary>
        /// 扫码付接口
        /// </summary>
        /// <returns></returns>
        [ProducesResponseType(typeof(PayOutput), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(Result), StatusCodes.Status400BadRequest)]
        [HttpPost("Scan")]
        public async Task<IActionResult> Scan([FromBody]SacnPayInput input)
        {
            var ip = GetUserIp();

            //var payType=  AutomaticDetectionPayType(input.AuthCode);
            var payType = input.PayType;

            Result<PayOutput> result = new Result<PayOutput>(ResultCode.Fail, null);

            switch (payType)
            {
                case PayType.Ali:
                    result = await _payAppService.AliScanPay(input,ip);
                    break; 
                case PayType.Wx:
                    result = await _payAppService.WxScanPay(input,ip);
                    break;

                case PayType.Ft:
                    result = await _payAppService.FtScanPay(input);
                    break;
                case PayType.Mutone:
                    result = await _payAppService.MutoneScanPay(input);
                    break;
                default:
                    return BadRequest(new Result(result.Code, "支付方式参数错误"));
            }

            if (result.IsSuccess)
            {
                return CreatedAtAction(nameof(Scan), result.Data);
            }

            return BadRequest(new Result(result.Code, result.Message));
        }

        /// <summary>
        /// 扫码支付查询接口
        /// </summary>
        /// <param name="tradeNo">商户或第三方交易单号(秒通、乐游传第三方订单号  微信、支付宝传餐饮订单号)</param>
        /// <param name="payMoney">交易总金额[必填，交易金额必须大于0]</param>
        /// <param name="payType">支付方式【必填】</param>
        /// <returns></returns>
        [ProducesResponseType(typeof(PayOutput), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Result), StatusCodes.Status400BadRequest)]
        [HttpGet("PaySearch/{tradeNo}")]
        public async Task<IActionResult> PaySearch(string tradeNo, decimal payMoney,PayType payType)
        {

            Logger.Info("微信扫码支付查询：tradeNo=" + tradeNo+ "--payMoney=" + payMoney.ToString()+ "--payType=" + payType.ToString());

            PayQueryInput input = new PayQueryInput()
            {
                TradeNo = tradeNo,
                PayMoney = payMoney
            };
            Result<PayOutput> result = new Result<PayOutput>(ResultCode.Fail, null);
              
            switch (payType)
            {
                case PayType.Ali:
                    result = await _payAppService.AliScanPayQuery(input);
                    break;
                case PayType.Wx:
                    result = await _payAppService.WxScanPayQuery(input);
                    break;

                case PayType.Ft:
                    result = await _payAppService.FtScanPayQuery(input);
                    break;
                case PayType.Mutone:
                    result = await _payAppService.MutonePayQuery(input);
                    break;
                default:
                    return BadRequest(new Result(result.Code, "支付方式参数错误"));
            }

            if (result.IsSuccess)
            {
                return Ok(result.Data);
            }

            return BadRequest(new Result(result.Code, result.Message));
        }



        ///// <summary>
        ///// 秒通扫码付接口
        ///// </summary>
        ///// <returns></returns>
        //[ProducesResponseType(typeof(PayOutput), StatusCodes.Status201Created)]
        //[ProducesResponseType(typeof(Result), StatusCodes.Status400BadRequest)]
        //[HttpPost("MutoneScan")]
        //public async Task<IActionResult> MutoneScan([FromBody]SacnPayInput input)
        //{

        //    var result = await _payAppService.MutoneScanPay(input);

        //    if (result.IsSuccess)
        //    {
        //        return CreatedAtAction(nameof(MutoneScan), result.Data);
        //    }

        //    return BadRequest(new Result(result.Code, result.Message));
        //}

        ///// <summary>
        ///// 秒通支付查询接口
        ///// </summary>
        ///// <param name="tradeNo">商户或第三方交易单号(秒通、乐游传第三方订单号  微信、支付宝传餐饮订单号)</param>
        ///// <param name="payMoney">交易总金额</param>
        ///// <returns></returns>
        //[ProducesResponseType(typeof(PayOutput), StatusCodes.Status200OK)]
        //[ProducesResponseType(typeof(Result), StatusCodes.Status400BadRequest)]
        //[HttpGet("MutonePaySearch/{tradeNo}")]
        //public async Task<IActionResult> MutonePaySearch(string tradeNo, decimal payMoney)
        //{
        //    PayQueryInput input = new PayQueryInput()
        //    {
        //        TradeNo = tradeNo,
        //        PayMoney = payMoney
        //    };

        //    var result = await _payAppService.MutonePayQuery(input);

        //    if (result.IsSuccess)
        //    {
        //        return Ok(result);
        //    }

        //    return BadRequest(new Result(result.Code, result.Message));
        //}

        ///// <summary>
        ///// 微信扫码付接口
        ///// </summary>
        ///// <returns></returns>
        //[ProducesResponseType(typeof(PayOutput), StatusCodes.Status201Created)]
        //[ProducesResponseType(typeof(Result), StatusCodes.Status400BadRequest)]
        //[HttpPost("WxScan")]
        //public async Task<IActionResult> WxScan([FromBody]SacnPayInput input)
        //{
        //    var ip = GetUserIp();

        //    var result = await _payAppService.WxScanPay(input, ip);

        //    if (result.IsSuccess)
        //    {
        //        return CreatedAtAction(nameof(WxScan), result.Data);
        //    }

        //    return BadRequest(new Result(result.Code, result.Message));
        //}

        ///// <summary>
        ///// 支付宝扫码付接口
        ///// </summary>
        ///// <returns></returns>
        //[ProducesResponseType(typeof(PayOutput), StatusCodes.Status201Created)]
        //[ProducesResponseType(typeof(Result), StatusCodes.Status400BadRequest)]
        //[HttpPost("AliScan")]
        //public async Task<IActionResult> AliScan([FromBody]SacnPayInput input)
        //{
        //    var ip = GetUserIp();

        //    var result = await _payAppService.AliScanPay(input, ip);

        //    if (result.IsSuccess)
        //    {
        //        return CreatedAtAction(nameof(AliScan), result.Data);
        //    }

        //    return BadRequest(new Result(result.Code, result.Message));
        //}

        ///// <summary>
        ///// 方特扫码付接口
        ///// </summary>
        ///// <returns></returns>
        //[ProducesResponseType(typeof(PayOutput), StatusCodes.Status201Created)]
        //[ProducesResponseType(typeof(Result), StatusCodes.Status400BadRequest)]
        //[HttpPost("FtScan")]
        //public async Task<IActionResult> FtScan([FromBody]SacnPayInput input)
        //{

        //    var result = await _payAppService.FtScanPay(input);

        //    if (result.IsSuccess)
        //    {
        //        return CreatedAtAction(nameof(FtScan), result.Data);
        //    }

        //    return BadRequest(new Result(result.Code, result.Message));
        //}

        ///// <summary>
        ///// 方特支付查询接口
        ///// </summary>
        ///// <param name="tradeNo">商户或第三方交易单号(秒通、乐游传第三方订单号  微信、支付宝传餐饮订单号)</param>
        ///// <param name="payMoney">交易总金额</param>
        ///// <returns></returns>
        //[ProducesResponseType(typeof(PayOutput), StatusCodes.Status200OK)]
        //[ProducesResponseType(typeof(Result), StatusCodes.Status400BadRequest)]
        //[HttpGet("FtScanPaySerach/{tradeNo}")]
        //public async Task<IActionResult> FtScanPaySerach(string tradeNo, decimal payMoney)
        //{
        //    PayQueryInput input = new PayQueryInput()
        //    {
        //        TradeNo = tradeNo,
        //        PayMoney = payMoney
        //    };

        //    var result = await _payAppService.FtScanPayQuery(input);

        //    if (result.IsSuccess)
        //    {
        //        return Ok(result);
        //    }

        //    return BadRequest(new Result(result.Code, result.Message));
        //}

        ///// <summary>
        ///// 微信支付查询接口
        ///// </summary>
        ///// <param name="tradeNo">商户或第三方交易单号(秒通、乐游传第三方订单号  微信、支付宝传餐饮订单号)</param>
        ///// <param name="payMoney">交易总金额</param>
        ///// <returns></returns>
        //[ProducesResponseType(typeof(PayOutput), StatusCodes.Status200OK)]
        //[ProducesResponseType(typeof(Result), StatusCodes.Status400BadRequest)]
        //[HttpGet("WxScanPaySearch/{tradeNo}")]
        //public async Task<IActionResult> WxScanPaySearch(string tradeNo, decimal payMoney)
        //{
        //    PayQueryInput input = new PayQueryInput()
        //    {
        //        TradeNo = tradeNo,
        //        PayMoney = payMoney
        //    };

        //    var result = await _payAppService.WxScanPayQuery(input);

        //    if (result.IsSuccess)
        //    {
        //        return Ok(result);
        //    }

        //    return BadRequest(new Result(result.Code, result.Message));
        //}

        ///// <summary>
        ///// 支付宝支付查询接口
        ///// </summary>
        ///// <param name="tradeNo">商户或第三方交易单号(秒通、乐游传第三方订单号  微信、支付宝传餐饮订单号)</param>
        ///// <param name="payMoney">交易总金额</param>
        ///// <returns></returns>
        //[ProducesResponseType(typeof(PayOutput), StatusCodes.Status200OK)]
        //[ProducesResponseType(typeof(Result), StatusCodes.Status400BadRequest)]
        //[HttpGet("AliScanPaySearch/{tradeNo}")]
        //public async Task<IActionResult> AliScanPaySearch(string tradeNo, decimal payMoney)
        //{
        //    PayQueryInput input = new PayQueryInput()
        //    {
        //        TradeNo = tradeNo,
        //        PayMoney = payMoney
        //    };

        //    var result = await _payAppService.AliScanPayQuery(input);

        //    if (result.IsSuccess)
        //    {
        //        return Ok(result);
        //    }

        //    return BadRequest(new Result(result.Code, result.Message));
        //}


        /// <summary>
        /// 扫码付退款接口
        /// </summary>
        /// <returns></returns>
        [ProducesResponseType(typeof(PayOutput), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(Result), StatusCodes.Status400BadRequest)]
        [HttpPost("PayReturn")]
        public async Task<IActionResult> PayReturn([FromBody]PayReturnInput input)
        {
            var result = await _payAppService.PayReturn(input);

            if (result.IsSuccess)
            {
                return CreatedAtAction(nameof(PayReturn), result.Data);
            }

            return BadRequest(new Result(result.Code, result.Message));
        }


        /// <summary>
        /// 礼券查询接口
        /// </summary>
        /// <param name="volumeNo">礼券号</param>
        /// <returns></returns>
        [ProducesResponseType(typeof(VolumeQueryOutput), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Result), StatusCodes.Status400BadRequest)]
        [HttpGet("VolumeSearch/{volumeNo}")]
        public async Task<IActionResult> VolumeSearch(string volumeNo)
        {
            VolumeQueryInput input = new VolumeQueryInput()
            {
                VolumeNo = volumeNo
            };

            var result = await _payAppService.VolumeQuery(input);

            if (result.IsSuccess)
            {
                return Ok(result.Data);
            }

            return BadRequest(new Result(result.Code, result.Message));
        }

        /// <summary>
        /// 礼券状态更新接口
        /// </summary>
        /// <returns></returns>
        [ProducesResponseType(typeof(Result), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Result), StatusCodes.Status400BadRequest)]
        [HttpPatch("VolumeUpdateStatus")]
        public async Task<IActionResult> VolumeUpdateStatus([FromBody]VolumeUpdateInput input)
        {
            var result = await _payAppService.VolumeUpdateStatus(input);

            if (result.IsSuccess)
            {
                return Ok();
            }

            return BadRequest(new Result(result.Code, result.Message));
        }


    }
}
