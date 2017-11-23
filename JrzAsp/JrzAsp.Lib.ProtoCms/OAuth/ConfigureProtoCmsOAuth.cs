using System;
using System.Web.Http;
using Microsoft.Owin;
using Microsoft.Owin.Cors;
using Microsoft.Owin.Security.OAuth;
using Owin;
using JrzAsp.Lib.ProtoCms.OAuth.Providers;

namespace JrzAsp.Lib.ProtoCms.OAuth {
    public static class ConfigureProtoCmsOAuth {
        /// <summary>
        ///     This will setup OAuth system for authorization of WebAPI (not just proto cms)
        ///     Also this will make all WebAPI authorization to use this OAuth exclusively.
        ///     ProtoCMS requires this kind of OAuth, so if it conflicts your webapp, then the cms
        ///     must be hosted in different domain
        /// </summary>
        public static void UseOAuthForProtoCmsApi(this IAppBuilder app, HttpConfiguration globalHttpConfiguration,
            bool useDifferentHttpConfigurationThanGlobal = false) {
            var oauthOpts = new OAuthAuthorizationServerOptions {
                AllowInsecureHttp = true,
                TokenEndpointPath = new PathString($"{ProtoCmsAppSettings.ApiRoutePrefix}/oauth/token"),
                AccessTokenExpireTimeSpan = TimeSpan.FromMinutes(15),
                Provider = new ProtoCmsAuthorizationServerProvider(),
                RefreshTokenProvider = new ProtoCmsRefreshTokenProvider()
            };

            // Token Generation
            app.UseOAuthAuthorizationServer(oauthOpts);
            app.UseOAuthBearerAuthentication(new OAuthBearerAuthenticationOptions());
            app.UseCors(CorsOptions.AllowAll);

            var config = globalHttpConfiguration;
            if (useDifferentHttpConfigurationThanGlobal) {
                config = new HttpConfiguration {
                    DependencyResolver = globalHttpConfiguration.DependencyResolver,
                    IncludeErrorDetailPolicy = globalHttpConfiguration.IncludeErrorDetailPolicy,
                    Initializer = globalHttpConfiguration.Initializer
                };
                config.MapHttpAttributeRoutes();

                // Enable OAuth for this config
                app.UseWebApi(config);
            }
            // ProtoCMS http config getter
            ProtoEngine.GetHttpConfiguration = () => config;
        }
    }
}