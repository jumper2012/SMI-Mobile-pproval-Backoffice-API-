namespace JrzAsp.Lib.TypeUtilities {
    public static class ObjectTypeChangeExtensions {
        /// <summary>
        ///     Returns "(T) obj"
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static T DirectCastTo<T>(this object obj) {
            return (T) obj;
        }

        /// <summary>
        ///     Returns "obj as T"
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static T ChangeTypeTo<T>(this object obj) where T : class {
            return obj as T;
        }
    }
}