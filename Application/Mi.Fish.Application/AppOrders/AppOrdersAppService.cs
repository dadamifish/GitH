using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Mi.Fish.Infrastructure.Results;
using System.Net.Http;
using Mi.Fish.ApplicationDto;
using Newtonsoft.Json;
using System.Linq;
using Abp.Runtime.Caching;
using Mi.Fish.EntityFramework;
using System.Data;
using Mi.Fish.Common;
using Mi.Fish.Application.Order;
using System.Security.Cryptography;
using Mi.Fish.Application.Sync;
using Mi.Fish.Application.SaleMenu;

namespace Mi.Fish.Application.AppOrders
{
    public class AppOrdersAppService : AppServiceBase, IAppOrdersAppService
    {
        private readonly LocalDbContext _localDbContext;
        private readonly ParkDbContext _parkDbContext;
        private readonly ICacheManager _cacheManager;
        private readonly IOrderNoAppService _orderNoAppService;
        private readonly ISyncAppService _syncAppService;
        private readonly ISaleMenuAppService _saleMenuAppService;

        public AppOrdersAppService(LocalDbContext localDbContext, ParkDbContext parkDbContext, ICacheManager cacheManager, IOrderNoAppService orderNoAppService,
            ISyncAppService syncAppService, ISaleMenuAppService saleMenuAppService)
        {
            _localDbContext = localDbContext;
            _parkDbContext = parkDbContext;
            _cacheManager = cacheManager;
            _orderNoAppService = orderNoAppService;
            _syncAppService = syncAppService;
            _saleMenuAppService = saleMenuAppService;
        }
        /// <summary>
        /// 获取app订单，根据POS系统进行逻辑处理
        /// </summary>
        /// <param name="status">订单状态</param>
        /// <returns></returns>
        public async Task<Result<List<FishOrdersOutputDto>>> GetAppOrdersAsync(AppOrderStatus status)
        {
            string userId = FishSession.UserId;
            var userData = _cacheManager.GetUserDataCacheByUserId(userId);
            List<AppOrderMain> appOrder;
            if (status == AppOrderStatus.Paid)
            {
                appOrder = await GetAppOrdersByStatusAsync(AppOrderStatus.Paid);
                appOrder.Intersect(await GetAppOrdersByStatusAsync(AppOrderStatus.Arrive));
            }
            else
            {
                appOrder = await GetAppOrdersByStatusAsync(status);
            }
            List<FishOrdersOutputDto> posOrders = new List<FishOrdersOutputDto>();
            if (appOrder.Any())
            {
                string itemStatus = string.Empty;
                foreach (var main in appOrder)
                {
                    FishOrdersOutputDto order = new FishOrdersOutputDto();

                    order.OrderId = main.Id;
                    order.StoreId = userData.StorageNo;
                    order.MealId = string.Join("-", main.MealDetailList.Select(s => s.AssignedDiningMealMealId.Replace("-", "").Trim()).ToArray());
                    order.MealName = string.Join("-", main.MealDetailList.Select(s => s.AssignedDiningMealMealName.Trim()).ToArray());
                    order.Qty = string.Join("-", main.MealDetailList.Select(s => s.Quantity.Trim()).ToArray());
                    order.Price = string.Join("-", main.MealDetailList.Select(s =>Convert.ToDouble(s.Price.Trim()).ToString("N")).ToArray());
                    order.BookingTime = main.MealTimeValue;

                    if (main.MealDetailList.Any())
                    {
                        itemStatus = main.MealDetailList[0].Status;
                    }
                    if (itemStatus == "1")
                    {
                        if (main.GetMealType == "0")
                        {
                            order.status = AppOrderStatus.Paid;
                        }
                        else
                        {
                            order.status = AppOrderStatus.WaitTake;
                        }
                    }
                    else
                    {
                        order.status = (AppOrderStatus)int.Parse(itemStatus);
                    }

                    order.GetMealType = main.GetMealType.Trim();
                    order.Remark = main.Remark ?? main.Remark.Trim();
                    posOrders.Add(order);
                }
            }
            return Result.FromData(posOrders);
        }
        /// <summary>
        /// 获取app订单
        /// </summary>
        /// <param name="status">订单状态</param>
        /// <returns></returns>
        private async Task<List<AppOrderMain>> GetAppOrdersByStatusAsync(AppOrderStatus status)
        {
            string userId = FishSession.UserId;
            var _userData = _cacheManager.GetUserDataCacheByUserId(userId);
            string storeId = _userData.StorageNo;
            string token = await GetAppTokenAsync(_userData.AppLink, _userData.LeyouAccount, _userData.LeyouPwd);
            string date = DateTime.Now.ToString("yyyy-MM-dd");
            //不分页，获取所有数据
            int pageSize = 10000;
            int pageIndex = 0;

            string url = _userData.AppLink + "api/DinnerSystemSynchronization/QueryOrderListForMealSystem";
            string data = $"StoreId={storeId}&Status={status}&pageSize={pageSize}&pageIndex={pageIndex}&OrderBeginDate={date}&OrderEndDate={date}&token={token}";
            AppOrdersOutputDto appResult = await HttpClientExtensions.GetAsync<AppOrdersOutputDto>(url, data);
            return appResult.Data.List;
        }

