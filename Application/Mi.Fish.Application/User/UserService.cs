using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Abp.AutoMapper;
using Abp.Domain.Uow;
using Abp.EntityFrameworkCore.Uow;
using Abp.Runtime.Caching;
using Abp.UI;
using Mi.Fish.ApplicationDto;
using Mi.Fish.Core;
using Mi.Fish.EntityFramework;
using Castle.Core.Logging;
using Mi.Fish.Infrastructure.Results;
using Microsoft.Extensions.Options;
using Mi.Fish.Application.Sync;
using Mi.Fish.Common;
using Microsoft.IdentityModel.Tokens;

namespace Mi.Fish.Application
{
    public class UserService: AppServiceBase, IUserService
    {             
        private readonly ICacheManager _cacheManager;
        private readonly ParkDbContext _parkDbContext;
        private readonly LocalDbContext _localDbContext;
        private readonly ISyncAppService _syncAppService;
     
        public UserService(ICacheManager cacheManager, ParkDbContext parkDbContext, LocalDbContext localDbContext, ISyncAppService syncAppService)
        {
            _cacheManager = cacheManager;
            _parkDbContext = parkDbContext;
            _localDbContext = localDbContext;
            _syncAppService = syncAppService;
        }

        /// <summary>
        /// 用户登录
        /// </summary>
        /// <param name="userLoginInput"></param>
        /// <returns></returns>
        public Result<UserData> UserLogin(UserLoginInput userLoginInput)
        {
            var userLoginInputParam = userLoginInput.MapTo<UserLoginDto>();
            var model = new Result<UserData>(ResultCode.Fail, null);

            try
            {
                var execLoginSql = @"exec ft_pr_login @cashierid,@cashierpw,@startamount,@banci";
                DataSet ds = _localDbContext.GetDataSet(execLoginSql, userLoginInputParam);
                
                if (ds.Tables[0].Rows[0]["err"].ToString() != "0")  //账号密码错误
                {
                    model.Message = ds.Tables[0].Rows[0]["errmsg"].ToString();
                    return model;
                }
                if (ds.Tables[1].Rows[0]["err"].ToString() != "0") //班次检查错误
                {
                    model.Message = ds.Tables[0].Rows[0]["errmsg"].ToString();
                    return model;
                }
                if (ds.Tables[2].Rows[0]["err"].ToString() != "0") //班次输入错误
                {
                    string strMessage = ds.Tables[2].Rows[0]["errmsg"].ToString();

                    try
                    {
                        string test = ds.Tables[3].Rows[0]["IntValue"].ToString();
                    }
                    catch (Exception ex)
                    {
                        model.Message = strMessage;
                        Logger.Error($"登录错误:{ex}");
                        return model;
                    }

                    strMessage += ",上一班次号为：" + ds.Tables[3].Rows[0]["IntValue"].ToString();
                    model.Message = strMessage;
                    return model;
                }
                if (ds.Tables.Count == 5)//信息表
                {
                    UserData userData = new UserData();

                    userData.UserID = userLoginInputParam.cashierid;
                    userData.UserName = ds.Tables[4].Rows[0]["cashier_name"].ToString();
                    userData.StorageNo = userLoginInput.StorageNo;
                    userData.TerminalID = userLoginInput.FishNo; 
                    userData.Min_ZK = Convert.ToDecimal(ds.Tables[4].Rows[0]["min_zk"]); //全部打折最小折扣
                    //获取权限
                    string getRrantSql = "declare @ModulePower varchar(20)	declare @quanxian varchar(30)" +
                                         "	select @ModulePower=sCashierLevelID from tCashier where sCashierNO=@cashierid	" +
                                         "set @quanxian='0000000000000000000000000000000000000000000000'	" +
                                         "if @ModulePower='5' " +
                                         " set @quanxian='1111111111111111111111111111111111111111111111'	" +
                                         "if ( @ModulePower='1' or @ModulePower='0' or @ModulePower='4' or @ModulePower='3')	  " +
                                         "set @quanxian='0010000000001010000000010000000000000000000000'	" +
                                         "if @ModulePower='2'	  set @quanxian='0000100000000000000000000000000000000000000000' " +
                                         "select @quanxian";


                    DataTable dtPowerDataTable = _parkDbContext.GetDataSet(getRrantSql, userLoginInputParam).Tables[0];
                    if (dtPowerDataTable != null && dtPowerDataTable.Rows.Count > 0)
                    {
                        string grant = dtPowerDataTable.Rows[0][0].ToString();
                        userData.SetGrant = grant.Substring(4, 1) == "1";
                        userData.SearchSaleGrant = grant.Substring(2, 1) == "1";
                        userData.BackGoods = grant.Substring(9, 1) == "1";
                        userData.RePrint = grant.Substring(12, 1) == "1";
                        userData.OpenBox = grant.Substring(14, 1) == "1";
                        userData.SingleDaZhe = grant.Substring(17, 1) == "1";
                        userData.AllDaZhe = grant.Substring(18, 1) == "1";
                        userData.TuiCai = grant.Substring(23, 1) == "1";
                        userData.DeleOrder = grant.Substring(25, 1) == "1";
                    }

                   
                    //票务餐品
                    DataTable ticketFood = _parkDbContext.GetDataSet("SELECT kindname,kindvalue FROM ft_zdb WHERE kindid = 8", new object()).Tables[0];
                    if (ticketFood != null && ticketFood.Rows.Count > 0)
                    {
                        userData.TicketFood.Clear();
                        for (int i = 0; i < ticketFood.Rows.Count; i++)
                        {
                            userData.TicketFood.Add(ticketFood.Rows[i][0].ToString(), ticketFood.Rows[i][1].ToString());
                        }
                    }

                    //获取前端用户配置数据
                    string localConfigDataSql = "select ChrValue from sysvar where Name = 'STORENO'" + " select ChrValue from sysvar where Name = 'STORENAME'" +
                        " SELECT IntValue FROM SysVar WHERE NAME = 'LASTTURN'" + " select ChrValue from SysVar where Name = 'LASTUSERID'" +
                        " select DatValue from SysVar where Name = 'LASTLOGTIME'" + " select DecValue from SysVar where Name = 'CURRMONEY'" +
                        " select intvalue+1 from sysvar where name = 'CURRMAXNO'";

                    DataSet localConfigDataDataSet = _localDbContext.GetDataSet(localConfigDataSql, new object());
                    userData.StorageName = localConfigDataDataSet.Tables[1].Rows[0][0].ToString();
                    //获取当班数据
                    userData.WorkGroup = localConfigDataDataSet.Tables[2].Rows[0][0].ToString();
                    userData.CurrentOperID = localConfigDataDataSet.Tables[3].Rows[0][0].ToString();//当前当班人
                    userData.CurrentDealTime = Convert.ToDateTime(localConfigDataDataSet.Tables[4].Rows[0][0].ToString());//当前当班人当班起始时间
                    userData.CurrentDealMoney = Convert.ToDecimal(localConfigDataDataSet.Tables[5].Rows[0][0].ToString());
                    userData.CurrencyEN = "RMB";//英文币种
                    userData.CoinRate = 1;//汇率
                    userData.CurrencyCN = "人民币";//中文币种


                    //获取后端用户配置数据
                    string getConfigDadaSql = "select kindid ,kindname ,kindvalue,branchno  from ft_zdb  where kindid >= 10 and kindid<=16  or (kindid >= 18 and kindid<=28)";
                    DataSet configDadaSqlDataSet = _parkDbContext.GetDataSet(getConfigDadaSql, new object());

                    if (configDadaSqlDataSet.Tables.Count != 0 && configDadaSqlDataSet.Tables[0].Rows.Count > 0)
                    {
                        DataTable dt = configDadaSqlDataSet.Tables[0];
                        userData.AppLink = dt.GetValue("kindid=10");
                        userData.WebKey = dt.GetValue("kindid=12");
                        userData.PayLink = dt.GetValue("kindid=13");
                        userData.MutonpayLink = dt.GetValue("kindid=14");
                        userData.MutonParkid = dt.GetValue("kindid=15");
                        userData.MutonpayRole = dt.GetValue("kindid=16");
                        userData.MutoneKey = dt.GetValue("kindid=18");
                        userData.MutonePrice = "1";
                        userData.MutonePrice = dt.GetValue("kindid=19");
                        userData.LeyouAccount = dt.GetValue("kindid=20");
                        userData.LeyouPwd = dt.GetValue("kindid=21");
                        userData.LeyoupayLink = dt.GetValue("kindid=22");
                        userData.LeyouKey = dt.GetValue("kindid=23");
                        userData.LeyouPrice = "1";
                        userData.LeyouPrice = dt.GetValue("kindid=24");
                        userData.VolumePayParkId = dt.GetValue("kindid=25");
                        userData.VolumePayLink = dt.GetValue("kindid=26");

                        userData.SyncDataPath = dt.GetValue("kindid=27 and branchno='" + userData.StorageNo.ToString() + userData.TerminalID.ToString() + "'");
                        userData.FishDataSyncPath = dt.GetValue("kindid=28 and branchno='" + userData.StorageNo.ToString() + userData.TerminalID.ToString() + "'");
                    }

                    if (string.IsNullOrEmpty(userData.Flow_No))//生成单号
                    {
                        userData.Flow_No = userData.TerminalID + localConfigDataDataSet.Tables[6].Rows[0][0].ToString().PadLeft(4, '0');
                    }
                    userData.StorageValue = 0;

                    _cacheManager.SetUserDataCache(userLoginInput.CashierNo, userData);

                    model.Code = ResultCode.Ok;
                    model.Data = userData;
                }
            }
            catch (Exception e)
            {
                model.Message = "登录异常";
                Logger.Error($"登录异常:{e.Message}");
            }

            return model;
        }

