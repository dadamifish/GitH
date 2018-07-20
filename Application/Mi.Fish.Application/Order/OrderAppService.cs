using Abp.Runtime.Caching;
using Mi.Fish.ApplicationDto;
using Mi.Fish.EntityFramework;
using Mi.Fish.Infrastructure.Results;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Mi.Fish.Application
{
    /// <summary>
    /// 
    /// </summary>
    public class OrderAppService : AppServiceBase, IOrderAppService
    {
        private readonly ParkDbContext _parkContext;
        private readonly LocalDbContext _localContext;
        private readonly ICacheManager _cacheManager;

        public OrderAppService(ParkDbContext parkcontext, LocalDbContext localcontext, ICacheManager cacheManager)
        {
            _parkContext = parkcontext;
            _localContext = localcontext;
            _cacheManager = cacheManager;
        }

        #region private function

        /// <summary>
        /// 
        /// </summary>
        /// <param name="orderStatus"></param>
        /// <returns></returns>
        private string GetSqlOrderStatus(OrderReportStatus? orderStatus)
        {
            if (orderStatus == OrderReportStatus.All)
            {
                return "10,20,30,40";
            }
            else if (orderStatus == OrderReportStatus.Commited)
            {
                return "10,20";
            }
            else if (orderStatus == OrderReportStatus.Paid)
            {
                return "30";
            }
            else
            {
                return "40";
            }
        }

        #endregion

        /// <summary>
        /// 检测商品库存
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        private async Task<Result<GetRealStockDto>> CheckStock(CheckStockInput input)
        {
            var qty = input?.Qty ?? 1;
            qty = qty >= 0 ? qty : 0;
            var dto = new GetRealStockDto();
            dto.GoodsNo = input.GoodsNo;

            var sql = "select HavaStock = havestock,VirtualStock = guqingqty from base_entry_message_allow WHERE  activeflag=1 and branch_no=@branch_no and jh=@jh AND item_no=@item_no";
            object sqlInput = new { branch_no = FishSession.StorageNo, jh = FishSession.TerminalId, item_no = input.GoodsNo };
            var messageAllow = await _localContext.ExecuteFunctionAsync<SqlVirtualStock>(sql, sqlInput);

            //判断估库存
            if (messageAllow != null)
            {
                if (messageAllow.VirtualStock >= 0 && messageAllow.VirtualStock < qty)
                {
                    dto.HasStock = false;
                    dto.Message = "(估清)库存不足";
                    return Result.FromData(dto); ;
                }
                else
                {
                    dto.Stock = messageAllow.VirtualStock;
                }
            }

            //判断实时库存
            if (messageAllow == null || (messageAllow != null && messageAllow.HavaStock == 0))
            {
                sql = "exec p_get_nowstock @branch_no,@item_no";
                sqlInput = new { branch_no = FishSession.StorageNo, item_no = input.GoodsNo };
                var nowStock = await _parkContext.ExecuteFunctionAsync<ProcNowStock>(sql, sqlInput);

                if (nowStock == null || nowStock.nStockQty < qty)
                {
                    dto.HasStock = false;
                    dto.Message = "库存不足";
                    return Result.FromData(dto); ;
                }
                else
                {
                    dto.Stock = nowStock.nStockQty;
                }
            }

            dto.HasStock = true;
            dto.Message = "库存足够";

            return Result.FromData(dto);
        }

        /// <summary>
        /// 下单付款前库存检验
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<Result<GetOrderStockDto>> CheckOrder(CheckOrderInput input)
        {
            GetOrderStockDto result = new GetOrderStockDto();

            var menuCache = _cacheManager.GetSaleMenuCache(FishSession.UserId);
            var orderCache = menuCache?.FirstOrDefault(w => w.OrderNo == input.OrderNo);

            if (orderCache == null)
            {
                return Result.Fail<GetOrderStockDto>("该订单数据失效，请重新销售！");
            }

            result.OrderNo = input.OrderNo;

            List<string> goodList = new List<string>();
            var cacheList = orderCache.Menus;
            foreach (var item in cacheList)
            {
                var stock = await CheckStock(new CheckStockInput { GoodsNo = item.GoodsNo, Qty = item.Qty });
                if (stock.IsSuccess && !stock.Data.HasStock)
                {
                    goodList.Add(item.GoodsName);
                }
            }

            if (goodList.Count > 0)
            {
                result.HasStock = false;
                result.Message = string.Format("{0} 库存不足", string.Join(",", goodList.ToArray()));
                return new Result<GetOrderStockDto>(ResultCode.Ok, result);
            }

            result.HasStock = true;
            result.Message = "库存足够";

            return new Result<GetOrderStockDto>(ResultCode.Ok, result);
        }

        /// <summary>
        /// 订单报表
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<Result<List<GetOrderReportDto>>> GetOrderReport(GetOrderReportInput input)
        {
            var sqlInput = new GetOrderReportSqlInput();
            sqlInput.OrderNo = input.OrderNo == null ? "" : input.OrderNo.Trim();
            sqlInput.StoreNo = FishSession.StorageNo;

            var orderStatus = GetSqlOrderStatus(input.OrderStatus);
            var orderStatusSql = $"and t3.nOrderStatusID in ({orderStatus})";

            var sql = string.Format(@"select OrderNo = t3.sPaperNO,
GoodsName = t4.sGoodsDesc,
PayType = t3.sPayType,
TradeTime = CONVERT(varchar(100), t1.dTradeDate, 20),
TableNum = t3.sTableNum,
Qty = t2.nSaleQty,
SalePrice = t2.nSalePrice,
PayPrice = t2.nSaleAmount,
OrderStatus = t3.sOrderStatus,
TradeStatus = (case t1.nTradeStatus when 0 then '未完成' when 1 then '赠品发放' when 2 then '完成交易'  else '未知' end),
TradeType = (case t1.nTradeType when 0 then case when t3.nOrderStatusID = 40 then '已退单'  else '销售' end when 1 then '赠品发放' when 2 then '退货销退' when 4 then '培训模式' else '未知' end),
PrintStatus = (case isnull(t5.status,0) when 0 then '未打印' else '已打印' end),
ItemNo = t2.nItem  
from tFishSale t1
inner join tFishSaleDtl t2 on (t1.sStoreNO = t2.sStoreNO and t1.nSerID = t2.nSerID and t1.dTradeDate = t2.dTradeDate and t1.sFishNO = t2.sFishNO)
inner join tStoreAppOrder t3 on (t1.sStoreNO = t3.sStoreNO and t1.sFishNO = t3.sFishSaleFishNO and t1.nSerID = t3.nFishSaleSerID and t1.dTradeDate = t3.dFishSaleTradeDate)
inner join tGoods as t4 on t2.nGoodsID = t4.nGoodsID 
left join ft_phoneOrder t5 on (t5.storeno = t1.sStoreNO and t5.serid = t1.nSerID and t5.posno = t1.sFishNO and t5.tradeTime = CONVERT(varchar(100), GETDATE(), 23))  
where t1.dTradeDate >= CONVERT(varchar(100), GETDATE(), 23)
and t1.sStoreNO = @StoreNo
and t3.sPaperNO like @OrderNo 
{0}
order by OrderNo desc,ItemNo", orderStatusSql);

            var result = await _parkContext.ExecuteFunctionAsync<List<GetOrderReportDto>>(sql, sqlInput);

            return Result.FromData(result);
        }

        /// <summary>
        /// 获取新的订单
        /// </summary>
        /// <returns></returns>
        public async Task<Result<List<GetNewOrderDto>>> GetNewOrder()
        {
            var input = new { StoreNo = FishSession.StorageNo };
            var sql = @"select StoreNo = t1.sStoreNO,
OrderNo = t1.sPaperNO,
TableNum = t1.sTableNum,
PayPrice = t1.nPayPrice,
TradeTime = CONVERT(nvarchar(19),t1.dFishSaleTradeDate,20),
CteateTime = CONVERT(nvarchar(19),t1.dCreateDate,20),
GoodsId = t2.nGoodsID,
GoodsName = t3.sGoodsDesc,
BarCode = t3.sMainBarcode,
Qty = t2.nGoodsQty,
SalePrice = t2.nRealSalePrice 
from tStoreAppOrder t1
left join tStoreAppOrderDtl t2 on t1.sPaperNO = t2.sPaperNO
left join tStoreAppGoods t3 on (t2.sStoreNO = t3.sStoreNO and t2.nGoodsID = t3.nGoodsID)
where t1.nTag = 0 and t1.nOrderStatusID = 30 
and t1.dCreateDate >= CONVERT(varchar(100), GETDATE(), 23)
and t1.sStoreNO = @StoreNo";

            var list = await _parkContext.ExecuteFunctionAsync<List<GetNewOrderOutput>>(sql, input);
            var result = new List<GetNewOrderDto>();
            if (list?.Count > 0)
            {
                //获取最新订单号
                var maxOrder = list.Max(w => w.OrderNo);
                var maxCache = _cacheManager.GetNewOrderCache(FishSession.StorageNo);
                if (maxOrder == maxCache?.MaxOrderNo)
                {
                    return Result.FromData(maxCache.Data);
                }

                //最新订单
                var orders = list.Where(w => w.OrderNo == maxOrder).ToList();
                var goods = new List<GetNewOrderGoods>();
                foreach (var goodsItem in orders)
                {
                    var newGoods = new GetNewOrderGoods();
                    newGoods.BarCode = goodsItem.BarCode;
                    newGoods.Id = goodsItem.GoodsId;
                    newGoods.Name = goodsItem.GoodsName;
                    newGoods.Qty = goodsItem.Qty;
                    newGoods.SalePrice = goodsItem.SalePrice;
                    goods.Add(newGoods);
                }

                var order = orders.FirstOrDefault();
                var dto = new GetNewOrderDto();
                dto.CteateTime = order.CteateTime;
                dto.Goods = goods;
                dto.OrderNo = order.OrderNo;
                dto.PayPrice = order.PayPrice;
                dto.StoreNo = order.StoreNo;
                dto.TableNum = order.TableNum;
                dto.TradeTime = order.TradeTime;
                result.Add(dto);

                maxCache = new NewOrderCache();
                maxCache.Data = result;
                maxCache.MaxOrderNo = maxOrder;
                _cacheManager.SetNewOrderCache(FishSession.StorageNo, maxCache);
            }
            return Result.FromData(result);
        }
    }
}
