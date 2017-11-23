# How to Integrate #

1.  User webapp must have DI container that can do proper constructor injection.

2.  User webapp must create a class that implements `JrzAsp.Lib.ProtoCms.IDependencyProvider`.

3.  The user must register the class implementation in no 1 to its DI container, and then during that registration, a
    valid func to `JrzAsp.Lib.ProtoCms.ProtoEngine.GetDependencyProvider` must be assigned that'll return that class
    implementation. After that, user webapp assembly must be added to `AutoDependencyBinderModule.ScannedAssemblies`.
    After that, on global asax app start before routing and webapi config, call `ProtoEngine.Start()` so that all
    ProtoCMS internal dependencies will also be registered to the user webapp's DI container.
    Example if using Ninject:
    ```C#
    public class ProtoCmsNinjectModule : NinjectModule {
        public override void Load() {
            Kernel.Bind<IDependencyProvider, ProtoCmsDependencyProvider>().To<ProtoCmsDependencyProvider>()
                .InSingletonScope();
            ProtoEngine.GetDependencyProvider = () => Kernel.Get<IDependencyProvider>();
        }
    }
    ```

4.  Must add the statement `app.UseOAuthForProtoCmsApi();` before
    any auth config in the webapp's OWIN Startup(.auth).cs inside `Configuration(IAppBuilder app)` method. Inside
    `App_Start/WebApiConfig.cs` make sure to call `config.MapHttpAttributeRoutes();`.

5.  The webapp must use EF 6.1.3+ and has its dbcontext implement `JrzAsp.Lib.ProtoCms.Database.IProtoCmsDbContext`.
    Also, the webapp's dbcontext must override OnModelCreating method, and then call
    `modelBuilder.OnProtoCmsDbModelCreating()` after `base.OnModelCreating(modelBuilder)`.

6.  The webapp 'UserManager' (of ASP .NET Identity) must implement
    `JrzAsp.Lib.ProtoCms.User.Services.IProtoUserManager`. Also, the webapp 'RoleManager' (of ASP .NET Identity) must
    implement `JrzAsp.Lib.ProtoCms.Role.Services.IProtoRoleManager`.

7.  'Role' data model must implement `JrzAsp.Lib.ProtoCms.Role.Models.IProtoRole`.

8.  'User' data model must implement `JrzAsp.Lib.ProtoCms.User.Models.IProtoUser`.

9.  Use `FilePathCorsEnablerHttpModule.AllowedPathRegexPatterns.Add(<your folder containing static files>);` to enable
    serving your own custom static files to proto cms if needed. Add that statement in your global asax or OWIN startup
    class. Must also add that `FilePathCorsEnablerHttpModule` into web config http module.
    
10. In webapp's Startup auth cs, if using `CookieAuthentication` for MVC then all path to proto cms api must not
    return redirect to login page if not authorized. So, make changes so that the OWIN's cookie auth look like this:

    ```C#
    app.UseCookieAuthentication(new CookieAuthenticationOptions {
        ...
        Provider = new CookieAuthenticationProvider {
            ...
            OnApplyRedirect = (ctx) => {
                if (
                    // if request isn't directed to ProtoCMS API...
                    !ctx.Request.Path.StartsWithSegments(
                        new PathString(JrzAsp.Lib.ProtoCms.MyAppSettings.ApiRoutePrefix)
                    )
                ) {
                    ctx.Response.Redirect(ctx.RedirectUri);
                }
            }
            ...
        }
        ...
    });
    ```

11. Create an implementation of `JrzAsp.Lib.ProtoCms.IProtoCmsMainUrlsProvider` that can provide main URLs for ProtoCMS
    in cms dashboard view.

# Web.config appSettings #

-   `ProtoCms:ApiRoutePrefix`
    Route prefix for proto cms api. Defaults to '/proto-cms-api/v1'

-   `ProtoCMS:WebPageBaseTitle`
    CMS page base title, defaults to 'CMS Dash'

-   `ProtoCMS:FooterCopyright`
    Copyright text in CMS page footer

-   `ProtoCMS:LogoUrl`
    CMS dash logo URL, defaults to *~/Content/MetronicTheme/assets/layouts/layout/img/logo.png*
