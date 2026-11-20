namespace TAJAN_TRAVEL.Areas.Admin.Models
{
    public class ProfileForm
    {
        public string? Name { get; set; }
        public string? Email { get; set; }
        public string? CurrentPassword { get; set; }
    }

    public class PasswordForm
    {
        public string? CurrentPassword { get; set; }
        public string? NewPassword { get; set; }
        public string? ConfirmPassword { get; set; }
    }

    public class ProfilePageVm
    {
        public ProfileForm Profile { get; set; } = new();
        public string Role { get; set; } = "Admin";
    }
}
