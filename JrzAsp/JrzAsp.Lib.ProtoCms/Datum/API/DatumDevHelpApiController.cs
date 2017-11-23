using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Description;
using JrzAsp.Lib.ProtoCms.Datum.Models;
using JrzAsp.Lib.ProtoCms.Datum.Services;
using JrzAsp.Lib.ProtoCms.WebApi.API;
using JrzAsp.Lib.ProtoCms.WebApi.Routing;
using JrzAsp.Lib.TypeUtilities;

namespace JrzAsp.Lib.ProtoCms.Datum.API {
    [RoutePrefixProtoCms("dev-help")]
    public class DatumDevHelpApiController : BaseProtoApiController {
        private readonly IDependencyProvider _dp;

        public DatumDevHelpApiController(IDependencyProvider dp) {
            _dp = dp;
        }

        [Route("datum-types")]
        [HttpGet]
        [ResponseType(typeof(IEnumerable<DatumType>))]
        public IHttpActionResult GetDatumTypeInfos() {
            return JsonProto(DatumType.DefinedTypes);
        }

        [Route("datum-field-names/{datumTypeId}")]
        [HttpGet]
        [ResponseType(typeof(IEnumerable<string>))]
        public IHttpActionResult GetDatumTypeFieldNames(string datumTypeId) {
            var dt = DatumType.DefinedTypesMap[datumTypeId];
            return JsonProto(dt.FinderBase().DefinedFieldNames());
        }

        [Route("datum-permissions-handlers")]
        [HttpGet]
        [ResponseType(typeof(IEnumerable<DatumHandlerApiInfo>))]
        public IHttpActionResult GetDatumPermissionsHandlers() {
            var types = DatumFinder.PermissionsHandlerTypes;
            var inf = new List<DatumHandlerApiInfo>();
            foreach (var t in types) {
                var h = _dp.GetService(t).DirectCastTo<IDatumHandler>();
                var dt = h.DatumModelType.GetDatumTypeFromType();
                inf.Add(new DatumHandlerApiInfo {
                    HandlerType = t,
                    HandledType = $"{h.DatumModelType.FullName}, {h.DatumModelType.Assembly.GetName().Name}",
                    HandledDatumType = $"{dt.Id}",
                    Priority = h.Priority
                });
            }
            inf = inf.OrderBy(x => x.Priority).ToList();
            return JsonProto(inf);
        }

        [Route("datum-getter-handlers")]
        [HttpGet]
        [ResponseType(typeof(IEnumerable<DatumHandlerApiInfo>))]
        public IHttpActionResult GetDatumFinderGetterHandlers() {
            var types = DatumFinder.GetterHandlerTypes;
            var inf = new List<DatumHandlerApiInfo>();
            foreach (var t in types) {
                var h = _dp.GetService(t).DirectCastTo<IDatumHandler>();
                var dt = h.DatumModelType.GetDatumTypeFromType();
                inf.Add(new DatumHandlerApiInfo {
                    HandlerType = t,
                    HandledType = $"{h.DatumModelType.FullName}, {h.DatumModelType.Assembly.GetName().Name}",
                    HandledDatumType = $"{dt.Id}",
                    Priority = h.Priority
                });
            }
            inf = inf.OrderBy(x => x.Priority).ToList();
            return JsonProto(inf);
        }

        [Route("datum-viewer-handlers")]
        [HttpGet]
        [ResponseType(typeof(IEnumerable<DatumHandlerApiInfo>))]
        public IHttpActionResult GetDatumFinderViewerHandlers() {
            var types = DatumFinder.ViewerHandlerTypes;
            var inf = new List<DatumHandlerApiInfo>();
            foreach (var t in types) {
                var h = _dp.GetService(t).DirectCastTo<IDatumHandler>();
                var dt = h.DatumModelType.GetDatumTypeFromType();
                inf.Add(new DatumHandlerApiInfo {
                    HandlerType = t,
                    HandledType = $"{h.DatumModelType.FullName}, {h.DatumModelType.Assembly.GetName().Name}",
                    HandledDatumType = $"{dt.Id}",
                    Priority = h.Priority
                });
            }
            inf = inf.OrderBy(x => x.Priority).ToList();
            return JsonProto(inf);
        }

        [Route("datum-search-handlers")]
        [HttpGet]
        [ResponseType(typeof(IEnumerable<DatumHandlerApiInfo>))]
        public IHttpActionResult GetDatumFinderSearchHandlers() {
            var types = DatumFinder.SearchHandlerTypes;
            var inf = new List<DatumHandlerApiInfo>();
            foreach (var t in types) {
                var h = _dp.GetService(t).DirectCastTo<IDatumHandler>();
                var dt = h.DatumModelType.GetDatumTypeFromType();
                inf.Add(new DatumHandlerApiInfo {
                    HandlerType = t,
                    HandledType = $"{h.DatumModelType.FullName}, {h.DatumModelType.Assembly.GetName().Name}",
                    HandledDatumType = $"{dt.Id}",
                    Priority = h.Priority
                });
            }
            inf = inf.OrderBy(x => x.Priority).ToList();
            return JsonProto(inf);
        }

