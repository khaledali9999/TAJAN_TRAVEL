namespace TAJAN_TRAVEL.Models.ViewModels
{
    /// <summary>A bullet; <see cref="Lead"/> (optional) is shown in bold before the text.</summary>
    public record LegalItem(string Text, string? Lead = null);

    public record LegalSection(string Heading, string? Intro, LegalItem[] Items);

    public class LegalPageViewModel
    {
        public string Title { get; set; } = string.Empty;
        public string Updated { get; set; } = string.Empty;
        public string? Intro { get; set; }
        public List<LegalSection> Sections { get; set; } = new();
    }
}
