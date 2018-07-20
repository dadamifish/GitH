using Abp.AutoMapper;
using Abp.Runtime.Caching;
using Mi.Fish.Application.Order;
using Mi.Fish.ApplicationDto;
using Mi.Fish.EntityFramework;
using Mi.Fish.Infrastructure.Results;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using Mi.Fish.Application.Sync;
using Mi.Fish.Common;

namespace Mi.Fish.Application.SaleMenu
{
    /// <summary>
    /// POS销售菜单应用服务
    /// </summary>
    public class SaleMenuAppService : AppServiceBase, ISaleMenuAppService
    {
        #region fileds

        private readonly ParkDbContext _parkContext;
        private readonly LocalDbContext _localContext;
        private readonly ICacheManager _cacheManager;
        private readonly ICookProvider _cookSetting;
        private readonly IOrderNoAppService _orderNoAppService;
        private readonly IPayAppService _payAppService;
        private readonly ISyncAppService _syncAppService;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="parkContext"></param>
        /// <param name="localContext"></param>
        /// <param name="cacheManager"></param>
        /// <param name="settings"></param>
        public SaleMenuAppService(ParkDbContext parkContext, LocalDbContext localContext, ICacheManager cacheManager, IOrderNoAppService orderNoAppService, IPayAppService payAppService, ISyncAppService syncAppService, ICookProvider settings)
        {
            _parkContext = parkContext;
            _localContext = localContext;
            _cacheManager = cacheManager;
            _cookSetting = settings;
            _orderNoAppService = orderNoAppService;
            _payAppService = payAppService;
            _syncAppService = syncAppService;
        }

        #endregion

        #region private function

        /// <summary>
        /// 从本地库获取销售商品
        /// </summary>
        /// <param name="goodsNo"></param>
        /// <param name="userData"></param>
        /// <returns></returns>
        private async Task<GoodsMenu> GetSaleGoods(string goodsNo, UserData userData)
        {

            var sql = @"select GoodsNo = sGoodsNO,GoodsName = sGoodsName,SalePrice = nRealSalePrice, DefaultPrice = nSalePrice, CostPrice = cost_price, 
                        BarCode = sMainBarcode,GoodsType = sGoodTypeID, CategoryType = isnull(substring(sPath,dbo.fn_find('/',sPath,2)+1,dbo.fn_find('/',sPath,2+1)-dbo.fn_find('/',sPath,2)-1),'')
                        from ft_v_SaleGoods where sGoodsNo = @GoodsNo and sStoreNO = @StoreNo";
            var sqlInput = new { GoodsNo = goodsNo, StoreNo = userData.StorageNo };
            var goodsMenu = await _localContext.ExecuteFunctionAsync<GoodsMenu>(sql, sqlInput);
            if (goodsMenu == null)
            {
                return null;
            }

            goodsMenu.MealSubs = new List<MealSub>();
            //判断促销套餐
            var hasPromotion = false;
            if (!string.IsNullOrEmpty(userData.PromotionName))
            {
                var csql = @"select CategoryDesc= sCategoryDesc from tCategory where sCategoryno = @sCategoryno";
                var csqlInput = new { sCategoryno = goodsMenu.CategoryType };
                var categoryList = await _localContext.ExecuteFunctionAsync<List<GoodsCategory>>(csql, csqlInput);
                hasPromotion = categoryList.Any(t => t.CategoryDesc == userData.PromotionName);
            }
            if (goodsMenu.GoodsType == "C" || hasPromotion)
            {
                sql = @"SELECT GoodsNo = b.sGoodsNO,GoodsName = b.sGoodsName,SalePrice = b.nRealSalePrice,DefaultPrice = nSalePrice,Qty = Convert(int,a.nQty),
                        MealQty = Convert(int,a.nQty), MasterNo = (select sGoodsNO from tGoods where nGoodsID=a.nGoodsID) FROM tComplexElement  
                         a left join ft_v_SaleGoods b on a.nElementID=b.nGoodsID and b.sStoreNO = @StoreNo WHERE
                         (select sGoodsNO from tGoods where a.nTag&1=0 and nGoodsID=a.nGoodsID)= @GoodsNo 
                         ORDER BY a.nElementID ASC ";
                var mealSubs = await _parkContext.ExecuteFunctionAsync<List<MealSub>>(sql, sqlInput);
                if (mealSubs != null)
                {
                    goodsMenu.MealSubs = mealSubs;
                }
            }

            return goodsMenu;
        }

        /// <summary>
        /// 获取商品合计金额
        /// 清空购买商品和删除商品无需调用此方法
        /// </summary>
        /// <param name="input"></param>
        /// <param name="orderNo"></param>
        /// <param name="userData"></param>
        /// <returns></returns>
        private GetGoodsTotalPriceDto GetGoodsTotalPrice(List<GoodsMenu> input, string orderNo, UserData userData)
        {

            var sqlInput = new DiscountProcInput();
            sqlInput.cashierno = userData.UserID;
            sqlInput.icno = userData.icno;
            sqlInput.receiptid = orderNo;
            sqlInput.saleway = ((int)EnumSaleWay.Sale).ToString();
            sqlInput.storeno = userData.StorageNo;
            //组合套餐||普通商品
            input.Where(w => w.GoodsType == "S" || w.GoodsType == "C").ToList().ForEach(w =>
            {
                sqlInput.barcodes += w.BarCode + "-";
                sqlInput.rates += (w.Discount / 100.00).ToString("0.00") + "-";
                sqlInput.goodsids += w.GoodsNo + "-";
                sqlInput.prices += w.SalePrice + "-";
                sqlInput.qtys += w.Qty + "-";
            });

            var sql = @"exec ft_pr_saletmp_ipad @storeno,@receiptid,@cashierno,@saleway,@barcodes,@qtys,@prices,@goodsids,@icno,@rates";
            var dataSet = _localContext.GetDataSet(sql, sqlInput);
            var flag = Convert.ToDecimal(dataSet.Tables[0].Rows[0][0]);
            if (flag == -1)
            {
                throw new Abp.UI.UserFriendlyException("出现单号重复，请重新获取单号！");
            }
            var dto = new GetGoodsTotalPriceDto();
            dto.DiscountAmount = Convert.ToDecimal(dataSet.Tables[dataSet.Tables.Count - 1].Rows[0]["dis"]);
            dto.PayAmount = Convert.ToDecimal(dataSet.Tables[dataSet.Tables.Count - 1].Rows[0]["sumamount"]);
            dto.SaleAmount = Convert.ToDecimal(dataSet.Tables[dataSet.Tables.Count - 1].Rows[0]["TotalAmount"]);
            dto.TotalQty = Convert.ToInt32(dataSet.Tables[dataSet.Tables.Count - 1].Rows[0]["totalq"]);
            dto.UnpaidAmount = Convert.ToDecimal(dataSet.Tables[dataSet.Tables.Count - 1].Rows[0]["topay"]);
            return dto;
        }

