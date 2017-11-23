using System.Collections.Generic;
using System.Linq;
using System.Web;
using JrzAsp.Lib.ProtoCms.User.Models;
using JrzAsp.Lib.RepoPattern;

namespace JrzAsp.Lib.ProtoCms.User.Services {
    /// <summary>
    ///     Required interface to be attached to webapp's UserManager class (of ASP .NET Identity)
    ///     to enable ProtoCMS to check user data
    /// </summary>
    public interface IProtoUserManager : IPerRequestDependency {

        /// <summary>
        ///     Get a queryable of proto user
        /// </summary>
        IQueryable<ProtoUserProjection> ProtoUsers { get; }

        /// <summary>
        ///     Get current authenticated user
        /// </summary>
        /// <param name="httpContext">HTTP context</param>
        /// <returns>Null if no authenticated user or an instance of ProtoUserProjection if there's any authenticated user.</returns>
        ProtoUserProjection GetAuthenticatedProtoUser(HttpContextBase httpContext);

        /// <summary>
        ///     Get role names of user given a user id
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        IEnumerable<string> GetProtoRoleNames(string userId);

        /// <summary>
        ///     Get hash of user password
        /// </summary>
        /// <param name="plainPassword"></param>
        /// <returns></returns>
        string HashProtoUserPassword(string plainPassword);

        /// <summary>
        ///     Verify hashed password 'match' plain password
        /// </summary>
        /// <param name="hashedPassword"></param>
        /// <returns></returns>
        bool VerifyProtoUserPassword(string plainPassword, string hashedPassword);

        /// <summary>
        ///     Create proto user and save it to the db
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        FurtherValidationResult CreateProtoUser(IProtoUser user);

        /// <summary>
        ///     Update proto user and save it to the db
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        FurtherValidationResult UpdateProtoUser(IProtoUser user);

        /// <summary>
        ///     Delete proto user and save it to the db
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        FurtherValidationResult DeleteProtoUser(IProtoUser user);
    }
}