using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using JrzAsp.Lib.RepoPattern;
using JrzAsp.Lib.TypeUtilities;
using JrzAsp.Lib.ProtoCms.Content.Forms;
using JrzAsp.Lib.ProtoCms.Content.Models;
using JrzAsp.Lib.ProtoCms.Database;
using JrzAsp.Lib.ProtoCms.Vue.Models;

namespace JrzAsp.Lib.ProtoCms.Content.Services {
    public class ContentModifier : IContentModifier {
        public const string VALID_FORM_PART_KEY_PATTERN = @"^[A-Z_][a-zA-Z_0-9]*$";
        public const string EXTRA_FORM_VUES_PRE_KEY = "__PreModifyOperationFormVues";
        public const string EXTRA_FORM_VUES_POST_KEY = "__PostModifyOperationFormVues";

        private static readonly IDependencyProvider _dp = ProtoEngine.GetDependencyProvider();

        public static readonly Regex VALID_FORM_PART_KEY_REGEX =
            new Regex(VALID_FORM_PART_KEY_PATTERN, RegexOptions.Compiled);

        public static readonly Type[] HandlerTypes;
        private static IContentModifyOperationsProvider[] _modifyOperationsProviders;
        private static ContentModifyOperation[] _definedModifyOperations;

        private readonly IProtoCmsDbContext _dbContext;
        private IContentModifierHandler[] _handlers;

        static ContentModifier() {
            var sts = new List<Type>();

            foreach (var t in TypesCache.AppDomainTypes.Where(
                x => x.IsNonDynamicallyGeneratedConcreteClass())) {
                if (typeof(IContentModifierHandler).IsAssignableFrom(t)) sts.Add(t);
            }

            HandlerTypes = sts.ToArray();
        }

        public ContentModifier(IProtoCmsDbContext dbContext) {
            _dbContext = dbContext;
        }

        public static IContentModifyOperationsProvider[] ModifyOperationsProviders {
            get {
                if (_modifyOperationsProviders != null) return _modifyOperationsProviders;
                _modifyOperationsProviders = _dp.GetServices(typeof(IContentModifyOperationsProvider))
                    .Cast<IContentModifyOperationsProvider>().OrderBy(x => x.Priority).ToArray();
                return _modifyOperationsProviders;
            }
        }
        public static ContentModifyOperation[] DefinedModifyOperations {
            get {
                if (_definedModifyOperations != null) return _definedModifyOperations;
                var map = new Dictionary<string, List<Tuple<ContentModifyOperation, Type>>>();
                var mops = ModifyOperationsProviders;
                foreach (var mop in mops) {
                    foreach (var mo in mop.DefineModifyOperations()) {
                        if (!map.TryGetValue(mo.Name, out var moMap)) {
                            moMap = new List<Tuple<ContentModifyOperation, Type>>();
                            map[mo.Name] = moMap;
                        }
                        moMap.Add(Tuple.Create(mo, mop.GetType()));
                    }
                }
                var dmos = new List<ContentModifyOperation>();
                foreach (var mapKv in map) {
                    var mos = mapKv.Value.Select(x => x.Item1);
                    var provs = mapKv.Value.Select(x => x.Item2);
                    var dmo = new CombinedContentModifyOperation(mapKv.Key, mos, provs);
                    dmos.Add(dmo);
                }
                _definedModifyOperations = dmos.ToArray();
                return _definedModifyOperations;
            }
        }

        public ContentType ContentType { get; private set; }
        public IContentModifierHandler[] Handlers {
            get {
                if (_handlers == null) InitModifierHandlers();
                return _handlers;
            }
        }

        public void Initialize(ContentType contentType) {
            ContentType = contentType;
        }

        public IDictionary<string, ContentModifierForm> BuildModifierForm(string contentId,
            string operationName) {
            var operation = FindModifyOperation(operationName);
            var content = FindProtoContent(contentId, operationName);
            var cmf = new Dictionary<string, ContentModifierForm>();
            foreach (var h in Handlers) {
                var parts = h.BuildModifierForm(content, operation, ContentType);
                if (parts == null) continue;
                foreach (var part in parts) {
                    if (part.Value != null) {
                        if (string.IsNullOrWhiteSpace(part.Key)) {
                            throw new HttpException(400,
                                $"ProtoCMS: content modifier form requires a valid key."
                            );
                        }
                        if (!VALID_FORM_PART_KEY_REGEX.IsMatch(part.Key)) {
                            throw new HttpException(400,
                                $"ProtoCMS: content modifier form key must match regex " +
                                $"'{VALID_FORM_PART_KEY_REGEX}'."
                            );
                        }
                        cmf[part.Key] = part.Value;
                    }
                }
            }
            return cmf;
        }

