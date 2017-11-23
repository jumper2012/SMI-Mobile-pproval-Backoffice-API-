using System.Linq;
using JrzAsp.Lib.ProtoCms.Content.Models;
using JrzAsp.Lib.ProtoCms.Datum.Services;
using JrzAsp.Lib.ProtoCms.Vue.Models;
using Microsoft.AspNet.Identity;
using WebApp.Models;

namespace WebApp.Features.ApplicationDatumTypes.User {
    public class UserDatumViewerHandler : BaseDatumViewerHandler<ApplicationUser> {

        private readonly ApplicationUserManager _userMgr;

        public UserDatumViewerHandler(ApplicationUserManager userMgr) {
            _userMgr = userMgr;
        }

        public override decimal Priority => 0;

        public override string[] GetValidFieldNames() {
            return new[] {
                nameof(ApplicationUser.IsActivated),
                nameof(ApplicationUser.DisplayName),
                nameof(ApplicationUser.PhotoUrl),
                nameof(ApplicationUser.UserName),
                nameof(ApplicationUser.Email),
                nameof(ApplicationUser.EmailConfirmed),
                nameof(ApplicationUser.PhoneNumber),
                nameof(ApplicationUser.PhoneNumberConfirmed),
                nameof(ApplicationUser.Roles),
                nameof(ApplicationUser.CreatedUtc),
                nameof(ApplicationUser.UpdatedUtc),
                nameof(ApplicationUser.Id)
            };
        }

        public override ContentTableHeader GetTableHeader(string fieldName) {
            if (fieldName == nameof(ApplicationUser.IsActivated)) {
                return new ContentTableHeader(fieldName, "Is Activated?", true);
            }
            if (fieldName == nameof(ApplicationUser.DisplayName)) {
                return new ContentTableHeader(fieldName, "Display Name", true);
            }
            if (fieldName == nameof(ApplicationUser.PhotoUrl)) {
                return new ContentTableHeader(fieldName, "Photo", false);
            }
            if (fieldName == nameof(ApplicationUser.UserName)) {
                return new ContentTableHeader(fieldName, "User Name", true);
            }
            if (fieldName == nameof(ApplicationUser.Email)) {
                return new ContentTableHeader(fieldName, "Email", true);
            }
            if (fieldName == nameof(ApplicationUser.EmailConfirmed)) {
                return new ContentTableHeader(fieldName, "Email Confirmed", true);
            }
            if (fieldName == nameof(ApplicationUser.PhoneNumber)) {
                return new ContentTableHeader(fieldName, "Phone No.", true);
            }
            if (fieldName == nameof(ApplicationUser.PhoneNumberConfirmed)) {
                return new ContentTableHeader(fieldName, "Phone No. Confirmed", true);
            }
            if (fieldName == nameof(ApplicationUser.Roles)) {
                return new ContentTableHeader(fieldName, "Roles", false);
            }
            if (fieldName == nameof(ApplicationUser.CreatedUtc)) {
                return new ContentTableHeader(fieldName, "Created UTC", true);
            }
            if (fieldName == nameof(ApplicationUser.UpdatedUtc)) {
                return new ContentTableHeader(fieldName, "Updated UTC", true);
            }
            if (fieldName == nameof(ApplicationUser.Id)) {
                return new ContentTableHeader(fieldName, "Id", true);
            }
            return null;
        }

        public override string GetSummarizedValue(ApplicationUser datum, string fieldName) {
            if (fieldName == nameof(ApplicationUser.IsActivated)) {
                return datum.IsActivated ? "Activated" : "Deactivated";
            }
            if (fieldName == nameof(ApplicationUser.DisplayName)) {
                return datum.DisplayName;
            }
            if (fieldName == nameof(ApplicationUser.PhotoUrl)) {
                return datum.PhotoUrl;
            }
            if (fieldName == nameof(ApplicationUser.UserName)) {
                return datum.UserName;
            }
            if (fieldName == nameof(ApplicationUser.Email)) {
                return datum.Email;
            }
            if (fieldName == nameof(ApplicationUser.EmailConfirmed)) {
                return datum.EmailConfirmed ? "Email Confirmed" : "Email Unconfirmed";
            }
            if (fieldName == nameof(ApplicationUser.PhoneNumber)) {
                return datum.PhoneNumber;
            }
            if (fieldName == nameof(ApplicationUser.PhoneNumberConfirmed)) {
                return datum.PhoneNumberConfirmed ? "Phone Confirmed" : "Phone Unconfirmed";
            }
            if (fieldName == nameof(ApplicationUser.Roles)) {
                var roleNames = _userMgr.GetRoles(datum.Id).ToArray();
                return $"Roles: {string.Join(", ", roleNames)}";
            }
            if (fieldName == nameof(ApplicationUser.CreatedUtc)) {
                return datum.CreatedUtc.ToString(ProtoContent.DATE_FORMAT_FOR_DISPLAY);
            }
            if (fieldName == nameof(ApplicationUser.UpdatedUtc)) {
                return datum.UpdatedUtc.ToString(ProtoContent.DATE_FORMAT_FOR_DISPLAY);
            }
            if (fieldName == nameof(ApplicationUser.Id)) {
                return datum.Id;
            }
            return null;
        }