        /// <summary>
        /// 检测商品库存
        /// </summary>
        /// <param name="input"></param>
        /// <param name="userData"></param>
        /// <returns></returns>
        private async Task<Result<GetRealStockDto>> CheckStock(CheckStockInput input, UserData userData)
        {
            var qty = input?.Qty ?? 1;
            qty = qty >= 0 ? qty : 0;
            var dto = new GetRealStockDto
            {
                GoodsNo = input?.GoodsNo
            };

            var sql = "select HavaStock = havestock,VirtualStock = guqingqty from base_entry_message_allow WHERE  activeflag=1 and branch_no=@branch_no and jh=@jh AND item_no=@item_no";
            object sqlInput = new { branch_no = userData.StorageNo, jh = userData.TerminalID, item_no = input.GoodsNo };
            var messageAllow = await _localContext.ExecuteFunctionAsync<SqlVirtualStock>(sql, sqlInput);

            //判断估库存
            if (messageAllow != null)
            {
                if (messageAllow.VirtualStock > 0 && messageAllow.VirtualStock < qty)
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
            if (messageAllow == null || messageAllow.HavaStock == 0)
            {
                sql = "exec p_get_nowstock @branch_no,@item_no";
                sqlInput = new { branch_no = userData.StorageNo, item_no = input.GoodsNo };
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

        #endregion

        /// <summary>
        /// 获取当前购物车数据
        /// </summary>
        /// <returns></returns>
        public async Task<Result<SaleMenuDto>> GetMenu()
        {
            var orderNo = await _orderNoAppService.CreateOrderNo();
            var menuCache = _cacheManager.GetSaleMenuCache(FishSession.UserId) ?? new List<SaleMenuCache>();
            var orderCache = menuCache.FirstOrDefault(w => w.OrderNo == orderNo.Data) ?? new SaleMenuCache();

            var result = orderCache.MapTo<SaleMenuDto>();
            return Result.FromData(result);
        }

        /// <summary>
        /// 新增商品栏
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<Result<SaleMenuDto>> AddGoods(SaleMenuInput input)
        {
            string userId = FishSession.UserId;
            var userData = _cacheManager.GetUserDataCacheByUserId(userId);
            var orderNo = await _orderNoAppService.CreateOrderNo();
            var goodsNo = input.GoodsNo.Trim();

            //判断是否正在付款中
            var orderPay = _cacheManager.GetPayDataCache(userId)?.FirstOrDefault(w => w.OrderNo == orderNo.Data);
            if (orderPay != null && orderPay.PaidAmount > 0)
            {
                return Result.Fail<SaleMenuDto>("正在付款中，不允许操作产品数据！");
            }

            var menuCache = _cacheManager.GetSaleMenuCache(userId) ?? new List<SaleMenuCache>();
            var orderCache = menuCache.FirstOrDefault(w => w.OrderNo == orderNo.Data);
            var qty = 1;

            if (orderCache != null)
            {
                //存在该订单缓存
                var goodsCache = orderCache.Menus.FirstOrDefault(w => w.GoodsNo == goodsNo);
                if (goodsCache != null)
                {
                    //已存在该商品，更改购买数量
                    goodsCache.Qty += 1;
                    qty = goodsCache.Qty;
                    goodsCache.MealSubs?.ForEach(t =>
                    {
                        t.Qty += t.MealQty;
                    });
                }
                else
                {
                    //不存在，新增该商品到缓存
                    var saleGoods = await GetSaleGoods(goodsNo, userData);
                    if (saleGoods == null)
                    {
                        return Result.Fail<SaleMenuDto>($"sGoodsNO为{goodsNo}的商品不存在!");
                    }
                    orderCache.Menus.Add(saleGoods);
                }
            }
            else
            {
                //不存在该订单缓存
                var saleGoods = await GetSaleGoods(goodsNo, userData);
                if (saleGoods == null)
                {
                    return Result.Fail<SaleMenuDto>($"sGoodsNO为{goodsNo}的商品不存在!");
                }
                var menus = new List<GoodsMenu>();
                menus.Add(saleGoods);

                orderCache = new SaleMenuCache();
                orderCache.OrderNo = orderNo.Data;
                orderCache.Menus = menus;
                menuCache.Add(orderCache);
            }

            //库存判断
            var stock = await CheckStock(new CheckStockInput
            {
                GoodsNo = goodsNo,
                Qty = qty
            }, userData);
            if (!stock.Data.HasStock)
            {
                return Result.Fail<SaleMenuDto>(stock.Data.Message);
            }

            orderCache.DiscountAmount = GetGoodsTotalPrice(orderCache.Menus, orderNo.Data, userData).DiscountAmount;
            _cacheManager.SetSaleMenuCache(FishSession.UserId, menuCache);

            var result = orderCache.MapTo<SaleMenuDto>();
            return Result.FromData(result);
        }

        /// <summary>
        /// 修改商品购买数量
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<Result<SaleMenuDto>> UpdateGoodsCount(UpdateGoodsCountInput input)
        {
            string userId = FishSession.UserId;
            var userData = _cacheManager.GetUserDataCacheByUserId(userId);
            var orderNo = await _orderNoAppService.CreateOrderNo();
            var goodsNo = input.GoodsNo.Trim();

            //判断是否正在付款中
            var orderPay = _cacheManager.GetPayDataCache(userId)?.FirstOrDefault(w => w.OrderNo == orderNo.Data);
            if (orderPay != null && orderPay.PaidAmount > 0)
            {
                return Result.Fail<SaleMenuDto>("正在付款中，不允许操作产品数据！");
            }

            try
            {
                var menuCache = _cacheManager.GetSaleMenuCache(FishSession.UserId);
                var orderCache = menuCache.FirstOrDefault(w => w.OrderNo == orderNo.Data);
                var goodsCache = orderCache?.Menus.SingleOrDefault(w => w.GoodsNo == goodsNo);

                if (goodsCache == null)
                {
                    return Result.Fail<SaleMenuDto>("该商品数据失效，请重新移除添加！");
                }

                if (input.Type == UpdateGoodsCountType.Plus)
                {
                    goodsCache.Qty += input.Qty;
                }
                else
                {
                    goodsCache.Qty = input.Qty;
                }

                //更新套餐子项
                if (goodsCache.GoodsType == "C")
                {
                    goodsCache.MealSubs?.ForEach(a => a.Qty = a.MealQty * goodsCache.Qty);
                }

                //重新判断库存
                if (goodsCache.Qty > 0)
                {
                    var stock = await CheckStock(new CheckStockInput
                    {
                        GoodsNo = goodsCache.GoodsNo,
                        Qty = goodsCache.Qty
                    }, userData);
                    if (!stock.Data.HasStock)
                    {
                        return Result.Fail<SaleMenuDto>(stock.Data.Message + "，无法修改数量！");
                    }
                }
                else
                {
                    orderCache.Menus.Remove(goodsCache);
                }

                if (orderCache.Menus.Count > 0)
                {
                    orderCache.DiscountAmount = GetGoodsTotalPrice(orderCache.Menus, orderNo.Data, userData).DiscountAmount;
                }
                _cacheManager.SetSaleMenuCache(FishSession.UserId, menuCache);

                var result = orderCache.MapTo<SaleMenuDto>();
                return Result.FromData(result);
            }
            catch
            {
                return Result.Fail<SaleMenuDto>("缓存数据失效，请重新销售！");
            }
        }

        /// <summary>
        /// 移除该商品
        /// </summary>
        /// <param name="goodsNo"></param>
        /// <returns></returns>
        public async Task<Result<SaleMenuDto>> RemoveGoods(string goodsNo)
        {
            string userId = FishSession.UserId;
            var userData = _cacheManager.GetUserDataCacheByUserId(userId);
            var orderNo = await _orderNoAppService.CreateOrderNo();

            //判断是否正在付款中
            var orderPay = _cacheManager.GetPayDataCache(userId)?.FirstOrDefault(w => w.OrderNo == orderNo.Data);
            if (orderPay != null && orderPay.PaidAmount > 0)
            {
                return Result.Fail<SaleMenuDto>("正在付款中，不允许操作产品数据！");
            }

            try
            {
                var menuCache = _cacheManager.GetSaleMenuCache(FishSession.UserId);
                var orderCache = menuCache.FirstOrDefault(w => w.OrderNo == orderNo.Data);
                var goodsCache = orderCache?.Menus.SingleOrDefault(w => w.GoodsNo == goodsNo);

                if (goodsCache == null)
                {
                    return Result.Fail<SaleMenuDto>("该商品数据失效，请重新获取商品数据！");
                }

                orderCache.Menus.Remove(goodsCache);
                if (orderCache.Menus.Count > 0)
                {
                    orderCache.DiscountAmount = GetGoodsTotalPrice(orderCache.Menus, orderNo.Data, userData).DiscountAmount;
                }
                _cacheManager.SetSaleMenuCache(FishSession.UserId, menuCache);

                var result = orderCache.MapTo<SaleMenuDto>();
                return Result.FromData(result);
            }
            catch
            {
                return Result.Fail<SaleMenuDto>("缓存数据失效，请重新销售！");
            }
        }

        /// <summary>
        /// 清空购买商品
        /// </summary>
        /// <returns></returns>
        public async Task<Result<SaleMenuDto>> ClearMenu()
        {
            string userId = FishSession.UserId;
            var orderNo = await _orderNoAppService.CreateOrderNo();
            var menuCache = _cacheManager.GetSaleMenuCache(userId) ?? new List<SaleMenuCache>();
            var orderCache = menuCache.FirstOrDefault(w => w.OrderNo == orderNo.Data);
            var payCache = _cacheManager.GetPayDataCache(userId) ?? new List<OrderPayInfo>();
            var orderPay = payCache.FirstOrDefault(w => w.OrderNo == orderNo.Data);

            menuCache.Remove(orderCache);
            _cacheManager.SetSaleMenuCache(userId, menuCache);
            payCache.Remove(orderPay);
            _cacheManager.SetPayDataCache(userId, payCache);

            if (orderCache != null)
            {
                //清空临时数据
                await ClearTempItem(orderNo.Data);
            }
            return Result.FromData(new SaleMenuDto());
        }

        /// <summary>
        /// 单项打折
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<Result<SaleMenuDto>> SingleDiscount(SingleDiscountInput input)
        {
            string userId = FishSession.UserId;
            var userData = _cacheManager.GetUserDataCacheByUserId(userId);
            var orderNo = await _orderNoAppService.CreateOrderNo();
            var goodsNo = input.GoodsNo.Trim();

            //打折权限判断
            var checkPermission = _orderNoAppService.CheckPermission(EnumAuthorityType.SingleDiscount);
            if (!checkPermission.Data)
            {
                return Result.Fail<SaleMenuDto>("无单项打折权限");
            }

            //判断是否正在付款中
            var orderPay = _cacheManager.GetPayDataCache(userId)?.FirstOrDefault(w => w.OrderNo == orderNo.Data);
            if (orderPay != null && orderPay.PaidAmount > 0)
            {
                return Result.Fail<SaleMenuDto>("正在付款中，不允许操作产品数据！");
            }

            try
            {
                var menuCache = _cacheManager.GetSaleMenuCache(FishSession.UserId);
                var orderCache = menuCache.FirstOrDefault(w => w.OrderNo == orderNo.Data);
                var goodsCache = orderCache?.Menus.FirstOrDefault(w => w.GoodsNo == goodsNo);

                if (goodsCache == null)
                {
                    return Result.Fail<SaleMenuDto>("该商品数据失效，请重新移除添加！");
                }

                if (goodsCache.GoodsType == "C")
                {
                    return Result.Fail<SaleMenuDto>("套餐商品，不允许打折！");
                }

                //if (goodsCache.SalePrice * (decimal)(input.Discount * 0.01) < goodsCache.CostPrice)
                //{
                //    return Result.Fail<SaleMenuDto>("该商品折扣后价格不能低于成本价！");
                //}

                goodsCache.Discount = input.Discount;
                orderCache.DiscountAmount = GetGoodsTotalPrice(orderCache.Menus, orderNo.Data, userData).DiscountAmount;
                _cacheManager.SetSaleMenuCache(FishSession.UserId, menuCache);

                var result = orderCache.MapTo<SaleMenuDto>();
                return Result.FromData(result);
            }
            catch
            {
                return Result.Fail<SaleMenuDto>("缓存数据失效，请重新销售！");
            }
        }

        /// <summary>
        /// 全部打折
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<Result<SaleMenuDto>> AllDiscount(AllDiscountInput input)
        {
            string userId = FishSession.UserId;
            var userData = _cacheManager.GetUserDataCacheByUserId(userId);
            var orderNo = await _orderNoAppService.CreateOrderNo();

            //打折权限判断
            var checkPermission = _orderNoAppService.CheckPermission(EnumAuthorityType.AllDiscount);
            if (!checkPermission.Data)
            {
                return Result.Fail<SaleMenuDto>("无全部打折权限");
            }

            //判断是否正在付款中
            var orderPay = _cacheManager.GetPayDataCache(userId)?.FirstOrDefault(w => w.OrderNo == orderNo.Data);
            if (orderPay != null && orderPay.PaidAmount > 0)
            {
                return Result.Fail<SaleMenuDto>("正在付款中，不允许操作产品数据！");
            }

            try
            {
                if ((decimal)(input.Discount * 0.01) < userData.Min_ZK)
                {
                    return Result.Fail<SaleMenuDto>("折扣最小不能低于" + $"{userData.Min_ZK * 100}" + "折！");
                }

                var menuCache = _cacheManager.GetSaleMenuCache(FishSession.UserId);
                var orderCache = menuCache.FirstOrDefault(w => w.OrderNo == orderNo.Data);

                if (orderCache == null)
                {
                    return Result.Fail<SaleMenuDto>("商品数据失效，请重新获取商品数据！");
                }

                //排除套餐商品
                var hasSingle = false;
                orderCache.Menus.ForEach(t =>
                {
                    if (t.GoodsType != "C")
                    {
                        hasSingle = true;
                        t.Discount = input.Discount;
                    }
                });

                if (!hasSingle)
                {
                    return Result.Fail<SaleMenuDto>("套餐商品，不允许打折！");
                }

                orderCache.DiscountAmount = GetGoodsTotalPrice(orderCache.Menus, orderNo.Data, userData).DiscountAmount;
                _cacheManager.SetSaleMenuCache(FishSession.UserId, menuCache);

                var result = orderCache.MapTo<SaleMenuDto>();
                return Result.FromData(result);
            }
            catch
            {
                return Result.Fail<SaleMenuDto>("缓存数据失效，请重新销售！");
            }
        }

        /// <summary>
        /// 订单商品销售总额（不参与折扣）
        /// </summary>
        /// <returns></returns>
        public async Task<Result<ActualMenu>> GetActualMenu()
        {
            string userId = FishSession.UserId;
            var userData = _cacheManager.GetUserDataCacheByUserId(userId);
            var orderNo = await _orderNoAppService.CreateOrderNo();
            var menuCache = _cacheManager.GetSaleMenuCache(FishSession.UserId);
            var orderCache = menuCache?.FirstOrDefault(w => w.OrderNo == orderNo.Data);

            if (orderCache == null)
            {
                return Result.Fail<ActualMenu>("无发现订单数据！");
            }

            orderCache.Menus.ForEach(t =>
            {
                if (userData.LeyouPrice == ((int)EnumPriceType.Default).ToString())
                {
                    t.SalePrice = t.DefaultPrice;
                }
            });

            var result = new ActualMenu
            {
                SaleAmount = orderCache.SaleAmount
            };

            return Result.FromData(result);
        }

        /// <summary>
        /// 获取当前已付款信息
        /// </summary>
        /// <returns></returns>
        public async Task<Result<SMPayDto>> GetPay()
        {
            var orderNo = await _orderNoAppService.CreateOrderNo();
            var payCache = _cacheManager.GetPayDataCache(FishSession.UserId) ?? new List<OrderPayInfo>();
            var orderPay = payCache.FirstOrDefault(w => w.OrderNo == orderNo.Data) ?? new OrderPayInfo();

            SMPayDto result = new SMPayDto
            {
                IsComplete = false,
                PayInfos = GetPayInfos(orderPay,true)
            };

            return Result.FromData(result);
        }

        /// <summary>
        /// 现金支付
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<Result<SMPayDto>> CashPay(CashPayInput input)
        {
            SMPayDto result = new SMPayDto();
            OrderSaleET et = new OrderSaleET();

            string userId = FishSession.UserId;
            var userData = _cacheManager.GetUserDataCacheByUserId(userId);
            var orderNo = await _orderNoAppService.CreateOrderNo();
            var menuCache = _cacheManager.GetSaleMenuCache(FishSession.UserId);
            var orderCache = menuCache?.FirstOrDefault(w => w.OrderNo == orderNo.Data);

            if (orderCache == null)
            {
                return Result.Fail<SMPayDto>("没发现支付数据，请重新操作");
            }

            if ((input.CashType == CashPayType.WxWai || input.CashType == CashPayType.AliWai) 
                && input.PayAmount == 0)
            {
                return Result.Fail<SMPayDto>("支付金额需大于0");
            }

            var payCache = _cacheManager.GetPayDataCache(FishSession.UserId) ?? new List<OrderPayInfo>();
            var orderPay = payCache.FirstOrDefault(w => w.OrderNo == orderNo.Data);

            if (orderPay == null)
            {
                orderPay = new OrderPayInfo { OrderNo = orderNo.Data, TotalAmount = orderCache.PayAmount };
                payCache.Add(orderPay);
            }

            et.flow_no = orderNo.Data;
            orderPay.TableNo = input.TableNo;

            if (input.CashType == CashPayType.Cash)
            {
                orderPay.CashAmount = input.PayAmount;
            }
            else if (input.CashType == CashPayType.CashCoupon)
            {
                //代金券大于应付当应付金额使用
                orderPay.VoucherAmount = input.PayAmount > orderPay.PayableAmount ? orderPay.PayableAmount : input.PayAmount;
            }
            else if (input.CashType == CashPayType.AliWai)
            {
                orderPay.ALiPayWaiAmount = input.PayAmount;
                if (orderPay.ALiPayWaiAmount != orderPay.PaidAmount || orderPay.PayableAmount > 0)
                {
                    return Result.Fail<SMPayDto>("付款金额必须只能由支付宝（外）全额付款，");
                }
                if (orderPay.GiveAmount > 0)
                {
                    return Result.Fail<SMPayDto>("使用支付宝（外）付款，支付总额不能超过购物金额");
                }
            }
            else if (input.CashType == CashPayType.WxWai)
            {
                orderPay.WXWaiAmount = input.PayAmount;
                if (orderPay.WXWaiAmount != orderPay.PaidAmount || orderPay.PayableAmount > 0)
                {
                    return Result.Fail<SMPayDto>("付款金额必须只能由微信（外）全额付款");
                }
                if (orderPay.GiveAmount > 0)
                {
                    return Result.Fail<SMPayDto>("使用微信（外）付款，支付总额不能超过购物金额");
                }
            }

            if (orderPay.PayableAmount > 0)
            {
                result.IsComplete = false;
                result.PayInfos = GetPayInfos(orderPay, true);

                //更新付款缓存
                _cacheManager.SetPayDataCache(FishSession.UserId, payCache);
            }
            else
            {
                //创建订单
                var res = await CreateOrder(et, orderCache, orderPay, userData);
                if (res.IsSuccess)
                {
                    result.IsComplete = true;
                    result.PayInfos = GetPayInfos(orderPay, true);
                    result.PrintInfo = GetPrintInfo(orderCache, orderPay, userData, false, "");

                    var newestOrderNo = await _orderNoAppService.CreateOrderNo();
                    result.NewestOrderNo = newestOrderNo.Data;

                    //订单付完成清空当前订单及付款缓存
                    menuCache.Remove(orderCache);
                    _cacheManager.SetSaleMenuCache(FishSession.UserId, menuCache);
                    payCache.Remove(orderPay);
                    _cacheManager.SetPayDataCache(FishSession.UserId, payCache);
                }
            }

            return new Result<SMPayDto>(ResultCode.Ok, result);
        }

        /// <summary>
        /// 第三方支付
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<Result<SMPayDto>> ThirdPay(ThirdPayInput input)
        {
            SMPayDto result = new SMPayDto();
            OrderSaleET et = new OrderSaleET();

            string userId = FishSession.UserId;
            var userData = _cacheManager.GetUserDataCacheByUserId(userId);
            var orderNo = await _orderNoAppService.CreateOrderNo();
            var menuCache = _cacheManager.GetSaleMenuCache(FishSession.UserId);
            var orderCache = menuCache?.FirstOrDefault(w => w.OrderNo == orderNo.Data);

            if (orderCache == null)
            {
                return Result.Fail<SMPayDto>("没发现支付数据，请重新操作");
            }

            var payCache = _cacheManager.GetPayDataCache(FishSession.UserId) ?? new List<OrderPayInfo>();
            var orderPay = payCache.FirstOrDefault(w => w.OrderNo == orderNo.Data);

            if (orderPay == null)
            {
                orderPay = new OrderPayInfo { OrderNo = orderNo.Data, TotalAmount = orderCache.PayAmount };
                payCache.Add(orderPay);
            }

            et.flow_no = orderNo.Data;
            var isMutoneOrFangtePay = false;
            orderPay.TableNo = input.TableNo;

            if (input.ThirdPay == ThirdPayType.Ft)
            {
                isMutoneOrFangtePay = true;
                try
                {
                    var actualMenu = await GetActualMenu();
                    //校验支付结果查询
                    var queryResult = await _payAppService.FtScanPayQuery(new PayQueryInput
                    {
                        PayMoney = actualMenu.Data.SaleAmount,
                        TradeNo = input.TradeNo
                    });
                    if (!queryResult.IsSuccess)
                    {
                        return Result.Fail<SMPayDto>(queryResult.Message);
                    }
                }
                catch (Exception ex)
                {
                    Logger.Error(ex.Message);
                    return Result.Fail<SMPayDto>("校验方特支付结果查询出错");
                }

                //方特支付借用秒能字段
                orderPay.LeYouScanAmount = input.PayAmount;
                orderPay.DiscountAmount = orderPay.TotalAmount - input.PayAmount;
                if (orderPay.LeYouScanAmount != orderPay.PaidAmount || orderPay.PayableAmount > 0)
                {
                    return Result.Fail<SMPayDto>("付款金额必须只能由方特支付全额付款");
                }
                et.mutoneorder = input.TradeNo;
                et.mutontradeno = input.PayOrderNo;
                et.mutonerate = decimal.Parse(string.Format("{0:N}", input.PayAmount / orderPay.TotalAmount));
                et.mutonedisAmount = orderPay.DiscountAmount;
            }
            else if (input.ThirdPay == ThirdPayType.Mutone)
            {
                isMutoneOrFangtePay = true;
                try
                {
                    var actualMenu = await GetActualMenu();
                    //校验支付结果查询
                    var queryResult = await _payAppService.MutonePayQuery(new PayQueryInput
                    {
                        PayMoney = actualMenu.Data.SaleAmount,
                        TradeNo = input.TradeNo
                    });
                    if (!queryResult.IsSuccess)
                    {
                        return Result.Fail<SMPayDto>(queryResult.Message);
                    }
                }
                catch (Exception ex)
                {
                    Logger.Error(ex.Message);
                    return Result.Fail<SMPayDto>("校验秒通支付结果查询出错");
                }

                orderPay.MuTonAmount = input.PayAmount;
                orderPay.DiscountAmount = orderPay.TotalAmount - input.PayAmount;
                if (orderPay.MuTonAmount != orderPay.PaidAmount || orderPay.PayableAmount > 0)
                {
                    return Result.Fail<SMPayDto>("付款金额必须只能由秒通全额付款");
                }

                et.mutoneorder = input.TradeNo;
                et.mutontradeno = input.PayOrderNo;
                et.mutonerate = decimal.Parse(string.Format("{0:N}", input.PayAmount / orderPay.TotalAmount));
                et.mutonedisAmount = orderPay.DiscountAmount;
            }
            else if (input.ThirdPay == ThirdPayType.Ali)
            {
                try
                {
                    //校验支付结果查询
                    var queryResult = await _payAppService.AliScanPayQuery(new PayQueryInput
                    {
                        PayMoney = input.PayAmount,
                        TradeNo = input.PayOrderNo
                    });
                    if (!queryResult.IsSuccess)
                    {
                        return Result.Fail<SMPayDto>(queryResult.Message);
                    }
                }
                catch (Exception ex)
                {
                    Logger.Error(ex.Message);
                    return Result.Fail<SMPayDto>("校验支付宝支付结果查询出错");
                }

                orderPay.ALiPayAmount = input.PayAmount;
                if (orderPay.PayableAmount > 0)
                {
                    return Result.Fail<SMPayDto>("选择支付宝支付，必须完成剩余金额付款");
                }

                et.altradeno = input.PayOrderNo;
            }
            else if (input.ThirdPay == ThirdPayType.Wx)
            {
                try
                {
                    //校验支付结果查询
                    var queryResult = await _payAppService.WxScanPayQuery(new PayQueryInput
                    {
                        PayMoney = input.PayAmount,
                        TradeNo = input.PayOrderNo
                    });
                    if (!queryResult.IsSuccess)
                    {
                        return Result.Fail<SMPayDto>(queryResult.Message);
                    }
                }
                catch (Exception ex)
                {
                    Logger.Error(ex.Message);
                    return Result.Fail<SMPayDto>("校验微信支付结果查询出错");
                }

                orderPay.WXAmount = input.PayAmount;
                if (orderPay.PayableAmount > 0)
                {
                    return Result.Fail<SMPayDto>("选择微信支付，必须完成剩余金额付款");
                }

                et.wxtradeno = input.PayOrderNo;
            }

            //判断支付完成状态
            if (orderPay.PayableAmount > 0)
            {
                result.IsComplete = false;
                result.PayInfos = GetPayInfos(orderPay, true);

                //更新付款缓存
                _cacheManager.SetPayDataCache(FishSession.UserId, payCache);
            }
            else
            {
                //创建订单
                var res = await CreateOrder(et, orderCache, orderPay, userData);
                if (res.IsSuccess)
                {
                    result.IsComplete = true;
                    result.PayInfos = GetPayInfos(orderPay, true);
                    result.PrintInfo = GetPrintInfo(orderCache, orderPay, userData, isMutoneOrFangtePay, input.PayOrderNo);

                    var newestOrderNo = await _orderNoAppService.CreateOrderNo();
                    result.NewestOrderNo = newestOrderNo.Data;

                    //订单付完成清空当前订单及付款缓存
                    menuCache.Remove(orderCache);
                    _cacheManager.SetSaleMenuCache(FishSession.UserId, menuCache);
                    payCache.Remove(orderPay);
                    _cacheManager.SetPayDataCache(FishSession.UserId, payCache);
                }
                else
                {
                    return Result.Fail<SMPayDto>("订单创建失败");
                }
            }

            return new Result<SMPayDto>(ResultCode.Ok, result);
        }

        /// <summary>
        /// 礼券支付
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<Result<SMPayDto>> CouponPay(CouponPayInput input)
        {
            SMPayDto result = new SMPayDto();
            OrderSaleET et = new OrderSaleET();

            string userId = FishSession.UserId;
            var userData = _cacheManager.GetUserDataCacheByUserId(userId);

            var orderNo = await _orderNoAppService.CreateOrderNo();
            var menuCache = _cacheManager.GetSaleMenuCache(FishSession.UserId);
            var orderCache = menuCache?.FirstOrDefault(w => w.OrderNo == orderNo.Data);

            if (orderCache == null)
            {
                return Result.Fail<SMPayDto>("没发现支付数据，请重新操作");
            }

            var payCache = _cacheManager.GetPayDataCache(FishSession.UserId) ?? new List<OrderPayInfo>();
            var orderPay = payCache.FirstOrDefault(w => w.OrderNo == orderNo.Data);

            if (orderPay == null)
            {
                orderPay = new OrderPayInfo { OrderNo = orderNo.Data, TotalAmount = orderCache.PayAmount };
                payCache.Add(orderPay);
            }

            et.flow_no = orderNo.Data;
            orderPay.TableNo = input.TableNo;

            if (input.TicketVolumes.Count > 0)
            {
                foreach (var item in input.TicketVolumes)
                {
                    et.tvolumelist += item + "@";

                    //校验支付结果查询
                    var queryResult = await _payAppService.VolumeQuery(new VolumeQueryInput
                    {
                        VolumeNo = item
                    });
                    if (!queryResult.IsSuccess)
                    {
                        return Result.Fail<SMPayDto>(queryResult.Message);
                    }
                }
            }

            //礼券大于应付金额当应付金额使用
            orderPay.TicketVolumeAmount = input.PayAmount > orderPay.PayableAmount ? orderPay.PayableAmount : input.PayAmount;

            //判断支付完成状态
            if (orderPay.PayableAmount > 0)
            {
                result.IsComplete = false;
                result.PayInfos = GetPayInfos(orderPay, true);

                //更新付款缓存
                _cacheManager.SetPayDataCache(FishSession.UserId, payCache);
            }
            else
            {
                //创建订单
                var res = await CreateOrder(et, orderCache, orderPay, userData);
                if (res.IsSuccess)
                {
                    result.IsComplete = true;
                    result.PayInfos = GetPayInfos(orderPay, true);
                    result.PrintInfo = GetPrintInfo(orderCache, orderPay, userData, false, "");

                    var newestOrderNo = await _orderNoAppService.CreateOrderNo();
                    result.NewestOrderNo = newestOrderNo.Data;

                    //订单付完成清空当前订单及付款缓存
                    menuCache.Remove(orderCache);
                    _cacheManager.SetSaleMenuCache(FishSession.UserId, menuCache);
                    payCache.Remove(orderPay);
                    _cacheManager.SetPayDataCache(FishSession.UserId, payCache);
                }
                else
                {
                    return Result.Fail<SMPayDto>("订单创建失败");
                }
            }

            return new Result<SMPayDto>(ResultCode.Ok, result);
        }

        /// <summary>
        /// 信用卡支付
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<Result<SMPayDto>> CreditCardPay(CreditCardPayInput input)
        {
            SMPayDto result = new SMPayDto();
            OrderSaleET et = new OrderSaleET();

            string userId = FishSession.UserId;
            var userData = _cacheManager.GetUserDataCacheByUserId(userId);

            var orderNo = await _orderNoAppService.CreateOrderNo();
            var menuCache = _cacheManager.GetSaleMenuCache(FishSession.UserId);
            var orderCache = menuCache?.FirstOrDefault(w => w.OrderNo == orderNo.Data);

            if (orderCache == null)
            {
                return Result.Fail<SMPayDto>("没发现支付数据，请重新操作");
            }

            var payCache = _cacheManager.GetPayDataCache(FishSession.UserId) ?? new List<OrderPayInfo>();
            var orderPay = payCache.FirstOrDefault(w => w.OrderNo == orderNo.Data);

            if (orderPay == null)
            {
                orderPay = new OrderPayInfo { OrderNo = orderNo.Data, TotalAmount = orderCache.PayAmount };
                payCache.Add(orderPay);
            }

            et.flow_no = orderNo.Data;
            et.CreditCardNo = input.CardNo;
            orderPay.TableNo = input.TableNo;

            if (input.PayAmount > orderPay.PayableAmount)
            {
                return Result.Fail<SMPayDto>("使用信用卡支付，支付总额不能超过购物应付金额");
            }

            orderPay.CreditCardAmount = input.PayAmount;
            if (orderPay.PayableAmount > 0)
            {
                return Result.Fail<SMPayDto>("使用信用卡支付，必须完成剩余金额付款");
            }

            //判断支付完成状态
            if (orderPay.PayableAmount > 0)
            {
                result.IsComplete = false;
                result.PayInfos = GetPayInfos(orderPay, true);

                //更新付款缓存
                _cacheManager.SetPayDataCache(FishSession.UserId, payCache);
            }
            else
            {
                //创建订单
                var res = await CreateOrder(et, orderCache, orderPay, userData);
                if (res.IsSuccess)
                {
                    result.IsComplete = true;
                    result.PayInfos = GetPayInfos(orderPay, true);
                    result.PrintInfo = GetPrintInfo(orderCache, orderPay, userData, false, "");

                    var newestOrderNo = await _orderNoAppService.CreateOrderNo();
                    result.NewestOrderNo = newestOrderNo.Data;

                    //订单付完成清空当前订单及付款缓存
                    menuCache.Remove(orderCache);
                    _cacheManager.SetSaleMenuCache(FishSession.UserId, menuCache);
                    payCache.Remove(orderPay);
                    _cacheManager.SetPayDataCache(FishSession.UserId, payCache);
                }
                else
                {
                    return Result.Fail<SMPayDto>("订单创建失败");
                }
            }

            return new Result<SMPayDto>(ResultCode.Ok, result);
        }

        #region private function

        /// <summary>
        /// 创建订单
        /// </summary>
        /// <param name="et"></param>
        /// <param name="sale"></param>
        /// <param name="pay"></param>
        /// <param name="userData"></param>
        /// <returns></returns>
        private async Task<Result> CreateOrder(OrderSaleET et, SaleMenuCache sale, OrderPayInfo pay, UserData userData)
        {
            #region 销售数据

            Logger.Info(string.Format("START - 写入销售数据和状态更新：flow_no={0}", et.flow_no));

            et.branch_no = userData.StorageNo;
            et.sell_way = ((int)EnumSaleWay.Sale).ToString();
            et.oper_id = userData.UserID;
            et.jh = userData.TerminalID;
            et.work_group = userData.WorkGroup;
            et.deal_no = "";
            et.coin_no = userData.CurrencyEN;
            et.coin_rate = userData.CoinRate;
            et.total_amount = pay.TotalAmount;
            et.old_amount = pay.PaidAmount;
            et.tjstorage = 1;
            et.returnsflowno = "";
            et.returnsflowid = "";
            et.deskno = pay.TableNo;
            et.isfast = 0;
            et.CreditCardType = "";
            et.CreditCardMoney = pay.CreditCardAmount;
            et.icno = "";
            et.consumetype = 0;
            et.fangtemoney = 0;
            et.fangteisintegral = 0;
            et.card_no = "";
            et.fangterate = 1;
            et.goldcardmoney = 0;
            et.vip_id = "";
            et.tvolumelist = et.tvolumelist;
            et.tvolumeamount = pay.TicketVolumeAmount;
            et.zhaolin_amount = pay.GiveAmount;
            et.rmbmoney = pay.CashAmount;
            et.ticketcode = "";
            et.wxmoney = pay.WXAmount;
            et.almoney = pay.ALiPayAmount;
            et.mutonmoney = pay.MuTonAmount;
            et.vouchermoney = pay.VoucherAmount;
            et.leyoumoney = 0;
            et.wxwaimoney = pay.WXWaiAmount;
            et.aliwaimoney = pay.ALiPayWaiAmount;
            et.leyouscanMoney = pay.LeYouScanAmount;
            et.mutonerate = et.mutonerate == 0 ? 1M : et.mutonerate;

            int fid = 1;
            sale.Menus.ForEach(t =>
            {
                et.flow_id += fid + "@";
                et.item_no += t.GoodsNo + "@";
                et.item_subno = t.BarCode + "@";
                et.source_price += t.SalePrice + "@";
                et.sale_qnty += t.Qty + "@";
                et.source_qnty += t.Qty + "@";
                et.sale_money += t.SalePrice + "@";
                et.cost_price += t.SalePrice + "@";
                et.sale_price += t.SalePrice + "@";
                et.have_stocklist = "0" + "@";
                et.other1 = userData.UserID + "@";
                et.special_type = t.Discount < 1
                ? EnumSpecialType.SingleDiscount.Description()
                : EnumSpecialType.Normal.Description() + "@";
                et.parentid = t.GoodsNo + "@";

                fid++;
            });

            var res = await AddOrderSaleInfo(et, sale, userData);

            Logger.Info(string.Format("END - 写入销售数据和状态更新：flow_no={0}", et.flow_no));

            //销售写入失败
            if (!res.IsSuccess)
            {
                //清空当前销售信息，重新开始销售
                var menuCache = _cacheManager.GetSaleMenuCache(FishSession.UserId);
                var orderCache = menuCache?.FirstOrDefault(w => w.OrderNo == et.flow_no);

                menuCache?.Remove(orderCache);
                _cacheManager.SetSaleMenuCache(FishSession.UserId, menuCache);

                var payCache = _cacheManager.GetPayDataCache(FishSession.UserId);
                var orderPay = payCache?.FirstOrDefault(w => w.OrderNo == et.flow_no);

                payCache?.Remove(orderPay);
                _cacheManager.SetPayDataCache(FishSession.UserId, payCache);

                Logger.Info("销售数据写入失败，请重新销售！");
                return new Result<OrderPayOutput>(ResultCode.Fail, null, res.Message);
            }

            #endregion

            #region 同步数据

            Logger.Info("调用同步数据！");
            try
            {
                _syncAppService.SyncData(userData.StorageNo, userData.TerminalID, false);
            }
            catch (Exception ex)
            {
                Logger.Error("调用同步数据失败：" + ex);
            }

            #endregion

            #region 发送厨打

            var cookPrint = this._cookSetting.GetGetCookSetting(userData.StorageNo);
            if (cookPrint != null)
            {
                Logger.Info("调用发送厨打");
                try
                {
                    var goodsList = new List<GoodsCook>();
                    sale.Menus.ForEach(t =>
                    {
                        if (t.GoodsType == "S")
                        {
                            goodsList.Add(new GoodsCook
                            {
                                GoodsNo = t.GoodsNo,
                                GoodsName = t.GoodsName,
                                Qty = t.Qty
                            });
                        }
                        if (t.GoodsType == "C" && t.MealSubs.Count > 0)
                        {
                            t.MealSubs.ForEach(a =>
                            {
                                goodsList.Add(new GoodsCook
                                {
                                    GoodsNo = a.GoodsNo,
                                    GoodsName = a.GoodsName,
                                    Qty = a.Qty
                                });
                            });
                        }
                    });

                    var print = new OrderPrint
                    {
                        flow_no = pay.OrderNo,
                        deskno = pay.TableNo
                    };
                    var cookPrints = GetGoodsCookPrint(goodsList, userData).Result.GroupBy(t => new { t.PrintName });
                    foreach (var item in cookPrints)
                    {
                        print.GoodsList = new List<GoodsPrint>();
                        var goodsNos = item.Select(t => t.GoodsNo);
                        var goodsCooks = goodsList.Where(t => goodsNos.Contains(t.GoodsNo)).ToList();
                        goodsCooks.ForEach(t =>
                        {
                            print.GoodsList.Add(new GoodsPrint
                            {
                                item_name = t.GoodsName,
                                sale_qnty = (decimal)t.Qty
                            });
                        });

                        //调用厨打方法
                        _orderNoAppService.SendCookPrint(print, item.Key.PrintName, cookPrint);
                    }
                }
                catch (Exception ex)
                {
                    Logger.Error("发送厨打报错失败：" + ex);
                }
            }
            #endregion

            #region 发送配餐显示屏

            if (userData.pc_send)
            {
                Logger.Info("调用发送配餐显示屏");
                try
                {
                    //
                }
                catch (Exception ex)
                {
                    Logger.Error("发送配餐显示屏报错失败：" + ex);
                }
            }

            #endregion

            #region 操作日志

            LogET log = new LogET();
            log.casher_no = userData.UserID;
            log.power_man = "";
            log.oper_type = LogOperType.srsp;
            log.dj_no = et.flow_no;
            log.jh = userData.TerminalID;
            log.oper_text = string.Format("成功交易订单：{0}", et.flow_no);

            if (pay.CreditCardAmount > 0)
            {
                log.oper_text = log.oper_text + " 信用卡付款[" + et.CreditCardNo + "]：" + string.Format("{0:F2}", pay.CreditCardAmount);
                log.oper_type = LogOperType.xykzf;
            }
            if (pay.PaidAmount > pay.CreditCardAmount + pay.TicketVolumeAmount + pay.VoucherAmount + pay.WXWaiAmount + pay.ALiPayWaiAmount)
            {
                log.oper_text = log.oper_text + " 人民币付款：" + string.Format("{0:F2}", pay.PaidAmount - pay.CreditCardAmount);
                log.oper_type = LogOperType.xsfk;
            }

            string strSql = string.Format(@"INSERT INTO pos_operator_log (casher_no, oper_date, power_man, oper_type, dj_no, jh, oper_text) 
                                  VALUES ('{0}',getdate(),'{1}','{2}','{3}','{4}','{5}')",
                              log.casher_no, log.power_man, log.oper_type, log.dj_no, log.jh, log.oper_text);
            await _localContext.ExecuteNonQueryAsync(strSql, null);

            #endregion

            return new Result(ResultCode.Ok, "下单成功！");
        }

        /// <summary>
        /// 获取产品厨打参数
        /// </summary>
        /// <param name="goodsCooks"></param>
        /// <param name="userData"></param>
        /// <returns></returns>
        private async Task<List<GoodsCookPrint>> GetGoodsCookPrint(List<GoodsCook> goodsCooks, UserData userData)
        {
            var goods = new List<string>();
            goodsCooks.ForEach(t => { goods.Add(t.GoodsNo); });

            var sql = string.Format(@"SELECT GoodsNo = a.item_no,PrintName = b.printername FROM base_entry_message_allow a
                    INNER JOIN ft_printer b ON b.printercode = a.printercode
                    WHERE a.isPrint = 1 AND branch_no = @branch_no and jh = @jh AND item_no in ('{0}')", string.Join("','", goods.ToArray()));
            var sqlInput = new { branch_no = userData.StorageNo, jh = userData.TerminalID };
            var cookPrints = await _parkContext.ExecuteFunctionAsync<List<GoodsCookPrint>>(sql, sqlInput);
            return cookPrints;

        }

        /// <summary>
        /// 付款- 更新状态、删除临时表、写入销售、更新库存
        /// </summary>
        /// <param name="et"></param>
        /// <param name="sale"></param>
        /// <param name="userData"></param>
        /// <returns></returns>
        private async Task<Result> AddOrderSaleInfo(OrderSaleET et, SaleMenuCache sale, UserData userData)
        {
            try
            {
                //写入销售数据
                var sql = @"exec ft_pr_sale @flow_no,@flow_id,@branch_no,@item_no,@source_price,@sale_price,@sale_qnty,@source_qnty,
                    @sale_money,@cost_price,@sell_way,@oper_id,'0','0','0',@other1,@item_subno,@special_type,@jh,@work_group,0,'',0,0,@deal_no,'',@pay_way,@coin_no,@coin_rate,'','',
                    @total_amount,@old_amount,@zhaolin_amount,@rmbmoney,'','',@tjstorage,@parentid,@returnsflowno,@returnsflowid,@deskno,@isfast,@have_stocklist,
                    @CreditCardMoney,@CreditCardNo,@CreditCardRemark,@CreditCardType,@icno,@consumetype,@card_no,@fangtemoney,@fangteisintegral,@fangterate,@vip_id,@goldcardmoney,
                    @tvolumelist,@tvolumeamount,@ticketcode,@wxmoney,@wxtradeno,@almoney,@altradeno,@mutonmoney,
                    @mutontradeno,@vouchermoney,@mutonedisAmount,@mutonerate,@mutoneorder,@leyoumoney,@wxwaimoney,@aliwaimoney,@leyouscanMoney";
                DataTable dt = await _localContext.GetDataTableAsync(sql, et);

                if (dt.Rows.Count > 0 && dt.Rows[0][0].ToString() == "1")
                {
                    //更新后台产品库存
                    sql = "exec ft_up_FishUpdateStockAccount @branch_no,@item_no,@sale_qnty,@sell_way";
                    var inputSql = new
                    {
                        branch_no = et.branch_no,
                        item_no = et.item_no,
                        sale_qnty = et.sale_qnty,
                        sell_way = et.sell_way
                    };
                    DataTable stockDt = await _parkContext.GetDataTableAsync(sql, inputSql);

                    if (stockDt.Rows.Count > 0 && stockDt.Rows[0][0].ToString() == "-1")
                    {
                        Logger.Info($"更新商品库存失败：flow_no = {et.flow_no}");
                        return new Result(ResultCode.Fail, "更新商品库存失败！");
                    }

                    //获取上单，并更新最后时间
                    string receiptId = userData.TerminalID + Convert.ToString(Convert.ToInt32(et.flow_no.Substring(3, 4)) - 1).PadLeft(4, '0');
                    sql = @"update Purchase set dLastUpdateTime=dateadd(mi,3,GETDATE()) where DATEDIFF(DAY,BuyDate,getdate()) = 0 and Status=2 ";
                    if (!string.IsNullOrEmpty(receiptId))
                    {
                        sql += $" and ReceiptID = '{receiptId}'";
                    }
                    var result = await _localContext.ExecuteNonQueryAsync(sql, null);

                    //更新订单状态、删除临时商品表、订单编号基数表+1
                    sql = $"update Purchase set Status = 2,dLastUpdateTime = dateadd(mi,3,GETDATE()) where DATEDIFF(DAY,BuyDate,getdate()) = 0 and ReceiptID = '{et.flow_no}'";
                    result += await _localContext.ExecuteNonQueryAsync(sql, null);
                    sql = $"delete from TempItem where DATEDIFF(DAY,BuyDate,getdate()) = 0 and ReceiptID= '{et.flow_no}'";
                    result += await _localContext.ExecuteNonQueryAsync(sql, null);
                    sql = "update sysvar set intvalue = intvalue+1 where name = 'CURRMAXNO'";
                    result += await _localContext.ExecuteNonQueryAsync(sql, null);

                    //更新本地估清库存
                    if (sale.Menus != null && sale.Menus.Count > 0)
                    {
                        foreach (var item in sale.Menus)
                        {
                            sql = "select VirtualStock = guqingqty from base_entry_message_allow WHERE  activeflag=1 and guqingqty > -1 and branch_no=@branch_no and jh=@jh AND item_no=@item_no";
                            object sqlInput = new { branch_no = userData.StorageNo, jh = userData.TerminalID, item_no = item.GoodsNo };
                            var virtualStock = await _localContext.ExecuteFunctionAsync<SqlVirtualStock>(sql, sqlInput);
                            if (virtualStock != null)
                            {
                                sql = $"update base_entry_message_allow set guqingqty = guqingqty - {item.Qty} where activeflag=1 and item_no = '{item.GoodsNo}' and branch_no='{userData.StorageNo}' and jh='{userData.TerminalID}'";
                                result += await _localContext.ExecuteNonQueryAsync(sql, null);
                            }
                        }
                    }
                    if (result <= 0)
                    {
                        Logger.Info($"更新订单状态、更新估清库存失败：flow_no = {et.flow_no}");
                        return new Result(ResultCode.Fail, "更新订单状态、更新估清库存失败！");
                    }
                }
                else
                {
                    Logger.Info($"写入销售数据失败：flow_no = {et.flow_no}");
                    return new Result(ResultCode.Fail, "写入销售记录失败！");
                }
            }
            catch (Exception ex)
            {
                Logger.Error("更新订单相关数据报错：" + ex);
                return new Result(ResultCode.Fail, "更新订单相关数据报错！");
            }

            return new Result(ResultCode.Ok, "写入销售记录，更新库存成功！");
        }

        /// <summary>
        /// 清空订单临时数据
        /// </summary>
        /// <param name="orderNo"></param>
        /// <returns></returns>
        private async Task<Result> ClearTempItem(string orderNo)
        {
            var sql = $"delete from TempItem where DATEDIFF(DAY,BuyDate,getdate()) = 0 and ReceiptID= '{orderNo}'";
            var result = await _localContext.ExecuteNonQueryAsync(sql, null);
            sql = $"delete from Purchase where DATEDIFF(DAY,BuyDate,getdate()) = 0 and ReceiptID= '{orderNo}'";
            result += await _localContext.ExecuteNonQueryAsync(sql, null);
            if (result == 0)
            {
                return new Result(ResultCode.Fail, "订单临时记录清除失败！");
            }

            return new Result(ResultCode.Ok, "订单临时记录清除成功！");
        }

        /// <summary>
        /// 支付数据输出转化
        /// </summary>
        /// <param name="info"></param>
        /// <param name="isSummary"></param>
        /// <returns></returns>
        private List<SMPayInfo> GetPayInfos(OrderPayInfo info, bool isSummary)
        {
            List<SMPayInfo> payInfos = new List<SMPayInfo>();
            if (info != null)
            {
                if (info.CashAmount > 0)
                    payInfos.Add(new SMPayInfo { PayName = CashPayType.Cash.DisplayName(), Amount = info.CashAmount });
                if (info.VoucherAmount > 0)
                    payInfos.Add(new SMPayInfo { PayName = CashPayType.CashCoupon.DisplayName(), Amount = info.VoucherAmount });
                if (info.WXWaiAmount > 0)
                    payInfos.Add(new SMPayInfo { PayName = CashPayType.WxWai.DisplayName(), Amount = info.WXWaiAmount });
                if (info.ALiPayWaiAmount > 0)
                    payInfos.Add(new SMPayInfo { PayName = CashPayType.AliWai.DisplayName(), Amount = info.ALiPayWaiAmount });
                if (info.WXAmount > 0)
                    payInfos.Add(new SMPayInfo { PayName = ThirdPayType.Wx.DisplayName(), Amount = info.WXAmount });
                if (info.ALiPayAmount > 0)
                    payInfos.Add(new SMPayInfo { PayName = ThirdPayType.Ali.DisplayName(), Amount = info.ALiPayAmount });
                if (info.MuTonAmount > 0)
                    payInfos.Add(new SMPayInfo { PayName = ThirdPayType.Mutone.DisplayName(), Amount = info.MuTonAmount });
                if (info.LeYouScanAmount > 0)
                    payInfos.Add(new SMPayInfo { PayName = ThirdPayType.Ft.DisplayName(), Amount = info.LeYouScanAmount });
                if (info.CreditCardAmount > 0)
                    payInfos.Add(new SMPayInfo { PayName = OtherPayType.Unionpay.DisplayName(), Amount = info.CreditCardAmount });
                if (info.TicketVolumeAmount > 0)
                    payInfos.Add(new SMPayInfo { PayName = OtherPayType.Coupons.DisplayName(), Amount = info.TicketVolumeAmount });

                if (isSummary)
                {
                    payInfos.Add(new SMPayInfo
                    {
                        PayName = EnumAmountType.TotalAmount.DisplayName(),
                        Amount = info.TotalAmount
                    });
                    payInfos.Add(new SMPayInfo
                    {
                        PayName = EnumAmountType.DiscountAmount.DisplayName(),
                        Amount = info.DiscountAmount
                    });
                    payInfos.Add(new SMPayInfo
                    {
                        PayName = EnumAmountType.PayableAmount.DisplayName(),
                        Amount = info.PayableAmount
                    });
                    payInfos.Add(new SMPayInfo
                    {
                        PayName = EnumAmountType.PaidAmount.DisplayName(),
                        Amount = info.PaidAmount
                    });
                    payInfos.Add(new SMPayInfo
                    {
                        PayName = EnumAmountType.GiveAmount.DisplayName(),
                        Amount = info.GiveAmount
                    });
                }
            }
            return payInfos;
        }

        /// <summary>
        /// 打印
        /// </summary>
        /// <param name="menu"></param>
        /// <param name="pay"></param>
        /// <param name="user"></param>
        /// <param name="isMutoneOrFangtePay"></param>
        /// <param name="payOrderNo"></param>
        /// <returns></returns>
        private OrdersPrintDto GetPrintInfo(SaleMenuCache menu, OrderPayInfo pay, UserData user, bool isMutoneOrFangtePay, string payOrderNo)
        {

            //判断是否为秒通或方特支付
            if (isMutoneOrFangtePay)
            {
                menu.DiscountAmount = 0;
                menu.Menus.ForEach(t =>
                {
                    t.Discount = 100;
                    if (user.LeyouPrice == ((int)EnumPriceType.Default).ToString())
                    {
                        t.SalePrice = t.DefaultPrice;
                    }
                    if (t.CategoryType == "C")
                    {
                        t.MealSubs.ForEach(w =>
                        {
                            w.Discount = 100;
                            if (user.LeyouPrice == ((int)EnumPriceType.Default).ToString())
                            {
                                w.SalePrice = w.DefaultPrice;
                            }
                        });
                    }
                });
            }

            var model = new PrintInfo
            {
                TableId = pay.TableNo,
                Cashier = user.UserName,
                TerminalID = user.TerminalID,
                SaleMode = EnumSaleWay.Sale.DisplayName(),
                Amount = menu.SaleAmount,
                Discount = menu.DiscountAmount + pay.DiscountAmount,
                PayableAmount = pay.TotalAmount - pay.DiscountAmount,
                PayAmount = pay.PaidAmount,
                GiveAmount = pay.GiveAmount,
                OrderNo = pay.OrderNo,
                ThirdTradeNo = payOrderNo,
                TradeTime = DateTime.Now,
                PayMents = GetPayInfos(pay, false).MapTo<List<PayMentWay>>(),
                Menus = menu.Menus.MapTo<List<GoodsInfo>>()
            };

            return model.MapTo<OrdersPrintDto>();
        }

        #endregion

    }
}
