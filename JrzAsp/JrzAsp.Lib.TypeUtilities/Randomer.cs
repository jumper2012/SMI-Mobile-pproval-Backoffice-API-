using System;

namespace JrzAsp.Lib.TypeUtilities {
    public static class Randomer {

        static Randomer() {
            Self = new Random();
        }

        public static Random Self { get; }
    }
}