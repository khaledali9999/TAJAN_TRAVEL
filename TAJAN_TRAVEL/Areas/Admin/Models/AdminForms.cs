using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;
using TAJAN_TRAVEL.Models.Entities;

namespace TAJAN_TRAVEL.Areas.Admin.Models
{
    // NOTE: every optional text property is declared "string?" - with nullable reference types on,
    // MVC treats a non-nullable "string" as [Required] automatically.

    public static class FormRegex
    {
        public const string Slug = "^[a-z0-9]+(?:-[a-z0-9]+)*$";
        public const string SlugMsg = "Use lowercase letters, numbers and hyphens only (e.g. pyramids-of-giza).";
    }

    // ------------------------------------------------------------------ TOURS
    public class ItineraryRowVm
    {
        public int DayNumber { get; set; }
        public string? TitleEn { get; set; }
        public string? TitleFr { get; set; }

        public string? DescriptionEn { get; set; }
        public string? DescriptionFr { get; set; }

        public bool IsBlank =>
            string.IsNullOrWhiteSpace(TitleEn) && string.IsNullOrWhiteSpace(TitleFr) &&
            string.IsNullOrWhiteSpace(DescriptionEn) && string.IsNullOrWhiteSpace(DescriptionFr);
    }

    public class PriceRowVm
    {
        public int Persons { get; set; }
        public decimal? PricePerPerson { get; set; }   // empty = no price for this group size
    }

    public class GalleryRowVm
    {
        public int Id { get; set; }
        public string? ImageUrl { get; set; }      // display only - the saved URL is always read from the database
        public string? AltTextEn { get; set; }
        public string? AltTextFr { get; set; }
        public bool Remove { get; set; }
    }

    public class TourFormVm
    {
        public int Id { get; set; }

        [Required, StringLength(200)] public string TitleEn { get; set; } = string.Empty;
        [Required, StringLength(200)] public string TitleFr { get; set; } = string.Empty;

        [StringLength(250), RegularExpression(FormRegex.Slug, ErrorMessage = FormRegex.SlugMsg)]
        public string? Slug { get; set; }

        [Required] public string SummaryEn { get; set; } = string.Empty;
        [Required] public string SummaryFr { get; set; } = string.Empty;
        public string? DescriptionEn { get; set; }
        public string? DescriptionFr { get; set; }

        // Tour page content (optional)
        [StringLength(600)] public string? QuoteEn { get; set; }
        [StringLength(600)] public string? QuoteFr { get; set; }
        public string? HighlightsEn { get; set; }
        public string? HighlightsFr { get; set; }
        public string? IncludedEn { get; set; }
        public string? IncludedFr { get; set; }
        public string? ExcludedEn { get; set; }
        public string? ExcludedFr { get; set; }
        public string? ImportantInfoEn { get; set; }
        public string? ImportantInfoFr { get; set; }
        [StringLength(120)] public string? AvailabilityEn { get; set; }
        [StringLength(120)] public string? AvailabilityFr { get; set; }
        [StringLength(300)] public string? PaymentNoteEn { get; set; }
        [StringLength(300)] public string? PaymentNoteFr { get; set; }
        [Range(1, 24, ErrorMessage = "Hours must be between 1 and 24.")]
        public int? DurationHours { get; set; }

        public TourType Type { get; set; }

        [Range(1, 60, ErrorMessage = "Duration must be between 1 and 60 days.")]
        public int DurationDays { get; set; } = 1;

        [Range(0, 1000000, ErrorMessage = "Enter a valid price.")]
        public decimal PriceFrom { get; set; }

        [Required, StringLength(10)] public string Currency { get; set; } = "USD";

        [StringLength(300)] public string? CoverImageUrl { get; set; }
        public IFormFile? CoverFile { get; set; }

        public bool IsPrivate { get; set; } = true;
        public bool IsFeatured { get; set; }
        public bool IsActive { get; set; } = true;

        [Range(1, int.MaxValue, ErrorMessage = "Please choose a destination.")]
        public int DestinationId { get; set; }
        public int? ActivityTypeId { get; set; }

        public List<ItineraryRowVm> Itinerary { get; set; } = new();
        public List<PriceRowVm> Prices { get; set; } = new();
        public List<GalleryRowVm> Gallery { get; set; } = new();
        public List<IFormFile> NewGalleryFiles { get; set; } = new();

