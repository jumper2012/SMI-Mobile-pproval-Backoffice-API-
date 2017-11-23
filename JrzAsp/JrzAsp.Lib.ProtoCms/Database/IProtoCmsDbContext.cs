using System.Data.Entity;
using System.Linq;
using JrzAsp.Lib.ProtoCms.Content.Models;
using JrzAsp.Lib.ProtoCms.OAuth.Models;
using JrzAsp.Lib.ProtoCms.Permission.Models;
using JrzAsp.Lib.ProtoCms.Role.Models;
using JrzAsp.Lib.ProtoCms.Setting.Database;
using JrzAsp.Lib.ProtoCms.User.Models;

namespace JrzAsp.Lib.ProtoCms.Database {
    /// <summary>
    ///     Required database model interface to enable ProtoCMS power
    /// </summary>
    public interface IProtoCmsDbContext : IPerRequestDependency {
        /// <summary>
        ///     Queryable IDbSet of webapp users projected to ProtoUserProjection
        /// </summary>
        IQueryable<ProtoUserProjection> UsersProjected { get; }

        /// <summary>
        ///     Queryable IDbSet of webapp roles projected to ProtoRoleProjection
        /// </summary>
        IQueryable<ProtoRoleProjection> RolesProjected { get; }

        /// <summary>
        ///     PermissionsMap table
        /// </summary>
        IDbSet<ProtoPermissionsMap> ProtoPermissionsMaps { get; set; }

        /// <summary>
        ///     OAuth clients table for proto cms oauth config
        /// </summary>
        IDbSet<ProtoOAuthClient> ProtoOAuthClients { get; set; }

        /// <summary>
        ///     OAuth refresh token table for proto cms oauth config
        /// </summary>
        IDbSet<ProtoOAuthRefreshToken> ProtoOAuthRefreshTokens { get; set; }

        /// <summary>
        ///     ProtoContent table
        /// </summary>
        IDbSet<ProtoContent> ProtoContents { get; set; }

        /// <summary>
        ///     ProtoField table
        /// </summary>
        IDbSet<ProtoField> ProtoFields { get; set; }

        /// <summary>
        ///     ProtoSettingField table
        /// </summary>
        IDbSet<ProtoSettingField> ProtoSettingFields { get; set; }
    }

    /// <summary>
    ///     Extends IProtoCmsDbContext with more utility methods
    /// </summary>
    public static class ProtoCmsDbContextExtensions {
        /// <summary>
        ///     Get the 'this' of IProtoCmsDbContext as concrete DbContext object
        /// </summary>
        /// <param name="dbContext">An instance of IProtoCmsDbContext that should inherit from DbContext too</param>
        /// <returns>The DbContext object that is the same as 'this'</returns>
        public static DbContext ThisDbContext(this IProtoCmsDbContext dbContext) {
            return (DbContext) dbContext;
        }

        /// <summary>
        ///     Setup dbcontext for proto cms power
        /// </summary>
        /// <param name="modelBuilder"></param>
        public static void OnProtoCmsDbModelCreating(this DbModelBuilder modelBuilder) {
            modelBuilder.Entity<ProtoPermissionsMap>().ToTable(nameof(IProtoCmsDbContext.ProtoPermissionsMaps));
            modelBuilder.Entity<ProtoOAuthClient>().ToTable(nameof(IProtoCmsDbContext.ProtoOAuthClients));
            modelBuilder.Entity<ProtoOAuthRefreshToken>().ToTable(nameof(IProtoCmsDbContext.ProtoOAuthRefreshTokens));
            modelBuilder.Entity<ProtoContent>().ToTable(nameof(IProtoCmsDbContext.ProtoContents));
            modelBuilder.Entity<ProtoField>().ToTable(nameof(IProtoCmsDbContext.ProtoFields));
            modelBuilder.Entity<ProtoField>().Property(x => x.NumberValue).HasPrecision(21, 6);
            modelBuilder.Entity<ProtoSettingField>().ToTable(nameof(IProtoCmsDbContext.ProtoSettingFields));
            modelBuilder.Entity<ProtoSettingField>().Property(x => x.NumberValue).HasPrecision(21, 6);
        }
    }
}