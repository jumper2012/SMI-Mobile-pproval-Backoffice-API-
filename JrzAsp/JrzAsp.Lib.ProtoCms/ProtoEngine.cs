using System;
using System.Linq;
using System.Web.Http;
using JrzAsp.Lib.TypeUtilities;

namespace JrzAsp.Lib.ProtoCms {
    public static class ProtoEngine {

        private static Func<IDependencyProvider> _getDependencyProvider;
        public static bool HasStarted { get; private set; }

        public static Func<IDependencyProvider> GetDependencyProvider {
            get {
                if (_getDependencyProvider == null) {
                    throw new InvalidOperationException(
                        $"{typeof(ProtoEngine)}.{nameof(GetDependencyProvider)} hasn't been set. Please set it on webapp startup once.");
                }
                if (!HasStarted) throw new InvalidOperationException($"{typeof(ProtoEngine)} must be started first.");
                return _getDependencyProvider;
            }
            set {
                if (_getDependencyProvider != null) {
                    throw new InvalidOperationException(
                        $"{typeof(ProtoEngine)}.{nameof(GetDependencyProvider)} can only be set once on webapp startup.");
                }
                _getDependencyProvider = value;
            }
        }

        internal static Func<HttpConfiguration> GetHttpConfiguration { get; set; }

        public static void Start() {
            if (HasStarted) {
                throw new InvalidOperationException($"{typeof(ProtoEngine)} has been started. Can't start again.");
            }

            HasStarted = true;

            var dp = GetDependencyProvider();
            var autoBinder = new AutoDependencyBinder(dp);
            autoBinder.RegisterDependencies();
        }
    }
}