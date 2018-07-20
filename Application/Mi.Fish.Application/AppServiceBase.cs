using Abp.Application.Services;
using Mi.Fish.Infrastructure;
using Mi.Fish.Infrastructure.Session;

namespace Mi.Fish.Application
{
    public abstract class AppServiceBase : ApplicationService
    {
        public IFishSession FishSession { get; set; }

        protected AppServiceBase()
        {
            LocalizationSourceName = InfrastructureConsts.LocalizationSourceName;
        }
    }
}
