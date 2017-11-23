using System.Collections.Generic;
using JrzAsp.Lib.ProtoCms.Datum.Models;
using JrzAsp.Lib.ProtoCms.Datum.Services;
using WebApp.Models;

namespace WebApp.Features.ApplicationDatumTypes {
    public class AppDatumTypesProvider : IDatumTypesProvider {
        public IEnumerable<DatumType> DefineDatumTypeInfos() {
            yield return new DatumType<ApplicationRole>(
                "role",
                "User Role",
                "User roles used for grouping users access permissions.",
                new [] {
                    nameof(ApplicationRole.Name)
                },
                new [] {
                    nameof(ApplicationRole.Name),
                    nameof(ApplicationRole.Description),
                    nameof(ApplicationRole.CreatedUtc),
                    nameof(ApplicationRole.UpdatedUtc),
                    nameof(ApplicationRole.Id),
                },
                nameof(ApplicationRole.Name),
                false,
                null,
                null);

            yield return new DatumType<ApplicationUser>(
                "user",
                "User",
                "The website user.",
                new [] {
                    nameof(ApplicationUser.DisplayName),
                    nameof(ApplicationUser.Email),
                    nameof(ApplicationUser.IsActivated),
                    nameof(ApplicationUser.UserName),
                },
                new [] {
                    nameof(ApplicationUser.IsActivated),
                    nameof(ApplicationUser.DisplayName),
                    nameof(ApplicationUser.UserName),
                    nameof(ApplicationUser.Roles),
                    nameof(ApplicationUser.Email),
                    nameof(ApplicationUser.PhoneNumber),
                    nameof(ApplicationUser.PhotoUrl),
                    nameof(ApplicationUser.CreatedUtc),
                    nameof(ApplicationUser.UpdatedUtc),
                    nameof(ApplicationUser.Id),
                },
                nameof(ApplicationUser.DisplayName),
                false,
                null,
                null);
        }
    }
}