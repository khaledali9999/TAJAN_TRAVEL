// حطه في نفس الـ namespace بتاع CountryDialCodes (عشان الـ Views تشوفه من غير @using إضافي)
namespace TAJAN_TRAVEL.Models
{
    /// <summary>
    /// اللغات اللي العميل يقدر يختارها (القيمة بالإنجليزي عشان الفريق يقراها بسهولة، والعنوان باللغة نفسها).
    /// عشان تضيف لغة جديدة ضيفها هنا بس، وهتظهر في كل الصفحات.
    /// </summary>
    public static class InquiryLanguages
    {
        public static readonly (string Value, string Label)[] All =
        {
            ("English",    "English"),
            ("Arabic",     "العربية (Arabic)"),
            ("French",     "Français (French)"),
            ("German",     "Deutsch (German)"),
            ("Spanish",    "Español (Spanish)"),
            ("Italian",    "Italiano (Italian)"),
            ("Russian",    "Русский (Russian)"),
            ("Portuguese", "Português (Portuguese)"),
            ("Chinese",    "中文 (Chinese)"),
            ("Japanese",   "日本語 (Japanese)"),
            ("Other",      "Other"),
        };
    }
}
