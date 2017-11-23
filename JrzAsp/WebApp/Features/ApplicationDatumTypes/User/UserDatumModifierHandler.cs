using System;
using System.Collections.Generic;
using System.Linq;
using JrzAsp.Lib.ProtoCms;
using JrzAsp.Lib.ProtoCms.Content.Forms;
using JrzAsp.Lib.ProtoCms.Core.Models;
using JrzAsp.Lib.ProtoCms.Datum.Models;
using JrzAsp.Lib.ProtoCms.Datum.Services;
using JrzAsp.Lib.ProtoCms.Role.Models;
using JrzAsp.Lib.ProtoCms.Vue.Models;
using JrzAsp.Lib.RepoPattern;
using JrzAsp.Lib.TypeUtilities;
using Microsoft.AspNet.Identity;
using WebApp.Models;

namespace WebApp.Features.ApplicationDatumTypes.User {
    public class UserDatumModifierHandler : BaseDatumModifierHandler<ApplicationUser> {
        public const string FORM_KEY = "ApplicationUser";
        private readonly ApplicationDbContext _dbContext;
        private readonly ApplicationRoleManager _roleMgr;
        private readonly ApplicationUserManager _userMgr;
        private readonly IProtoCmsMainUrlsProvider _urlProv;

        public UserDatumModifierHandler(ApplicationDbContext dbContext, ApplicationUserManager userMgr,
            ApplicationRoleManager roleMgr, IProtoCmsMainUrlsProvider urlProv) {
            _userMgr = userMgr;
            _dbContext = dbContext;
            _roleMgr = roleMgr;
            _urlProv = urlProv;
        }

        public override decimal Priority => 0;

        public override IDictionary<string, ContentModifierForm> BuildModifierForm(ApplicationUser datum,
            DatumModifyOperation operation, Type datumType) {
            if (!operation.IsCreateOrUpdateOperation()) return null;
            var f = new UserDatumModifierForm {
                Id = datum.Id,
                UserName = datum.UserName,
                IsActivated = datum.IsActivated,
                DisplayName = datum.DisplayName,
                PhotoUrl = !string.IsNullOrWhiteSpace(datum.PhotoUrl) ? new[] {datum.PhotoUrl} : null,
                Email = datum.Email,
                EmailConfirmed = datum.EmailConfirmed,
                PhoneNumber = datum.PhoneNumber,
                PhoneNumberConfirmed = datum.PhoneNumberConfirmed
            };
            if (operation.IsCreateOperation()) {
                f.ChangePassword = true;
            }
            if (operation.IsUpdateOperation()) {
                var roleIds = new List<string>();
                var roleNames = _userMgr.GetRoles(datum.Id);
                foreach (var rn in roleNames) {
                    var role = _roleMgr.Roles.FirstOrDefault(x => x.Name == rn);
                    if (role != null) {
                        roleIds.Add(role.Id);
                    }
                }
                f.RoleIds = roleIds.ToArray();
            }
            return new Dictionary<string, ContentModifierForm> {
                [FORM_KEY] = f
            };
        }

