using Abp.Localization.Dictionaries;
using Abp.Localization.Dictionaries.Xml;
using Abp.Modules;
using Abp.Reflection.Extensions;
using Mi.Fish.Infrastructure;


namespace Mi.Fish.Core
{
    [DependsOn(typeof(InfrastructureModule))]
    public class FishCoreModule : AbpModule
    {
        /// <summary>
        /// This is the first event called on application startup.
        /// Codes can be placed here to run before dependency injection registrations.
        /// </summary>
        public override void PreInitialize()
        {
            Configuration.Localization.Sources.Add(new DictionaryBasedLocalizationSource(
                InfrastructureConsts.LocalizationSourceName,
                new XmlEmbeddedFileLocalizationDictionaryProvider(typeof(FishCoreModule).GetAssembly(),
                    "Mi.Fish.Core.Localization.SourceFiles")));
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(typeof(FishCoreModule).GetAssembly());
        }
    }
}
