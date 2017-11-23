using System.Reflection;

namespace JrzAsp.Lib.TypeUtilities {
    public static class ParameterInfoArrayExtensions {
        public static bool IsMatchWithParameterInstances(this ParameterInfo[] parameterInfos, object[] parameters) {
            if (parameters == null) {
                if (parameterInfos == null) return true;
                if (parameterInfos.Length > 0) return false;
            }

            if (parameterInfos == null) if (parameters.Length > 0) return false;

            if (parameterInfos.Length != parameters.Length) return false;
            for (var i = 0; i < parameterInfos.Length; i++) {
                if (parameters[i] == null) {
                    if (!parameterInfos[i].ParameterType.CanBeAssignedWithNull()) return false;
                } else {
                    if (parameterInfos[i].ParameterType != parameters[i].GetType() &&
                        !parameterInfos[i].ParameterType.IsInstanceOfType(parameters[i])) {
                        return false;
                    }

                }
            }
            return true;

        }
    }
}