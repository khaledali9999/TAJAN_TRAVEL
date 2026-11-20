using System.Globalization;
using System.Text.Json;
using Microsoft.Extensions.Localization;
using TAJAN_TRAVEL.Models.Entities;

namespace TAJAN_TRAVEL.Localization
{
    /// <summary>Static helpers: current language + translation of UI text and database content.</summary>
    public static class Lang
    {
        public const string English = "en";
        public const string French = "fr";
        public static readonly string[] Supported = { English, French };

        public static bool IsFrench => CultureInfo.CurrentUICulture.TwoLetterISOLanguageName == French;
        public static string Code => IsFrench ? French : English;

        /// <summary>Database content: French column when the site is in French and it is filled in, else English.</summary>
        public static string Pick(string? en, string? fr) =>
            IsFrench && !string.IsNullOrWhiteSpace(fr) ? fr! : (en ?? string.Empty);

        /// <summary>Like Pick but returns null when nothing is filled in (so "?? fallback" keeps working).</summary>
        public static string? PickN(string? en, string? fr)
        {
            var v = Pick(en, fr);
            return string.IsNullOrWhiteSpace(v) ? null : v;
        }

        /// <summary>UI text: key is the English text. Falls back to English when no translation exists.</summary>
        public static string T(string? key)
        {
            if (string.IsNullOrEmpty(key)) return string.Empty;
            return IsFrench && FrenchStrings.Map.TryGetValue(key, out var v) ? v : key;
        }

        public static string T(string key, params object[] args) => string.Format(CultureInfo.CurrentUICulture, T(key), args);

        public static string TourTypeLabel(TourType type) =>
            T(System.Text.RegularExpressions.Regex.Replace(type.ToString(), "(?<=[a-z])(?=[A-Z])", " "));

        /// <summary>"1 day" / "3 days" in the current language.</summary>
        public static string Days(int n) => $"{n} {(n == 1 ? T("Day") : T("Days"))}".ToLowerInvariant();

        /// <summary>Texts used by inline JavaScript (window.__t). Only needed when the site is French.</summary>
        public static string ClientStringsJson()
        {
            if (!IsFrench) return "{}";
            var keys = new[]
            {
                "See Less", "See More", "See More Stories", "MIN READ", "Link copied",
                "Item", "Items", "Loading...", "Please enter a valid email address.", "Thank you! Our team will reach out within 24 hours."
            };
            var d = keys.ToDictionary(k => k, k => T(k));
            return JsonSerializer.Serialize(d).Replace("<", "\\u003c");
        }

        /// <summary>Localised date helpers (English: "August 2, 2027" / French: "2 août 2027").</summary>
        public static string LongDate(DateTime d) =>
            d.ToString(IsFrench ? "d MMMM yyyy" : "MMMM d, yyyy", CultureInfo.CurrentUICulture);

        public static string MonthYear(DateTime d) => d.ToString("MMMM yyyy", CultureInfo.CurrentUICulture);

        public static string DayMonthShort(DateTime d) =>
            d.ToString("dd MMM yyyy", CultureInfo.CurrentUICulture).ToUpperInvariant();
    }

    /// <summary>Injected in views as <c>T</c>:  @T["Some English text"]</summary>
    public class Translator
    {
        public string this[string key] => Lang.T(key);
        public string this[string key, params object[] args] => Lang.T(key, args);
    }

    // ---- DataAnnotations (validation messages / [Display] names) --------------------------------
    public class SharedResource { }

    public class DictionaryLocalizer : IStringLocalizer
    {
        public LocalizedString this[string name]
        {
            get
            {
                var found = Lang.IsFrench && FrenchStrings.Map.ContainsKey(name);
                return new LocalizedString(name, Lang.T(name), resourceNotFound: !found);
            }
        }

        public LocalizedString this[string name, params object[] arguments]
        {
            get
            {
                var s = Lang.T(name);
                return new LocalizedString(name, arguments.Length > 0 ? string.Format(CultureInfo.CurrentUICulture, s, arguments) : s);
            }
        }

        public IEnumerable<LocalizedString> GetAllStrings(bool includeParentCultures) =>
            Enumerable.Empty<LocalizedString>();
    }

    public class DictionaryLocalizerFactory : IStringLocalizerFactory
    {
        public IStringLocalizer Create(Type resourceSource) => new DictionaryLocalizer();
        public IStringLocalizer Create(string baseName, string location) => new DictionaryLocalizer();
    }
}