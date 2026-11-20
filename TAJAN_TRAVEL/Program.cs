using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using TAJAN_TRAVEL.Data;
using TAJAN_TRAVEL.Data.Seed;
using System.Globalization;
using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.Localization;
using TAJAN_TRAVEL.Localization;

var builder = WebApplication.CreateBuilder(args);

// MVC (Controllers/Views come in Phase 2)
// Localization: English + French. UI text comes from Localization/FrenchStrings.cs, content from the *Fr columns.
builder.Services.AddSingleton<IStringLocalizerFactory, DictionaryLocalizerFactory>();   // must be before AddControllersWithViews
builder.Services.AddSingleton<Translator>();
builder.Services.AddControllersWithViews()
    .AddDataAnnotationsLocalization(o =>
        o.DataAnnotationLocalizerProvider = (type, factory) => factory.Create(typeof(SharedResource)));

builder.Services.Configure<RequestLocalizationOptions>(o =>
{
    var cultures = Lang.Supported.Select(c => new CultureInfo(c)).ToList();
    o.DefaultRequestCulture = new RequestCulture(Lang.English);
    o.SupportedCultures = cultures;
    o.SupportedUICultures = cultures;
    // The admin area always stays English (forms post decimals like 1450.50, which French culture would reject)
    o.RequestCultureProviders.Insert(0, new CustomRequestCultureProvider(ctx =>
        Task.FromResult<ProviderCultureResult?>(
            ctx.Request.Path.StartsWithSegments("/Admin", StringComparison.OrdinalIgnoreCase)
                ? new ProviderCultureResult(Lang.English) : null)));
});

// Admin authentication (cookie). Credentials come from configuration: Admin:Email / Admin:Password
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Admin/Account/Login";
        options.AccessDeniedPath = "/Admin/Account/Login";
        options.Cookie.Name = "tajan.admin";
        options.Cookie.HttpOnly = true;
        options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
        options.ExpireTimeSpan = TimeSpan.FromHours(8);
        options.SlidingExpiration = true;
    });

// Database
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddHttpContextAccessor();

var app = builder.Build();

SiteCurrency.Configure(
    app.Services.GetRequiredService<IHttpContextAccessor>(),
    app.Configuration);

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseRequestLocalization();
app.UseAuthentication();
app.UseAuthorization();

// Admin area: /Admin -> Dashboard
app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Dashboard}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// Apply migrations + seed data on startup (dev convenience)
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    DbSeeder.Seed(db);
}

app.Run();

