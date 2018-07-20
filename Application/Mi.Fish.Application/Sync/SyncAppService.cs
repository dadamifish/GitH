using Abp.Runtime.Caching;
using Abp.UI;
using Mi.Fish.Application.Order;
using Mi.Fish.Application.SaleMenu;
using Mi.Fish.ApplicationDto;
using Mi.Fish.Common;
using Mi.Fish.EntityFramework;
using Mi.Fish.Infrastructure.Results;
using Microsoft.Extensions.Options;
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

namespace Mi.Fish.Application.Sync
{
    /// <summary>
    /// 数据同步
    /// </summary>
    public class SyncAppService : AppServiceBase, ISyncAppService
    {
        private readonly ParkDbContext _parkDbContext;

        private readonly ICacheManager _cacheManager;
        /// <summary>
        /// 
        /// </summary>
        public SyncAppService(ParkDbContext parkDbContext,ICacheManager cacheManager)
        {
            _parkDbContext = parkDbContext;
            _cacheManager = cacheManager;
        }

        /// <summary>
        /// 数据同步接口
        /// </summary>
        /// <param name="storageNo">分店编码</param>
        /// <param name="terminalID">机号</param>
        /// <param name="wait">是否等待执行完成--登录、交班之前必须等待完成，下单不等待</param>
        /// <returns></returns>
        public Result SyncData(string storageNo, string terminalID,bool wait)
        {
            try
            {
                var path = "";
                var middlePath = "";

                string userId = FishSession.UserId;

                if (string.IsNullOrWhiteSpace(userId))
                {
                    string sql = "select kindid ,kindname ,kindvalue,branchno  from ft_zdb  where (kindid = 27 or kindid=28) and branchno='"+ storageNo + terminalID + "'";

                    DataSet syncDs = _parkDbContext.GetDataSet(sql, new object());

                    if (syncDs.Tables.Count != 0 && syncDs.Tables[0].Rows.Count > 0)
                    {
                        DataTable dt = syncDs.Tables[0];

                        middlePath = dt.GetValue("kindid=27");
                        path = dt.GetValue("kindid=28");
                    }
                    else
                    {
                        Logger.Error("同步异常：" + "未配置同步程序地址");
                        return Result.Fail<string>(ResultCode.Fail, "同步失败：请联系管理员配置同步程序目录");
                    }
                }
                else
                {
                    var userData = _cacheManager.GetUserDataCacheByUserId(userId);
                    path = userData.FishDataSyncPath;
                    middlePath = userData.SyncDataPath;
                }
             
                if (wait)
                {
                    System.Diagnostics.Process.Start(middlePath, path).WaitForExit();
                }
                else
                {
                    var process = System.Diagnostics.Process.Start(middlePath, path);
                }
                return Result.Ok;
            }
            catch (Exception ex)
            {
                Logger.Error("同步异常：" + ex.Message);
                return Result.Fail<string>(ResultCode.Fail, "同步失败：请联系管理员检查同步程序地址配置是否正确 " + ex.Message);
            }

        }

        public Result SyncCookPrint(string sendStr, CookSetting setting)
        {
            Logger.Info("调用发送厨打");
            try
            {
                TcpHelper.Sendstring(sendStr, setting.CookPrintIP, setting.CookPort);
            }
            catch (Exception ex)
            {
                Logger.Error("发送厨打报错失败：" + ex);
                return Result.Fail<string>(ResultCode.Fail, "厨打数据失败：请联系管理员检查同步程序地址配置是否正确 " + ex.Message);
            }
            return Result.Ok;
        }

    }
}
