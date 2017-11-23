using JrzAsp.Lib.ProtoCms.Content.API;
using JrzAsp.Lib.ProtoCms.Core.Models;
using JrzAsp.Lib.ProtoCms.Datum.Models;
using JrzAsp.Lib.ProtoCms.Vue.Models;

namespace JrzAsp.Lib.ProtoCms.Datum.API {
    public class DatumShapedApiModelBuilder {
        public dynamic GetDatumShaped(object datum, DatumType datumType,
            ProtoCmsRuntimeContext cmsContext, ContentListShape shape) {
            var finder = datumType.FinderBase();
            var datumId = finder.GetDatumIdBase(datum);
            switch (shape) {
                case ContentListShape.TableRowVue:
                    var cdt = finder.AsTableRowVueBase(datum);
                    cdt.Add(DatumListApiResult.DATUM_TABLE_ACTION_KEY, new[] {
                        new VueComponentDefinition {
                            Name = "cms-widget-dropdown-button",
                            Props = new {
                                label = "Opt",
                                size = "xs",
                                type = "warning",
                                iconCssClass = "fa fa-angle-down",
                                items = finder.TableActionsForSingleContent(datumId, cmsContext)
                            }
                        }
                    });
                    return cdt;
                case ContentListShape.Summary:
                    return new {
                        DatumId = datumId,
                        DatumTypeId = datumType?.Id,
                        Summary = finder.AsSummarizedValueBase(datum)
                    };
                case ContentListShape.FullPreview:
                    return finder.AsFullPreviewBase(datum);
                default:
                    return datum;
            }
        }
    }
}