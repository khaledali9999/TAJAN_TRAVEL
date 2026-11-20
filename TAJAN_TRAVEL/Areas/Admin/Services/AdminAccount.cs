using System.Security.Cryptography;
using System.Text;
using Microsoft.EntityFrameworkCore;
using TAJAN_TRAVEL.Data;
using TAJAN_TRAVEL.Models.Entities;

namespace TAJAN_TRAVEL.Areas.Admin.Services
{
    // Admin account stored in the SiteSettings table (no migration needed).
    // - Name / Email overrides: keys "Admin.Name" / "Admin.Email"
    // - Password: PBKDF2 hash in "Admin.PasswordHash" (the plain password is never stored)
    // Until the admin changes something from the Settings page, appsettings.json (Admin:*) is used.
    public static class AdminAccount
    {
        public const string Prefix = "Admin.";
        public const string NameKey = Prefix + "Name";
        public const string EmailKey = Prefix + "Email";
        public const string HashKey = Prefix + "PasswordHash";

        private const int Iterations = 210_000;

        private static async Task<Dictionary<string, string>> LoadAsync(ApplicationDbContext db) =>
            await db.SiteSettings.AsNoTracking()
                .Where(s => s.Key.StartsWith(Prefix))
                .ToDictionaryAsync(s => s.Key, s => s.Value);

        public static async Task<(string Name, string Email)> GetProfileAsync(ApplicationDbContext db, IConfiguration config)
        {
            var o = await LoadAsync(db);
            var name = o.GetValueOrDefault(NameKey) ?? config["Admin:Name"] ?? "Admin User";
            var email = o.GetValueOrDefault(EmailKey) ?? config["Admin:Email"] ?? string.Empty;
            return (name, email);
        }

        public static async Task<bool> VerifyPasswordAsync(ApplicationDbContext db, IConfiguration config, string? password)
        {
            if (string.IsNullOrEmpty(password)) return false;

            var o = await LoadAsync(db);
            if (o.TryGetValue(HashKey, out var stored)) return VerifyHash(password, stored);

            var fromConfig = config["Admin:Password"];
            return !string.IsNullOrEmpty(fromConfig) && FixedEquals(password, fromConfig);
        }

        // Adds / updates the row; the caller calls SaveChangesAsync.
        public static async Task SetAsync(ApplicationDbContext db, string key, string value)
        {
            var row = await db.SiteSettings.FirstOrDefaultAsync(s => s.Key == key);
            if (row == null)
                db.SiteSettings.Add(new SiteSetting { Key = key, Value = value, Description = "Admin account (managed from the Settings page)" });
            else
                row.Value = value;
        }

        public static string Hash(string password)
        {
            var salt = RandomNumberGenerator.GetBytes(16);
            var hash = Rfc2898DeriveBytes.Pbkdf2(password, salt, Iterations, HashAlgorithmName.SHA256, 32);
            return $"v1.{Iterations}.{Convert.ToBase64String(salt)}.{Convert.ToBase64String(hash)}";
        }

        private static bool VerifyHash(string password, string stored)
        {
            var p = stored.Split('.');
            if (p.Length != 4 || p[0] != "v1" || !int.TryParse(p[1], out var iter)) return false;
            try
            {
                var salt = Convert.FromBase64String(p[2]);
                var expected = Convert.FromBase64String(p[3]);
                var actual = Rfc2898DeriveBytes.Pbkdf2(password, salt, iter, HashAlgorithmName.SHA256, expected.Length);
                return CryptographicOperations.FixedTimeEquals(actual, expected);
            }
            catch (FormatException) { return false; }
        }

        public static bool FixedEquals(string a, string b) =>
            CryptographicOperations.FixedTimeEquals(Encoding.UTF8.GetBytes(a), Encoding.UTF8.GetBytes(b));
    }
}