        public override IDictionary<string, VueComponentDefinition[]> ConvertFormToVues(
            IDictionary<string, ContentModifierForm> modifierForm, ApplicationUser datum,
            DatumModifyOperation operation,
            Type datumType) {
            if (operation.IsCreateOrUpdateOperation()) {
                var vues = new List<VueComponentDefinition>();
                vues.AddRange(new[] {
                    new VueComponentDefinition {
                        Name = "cms-form-field-hidden",
                        Props = new {
                            valuePath = nameof(UserDatumModifierForm.Id)
                        }
                    },
                    new VueComponentDefinition {
                        Name = "cms-form-field-text",
                        Props = new {
                            label = "UserName",
                            valuePath = nameof(UserDatumModifierForm.UserName)
                        }
                    },
                    new VueComponentDefinition {
                        Name = "cms-form-field-checkbox",
                        Props = new {
                            label = "Activated?",
                            valuePath = nameof(UserDatumModifierForm.IsActivated),
                            yesLabel = "Activated",
                            noLabel = "Deactivated"
                        }
                    }
                });
                if (operation.IsUpdateOperation()) {
                    vues.Add(new VueComponentDefinition {
                        Name = "cms-form-field-checkbox",
                        Props = new {
                            label = "Change Password?",
                            valuePath = nameof(UserDatumModifierForm.ChangePassword),
                            helpText = "Enable this to also change user password when updating data.",
                            yesLabel = "Change",
                            noLabel = "Don't Change"
                        }
                    });
                }
                vues.AddRange(new[] {
                    new VueComponentDefinition {
                        Name = "cms-form-field-password",
                        Props = new {
                            label = "Password",
                            valuePath = nameof(UserDatumModifierForm.Password)
                        }
                    },
                    new VueComponentDefinition {
                        Name = "cms-form-field-password",
                        Props = new {
                            label = "Password Confirmation",
                            valuePath = nameof(UserDatumModifierForm.PasswordConfirmation)
                        }
                    },
                    new VueComponentDefinition {
                        Name = "cms-form-field-text",
                        Props = new {
                            label = "Display Name",
                            valuePath = nameof(UserDatumModifierForm.DisplayName)
                        }
                    },
                    new VueComponentDefinition {
                        Name = "cms-form-field-file-picker",
                        Props = new {
                            label = "Photo",
                            valuePath = nameof(UserDatumModifierForm.PhotoUrl),
                            fileExplorerPageUrl = _urlProv.GenerateManageFileExplorerUrl()
                        }
                    },
                    new VueComponentDefinition {
                        Name = "cms-form-field-text",
                        Props = new {
                            label = "Email",
                            valuePath = nameof(UserDatumModifierForm.Email)
                        }
                    },
                    new VueComponentDefinition {
                        Name = "cms-form-field-checkbox",
                        Props = new {
                            label = "Email Confirmed?",
                            valuePath = nameof(UserDatumModifierForm.EmailConfirmed),
                            yesLabel = "Confirmed",
                            noLabel = "Not Confirmed"
                        }
                    },
                    new VueComponentDefinition {
                        Name = "cms-form-field-text",
                        Props = new {
                            label = "Phone No.",
                            valuePath = nameof(UserDatumModifierForm.PhoneNumber)
                        }
                    },
                    new VueComponentDefinition {
                        Name = "cms-form-field-checkbox",
                        Props = new {
                            label = "Phone No. Confirmed?",
                            valuePath = nameof(UserDatumModifierForm.PhoneNumberConfirmed),
                            yesLabel = "Confirmed",
                            noLabel = "Not Confirmed"
                        }
                    },
                    new VueComponentDefinition {
                        Name = "cms-form-field-select",
                        Props = new {
                            label = "Roles",
                            valuePath = nameof(UserDatumModifierForm.RoleIds),
                            isMultiSelect = true,
                            optionsHandlerId = DatumSelectFieldOptionsHandler.HANDLER_ID,
                            optionsHandlerParam = Jsonizer.Convert(new DatumSelectFieldOptionsHandlerParam {
                                DatumTypeId = "role",
                                SortInfos = new[] {Tuple.Create("Name", false)}
                            })
                        }
                    }
                });
                return new Dictionary<string, VueComponentDefinition[]> {
                    [FORM_KEY] = vues.ToArray()
                };
            }
            if (operation.IsDeleteOperation()) {
                return new Dictionary<string, VueComponentDefinition[]> {
                    [FORM_KEY] = new VueComponentDefinition[] {
                        new VueHtmlWidget("Proceed to delete?")
                    }
                };
            }
            return null;
        }

