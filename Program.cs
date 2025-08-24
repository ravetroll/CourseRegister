using CourseRegisterApp.Components;
using CourseRegisterApp.Components.Account;
using CourseRegisterApp.CourseData;
using CourseRegisterApp.Data;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MudBlazor.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();
builder.Services.AddMudServices();
builder.Services.AddCascadingAuthenticationState();
builder.Services.AddScoped<IdentityUserAccessor>();
builder.Services.AddScoped<IdentityRedirectManager>();
builder.Services.AddScoped<AuthenticationStateProvider, IdentityRevalidatingAuthenticationStateProvider>();

builder.Services.AddAuthentication(options =>
    {
        options.DefaultScheme = IdentityConstants.ApplicationScheme;
        options.DefaultSignInScheme = IdentityConstants.ExternalScheme;
    })
    .AddIdentityCookies();

var IdconnectionString = builder.Configuration.GetConnectionString("IdentityConnection") ?? throw new InvalidOperationException("Connection string 'IdentityConnection' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(IdconnectionString));

var CrsconnectionString = builder.Configuration.GetConnectionString("CourseConnection") ?? throw new InvalidOperationException("Connection string 'CourseConnection' not found.");
builder.Services.AddDbContext<CourseRegisterContext>(options =>
    options.UseSqlServer(CrsconnectionString));

builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddIdentityCore<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = true)
    
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddSignInManager()
    .AddDefaultTokenProviders();

builder.Services.AddScoped<IEmailSender<ApplicationUser>, IdentityNoOpEmailSender>();

var app = builder.Build();


// Apply migrations at startup
using (var scope = app.Services.CreateScope())
{
    var dbCourseContext = scope.ServiceProvider.GetRequiredService<CourseRegisterContext>();
    dbCourseContext.Database.Migrate();
    var dbIdentityContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    dbIdentityContext.Database.Migrate();
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();


app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

// Add additional endpoints required by the Identity /Account Razor components.
app.MapAdditionalIdentityEndpoints();
await CreateRolesAsync(app.Services);
app.Run();

async Task CreateRolesAsync(IServiceProvider serviceProvider)
{
    // Fix: Create a service scope to correctly resolve the scoped services
    using (var scope = serviceProvider.CreateScope())
    {
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

        // Define the roles you want to create
        string[] roleNames = { "Admin", "Lecturer", "Student" };

        foreach (var roleName in roleNames)
        {
            // Check if the role already exists
            bool roleExists = await roleManager.RoleExistsAsync(roleName);
            if (!roleExists)
            {
                // Create the new role if it doesn't exist
                await roleManager.CreateAsync(new IdentityRole(roleName));
            }
        }
    }
}