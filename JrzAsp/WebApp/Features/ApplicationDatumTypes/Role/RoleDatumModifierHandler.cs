using System;
using System.Collections.Generic;
using System.Linq;
using JrzAsp.Lib.ProtoCms;
using JrzAsp.Lib.ProtoCms.Content.Forms;
using JrzAsp.Lib.ProtoCms.Database;
using JrzAsp.Lib.ProtoCms.Datum.Models;
using JrzAsp.Lib.ProtoCms.Datum.Services;
using JrzAsp.Lib.ProtoCms.Permission.Models;
using JrzAsp.Lib.ProtoCms.Permission.Services;
using JrzAsp.Lib.ProtoCms.Role.Models;
using JrzAsp.Lib.ProtoCms.Vue.Models;
using JrzAsp.Lib.RepoPattern;
using JrzAsp.Lib.TypeUtilities;
using Microsoft.AspNet.Identity;
using WebApp.Models;

namespace WebApp.Features.ApplicationDatumTypes.Role {
    public class RoleDatumModifierHandler : BaseDatumModifierHandler<ApplicationRole> {
        public const string FORM_KEY = "ApplicationRole";

        private readonly ApplicationDbContext _dbContext;
        private readonly IPermissionManager _permMgr;
        private readonly ApplicationRoleManager _roleMgr;

        public RoleDatumModifierHandler(ApplicationDbContext dbContext, ApplicationRoleManager roleMgr,
            IPermissionManager permMgr) {
            _roleMgr = roleMgr;
            _dbContext = dbContext;
            _permMgr = permMgr;
        }

        public override decimal Priority => 0;

        public override IDictionary<string, ContentModifierForm> BuildModifierForm(ApplicationRole datum,
            DatumModifyOperation operation, Type datumType) {
            if (!operation.IsCreateOrUpdateOperation()) return null;
            var f = new RoleDatumModifierForm {
                Id = datum.Id,
                Name = datum.Name,
                Description = datum.Description
            };
            var perms = new List<string>();
            var permIds = _permMgr.GetRolesPermissions(new[] {datum.Name}).ToArray();
            foreach (var perm in _permMgr.AllPermissions) {
                foreach (var permId in permIds) {
                    if (perm.Id != permId) continue;
                    perms.Add(perm.Id);
                }
            }
            f.Permissions = perms.ToArray();
            return new Dictionary<string, ContentModifierForm> {
                [FORM_KEY] = f
            };
        }

