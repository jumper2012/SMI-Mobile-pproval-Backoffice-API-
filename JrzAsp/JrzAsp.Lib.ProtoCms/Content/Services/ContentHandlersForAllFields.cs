using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using JrzAsp.Lib.ProtoCms.Content.Forms;
using JrzAsp.Lib.ProtoCms.Content.Models;
using JrzAsp.Lib.ProtoCms.Fields.Common;
using JrzAsp.Lib.ProtoCms.Vue.Models;
using JrzAsp.Lib.QueryableUtilities;
using JrzAsp.Lib.RepoPattern;
using JrzAsp.Lib.TypeUtilities;

namespace JrzAsp.Lib.ProtoCms.Content.Services {
    public class ContentHandlersForAllFields : IContentSearchHandler,
        IContentSortHandler, IContentModifierHandler {

        public IDictionary<string, ContentModifierForm> BuildModifierForm(ProtoContent content,
            ContentModifyOperation operation, ContentType contentType) {
            var fps = new Dictionary<string, ContentModifierForm>();
            foreach (var fd in contentType.Fields) {
                if (!operation.IsAny(fd.Config.HandledModifyOperationNames)) continue;
                var fdr = fd.FieldFinder();
                var mod = fd.FieldModifier();
                var field = operation.Is(CommonFieldModifyOperationsProvider.CREATE_OPERATION_NAME)
                    ? Activator.CreateInstance(fd.ModelType).DirectCastTo<ContentField>()
                    : fdr.GetModel(content, fd);
                var form = mod.BuildModifierForm(field, operation, content, fd);
                if (form != null) {
                    fps.Add(fd.FieldName, form);
                }
            }
            return fps;
        }

        public IDictionary<string, VueComponentDefinition[]> ConvertFormToVues(
            IDictionary<string, ContentModifierForm> modifierForm,
            ProtoContent content,
            ContentModifyOperation operation,
            ContentType contentType) {
            var vues = new Dictionary<string, VueComponentDefinition[]>();
            foreach (var fd in contentType.Fields) {
                if (!operation.IsAny(fd.Config.HandledModifyOperationNames)) continue;
                var fdr = fd.FieldFinder();
                var mod = fd.FieldModifier();
                var field = operation.Is(CommonFieldModifyOperationsProvider.CREATE_OPERATION_NAME)
                    ? Activator.CreateInstance(fd.ModelType).DirectCastTo<ContentField>()
                    : fdr.GetModel(content, fd);
                modifierForm.TryGetValue(fd.FieldName, out var form);
                var fvs = mod.ConvertFormToVues(form, field, operation, content, fd);
                if (fvs != null && fvs.Length > 0) {
                    vues.Add(fd.FieldName, fvs);
                }
            }
            return vues;
        }

        public FurtherValidationResult ValidateModifierForm(IDictionary<string, ContentModifierForm> modifierForm,
            ProtoContent content, ContentModifyOperation operation, ContentType contentType) {
            var result = new FurtherValidationResult();
            foreach (var fd in contentType.Fields) {
                if (!operation.IsAny(fd.Config.HandledModifyOperationNames)) continue;
                var fdr = fd.FieldFinder();
                var mod = fd.FieldModifier();
                var field = operation.Is(CommonFieldModifyOperationsProvider.CREATE_OPERATION_NAME)
                    ? Activator.CreateInstance(fd.ModelType).DirectCastTo<ContentField>()
                    : fdr.GetModel(content, fd);
                modifierForm.TryGetValue(fd.FieldName, out var form);
                if (form == null) {
                    var testForm = mod.BuildModifierForm(field, operation, content, fd);
                    if (testForm != null) {
                        throw new HttpException(400, $"ProtoCMS: content modifier form for field '{fd.FieldName}' " +
                                                     $"is required and must be an instance of " +
                                                     $"'{testForm.GetType().FullName}'.");
                    }
                    continue;
                }
                foreach (var fvkv in fd.Config.Validators) {
                    var hasValidator = false;
                    foreach (var dv in ContentFieldValidator.DefinedValidators) {
                        if (dv.Name != fvkv.Key || !dv.HandledFormTypes.Contains(form.GetType())) continue;
                        hasValidator = true;
                        var valCfg = fvkv.Value ?? Activator.CreateInstance(dv.ConfigType)
                                         .DirectCastTo<ContentFieldValidatorConfiguration>();
                        if (operation.IsAny(valCfg.ModifyOperationNamesThatIgnoreValidation)) continue;
                        if (valCfg.GetType() != dv.ConfigType) {
                            throw new InvalidOperationException(
                                $"ProtoCMS: content field validator '{dv.Name}' (for validating form " +
                                $"'{form.GetType().FullName}') can only accept config instance of type " +
                                $"'{dv.ConfigType.FullName}'.");
                        }
                        var valRes = dv.ValidateForm(form, valCfg, contentType, fd);
                        if (valRes != null) {
                            foreach (var kv in valRes.Errors) {
                                if (kv.Value == null || kv.Value.Length == 0) continue;
                                foreach (var err in kv.Value) {
                                    result.AddError($"{fd.FieldName}.{kv.Key}", err);
                                }
                            }
                        }
                    }
                    if (!hasValidator) {
                        throw new InvalidOperationException($"ProtoCMS: there's no content field validator " +
                                                            $"named '{fvkv.Key}' that can handle form " +
                                                            $"'{form.GetType().FullName}' validation.");
                    }
                }
            }
            return result;
        }

        public void PerformModification(IDictionary<string, ContentModifierForm> modifierForm, ProtoContent content,
            ContentModifyOperation operation, ContentType contentType) {
            foreach (var fd in contentType.Fields) {
                if (!operation.IsAny(fd.Config.HandledModifyOperationNames)) continue;
                var fdr = fd.FieldFinder();
                var mod = fd.FieldModifier();
                var field = operation.Is(CommonFieldModifyOperationsProvider.CREATE_OPERATION_NAME)
                    ? Activator.CreateInstance(fd.ModelType).DirectCastTo<ContentField>()
                    : fdr.GetModel(content, fd);
                modifierForm.TryGetValue(fd.FieldName, out var form);
                mod.PerformModification(form, field, operation, content, fd);
            }
        }

        public string[] HandledContentTypeIds => new[] {ContentType.ANY_CONTENT_TYPE_ID};
        public decimal Priority => 0;

        public Expression<Func<ProtoContent, bool>> HandleSearch(string keywords, string[] splittedKeywords,
            ContentType contentType, out bool callNextHandler) {
            callNextHandler = true;
            var pred = PredicateBuilder.False<ProtoContent>();
            foreach (var fd in contentType.Fields) {
                if (fd.Config?.IncludeWhenSearching == false) continue;
                var finder = fd.FieldFinder();
                var cond = finder.Search(keywords, splittedKeywords, fd);
                if (cond != null) pred = pred.Or(cond);
            }
            return pred;
        }

        public IQueryable<ProtoContent> HandleSort(IQueryable<ProtoContent> currentQuery, string fieldName,
            bool descending, ContentType contentType, out bool callNextHandler) {
            callNextHandler = true;
            var q = currentQuery;
            foreach (var fd in contentType.Fields) {
                if (!fieldName.StartsWith($"{fd.FieldName}.") && fd.FieldName != fieldName) continue;
                var finder = fd.FieldFinder();
                var nextQ = finder.Sort(q, fieldName, descending, fd);
                if (nextQ != null) q = nextQ;
            }
            return q;
        }
    }
}