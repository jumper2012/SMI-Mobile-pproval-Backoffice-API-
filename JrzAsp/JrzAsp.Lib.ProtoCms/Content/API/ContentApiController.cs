using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Http;
using System.Web.Http.Description;
using JrzAsp.Lib.ProtoCms.Content.Forms;
using JrzAsp.Lib.ProtoCms.Content.Models;
using JrzAsp.Lib.ProtoCms.Content.Permissions;
using JrzAsp.Lib.ProtoCms.Content.Services;
using JrzAsp.Lib.ProtoCms.Core.Models;
using JrzAsp.Lib.ProtoCms.Fields.Common;
using JrzAsp.Lib.ProtoCms.Vue.Models;
using JrzAsp.Lib.ProtoCms.WebApi.API;
using JrzAsp.Lib.ProtoCms.WebApi.Routing;
using JrzAsp.Lib.TypeUtilities;

namespace JrzAsp.Lib.ProtoCms.Content.API {
    [RoutePrefixProtoCms("content")]
    public class ContentApiController : BaseProtoApiController {
        private readonly IDependencyProvider _dp;

        public ContentApiController(IDependencyProvider dp) {
            _dp = dp;
        }

        [Route("{contentTypeId}/get")]
        [HttpGet]
        [ResponseType(typeof(object))]
        public IHttpActionResult GetContent(string contentTypeId, string id, ContentListShape? shape = null) {
            try {
                var contentId = id;
                var rctx = ProtoCmsRuntimeContext.Current;
                var ct = FindContentType(contentTypeId);
                CheckUserHasPermissionToViewContent(ct, rctx);
                var finder = ct.Finder();
                var content = finder.FindById(contentId);
                if (content == null) {
                    throw new HttpException(404, $"ProtoCMS: no {ct.Id} content found with id '{contentId}'.");
                }
                var getter = new ContentShapedApiModelBuilder();
                var result = getter.GetContentShaped(content, ct, rctx, shape ?? ContentListShape.Normal);
                return JsonProto(result);
            } catch (HttpException hexc) {
                throw RestfulApiError(hexc);
            }
        }

        [Route("{contentTypeId}/list")]
        [HttpGet]
        [ResponseType(typeof(ContentListApiResult))]
        public IHttpActionResult ListContents(string contentTypeId, string search = null, string sortColumn = null,
            bool? isDescending = null, int offset = 0, int limit = 100, ContentListShape? shape = null) {

            try {
                var rctx = ProtoCmsRuntimeContext.Current;
                var ct = FindContentType(contentTypeId);
                CheckUserHasPermissionToViewContent(ct, rctx);
                var result = new ContentListApiResult(rctx, ct, search, sortColumn, isDescending,
                    offset, limit, shape);
                return JsonProto(result);
            } catch (HttpException hexc) {
                throw RestfulApiError(hexc);
            }
        }

        [Route("{contentTypeId}/table-filters")]
        [HttpPost]
        [ResponseType(typeof(IDictionary<string, IContentTableFilterHandler>))]
        public IHttpActionResult GetTableFiltersInfoFor(string contentTypeId) {
            try {
                var rctx = ProtoCmsRuntimeContext.Current;
                var ct = FindContentType(contentTypeId);
                CheckUserHasPermissionToViewContent(ct, rctx);
                var finder = ct.Finder();
                var result = new Dictionary<string, IContentTableFilterHandler>();
                foreach (var tfh in finder.TableFilterHandlers) {
                    result.Add(tfh.Id, tfh);
                }
                return JsonProto(result);
            } catch (HttpException hexc) {
                throw RestfulApiError(hexc);
            }
        }

