using Ninject;
using Ninject.Modules;
using JrzAsp.Lib.ProtoCms;

namespace WebApp.Features.ProtoCmsIntegration {
    public class ProtoCmsNinjectModule : NinjectModule {
        public override void Load() {
            Kernel.Bind<IDependencyProvider, ProtoCmsDependencyProvider>()
                .To<ProtoCmsDependencyProvider>()
                .InSingletonScope();
            ProtoEngine.GetDependencyProvider = () => Kernel.Get<IDependencyProvider>();
        }
    }
}