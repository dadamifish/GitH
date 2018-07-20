using Abp.Application.Services;
using Mi.Fish.ApplicationDto;
using Mi.Fish.Infrastructure.Results;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Mi.Fish.Application
{
    public interface IOrderAppService : IApplicationService
    {

        /// <summary>
        /// 下单付款前库存检验
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<Result<GetOrderStockDto>> CheckOrder(CheckOrderInput input);

        /// <summary>
        /// 订单报表
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<Result<List<GetOrderReportDto>>> GetOrderReport(GetOrderReportInput input);

        /// <summary>
        /// 获取新的订单
        /// </summary>
        /// <returns></returns>
        Task<Result<List<GetNewOrderDto>>> GetNewOrder();
    }
}
