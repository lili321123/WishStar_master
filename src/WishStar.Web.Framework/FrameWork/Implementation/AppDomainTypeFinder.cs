using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

namespace WishStar.Web.Framework.FrameWork.Implementation
{
    public class AppDomainTypeFinder: ITypeFinder
    {
        #region 字段

        private bool ignoreReflectionErrors = true;
        private bool loadAppDomainAssemblies = true;
        /// <summary>
        /// 默认排除的程序集
        /// </summary>
        private string assemblySkipLoadingPattern = @"^System|^mscorlib|^Microsoft|^AjaxControlToolkit
            |^Antlr3|^Autofac|^AutoMapper|^Castle|^ComponentArt|^CppCodeProvider|^DotNetOpenAuth|^EntityFramework
            |^EPPlus|^FluentValidation|^ImageResizer|^itextsharp|^log4net|^MaxMind|^MbUnit|^MiniProfiler|^Mono.Math
            |^MvcContrib|^Newtonsoft|^NHibernate|^nunit|^Org.Mentalis|^PerlRegex|^QuickGraph|^Recaptcha|^Remotion
            |^RestSharp|^Rhino|^Telerik|^Iesi|^TestDriven|^TestFu|^UserAgentStringLibrary|^VJSharpCodeProvider
            |^WebActivator|^WebDev|^WebGrease";
        private string assemblyRestrictToLoadingPattern = ".*";
        private IList<string> assemblyNames = new List<string>();

        #endregion

        #region 属性

        /// <summary>The app domain to look for types in.</summary>
        public virtual AppDomain App
        {
            get { return AppDomain.CurrentDomain; }
        }

        /// <summary>Gets or sets wether Nop should iterate assemblies in the app domain when loading Nop types. Loading patterns are applied when loading these assemblies.</summary>
        public bool LoadAppDomainAssemblies
        {
            get { return loadAppDomainAssemblies; }
            set { loadAppDomainAssemblies = value; }
        }

        /// <summary>加载的程序集名称集合</summary>
        public IList<string> AssemblyNames
        {
            get { return assemblyNames; }
            set { assemblyNames = value; }
        }

        /// <summary>Gets the pattern for dlls that we know don't need to be investigated.</summary>
        public string AssemblySkipLoadingPattern
        {
            get { return assemblySkipLoadingPattern; }
            set { assemblySkipLoadingPattern = value; }
        }

        /// <summary>Gets or sets the pattern for dll that will be investigated. For ease of use this defaults to match all but to increase performance you might want to configure a pattern that includes assemblies and your own.</summary>
        /// <remarks>If you change this so that Nop assemblies arn't investigated (e.g. by not including something like "^Nop|..." you may break core functionality.</remarks>
        public string AssemblyRestrictToLoadingPattern
        {
            get { return assemblyRestrictToLoadingPattern; }
            set { assemblyRestrictToLoadingPattern = value; }
        }

        #endregion

        public IEnumerable<Type> FindClassesOfType(Type assTypeFrom, bool isOnlyClass)
        {
            return this.FindClassesOfType(assTypeFrom, this.GetAssemblies(), isOnlyClass);
        }

        public IEnumerable<Type> FindClassesOfType<T>(bool isOnlyClass)
        {
            return this.FindClassesOfType<T>(this.GetAssemblies(), isOnlyClass);
        }

        public IEnumerable<Type> FindClassesOfType<T>(IEnumerable<Assembly> assemblies, bool isOnlyClass)
        {
            return this.FindClassesOfType(typeof(T), assemblies, isOnlyClass);
        }

        public virtual IList<Assembly> GetAssemblies()
        {
            var addedAssemblyNames = new List<string>();
            var assemblies = new List<Assembly>();

            if (LoadAppDomainAssemblies)
            {
                this.AddAssembliesInAppDomain(addedAssemblyNames, assemblies);
            }
            return assemblies;
        }

