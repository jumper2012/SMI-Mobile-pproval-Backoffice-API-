using System;
using System.Collections.Generic;
using JrzAsp.Lib.TypeUtilities;

namespace JrzAsp.Lib.ProtoCms {
    public interface IDependencyProvider {
        /// <summary>
        ///     Register services to a type within a specific scope
        /// </summary>
        /// <param name="fromTypes"></param>
        /// <param name="toType"></param>
        /// <param name="scope"></param>
        void RegisterService(Type[] fromTypes, Type toType, DependencyScope scope);

        /// <summary>Retrieves a service from the scope.</summary>
        /// <returns>The retrieved service.</returns>
        /// <param name="serviceType">The service to be retrieved.</param>
        object GetService(Type serviceType);

        /// <summary>Retrieves a collection of services from the scope.</summary>
        /// <returns>The retrieved collection of services.</returns>
        /// <param name="serviceType">The collection of services to be retrieved.</param>
        IEnumerable<object> GetServices(Type serviceType);
    }

    public enum DependencyScope {
        AlwaysFresh,
        PerRequest,
        GlobalSingleton
    }

    public static class DependencyProviderExtensions {
        public static T GetService<T>(this IDependencyProvider dp) {
            return dp.GetService(typeof(T)).DirectCastTo<T>();
        }

        public static IEnumerable<T> GetServices<T>(this IDependencyProvider dp) {
            return dp.GetServices(typeof(T)).DirectCastTo<IEnumerable<T>>();
        }
    }
}