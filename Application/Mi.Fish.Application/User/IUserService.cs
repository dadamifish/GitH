using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;
using Abp.Application.Services;
using Mi.Fish.ApplicationDto;
using Mi.Fish.Infrastructure.Results;


namespace Mi.Fish.Application
{
    public interface IUserService: IApplicationService
     {
        /// <summary>
        /// 用户登录
        /// </summary>
        /// <param name="userLoginInput"></param>
        /// <returns></returns>
        Result<UserData> UserLogin(UserLoginInput userLoginInput);


        /// <summary>
        /// 用户退出
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <returns></returns>
        Task<Result> UserLogOut(string userId);

        /// <summary>
        /// 用户密码修改
        /// </summary>
        /// <param name="userPwdUpdateInput"></param>
        /// <returns></returns>
        Task<Result> UserPwdUpdate(UserPwdUpdateInput userPwdUpdateInput);

        /// <summary>
        /// 用户交班
        /// </summary>
        /// <returns></returns>
        Task<Result> UserTurnClass();

        /// <summary>
        /// 获取用户交班明细
        /// </summary>
        /// <returns></returns>
        Task<Result<TurnClassDetailOutPut>> GetUserTurnClassDetail();

    } 
}
