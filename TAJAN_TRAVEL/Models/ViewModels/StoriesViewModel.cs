using System.Text.Json;
using TAJAN_TRAVEL.Models.Entities;

namespace TAJAN_TRAVEL.Models.ViewModels
{
    public class StoriesViewModel
    {
        // Static page parts (not re-sent when filtering / paging)
        public List<Testimonial> Testimonials { get; set; } = new();
        public List<BlogPost> Moments { get; set; } = new();
        public BlogPost? DeepLinkStory { get; set; }

        // Filterable part
        public BlogPost? Featured { get; set; }
        public List<BlogPost> Posts { get; set; } = new();
        public List<BlogCategory> Categories { get; set; } = new();

        public string? Category { get; set; }
        public bool IsFiltered => !string.IsNullOrWhiteSpace(Category);

        public int CurrentPage { get; set; } = 1;
        public int TotalPages { get; set; } = 1;
    }

    /// <summary>
    /// Serialises stories for the popup. System.Text.Json escapes &lt; &gt; &amp; by default,
    /// so the result is safe to drop inside a &lt;script type="application/json"&gt; tag.
    /// </summary>
    public static class StoryJson
    {
        public static string Serialize(IEnumerable<BlogPost?> posts)
        {
            var data = posts
                .Where(p => p != null)
                .GroupBy(p => p!.Slug)
                .Select(g => g.First()!)
                .Select(p => new
                {
                    slug = p.Slug,
                    title = p.Title,
                    category = p.Category?.Name,
                    readTime = p.ReadTimeMinutes,
                    author = p.Author,
                    published = p.PublishedAt.HasValue ? TAJAN_TRAVEL.Localization.Lang.MonthYear(p.PublishedAt.Value) : null,
                    image = p.CoverImageUrl,
                    caption = TAJAN_TRAVEL.Localization.Lang.PickN(p.CoverImageCaption, p.CoverImageCaptionFr),
                    content = p.Content
                });
            return JsonSerializer.Serialize(data);
        }
    }
}