        public FurtherValidationResult ValidateModifierForm(IDictionary<string, ContentModifierForm> modifierForm,
            string contentId, string operationName) {
            var operation = FindModifyOperation(operationName);
            var content = FindProtoContent(contentId, operationName);
            var result = new FurtherValidationResult();
            foreach (var h in Handlers) {
                var r = h.ValidateModifierForm(modifierForm, content, operation, ContentType);
                if (r != null && r.HasError) {
                    foreach (var kv in r.Errors) foreach (var m in kv.Value) result.AddError(kv.Key, m);
                }
            }
            return result;
        }

        public void PerformModification(IDictionary<string, ContentModifierForm> modifierForm, string contentId,
            out ProtoContent modifiedContent, string operationName) {
            var operation = FindModifyOperation(operationName);
            var db = _dbContext.ThisDbContext();
            using (var dbTrx = db.Database.BeginTransaction()) {
                try {
                    modifiedContent = FindProtoContent(contentId, operationName);
                    foreach (var h in Handlers) {
                        h.PerformModification(modifierForm, modifiedContent, operation, ContentType);
                    }
                    dbTrx.Commit();
                } catch (Exception) {
                    dbTrx.Rollback();
                    throw;
                }
            }
        }

        public IDictionary<string, VueComponentDefinition[]> ConvertFormToVues(
            IDictionary<string, ContentModifierForm> modifierForm, string contentId, string operationName) {
            var operation = FindModifyOperation(operationName);
            var content = FindProtoContent(contentId, operationName);
            var vues = new Dictionary<string, VueComponentDefinition[]>();
            var preVues = new List<VueComponentDefinition> {
                new VueHtmlWidget($"<p class='small text-muted'>{(operation.IsCreatingNewContent ? "New " : "")}" +
                                  $"{ContentType.Name} Id: <code>{content.Id}</code></p>")
            };
            foreach (var getPreMovs in ContentType.PreModifyOperationFormVues) {
                var preMovs = getPreMovs.Invoke(ContentType, content, operation);
                if (preMovs != null) preVues.AddRange(preMovs);
            }
            vues[EXTRA_FORM_VUES_PRE_KEY] = preVues.ToArray();
            foreach (var h in Handlers) {
                var partVues = h.ConvertFormToVues(modifierForm, content, operation, ContentType);
                if (partVues != null) {
                    foreach (var pvkv in partVues) {
                        if (vues.TryGetValue(pvkv.Key, out var pvs)) {
                            var lpvs = new List<VueComponentDefinition>(pvs);
                            if (pvkv.Value != null && pvkv.Value.Length > 0) {
                                lpvs.AddRange(pvkv.Value);
                            }
                            vues[pvkv.Key] = lpvs.ToArray();
                        } else if (pvkv.Value != null && pvkv.Value.Length > 0) {
                            vues[pvkv.Key] = pvkv.Value;
                        }
                    }
                }
            }
            var postVues = new List<VueComponentDefinition>();
            foreach (var getPostMovs in ContentType.PostModifyOperationFormVues) {
                var postMovs = getPostMovs.Invoke(ContentType, content, operation);
                if (postMovs != null) postVues.AddRange(postMovs);
            }
            vues[EXTRA_FORM_VUES_POST_KEY] = postVues.ToArray();
            return vues;
        }

        public ContentModifyOperation FindModifyOperation(string operationName) {
            var modOp = DefinedModifyOperations.FirstOrDefault(x => x.Name == operationName);
            if (modOp == null) {
                throw new HttpException(400, $"ProtoCMS: content modify operation '{operationName}' " +
                                             $"doesn't exist.");
            }
            return modOp;
        }

        public ProtoContent FindProtoContent(string contentId, string operationName) {
            ProtoContent content;
            var operation = FindModifyOperation(operationName);
            if (operation.IsCreatingNewContent) {
                content = new ProtoContent {
                    Id = string.IsNullOrWhiteSpace(contentId) ? Guid.NewGuid().ToString() : contentId,
                    ContentTypeId = ContentType.Id
                };
                if (string.IsNullOrWhiteSpace(content.Id)) {
                    throw new HttpException(400, $"ProtoCMS: ContentId is required.");
                }
            } else {
                content = ContentType.Finder().FindById(contentId);
                if (content == null) {
                    throw new HttpException(400, $"ProtoCMS: no {ContentType.Id} content found with id " +
                                                 $"'{contentId}'.");
                }
                if (content.ContentTypeId != ContentType.Id) content.ContentTypeId = ContentType.Id;
            }
            return content;
        }

        private void InitModifierHandlers() {
            var handlerTypes = HandlerTypes;
            var handlers = new List<IContentModifierHandler>();
            foreach (var ht in handlerTypes) {
                var h = _dp.GetService(ht).DirectCastTo<IContentModifierHandler>();
                if (h.HandledContentTypeIds != null &&
                    (h.HandledContentTypeIds.Contains(ContentType.ANY_CONTENT_TYPE_ID) ||
                     h.HandledContentTypeIds.Contains(ContentType.Id))) {
                    handlers.Add(h);
                }
            }
            _handlers = handlers.OrderBy(x => x.Priority).ToArray();
        }
    }
}