        public IEnumerable<Type> FindClassesOfType(Type assTypeFrom, 
            IEnumerable<Assembly> assemblies, 
            bool isOnlyClass)
        {
            var result = new List<Type>();
            try
            {
                foreach (var assembly in assemblies)
                {
                    Type[] types = null;
                    try
                    {
                        types = assembly.GetTypes();
                    }
                    catch (ReflectionTypeLoadException reflectionTypeLoadException)
                    {
                        StringBuilder sb = new StringBuilder();
                        sb.AppendLine("获取程序集内部定义的类型异常，程序集名称：" + assembly.FullName);
                        sb.AppendLine(reflectionTypeLoadException.ToString());
                        //记日志
                    }
                    catch (Exception ex)
                    {
                        StringBuilder sb = new StringBuilder();
                        sb.AppendLine("获取程序集内部定义的类型异常，程序集名称：" + assembly.FullName);
                        sb.AppendLine(ex.ToString());
                        //记日志
                    }
                    if (types == null || types.Length <= 0)
                    {
                        continue;
                    }
                    foreach (var type in types)
                    {
                        if (assTypeFrom.IsAssignableFrom(type)
                            || (assTypeFrom.IsGenericTypeDefinition && this.IsTypeImplementOpenGeneric(type, assTypeFrom)))
                        {
                            if (type.IsInterface)//排除接口
                            {
                                continue;
                            }
                            if (!isOnlyClass || (type.IsClass && !type.IsAbstract))
                            {
                                result.Add(type);
                            }

                        }
                    }
                }
            }
            catch (Exception ex)
            {
                //异常需抛出 不能让程序正常运行
                throw;
                //log
            }
            return result;

        }

        

        protected virtual bool IsTypeImplementOpenGeneric(Type type, Type openGeneric)
        {
            try
            {
                //表示可用于构造当前类型的泛型类型的 System.Type 对象
                var implementedInterface = openGeneric.GetGenericTypeDefinition();
                foreach (var genericTypeDefinition in type.FindInterfaces((objType, objCriteria) => true, null))//寻找继承该接口类型的集合
                {
                    if (genericTypeDefinition.IsGenericType)
                    {
                        continue;
                    }
                    var isMatch = genericTypeDefinition.IsAssignableFrom(implementedInterface.GetGenericTypeDefinition());
                    return isMatch;
                }
                //判断基类是否满足该类型
                if (type.BaseType != null
                    && type.BaseType.IsGenericType
                    && type.BaseType.GetGenericTypeDefinition().Equals(openGeneric))
                {
                    return true;
                }
            }
            catch (Exception)
            {
                //记日志
            }
            return false;
        }

        private void AddAssembliesInAppDomain(List<string> addedAssemblyNames, List<Assembly> assemblies)
        {
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())//获取项目下已加载所有引用程序集
            {
                if (Matches(assembly.FullName))
                {
                    if (!addedAssemblyNames.Contains(assembly.FullName))
                    {
                        assemblies.Add(assembly);
                        addedAssemblyNames.Add(assembly.FullName);
                    }
                }
            }
        }

        /// <summary>
        /// 未使用
        /// </summary>
        /// <param name="addedAssemblyNames"></param>
        /// <param name="assemblies"></param>
        private void AddConfiguredAssemblies(List<string> addedAssemblyNames, List<Assembly> assemblies)
        {
            foreach (string assemblyName in AssemblyNames)
            {
                //未能加载文件或程序集“***”或它的某一个依赖项。给定程序集名称或基本代码无效。 (异常来自 HRESULT:0x80131047)  colipu 4.0环境处异常
                //Assembly assembly = Assembly.Load(assemblyName);
                //先把DLL加载到内存,再从内存中加载
                FileStream fs = new FileStream(assemblyName, FileMode.Open, FileAccess.Read);
                BinaryReader br = new BinaryReader(fs);
                byte[] bFile = br.ReadBytes((int)fs.Length);
                br.Close();
                fs.Close();
                Assembly assembly = Assembly.Load(bFile);

                if (!addedAssemblyNames.Contains(assembly.FullName))
                {
                    assemblies.Add(assembly);
                    addedAssemblyNames.Add(assembly.FullName);
                }
            }
        }

        private bool Matches(string assemblyFullName)
        {
            Func<string, bool> isMatch = (pattern) =>
            {
                return Regex.IsMatch(assemblyFullName, pattern, RegexOptions.IgnoreCase | RegexOptions.Compiled);
            };
            return !isMatch(AssemblySkipLoadingPattern)
                && isMatch(AssemblyRestrictToLoadingPattern);
        }
    }
}
