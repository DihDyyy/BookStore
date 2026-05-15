using System.Globalization;
using System.Text;

namespace bookstore.Helpers
{
    public static class VietnameseHelper
    {
        /// <summary>
        /// Remove Vietnamese diacritics for search normalization
        /// </summary>
        public static string RemoveDiacritics(string text)
        {
            if (string.IsNullOrEmpty(text)) return text;

            // Vietnamese specific replacements
            var result = text.ToLowerInvariant();
            result = result.Replace("đ", "d").Replace("Đ", "D");

            var normalizedString = result.Normalize(NormalizationForm.FormD);
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

        /// <summary>
        /// Normalize search query: remove diacritics and extra spaces
        /// </summary>
        public static string NormalizeSearch(string query)
        {
            if (string.IsNullOrEmpty(query)) return query;

            var normalized = RemoveDiacritics(query.Trim());
            // Remove extra spaces
            while (normalized.Contains("  "))
                normalized = normalized.Replace("  ", " ");

            return normalized;
        }

        /// <summary>
        /// Check if text contains query (case-insensitive, diacritics-insensitive)
        /// </summary>
        public static bool ContainsVietnamese(string text, string query)
        {
            if (string.IsNullOrEmpty(text) || string.IsNullOrEmpty(query))
                return false;

            var normalizedText = RemoveDiacritics(text);
            var normalizedQuery = RemoveDiacritics(query);
            return normalizedText.Contains(normalizedQuery, StringComparison.OrdinalIgnoreCase);
        }
    }
}
