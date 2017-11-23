using System.Collections.Generic;
using System.Web;
using System.Web.Http;
using System.Web.Http.Description;
using JrzAsp.Lib.ProtoCms.Core.Models;
using JrzAsp.Lib.ProtoCms.Setting.Models;
using JrzAsp.Lib.ProtoCms.Setting.Permissions;
using JrzAsp.Lib.ProtoCms.Setting.Services;
using JrzAsp.Lib.ProtoCms.Vue.Models;
using JrzAsp.Lib.ProtoCms.WebApi.API;
using JrzAsp.Lib.ProtoCms.WebApi.Routing;

namespace JrzAsp.Lib.ProtoCms.Setting.API {
    [RoutePrefixProtoCms("site-setting")]
    public class SiteSettingApiController : BaseProtoApiController {
        private readonly ISiteSettingManager _ssmgr;

        public SiteSettingApiController(ISiteSettingManager ssmgr) {
            _ssmgr = ssmgr;
        }

        [Route("{settingId?}")]
        [HttpGet]
        [ResponseType(typeof(IDictionary<string, SiteSetting>))]
        public IHttpActionResult ViewSettings(string settingId = null) {
            try {
                settingId = settingId?.Trim();
                var rctx = ProtoCmsRuntimeContext.Current;
                var viewAllowed = false;
                var result = new Dictionary<string, SiteSetting>();
                if (string.IsNullOrWhiteSpace(settingId)) {
                    var stgs = _ssmgr.GetSettings();
                    foreach (var kv in stgs) {
                        if (rctx.UserHasPermission(ViewSiteSettingPermission.GetIdFor(kv.Key))) {
                            result[kv.Key] = kv.Value;
                            viewAllowed = true;
                        }
                    }
                } else {
                    var stg = _ssmgr.GetSetting(settingId);
                    if (rctx.UserHasPermission(ViewSiteSettingPermission.GetIdFor(settingId))) {
                        result[settingId] = stg;
                        viewAllowed = true;
                    }
                }
                if (!viewAllowed) {
                    throw new HttpException(403, $"ProtoCMS: user has no permission to view the setting.");
                }
                return JsonProto(result);
            } catch (HttpException hexc) {
                throw RestfulApiError(hexc);
            }
        }

        [Route("{settingId}/form")]
        [HttpPost]
        [ResponseType(typeof(IDictionary<string, SiteSettingModifierForm>))]
        public IHttpActionResult BuildSettingModifyForm(string settingId) {
            try {
                settingId = settingId?.Trim();
                var rctx = ProtoCmsRuntimeContext.Current;
                var form = _ssmgr.BuildSettingModifierForm(settingId);
                CheckUserCanModifySetting(rctx, settingId);
                return JsonProto(new Dictionary<string, SiteSettingModifierForm> {["form"] = form});
            } catch (HttpException hexc) {
                throw RestfulApiError(hexc);
            }
        }

        [Route("{settingId}/form-vue")]
        [HttpPost]
        [ResponseType(typeof(IDictionary<string, VueComponentDefinition[]>))]
        public IHttpActionResult BuildSettingModifyFormVue(string settingId) {
            try {
                settingId = settingId?.Trim();
                var rctx = ProtoCmsRuntimeContext.Current;
                var form = _ssmgr.BuildSettingModifierForm(settingId);
                CheckUserCanModifySetting(rctx, settingId);
                var vues = _ssmgr.CreateSettingModifierFormVues(settingId, form);
                return JsonProto(new Dictionary<string, VueComponentDefinition[]> {["vues"] = vues});
            } catch (HttpException hexc) {
                throw RestfulApiError(hexc);
            }
        }

        [Route("{settingId}/modify")]
        [HttpPost]
        [ResponseType(typeof(SiteSettingModificationApiResult))]
        public IHttpActionResult ModifySetting(string settingId,
            [FromBody] IDictionary<string, SiteSettingModifierForm> modForm) {
            try {
                var result = new SiteSettingModificationApiResult {SettingId = settingId};
                if (ModelState.IsValid) {
                    var rctx = ProtoCmsRuntimeContext.Current;
                    var fm = modForm["form"];
                    CheckUserCanModifySetting(rctx, settingId);
                    var modValRes = _ssmgr.ValidateSettingModifierForm(settingId, fm);
                    if (!modValRes.HasError) {
                        _ssmgr.UpdateSetting(settingId, fm);
                        result.IsSuccess = true;
                    } else {
                        foreach (var ferr in modValRes.Errors) {
                            foreach (var err in ferr.Value) {
                                result.ValidationResult.AddError(ferr.Key, err);
                            }
                        }
                    }
                } else {
                    foreach (var mserr in ModelState) {
                        foreach (var msg in mserr.Value.Errors) {
                            if (!string.IsNullOrWhiteSpace(msg.ErrorMessage)) {
                                result.ValidationResult.AddError(mserr.Key?.Replace("modForm[0].Value.", ""),
                                    msg.ErrorMessage);
                            }
                            if (msg.Exception != null) {
                                var exmsg = msg.Exception.Message;
                                if (string.IsNullOrWhiteSpace(exmsg)) {
                                    exmsg = msg.Exception.GetType().ToString();
                                }
                                result.ValidationResult.AddError(mserr.Key?.Replace("modForm[0].Value.", ""), exmsg);
                            }
                        }
                    }
                }
                return JsonProto(result);
            } catch (HttpException hexc) {
                throw RestfulApiError(hexc);
            }
        }

        protected void CheckUserCanModifySetting(ProtoCmsRuntimeContext rctx, string settingId) {
            if (!rctx.UserHasPermission(ModifySiteSettingPermission.GetIdFor(settingId))) {
                throw new HttpException(403,
                    $"ProtoCMS: user has no permission to modify setting '{settingId}'.");
            }
        }
    }
}