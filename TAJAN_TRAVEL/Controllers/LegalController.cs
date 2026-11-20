using Microsoft.AspNetCore.Mvc;
using TAJAN_TRAVEL.Localization;
using TAJAN_TRAVEL.Models;

namespace TAJAN_TRAVEL.Controllers
{
    public class LegalController : Controller
    {
        [HttpGet("terms-of-service")]
        public IActionResult Terms() => View("Page", LegalContent.Terms(Lang.IsFrench));

        [HttpGet("privacy-policy")]
        public IActionResult Privacy() => View("Page", LegalContent.Privacy(Lang.IsFrench));
    }
}
