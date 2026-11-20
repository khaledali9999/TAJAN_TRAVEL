using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace TAJAN_TRAVEL.Areas.Admin.Services
{
    /// <summary>URL slug helpers (used by Tours / Destinations / Stories / Events).</summary>
    public static class Slugger
    {
        public static string Slugify(string? text)
        {
            if (string.IsNullOrWhiteSpace(text)) return string.Empty;

            var decomposed = text.Trim().ToLowerInvariant().Normalize(NormalizationForm.FormD);
            var sb = new StringBuilder(decomposed.Length);
            foreach (var ch in decomposed)
            {
                if (CharUnicodeInfo.GetUnicodeCategory(ch) == UnicodeCategory.NonSpacingMark) continue; // strip accents
                sb.Append(ch);
            }

            var slug = Regex.Replace(sb.ToString(), "[^a-z0-9]+", "-").Trim('-');
            if (slug.Length > 200) slug = slug[..200].Trim('-');
            return slug;
        }

        /// <summary>Returns "base", "base-2", "base-3"... until <paramref name="exists"/> says it is free.</summary>
        public static async Task<string> UniqueAsync(string baseSlug, Func<string, Task<bool>> exists)
        {
            if (string.IsNullOrWhiteSpace(baseSlug))
                baseSlug = "item-" + Guid.NewGuid().ToString("N")[..8];

            var candidate = baseSlug;
            var n = 2;
            while (await exists(candidate))
            {
                candidate = $"{baseSlug}-{n++}";
            }
            return candidate;
        }
    }

    /// <summary>Safe image upload / delete helpers. Files are stored under wwwroot/uploads/{folder}/.</summary>
    public static class AdminImages
    {
        private static readonly HashSet<string> AllowedExt =
            new(StringComparer.OrdinalIgnoreCase) { ".jpg", ".jpeg", ".png", ".webp", ".gif" };

        public const long MaxBytes = 5 * 1024 * 1024; // 5 MB

        /// <summary>Returns an error message, or null when the file is an acceptable image (or no file was sent).</summary>
        public static async Task<string?> CheckAsync(IFormFile? file)
        {
            if (file == null || file.Length == 0) return null;

            var name = Path.GetFileName(file.FileName);
            if (file.Length > MaxBytes) return $"'{name}' is larger than 5 MB.";
            if (!AllowedExt.Contains(Path.GetExtension(name))) return $"'{name}': only JPG, PNG, WEBP or GIF images are allowed.";

            var header = new byte[12];
            var read = 0;
            await using (var s = file.OpenReadStream())
            {
                int n;
                while (read < header.Length && (n = await s.ReadAsync(header.AsMemory(read, header.Length - read))) > 0)
                    read += n;
            }

            return read >= 12 && LooksLikeImage(header) ? null : $"'{name}' is not a valid image file.";
        }

        private static bool LooksLikeImage(byte[] h)
        {
            if (h[0] == 0xFF && h[1] == 0xD8 && h[2] == 0xFF) return true;                                  // JPEG
            if (h[0] == 0x89 && h[1] == 0x50 && h[2] == 0x4E && h[3] == 0x47) return true;                  // PNG
            if (h[0] == 'G' && h[1] == 'I' && h[2] == 'F' && h[3] == '8') return true;                      // GIF
            if (h[0] == 'R' && h[1] == 'I' && h[2] == 'F' && h[3] == 'F' &&
                h[8] == 'W' && h[9] == 'E' && h[10] == 'B' && h[11] == 'P') return true;                    // WEBP
            return false;
        }

        /// <summary>Validates and saves the file. Returns the public URL (e.g. /uploads/tours/abc.jpg).</summary>
        public static async Task<string> SaveAsync(IFormFile file, IWebHostEnvironment env, string folder)
        {
            var error = await CheckAsync(file);
            if (error != null) throw new InvalidOperationException(error);

            var dir = Path.Combine(WebRoot(env), "uploads", folder);
            Directory.CreateDirectory(dir);

            var fileName = Guid.NewGuid().ToString("N") + Path.GetExtension(file.FileName).ToLowerInvariant();
            await using (var fs = new FileStream(Path.Combine(dir, fileName), FileMode.Create))
            {
                await file.CopyToAsync(fs);
            }

            return $"/uploads/{folder}/{fileName}";
        }

        /// <summary>
        /// Picks the final image URL: a freshly uploaded file wins, otherwise the typed/kept URL.
        /// Call <see cref="CheckAsync"/> on the file first so errors are reported before anything is written.
        /// </summary>
        public static async Task<string?> ResolveAsync(string? currentUrl, IFormFile? file, IWebHostEnvironment env, string folder)
        {
            if (file != null && file.Length > 0)
                return await SaveAsync(file, env, folder);

            return string.IsNullOrWhiteSpace(currentUrl) ? null : currentUrl.Trim();
        }

        /// <summary>Accepts empty, site-relative (/images/..) or absolute http(s) URLs only.</summary>
        public static bool IsSafeUrl(string? url)
        {
            if (string.IsNullOrWhiteSpace(url)) return true;
            url = url.Trim();

            if (url.StartsWith("/")) return !url.StartsWith("//") && !url.StartsWith("/\\");

            return Uri.TryCreate(url, UriKind.Absolute, out var u) &&
                   (u.Scheme == Uri.UriSchemeHttp || u.Scheme == Uri.UriSchemeHttps);
        }

        /// <summary>Deletes a file only if it lives inside wwwroot/uploads (never touches /images etc.).</summary>
        public static void DeleteIfUploaded(string? url, IWebHostEnvironment env)
        {
            if (string.IsNullOrWhiteSpace(url) || !url.StartsWith("/uploads/", StringComparison.OrdinalIgnoreCase)) return;

            try
            {
                var uploadsRoot = Path.GetFullPath(Path.Combine(WebRoot(env), "uploads")) + Path.DirectorySeparatorChar;
                var full = Path.GetFullPath(Path.Combine(WebRoot(env), url.TrimStart('/').Replace('/', Path.DirectorySeparatorChar)));

                if (full.StartsWith(uploadsRoot, StringComparison.OrdinalIgnoreCase) && File.Exists(full))
                    File.Delete(full);
            }
            catch
            {
                // a leftover file is harmless - never fail the request because of it
            }
        }

        /// <summary>Adds a model error when the typed URL is not site-relative or http(s).</summary>
        public static void ValidateUrl(ModelStateDictionary ms, string key, string? url)
        {
            if (!IsSafeUrl(url))
                ms.AddModelError(key, "Use a link that starts with / or http(s)://");
        }

        /// <summary>Runs <see cref="CheckAsync"/> and records the error under <paramref name="key"/>.</summary>
        public static async Task ValidateFileAsync(ModelStateDictionary ms, string key, IFormFile? file)
        {
            var err = await CheckAsync(file);
            if (err != null) ms.AddModelError(key, err);
        }

        private static string WebRoot(IWebHostEnvironment env) =>
            env.WebRootPath ?? Path.Combine(env.ContentRootPath, "wwwroot");
    }
}
