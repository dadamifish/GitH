using Abp.Runtime.Caching;
using Abp.UI;
using Mi.Fish.Application.Order;
using Mi.Fish.Application.SaleMenu;
using Mi.Fish.ApplicationDto;
using Mi.Fish.EntityFramework;
using Mi.Fish.Infrastructure.Results;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
using System.Security.Policy;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace Mi.Fish.Application
{

    /// <summary>
    /// 支付接口
    /// </summary>
    public class PayAppService : AppServiceBase, IPayAppService
    {
        private readonly ParkDbContext _parkDbContext;

        private readonly LocalDbContext _context;

        private readonly ICacheManager _cacheManager;

        private readonly IOrderNoAppService _orderNoAppService;

        private readonly IOrderAppService _orderAppService;

        public PayAppService(ParkDbContext parkDbContext, ICacheManager cacheManager, LocalDbContext context
            , IOrderNoAppService orderNoAppService
            , IOrderAppService orderAppService)
        {
            _parkDbContext = parkDbContext;
            _cacheManager = cacheManager;
            _context = context;
            _orderNoAppService = orderNoAppService;
            _orderAppService = orderAppService;
        }

        #region 获取秒通四位码接口
        /// <summary>
        /// 获取秒通四位码接口
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<Result<MutoneSnCodeOutput>> GetMutoneSnCode(GetMutoneSnCodePayInput input)
        {
            Result<MutoneSnCodeOutput> model = new Result<MutoneSnCodeOutput>(ResultCode.Fail, null);

            if (input.PayMoney <= 0)
            {
                model.Message = "支付失败，" + "支付金额必须大于0";
                return model;
            }

            string userId = FishSession.UserId;
            var userData = _cacheManager.GetUserDataCacheByUserId(userId);

            if (userData == null)
            {
                model.Message = "未获取到用户信息，请先登录";
                return model;
            }
            var orderNo = (await _orderNoAppService.CreateOrderNo()).Data;

            var checkOrderInput = new CheckOrderInput()
            {
                OrderNo = orderNo
            };

            //验证库存
            var checkOrderResult = await _orderAppService.CheckOrder(checkOrderInput);
            if (checkOrderResult.Code == ResultCode.Fail || checkOrderResult.Data == null)
            {
                model.Message = checkOrderResult.Message;
                return model;
            }
            if (!checkOrderResult.Data.HasStock)
            {
                model.Message = checkOrderResult.Data.Message;
                return model;
            }


            Random rd = new Random();
            int rdnum = rd.Next(0, 20);
            string url = userData.MutonpayLink + "CGenSerialNumber?";

            string out_trade_no = "CY" + System.DateTime.Now.ToString("yyyyMMddHHmmss") + userData.StorageNo.ToString() + userData.TerminalID.ToString() + userData.UserID; //商户唯一订单号 
            string timespan = GetTimeStamp();

            string data = "merchant=" + userData.StorageName + "&orderno=" + out_trade_no + "&amount=" + input.PayMoney + "&des=" + input.GoodsName + "&clienttype=1&sncode=" + "" + "&parkid=" + userData.MutonParkid.ToString() + "&timestamp=" + timespan;

            data = "&signature=" + SHAEncrypt(data + "&key=" + userData.MutoneKey);
            string data2 = "merchant=" + System.Web.HttpUtility.UrlEncode(userData.StorageName) + "&orderno=" + out_trade_no + "&amount=" + input.PayMoney + "&des=" + System.Web.HttpUtility.UrlEncode(input.GoodsName) + "&clienttype=1&sncode=" + "" + "&parkid=" + userData.MutonParkid.ToString() + "&timestamp=" + timespan;
            data2 = data2 + data;

            Logger.Info("获取秒通四位码：" + data2);

            using (HttpClient http = new HttpClient())
            {
                var result = await http.GetAsync(url + data2);
                string json = result.Content.ReadAsStringAsync().Result;

                Logger.Info("获取秒通四位码结果：" + json);

                if (json != null)//返回结果
                {
                    MutonePayDto mutonePay = JsonConvert.DeserializeObject<MutonePayDto>(json);

                    if (mutonePay.result == "0")
                    {
                        MutoneSnCodeOutput mutoneSnCodeOutput = new MutoneSnCodeOutput();
                        mutoneSnCodeOutput.SnCode = mutonePay.data.sncode;
                        mutoneSnCodeOutput.PayOrderNo = out_trade_no;

                        model.Data = mutoneSnCodeOutput;
                        model.Code = ResultCode.Ok;
                        return model;
                    }
                    else
                    {
                        model.Message = mutonePay.message;
                        return model;
                    }
                }

            }

            return model;
        }

        #endregion

        #region 秒通四位码支付
        /// <summary>
        /// 秒通四位码支付
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<Result<PayOutput>> MutoneSnCodePay(MutoneSnCodePayInput input)
        {
            Result<PayOutput> model = new Result<PayOutput>(ResultCode.Fail, null);

            string userId = FishSession.UserId;
          
            if (string.IsNullOrWhiteSpace(userId))
            {
                model.Message = "未获取到用户信息，请先登录";
                return model;
            }

            var userData = _cacheManager.GetUserDataCacheByUserId(userId);


            var orderNo = (await _orderNoAppService.CreateOrderNo()).Data;


            string url = userData.MutonpayLink + "CSNPayResult?";
            Random rd = new Random();
            int rdnum = rd.Next(0, 20);
            string timespan = GetTimeStamp();
            string data = "merchant=" + userData.StorageName + "&orderno=" + input.PayOrderNo + "&amount=" + input.PayMoney + "&des=" + input.GoodsName + "&clienttype=1&sncode=" + input.SnCode + "&parkid=" + userData.MutonParkid.ToString() + "&timestamp=" + timespan;

            data = "&signature=" + SHAEncrypt(data + "&key=" + userData.MutoneKey);
            string data2 = "merchant=" + System.Web.HttpUtility.UrlEncode(userData.StorageName) + "&orderno=" + input.PayOrderNo + "&amount=" + input.PayMoney + "&des=" + System.Web.HttpUtility.UrlEncode(input.GoodsName) + "&clienttype=1&sncode=" + input.SnCode + "&parkid=" + userData.MutonParkid.ToString() + "&timestamp=" + timespan;
            data2 = data2 + data;

            Logger.Info("秒通四位码支付：" + data2);

            using (HttpClient http = new HttpClient())
            {
                var result = await http.GetAsync(url + data2);
                string json = result.Content.ReadAsStringAsync().Result;

                Logger.Info("秒通扫码付结果：" + json);

                if (json != null)//支付返回结果
                {
                    MutonePayDto mutonePay = JsonConvert.DeserializeObject<MutonePayDto>(json);

                    if (mutonePay.result.ToUpper() == "0")
                    {
                        if (input.PayMoney != (Convert.ToDecimal(mutonePay.data.amount.ToString())))
                        {
                            model.Message = "秒通支付成功，但支付金额异常，请联系管理员";
                            return model;
                        }

                        PayOutput payResult = new PayOutput();

                        payResult.CouponAmount = 0;
                        payResult.PayAmount = mutonePay.data.payamount;
                        payResult.PayMoney = mutonePay.data.amount;
                        payResult.PayOrderNo = input.PayOrderNo;
                        payResult.Rate = mutonePay.data.discount;
                        payResult.RateAmount = mutonePay.data.amount - mutonePay.data.payamount;
                        payResult.TradeNo = mutonePay.data.mtorderno;
                        payResult.PayType = PayType.Mutone;

                        //插入支付记录
                        try
                        {
                            string sql = "insert into ft_thirdpayment (paymodeid,postradeno,out_trade_no,tradeno,status,inputtime,inputby,paymoney,branch_no,jh) values" +
                                "('" + (int)PayType.Mutone + "','" + orderNo + "','" + input.PayOrderNo + "','" + mutonePay.data.mtorderno + "',1,GETDATE(),'"
                                + userId.ToString() + "'," + input.PayMoney + ",'" + userData.StorageNo + "','" + userData.TerminalID + "')";

                            await _parkDbContext.ExecuteNonQueryAsync(sql, null);
                        }
                        catch (Exception ex)
                        {
                            Logger.Error($"秒通支付成功，但保存数据库记录出错：" + ex.Message);
                        }

                        model.Data = payResult;
                        model.Code = ResultCode.Ok;
                        return model;

                    }
                    else
                    {
                        model.Message = "支付失败，" + mutonePay.message;
                        return model;
                    }
                }
            }

            return model;
        }
        #endregion



        #region 秒通扫码付
        /// <summary>
        /// 秒通扫码付
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<Result<PayOutput>> MutoneScanPay(SacnPayInput input)
        {

            Result<PayOutput> model = new Result<PayOutput>(ResultCode.Fail, null);


            if (input.PayMoney <= 0)
            {
                model.Message = "支付失败，" + "支付金额必须大于0";
                return model;
            }
            if (string.IsNullOrWhiteSpace(input.AuthCode))
            {
                model.Message = "支付失败，" + "请扫码支付码";
                return model;
            }

            var orderNoResult = await _orderNoAppService.CreateOrderNo();

            var orderNo = "";
            if (orderNoResult != null && orderNoResult.Code == ResultCode.Ok)
            {
                orderNo = orderNoResult.Data;
            }
            if (string.IsNullOrWhiteSpace(orderNo))
            {
                model.Message = "支付失败，" + "获取订单号失败";
                return model;
            }

            string userId = FishSession.UserId;
            if (string.IsNullOrWhiteSpace(userId))
            {
                model.Message = "未获取到用户信息，请先登录";
                return model;
            }

            var userData = _cacheManager.GetUserDataCacheByUserId(userId);


            var checkOrderInput = new CheckOrderInput()
            {
                OrderNo = orderNo
            };

            //验证码库存
            var checkOrderResult = await _orderAppService.CheckOrder(checkOrderInput);
            if (checkOrderResult.Code == ResultCode.Fail || checkOrderResult.Data == null)
            {
                model.Message = checkOrderResult.Message;
                return model;
            }
            if (!checkOrderResult.Data.HasStock)
            {
                model.Message = checkOrderResult.Data.Message;
                return model;
            }

            //paytype 1扫码支付  0 刷卡支付

            string url = userData.MutonpayLink + "CScanPayResult?";

            string timespan = GetTimeStamp();

            string out_trade_no = "CY" + System.DateTime.Now.ToString("yyyyMMddHHmmss") + userData.StorageNo.ToString() + userData.TerminalID.ToString() + userId; //商户唯一订单号

            string data = "merchant=" + userData.StorageName + "&orderno=" + out_trade_no + "&amount=" + input.PayMoney + "&des=" + input.GoodsName + "&clienttype=1&token=" + input.AuthCode + "&parkid=" + userData.MutonParkid.ToString() + "&timestamp=" + timespan;

            data = "&signature=" + SHAEncrypt(data + "&key=" + userData.MutoneKey);

            string data2 = "merchant=" + System.Web.HttpUtility.UrlEncode(userData.StorageName) + "&orderno=" + out_trade_no + "&amount=" + input.PayMoney + "&des=" + System.Web.HttpUtility.UrlEncode(input.GoodsName) + "&clienttype=1&token=" + input.AuthCode + "&parkid=" + userData.MutonParkid.ToString() + "&timestamp=" + timespan;

            data2 = data2 + data;

            Logger.Info("秒通扫码付：" + data2);

            using (HttpClient http = new HttpClient())
            {
                var result = await http.GetAsync(url + data2);
                string json = result.Content.ReadAsStringAsync().Result;

                Logger.Info("秒通扫码付结果：" + json);

                if (json != null)//支付返回结果
                {
                    MutonePayDto mutonePay = JsonConvert.DeserializeObject<MutonePayDto>(json);

                    if (mutonePay.result.ToUpper() == "0")
                    {
                        if (input.PayMoney != (Convert.ToDecimal(mutonePay.data.amount.ToString())))
                        {
                            model.Message = "秒通支付成功，但支付金额异常，请联系管理员";
                            return model;
                        }


                        PayOutput payResult = new PayOutput();

                        payResult.CouponAmount = 0;
                        payResult.PayAmount = mutonePay.data.payamount;
                        payResult.PayMoney = mutonePay.data.amount;
                        payResult.PayOrderNo = out_trade_no;
                        payResult.Rate = mutonePay.data.discount;
                        payResult.RateAmount = mutonePay.data.amount - mutonePay.data.payamount;
                        payResult.TradeNo = mutonePay.data.mtorderno;
                        payResult.PayType = PayType.Mutone;

                        //插入支付记录
                        try
                        {
                            string sql = "insert into ft_thirdpayment (paymodeid,postradeno,out_trade_no,tradeno,status,inputtime,inputby,paymoney,branch_no,jh) values" +
                                "('" + (int)PayType.Mutone + "','" + orderNo + "','" + out_trade_no + "','" + mutonePay.data.mtorderno + "',1,GETDATE(),'"
                                + userId.ToString() + "'," + input.PayMoney + ",'" + userData.StorageNo + "','" + userData.TerminalID + "')";

                            await _parkDbContext.ExecuteNonQueryAsync(sql, null);
                        }
                        catch (Exception ex)
                        {
                            Logger.Error($"秒通支付成功，但保存数据库记录出错：" + ex.Message);
                        }

                        model.Data = payResult;
                        model.Code = ResultCode.Ok;
                        return model;

                    }
                    else
                    {
                        model.Message = "支付失败，" + mutonePay.message;
                        return model;

                    }

                }
                else
                {
                    model.Message = "支付失败,请稍后重试！";
                    return model;
                }
            }

        }

        #endregion

        public static string GetTimeStamp()
        {
            TimeSpan ts = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0);
            return Convert.ToInt64(ts.TotalSeconds).ToString();
        }

        public static string SHAEncrypt(string input)
        {
            byte[] data = SHA1.Create().ComputeHash(Encoding.UTF8.GetBytes(input));

            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < data.Length; i++)
            {
                sb.Append(data[i].ToString("x2"));
            }
            return sb.ToString();
        }

        #region 微信扫码付

        /// <summary>
        /// 微信扫码付
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<Result<PayOutput>> WxScanPay(SacnPayInput input,string ip)
        {
            Result<PayOutput> model = new Result<PayOutput>(ResultCode.Fail, null);

            if (input.PayMoney <= 0)
            {
                model.Message = "支付失败，" + "支付金额必须大于0";
                return model;
            }
            if (string.IsNullOrWhiteSpace(input.AuthCode))
            {
                model.Message = "支付失败，" + "请扫码支付码";
                return model;
            }

            var orderNo = (await _orderNoAppService.CreateOrderNo()).Data;

            string userId = FishSession.UserId;
            if (string.IsNullOrWhiteSpace(userId))
            {
                model.Message = "未获取到用户信息，请先登录";
                return model;
            }

            var userData = _cacheManager.GetUserDataCacheByUserId(userId);

            var checkOrderInput = new CheckOrderInput()
            {
                OrderNo = orderNo
            };

            //验证库存
            var checkOrderResult = await _orderAppService.CheckOrder(checkOrderInput);
            if (checkOrderResult.Code == ResultCode.Fail || checkOrderResult.Data == null)
            {
                model.Message = checkOrderResult.Message;
                return model;
            }
            if (!checkOrderResult.Data.HasStock)
            {
                model.Message = checkOrderResult.Data.Message;
                return model;
            }

            string url = userData.PayLink + "/WeiXinMicroPayNew";

            string name = "";
            try
            {
                if (userData.StorageName.ToString().Trim().Length > 32)
                {
                    name = userData.StorageName.ToString().Trim().Substring(0, 30) + "..";
                }
                else
                {
                    name = userData.StorageName.ToString().Trim();
                }
            }
            catch
            {
                name = userData.StorageName.ToString().Trim();
            }

            string out_trade_no = System.DateTime.Now.ToString("yyyyMMddHHmmss") + userData.StorageNo.ToString() + userData.TerminalID.ToString() + userId; //商户唯一订单号

            string data = "parkid=" + userData.StorageNo.ToString() + "&ip=" + ip + "&terminalid=" + userData.TerminalID.ToString() + "&orderid=" + out_trade_no + "&goodsname=" + HttpUtility.UrlEncode(name) + "&goodsdetail=" + HttpUtility.UrlEncode(input.GoodsName) + "&price=" + Convert.ToInt32(input.PayMoney * 100) + "&auth_code=" + input.AuthCode + "&key=" + userData.WebKey + "&random=" + (new Random()).Next(1, 10000).ToString() + "&device_info=" + userData.StorageNo + "-" + userData.TerminalID.ToString() + "-" + orderNo;
            Logger.Info("微信扫码付：" + data);

            using (HttpClient http = new HttpClient())
            {
                StringContent content = new StringContent(data, Encoding.UTF8, "application/x-www-form-urlencoded");
                var result = await http.PostAsync(url, content);


                string json = result.Content.ReadAsStringAsync().Result;

                Logger.Info("微信扫码付：" + json);

                if (json != null)//支付返回结果
                {
                    int st = json.IndexOf('{');
                    try
                    {
                        json = json.Substring(st);
                        if (json.Length >= 9)
                        {
                            json = json.Substring(0, json.Length - 9);
                        }
                    }
                    catch (Exception ex)
                    {
                        Logger.Info("微信扫码付解析错误：" + ex.Message);
                    }

                    WxMicroPayDto wxPay = JsonConvert.DeserializeObject<WxMicroPayDto>(json);

                    if (wxPay.result.ToUpper() == "SUCCESS")
                    {
                        if (input.PayMoney != (Convert.ToDecimal(wxPay.total_fee.ToString()) / 100))
                        {
                            model.Message = "微信支付成功，但支付金额异常，请联系管理员";
                            return model;
                        }


                        PayOutput payResult = new PayOutput();

                        payResult.CouponAmount = 0;
                        payResult.PayAmount = input.PayMoney;
                        payResult.PayMoney = input.PayMoney;
                        payResult.PayOrderNo = out_trade_no;
                        payResult.Rate = 1;
                        payResult.RateAmount = 0;
                        payResult.TradeNo = wxPay.transaction_id;
                        payResult.PayType = PayType.Wx;

                        //插入支付记录
                        try
                        {
                            string sql = "insert into ft_thirdpayment (paymodeid,postradeno,out_trade_no,tradeno,status,inputtime,inputby,paymoney,branch_no,jh) values" +
                                "('" + (int)PayType.Wx + "','" + orderNo + "','" + out_trade_no + "','" + wxPay.transaction_id + "',1,GETDATE(),'"
                                + userId.ToString() + "'," + input.PayMoney + ",'" + userData.StorageNo + "','" + userData.TerminalID + "')";

                            await _parkDbContext.ExecuteNonQueryAsync(sql, null);
                        }
                        catch (Exception ex)
                        {
                            Logger.Error($"微信支付成功，但保存数据库记录出错：" + ex.Message);
                        }

                        model.Data = payResult;
                        model.Code = ResultCode.Ok;
                        return model;

                    }
                    else
                    {
                        //确定不是微信支付码 则走 方特支付
                        if (input.PayType== PayType.Scan && wxPay.err_code_des.Contains("请扫描微信支付被扫"))
                        {
                           return await FtScanPay(input);
                        }

                        //等待1秒后再查询
                        Thread.Sleep(1000);

                        url = userData.PayLink + "/WeixinQuery";
                        data = "out_trade_no=" + out_trade_no + "&key=" + userData.WebKey + "&random=" + (new Random()).Next(1, 10000).ToString();

                        content = new StringContent(data, Encoding.UTF8, "application/x-www-form-urlencoded");
                        var resultQuery = await http.PostAsync(url, content);
                        json = resultQuery.Content.ReadAsStringAsync().Result;

                        Logger.Info("微信扫码查询：" + json);

                        st = json.IndexOf('{');
                        try
                        {
                            json = json.Substring(st);
                            if (json.Length >= 9)
                            {
                                json = json.Substring(0, json.Length - 9);
                            }
                        }
                        catch (Exception ex)
                        {
                            Logger.Info("微信扫码付解析错误：" + ex.Message);
                        }

                        WxMicroPayDto wxPayQuery = JsonConvert.DeserializeObject<WxMicroPayDto>(json);
                        if (wxPayQuery != null && wxPayQuery.result.ToUpper() == "SUCCESS")
                        {
                            if (input.PayMoney != (Convert.ToDecimal(wxPayQuery.total_fee.ToString()) / 100))
                            {
                                model.Message = "微信支付成功，但支付金额异常，请联系管理员";
                                return model;
                            }


                            PayOutput payResult = new PayOutput();

                            payResult.CouponAmount = 0;
                            payResult.PayAmount = input.PayMoney;
                            payResult.PayMoney = input.PayMoney;
                            payResult.PayOrderNo = out_trade_no;
                            payResult.Rate = 1;
                            payResult.RateAmount = 0;
                            payResult.TradeNo = wxPayQuery.transaction_id;
                            payResult.PayType = PayType.Wx;

                            //插入支付记录
                            try
                            {
                                string sql = "insert into ft_thirdpayment (paymodeid,postradeno,out_trade_no,tradeno,status,inputtime,inputby,paymoney,branch_no,jh) values" +
                                    "('" + (int)PayType.Wx + "','" + orderNo + "','" + out_trade_no + "','" + wxPayQuery.transaction_id + "',1,GETDATE(),'"
                                    + userId.ToString() + "'," + input.PayMoney + ",'" + userData.StorageNo + "','" + userData.TerminalID + "')";

                                await _parkDbContext.ExecuteNonQueryAsync(sql, null);
                            }
                            catch (Exception ex)
                            {
                                Logger.Error($"微信支付成功，但保存数据库记录出错：" + ex.Message);
                            }

                            model.Data = payResult;
                            model.Code = ResultCode.Ok;
                            return model;

                        }
                        else
                        {
                            model.Message = "支付失败:" + wxPay.err_code_des;
                            return model;
                        }
                    }

                }
                else
                {
                    //等待1秒后再查询
                    Thread.Sleep(1000);

                    url = userData.PayLink + "/WeixinQuery";
                    data = "out_trade_no=" + out_trade_no + "&key=" + userData.WebKey + "&random=" + (new Random()).Next(1, 10000).ToString();

                    content = new StringContent(data, Encoding.UTF8, "application/x-www-form-urlencoded");
                    var resultQuery = await http.PostAsync(url, content);
                    json = resultQuery.Content.ReadAsStringAsync().Result;

                    Logger.Info("微信扫码查询：" + json);

                    int st = json.IndexOf('{');
                    try
                    {
                        json = json.Substring(st);
                        if (json.Length >= 9)
                        {
                            json = json.Substring(0, json.Length - 9);
                        }
                    }
                    catch (Exception ex)
                    {
                        Logger.Info("微信扫码付解析错误：" + ex.Message);
                    }

                    WxMicroPayDto wxPayQuery = JsonConvert.DeserializeObject<WxMicroPayDto>(json);
                    if (wxPayQuery != null && wxPayQuery.result.ToUpper() == "SUCCESS")
                    {
                        if (input.PayMoney != (Convert.ToDecimal(wxPayQuery.total_fee.ToString()) / 100))
                        {
                            model.Message = "微信支付成功，但支付金额异常，请联系管理员";
                            return model;
                        }


                        PayOutput payResult = new PayOutput();

                        payResult.CouponAmount = 0;
                        payResult.PayAmount = input.PayMoney;
                        payResult.PayMoney = input.PayMoney;
                        payResult.PayOrderNo = out_trade_no;
                        payResult.Rate = 1;
                        payResult.RateAmount = 0;
                        payResult.TradeNo = wxPayQuery.transaction_id;
                        payResult.PayType = PayType.Wx;

                        //插入支付记录
                        try
                        {
                            string sql = "insert into ft_thirdpayment (paymodeid,postradeno,out_trade_no,tradeno,status,inputtime,inputby,paymoney,branch_no,jh) values" +
                                "('" + (int)PayType.Wx + "','" + orderNo + "','" + out_trade_no + "','" + wxPayQuery.transaction_id + "',1,GETDATE(),'"
                                + userId.ToString() + "'," + input.PayMoney + ",'" + userData.StorageNo + "','" + userData.TerminalID + "')";

                            await _parkDbContext.ExecuteNonQueryAsync(sql, null);
                        }
                        catch (Exception ex)
                        {
                            Logger.Error($"微信支付成功，但保存数据库记录出错：" + ex.Message);
                        }

                        model.Data = payResult;
                        model.Code = ResultCode.Ok;
                        return model;

                    }
                    else
                    {
                        model.Message = "支付失败";
                        return model;
                    }

                }

            }

        }

        #endregion

        #region 支付宝扫码付
        /// <summary>
        /// 支付宝扫码付
        /// </summary>
        /// <param name="input"></param>
        /// <param name="ip"></param>
        /// <returns></returns>
        public async Task<Result<PayOutput>> AliScanPay(SacnPayInput input,string ip)
        {
            Result<PayOutput> model = new Result<PayOutput>(ResultCode.Fail, null);

            if (input.PayMoney <= 0)
            {
                model.Message = "支付失败，" + "支付金额必须大于0";
                return model;
            }
            if (string.IsNullOrWhiteSpace(input.AuthCode))
            {
                model.Message = "支付失败，" + "请扫码支付码";
                return model;
            }

            var orderNo = (await _orderNoAppService.CreateOrderNo()).Data;

            string userId = FishSession.UserId;
            if (string.IsNullOrWhiteSpace(userId))
            {
                model.Message = "未获取到用户信息，请先登录";
                return model;
            }

            var userData = _cacheManager.GetUserDataCacheByUserId(userId);


            var checkOrderInput = new CheckOrderInput()
            {
                OrderNo = orderNo
            };

            //验证库存
            var checkOrderResult = await _orderAppService.CheckOrder(checkOrderInput);
            if (checkOrderResult.Code == ResultCode.Fail || checkOrderResult.Data == null)
            {
                model.Message = checkOrderResult.Message;
                return model;
            }
            if (!checkOrderResult.Data.HasStock)
            {
                model.Message = checkOrderResult.Data.Message;
                return model;
            }

            string url = userData.PayLink + "/AlipayNew";

            string name = "";
            try
            {

                name = userData.StorageName.ToString().Trim();
            }
            catch
            {
                name = userData.StorageName.ToString().Trim();
            }

            string out_trade_no = System.DateTime.Now.ToString("yyyyMMddHHmmss") + userData.StorageNo.ToString() + userData.TerminalID.ToString() + userId; //商户唯一订单号

            string data = "parkid=" + userData.StorageNo.ToString() + "&ip=" + ip + "&terminalid=" + userData.TerminalID.ToString() + "&out_trade_nopar=" + out_trade_no + "&goodsname=" + HttpUtility.UrlEncode(name) + "&goodsdetail=" + HttpUtility.UrlEncode(input.GoodsName) + "&total_amountpar=" + input.PayMoney + "&auth_codepar=" + input.AuthCode + "&key=" + userData.WebKey + "&random=" + (new Random()).Next(1, 10000).ToString() + "&device_info=" + userData.StorageNo + "-" + userData.TerminalID.ToString() + "-" + orderNo;

            Logger.Info("支付宝扫码付：" + data);

            using (HttpClient http = new HttpClient())
            {
                StringContent content = new StringContent(data, Encoding.UTF8, "application/x-www-form-urlencoded");
                var result = await http.PostAsync(url, content);

                string json = result.Content.ReadAsStringAsync().Result;

                Logger.Info("支付宝扫码付：" + json);

                if (json != null)//支付返回结果
                {
                    int st = json.IndexOf('{');
                    try
                    {
                        json = json.Substring(st);
                        if (json.Length >= 9)
                        {
                            json = json.Substring(0, json.Length - 9);
                        }
                    }
                    catch (Exception ex)
                    {
                        Logger.Info("支付宝扫码付解析错误：" + ex.Message);
                    }
                }

                AliPayDto aliPay = JsonConvert.DeserializeObject<AliPayDto>(json);

                if (aliPay.alipay_trade_pay_response != null)//支付返回结果
                {
                    if (aliPay.alipay_trade_pay_response.code.Equals("10000") && string.IsNullOrWhiteSpace(aliPay.alipay_trade_pay_response.sub_code))
                    {
                        if (input.PayMoney != Convert.ToDecimal(aliPay.alipay_trade_pay_response.total_amount))
                        {
                            model.Message = "支付宝支付成功，但支付金额异常，请联系管理员";
                            return model;
                        }

                        PayOutput payResult = new PayOutput();

                        payResult.CouponAmount = 0;
                        payResult.PayAmount = input.PayMoney;
                        payResult.PayMoney = input.PayMoney;
                        payResult.PayOrderNo = out_trade_no;
                        payResult.Rate = 1;
                        payResult.RateAmount = 0;
                        payResult.TradeNo = aliPay.alipay_trade_pay_response.trade_no;
                        payResult.PayType = PayType.Ali;

                        //插入支付记录
                        try
                        {
                            string sql = "insert into ft_thirdpayment (paymodeid,postradeno,out_trade_no,tradeno,status,inputtime,inputby,paymoney,branch_no,jh) values" +
                                "('" + (int)PayType.Ali + "','" + orderNo + "','" + out_trade_no + "','" + aliPay.alipay_trade_pay_response.trade_no + "',1,GETDATE(),'"
                                + userId.ToString() + "'," + input.PayMoney + ",'" + userData.StorageNo + "','" + userData.TerminalID + "')";

                            await _parkDbContext.ExecuteNonQueryAsync(sql, null);
                        }
                        catch (Exception ex)
                        {
                            Logger.Error($"支付宝支付成功，但保存数据库记录出错：" + ex.Message);
                        }

                        model.Data = payResult;
                        model.Code = ResultCode.Ok;
                        return model;
                    }
                    else
                    {
                        //扫码付 确认返回失败 不是支付宝收款码 则走方特支付
                        if (input.PayType == PayType.Scan && aliPay.alipay_trade_pay_response.sub_code.ToUpper().Contains("PAYMENT_AUTH_CODE_INVALID"))
                        {
                            return await FtScanPay(input);           
                        }

                        Thread.Sleep(1000);

                        url = userData.PayLink + "/AliQuery";
                        data = "out_trade_nopar=" + out_trade_no + "&key=" + userData.WebKey + "&random=" + (new Random()).Next(1, 10000).ToString();

                        content = new StringContent(data, Encoding.UTF8, "application/x-www-form-urlencoded");
                        result = await http.PostAsync(url, content);

                        json = result.Content.ReadAsStringAsync().Result;

                        Logger.Info("支付宝扫码付：" + json);

                        if (json != null)//支付返回结果
                        {
                            int st = json.IndexOf('{');
                            try
                            {
                                json = json.Substring(st);
                                if (json.Length >= 9)
                                {
                                    json = json.Substring(0, json.Length - 9);
                                }
                            }
                            catch (Exception ex)
                            {
                                Logger.Info("支付宝扫码付解析错误：" + ex.Message);
                            }
                        }

                        AliPayDto aliPayQuery = JsonConvert.DeserializeObject<AliPayDto>(json);
                        if (aliPayQuery.alipay_trade_query_response != null && aliPayQuery.alipay_trade_query_response.code.Equals("10000") && aliPayQuery.alipay_trade_query_response.trade_status == "TRADE_SUCCESS")
                        {
                            if (input.PayMoney != Convert.ToDecimal(aliPayQuery.alipay_trade_pay_response.total_amount))
                            {
                                model.Message = "支付宝支付成功，但支付金额异常，请联系管理员";
                                return model;
                            }

                            PayOutput payResult = new PayOutput();

                            payResult.CouponAmount = 0;
                            payResult.PayAmount = input.PayMoney;
                            payResult.PayMoney = input.PayMoney;
                            payResult.PayOrderNo = out_trade_no;
                            payResult.Rate = 1;
                            payResult.RateAmount = 0;
                            payResult.TradeNo = aliPayQuery.alipay_trade_query_response.trade_no;
                            payResult.PayType = PayType.Ali;

                            //插入支付记录
                            try
                            {
                                string sql = "insert into ft_thirdpayment (paymodeid,postradeno,out_trade_no,tradeno,status,inputtime,inputby,paymoney,branch_no,jh) values" +
                                    "('" + (int)PayType.Ali + "','" + orderNo + "','" + out_trade_no + "','" + aliPayQuery.alipay_trade_query_response.trade_no + "',1,GETDATE(),'"
                                    + userId.ToString() + "'," + input.PayMoney + ",'" + userData.StorageNo + "','" + userData.TerminalID + "')";

                                await _parkDbContext.ExecuteNonQueryAsync(sql, null);
                            }
                            catch (Exception ex)
                            {
                                Logger.Error($"支付宝支付成功，但保存数据库记录出错：" + ex.Message);
                            }

                            model.Data = payResult;
                            model.Code = ResultCode.Ok;
                            return model;

                        }
                        else
                        {
                            model.Message = "支付失败:" + aliPay.alipay_trade_pay_response.sub_msg;
                            return model;
                        }

                    }

                }
                else
                {
                    Thread.Sleep(1000);

                    url = userData.PayLink + "/AliQuery";
                    data = "out_trade_nopar=" + out_trade_no + "&key=" + userData.WebKey + "&random=" + (new Random()).Next(1, 10000).ToString();

                    content = new StringContent(data, Encoding.UTF8, "application/x-www-form-urlencoded");
                    result = await http.PostAsync(url, content);

                    json = result.Content.ReadAsStringAsync().Result;

                    Logger.Info("支付宝扫码付：" + json);

                    if (json != null)//支付返回结果
                    {
                        int st = json.IndexOf('{');
                        try
                        {
                            json = json.Substring(st);
                            if (json.Length >= 9)
                            {
                                json = json.Substring(0, json.Length - 9);
                            }
                        }
                        catch (Exception ex)
                        {
                            Logger.Info("支付宝扫码付解析错误：" + ex.Message);
                        }
                    }

                    AliPayDto aliPayQuery = JsonConvert.DeserializeObject<AliPayDto>(json);
                    if (aliPayQuery.alipay_trade_query_response != null && aliPayQuery.alipay_trade_query_response.code.Equals("10000") && aliPayQuery.alipay_trade_query_response.trade_status == "TRADE_SUCCESS")
                    {
                        if (input.PayMoney != Convert.ToDecimal(aliPayQuery.alipay_trade_pay_response.total_amount))
                        {
                            model.Message = "支付宝支付成功，但支付金额异常，请联系管理员";
                            return model;
                        }

                        PayOutput payResult = new PayOutput();

                        payResult.CouponAmount = 0;
                        payResult.PayAmount = input.PayMoney;
                        payResult.PayMoney = input.PayMoney;
                        payResult.PayOrderNo = out_trade_no;
                        payResult.Rate = 1;
                        payResult.RateAmount = 0;
                        payResult.TradeNo = aliPayQuery.alipay_trade_query_response.trade_no;
                        payResult.PayType = PayType.Ali;

                        //插入支付记录
                        try
                        {
                            string sql = "insert into ft_thirdpayment (paymodeid,postradeno,out_trade_no,tradeno,status,inputtime,inputby,paymoney,branch_no,jh) values" +
                                "('" + (int)PayType.Ali + "','" + orderNo + "','" + out_trade_no + "','" + aliPayQuery.alipay_trade_query_response.trade_no + "',1,GETDATE(),'"
                                + userId.ToString() + "'," + input.PayMoney + ",'" + userData.StorageNo + "','" + userData.TerminalID + "')";

                            await _parkDbContext.ExecuteNonQueryAsync(sql, null);
                        }
                        catch (Exception ex)
                        {
                            Logger.Error($"支付宝支付成功，但保存数据库记录出错：" + ex.Message);
                        }

                        model.Data = payResult;
                        model.Code = ResultCode.Ok;
                        return model;

                    }
                    else
                    {
                        model.Message = "支付失败:" + aliPayQuery.alipay_trade_query_response.sub_msg;
                        return model;
                    }
                }
            }

        }

        #endregion

        /// <summary>
        /// 方特扫码付
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<Result<PayOutput>> FtScanPay(SacnPayInput input)
        {
            Result<PayOutput> model = new Result<PayOutput>(ResultCode.Fail, null);

            if (input.PayMoney <= 0)
            {
                model.Message = "支付失败，" + "支付金额必须大于0";
                return model;
            }
            if (string.IsNullOrWhiteSpace(input.AuthCode))
            {
                model.Message = "支付失败，" + "请扫码支付码";
                return model;
            }

            var orderNo = (await _orderNoAppService.CreateOrderNo()).Data;

            string userId = FishSession.UserId;
            if (string.IsNullOrWhiteSpace(userId))
            {
                model.Message = "未获取到用户信息，请先登录";
                return model;
            }

            var userData = _cacheManager.GetUserDataCacheByUserId(userId);


            var checkOrderInput = new CheckOrderInput()
            {
                OrderNo = orderNo
            };

            //验证库存
            var checkOrderResult = await _orderAppService.CheckOrder(checkOrderInput);
            if (checkOrderResult.Code == ResultCode.Fail || checkOrderResult.Data == null)
            {
                model.Message = checkOrderResult.Message;
                return model;
            }
            if (!checkOrderResult.Data.HasStock)
            {
                model.Message = checkOrderResult.Data.Message;
                return model;
            }

            string out_trade_no = "CY" + System.DateTime.Now.ToString("yyyyMMddHHmmss") + userData.StorageNo.ToString() + userData.TerminalID.ToString() + userId; //商户唯一订单号

            var url = userData.LeyoupayLink + "CSCanPayResult";

            string timespan = GetTimeStamp();

            //参数加密必须按字母排序 
            string data = "amount=" + input.PayMoney;
            data += "&clienttype=1";
            data += "&des=" + input.GoodsName;
            data += "&key=" + userData.LeyouKey;
            data += "&merchant=" + userData.StorageNo + "-" + userData.StorageName;
            data += "&orderno=" + out_trade_no;
            data += "&parkid=" + userData.MutonParkid.ToString();

            data += "&posTicketNumber=" + System.DateTime.Now.ToString("yyyyMMdd") + userData.StorageNo + userData.TerminalID + orderNo;

            data += "&timestamp=" + timespan;
            data += "&token=" + input.AuthCode;
            data = "&signature=" + SHAEncrypt(data);

            string data2 = "amount=" + input.PayMoney;
            data2 += "&clienttype=1";
            data2 += "&des=" + System.Web.HttpUtility.UrlEncode(input.GoodsName);
            data2 += "&merchant=" + userData.StorageNo + "-" + System.Web.HttpUtility.UrlEncode(userData.StorageName);
            data2 += "&orderno=" + out_trade_no;
            data2 += "&parkid=" + userData.MutonParkid.ToString();

            data2 += "&posTicketNumber=" + System.DateTime.Now.ToString("yyyyMMdd") + userData.StorageNo + userData.TerminalID + orderNo;

            data2 += "&timestamp=" + timespan;
            data2 += "&token=" + input.AuthCode;
            data2 = data2 + data;

            Logger.Info("方特扫码付：" + data2);

            using (HttpClient http = new HttpClient())
            {
                StringContent content = new StringContent(data2, Encoding.UTF8, "application/x-www-form-urlencoded");
                var result = await http.PostAsync(url, content);

                string json = result.Content.ReadAsStringAsync().Result;

                Logger.Info("方特扫码付：" + json);

                if (json != null)//支付返回结果
                {

                    FtPayDto ftPay = JsonConvert.DeserializeObject<FtPayDto>(json);


                    if (ftPay.result.ToUpper() == "0")
                    {
                        if (input.PayMoney != (Convert.ToDecimal(ftPay.data.amount.ToString())))
                        {
                            model.Message = "方特支付成功，但支付金额异常，请联系管理员";
                            return model;
                        }


                        PayOutput payResult = new PayOutput();

                        payResult.CouponAmount = ftPay.data.couponamount;
                        payResult.PayAmount = ftPay.data.payamount;
                        payResult.PayMoney = ftPay.data.amount;
                        payResult.PayOrderNo = out_trade_no;
                        payResult.Rate = ftPay.data.discount == null ? 1 : Convert.ToDecimal(ftPay.data.discount);
                        payResult.RateAmount = ftPay.data.amount - ftPay.data.payamount;
                        payResult.TradeNo = ftPay.data.mtorderno;
                        payResult.PayType = PayType.Ft;

                        //插入支付记录
                        try
                        {
                            string sql = "insert into ft_thirdpayment (paymodeid,postradeno,out_trade_no,tradeno,status,inputtime,inputby,paymoney,branch_no,jh) values" +
                                "('" + (int)PayType.Ft + "','" + orderNo + "','" + out_trade_no + "','" + ftPay.data.mtorderno + "',1,GETDATE(),'"
                                + userId.ToString() + "'," + input.PayMoney + ",'" + userData.StorageNo + "','" + userData.TerminalID + "')";

                            await _parkDbContext.ExecuteNonQueryAsync(sql, null);
                        }
                        catch (Exception ex)
                        {
                            Logger.Error($"方特支付成功，但保存数据库记录出错：" + ex.Message);
                        }

                        model.Data = payResult;
                        model.Code = ResultCode.Ok;
                        return model;

                    }
                    else
                    {
                        model.Message = "支付失败，" + ftPay.message;
                        return model;

                    }

                }
                else
                {
                    model.Message = "支付失败,请稍后重试！";
                    return model;
                }

            }

        }

        /// <summary>
        /// 秒通支付查询
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<Result<PayOutput>> MutonePayQuery(PayQueryInput input)
        {
            Result<PayOutput> model = new Result<PayOutput>(ResultCode.Fail, null);

            if (input.PayMoney <= 0)
            {
                model.Message = "支付查询失败，" + "支付金额必须大于0";
                return model;
            }

            string userId = FishSession.UserId;
            if (string.IsNullOrWhiteSpace(userId))
            {
                model.Message = "未获取到用户信息，请先登录";
                return model;
            }

            var userData = _cacheManager.GetUserDataCacheByUserId(userId);

            var orderNo = (await _orderNoAppService.CreateOrderNo()).Data;


            string url = userData.MutonpayLink + "CGetComsuseDetial?";
            string data = "merchant=" + userData.StorageName + "&mtorderno=" + input.TradeNo + "&type=2&clienttype=1&parkid=" + userData.MutonParkid.ToString() + "&timestamp=" + GetTimeStamp();

            data = data + "&signature=" + SHAEncrypt(data + "&key=" + userData.MutoneKey);

            Logger.Info("秒通支付查询：" + data);

            using (HttpClient http = new HttpClient())
            {
                var result = await http.GetAsync(url + data);
                string json = result.Content.ReadAsStringAsync().Result;

                Logger.Info("获取秒通四位码结果：" + json);

                if (json != null)//返回结果
                {
                    json = System.Text.RegularExpressions.Regex.Replace(json, "-", "");
                    MutonPayQueryDto mutoneQueryPay = JsonConvert.DeserializeObject<MutonPayQueryDto>(json);

                    if (mutoneQueryPay != null && mutoneQueryPay.result.ToUpper() == "0")
                    {
                        //秒通接口异常 ，暂时 允许差异2分钱  后期不允许
                        decimal roundAmount = System.Math.Abs((input.PayMoney - Convert.ToDecimal(mutoneQueryPay.data[0].amount.ToString())));
                        if (roundAmount > Convert.ToDecimal(0.02))
                        {
                            model.Message = "秒通已支付，但支付金额不一致，请确认选择的商品是否一致";
                            return model;
                        }

                        //if (input.PayMoney != (Convert.ToDecimal(mutoneQueryPay.data[0].amount.ToString())))
                        //{
                        //    model.Message = "秒通已支付，但支付金额不一致，请确认选择的商品是否一致";
                        //    return model;
                        //}

                        PayOutput payResult = new PayOutput();

                        payResult.CouponAmount = 0;
                        payResult.PayAmount = mutoneQueryPay.data[0].payamount;
                        payResult.PayMoney = mutoneQueryPay.data[0].amount;
                        payResult.PayOrderNo = mutoneQueryPay.data[0].orderno;
                        payResult.Rate = mutoneQueryPay.data[0].discount == null ? 1 : mutoneQueryPay.data[0].discount.Value;
                        payResult.RateAmount = mutoneQueryPay.data[0].amount - mutoneQueryPay.data[0].payamount;
                        payResult.TradeNo = mutoneQueryPay.data[0].mtorderno;
                        payResult.PayType = PayType.Mutone;

                        string checkOutTradeNo = "CY" + System.DateTime.Now.ToString("yyyyMMddHHmmss") + userData.StorageNo.ToString() + userData.TerminalID.ToString() + (new Random()).Next(1, 10000).ToString(); //商户唯一订单号 
                        if (checkOutTradeNo.Substring(0, 10) != payResult.PayOrderNo.Substring(0, 10) || checkOutTradeNo.Substring(16, 7) != payResult.PayOrderNo.Substring(16, 7))
                        {
                            model.Message = "此订单号 不是此POS机订单，请勿使用！！";
                            return model;
                        }

                        var sqlQuery = "select 1 from Payment where CardID = '" + payResult.PayOrderNo + "'";
                        DataSet payDs = _context.GetDataSet(sqlQuery, new object());
                        if (payDs != null && payDs.Tables.Count > 0 && payDs.Tables[0].Rows.Count > 0)
                        {
                            model.Message = "此订单已在餐饮系统使用，请勿重复使用！！";
                            return model;
                        }

                        //插入支付记录
                        try
                        {
                            string sql = "if not  exists (select 1 from ft_thirdpayment where out_trade_no='" + payResult.PayOrderNo + "')  begin  " +
                                "insert into ft_thirdpayment (paymodeid,postradeno,out_trade_no,tradeno,status,inputtime,inputby,paymoney,branch_no,jh) values" +
                                "('" + (int)PayType.Mutone + "','" + orderNo + "','" + payResult.PayOrderNo + "','" + payResult.TradeNo + "',1,GETDATE(),'"
                                + userId.ToString() + "'," + input.PayMoney + ",'" + userData.StorageNo + "','" + userData.TerminalID + "')  end";

                            await _parkDbContext.ExecuteNonQueryAsync(sql, null);
                        }
                        catch (Exception ex)
                        {
                            Logger.Error($"秒通支付成功，但保存数据库记录出错：" + ex.Message);
                        }


                        model.Data = payResult;
                        model.Code = ResultCode.Ok;
                        return model;
                    }
                    else
                    {
                        model.Message = "请确认第三方支付订单号是否正确！" + mutoneQueryPay.message;
                        return model;
                    }
                }
                else
                {
                    model.Message = "请确认第三方支付订单号是否正确！";
                    return model;
                }
            }
        }

        /// <summary>
        /// 方特支付查询
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<Result<PayOutput>> FtScanPayQuery(PayQueryInput input)
        {
            Result<PayOutput> model = new Result<PayOutput>(ResultCode.Fail, null);

            if (input.PayMoney <= 0)
            {
                model.Message = "支付查询失败，" + "支付金额必须大于0";
                return model;
            }
            var orderNo = (await _orderNoAppService.CreateOrderNo()).Data;

            string userId = FishSession.UserId;
            if (string.IsNullOrWhiteSpace(userId))
            {
                model.Message = "未获取到用户信息，请先登录";
                return model;
            }

            var userData = _cacheManager.GetUserDataCacheByUserId(userId);


            string url = userData.LeyoupayLink + "CGetComsuseDetail?";
            string timespan = GetTimeStamp();

            //参数加密必须按字母排序 
            string data = "";
            data += "clienttype=1";
            data += "&key=" + userData.LeyouKey;
            data += "&merchant=" + userData.StorageName;
            data += "&mtorderno=" + input.TradeNo;
            data += "&orderno=";
            data += "&parkid=" + userData.MutonParkid.ToString();
            data += "&refundno=" + "";
            data += "&timestamp=" + timespan;
            data += "&type=2";
            data = "&signature=" + SHAEncrypt(data);
            string data2 = "";
            data2 += "clienttype=1";
            data2 += "&merchant=" + System.Web.HttpUtility.UrlEncode(userData.StorageName);
            data2 += "&mtorderno=" + input.TradeNo;
            data2 += "&orderno=" ;
            data2 += "&parkid=" + userData.MutonParkid.ToString();
            data2 += "&refundno=" + "";
            data2 += "&timestamp=" + timespan;
            data2 += "&type=2";
            data2 = data2 + data;

            Logger.Info("方特支付查询：" + data2);

            using (HttpClient http = new HttpClient())
            {
                var result = await http.GetAsync(url + data2);
                string json = result.Content.ReadAsStringAsync().Result;

                Logger.Info("方特支付查询：" + json);

                if (json != null)//返回结果
                {
                    json = System.Text.RegularExpressions.Regex.Replace(json, "-", "");
                    MutonPayQueryDto mutoneQueryPay = JsonConvert.DeserializeObject<MutonPayQueryDto>(json);

                    if (mutoneQueryPay != null && mutoneQueryPay.result.ToUpper() == "0")
                    {


                        if (input.PayMoney != (Convert.ToDecimal(mutoneQueryPay.data[0].amount.ToString())))
                        {
                            model.Message = "方特已支付，但支付金额不一致，请确认选择的商品是否一致";
                            return model;
                        }

                        PayOutput payResult = new PayOutput();

                        payResult.CouponAmount = mutoneQueryPay.data[0].couponamount;
                        payResult.PayAmount = mutoneQueryPay.data[0].payamount;
                        payResult.PayMoney = mutoneQueryPay.data[0].amount;
                        payResult.PayOrderNo = mutoneQueryPay.data[0].orderno;
                        payResult.Rate = mutoneQueryPay.data[0].discount == null ? 1 : mutoneQueryPay.data[0].discount.Value;
                        payResult.RateAmount = mutoneQueryPay.data[0].amount - mutoneQueryPay.data[0].payamount;
                        payResult.TradeNo = mutoneQueryPay.data[0].mtorderno;
                        payResult.PayType = PayType.Ft;

                        string checkOutTradeNo = "CY" + System.DateTime.Now.ToString("yyyyMMddHHmmss") + userData.StorageNo.ToString() + userData.TerminalID.ToString() + (new Random()).Next(1, 10000).ToString(); //商户唯一订单号 
                        if (checkOutTradeNo.Substring(0, 10) != payResult.PayOrderNo.Substring(0, 10) || checkOutTradeNo.Substring(16, 7) != payResult.PayOrderNo.Substring(16, 7))
                        {
                            model.Message = "此订单号 不是此POS机订单，请勿使用！！";
                            return model;
                        }

                        var sqlQuery = "select 1 from Payment where CardID = '" + payResult.PayOrderNo + "'";
                        DataSet payDs = _context.GetDataSet(sqlQuery, new object());
                        if (payDs != null && payDs.Tables.Count > 0 && payDs.Tables[0].Rows.Count > 0)
                        {
                            model.Message = "此订单已在餐饮系统使用，请勿重复使用！！";
                            return model;
                        }

                        //插入支付记录
                        try
                        {
                            string sql = "if not  exists (select 1 from ft_thirdpayment where out_trade_no='" + payResult.PayOrderNo + "')  begin  " +
                                "insert into ft_thirdpayment (paymodeid,postradeno,out_trade_no,tradeno,status,inputtime,inputby,paymoney,branch_no,jh) values" +
                                "('" + (int)PayType.Ft + "','" + orderNo + "','" + payResult.PayOrderNo + "','" + payResult.TradeNo + "',1,GETDATE(),'"
                                + userId.ToString() + "'," + input.PayMoney + ",'" + userData.StorageNo + "','" + userData.TerminalID + "') end";

                            await _parkDbContext.ExecuteNonQueryAsync(sql, null);
                        }
                        catch (Exception ex)
                        {
                            Logger.Error($"方特支付成功，但保存数据库记录出错：" + ex.Message);
                        }


                        model.Data = payResult;
                        model.Code = ResultCode.Ok;
                        return model;
                    }
                    else
                    {
                        model.Message = "请确认第三方支付订单号是否正确！" + mutoneQueryPay.message;
                        return model;
                    }
                }
                else
                {
                    model.Message = "请确认第三方支付订单号是否正确！";
                    return model;
                }
            }
        }

        /// <summary>
        /// 微信扫码支付查询
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<Result<PayOutput>> WxScanPayQuery(PayQueryInput input)
        {
            Result<PayOutput> model = new Result<PayOutput>(ResultCode.Fail, null);

            if (input.PayMoney <= 0)
            {
                model.Message = "支付查询失败，" + "支付金额必须大于0";
                return model;
            }

            var orderNo = (await _orderNoAppService.CreateOrderNo()).Data;

            string userId = FishSession.UserId;
            if (string.IsNullOrWhiteSpace(userId))
            {
                model.Message = "未获取到用户信息，请先登录";
                return model;
            }

            var userData = _cacheManager.GetUserDataCacheByUserId(userId);


            string outTradeNo = System.DateTime.Now.ToString("yyyyMMddHHmmss") + userData.StorageNo.ToString() + userData.TerminalID.ToString() + (new Random()).Next(1, 10000).ToString(); //商户唯一订单号 
            if (input.TradeNo.Trim().Substring(0, 8) != outTradeNo.Substring(0, 8) || input.TradeNo.Trim().Substring(14, 7) != outTradeNo.Substring(14, 7))
            {
                model.Message = "此订单号 不是此POS机订单，请勿使用！";
                return model;

            }

            string url = userData.PayLink + "/WeixinQuery";
            string data = "out_trade_no=" + input.TradeNo + "&key=" + userData.WebKey + "&random=" + (new Random()).Next(1, 10000).ToString();

            Logger.Info("微信扫码支付查询：" + data);

            using (HttpClient http = new HttpClient())
            {
                StringContent content = new StringContent(data, Encoding.UTF8, "application/x-www-form-urlencoded");
                var result = await http.PostAsync(url, content);
                
                string json = result.Content.ReadAsStringAsync().Result;

                Logger.Info("微信扫码支付查询：" + json);

                if (json != null)//支付返回结果
                {
                    int st = json.IndexOf('{');
                    try
                    {
                        json = json.Substring(st);
                        if (json.Length >= 9)
                        {
                            json = json.Substring(0, json.Length - 9);
                        }
                    }
                    catch (Exception ex)
                    {
                        Logger.Info("微信扫码支付查询解析错误：" + ex.Message);
                    }

                    WxMicroPayDto wxQueryPay = JsonConvert.DeserializeObject<WxMicroPayDto>(json);
                    if (wxQueryPay != null && wxQueryPay.result.ToUpper() == "SUCCESS")
                    {
                        if (input.PayMoney != (Convert.ToDecimal(wxQueryPay.total_fee.ToString()) / 100))
                        {
                            model.Message = "微信已支付，但支付金额不一致，请确认选择的商品是否一致";
                            return model;
                        }

                        PayOutput payResult = new PayOutput();

                        payResult.CouponAmount = 0;
                        payResult.PayAmount = Convert.ToDecimal(wxQueryPay.total_fee) / 100;
                        payResult.PayMoney = Convert.ToDecimal(wxQueryPay.total_fee) / 100;
                        payResult.PayOrderNo = wxQueryPay.out_trade_no;
                        payResult.Rate = 1;
                        payResult.RateAmount = 0;
                        payResult.TradeNo = wxQueryPay.transaction_id;
                        payResult.PayType = PayType.Wx;

                        var sqlQuery = "select 1 from Payment where CardID = '" + payResult.PayOrderNo + "'";
                        DataSet payDs = _context.GetDataSet(sqlQuery, new object());
                        if (payDs != null && payDs.Tables.Count > 0 && payDs.Tables[0].Rows.Count > 0)
                        {
                            model.Message = "此订单已在餐饮系统使用，请勿重复使用！！";
                            return model;
                        }

                        //插入支付记录
                        try
                        {
                            string sql = "if not  exists (select 1 from ft_thirdpayment where out_trade_no='" + payResult.PayOrderNo + "')  begin  " +
                                "insert into ft_thirdpayment (paymodeid,postradeno,out_trade_no,tradeno,status,inputtime,inputby,paymoney,branch_no,jh) values" +
                                "('" + (int)PayType.Wx + "','" + orderNo + "','" + payResult.PayOrderNo + "','" + payResult.TradeNo + "',1,GETDATE(),'"
                                + userId.ToString() + "'," + input.PayMoney + ",'" + userData.StorageNo + "','" + userData.TerminalID + "')  end";

                            await _parkDbContext.ExecuteNonQueryAsync(sql, null);
                        }
                        catch (Exception ex)
                        {
                            Logger.Error($"微信支付成功，但保存数据库记录出错：" + ex.Message);
                        }


                        model.Data = payResult;
                        model.Code = ResultCode.Ok;
                        return model;

                    }
                    else
                    {
                        model.Message = "查询失败，请确认订单号是否正确！" + wxQueryPay.err_code_des;
                        return model;
                    }
                }
                else
                {
                    model.Message = "查询失败，请确认订单号是否正确！";
                    return model;
                }
            }

        }

        /// <summary>
        /// 支付宝扫码付查询
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<Result<PayOutput>> AliScanPayQuery(PayQueryInput input)
        {
            Result<PayOutput> model = new Result<PayOutput>(ResultCode.Fail, null);

            if (input.PayMoney <= 0)
            {
                model.Message = "支付查询失败，" + "支付金额必须大于0";
                return model;
            }
            var orderNo = (await _orderNoAppService.CreateOrderNo()).Data;

            string userId = FishSession.UserId;
            if (string.IsNullOrWhiteSpace(userId))
            {
                model.Message = "未获取到用户信息，请先登录";
                return model;
            }

            var userData = _cacheManager.GetUserDataCacheByUserId(userId);


            string url = userData.PayLink + "/AliQuery";
            string data = "out_trade_nopar=" + input.TradeNo + "&key=" + userData.WebKey + "&random=" + (new Random()).Next(1, 10000).ToString();

            Logger.Info("支付宝支付查询：" + data);

            using (HttpClient http = new HttpClient())
            {
                StringContent content = new StringContent(data, Encoding.UTF8, "application/x-www-form-urlencoded");
                var result = await http.PostAsync(url, content);

                string json = result.Content.ReadAsStringAsync().Result;

                Logger.Info("支付宝支付查询：" + json);

                if (json != null)//支付返回结果
                {
                    int st = json.IndexOf('{');
                    try
                    {
                        json = json.Substring(st);
                        if (json.Length >= 9)
                        {
                            json = json.Substring(0, json.Length - 9);
                        }
                    }
                    catch (Exception ex)
                    {
                        Logger.Info("支付宝支付查询解析错误：" + ex.Message);
                    }

                    AliPayDto aliQueryPay = JsonConvert.DeserializeObject<AliPayDto>(json);
                    if (aliQueryPay.alipay_trade_query_response != null && aliQueryPay.alipay_trade_query_response.code.Equals("10000") && aliQueryPay.alipay_trade_query_response.trade_status == "TRADE_SUCCESS")
                    {
                        if (input.PayMoney != (Convert.ToDecimal(aliQueryPay.alipay_trade_query_response.total_amount)))
                        {
                            model.Message = "支付宝已支付，但支付金额不一致，请确认选择的商品是否一致";
                            return model;
                        }

                        PayOutput payResult = new PayOutput();

                        payResult.CouponAmount = 0;
                        payResult.PayAmount = Convert.ToDecimal(aliQueryPay.alipay_trade_query_response.total_amount);
                        payResult.PayMoney = Convert.ToDecimal(aliQueryPay.alipay_trade_query_response.total_amount);
                        payResult.PayOrderNo = aliQueryPay.alipay_trade_query_response.out_trade_no;
                        payResult.Rate = 1;
                        payResult.RateAmount = 0;
                        payResult.TradeNo = aliQueryPay.alipay_trade_query_response.trade_no;
                        payResult.PayType = PayType.Ali;

                        var sqlQuery = "select 1 from Payment where CardID = '" + payResult.PayOrderNo + "'";
                        DataSet payDs = _context.GetDataSet(sqlQuery, new object());
                        if (payDs != null && payDs.Tables.Count > 0 && payDs.Tables[0].Rows.Count > 0)
                        {
                            model.Message = "此订单已在餐饮系统使用，请勿重复使用！！";
                            return model;
                        }

                        //插入支付记录
                        try
                        {
                            string sql = "if not  exists (select 1 from ft_thirdpayment where out_trade_no='" +payResult.PayOrderNo+"')  begin " +
                                " insert into ft_thirdpayment (paymodeid,postradeno,out_trade_no,tradeno,status,inputtime,inputby,paymoney,branch_no,jh) values" +
                                "('" + (int)PayType.Ali + "','" + orderNo + "','" + payResult.PayOrderNo + "','" + payResult.TradeNo + "',1,GETDATE(),'"
                                + userId.ToString() + "'," + input.PayMoney + ",'" + userData.StorageNo + "','" + userData.TerminalID + "')  end";

                            await _parkDbContext.ExecuteNonQueryAsync(sql, null);
                        }
                        catch (Exception ex)
                        {
                            Logger.Error($"支付宝支付成功，但保存数据库记录出错：" + ex.Message);
                        }


                        model.Data = payResult;
                        model.Code = ResultCode.Ok;
                        return model;

                    }
                    else
                    {
                        model.Message = "查询失败，请确认订单号是否正确！" + aliQueryPay.alipay_trade_query_response.sub_msg;
                        return model;
                    }
                }
                else
                {
                    model.Message = "查询失败，请确认订单号是否正确！";
                    return model;
                }
            }
        }

        /// <summary>
        /// 第三方支付退款接口
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<Result<PayReturnOutput>> PayReturn(PayReturnInput input)
        {

            Result<PayReturnOutput> model = new Result<PayReturnOutput>(ResultCode.Fail, null);

            string userId = FishSession.UserId;
            if (string.IsNullOrWhiteSpace(userId))
            {
                model.Message = "未获取到用户信息，请先登录";
                return model;
            }

            var userData = _cacheManager.GetUserDataCacheByUserId(userId);


            var orderNo = (await _orderNoAppService.CreateOrderNo()).Data;

            //查询原始交易支付信息

            string sqlQuery = string.Format("select a.sPayTypeID as pay_way,a.nItem as sell_way,a.nPaytAmount as old_amount,a.nPaytAmount as sale_amount," +
                "a.sPaytNO as thirdtradeno ,b.tradeno  from tFishPayt a left join ft_thirdpayment b on a.sPaytNO=b.out_trade_no and a.sStoreNO=b.branch_no " +
                "and a.sFishNO=b.jh where a.sStoreNO = '{0}' and a.sFishNO = '{1}' and a.nSerID = '{2}' and a.dTradeDate>=Convert(varchar(10),getdate(),120) " +
                "and a.dTradeDate<Convert(varchar(10),getdate()+1,120)", userData.StorageNo, userData.TerminalID, input.OriginalOrderNo);

            DataSet dsPayInfo = _parkDbContext.GetDataSet(sqlQuery, new object());

            if (dsPayInfo == null || dsPayInfo.Tables.Count == 0 || dsPayInfo.Tables[0].Rows.Count == 0)
            {
                model.Message = "退款失败，" + "未查询到原付款信息";
                return model;
            }

            //原付款金额
            decimal payMoney = 0;
            //原商户交易单号
            var payOrderNo = "";
            //原第三方交易单号
            var payTradeNo = "";

            #region 微信退款
            ///微信退款
            if (input.PayType == PayType.Wx)
            {
                for (int i = 0; i < dsPayInfo.Tables[0].Rows.Count; i++)
                {
                    if (Convert.ToInt32(dsPayInfo.Tables[0].Rows[i]["pay_way"].ToString()) == (int)PayType.Wx)
                    {
                        payMoney += (decimal)dsPayInfo.Tables[0].Rows[i]["old_amount"];
                        payOrderNo = dsPayInfo.Tables[0].Rows[i]["thirdtradeno"].ToString();
                    }
                }


                if (payMoney <= 0)
                {

                    model.Message = "退款失败，" + "此单未使用微信支付";
                    return model;
                }

                if (payMoney < input.ReturnAmount)
                {
                    model.Message = "退款失败，" + "退款金额不能大于支付金额";
                    return model;
                }


                string outRefundNo = System.DateTime.Now.ToString("yyyyMMddHHmmss") + userData.StorageNo.ToString() + userData.TerminalID.ToString() + userId; //商户唯一订单号 

                ///注意：微信金额以分为单位
                int returnPayMoney = Convert.ToInt32(input.ReturnAmount * 100);

                string url = userData.PayLink + "/WeixinReFund";
                string data = "transaction_id=" + "" + "&out_trade_no=" + payOrderNo + "&total_fee=" + returnPayMoney.ToString() + "&refund_fee=" + returnPayMoney.ToString() + "&out_refund_no=" + outRefundNo + "&key=" + userData.WebKey + "&random=" + (new Random()).Next(1, 10000).ToString();

                Logger.Info("微信退款：" + data);

                using (HttpClient http = new HttpClient())
                {
                    StringContent content = new StringContent(data, Encoding.UTF8, "application/x-www-form-urlencoded");
                    var result = await http.PostAsync(url, content);

                    string json = result.Content.ReadAsStringAsync().Result;

                    Logger.Info("微信退款：" + json);

                    if (json != null)//支付返回结果
                    {
                        int st = json.IndexOf('{');
                        try
                        {
                            json = json.Substring(st);
                            if (json.Length >= 9)
                            {
                                json = json.Substring(0, json.Length - 9);
                            }
                        }
                        catch (Exception ex)
                        {
                            Logger.Info("微信退款解析错误：" + ex.Message);
                        }

                        WxMicroPayDto wxReturnPay = JsonConvert.DeserializeObject<WxMicroPayDto>(json);

                        if (wxReturnPay != null && wxReturnPay.result.ToUpper() == "SUCCESS")//支付返回结果
                        {
                            //插入支付记录
                            try
                            {
                                string sql = "insert into ft_thirdpayment (paymodeid,postradeno,out_trade_no,tradeno,status,inputtime,inputby,paymoney,branch_no,jh) values" +
                                    "('" + (int)PayType.Wx + "','" + orderNo + "','" + outRefundNo + "','" + wxReturnPay.refund_id + "',2,GETDATE(),'"
                                    + userId.ToString() + "'," + input.ReturnAmount + ",'" + userData.StorageNo + "','" + userData.TerminalID + "')";

                                await _parkDbContext.ExecuteNonQueryAsync(sql, null);
                            }
                            catch (Exception ex)
                            {
                                Logger.Error($"微信退款成功，但保存数据库记录出错：" + ex.Message);
                            }

                            PayReturnOutput payReturnOutput = new PayReturnOutput();
                            payReturnOutput.RefundFee = Convert.ToDecimal(wxReturnPay.refund_fee) / 100;
                            payReturnOutput.ReturnTradeNo = wxReturnPay.refund_id;
                            payReturnOutput.PayReturnOrderNo = outRefundNo;
                            payReturnOutput.PayOrderNo = payOrderNo;

                            model.Data = payReturnOutput;
                            model.Code = ResultCode.Ok;
                            return model;
                        }
                        else
                        {
                            Thread.Sleep(1000);

                            ///退款查询

                            url = userData.PayLink + "/WeixinRefundQuery";
                            data = "refund_id=" + "" + "&out_refund_no=" + "" + "&transaction_id=" + "" + "&out_trade_no=" + payOrderNo + "&key=" + userData.WebKey + "&random=" + (new Random()).Next(1, 10000).ToString();

                            Logger.Info("微信退款查询：" + data);

                            content = new StringContent(data, Encoding.UTF8, "application/x-www-form-urlencoded");
                            result = await http.PostAsync(url, content);

                            json = result.Content.ReadAsStringAsync().Result;

                            Logger.Info("微信退款查询：" + json);

                            if (json != null)//支付返回结果
                            {
                                st = json.IndexOf('{');
                                try
                                {
                                    json = json.Substring(st);
                                    if (json.Length >= 9)
                                    {
                                        json = json.Substring(0, json.Length - 9);
                                    }
                                }
                                catch (Exception ex)
                                {
                                    Logger.Info("微信退款解析错误：" + ex.Message);
                                }

                                WxMicroPayDto wxReturnPayQuery = JsonConvert.DeserializeObject<WxMicroPayDto>(json);
                                if (wxReturnPayQuery.result.ToUpper() == "SUCCESS")
                                {

                                    //插入支付记录
                                    try
                                    {
                                        string sql = "insert into ft_thirdpayment (paymodeid,postradeno,out_trade_no,tradeno,status,inputtime,inputby,paymoney,branch_no,jh) values" +
                                            "('" + (int)PayType.Wx + "','" + orderNo + "','" + wxReturnPayQuery.out_refund_no + "','" + wxReturnPayQuery.refund_id + "',2,GETDATE(),'"
                                            + userId.ToString() + "'," + input.ReturnAmount + ",'" + userData.StorageNo + "','" + userData.TerminalID + "')";

                                        await _parkDbContext.ExecuteNonQueryAsync(sql, null);
                                    }
                                    catch (Exception ex)
                                    {
                                        Logger.Error($"微信退款成功，但保存数据库记录出错：" + ex.Message);
                                    }

                                    PayReturnOutput payReturnOutput = new PayReturnOutput();
                                    payReturnOutput.RefundFee = Convert.ToDecimal(wxReturnPayQuery.refund_fee) / 100;
                                    payReturnOutput.ReturnTradeNo = wxReturnPayQuery.refund_id;
                                    payReturnOutput.PayReturnOrderNo = wxReturnPayQuery.out_refund_no;
                                    payReturnOutput.PayOrderNo = payOrderNo;

                                    model.Data = payReturnOutput;
                                    model.Code = ResultCode.Ok;
                                    return model;
                                }
                                else
                                {
                                    model.Message = "微信退款失败！" + wxReturnPay.err_code_des;
                                    return model;
                                }
                            }
                            else
                            {
                                model.Message = "微信退款失败！";
                                return model;
                            }

                        }
                    }
                    else
                    {
                        Thread.Sleep(1000);

                        ///退款查询

                        url = userData.PayLink + "/WeixinRefundQuery";
                        data = "refund_id=" + "" + "&out_refund_no=" + "" + "&transaction_id=" + "" + "&out_trade_no=" + payOrderNo + "&key=" + userData.WebKey + "&random=" + (new Random()).Next(1, 10000).ToString();

                        Logger.Info("微信退款查询：" + data);
                        content = new StringContent(data, Encoding.UTF8, "application/x-www-form-urlencoded");
                        result = await http.PostAsync(url, content);

                        json = result.Content.ReadAsStringAsync().Result;

                        Logger.Info("微信退款查询：" + json);

                        if (json != null)//支付返回结果
                        {
                            int st = json.IndexOf('{');

                            try
                            {
                                json = json.Substring(st);
                                if (json.Length >= 9)
                                {
                                    json = json.Substring(0, json.Length - 9);
                                }
                            }
                            catch (Exception ex)
                            {
                                Logger.Info("微信退款解析错误：" + ex.Message);
                            }

                            WxMicroPayDto wxReturnPayQuery = JsonConvert.DeserializeObject<WxMicroPayDto>(json);
                            if (wxReturnPayQuery.result.ToUpper() == "SUCCESS")
                            {

                                //插入支付记录
                                try
                                {
                                    string sql = "insert into ft_thirdpayment (paymodeid,postradeno,out_trade_no,tradeno,status,inputtime,inputby,paymoney,branch_no,jh) values" +
                                        "('" + (int)PayType.Wx + "','" + orderNo + "','" + wxReturnPayQuery.out_refund_no + "','" + wxReturnPayQuery.refund_id + "',2,GETDATE(),'"
                                        + userId.ToString() + "'," + input.ReturnAmount + ",'" + userData.StorageNo + "','" + userData.TerminalID + "')";

                                    await _parkDbContext.ExecuteNonQueryAsync(sql, null);
                                }
                                catch (Exception ex)
                                {
                                    Logger.Error($"微信退款成功，但保存数据库记录出错：" + ex.Message);
                                }

                                PayReturnOutput payReturnOutput = new PayReturnOutput();
                                payReturnOutput.RefundFee = Convert.ToDecimal(wxReturnPayQuery.refund_fee) / 100;
                                payReturnOutput.ReturnTradeNo = wxReturnPayQuery.refund_id;
                                payReturnOutput.PayReturnOrderNo = wxReturnPayQuery.out_refund_no;
                                payReturnOutput.PayOrderNo = payOrderNo;

                                model.Data = payReturnOutput;
                                model.Code = ResultCode.Ok;
                                return model;
                            }
                            else
                            {
                                model.Message = "微信退款失败！" + wxReturnPayQuery.err_code_des;
                                return model;
                            }
                        }
                        else
                        {
                            model.Message = "微信退款失败！";
                            return model;
                        }
                    }

                }
            }
            #endregion
            #region 支付宝退款
            else if (input.PayType == PayType.Ali)
            {
                for (int i = 0; i < dsPayInfo.Tables[0].Rows.Count; i++)
                {
                    if (Convert.ToInt32(dsPayInfo.Tables[0].Rows[i]["pay_way"].ToString()) == (int)PayType.Ali)
                    {
                        payMoney += (decimal)dsPayInfo.Tables[0].Rows[i]["old_amount"];
                        payOrderNo = dsPayInfo.Tables[0].Rows[i]["thirdtradeno"].ToString();
                        //支付宝退款只能使用支付宝订单号 不能传商户订单号
                        payTradeNo = dsPayInfo.Tables[0].Rows[i]["tradeno"].ToString();
                    }
                }

                if (payMoney <= 0)
                {
                    model.Message = "退款失败，" + "此单未使用支付宝支付";
                    return model;
                }

                if (payMoney < input.ReturnAmount)
                {
                    model.Message = "退款失败，" + "退款金额不能大于支付金额";
                    return model;
                }

                string outRefundNo = System.DateTime.Now.ToString("yyyyMMddHHmmss") + userData.StorageNo.ToString() + userData.TerminalID.ToString() + userId; //商户唯一订单号 

                string url = userData.PayLink + "/AliReturnPay";
                string data = "trade_nopar=" + payTradeNo + "&out_request_nopar=" + outRefundNo + "&refund_amountpar=" + input.ReturnAmount.ToString() + "&key=" + userData.WebKey + "&random=" + (new Random()).Next(1, 10000).ToString();

                Logger.Info("支付宝退款：" + data);

                using (HttpClient http = new HttpClient())
                {
                    StringContent content = new StringContent(data, Encoding.UTF8, "application/x-www-form-urlencoded");
                    var result = await http.PostAsync(url, content);

                    string json = result.Content.ReadAsStringAsync().Result;

                    Logger.Info("支付宝退款：" + json);

                    if (json != null)//支付返回结果
                    {
                        int st = json.IndexOf('{');
                        try
                        {
                            json = json.Substring(st);
                            if (json.Length >= 9)
                            {
                                json = json.Substring(0, json.Length - 9);
                            }
                        }
                        catch (Exception ex)
                        {
                            Logger.Info("支付宝退款解析错误：" + ex.Message);
                        }

                        AliPayDto aliReturnPay = JsonConvert.DeserializeObject<AliPayDto>(json);
                        if (aliReturnPay.alipay_trade_refund_response != null && aliReturnPay.alipay_trade_refund_response.code.Equals("10000") && string.IsNullOrWhiteSpace(aliReturnPay.alipay_trade_refund_response.sub_code))
                        {
                            //插入支付记录
                            try
                            {
                                string sql = "insert into ft_thirdpayment (paymodeid,postradeno,out_trade_no,tradeno,status,inputtime,inputby,paymoney,branch_no,jh) values" +
                                    "('" + (int)PayType.Ali + "','" + orderNo + "','" + outRefundNo + "','" + aliReturnPay.alipay_trade_refund_response.trade_no + "',2,GETDATE(),'"
                                    + userId.ToString() + "'," + input.ReturnAmount + ",'" + userData.StorageNo + "','" + userData.TerminalID + "')";

                                await _parkDbContext.ExecuteNonQueryAsync(sql, null);
                            }
                            catch (Exception ex)
                            {
                                Logger.Error($"支付宝退款成功，但保存数据库记录出错：" + ex.Message);
                            }

                            PayReturnOutput payReturnOutput = new PayReturnOutput();
                            payReturnOutput.RefundFee = Convert.ToDecimal(aliReturnPay.alipay_trade_refund_response.refund_fee);
                            payReturnOutput.ReturnTradeNo = aliReturnPay.alipay_trade_refund_response.trade_no;
                            payReturnOutput.PayReturnOrderNo = outRefundNo;
                            payReturnOutput.PayOrderNo = payOrderNo;

                            model.Data = payReturnOutput;
                            model.Code = ResultCode.Ok;
                            return model;
                        }
                        else
                        {
                            Thread.Sleep(1000);

                            url = userData.PayLink + "/AliQuery";
                            data = "out_trade_nopar=" + payOrderNo + "&key=" + userData.WebKey + "&random=" + (new Random()).Next(1, 10000).ToString();

                            Logger.Info("支付宝退款查询：" + data);

                            content = new StringContent(data, Encoding.UTF8, "application/x-www-form-urlencoded");
                            result = await http.PostAsync(url, content);

                            json = result.Content.ReadAsStringAsync().Result;

                            Logger.Info("支付宝退款查询：" + json);

                            if (json != null)//支付返回结果
                            {
                                st = json.IndexOf('{');
                                try
                                {
                                    json = json.Substring(st);
                                    if (json.Length >= 9)
                                    {
                                        json = json.Substring(0, json.Length - 9);
                                    }
                                }
                                catch (Exception ex)
                                {
                                    Logger.Info("支付宝退款解析错误：" + ex.Message);
                                }

                                AliPayDto aliReturnPayQuery = JsonConvert.DeserializeObject<AliPayDto>(json);
                                if (aliReturnPayQuery.alipay_trade_query_response != null && aliReturnPayQuery.alipay_trade_query_response.code.Equals("10000") && aliReturnPayQuery.alipay_trade_query_response.trade_status == "TRADE_CLOSED")
                                {
                                    //插入支付记录
                                    try
                                    {
                                        string sql = "insert into ft_thirdpayment (paymodeid,postradeno,out_trade_no,tradeno,status,inputtime,inputby,paymoney,branch_no,jh) values" +
                                            "('" + (int)PayType.Ali + "','" + orderNo + "','" + outRefundNo + "','" + aliReturnPayQuery.alipay_trade_query_response.trade_no + "',2,GETDATE(),'"
                                            + userId.ToString() + "'," + input.ReturnAmount + ",'" + userData.StorageNo + "','" + userData.TerminalID + "')";

                                        await _parkDbContext.ExecuteNonQueryAsync(sql, null);
                                    }
                                    catch (Exception ex)
                                    {
                                        Logger.Error($"支付宝退款成功，但保存数据库记录出错：" + ex.Message);
                                    }

                                    PayReturnOutput payReturnOutput = new PayReturnOutput();
                                    payReturnOutput.RefundFee = Convert.ToDecimal(aliReturnPayQuery.alipay_trade_query_response.refund_fee);
                                    payReturnOutput.ReturnTradeNo = aliReturnPayQuery.alipay_trade_query_response.trade_no;
                                    payReturnOutput.PayReturnOrderNo = outRefundNo;
                                    payReturnOutput.PayOrderNo = payOrderNo;

                                    model.Data = payReturnOutput;
                                    model.Code = ResultCode.Ok;
                                    return model;
                                }
                                else
                                {
                                    model.Message = "支付宝退款失败！" + aliReturnPay.alipay_trade_query_response.sub_msg;
                                    return model;
                                }
                            }
                            else
                            {
                                model.Message = "支付宝退款失败！" + aliReturnPay.alipay_trade_refund_response.sub_msg;
                                return model;
                            }

                        }
                    }
                    else
                    {
                        Thread.Sleep(1000);

                        url = userData.PayLink + "/AliQuery";
                        data = "out_trade_nopar=" + payOrderNo + "&key=" + userData.WebKey + "&random=" + (new Random()).Next(1, 10000).ToString();

                        Logger.Info("支付宝退款查询：" + data);
                        content = new StringContent(data, Encoding.UTF8, "application/x-www-form-urlencoded");
                        result = await http.PostAsync(url, content);

                        json = result.Content.ReadAsStringAsync().Result;

                        Logger.Info("支付宝退款查询：" + json);

                        if (json != null)//支付返回结果
                        {
                            int st = json.IndexOf('{');
                            try
                            {
                                json = json.Substring(st);
                                if (json.Length >= 9)
                                {
                                    json = json.Substring(0, json.Length - 9);
                                }
                            }
                            catch (Exception ex)
                            {
                                Logger.Info("支付宝退款解析错误：" + ex.Message);
                            }

                            AliPayDto aliReturnPayQuery = JsonConvert.DeserializeObject<AliPayDto>(json);
                            if (aliReturnPayQuery.alipay_trade_query_response != null && aliReturnPayQuery.alipay_trade_query_response.code.Equals("10000") && aliReturnPayQuery.alipay_trade_query_response.trade_status == "TRADE_CLOSED")
                            {
                                //插入支付记录
                                try
                                {
                                    string sql = "insert into ft_thirdpayment (paymodeid,postradeno,out_trade_no,tradeno,status,inputtime,inputby,paymoney,branch_no,jh) values" +
                                        "('" + (int)PayType.Ali + "','" + orderNo + "','" + outRefundNo + "','" + aliReturnPayQuery.alipay_trade_query_response.trade_no + "',2,GETDATE(),'"
                                        + userId.ToString() + "'," + input.ReturnAmount + ",'" + userData.StorageNo + "','" + userData.TerminalID + "')";

                                    await _parkDbContext.ExecuteNonQueryAsync(sql, null);
                                }
                                catch (Exception ex)
                                {
                                    Logger.Error($"支付宝退款成功，但保存数据库记录出错：" + ex.Message);
                                }

                                PayReturnOutput payReturnOutput = new PayReturnOutput();
                                payReturnOutput.RefundFee = Convert.ToDecimal(aliReturnPayQuery.alipay_trade_query_response.refund_fee);
                                payReturnOutput.ReturnTradeNo = aliReturnPayQuery.alipay_trade_query_response.trade_no;
                                payReturnOutput.PayReturnOrderNo = outRefundNo;
                                payReturnOutput.PayOrderNo = payOrderNo;

                                model.Data = payReturnOutput;
                                model.Code = ResultCode.Ok;
                                return model;
                            }
                            else
                            {
                                model.Message = "支付宝退款失败！" + aliReturnPayQuery.alipay_trade_query_response.sub_msg;
                                return model;
                            }
                        }
                        else
                        {
                            model.Message = "支付宝退款失败！";
                            return model;
                        }
                    }
                }

            }
            #endregion
            #region 方特退款
            else if (input.PayType == PayType.Ft)
            {
                for (int i = 0; i < dsPayInfo.Tables[0].Rows.Count; i++)
                {
                    if (Convert.ToInt32(dsPayInfo.Tables[0].Rows[i]["pay_way"].ToString()) == (int)PayType.Ft)
                    {
                        payMoney += (decimal)dsPayInfo.Tables[0].Rows[i]["old_amount"];
                        payOrderNo = dsPayInfo.Tables[0].Rows[i]["thirdtradeno"].ToString();
                        //方特退款只能使用方特订单号 不能传商户订单号
                        payTradeNo = dsPayInfo.Tables[0].Rows[i]["tradeno"].ToString();
                    }

                    if (payMoney <= 0)
                    {
                        model.Message = "退款失败，" + "此单未使用方特支付";
                        return model;
                    }

                    if (payMoney != input.ReturnAmount)
                    {
                        model.Message = "退款失败，" + "方特支付必须整单退";
                        return model;
                    }

                    if (payMoney < input.ReturnAmount)
                    {
                        model.Message = "退款失败，" + "退款金额不能大于支付金额";
                        return model;
                    }

                    string timespan = GetTimeStamp();

                    var goodsname = "餐饮退单-" +orderNo ;

                    string url = userData.LeyoupayLink + "CRefund?";

                    string data = "amount=" + input.ReturnAmount.ToString();
                    data += "&clienttype=1";
                    data += "&des=" + goodsname;
                    data += "&isRefundAllProduct=1";
                    data += "&key=" + userData.LeyouKey;
                    data += "&merchant=" + userData.StorageName;//
                    data += "&mtorderno=" + payTradeNo;//
                    data += "&orderno=" + "";
                    data += "&parkid=" + userData.MutonParkid.ToString();

                    data += "&posTicketNumber=" + System.DateTime.Now.ToString("yyyyMMdd") + userData.StorageNo + userData.TerminalID + userData.Flow_No;

                    data += "&refundno=" + "";
                    data += "&timestamp=" + timespan;
                    data = "&signature=" + SHAEncrypt(data);

                    string data2 = "amount=" + input.ReturnAmount.ToString();
                    data2 += "&clienttype=1";
                    data2 += "&des=" + System.Web.HttpUtility.UrlEncode(goodsname);

                    data2 += "&isRefundAllProduct=1";

                    data2 += "&merchant=" + System.Web.HttpUtility.UrlEncode(userData.StorageName);//
                    data2 += "&mtorderno=" + payTradeNo;//
                    data2 += "&orderno=" + "";
                    data2 += "&parkid=" + userData.MutonParkid.ToString();

                    data2 += "&posTicketNumber=" + System.DateTime.Now.ToString("yyyyMMdd") + userData.StorageNo + userData.TerminalID + userData.Flow_No;

                    data2 += "&refundno=" + "";
                    data2 += "&timestamp=" + timespan;

                    data2 = data2 + data;

                    Logger.Info("方特退款：" + data);

                    using (HttpClient http = new HttpClient())
                    {
                        StringContent content = new StringContent(data2, Encoding.UTF8, "application/x-www-form-urlencoded");
                        var result = await http.PostAsync(url, content);

                        string json = result.Content.ReadAsStringAsync().Result;

                        Logger.Info("方特退款：" + json);

                        if (json != null)//支付返回结果
                        {

                            FtPayDto ftReturnPay = JsonConvert.DeserializeObject<FtPayDto>(json);
                            if (ftReturnPay != null && ftReturnPay.result.ToUpper() == "0")//支付返回结果
                            {
                                //插入支付记录
                                try
                                {
                                    string sql = "insert into ft_thirdpayment (paymodeid,postradeno,out_trade_no,tradeno,status,inputtime,inputby,paymoney,branch_no,jh) values" +
                                        "('" + (int)PayType.Ft + "','" + orderNo + "','" + payOrderNo + "','" + payTradeNo + "',2,GETDATE(),'"
                                        + userId.ToString() + "'," + input.ReturnAmount + ",'" + userData.StorageNo + "','" + userData.TerminalID + "')";

                                    await _parkDbContext.ExecuteNonQueryAsync(sql, null);
                                }
                                catch (Exception ex)
                                {
                                    Logger.Error($"方特退款成功，但保存数据库记录出错：" + ex.Message);
                                }

                                PayReturnOutput payReturnOutput = new PayReturnOutput();
                                payReturnOutput.RefundFee = Convert.ToDecimal(ftReturnPay.data.payamount);
                                payReturnOutput.ReturnTradeNo = ftReturnPay.data.mtorderno;
                                payReturnOutput.PayReturnOrderNo = payOrderNo;
                                payReturnOutput.PayOrderNo = payOrderNo;

                                model.Data = payReturnOutput;
                                model.Code = ResultCode.Ok;
                                return model;
                            }
                            else
                            {
                                Thread.Sleep(1000);

                                url = userData.LeyoupayLink + "CGetComsuseDetail?";
                                //参数加密必须按字母排序 
                                data = "";
                                data += "clienttype=1";
                                data += "&key=" + userData.LeyouKey;
                                data += "&merchant=" + userData.StorageName;
                                data += "&mtorderno=" + payTradeNo;
                                data += "&orderno=" + "";
                                data += "&parkid=" + userData.MutonParkid.ToString();
                                data += "&refundno=" + "";
                                data += "&timestamp=" + timespan;
                                data += "&type=3";
                                data = "&signature=" + SHAEncrypt(data);

                                data2 = "";
                                data2 += "clienttype=1";
                                data2 += "&merchant=" + System.Web.HttpUtility.UrlEncode(userData.StorageName);
                                data2 += "&mtorderno=" + payTradeNo;
                                data2 += "&orderno=" + "";
                                data2 += "&parkid=" + userData.MutonParkid.ToString();
                                data2 += "&refundno=" + "";
                                data2 += "&timestamp=" + timespan;
                                data2 += "&type=3";
                                data2 = data2 + data;

                                Logger.Info("方特退款查询：" + data2);

                                result = await http.GetAsync(url + data2);

                                json = result.Content.ReadAsStringAsync().Result;

                                Logger.Info("方特退款查询：" + json);

                                MutonPayQueryDto ftReturnPayQuery = JsonConvert.DeserializeObject<MutonPayQueryDto>(json);
                                if (ftReturnPayQuery != null && ftReturnPayQuery.result.ToUpper() == "0")//支付返回结果
                                {
                                    //插入支付记录
                                    try
                                    {
                                        string sql = "insert into ft_thirdpayment (paymodeid,postradeno,out_trade_no,tradeno,status,inputtime,inputby,paymoney,branch_no,jh) values" +
                                            "('" + (int)PayType.Ft + "','" + orderNo + "','" + payOrderNo + "','" + payTradeNo + "',2,GETDATE(),'"
                                            + userId.ToString() + "'," + input.ReturnAmount + ",'" + userData.StorageNo + "','" + userData.TerminalID + "')";

                                        await _parkDbContext.ExecuteNonQueryAsync(sql, null);
                                    }
                                    catch (Exception ex)
                                    {
                                        Logger.Error($"方特退款成功，但保存数据库记录出错：" + ex.Message);
                                    }

                                    PayReturnOutput payReturnOutput = new PayReturnOutput();
                                    payReturnOutput.RefundFee = Convert.ToDecimal(ftReturnPayQuery.data[0].payamount);
                                    payReturnOutput.ReturnTradeNo = ftReturnPayQuery.data[0].mtorderno;
                                    payReturnOutput.PayReturnOrderNo = payOrderNo;
                                    payReturnOutput.PayOrderNo = payOrderNo;

                                    model.Data = payReturnOutput;
                                    model.Code = ResultCode.Ok;
                                    return model;
                                }
                                else
                                {
                                    model.Message = "方特退款失败！" + ftReturnPay.message;
                                    return model;
                                }

                            }
                        }
                        else
                        {
                            Thread.Sleep(1000);

                            url = userData.LeyoupayLink + "CGetComsuseDetail?";
                            //参数加密必须按字母排序 
                            data = "";
                            data += "clienttype=1";
                            data += "&key=" + userData.LeyouKey;
                            data += "&merchant=" + userData.StorageName;
                            data += "&mtorderno=" + payTradeNo;
                            data += "&orderno=" + "";
                            data += "&parkid=" + userData.MutonParkid.ToString();
                            data += "&refundno=" + "";
                            data += "&timestamp=" + timespan;
                            data += "&type=3";
                            data = "&signature=" + SHAEncrypt(data);

                            data2 = "";
                            data2 += "clienttype=1";
                            data2 += "&merchant=" + System.Web.HttpUtility.UrlEncode(userData.StorageName);
                            data2 += "&mtorderno=" + payTradeNo;
                            data2 += "&orderno=" + "";
                            data2 += "&parkid=" + userData.MutonParkid.ToString();
                            data2 += "&refundno=" + "";
                            data2 += "&timestamp=" + timespan;
                            data2 += "&type=3";
                            data2 = data2 + data;

                            Logger.Info("方特退款查询：" + data2);

                            result = await http.GetAsync(url + data2);

                            json = result.Content.ReadAsStringAsync().Result;

                            Logger.Info("方特退款查询：" + json);

                            MutonPayQueryDto ftReturnPayQuery = JsonConvert.DeserializeObject<MutonPayQueryDto>(json);
                            if (ftReturnPayQuery != null && ftReturnPayQuery.result.ToUpper() == "0")//支付返回结果
                            {
                                //插入支付记录
                                try
                                {
                                    string sql = "insert into ft_thirdpayment (paymodeid,postradeno,out_trade_no,tradeno,status,inputtime,inputby,paymoney,branch_no,jh) values" +
                                        "('" + (int)PayType.Ft + "','" + orderNo + "','" + payOrderNo + "','" + payTradeNo + "',2,GETDATE(),'"
                                        + userId.ToString() + "'," + input.ReturnAmount + ",'" + userData.StorageNo + "','" + userData.TerminalID + "')";

                                    await _parkDbContext.ExecuteNonQueryAsync(sql, null);
                                }
                                catch (Exception ex)
                                {
                                    Logger.Error($"方特退款成功，但保存数据库记录出错：" + ex.Message);
                                }

                                PayReturnOutput payReturnOutput = new PayReturnOutput();
                                payReturnOutput.RefundFee = Convert.ToDecimal(ftReturnPayQuery.data[0].payamount);
                                payReturnOutput.ReturnTradeNo = ftReturnPayQuery.data[0].mtorderno;
                                payReturnOutput.PayReturnOrderNo = payOrderNo;
                                payReturnOutput.PayOrderNo = payOrderNo;

                                model.Data = payReturnOutput;
                                model.Code = ResultCode.Ok;
                                return model;
                            }
                            else
                            {
                                model.Message = "方特退款失败！";
                                return model;
                            }

                        }
                    }
                }


            }
            #endregion
            #region 秒通退款
            else if (input.PayType == PayType.Mutone)
            {
                for (int i = 0; i < dsPayInfo.Tables[0].Rows.Count; i++)
                {
                    if (Convert.ToInt32(dsPayInfo.Tables[0].Rows[i]["pay_way"].ToString()) == (int)PayType.Mutone)
                    {
                        payMoney += (decimal)dsPayInfo.Tables[0].Rows[i]["old_amount"];
                        payOrderNo = dsPayInfo.Tables[0].Rows[i]["thirdtradeno"].ToString();
                        //秒通退款只能使用秒通订单号 不能传商户订单号
                        payTradeNo = dsPayInfo.Tables[0].Rows[i]["tradeno"].ToString();
                    }
                }

                if (payMoney <= 0)
                {
                    model.Message = "退款失败，" + "此单未使用秒通支付";
                    return model;
                }
                if (payMoney < input.ReturnAmount)
                {
                    model.Message = "退款失败，" + "退款金额不能大于支付金额";
                    return model;
                }
                var goodsname = "餐饮系统秒通支付退单-" + orderNo;

                string timespan = GetTimeStamp();

                string url = userData.MutonpayLink + "CRefund?";

                string data = "merchant=" + userData.StorageName + "&mtorderno=" + payTradeNo + "&amount=" + input.ReturnAmount.ToString() + "&des=" + goodsname + "&clienttype=1&parkid=" + userData.MutonParkid.ToString() + "&timestamp=" + timespan;

                data = "&signature=" + SHAEncrypt(data + "&key=" + userData.MutoneKey);

                string data2 = "merchant=" + System.Web.HttpUtility.UrlEncode(userData.StorageName) + "&mtorderno=" + payTradeNo + "&amount=" + input.ReturnAmount + "&des=" + System.Web.HttpUtility.UrlEncode(goodsname) + "&clienttype=1&parkid=" + userData.MutonParkid.ToString() + "&timestamp=" + timespan;

                data2 = data2 + data;

                Logger.Info("秒通退款：" + data2);

                using (HttpClient http = new HttpClient())
                {

                    var result = await http.GetAsync(url + data2);

                    string json = result.Content.ReadAsStringAsync().Result;

                    Logger.Info("秒通退款：" + json);

                    if (json != null)//支付返回结果
                    {

                        MutonePayDto mutoneReturnPay = JsonConvert.DeserializeObject<MutonePayDto>(json);
                        if (mutoneReturnPay != null && mutoneReturnPay.result.ToUpper() == "0")//支付返回结果
                        {
                            //插入支付记录
                            try
                            {
                                string sql = "insert into ft_thirdpayment (paymodeid,postradeno,out_trade_no,tradeno,status,inputtime,inputby,paymoney,branch_no,jh) values" +
                                    "('" + (int)PayType.Mutone + "','" + orderNo + "','" + payOrderNo + "','" + payTradeNo + "',2,GETDATE(),'"
                                    + userId.ToString() + "'," + input.ReturnAmount + ",'" + userData.StorageNo + "','" + userData.TerminalID + "')";

                                await _parkDbContext.ExecuteNonQueryAsync(sql, null);
                            }
                            catch (Exception ex)
                            {
                                Logger.Error($"秒通退款成功，但保存数据库记录出错：" + ex.Message);
                            }

                            PayReturnOutput payReturnOutput = new PayReturnOutput();
                            payReturnOutput.RefundFee = Convert.ToDecimal(mutoneReturnPay.data.payamount);
                            payReturnOutput.ReturnTradeNo = mutoneReturnPay.data.mtorderno;
                            payReturnOutput.PayReturnOrderNo = payOrderNo;
                            payReturnOutput.PayOrderNo = payOrderNo;

                            model.Data = payReturnOutput;
                            model.Code = ResultCode.Ok;
                            return model;
                        }
                        else
                        {
                            url = userData.MutonpayLink + "CGetComsuseDetial?";

                            data = "merchant=" + userData.StorageName + "&mtorderno=" + payTradeNo + "&type=3&clienttype=1&parkid=" + userData.MutonParkid.ToString() + "&timestamp=" + timespan;

                            data = "&signature=" + SHAEncrypt(data + "&key=" + userData.MutoneKey);

                            data2 = "merchant=" + System.Web.HttpUtility.UrlEncode(userData.StorageName) + "&mtorderno=" + payTradeNo + "&type=3&clienttype=1&parkid=" + userData.MutonParkid.ToString() + "&timestamp=" + timespan;

                            data2 = data2 + data;
                            result = await http.GetAsync(url + data2);

                            json = result.Content.ReadAsStringAsync().Result;

                            Logger.Info("秒通退款查询：" + json);

                            MutonPayQueryDto mutoneReturnPayQuery = JsonConvert.DeserializeObject<MutonPayQueryDto>(json);
                            if (mutoneReturnPayQuery != null && mutoneReturnPayQuery.result.ToUpper() == "0")//支付返回结果
                            {
                                //插入支付记录
                                try
                                {
                                    string sql = "insert into ft_thirdpayment (paymodeid,postradeno,out_trade_no,tradeno,status,inputtime,inputby,paymoney,branch_no,jh) values" +
                                        "('" + (int)PayType.Mutone + "','" + orderNo + "','" + payOrderNo + "','" + payTradeNo + "',2,GETDATE(),'"
                                        + userId.ToString() + "'," + input.ReturnAmount + ",'" + userData.StorageNo + "','" + userData.TerminalID + "')";

                                    await _parkDbContext.ExecuteNonQueryAsync(sql, null);
                                }
                                catch (Exception ex)
                                {
                                    Logger.Error($"秒通退款成功，但保存数据库记录出错：" + ex.Message);
                                }

                                PayReturnOutput payReturnOutput = new PayReturnOutput();
                                payReturnOutput.RefundFee = Convert.ToDecimal(mutoneReturnPayQuery.data[0].payamount);
                                payReturnOutput.ReturnTradeNo = mutoneReturnPayQuery.data[0].mtorderno;
                                payReturnOutput.PayReturnOrderNo = payOrderNo;
                                payReturnOutput.PayOrderNo = payOrderNo;

                                model.Data = payReturnOutput;
                                model.Code = ResultCode.Ok;
                                return model;
                            }
                            else
                            {
                                model.Message = "秒通退款失败！" + mutoneReturnPay.message;
                                return model;
                            }

                        }
                    }
                    else
                    {
                        url = userData.MutonpayLink + "CGetComsuseDetial?";

                        data = "merchant=" + userData.StorageName + "&mtorderno=" + payTradeNo + "&type=3&clienttype=1&parkid=" + userData.MutonParkid.ToString() + "&timestamp=" + timespan;

                        data = "&signature=" + SHAEncrypt(data + "&key=" + userData.MutoneKey);

                        data2 = "merchant=" + System.Web.HttpUtility.UrlEncode(userData.StorageName) + "&mtorderno=" + payTradeNo + "&type=3&clienttype=1&parkid=" + userData.MutonParkid.ToString() + "&timestamp=" + timespan;

                        data2 = data2 + data;
                        result = await http.GetAsync(url + data2);

                        json = result.Content.ReadAsStringAsync().Result;

                        Logger.Info("秒通退款查询：" + json);

                        MutonPayQueryDto mutoneReturnPayQuery = JsonConvert.DeserializeObject<MutonPayQueryDto>(json);
                        if (mutoneReturnPayQuery != null && mutoneReturnPayQuery.result.ToUpper() == "0")//支付返回结果
                        {
                            //插入支付记录
                            try
                            {
                                string sql = "insert into ft_thirdpayment (paymodeid,postradeno,out_trade_no,tradeno,status,inputtime,inputby,paymoney,branch_no,jh) values" +
                                    "('" + (int)PayType.Mutone + "','" + orderNo + "','" + payOrderNo + "','" + payTradeNo + "',2,GETDATE(),'"
                                    + userId.ToString() + "'," + input.ReturnAmount + ",'" + userData.StorageNo + "','" + userData.TerminalID + "')";

                                await _parkDbContext.ExecuteNonQueryAsync(sql, null);
                            }
                            catch (Exception ex)
                            {
                                Logger.Error($"秒通退款成功，但保存数据库记录出错：" + ex.Message);
                            }

                            PayReturnOutput payReturnOutput = new PayReturnOutput();
                            payReturnOutput.RefundFee = Convert.ToDecimal(mutoneReturnPayQuery.data[0].payamount);
                            payReturnOutput.ReturnTradeNo = mutoneReturnPayQuery.data[0].mtorderno;
                            payReturnOutput.PayReturnOrderNo = payOrderNo;
                            payReturnOutput.PayOrderNo = payOrderNo;

                            model.Data = payReturnOutput;
                            model.Code = ResultCode.Ok;
                            return model;
                        }
                        else
                        {
                            model.Message = "秒通退款失败！";
                            return model;
                        }
                    }
                }
            }

            #endregion

            model.Message = "退款失败！";
            return model;
        }

        #region 礼券查询

        /// <summary>
        /// 礼券查询
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<Result<VolumeQueryOutput>> VolumeQuery(VolumeQueryInput input)
        {
            Result<VolumeQueryOutput> model = new Result<VolumeQueryOutput>(ResultCode.Fail, null);

            string userId = FishSession.UserId;
            if (string.IsNullOrWhiteSpace(userId))
            {
                model.Message = "未获取到用户信息，请先登录";
                return model;
            }

            var userData = _cacheManager.GetUserDataCacheByUserId(userId);


            string url = userData.VolumePayLink + "/CheckVolumeInfo?";
            string data = "ParkID=" + userData.VolumePayParkId + "&Card_ID=" + input.VolumeNo ;


            using (HttpClient http = new HttpClient())
            {
                StringContent content = new StringContent(data, Encoding.UTF8, "application/x-www-form-urlencoded");
                var result = await http.GetAsync(url+ data);

                string json = result.Content.ReadAsStringAsync().Result;

                Logger.Info("礼券查询：" + json);

                if (json != null)//支付返回结果
                {

                    int st = json.IndexOf(".com/\">");
                    try
                    {
                        json = json.Substring(st);
                        if (json.Length >= 9)
                        {
                            json = json.Substring(7, json.Length - 9 -7);
                        }
                    }
                    catch (Exception ex)
                    {
                        Logger.Info("礼券查询解析错误：" + ex.Message);
                    }

                    var resultData = json.Split(',');
                    if (resultData.Length > 1)
                    {
                        VolumeQueryOutput volumeQueryOutput = new VolumeQueryOutput();

                        if (resultData[0] == "1")
                        {
                            volumeQueryOutput.Amount = decimal.Parse(resultData[1]);//票券面额
                            model.Data = volumeQueryOutput;
                            model.Code = ResultCode.Ok;
                            return model;
                        }
                        //礼券无效，输出错误信息
                        else
                        {
                            model.Message = resultData[1];
                            return model;
                        }
                    }
                }

            }
            model.Message = "查询失败";
            return model;


        }

        #endregion


        #region 礼券状态更新接口
        /// <summary>
        /// 礼券状态更新接口
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<Result> VolumeUpdateStatus(VolumeUpdateInput input)
        {
            Result model = new Result(ResultCode.Fail, null);

            string userId = FishSession.UserId;
            if (string.IsNullOrWhiteSpace(userId))
            {
                model.Message = "未获取到用户信息，请先登录";
                return model;
            }

            var userData = _cacheManager.GetUserDataCacheByUserId(userId);

            var volumeNos = "";
            foreach (var item in input.VolumeNos)
            {
                volumeNos += item + "@";
            }

            string url = userData.VolumePayLink + "/UpdateVolumeStatus?";
            string data = "ParkID=" + userData.VolumePayParkId + "&Card_IDS=" + volumeNos + "&inputby="+ userId + "&key=&trandtype=" + input.Status;

            using (HttpClient http = new HttpClient())
            {
                StringContent content = new StringContent(data, Encoding.UTF8, "application/x-www-form-urlencoded");
                var result = await http.GetAsync(url + data);

                string json = result.Content.ReadAsStringAsync().Result;

                Logger.Info("礼券状态更新接口：" + json);

                if (json != null)//支付返回结果
                {

                    int st = json.IndexOf(".com/\">");
                    try
                    {
                        json = json.Substring(st);
                        if (json.Length >= 9)
                        {
                            json = json.Substring(7, 4);
                        }
                    }
                    catch (Exception ex)
                    {
                        Logger.Info("礼券状态更新接口解析错误：" + ex.Message);
                    }

                    
                    if (json.ToUpper()=="TRUE")
                    {
                        model.Code = ResultCode.Ok;
                        return model;
                    }
                }

            }
            model.Message = "更新失败";
            return model;

        }
        #endregion

    }
}
