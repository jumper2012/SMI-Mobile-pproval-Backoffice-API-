using System;
using System.Collections.Generic;
using Ninject;
using Ninject.Web.Common;
using JrzAsp.Lib.ProtoCms;

namespace WebApp.Features.ProtoCmsIntegration {
    public class ProtoCmsDependencyProvider : IDependencyProvider {
        private readonly IKernel _ninjectKernel;

        public ProtoCmsDependencyProvider(IKernel ninjectKernel) {
            _ninjectKernel = ninjectKernel;
        }

        public void RegisterService(Type[] fromTypes, Type toType, DependencyScope scope) {
            switch (scope) {
                case DependencyScope.AlwaysFresh:
                    _ninjectKernel.Bind(fromTypes).To(toType).InTransientScope();
                    break;
                case DependencyScope.PerRequest:
                    _ninjectKernel.Bind(fromTypes).To(toType).InRequestScope();
                    break;
                case DependencyScope.GlobalSingleton:
                    _ninjectKernel.Bind(fromTypes).To(toType).InSingletonScope();
                    break;
            }
        }

        public object GetService(Type serviceType) {
            return _ninjectKernel.Get(serviceType);
        }

        public IEnumerable<object> GetServices(Type serviceType) {
            return _ninjectKernel.GetAll(serviceType);
        }
    }

}