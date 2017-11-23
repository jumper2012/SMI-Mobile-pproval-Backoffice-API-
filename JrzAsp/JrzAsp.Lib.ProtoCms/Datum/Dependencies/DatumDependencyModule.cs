using System;
using JrzAsp.Lib.ProtoCms.Datum.Services;
using JrzAsp.Lib.TypeUtilities;

namespace JrzAsp.Lib.ProtoCms.Datum.Dependencies {
    public class DatumDependencyModule : IDependencyModule {
        public decimal Priority { get; }
        public Type[] IgnoredTargetTypes { get; }
        public Type[] ExtraTargetTypes { get; }

        public GenericTypeInfo[] ExtraTargetGenericTypes => new[] {
            new GenericTypeInfo(typeof(DatumFinder<>), new Func<Type, bool>[] {
                t => t.IsNonDynamicallyGeneratedClass()
            }),
            new GenericTypeInfo(typeof(DatumModifier<>), new Func<Type, bool>[] {
                t => t.IsNonDynamicallyGeneratedClass()
            })
        };

        public Type[] ExtraDependencyTypesAlwaysFresh { get; }

        public GenericTypeInfo[] ExtraDependencyGenericTypesAlwaysFresh => new[] {
            new GenericTypeInfo(typeof(IDatumFinder<>), new Func<Type, bool>[] {
                t => t.IsNonDynamicallyGeneratedClass()
            }),
            new GenericTypeInfo(typeof(IDatumModifier<>), new Func<Type, bool>[] {
                t => t.IsNonDynamicallyGeneratedClass()
            })
        };

        public Type[] ExtraDependencyTypesPerRequest { get; }
        public GenericTypeInfo[] ExtraDependencyGenericTypesPerRequest => new[] {
            new GenericTypeInfo(typeof(IDatumGetterHandler<>), new Func<Type, bool>[] {
                t => t.IsNonDynamicallyGeneratedClass()
            }),
            new GenericTypeInfo(typeof(IDatumViewerHandler<>), new Func<Type, bool>[] {
                t => t.IsNonDynamicallyGeneratedClass()
            }),
            new GenericTypeInfo(typeof(IDatumModifierHandler<>), new Func<Type, bool>[] {
                t => t.IsNonDynamicallyGeneratedClass()
            }),
            new GenericTypeInfo(typeof(IDatumPermissionsHandler<>), new Func<Type, bool>[] {
                t => t.IsNonDynamicallyGeneratedClass()
            }),
            new GenericTypeInfo(typeof(IDatumSearchHandler<>), new Func<Type, bool>[] {
                t => t.IsNonDynamicallyGeneratedClass()
            }),
            new GenericTypeInfo(typeof(IDatumSortHandler<>), new Func<Type, bool>[] {
                t => t.IsNonDynamicallyGeneratedClass()
            }),
            new GenericTypeInfo(typeof(IDatumTableActionsHandler<>), new Func<Type, bool>[] {
                t => t.IsNonDynamicallyGeneratedClass()
            }),
            new GenericTypeInfo(typeof(IDatumWhereHandler<>), new Func<Type, bool>[] {
                t => t.IsNonDynamicallyGeneratedClass()
            }),
        };
        public Type[] ExtraDependencyTypesGlobalSingleton { get; }
        public GenericTypeInfo[] ExtraDependencyGenericTypesGlobalSingleton { get; }

        public void OnBindingType(Type targetType, Type[] serviceTypes, DependencyScope scope, bool isIgnored) {
            // no op
        }

        public void OnTypeBound(Type targetType, Type[] serviceTypes, DependencyScope scope) {
            // no op
        }
    }
}