namespace Mi.Fish.Application
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Abp.Application.Services;
    using Mi.Fish.ApplicationDto;
    using Mi.Fish.Infrastructure.Results;

    public interface IGoodsAppService : IApplicationService
    {
        Task<Result<List<GoodsOutPut>>> GetHotGoodsList();
        Task<Result<List<GoodsOutPut>>> GetGoodsList(string categoryNo, int type);
        Task<Result<List<GoodsSearchResult>>> GetGoodsSearchResult(string key);
    }
}
