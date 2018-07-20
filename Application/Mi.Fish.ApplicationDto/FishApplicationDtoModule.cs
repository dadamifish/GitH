using Abp.AutoMapper;
using Abp.Modules;
using Abp.Reflection.Extensions;
using Mi.Fish.Core;

namespace Mi.Fish.ApplicationDto
{
    [DependsOn(typeof(FishCoreModule), typeof(AbpAutoMapperModule))]
    public class FishApplicationDtoModule : AbpModule
    {
        /// <summary>
        /// This is the first event called on application startup.
        /// Codes can be placed here to run before dependency injection registrations.
        /// </summary>
        public override void PreInitialize()
        {
            Configuration.Modules.AbpAutoMapper().Configurators.Add(AutoMapperConfiguration.Config);
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(typeof(FishApplicationDtoModule).GetAssembly());
        }
    }
}