        /// <summary>
        /// 接单操作，App订单转Fish单
        /// 1.判断是否已接单
        /// 2.修改app端订单状态为已接单
        /// 3.转POS单(成功：减库存，打小票；失败：修改线上app订单的状态)
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<Result<CreateOrderNoDto>> CreateFishOrdersAsync(AppOrderInputDto input)
        {
            if (input.Status != AppOrderStatus.Paid && input.Status != AppOrderStatus.WaitTake && input.Status != AppOrderStatus.Arrive)
            {
                return new Result<CreateOrderNoDto>(ResultCode.Fail, null, "该单状态为不可接单");
            }
            //判断是否已接单
            string sql = "select 1 from ft_app_order where saleno=@OrderId ";
            object obj = new { OrderId = input.OrderId };
            object objResult = await _parkDbContext.ExecuteScalarAsync(sql, obj);
            if (objResult != null)
            {
                return new Result<CreateOrderNoDto>(ResultCode.Fail, null, "该单为已接单，不可再接单");
            }
            //type 0 立即接单  1 预定接单扣库存 2 预定接单取餐出票
            int type = 0;
            int OrgStatus = 0;
            int DesStatus = 0;
            if (input.Status == AppOrderStatus.Paid || input.Status == AppOrderStatus.Arrive)
            {
                if (input.Status == AppOrderStatus.Arrive)
                {
                    type = 2;
                }
                OrgStatus = (int)input.Status;
                DesStatus = 10;
            }
            else
            {
                OrgStatus = 1;
                DesStatus = 8;
            }
            using (HttpClient http = new HttpClient())
            {
                string userId = FishSession.UserId;
                var _userData = _cacheManager.GetUserDataCacheByUserId(userId);
                //修改app端订单状态
                string storeId = _userData.StorageNo;
                string url = _userData.AppLink + "api/DinnerSystemSynchronization/AlterOrderStatusForSystem";
                string token = await GetAppTokenAsync(_userData.AppLink, _userData.LeyouAccount, _userData.LeyouPwd);
                string data = string.Concat("{\"OrgStatus\":", OrgStatus, ",\"DesStatus\":", DesStatus, ",\"StoreId\":", storeId, ",\"OrderId\":\"", input.OrderId, "\"}");
                StringContent content = new StringContent(data, Encoding.UTF8, "application/json");
                http.DefaultRequestHeaders.Add("token", token);
                var response = await http.PostAsync(url, content);
                string result = await response.Content.ReadAsStringAsync();
                AppResult<AlterAppOrderStatusOutputDto> appResult = JsonConvert.DeserializeObject<AppResult<AlterAppOrderStatusOutputDto>>(result);
                if (appResult.Result == "0" && appResult.Data.Result)
                {
                    string mealCode = appResult.Data.MealCode.Trim();
                    string flowNo = (await _orderNoAppService.CreateOrderNo()).Data;
                    await _saleMenuAppService.ClearMenu();//清空购物车
                    Result<string> resultFish = await SaleGoodsAsync(flowNo,input.MealId, input.Qty, input.Price, input.OrderId, type, mealCode, input.Remark);

                    if (resultFish.IsSuccess)
                    {
                        string orderMarkSql = String.Format(@"insert into ft_app_order (saleno,status,inputby,inputtime,flow_no,jh,branch_no,qrCode,dLastUpdateTime) values ('{0}',0,'{1}',GETDATE(),'{2}','{3}','{4}','{5}',GETDATE()) ",
                        input.OrderId, _userData.UserID, resultFish.Data.Trim(), _userData.TerminalID, _userData.StorageNo, appResult.Message);
                        await _parkDbContext.ExecuteNonQueryAsync(orderMarkSql, null);

                        try
                        {
                            _syncAppService.SyncData(_userData.StorageNo,_userData.TerminalID,false);
                        }
                        catch (Exception ex)
                        {
                            Logger.Error($"门店{storeId}机号{_userData.TerminalID}接单同步失败！");
                        }

                        return Result.FromData(new CreateOrderNoDto() { OrderNo = flowNo });
                        //if (type == 1)
                        //{
                        //    //预定 接单成功， 不打印小票
                        //    return Result.FromData(new CreateOrderNoDto() { OrderNo = flowNo });
                        //}

                        //生成打印信息
                        //OrdersPrintOutputDto printDto = await GetOrderPrintInfoAsync(input.OrderId, "方特旅游App订单", resultFish.Data.Trim(), SaleMode.A, mealCode, input.Remark);
                    }
                    else
                    {
                        //调接口更改 方特旅游App 状态为未接单
                        if (input.Status == AppOrderStatus.Paid || input.Status == AppOrderStatus.Arrive)
                        {
                            OrgStatus = 10;
                            DesStatus = Convert.ToInt32(input.Status);
                        }
                        else
                        {
                            OrgStatus = 8;
                            DesStatus = 1;
                        }
                        url = _userData.AppLink + "api/DinnerSystemSynchronization/RollBackOrderStatusForSystem";
                        data = string.Concat("{\"OrgStatus\":", OrgStatus, ",\"DesStatus\":", DesStatus, ",\"StoreId\":", storeId, ",\"OrderId\":\"", input.OrderId, "\"}");
                        using (HttpClient http1 = new HttpClient())
                        {
                            http1.DefaultRequestHeaders.Add("token", token);
                            StringContent content1 = new StringContent(data, Encoding.UTF8, "application/json");
                            var response1 = await http1.PostAsync(url, content1);
                            string result1 = await response1.Content.ReadAsStringAsync();
                            if (result1.Split(',')[0].ToString().Trim() != "success")
                            {
                                Logger.Info($"接单失败：{input.OrderId}退款成功；" + resultFish.Message);
                                return new Result<CreateOrderNoDto>(ResultCode.FailAndRefresh, null, "单号：" + input.OrderId + " ，接单失败," + resultFish.Message);
                            }
                            else
                            {
                                Logger.Info($"接单失败：{input.OrderId}退款失败");
                                return new Result<CreateOrderNoDto>(ResultCode.Fail, null, "单号：" + input.OrderId + " ，接单失败");
                            }
                        }
                    }
                }
                else
                {
                    Logger.Info($"接单失败：{input.OrderId}修改app端订单状态失败；" + appResult.Data.Message);
                    return new Result<CreateOrderNoDto>(ResultCode.Fail, null, appResult.Data.Message);
                }
            }
        }

