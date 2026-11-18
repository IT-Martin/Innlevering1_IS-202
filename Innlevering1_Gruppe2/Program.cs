using System.Globalization;
using Reachout.DataAccess;
using Reachout.DataAccess.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.Cookies;
using Reachout.Services;
using Microsoft.AspNetCore.DataProtection;
using System.Threading.RateLimiting;
using Microsoft.AspNetCore.RateLimiting;

CultureInfo.DefaultThreadCurrentCulture = CultureInfo.InvariantCulture;
var builder = WebApplication.CreateBuilder(args);
builder.Configuration.AddJsonFile("appsettings.Local.json", optional: true).AddEnvironmentVariables();
builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseMySql(builder.Configuration.GetConnectionString("DefaultConnection"), new MariaDbServerVersion(new Version(11, 8, 0))));
builder.Services.AddScoped<IResourceRepository, ResourceRepository>();
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie(options =>
{
    options.Cookie.Name = ".Reachout";
    options.Cookie.HttpOnly = true;
    options.Cookie.SameSite = SameSiteMode.Lax;
    options.ExpireTimeSpan = TimeSpan.FromHours(8);
    options.LoginPath = "/Login";
    options.AccessDeniedPath = "/Login/AccessDenied";
});
builder.Services.AddScoped<AccountService>();
builder.Services.AddScoped<AccessFilter>();
builder.Services.AddScoped<WorkflowService>();
var data = builder.Configuration["DataDirectory"] ?? Path.Combine(builder.Environment.ContentRootPath, "App_Data");
Directory.CreateDirectory(data);
builder.Services.AddDataProtection().SetApplicationName("Reachout").PersistKeysToFileSystem(new DirectoryInfo(data));
builder.Services.AddRateLimiter(options =>
{
    options.RejectionStatusCode = 429;
    options.AddPolicy("accounts", http => RateLimitPartition.GetFixedWindowLimiter(http.Connection.RemoteIpAddress?.ToString() ?? "unknown", _ => new FixedWindowRateLimiterOptions { PermitLimit = 40, Window = TimeSpan.FromMinutes(1), QueueLimit = 0 }));
});
builder.Services.AddAuthorization();
// Add services to the container.
builder.Services.AddControllersWithViews(options => options.Filters.Add<AccessFilter>());
var app = builder.Build();
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var db = services.GetRequiredService<ApplicationDbContext>();
        db.Database.Migrate();
        await InitialAdmin.InitializeAsync(db, builder.Configuration, app.Environment);
        await InitialPublicActor.InitializeAsync(db, builder.Configuration, app.Environment);

        await DemoData.InitializeAsync(db, builder.Configuration);
    }
    catch (Exception ex)
    {
        var logger = services.GetService<ILogger<Program>>();
        if (logger != null)
        {
            logger.LogError(ex, "An error occured while applying database migtrations.");
        }
        else
        {
            Console.Error.WriteLine($"An error occurred whie applying database migrations.");
        }

        throw;
    }
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

if (!app.Environment.IsDevelopment() && !builder.Configuration.GetValue<bool>("LocalPreview"))
{
    app.UseHttpsRedirection();
}

app.UseStatusCodePagesWithReExecute("/Home/HttpError", "?statusCode={0}");
app.UseRouting();
app.UseRateLimiter();
app.UseAuthentication();
app.UseAuthorization();
app.MapStaticAssets();
app.MapControllerRoute(name: "default", pattern: "{controller=Home}/{action=Index}/{id?}").WithStaticAssets();
app.Run();
