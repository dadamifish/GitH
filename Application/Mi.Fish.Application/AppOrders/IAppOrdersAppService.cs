using Abp.Application.Services;
using Mi.Fish.ApplicationDto;
using Mi.Fish.Infrastructure.Results;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Mi.Fish.Application.AppOrders
{
    public interface IAppOrdersAppService:IApplicationService
    {
        /// <summary>
        /// 获取app订单
        /// </summary>
        /// <param name="status">订单状态</param>
        /// <returns></returns>
        Task<Result<List<FishOrdersOutputDto>>> GetAppOrdersAsync(AppOrderStatus status);
        /// <summary>
        /// 接单
        /// </summary>
        /// <param name="input">App订单号转POS单</param>
        /// <returns></returns>
        Task<Result<CreateOrderNoDto>> CreateFishOrdersAsync(AppOrderInputDto input);
        /// <summary>
        /// 退单
        /// </summary>
        /// <param name="order">POS系统已接app订单号</param>
        /// <returns></returns>
        Task<Result<CreateOrderNoDto>> DeleteOrdersAsync(string order);

        /// <summary>
        /// 打印
        /// </summary>
        /// <param name="orderNo">pos单号</param>
        /// <param name="isRePrint">是否是重打印</param>
        /// <returns></returns>
        Task<Result<OrdersPrintDto>> PrintInfoAsync(string orderNo,int isRePrint);
    }
}
