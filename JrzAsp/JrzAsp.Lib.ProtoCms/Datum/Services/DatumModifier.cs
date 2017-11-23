using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using JrzAsp.Lib.ProtoCms.Content.Forms;
using JrzAsp.Lib.ProtoCms.Content.Services;
using JrzAsp.Lib.ProtoCms.Database;
using JrzAsp.Lib.ProtoCms.Datum.Forms;
using JrzAsp.Lib.ProtoCms.Datum.Models;
using JrzAsp.Lib.ProtoCms.Datum.Permissions;
using JrzAsp.Lib.ProtoCms.Vue.Models;
using JrzAsp.Lib.RepoPattern;
using JrzAsp.Lib.TypeUtilities;

namespace JrzAsp.Lib.ProtoCms.Datum.Services {
    public abstract class DatumModifier : IDatumModifier {
        public const string COMMON_DATUM_METADATA_FORM_KEY = "__CommonFormMetadata";
        public const string EXTRA_FORM_VUES_PRE_KEY = "__PreModifyOperationFormVues";
        public const string EXTRA_FORM_VUES_POST_KEY = "__PostModifyOperationFormVues";
        protected static readonly IDependencyProvider _dp = ProtoEngine.GetDependencyProvider();

        private static IDatumModifyOperationsProvider[] _modifyOperationsProviders;
        private static DatumModifyOperation[] _definedModifyOperations;
        private static Type[] _handlerTypes;

        public static IDatumModifyOperationsProvider[] ModifyOperationsProviders {
            get {
                if (_modifyOperationsProviders != null) return _modifyOperationsProviders;
                _modifyOperationsProviders = _dp.GetServices(typeof(IDatumModifyOperationsProvider))
                    .Cast<IDatumModifyOperationsProvider>().OrderBy(x => x.Priority).ToArray();
                return _modifyOperationsProviders;
            }
        }
        public static DatumModifyOperation[] DefinedModifyOperations {
            get {
                if (_definedModifyOperations != null) return _definedModifyOperations;
                var map = new Dictionary<string, List<Tuple<DatumModifyOperation, Type>>>();
                var mops = ModifyOperationsProviders;
                foreach (var mop in mops) {
                    foreach (var mo in mop.DefineModifyOperations()) {
                        if (!map.TryGetValue(mo.Name, out var moMap)) {
                            moMap = new List<Tuple<DatumModifyOperation, Type>>();
                            map[mo.Name] = moMap;
                        }
                        moMap.Add(Tuple.Create(mo, mop.GetType()));
                    }
                }
                var dmos = new List<DatumModifyOperation>();
                foreach (var mapKv in map) {
                    var mos = mapKv.Value.Select(x => x.Item1);
                    var provs = mapKv.Value.Select(x => x.Item2);
                    var dmo = new CombinedDatumModifyOperation(mapKv.Key, mos, provs);
                    dmos.Add(dmo);
                }
                _definedModifyOperations = dmos.ToArray();
                return _definedModifyOperations;
            }
        }
        public static Type[] HandlerTypes {
            get {
                if (_handlerTypes != null) return _handlerTypes;
                _handlerTypes = TypesCache.AppDomainTypes.Where(x =>
                        typeof(IDatumModifierHandler).IsAssignableFrom(x) &&
                        x.IsNonDynamicallyGeneratedConcreteClass())
                    .ToArray();
                return _handlerTypes;
            }
        }
        public static Type[] PermissionsHandlerTypes => DatumFinder.PermissionsHandlerTypes;

        public abstract Type DatumModelType { get; }
        public abstract DatumType DatumTypeBase { get; }
        public abstract IDatumModifierHandler[] HandlersBase { get; }
        public abstract IDatumPermissionsHandler[] PermissionsHandlersBase { get; }
        public abstract ModifyDatumPermission[] PermissionsToModifyBase { get; }
        public abstract object FindDatumBase(string datumId, string operationName);

        public abstract IDictionary<string, ContentModifierForm>
            BuildModifierForm(string datumId, string operationName);

        public abstract IDictionary<string, VueComponentDefinition[]> ConvertFormToVues(
            IDictionary<string, ContentModifierForm> modifierForm, string datumId, string operationName);

        public abstract FurtherValidationResult ValidateModifierForm(
            IDictionary<string, ContentModifierForm> modifierForm, string datumId, string operationName);

        public abstract void PerformModificationBase(IDictionary<string, ContentModifierForm> modifierForm,
            string datumId, out object modifiedDatum, string operationName);

        public abstract DatumModifyOperation FindModifyOperation(string operationName);
    }

    public class DatumModifier<TDat> : DatumModifier, IDatumModifier<TDat> {
        private readonly IProtoCmsDbContext _dbContext;

        private IDatumModifierHandler<TDat>[] _handlers;
        private IDatumPermissionsHandler<TDat>[] _permissionsHandlers;
        private ModifyDatumPermission<TDat>[] _permissionsToModify;

