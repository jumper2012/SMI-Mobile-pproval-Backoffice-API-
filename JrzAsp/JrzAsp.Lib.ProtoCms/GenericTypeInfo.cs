using System;
using System.Collections.Generic;
using System.Linq;

namespace JrzAsp.Lib.ProtoCms {
    public class GenericTypeInfo {
        public GenericTypeInfo(Type genericTypeDefinition, Func<Type, bool>[] genericTypeArgumentsMatcher) {
            GenericTypeDefinition = genericTypeDefinition;
            GenericTypeArgumentsMatcher = genericTypeArgumentsMatcher;
            ValidateProps();
        }

        public Type GenericTypeDefinition { get; }

        public Func<Type, bool>[] GenericTypeArgumentsMatcher { get; }

        public Type[] GetConcreteGenericTypes(IEnumerable<Type> scannedTypes) {
            var sts = scannedTypes.ToArray();
            var gtd = GenericTypeDefinition;
            var result = new List<Type>();

            var genArgs = new List<Type[]>();
            foreach (var matcher in GenericTypeArgumentsMatcher) {
                var args = sts.Where(x => matcher(x)).Distinct().ToArray();
                genArgs.Add(args);
            }

            var currentIndex = new int[genArgs.Count];
            while (currentIndex != null) {
                var typeArgs = new Type[genArgs.Count];
                for (var i = 0; i < genArgs.Count; i++) {
                    typeArgs[i] = genArgs[i][currentIndex[i]];
                }
                var concreteType = gtd.MakeGenericType(typeArgs);
                result.Add(concreteType);
                currentIndex = GetNextGetterIndexes(currentIndex, genArgs);
            }

            return result.ToArray();
        }

        private int[] GetNextGetterIndexes(int[] prevIndexes, List<Type[]> genArgs) {
            var result = new int[genArgs.Count];
            var carry = 1;
            for (var i = 0; i < genArgs.Count; i++) {
                result[i] = prevIndexes[i];
                result[i] += carry;
                if (result[i] >= genArgs[i].Length) {
                    result[i] = 0;
                    if (i == genArgs.Count - 1) {
                        return null;
                    }
                    carry = 1;
                } else {
                    carry = 0;
                }
            }

            return result;
        }

        private void ValidateProps() {
            if (GenericTypeDefinition == null) {
                throw new InvalidOperationException(
                    $"ProtoCMS: generic type definition required.");
            }
            if (!GenericTypeDefinition.IsGenericTypeDefinition) {
                throw new InvalidOperationException(
                    $"ProtoCMS: provided type is not a generic type definition.");
            }
            if (GenericTypeArgumentsMatcher == null) {
                throw new InvalidOperationException(
                    $"ProtoCMS: generic type arguments matcher is required.");
            }
            if (GenericTypeDefinition.GetGenericArguments().Length != GenericTypeArgumentsMatcher.Length) {
                throw new InvalidOperationException(
                    $"ProtoCMS: generic type arguments matcher length is different than expected arguments length of " +
                    $"the generic type definition.");
            }
            if (GenericTypeArgumentsMatcher.Any(x => x == null)) {
                throw new InvalidOperationException(
                    $"ProtoCMS: generic type arguments matcher must not have null item.");
            }
        }
    }
}