using FcConnect.Data;
using FcConnect.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.UI.Services;
using FcConnect.Utilities;
using FcConnect.Models;
using FcConnect.Migrations;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.Configure<IdentityOptions>(options => 
{
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(Constants.MaxPasswordRetries);
    options.Lockout.MaxFailedAccessAttempts = Constants.LockoutTimeMins;
    options.Lockout.AllowedForNewUsers = true;
});

builder.Services.AddRazorPages();

builder.Services.AddTransient<IEmailSender, EmailSender>();
builder.Services.Configure<AuthMessageSenderOptions>(builder.Configuration);

builder.Services.AddDistributedMemoryCache();

builder.Services.AddAuthorization(options =>
{
    options.FallbackPolicy = new AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .Build();
});

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

builder.Services.AddScoped<LogEvent>();

var app = builder.Build();  

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{    
    app.UseDeveloperExceptionPage();
    app.UseStatusCodePagesWithReExecute("/Error/{0}");
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Error");
    app.UseStatusCodePagesWithReExecute("/Error/{0}");

    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.UseSession();

app.MapRazorPages();

using (var scope = app.Services.CreateScope()) 
{
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

    var roles = new[] { "User", "Admin" };

    foreach (var role in roles) 
    {
        if (!await roleManager.RoleExistsAsync(role))
            await roleManager.CreateAsync(new IdentityRole(role));
    }

    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();
    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

    string email = "test@admin.com";
    string password = "Regre55i0n#";

    if (await userManager.FindByEmailAsync(email) == null) 
    {
        var user = new IdentityUser();
        user.UserName = email;
        user.Email = email;
        user.EmailConfirmed = true;

        await userManager.CreateAsync(user, password);
        await userManager.AddToRoleAsync(user, "Admin");

        var createdUser = await userManager.FindByEmailAsync(email);

        User newUser = new()
        {
            Id = createdUser.Id,
            Email = createdUser.Email,
            Forename = "Test",
            Surname = "Admin",
            RoleId = Constants.RoleAdmin,
            UserStatusId = Constants.StatusUserActive
        };

        await userManager.AddClaimAsync(createdUser, new System.Security.Claims.Claim("TermsAccepted", "true"));
        await context.User.AddAsync(newUser);
        await context.SaveChangesAsync();
    }

    string userEmail = "test@user.com";
    string userPassword = "Regre55i0n#";

    if (await userManager.FindByEmailAsync(userEmail) == null)
    {
        var userU = new IdentityUser();
        userU.UserName = userEmail;
        userU.Email = userEmail;
        userU.EmailConfirmed = true;

        await userManager.CreateAsync(userU, userPassword);
        await userManager.AddToRoleAsync(userU, "User");

        var createdUserU = await userManager.FindByEmailAsync(userEmail);

        User newUser = new()
        {
            Id = createdUserU.Id,
            Email = createdUserU.Email,
            Forename = "Test",
            Surname = "User",
            RoleId = Constants.RoleUser,
            UserStatusId = Constants.StatusUserActive
        };

        await userManager.AddClaimAsync(createdUserU, new System.Security.Claims.Claim("TermsAccepted", "true"));
        await context.User.AddAsync(newUser);
        await context.SaveChangesAsync();
    }
}

app.Run();