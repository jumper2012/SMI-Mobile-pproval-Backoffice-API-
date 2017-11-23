using System;
using System.Collections.Generic;
using System.Linq;
using JrzAsp.Lib.TypeUtilities;

namespace JrzAsp.Lib.ProtoCms {
    /// <summary>
    ///     Performs automatic dependency binding from interfaces and other service types to target type according to marker
    ///     interfaces (<see cref="IAlwaysFreshDependency" />, <see cref="IPerRequestDependency" />,
    ///     <see cref="IGlobalSingletonDependency" />)
    /// </summary>
    public sealed class AutoDependencyBinder {

        private readonly IDependencyProvider _dp;

        /// <summary>
        ///     See <see cref="IDependencyModule.ExtraDependencyGenericTypesAlwaysFresh" />.
        /// </summary>
        private readonly GenericTypeInfo[] ExtraDependencyGenericTypesAlwaysFresh;

        /// <summary>
        ///     See <see cref="IDependencyModule.ExtraDependencyGenericTypesGlobalSingleton" />.
        /// </summary>
        public readonly GenericTypeInfo[] ExtraDependencyGenericTypesGlobalSingleton;

        /// <summary>
        ///     See <see cref="IDependencyModule.ExtraDependencyGenericTypesPerRequest" />.
        /// </summary>
        public readonly GenericTypeInfo[] ExtraDependencyGenericTypesPerRequest;

        /// <summary>
        ///     See <see cref="IDependencyModule.ExtraDependencyTypesAlwaysFresh" />.
        /// </summary>
        public readonly Type[] ExtraDependencyTypesAlwaysFresh;

        /// <summary>
        ///     See <see cref="IDependencyModule.ExtraDependencyTypesGlobalSingleton" />.
        /// </summary>
        public readonly Type[] ExtraDependencyTypesGlobalSingleton;

        /// <summary>
        ///     See <see cref="IDependencyModule.ExtraDependencyTypesPerRequest" />.
        /// </summary>
        public readonly Type[] ExtraDependencyTypesPerRequest;

        /// <summary>
        ///     See <see cref="IDependencyModule.ExtraTargetGenericTypes" />.
        /// </summary>
        public readonly GenericTypeInfo[] ExtraTargetGenericTypes;

        /// <summary>
        ///     See <see cref="IDependencyModule.ExtraTargetTypes" />.
        /// </summary>
        public readonly Type[] ExtraTargetTypes;

        /// <summary>
        ///     See <see cref="IDependencyModule.IgnoredTargetTypes" />.
        /// </summary>
        public readonly Type[] IgnoredTargetTypes;

        /// <summary>
        ///     Dependency modules.
        /// </summary>
        public readonly IDependencyModule[] Modules;

        /// <summary>
        ///     Scanned types for auto binding.
        /// </summary>
        public readonly Type[] ScannedTypes;

        internal AutoDependencyBinder(IDependencyProvider dependencyProvider) {
            _dp = dependencyProvider;
            Modules = TypesCache.AppDomainTypes
                .Where(x => typeof(IDependencyModule).IsAssignableFrom(x) && x.IsNonDynamicallyGeneratedConcreteClass())
                .Select(x => Activator.CreateInstance(x).DirectCastTo<IDependencyModule>())
                .OrderBy(x => x.Priority)
                .ToArray();

            ScannedTypes = Modules.Select(x => x.GetType().Assembly).Distinct()
                .SelectMany(x => x.GetTypes()).Union(typeof(AutoDependencyBinder).Assembly.GetTypes())
                .Distinct().ToArray();

            ExtraDependencyTypesAlwaysFresh = Modules.Where(x => x.ExtraDependencyTypesAlwaysFresh != null)
                .SelectMany(x => x.ExtraDependencyTypesAlwaysFresh).Distinct().ToArray();
            ExtraDependencyGenericTypesAlwaysFresh = Modules
                .Where(x => x.ExtraDependencyGenericTypesAlwaysFresh != null)
                .SelectMany(x => x.ExtraDependencyGenericTypesAlwaysFresh).Distinct().ToArray();

            ExtraDependencyTypesPerRequest = Modules.Where(x => x.ExtraDependencyTypesPerRequest != null)
                .SelectMany(x => x.ExtraDependencyTypesPerRequest).Distinct().ToArray();
            ExtraDependencyGenericTypesPerRequest = Modules.Where(x => x.ExtraDependencyGenericTypesPerRequest != null)
                .SelectMany(x => x.ExtraDependencyGenericTypesPerRequest).Distinct().ToArray();

            ExtraDependencyTypesGlobalSingleton = Modules.Where(x => x.ExtraDependencyTypesGlobalSingleton != null)
                .SelectMany(x => x.ExtraDependencyTypesGlobalSingleton).Distinct().ToArray();
            ExtraDependencyGenericTypesGlobalSingleton = Modules
                .Where(x => x.ExtraDependencyGenericTypesGlobalSingleton != null)
                .SelectMany(x => x.ExtraDependencyGenericTypesGlobalSingleton).Distinct().ToArray();

            ExtraTargetTypes = Modules.Where(x => x.ExtraTargetTypes != null)
                .SelectMany(x => x.ExtraTargetTypes).Distinct().ToArray();
            ExtraTargetGenericTypes = Modules.Where(x => x.ExtraTargetGenericTypes != null)
                .SelectMany(x => x.ExtraTargetGenericTypes).Distinct().ToArray();

            IgnoredTargetTypes = Modules.Where(x => x.IgnoredTargetTypes != null)
                .SelectMany(x => x.IgnoredTargetTypes).Distinct().ToArray();
        }

