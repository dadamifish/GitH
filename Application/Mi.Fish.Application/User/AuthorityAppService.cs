using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;
using Abp.Runtime.Caching;
using Mi.Fish.ApplicationDto;
using Mi.Fish.EntityFramework;
using Mi.Fish.Infrastructure.Results;

namespace Mi.Fish.Application.User
{
    public class AuthorityAppService : AppServiceBase, IAuthorityAppService
    {
        private readonly ParkDbContext _parkDbContext;
        private readonly ICacheManager _cacheManager;
        public AuthorityAppService(ParkDbContext parkDbContext, ICacheManager cacheManager)
        {
            _parkDbContext = parkDbContext;
            _cacheManager = cacheManager;
        }
        /// <summary>
        /// 清除授权
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public async Task<Result> DeleteAuthorityAsync(EnumAuthorityType type)
        {
            string userId = FishSession.UserId;
            _cacheManager.RemoveUserAuthority(userId, type);
            return Result.Ok;
        }

        /// <summary>
        /// 授权
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<Result> UserAuthorityAsync(UserAuthorityInputDto input)
        {
            string sql = string.Format("	declare @ModulePower varchar(20)	declare @quanxian varchar(30)='0000000000000000000000000000000000000000000000'	" +
                "select @ModulePower=sCashierLevelID from tCashier where sCashierNO=@userId and sPassword=@pwd	" +
                "if @ModulePower is null  set @quanxian='' " +
                "if @ModulePower='5'	  set @quanxian='1111111111111111111111111111111111111111111111'	" +
                "if ( @ModulePower='1' or @ModulePower='0' or @ModulePower='4' or @ModulePower='3')	  set @quanxian='0010000000001010000000010000000000000000000000'	" +
                "if @ModulePower='2'	  set @quanxian='0000100000000000000000000000000000000000000000' " +
                "select @quanxian as cashier_grant");
            object obj = new { userId = input.UserId, pwd = input.PWD };
            string result = (await _parkDbContext.ExecuteScalarAsync(sql, obj))?.ToString();
            if (!string.IsNullOrEmpty(result))
            {
                if (result.Substring((int)input.AuthorityType, 1) == "1")
                {
                    string userId = FishSession.UserId;
                    _cacheManager.SetUserAuthority(userId, input.AuthorityType, true);
                    return Result.Ok;
                }
                else
                {
                    return Result.Fail<string>("此工号无权限");
                }
            }
            else
            {
                return Result.Fail<string>("账号密码错误");
            }
        }
        /// <summary>
        /// 登录用户权限判断
        /// </summary>
        /// <param name="type">权限类型</param>
        /// <returns></returns>
        public async Task<Result<Judge>> UserPowerAsync(EnumAuthorityType type)
        {
            string userId = FishSession.UserId;
            var _userData = _cacheManager.GetUserDataCacheByUserId(userId);
            Judge judge = new Judge { result = false };
            switch (type)
            {
                case EnumAuthorityType.SearchSaleGrant:
                    judge.result = _userData.SearchSaleGrant;
                    break;
                case EnumAuthorityType.SetGrant:
                    judge.result = _userData.SetGrant;
                    break;
                case EnumAuthorityType.BackGood:
                    judge.result = _userData.BackGoods;
                    break;
                case EnumAuthorityType.RePrint:
                    judge.result = _userData.RePrint;
                    break;
                case EnumAuthorityType.SingleDiscount:
                    judge.result = _userData.SingleDaZhe;
                    break;
                case EnumAuthorityType.AllDiscount:
                    judge.result = _userData.AllDaZhe;
                    break;
                case EnumAuthorityType.EditOrder:
                    judge.result = _userData.TuiCai;
                    break;
                case EnumAuthorityType.Revoke:
                    judge.result = _userData.DeleOrder;
                    break;
            }
            return Result.FromData(judge);
        }


    }
}