        /// <summary>
        /// 用户退出
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <returns></returns>
        public async Task<Result> UserLogOut(string userId)
        {
            Result result = new Result(ResultCode.Fail, string.Empty);
            UserData userData = new UserData();
            try
            {
                userData = _cacheManager.GetUserDataCacheByUserId(userId);
                if (userData == null)
                {
                    result.Code = ResultCode.Expired;
                    result.Message = result.CodeDesc;
                    return result;
                }


                #region 同步数据
                //退出系统前判断当天前后台销售数据是否一致
                CheckSyncData ckSyncData = new CheckSyncData(_localDbContext, _parkDbContext, userData);
                bool isDataSyncSuccess = await ckSyncData.IsDataSyncSuccess("");
                if (!isDataSyncSuccess)
                {
                    //更新前台Purchase表时间，同步数据
                    bool isUpdateSucces = await ckSyncData.UpdateTB_purchase("");
                    if (isUpdateSucces)
                    {
                        //同步数据
                        _syncAppService.SyncData(userData.StorageNo, userData.TerminalID, true);
                    }
                }
                #endregion

              
                //清理用户缓存
                _cacheManager.RemoveUserDataCacheByUserId(userId);
                result.Code = ResultCode.Ok;
            }
            catch (Exception e)
            {

                #region    操作日志

                LogET log = new LogET
                {
                    jh = userData?.TerminalID,
                    casher_no = userData?.UserID,
                    power_man = userData?.UserID,
                    oper_text = userData?.UserID + "用户退出,同步失败！",
                    dj_no = "",
                    oper_type = LogOperType.tbsb
                };

                string strSql =
                    $@"INSERT INTO pos_operator_log (casher_no, oper_date, power_man, oper_type, dj_no, jh, oper_text) 
                                  VALUES ('{log.casher_no}',getdate(),'{log.power_man}','{log.oper_type}','{log.dj_no}','{log.jh}','{log.oper_text}')";
                await _localDbContext.ExecuteNonQueryAsync(strSql, null);

                #endregion

                result.Message = "退出异常";
                Logger.Error($"用户退出:{e}");
            }

            return result;
        }



