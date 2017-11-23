using System.Linq;
using JrzAsp.Lib.ProtoCms.Role.Models;
using JrzAsp.Lib.RepoPattern;

namespace JrzAsp.Lib.ProtoCms.Role.Services {
    public interface IProtoRoleManager : IPerRequestDependency {

        /// <summary>
        ///     Get a queryable of proto role
        /// </summary>
        IQueryable<ProtoRoleProjection> ProtoRoles { get; }

        /// <summary>
        ///     Create proto role and save it to the db
        /// </summary>
        /// <param name="role"></param>
        /// <returns></returns>
        FurtherValidationResult CreateProtoRole(IProtoRole role);

        /// <summary>
        ///     Update proto role and save it to the db
        /// </summary>
        /// <param name="role"></param>
        /// <returns></returns>
        FurtherValidationResult UpdateProtoRole(IProtoRole role);

        /// <summary>
        ///     Delete proto role and save it to the db
        /// </summary>
        /// <param name="role"></param>
        /// <returns></returns>
        FurtherValidationResult DeleteProtoRole(IProtoRole role);
    }
}