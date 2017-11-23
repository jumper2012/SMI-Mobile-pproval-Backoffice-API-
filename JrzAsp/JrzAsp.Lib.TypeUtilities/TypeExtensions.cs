using System;
using System.Linq;
using System.Reflection;

namespace JrzAsp.Lib.TypeUtilities {
    public static class TypeExtensions {
        public static bool CanBeAssignedWithNull(this Type t) {
            return !t.IsValueType || t.IsGenericType && t.GetGenericTypeDefinition() == typeof(Nullable<>);
        }

        public static MethodBase GetGenericMethod(this Type type, string name, Type[] typeArgs,
            Type[] argTypes, BindingFlags flags) {
            var typeArity = typeArgs.Length;
            var methods = type.GetMethods()
                .Where(m => m.Name == name)
                .Where(m => m.GetGenericArguments().Length == typeArity)
                .Select(m => m.MakeGenericMethod(typeArgs));

            return Type.DefaultBinder.SelectMethod(flags, methods.ToArray(), argTypes, null);
        }

        public static MethodBase GetNonGenericMethod(this Type type, string name,
            Type[] argTypes, BindingFlags flags) {
            var methods = type.GetMethods()
                .Where(m => m.Name == name)
                .Where(m => m.GetGenericArguments().Length == 0);

            return Type.DefaultBinder.SelectMethod(flags, methods.ToArray(), argTypes, null);
        }

        public static bool IsConcreteClass(this Type type) {
            return type.IsClass && !type.IsAbstract && !type.IsInterface && !type.IsGenericTypeDefinition;
        }

        public static bool IsNonDynamicallyGeneratedConcreteClass(this Type type) {
            return type.IsClass && !type.IsAbstract && !type.IsInterface && !type.IsGenericTypeDefinition
                   && !type.Name.StartsWith("<");
        }

        public static bool IsNonDynamicallyGeneratedClass(this Type type) {
            return type.IsClass && !type.IsInterface && !type.IsGenericTypeDefinition
                   && !type.Name.StartsWith("<");
        }
    }
}