        /// <summary>
        /// 用户密码修改
        /// </summary>
        /// <param name="userPwdUpdateInput"></param>
        /// <returns></returns>
        public async Task<Result> UserPwdUpdate(UserPwdUpdateInput userPwdUpdateInput)
        {
            string userId = FishSession.UserId;
            Result model = new Result(ResultCode.Fail);
            string sql = "exec ft_pr_editpassword @oldpwd,@newpwd,@userid";
            var pattern = "^[0-9]{6}$";
            Regex regex = new Regex(pattern);
            if (!regex.IsMatch(userPwdUpdateInput.NewPwd))
            {
                model.Message = "密码必须为6位数字";
                return model;
            }

            try
            {
                var userData = _cacheManager.GetUserDataCacheByUserId(userId);
                if (userData==null)
                {
                    model.Code = ResultCode.Expired;
                    model.Message = model.CodeDesc;
                    return model;
                }
                var inputParam = userPwdUpdateInput.MapTo<UserPwdUpdateDto>();
                inputParam.userid = userData.UserID;
                DataTable dt =await _parkDbContext.GetDataTableAsync(sql, inputParam);
                
                if (dt.Rows.Count > 0)
                {
                    if (dt.Rows[0][0].ToString() == "1")
                    {
                        model.Code = ResultCode.Ok;

                        //密码同步
                        _syncAppService.SyncData(userData.StorageNo, userData.TerminalID, false);
                    }
                    else
                    {
                         model.Message= dt.Rows[0][1].ToString();
                    }
                }

            }
            catch (Exception e)
            {
                Logger.Error($"密码修改:{e}");
                model.Message = "密码修改失败";
            }

            return model;
        }



