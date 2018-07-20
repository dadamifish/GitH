using System;
namespace Mi.Fish.Application
{
    using System.Threading.Tasks;
    using Abp.Application.Services;
    using Mi.Fish.ApplicationDto;
    using Mi.Fish.Infrastructure.Results;

    public interface ICategoryService : IApplicationService
    {
        Task<Result<CategoryDetailOutPut>> GetCategoryList(int level, string parentid);
    }
}
