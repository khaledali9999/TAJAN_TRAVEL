using System.Globalization;

namespace TAJAN_TRAVEL.Localization
{
    /// <summary>
    /// Site-wide display currency (USD / EUR). The visitor's choice is kept in a cookie.
    /// Prices stay stored in each tour's own currency; they are converted only when displayed.
    /// The exchange rate comes from appsettings.json ("Currency:EurPerUsd").
    /// </summary>
    public static class SiteCurrency
    {
        public const string CookieName = "tajan.currency";
        public const string USD = "USD";
        public const string EUR = "EUR";
        public static readonly string[] Supported = { USD, EUR };

        private static IHttpContextAccessor? _http;
        private static decimal _eurPerUsd = 0.92m;

        public static void Configure(IHttpContextAccessor http, IConfiguration config)
        {
            _http = http;
            var rate = config.GetValue<decimal?>("Currency:EurPerUsd");
            if (rate is > 0) _eurPerUsd = rate.Value;
        }

        /// <summary>The currency the visitor chose (USD when nothing is chosen).</summary>
        public static string Current
        {
            get
            {
                var v = _http?.HttpContext?.Request.Cookies[CookieName]?.ToUpperInvariant();
                return v != null && Supported.Contains(v) ? v : USD;
            }
        }

        public static string Symbol(string code) => code.ToUpperInvariant() switch
        {
            "USD" => "$",
            "EUR" => "€",
            "GBP" => "£",
            var c => c + " "
        };

        /// <summary>How many units of <paramref name="code"/> equal 1 USD (null = unknown currency).</summary>
        public static decimal? PerUsd(string code) => code.ToUpperInvariant() switch
        {
            "USD" => 1m,
            "EUR" => _eurPerUsd,
            _ => null
        };

        /// <summary>Converts an amount stored in <paramref name="from"/> to the visitor's currency.</summary>
        public static decimal Convert(decimal amount, string? from = USD)
        {
            from = string.IsNullOrWhiteSpace(from) ? USD : from.ToUpperInvariant();
            var to = Current;
            var rf = PerUsd(from);
            var rt = PerUsd(to);
            if (from == to || rf is null || rt is null) return amount;
            return amount / rf.Value * rt.Value;
        }

        /// <summary>"$1,200" or "€1,104" depending on the visitor's currency.</summary>
        public static string Format(decimal amount, string? from = USD, string numberFormat = "N0")
        {
            from = string.IsNullOrWhiteSpace(from) ? USD : from.ToUpperInvariant();
            // a currency we cannot convert (e.g. GBP) is shown as stored
            var shown = PerUsd(from) is null ? from : Current;
            var value = PerUsd(from) is null ? amount : Convert(amount, from);
            return Symbol(shown) + value.ToString(numberFormat, CultureInfo.CurrentUICulture);
        }

        /// <summary>Inline JS object: { code, symbol, perUsd } used by the price slider.</summary>
        public static string ClientJson()
        {
            var c = Current;
            return $"{{\"code\":\"{c}\",\"symbol\":\"{Symbol(c)}\",\"perUsd\":{PerUsd(c)!.Value.ToString(CultureInfo.InvariantCulture)}}}";
        }
    }
}
