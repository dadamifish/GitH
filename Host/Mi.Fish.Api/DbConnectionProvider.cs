using System;
using System.IO;
using Abp.Runtime.Caching;
using Abp.UI;
using Mi.Fish.Api.Controllers;
using Mi.Fish.Application;
using Mi.Fish.ApplicationDto;
using Mi.Fish.EntityFramework;
using Mi.Fish.Infrastructure.Session;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace Mi.Fish.Api
{
    public class DbConnectionProvider : IDbConnectionProvider
    {
        private readonly DbOptions _localDbOptions;

        private readonly IHttpContextAccessor _httpContextAccessor;

        private readonly IUrlHelper _urlHelper;

        public DbConnectionProvider(IOptions<DbOptions> options, IHttpContextAccessor httpContextAccessor, IUrlHelper urlHelper)
        {
            _httpContextAccessor = httpContextAccessor;
            _urlHelper = urlHelper;
            _localDbOptions = options.Value;
        }

        public IFishSession FishSession { get; set; }

        /// <summary>
        /// Gets the connection string.
        /// </summary>
        /// <typeparam name="TDbContext">The type of the database context.</typeparam>
        /// <returns></returns>
        /// <exception cref="Exception">没有为该机器配置数据库连接。</exception>
        public string GetConnectionString<TDbContext>()
        {
            if (typeof(TDbContext) == typeof(ParkDbContext))
            {
                return _localDbOptions.ParkDbConnString;
            }
            else
            {
                string dbKey = string.Empty;

                if (FishSession.UserId == null)
                {
                    var loginUrl = _urlHelper.RouteUrl(UserController.LoginRouteName, new { });
                    var context = _httpContextAccessor.HttpContext;

                    //由于系统原因这里用url判断，如果通用可以改为AllowAnonymous属性判断
                    if (context.Request.Path.HasValue && loginUrl.Equals(context.Request.Path.Value))
                    {
                        context.Request.EnableRewind();

                        using (var stream = new MemoryStream())
                        {
                            context.Request.Body.CopyTo(stream);

                            stream.Seek(0, SeekOrigin.Begin);
                            using (var reader = new StreamReader(stream))
                            {
                                var content = reader.ReadToEnd();
                                var input = JsonConvert.DeserializeObject<UserLoginInput>(content);
                                dbKey = input.StorageNo + input.FishNo;
                            }
                        }
                        
                        context.Request.Body.Seek(0, SeekOrigin.Begin);
                    }
                }
                else
                {
                    dbKey = FishSession.StorageNo + FishSession.TerminalId;
                }

                if (!_localDbOptions.LocalDbConnStrings.ContainsKey(dbKey))
                {
                    throw new UserFriendlyException("没有为该机器配置数据库连接。");
                }

                return _localDbOptions.LocalDbConnStrings[dbKey];
            }
        }
    }
}
