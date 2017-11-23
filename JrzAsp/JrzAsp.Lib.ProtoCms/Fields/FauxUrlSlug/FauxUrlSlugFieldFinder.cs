using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text.RegularExpressions;
using JrzAsp.Lib.ProtoCms.Content.Models;
using JrzAsp.Lib.ProtoCms.Content.Services;
using JrzAsp.Lib.ProtoCms.Vue.Models;
using JrzAsp.Lib.TypeUtilities;

namespace JrzAsp.Lib.ProtoCms.Fields.FauxUrlSlug {
    public class FauxUrlSlugFieldFinder : IContentFieldFinder {
        public const string ID_PATTERN_PART = "[###]";

        public static readonly string PART_PATTERN_COMPLEX_REGEX =
            $@"^\[(?<fieldName>[A-Z_][_A-Za-z0-9]+):(?<slugGeneratorParam>.*)\]$";

        public static readonly string PART_PATTERN_SIMPLE_REGEX =
            $@"^\[(?<fieldName>[A-Z_][_A-Za-z0-9]+)\]$";

        private static Regex _partPatternComplexRegex;
        private static Regex _partPatternSimpleRegex;

        private readonly IProtoCmsMainUrlsProvider _urlProv;

        public FauxUrlSlugFieldFinder(IProtoCmsMainUrlsProvider urlProv) {
            _urlProv = urlProv;
        }

        public static Regex PartPatternSimpleRegex {
            get {
                if (_partPatternSimpleRegex != null) return _partPatternSimpleRegex;
                _partPatternSimpleRegex = new Regex(PART_PATTERN_SIMPLE_REGEX, RegexOptions.Compiled);
                return _partPatternSimpleRegex;
            }
        }
        public static Regex PartPatternComplexRegex {
            get {
                if (_partPatternComplexRegex != null) return _partPatternComplexRegex;
                _partPatternComplexRegex = new Regex(PART_PATTERN_COMPLEX_REGEX, RegexOptions.Compiled);
                return _partPatternComplexRegex;
            }
        }

        public ContentFieldColumn[] Columns => new[] {
            new ContentFieldColumn(nameof(FauxUrlSlugField.Slug),
                definition => definition.Config.DirectCastTo<FauxUrlSlugFieldConfiguration>().Label ??
                              definition.FieldName,
                definition => false,
                CellValue,
                SummarizedValue,
                FullPreviewValue)
        };

        public ContentField GetModel(ProtoContent content, ContentFieldDefinition fieldDefinition) {
            var mdl = new FauxUrlSlugField();
            var ct = fieldDefinition.ContentType;
            var fcfg = fieldDefinition.Config.DirectCastTo<FauxUrlSlugFieldConfiguration>();
            var slugPatternParts = fcfg.SlugPattern.Trim('/').Split('/');
            var slugParts = new List<string>();
            foreach (var spp in slugPatternParts) {
                if (spp == ID_PATTERN_PART) {
                    var cid = content.Id.Replace("-", "");
                    var pt = cid.Length <= fcfg.ContentIdSlugPartLength
                        ? cid
                        : cid.Substring(0, fcfg.ContentIdSlugPartLength);
                    if (!string.IsNullOrWhiteSpace(pt)) slugParts.Add(pt);
                    continue;
                }
                var matchSimple = PartPatternSimpleRegex.Match(spp);
                if (matchSimple.Success) {
                    var fieldName = matchSimple.Groups["fieldName"].Value;
                    var slugGenFieldDef = ct.Fields.First(x => x.FieldName == fieldName);
                    var slugGenField = slugGenFieldDef.FieldFinder().GetModel(content, slugGenFieldDef)
                        .DirectCastTo<IFauxUrlSlugGenerator>();
                    var pt = slugGenField?.GenerateFauxUrlSlugPart(null, slugGenFieldDef);
                    if (!string.IsNullOrWhiteSpace(pt)) slugParts.Add(pt);
                    continue;
                }
                var matchComplex = PartPatternComplexRegex.Match(spp);
                if (matchComplex.Success) {
                    var fieldName = matchComplex.Groups["fieldName"].Value;
                    var slugGenFieldDef = ct.Fields.First(x => x.FieldName == fieldName);
                    var slugGenField = slugGenFieldDef.FieldFinder().GetModel(content, slugGenFieldDef)
                        .DirectCastTo<IFauxUrlSlugGenerator>();
                    var slugGenParam = matchComplex.Groups["slugGeneratorParam"].Value;
                    var pt = slugGenField?.GenerateFauxUrlSlugPart(slugGenParam, slugGenFieldDef);
                    if (!string.IsNullOrWhiteSpace(pt)) slugParts.Add(pt);
                    continue;
                }
                slugParts.Add(spp);
            }
            mdl.Slug = string.Join("/", slugParts);
            return mdl;
        }

        public Expression<Func<ProtoContent, bool>> Search(string keywords, string[] splittedKeywords,
            ContentFieldDefinition fieldDefinition) {
            var fcfg = fieldDefinition.Config.DirectCastTo<FauxUrlSlugFieldConfiguration>();
            var idLen = fcfg.ContentIdSlugPartLength;
            return c => splittedKeywords.Any(
                k => c.Id.Replace("-", "").Length >= idLen && c.Id.Replace("-", "").Substring(0, idLen).Contains(k) ||
                     c.Id.Replace("-", "").Contains(k)
            );
        }

        public IQueryable<ProtoContent> Sort(IQueryable<ProtoContent> currentQuery, string sortFieldName,
            bool isDescending,
            ContentFieldDefinition fieldDefinition) {
            return null;
        }

        private VueComponentDefinition[] FullPreviewValue(ContentField field, ContentFieldDefinition definition) {
            var f = field.DirectCastTo<FauxUrlSlugField>();
            var fullUrl = GetFullUrl(f);
            return new VueComponentDefinition[] {
                new VueHtmlWidget($"<a href='{fullUrl}' target='_blank'>{fullUrl}</a>")
            };
        }

        private string SummarizedValue(ContentField field, ContentFieldDefinition definition) {
            var f = field.DirectCastTo<FauxUrlSlugField>();
            return GetFullUrl(f);
        }

        private string GetFullUrl(FauxUrlSlugField f) {
            return $"{_urlProv.GetBaseWebsiteContentUrl()}{f.Slug?.Trim('/')}";
        }

        private VueComponentDefinition[] CellValue(ContentField field, ContentFieldDefinition definition) {
            return FullPreviewValue(field, definition);
        }
    }
}