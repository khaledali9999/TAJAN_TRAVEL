using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using TAJAN_TRAVEL.Data;
using TAJAN_TRAVEL.Models.Entities;

namespace TAJAN_TRAVEL.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize]
    public abstract class AdminBaseController : Controller
    {
        protected readonly ApplicationDbContext Db;

        protected AdminBaseController(ApplicationDbContext db)
        {
            Db = db;
        }

        // Green / red banner shown at the top of the next page (see _AdminLayout)
        protected void Flash(string message) => TempData["Flash"] = message;
        protected void FlashError(string message) => TempData["FlashError"] = message;

        // Sidebar badge counts (Notifications / Tour Requests / File confirmed)
        public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            ViewBag.NewRequests = await Db.Inquiries.CountAsync(i => i.Status == InquiryStatus.New);
            ViewBag.NewTourRequests = await Db.Inquiries.CountAsync(i => i.Status == InquiryStatus.New && i.Type == InquiryType.TourInquiry);
            ViewBag.NewCustomRequests = await Db.Inquiries.CountAsync(i => i.Status == InquiryStatus.New && i.Type == InquiryType.CustomItinerary);
            ViewBag.ConfirmedFiles = await Db.Inquiries.CountAsync(i => i.Status == InquiryStatus.Confirmed);
            ViewBag.UnreadMessages = await Db.ContactMessages.CountAsync(m => !m.IsRead);
            await next();
        }
    }
}
