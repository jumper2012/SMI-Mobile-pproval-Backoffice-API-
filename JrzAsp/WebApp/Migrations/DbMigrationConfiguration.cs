using System;
using System.Data.Entity.Migrations;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using JrzAsp.Lib.ProtoCms.OAuth.Models;
using JrzAsp.Lib.ProtoCms.OAuth.Services;
using JrzAsp.Lib.ProtoCms.Role.Models;
using WebApp.Models;

namespace WebApp.Migrations {

    public sealed class DbMigrationConfiguration : DbMigrationsConfiguration<ApplicationDbContext> {
        public DbMigrationConfiguration() {
            AutomaticMigrationsEnabled = true;
            AutomaticMigrationDataLossAllowed = GlobalWebConfig.DbMigration_AllowDataLoss;
        }

        protected override void Seed(ApplicationDbContext context) {
            //  This method will be called after migrating to the latest version.

            //  You can use the DbSet<T>.AddOrUpdate() helper extension method 
            //  to avoid creating duplicate seed data. E.g.
            //
            //    context.People.AddOrUpdate(
            //      p => p.FullName,
            //      new Person { FullName = "Andrew Peters" },
            //      new Person { FullName = "Brice Lambson" },
            //      new Person { FullName = "Rowan Miller" }
            //    );
            //

            // seed required proto cms roles
            context.Roles.AddOrUpdate(
                d => d.Name,
                new ApplicationRole {
                    Name = RequiredRoleInfo.SuperAdmin.Name,
                    Description = RequiredRoleInfo.SuperAdmin.Description
                },
                new ApplicationRole {
                    Name = RequiredRoleInfo.Authenticated.Name,
                    Description = RequiredRoleInfo.Authenticated.Description
                },
                new ApplicationRole {
                    Name = RequiredRoleInfo.Guest.Name,
                    Description = RequiredRoleInfo.Guest.Description
                });

            // seed default superadmin user
            var userMgr =
                new ApplicationUserManager(
                    new UserStore<ApplicationUser, ApplicationRole, string, IdentityUserLogin, IdentityUserRole,
                        IdentityUserClaim>(context), null, null);
            var superadminUser = userMgr.FindByName(RequiredRoleInfo.SuperAdmin.Name);
            if (superadminUser == null) {
                superadminUser = new ApplicationUser {
                    UserName = RequiredRoleInfo.SuperAdmin.Name,
                    DisplayName = "Super Administrator",
                    IsActivated = true,
                    Email = "superadmin@example.com"
                };
                var superadminPassword = "P@ssw0rD;";
                var result = userMgr.Create(superadminUser, superadminPassword);
                if (!result.Succeeded) throw new Exception(string.Join(";\n", result.Errors));
            }
            if (!userMgr.IsInRole(superadminUser.Id, RequiredRoleInfo.SuperAdmin.Name)) {
                var result = userMgr.AddToRole(superadminUser.Id, RequiredRoleInfo.SuperAdmin.Name);
                if (!result.Succeeded) throw new Exception(string.Join(";\n", result.Errors));
            }

            // seed default proto cms oauth client
            var oauthMgr = new ProtoOAuthManager(context);
            context.ProtoOAuthClients.AddOrUpdate(
                d => d.Id,
                new ProtoOAuthClient {
                    Id = "ProtoCMS-Vue-Frontend",
                    SecretHash = oauthMgr.HashSecret("pR0toCMs;"),
                    Name = "ProtoCMS Vue Frontend",
                    ApplicationType = ProtoOAuthApplicationType.NonConfidential,
                    IsActive = true,
                    RefreshTokenLifetimeSeconds = 14400, // 4 hours
                    AllowedOriginsCsv = "*"
                }
            );
        }
    }
}