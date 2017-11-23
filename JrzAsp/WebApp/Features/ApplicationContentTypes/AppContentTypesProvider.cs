using System;
using System.Collections.Generic;
using JrzAsp.Lib.ProtoCms;
using JrzAsp.Lib.ProtoCms.Content.Models;
using JrzAsp.Lib.ProtoCms.Content.Services;
using JrzAsp.Lib.ProtoCms.Datum.Models;
using JrzAsp.Lib.ProtoCms.Datum.Services;
using JrzAsp.Lib.ProtoCms.Fields.Boolean;
using JrzAsp.Lib.ProtoCms.Fields.Chrono;
using JrzAsp.Lib.ProtoCms.Fields.FauxUrlSlug;
using JrzAsp.Lib.ProtoCms.Fields.FilePicker;
using JrzAsp.Lib.ProtoCms.Fields.Number;
using JrzAsp.Lib.ProtoCms.Fields.Select;
using JrzAsp.Lib.ProtoCms.Fields.Text;
using JrzAsp.Lib.ProtoCms.Vue.Models;
using Newtonsoft.Json;

namespace WebApp.Features.ApplicationContentTypes {
    public class AppContentTypesProvider : IContentTypesProvider {

        public IEnumerable<ContentType> DefineContentTypes() {
            yield return new ContentType(
                "sample-article",
                "Sample Article",
                "A sample article content.",
                new[] {
                    new ContentFieldDefinition("KataKata", typeof(TextField), new TextFieldConfiguration {
                        Label = "Title",
                        Validators = new Dictionary<string, ContentFieldValidatorConfiguration> {
                            ["required"] = null,
                            ["regex"] = new TextFieldRegexValidatorConfiguration {
                                Pattern = @"^[a-zA-Z0-9\ ]+$"
                            },
                            ["length"] = new TextFieldLengthValidatorConfiguration {
                                MaxLength = 256
                            }
                        }
                    }),
                    new ContentFieldDefinition("IsPowerful", typeof(BooleanField), new BooleanFieldConfiguration {
                        Label = "Does it happen?",
                        InitialValue = false,
                        TrueBoolLabel = "Indeed",
                        FalseBoolLabel = "Never",
                        Validators = new Dictionary<string, ContentFieldValidatorConfiguration> {
                            ["required"] = null
                        }
                    }),
                    new ContentFieldDefinition("MagicUtc", typeof(ChronoField), new ChronoFieldConfiguration {
                        Label = "When It Happen",
                        Kind = DateTimeKind.Local,
                        Validators = new Dictionary<string, ContentFieldValidatorConfiguration> {
                            ["required"] = null
                        }
                    }),
                    new ContentFieldDefinition("AngkaMantab", typeof(NumberField), new NumberFieldConfiguration {
                        Label = "Magic Number",
                        DefaultValue = 250,
                        NumberKind = NumberValueKind.Integer,
                        Validators = new Dictionary<string, ContentFieldValidatorConfiguration> {
                            ["required"] = null,
                            ["range"] = new NumberFieldRangeValidatorConfiguration {
                                MinValue = 100,
                                MaxValue = 1000000
                            }
                        }
                    }),
                    new ContentFieldDefinition("UrlSlug", typeof(FauxUrlSlugField), new FauxUrlSlugFieldConfiguration {
                        Label = "URL Slug",
                        ContentIdSlugPartLength = 8,
                        SlugPattern = "mantab/[###]/[MagicUtc:year]/[MagicUtc:month]/[KataKata:lowercase]"
                    }),
                    new ContentFieldDefinition("MyStuffs", typeof(SelectField), new SelectFieldConfiguration {
                        Label = "My Stuffs",
                        IsMultiSelect = true,
                        OptionsHandlerId = ContentSelectFieldOptionsHandler.HANDLER_ID,
                        OptionsHandlerParam = Jsonizer.Convert(new ContentSelectFieldOptionsHandlerParam {
                            ContentTypeIds = new[] {"sample-article"},
                            SortInfos = new[] {Tuple.Create("KataKata.Val", true)}
                        })
                    }),
                    new ContentFieldDefinition("MyFavArticle", typeof(SelectField), new SelectFieldConfiguration {
                        Label = "My Fav Article",
                        IsMultiSelect = false,
                        OptionsHandlerId = ContentSelectFieldOptionsHandler.HANDLER_ID,
                        OptionsHandlerParam = Jsonizer.Convert(new ContentSelectFieldOptionsHandlerParam {
                            ContentTypeIds = new[] {"sample-article"}
                        }),
                        Validators = new Dictionary<string, ContentFieldValidatorConfiguration> {
                            ["required"] = null
                        }
                    }),
                    new ContentFieldDefinition("File", typeof(FilePickerField), new FilePickerFieldConfiguration {
                        Label = "File",
                        IsMultiSelect = false,
                        Validators = new Dictionary<string, ContentFieldValidatorConfiguration> {
                            ["required"] = null,
                            ["file-extension"] = new FileExtensionValidatorConfiguration {
                                AllowedFileExtensions = new[] {"jpg", "png", "gif"}
                            }
                        }
                    }),
                    new ContentFieldDefinition("Files", typeof(FilePickerField), new FilePickerFieldConfiguration {
                        Label = "Files",
                        IsMultiSelect = true,
                        Validators = new Dictionary<string, ContentFieldValidatorConfiguration> {
                            ["required"] = null,
                            ["file-extension"] = new FileExtensionValidatorConfiguration {
                                AllowedFileExtensions = new[] {"jpg", "png", "gif"}
                            }
                        }
                    }),
                    new ContentFieldDefinition("BestMantapMan", typeof(SelectField), new SelectFieldConfiguration {
                        Label = "Best Mantap Man",
                        IsMultiSelect = false,
                        OptionsHandlerId = DatumSelectFieldOptionsHandler.HANDLER_ID,
                        OptionsHandlerParam = Jsonizer.Convert(new DatumSelectFieldOptionsHandlerParam {
                            DatumTypeId = "user",
                            WhereConditions = new[] {Tuple.Create("IsInRole", (object) new[] {"Mantap"})}
                        }),
                        Validators = new Dictionary<string, ContentFieldValidatorConfiguration> {
                            ["required"] = null
                        }
                    })
                },
                new[] {
                    "KataKata"
                },
                new[] {
                    "KataKata",
                    "IsPowerful",
                    "MagicUtc",
                    "AngkaMantab",
                    "UrlSlug",
                    "MyStuffs",
                    "MyFavArticle",
                    "File.Val",
                    "Files.Val",
                    "BestMantapMan",
                    $"{ContentType.FIELD_NAME_PUBLISH_STATUS}.IsPublished",
                    $"{ContentType.FIELD_NAME_PUBLISH_STATUS}.IsDraft",
                    $"{ContentType.FIELD_NAME_TRASH_STATUS}.IsTrashed",
                    $"{ContentType.FIELD_NAME_PUBLISH_STATUS}.PublishedAt",
                    $"{ContentType.FIELD_NAME_PUBLISH_STATUS}.UnpublishedAt",
                    $"{ContentType.FIELD_NAME_COMMON_META}.UpdatedAt",
                    $"{ContentType.FIELD_NAME_COMMON_META}.CreatedAt",
                    $"{ContentType.FIELD_NAME_COMMON_META}.Id"
                },
                "KataKata.Val",
                false,
                null,
                null
            ) {
                PreModifyOperationFormVues = new GetExtraModifyOperationFormVues[] {
                    (contentType, content, operation) => {
                        if (operation.IsCreateOperation()) {
                            return new VueComponentDefinition[] {
                                new VueHtmlWidget($"<p class='note note-info'>Let the creating magic happen!</p>")
                            };
                        }
                        if (operation.IsUpdateOperation()) {
                            return new VueComponentDefinition[] {
                                new VueHtmlWidget($"<p class='note note-info'>Let the updating magic happen!</p>")
                            };
                        }
                        return null;
                    }
                }
            };
        }
    }
}