        public DatumModifier(IProtoCmsDbContext dbContext) {
            _dbContext = dbContext;
        }

        public override Type DatumModelType => typeof(TDat);
        public override DatumType DatumTypeBase => DatumType;
        public override IDatumModifierHandler[] HandlersBase => Handlers;
        public override IDatumPermissionsHandler[] PermissionsHandlersBase => PermissionsHandlers;
        public override ModifyDatumPermission[] PermissionsToModifyBase => PermissionsToModify;
        public DatumType<TDat> DatumType => DatumModelType.GetDatumTypeFromType<TDat>();
        public IDatumModifierHandler<TDat>[] Handlers {
            get {
                if (_handlers != null) return _handlers;
                _handlers = _dp.GetServices(typeof(IDatumModifierHandler<TDat>))
                    .Cast<IDatumModifierHandler<TDat>>().OrderBy(x => x.Priority).ToArray();
                return _handlers;
            }
        }
        public IDatumPermissionsHandler<TDat>[] PermissionsHandlers {
            get {
                if (_permissionsHandlers != null) return _permissionsHandlers;
                _permissionsHandlers = _dp.GetServices(typeof(IDatumPermissionsHandler<TDat>))
                    .Cast<IDatumPermissionsHandler<TDat>>().OrderBy(x => x.Priority).ToArray();
                return _permissionsHandlers;
            }
        }
        public ModifyDatumPermission<TDat>[] PermissionsToModify {
            get {
                if (_permissionsToModify != null) return _permissionsToModify;
                var perms = new List<ModifyDatumPermission<TDat>>();
                foreach (var mo in DefinedModifyOperations) {
                    var perm = new ModifyDatumPermission<TDat>(mo.Name, GetFirstPermissionsHandler());
                    if (perm.DisplayName != null) perms.Add(perm);
                }
                _permissionsToModify = perms.ToArray();
                return _permissionsToModify;
            }
        }

        public TDat FindDatum(string datumId, string operationName) {
            var op = FindModifyOperation(operationName);
            var finder = _dp.GetService(typeof(IDatumFinder<TDat>)).DirectCastTo<IDatumFinder<TDat>>();
            if (op.IsCreatingNewDatum) {
                datumId = datumId ?? Guid.NewGuid().ToString();
                var newDt = finder.CreateInMemory(datumId);
                var newDtId = finder.GetDatumId(newDt);
                if (string.IsNullOrWhiteSpace(newDtId)) {
                    throw new HttpException(400, $"ProtoCMS: DatumId is required.");
                }
                return newDt;
            }
            var dt = finder.FindById(datumId);
            if (dt == null) {
                throw new HttpException(400, $"ProtoCMS: no '{typeof(object).FullName}, " +
                                             $"{typeof(object).Assembly.GetName().Name}' datum found with id " +
                                             $"'{datumId}'.");
            }
            return dt;
        }

        public override object FindDatumBase(string datumId, string operationName) {
            return FindDatum(datumId, operationName);
        }

        public override IDictionary<string, ContentModifierForm> BuildModifierForm(
            string datumId, string operationName) {
            var operation = FindModifyOperation(operationName);
            var dt = FindDatum(datumId, operationName);
            var dtt = typeof(TDat).GetDatumTypeFromType<TDat>();
            var finder = dtt.Finder();
            datumId = finder.GetDatumId(dt);
            var cmf = new Dictionary<string, ContentModifierForm>();
            foreach (var h in Handlers) {
                var parts = h.BuildModifierForm(dt, operation, DatumModelType);
                if (parts == null) continue;
                foreach (var part in parts) {
                    if (part.Value != null) {
                        if (string.IsNullOrWhiteSpace(part.Key)) {
                            throw new HttpException(400,
                                $"ProtoCMS: datum modifier form requires a valid key."
                            );
                        }
                        if (!ContentModifier.VALID_FORM_PART_KEY_REGEX.IsMatch(part.Key)) {
                            throw new HttpException(400,
                                $"ProtoCMS: datum modifier form key must match regex " +
                                $"'{ContentModifier.VALID_FORM_PART_KEY_REGEX}'."
                            );
                        }
                        cmf[part.Key] = part.Value;
                    }
                }
            }
            cmf[COMMON_DATUM_METADATA_FORM_KEY] = new CommonDatumModifierMetadataForm {
                DatumId = datumId,
                DatumTypeId = dtt?.Id,
                OperationName = operationName
            };
            return cmf;
        }

