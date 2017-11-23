using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Http;
using System.Web.Http.Description;
using JrzAsp.Lib.ProtoCms.Content.API;
using JrzAsp.Lib.ProtoCms.Content.Forms;
using JrzAsp.Lib.ProtoCms.Content.Models;
using JrzAsp.Lib.ProtoCms.Core.Models;
using JrzAsp.Lib.ProtoCms.Datum.Forms;
using JrzAsp.Lib.ProtoCms.Datum.Models;
using JrzAsp.Lib.ProtoCms.Datum.Permissions;
using JrzAsp.Lib.ProtoCms.Datum.Services;
using JrzAsp.Lib.ProtoCms.Vue.Models;
using JrzAsp.Lib.ProtoCms.WebApi.API;
using JrzAsp.Lib.ProtoCms.WebApi.Routing;
using JrzAsp.Lib.TypeUtilities;

namespace JrzAsp.Lib.ProtoCms.Datum.API {

    [RoutePrefixProtoCms("datum")]
    public class DatumApiController : BaseProtoApiController {
        private readonly IDependencyProvider _dp;

        public DatumApiController(IDependencyProvider dp) {
            _dp = dp;
        }

        [Route("{datumTypeId}/get")]
        [HttpGet]
        [ResponseType(typeof(object))]
        public IHttpActionResult GetDatum(string datumTypeId, string id, ContentListShape? shape = null) {
            try {
                var datumId = id;
                var rctx = ProtoCmsRuntimeContext.Current;
                var ct = FindDatumType(datumTypeId);
                CheckUserHasPermissionToViewDatum(ct, rctx);
                var finder = ct.FinderBase();
                var content = finder.FindByIdBase(datumId);
                if (content == null) {
                    throw new HttpException(404, $"ProtoCMS: no {ct.Id} datum found with id '{datumId}'.");
                }
                var getter = new DatumShapedApiModelBuilder();
                var result = getter.GetDatumShaped(content, ct, rctx, shape ?? ContentListShape.Normal);
                return JsonProto(result);
            } catch (HttpException hexc) {
                throw RestfulApiError(hexc);
            }
        }

        [Route("{datumTypeId}/list")]
        [HttpGet]
        [ResponseType(typeof(DatumListApiResult))]
        public IHttpActionResult ListData(string datumTypeId, string search = null, string sortColumn = null,
            bool? isDescending = null, int offset = 0, int limit = 100, ContentListShape? shape = null) {

            try {
                var rctx = ProtoCmsRuntimeContext.Current;
                var dt = FindDatumType(datumTypeId);
                CheckUserHasPermissionToViewDatum(dt, rctx);
                var result = new DatumListApiResult(rctx, dt, search, sortColumn, isDescending,
                    offset, limit, shape);
                return JsonProto(result);
            } catch (HttpException hexc) {
                throw RestfulApiError(hexc);
            }
        }

        [Route("{datumTypeId}/table-filters")]
        [HttpPost]
        [ResponseType(typeof(IDictionary<string, IDatumTableFilterHandler>))]
        public IHttpActionResult GetTableFiltersInfoFor(string datumTypeId) {
            try {
                var rctx = ProtoCmsRuntimeContext.Current;
                var dt = FindDatumType(datumTypeId);
                CheckUserHasPermissionToViewDatum(dt, rctx);
                var finder = dt.FinderBase();
                var result = new Dictionary<string, IDatumTableFilterHandler>();
                foreach (var tfh in finder.TableFilterHandlers) {
                    result.Add(tfh.Id, tfh);
                }
                return JsonProto(result);
            } catch (HttpException hexc) {
                throw RestfulApiError(hexc);
            }
        }

        [Route("{datumTypeId}/table-filters-form")]
        [HttpPost]
        [ResponseType(typeof(IDictionary<string, ContentTableFilterForm>))]
        public IHttpActionResult GetTableFiltersFormFor(string datumTypeId) {
            try {
                var rctx = ProtoCmsRuntimeContext.Current;
                var dt = FindDatumType(datumTypeId);
                CheckUserHasPermissionToViewDatum(dt, rctx);
                var finder = dt.FinderBase();
                var result = new Dictionary<string, ContentTableFilterForm>();
                foreach (var tfh in finder.TableFilterHandlers) {
                    var tff = tfh.BuildFilterForm(dt.ModelType);
                    if (tff == null) continue;
                    result.Add(tfh.Id, tff);
                }
                return JsonProto(result);
            } catch (HttpException hexc) {
                throw RestfulApiError(hexc);
            }
        }

