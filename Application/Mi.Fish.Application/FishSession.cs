using System;
using System.Linq;
using System.Security.Claims;
using Abp.Configuration.Startup;
using Abp.Dependency;
using Abp.MultiTenancy;
using Abp.Runtime;
using Abp.Runtime.Session;
using Mi.Fish.Infrastructure.Session;

namespace Mi.Fish.Application
{
    /// <summary>
    /// 扩展AbpSession
    /// </summary>
    public class FishSession : IFishSession, ISingletonDependency
    {
        private readonly IPrincipalAccessor _principalAccessor;

        private readonly IAmbientScopeProvider<StoreAndTerminalOverride> _ambientScopeProvider;

        public FishSession(IPrincipalAccessor principalAccessor,
            IAmbientScopeProvider<StoreAndTerminalOverride> sessionOverrideScopeProvider)
        {
            _principalAccessor = principalAccessor;
            _ambientScopeProvider = sessionOverrideScopeProvider;
        }

        /// <summary>
        /// 当前用户ID
        /// </summary>
        /// <value>
        /// The user identifier.
        /// </value>
        public string UserId =>
            _principalAccessor.Principal?.Claims.FirstOrDefault(o => o.Type == ClaimTypes.Name)?.Value;

        /// <summary>
        /// 门店号
        /// </summary>
        /// <value>
        /// The storage no.
        /// </value>
        public string StorageNo
        {
            get
            {
                return StoreAndTerminalOverride != null ? StoreAndTerminalOverride.StorageNo : 
                    _principalAccessor.Principal?.Claims.FirstOrDefault(o => o.Type == FishClaimTypes.StorageNo)?.Value;
            }
        }
    

        /// <summary>
        /// 机器号
        /// </summary>
        /// <value>
        /// The terminal identifier.
        /// </value>
        public string TerminalId {
            get
            {
                return StoreAndTerminalOverride != null ? StoreAndTerminalOverride.TerminalId :
                    _principalAccessor.Principal?.Claims.FirstOrDefault(o => o.Type == FishClaimTypes.TerminalId)?.Value;
            }
        }

        private StoreAndTerminalOverride StoreAndTerminalOverride =>
            _ambientScopeProvider.GetValue(UseStoreAndTerminalKey);

        public const string UseStoreAndTerminalKey = "FishSession.UseStoreAndTerminal";

        /// <summary>
        /// 设置临时门店号和机器号
        /// </summary>
        /// <param name="storageNo">The storage no.</param>
        /// <param name="terminalId">The terminal identifier.</param>
        public IDisposable UseStoreAndTerminal(string storageNo, string terminalId)
        {
            return _ambientScopeProvider.BeginScope(UseStoreAndTerminalKey, new StoreAndTerminalOverride(storageNo, terminalId));
        }
    }
}
