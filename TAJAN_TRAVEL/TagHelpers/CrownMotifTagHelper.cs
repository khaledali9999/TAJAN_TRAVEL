using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace TAJAN_TRAVEL.TagHelpers;

public enum CrownSide { Left, Right }
public enum CrownSize { Sm, Md, Lg }

/// <summary>
/// Renders the decorative crown motif: <crown-motif side="Left" size="Lg" top="6%" />
/// Place it inside an element that has the "has-crown" CSS class.
/// </summary>
public class CrownMotifTagHelper : TagHelper
{
    // only simple CSS lengths are accepted for top/bottom (e.g. 6%, 40px, 2rem)
    private static readonly Regex SafeLength =
        new(@"^-?\d+(\.\d+)?(%|px|rem|em)$", RegexOptions.Compiled);

    public CrownSide Side { get; set; } = CrownSide.Left;
    public CrownSize Size { get; set; } = CrownSize.Md;
    public string? Top { get; set; }
    public string? Bottom { get; set; }

    public override void Process(TagHelperContext context, TagHelperOutput output)
    {
        output.TagName = "span";
        output.TagMode = TagMode.StartTagAndEndTag;
        output.Attributes.SetAttribute("class",
            $"crown-motif crown-motif--{Side.ToString().ToLowerInvariant()} crown-motif--{Size.ToString().ToLowerInvariant()}");
        output.Attributes.SetAttribute("aria-hidden", "true");

        var style = "";
        if (Top != null && SafeLength.IsMatch(Top)) style += $"top:{Top};";
        if (Bottom != null && SafeLength.IsMatch(Bottom)) style += $"bottom:{Bottom};";
        if (style.Length > 0) output.Attributes.SetAttribute("style", style);
    }
}
