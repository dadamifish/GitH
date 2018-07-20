using Abp.Application.Services;
using Mi.Fish.ApplicationDto;
using Mi.Fish.Infrastructure.Results;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Mi.Fish.Application.SaleDetail
{
    /// <summary>
    /// 销售明细服务接口
    /// </summary>
    public interface ISaleDetailAppService : IApplicationService
    {
        /// <summary>
        /// 获取收银员当前班次销售明细
        /// </summary>
        /// <param name="input">the input</param>
        /// <returns></returns>
        Task<Result<List<GetSaleDetailOutput>>> GetCashierSaleDetail(GetSaleDetailInput input);
    }
}
