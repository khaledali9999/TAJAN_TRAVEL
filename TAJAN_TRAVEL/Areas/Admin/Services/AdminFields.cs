using System.Globalization;
using System.Linq.Expressions;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace TAJAN_TRAVEL.Areas.Admin.Services
{
    /// <summary>
    /// Small HTML helpers so every admin form field gets the same label / control / error / hint markup.
    /// Usage in a view:  @Html.AdminText(m => m.TitleEn, "Title (English)", required: true)
    /// </summary>
    public static class AdminFields
    {
        public const string Ctl =
            "w-full rounded-lg border border-slate-200 bg-white px-3 py-2.5 text-sm text-slate-800 placeholder-slate-400 " +
            "focus:border-tajan-gold focus:outline-none focus:ring-1 focus:ring-tajan-gold";

        private static string E(string? s) => HtmlEncoder.Default.Encode(s ?? string.Empty);

        // Reads the current value of the bound property straight from the view's model.
        private static object? Eval<T, V>(IHtmlHelper<T> h, Expression<Func<T, V>> e)
        {
            try
            {
                var m = h.ViewData.Model;
                return m == null ? null : e.Compile()(m);
            }
            catch
            {
                return null;
            }
        }

        private static Dictionary<string, object> Attrs(string? placeholder = null, params (string k, string v)[] extra)
        {
            var d = new Dictionary<string, object> { ["class"] = Ctl };
            if (!string.IsNullOrEmpty(placeholder)) d["placeholder"] = placeholder;
            foreach (var (k, v) in extra) d[k] = v;
            return d;
        }

        private static IHtmlContent Wrap<T, V>(IHtmlHelper<T> h, Expression<Func<T, V>> e, string label, bool required,
            string? hint, IHtmlContent control, string? cls)
        {
            var b = new HtmlContentBuilder();
            b.AppendHtml($"<div class=\"{E(cls)}\">");
            b.AppendHtml($"<label for=\"{E(h.IdFor(e))}\" class=\"mb-1.5 block text-[13px] font-medium text-slate-700\">{E(label)}");
            if (required) b.AppendHtml(" <span class=\"text-red-500\">*</span>");
            b.AppendHtml("</label>");
            b.AppendHtml(control);
            b.AppendHtml(h.ValidationMessageFor(e, null, new { @class = "mt-1 block text-xs text-red-600" }));
            if (!string.IsNullOrWhiteSpace(hint)) b.AppendHtml($"<p class=\"mt-1 text-xs text-slate-400\">{E(hint)}</p>");
            b.AppendHtml("</div>");
            return b;
        }

        public static IHtmlContent AdminText<T, V>(this IHtmlHelper<T> h, Expression<Func<T, V>> e, string label,
            bool required = false, string? hint = null, string? placeholder = null, string? cls = null, string? list = null)
        {
            var a = Attrs(placeholder);
            if (list != null) a["list"] = list;
            return Wrap(h, e, label, required, hint, h.TextBoxFor(e, a), cls);
        }

        public static IHtmlContent AdminTextArea<T, V>(this IHtmlHelper<T> h, Expression<Func<T, V>> e, string label,
            int rows = 4, bool required = false, string? hint = null, string? cls = null)
        {
            var a = Attrs(null, ("rows", rows.ToString(CultureInfo.InvariantCulture)));
            return Wrap(h, e, label, required, hint, h.TextAreaFor(e, a), cls);
        }

        /// <summary>Number input that always uses "." as decimal separator, whatever the server culture is.</summary>
        public static IHtmlContent AdminNumber<T, V>(this IHtmlHelper<T> h, Expression<Func<T, V>> e, string label,
            bool required = false, string? hint = null, string? min = null, string? max = null, string step = "1", string? cls = null)
        {
            var model = Eval(h, e);
            var a = Attrs(null, ("type", "number"), ("step", step));
            if (min != null) a["min"] = min;
            if (max != null) a["max"] = max;
            var raw = Convert.ToString(model, CultureInfo.InvariantCulture);
            return Wrap(h, e, label, required, hint, h.TextBox(h.NameFor(e), raw, a), cls);
        }

        public static IHtmlContent AdminDate<T>(this IHtmlHelper<T> h, Expression<Func<T, DateTime?>> e, string label,
            bool required = false, string? hint = null, string? cls = null)
        {
            var model = Eval(h, e) as DateTime?;
            var raw = model?.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
            return Wrap(h, e, label, required, hint, h.TextBox(h.NameFor(e), raw, Attrs(null, ("type", "date"))), cls);
        }

        public static IHtmlContent AdminSelect<T, V>(this IHtmlHelper<T> h, Expression<Func<T, V>> e, string label,
            IEnumerable<SelectListItem> items, string? emptyOption = null, bool required = false, string? hint = null, string? cls = null)
        {
            return Wrap(h, e, label, required, hint, h.DropDownListFor(e, items, emptyOption, Attrs()), cls);
        }

        public static IHtmlContent AdminToggle<T>(this IHtmlHelper<T> h, Expression<Func<T, bool>> e, string label, string? hint = null)
        {
            var b = new HtmlContentBuilder();
            b.AppendHtml("<label class=\"flex cursor-pointer items-start gap-3 rounded-lg border border-slate-200 bg-white px-3 py-2.5\">");
            b.AppendHtml(h.CheckBoxFor(e, new { @class = "mt-0.5 h-4 w-4 rounded border-slate-300 text-tajan-red focus:ring-tajan-gold" }));
            b.AppendHtml($"<span class=\"text-sm text-slate-700\">{E(label)}");
            if (!string.IsNullOrWhiteSpace(hint)) b.AppendHtml($"<span class=\"block text-xs text-slate-400\">{E(hint)}</span>");
            b.AppendHtml("</span></label>");
            return b;
        }

        /// <summary>Image URL text box + file picker + current preview. The file input is named <paramref name="fileField"/>.</summary>
        public static IHtmlContent AdminImage<T>(this IHtmlHelper<T> h, Expression<Func<T, string?>> urlExpr, string fileField,
            string label, string? hint = null)
        {
            var current = Eval(h, urlExpr) as string;

            var b = new HtmlContentBuilder();
            b.AppendHtml("<div>");
            b.AppendHtml($"<label for=\"{E(h.IdFor(urlExpr))}\" class=\"mb-1.5 block text-[13px] font-medium text-slate-700\">{E(label)}</label>");

            if (!string.IsNullOrWhiteSpace(current))
                b.AppendHtml($"<img src=\"{E(current)}\" alt=\"\" class=\"mb-3 h-32 w-full max-w-xs rounded-lg border border-slate-200 object-cover\" />");

            b.AppendHtml("<div class=\"grid gap-3 sm:grid-cols-2\">");
            b.AppendHtml("<div>");
            b.AppendHtml(h.TextBoxFor(urlExpr, Attrs("Image link, e.g. /images/tours/giza.jpg")));
            b.AppendHtml("<p class=\"mt-1 text-xs text-slate-400\">Paste a link, or…</p></div>");
            b.AppendHtml("<div>");
            b.AppendHtml($"<input type=\"file\" name=\"{E(fileField)}\" accept=\"image/jpeg,image/png,image/webp,image/gif\" " +
                         "class=\"block w-full text-sm text-slate-600 file:mr-3 file:rounded-lg file:border-0 file:bg-tajan-sand/40 file:px-4 file:py-2.5 file:text-sm file:font-medium file:text-tajan-red hover:file:bg-tajan-sand/60\" />");
            b.AppendHtml("<p class=\"mt-1 text-xs text-slate-400\">…upload one (JPG/PNG/WEBP/GIF, max 5 MB). An upload replaces the link.</p></div>");
            b.AppendHtml("</div>");
            b.AppendHtml(h.ValidationMessageFor(urlExpr, null, new { @class = "mt-1 block text-xs text-red-600" }));
            if (!string.IsNullOrWhiteSpace(hint)) b.AppendHtml($"<p class=\"mt-1 text-xs text-slate-400\">{E(hint)}</p>");
            b.AppendHtml("</div>");
            return b;
        }
    }
}
