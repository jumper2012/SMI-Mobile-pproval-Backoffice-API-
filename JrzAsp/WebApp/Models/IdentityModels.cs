using System;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using JrzAsp.Lib.ProtoCms;
using JrzAsp.Lib.ProtoCms.Content.Models;
using JrzAsp.Lib.ProtoCms.Database;
using JrzAsp.Lib.ProtoCms.FileSys;
using JrzAsp.Lib.ProtoCms.OAuth.Models;
using JrzAsp.Lib.ProtoCms.Permission.Models;
using JrzAsp.Lib.ProtoCms.Role.Models;
using JrzAsp.Lib.ProtoCms.Setting.Database;
using JrzAsp.Lib.ProtoCms.User.Models;
using JrzAsp.Lib.RazorTools;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace WebApp.Models {
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit https://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class ApplicationUser : IdentityUser, IProtoUser {

        private static IDependencyProvider _dp => ProtoEngine.GetDependencyProvider();
        private static IFileExplorerHandler _fileHandler => _dp.GetService<IFileExplorerManager>().GetHandler();

        private DateTime _createdUtc;

        private DateTime _updatedUtc;

        public ApplicationUser() {
            CreatedUtc = DateTime.UtcNow;
            UpdatedUtc = CreatedUtc;
            SecurityStamp = Guid.NewGuid().ToString("N");
            IsActivated = true;
        }

        [Required]
        public DateTime CreatedUtc {
            get => DateTime.SpecifyKind(_createdUtc, DateTimeKind.Utc);
            set => _createdUtc = value;
        }

        [Required]
        public DateTime UpdatedUtc {
            get => DateTime.SpecifyKind(_updatedUtc, DateTimeKind.Utc);
            set => _updatedUtc = value;
        }

        [Required]
        [MaxLength(256)]
        public string DisplayName { get; set; }

        [Required]
        public bool IsActivated { get; set; }

        [MaxLength(256)]
        public string PhotoUrl { get; set; }

        public string GetPhotoDownloadUrl(string extraQueryString = null) {
            string photoDownloadUrl = "";
            if (PhotoUrl == null) {
                photoDownloadUrl = UrlHelperGlobal.Self.Content("~/Content/MetronicTheme/assets/layouts/layout/img/avatar.png");
            }
            else if (PhotoUrl.StartsWith("http://") || PhotoUrl.StartsWith("https://")) {
                photoDownloadUrl = PhotoUrl;
            } else {
                var photoRealPath = _fileHandler.GetRealPath(PhotoUrl);
                photoDownloadUrl = UrlHelperGlobal.Self.Content("/") + _fileHandler.GetDownloadPath(photoRealPath);
            }
            var extra = "";
            if (extraQueryString != null) {
                extra = photoDownloadUrl.Contains("?") ? $"&{extraQueryString}" : $"?{extraQueryString}";
            }
            return photoDownloadUrl + extra;
        }

        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser, string> manager) {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, GlobalWebConfig.Authentication_CookieAuthName);
            // Add custom user claims here
            return userIdentity;
        }
    }

    public class ApplicationRole : IdentityRole, IProtoRole {

        private DateTime _createdUtc;

        private DateTime _updatedUtc;

        public ApplicationRole() {
            CreatedUtc = DateTime.UtcNow;
            UpdatedUtc = CreatedUtc;
        }

        [Required]
        public DateTime CreatedUtc {
            get => DateTime.SpecifyKind(_createdUtc, DateTimeKind.Utc);
            set => _createdUtc = value;
        }

        [Required]
        public DateTime UpdatedUtc {
            get => DateTime.SpecifyKind(_updatedUtc, DateTimeKind.Utc);
            set => _updatedUtc = value;
        }

        [MaxLength(512)]
        public string Description { get; set; }
    }

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, string, IdentityUserLogin,
        IdentityUserRole, IdentityUserClaim>, IProtoCmsDbContext {
        public ApplicationDbContext()
            : base("DefaultConnection") { }

        public IQueryable<ProtoUserProjection> UsersProjected => from user in Users
            select new ProtoUserProjection {
                CreatedUtc = user.CreatedUtc,
                DisplayName = user.UserName,
                Id = user.Id,
                IsActivated = user.IsActivated,
                PasswordHash = user.PasswordHash,
                PhotoUrl = user.PhotoUrl,
                UpdatedUtc = user.UpdatedUtc,
                UserName = user.UserName
            };

        public IQueryable<ProtoRoleProjection> RolesProjected => from role in Roles
            select new ProtoRoleProjection {
                CreatedUtc = role.CreatedUtc,
                Description = role.Description,
                Id = role.Id,
                Name = role.Name,
                UpdatedUtc = role.UpdatedUtc
            };

        public IDbSet<ProtoPermissionsMap> ProtoPermissionsMaps { get; set; }
        public IDbSet<ProtoOAuthClient> ProtoOAuthClients { get; set; }
        public IDbSet<ProtoOAuthRefreshToken> ProtoOAuthRefreshTokens { get; set; }
        public IDbSet<ProtoContent> ProtoContents { get; set; }
        public IDbSet<ProtoField> ProtoFields { get; set; }
        public IDbSet<ProtoSettingField> ProtoSettingFields { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder) {
            base.OnModelCreating(modelBuilder);
            modelBuilder.OnProtoCmsDbModelCreating();
        }
    }
}