using Abp.Application.Services;
using Mi.Fish.ApplicationDto;
using Mi.Fish.Infrastructure.Results;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Mi.Fish.Application.User
{
    public interface IAuthorityAppService:IApplicationService
    {
        /// <summary>
        /// 功能授权
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<Result> UserAuthorityAsync(UserAuthorityInputDto input);
        /// <summary>
        /// 权限查询
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        Task<Result<Judge>> UserPowerAsync(EnumAuthorityType type);
        /// <summary>
        /// 删除授权
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        Task<Result> DeleteAuthorityAsync(EnumAuthorityType type);
    }
}
