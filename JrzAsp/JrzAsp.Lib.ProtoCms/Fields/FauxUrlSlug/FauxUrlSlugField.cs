using JrzAsp.Lib.ProtoCms.Content.Models;

namespace JrzAsp.Lib.ProtoCms.Fields.FauxUrlSlug {
    /// <summary>
    ///     Faux url slug field is used to make a custom slug field for content so that it can be accessed by URL.
    ///     It's faux because url matching to content is not by full url slug, but by content id, because the generated
    ///     url slug will have a part of the content id, so that content can be searched by using it.
    /// </summary>
    public class FauxUrlSlugField : ContentField {

        public string Slug { get; set; }

        public override ContentFieldSpec __FieldSpec => new ContentFieldSpec(
            typeof(FauxUrlSlugFieldFinder),
            typeof(FauxUrlSlugFieldModifier),
            typeof(FauxUrlSlugFieldConfiguration),
            1);
    }
}