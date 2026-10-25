using System;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace SlugHub.SlugAlgorithm
{
    public class DefaultSlugAlgorithm : ISlugAlgorithm
    {
        private readonly SlugAlgorithmOptions _defaultOptions;

        public DefaultSlugAlgorithm()
        {
            _defaultOptions = new SlugAlgorithmOptions
            (
                //lowercase the whole string
                str => str.ToLower(),

                //remove diacritics
                RemoveDiacritics,

                // invalid chars           
                str => Regex.Replace(str, @"[^a-z0-9\s-]", ""),

                // convert multiple spaces into one space   
                str => Regex.Replace(str, @"\s+", " ").Trim(),

                // hyphens   
                str => Regex.Replace(str, @"\s", "-")
            );
        }

        public string Slug(string phrase)
        {
            return Slug(phrase, _defaultOptions);
        }

        public string Slug(string phrase, SlugAlgorithmOptions options)
        {
            var result = phrase;

            foreach (var manipulator in options.Manipulators)
                result = manipulator(result);

            return result;
        }

        private static string RemoveDiacritics(string text)
        {
            var normalizedString = text.Normalize(NormalizationForm.FormD);
            var stringBuilder = new StringBuilder();

            foreach (var c in normalizedString)
            {
                var unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c);
                if (unicodeCategory != UnicodeCategory.NonSpacingMark)
                {
                    stringBuilder.Append(c);
                }
            }

            return stringBuilder.ToString().Normalize(NormalizationForm.FormC);
        }
    }
}