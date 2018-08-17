using System;
using StackExchange.Redis;

namespace ServiceSharedCore
{
    public static class RedisCacheManager
    {
        private static ConnectionMultiplexer connection;
        private static IDatabase cache;
        private static int DefaultCacheDuration = 360;
        public static bool Initialized { get; private set; }
        public static void Initialize(string connStr, int defaultCacheDuration = 360)
        {
            try
            {
                connection = ConnectionMultiplexer.Connect(connStr);
                cache = connection.GetDatabase();
                Initialized = true;
                DefaultCacheDuration = defaultCacheDuration;
            }
            catch
            {
                Initialized = false;
            } 
        }

        public static void Store(string key, object content, int? duration = null)
        {
            if (!Initialized) return;

            string toStore;
            if (content is string)
            {
                toStore = (string)content;
            }
            else
            {
                toStore = Newtonsoft.Json.JsonConvert.SerializeObject(content);
            }
            if (duration == null) duration = DefaultCacheDuration;

            cache.StringSet(key, toStore, new TimeSpan(0, 0, duration.Value));
        }

        public static T Get<T>(string key) where T : class
        {
            if (!Initialized)
                return null;
            try
            {
                var fromCache = cache.StringGet(key);
                if (!fromCache.HasValue)
                    return null;

                string str = fromCache.ToString();

                if (typeof(T) == typeof(string))
                    return str as T;
                return Newtonsoft.Json.JsonConvert.DeserializeObject<T>(str);
            }
            catch
            {
                return null;
            }
        }

        public static void Invalidate(string key)
        {
            if (connection != null)
                cache.KeyDelete(key);
        }
    }
}
