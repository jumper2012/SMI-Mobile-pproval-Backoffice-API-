using Microsoft.Owin;
using Owin;
using WebApp;

[assembly: OwinStartup(typeof(Startup))]

namespace WebApp {
    public partial class Startup {
        public void Configuration(IAppBuilder app) {
            ConfigureAuth(app);
        }

        public void Configure(IAppBuilder app)
        {
            //app.M
        }
    }
}