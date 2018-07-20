using Abp.EntityFrameworkCore;
using Abp.EntityFrameworkCore.Configuration;
using Abp.Modules;
using Abp.Reflection.Extensions;
using Castle.MicroKernel.Registration;
using Mi.Fish.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Mi.Fish.EntityFramework
{
    [DependsOn(typeof(FishCoreModule), typeof(AbpEntityFrameworkCoreModule))]
    public class EntityFrameworkModule : AbpModule
    {
        /// <summary>
        /// This is the first event called on application startup.
        /// Codes can be placed here to run before dependency injection registrations.
        /// </summary>
        public override void PreInitialize()
        {
            IocManager.Register<DbContextOptionsFactory>();

            IocManager.IocContainer.Register(Component.For<DbContextOptions<ParkDbContext>>()
                .UsingFactory<DbContextOptionsFactory, DbContextOptions<ParkDbContext>>(factory =>
                    factory.GetDbContextOptions<ParkDbContext>())
                .LifestyleSingleton());

            IocManager.IocContainer.Register(Component.For<DbContextOptions<LocalDbContext>>()
                .UsingFactory<DbContextOptionsFactory, DbContextOptions<LocalDbContext>>(factory =>
                    factory.GetDbContextOptions<LocalDbContext>())
                .LifestyleTransient());

            Configuration.Modules.AbpEfCore().AddDbContext<ParkDbContext>(configuration =>
            {
                if (configuration.ExistingConnection != null)
                {
                    configuration.DbContextOptions.UseSqlServer(configuration.ExistingConnection);
                }
                else
                {
                    var connProvider = IocManager.Resolve<IDbConnectionProvider>();

                    configuration.DbContextOptions.UseSqlServer(connProvider.GetConnectionString<ParkDbContext>());
                }
            });

            Configuration.Modules.AbpEfCore().AddDbContext<LocalDbContext>(configuration =>
            {
                if (configuration.ExistingConnection != null)
                {
                    configuration.DbContextOptions.UseSqlServer(configuration.ExistingConnection);
                }
                else
                {
                    var connProvider = IocManager.Resolve<IDbConnectionProvider>();

                    configuration.DbContextOptions.UseSqlServer(connProvider.GetConnectionString<LocalDbContext>());
                }
            });
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(typeof(EntityFrameworkModule).GetAssembly());
        }

        public void RegisterDbContextOptions<TDbContext>(DbContextOptions<TDbContext> options) where TDbContext : DbContext
        {
            IocManager.IocContainer.Register(Component.For<DbContextOptions<TDbContext>>().Instance(options).LifestyleSingleton());
        }

    }
}
