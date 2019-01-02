using Microsoft.Extensions.Configuration;
using RedLockNet.SERedis;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using RedLockNet.SERedis.Configuration;
using static WishStar.Format.Core.JsonConvert.JsonHelper;

namespace WishStar.Caching.Core.Implementation
{
    public class RedisCacheProvider : ICacheProvider,ILocker
    {
        private static ConnectionMultiplexer _stackExchangeRedis;
        private static IConfiguration _configuration;
        private static string ConnectionString = string.Empty;
        private readonly Lazy<string> _connectionString;
        private static readonly object obj = new object();
        private volatile RedLockFactory _redisLockFactory;

        public RedisCacheProvider(IConfiguration configuration)
        {
            _configuration = configuration;
            ConnectionString = GetConnectionString();
            _connectionString = new Lazy<string>(GetConnectionString);
            this._redisLockFactory = CreateRedisLockFactory();
        }

        protected string GetConnectionString()
        {
            return _configuration["ConnectionString:Redis"];
        }

        public static ConnectionMultiplexer StackExchangeRedis
        {
            get
            {
                if (_stackExchangeRedis != null && _stackExchangeRedis.IsConnected)
                {
                    return _stackExchangeRedis;
                }

                lock (obj)
                {
                    if (_stackExchangeRedis != null && _stackExchangeRedis.IsConnected)
                    {
                        return _stackExchangeRedis;
                    }
                    //存在实例但未连接状态先释放
                    _stackExchangeRedis?.Dispose();

                    _stackExchangeRedis = RedisConnection();
                } 

                return _stackExchangeRedis;
            }
        }

        private static ConnectionMultiplexer RedisConnection()
        {
            if (string.IsNullOrWhiteSpace(ConnectionString))
            {
                ConnectionString = _configuration["ConnectionString:Redis"];
            }
            try
            {
                return ConnectionMultiplexer.Connect(ConnectionString);
            }
            catch (Exception e)
            {
                //log
                return null;
            }
        }

        /// <summary>
        /// 创建redis独占锁
        /// </summary>
        /// <returns></returns>
        protected RedLockFactory CreateRedisLockFactory()
        {
            //get RedLock endpoints
            var configurationOptions = ConfigurationOptions.Parse(_connectionString.Value);
            var redLockEndPoints = StackExchangeRedis.GetEndPoints().Select(endPoint => new RedLockEndPoint
            {
                EndPoint = endPoint,
                Password = configurationOptions.Password,
                Ssl = configurationOptions.Ssl,
                RedisDatabase = configurationOptions.DefaultDatabase,
                ConfigCheckSeconds = configurationOptions.ConfigCheckSeconds,
                ConnectionTimeout = configurationOptions.ConnectTimeout,
                SyncTimeout = configurationOptions.SyncTimeout
            }).ToList();
            
            return RedLockFactory.Create(redLockEndPoints);
        }

        /// <summary>
        /// 清除全部缓存
        /// </summary>
        public async void Clear()
        {
            if (StackExchangeRedis == null)
            {
                return;
            }
            foreach (var endPoint in StackExchangeRedis.GetEndPoints())
            {
                var service = StackExchangeRedis.GetServer(endPoint);
                //获取全部key
                var keys = service.Keys(database: StackExchangeRedis.GetDatabase().Database);
                //排除sessionkey
                keys = keys.Where(key => key.ToString().IndexOf("ASP.NET_SessionId", StringComparison.OrdinalIgnoreCase) > -1);
                //删除
                await StackExchangeRedis.GetDatabase().KeyDeleteAsync(keys.ToArray());
            }
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public T Get<T>(string key)
        {
            if (StackExchangeRedis == null)
            {
                return default(T);
            }
            var db = StackExchangeRedis.GetDatabase();
            string val = db.StringGetAsync(key).Result;
            T result = default(T);
            if (val != null)
            {
                result = DeserializeObject<T>(val);
            }
            return result;
        }

        public bool IsExist(string key)
        {
            if (StackExchangeRedis == null)
            {
                return false;
            }
            var db = StackExchangeRedis.GetDatabase();
            return db.KeyExistsAsync(key).Result;

        }

        public bool KeyExpire(string key, DateTime? expiry)
        {
            if (StackExchangeRedis == null)
            {
                return false;
            }
            var db = StackExchangeRedis.GetDatabase();

            return db.KeyExpireAsync(key, expiry).Result;
        }

        public Dictionary<string, T> MGet<T>(IEnumerable<string> keys)
        {
            if (StackExchangeRedis == null || keys == null)
            {
                return null;
            }
            var db = StackExchangeRedis.GetDatabase();
            List<RedisKey> redisKeys = keys.Select(x => (RedisKey)x).ToList();
            RedisValue[] values =  db.StringGetAsync(redisKeys.ToArray()).Result;
            Dictionary<string, T> dic = new Dictionary<string, T>();
            if (values != null)
            {
                for (int i = 0; i < keys.Count(); i++)
                {
                    if (values[i].HasValue)
                    {
                        T result = default(T);
                        if (!values[i].IsNull)
                        {
                            result = DeserializeObject<T>(values[i]);
                        }
                        dic.Add(keys.ElementAt(i), result);
                    }
                }
            }
            return dic;
        }

        public async void Remove(string key)
        {
            if (StackExchangeRedis == null)
            {
                return;
            }
            var db = StackExchangeRedis.GetDatabase();
            await db.KeyDeleteAsync(key);
        }

        public async void RemoveByPattern(string pattern)
        {
            if (StackExchangeRedis == null)
            {
                return;
            }
            foreach (var endPoint in StackExchangeRedis.GetEndPoints())
            {
                var service = StackExchangeRedis.GetServer(endPoint);
                //获取全部key
                var keys = service.Keys(database: StackExchangeRedis.GetDatabase().Database, pattern: $"*{pattern}*");
                //排除sessionkey
                keys = keys.Where(key => key.ToString().IndexOf("ASP.NET_SessionId", StringComparison.OrdinalIgnoreCase) > -1);
                //删除
                await StackExchangeRedis.GetDatabase().KeyDeleteAsync(keys.ToArray());
            }
        }

        public async void Set(string key, object data, int cacheTime)
        {
            if (StackExchangeRedis == null)
            {
                return;
            }
            string value = null;
            var db = StackExchangeRedis.GetDatabase();
            if(data is string)
            {
                value = data as string;
            }
            else
            {
                value = SerializeObject(data);
            }
            await db.StringSetAsync(key, value, TimeSpan.FromMinutes(cacheTime));
        }

        public void Set(string key, object data, DateTime expiresAt)
        {
            int cacheTime = (int)expiresAt.Subtract(DateTime.Now).TotalMinutes;
            this.Set(key, data, cacheTime);
        }

        public string StringGet(string key)
        {
            if (StackExchangeRedis == null)
            {
                return null;
            }

            var db = StackExchangeRedis.GetDatabase();

            return db.StringGetAsync(key).Result;
        }

        public bool PerformActionWithLock(string key, TimeSpan expirationTime, Action action)
        {
            using (var redisLock = _redisLockFactory.CreateLock(key, expirationTime))
            {
                if (!redisLock.IsAcquired)
                {
                    return false;
                }

                action.Invoke();
                return true;
            }
        }
    }
}
