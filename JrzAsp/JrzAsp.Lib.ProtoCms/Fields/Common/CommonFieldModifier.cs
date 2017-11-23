using System;
using System.Collections.Generic;
using System.Linq;
using JrzAsp.Lib.ProtoCms.Content.Forms;
using JrzAsp.Lib.ProtoCms.Content.Models;
using JrzAsp.Lib.ProtoCms.Content.Services;
using JrzAsp.Lib.ProtoCms.Core.Models;
using JrzAsp.Lib.ProtoCms.Database;
using JrzAsp.Lib.ProtoCms.User.Services;
using JrzAsp.Lib.ProtoCms.Vue.Models;

namespace JrzAsp.Lib.ProtoCms.Fields.Common {
    public class CommonFieldModifier : IContentFieldModifier {

        private readonly IProtoCmsDbContext _dbContext;
        private readonly IProtoUserManager _userMgr;

        public CommonFieldModifier(IProtoCmsDbContext dbContext, IProtoUserManager userMgr) {
            _dbContext = dbContext;
            _userMgr = userMgr;
        }

        public ContentModifierForm BuildModifierForm(ContentField field,
            ContentModifyOperation operation, ProtoContent content,
            ContentFieldDefinition fieldDefinition) {
            var f = new CommonFieldModifierForm {
                ContentId = content?.Id,
                ContentTypeId = content?.ContentTypeId,
                OperationName = operation.Name
            };
            return f;
        }

        public void PerformModification(ContentModifierForm modifierForm, ContentField field,
            ContentModifyOperation operation,
            ProtoContent content, ContentFieldDefinition fieldDefinition) {
            if (operation.Is(CommonFieldModifyOperationsProvider.CREATE_OPERATION_NAME)) {
                content.CreatedUtc = DateTime.UtcNow;
                content.UpdatedUtc = content.CreatedUtc;
                _dbContext.ProtoContents.Add(content);
            } else if (operation.Is(CommonFieldModifyOperationsProvider.DELETE_OPERATION_NAME)) {
                content.UpdatedUtc = DateTime.UtcNow;
                _dbContext.ProtoContents.Remove(content);
            } else {
                content.UpdatedUtc = DateTime.UtcNow;
            }
            if (string.IsNullOrWhiteSpace(content.CreatedByUserId)) {
                var rctx = ProtoCmsRuntimeContext.Current;
                if (rctx.CurrentUser != null) {
                    content.CreatedByUserId = rctx.CurrentUser.Id;
                }
            }
            var creator = _userMgr.ProtoUsers.FirstOrDefault(x => x.Id == content.CreatedByUserId);
            if (creator != null) {
                content.CreatedByUserName = creator.UserName;
                content.CreatedByUserDisplayName = creator.DisplayName;
            }
            _dbContext.ThisDbContext().SaveChanges();
        }

        public VueComponentDefinition[] ConvertFormToVues(ContentModifierForm modifierForm,
            ContentField field,
            ContentModifyOperation operation,
            ProtoContent content, ContentFieldDefinition fieldDefinition) {
            var vues = new List<VueComponentDefinition>();
            if (operation.IsDeleteOperation()) {
                vues.Add(new VueHtmlWidget("Proceed to delete?"));
            }
            vues.AddRange(new[] {
                new VueComponentDefinition {
                    Name = "cms-form-field-hidden",
                    Props = new {
                        valuePath = nameof(CommonFieldModifierForm.ContentId)
                    }
                },
                new VueComponentDefinition {
                    Name = "cms-form-field-hidden",
                    Props = new {
                        valuePath = nameof(CommonFieldModifierForm.ContentTypeId)
                    }
                },
                new VueComponentDefinition {
                    Name = "cms-form-field-hidden",
                    Props = new {
                        valuePath = nameof(CommonFieldModifierForm.OperationName)
                    }
                }
            });

            return vues.ToArray();
        }
    }
}