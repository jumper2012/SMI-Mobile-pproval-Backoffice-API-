using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using JrzAsp.Lib.ProtoCms.Database;
using JrzAsp.Lib.ProtoCms.OAuth.Models;

namespace JrzAsp.Lib.ProtoCms.OAuth.Services {
    public class ProtoOAuthManager : IProtoOAuthManager {
        private readonly IProtoCmsDbContext _dbContext;

        public ProtoOAuthManager(IProtoCmsDbContext dbContext) {
            _dbContext = dbContext;
        }

        public IQueryable<ProtoOAuthClient> Clients => _dbContext.ProtoOAuthClients;
        public IQueryable<ProtoOAuthRefreshToken> RefreshTokens => _dbContext.ProtoOAuthRefreshTokens;

        public ProtoOAuthClient FindClient(string clientId) {
            var client = _dbContext.ProtoOAuthClients.Find(clientId);
            return client;
        }

        public bool AddRefreshToken(ProtoOAuthRefreshToken refreshToken) {
            var existingToken =
                _dbContext.ProtoOAuthRefreshTokens.SingleOrDefault(
                    r => r.Subject == refreshToken.Subject && r.ClientId == refreshToken.ClientId);

            if (existingToken != null) RemoveRefreshToken(existingToken);

            _dbContext.ProtoOAuthRefreshTokens.Add(refreshToken);

            return _dbContext.ThisDbContext().SaveChanges() > 0;
        }

        public bool RemoveRefreshToken(string refreshTokenId) {
            var refreshToken = _dbContext.ProtoOAuthRefreshTokens.Find(refreshTokenId);

            if (refreshToken != null) {
                _dbContext.ProtoOAuthRefreshTokens.Remove(refreshToken);
                return _dbContext.ThisDbContext().SaveChanges() > 0;
            }

            return false;
        }

        public bool RemoveRefreshToken(ProtoOAuthRefreshToken refreshToken) {
            _dbContext.ProtoOAuthRefreshTokens.Remove(refreshToken);
            return _dbContext.ThisDbContext().SaveChanges() > 0;
        }

        public ProtoOAuthRefreshToken FindRefreshToken(string refreshTokenId) {
            var refreshToken = _dbContext.ProtoOAuthRefreshTokens.Find(refreshTokenId);
            return refreshToken;
        }

        public string HashSecret(string secret) {
            using (var sha256 = new SHA256CryptoServiceProvider()) {
                var secretBytes = Encoding.UTF8.GetBytes(secret);
                var hashBytes = sha256.ComputeHash(secretBytes);
                return Convert.ToBase64String(hashBytes);
            }
        }

        public bool VerifySecretHash(string secretHash, string secret) {
            using (var sha256 = new SHA256CryptoServiceProvider()) {
                var secretBytes = Encoding.UTF8.GetBytes(secret);
                var hashBytes = sha256.ComputeHash(secretBytes);
                return Convert.ToBase64String(hashBytes) == secretHash;
            }
        }
    }
}