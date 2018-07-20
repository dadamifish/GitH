using Mi.Fish.ApplicationDto;
using Mi.Fish.EntityFramework;
using Mi.Fish.Infrastructure.Results;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Mi.Fish.Application.Stock
{
    /// <summary>
    /// 库存应用服务
    /// </summary>
    public class StockAppService : AppServiceBase, IStockAppService
    {
        #region field

        private readonly ParkDbContext _parkContext;

        #endregion

        #region ctor

        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="parkContext">DbContext</param>
        public StockAppService(ParkDbContext parkContext)
        {
            _parkContext = parkContext;
        }

        #endregion

        /// <summary>
        /// 获取门店库存
        /// </summary>
        /// <param name="input">the input</param>
        /// <returns></returns>
        public async Task<Result<List<GetStoreStockDto>>> GetStoreStock(GetStoreStockInput input)
        {
            var sqlstr = @"select t2.sMainBarcode as BarCode,t2.sGoodsName as GoodsName,ISNULL(t3.nStockQty,0) as StockQty,ISNULL(t3.nOffsetDiffNetAmount,0) as LockStock
from base_entry_message_allow t1
left join tGoods t2 on t2.sGoodsNO = t1.item_no 
left join tStockAccount t3 on (t2.nGoodsID = t3.nGoodsID and t3.sStoreNO = @StoreNo)
where t1.activeflag = 1 and t1.branch_no = @StoreNo and t1.jh = @TerminalNo ";

            if (!string.IsNullOrEmpty(input.Key))
            {
                //因为现在条码都是数字，所以可以依据KEY是否为整形区分商品名称和条码
                long barCode = 0;
                if (long.TryParse(input.Key.Trim(), out barCode))
                {
                    sqlstr += " and t2.sMainBarcode like @Key";
                }
                else
                {
                    sqlstr += " and t2.sGoodsName like @Key";
                }
            }

            var par = new GetStoreStockSqlInput();
            par.StoreNo = FishSession.StorageNo;
            par.TerminalNo = FishSession.TerminalId;
            if (!string.IsNullOrEmpty(input.Key))
            {
                par.Key = "%" + input.Key.Trim() + "%";
            }
            var result = await _parkContext.ExecuteFunctionAsync<List<GetStoreStockDto>>(sqlstr, par);
            return Result.FromData(result);
        }
    }
}