        /// <summary>
        /// 交班
        /// </summary>
        /// <returns></returns>
        public async Task<Result> UserTurnClass()
        {
            Result model = new Result(ResultCode.Ok, String.Empty);
            string userId = FishSession.UserId;
            UserData userData = new UserData();
            try
            {
                userData = _cacheManager.GetUserDataCacheByUserId(userId);
                if (userData == null)
                {
                    model.Code = ResultCode.Expired;
                    model.Message = model.CodeDesc;
                    return model;
                }

                #region 同步数据
                CheckSyncData ckSyncData = new CheckSyncData(_localDbContext, _parkDbContext, userData);
                bool isDataSyncSuccess = await ckSyncData.IsDataSyncSuccess("");
                if (!isDataSyncSuccess)
                {
                    //更新前台Purchase表时间，同步数据
                    bool isUpdateSucces = await ckSyncData.UpdateTB_purchase("");
                    if (isUpdateSucces)
                    {
                        //同步数据
                        _syncAppService.SyncData(userData.StorageNo, userData.TerminalID, true);
                    }
                }
                #endregion



                string clearSql = "exec ft_pr_clearTurn @cashierid ,@cashierpw";
                var clearObj = new
                {
                    cashierid = userData.UserID,
                    cashierpw = DBNull.Value
                };
                DataTable clearDataTable = await _localDbContext.GetDataTableAsync(clearSql, clearObj);

                //清机
                if (clearDataTable.Rows[0][0].ToString() != "0")  //清机错误
                {
                    model.Code = ResultCode.Fail;
                    model.Message = "清机错误！";
                    return model;
                }

                //交班
                string turnClassSql = "exec ft_pr_successionTurn";
                DataTable turnClassDataTable = await _localDbContext.GetDataTableAsync(turnClassSql, new object());
                if (turnClassDataTable.Rows[0]["err"].ToString() != "0")
                {
                    model.Code = ResultCode.Fail;
                    model.Message = turnClassDataTable.Rows[0]["errmsg"].ToString();
                    return model;
                }


                //清理用户缓存
                _cacheManager.RemoveUserDataCacheByUserId(userId);
                //清理用户Token
                _cacheManager.RemoveUserTokenCache(userId);
                //删除销售菜单缓存
                _cacheManager.RemoveSaleMenuCache(userId);
            }
            catch (Exception e)
            {
                #region    操作日志

                LogET log = new LogET
                {
                    jh = userData?.TerminalID,
                    casher_no = userData?.UserID,
                    power_man = userData?.UserID,
                    oper_text = userData?.UserID + "用户交班,同步失败！",
                    dj_no = "",
                    oper_type = LogOperType.tbsb
                };

                string strSql =
                    $@"INSERT INTO pos_operator_log (casher_no, oper_date, power_man, oper_type, dj_no, jh, oper_text) 
                                  VALUES ('{log.casher_no}',getdate(),'{log.power_man}','{log.oper_type}','{log.dj_no}','{log.jh}','{log.oper_text}')";
                await _localDbContext.ExecuteNonQueryAsync(strSql, null);

                #endregion

               Logger.Error($"交班失败:{e}");
               model.Message = "交班异常";
            }


            return model;
        }

