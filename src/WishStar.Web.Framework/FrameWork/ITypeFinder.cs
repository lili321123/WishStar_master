using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace WishStar.Web.Framework.FrameWork
{
    public interface ITypeFinder
    {
        /// <summary>
        /// 获取全部加载的程序集
        /// </summary>
        /// <returns></returns>
        IList<Assembly> GetAssemblies();

        /// <summary>
        /// 获取满足指定类型的class
        /// </summary>
        /// <param name="assTypeFrom"></param>
        /// <param name="isOnlyClass"></param>
        /// <returns></returns>
        IEnumerable<Type> FindClassesOfType(Type assTypeFrom, bool isOnlyClass);
        IEnumerable<Type> FindClassesOfType(Type assTypeFrom, IEnumerable<Assembly> assemblies, bool isOnlyClass);
        IEnumerable<Type> FindClassesOfType<T>(bool isOnlyClass);
        IEnumerable<Type> FindClassesOfType<T>(IEnumerable<Assembly> assemblies, bool isOnlyClass);
    }
}
