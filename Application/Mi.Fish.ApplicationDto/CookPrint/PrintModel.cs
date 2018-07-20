using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mi.Fish.ApplicationDto
{
    /// <summary>
    /// 打印model
    /// </summary>
    public class PrintModel
    {
        /// <summary>
        /// 打印机名称
        /// </summary>
        public string PrintName { get; set; }
        /// <summary>
        /// 是否切纸
        /// </summary>
        public int IsCut { get; set; }
        /// <summary>
        /// 打印详情
        /// </summary>
        public List<PrintDetail> Contents { get; set; }
    }
    /// <summary>
    /// 打印详情
    /// </summary>
    public class PrintDetail
    {
        /// <summary>
        /// 打印内容
        /// </summary>
        public string Content { get; set; }
        /// <summary>
        /// 打印类型
        /// </summary>
        public PrintType Type { get; set; }
    }
    /// <summary>
    /// 打印详情
    /// </summary>
    public enum PrintType
    {
        /// <summary>
        /// 打印字符窜一行
        /// </summary>
        WriteLine = 1,
        /// <summary>
        /// 打印一条指令
        /// </summary>
        CmdWrite = 2,
    }
}