        public override FurtherValidationResult ValidateModifierForm(
            IDictionary<string, ContentModifierForm> modifierForm,
            ApplicationUser datum,
            DatumModifyOperation operation, Type datumType) {
            var result = new FurtherValidationResult();
            if (operation.IsCreateOrUpdateOperation()) {
                var fm = modifierForm[FORM_KEY].DirectCastTo<UserDatumModifierForm>();
                if (operation.IsCreateOperation()) {
                    var existingUser = _userMgr.Users.FirstOrDefault(x => x.UserName == fm.UserName);
                    if (existingUser != null) {
                        result.AddError($"{FORM_KEY}.{nameof(fm.UserName)}",
                            $"Username '{fm.UserName}' is already taken.");
                    }
                }
                var checkPassword = operation.IsCreateOperation() || operation.IsUpdateOperation() && fm.ChangePassword;
                if (checkPassword) {
                    var validator = _userMgr.PasswordValidator;
                    if (fm.Password == null) {
                        result.AddError($"{FORM_KEY}.{nameof(fm.Password)}", "Password is required.");
                    } else {
                        var ir = validator.ValidateAsync(fm.Password).Result;
                        if (!ir.Succeeded) {
                            foreach (var err in ir.Errors) {
                                result.AddError($"{FORM_KEY}.{nameof(fm.Password)}", err);
                            }
                        }
                        if (fm.Password != fm.PasswordConfirmation) {
                            result.AddError($"{FORM_KEY}.{nameof(fm.PasswordConfirmation)}",
                                "Password and password confirmation must match.");
                        }
                    }
                }
            }
            if (operation.IsDeleteOperation()) {
                var rctx = ProtoCmsRuntimeContext.Current;
                if (rctx.CurrentUser != null && rctx.CurrentUser.Id == datum.Id) {
                    result.AddError($"{FORM_KEY}.{nameof(UserDatumModifierForm.UserName)}",
                        "You may not delete yourself.");
                }
                var superAdminRoleName = RequiredRoleInfo.SuperAdmin.Name;
                var superadmins = (from u in _dbContext.Users
                    from ur in u.Roles
                    join r in _dbContext.Roles
                        on ur.RoleId equals r.Id
                    where r.Name == superAdminRoleName
                    select u).ToArray();

                if (superadmins.Length == 1 && superadmins.FirstOrDefault(x => x.Id == datum.Id) != null) {
                    result.AddError($"{FORM_KEY}.{nameof(UserDatumModifierForm.UserName)}",
                        "You may not delete the only superadmin left in the system.");
                }
            }
            return result;
        }

        public override void PerformModification(IDictionary<string, ContentModifierForm> modifierForm,
            ApplicationUser datum,
            DatumModifyOperation operation,
            Type datumType) {
            IdentityResult ir = null;
            var hasOp = false;
            if (operation.IsDeleteOperation()) {
                ir = _userMgr.Delete(datum);
                hasOp = true;
            } else if (operation.IsCreateOrUpdateOperation()) {
                var isCreateOperation = operation.IsCreateOperation();
                var isUpdateOperation = operation.IsUpdateOperation();
                var fm = modifierForm[FORM_KEY].DirectCastTo<UserDatumModifierForm>();
                datum.IsActivated = fm.IsActivated;
                datum.UserName = fm.UserName;
                datum.DisplayName = fm.DisplayName;
                datum.PhotoUrl = fm.PhotoUrl.Length > 0 ? fm.PhotoUrl[0] : null;
                datum.Email = fm.Email;
                datum.EmailConfirmed = fm.EmailConfirmed;
                datum.PhoneNumber = fm.PhoneNumber;
                datum.PhoneNumberConfirmed = fm.PhoneNumberConfirmed;
                var changePassword = isCreateOperation || isUpdateOperation && fm.ChangePassword;
                if (changePassword) {
                    var hasher = _userMgr.PasswordHasher;
                    var pwdHash = hasher.HashPassword(fm.Password);
                    datum.PasswordHash = pwdHash;
                }
                ir = isCreateOperation ? _userMgr.Create(datum) : _userMgr.Update(datum);
                if (ir.Succeeded) {
                    var allRoles = _roleMgr.Roles.ToArray();
                    var userRoleNames = new List<string>();
                    foreach (var role in allRoles) {
                        var hasRole = role.Name != RequiredRoleInfo.Authenticated.Name &&
                                      role.Name != RequiredRoleInfo.Guest.Name &&
                                      fm.RoleIds.Contains(role.Id);
                        if (hasRole) userRoleNames.Add(role.Name);
                    }
                    if (isCreateOperation) {
                        ir = _userMgr.AddToRoles(datum.Id, userRoleNames.ToArray());
                    } else if (isUpdateOperation) {
                        var currentRoles = _userMgr.GetRoles(datum.Id).ToArray();
                        if (currentRoles.Length > 0) {
                            ir = _userMgr.RemoveFromRoles(datum.Id, currentRoles);
                        }
                        if (ir.Succeeded) {
                            ir = _userMgr.AddToRoles(datum.Id, userRoleNames.ToArray());
                        }
                    }
                }
            }
            if (hasOp) {
                if (!ir.Succeeded) throw new Exception(string.Join("; ", ir.Errors));
            }
        }
    }
}