        public override IDictionary<string, VueComponentDefinition[]> ConvertFormToVues(
            IDictionary<string, ContentModifierForm> modifierForm, ApplicationRole datum,
            DatumModifyOperation operation,
            Type datumType) {
            if (operation.IsCreateOrUpdateOperation()) {
                return new Dictionary<string, VueComponentDefinition[]> {
                    [FORM_KEY] = new[] {
                        new VueComponentDefinition {
                            Name = "cms-form-field-hidden",
                            Props = new {
                                valuePath = nameof(RoleDatumModifierForm.Id)
                            }
                        },
                        new VueComponentDefinition {
                            Name = "cms-form-field-text",
                            Props = new {
                                label = "Name",
                                valuePath = nameof(RoleDatumModifierForm.Name)
                            }
                        },
                        new VueComponentDefinition {
                            Name = "cms-form-field-textarea",
                            Props = new {
                                label = "Description",
                                valuePath = nameof(RoleDatumModifierForm.Description)
                            }
                        },
                        new VueComponentDefinition {
                            Name = "cms-form-field-select",
                            Props = new {
                                label = "Permissions",
                                valuePath = nameof(RoleDatumModifierForm.Permissions),
                                isMultiSelect = true,
                                optionsHandlerId = DatumSelectFieldOptionsHandler.HANDLER_ID,
                                optionsHandlerParam = Jsonizer.Convert(new DatumSelectFieldOptionsHandlerParam {
                                    DatumTypeId = "permission"
                                })
                            }
                        }

                    }
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

        public override FurtherValidationResult ValidateModifierForm(IDictionary<string, ContentModifierForm> modifierForm,
            ApplicationRole datum,
            DatumModifyOperation operation, Type datumType) {
            var result = new FurtherValidationResult();
            if (operation.IsDeleteOperation()) {
                if (datum.Name == RequiredRoleInfo.SuperAdmin.Name ||
                    datum.Name == RequiredRoleInfo.Authenticated.Name ||
                    datum.Name == RequiredRoleInfo.Guest.Name) {
                    result.AddError($"{FORM_KEY}.{nameof(RoleDatumModifierForm.Name)}",
                        $"{RequiredRoleInfo.SuperAdmin.Name}, {RequiredRoleInfo.Authenticated.Name}, " +
                        $"and {RequiredRoleInfo.Guest.Name} are reserved by the system and should not " +
                        $"be deleted.");
                }
            } else {
                var fm = modifierForm[FORM_KEY].DirectCastTo<RoleDatumModifierForm>();
                if (operation.IsCreatingNewDatum) {
                    if (fm.Name == RequiredRoleInfo.SuperAdmin.Name ||
                        fm.Name == RequiredRoleInfo.Authenticated.Name ||
                        fm.Name == RequiredRoleInfo.Guest.Name) {
                        result.AddError($"{FORM_KEY}.{nameof(RoleDatumModifierForm.Name)}",
                            $"{RequiredRoleInfo.SuperAdmin.Name}, {RequiredRoleInfo.Authenticated.Name}, " +
                            $"and {RequiredRoleInfo.Guest.Name} are reserved by the system and no new " +
                            $"role should use their name.");
                    }
                }
                if ((datum.Name == RequiredRoleInfo.SuperAdmin.Name ||
                     datum.Name == RequiredRoleInfo.Authenticated.Name ||
                     datum.Name == RequiredRoleInfo.Guest.Name) && fm.Name != datum.Name) {
                    result.AddError($"{FORM_KEY}.{nameof(RoleDatumModifierForm.Name)}",
                        $"{RequiredRoleInfo.SuperAdmin.Name}, {RequiredRoleInfo.Authenticated.Name}, " +
                        $"and {RequiredRoleInfo.Guest.Name} are reserved by the system and should not " +
                        $"have their name be modified.");
                }
            }
            return result;
        }

        public override void PerformModification(IDictionary<string, ContentModifierForm> modifierForm, ApplicationRole datum,
            DatumModifyOperation operation,
            Type datumType) {
            IdentityResult ir = null;
            var hasOp = false;
            if (operation.IsCreateOrUpdateOperation()) {
                var f = modifierForm[FORM_KEY].DirectCastTo<RoleDatumModifierForm>();
                datum.Name = f.Name;
                datum.Description = f.Description;
                ir = operation.IsCreateOperation() ? _roleMgr.Create(datum) : _roleMgr.Update(datum);
                if (ir.Succeeded) {
                    foreach (var perm in _permMgr.AllPermissions) {
                        var pmap = _dbContext.ProtoPermissionsMaps
                            .FirstOrDefault(x => x.RoleName == datum.Name && x.PermissionId == perm.Id);
                        if (pmap == null) {
                            pmap = new ProtoPermissionsMap {
                                RoleName = datum.Name,
                                PermissionId = perm.Id
                            };
                            _dbContext.ProtoPermissionsMaps.Add(pmap);
                        }
                        pmap.HasPermission = f.Permissions.Contains(perm.Id);
                    }
                    _dbContext.ThisDbContext().SaveChanges();
                }
                hasOp = true;
            } else if (operation.IsDeleteOperation()) {
                ir = _roleMgr.Delete(datum);
                if (ir.Succeeded) {
                    var pmaps = _dbContext.ProtoPermissionsMaps.Where(x => x.RoleName == datum.Name).ToList();
                    foreach (var pmap in pmaps) {
                        _dbContext.ProtoPermissionsMaps.Remove(pmap);
                    }
                }
                hasOp = true;
            }
            if (hasOp) {
                if (!ir.Succeeded) throw new Exception(string.Join("; ", ir.Errors));
            }
        }
    }
}