        /// <summary>
        ///     Register dependencies to concrete class types automatically according to marker interfaces
        ///     (<see cref="IAlwaysFreshDependency" />, <see cref="IPerRequestDependency" />,
        ///     <see cref="IGlobalSingletonDependency" />) and <see cref="IDependencyModule" />.
        /// </summary>
        public void RegisterDependencies() {
            var alwFreshDepTypes = ScannedTypes.Where(
                    x => typeof(IAlwaysFreshDependency).IsAssignableFrom(x) && (x.IsInterface || x.IsAbstract)
                ).Union(ExtraDependencyTypesAlwaysFresh)
                .Union(ExtraDependencyGenericTypesAlwaysFresh.SelectMany(x => x.GetConcreteGenericTypes(ScannedTypes)))
                .Distinct().ToArray();

            var perReqDepTypes = ScannedTypes.Where(
                    x => typeof(IPerRequestDependency).IsAssignableFrom(x) && (x.IsInterface || x.IsAbstract)
                ).Union(ExtraDependencyTypesPerRequest)
                .Union(ExtraDependencyGenericTypesPerRequest.SelectMany(x => x.GetConcreteGenericTypes(ScannedTypes)))
                .Distinct().ToArray();

            var globSingDepTypes = ScannedTypes.Where(
                    x => typeof(IGlobalSingletonDependency).IsAssignableFrom(x) && (x.IsInterface || x.IsAbstract)
                ).Union(ExtraDependencyTypesGlobalSingleton)
                .Union(ExtraDependencyGenericTypesGlobalSingleton.SelectMany(x =>
                    x.GetConcreteGenericTypes(ScannedTypes)))
                .Distinct().ToArray();

            var typesToProcess = ScannedTypes.Where(x => x.IsNonDynamicallyGeneratedConcreteClass())
                .Union(ExtraTargetTypes)
                .Union(ExtraTargetGenericTypes.SelectMany(x => x.GetConcreteGenericTypes(ScannedTypes)))
                .Distinct().ToArray();

            foreach (var t in typesToProcess) {
                var serviceTypes = new List<Type> {t};
                var hasAlwaysFresh = false;
                var hasPerRequest = false;
                foreach (var dt in alwFreshDepTypes) {
                    if (!dt.IsAssignableFrom(t)) continue;
                    hasAlwaysFresh = true;
                    serviceTypes.Add(dt);
                }
                foreach (var dt in perReqDepTypes) {
                    if (!dt.IsAssignableFrom(t)) continue;
                    hasPerRequest = true;
                    serviceTypes.Add(dt);
                }
                foreach (var dt in globSingDepTypes) {
                    if (!dt.IsAssignableFrom(t)) continue;
                    serviceTypes.Add(dt);
                }
                serviceTypes = serviceTypes.Distinct().ToList();
                if (serviceTypes.Count > 1) {
                    var sta = serviceTypes.ToArray();

                    var scope = DependencyScope.GlobalSingleton;

                    if (hasAlwaysFresh) {
                        scope = DependencyScope.AlwaysFresh;
                    } else if (hasPerRequest) {
                        scope = DependencyScope.PerRequest;
                    }

                    if (IgnoredTargetTypes.Contains(t)) {
                        foreach (var mod in Modules) {
                            mod.OnBindingType(t, sta, scope, true);
                        }
                    } else {
                        foreach (var mod in Modules) {
                            mod.OnBindingType(t, sta, scope, false);
                        }
                        _dp.RegisterService(sta, t, scope);
                        foreach (var mod in Modules) {
                            mod.OnTypeBound(t, sta, scope);
                        }
                    }
                }
            }
        }
    }
}