        [Route("{datumTypeId}/table-filters-vue")]
        [HttpPost]
        [ResponseType(typeof(IDictionary<string, VueComponentDefinition[]>))]
        public IHttpActionResult GetTableFiltersVueFor(string datumTypeId) {
            try {
                var rctx = ProtoCmsRuntimeContext.Current;
                var dt = FindDatumType(datumTypeId);
                CheckUserHasPermissionToViewDatum(dt, rctx);
                var finder = dt.FinderBase();
                var result = new Dictionary<string, VueComponentDefinition[]>();
                foreach (var tfh in finder.TableFilterHandlers) {
                    var tff = tfh.BuildFilterForm(dt.ModelType);
                    if (tff == null) continue;
                    var tfv = tfh.FilterFormVues(tff, dt.ModelType);
                    if (tfv == null) continue;
                    result.Add(tfh.Id, tfv);
                }
                return JsonProto(result);
            } catch (HttpException hexc) {
                throw RestfulApiError(hexc);
            }
        }

        [Route("{datumTypeId}/list-filtered")]
        [HttpPost]
        [ResponseType(typeof(ContentListApiResult))]
        public IHttpActionResult ListDataFiltered([FromBody] IDictionary<string, ContentTableFilterForm> filters,
            string datumTypeId, string search = null, string sortColumn = null,
            bool? isDescending = null, int offset = 0, int limit = 100, ContentListShape? shape = null) {

            try {
                if (ModelState.IsValid) {
                    var rctx = ProtoCmsRuntimeContext.Current;
                    var dt = FindDatumType(datumTypeId);
                    CheckUserHasPermissionToViewDatum(dt, rctx);
                    var finder = dt.FinderBase();
                    if (filters != null) {
                        var filterOps = new List<ContentTableFilterOperation>();
                        foreach (var fkv in filters) {
                            if (!fkv.Value.__IsFilterEnabled) continue;
                            var filterHandler = finder.TableFilterHandlers.FirstOrDefault(x => x.Id == fkv.Key);
                            var filterOp = filterHandler?.SetupFilteringOperations(fkv.Value, dt.ModelType);
                            if (filterOp != null) filterOps.AddRange(filterOp);
                        }
                        finder = finder.TableFilterBase(filterOps);
                    }
                    var result = new DatumListApiResult(rctx, dt, search, sortColumn, isDescending,
                        offset, limit, shape, finder);
                    return JsonProto(result);
                }
                return ResponseMessage(Request.CreateErrorResponse(
                    HttpStatusCode.BadRequest, ModelState
                ));
            } catch (HttpException hexc) {
                throw RestfulApiError(hexc);
            }
        }

        [Route("{datumTypeId}/table-headers")]
        [HttpGet]
        [ResponseType(typeof(IEnumerable<ContentTableHeader>))]
        public IHttpActionResult BuildTableHeaders(string datumTypeId) {
            try {
                var ct = FindDatumType(datumTypeId);
                var rctx = ProtoCmsRuntimeContext.Current;
                CheckUserHasPermissionToViewDatum(ct, rctx);
                var finder = ct.FinderBase();
                var result = finder.TableHeaders();
                return JsonProto(result);
            } catch (HttpException hexc) {
                throw RestfulApiError(hexc);
            }
        }

        [Route("{datumTypeId}/form/{operationName}")]
        [HttpPost]
        [ResponseType(typeof(IDictionary<string, ContentModifierForm>))]
        public IHttpActionResult CreateModifierForm(string operationName, string datumTypeId,
            string id = null) {
            try {
                var datumId = id;
                var ct = FindDatumType(datumTypeId);
                var rctx = ProtoCmsRuntimeContext.Current;

                CheckModifyOperationAllowed(operationName, ct);

                var modifier = ct.ModifierBase();
                var modOp = modifier.FindModifyOperation(operationName);
                var modPerm = FindModifyDatumPermission(operationName, modifier);

                CheckUserHasPermissionToModifyDatum(rctx, modPerm, modOp, ct);
                var form = modifier.BuildModifierForm(datumId, operationName);

                return JsonProto(form);
            } catch (HttpException ex) {
                throw RestfulApiError(ex);
            }
        }

        [Route("{datumTypeId}/form-vue/{operationName}")]
        [HttpPost]
        [ResponseType(typeof(IDictionary<string, VueComponentDefinition[]>))]
        public IHttpActionResult CreateModifierFormVues(string operationName, string datumTypeId,
            string id = null) {
            try {
                var datumId = id;
                var ct = FindDatumType(datumTypeId);
                var rctx = ProtoCmsRuntimeContext.Current;

                CheckModifyOperationAllowed(operationName, ct);

                var modifier = ct.ModifierBase();
                var modOp = modifier.FindModifyOperation(operationName);
                var modPerm = FindModifyDatumPermission(operationName, modifier);

                CheckUserHasPermissionToModifyDatum(rctx, modPerm, modOp, ct);
                var form = modifier.BuildModifierForm(datumId, operationName);
                var vues = modifier.ConvertFormToVues(form, datumId, operationName);

                return JsonProto(vues);
            } catch (HttpException ex) {
                throw RestfulApiError(ex);
            }
        }

