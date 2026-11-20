using System.Net.Mail;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using TAJAN_TRAVEL.Areas.Admin.Models;
using TAJAN_TRAVEL.Areas.Admin.Services;
using TAJAN_TRAVEL.Data;

namespace TAJAN_TRAVEL.Areas.Admin.Controllers
{
    // Admin "Settings" page: view / edit own details + change password.
    public class ProfileController : AdminBaseController
    {
        private readonly IConfiguration _config;

        public ProfileController(ApplicationDbContext db, IConfiguration config) : base(db)
        {
            _config = config;
        }

        public async Task<IActionResult> Index() => View(await BuildAsync());

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> SaveProfile(ProfileForm form)
        {
            var name = (form.Name ?? string.Empty).Trim();
            var email = (form.Email ?? string.Empty).Trim();

            if (name.Length == 0) ModelState.AddModelError("profile", "Name is required.");
            else if (name.Length > 100) ModelState.AddModelError("profile", "Name is too long (100 characters max).");

            if (!MailAddress.TryCreate(email, out var addr) || addr.Address != email || email.Length > 150)
                ModelState.AddModelError("profile", "Please enter a valid email address.");

            if (!await AdminAccount.VerifyPasswordAsync(Db, _config, form.CurrentPassword))
            {
                await Task.Delay(400);
                ModelState.AddModelError("profile", "Current password is incorrect.");
            }

            if (!ModelState.IsValid)
            {
                var vm = await BuildAsync();
                vm.Profile.Name = name;
                vm.Profile.Email = email;
                return View("Index", vm);
            }

            await AdminAccount.SetAsync(Db, AdminAccount.NameKey, name);
            await AdminAccount.SetAsync(Db, AdminAccount.EmailKey, email);
            await Db.SaveChangesAsync();

            await RefreshSignInAsync(name, email);
            Flash("Your details were updated.");
            return RedirectToAction(nameof(Index));
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(PasswordForm form)
        {
            if (!await AdminAccount.VerifyPasswordAsync(Db, _config, form.CurrentPassword))
            {
                await Task.Delay(400);
                ModelState.AddModelError("password", "Current password is incorrect.");
            }

            var next = form.NewPassword ?? string.Empty;
            if (next.Length < 8) ModelState.AddModelError("password", "New password must be at least 8 characters.");
            else if (next.Length > 128) ModelState.AddModelError("password", "New password is too long (128 characters max).");
            else if (next != form.ConfirmPassword) ModelState.AddModelError("password", "New password and confirmation do not match.");
            else if (next == form.CurrentPassword) ModelState.AddModelError("password", "New password must be different from the current one.");

            if (!ModelState.IsValid) return View("Index", await BuildAsync());

            await AdminAccount.SetAsync(Db, AdminAccount.HashKey, AdminAccount.Hash(next));
            await Db.SaveChangesAsync();

            Flash("Password changed. Use the new password next time you sign in.");
            return RedirectToAction(nameof(Index));
        }

        private async Task<ProfilePageVm> BuildAsync()
        {
            var (name, email) = await AdminAccount.GetProfileAsync(Db, _config);
            return new ProfilePageVm
            {
                Profile = new ProfileForm { Name = name, Email = email },
                Role = User.FindFirst(ClaimTypes.Role)?.Value ?? "Admin"
            };
        }

        // The cookie holds name/email claims, so re-issue it after they change.
        private async Task RefreshSignInAsync(string name, string email)
        {
            var current = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            var claims = new List<Claim>
            {
                new(ClaimTypes.Name, name),
                new(ClaimTypes.Email, email),
                new(ClaimTypes.Role, "Admin")
            };
            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme)),
                new AuthenticationProperties { IsPersistent = current.Properties?.IsPersistent ?? false });
        }
    }
}