        [Route("{contentTypeId}/table-filters-form")]
        [HttpPost]
        [ResponseType(typeof(IDictionary<string, ContentTableFilterForm>))]
        public IHttpActionResult GetTableFiltersFormFor(string contentTypeId) {
            try {
                var rctx = ProtoCmsRuntimeContext.Current;
                var ct = FindContentType(contentTypeId);
                CheckUserHasPermissionToViewContent(ct, rctx);
                var finder = ct.Finder();
                var result = new Dictionary<string, ContentTableFilterForm>();
                foreach (var tfh in finder.TableFilterHandlers) {
                    var tff = tfh.BuildFilterForm(ct);
                    if (tff == null) continue;
                    result.Add(tfh.Id, tff);
                }
                return JsonProto(result);
            } catch (HttpException hexc) {
                throw RestfulApiError(hexc);
            }
        }

        [Route("{contentTypeId}/table-filters-vue")]
        [HttpPost]
        [ResponseType(typeof(IDictionary<string, VueComponentDefinition[]>))]
        public IHttpActionResult GetTableFiltersVueFor(string contentTypeId) {
            try {
                var rctx = ProtoCmsRuntimeContext.Current;
                var ct = FindContentType(contentTypeId);
                CheckUserHasPermissionToViewContent(ct, rctx);
                var finder = ct.Finder();
                var result = new Dictionary<string, VueComponentDefinition[]>();
                foreach (var tfh in finder.TableFilterHandlers) {
                    var tff = tfh.BuildFilterForm(ct);
                    if (tff == null) continue;
                    var tfv = tfh.FilterFormVues(tff, ct);
                    if (tfv == null) continue;
                    result.Add(tfh.Id, tfv);
                }
                return JsonProto(result);
            } catch (HttpException hexc) {
                throw RestfulApiError(hexc);
            }
        }

