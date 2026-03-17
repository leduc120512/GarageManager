using AutoGarageManager.Data;
using AutoGarageManager.Models;
using AutoGarageManager.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// ================= DATABASE =================
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(connectionString));

// ================= IDENTITY =================
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    options.Password.RequireDigit = true;
    options.Password.RequiredLength = 6;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = true;
    options.Password.RequireLowercase = true;
})
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders();

builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Login";
    options.AccessDeniedPath = "/AccessDenied";
});

// ================= SERVICES =================
builder.Services.AddScoped<ISmsService, TwilioSmsService>();

builder.Services.AddRazorPages();

// Background job
builder.Services.AddHostedService<MaintenanceReminderService>();

var app = builder.Build();

// ================= INIT DATABASE =================
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;

    try
    {
        var context = services.GetRequiredService<ApplicationDbContext>();

        // Tạo DB + migrate
        context.Database.Migrate();

        // Seed data
        var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
        var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();

        await SeedData.InitializeAsync(userManager, roleManager);

        Console.WriteLine("✅ Database migrated & seeded successfully");
    }
    catch (Exception ex)
    {
        Console.WriteLine("❌ ERROR DURING DB INIT:");
        Console.WriteLine(ex.Message);
        Console.WriteLine(ex.StackTrace);
    }
}

// ================= MIDDLEWARE =================
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage(); // HIỆN LỖI CHI TIẾT
}
else
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();

app.Run();