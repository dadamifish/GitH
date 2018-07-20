using Mi.Fish.Infrastructure.Results;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Mi.Fish.Application.Sync
{
    /// <summary>
    /// 数据同步接口
    /// </summary>
    public interface ISyncAppService
    {
        /// <summary>
        /// 数据同步接口
        /// </summary>
        /// <param name="storageNo">分店编码</param>
        /// <param name="terminalID">机号</param>
        /// <param name="wait">是否等待执行完成--登录、交班之前必须等待完成，下单不等待</param>
        /// <returns></returns>
        Result SyncData(string storageNo, string terminalID,bool wait);
        Result SyncCookPrint(string sendStr,CookSetting setting);

    }
}