        [Route("{contentTypeId}/list-filtered")]
        [HttpPost]
        [ResponseType(typeof(ContentListApiResult))]
        public IHttpActionResult ListContentsFiltered([FromBody] IDictionary<string, ContentTableFilterForm> filters,
            string contentTypeId, string search = null, string sortColumn = null,
            bool? isDescending = null, int offset = 0, int limit = 100, ContentListShape? shape = null) {

            try {
                if (ModelState.IsValid) {
                    var rctx = ProtoCmsRuntimeContext.Current;
                    var ct = FindContentType(contentTypeId);
                    CheckUserHasPermissionToViewContent(ct, rctx);
                    var finder = ct.Finder();
                    var filterOps = new List<ContentTableFilterOperation>();
                    if (filters != null) {
                        foreach (var fkv in filters) {
                            if (!fkv.Value.__IsFilterEnabled) continue;
                            var filterHandler = finder.TableFilterHandlers.FirstOrDefault(x => x.Id == fkv.Key);
                            var filterOp = filterHandler?.SetupFilteringOperations(fkv.Value, ct);
                            if (filterOp != null) filterOps.AddRange(filterOp);
                        }
                        finder = finder.TableFilter(filterOps);
                    }
                    var result = new ContentListApiResult(rctx, ct, search, sortColumn, isDescending,
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

        [Route("{contentTypeId}/table-headers")]
        [HttpGet]
        [ResponseType(typeof(IEnumerable<ContentTableHeader>))]
        public IHttpActionResult BuildTableHeaders(string contentTypeId) {
            try {
                var ct = FindContentType(contentTypeId);
                var rctx = ProtoCmsRuntimeContext.Current;
                CheckUserHasPermissionToViewContent(ct, rctx);
                var finder = ct.Finder();
                var result = finder.TableHeaders();
                return JsonProto(result);
            } catch (HttpException hexc) {
                throw RestfulApiError(hexc);
            }
        }

        [Route("{contentTypeId}/form/{operationName}")]
        [HttpPost]
        [ResponseType(typeof(IDictionary<string, ContentModifierForm>))]
        public IHttpActionResult CreateModifierForm(string operationName, string contentTypeId,
            string id = null) {
            try {
                var contentId = id;
                var ct = FindContentType(contentTypeId);
                var rctx = ProtoCmsRuntimeContext.Current;

                CheckModifyOperationAllowed(operationName, ct);

                var modifier = ct.Modifier();
                var modOp = modifier.FindModifyOperation(operationName);
                var modPerm = modOp.BuildPermission(ct);

                CheckUserHasPermissionToModifyContent(rctx, modPerm, modOp, ct);
                var form = modifier.BuildModifierForm(contentId, operationName);

                return JsonProto(form);
            } catch (HttpException ex) {
                throw RestfulApiError(ex);
            }
        }

        [Route("{contentTypeId}/form-vue/{operationName}")]
        [HttpPost]
        [ResponseType(typeof(IDictionary<string, VueComponentDefinition[]>))]
        public IHttpActionResult CreateModifierFormVues(string operationName, string contentTypeId,
            string id = null) {
            try {
                var contentId = id;
                var ct = FindContentType(contentTypeId);
                var rctx = ProtoCmsRuntimeContext.Current;

                CheckModifyOperationAllowed(operationName, ct);

                var modifier = ct.Modifier();
                var modOp = modifier.FindModifyOperation(operationName);
                var modPerm = modOp.BuildPermission(ct);

                CheckUserHasPermissionToModifyContent(rctx, modPerm, modOp, ct);
                var form = modifier.BuildModifierForm(contentId, operationName);
                var vues = modifier.ConvertFormToVues(form, contentId, operationName);

                return JsonProto(vues);
            } catch (HttpException ex) {
                throw RestfulApiError(ex);
            }
        }

        [Route("{contentTypeId}/modify/{operationName}")]
        [HttpPost]
        [ResponseType(typeof(ContentModificationApiResult))]
        public IHttpActionResult PerformContentModification([FromBody] IDictionary<string, ContentModifierForm> form,
            string operationName, string contentTypeId) {
            try {
                var result = new ContentModificationApiResult();
                if (ModelState.IsValid) {
                    var ct = FindContentType(contentTypeId);
                    result.ContentTypeId = ct.Id;
                    var rctx = ProtoCmsRuntimeContext.Current;

                    CheckModifyOperationAllowed(operationName, ct);

                    var modifier = ct.Modifier();
                    var modOp = modifier.FindModifyOperation(operationName);
                    result.OperationName = modOp.Name;
                    var modPerm = modOp.BuildPermission(ct);

                    CheckUserHasPermissionToModifyContent(rctx, modPerm, modOp, ct);
                    var contentId = form[ContentType.FIELD_NAME_COMMON_META]
                        .DirectCastTo<CommonFieldModifierForm>()
                        .ContentId;
                    var modValRes = modifier.ValidateModifierForm(form, contentId, operationName);
                    if (!modValRes.HasError) {
                        modifier.PerformModification(form, contentId, out var content, operationName);
                        result.ContentId = content.Id;
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

        protected void CheckUserHasPermissionToModifyContent(ProtoCmsRuntimeContext rctx,
            ModifyContentPermission modPerm, ContentModifyOperation modOp, ContentType ct) {
            if (!rctx.UserHasPermission(modPerm.Id)) {
                throw new HttpException(403, $"ProtoCMS: user is forbidden to perform modify operation " +
                                             $"'{modOp.Name}' on content type '{ct.Id}'.");
            }
        }

        protected void CheckModifyOperationAllowed(string operationName, ContentType ct) {
            if (!ct.IsModifyOperationAllowed(operationName)) {
                throw new HttpException(400, $"ProtoCMS: content type '{ct.Id}' doesn't allow modify operation " +
                                             $"'{operationName}'.");
            }
        }

        protected ContentType FindContentType(string contentTypeId) {
            var ct = ContentType.DefinedTypes.FirstOrDefault(x => x.Id == contentTypeId);
            if (ct == null) {
                throw new HttpException(404, $"ProtoCMS: no content type found with id '{contentTypeId}'.");
            }
            return ct;
        }

        protected void CheckUserHasPermissionToViewContent(ContentType ct, ProtoCmsRuntimeContext rctx) {
            var viewPerm = new ViewContentPermission(ct);
            if (!rctx.UserHasPermission(viewPerm.Id)) {
                throw new HttpException(403, $"ProtoCMS: user is forbidden to view content type '{ct.Id}'.");
            }
        }
    }
}