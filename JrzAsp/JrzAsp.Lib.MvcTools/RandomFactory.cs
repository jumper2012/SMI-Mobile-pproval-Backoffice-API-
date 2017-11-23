using System;
using System.Web;

namespace JrzAsp.Lib.MvcTools {
    public static class RandomFactory {
        private static readonly string RANDOM_FACTORY_CACHE_KEY =
            $"{typeof(RandomFactory)}:{nameof(RANDOM_FACTORY_CACHE_KEY)}";

        public static Random Get() {
            var inst = HttpContext.Current.Items[RANDOM_FACTORY_CACHE_KEY];
            if (inst != null) return inst as Random;
            inst = new Random();
            HttpContext.Current.Items[RANDOM_FACTORY_CACHE_KEY] = inst;
            return (Random) inst;
        }
    }
}