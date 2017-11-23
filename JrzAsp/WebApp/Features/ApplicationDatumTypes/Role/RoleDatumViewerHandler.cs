using System;
using System.Collections.Generic;
using System.Linq;
using JrzAsp.Lib.ProtoCms.Content.Models;
using JrzAsp.Lib.ProtoCms.Datum.Services;
using JrzAsp.Lib.ProtoCms.Permission.Services;
using JrzAsp.Lib.ProtoCms.Vue.Models;
using WebApp.Models;

namespace WebApp.Features.ApplicationDatumTypes.Role {
    public class RoleDatumViewerHandler : BaseDatumViewerHandler<ApplicationRole> {
        private readonly IPermissionManager _permMgr;

        public RoleDatumViewerHandler(IPermissionManager permMgr) {
            _permMgr = permMgr;
        }

        public override decimal Priority => 0;

        public override string[] GetValidFieldNames() {
            return new[] {
                nameof(ApplicationRole.Name),
                nameof(ApplicationRole.Description),
                "Permissions",
                nameof(ApplicationRole.CreatedUtc),
                nameof(ApplicationRole.UpdatedUtc),
                nameof(ApplicationRole.Id),
            };
        }

        public override ContentTableHeader GetTableHeader(string fieldName) {
            switch (fieldName) {
                case nameof(ApplicationRole.Name):
                    return new ContentTableHeader(nameof(ApplicationRole.Name), "Name", true);
                case nameof(ApplicationRole.Description):
                    return new ContentTableHeader(nameof(ApplicationRole.Description), "Description", false);
                case "Permissions":
                    return new ContentTableHeader("Permissions", "Permissions", false);
                case nameof(ApplicationRole.CreatedUtc):
                    return new ContentTableHeader(nameof(ApplicationRole.CreatedUtc), "Created UTC", true);
                case nameof(ApplicationRole.UpdatedUtc):
                    return new ContentTableHeader(nameof(ApplicationRole.UpdatedUtc), "Updated UTC", true);
                case nameof(ApplicationRole.Id):
                    return new ContentTableHeader(nameof(ApplicationRole.Id), "Id", true);
            }
            return null;
        }

        public override string GetSummarizedValue(ApplicationRole datum, string fieldName) {
            switch (fieldName) {
                case nameof(ApplicationRole.Name):
                    return datum.Name;
                case nameof(ApplicationRole.Description):
                    return datum.Description;
                case nameof(ApplicationRole.CreatedUtc):
                    return datum.CreatedUtc.ToString(ProtoContent.DATE_FORMAT_FOR_DISPLAY);
                case nameof(ApplicationRole.UpdatedUtc):
                    return datum.UpdatedUtc.ToString(ProtoContent.DATE_FORMAT_FOR_DISPLAY);
                case nameof(ApplicationRole.Id):
                    return datum.Id;
            }
            return null;
        }

        public override VueComponentDefinition[] GetTableRowVue(ApplicationRole datum, string fieldName) {
            switch (fieldName) {
                case nameof(ApplicationRole.Name):
                    return new VueComponentDefinition[] {
                        new VueHtmlWidget(datum.Name)
                    };
                case nameof(ApplicationRole.Description):
                    return new VueComponentDefinition[] {
                        new VueHtmlWidget(datum.Description)
                    };
                case "Permissions":
                    var permHead = new[] {"Category", "Sub Category", "Name", "Description", "Id"};
                    var permBody = new List<string[]>();
            
                    var permIds = _permMgr.GetRolesPermissions(new[] {datum.Name}).ToArray();
                    foreach (var perm in _permMgr.AllPermissions) {
                        foreach (var permId in permIds) {
                            if (perm.Id != permId) continue;

                            permBody.Add(new[] {
                                perm.CategoryName, perm.SubCategoryName, perm.DisplayName, perm.Description,
                                $"<code>{perm.Id}</code>"
                            });
                        }
                    }

                    var permVue = permBody.Count > 0
                        ? (VueComponentDefinition) new VueTableWidget(permHead, permBody.OrderBy(x => x[0]).ThenBy(x => x[1]))
                        : new VueHtmlWidget("-n/a-");

                    return new [] {permVue};
                case nameof(ApplicationRole.CreatedUtc):
                    return new VueComponentDefinition[] {
                        new VueHtmlWidget(datum.CreatedUtc.ToString(ProtoContent.DATE_FORMAT_FOR_DISPLAY))
                    };
                case nameof(ApplicationRole.UpdatedUtc):
                    return new VueComponentDefinition[] {
                        new VueHtmlWidget(datum.UpdatedUtc.ToString(ProtoContent.DATE_FORMAT_FOR_DISPLAY))
                    };
                case nameof(ApplicationRole.Id):
                    return new VueComponentDefinition[] {
                        new VueHtmlWidget(datum.Id)
                    };
            }
            return null;
        }

        public override ContentPreviewPart GetFullPreview(ApplicationRole datum, string fieldName) {
            switch (fieldName) {
                case nameof(ApplicationRole.Name):
                    return new ContentPreviewPart("Name", fieldName, GetTableRowVue(datum, fieldName));
                case nameof(ApplicationRole.Description):
                    return new ContentPreviewPart("Description", fieldName, GetTableRowVue(datum, fieldName));
                case "Permissions":
                    return new ContentPreviewPart("Permissions", fieldName, GetTableRowVue(datum, fieldName));
                case nameof(ApplicationRole.CreatedUtc):
                    return new ContentPreviewPart("Created UTC", fieldName, GetTableRowVue(datum, fieldName));
                case nameof(ApplicationRole.UpdatedUtc):
                    return new ContentPreviewPart("Updated UTC", fieldName, GetTableRowVue(datum, fieldName));
                case nameof(ApplicationRole.Id):
                    return new ContentPreviewPart("Id", fieldName, GetTableRowVue(datum, fieldName));
            }
            return null;
        }
    }
}