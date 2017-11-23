using System.Linq;
using JrzAsp.Lib.ProtoCms.OAuth.Models;

namespace JrzAsp.Lib.ProtoCms.OAuth.Services {
    public interface IProtoOAuthManager : IPerRequestDependency {

        IQueryable<ProtoOAuthClient> Clients { get; }
        IQueryable<ProtoOAuthRefreshToken> RefreshTokens { get; }

        ProtoOAuthClient FindClient(string clientId);
        bool AddRefreshToken(ProtoOAuthRefreshToken refreshToken);
        bool RemoveRefreshToken(string refreshTokenId);
        bool RemoveRefreshToken(ProtoOAuthRefreshToken refreshToken);
        ProtoOAuthRefreshToken FindRefreshToken(string refreshTokenId);

        string HashSecret(string secret);
        bool VerifySecretHash(string secretHash, string secret);
    }
}