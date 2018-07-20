using System.Linq;
using System.Threading.Tasks;
using Abp.Runtime.Caching;
using Mi.Fish.Application;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace Mi.Fish.Api.JwtBearer
{
    /// <summary>
    /// 权限授权Handler
    /// </summary>
    public class AuthHandler : AuthorizationHandler<AuthRequirement>
    {
        private readonly ICacheManager _cacheManager;

        /// <summary>
        /// 验证方案提供对象
        /// </summary>
        public IAuthenticationSchemeProvider Schemes { get; set; }
        /// <summary>
        /// 自定义策略参数
        /// </summary>
        public AuthRequirement Requirement { get; set; }
        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="schemes"></param>
        public AuthHandler(IAuthenticationSchemeProvider schemes, ICacheManager cacheManager)
        {
            Schemes = schemes;
            _cacheManager = cacheManager;
        }

        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, AuthRequirement requirement)
        {
            var httpContext = (context.Resource as Microsoft.AspNetCore.Mvc.Filters.AuthorizationFilterContext)?.HttpContext;

            //判断请求是否停止
            var handlers = httpContext?.RequestServices.GetRequiredService<IAuthenticationHandlerProvider>();
            foreach (var scheme in await Schemes.GetRequestHandlerSchemesAsync())
            {
                var handler = await handlers.GetHandlerAsync(httpContext, scheme.Name) as IAuthenticationRequestHandler;
                if (handler != null && await handler.HandleRequestAsync())
                {
                    context.Fail();
                    return;
                }
            }
            //判断请求是否拥有凭据，即有没有登录
            var defaultAuthenticate = await Schemes.GetDefaultAuthenticateSchemeAsync();
            if (defaultAuthenticate != null)
            {
                var result = await httpContext.AuthenticateAsync(defaultAuthenticate.Name);
                //result?.Principal不为空即登录成功
                if (result?.Principal != null)
                {
                    var userId = result.Principal.Identity.Name;
                    var userTokenCache = _cacheManager.GetUserTokenCache(userId);
                    var token = httpContext?.Request.Headers["Authorization"].FirstOrDefault()?.Split(' ')[1];

                    if (!string.IsNullOrEmpty(userTokenCache) && userTokenCache == token)
                    {
                        //验证成功续期至2小时
                        _cacheManager.SetUserTokenCache(userId, token);
                        context.Succeed(requirement);
                        return;
                    }
                }
            }
            context.Fail();
            return;
        }
    }
}
