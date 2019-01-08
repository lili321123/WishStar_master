using System;
using System.Collections.Generic;
using System.Text;

namespace WishStar.Web.Framework.Data
{
    public abstract class BaseModel
    {
        /// <summary>
        /// 获取唯一键
        /// </summary>
        /// <returns></returns>
        public abstract object GetKey();
    }
}
