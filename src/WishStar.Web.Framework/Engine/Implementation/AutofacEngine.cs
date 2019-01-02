using Autofac;
using Autofac.Core.Lifetime;
using Autofac.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WishStar.Web.Framework.FrameWork;

namespace WishStar.Web.Framework.Engine.Implementation
{
    public class AutofacEngine : IEngine
    {

        protected readonly AutofacExpand _autofacExpand;
        protected IContainer _container;
        protected IServiceProvider _serviceProvider;
        protected ILifetimeScope _scope;

        public AutofacEngine() { }

        public AutofacEngine(AutofacExpand autofacExpand)
        {
            _autofacExpand = autofacExpand;
        }

        protected virtual IContainer Container => _container;

        public IServiceProvider ServiceProvider => _serviceProvider;

        protected virtual ILifetimeScope Scope
        {
            get
            {
                try
                {

                    if (_scope == null && this._autofacExpand != null 
                        && this._autofacExpand.ScopeExpand != null)
                    {
                        _scope = this._autofacExpand.ScopeExpand.Invoke();
                    }
                }
                catch (Exception)
                {
                    //log
                }
                if (_scope == null)
                {
                    _scope = Container.BeginLifetimeScope(MatchingScopeLifetimeTags.RequestLifetimeScopeTag);
                }
                return _scope;
            }
        }

        public void Initialize(IServiceCollection services,ITypeFinder typeFinder)
        {
            try
            {
                //依赖注册
                this.RegisterDependencies(services, typeFinder);
            }
            catch (Exception)
            {
                //log
                //需要抛异常 未注册成功项目不可正常启动
                throw;
            }
        }
        public T Resolve<T>()
        {
            return this.Scope.Resolve<T>();
        }

        public T Resolve<T>(string name)
        {
            return this.Scope.ResolveKeyed<T>(name);
        }

        public T[] ResolveAll<T>()
        {
            return this.Scope.Resolve<T[]>();
        }

        public T[] ResolveAll<T>(string name)
        {
            return this.Scope.ResolveKeyed<T[]>(name);
        }

        protected IServiceProvider RegisterDependencies(IServiceCollection services,ITypeFinder typeFinder)
        {
            var containerBuilder = new ContainerBuilder();
            //注册引擎
            containerBuilder.RegisterInstance(this).As<IEngine>().SingleInstance();

            //注册type finder
            containerBuilder.RegisterInstance(typeFinder).As<ITypeFinder>().SingleInstance();

            var definedRegisters = typeFinder.FindClassesOfType<IDefinedRegister>(true);

            var instances = definedRegisters
                //.Where(dependencyRegistrar => PluginManager.FindPlugin(dependencyRegistrar)?.Installed ?? true) //ignore not installed plugins
                .Select(dependencyRegistrar => (IDefinedRegister)Activator.CreateInstance(dependencyRegistrar))
                .OrderBy(dependencyRegistrar => dependencyRegistrar.ShowOrder);
            foreach (var definedRegister in instances)
            {
                definedRegister.Register(containerBuilder, typeFinder);
            }
            //扩展构造容器
            if (this._autofacExpand != null && this._autofacExpand.BuildExpand != null)
            {
                this._autofacExpand.BuildExpand.Invoke(containerBuilder);
            }

            //已注册的内置注入填充至autofac
            containerBuilder.Populate(services);

            _container = containerBuilder.Build();

            //注册扩展依赖
            if (this._autofacExpand != null && this._autofacExpand.RegisterExpand != null)
            {
                this._autofacExpand.RegisterExpand.Invoke(_container);
            }
            _serviceProvider = new AutofacServiceProvider(_container);
            return _serviceProvider;
        } 
    }
}
