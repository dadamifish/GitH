using Abp.AutoMapper;
using Abp.Runtime.Caching;
using Mi.Fish.Application.Sync;
using Mi.Fish.ApplicationDto;
using Mi.Fish.EntityFramework;
using Mi.Fish.Infrastructure.Results;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Mi.Fish.Application.Order
{
    /// <summary>
    /// 
    /// </summary>
    public class OrderNoAppService : AppServiceBase, IOrderNoAppService
    {
        private readonly LocalDbContext _localContext;
        private readonly ICacheManager _cacheManager;
        private readonly ISyncAppService _syncService;
        private readonly ParkDbContext _parkDb;

        /// <summary>Initializes a new instance of the <see cref="T:System.Object"></see> class.</summary>
        public OrderNoAppService(LocalDbContext localcontext, ICacheManager cacheManager, ISyncAppService syncService, ParkDbContext parkDb)
        {
            _localContext = localcontext;
            this._cacheManager = cacheManager;
            this._syncService = syncService;
            this._parkDb = parkDb;
        }

        /// <summary>
        /// 生成单号
        /// </summary>
        /// <returns></returns>
        public async Task<Result<string>> CreateOrderNo()
        {
            string userId = FishSession.UserId;
            var _userData = _cacheManager.GetUserDataCacheByUserId(userId);
            var sql = "select intvalue + 1 from sysvar where name = 'CURRMAXNO'";
            var order = await _localContext.ExecuteScalarAsync(sql, null);
            var result = _userData.TerminalID + order.ToString().PadLeft(4, '0');
            return Result.FromData(result);
        }

        public Result<bool> CheckPermission(EnumAuthorityType authorityType)
        {
            string userId = FishSession.UserId;
            var _userData = _cacheManager.GetUserDataCacheByUserId(userId);
            switch (authorityType)
            {
                case EnumAuthorityType.BackGood:
                    if (_userData.BackGoods)
                    {
                        return Result.FromData<bool>(true);
                    }
                    break;
                case EnumAuthorityType.SingleDiscount:
                    if (_userData.SingleDaZhe)
                    {
                        return Result.FromData<bool>(true);
                    }
                    break;
                case EnumAuthorityType.AllDiscount:
                    if (_userData.AllDaZhe)
                    {
                        return Result.FromData<bool>(true);
                    }
                    break;
                default:
                    break;
            }
            var isAuthority = _cacheManager.GetUserAuthority(_userData.UserID, authorityType);
            if (isAuthority)
            {
                _cacheManager.RemoveUserAuthority(_userData.UserID, authorityType);
            }
            return Result.FromData<bool>(isAuthority);
        }
        public async Task<Result> SendCookPrint(string orderNo, string printName, CookSetting setting)
        {
            var _userData = _cacheManager.GetUserDataCacheByUserId(FishSession.UserId);
            var obj = new
            {
                StoreNO = _userData.StorageNo,
                FishNo = _userData.TerminalID,
                orderNo,
            };
            var orderDetail = await _parkDb.ExecuteFunctionAsync<List<OrderDetail>>(ApplicationConsts.QueryOrderDetail, obj);
            if (orderDetail.Count > 0)
            {
                var orderPrint = new OrderPrint
                {
                    flow_no = orderDetail[0].flow_no.ToString(),
                    deskno = orderDetail[0].deskno
                };
                var goodsList = orderDetail.MapTo<List<GoodsPrint>>();
                return SendCookPrint(orderPrint, printName, setting);
            }
            return new Result(ResultCode.Fail, "订单数据为空");
        }

        public Result SendCookPrint(OrderPrint orderPrint, string printName, CookSetting setting)
        {
            PrintModel print = new PrintModel
            {
                PrintName = printName,
                IsCut=setting.IsCut,
                Contents = new List<PrintDetail>
            {
                InlizePrintDetail("0x 1B 21 00", PrintType.CmdWrite)
            }
            };
            var FlowNo = orderPrint.flow_no;
            var operaDate = DateTime.Now.ToLongTimeString();
            var orderStr = strPadright(("订单:" + FlowNo), 25) + strPadleft(operaDate, 8);
            print.Contents.Add(InlizePrintDetail(orderStr, PrintType.WriteLine));
            var deskno = orderPrint.deskno;
            if (!string.IsNullOrWhiteSpace(deskno))
            {
                print.Contents.Add(InlizePrintDetail("0x 1B 21 30", PrintType.CmdWrite));
                print.Contents.Add(InlizePrintDetail(strPadright(("台号:" + deskno), 10), PrintType.WriteLine));
                print.Contents.Add(InlizePrintDetail("0x 1B 21 00", PrintType.CmdWrite));
            }
            if (!string.IsNullOrWhiteSpace(orderPrint.Other1))
            {
                print.Contents.Add(InlizePrintDetail(strPadright("退单:" + orderPrint.Other1, 33), PrintType.WriteLine));
            }
            print.Contents.Add(InlizePrintDetail(strPadright("内容", 27) + strPadleft("数量", 6), PrintType.WriteLine));
            foreach (var item in orderPrint.GoodsList)
            {
                //print.Contents.Add(InlizePrintDetail("0x 1B 21 00", PrintType.CmdWrite));
                var goodsName = item.item_name;
                var quantity = item.sale_qnty;
                string strcmd = strPadright(goodsName) + strPadleft(quantity.ToString("#0"), 6);
                print.Contents.Add(InlizePrintDetail(strcmd, PrintType.WriteLine));
            }
            var result = this._syncService.SyncCookPrint(JsonConvert.SerializeObject(print), setting);
            return result;
        }

        private PrintDetail InlizePrintDetail(string content, PrintType type)
        {
            return new PrintDetail() { Content = content, Type = type };
        }

        /// <summary>
        /// 处理汉字占长度问题
        /// </summary>
        /// <param name="str"></param>
        /// <param name="n"></param>
        /// <returns></returns>
        private string strPadright(string str, int n)
        {

            int len = str.Length;
            int hcount = 0;
            for (int i = 0; i < len; i++)
            {
                int xx = (int)str[i];
                if (xx > 128)
                {
                    hcount++;
                }
            }
            return str.PadRight(n - hcount, ' ');
        }
        /// <summary>
        /// 处理汉字占长度问题
        /// </summary>
        /// <param name="str"></param>
        /// <param name="n"></param>
        /// <returns></returns>
        private string strPadright(string str)
        {
            int n = 27;
            int len = str.Length;
            int hcount = 0;
            for (int i = 0; i < len; i++)
            {
                int xx = (int)str[i];
                if (xx > 128)
                {
                    hcount++;
                }
            }
            if (hcount <= 18)
            {
                if (hcount % 2 == 0)
                {
                    hcount = (int)(hcount * 0.5);
                }
                else
                {
                    hcount = (int)((hcount + 1) * 0.5);
                }
            }
            str = str.PadRight(n - hcount, ' ');
            return str;
        }

        /// <summary>
        /// 处理汉字占长度问题
        /// </summary>
        /// <param name="str"></param>
        /// <param name="n"></param>
        /// <returns></returns>
        private string strPadleft(string str, int n)
        {
            int len = str.Length;
            int hcount = 0;
            for (int i = 0; i < len; i++)
            {
                int xx = (int)str[i];
                if (xx > 128)
                {
                    hcount++;
                }
            }

            return str.PadLeft(n - hcount, ' ');
        }
    }
}
