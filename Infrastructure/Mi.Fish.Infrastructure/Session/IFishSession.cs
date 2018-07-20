using System;
using Abp.Runtime.Session;

namespace Mi.Fish.Infrastructure.Session
{
    public interface IFishSession
    {
        /// <summary>
        /// 当前用户ID
        /// </summary>
        /// <value>
        /// The user identifier.
        /// </value>
        string UserId { get; }

        /// <summary>
        /// 门店号
        /// </summary>
        /// <value>
        /// The storage no.
        /// </value>
        string StorageNo { get; }

        /// <summary>
        /// 机器号
        /// </summary>
        /// <value>
        /// The terminal identifier.
        /// </value>
        string TerminalId { get; }

        /// <summary>
        /// 设置临时门店号和机器号
        /// </summary>
        /// <param name="storageNo">The storage no.</param>
        /// <param name="terminalId">The terminal identifier.</param>
        IDisposable UseStoreAndTerminal(string storageNo, string terminalId);
    }
}
