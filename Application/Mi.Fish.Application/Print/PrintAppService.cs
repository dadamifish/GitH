using Abp.Runtime.Caching;
using Mi.Fish.ApplicationDto;
using Mi.Fish.EntityFramework;
using Mi.Fish.Infrastructure.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Mi.Fish.Application.Print
{
    /// <summary>
    /// 
    /// </summary>
    public class PrintAppService : AppServiceBase, IPrintAppService
    {
        private readonly ParkDbContext _parkContext;
        private readonly ICacheManager _cacheManager;

        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="parkContext"></param>
        /// <param name="cacheManager"></param>
        public PrintAppService(ParkDbContext parkContext, ICacheManager cacheManager)
        {
            _parkContext = parkContext;
            _cacheManager = cacheManager;
        }

        /// <summary>
        /// 订单重打印
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<Result<RePrintDto>> RePrint(RePrintInput input)
        {
            int flag = 0;
            if (input.OrderNo.Length != 7 || !int.TryParse(input.OrderNo, out flag))
            {
                return Result.Fail<RePrintDto>("订单号有误！");
            }
            //机号
            var pos = input.OrderNo.Substring(0, 3);
            //订单序列号
            var order = int.Parse(input.OrderNo.Substring(3));
            //用户信息
            var userData = _cacheManager.GetUserDataCacheByUserId(FishSession.UserId);

            //获取订单信息
            var sql = @"select TradeTime = dTradeDate,Cashier = sCashierNO,PayPrice = nSaleAmount,SaleMode = nTradeType
from tFishSale t1 
where CONVERT(varchar(10),t1.dTradeDate,120) = CONVERT(varchar(10),GETDATE(),120)
and t1.sStoreNO = @StoreNo and t1.sFishNO = @Fish and t1.nSerID = @Order";
            var sqlInput = new
            {
                StoreNo = userData.StorageNo,
                Fish = pos,
                Order = order
            };
            var sale = await _parkContext.ExecuteFunctionAsync<RePrintSaleDto>(sql, sqlInput);
            if (sale == null)
            {
                return Result.Fail<RePrintDto>($"{input.OrderNo}订单不存在！");
            }
            var dto = new RePrintDto();
            dto.Cashier = sale.Cashier;
            dto.TradeTime = sale.TradeTime.ToString("yyyy-MM-dd HH:mm:ss");
            dto.PayAbleAmount = sale.SaleMode == "0" ? sale.PayPrice : 0;
            dto.SaleMode = sale.SaleMode == "0" ? "销售" : "退货";
            dto.TerminalID = pos;
            dto.OrderNo = input.OrderNo;

            //获取订单商品信息
            sql = @"select GoodsName = t2.sGoodsName,Qty = t1.nSaleQty,PayPrice = t1.nSaleAmount 
from tFishSaleDtl t1
inner join tGoods t2 on t1.nGoodsID = t2.nGoodsID
where CONVERT(varchar(10),t1.dTradeDate,120) = CONVERT(varchar(10),GETDATE(),120)
and t1.sStoreNO = @StoreNo and t1.sFishNO = @Fish and t1.nSerID = @Order";
            var goods = await _parkContext.ExecuteFunctionAsync<List<RePrintGoodsDto>>(sql, sqlInput);
            if (sale.SaleMode == "2")
            {
                goods.ForEach(w =>
                  {
                      w.PayPrice = Math.Abs(w.PayPrice);
                      w.Qty = Math.Abs(w.Qty);
                  });
            }

            //获取支付信息
            sql = @"select PayTypeId = sPayTypeID,PayType = sPayType,TradeNo = ISNULL(sPaytNO,''),PayPrice = nPaytAmount  
from tFishPayt t1
where CONVERT(varchar(10),t1.dTradeDate,120) = CONVERT(varchar(10),GETDATE(),120)
and t1.sStoreNO = @StoreNo and t1.sFishNO = @Fish and t1.nSerID = @Order";
            var payList = await _parkContext.ExecuteFunctionAsync<List<PaySqlDto>>(sql, sqlInput);

            var payDto = new List<PayMent>();

            var thirdList = new List<string>();
            var thirdEnum = Enum.GetValues(typeof(ThirdPayType));
            foreach (var item in thirdEnum)
            {
                var enumItem = Enum.Parse(typeof(ThirdPayType), item.ToString());
                var enumValue = ((int)enumItem).ToString();
                thirdList.Add(enumValue);
            }

            //计算各种支付类型支付金额(找零不计)
            foreach (var item in payList)
            {
                if (sale.SaleMode == "0")
                {
                    if (item.PayPrice > 0)
                    {
                        var payMent = new PayMent();
                        payMent.PayName = item.PayType;
                        payMent.Amount = item.PayPrice.ToString("N2");
                        payDto.Add(payMent);
                        if (string.IsNullOrEmpty(dto.ThirdTradeNo))
                        {
                            if (thirdList.Contains(item.PayTypeId))
                            {
                                dto.ThirdTradeNo = item.TradeNo;
                            }
                        }
                    }
                }
                else
                {
                    var payMent = new PayMent();
                    payMent.PayName = item.PayType;
                    payMent.Amount = Math.Abs(item.PayPrice).ToString("N2");
                    payDto.Add(payMent);
                    if (string.IsNullOrEmpty(dto.ThirdTradeNo))
                    {
                        if (thirdList.Contains(item.PayTypeId))
                        {
                            dto.ThirdTradeNo = item.TradeNo;
                        }
                    }
                }
            }

            //实付
            dto.PayAmount = sale.SaleMode == "0" ? payDto.Sum(w => Convert.ToDecimal(w.Amount)) : 0.00M;

            //找零
            if (sale.SaleMode == "0")
            {
                var obj = payList.FirstOrDefault(w => w.PayTypeId == ((int)PayType.Cash).ToString() && w.PayPrice < 0);
                dto.ReturnMoney = Math.Abs(obj?.PayPrice ?? 0);
            }
            else
            {
                dto.ReturnMoney = Math.Abs(sale.PayPrice);
            }

            dto.Goods = goods;
            dto.PayMents = payDto;

            return Result.FromData(dto);
        }
    }
}
