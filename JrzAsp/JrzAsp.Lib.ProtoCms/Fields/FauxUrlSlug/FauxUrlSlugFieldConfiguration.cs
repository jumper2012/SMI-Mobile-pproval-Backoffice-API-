using System;
using System.Linq;
using JrzAsp.Lib.ProtoCms.Content.Models;

namespace JrzAsp.Lib.ProtoCms.Fields.FauxUrlSlug {
    public class FauxUrlSlugFieldConfiguration : ContentFieldConfiguration {

        private int _contentIdSlugLength;

        private string _slugPattern;

        /// <summary>
        ///     The url slug pattern.
        ///     The pattern is a string that looks like '<b>something/[###]/something-else/others</b>'. As seen here, it has 4
        ///     parts.
        ///     The part '<b>[###]</b>' is the content id slug part that must exists. So, the minimum pattern is '[###]'.
        ///     Then, if any part looks like '<b>[aaa]</b>' or '<b>[aaa:bbb]</b>' then it'll be parsed
        ///     with the following rules: 'aaa' means the content field name that will be used to get url slug part,
        ///     and 'bbb' is a string param that will be given to that content field object to generate the url slug part.
        ///     Therefore, to make this possible, the content field type must implement <see cref="IFauxUrlSlugGenerator" />.
        ///     Furthermore, don't mix simple string with special part (the one with square braces), i.e. part that looks like
        ///     '[aaa]-something' is not supported yet.
        /// </summary>
        public string SlugPattern {
            get => _slugPattern;
            set {
                _slugPattern = value;
                if (string.IsNullOrWhiteSpace(_slugPattern)) {
                    throw new InvalidOperationException(
                        $"ProtoCMS: faux url slug pattern must not be null or empty or just whitespaces.");
                }
                var spps = _slugPattern.Trim('/').Split('/');
                if (!spps.Contains(FauxUrlSlugFieldFinder.ID_PATTERN_PART)) {
                    throw new InvalidOperationException(
                        $"ProtoCMS: faux url slug pattern must have id pattern part ('{FauxUrlSlugFieldFinder.ID_PATTERN_PART}').");
                }
            }
        }

        /// <summary>
        ///     The length of content id string to get as slug part. Defaults to 8, as by default <see cref="ProtoContent" />
        ///     use GUID as id.
        /// </summary>
        public int ContentIdSlugPartLength {
            get {
                if (_contentIdSlugLength > 0) return _contentIdSlugLength;
                _contentIdSlugLength = 8;
                return _contentIdSlugLength;
            }
            set => _contentIdSlugLength = value;
        }
    }
}