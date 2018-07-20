using Mi.Fish.Common;

namespace Mi.Fish.Application.Order
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Abp.Runtime.Caching;
    using Mi.Fish.ApplicationDto;
    using Mi.Fish.EntityFramework;
    using Abp.AutoMapper;
    using Mi.Fish.Infrastructure.Results;
    using System.Data;
    using Microsoft.Extensions.Options;
    using Mi.Fish.Application.Sync;

    public class OrderDetailService : AppServiceBase, IOrderDetailService
    {
        private readonly ParkDbContext _context;

        private readonly ICacheManager _cacheManager;
        private readonly IPayAppService _payAppService;

        private readonly LocalDbContext _localDb;

        private readonly ISyncAppService _SyncService;
        private readonly IOrderNoAppService _orderNoApp;

        public OrderDetailService(ICacheManager cacheManager, ParkDbContext context, IPayAppService payAppService, LocalDbContext localDb,
            ISyncAppService SyncService, IOrderNoAppService orderNoApp)
        {
            this._cacheManager = cacheManager;
            this._context = context;
            this._payAppService = payAppService;
            this._localDb = localDb;
            this._SyncService = SyncService;
            this._orderNoApp = orderNoApp;
        }
        public async Task<Result<OrderDetailOutput>> GetOrderDetail(string orderNo)
        {
            Logger.Info("开始查询订单详情，订单号：" + orderNo);
            if (orderNo.Length > 4)
            {
                orderNo = int.Parse(orderNo.Substring(3, 4)).ToString();
            }
            var query = ApplicationConsts.QueryOrderDetail;

            try
            {
                string userid = FishSession.UserId;
                var userinfo = _cacheManager.GetUserDataCacheByUserId(userid);
                if (string.IsNullOrEmpty(userinfo.StorageNo) || string.IsNullOrEmpty(userinfo.TerminalID))
                {
                    return new Result<OrderDetailOutput>(ResultCode.Expired, new OrderDetailOutput());
                }
                var obj = new
                {
                    StoreNO = userinfo.StorageNo,
                    FishNo = userinfo.TerminalID,
                    orderNo,
                };
                var orderDetail = await _context.ExecuteFunctionAsync<List<OrderDetail>>(query, obj);
                if (orderDetail == null || orderDetail.Count == 0)
                {
                    Logger.Info("订单数据为空，订单号：" + orderNo);
                    return new Result<OrderDetailOutput>(ResultCode.Fail, new OrderDetailOutput(), "订单不存在");
                }
                var paymentList = await GetOrderPaymentList(orderNo, userinfo);
                var goodsList = orderDetail.MapTo<List<OrderGoods>>();
                var result = new OrderDetailOutput();
                result.OrderGoodsList = goodsList;
                if (orderDetail.Count > 0)
                {
                    result.DeskNo = orderDetail[0].deskno;
                    result.VipNo = orderDetail[0].vipno;
                    result.FlowNo = orderDetail[0].flow_no.ToString();
                }
                result.Totalamount = paymentList.Sum(p => p.sale_amount);
                var resultPayList = new List<OrderPayDetailOutput>();
                foreach (var payItem in paymentList)
                {
                    var payResult = new OrderPayDetailOutput();
                    switch (payItem.pay_way)
                    {
                        case "1": //现金

                            payResult.Payway = PayType.Cash;
                            break;
                        case "5": //信用卡
                            payResult.Payway = PayType.Unionpay;
                            break;
                        case "8"://礼券
                            payResult.Payway = PayType.Coupons;
                            break;
                        case "84"://代金券
                            payResult.Payway = PayType.CashCoupon;
                            break;
                        case "120"://微信
                            payResult.Payway = PayType.Wx;
                            break;
                        case "130"://支付宝
                            payResult.Payway = PayType.Ali;
                            break;
                        case "992"://微信(外)
                            payResult.Payway = PayType.WxWai;
                            break;
                        case "993"://支付宝(外)
                            payResult.Payway = PayType.AliWai;
                            break;
                        case "112"://秒通
                            payResult.Payway = PayType.Mutone;
                            break;
                        case "113": //方特扫码
                            payResult.Payway = PayType.Ft;
                            break;
                        case "105"://方特 来自第三方方特订单
                            payResult.Payway = PayType.FtApp;
                            break;
                        default:
                            break;
                    }
                    var payway = payResult.Payway;
                    payResult = resultPayList.Find(p => p.Payway == payway);
                    if (payResult == null)
                    {
                        payResult = new OrderPayDetailOutput();
                        payResult.Payway = payway;
                        resultPayList.Add(payResult);
                    }
                    payResult.PayDescription = payResult.Payway.DisplayName();
                    payResult.Saleamount += payItem.old_amount;
                }
                result.PaymentList = resultPayList;
                return Result<OrderDetailOutput>.FromData(result);
            }
            catch (Exception ex)
            {
                Logger.Error("获取订单详情失败，订单号：" + orderNo, ex);
            }
            return new Result<OrderDetailOutput>(ResultCode.Fail, new OrderDetailOutput());
        }

        private async Task<List<OrderpayDetail>> GetOrderPaymentList(string orderNo, UserData userinfo)
        {
            var paymentList = new List<OrderpayDetail>();
            try
            {
                var query = ApplicationConsts.QueryOrderPayList;
                var obj = new
                {
                    StoreNO = userinfo.StorageNo,
                    FishNo = userinfo.TerminalID,
                    orderNo,
                };
                paymentList = await _context.ExecuteFunctionAsync<List<OrderpayDetail>>(query, obj);
            }
            catch (Exception ex)
            {
                Logger.Error("获取订单详情失败，订单号:" + orderNo, ex);
            }
            return paymentList;
        }

        public async Task<Result<List<OrderSaleListOutPut>>> GetLastSaleList()
        {
            try
            {
                string userId = FishSession.UserId;
                var userData = _cacheManager.GetUserDataCacheByUserId(userId);
                if (userData == null)
                {
                    return Result.Fail<List<OrderSaleListOutPut>>(ResultCode.Expired.DisplayName());
                }
                var query = ApplicationConsts.QueryLastSaleList;
                var obj = new
                {
                    StoreNO = userData.StorageNo,
                    FishNO = userData.TerminalID,
                    CashierNO = userData.UserID,
                };
                var data = await _context.ExecuteFunctionAsync<List<OrderSaleListDto>>(query, obj);
                var result = data.MapTo<List<OrderSaleListOutPut>>();
                //完整单号
                var formatResult = new List<OrderSaleListOutPut>();
                foreach (var item in result)
                {
                    formatResult.Add(new OrderSaleListOutPut() { FlowNo = userData.TerminalID + item.FlowNo.PadLeft(4, '0') });
                }

                return Result.FromData(formatResult);
            }
            catch (Exception e)
            {
                return Result.Fail<List<OrderSaleListOutPut>>(e.Message);
            }
        }


        public async Task<Result<BaseOrderRefundOutput>> OrderRefund(string orderNo, string ip)
        {
            Logger.Info("IP :" + ip);
            var checkPermission = _orderNoApp.CheckPermission(EnumAuthorityType.BackGood);
            if (!checkPermission.Data)
            {
                return new Result<BaseOrderRefundOutput>(ResultCode.Proxyauthorized, new BaseOrderRefundOutput(), "没有退款权限");
            }
            var result = new BaseOrderRefundOutput();
            result.OrderRefundList = new List<OrderRefundInfo>();
            Logger.Info("开始查询订单详情，订单号：" + orderNo);
            if (String.IsNullOrEmpty(orderNo))
            {
                return new Result<BaseOrderRefundOutput>(ResultCode.Fail, new BaseOrderRefundOutput(), "单号不能为空");
            }
            try
            {
                if (orderNo.Length > 4)
                {
                    orderNo = int.Parse(orderNo.Substring(3, 4)).ToString();
                }
                var query = ApplicationConsts.QueryOrderDetail;

                string userid = FishSession.UserId;
                var userinfo = _cacheManager.GetUserDataCacheByUserId(userid);
                var obj = new
                {
                    StoreNO = userinfo.StorageNo,
                    FishNo = userinfo.TerminalID,
                    orderNo,
                };
                var objRefund = await _context.ExecuteScalarAsync(ApplicationConsts.QueryRefundOrder, obj);
                if ((int)objRefund > 0)
                {
                    return new Result<BaseOrderRefundOutput>(ResultCode.Fail, new BaseOrderRefundOutput(), "订单已经退过款，不能重复退款");
                }
                objRefund = await _context.ExecuteScalarAsync(ApplicationConsts.QueryRefund, obj);
                if ((int)objRefund > 0)
                {
                    return new Result<BaseOrderRefundOutput>(ResultCode.Fail, new BaseOrderRefundOutput(), "订单已经退过款，不能重复退款");
                }

                var orderGoods = await _context.ExecuteFunctionAsync<List<OrderDetail>>(query, obj);
                if (orderGoods == null || orderGoods.Count == 0)
                {
                    Logger.Info("订单数据为空，订单号：" + orderNo);
                    return new Result<BaseOrderRefundOutput>(ResultCode.Fail, new BaseOrderRefundOutput(), "订单为空");
                }
                var newOrderNoResult = await this._orderNoApp.CreateOrderNo();
                var newOrderNo = newOrderNoResult.Data;
                var orderDetail = new OrderDetailOutput();
                if (orderGoods.Count > 0)
                {
                    orderDetail.DeskNo = orderGoods[0].deskno;
                    orderDetail.VipNo = orderGoods[0].vipno;
                    orderDetail.FlowNo = orderGoods[0].flow_no.ToString();
                }
                var paymentList = await GetOrderPaymentList(orderNo, userinfo);
                orderDetail.Totalamount = paymentList.Sum(p => p.sale_amount);
                PayReturnInput thirdPayInput = null;

                var SaleParm = new OrderSaleET();
                #region Payment List
                foreach (var payItem in paymentList)
                {
                    switch (payItem.pay_way)
                    {
                        case "1": //现金
                            SaleParm.zhaolin_amount += payItem.old_amount;
                            break;
                        case "5": //信用卡
                            orderDetail.Totalamount -= payItem.sale_amount;
                            SaleParm.CreditCardMoney += payItem.old_amount;
                            break;
                        case "8"://礼券

                            orderDetail.Totalamount -= payItem.sale_amount;
                            SaleParm.tvolumeamount += payItem.old_amount;
                            break;
                        case "84"://代金券
                            orderDetail.Totalamount -= payItem.sale_amount;
                            SaleParm.vouchermoney += payItem.old_amount;
                            break;
                        case "120"://微信
                            thirdPayInput = new PayReturnInput()
                            {
                                OriginalOrderNo = orderNo,
                                PayType = PayType.Wx,
                                AllReturn = true,
                                ReturnAmount = payItem.old_amount,
                            };
                            orderDetail.Totalamount -= payItem.sale_amount;
                            SaleParm.wxmoney += payItem.old_amount;
                            SaleParm.wxtradeno = payItem.thirdtradeno;
                            break;
                        case "130"://支付宝
                            thirdPayInput = new PayReturnInput()
                            {
                                OriginalOrderNo = orderNo,
                                PayType = PayType.Ali,
                                AllReturn = true,
                                ReturnAmount = payItem.old_amount,
                            };
                            orderDetail.Totalamount -= payItem.sale_amount;
                            SaleParm.almoney += payItem.old_amount;
                            SaleParm.altradeno = payItem.thirdtradeno;
                            break;
                        case "992"://微信(外)
                            orderDetail.Totalamount -= payItem.sale_amount;
                            SaleParm.wxwaimoney += payItem.old_amount;
                            break;
                        case "993"://支付宝(外)
                            orderDetail.Totalamount -= payItem.sale_amount;
                            SaleParm.aliwaimoney += payItem.old_amount;
                            break;
                        case "112"://秒通
                            thirdPayInput = new PayReturnInput()
                            {
                                OriginalOrderNo = orderNo,
                                PayType = PayType.Mutone,
                                AllReturn = true,
                                ReturnAmount = payItem.old_amount,
                            };
                            orderDetail.Totalamount -= payItem.sale_amount;
                            SaleParm.mutonmoney += payItem.old_amount;
                            SaleParm.mutontradeno = payItem.thirdtradeno;
                            break;
                        case "113": //方特扫码
                            thirdPayInput = new PayReturnInput()
                            {
                                OriginalOrderNo = orderNo,
                                PayType = PayType.Ft,
                                AllReturn = true,
                                ReturnAmount = payItem.old_amount,
                            };

                            orderDetail.Totalamount -= payItem.sale_amount;
                            SaleParm.leyouscanMoney += payItem.old_amount;
                            break;
                        case "105"://方特 来自第三方方特订单，不在这里退单
                            return new Result<BaseOrderRefundOutput>(ResultCode.Fail, new BaseOrderRefundOutput(), "第三方订单不能在这退");
                        default:
                            break;
                    }
                }
                #endregion

                #region Refund Detail
                result.Message = string.Empty;
                if (SaleParm.CreditCardMoney > 0)
                {
                    result.OrderRefundList.Add(new OrderRefundInfo()
                    {
                        PayAmount = SaleParm.CreditCardMoney,
                        PayType = "信用卡",
                        RefundType = EnumRefundType.NoRefund,
                    });
                }
                if (SaleParm.tvolumeamount > 0)
                {
                    result.OrderRefundList.Add(new OrderRefundInfo()
                    {
                        PayAmount = SaleParm.tvolumeamount,
                        PayType = "礼券",
                        RefundType = EnumRefundType.NotAllowed,
                    });
                }
                if (SaleParm.vouchermoney > 0)
                {
                    result.OrderRefundList.Add(new OrderRefundInfo()
                    {
                        PayAmount = SaleParm.vouchermoney,
                        PayType = "代金券",
                        RefundType = EnumRefundType.NotAllowed,
                    });
                }
                if (SaleParm.wxwaimoney > 0)
                {
                    result.OrderRefundList.Add(new OrderRefundInfo()
                    {
                        PayAmount = SaleParm.wxwaimoney,
                        PayType = "微信(外)",
                        RefundType = EnumRefundType.NotAllowed,
                    });
                }
                if (SaleParm.aliwaimoney > 0)
                {
                    result.OrderRefundList.Add(new OrderRefundInfo()
                    {
                        PayAmount = SaleParm.aliwaimoney,
                        PayType = "支付宝(外)",
                        RefundType = EnumRefundType.NotAllowed,
                    });
                }
                if (SaleParm.zhaolin_amount > 0)
                {
                    result.OrderRefundList.Add(new OrderRefundInfo()
                    {
                        PayAmount = SaleParm.zhaolin_amount,
                        PayType = "找零",
                        RefundType = EnumRefundType.Allowed,
                    });
                    result.Message = $"订单退款现金 {SaleParm.zhaolin_amount} 元，";
                }
                if (SaleParm.wxmoney > 0)
                {
                    result.OrderRefundList.Add(new OrderRefundInfo()
                    {
                        PayAmount = SaleParm.wxmoney,
                        PayType = "微信",
                        RefundType = EnumRefundType.Allowed,
                    });
                    result.Message += $"订单微信退款 {SaleParm.wxmoney} 元，";
                }
                if (SaleParm.almoney > 0)
                {
                    result.OrderRefundList.Add(new OrderRefundInfo()
                    {
                        PayAmount = SaleParm.almoney,
                        PayType = "支付宝",
                        RefundType = EnumRefundType.Allowed,
                    });
                    result.Message += $"订单支付宝退款 {SaleParm.almoney} 元，";
                }
                if (SaleParm.mutonmoney > 0)
                {
                    result.OrderRefundList.Add(new OrderRefundInfo()
                    {
                        PayAmount = SaleParm.mutonmoney,
                        PayType = "秒通",
                        RefundType = EnumRefundType.Allowed,
                    });
                    result.Message += $"订单秒通退款 {SaleParm.mutonmoney} 元，";
                }
                if (SaleParm.leyouscanMoney > 0)
                {
                    result.OrderRefundList.Add(new OrderRefundInfo()
                    {
                        PayAmount = SaleParm.leyouscanMoney,
                        PayType = "方特扫码",
                        RefundType = EnumRefundType.Allowed,
                    });
                    result.Message += $"订单方特扫码退款 {SaleParm.leyouscanMoney} 元，";
                }
                result.Message = result.Message.TrimEnd(new char[] { '，' });
                #endregion

                if (thirdPayInput != null)
                {
                    var thirdPayResult = await _payAppService.PayReturn(thirdPayInput);
                    if (thirdPayResult.Code != ResultCode.Ok)
                    {
                        return new Result<BaseOrderRefundOutput>(thirdPayResult.Code, null, thirdPayResult.Message);
                    }
                    switch (thirdPayInput.PayType)
                    {
                        case PayType.Ali:
                            SaleParm.altradeno = thirdPayResult.Data.PayReturnOrderNo;
                            break;
                        case PayType.Wx:
                            SaleParm.wxtradeno = thirdPayResult.Data.PayReturnOrderNo;
                            break;
                        default: break;
                    }
                }
                if (orderDetail.Totalamount < 0)
                {
                    return new Result<BaseOrderRefundOutput>(ResultCode.Fail, new BaseOrderRefundOutput(), "退款金额不能大于订单金额");
                }

                #region Inilize refund order
                CreateTempOrder(orderGoods, newOrderNo, userinfo);
                int fid = 1;
                foreach (var ordergoods in orderGoods)
                {
                    SaleParm.flow_id += fid.ToString() + "@";
                    SaleParm.item_no += ordergoods.dishno + "@";
                    SaleParm.item_subno += ordergoods.item_subno + "@";
                    SaleParm.cost_price += "@";
                    SaleParm.sale_money += ordergoods.sale_money + "@";
                    SaleParm.sale_price += ordergoods.orderprice + "@";
                    SaleParm.source_price += "0@";
                    SaleParm.sale_qnty += ordergoods.sale_qnty + "@";
                    SaleParm.source_qnty += "1@";
                    SaleParm.other1 += "@";
                    SaleParm.special_type += "0@";
                    SaleParm.parentid += "@";
                    SaleParm.returnsflowno += orderNo + "@";
                    SaleParm.returnsflowid += ordergoods.flow_id + "@";
                    fid++;
                }

                SaleParm.flow_no = newOrderNo;
                SaleParm.branch_no = userinfo.StorageNo;
                SaleParm.sell_way = "B";
                SaleParm.oper_id = userinfo.UserID;
                SaleParm.jh = userinfo.TerminalID;
                SaleParm.work_group = userinfo.WorkGroup;
                SaleParm.deal_no = null;

                SaleParm.coin_no = userinfo.CurrencyEN;
                SaleParm.coin_rate = userinfo.CoinRate;
                SaleParm.deskno = "";
                SaleParm.total_amount = orderDetail.Totalamount;
                SaleParm.mutonedisAmount = 0;
                SaleParm.mutonerate = 1;
                #endregion

                var res = await AddOrderSaleInfo(SaleParm, userinfo);

                Logger.Info(string.Format("END - 写入销售数据和状态更新：flow_no={0}", orderNo));

                //销售写入失败
                if (!res.IsSuccess)
                {
                    Logger.Info("销售数据写入失败！");
                    return new Result<BaseOrderRefundOutput>(ResultCode.Fail, new BaseOrderRefundOutput(), "退款失败");
                }

                #region 同步数据

                Logger.Info("调用同步数据！");
                this._SyncService.SyncData(userinfo.StorageNo, userinfo.TerminalID, false);
                #endregion

                #region 操作日志

                try
                {
                    LogET log = new LogET();
                    log.casher_no = userinfo.UserID;
                    log.power_man = "";
                    log.oper_type = LogOperType.srsp;
                    log.dj_no = SaleParm.flow_no;
                    log.jh = userinfo.TerminalID;
                    log.oper_text = string.Format("成功交易订单：{0}", SaleParm.flow_no);
                    log.oper_text = "人民币退款：" + string.Format("{0:F2}", SaleParm.zhaolin_amount);
                    log.oper_type = LogOperType.xsfk;


                    string strSql = string.Format(@"INSERT INTO pos_operator_log (casher_no, oper_date, power_man, oper_type, dj_no, jh, oper_text) 
                                  VALUES ('{0}',getdate(),'{1}','{2}','{3}','{4}','{5}')",
                                      log.casher_no, log.power_man, log.oper_type, log.dj_no, log.jh, log.oper_text);
                    var logRes = await _localDb.ExecuteNonQueryAsync(strSql, null);
                }
                catch (Exception ex)
                {
                    Logger.Info("操作日志插入失败。detail:" + ex.ToString());
                }
                Logger.Info("获取退单后的新单号。");
                newOrderNoResult = await this._orderNoApp.CreateOrderNo();
                result.OrderNo = newOrderNoResult.Data;
                #endregion
            }
            catch (Exception ex)
            {
                Logger.Error("获取订单详情失败，订单号：" + orderNo, ex);
                return new Result<BaseOrderRefundOutput>(ResultCode.Fail, new BaseOrderRefundOutput(), ex.Message);
            }
            return Result.FromData(result);

        }

        public async Task<Result<OrderRefundOutput>> OrderRefundCheck(string orderNo)
        {
            var result = new OrderRefundOutput();
            result.OrderRefundList = new List<OrderRefundInfo>();
            Logger.Info("开始查询订单详情，订单号：" + orderNo);
            if (String.IsNullOrEmpty(orderNo))
            {
                return new Result<OrderRefundOutput>(ResultCode.Fail, new OrderRefundOutput(), "单号不能为空");
            }
            try
            {
                if (orderNo.Length > 4)
                {
                    orderNo = int.Parse(orderNo.Substring(3, 4)).ToString();
                }

                string userid = FishSession.UserId;
                var userinfo = _cacheManager.GetUserDataCacheByUserId(userid);
                var obj = new
                {
                    StoreNO = userinfo.StorageNo,
                    FishNo = userinfo.TerminalID,
                    orderNo,
                };
                var objRefund = await _context.ExecuteScalarAsync(ApplicationConsts.QueryRefundOrder, obj);
                if ((int)objRefund > 0)
                {
                    return new Result<OrderRefundOutput>(ResultCode.Fail, new OrderRefundOutput(), "订单已经退款");
                }

                objRefund = await _context.ExecuteScalarAsync(ApplicationConsts.QueryRefund, obj);
                if ((int)objRefund > 0)
                {
                    return new Result<OrderRefundOutput>(ResultCode.Fail, new OrderRefundOutput(), "订单已经退款");
                }
                var paymentList = await GetOrderPaymentList(orderNo, userinfo);

                if (paymentList == null || paymentList.Count == 0)
                {
                    Logger.Info("订单数据为空，订单号：" + orderNo);
                    return new Result<OrderRefundOutput>(ResultCode.Fail, new OrderRefundOutput(), "订单不存在");
                }

                var totalamount = paymentList.Sum(p => p.sale_amount);
                var isQuit = false;
                var returnMessage = "订单";
                var SaleParm = new OrderSaleET();

                #region Payment List
                foreach (var item in paymentList)
                {
                    switch (item.pay_way)
                    {
                        case "5": //信用卡
                            isQuit = true;
                            SaleParm.CreditCardMoney += item.old_amount;
                            returnMessage += $"有信用卡支付{item.sale_amount} 元，不允许退单，";
                            totalamount -= item.old_amount;
                            break;
                        case "8"://礼券
                            isQuit = true;
                            returnMessage += $"有礼券金额{item.sale_amount} 元，不退，";
                            SaleParm.tvolumeamount += item.old_amount;
                            totalamount -= item.old_amount;
                            break;
                        case "84"://代金券
                            isQuit = true;
                            returnMessage += $"有代金券金额{item.sale_amount} 元，不退，";
                            SaleParm.vouchermoney += item.old_amount;
                            totalamount -= item.old_amount;
                            break;
                        case "992"://微信(外)
                            isQuit = true;
                            returnMessage += $"有微信(外)支付{item.sale_amount} 元，不可退，";
                            SaleParm.wxwaimoney += item.old_amount;
                            totalamount -= item.old_amount;
                            break;
                        case "993"://支付宝(外)
                            isQuit = true;
                            returnMessage += $"有支付宝(外)支付{item.sale_amount} 元，不可退，";
                            SaleParm.aliwaimoney += item.old_amount;
                            totalamount -= item.old_amount;
                            break;
                        case "105"://方特 来自第三方方特订单，不在这里退单
                            return new Result<OrderRefundOutput>(ResultCode.Fail, new OrderRefundOutput(), "第三方订单不能在这退");
                        default:
                            break;
                    }

                }
                #endregion

                #region Refund Alert
                if (isQuit)
                {
                    if (SaleParm.CreditCardMoney > 0)
                    {
                        result.OrderRefundList.Add(new OrderRefundInfo()
                        {
                            PayAmount = SaleParm.CreditCardMoney,
                            PayType = "信用卡",
                            RefundType = EnumRefundType.NoRefund,
                        });
                    }
                    if (SaleParm.tvolumeamount > 0)
                    {
                        result.OrderRefundList.Add(new OrderRefundInfo()
                        {
                            PayAmount = SaleParm.tvolumeamount,
                            PayType = "礼券",
                            RefundType = EnumRefundType.NotAllowed,
                        });
                    }
                    if (SaleParm.vouchermoney > 0)
                    {
                        result.OrderRefundList.Add(new OrderRefundInfo()
                        {
                            PayAmount = SaleParm.vouchermoney,
                            PayType = "代金券",
                            RefundType = EnumRefundType.NotAllowed,
                        });
                    }
                    if (SaleParm.wxwaimoney > 0)
                    {
                        result.OrderRefundList.Add(new OrderRefundInfo()
                        {
                            PayAmount = SaleParm.wxwaimoney,
                            PayType = "微信(外)",
                            RefundType = EnumRefundType.NotAllowed,
                        });
                    }
                    if (SaleParm.aliwaimoney > 0)
                    {
                        result.OrderRefundList.Add(new OrderRefundInfo()
                        {
                            PayAmount = SaleParm.aliwaimoney,
                            PayType = "支付宝(外)",
                            RefundType = EnumRefundType.NotAllowed,
                        });
                    }
                    result.IsRefund = false;
                    result.Message = returnMessage.TrimEnd('，') + "。" + $"订单可退金额{totalamount}元，是否确定退款。";
                    return Result.FromData(result);
                }
                else
                {
                    result.Message = "订单可以直接退款。";
                }
                #endregion
            }
            catch (Exception ex)
            {
                Logger.Error("获取订单详情失败，订单号：" + orderNo, ex);
                return new Result<OrderRefundOutput>(ResultCode.Fail, new OrderRefundOutput(), ex.Message);
            }
            result.IsRefund = true;
            return Result.FromData(result);

        }

        /// <summary>
        /// 付款- 更新状态、删除临时表、写入销售、更新库存
        /// </summary>
        /// <param name="et"></param>
        /// <returns></returns>
        private async Task<Result> AddOrderSaleInfo(OrderSaleET et, UserData userinfo)
        {
            try
            {
                var sql = "";


                et.tjstorage = userinfo.StorageValue;
                Logger.Info(string.Format("更新订单状态：flow_no={0}", et.flow_no));

                //写入销售数据
                sql = @"exec ft_pr_sale_Refund @flow_no,@flow_id,@branch_no,@item_no,@source_price,@sale_price,@sale_qnty,@source_qnty,
                    @sale_money,@cost_price,@sell_way,@oper_id,'0','0','0',@other1,@item_subno,@special_type,@jh,@work_group,0,'',0,0,@deal_no,'',@pay_way,@coin_no,@coin_rate,'','',
                    @total_amount,@old_amount,@zhaolin_amount,@rmbmoney,'','',@tjstorage,@parentid,@returnsflowno,@returnsflowid,@deskno,@isfast,@have_stocklist,
                    @CreditCardMoney,@CreditCardNo,@CreditCardRemark,@CreditCardType,@icno,@consumetype,@card_no,@fangtemoney,@fangteisintegral,@fangterate,@vip_id,@goldcardmoney,
                    @tvolumelist,@tvolumeamount,@ticketcode,@wxmoney,@wxtradeno,@almoney,@altradeno,@mutonmoney,
                    @mutontradeno,@vouchermoney,@mutonedisAmount,@mutonerate,@mutoneorder,@leyoumoney,@wxwaimoney,@aliwaimoney,@leyouscanMoney";
                DataTable dt = await _localDb.GetDataTableAsync(sql, et);
                Logger.Info(string.Format("写入销售：flow_no={0}", et.flow_no));

                if (dt.Rows.Count > 0 && dt.Rows[0][0].ToString() == "1" && string.IsNullOrEmpty(et.icno))
                {
                    sql = "exec ft_up_FishUpdateStockAccount @branch_no,@item_no,@sale_qnty,@sell_way";
                    var inputSql = new
                    {
                        et.branch_no,
                        et.item_no,
                        et.sale_qnty,
                        et.sell_way
                    };
                    DataTable dt2 = await _context.GetDataTableAsync(sql, inputSql);
                    Logger.Info(string.Format("更新库存：flow_no={0}", et.flow_no));

                    string rererf = et.returnsflowno.Substring(0, et.returnsflowno.IndexOf("@"));
                    sql = "insert into ft_return_order(sStoreNO,dTradeDate,sFishNO,nSerID,nItem) values ('" + userinfo.StorageNo + "',getdate(),'" + userinfo.TerminalID + "','" + rererf + "','0')";
                    var countRer = await _context.ExecuteNonQueryAsync(sql, null);
                    Logger.Info(string.Format("获取上单，并更新最后时间：flow_no={0}", et.flow_no));
                    //获取上单，并更新最后时间
                    string receiptId = userinfo.TerminalID.ToString() + Convert.ToString(Convert.ToInt32(et.flow_no.Substring(3, 4)) - 1).PadLeft(4, '0');
                    sql = @"update Purchase set dLastUpdateTime=dateadd(mi,3,GETDATE()) where DATEDIFF(DAY,BuyDate,getdate()) = 0 and Status=2 ";
                    if (!string.IsNullOrEmpty(receiptId))
                    {
                        sql += string.Format(" and ReceiptID = '{0}'", receiptId);
                    }
                    var result = await _localDb.ExecuteNonQueryAsync(sql, null);
                    Logger.Info(string.Format("更新订单状态，删除临时菜品表：flow_no={0}", et.flow_no));
                    //更新订单状态，删除临时菜品表
                    sql = string.Format(@"update Purchase set Status = 2,dLastUpdateTime = dateadd(mi,3,GETDATE())
                                    where DATEDIFF(DAY,BuyDate,getdate()) = 0 and ReceiptID = '{0}'", et.flow_no);
                    result += await _localDb.ExecuteNonQueryAsync(sql, null);

                    sql = string.Format(@"delete from TempItem where DATEDIFF(DAY,BuyDate,getdate()) = 0 and ReceiptID= '{0}'", et.flow_no);
                    result += await _localDb.ExecuteNonQueryAsync(sql, null);

                    Logger.Info(string.Format("更新单号：flow_no={0}", et.flow_no));
                    sql = "update sysVar set IntValue=IntValue+1 where Name='CURRMAXNO'";
                    result += await _localDb.ExecuteNonQueryAsync(sql, null);
                }
            }
            catch (Exception ex)
            {
                Logger.Error("写入销售数据失败：" + ex.ToString());
                return new Result(ResultCode.Fail, "写入销售记录失败！");
            }

            return new Result(ResultCode.Ok, "写入销售记录，更新库存成功！");
        }

        private void CreateTempOrder(List<OrderDetail> ordergoods, string orderNo, UserData userData)
        {
            var sqlInput = new DiscountProcInput
            {
                cashierno = userData.UserID,
                icno = userData.icno,
                receiptid = orderNo,
                saleway = ((int)EnumSaleWay.Refund).ToString(),
                storeno = userData.StorageNo
            };
            //组合套餐||普通商品
            ordergoods.ForEach(w =>
            {
                sqlInput.barcodes += w.bbarcode + "-";
                sqlInput.rates += (100 / 100.00).ToString("0.00") + "-";
                sqlInput.goodsids += w.item_subno + "-";
                sqlInput.prices += w.orderprice + "-";
                sqlInput.qtys += w.sale_qnty + "-";
            });

            var sql = @"exec ft_pr_saletmp_ipad @storeno,@receiptid,@cashierno,@saleway,@barcodes,@qtys,@prices,@goodsids,@icno,@rates";
            var dataSet = _localDb.GetDataSet(sql, sqlInput);
            var flag = Convert.ToDecimal(dataSet.Tables[0].Rows[0][0]);
            if (flag == -1)
            {
                throw new Abp.UI.UserFriendlyException("出现单号重复，请重新获取单号！");
            }
        }
    }
}
