using System;

namespace JrzAsp.Lib.ProtoCms {
    /// <summary>
    ///     Implementors must have default parameterless constructor and not require other dependencies.
    /// </summary>
    public interface IDependencyModule {
        /// <summary>
        /// Low number for priority means executed first.
        /// </summary>
        decimal Priority { get; }

        /// <summary>
        ///     Ignored target types to prevent automatic binding. You can attach event to <see cref="OnBindingType" /> to
        ///     manually perform binding if needed.
        /// </summary>
        Type[] IgnoredTargetTypes { get; }

        /// <summary>
        ///     Extra target types to include in auto binding. Only classes that's not abstract that will be included as
        ///     target types, use this if what you want isn't so.
        /// </summary>
        Type[] ExtraTargetTypes { get; }

        /// <summary>
        /// See <see cref="ExtraTargetTypes"/>, but this is for generic type.
        /// </summary>
        GenericTypeInfo[] ExtraTargetGenericTypes { get; }

        /// <summary>
        ///     Extra dependency types to use for always fresh scope auto binding.
        ///     See <see cref="ExtraDependencyTypesPerRequest" /> for more info/examples.
        /// </summary>
        Type[] ExtraDependencyTypesAlwaysFresh { get; }
        
        /// <summary>
        /// See <see cref="ExtraDependencyTypesAlwaysFresh"/>, but this is for generic type.
        /// </summary>
        GenericTypeInfo[] ExtraDependencyGenericTypesAlwaysFresh { get; }

        /// <summary>
        ///     Extra dependency types to use for per request scope auto binding. E.g. asp net identity's UserManager is a
        ///     class that should be a dependency type, but because it obviously doesn't implement marker interfaces, auto
        ///     binder can't use it. Therefore, if you add that type to this prop, then the auto binder will notice and make
        ///     use of it for target type that implements/inherits from asp net identity's UserManager.
        /// </summary>
        Type[] ExtraDependencyTypesPerRequest { get; }
        
        /// <summary>
        /// See <see cref="ExtraDependencyTypesPerRequest"/>, but this is for generic type.
        /// </summary>
        GenericTypeInfo[] ExtraDependencyGenericTypesPerRequest { get; }

        /// <summary>
        ///     Extra dependency types to use for always fresh scope auto binding.
        ///     See <see cref="ExtraDependencyTypesPerRequest" /> for more info/examples.
        /// </summary>
        Type[] ExtraDependencyTypesGlobalSingleton { get; }
        
        /// <summary>
        /// See <see cref="ExtraDependencyTypesGlobalSingleton"/>, but this is for generic type.
        /// </summary>
        GenericTypeInfo[] ExtraDependencyGenericTypesGlobalSingleton { get; }

        /// <summary>
        ///     This method will be run before dependency binding occurs.
        /// </summary>
        /// <param name="targetType">The target type</param>
        /// <param name="serviceTypes">The service type that should be resolved to target type when requested.</param>
        /// <param name="scope">The dependency scope</param>
        /// <param name="isIgnored">Whether the target type is ignored or not</param>
        void OnBindingType(Type targetType, Type[] serviceTypes, DependencyScope scope, bool isIgnored);

        /// <summary>
        ///     This method will be run after dependency binding occurs. Ignored target type will not be included in this
        ///     method.
        /// </summary>
        /// <param name="targetType">The target type</param>
        /// <param name="serviceTypes">The service type that should be resolved to target type when requested.</param>
        /// <param name="scope">The dependency scope</param>
        void OnTypeBound(Type targetType, Type[] serviceTypes, DependencyScope scope);
    }
}