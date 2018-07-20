using Mi.Fish.ApplicationDto;
using Mi.Fish.Infrastructure.Results;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Mi.Fish.Application
{
	/// <summary>
	/// 第三方支付接口
	/// </summary>
    public interface IPayAppService
    {

        /// <summary>
        /// 获取秒通四位码接口
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<Result<MutoneSnCodeOutput>> GetMutoneSnCode(GetMutoneSnCodePayInput input);


        /// <summary>
        /// 秒通四位码支付
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<Result<PayOutput>> MutoneSnCodePay(MutoneSnCodePayInput input);


        /// <summary>
        /// 秒通扫码支付
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>

        Task<Result<PayOutput>> MutoneScanPay(SacnPayInput input);

        /// <summary>
        /// 秒通支付查询
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>

        Task<Result<PayOutput>> MutonePayQuery(PayQueryInput input);

        /// <summary>
        /// 微信扫码支付
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>

        Task<Result<PayOutput>> WxScanPay(SacnPayInput input,string ip);

        /// <summary>
        /// 微信扫码支付查询
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>

        Task<Result<PayOutput>> WxScanPayQuery(PayQueryInput input);

        /// <summary>
        /// 支付宝扫码支付
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>

        Task<Result<PayOutput>> AliScanPay(SacnPayInput input,string ip);

        /// <summary>
        /// 支付宝扫码支付查询
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>

        Task<Result<PayOutput>> AliScanPayQuery(PayQueryInput input);


        /// <summary>
        /// 方特扫码支付
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>

        Task<Result<PayOutput>> FtScanPay(SacnPayInput input);

        /// <summary>
        /// 方特扫码支付查询
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>

        Task<Result<PayOutput>> FtScanPayQuery(PayQueryInput input);


        /// <summary>
        /// 第三方支付支付退款接口
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<Result<PayReturnOutput>> PayReturn(PayReturnInput input);



        /// <summary>
        /// 礼券查询接口
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>

        Task<Result<VolumeQueryOutput>> VolumeQuery(VolumeQueryInput input);


        /// <summary>
        /// 礼券状态更新接口
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>

        Task<Result> VolumeUpdateStatus(VolumeUpdateInput input);




    }
}
