using System;
using System.Collections.Generic;
using System.Text;

namespace WishStar.Caching.Core
{
    public interface ICacheProvider: IDisposable
    {
        /// <summary>
        /// 根据key获取字符串缓存值
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        string StringGet(string key);

        /// <summary>
        /// 根据key获取缓存
        /// </summary>
        /// <typeparam name="T">数据类型</typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        T Get<T>(string key);

        /// <summary>
        /// 根据key批量获取缓存
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="keys"></param>
        /// <returns></returns>
        Dictionary<string, T> MGet<T>(IEnumerable<string> keys);

        /// <summary>
        /// 写入缓存
        /// </summary>
        /// <param name="key">缓存键</param>
        /// <param name="data">缓存数据</param>
        /// <param name="cacheTime">缓存时间（m）</param>
        void Set(string key, object data, int cacheTime);

        /// <summary>
        /// 写入缓存
        /// </summary>
        /// <param name="key">缓存键</param>
        /// <param name="data">缓存数据</param>
        /// <param name="expiresAt">过期时间</param>
        void Set(string key, object data, DateTime expiresAt);

        /// <summary>
        /// 判断缓存是否存在
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        bool IsExist(string key);

        /// <summary>
        /// 根据key移除缓存
        /// </summary>
        /// <param name="key"></param>
        void Remove(string key);

        /// <summary>
        /// 根据正则匹配移除缓存
        /// </summary>
        /// <param name="pattern"></param>
        void RemoveByPattern(string pattern);

        /// <summary>
        /// 清除全部缓存
        /// </summary>
        void Clear();

        /// <summary>
        /// 延长缓存过期时间
        /// </summary>
        /// <param name="key"></param>
        /// <param name="expiry"></param>
        /// <returns></returns>
        bool KeyExpire(string key, DateTime? expiry);
    }
}
