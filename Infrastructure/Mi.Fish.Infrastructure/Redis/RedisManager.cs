using System;
using System.Collections.Generic;
using System.Text;
using StackExchange.Redis;

namespace Mi.Fish.Infrastructure
{
    public class RedisManager
    {
        private RedisManager() { }
        private static ConnectionMultiplexer instance;
        private static readonly object locker = new object();
        public static string ConnectionString { get; set; }
        /// <summary>
        /// 单例模式获取redis连接实例
        /// </summary>
        public static ConnectionMultiplexer Instance
        {
            get
            {
                lock (locker)
                {
                    if (instance == null)
                    {
                        if (instance == null)
                            instance = ConnectionMultiplexer.Connect(ConnectionString);

                    }
                }
                return instance;
            }
        }
    }
}