        public override IDictionary<string, VueComponentDefinition[]> ConvertFormToVues(
            IDictionary<string, ContentModifierForm> modifierForm, string datumId, string operationName) {
            var operation = FindModifyOperation(operationName);
            var dt = FindDatum(datumId, operationName);
            var dtId = DatumType?.Finder().GetDatumId(dt);
            var vues = new Dictionary<string, VueComponentDefinition[]>();
            var preVues = new List<VueComponentDefinition> {
                new VueHtmlWidget($"<p class='small text-muted'>{(operation.IsCreatingNewDatum ? "New " : "")}" +
                                  $"{DatumType?.Name} Id: <code>{dtId}</code></p>")
            };
            if (DatumType != null) {
                foreach (var getPreMovs in DatumType.PreModifyOperationFormVues) {
                    var preMovs = getPreMovs.Invoke(DatumType, dt, operation);
                    if (preMovs != null) preVues.AddRange(preMovs);
                }
            }
            vues[EXTRA_FORM_VUES_PRE_KEY] = preVues.ToArray();
            foreach (var h in Handlers) {
                var partVues = h.ConvertFormToVues(modifierForm, dt, operation, DatumModelType);
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
            if (DatumType != null) {
                foreach (var getPostMovs in DatumType.PostModifyOperationFormVues) {
                    var postMovs = getPostMovs.Invoke(DatumType, dt, operation);
                    if (postMovs != null) postVues.AddRange(postMovs);
                }
            }
            vues[EXTRA_FORM_VUES_POST_KEY] = postVues.ToArray();
            vues[COMMON_DATUM_METADATA_FORM_KEY] = new[] {
                new VueComponentDefinition {
                    Name = "cms-form-field-hidden",
                    Props = new {
                        valuePath = nameof(CommonDatumModifierMetadataForm.DatumId)
                    }
                },
                new VueComponentDefinition {
                    Name = "cms-form-field-hidden",
                    Props = new {
                        valuePath = nameof(CommonDatumModifierMetadataForm.DatumTypeId)
                    }
                },
                new VueComponentDefinition {
                    Name = "cms-form-field-hidden",
                    Props = new {
                        valuePath = nameof(CommonDatumModifierMetadataForm.OperationName)
                    }
                }
            };
            return vues;
        }

        public override FurtherValidationResult ValidateModifierForm(
            IDictionary<string, ContentModifierForm> modifierForm,
            string datumId, string operationName) {
            var operation = FindModifyOperation(operationName);
            var dt = FindDatum(datumId, operationName);
            var result = new FurtherValidationResult();
            foreach (var h in Handlers) {
                var r = h.ValidateModifierForm(modifierForm, dt, operation, DatumModelType);
                if (r != null && r.HasError) {
                    foreach (var kv in r.Errors) foreach (var m in kv.Value) result.AddError(kv.Key, m);
                }
            }
            if (modifierForm.TryGetValue(COMMON_DATUM_METADATA_FORM_KEY, out var aMetaF)) {
                if (!(aMetaF is CommonDatumModifierMetadataForm)) {
                    result.AddError(COMMON_DATUM_METADATA_FORM_KEY,
                        $"Meta form in field '{COMMON_DATUM_METADATA_FORM_KEY}' must be an instance of " +
                        $"'{typeof(CommonDatumModifierMetadataForm).FullName}'.");
                }
                var metaF = aMetaF.DirectCastTo<CommonDatumModifierMetadataForm>();
                
                if (metaF.OperationName != operationName) {
                    result.AddError(COMMON_DATUM_METADATA_FORM_KEY,
                        $"Operation name mismatch between request and meta form.");
                }

            } else {
                result.AddError(COMMON_DATUM_METADATA_FORM_KEY,
                    $"Meta form in field '{COMMON_DATUM_METADATA_FORM_KEY}' is required.");
            }
            return result;
        }

        public override void PerformModificationBase(IDictionary<string, ContentModifierForm> modifierForm,
            string datumId, out object modifiedDatum, string operationName) {
            PerformModification(modifierForm, datumId, out var modDat, operationName);
            modifiedDatum = modDat;
        }

        public void PerformModification(IDictionary<string, ContentModifierForm> modifierForm, string datumId,
            out TDat modifiedDatum, string operationName) {
            var operation = FindModifyOperation(operationName);
            var db = _dbContext.ThisDbContext();
            var finder = _dp.GetService(typeof(IDatumFinder<TDat>)).DirectCastTo<IDatumFinder<TDat>>();
            using (var dbTrx = db.Database.BeginTransaction()) {
                try {
                    modifiedDatum = FindDatum(datumId, operationName);
                    foreach (var h in Handlers) {
                        h.PerformModification(modifierForm, modifiedDatum, operation, DatumModelType);
                    }
                    dbTrx.Commit();
                } catch (Exception) {
                    dbTrx.Rollback();
                    throw;
                }
            }
        }

        public override DatumModifyOperation FindModifyOperation(string operationName) {
            var modOp = DefinedModifyOperations.FirstOrDefault(x => x.Name == operationName);
            if (modOp == null) {
                throw new HttpException(400, $"ProtoCMS: datum modify operation '{operationName}' " +
                                             $"doesn't exist.");
            }
            return modOp;
        }

        private IDatumPermissionsHandler<TDat> GetFirstPermissionsHandler() {
            if (PermissionsHandlers.Length > 0) {
                return PermissionsHandlers[0];
            }
            throw new InvalidOperationException(
                $"ProtoCMS: no permissions handler defined for datum type '{DatumModelType}'.");
        }
    }
}