        private async Task<Result<string>> SaleGoodsAsync(string receiptId,string GoodsNoList, string qtyList, string priceList, string salenoAccept, int type,string mealCode,string Memo)
        {
            try
            {
                string userId = FishSession.UserId;
                var _userData = _cacheManager.GetUserDataCacheByUserId(userId);
                string storeId = _userData.StorageNo;
                string cashierno = _userData.UserID;
                string saleway = "0";

                StringBuilder rates = new StringBuilder();
                GoodsNoList = GoodsNoList.TrimEnd('-') + '-';
                qtyList = qtyList.TrimEnd('-') + '-';
                priceList = priceList.TrimEnd('-') + '-';

                string[] goodsNoArr = GoodsNoList.Split('-', StringSplitOptions.RemoveEmptyEntries);
                string[] qtyArr = qtyList.Split('-', StringSplitOptions.RemoveEmptyEntries);
                string[] priceArr = priceList.Split('-', StringSplitOptions.RemoveEmptyEntries);
                if (goodsNoArr.Length != qtyArr.Length || goodsNoArr.Length != priceArr.Length)
                {
                    return Result.Fail<string>("订单数据异常，商品数量与与种类不匹配");
                }

                for (int i = 0; i < goodsNoArr.Length; i++)
                {
                    //判断是否可售商品
                    string sql = string.Format("SELECT COUNT(*) FROM base_entry_message_allow WHERE item_no = '{0}' and branch_no = '{1}' and jh = '{2}' and activeflag = 1 ", goodsNoArr[i], _userData.StorageNo, _userData.TerminalID);
                    object objResult = await _parkDbContext.ExecuteScalarAsync(sql, null);
                    if (objResult == null || int.Parse(objResult.ToString()) <= 0)
                    {
                        return Result.Fail<string>("提示：商品编码 " + goodsNoArr[i] + "  无效或不可售套餐，请添加可售商品设置后再销售！");
                    }

                    //查看库存，库存不足返回
                    if (!await haveStockAsync(storeId, goodsNoArr[i], int.Parse(qtyArr[i].ToString())))
                    {
                        return Result.Fail<string>("商品编码为 " + goodsNoArr[i] + " 的商品库存不足，请添加库存！");
                    }
                    rates.Append("1").Append("-");
                }

                //type 0 立即接单  1 预定接单扣库存 2 预定接单取餐出票
                DataSet b_dttmp = await AddSaleInfoAppAsync(storeId, receiptId, cashierno, saleway, GoodsNoList, qtyList, priceList, GoodsNoList, salenoAccept, rates.ToString(), type, mealCode, Memo);
                //生成新单号
                //_userData.Flow_No = await UpdateFlowNoAsync(_userData.TerminalID);

                if (type == 1)
                {
                    return Result.FromData("");
                }
                if (b_dttmp != null && b_dttmp.Tables.Count > 0 && b_dttmp.Tables[0].Rows.Count > 0)
                {

                    if (int.Parse(b_dttmp.Tables[0].Rows[0][0].ToString()) > 0)
                    {
                        return Result.FromData(b_dttmp.Tables[0].Rows[0]["flowno"].ToString());
                    }
                    else
                    {
                        return Result.Fail<string>("转销售单错误");
                    }
                }
                else
                {
                    return Result.Fail<string>(b_dttmp.Tables[0].Rows[0][1].ToString());
                }
            }
            catch (Exception ex)
            {
                return Result.Fail<string>(ex.Message);
            }
        }
        //根据商品类别查看是否库存
        private async Task<Boolean> haveStockAsync(string storeId, string itemNo, int count)
        {
            try
            {
                //获取库存信息
                int stockQty = await SearchStorageAsync(storeId, itemNo);
                return stockQty >= count;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        //返回库存信息
        private async Task<int> SearchStorageAsync(string storeId, string itemNo)
        {
            try
            {
                string sql = " exec p_get_nowstock @branch_no,@item_no";
                object obj = new { branch_no = storeId, item_no = itemNo };
                object objResult = await _parkDbContext.ExecuteScalarAsync(sql, obj);
                return objResult == null ? -1 : Convert.ToInt32(objResult);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 增加方特旅游App接单写销售数据
        /// </summary>
        /// <param name="s">销售信息实体类</param>
        /// <returns></returns>
        private async Task<DataSet> AddSaleInfoAppAsync(string storeno, string receiptid, string cashierno, string saleway, string barcodes, string qtys, string prices, string goodsids, string salenoAccept, string rates, int type, string mealCode, string Memo)
        {
            try
            {
                //type 0 立即接单  1 预定接单扣库存 2 预定接单取餐出票
                if (type == 0)
                {
                    string salqSql = " exec ft_pr_sale_app_addInfo @storeno,@receiptid,@cashierno,@saleway,@barcodes,@qtys,@prices,@goodsids,@rates,@appsaleno,@mealCode,@Memo";
                    object salqObj = new { storeno, receiptid, cashierno, saleway, barcodes, qtys, prices, goodsids, rates, appsaleno = salenoAccept, mealCode, Memo };
                    DataSet ds = _localDbContext.GetDataSet(salqSql, salqObj);

                    ///成功后扣库存
                    if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                    {
                        if (int.Parse(ds.Tables[0].Rows[0][0].ToString()) > 0)
                        {
                            //更新库存
                            await UpdataStockAsync(storeno, barcodes, qtys, SaleMode.A);
                        }
                    }
                    return ds;
                }
                else if (type == 1)
                {
                    //更新库存
                    await UpdataStockAsync(storeno, barcodes, qtys, SaleMode.A);
                    return null;
                }
                else
                {
                    string salqSql = " exec ft_pr_sale_app_addInfo @storeno,@receiptid,@cashierno,@saleway,@barcodes,@qtys,@prices,@goodsids,@rates,@appsaleno,@mealCode,@Memo";
                    object salqObj = new { storeno, receiptid, cashierno, saleway, barcodes, qtys, prices, goodsids, rates, appsaleno = salenoAccept, mealCode, Memo };
                    DataSet ds = _localDbContext.GetDataSet(salqSql, salqObj);
                    return ds;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// App订单退单
        /// </summary>
        /// <param name="orderId"></param>
        /// <returns></returns>
        public async Task<Result<CreateOrderNoDto>> DeleteOrdersAsync(string orderId)
        {
            string userId = FishSession.UserId;
            var _userData = _cacheManager.GetUserDataCacheByUserId(userId);
            if (!_userData.BackGoods)
            {
                if (!_cacheManager.GetUserAuthority(_userData.UserID, EnumAuthorityType.BackGood))
                {
                    return new Result<CreateOrderNoDto>(ResultCode.Proxyauthorized, null, "没有退款权限");
                }
            }
            _cacheManager.RemoveUserAuthority(_userData.UserID, EnumAuthorityType.BackGood);
            string storeId = _userData.StorageNo;
            string TerminalID = _userData.TerminalID;
            string sql = "select nSerID from tFishPayt where sStoreNO = '" + _userData.StorageNo + "' and sFishNO = '" + TerminalID + "' and dTradeDate >= Convert(varchar(10), getdate(), 120) and sPaytNO= '" + orderId + "'";
            string oldFlowNo = string.Empty;
            string newFlowNo = (await _orderNoAppService.CreateOrderNo()).Data;
            string token = await GetAppTokenAsync(_userData.AppLink, _userData.LeyouAccount, _userData.LeyouPwd);
            object objReturn = await _parkDbContext.ExecuteScalarAsync(sql, null);
            if (objReturn != null)
            {
                oldFlowNo = objReturn.ToString();
            }
            else
            {
                return new Result<CreateOrderNoDto>(ResultCode.Fail, null, "该单号无效，或未同步");
            }
            sql = "select 1 from ft_return_order where sStoreNO = '" + _userData.StorageNo + "' and sFishNO = '" + _userData.TerminalID + "' and nSerID = '" + oldFlowNo + "' and dTradeDate>= Convert(varchar(10), getdate(), 120) and dTradeDate<Convert(varchar(10),getdate() + 1,120)";
            objReturn = await _parkDbContext.ExecuteScalarAsync(sql, null);
            if (objReturn != null)
            {
                return new Result<CreateOrderNoDto>(ResultCode.Fail, null, "该单号已经退过，不能重复");
            }
            sql = "select 1 from tFishSale where sStoreNO = '" + _userData.StorageNo + "' and sFishNO = '" + _userData.TerminalID + "' and nSerID = '" + oldFlowNo + "' and dTradeDate>= Convert(varchar(10), getdate(), 120) and dTradeDate<Convert(varchar(10),getdate() + 1,120) and nTradeType = 2 ";
            objReturn = await _parkDbContext.ExecuteScalarAsync(sql, null);
            if (objReturn != null)
            {
                return new Result<CreateOrderNoDto>(ResultCode.Fail, null, "无效单号，不能退单");
            }
            sql = string.Format("select 1 from tFishSaleDtl t1 where t1.sStoreNO = '{0}' and t1.sFishNO = '{1}' and t1.nSerID = '{2}' and dTradeDate>=Convert(varchar(10),getdate(),120) and dTradeDate<Convert(varchar(10),getdate()+1,120)",
                storeId, TerminalID, oldFlowNo);
            objReturn = await _parkDbContext.ExecuteScalarAsync(sql, null);
            if (objReturn == null)
            {
                return new Result<CreateOrderNoDto>(ResultCode.Fail, null, "无此单号或无效单号");
            }
            await _saleMenuAppService.ClearMenu();//清空购物车
            sql = "exec ft_pr_Resale_app @store,@jh,@orderId,@newFlowNo";
            object obj = new { store = storeId, jh = TerminalID, orderId = orderId, newFlowNo = newFlowNo };
            DataTable dtReOrder = await _localDbContext.GetDataTableAsync(sql, obj);
            if (dtReOrder != null && dtReOrder.Rows.Count > 0)
            {
                if (dtReOrder.Rows[0][0].ToString() == "1")
                {
                    //生成新单号
                    //_userData.Flow_No = await UpdateFlowNoAsync(_userData.TerminalID);

                    string GoodsCodes = dtReOrder.Rows[0]["GoodsCodes"].ToString();
                    string StockQtys = dtReOrder.Rows[0]["StockQtys"].ToString();
                    //更新库存
                    await UpdataStockAsync(storeId, GoodsCodes, StockQtys, SaleMode.B);

                    sql = "insert into ft_return_order(sStoreNO, dTradeDate, sFishNO, nSerID, nItem) values('" + _userData.StorageNo + "', getdate(), '" + _userData.TerminalID + "', '" + oldFlowNo + "', '0')";
                    await _parkDbContext.ExecuteNonQueryAsync(sql, null);
                    string url = _userData.AppLink + "api/DinnerSystemSynchronization/AlterOrderStatusForSystem";
                    //"OrgStatus=" + "10" + "&DesStatus=997&StoreId=" + UserData.StorageNo + "&OrderId=" + leyoutradeno + "&token=" + UserData.leyouToken;
                    string data = "{\"OrgStatus\":10,\"DesStatus\":997,\"StoreId\":\"" + storeId + "\",\"OrderId\":\"" + orderId + "\"}";
                    using (HttpClient http = new HttpClient())
                    {
                        http.DefaultRequestHeaders.Add("token", token);
                        StringContent content = new StringContent(data, Encoding.UTF8, "application/json");
                        var response = await http.PostAsync(url, content);
                        string result = await response.Content.ReadAsStringAsync();
                        AppResult<AlterAppOrderStatusOutputDto> appResult = JsonConvert.DeserializeObject<AppResult<AlterAppOrderStatusOutputDto>>(result);
                        if (appResult.Result == "0" && appResult.Data.Result)
                        {
                            try
                            {
                                _syncAppService.SyncData(_userData.StorageNo, _userData.TerminalID, false);
                            }
                            catch (Exception ex)
                            {
                                Logger.Error($"门店{storeId}机号{TerminalID}接单同步失败！");
                            }
                            //OrdersPrintOutputDto printDto = await GetOrderPrintInfoAsync(orderId, "方特旅游App订单", newFlowNo, SaleMode.B);
                            return Result.FromData(new CreateOrderNoDto() { OrderNo = newFlowNo });
                        }
                        else
                        {
                            Logger.Info($"退单失败：{orderId}修改app端订单状态失败；" + appResult.Message);
                            return new Result<CreateOrderNoDto>(ResultCode.Fail, null, "退单接口异常");
                        }
                    }
                }
                else
                {
                    Logger.Info($"退单失败：{orderId}生成负单失败；" + dtReOrder.Rows[0][1].ToString());
                    return new Result<CreateOrderNoDto>(ResultCode.Fail, null, "退单失败:" + dtReOrder.Rows[0][1].ToString());
                }
            }
            else
            {
                Logger.Info($"退单失败：{orderId}生成负单失败；");
                return new Result<CreateOrderNoDto>(ResultCode.Fail, null, "退单失败");
            }
        }

        /// <summary>
        /// 更新库存
        /// </summary>
        /// <param name="storeId">门店号</param>
        /// <param name="goodsCodes">商品编码</param>
        /// <param name="stockQtys">数量</param>
        /// <param name="saleMode">类型</param>
        /// <returns></returns>
        private async Task<bool> UpdataStockAsync(string storeId, string goodsCodes, string stockQtys, SaleMode saleMode)
        {
            try
            {
                //更新库存
                string stockSql = " exec ft_up_FishUpdateStockAccount @StoreNO,@GoodsCodes,@StockQtys,@sell_way";
                object stockObj = new { StoreNO = storeId, GoodsCodes = goodsCodes.Replace("-", "@").ToString(), StockQtys = stockQtys.Replace("-", "@"), sell_way = saleMode.ToString() };
                DataTable dtStock = await _parkDbContext.GetDataTableAsync(stockSql, stockObj);
                if (dtStock != null && dtStock.Rows.Count > 0 && dtStock.Rows[0][0].ToString() == "0")
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        /// <summary>
        /// 打印小票信息
        /// </summary>
        /// <param name="OrderId">App订单号</param>
        /// <param name="FlowNo">Fish系统订单号</param>
        /// <param name="SaleMode">模式：A 销售,B 退单</param>
        /// <param name="QRCode">取餐码</param>
        /// <param name="Remark">备注</param>
        /// <returns></returns>
        private async Task<OrdersPrintOutputDto> GetOrderPrintInfoAsync(string OrderId, string OrderName, string FlowNo, SaleMode SaleMode, string QRCode = "", string Remark = "")
        {
            string userId = FishSession.UserId;
            var _userData = _cacheManager.GetUserDataCacheByUserId(userId);
            string sqlPrint = "";

            sqlPrint += " select a.ReceiptID,BuyTime,sGoodsName,sMainBarcode,Qty,Price,Amount from Purchase a";
            sqlPrint += " left join PurchaseItem b on a.buydate=b.buydate and a.receiptid=b.receiptid";
            sqlPrint += " left join ft_v_SaleGoods c on  b.goodsid=c.ngoodsid";
            sqlPrint += " where a.receiptid='" + FlowNo.Trim() + "' and CONVERT(varchar(100), a.buydate, 112)=CONVERT(varchar(100), getdate(), 112)";
            //获取订单信息打印小票
            DataTable orderDt = await _localDbContext.GetDataTableAsync(sqlPrint, null);
            OrdersPrintOutputDto printDto = new OrdersPrintOutputDto();
            if (orderDt != null && orderDt.Rows.Count > 0)
            {
                //设置打印信息
                printDto.ThirdTradeNo = OrderId;
                printDto.Cashier = _userData.UserName;
                printDto.TerminalID = _userData.TerminalID;
                printDto.SaleMode = SaleMode.DisplayName();
                printDto.TableId = "";
                printDto.Title = OrderName;
                printDto.QrCode = QRCode;
                printDto.Remark = Remark;

                if (orderDt != null && orderDt.Rows.Count > 0)
                {
                    printDto.Amount = (Convert.ToDouble(orderDt.Rows[0]["Amount"])).ToString("N");
                    printDto.PayAmount = printDto.Amount;

                    printDto.OrderNo = orderDt.Rows[0]["ReceiptID"].ToString();
                    printDto.TradeTime = orderDt.Rows[0]["BuyTime"].ToString();
                    for (int i = 0; i < orderDt.Rows.Count; i++)
                    {
                        OrderMealPrint p = new OrderMealPrint();
                        p.GoodsName = orderDt.Rows[i]["sGoodsName"].ToString();
                        p.Qty = (Convert.ToInt32(orderDt.Rows[i]["Qty"])).ToString();
                        p.Price = (Convert.ToDouble(orderDt.Rows[i]["Price"])).ToString("N");
                        p.Amount = (Convert.ToDouble(orderDt.Rows[i]["Amount"])).ToString("N");
                        printDto.Goods.Add(p);
                    }
                }
            }
            return printDto;
        }

        public async Task<Result<OrdersPrintDto>> PrintInfoAsync(string orderNo,int isRePrint)
        {
            string userId = FishSession.UserId;
            var _userData = _cacheManager.GetUserDataCacheByUserId(userId);
            string sqlPrint = "";

            sqlPrint += " select a.ReceiptID,a.BuyType,BuyTime,sGoodsName,sMainBarcode,Qty,Price,Amount,a.Memo,c.sGoodTypeID from Purchase a";
            sqlPrint += " left join PurchaseItem b on a.buydate=b.buydate and a.receiptid=b.receiptid";
            sqlPrint += " left join ft_v_SaleGoods c on  b.goodsid=c.ngoodsid";
            sqlPrint += " where a.receiptid='" + orderNo + "' and CONVERT(varchar(100), a.buydate, 23)=CONVERT(varchar(100), getdate(), 23)";//getdate()
            //获取订单信息打印小票
            DataTable orderDt = await _localDbContext.GetDataTableAsync(sqlPrint, null);
            OrdersPrintOutputDto printDto = new OrdersPrintOutputDto();
            if (orderDt != null && orderDt.Rows.Count > 0)
            {
                //设置打印信息
                sqlPrint = " select * from Payment a where a.receiptid='" + orderNo + "' and CONVERT(varchar(100), a.buydate, 112)=CONVERT(varchar(100), getdate(), 112)";
                DataTable payDt = await _localDbContext.GetDataTableAsync(sqlPrint, null);
                if (payDt != null && payDt.Rows.Count > 0)
                {
                    printDto.ThirdTradeNo = payDt.Rows[0]["CardID"].ToString();
                    printDto.QrCode = payDt.Rows[0]["Memo"].ToString();
                }
                printDto.Cashier = _userData.UserName;
                printDto.TerminalID = _userData.TerminalID;
                printDto.TableId = "";
                printDto.Title = "方特旅游App订单";
                //printDto.IsRePrint = isRePrint > 0 ? true : false;


                //printDto.Discount = "0.00";
                //printDto.DueAmount = "0.00";
                //printDto.MuToneId = "";
                if (orderDt != null && orderDt.Rows.Count > 0)
                {
                    printDto.Amount = (Convert.ToDouble(orderDt.Rows[0]["Amount"])).ToString("N");
                    printDto.PayAmount = printDto.Amount;
                    printDto.PayMents.Add(new PayMent { PayName = "乐游方特APP", Amount = printDto.Amount });
                    printDto.Remark = orderDt.Rows[0]["Memo"].ToString();
                    printDto.OrderNo = orderDt.Rows[0]["ReceiptID"].ToString();
                    printDto.TradeTime = orderDt.Rows[0]["BuyTime"].ToString();
                    printDto.SaleMode = orderDt.Rows[0]["BuyType"].ToString() == "0" ? SaleMode.A.DisplayName() : SaleMode.B.DisplayName();
                    for (int i = 0; i < orderDt.Rows.Count; i++)
                    {
                        OrderMealPrint p = new OrderMealPrint();
                        p.GoodsName = orderDt.Rows[i]["sGoodsName"].ToString();
                        //p.MealBarcode = orderDt.Rows[i]["sMainBarcode"].ToString();
                        p.Qty = (Convert.ToInt32(orderDt.Rows[i]["Qty"])).ToString();
                        p.Price = (Convert.ToDouble(orderDt.Rows[i]["Price"])).ToString("N");
                        p.Amount = (Convert.ToDouble(orderDt.Rows[i]["Amount"])).ToString("N");
                        if (orderDt.Rows[i]["sGoodTypeID"].ToString() == "C")
                        {
                            string GoodNo= orderDt.Rows[i]["sMainBarcode"].ToString();
                            string SqlGoodsSub = @"SELECT GoodsNo = b.sGoodsNO,GoodsName = b.sGoodsName,Qty=convert(int,a.nQty),Price = b.nRealSalePrice,Amount=b.nRealSalePrice FROM tComplexElement  
                                                    a left join ft_v_SaleGoods b on a.nElementID = b.nGoodsID and b.sStoreNO = @StoreNO WHERE
                                                    (select sGoodsNO from tGoods where a.nTag & 1 = 0 and nGoodsID = a.nGoodsID) = @GoodsNo
                                                    ORDER BY a.nElementID ASC ";
                            object objGoodSub = new { StoreNO = _userData.StorageNo, GoodsNo = GoodNo };
                            List<GoodSub> goodSub = await _parkDbContext.ExecuteFunctionAsync<List<GoodSub>>(SqlGoodsSub, objGoodSub);
                            if (int.Parse(p.Qty) > 1)
                            {
                                goodSub.ForEach((item) =>
                                {
                                    item.Qty = (Convert.ToInt32(p.Qty) * Convert.ToInt32(item.Qty)).ToString();
                                    item.Amount = (Convert.ToInt32(item.Qty) * Convert.ToDouble(item.Price)).ToString("N");
                                });
                            }
                            p.GoodSubs= goodSub;
                        }
                        printDto.Goods.Add(p);
                    }
                }
            }
            return Result.FromData(AutoMapper.Mapper.Map<OrdersPrintDto>(printDto));
        }
        /// <summary>
        /// 获取 token
        /// </summary>
        /// <returns></returns>
        private async Task<string> GetAppTokenAsync(string url, string userName, string encryPsw)
        {
            LeyouTokenCache Apptoken = _cacheManager.GetLeyouTokenCache();
            if (Apptoken != null && Apptoken.ExpirationTime > DateTime.Now)
            {
                return Apptoken.Token;
            }
            url = url + "api/DinnerSystemSynchronization/Login";
            encryPsw = Md5Hash(encryPsw).ToUpper();
            string data = string.Concat("{\"username\":\"", userName, "\",\"encryPsw\":\"", encryPsw, "\"}");
            using (HttpClient http = new HttpClient())
            {
                StringContent content = new StringContent(data, Encoding.UTF8, "application/json");
                var response = await http.PostAsync(url, content);
                string result = await response.Content.ReadAsStringAsync();
                AppTokenOutputDto token = JsonConvert.DeserializeObject<AppTokenOutputDto>(result);
                Apptoken = new LeyouTokenCache();
                Apptoken.Token = token.Data.Token;
                Apptoken.ExpirationTime = DateTime.Now.AddMinutes(token.Data.TokenValidTime - 10);
                _cacheManager.SetLeyouTokenCache(Apptoken);
                return token.Data.Token;
            }
        }
        /// <summary>
        /// 32位MD5加密
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        private string Md5Hash(string input)
        {
            MD5CryptoServiceProvider md5Hasher = new MD5CryptoServiceProvider();
            byte[] data = md5Hasher.ComputeHash(Encoding.Default.GetBytes(input));
            StringBuilder sBuilder = new StringBuilder();
            for (int i = 0; i < data.Length; i++)
            {
                sBuilder.Append(data[i].ToString("x2"));
            }
            return sBuilder.ToString();
        }

    }
}
