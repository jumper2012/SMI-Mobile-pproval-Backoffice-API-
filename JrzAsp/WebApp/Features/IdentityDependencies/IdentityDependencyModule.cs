using System;
using System.Data.Entity;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using JrzAsp.Lib.ProtoCms;
using WebApp.Models;

namespace WebApp.Features.IdentityDependencies {
    public class IdentityDependencyModule : IDependencyModule {
        public decimal Priority => 0;
        public Type[] IgnoredTargetTypes { get; }

        public Type[] ExtraTargetTypes => new[] {
            typeof(UserStore<ApplicationUser, ApplicationRole, string, IdentityUserLogin, IdentityUserRole,
                IdentityUserClaim>),
            typeof(RoleStore<ApplicationRole, string, IdentityUserRole>)
        };

        public GenericTypeInfo[] ExtraTargetGenericTypes { get; }

        public Type[] ExtraDependencyTypesAlwaysFresh { get; }
        public GenericTypeInfo[] ExtraDependencyGenericTypesAlwaysFresh { get; }

        public Type[] ExtraDependencyTypesPerRequest => new[] {
            typeof(DbContext),
            typeof(UserManager<ApplicationUser, string>),
            typeof(RoleManager<ApplicationRole, string>),
            typeof(IUserStore<ApplicationUser, string>),
            typeof(IRoleStore<ApplicationRole, string>),
            typeof(IEmailService),
            typeof(ISmsService),
            typeof(SignInManager<ApplicationUser, string>)
        };

        public GenericTypeInfo[] ExtraDependencyGenericTypesPerRequest { get; }

        public Type[] ExtraDependencyTypesGlobalSingleton { get; }
        public GenericTypeInfo[] ExtraDependencyGenericTypesGlobalSingleton { get; }

        public void OnBindingType(Type targetType, Type[] serviceTypes, DependencyScope scope, bool isIgnored) {
            // no need for handler
        }

        public void OnTypeBound(Type targetType, Type[] serviceTypes, DependencyScope scope) {
            // no need for handler
        }
    }
}