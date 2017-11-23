using System;
using JrzAsp.Lib.ProtoCms.Content.Models;
using JrzAsp.Lib.ProtoCms.Datum.Services;
using JrzAsp.Lib.ProtoCms.Permission.Models;
using JrzAsp.Lib.ProtoCms.Vue.Models;

namespace JrzAsp.Lib.ProtoCms.Permission.Datum {
    public class PermissionDatumViewerHandler : BaseDatumViewerHandler<ProtoPermission> {
        public override decimal Priority => 0;

        public override string[] GetValidFieldNames() {
            return new[] {
                nameof(ProtoPermission.CategoryName),
                nameof(ProtoPermission.SubCategoryName),
                nameof(ProtoPermission.DisplayName),
                nameof(ProtoPermission.Description),
                nameof(ProtoPermission.Id),
            };
        }

        public override ContentTableHeader GetTableHeader(string fieldName) {
            switch (fieldName) {
                case nameof(ProtoPermission.CategoryName):
                    return new ContentTableHeader(
                        nameof(ProtoPermission.CategoryName),
                        "Category",
                        true
                    );
                case nameof(ProtoPermission.SubCategoryName):
                    return new ContentTableHeader(
                        nameof(ProtoPermission.SubCategoryName),
                        "Sub Category",
                        true
                    );
                case nameof(ProtoPermission.DisplayName):
                    return new ContentTableHeader(
                        nameof(ProtoPermission.DisplayName),
                        "Display Name",
                        true
                    );
                case nameof(ProtoPermission.Description):
                    return new ContentTableHeader(
                        nameof(ProtoPermission.Description),
                        "Description",
                        true
                    );
                case nameof(ProtoPermission.Id):
                    return new ContentTableHeader(
                        nameof(ProtoPermission.Id),
                        "Id",
                        true
                    );
            }
            return null;
        }

        public override string GetSummarizedValue(ProtoPermission datum, string fieldName) {
            switch (fieldName) {
                case nameof(ProtoPermission.CategoryName):
                    return datum.CategoryName;
                case nameof(ProtoPermission.SubCategoryName):
                    return datum.SubCategoryName;
                case nameof(ProtoPermission.DisplayName):
                    return datum.DisplayName;
                case nameof(ProtoPermission.Description):
                    return datum.Description;
                case nameof(ProtoPermission.Id):
                    return datum.Id;
            }
            return null;
        }

        public override VueComponentDefinition[] GetTableRowVue(ProtoPermission datum, string fieldName) {
            switch (fieldName) {
                case nameof(ProtoPermission.CategoryName):
                    return new VueComponentDefinition[] {
                        new VueHtmlWidget(datum.CategoryName),
                    };
                case nameof(ProtoPermission.SubCategoryName):
                    return new VueComponentDefinition[] {
                        new VueHtmlWidget(datum.SubCategoryName),
                    };
                case nameof(ProtoPermission.DisplayName):
                    return new VueComponentDefinition[] {
                        new VueHtmlWidget(datum.DisplayName),
                    };
                case nameof(ProtoPermission.Description):
                    return new VueComponentDefinition[] {
                        new VueHtmlWidget(datum.Description),
                    };
                case nameof(ProtoPermission.Id):
                    return new VueComponentDefinition[] {
                        new VueHtmlWidget(datum.Id),
                    };
            }
            return null;
        }

        public override ContentPreviewPart GetFullPreview(ProtoPermission datum, string fieldName) {switch (fieldName) {
                case nameof(ProtoPermission.CategoryName):
                    return new ContentPreviewPart(
                        "Category",
                        nameof(ProtoPermission.CategoryName),
                        GetTableRowVue(datum, fieldName));
                case nameof(ProtoPermission.SubCategoryName):
                    return new ContentPreviewPart(
                        "Sub Category",
                        nameof(ProtoPermission.SubCategoryName),
                        GetTableRowVue(datum, fieldName));
                case nameof(ProtoPermission.DisplayName):
                    return new ContentPreviewPart(
                        "Display Name",
                        nameof(ProtoPermission.DisplayName),
                        GetTableRowVue(datum, fieldName));
                case nameof(ProtoPermission.Description):
                    return new ContentPreviewPart(
                        "Description",
                        nameof(ProtoPermission.Description),
                        GetTableRowVue(datum, fieldName));
                case nameof(ProtoPermission.Id):
                    return new ContentPreviewPart(
                        "Id",
                        nameof(ProtoPermission.Id),
                        GetTableRowVue(datum, fieldName));
            }
            return null;
        }
    }
}