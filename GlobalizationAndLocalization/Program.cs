using System.Globalization;
using GlobalizationAndLocalization.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<ApplicationDbContext>();
builder.Services.AddControllersWithViews();

// Adds localization services to the container and sets the folder name where resource (.resx) files are stored.
// In this case, all localization resource files should be placed inside a folder named "Resources"
builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");

// Adds MVC services to the application's dependency injection container
builder.Services.AddMvc()
    // Enables support for localized views.
    // The 'Suffix' format means the framework will look for views like "Index.ar.cshtml" for Arabic, "Index.fr.cshtml" for French, etc.
    .AddViewLocalization(Microsoft.AspNetCore.Mvc.Razor.LanguageViewLocationExpanderFormat.Suffix)
    // Enables localization of strings used in data annotation attributes (like [Required], [Display(Name = "...")])
    .AddDataAnnotationsLocalization();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

// Define a list of supported cultures (languages) for the application.
var cultures = new List<CultureInfo> {
    new CultureInfo("en"),
    new CultureInfo("fr")
};
// Configures localization middleware to handle culture settings based on the request.
app.UseRequestLocalization(options => {

    // Sets the default culture to English when no culture is specified in the request
    options.DefaultRequestCulture = new Microsoft.AspNetCore.Localization.RequestCulture("en");

    // List of cultures supported for formatting (numbers, dates, etc.)
    options.SupportedCultures = cultures;

    // List of cultures supported for UI strings (like resource file translations)
    options.SupportedUICultures = cultures;
});

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();

app.Run();
