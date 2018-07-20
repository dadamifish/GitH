using Abp;
using Abp.Modules;
using Abp.Reflection.Extensions;

namespace Mi.Fish.Infrastructure
{
    [DependsOn(typeof(AbpKernelModule))]
    public class InfrastructureModule : AbpModule
    {
        /// <summary>
        /// This is the first event called on application startup. 
        /// Codes can be placed here to run before dependency injection registrations.
        /// </summary>
        public override void PreInitialize()
        {

        }

        /// <summary>
        /// This method is used to register dependencies for this module.
        /// </summary>
        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(typeof(InfrastructureModule).GetAssembly());
        }

        /// <summary>
        /// This method is called lastly on application startup.
        /// </summary>
        public override void PostInitialize()
        {

        }
    }
}
