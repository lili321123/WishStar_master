using System;
using System.Collections.Generic;
using System.Text;

namespace WishStar.Web.Framework.Engine.Implementation
{
    public class CommonEngine : IEngine
    {
        #region 字段

        #endregion

        #region 属性

        #endregion


        #region 方法

        #endregion
        public T Resolve<T>()
        {
            throw new NotImplementedException();
        }

        public T Resolve<T>(string name)
        {
            throw new NotImplementedException();
        }

        public T[] ResolveAll<T>()
        {
            throw new NotImplementedException();
        }

        public T[] ResolveAll<T>(string name)
        {
            throw new NotImplementedException();
        }
    }
}
