using Microsoft.AspNetCore.Mvc;
using TAJAN_TRAVEL.Localization;

namespace TAJAN_TRAVEL.Controllers
{
    public class CurrencyController : Controller
    {
        // /Currency/Set?code=EUR&returnUrl=/Tours
        [HttpGet]
        public IActionResult Set(string code, string? returnUrl)
        {
            code = (code ?? "").ToUpperInvariant();
            if (!SiteCurrency.Supported.Contains(code)) code = SiteCurrency.USD;

            Response.Cookies.Append(SiteCurrency.CookieName, code, new CookieOptions
            {
                Expires = DateTimeOffset.UtcNow.AddYears(1),
                IsEssential = true,
                HttpOnly = true,
                SameSite = SameSiteMode.Lax,
                Secure = Request.IsHttps
            });

            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                return LocalRedirect(returnUrl);

            return RedirectToAction("Index", "Home");
        }
    }
}
