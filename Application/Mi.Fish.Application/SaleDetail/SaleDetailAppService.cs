using Abp.AutoMapper;
using Mi.Fish.ApplicationDto;
using Mi.Fish.EntityFramework;
using Mi.Fish.Infrastructure.Results;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Abp.Runtime.Caching;
using Mi.Fish.Application.Order;
using System.Linq;

namespace Mi.Fish.Application.SaleDetail
{
    /// <summary>
    /// 销售明细应用服务
    /// </summary>
    public class SaleDetailAppService : AppServiceBase, ISaleDetailAppService
    {
        private readonly LocalDbContext _context;
        private readonly ICacheManager _cacheManager;
        private readonly IOrderNoAppService _orderNoAppService;

        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="context"></param>
        /// <param name="cacheManager"></param>
        /// <param name="orderNoAppService"></param>
        public SaleDetailAppService(LocalDbContext context, ICacheManager cacheManager, IOrderNoAppService orderNoAppService)
        {
            _context = context;
            _cacheManager = cacheManager;
            _orderNoAppService = orderNoAppService;
        }

        /// <summary>
        /// 获取收银员当前班次销售明细
        /// </summary>
        /// <param name="input">the input</param>
        /// <returns></returns>
        public async Task<Result<List<GetSaleDetailOutput>>> GetCashierSaleDetail(GetSaleDetailInput input)
        {
            var userId = FishSession.UserId;
            var userData = _cacheManager.GetUserDataCacheByUserId(userId);

            if (!userData.SearchSaleGrant)
            {
                if (!_cacheManager.GetUserAuthority(userData.UserID, EnumAuthorityType.SearchSaleGrant))
                {
                    return Result.Fail<List<GetSaleDetailOutput>>("无查询销售明细权限");
                }
            }
            else
            {
                _cacheManager.RemoveUserAuthority(userData.UserID, EnumAuthorityType.SearchSaleGrant);
            }

            DateTime today = DateTime.Today;
            if (userData.CurrentDealTime > today)
            {
                today = userData.CurrentDealTime;
            }

            var sqlInput = input.MapTo<GetSaleDetailProcInput>();
            sqlInput.CashierNO = userId;
            sqlInput.oper_date = today;
            sqlInput.sell_way = sqlInput.sell_way == "-1" ? "" : sqlInput.sell_way;

            var sql = @"exec ft_pr_searchSaleDtl @CashierNO,@item_no,@item_name,@sell_way,@oper_date,@flow_no";
            var list = await _context.ExecuteFunctionAsync<List<GetSaleDetailDto>>(sql, sqlInput);

            var sales = list.MapTo<List<GetSaleDetailOutput>>();
            var result = new List<GetSaleDetailOutput>();
            result.Add(sales[0]);
            if (sales.Count > 1)
            {
                sales.RemoveAt(0);
                var saleSort = sales.OrderByDescending(w => w.SaleNo).ThenBy(w => int.Parse(w.Sort)).ToList();
                result.AddRange(saleSort);
            }
            return Result.FromData(result);
        }
    }
}
