using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using JrzAsp.Lib.ProtoCms.OAuth.Models;
using JrzAsp.Lib.ProtoCms.OAuth.Services;
using JrzAsp.Lib.ProtoCms.User.Models;
using JrzAsp.Lib.ProtoCms.User.Services;
using JrzAsp.Lib.TypeUtilities;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.OAuth;

namespace JrzAsp.Lib.ProtoCms.OAuth.Providers {
    public class ProtoCmsAuthorizationServerProvider : OAuthAuthorizationServerProvider {
        private IDependencyProvider DepPro => ProtoEngine.GetDependencyProvider();

        private IProtoUserManager UserMgr => DepPro.GetService(typeof(IProtoUserManager))
            .DirectCastTo<IProtoUserManager>();

        private IProtoOAuthManager OAuthMgr => DepPro.GetService(typeof(IProtoOAuthManager))
            .DirectCastTo<IProtoOAuthManager>();

        public override Task ValidateClientAuthentication(OAuthValidateClientAuthenticationContext context) {
            if (!context.TryGetBasicCredentials(out var clientId, out var clientSecret)) {
                context.TryGetFormCredentials(out clientId, out clientSecret);
            }

            if (context.ClientId == null) {
                context.SetError("invalid_clientId", "ClientId should be sent.");
                return Task.FromResult(-1);
            }

            var client = OAuthMgr.FindClient(context.ClientId);

            if (client == null) {
                context.SetError("invalid_clientId",
                    $"Client '{context.ClientId}' is not registered in the system.");
                return Task.FromResult(-2);
            }

            if (client.ApplicationType == ProtoOAuthApplicationType.Confidential) {
                if (string.IsNullOrWhiteSpace(clientSecret)) {
                    context.SetError("invalid_clientId", "Client secret should be sent.");
                    return Task.FromResult(-3);
                }
                if (!OAuthMgr.VerifySecretHash(client.SecretHash, clientSecret)) {
                    context.SetError("invalid_clientId", "Client secret is invalid.");
                    return Task.FromResult(-4);
                }
            }

            if (!client.IsActive) {
                context.SetError("invalid_clientId", "Client is inactive.");
                return Task.FromResult(-5);
            }

            context.OwinContext.Set("as:clientAllowedOrigin", client.AllowedOriginsCsv);
            context.OwinContext.Set("as:clientRefreshTokenLifeTime",
                client.RefreshTokenLifetimeSeconds.ToString());

            context.Validated();
            return Task.FromResult(0);
        }

        public override Task GrantResourceOwnerCredentials(OAuthGrantResourceOwnerCredentialsContext context) {
            var allowedOrigins =
                ProtoOAuthClient.GetAllowedOrigins(context.OwinContext.Get<string>("as:clientAllowedOrigin")).ToArray();

            context.OwinContext.Response.Headers.Add("Access-Control-Allow-Origin", allowedOrigins);
            var user = UserMgr.ProtoUsers.FirstOrDefault(u => u.UserName == context.UserName);
            var passwordOk = user != null && UserMgr.VerifyProtoUserPassword(context.Password, user.PasswordHash);

            if (user == null || !passwordOk || !user.IsActivated) {
                context.SetError("invalid_grant", "The user name or password is incorrect.");
                return Task.FromResult(-1);
            }

            var identity = GenerateUserIdentity(context, user, UserMgr);
            var props = new AuthenticationProperties(new Dictionary<string, string> {
                ["as:clientId"] = context.ClientId ?? string.Empty,
                ["userId"] = user.Id,
                ["userName"] = context.UserName,
                [ClaimTypes.NameIdentifier] = user.Id,
                [ClaimTypes.Name] = user.UserName
            });

            var ticket = new AuthenticationTicket(identity, props);
            context.Validated(ticket);
            return Task.FromResult(0);
        }

        public override Task TokenEndpoint(OAuthTokenEndpointContext context) {
            foreach (var property in context.Properties.Dictionary) {
                context.AdditionalResponseParameters.Add(property.Key, property.Value);
            }

            return Task.FromResult<object>(null);
        }

        public static ClaimsIdentity GenerateUserIdentity(OAuthGrantResourceOwnerCredentialsContext context,
            IProtoUser user, IProtoUserManager userMgr) {
            var identity = new ClaimsIdentity(context.Options.AuthenticationType);
            identity.AddClaim(new Claim("sub", context.UserName));
            identity.AddClaim(new Claim("userId", user.Id));
            identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, user.Id));
            identity.AddClaim(new Claim("userName", user.UserName));
            identity.AddClaim(new Claim(ClaimTypes.Name, user.UserName));
            identity.AddClaim(new Claim("displayName", user.DisplayName ?? user.UserName));
            identity.AddClaim(new Claim(ClaimTypes.GivenName, user.DisplayName ?? user.UserName));
            foreach (var r in userMgr.GetProtoRoleNames(user.Id)) {
                identity.AddClaim(new Claim(ClaimTypes.Role, r));
                identity.AddClaim(new Claim("role", r));
            }
            return identity;
        }
    }
}