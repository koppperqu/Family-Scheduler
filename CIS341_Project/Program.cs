using Microsoft.EntityFrameworkCore;
using CIS341_Project.Data;
using Microsoft.AspNetCore.Identity;
using CIS341_Project.Areas.Identity.Data;
using Microsoft.AspNetCore.Authorization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();
// Add DbContext to the service container.
builder.Services.AddDbContext<FamilySchedulerContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddDbContext<AuthenticationContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("AuthenticationContextConnection")));

builder.Services.AddDefaultIdentity<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddRoles<IdentityRole>() // Add role-related services
    .AddEntityFrameworkStores<AuthenticationContext>();

builder.Services.Configure<IdentityOptions>(options =>
{
    options.Password.RequiredLength = 12;
});

builder.Services.AddAuthorization(options =>
{
    options.FallbackPolicy = new AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .Build();

    // Define a policy that requires user to be in a specific role 
    // -- used for View-based authorization
    options.AddPolicy("RequireAdministratorRole", policy =>
        policy.RequireRole("Admin"));
});
var app = builder.Build();

// Since the DbContext is a scoped service(?), we need to create a scope to retrieve the service.
using (var scope = app.Services.CreateScope())
{
    // Service provider for the scope
    var services = scope.ServiceProvider;
    try
    {
        // Get the DbContext from the service provider
        var familySchedulerContext = new FamilySchedulerContext(services.GetRequiredService<DbContextOptions<FamilySchedulerContext>>());
        DbInitializer.Initialize(familySchedulerContext);
        InitializeUsersAndAdmin.Initialize(services).Wait();
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred seeding data in DB's");
    }
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseAuthentication(); ;

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Tasks}/{action=Index}/{id?}");

app.MapRazorPages();

app.Run();