        [Route("datum-where-handlers")]
        [HttpGet]
        [ResponseType(typeof(IEnumerable<DatumHandlerApiInfo>))]
        public IHttpActionResult GetDatumFinderWhereHandlers() {
            var types = DatumFinder.WhereHandlerTypes;
            var inf = new List<DatumHandlerApiInfo>();
            foreach (var t in types) {
                var h = _dp.GetService(t).DirectCastTo<IDatumHandler>();
                var dt = h.DatumModelType.GetDatumTypeFromType();
                inf.Add(new DatumHandlerApiInfo {
                    HandlerType = t,
                    HandledType = $"{h.DatumModelType.FullName}, {h.DatumModelType.Assembly.GetName().Name}",
                    HandledDatumType = $"{dt.Id}",
                    Priority = h.Priority
                });
            }
            inf = inf.OrderBy(x => x.Priority).ToList();
            return JsonProto(inf);
        }

        [Route("datum-sort-handlers")]
        [HttpGet]
        [ResponseType(typeof(IEnumerable<DatumHandlerApiInfo>))]
        public IHttpActionResult GetDatumFinderSortHandlers() {
            var types = DatumFinder.SortHandlerTypes;
            var inf = new List<DatumHandlerApiInfo>();
            foreach (var t in types) {
                var h = _dp.GetService(t).DirectCastTo<IDatumHandler>();
                var dt = h.DatumModelType.GetDatumTypeFromType();
                inf.Add(new DatumHandlerApiInfo {
                    HandlerType = t,
                    HandledType = $"{h.DatumModelType.FullName}, {h.DatumModelType.Assembly.GetName().Name}",
                    HandledDatumType = $"{dt.Id}",
                    Priority = h.Priority
                });
            }
            inf = inf.OrderBy(x => x.Priority).ToList();
            return JsonProto(inf);
        }

        [Route("datum-table-actions-handlers")]
        [HttpGet]
        [ResponseType(typeof(IEnumerable<DatumHandlerApiInfo>))]
        public IHttpActionResult GetDatumFinderTableActionsHandlers() {
            var types = DatumFinder.TableActionsHandlerTypes;
            var inf = new List<DatumHandlerApiInfo>();
            foreach (var t in types) {
                var h = _dp.GetService(t).DirectCastTo<IDatumHandler>();
                var dt = h.DatumModelType.GetDatumTypeFromType();
                inf.Add(new DatumHandlerApiInfo {
                    HandlerType = t,
                    HandledType = $"{h.DatumModelType.FullName}, {h.DatumModelType.Assembly.GetName().Name}",
                    HandledDatumType = $"{dt.Id}",
                    Priority = h.Priority
                });
            }
            inf = inf.OrderBy(x => x.Priority).ToList();
            return JsonProto(inf);
        }

        [Route("datum-table-filter-handlers")]
        [HttpGet]
        [ResponseType(typeof(IEnumerable<DatumHandlerApiInfo>))]
        public IHttpActionResult GetDatumFinderTableFilterHandlers() {
            var types = DatumFinder.TableFilterHandlerTypes;
            var inf = new List<DatumHandlerApiInfo>();
            foreach (var t in types) {
                var h = _dp.GetService(t).DirectCastTo<IDatumHandler>();
                var dt = h.DatumModelType.GetDatumTypeFromType();
                inf.Add(new DatumHandlerApiInfo {
                    HandlerType = t,
                    HandledType = $"{h.DatumModelType.FullName}, {h.DatumModelType.Assembly.GetName().Name}",
                    HandledDatumType = $"{dt.Id}",
                    Priority = h.Priority
                });
            }
            inf = inf.OrderBy(x => x.Priority).ToList();
            return JsonProto(inf);
        }

        [Route("datum-modifier-handlers")]
        [HttpGet]
        [ResponseType(typeof(IEnumerable<DatumHandlerApiInfo>))]
        public IHttpActionResult GetDatumModifierHandlers() {
            var types = DatumModifier.HandlerTypes;
            var inf = new List<DatumHandlerApiInfo>();
            foreach (var t in types) {
                var h = _dp.GetService(t).DirectCastTo<IDatumHandler>();
                var dt = h.DatumModelType.GetDatumTypeFromType();
                inf.Add(new DatumHandlerApiInfo {
                    HandlerType = t,
                    HandledType = $"{h.DatumModelType.FullName}, {h.DatumModelType.Assembly.GetName().Name}",
                    HandledDatumType = $"{dt.Id}",
                    Priority = h.Priority
                });
            }
            inf = inf.OrderBy(x => x.Priority).ToList();
            return JsonProto(inf);
        }

        [Route("datum-modify-operations")]
        [HttpGet]
        [ResponseType(typeof(IEnumerable<DatumModifyOperation>))]
        public IHttpActionResult GetDatumModifyOperations() {
            return JsonProto(DatumModifier.DefinedModifyOperations);
        }

        [Route("datum-where-conditions")]
        [HttpGet]
        [ResponseType(typeof(IEnumerable<DatumWhereCondition>))]
        public IHttpActionResult GetDatumWhereConditions() {
            return JsonProto(DatumFinder.DefinedWhereConditions);
        }
    }
}