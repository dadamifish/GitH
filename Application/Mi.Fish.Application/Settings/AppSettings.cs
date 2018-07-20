using System;
using System.Collections.Generic;
using System.Text;

namespace Mi.Fish.Application
{
    /// <summary>
    /// 配置
    /// </summary>
    public class AppSettings
    {
        /// <summary>
        /// 数据同步目录
        /// </summary>
        public string SyncProgramDirectory { get; set; }
        /// <summary>
        /// 厨打IP
        /// </summary>
        public string CookPrintIP { get; set; }
        /// <summary>
        /// 厨打端口
        /// </summary>
        public int CookPort { get; set; }
        public Dictionary<string, CookSetting> CookSetting { get; set; }

    }
}
