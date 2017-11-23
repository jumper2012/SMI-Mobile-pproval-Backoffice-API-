using JrzAsp.Lib.ProtoCms.Content.Models;
using JrzAsp.Lib.ProtoCms.Core.Models;
using JrzAsp.Lib.ProtoCms.Vue.Models;

namespace JrzAsp.Lib.ProtoCms.Content.API {
    public class ContentShapedApiModelBuilder {
        public dynamic GetContentShaped(ProtoContent content, ContentType contentType,
            ProtoCmsRuntimeContext cmsContext, ContentListShape shape) {
            var finder = contentType.Finder();
            switch (shape) {
                case ContentListShape.TableRowVue:
                    var cdt = finder.AsTableRowVue(content);
                    cdt.Add(ContentListApiResult.CONTENT_TABLE_ACTION_KEY, new[] {
                        new VueComponentDefinition {
                            Name = "cms-widget-dropdown-button",
                            Props = new {
                                label = "Opt",
                                size = "xs",
                                type = "warning",
                                iconCssClass = "fa fa-angle-down",
                                items = finder.TableActionsForSingleContent(content.Id, cmsContext)
                            }
                        }
                    });
                    return cdt;
                case ContentListShape.Summary:
                    return new {
                        content.Id,
                        content.ContentTypeId,
                        Summary = finder.AsSummarizedValue(content, contentType.FieldNamesIncludedInSummary)
                    };
                case ContentListShape.FullPreview:
                    return finder.AsFullPreview(content);
                default:
                    return finder.AsDynamic(content);
            }
        }
    }
}