using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace WishStar.Caching.Core.Implementation
{
    public class PerRequestCacheProvider:ICacheProvider
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public PerRequestCacheProvider(IHttpContextAccessor httpContextAccessor)
        {
            this._httpContextAccessor = httpContextAccessor;
        }

        protected IDictionary<object, object> GetItems()
        {
            return this._httpContextAccessor.HttpContext?.Items;
        }


        public void Clear()
        {
            var items = this.GetItems();
            items?.Clear();
        }

        public void Dispose()
        {
        }

        public T Get<T>(string key)
        {
            var items = this.GetItems();
            return (T)items?[key];
        }

        public bool IsExist(string key)
        {
            var items = this.GetItems();

            return items?[key] != null;
        }

        public bool KeyExpire(string key, DateTime? expiry)
        {
            throw new NotImplementedException();
        }

        public Dictionary<string, T> MGet<T>(IEnumerable<string> keys)
        {
            Dictionary<string, T> dic = new Dictionary<string, T>();
            foreach (var key in keys)
            {
                var val = this.Get<T>(key);
                if (!dic.ContainsKey(key) && !EqualityComparer<T>.Default.Equals(val, default(T)))
                {
                    dic.Add(key, val);
                }
            }
            return dic;
        }

        public void Remove(string key)
        {
            var items = this.GetItems();

            items?.Remove(key);
        }

        public void RemoveByPattern(string pattern)
        {
            var items = GetItems();
            if (items == null)
                return;
            
            var regex = new Regex(pattern, RegexOptions.Singleline | RegexOptions.Compiled | RegexOptions.IgnoreCase);
            var matchesKeys = items.Keys.Where(key => regex.IsMatch(key.ToString())).ToList();
            
            foreach (var key in matchesKeys)
            {
                items.Remove(key);
            }
        }

        public void Set(string key, object data, int cacheTime)
        {
            var items = GetItems();
            if (items == null)
                return;

            if (data != null)
                items[key] = data;
        }

        public void Set(string key, object data, DateTime expiresAt)
        {
            this.Set(key, data, 0);
        }

        public string StringGet(string key)
        {
            return this.Get<string>(key);
        }



    }
}