        public override VueComponentDefinition[] GetTableRowVue(ApplicationUser datum, string fieldName) {
            if (fieldName == nameof(ApplicationUser.IsActivated)) {
                return new VueComponentDefinition[] {
                    new VueHtmlWidget(
                        datum.IsActivated
                            ? "<i class='fa fa-check-square'></i> Activated"
                            : "<i class='fa fa-square-o'></i> Deactivated"
                    )
                };
            }
            if (fieldName == nameof(ApplicationUser.DisplayName)) {
                return new VueComponentDefinition[] {
                    new VueHtmlWidget(datum.DisplayName)
                };
            }
            if (fieldName == nameof(ApplicationUser.PhotoUrl)) {
                var photoUrl = datum.GetPhotoDownloadUrl();
                return new VueComponentDefinition[] {
                    new VueHtmlWidget($"<a href='{photoUrl}' " +
                                      $"target='_blank'>{datum.PhotoUrl}</a>")
                };
            }
            if (fieldName == nameof(ApplicationUser.UserName)) {
                return new VueComponentDefinition[] {
                    new VueHtmlWidget(datum.UserName)
                };
            }
            if (fieldName == nameof(ApplicationUser.Email)) {
                return new VueComponentDefinition[] {
                    new VueHtmlWidget(datum.Email)
                };
            }
            if (fieldName == nameof(ApplicationUser.EmailConfirmed)) {
                return new VueComponentDefinition[] {
                    new VueHtmlWidget(
                        datum.EmailConfirmed
                            ? "<i class='fa fa-check-square'></i> Yes"
                            : "<i class='fa fa-square-o'></i> No"
                    )
                };
            }
            if (fieldName == nameof(ApplicationUser.PhoneNumber)) {
                return new VueComponentDefinition[] {
                    new VueHtmlWidget(datum.PhoneNumber)
                };
            }
            if (fieldName == nameof(ApplicationUser.PhoneNumberConfirmed)) {
                return new VueComponentDefinition[] {
                    new VueHtmlWidget(
                        datum.PhoneNumberConfirmed
                            ? "<i class='fa fa-check-square'></i> Yes"
                            : "<i class='fa fa-square-o'></i> No"
                    )
                };
            }
            if (fieldName == nameof(ApplicationUser.Roles)) {
                var roleNames = _userMgr.GetRoles(datum.Id).ToArray();
                return new VueComponentDefinition[] {
                    new VueHtmlWidget($"<li>{string.Join("</li><li>", roleNames)}</li>")
                };
            }
            if (fieldName == nameof(ApplicationUser.CreatedUtc)) {
                return new VueComponentDefinition[] {
                    new VueHtmlWidget(datum.CreatedUtc.ToString(ProtoContent.DATE_FORMAT_FOR_DISPLAY))
                };
            }
            if (fieldName == nameof(ApplicationUser.UpdatedUtc)) {
                return new VueComponentDefinition[] {
                    new VueHtmlWidget(datum.UpdatedUtc.ToString(ProtoContent.DATE_FORMAT_FOR_DISPLAY))
                };
            }
            if (fieldName == nameof(ApplicationUser.Id)) {
                return new VueComponentDefinition[] {
                    new VueHtmlWidget(datum.Id)
                };
            }
            return null;
        }

        public override ContentPreviewPart GetFullPreview(ApplicationUser datum, string fieldName) {
            if (fieldName == nameof(ApplicationUser.IsActivated)) {
                return new ContentPreviewPart("Is Activated?", fieldName, GetTableRowVue(datum, fieldName));
            }
            if (fieldName == nameof(ApplicationUser.DisplayName)) {
                return new ContentPreviewPart("Display Name", fieldName, GetTableRowVue(datum, fieldName));
            }
            if (fieldName == nameof(ApplicationUser.PhotoUrl)) {
                return new ContentPreviewPart("Photo", fieldName, GetTableRowVue(datum, fieldName));
            }
            if (fieldName == nameof(ApplicationUser.UserName)) {
                return new ContentPreviewPart("User Name", fieldName, GetTableRowVue(datum, fieldName));
            }
            if (fieldName == nameof(ApplicationUser.Email)) {
                return new ContentPreviewPart("Email", fieldName, GetTableRowVue(datum, fieldName));
            }
            if (fieldName == nameof(ApplicationUser.EmailConfirmed)) {
                return new ContentPreviewPart("Email Confirmed?", fieldName, GetTableRowVue(datum, fieldName));
            }
            if (fieldName == nameof(ApplicationUser.PhoneNumber)) {
                return new ContentPreviewPart("Phone No.", fieldName, GetTableRowVue(datum, fieldName));
            }
            if (fieldName == nameof(ApplicationUser.PhoneNumberConfirmed)) {
                return new ContentPreviewPart("Phone No. Confirmed", fieldName, GetTableRowVue(datum, fieldName));
            }
            if (fieldName == nameof(ApplicationUser.Roles)) {
                return new ContentPreviewPart("Roles", fieldName, GetTableRowVue(datum, fieldName));
            }
            if (fieldName == nameof(ApplicationUser.CreatedUtc)) {
                return new ContentPreviewPart("Created UTC", fieldName, GetTableRowVue(datum, fieldName));
            }
            if (fieldName == nameof(ApplicationUser.UpdatedUtc)) {
                return new ContentPreviewPart("Updated UTC", fieldName, GetTableRowVue(datum, fieldName));
            }
            if (fieldName == nameof(ApplicationUser.Id)) {
                return new ContentPreviewPart("Id", fieldName, GetTableRowVue(datum, fieldName));
            }
            return null;
        }
    }
}