using Mi.Fish.ApplicationDto;
using Mi.Fish.Infrastructure.Results;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Mi.Fish.Application.Stock
{
    /// <summary>
    /// 库存服务接口
    /// </summary>
    public interface IStockAppService
    {
        /// <summary>
        /// 获取门店库存
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<Result<List<GetStoreStockDto>>> GetStoreStock(GetStoreStockInput input);
    }
}
