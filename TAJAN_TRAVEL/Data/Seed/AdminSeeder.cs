using TAJAN_TRAVEL.Models.Entities;

namespace TAJAN_TRAVEL.Data.Seed
{
    public static class AdminSeeder
    {
        // Basic site settings seed. Admin auth (Identity user/roles) will be wired
        // in the Admin Dashboard phase once the auth approach is decided.
        public static List<SiteSetting> GetSiteSettings()
        {
            return new List<SiteSetting>
            {
                new SiteSetting { Key = "SiteName", Value = "TAJAN TRAVEL" },
                new SiteSetting { Key = "ContactEmail", Value = "hello@tajantravel.com" },
                new SiteSetting { Key = "ContactPhone", Value = "+20 2 1234 5678" },
                new SiteSetting { Key = "WhatsAppNumber", Value = "+201234567890" },
                new SiteSetting { Key = "DefaultCurrency", Value = "USD" },
                new SiteSetting { Key = "PrimaryColor", Value = "#4B1E23" },
                new SiteSetting { Key = "SecondaryColor", Value = "#D8C3A5" },
                new SiteSetting { Key = "AccentColor", Value = "#C8A44D" }
            };
        }
    }
}
