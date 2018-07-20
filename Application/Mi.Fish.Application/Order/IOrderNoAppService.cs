using Abp.Application.Services;
using Mi.Fish.ApplicationDto;
using Mi.Fish.Infrastructure.Results;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Mi.Fish.Application.Order
{
    /// <summary>
    /// 
    /// </summary>
    public interface IOrderNoAppService : IApplicationService
    {
        /// <summary>
        /// 生成单号
        /// </summary>
        /// <returns></returns>
        Task<Result<string>> CreateOrderNo();
        Result<bool> CheckPermission(EnumAuthorityType authorityType);
        Result SendCookPrint(OrderPrint orderDetails, string printName,CookSetting setting);
        Task<Result> SendCookPrint(string orderNo, string printName, CookSetting setting);
    }
}
