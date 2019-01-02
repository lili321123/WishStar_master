using System;
using System.Collections.Generic;
using System.Text;

namespace WishStar.Caching.Core
{
    public interface ILocker
    {
        /// <summary>
        /// 使用独占锁执行操作
        /// </summary>
        /// <param name="key"></param>
        /// <param name="expirationTime">锁过期时间</param>
        /// <param name="action">操作</param>
        /// <returns></returns>
        bool PerformActionWithLock(string key, TimeSpan expirationTime, Action action);
    }
}
