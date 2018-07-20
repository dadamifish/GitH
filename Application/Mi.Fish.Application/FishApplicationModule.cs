using Abp.Modules;
using Abp.Reflection.Extensions;
using Mi.Fish.ApplicationDto;
using Mi.Fish.EntityFramework;
using System;

namespace Mi.Fish.Application
{
    [DependsOn(typeof(EntityFrameworkModule), typeof(FishApplicationDtoModule))]
    public class FishApplicationModule : AbpModule
    {
        public override void PreInitialize()
        {
            Configuration.Caching.Configure("LeyouTokenKey", cache =>
            {
                cache.DefaultAbsoluteExpireTime = TimeSpan.FromDays(7);
            });
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(typeof(FishApplicationModule).GetAssembly());
        }
    }
}
