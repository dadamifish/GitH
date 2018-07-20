using System.Collections.Generic;

namespace Mi.Fish.Application
{
    using System.Threading.Tasks;
    using Abp.Application.Services;
    using Mi.Fish.ApplicationDto;
    using Mi.Fish.Infrastructure.Results;

    public interface IOrderDetailService : IApplicationService
    {
        Task<Result<OrderDetailOutput>> GetOrderDetail(string orderNo);

        Task<Result<List<OrderSaleListOutPut>>> GetLastSaleList();
        Task<Result<BaseOrderRefundOutput>> OrderRefund(string orderNo, string ip);
        Task<Result<OrderRefundOutput>> OrderRefundCheck(string orderNo);
    }
}
