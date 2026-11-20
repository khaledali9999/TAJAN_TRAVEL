using System.ComponentModel.DataAnnotations;

namespace TAJAN_TRAVEL.Models.ViewModels
{
    /// <summary>"Let's Work Together" popup on the Contact page (B2B / partnership requests).</summary>
    public class PartnerRequestViewModel
    {
        [Required(ErrorMessage = "Please enter your full name.")]
        [StringLength(120)]
        public string FullName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Please enter your business email.")]
        [EmailAddress(ErrorMessage = "Please enter a valid email.")]
        [StringLength(150)]
        public string BusinessEmail { get; set; } = string.Empty;

        [Required(ErrorMessage = "Please enter your phone or WhatsApp number.")]
        [Phone(ErrorMessage = "Please enter a valid phone number.")]
        [StringLength(30)]
        public string Phone { get; set; } = string.Empty;

        [StringLength(120)]
        public string? Company { get; set; }

        [Required(ErrorMessage = "Please select your country.")]
        [StringLength(80)]
        public string Country { get; set; } = string.Empty;

        [Required(ErrorMessage = "Please tell us how you'd like to collaborate.")]
        public string Message { get; set; } = string.Empty;
    }
}