        [ValidateNever] public List<SelectListItem> DestinationOptions { get; set; } = new();
        [ValidateNever] public List<SelectListItem> ActivityTypeOptions { get; set; } = new();
        [ValidateNever] public List<SelectListItem> TypeOptions { get; set; } = new();
    }

    public class ToursIndexVm
    {
        public List<Tour> Items { get; set; } = new();
        public string? Q { get; set; }
        public int? DestinationId { get; set; }
        public string? Status { get; set; }   // active | inactive | featured
        public List<SelectListItem> Destinations { get; set; } = new();
    }

    // ----------------------------------------------------------- DESTINATIONS
    public class DestinationFormVm
    {
        public int Id { get; set; }

        [Required, StringLength(150)] public string NameEn { get; set; } = string.Empty;
        [Required, StringLength(150)] public string NameFr { get; set; } = string.Empty;

        [StringLength(200), RegularExpression(FormRegex.Slug, ErrorMessage = FormRegex.SlugMsg)]
        public string? Slug { get; set; }

        [Required] public string DescriptionEn { get; set; } = string.Empty;
        [Required] public string DescriptionFr { get; set; } = string.Empty;
        public string? LongDescriptionEn { get; set; }
        public string? LongDescriptionFr { get; set; }

        [StringLength(300)] public string? CoverImageUrl { get; set; }
        public IFormFile? CoverFile { get; set; }

        [StringLength(300)] public string? HeroImageUrl { get; set; }
        public IFormFile? HeroFile { get; set; }

        [StringLength(100)] public string? BestTime { get; set; }
        [StringLength(100)] public string? BestTimeFr { get; set; }
        [StringLength(150)] public string? KnownFor { get; set; }
        [StringLength(150)] public string? KnownForFr { get; set; }
        [Required, StringLength(50)] public string Category { get; set; } = string.Empty;
        [StringLength(50)] public string? CategoryFr { get; set; }

        public int DisplayOrder { get; set; }
        public bool IsFeatured { get; set; }
        public bool IsActive { get; set; } = true;

        [ValidateNever] public List<string> CategorySuggestions { get; set; } = new();
    }

    // ---------------------------------------------------------------- STORIES
    public class StoryFormVm
    {
        public int Id { get; set; }

        [Required, StringLength(200)] public string TitleEn { get; set; } = string.Empty;
        [Required, StringLength(200)] public string TitleFr { get; set; } = string.Empty;

        [StringLength(250), RegularExpression(FormRegex.Slug, ErrorMessage = FormRegex.SlugMsg)]
        public string? Slug { get; set; }

        [Required] public string SummaryEn { get; set; } = string.Empty;
        [Required] public string SummaryFr { get; set; } = string.Empty;
        [Required] public string ContentEn { get; set; } = string.Empty;
        [Required] public string ContentFr { get; set; } = string.Empty;

        [StringLength(300)] public string? CoverImageUrl { get; set; }
        public IFormFile? CoverFile { get; set; }
        [StringLength(200)] public string? CoverImageCaption { get; set; }
        [StringLength(200)] public string? CoverImageCaptionFr { get; set; }

        [Range(1, 120, ErrorMessage = "Read time must be between 1 and 120 minutes.")]
        public int ReadTimeMinutes { get; set; } = 5;

        public int? BlogCategoryId { get; set; }
        [StringLength(100)] public string? Author { get; set; }

        public bool IsFeatured { get; set; }
        public bool IsPublished { get; set; }
        public DateTime? PublishedAt { get; set; }

        [ValidateNever] public List<SelectListItem> CategoryOptions { get; set; } = new();
    }

    public class StoriesIndexVm
    {
        public List<BlogPost> Items { get; set; } = new();
        public string? Q { get; set; }
        public string? Status { get; set; }   // published | draft
        public int Page { get; set; } = 1;
        public int TotalPages { get; set; } = 1;
        public int Total { get; set; }
        public int PublishedCount { get; set; }
        public int DraftCount { get; set; }
    }

    // ----------------------------------------------------------------- EVENTS
    public class EventFormVm
    {
        public int Id { get; set; }

        [Required, StringLength(200)] public string TitleEn { get; set; } = string.Empty;
        [Required, StringLength(200)] public string TitleFr { get; set; } = string.Empty;

        [StringLength(250), RegularExpression(FormRegex.Slug, ErrorMessage = FormRegex.SlugMsg)]
        public string? Slug { get; set; }

        [Required] public string SummaryEn { get; set; } = string.Empty;
        [Required] public string SummaryFr { get; set; } = string.Empty;
        [Required] public string DescriptionEn { get; set; } = string.Empty;
        [Required] public string DescriptionFr { get; set; } = string.Empty;

