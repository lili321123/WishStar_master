using System;
using System.Collections.Generic;
using System.Text;

namespace WishStar.Caching.Core.Common
{
    public sealed class CacheKeyCode
    {
        public CacheKeyCode(string keyName, string cacheType
            , int cacheTime = 60, int versionId = 1)
        {
            this.KeyName = keyName;
            this.CacheType = cacheType;
            this.VersionId = versionId;
            this.CacheTime = cacheTime;
        }

        public string KeyName { get; set; }

        public string CacheType { get; set; }

        public int CacheTime { get; set; }

        public int VersionId { get; set; }

        public override string ToString()
        {
            return $"v_{VersionId}_{CacheType}_{KeyName}";
        }
    }
}
