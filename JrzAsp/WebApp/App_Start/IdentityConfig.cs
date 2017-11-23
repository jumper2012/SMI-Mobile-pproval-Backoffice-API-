using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using JrzAsp.Lib.RepoPattern;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Microsoft.Owin.Security;
using JrzAsp.Lib.ProtoCms.Role.Models;
using JrzAsp.Lib.ProtoCms.Role.Services;
using JrzAsp.Lib.ProtoCms.User.Models;
using JrzAsp.Lib.ProtoCms.User.Services;
using Simplify.Mail;
using WebApp.Models;

namespace WebApp {
    // Configure the application user manager used in this application. UserManager is defined in ASP.NET Identity and is used by the application.
    public class ApplicationUserManager : UserManager<ApplicationUser, string>, IProtoUserManager {
        public ApplicationUserManager(
            IUserStore<ApplicationUser, string> store, IEmailService emailService, ISmsService smsService)
            : base(store) {
            // Configure validation logic for usernames
            UserValidator = new UserValidator<ApplicationUser>(this) {
                AllowOnlyAlphanumericUserNames = false,
                RequireUniqueEmail = true
            };

            // Configure validation logic for passwords
            PasswordValidator = new PasswordValidator {
                RequiredLength = 6,
                RequireNonLetterOrDigit = true,
                RequireDigit = true,
                RequireLowercase = true,
                RequireUppercase = true
            };

            // Configure user lockout defaults
            UserLockoutEnabledByDefault = true;
            DefaultAccountLockoutTimeSpan = TimeSpan.FromMinutes(5);
            MaxFailedAccessAttemptsBeforeLockout = 5;

            EmailService = emailService;
            SmsService = smsService;

            InitTwoFactorSystem();
        }

        public IQueryable<ProtoUserProjection> ProtoUsers => from user in Users
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

        public ProtoUserProjection GetAuthenticatedProtoUser(HttpContextBase httpContext) {
            var user = this.FindById(httpContext.User?.Identity.GetUserId());
            if (user == null) return null;
            return new ProtoUserProjection(user);
        }

        public IEnumerable<string> GetProtoRoleNames(string userId) {
            return this.GetRoles(userId);
        }

        public string HashProtoUserPassword(string plainPassword) {
            return PasswordHasher.HashPassword(plainPassword);
        }

        public bool VerifyProtoUserPassword(string plainPassword, string hashedPassword) {
            var result = PasswordHasher.VerifyHashedPassword(hashedPassword, plainPassword);
            return result != PasswordVerificationResult.Failed;
        }

        public FurtherValidationResult CreateProtoUser(IProtoUser user) {
            var ir = CreateAsync((ApplicationUser) user).Result;
            if (ir.Succeeded) return FurtherValidationResult.Ok;
            return new FurtherValidationResult {
                Errors = {
                    [FurtherValidationResult.GLOBAL_ERROR_FIELD_NAME] = ir.Errors.ToArray()
                }
            };
        }

        public FurtherValidationResult UpdateProtoUser(IProtoUser user) {
            var ir = UpdateAsync((ApplicationUser) user).Result;
            if (ir.Succeeded) return FurtherValidationResult.Ok;
            return new FurtherValidationResult {
                Errors = {
                    [FurtherValidationResult.GLOBAL_ERROR_FIELD_NAME] = ir.Errors.ToArray()
                }
            };
        }

        public FurtherValidationResult DeleteProtoUser(IProtoUser user) {
            var ir = DeleteAsync((ApplicationUser) user).Result;
            if (ir.Succeeded) return FurtherValidationResult.Ok;
            return new FurtherValidationResult {
                Errors = {
                    [FurtherValidationResult.GLOBAL_ERROR_FIELD_NAME] = ir.Errors.ToArray()
                }
            };
        }

        public void InitTwoFactorSystem() {
            // Register two factor authentication providers. This application uses Phone and Emails as a step of receiving a code for verifying the user
            // You can write your own provider and plug it in here.
            RegisterTwoFactorProvider("Phone Code", new PhoneNumberTokenProvider<ApplicationUser> {
                MessageFormat = "Your security code is {0}"
            });
            RegisterTwoFactorProvider("Email Code", new EmailTokenProvider<ApplicationUser> {
                Subject = "Security Code",
                BodyFormat = "Your security code is {0}"
            });
        }
    }

    // Configure the application sign-in manager which is used in this application.
    public class ApplicationSignInManager : SignInManager<ApplicationUser, string> {
        public ApplicationSignInManager(ApplicationUserManager userManager,
            IAuthenticationManager authenticationManager)
            : base(userManager, authenticationManager) { }

        public override Task<ClaimsIdentity> CreateUserIdentityAsync(ApplicationUser user) {
            return user.GenerateUserIdentityAsync((ApplicationUserManager) UserManager);
        }

        public static ApplicationSignInManager Create(IdentityFactoryOptions<ApplicationSignInManager> options,
            IOwinContext context) {
            return new ApplicationSignInManager(context.GetUserManager<ApplicationUserManager>(),
                context.Authentication);
        }
    }

    // Role manager
    public class ApplicationRoleManager : RoleManager<ApplicationRole, string>, IProtoRoleManager {

        public ApplicationRoleManager(IRoleStore<ApplicationRole, string> store) : base(store) { }

        public IQueryable<ProtoRoleProjection> ProtoRoles => from role in Roles
            select new ProtoRoleProjection {
                Id = role.Id,
                Name = role.Name,
                Description = role.Description,
                CreatedUtc = role.CreatedUtc,
                UpdatedUtc = role.UpdatedUtc
            };

        public FurtherValidationResult CreateProtoRole(IProtoRole role) {
            var ir = CreateAsync((ApplicationRole) role).Result;
            if (ir.Succeeded) return FurtherValidationResult.Ok;
            return new FurtherValidationResult {
                Errors = {
                    [FurtherValidationResult.GLOBAL_ERROR_FIELD_NAME] = ir.Errors.ToArray()
                }
            };
        }

        public FurtherValidationResult UpdateProtoRole(IProtoRole role) {
            var ir = UpdateAsync((ApplicationRole) role).Result;
            if (ir.Succeeded) return FurtherValidationResult.Ok;
            return new FurtherValidationResult {
                Errors = {
                    [FurtherValidationResult.GLOBAL_ERROR_FIELD_NAME] = ir.Errors.ToArray()
                }
            };
        }

        public FurtherValidationResult DeleteProtoRole(IProtoRole role) {
            var ir = DeleteAsync((ApplicationRole) role).Result;
            if (ir.Succeeded) return FurtherValidationResult.Ok;
            return new FurtherValidationResult {
                Errors = {
                    [FurtherValidationResult.GLOBAL_ERROR_FIELD_NAME] = ir.Errors.ToArray()
                }
            };
        }
    }

    public interface IEmailService : IIdentityMessageService {}
    
    public class EmailService : IEmailService {
        public async Task SendAsync(IdentityMessage message) {
            // Plug in your email service here to send an email.
            await MailSender.Default.SendAsync(GlobalWebConfig.MailSender_DefaultFromEmailAddress, message.Destination,
                message.Subject, message.Body);
        }
    }

    public interface ISmsService : IIdentityMessageService { }

    public class SmsService : ISmsService {
        public Task SendAsync(IdentityMessage message) {
            // Plug in your SMS service here to send a text message.
            return Task.FromResult(0);
        }
    }
}