        [StringLength(300)] public string? CoverImageUrl { get; set; }
        public IFormFile? CoverFile { get; set; }

        public int? EventCategoryId { get; set; }
        [StringLength(200)] public string? Location { get; set; }
        [StringLength(200)] public string? LocationFr { get; set; }

        public DateTime? EventDate { get; set; }
        [StringLength(100)] public string? DateLabel { get; set; }

        [Range(1, 120)] public int ReadTimeMinutes { get; set; } = 5;
        public int DisplayOrder { get; set; }
        public bool IsFeatured { get; set; }
        public bool IsActive { get; set; } = true;

        [ValidateNever] public List<SelectListItem> CategoryOptions { get; set; } = new();
    }

    // ------------------------------------------------------------------- FAQS
    public class FaqFormVm
    {
        public int Id { get; set; }

        [Required, StringLength(250)] public string QuestionEn { get; set; } = string.Empty;
        [Required, StringLength(250)] public string QuestionFr { get; set; } = string.Empty;
        [Required] public string AnswerEn { get; set; } = string.Empty;
        [Required] public string AnswerFr { get; set; } = string.Empty;

        [StringLength(80)] public string? Category { get; set; }
        [StringLength(80)] public string? CategoryFr { get; set; }
        public int DisplayOrder { get; set; }
        public bool IsActive { get; set; } = true;

        [ValidateNever] public List<string> CategorySuggestions { get; set; } = new();
    }

    // ---------------------------------------------------------------- REVIEWS
    public class ReviewFormVm
    {
        public int Id { get; set; }

        [Required, StringLength(100)] public string ClientName { get; set; } = string.Empty;
        [StringLength(100)] public string? ClientCountry { get; set; }
        [StringLength(100)] public string? ClientCountryFr { get; set; }

        [StringLength(300)] public string? ClientPhotoUrl { get; set; }
        public IFormFile? PhotoFile { get; set; }

        [Required] public string QuoteEn { get; set; } = string.Empty;
        [Required] public string QuoteFr { get; set; } = string.Empty;

        [Range(1, 5, ErrorMessage = "Rating must be between 1 and 5.")]
        public int Rating { get; set; } = 5;

        public int? TourId { get; set; }
        public int DisplayOrder { get; set; }
        public bool IsFeatured { get; set; }
        public bool IsApproved { get; set; } = true;

        [ValidateNever] public List<SelectListItem> TourOptions { get; set; } = new();
    }

    // --------------------------------------------------- CATEGORIES / LOOKUPS
    public class LookupRowVm
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "English name is required."), StringLength(100)]
        public string? NameEn { get; set; }

        [Required(ErrorMessage = "French name is required."), StringLength(100)]
        public string? NameFr { get; set; }

        [StringLength(150)] public string? Slug { get; set; }
        public int DisplayOrder { get; set; }
        public bool ShowInFilter { get; set; } = true;   // blog categories only
        [StringLength(50)] public string? IconClass { get; set; }   // activity types only
        public int UsageCount { get; set; }
    }

    public class CategoriesIndexVm
    {
        public List<LookupRowVm> Blog { get; set; } = new();
        public List<LookupRowVm> Events { get; set; } = new();
        public List<LookupRowVm> Activities { get; set; } = new();
    }

    // --------------------------------------------------------------- SETTINGS
    public class SettingRowVm
    {
        public int Id { get; set; }

        [Required, StringLength(100), RegularExpression(@"^[A-Za-z0-9_.\-]+$", ErrorMessage = "Key: letters, numbers, . _ - only.")]
        public string? Key { get; set; }

        [Required(ErrorMessage = "Value is required.")]
        public string? Value { get; set; }

        [StringLength(200)] public string? Description { get; set; }
    }

    // -------------------------------------------------------------- DASHBOARD
    public record ContentStatVm(string Label, string Value, string Sub, string Icon, string Controller, string AddAction = "Create");
    public record TrendPointVm(string Label, int Requests, decimal Revenue);
    public record TopTourVm(int Id, string Title, int Requests);
    public record StatusSliceVm(string Label, int Count, string Bar);

    // -------------------------------------------------------------- PARTIALS
    public record RowActionsVm(string Controller, int Id, string Confirm);
    public record LookupRender(string Kind, LookupRowVm Row);
    public record ItineraryRowRender(string Idx, ItineraryRowVm Row);
}