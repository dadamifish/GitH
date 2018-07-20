using System;
using Abp.Dependency;
using Abp.Runtime.Caching.Redis;
using StackExchange.Redis;

namespace Mi.Fish.Infrastructure.Redis
{
    /// <summary>
    /// Implements <see cref="IAbpRedisCacheDatabaseProvider"/>.
    /// </summary>
    public class RedisDatabaseProvider : IAbpRedisCacheDatabaseProvider, IRedisSubscriberProvider, ISingletonDependency
    {
        public const string ConnectionStringKey = "Abp.Redis.Cache";

        private readonly AbpRedisCacheOptions _options;
        private readonly Lazy<ConnectionMultiplexer> _connectionMultiplexer;

        /// <summary>
        /// Initializes a new instance of the <see cref="AbpRedisCacheDatabaseProvider"/> class.
        /// </summary>
        public RedisDatabaseProvider(AbpRedisCacheOptions options)
        {
            _options = options;
            _connectionMultiplexer = new Lazy<ConnectionMultiplexer>(CreateConnectionMultiplexer);
        }

        /// <summary>
        /// Gets the database connection.
        /// </summary>
        public IDatabase GetDatabase()
        {
            return _connectionMultiplexer.Value.GetDatabase(_options.DatabaseId);
        }

        private ConnectionMultiplexer CreateConnectionMultiplexer()
        {
            return ConnectionMultiplexer.Connect(_options.ConnectionString);
        }

        public ISubscriber GetSubscriber(object asyncState = null)
        {
            return _connectionMultiplexer.Value.GetSubscriber();
        }
    }
}
