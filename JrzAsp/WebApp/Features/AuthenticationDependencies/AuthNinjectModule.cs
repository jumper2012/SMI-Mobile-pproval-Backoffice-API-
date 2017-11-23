using System.Web;
using Microsoft.Owin.Security;
using Ninject.Modules;
using Ninject.Web.Common;

namespace WebApp.Features.AuthenticationDependencies {
    public class AuthNinjectModule : NinjectModule {
        public override void Load() {
            Kernel.Bind<IAuthenticationManager>()
                .ToMethod(ctx => HttpContext.Current.GetOwinContext().Authentication).InRequestScope();
        }
    }
}