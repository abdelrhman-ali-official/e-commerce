using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace Services.Helpers
{
    public static class SlugHelper
    {
        /// <summary>
        /// Generates a URL-friendly slug from a string
        /// </summary>
        public static string GenerateSlug(string text, int maxLength = 100)
        {
            if (string.IsNullOrWhiteSpace(text))
                return string.Empty;

            // Convert to lowercase
            text = text.ToLowerInvariant();

            // Remove diacritics (accents)
            text = RemoveDiacritics(text);

            // Replace spaces and special characters with hyphens
            text = Regex.Replace(text, @"[^a-z0-9\s-]", "");
            text = Regex.Replace(text, @"\s+", " ").Trim();
            text = Regex.Replace(text, @"\s", "-");

            // Remove duplicate hyphens
            text = Regex.Replace(text, @"-+", "-");

            // Trim to max length
            if (text.Length > maxLength)
                text = text.Substring(0, maxLength).Trim('-');

            return text;
        }

        /// <summary>
        /// Generates a unique slug by appending product details
        /// </summary>
        public static string GenerateProductSlug(string name, string color, string size, int? productId = null)
        {
            var slug = GenerateSlug($"{name} {color} {size}");
            
            // Append ID if provided to ensure uniqueness
            if (productId.HasValue)
                slug = $"{slug}-{productId}";

            return slug;
        }

        /// <summary>
        /// Makes a slug unique by checking if it exists and appending a number
        /// </summary>
        public static string EnsureUnique(string baseSlug, IEnumerable<string> existingSlugs)
        {
            var slug = baseSlug;
            var counter = 1;

            while (existingSlugs.Contains(slug))
            {
                slug = $"{baseSlug}-{counter}";
                counter++;
            }

            return slug;
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
