using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using TAJAN_TRAVEL.Localization;

namespace TAJAN_TRAVEL.Controllers
{
    public class LanguageController : Controller
    {
        // /Language/Set?culture=fr&returnUrl=/Tours
        [HttpGet]
        public IActionResult Set(string culture, string? returnUrl)
        {
            culture = Lang.Supported.Contains(culture) ? culture : Lang.English;

            Response.Cookies.Append(
                CookieRequestCultureProvider.DefaultCookieName,
                CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(culture)),
                new CookieOptions
                {
                    Expires = DateTimeOffset.UtcNow.AddYears(1),
                    IsEssential = true,
                    HttpOnly = true,
                    SameSite = SameSiteMode.Lax,
                    Secure = Request.IsHttps
                });

            // only ever redirect inside our own site
            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                return LocalRedirect(returnUrl);

            return RedirectToAction("Index", "Home");
        }
    }
}