        /// <summary>
        /// 交班明细
        /// </summary>
        /// <returns></returns>
        public async Task<Result<TurnClassDetailOutPut>> GetUserTurnClassDetail()
        {
            try
            {
                string userId = FishSession.UserId;
                var userData = _cacheManager.GetUserDataCacheByUserId(userId);
                if (userData == null)
                {
                   throw new UserFriendlyException("登录已失效");
                }
                TurnClassDetailOutPut data = new TurnClassDetailOutPut()
                {
                    Shift = userData.WorkGroup,
                    CashierNo = userData.UserID,
                    PrintTime = DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToLongTimeString(),
                    ShiftStartTime = userData.CurrentDealTime.ToShortDateString() + " " + userData.CurrentDealTime.ToLongTimeString(),
                    ShiftEndTime = DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToLongTimeString(),
                    ShiftBalanceMoney = "(" + userData.CurrencyCN + ")" + string.Format("{0:F2}", userData.CurrentDealMoney),
                    RealSaleMoney= userData.CurrencyCN + "：0.00" 
                };

                //开张单数
                string saleCountSql =
                    "select count (distinct ReceiptID) , min (BuyTime) from Purchase  where  BuyDate = Convert(varchar,getdate(),112) and CashierNO = @CashierNO AND Status = 2 ";

                DataTable saleCountDataTable = await _localDbContext.GetDataTableAsync(saleCountSql, new { CashierNO = data.CashierNo });
                if (saleCountDataTable.Rows.Count > 0)
                {
                    int.TryParse(saleCountDataTable.Rows[0][0].ToString(), out var saleCount);
                    data.SaleCount = saleCount;
                }

                //是否显示金额
                int isShowMoney = 0;
                DataTable shouMoneyDataTable = await _parkDbContext.GetDataTableAsync("select kindvalue from dbo.ft_zdb where kindid=11 and kindname='tunclass'",
                     new object());
                if (shouMoneyDataTable.Rows.Count > 0)
                {
                    int.TryParse(shouMoneyDataTable.Rows[0][0].ToString(), out var shouMoneey);
                    isShowMoney = shouMoneey;
                }

                //获取当天实销价格，原始价格
                AmountReceivable amountReceivable=new AmountReceivable();
                string saleMoneySql =
                    "select P.PayWay as pay_way,P.CardID as card_no,P.PayAmount as pay_amount from Purchase T,Payment P where  T.BuyDate = P.BuyDate  and T.ReceiptID = P.ReceiptID and Convert(varchar(100), T.BuyDate,23)   = Convert(varchar(100),getdate(),23) and T.CashierNO =@CashierNO  AND T.Status = 2 and (T.Turn = @Turn or @Turn is null)";

                DataTable saleMoneyDataTable = await _localDbContext.GetDataTableAsync(saleMoneySql, new
                {
                    CashierNO = userData.UserID,
                    Turn = userData.WorkGroup
                });

                if (saleMoneyDataTable.Rows.Count > 0)
                {
                    object objMoney;
                    string hidemark = "****";
                    //实销金额
                    data.RealSaleMoney = isShowMoney == 0 ? userData.CurrencyCN + "：" + string.Format("{0:F2}", saleMoneyDataTable.Compute("sum(pay_amount)", "pay_way='1' and (card_no ='' or card_no is null)")) : hidemark;
                   
                    //人民币
                    amountReceivable.RmbSaleMoney = data.RealSaleMoney;

                    //信用卡
                    objMoney = saleMoneyDataTable.Compute("sum(pay_amount)", "pay_way='5'");
                    if (!string.IsNullOrEmpty(objMoney.ToString()))
                    {
                        amountReceivable.CreditCardMoney = "信用卡：" + (isShowMoney == 0 ? string.Format("{0:F2}", objMoney) : hidemark);
                    }

                    //金卡
                    //objMoney = saleMoneyDataTable.Compute("sum(pay_amount)", "pay_way='D'");
                    //if (!string.IsNullOrEmpty(objMoney.ToString()))
                    //{
                    //    amountReceivable.GoldMoney = "金  卡：" + (isShowMoney == 0 ? string.Format("{0:F2}", Convert.ToDecimal(objMoney)) : hidemark);
                    //}

                    //方特APP
                    objMoney = saleMoneyDataTable.Compute("sum(pay_amount)", "pay_way='105'");
                    if (!string.IsNullOrEmpty(objMoney.ToString()))
                    {
                        amountReceivable.AppMoney = "方特APP：" + (isShowMoney == 0 ? string.Format("{0:F2}", objMoney) : hidemark);
                    }

                    //微信
                    objMoney = saleMoneyDataTable.Compute("sum(pay_amount)", "pay_way='120'");
                    if (!string.IsNullOrEmpty(objMoney.ToString()))
                    {
                        amountReceivable.WxMoney = "微  信：" + (isShowMoney == 0 ? string.Format("{0:F2}", objMoney) : hidemark);
                    }

                    //支付宝
                    objMoney = saleMoneyDataTable.Compute("sum(pay_amount)", "pay_way='130'");
                    if (!string.IsNullOrEmpty(objMoney.ToString()))
                    {
                        amountReceivable.AppMoney = "支付宝：" + (isShowMoney == 0 ? string.Format("{0:F2}", objMoney) : hidemark);
                    }

                    //秒通
                    objMoney = saleMoneyDataTable.Compute("sum(pay_amount)", "pay_way='112'");
                    if (!string.IsNullOrEmpty(objMoney.ToString()))
                    {
                        amountReceivable.MutonMoney = "秒  通：" + (isShowMoney == 0 ? string.Format("{0:F2}",objMoney) : hidemark);
                    }

                    //方特支付
                    objMoney = saleMoneyDataTable.Compute("sum(pay_amount)", "pay_way='113'");
                    if (!string.IsNullOrEmpty(objMoney.ToString()))
                    {
                        amountReceivable.MutonMoney = "方特支付：" + (isShowMoney == 0 ? string.Format("{0:F2}", objMoney) : hidemark);
                    }

                    //方特卡
                    objMoney = saleMoneyDataTable.Compute("sum(pay_amount)", "pay_way='93' ");
                    if (!string.IsNullOrEmpty(objMoney.ToString()))
                    {
                        amountReceivable.FateCardSaleMoney = "方特卡：" + (isShowMoney == 0 ? string.Format("{0:F2}",objMoney) : hidemark);
                    }

                    //二维码
                    //objMoney = saleMoneyDataTable.Compute("sum(pay_amount)", "pay_way='QR'");
                    //if (!string.IsNullOrEmpty(objMoney.ToString()))
                    //{
                    //    amountReceivable.QRcodeMoney = "二维码：" + (isShowMoney == 0 ? string.Format("{0:F2}", Convert.ToDecimal(objMoney)) : hidemark);
                    //}

                    //礼券
                    objMoney = saleMoneyDataTable.Compute("sum(pay_amount)", "pay_way='8'");
                    if (!string.IsNullOrEmpty(objMoney.ToString()))
                    {
                        amountReceivable.CouponsMoney = "礼  券：" + (isShowMoney == 0 ? string.Format("{0:F2}",objMoney) : hidemark);
                    }

                    //代金券
                    objMoney = saleMoneyDataTable.Compute("sum(pay_amount)", "pay_way='84'");
                    if (!string.IsNullOrEmpty(objMoney.ToString()))
                    {
                        amountReceivable.VoucherMoney = "代金券：" + (isShowMoney == 0 ? string.Format("{0:F2}",objMoney) : hidemark);
                    }

                    //微信(外)
                    objMoney = saleMoneyDataTable.Compute("sum(pay_amount)", "pay_way='992'");
                    if (!string.IsNullOrEmpty(objMoney.ToString()))
                    {
                        amountReceivable.WxOutMoney = "微信(外)：" + (isShowMoney == 0 ? string.Format("{0:F2}",objMoney) : hidemark);
                    }

                    //支付宝(外)
                    objMoney = saleMoneyDataTable.Compute("sum(pay_amount)", "pay_way='993'");
                    if (!string.IsNullOrEmpty(objMoney.ToString()))
                    {
                        amountReceivable.AliOutMoney = "支付宝(外)：" + (isShowMoney == 0 ? string.Format("{0:F2}", objMoney) : hidemark);
                    }

                }

               //实销列表
               List<string> amountReceivableList=new List<string>();
               var count=  amountReceivable.GetType().GetProperties().Select(a => a.GetValue(amountReceivable, null)).Count(a => a != null);
                //设置默认是值
                if (count==0)
                {
                    amountReceivableList.Add(userData.CurrencyCN + "：0.00");
                }
                else
                {
                    foreach (var item in amountReceivable.GetType().GetProperties())
                    {
                        var getValue = item.GetValue(amountReceivable, null);
                        if (getValue != null)
                        {
                             amountReceivableList.Add(getValue.ToString());
                        }
                    }
                }
                data.ListAmountReceivable = amountReceivableList;

                return Result.FromData(data);
            }
            catch (Exception e)
            {
                Logger.Error($"交班明细:{e}");
                return Result.Fail<TurnClassDetailOutPut>(ResultCode.Fail, "交班明细获取失败");
            }
        }
    }

}
