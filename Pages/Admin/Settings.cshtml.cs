using AutoGarageManager.Data;
using AutoGarageManager.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace AutoGarageManager.Pages.Admin;

[Authorize(Roles = "Admin")]
public class SettingsModel : PageModel
{
    private readonly ApplicationDbContext _context;

    public SettingsModel(ApplicationDbContext context)
    {
        _context = context;
    }

    [BindProperty]
    public Settings? AppSettings { get; set; }

    [TempData]
    public string? Message { get; set; }

    public async Task<IActionResult> OnGetAsync()
    {
        AppSettings = await _context.Settings.FirstOrDefaultAsync();
        if (AppSettings == null)
        {
            AppSettings = new Settings
            {
                StoreName = "Auto Garage Manager",
                EnableEmailNotifications = true,
                AllowUserRegistration = true
            };
            _context.Settings.Add(AppSettings);
            await _context.SaveChangesAsync();
        }
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        var settings = await _context.Settings.FirstOrDefaultAsync();
        if (settings == null)
        {
            settings = new Settings();
            _context.Settings.Add(settings);
        }

        settings.StoreName = AppSettings!.StoreName;
        settings.Address = AppSettings.Address;
        settings.PhoneNumber = AppSettings.PhoneNumber;
        settings.Email = AppSettings.Email;
        settings.EnableEmailNotifications = AppSettings.EnableEmailNotifications;
        settings.AllowUserRegistration = AppSettings.AllowUserRegistration;
        settings.UpdatedAt = DateTime.Now;

        await _context.SaveChangesAsync();
        Message = "Cài đặt đã được lưu thành công.";
        return RedirectToPage();
    }
}
