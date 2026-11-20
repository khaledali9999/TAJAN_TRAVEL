namespace TAJAN_TRAVEL.Models.ViewModels
{
    public record CountryDialCode(string Name, string Dial);

    public static class CountryDialCodes
    {
        // Shown first in the dropdown (main markets), the rest follow alphabetically.
        private static readonly CountryDialCode[] Priority =
        {
            new("Egypt", "+20"),
            new("United States / Canada", "+1"),
            new("France", "+33"),
            new("United Kingdom", "+44"),
        };

        // Every dial code is unique so the <select> can bind by value without ambiguity.
        private static readonly CountryDialCode[] Others =
        {
            new("Algeria", "+213"), new("Argentina", "+54"), new("Australia", "+61"),
            new("Austria", "+43"), new("Bahrain", "+973"), new("Bangladesh", "+880"),
            new("Belgium", "+32"), new("Brazil", "+55"), new("Bulgaria", "+359"),
            new("Chile", "+56"), new("China", "+86"), new("Colombia", "+57"),
            new("Croatia", "+385"), new("Cyprus", "+357"), new("Czech Republic", "+420"),
            new("Denmark", "+45"), new("Estonia", "+372"), new("Ethiopia", "+251"),
            new("Finland", "+358"), new("Germany", "+49"), new("Ghana", "+233"),
            new("Greece", "+30"), new("Hong Kong", "+852"), new("Hungary", "+36"),
            new("Iceland", "+354"), new("India", "+91"), new("Indonesia", "+62"),
            new("Iran", "+98"), new("Iraq", "+964"), new("Ireland", "+353"),
            new("Israel", "+972"), new("Italy", "+39"), new("Japan", "+81"),
            new("Jordan", "+962"), new("Kenya", "+254"), new("Kuwait", "+965"),
            new("Latvia", "+371"), new("Lebanon", "+961"), new("Libya", "+218"),
            new("Lithuania", "+370"), new("Luxembourg", "+352"), new("Malaysia", "+60"),
            new("Malta", "+356"), new("Mexico", "+52"), new("Morocco", "+212"),
            new("Netherlands", "+31"), new("New Zealand", "+64"), new("Nigeria", "+234"),
            new("Norway", "+47"), new("Oman", "+968"), new("Pakistan", "+92"),
            new("Palestine", "+970"), new("Peru", "+51"), new("Philippines", "+63"),
            new("Poland", "+48"), new("Portugal", "+351"), new("Qatar", "+974"),
            new("Romania", "+40"), new("Russia / Kazakhstan", "+7"), new("Saudi Arabia", "+966"),
            new("Senegal", "+221"), new("Serbia", "+381"), new("Singapore", "+65"),
            new("Slovakia", "+421"), new("Slovenia", "+386"), new("South Africa", "+27"),
            new("South Korea", "+82"), new("Spain", "+34"), new("Sri Lanka", "+94"),
            new("Sudan", "+249"), new("Sweden", "+46"), new("Switzerland", "+41"),
            new("Syria", "+963"), new("Taiwan", "+886"), new("Tanzania", "+255"),
            new("Thailand", "+66"), new("Tunisia", "+216"), new("Turkey", "+90"),
            new("Uganda", "+256"), new("Ukraine", "+380"), new("United Arab Emirates", "+971"),
            new("Venezuela", "+58"), new("Vietnam", "+84"), new("Yemen", "+967"),
        };

        public static IReadOnlyList<CountryDialCode> All { get; } =
            Priority.Concat(Others.OrderBy(c => c.Name, StringComparer.Ordinal)).ToList();
    }
}
