using Abp;
using Abp.Modules;
using Abp.Reflection.Extensions;

namespace Mi.Fish.Infrastructure.Api
{
    [DependsOn(typeof(InfrastructureModule))]
    public class InfrastructureApiModule : AbpModule
    {
        /// <summary>
        /// This method is used to register dependencies for this module.
        /// </summary>
        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(typeof(InfrastructureApiModule).GetAssembly());
        }
    }
}
