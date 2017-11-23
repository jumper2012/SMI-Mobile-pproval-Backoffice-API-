using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Description;
using JrzAsp.Lib.ProtoCms.Content.Models;
using JrzAsp.Lib.ProtoCms.Content.Services;
using JrzAsp.Lib.ProtoCms.WebApi.API;
using JrzAsp.Lib.ProtoCms.WebApi.Routing;
using JrzAsp.Lib.TypeUtilities;

namespace JrzAsp.Lib.ProtoCms.Content.API {
    [RoutePrefixProtoCms("dev-help")]
    public class ContentDevHelpApiController : BaseProtoApiController {
        private readonly IDependencyProvider _dp;

        public ContentDevHelpApiController(IDependencyProvider dp) {
            _dp = dp;
        }

        [Route("content-types")]
        [HttpGet]
        [ResponseType(typeof(IEnumerable<ContentType>))]
        public IHttpActionResult GetContentTypes() {
            return JsonProto(ContentType.DefinedTypes);
        }

        [Route("content-fields")]
        [HttpGet]
        [ResponseType(typeof(IEnumerable<ContentFieldApiInfo>))]
        public IHttpActionResult GetContentFields() {
            var fieldModelTypes = TypesCache.AppDomainTypes.Where(
                x => typeof(ContentField).IsAssignableFrom(x) && x.GetConstructor(Type.EmptyTypes) != null && x.IsClass
                     && !x.IsInterface && !x.IsAbstract && !x.IsGenericTypeDefinition
            );
            var inf = new List<ContentFieldApiInfo>();
            foreach (var fmt in fieldModelTypes) {
                var fmdl = Activator.CreateInstance(fmt).DirectCastTo<ContentField>();
                inf.Add(new ContentFieldApiInfo {
                    ModelType = fmt,
                    FieldSpec = fmdl.__FieldSpec
                });
            }
            return JsonProto(inf);
        }

        [Route("content-search-handlers")]
        [HttpGet]
        [ResponseType(typeof(IEnumerable<ContentHandlerApiInfo>))]
        public IHttpActionResult GetContentFinderSearchHandlers() {
            var types = ContentFinder.SearchHandlerTypes;
            var inf = new List<ContentHandlerApiInfo>();
            foreach (var t in types) {
                var h = _dp.GetService(t).DirectCastTo<IContentHandler>();
                inf.Add(new ContentHandlerApiInfo {
                    HandlerType = t,
                    HandledContentTypes = h.HandledContentTypeIds,
                    Priority = h.Priority
                });
            }
            inf = inf.OrderBy(x => x.Priority).ToList();
            return JsonProto(inf);
        }

        [Route("content-where-handlers")]
        [HttpGet]
        [ResponseType(typeof(IEnumerable<ContentHandlerApiInfo>))]
        public IHttpActionResult GetContentFinderWhereHandlers() {
            var types = ContentFinder.WhereHandlerTypes;
            var inf = new List<ContentHandlerApiInfo>();
            foreach (var t in types) {
                var h = _dp.GetService(t).DirectCastTo<IContentHandler>();
                inf.Add(new ContentHandlerApiInfo {
                    HandlerType = t,
                    HandledContentTypes = h.HandledContentTypeIds,
                    Priority = h.Priority
                });
            }
            inf = inf.OrderBy(x => x.Priority).ToList();
            return JsonProto(inf);
        }

        [Route("content-sort-handlers")]
        [HttpGet]
        [ResponseType(typeof(IEnumerable<ContentHandlerApiInfo>))]
        public IHttpActionResult GetContentFinderSortHandlers() {
            var types = ContentFinder.SortHandlerTypes;
            var inf = new List<ContentHandlerApiInfo>();
            foreach (var t in types) {
                var h = _dp.GetService(t).DirectCastTo<IContentHandler>();
                inf.Add(new ContentHandlerApiInfo {
                    HandlerType = t,
                    HandledContentTypes = h.HandledContentTypeIds,
                    Priority = h.Priority
                });
            }
            inf = inf.OrderBy(x => x.Priority).ToList();
            return JsonProto(inf);
        }

        [Route("content-table-actions-handlers")]
        [HttpGet]
        [ResponseType(typeof(IEnumerable<ContentHandlerApiInfo>))]
        public IHttpActionResult GetContentFinderTableActionsHandlers() {
            var types = ContentFinder.TableActionsHandlerTypes;
            var inf = new List<ContentHandlerApiInfo>();
            foreach (var t in types) {
                var h = _dp.GetService(t).DirectCastTo<IContentTableActionsHandler>();
                inf.Add(new ContentHandlerApiInfo {
                    HandlerType = t,
                    HandledContentTypes = h.HandledContentTypeIds,
                    Priority = h.Priority
                });
            }
            inf = inf.OrderBy(x => x.Priority).ToList();
            return JsonProto(inf);
        }

        [Route("content-table-filter-handlers")]
        [HttpGet]
        [ResponseType(typeof(IEnumerable<ContentTableFilterHandlerApiInfo>))]
        public IHttpActionResult GetContentTableFilterHandlers() {
            var types = ContentFinder.TableFilterHandlerTypes;
            var inf = new List<ContentTableFilterHandlerApiInfo>();
            foreach (var t in types) {
                var h = _dp.GetService(t).DirectCastTo<IContentTableFilterHandler>();
                inf.Add(new ContentTableFilterHandlerApiInfo {
                    HandlerType = t,
                    HandledContentTypes = h.HandledContentTypeIds,
                    Priority = h.Priority,
                    Id = h.Id,
                    Name = h.Name,
                    Description = h.Description
                });
            }
            inf = inf.OrderBy(x => x.Priority).ToList();
            return JsonProto(inf);
        }

        [Route("content-modifier-handlers")]
        [HttpGet]
        [ResponseType(typeof(IEnumerable<ContentHandlerApiInfo>))]
        public IHttpActionResult GetContentModifierHandlers() {
            var types = ContentModifier.HandlerTypes;
            var inf = new List<ContentHandlerApiInfo>();
            foreach (var t in types) {
                var h = _dp.GetService(t).DirectCastTo<IContentModifierHandler>();
                inf.Add(new ContentHandlerApiInfo {
                    HandlerType = t,
                    HandledContentTypes = h.HandledContentTypeIds,
                    Priority = h.Priority
                });
            }
            inf = inf.OrderBy(x => x.Priority).ToList();
            return JsonProto(inf);
        }

        [Route("content-field-validators")]
        [HttpGet]
        [ResponseType(typeof(IDictionary<Type, ContentFieldValidator>))]
        public IHttpActionResult GetContentFieldValidators() {
            var valrs = ContentFieldValidator.DefinedValidators;
            var result = new Dictionary<Type, ContentFieldValidator>();
            foreach (var valr in valrs) {
                result[valr.GetType()] = valr;
            }
            return JsonProto(result);
        }

        [Route("content-modify-operations")]
        [HttpGet]
        [ResponseType(typeof(IEnumerable<ContentModifyOperation>))]
        public IHttpActionResult GetContentModifyOperations() {
            return JsonProto(ContentModifier.DefinedModifyOperations);
        }

        [Route("content-where-conditions")]
        [HttpGet]
        [ResponseType(typeof(IEnumerable<ContentWhereCondition>))]
        public IHttpActionResult GetContentWhereConditions() {
            return JsonProto(ContentFinder.DefinedWhereConditions);
        }
    }
}