using System;
using Abp.AspNetCore;
using Abp.AspNetCore.Configuration;
using Abp.Localization;
using Abp.Modules;
using Abp.Reflection.Extensions;
using Abp.Runtime.Caching.Redis;
using Castle.MicroKernel.Lifestyle;
using Castle.MicroKernel.Registration;
using Castle.Windsor.MsDependencyInjection;
using Mi.Fish.Application;
using Mi.Fish.EntityFramework;
using Mi.Fish.Infrastructure.Api;
using Mi.Fish.Infrastructure.Redis;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Mi.Fish.Api
{
    [DependsOn(typeof(FishApplicationModule),
        typeof(AbpAspNetCoreModule),
        typeof(AbpRedisCacheModule),
        typeof(InfrastructureApiModule))]
    public class FishApiModule : AbpModule
    {
        /// <summary>
        /// This is the first event called on application startup.
        /// Codes can be placed here to run before dependency injection registrations.
        /// </summary>
        public override void PreInitialize()
        {
            var configuration = IocManager.Resolve<Microsoft.Extensions.Configuration.IConfiguration>();
            Configuration.Caching.UseRedis(options => options.ConnectionString = configuration.GetConnectionString(RedisDatabaseProvider.ConnectionStringKey));

            IocManager.Register<IDbConnectionProvider, DbConnectionProvider>();
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(typeof(FishApiModule).GetAssembly());
        }

        /// <summary>This method is called lastly on application startup.</summary>
        public override void PostInitialize()
        {
            //Configuration.Localization.Languages.Add(new LanguageInfo("zh-Hans", "中文", isDefault: true));
            //Configuration.Localization.Languages.Add(new LanguageInfo("en", "English"));
        }   
    }
}
