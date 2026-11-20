namespace TAJAN_TRAVEL.Areas.Admin.Services
{
    public static class TextExtensions
    {
        /// <summary>Trims the text; returns null when it is empty / whitespace.</summary>
        public static string? Clean(this string? s) => string.IsNullOrWhiteSpace(s) ? null : s.Trim();

        /// <summary>Trims the text; returns "" when it is null (for non-nullable entity columns).</summary>
        public static string OrEmpty(this string? s) => s?.Trim() ?? string.Empty;
    }
}
