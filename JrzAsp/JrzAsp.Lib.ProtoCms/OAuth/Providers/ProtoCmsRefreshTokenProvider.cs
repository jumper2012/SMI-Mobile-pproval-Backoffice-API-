using System;
using System.Linq;
using System.Threading.Tasks;
using JrzAsp.Lib.ProtoCms.OAuth.Models;
using JrzAsp.Lib.ProtoCms.OAuth.Services;
using JrzAsp.Lib.TypeUtilities;
using Microsoft.Owin.Security.Infrastructure;

namespace JrzAsp.Lib.ProtoCms.OAuth.Providers {
    public class ProtoCmsRefreshTokenProvider : IAuthenticationTokenProvider {
        private IDependencyProvider DepPro => ProtoEngine.GetDependencyProvider();

        private IProtoOAuthManager OAuthMgr => DepPro.GetService(typeof(IProtoOAuthManager))
            .DirectCastTo<IProtoOAuthManager>();

        public void Create(AuthenticationTokenCreateContext context) {
            CreateAsync(context).Wait();
        }

        public Task CreateAsync(AuthenticationTokenCreateContext context) {
            var clientId = context.Ticket.Properties.Dictionary["as:clientId"];

            if (string.IsNullOrEmpty(clientId)) return Task.FromResult(-2);

            var refreshTokenId = Guid.NewGuid().ToString("n");

            var refreshTokenLifeTime = context.OwinContext.Get<string>("as:clientRefreshTokenLifeTime");

            var token = new ProtoOAuthRefreshToken {
                Id = OAuthMgr.HashSecret(refreshTokenId),
                ClientId = clientId,
                Subject = context.Ticket.Identity.Name,
                IssuedUtc = DateTime.UtcNow,
                ExpiresUtc = DateTime.UtcNow.AddSeconds(Convert.ToDouble(refreshTokenLifeTime))
            };

            context.Ticket.Properties.IssuedUtc = token.IssuedUtc;
            context.Ticket.Properties.ExpiresUtc = token.ExpiresUtc;

            token.ProtectedTicket = context.SerializeTicket();

            var result = OAuthMgr.AddRefreshToken(token);

            if (!result) return Task.FromResult(-1);

            context.SetToken(refreshTokenId);
            return Task.FromResult(0);
        }

        public void Receive(AuthenticationTokenReceiveContext context) {
            ReceiveAsync(context).Wait();
        }

        public Task ReceiveAsync(AuthenticationTokenReceiveContext context) {
            var allowedOrigins =
                ProtoOAuthClient.GetAllowedOrigins(context.OwinContext.Get<string>("as:clientAllowedOrigin")).ToArray();
            context.OwinContext.Response.Headers.Add("Access-Control-Allow-Origin", allowedOrigins);

            var hashedTokenId = OAuthMgr.HashSecret(context.Token);

            var refreshToken = OAuthMgr.FindRefreshToken(hashedTokenId);

            if (refreshToken != null) {
                //Get protectedTicket from refreshToken class
                context.DeserializeTicket(refreshToken.ProtectedTicket);
                var result = OAuthMgr.RemoveRefreshToken(hashedTokenId);
                return Task.FromResult(result ? 0 : -1);
            }
            return Task.FromResult(-2);
        }
    }
}