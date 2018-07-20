using StackExchange.Redis;

namespace Mi.Fish.Infrastructure.Redis
{
    public interface IRedisSubscriberProvider
    {
        ISubscriber GetSubscriber(object asyncState = null);
    }
}