        [Route("{datumTypeId}/modify/{operationName}")]
        [HttpPost]
        [ResponseType(typeof(DatumModificationApiResult))]
        public IHttpActionResult PerformContentModification([FromBody] IDictionary<string, ContentModifierForm> form,
            string operationName, string datumTypeId) {
            try {
                var result = new DatumModificationApiResult();
                if (ModelState.IsValid) {
                    var dt = FindDatumType(datumTypeId);
                    result.DatumTypeId = dt.Id;
                    var rctx = ProtoCmsRuntimeContext.Current;

                    CheckModifyOperationAllowed(operationName, dt);

                    var modifier = dt.ModifierBase();
                    var finder = dt.FinderBase();
                    var modOp = modifier.FindModifyOperation(operationName);
                    result.OperationName = modOp.Name;
                    var modPerm = FindModifyDatumPermission(operationName, modifier);

                    CheckUserHasPermissionToModifyDatum(rctx, modPerm, modOp, dt);
                    var datumId = form[DatumModifier.COMMON_DATUM_METADATA_FORM_KEY]
                        .DirectCastTo<CommonDatumModifierMetadataForm>()
                        .DatumId;
                    var modValRes = modifier.ValidateModifierForm(form, datumId, operationName);
                    if (!modValRes.HasError) {
                        modifier.PerformModificationBase(form, datumId, out var datum, operationName);
                        var modDatumId = finder.GetDatumIdBase(datum);
                        result.DatumId = modDatumId;
                        result.IsSuccess = true;
                    } else {
                        foreach (var ferr in modValRes.Errors) {
                            foreach (var err in ferr.Value) {
                                result.ValidationResult.AddError(ferr.Key, err);
                            }
                        }
                    }
                } else {
                    var dictKeyRegex = new Regex(@"^(?<dictKey>form\[(?<dictIndex>\d+)\].Value)\.?",
                        RegexOptions.Compiled);
                    var formArr = null as KeyValuePair<string, ContentModifierForm>[];
                    foreach (var mserr in ModelState) {
                        foreach (var msg in mserr.Value.Errors) {
                            var errKey = mserr.Key;
                            var dictKeyMatch = dictKeyRegex.Match(errKey);
                            if (dictKeyMatch.Success) {
                                var dictKey = dictKeyMatch.Groups["dictKey"].Value;
                                var dictIndex = int.Parse(dictKeyMatch.Groups["dictIndex"].Value);
                                if (formArr == null) {
                                    formArr = form.ToArray();
                                }
                                errKey = errKey.Replace(dictKey, formArr[dictIndex].Key);
                            }
                            if (!string.IsNullOrWhiteSpace(msg.ErrorMessage)) {
                                result.ValidationResult.AddError(errKey, msg.ErrorMessage);
                            }
                            if (msg.Exception != null) {
                                var exmsg = msg.Exception.Message;
                                if (string.IsNullOrWhiteSpace(exmsg)) {
                                    exmsg = msg.Exception.GetType().ToString();
                                }
                                result.ValidationResult.AddError(errKey, exmsg);
                            }
                        }
                    }
                }
                return JsonProto(result);

            } catch (HttpException ex) {
                throw RestfulApiError(ex);
            }
        }

        protected DatumType FindDatumType(string datumTypeId) {
            var ct = DatumType.DefinedTypes.FirstOrDefault(x => x.Id == datumTypeId);
            if (ct == null) {
                throw new HttpException(404, $"ProtoCMS: no datum type found with id '{datumTypeId}'.");
            }
            return ct;
        }

        protected void CheckUserHasPermissionToViewDatum(DatumType dt, ProtoCmsRuntimeContext rctx) {
            if (!rctx.UserHasPermission(dt.ViewPermissionBase.Id)) {
                throw new HttpException(403, $"ProtoCMS: user is forbidden to view datum type '{dt.Id}'.");
            }
        }

        protected void CheckUserHasPermissionToModifyDatum(ProtoCmsRuntimeContext rctx,
            ModifyDatumPermission modPerm, DatumModifyOperation modOp, DatumType ct) {
            if (!rctx.UserHasPermission(modPerm.Id)) {
                throw new HttpException(403, $"ProtoCMS: user is forbidden to perform modify operation " +
                                             $"'{modOp.Name}' on datum type '{ct.Id}'.");
            }
        }

        protected void CheckModifyOperationAllowed(string operationName, DatumType ct) {
            if (!ct.IsModifyOperationAllowed(operationName)) {
                throw new HttpException(400, $"ProtoCMS: datum type '{ct.Id}' doesn't allow modify operation " +
                                             $"'{operationName}'.");
            }
        }

        protected ModifyDatumPermission FindModifyDatumPermission(string operationName, IDatumModifier modifier) {
            return modifier.PermissionsToModifyBase.FirstOrDefault(x => x.ModifyOperationName == operationName);
        }
    }
}