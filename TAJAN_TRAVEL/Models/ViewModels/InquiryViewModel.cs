using System.ComponentModel.DataAnnotations;
using TAJAN_TRAVEL.Models.Entities;

namespace TAJAN_TRAVEL.Models.ViewModels
{
    public enum DateFlexibility
    {
        Exact,
        Flexible,
        NotDecided
    }

    public enum ContactPreference
    {
        Email,
        WhatsApp
    }

    public class InquiryViewModel
    {
        // Context (set when the request comes from a specific tour page)
        public int? TourId { get; set; }
        public string? TourTitle { get; set; }

        [Required(ErrorMessage = "Please enter your full name.")]
        [Display(Name = "Full Name")]
        public string FullName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Please enter your email.")]
        [EmailAddress(ErrorMessage = "Please enter a valid email.")]
        [Display(Name = "Email Address")]
        public string Email { get; set; } = string.Empty;

        [Display(Name = "Country Code")]
        public string CountryCode { get; set; } = "+20";

        [Required(ErrorMessage = "Please enter your mobile number.")]
        [Phone(ErrorMessage = "Please enter a valid mobile number.")]
        [Display(Name = "Mobile Number")]
        public string PhoneNumber { get; set; } = string.Empty;

        [Display(Name = "From Date")]
        [DataType(DataType.Date)]
        public DateTime? FromDate { get; set; }

        [Display(Name = "To Date")]
        [DataType(DataType.Date)]
        public DateTime? ToDate { get; set; }

        public DateFlexibility Flexibility { get; set; } = DateFlexibility.Exact;

        [Range(1, 20)]
        [Display(Name = "Adults (12+ Yrs)")]
        public int Adults { get; set; } = 1;

        [Range(0, int.MaxValue, ErrorMessage = "Please enter a valid number of children.")]
        [Display(Name = "Children (2-12 Yrs)")]
        public int Children { get; set; } = 0;

        // One age per child (2-12), posted as repeated "ChildrenAges" fields
        public List<int> ChildrenAges { get; set; } = new();

        [Range(0, 20)]
        [Display(Name = "Infants (0-2 Yrs)")]
        public int Infants { get; set; } = 0;

        // Multi-select: any of "Nile Journeys", "Desert & Oasis", "Coastal Escapes", "City & Culture", "Adrenaline"
        [Display(Name = "Your Preference")]
        public List<string> PreferredCategories { get; set; } = new();

        [Display(Name = "Tell Us More")]
        public string? AdditionalNotes { get; set; }

        [Required(ErrorMessage = "Please select your country.")]
        [Display(Name = "Country")]
        public string? Country { get; set; }

        [Required(ErrorMessage = "Please select a language.")]
        [Display(Name = "Language")]
        public string? Language { get; set; } = "English";

        public List<string> PreferredContacts { get; set; } = new() { "Email" };

        // Populated on GET for the category chip list
        public List<string> Categories { get; set; } = new()
        {
            "Nile Journeys", "Desert & Oasis", "Coastal Escapes", "City & Culture", "Adrenaline"
        };
    }
}