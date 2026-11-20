using System.ComponentModel.DataAnnotations;

namespace TAJAN_TRAVEL.Models.Entities
{
    public enum InquiryStatus
    {
        New,
        Contacted,
        InProgress,
        Confirmed,
        Closed
    }

    public enum InquiryType
    {
        TourInquiry,
        CustomItinerary,
        GeneralContact
    }

    // Named differently from the ViewModel enums (DateFlexibility / ContactPreference)
    // so both namespaces can be imported in views without ambiguity.
    public enum TripDateFlexibility
    {
        Exact,
        Flexible,
        NotDecided
    }

    [Flags]
    public enum InquiryContactMethod
    {
        Email = 1,
        WhatsApp = 2
    }

    public class Inquiry
    {
        public int Id { get; set; }

        [Required, MaxLength(120)]
        public string FullName { get; set; } = string.Empty;

        [Required, EmailAddress, MaxLength(150)]
        public string Email { get; set; } = string.Empty;

        [Required, MaxLength(30)]
        public string PhoneNumber { get; set; } = string.Empty;

        // Trip dates (PreferredDate = start date, kept for backward compatibility)
        public DateTime? PreferredDate { get; set; }
        public DateTime? PreferredEndDate { get; set; }
        public TripDateFlexibility DateFlexibility { get; set; } = TripDateFlexibility.Exact;

        // Travelers (GroupSize = Adults + Children, kept for backward compatibility)
        [Range(1, int.MaxValue)]
        public int GroupSize { get; set; } = 1;

        [Range(1, 20)]
        public int Adults { get; set; } = 1;

        // no upper limit on the number of children
        [Range(0, int.MaxValue)]
        public int Children { get; set; }

        [Range(0, 20)]
        public int Infants { get; set; }

        // Ages of the children, in the order entered, e.g. "5, 9" (no length limit: any number of children)
        public string? ChildrenAges { get; set; }

        // Where the traveler comes from + the language they want the tour in
        [MaxLength(80)]
        public string? Country { get; set; }

        [MaxLength(40)]
        public string? Language { get; set; }

        // Comma-separated selection, e.g. "Nile Journeys, Desert & Oasis"
        [MaxLength(200)]
        public string? PreferredCategory { get; set; }

        public InquiryContactMethod PreferredContact { get; set; } = InquiryContactMethod.Email;

        public string? AdditionalNotes { get; set; }

        public InquiryType Type { get; set; } = InquiryType.TourInquiry;

        public int? TourId { get; set; }
        public Tour? Tour { get; set; }

        public InquiryStatus Status { get; set; } = InquiryStatus.New;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}